using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Infrastructure.Caching.Redis;
using Infrastructure.Caching.Redis.Providers;
using Infrastructure.Data.EntityFramework.Enums;
using Infrastructure.Data.EntityFramework.Options;
using Infrastructure.Data.Repositories.EntityFramework.Extensions;
using Infrastructure.Extensions.DependencyInjection;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.InjectionConfigs;

[Injection]
public class DbConfig
{
    private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public DbConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        var config = options.Value;

        services.AddRedis<CachingCsRedis>(new RedisOptions
        {
            Connection = config.Connections.Redis["Default"].Connection,
            Instance = config.Connections.Redis["Default"].Instance ?? string.Empty
        });
        
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