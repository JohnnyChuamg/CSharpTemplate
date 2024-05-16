using System.Collections;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int ManagerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public required Priority Priority { get; set; }
    
    public required User Manager { get; set; }
    public required ICollection<User> Users { get; set; } = new HashSet<User>();
    public required ICollection<Task> WorkLogs { get; set; } = new HashSet<Task>();
}