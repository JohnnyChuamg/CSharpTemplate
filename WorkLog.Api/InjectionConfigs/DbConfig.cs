using Infrastructure.Data.EntityFramework.Enums;
using Microsoft.Extensions.Options;
using Infrastructure.Data.Repositories.EntityFramework.Extensions;
using Infrastructure.Data.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.InjectionConfigs;

public class DbConfig
{
    private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public DbConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        var config = options.Value;
        services.AddRepositoryFactory(option =>
        {
            //option.DbContextOptions.EntityMapSettings.ExtraMaps = new[] { typeof(ClientMap) };
            option.DbContextOptions.DbContextSettings = new[]
            {
                new DbContextSetting
                {
                    NodeType = NodeType.Master,
                    OptionsBuilder = new DbContextOptionsBuilder()
                        .UseLoggerFactory(MyLoggerFactory)
                        .UseSqlServer(config.Connections.MsSqlDb["Master"].Connection)
                },
                new DbContextSetting
                {
                    NodeType = NodeType.Slave,
                    OptionsBuilder = new DbContextOptionsBuilder()
                        .UseLoggerFactory(MyLoggerFactory)
                        .UseSqlServer(config.Connections.MsSqlDb["Slave"].Connection)
                }
            };
        });
    }
}