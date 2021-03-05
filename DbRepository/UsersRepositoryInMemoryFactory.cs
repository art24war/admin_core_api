using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbRepository
{
    public class UsersRepositoryContextFactory : IRepositoryContextFactory
    {
        public UsersRepositoryContext CreateUserDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UsersRepositoryContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new UsersRepositoryContext(optionsBuilder.Options);
        }
    }
}
