using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Infrastructure.Mvc.ModelBinding;

public class ArrayBinder<T> : IModelBinder
{
    public const string SEPARATOR = "";

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var key = string.IsNullOrWhiteSpace(bindingContext.ModelName)
            ? bindingContext.FieldName
            : bindingContext.ModelName;

        var text = bindingContext.ValueProvider.GetValue(key).Values.ToString();
        if (bindingContext.ModelMetadata is not DefaultModelMetadata defaultModelMetadata)
        {
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.CompletedTask;
        }

        var source = text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        var convert = TypeDescriptor.GetConverter(defaultModelMetadata.ElementType);
        if (!source.All(t => convert.IsValid(t)))
        {
            return Task.CompletedTask;
        }

        var model = source.Select(t => convert.ConvertFrom(t)).Cast<T>().ToArray();
        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}