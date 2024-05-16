using System.ComponentModel;

namespace Infrastructure.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class InjectionAttribute: Attribute
{
    [Description("Sortting Index")]
    [DefaultValue(0)]
    public int Index { get; set; }

    public InjectionAttribute()
    {
    }

    public InjectionAttribute(int index)
    {
        Index = index;
    }
}