using DbModels;
using DbModels.Filters;
using DbRepository.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbRepository.Repositories
{
    public interface IUsersRepository
    {
        public Task<UserModel> GetUserByIdAsync(int id);
        
        /// <summary>
        /// Create a User in DB
        /// </summary>
        /// <param name="userModel">User to be created</param>
        /// <returns></returns>
        Task<UserModel> CreateUser (UserModel userModel);

        /// <summary>
        /// Checks user registration in DB
        /// </summary>
        /// <param name="login">User login</param>
        /// <param name="pwdHash">User password hash(sha256 from password)</param>
        /// <returns></returns>
        Task<LoginResponse> LoginUserAsync(string login, string pwdHash);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<LoginResponse> RefreshToken(string login, RefreshToken token);
        
        /// <summary>
        /// Get list of users by filter
        /// </summary>
        /// <param name="userModelFilter"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetUsers(UserModelFilter userModelFilter);
        
        /// <summary>
        /// Get dealer instance by dealer Id
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        Task<DealerModel> GetDealer(int? dealerId);
        
        /// <summary>
        /// Check users refreshToken for refresh time expires and exists
        /// </summary>
        /// <param name="login"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<LoginResponse> CheckUserRefreshTokenAsync(string login, string refreshToken);
    }
}