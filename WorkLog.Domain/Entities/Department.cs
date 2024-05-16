using Infrastructure.Data.EntityFramework.Entities;

namespace WorkLog.Domain.Entities;

public abstract class Department : EntityBase
{
    public required string Name { get; set; }

    public virtual required ICollection<User> Users { get; set; } = new HashSet<User>();
}