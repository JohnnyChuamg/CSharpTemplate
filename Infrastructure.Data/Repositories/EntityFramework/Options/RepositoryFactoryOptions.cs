using Infrastructure.Data.EntityFramework.Options;

namespace Infrastructure.Data.Repositories.EntityFramework.Options;

public class RepositoryFactoryOptions
{
    public DbContextOption DbContextOptions { get; set; } = new DbContextOption();
}