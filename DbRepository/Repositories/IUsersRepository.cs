using DbModels;
using DbModels.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbRepository.Repositories
{
    public interface IUsersRepository
    {
        public Task<UserModel> GetUserById(int id);
        
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
        Task<Tuple<UserModel, bool>> LoginUser(string login, string pwdHash);
        
        /// <summary>
        /// Get list of users by filter
        /// </summary>
        /// <param name="userModelFilter"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetUsers(UserModelFilter userModelFilter);
        Task<DealerModel> GetDealer(int? dealerId);
    }
}