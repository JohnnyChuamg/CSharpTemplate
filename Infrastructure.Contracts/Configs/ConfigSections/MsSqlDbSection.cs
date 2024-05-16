namespace Infrastructure.Contracts.Configs.ConfigSections;

public class MsSqlDbSection : RelationDbSection
{
    public string? Database { get; set; }
}