using DbRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReactReduxApi.Helpers
{
    public static class MigrationExtension
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<IRepositoryContextFactory>().CreateUserDbContext(configuration.GetConnectionString("default"));
            if (context != null && context.Database != null)
            {
                context.Database.Migrate();
            }
            return app;
        }
    }
}
