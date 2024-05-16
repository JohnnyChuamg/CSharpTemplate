using Infrastructure.Contracts.Configs.ConfigSections;

namespace Infrastructure.Contracts.Configs;

public abstract class Config
{
    private const string DEFAULT_VERSION = "1.0.0";
    
    public string? Id { get; set; }
    public string Version { get; set; } = DEFAULT_VERSION;
    public long? MachineId { get; set; } = 0L;
    public long? DataCenterId { get; set; } = 0L;
    public virtual ConnectionSection Connections { get; set; }
}

public abstract class Config<T> : Config
{
    public virtual T AppSettings { get; set; }
}