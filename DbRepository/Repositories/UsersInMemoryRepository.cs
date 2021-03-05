using DbModels;
using DbModels.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbRepository.Repositories
{
    public class UsersInMemoryRepository : BaseInMemoryRepository, IUsersRepository
    {
        public UsersInMemoryRepository(string connectionString, IRepositoryContextFactory contextFactory) : base(connectionString, contextFactory)
        {
        }

        async Task<UserModel> IUsersRepository.GetUserById(int id)
        {
            using var context = GetContext();
            var users = context.Users.AsQueryable();
            return await users.FirstOrDefaultAsync(u => u.Id == id);
        }

        async Task<IEnumerable<UserModel>> IUsersRepository.GetUsers(UserModelFilter filter)
        {
            using var context = GetContext();
            var users = context.Users.AsQueryable();
           
            var result = users.Where(r =>
                (string.IsNullOrEmpty(filter.UserLogin) || r.Login.Equals(filter.UserLogin)) &&
                (!filter.UserId.HasValue || r.Id.Equals(filter.UserId.Value)) &&
                (!filter.DealerId.HasValue || (r.Dealer != null && r.Dealer.Id.Equals(filter.DealerId.Value)))
                );
            
            return await result.ToListAsync();
        }

        async Task<Tuple<UserModel, bool>> IUsersRepository.LoginUser(string login, string pwdHash)
        {
            using var context = GetContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Login.Equals(login));
            return new Tuple<UserModel, bool>(user, user?.PasswordHash.Equals(pwdHash)?? false);            
        }

        async Task<UserModel> IUsersRepository.CreateUser(UserModel userModel)
        {
            using var context = GetContext();
            var user = await context.Users.AddAsync(userModel);
            var result = await context.SaveChangesAsync();
            return await Task.FromResult(user.Entity);
        }

        private UsersRepositoryContext GetContext()
        {
            return ContextFactory.CreateUserDbContext(ConnectionString);
        }

        public async Task<DealerModel> GetDealer(int? dealerId)
        {
            using var context = GetContext();
            return dealerId.HasValue ? 
                await context.Dealers.FirstOrDefaultAsync(d => d.Id.Equals(dealerId.Value)) : 
                await Task.FromResult<DealerModel>(null);
        }
    }
}
