using System.Text.Json.Serialization;

namespace Infrastructure.Serialization.JsonSerializers.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreJsonConvertAttribute : JsonAttribute
{
    public Type[]? IgnoreTypes { get; set; }

    public IgnoreJsonConvertAttribute()
    {
    }

    public IgnoreJsonConvertAttribute(params Type[]? ignoreType)
    {
        IgnoreTypes = ignoreType;
    }
}