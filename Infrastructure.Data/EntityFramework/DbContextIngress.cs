using Infrastructure.Abstraction;
using Infrastructure.Data.EntityFramework.DbContext;
using Infrastructure.Data.EntityFramework.Enums;

namespace Infrastructure.Data.EntityFramework;

public class DbContextIngress(ServiceResolver<NodeType, RelationalDbContext> serviceResolver)
{
    private readonly ServiceResolver<NodeType, RelationalDbContext?> _serviceResolver = serviceResolver;
    private RelationalDbContext? _master;
    private RelationalDbContext? _slave;
    public RelationalDbContext Master => (_master ??= _serviceResolver(NodeType.Master)) ?? throw new InvalidOperationException();

    public RelationalDbContext Slave
    {
        get
        {
            var relationalDbContext = _slave;
            if (relationalDbContext != null) return relationalDbContext;

            var obj = _serviceResolver(NodeType.Slave) ?? Master;
            _slave = obj;
            return obj;
        }
    }
}