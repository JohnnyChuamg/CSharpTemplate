namespace WorkLog.Domain.Entities;

public class RoleFunction
{
    public int RoleId { get; set; }
    public int FunctionId { get; set; }

    public required ICollection<Function> Functions { get; set; } = new HashSet<Function>();
}