using System.Net.Mime;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;

using Infrastructure.Contracts.ResultContracts;
using Infrastructure.Extensions.DependencyInjection;
using Infrastructure.Mvc.Filters;
using Infrastructure.Mvc.ModelBinding;
using Infrastructure.Serialization.JsonSerializers.Converts;
using WorkLog.Api.Diagnostics;
using WorkLog.Application.Infrastructures.Contracts;
using WorkLog.Application.Services;

namespace WorkLog.Api.InjectionConfigs;

[Injection]
public class MvcConfig
{
    public MvcConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        services.AddCors()
            .AddHealthChecks()
            .AddCheck<ServiceHealthCheck>(nameof(ServiceHealthCheck))
            ;
        services.AddHttpContextAccessor()
            .AddHttpClient()
            .AddControllers(option =>
            {
                option.Filters.Add<CustomExceptionFilterAttribute>();

                option.Filters.Add(new IgnoreJsonConvertResultFilterAttribute(
                    services.GetService<IOptions<JsonOptions>>(), new[]
                    {
                        typeof(LongToStringJsonConvert),
                        typeof(NullableLongToStringJsonConvert)
                    }));
                // option.Filters.Add(new AuthorizeFilter(JwtBearerDefaults.AuthenticationScheme)); 
                option.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                option.Filters.Add(new ProducesResponseTypeAttribute(typeof(Result),
                    ResultHelper.ConvertHttpStatusCode(ResultCode.InvalidInput)));
                option.Filters.Add(new ProducesResponseTypeAttribute(typeof(Result),
                    ResultHelper.ConvertHttpStatusCode(ResultCode.ResourceNotFound)));
                option.Filters.Add(new ProducesResponseTypeAttribute(typeof(Result),
                    ResultHelper.ConvertHttpStatusCode(ResultCode.InternalServerError)));
                option.ModelBinderProviders.Insert(0,new PagingBinderProvider());
            })
            .AddJsonOptions(jsonOption =>
            {
                jsonOption.JsonSerializerOptions.Converters.Add(new NativeByteArrayJsonConvert());
                jsonOption.JsonSerializerOptions.Converters.Add(new LongToStringJsonConvert());
                jsonOption.JsonSerializerOptions.Converters.Add(new NullableLongToStringJsonConvert());
                jsonOption.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                jsonOption.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                jsonOption.JsonSerializerOptions.WriteIndented = false;
                jsonOption.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonOption.JsonSerializerOptions.Encoder =
                    JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
            })
            ;
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(typeof(IService).GetTypeInfo().Assembly)
            ;

        ((IServiceCollection)services).AddApiVersioning(option =>
        {
            option.ReportApiVersions = false;
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(option =>
        {
            option.GroupNameFormat = "'V'VVV";
            option.SubstituteApiVersionInUrl = true;
        });
        
        services.Configure<ApiBehaviorOptions>(option => option.SuppressModelStateInvalidFilter = true);
    }
}