using Infrastructure.Data.EntityFramework;
using Infrastructure.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WorkLog.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        //services.Configure<ConfigSettings>(Configuration);
        services.ConfigureDependencyInjections();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbContextIngress dbContext)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        dbContext.Master?.Database.EnsureCreated();

        app.UseHealthChecks(new PathString("/ping"),
            new HealthCheckOptions
            {
                ResponseWriter = (context, report) =>
                {
                    var description = string.Join("\n", report.Entries.Select(entry => entry.Value.Description));
                    return context.Response.WriteAsync(description);
                }
            });

        app.UseCors(builder =>
        {
            builder.SetIsOriginAllowed(host =>
            {
                var allowList = Configuration.GetValue("AllowedHosts", string.Empty)
                    ?.Split(";", StringSplitOptions.RemoveEmptyEntries);
                return allowList != null &&
                       allowList.Any(a => a.Equals(host, StringComparison.OrdinalIgnoreCase) || a == "*");
            });
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowCredentials();
        });

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}