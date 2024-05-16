using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.EntityFramework.Enums;

namespace Infrastructure.Data.EntityFramework.Options;

public class DbContextSetting
{
    public NodeType NodeType { get; set; }
    public required DbContextOptionsBuilder OptionsBuilder { get; set; }
}