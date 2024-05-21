using Microsoft.AspNetCore.Http.Extensions;
using Serilog;

namespace WorkLog.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((w, s) =>
            {
                var host = w.HostingEnvironment;
                s.SetBasePath(host.ContentRootPath)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{host.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();
            })
            .UseSerilog((w, c) =>
            {
                var host = w.HostingEnvironment;
                var configure = new ConfigurationBuilder().SetBasePath(host.ContentRootPath)
                    .AddJsonFile("serilog.json", false, true)
                    .AddJsonFile($"serilog.{host.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();
                c.ReadFrom.Configuration(configure);
                c.Destructure.ByTransformingWhere<HttpRequest>(type => typeof(HttpRequest).IsAssignableFrom(type),
                    value => new
                    {
                        value.Method,
                        Url = value.GetDisplayUrl(),
                        value.Headers,
                        value.Cookies,
                        Form = value.HasFormContentType ? value.Form : null,
                        value.Body,
                        value.Query
                    });
            })
            .ConfigureWebHostDefaults((webBuilder) => { webBuilder.UseStartup<Startup>(); })
            .ConfigureServices(services =>
            {
                // services.AddHostedService<>()
            });
}