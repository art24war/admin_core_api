using Microsoft.EntityFrameworkCore;
using DbModels;

namespace DbRepository
{
    public class UsersRepositoryContext: DbContext
    {
        public UsersRepositoryContext(DbContextOptions<UsersRepositoryContext> options): base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<RoleGroupModel> RoleGroups { get; set; }
        public DbSet<DealerModel> Dealers { get; set; }
    }
}
