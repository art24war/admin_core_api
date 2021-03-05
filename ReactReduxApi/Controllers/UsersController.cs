﻿using DbModels;
using DbModels.Enums;
using DbRepository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReactReduxApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ReactReduxApi.Helpers;
using DbModels.Filters;
using AutoMapper;
using ReactReduxApi.Models.Users;
using Microsoft.AspNetCore.Http;

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
            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
                return ReturnBadRequest("login or password is empty");
            var result = await _usersRepository.LoginUser(model.Login, CryptoHelper.GetSha256String(model.Password));
            if (result.Item1 != null )
            {
                if (result.Item2)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    HttpContext.Response.Cookies.Append(_configuration.GetValue<string>("Jwt:CookieToken"),
                            tokenHandler.WriteToken(GetSecurityToken(GetUserClaims(result.Item1), tokenHandler)),
                        new CookieOptions
                        {
                            MaxAge = TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:ExpiryTimeInMinutes"))
                        });
                }
                return result.Item2 ? Ok(GetUserToken(result.Item1)) : ReturnBadRequest("login failed");
            }
            else
                return ReturnBadRequest("user not found");            
        }

        [Route("list")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetUsers(UserModelFilter userModelFilter)
        {
            var result = await _usersRepository.GetUsers(userModelFilter);
            return Ok(_mapper.Map<IEnumerable<UserListModel>>(result));
        }

        private object GetUserToken(DbModels.UserModel user)
        {
            var claims = GetUserClaims(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = GetSecurityToken(claims, tokenHandler);
            return new {
                id = user.Id,
                userName = user.Login,
                token = tokenHandler.WriteToken(token),
                expired = token.ValidTo,
                userType = user.UserType,
                roles = user.Roles?.Select(role => role.Role.RoleCode) ?? Array.Empty<string>()
            };
        }

        private ClaimsIdentity GetUserClaims(UserModel user)
        {
            if (user.Roles == null)
                user.Roles = new List<UsersRoleRelation>();
            var claims = new ClaimsIdentity(new Claim[]
                                   {
                                    new Claim(JwtRegisteredClaimNames.Sub, user.Login),
                                    new Claim("UserId", user.Id.ToString()),
                                    new Claim("UserType", user.UserType.ToString() )
                                   });
            switch (user.UserType)
            {
                case UserTypeEnum.Admin:
                case UserTypeEnum.Manager:
                    user.Roles.Add(new UsersRoleRelation { Role = new RoleModel { RoleCode = user.UserType.GetDisplayName() } });
                    claims.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserType.GetDisplayName()));
                    break;
            }
            //Adding UserClaims to JWT claims
            foreach (var item in user.Roles ?? new List<UsersRoleRelation>())
            {
                claims.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, item.Role.RoleCode));
            }
            return claims;
        }

        private SecurityToken GetSecurityToken(ClaimsIdentity claims, JwtSecurityTokenHandler tokenHandler) {
            // this information will be retrived from you Configuration
            //I have injected Configuration provider service into my controller
            var encryptionkey = _configuration["Jwt:EncriptionKey"];
            var key = Encoding.ASCII.GetBytes(encryptionkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Subject = claims,
                // this information will be retrived from you Configuration
                //I have injected Configuration provider service into my controller
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryTimeInMinutes"])),
                //algorithm to sign the token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.CreateToken(tokenDescriptor);
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
