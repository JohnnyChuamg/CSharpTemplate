using Infrastructure.Abstraction;
using Infrastructure.Data.EntityFramework.DbContext;
using Infrastructure.Data.EntityFramework.Enums;

namespace Infrastructure.Data.EntityFramework;

public class DbContextIngress(ServiceResolver<NodeType, RelationalDbContext> serviceResolver)
{
    #nullable disable
    private RelationalDbContext _master;
    private RelationalDbContext _slave;
    public RelationalDbContext Master => _master ??= serviceResolver(NodeType.Master);

    public RelationalDbContext Slave
    {
        get
        {
            var relationalDbContext = _slave;
            if (relationalDbContext != null) return relationalDbContext;

            var obj = serviceResolver(NodeType.Slave) ?? Master;
            _slave = obj;
            return obj;
        }
    }
}