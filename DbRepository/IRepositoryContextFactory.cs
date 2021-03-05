namespace DbRepository
{
    public interface IRepositoryContextFactory
    {
        public UsersRepositoryContext CreateUserDbContext(string connectionString);
    }
}