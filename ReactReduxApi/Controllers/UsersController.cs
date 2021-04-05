using DbModels;
using DbModels.Enums;
using DbRepository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReactReduxApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ReactReduxApi.Helpers;
using DbModels.Filters;
using AutoMapper;
using ReactReduxApi.Models.Users;
using Microsoft.AspNetCore.Http;
using CommonLib;
using ReactReduxApi.Models.Enums;

namespace ReactReduxApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IConfiguration configuration, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {
            if (!model.DealerId.HasValue && !model.UserType.Equals((int)UserTypeEnum.Admin))
                return ReturnBadRequest($"DealerId field must be filled for non-admin userTypes.");

            var users = await _usersRepository.GetUsers(new UserModelFilter() { UserLogin = model.Login });
            if (users.Any())
                return ReturnBadRequest($"user with login({model.Login}) already exists");

            UserModel registerModel = _mapper.Map<UserModel>(model);
            registerModel.CreatedByUser = int.Parse(User.FindFirstValue("UserId"));
            
            var result = await _usersRepository.CreateUser(registerModel);
            return result != null ? Ok(result) : ReturnBadRequest("Creates failed");
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(LoginUserModel model)
        {
            if (model.Mode.ToLowerInvariant().Equals(LoginModeType.Password.ToString().ToLowerInvariant()) &&
                (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password)))
                return ReturnBadRequest("login or password is empty");

            var refreshTokenBuilder = new SecurityTokenBuilder()
                                .AddConfiguration(_configuration)
                                .AddEncriptionKey(Constants.JwtRefreshEncriptionKey)
                                .AddIssuerKey(Constants.JwtIssuer)
                                .AddAudienceKey(Constants.JwtAudience)
                                .AddExpiryKey(Constants.JwtRefreshTokenExpiration);

            var tokenBuilder = new SecurityTokenBuilder()
                                .AddConfiguration(_configuration)
                                .AddEncriptionKey(Constants.JwtEncryptionKey)
                                .AddIssuerKey(Constants.JwtIssuer)
                                .AddAudienceKey(Constants.JwtAudience)
                                .AddExpiryKey(Constants.JwtExpiryTime);

            switch (model.Mode.ToLowerInvariant())
            {
                case "password":
                    var result = await _usersRepository.LoginUserAsync(model.Login, CryptoHelper.GetSha256String(model.Password));
                    if (result.User != null)
                {
                        string refreshToken = string.Empty;
                        
                        if (result.LoginResult)
                        {                            
                            tokenBuilder.AddClaims(CryptoHelper.GetUserClaims(result.User));                             
                            refreshTokenBuilder.AddClaims(CryptoHelper.GetRefreshUserClaims(result.User));

                            HttpContext.Response.Cookies.Append(_configuration.GetValue<string>(Constants.JwtCookieToken),
                                    tokenBuilder.BuildAccessToken(),
                        new CookieOptions
                        {
                                    MaxAge = TimeSpan.FromMinutes(_configuration.GetValue<int>(Constants.JwtExpiryTime)),
                                    HttpOnly = true
                        });
                            refreshToken = refreshTokenBuilder.BuildAccessToken();

                            var refreshTokenModel = new RefreshToken
                            {
                                UserId = result.User.Id,
                                Token = refreshToken,
                                ValidTo = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>(Constants.JwtRefreshTokenExpiration))
                            };
                            var _ = await _usersRepository.RefreshToken(model.Login, refreshTokenModel);
                }
                        return result.LoginResult ? Ok(CryptoHelper.GetUserToken(result.User, tokenBuilder, refreshToken)) : ReturnBadRequest("login failed");
            }
            else
                return ReturnBadRequest("user not found");            
                case "refresh":
                    refreshTokenBuilder.AddAccessToken(model.RefreshToken);                    
                    var userId = refreshTokenBuilder.GetUserId();
                    var userResult = await _usersRepository.CheckUserRefreshTokenAsync(userId, model.RefreshToken);
                    if (userResult.LoginResult)
                    {
                        var user = userResult.User;
                        refreshTokenBuilder.AddClaims(CryptoHelper.GetRefreshUserClaims(user));
                        refreshTokenBuilder.SetCreateNew();
                        var refreshToken = refreshTokenBuilder.BuildAccessToken();
                        var refreshTokenModel = new RefreshToken
                        {
                            UserId = user.Id,
                            Token = refreshToken,
                            ValidTo = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>(Constants.JwtRefreshTokenExpiration))
                        };
                        var _ = await _usersRepository.RefreshToken(userId, refreshTokenModel);
                        
                        tokenBuilder.AddClaims(CryptoHelper.GetUserClaims(user));
                        return Ok(CryptoHelper.GetUserToken(user, tokenBuilder, refreshToken));
                    }
                    else 
                        return Unauthorized("refreshToken not valid");
                    
                default:
                    return Unauthorized("mode is not found");                   
            }
            
        }

        [Route("list")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetUsers(UserModelFilter userModelFilter)
        {
            var result = await _usersRepository.GetUsers(userModelFilter);
            return Ok(_mapper.Map<IEnumerable<UserListModel>>(result));
        }

        
        private BadRequestObjectResult ReturnBadRequest(string failedMessage)
        {
            return BadRequest(new ErrorMessageModel
            {
                Message = failedMessage
            });
        }
    }
}
