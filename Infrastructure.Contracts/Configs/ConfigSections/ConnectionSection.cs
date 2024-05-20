namespace Infrastructure.Contracts.Configs.ConfigSections;

public class ConnectionSection
{
    public Dictionary<string, MsSqlDbSection> RelationalDb { get; set; }
    public Dictionary<string,RedisSection> Redis { get; set; }
}