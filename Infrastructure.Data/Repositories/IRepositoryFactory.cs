namespace Infrastructure.Data.Repositories;

public interface IRepositoryFactory : IRepositoryTransaction, IDisposable
{
    IRepository<TEntity> Create<TEntity>() where TEntity : class, new();
}