using Infrastructure.Data.Repositories.EntityFramework.Repositories;

namespace Infrastructure.Data.Repositories.EntityFramework.Extensions;

public static class RepositoryExtensions
{
    public static IQueryable<TEntity> FromSql<TEntity>(this IRepository<TEntity> repository, string sql,
        params object[] parameters) where TEntity : class, new()
        => (repository as Repository<TEntity> ?? throw new ArgumentNullException(nameof(repository))).FromSql(sql,
            parameters);
}