using System.Collections;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Mvc.Swagger;

public class ResultExampleOperationFilter : IOperationFilter
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        SetRequestExample(operation, context);
        SetResponseExample(operation, context);
    }

    private void SetRequestExample(OpenApiOperation operation, OperationFilterContext context)
    {
    }

    private void SetResponseExample(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses)
        {
            response.Deconstruct(out var key, out var openApiResponse);

            if (openApiResponse.Content.ContainsKey(MediaTypeNames.Application.Json) ||
                !int.TryParse(key, out var result))
            {
                continue;
            }

            var apiResponseType =
                context.ApiDescription.SupportedResponseTypes.First(f => f.StatusCode == int.Parse(key));

            OpenApiString? openApiString = null;

            switch (result)
            {
                case 200:
                    if (typeof(FileResult).IsAssignableFrom(apiResponseType.Type?.GenericTypeArguments[0]))
                    {
                        return;
                    }

                    if (apiResponseType.Type?.GenericTypeArguments.Length != 0)
                    {
                        if (typeof(FileResult).IsAssignableFrom(apiResponseType.Type?.GenericTypeArguments[0]))
                        {
                            return;
                        }

                        var type = apiResponseType.Type?.GenericTypeArguments[0] ??
                                   throw new InvalidOperationException();

                        var obj2 = CreateInstance(type);
                        var result3 = Result.Success(obj2);
                        if (obj2 is ICollection)
                        {
                            result3.Paging = new Paging();
                        }

                        openApiString = new OpenApiString(JsonSerializer.Serialize(result3, _jsonOptions));
                    }

                    break;
                case 201:
                    if (typeof(FileResult).IsAssignableFrom(apiResponseType.Type?.GenericTypeArguments[0]))
                    {
                        return;
                    }

                    if (apiResponseType.Type?.GenericTypeArguments.Length != 0)
                    {
                        if (typeof(FileResult).IsAssignableFrom(apiResponseType.Type?.GenericTypeArguments[0]))
                        {
                            return;
                        }

                        var obj = CreateInstance(apiResponseType.Type?.GenericTypeArguments[0] ?? throw new InvalidOperationException());
                        var result2 = Result.SuccessCreated(obj);
                        if (obj is ICollection)
                        {
                            result2.Paging = new Paging();
                        }

                        openApiString = new OpenApiString(JsonSerializer.Serialize(result2, _jsonOptions));
                    }
                    else
                    {
                        openApiString =
                            new OpenApiString(JsonSerializer.Serialize(Result.SuccessCreated(), _jsonOptions));
                    }

                    break;
                case 204:
                    openApiString =
                        new OpenApiString(JsonSerializer.Serialize(Result.SuccessNoContent(), _jsonOptions));
                    break;
                case 400:
                    openApiString = new OpenApiString(JsonSerializer.Serialize(Result.InvalidInput(), _jsonOptions));
                    break;
                case 404:
                    openApiString = new OpenApiString(JsonSerializer.Serialize(Result.NotFound(), _jsonOptions));
                    break;
                case 406:
                    openApiString =
                        new OpenApiString(JsonSerializer.Serialize(Result.Create(ResultCode.ResourceNotAcceptable),
                            _jsonOptions));
                    break;
                case 409:
                    openApiString = new OpenApiString(JsonSerializer.Serialize(Result.Conflict(), _jsonOptions));
                    break;
                case 500:
                    openApiString =
                        new OpenApiString(JsonSerializer.Serialize(Result.InternalServerError(), _jsonOptions));
                    break;
            }

            if (openApiString != null)
            {
                openApiResponse.Content[MediaTypeNames.Application.Json].Example = openApiString;
            }
        }
    }

    private object? CreateInstance(Type type, Type? parentType = null)
    {
        if (type.IsValueType)
        {
            return type.GenericTypeArguments.Length == 1
                ? CreateInstance(type.GenericTypeArguments[0])
                : Activator.CreateInstance(type);
        }

        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(string[]))
        {
            return new[] { "123456789" };
        }

        if (typeof(ICollection).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type))
        {
            var obj = (IList?)Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GenericTypeArguments));
            obj?.Add(CreateInstance(type.GenericTypeArguments[0], parentType));
            return obj;
        }

        var list = new List<object?>();
        if (type.GenericTypeArguments.Length != 0)
        {
            var genericTypeArguments = type.GenericTypeArguments;
            list.AddRange(genericTypeArguments.Select(type2 => CreateInstance(type2)).Where(w => w != null));
        }

        var obj2 = ((list.Count > 0) ? Activator.CreateInstance(type, list.ToArray()) : Activator.CreateInstance(type));
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        {
            if (parentType != null && propertyInfo.PropertyType == parentType) continue;
            object? obj3 = null;
            if (propertyInfo.CanRead)
            {
                obj3 = propertyInfo.GetValue(obj2);
            }

            if (obj3 == null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj2, CreateInstance(propertyInfo.PropertyType, propertyInfo.PropertyType));
            }
        }

        return obj2;
    }
}