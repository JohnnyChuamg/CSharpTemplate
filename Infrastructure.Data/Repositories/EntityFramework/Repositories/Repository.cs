using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Data.Repositories.EntityFramework.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly RepositoryFactory _repositoryFactory;
    private readonly bool _isMasterTheSameAsSlave;
    private bool _disposed;

    internal Repository(IRepositoryFactory repositoryFactory)
    {
        var rf = repositoryFactory as RepositoryFactory;

        _repositoryFactory = rf ?? throw new ArgumentNullException(nameof(repositoryFactory));

        _isMasterTheSameAsSlave =
            _repositoryFactory.MasterDbContext.ContextId == _repositoryFactory.SlaveDbContext.ContextId;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
        }
    }

    public bool All(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().All(predicate);
    }

    public bool Any()
        => _repositoryFactory.SlaveDbContext.Set<TEntity>().Any();

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Any(predicate);
    }

    public long Count()
        => _repositoryFactory.SlaveDbContext.Set<TEntity>().Count();

    public long Count(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Count(predicate);
    }

    public void Create(TEntity instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _repositoryFactory.MasterDbContext.Set<TEntity>().Add(instance);
        if (!_isMasterTheSameAsSlave)
        {
            _repositoryFactory.SlaveDbContext.Set<TEntity>().Add(instance);
        }
    }

    public async Task CreateAsync(TEntity instance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        ArgumentNullException.ThrowIfNull(instance);
        await _repositoryFactory.MasterDbContext.Set<TEntity>().AddAsync(instance, cancellationToken);
        if (!_isMasterTheSameAsSlave)
        {
            await _repositoryFactory.SlaveDbContext.Set<TEntity>().AddAsync(instance, cancellationToken);
        }
    }

    public bool Delete(TEntity instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        var slaveEntry = _repositoryFactory.SlaveDbContext.Entry(instance);
        slaveEntry.State = EntityState.Deleted;
        if (_isMasterTheSameAsSlave)
        {
            return true;
        }

        var masterEntry = _repositoryFactory.MasterDbContext.Entry(instance);
        masterEntry.OriginalValues.SetValues(slaveEntry.OriginalValues);
        masterEntry.State = slaveEntry.State;
        return true;
    }

    public bool Delete(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        var queryable = _repositoryFactory.SlaveDbContext.Set<TEntity>().Where(predicate);
        if (!queryable.Any())
        {
            return false;
        }

        foreach (var entity in queryable)
        {
            Delete(entity);
        }

        return true;
    }

    public Task<bool> DeleteAsync(TEntity instance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return Task.FromResult(Delete(instance));
    }

    public Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return Task.FromResult(Delete(predicate));
    }

    public bool Exists(TEntity instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Any(entity => entity.Equals(instance));
    }

    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Any(predicate);
    }

    public Task<bool> ExistsAsync(TEntity instance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        ArgumentNullException.ThrowIfNull(instance);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>()
            .AnyAsync(entity => entity.Equals(instance), cancellationToken);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public IEnumerable<TEntity> Read()
        => _repositoryFactory.SlaveDbContext.Set<TEntity>().AsEnumerable();

    public IEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Where(predicate).AsEnumerable();
    }

    public Task<IEnumerable<TEntity>> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return Task.FromResult(Read());
    }

    public Task<IEnumerable<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        ArgumentNullException.ThrowIfNull(predicate);
        return Task.FromResult(Read(predicate));
    }

    public IQueryable<TEntity> Query()
        => _repositoryFactory.SlaveDbContext.Set<TEntity>().AsQueryable();

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _repositoryFactory.SlaveDbContext.Set<TEntity>().Where(predicate);
    }

    public Task<IQueryable<TEntity>> QueryAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return Task.FromResult(Query());
    }

    public Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        ArgumentNullException.ThrowIfNull(predicate);
        return Task.FromResult(Query(predicate));
    }

    public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
        => _repositoryFactory.SlaveDbContext.Set<TEntity>().FromSqlRaw(sql, parameters);

    public Task<IQueryable<TEntity>> FromSqlAsync(string sql, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(sql);
        return Task.FromResult(FromSql(sql, parameters));
    }

    public bool Update(TEntity instance)
    {
        return Update(instance, null);
    }

    public bool Update(TEntity instance, Action<TEntity>? result)
    {
        ArgumentNullException.ThrowIfNull(instance);
        result?.Invoke(instance);
        _repositoryFactory.MasterDbContext.ChangeTracker.TrackGraph(instance, delegate(EntityEntryGraphNode e)
        {
            if (_isMasterTheSameAsSlave) return;
            var entityEntry = _repositoryFactory.SlaveDbContext.Entry(e.Entry.Entity);
            e.Entry.OriginalValues.SetValues(entityEntry.OriginalValues);
            e.Entry.State = entityEntry.State;
        });
        return true;
    }

    public Task<bool> UpdateAsync(TEntity instance, CancellationToken cancellationToken = default)
        => UpdateAsync(instance, null, cancellationToken);

    public Task<bool> UpdateAsync(TEntity instance, Action<TEntity>? result,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        return Task.FromResult(Update(instance, result));
    }
}