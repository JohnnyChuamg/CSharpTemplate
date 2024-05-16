namespace Infrastructure.Contracts.Configs.ConfigSections;

public class ConnectionSection
{
    public Dictionary<string, MsSqlDbSection> MsSqlDb { get; set; }
    public Dictionary<string,RedisSection> Redis { get; set; }
}