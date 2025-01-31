using Infrastructure.Data.EntityFramework.Entities;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class User: EntityBase
{
    #nullable disable
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public long RoleId { get; set; }
    public long DepartmentId { get; set; }
    public Status Status { get; set; }
    public DateTime LastLoginTime { get; set; }

    //public required Role Role { get; set; } = new ();
    //public required Department Department { get; set; }
    //public required ICollection<Project> Projects { get; set; } = new HashSet<Project>();
}