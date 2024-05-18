using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingFilter : Attribute, IResourceFilter, IFilterMetadata
{
#nullable disable
    private readonly string[] _removeFactoryFullNames =
    [
        "Microsoft.AspNetCore.Mvc.ModelBinding.FormFileValueProviderFactory",
        typeof(FormValueProviderFactory).FullName,
        typeof(JQueryFormValueProviderFactory).FullName
    ];

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var valueProviderFactories = context.ValueProviderFactories;
        foreach (var valueProviderFactory in valueProviderFactories.Where(f =>
                     _removeFactoryFullNames.Contains(f.GetType().FullName)))
        {
            valueProviderFactories.Remove(valueProviderFactory);
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}