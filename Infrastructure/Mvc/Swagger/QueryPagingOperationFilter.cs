using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Mvc.Swagger;

public class QueryPagingOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        while (context.ApiDescription.ParameterDescriptions
                   .FirstOrDefault(f => f.Type == typeof(QueryPaging)) is { } parameterDescription)
        {
            if (parameterDescription.Type.CustomAttributes.Any(a => a.AttributeType != typeof(QueryPaging))) continue;
            var openApiParameter = operation.Parameters.FirstOrDefault(t => t.Name == parameterDescription.Name);
            if (openApiParameter == null) continue;
            operation.Parameters.Remove(openApiParameter);
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Offset".ToLower(),
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type= "integer",
                    Format = "int32"
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name= "Limit".ToLower(),
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "integer",
                    Format = "int32"
                }
            });
            context.ApiDescription.ParameterDescriptions.Remove(parameterDescription);
            context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                Name = "Offset".ToLower(),
                Source = BindingSource.Query,
                Type = typeof(int?),
                ParameterDescriptor = new ParameterDescriptor
                {
                    Name = "Offset".ToLower(),
                    ParameterType = typeof(int?)
                }
            });
            context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                Name = "Limit".ToLower(),
                Source = BindingSource.Query,
                Type = typeof(int?),
                ParameterDescriptor = new ParameterDescriptor
                {
                    Name = "Limit".ToLower(),
                    ParameterType = typeof(int?)
                }
            });
        }
    }
}