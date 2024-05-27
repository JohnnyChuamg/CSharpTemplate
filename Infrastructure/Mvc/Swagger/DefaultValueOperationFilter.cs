using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Mvc.Swagger;

public class DefaultValueOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var apiParameterDescription =
                apiDescription.ParameterDescriptions.FirstOrDefault(f => f.Name == parameter.Name);
            if (parameter.Description == null && apiParameterDescription != null)
            {
                parameter.Description = apiParameterDescription.ModelMetadata.Description;
            }

            if (parameter.Schema is { Default: null }
                && apiParameterDescription is { DefaultValue: not null })
            {
                parameter.Schema.Default = new OpenApiString(apiParameterDescription.DefaultValue.ToString());
            }

            parameter.Required |= apiParameterDescription?.IsRequired ?? false;
        }
    }
}