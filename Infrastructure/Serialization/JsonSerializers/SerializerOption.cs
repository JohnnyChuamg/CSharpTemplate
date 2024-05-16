namespace Infrastructure.Serialization.JsonSerializers;

public class SerializerOption
{
    public bool AllowTrailingCommas { get; set; }
    public bool IgnoreNullValues { get; set; } = true;
    public bool IgnoreReadOnlyProperties { get; set; }
    public bool PropertyNameCaseInsensitive { get; set; } = true;
    public bool WriteIndented { get; set; }
    public NamingPolicy DictionaryKeyPolicy { get; set; }
    public NamingPolicy PropertyNamingPolicy { get; set; }
}