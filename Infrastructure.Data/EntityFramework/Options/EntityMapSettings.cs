namespace Infrastructure.Data.EntityFramework.Options;

public class EntityMapSettings
{
    #nullable disable
    public IEnumerable<Type> ExtraMaps { get; set; }
    public bool IgnoreCreatorId { get; set; }
    public bool IgnoreCreationDate { get; set; }
    public bool IgnoreModifierId { get; set; }
    public bool IgnoreModificationDate { get; set; }
}