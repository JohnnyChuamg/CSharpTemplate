using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class Task
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int ProjectId { get; set; }
    public int AssigneeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public int Status { get; set; }
    public Priority Priority { get; set; }
    public int ParentId { get; set; }
    public int ManHours { get; set; }
    public required string Notes { get; set; }
}