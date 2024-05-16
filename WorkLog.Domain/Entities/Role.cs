using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    public string CreateUser { get; set; }
    public DateTime UpdateTime { get; set; }
    public string UpdateUser { get; set; }
    public Status Status { get; set; }

    public ICollection<Function> RoleFunctions { get; set; } = new HashSet<Function>();
}