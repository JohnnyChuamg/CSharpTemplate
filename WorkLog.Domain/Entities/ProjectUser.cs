namespace WorkLog.Domain.Entities;

public class ProjectUser
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public int ManHours { get; set; }
}