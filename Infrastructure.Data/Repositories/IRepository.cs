using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories;

public interface IRepository : IDisposable;

public interface IRepository<TEntity> : IRepository where TEntity : class, new()
{
    bool All(Expression<Func<TEntity, bool>> predicate);

    bool Any();

    bool Any(Expression<Func<TEntity, bool>> predicate);

    long Count();

    long Count(Expression<Func<TEntity, bool>> predicate);

    void Create(TEntity instance);

    Task CreateAsync(TEntity instance, CancellationToken cancellationToken = default);

    bool Delete(TEntity instance);

    bool Delete(Expression<Func<TEntity, bool>> predicate);

    Task<bool> DeleteAsync(TEntity instance, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    bool Exists(TEntity instance);

    bool Exists(Expression<Func<TEntity, bool>> predicate);

    Task<bool> ExistsAsync(TEntity instance, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    IEnumerable<TEntity> Read();

    IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> predicate);

    Task<IEnumerable<TEntity>> ReadAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    IQueryable<TEntity> Query();

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

    Task<IQueryable<TEntity>> QueryAsync(CancellationToken cancellationToken = default);

    Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    bool Update(TEntity instance);

    bool Update(TEntity instance, Action<TEntity> result);

    Task<bool> UpdateAsync(TEntity instance, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TEntity instance, Action<TEntity> result, CancellationToken cancellationToken = default);
}