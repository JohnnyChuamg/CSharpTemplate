using System.Reflection;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Infrastructure.Abstraction;
using Infrastructure.Data.EntityFramework.Entities;
using Infrastructure.Data.EntityFramework.Enums;
using Infrastructure.Data.EntityFramework.Mapping;
using Infrastructure.Data.EntityFramework.Options;

namespace Infrastructure.Data.EntityFramework.DbContext;

public class RelationalDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly EntityMapSettings _entityMapSettings;
    private readonly ClaimsPrincipal? _identity;
    internal NodeType NodeType { get; set; }

    internal RelationalDbContext(Microsoft.EntityFrameworkCore.DbContextOptions dbContextOptions,
        EntityMapSettings entityMapSettings, IGenericContainer<ClaimsPrincipal> identity) : base(dbContextOptions)
    {
        _identity = identity.Content;
        _entityMapSettings = entityMapSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var typeEntityMap = typeof(EntityMap<>).GetTypeInfo();

        var typeInfos =
            from typeInfo in AppDomain.CurrentDomain.GetAssemblies().SelectMany(typeInfo => typeInfo.GetTypes())
            where !typeInfo.IsGenericType && typeInfo.IsClass && typeInfo.BaseType.Name == typeEntityMap.Name
            select typeInfo;

        foreach (var typeInfo in typeInfos)
        {
            var baseType = typeInfo.BaseType;
            object? obj;
            if (baseType == null)
            {
                obj = null;
            }
            else
            {
                var genericTypeArguments = baseType.GenericTypeArguments;
                obj = genericTypeArguments[0];
            }

            var type2 = (Type?)obj;
            if (type2 == null)
            {
                throw new ArgumentNullException(nameof(baseType.GenericTypeArguments));
            }

            Activator.CreateInstance(typeInfo, modelBuilder);
            if (_entityMapSettings.IgnoreCreatorId)
            {
                modelBuilder.Entity(type2).Ignore("CreatorId");
            }

            if (_entityMapSettings.IgnoreCreationDate)
            {
                modelBuilder.Entity(type2).Ignore("CreationDate");
            }

            if (_entityMapSettings.IgnoreModifierId)
            {
                modelBuilder.Entity(type2).Ignore("ModifierId");
            }

            if (_entityMapSettings.IgnoreModificationDate)
            {
                modelBuilder.Entity(type2).Ignore("ModificationDate");
            }
        }

        if (_entityMapSettings.ExtraMaps?.Any() ?? false)
        {
            foreach (var extraMap in _entityMapSettings.ExtraMaps)
            {
                Activator.CreateInstance(extraMap, modelBuilder);
            }
        }

        foreach (var mutableForeignKey in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(type => type.GetForeignKeys()))
        {
            mutableForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public override int SaveChanges()
    {
        (from entry in ChangeTracker.Entries<EntityBase>()
            where entry.State is EntityState.Added or EntityState.Modified
            select entry).ToList().ForEach(delegate(EntityEntry<EntityBase> entry)
        {
            var num = (long.TryParse(_identity?.Identity?.Name, out var result) ? result : 0);
            var utcNow = DateTimeOffset.UtcNow;
            if (entry.State == EntityState.Added)
            {
                if (!_entityMapSettings.IgnoreCreatorId)
                {
                    entry.Entity.CreatorId = num;
                }

                if (!_entityMapSettings.IgnoreCreationDate)
                {
                    entry.Entity.CreationDate = utcNow;
                }
            }
            else
            {
                if (!_entityMapSettings.IgnoreModifierId)
                {
                    entry.Entity.ModifierId = num;
                }

                if (!_entityMapSettings.IgnoreModificationDate)
                {
                    entry.Entity.ModificationDate = utcNow;
                }
            }
        });
        return base.SaveChanges();
    }
}