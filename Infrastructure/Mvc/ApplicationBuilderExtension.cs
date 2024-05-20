using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.Mvc;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseRequestId(this IApplicationBuilder app)
        => UseRequestId(app, "X-REQUEST-ID");

    public static IApplicationBuilder UseRequestId(this IApplicationBuilder app, params string[] paramNames)
        => app.Use(async delegate(HttpContext context, Func<Task> func)
        {
            foreach (var key in paramNames)     
            {
                
                if (context.Request.Headers.TryGetValue(key, out var value)
                    && !StringValues.IsNullOrEmpty(value))
                {
                    context.Response.Headers[key] = value;
                }
                else
                {
                    context.Request.Headers[key] = new StringValues(context.TraceIdentifier);
                    context.Response.Headers[key] = new StringValues(context.TraceIdentifier);
                }
            }

            await func();
        });

    public static IApplicationBuilder UseMyIp(this IApplicationBuilder app, string path = "/MyIP")
        => app.Use(async delegate(HttpContext context, Func<Task> func)
        {
            if (!context.Request.Path.HasValue
                || !context.Request.Path.Value.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            {
                await func();
            }
            else
            {
                var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var header in context.Request.Headers)
                {
                    dictionary[header.Key] = header.Value.ToString();
                    dictionary["Client-IP"] = context.Connection.RemoteIpAddress.ToString();
                    var array = JsonSerializer.SerializeToUtf8Bytes(dictionary, new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = false
                    });
                    
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    context.Response.ContentLength = array.Length;
                    if (context.Request.Query.ContainsKey("download"))
                    {
                        context.Response.Headers.Add("Content-Disposition",
                            new StringValues("attachment; filename=MyIP-" + DateTimeOffset.UtcNow.Ticks + ".json"));
                    }

                    context.Response.StatusCode = ResultHelper.ConvertHttpStatusCode(ResultCode.Success);

                    await new MemoryStream(array).CopyToAsync(context.Response.Body);
                }
            }
            await func();
        });
}