namespace Infrastructure.Data.Repositories;

public interface IRepositoryTransaction
{
    void StartTransaction();
    void CommitTransaction();
    void AbortTransaction();
}