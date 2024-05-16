using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public required string HttpMethod { get; set; }
    public required string Route { get; set; }
    public Status Status { get; set; } = Status.Enable;
}