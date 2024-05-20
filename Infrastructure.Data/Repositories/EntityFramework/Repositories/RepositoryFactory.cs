using System.Data.Common;
using Infrastructure.Data.EntityFramework;
using Infrastructure.Data.EntityFramework.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Data.Repositories.EntityFramework.Repositories;

public class RepositoryFactory(DbContextIngress dbContextIngress)
    : IRepositoryFactory
{
    #nullable disable
    private readonly List<IRepository> _repositories = [];

    private RelationalDbContext _masterDbContext;
    private RelationalDbContext _slaveDbContext;
    private bool _disposed;

    internal RelationalDbContext MasterDbContext => _masterDbContext ??= dbContextIngress.Master;
    internal RelationalDbContext SlaveDbContext => _slaveDbContext ??= dbContextIngress.Slave;

    public void StartTransaction()
    {
        if (MasterDbContext.Database.CurrentTransaction == null)
        {
            MasterDbContext.Database.BeginTransaction();
        }
    }

    public void CommitTransaction()
    {
        if (MasterDbContext.Database.CurrentTransaction != null)
        {
            MasterDbContext.Database.CommitTransaction();
        }
    }

    public void AbortTransaction()
    {
        if (MasterDbContext.Database.CurrentTransaction != null)
        {
            MasterDbContext.Database.RollbackTransaction();
        }
    }

    public IRepository<TEntity> Create<TEntity>() where TEntity : class, new()
    {
        var repository = new Repository<TEntity>(this);
        _repositories.Add(repository);
        return repository;
    }

    public virtual int SaveChanges()
    {
        var flag = MasterDbContext.ContextId == SlaveDbContext.ContextId;
        if (!flag)
        {
            foreach (var item in from entry in SlaveDbContext.ChangeTracker.Entries()
                     where entry.State == EntityState.Unchanged
                     select entry)
            {
                MasterDbContext.Entry(item.Entity).State = item.State;
            }
        }

        var result = MasterDbContext.SaveChanges();
        if (flag)
        {
            return result;
        }

        SlaveDbContext.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        SlaveDbContext.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
        foreach (var item in from entry in SlaveDbContext.ChangeTracker.Entries()
                 where entry.State != EntityState.Unchanged
                 select entry)
        {
            if (item.State == EntityState.Deleted)
            {
                item.State = EntityState.Detached;
                continue;
            }

            item.OriginalValues.SetValues(item.CurrentValues);
            item.State = EntityState.Unchanged;
        }

        MasterDbContext.ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
        MasterDbContext.ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;

        foreach (var item in from entry in MasterDbContext.ChangeTracker.Entries()
                 where entry.State != EntityState.Detached
                 select entry)
        {
            item.State = EntityState.Detached;
        }

        return result;
    }

    public virtual DbParameter CreateParameter()
    {
        using var dbCommand = MasterDbContext.Database.GetDbConnection().CreateCommand();
        return dbCommand.CreateParameter();
    }

    public virtual int ExecuteSqlCommand(string sql, IEnumerable<object> parameters)
        => MasterDbContext.Database.ExecuteSqlRaw(sql, parameters);

    public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        => MasterDbContext.Database.ExecuteSqlRaw(sql, parameters);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }

        _disposed = true;

        foreach (var repository in _repositories)
        {
            try
            {
                repository.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        _repositories.Clear();
        _masterDbContext.Dispose();
        _slaveDbContext.Dispose();
    }
}