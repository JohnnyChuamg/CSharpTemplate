namespace Infrastructure.Data.EntityFramework.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public long CreatorId { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public long ModifierId { get; set; }
    public DateTimeOffset ModificationDate { get; set; }
    public string TraceId { get; set; }
}