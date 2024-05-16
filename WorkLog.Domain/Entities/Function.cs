using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class Function
{
    public required int Id { get; set; }
    public required int ParentId { get; set; }
    public required string Name { get; set; }
    public required Status Status { get; set; } = Status.Enable;

    public ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
}