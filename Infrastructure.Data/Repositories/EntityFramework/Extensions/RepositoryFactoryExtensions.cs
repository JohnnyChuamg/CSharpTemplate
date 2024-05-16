using System.Data.Common;
using Infrastructure.Data.Repositories.EntityFramework.Repositories;

namespace Infrastructure.Data.Repositories.EntityFramework.Extensions;

public static class RepositoryFactoryExtensions
{
    public static int SaveChanges(this IRepositoryFactory repositoryFactory)
        => (repositoryFactory as RepositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory)))
            .SaveChanges();

    public static DbParameter CreateParameter(this IRepositoryFactory repositoryFactory)
        => (repositoryFactory as RepositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory)))
            .CreateParameter();

    public static int ExecuteSqlCommand(this IRepositoryFactory repositoryFactory, string sql,
        IEnumerable<object> parameters)
        => (repositoryFactory as RepositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory)))
            .ExecuteSqlCommand(sql, parameters);

    public static int ExecuteSqlCommand(this IRepositoryFactory repositoryFactory, string sql,
        params object[] parameters)
        => (repositoryFactory as RepositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory)))
            .ExecuteSqlCommand(sql, parameters);
}