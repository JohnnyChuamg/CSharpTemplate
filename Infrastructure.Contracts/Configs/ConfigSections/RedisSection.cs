namespace Infrastructure.Contracts.Configs.ConfigSections;

public class RedisSection : ConnectionItemSection
{
    public string? Instance { get; set; }
}