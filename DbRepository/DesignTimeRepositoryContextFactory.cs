using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DbRepository.Repositories
{
    public class DesignTimeRepositoryContextFactory : IDesignTimeDbContextFactory<UsersRepositoryContext>
    {
        public UsersRepositoryContext CreateDbContext(string[] args)
        {
            var configurator = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");
            

            var config = configurator.Build();
            var connectionString = config.GetConnectionString("default");
            var repositoryFactory = new UsersRepositoryContextFactory();

            return repositoryFactory.CreateUserDbContext(connectionString);
        }
    }
}
