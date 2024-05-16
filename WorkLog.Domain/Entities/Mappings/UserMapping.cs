using Infrastructure.Data.EntityFramework.Mapping;
using Microsoft.EntityFrameworkCore;

namespace WorkLog.Domain.Entities.Mappings;

public class UserMapping : EntityMap<User>
{
    public UserMapping(ModelBuilder modelBuilder) : base(modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();

        builder.ToTable(nameof(User));

        builder.Property(p => p.Name)
            .HasColumnName(nameof(User.Name))
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Username)
            .HasColumnName(nameof(User.Username))
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Password)
            .HasColumnName(nameof(User.Password))
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasColumnName(nameof(User.Email))
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.RoleId)
            .HasColumnName(nameof(User.RoleId));

        builder.Property(p => p.DepartmentId)
            .HasColumnName(nameof(User.DepartmentId));

        builder.Property(p => p.Status)
            .HasColumnName(nameof(User.Status))
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.LastLoginTime)
            .HasColumnName(nameof(User.LastLoginTime));
    }
}