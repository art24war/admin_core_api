using DbModels;
using DbModels.Filters;
using DbRepository.Repositories.Models;
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

        async Task<UserModel> IUsersRepository.GetUserByIdAsync(int id)
        {
            using var context = GetContext();
            var users = context.Users.Include(u => u.Roles).AsQueryable().AsNoTracking();
            return await users.FirstOrDefaultAsync(u => u.Id == id);
        }

        async Task<IEnumerable<UserModel>> IUsersRepository.GetUsers(UserModelFilter filter)
        {
            using var context = GetContext();
            var users = context.Users.Include(u => u.Roles).AsQueryable().AsNoTracking();
           
            var result = users.Where(r =>
                (string.IsNullOrEmpty(filter.UserLogin) || r.Login.Equals(filter.UserLogin)) &&
                (!filter.UserId.HasValue || r.Id.Equals(filter.UserId.Value)) &&
                (!filter.DealerId.HasValue || (r.Dealer != null && r.Dealer.Id.Equals(filter.DealerId.Value)))
                );
            
            return await result.ToListAsync();
        }

        async Task<LoginResponse> IUsersRepository.LoginUserAsync(string login, string pwdHash)
        {
            using var context = GetContext();
            var user = (await context.Users.Include(u=> u.Roles).FirstOrDefaultAsync(u => u.Login.Equals(login)));
            return new LoginResponse { User = user, LoginResult = user?.PasswordHash.Equals(pwdHash) ?? false };            
        }

        async Task<LoginResponse> IUsersRepository.CheckUserRefreshTokenAsync(string login, string refreshToken)
        {
            using var context = GetContext();
            var userLogin = await context.Users.FirstOrDefaultAsync(c => c.Login.Equals(login));
            //            userLogin.RefreshTokens
            var refreshTokens = 
                await (userLogin != null ? 
                    context.Entry(userLogin).Collection(c=> c.RefreshTokens).Query().ToListAsync() : 
                    Task.FromResult(new List<RefreshToken>()));
            var result = userLogin != null &&
                (userLogin.RefreshTokens?.Any(t => t.Token.Equals(refreshToken) && t.ValidTo >= DateTime.UtcNow) ?? false);

            return await Task.FromResult(new LoginResponse { LoginResult = result, User = userLogin, RefreshToken = refreshToken });
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

        async Task<DealerModel> IUsersRepository.GetDealer(int? dealerId)
        {
            using var context = GetContext();
            return dealerId.HasValue ? 
                await context.Dealers.FirstOrDefaultAsync(d => d.Id.Equals(dealerId.Value)) : 
                await Task.FromResult<DealerModel>(null);
        }

        async Task<LoginResponse> IUsersRepository.RefreshToken(string login, RefreshToken token)
        {
            using var context = GetContext();
            var user = await context.Users.FirstOrDefaultAsync(c => c.Login.Equals(login));
            if(user != null)
            {
                if (user.RefreshTokens == null)
                    user.RefreshTokens = new List<RefreshToken>();
                var refreshTokens = await context.Entry(user).Collection(t => t.RefreshTokens).Query().ToListAsync();
                
                refreshTokens.ForEach(f => {
                    context.Remove(f);
                });
                user.RefreshTokens.Add(token);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(new LoginResponse { User = user, LoginResult = user != null });
        }
    }
}
