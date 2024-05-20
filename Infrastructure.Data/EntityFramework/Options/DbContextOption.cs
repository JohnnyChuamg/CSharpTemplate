namespace Infrastructure.Data.EntityFramework.Options;

public class DbContextOption
{
    #nullable disable
    public EntityMapSettings EntityMapSettings { get; set; } = new EntityMapSettings();
    public IEnumerable<DbContextSetting> DbContextSettings { get; set; }
}