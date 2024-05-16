using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.EntityFramework.Entities;

namespace Infrastructure.Data.EntityFramework.Mapping;

public abstract class EntityMap<TEntity> where TEntity : EntityBase
{
    protected EntityMap(ModelBuilder modelBuilder)
    {
        var entityTypeBuilder = modelBuilder.Entity<TEntity>();
        entityTypeBuilder.HasKey(entity => entity.Id);
        entityTypeBuilder.Property(entity => entity.Id);
        entityTypeBuilder.Property(entity => entity.CreatorId);
        entityTypeBuilder.Property(entity => entity.CreationDate);
        entityTypeBuilder.Property(entity => entity.ModifierId);
        entityTypeBuilder.Property(entity => entity.ModificationDate);
        entityTypeBuilder.Property(entity => entity.TraceId);
        entityTypeBuilder.Property(entity => entity.Id).ValueGeneratedNever().HasColumnName("Id").IsRequired();
        entityTypeBuilder.Property(entity => entity.CreatorId).HasColumnName("CreatorId").IsRequired();
        entityTypeBuilder.Property(entity => entity.CreationDate).HasColumnName("CreationDate").IsRequired();
        entityTypeBuilder.Property(entity => entity.ModifierId).HasColumnName("ModifierId").IsRequired();
        entityTypeBuilder.Property(entity => entity.ModificationDate).HasColumnName("ModificationDate").IsRequired();
        entityTypeBuilder.Property(entity => entity.TraceId).HasColumnName("TraceId").HasMaxLength(50);
    }
}