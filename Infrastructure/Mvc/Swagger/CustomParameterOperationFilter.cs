using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Mvc.Swagger;

public class CustomParameterOperationFilter(IList<OpenApiParameter>? parameters) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (parameters == null)
        {
            return;
        }

        if (operation.Parameters == null)
        {
            operation.Parameters = parameters;
            return;
        }

        foreach (var parameter in parameters)
        {
            if (operation.Parameters.All(a => a.Name != parameter.Name))
            {
                operation.Parameters.Add(parameter);
            }
        }
    }
}