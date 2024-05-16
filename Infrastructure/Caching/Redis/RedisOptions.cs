using Microsoft.Extensions.Logging;

namespace Infrastructure.Caching.Redis;

public class RedisOptions
{
    public string Connection { get; set; }
    
    public string Instance { get; set; }
    
    public int MultiGetBatchSize { get; set; }

    public int ScanSize { get; set; } = 1000;
    
    public ILogger Logger { get; internal set; }
}