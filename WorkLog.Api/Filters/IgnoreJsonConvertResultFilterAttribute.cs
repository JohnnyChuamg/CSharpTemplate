using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Infrastructure.Mvc.Filters;

public class IgnoreJsonConvertResultFilterAttribute(
    IOptions<JsonOptions>? options,
    IEnumerable<Type>? ignoreConvertTypes,
    string exceptHeaderKey = "X-Requested-From",
    string exceptHeaderValue = "Web")
    : ResultFilterAttribute
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions = options?.Value.JsonSerializerOptions;

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.HttpContext.Request.Headers.TryGetValue(exceptHeaderKey, out var value)
            && !string.IsNullOrWhiteSpace(value.ToString())
            && value.ToString().Equals(exceptHeaderValue, StringComparison.OrdinalIgnoreCase))
        {
            base.OnResultExecuting(context);
            return;
        }

        if (_jsonSerializerOptions != null && (ignoreConvertTypes?.Any() ?? false))
        {
            if (context.Result is ObjectResult objectResult)
            {
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = _jsonSerializerOptions.AllowTrailingCommas,
                    DefaultBufferSize = _jsonSerializerOptions.DefaultBufferSize,
                    Encoder = _jsonSerializerOptions.Encoder,
                    DictionaryKeyPolicy = _jsonSerializerOptions.DictionaryKeyPolicy,
                    DefaultIgnoreCondition = _jsonSerializerOptions.DefaultIgnoreCondition,
                    IgnoreReadOnlyProperties = _jsonSerializerOptions.IgnoreReadOnlyProperties,
                    MaxDepth = _jsonSerializerOptions.MaxDepth,
                    PropertyNamingPolicy = _jsonSerializerOptions.PropertyNamingPolicy,
                    PropertyNameCaseInsensitive = _jsonSerializerOptions.PropertyNameCaseInsensitive,
                    ReadCommentHandling = _jsonSerializerOptions.ReadCommentHandling,
                    WriteIndented = _jsonSerializerOptions.WriteIndented,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };
                foreach (var jsonConvert in _jsonSerializerOptions.Converters
                             .Where(w => !ignoreConvertTypes.Contains(w.GetType())).ToList())
                {
                    options.Converters.Add(jsonConvert);
                }
                objectResult.Formatters.Insert(0, new SystemTextJsonOutputFormatter(options));
            }
        }

        base.OnResultExecuting(context);
    }
}