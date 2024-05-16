using System.Collections.Concurrent;
using System.Diagnostics;
using CSRedis;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Caching.Redis.Providers;

public class CachingCsRedis : IRedis
{
    private static readonly ConcurrentDictionary<string, CSRedisClient.SubscribeObject> Subscribes = new();

    private readonly RedisOptions _options;
    private readonly ILogger _logger;
    
    public string Instance => _options.Instance;

    public CachingCsRedis(RedisOptions options)
    {
        _options = options;
        _logger = options.Logger;
        RedisHelper<RedisHelper>.Initialization(new CSRedisClient(options.Connection));
    }

    public TimeSpan Ping(int timeout = 5000)
    {
        var stopWatch = Stopwatch.StartNew();
        if (!RedisHelper<RedisHelper>.Ping())
        {
            throw new RedisClientException("Ping failed");
        }

        stopWatch.Stop();
        return stopWatch.Elapsed;
    }

    public Task<TimeSpan> PingAsync(int timeout = 5000, CancellationToken cancellationToken = default)
    {
        var stopWatch = Stopwatch.StartNew();
        var task = Task.Run(async () => await RedisHelper<RedisHelper>.PingAsync(), cancellationToken);
        if (task.Wait(timeout, cancellationToken) && !task.Result)
        {
            throw new RedisClientException("Ping failed");
        }

        stopWatch.Stop();
        return Task.FromResult(stopWatch.Elapsed);
    }


    public Task<bool> SetAsync(string key, byte[] value, CacheOption? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(value));
        key = GetKeys(key)[0];
        var expireSeconds = options is not { AbsoluteExpiration: not null }
            ? -1
            : (int)options.AbsoluteExpiration.Value.Subtract(DateTimeOffset.UtcNow).TotalSeconds;
        return RedisHelper<RedisHelper>.SetAsync(key, value, expireSeconds);
    }

    public async Task<bool> SetNxAsync(string key, byte[] value, CacheOption? option = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        ArgumentNullException.ThrowIfNull(value);
        key = GetKeys(key)[0];
        var result = await RedisHelper<RedisHelper>.SetNxAsync(key, value);
        if (option?.AbsoluteExpiration.HasValue ?? false)
        {
            await ExpireAsync(key, (int)option.AbsoluteExpiration.Value.Subtract(DateTimeOffset.UtcNow).TotalSeconds, cancellationToken);
        }
        return result;
    }

    public Task<bool> SetAsync(KeyValuePair<string, byte[]>[] keyValuePairs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrEmpty(nameof(keyValuePairs));
        return RedisHelper<RedisHelper>.MSetAsync(keyValuePairs);
    }
    
    public async Task<IEnumerable<byte[]>> GetAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrEmpty(nameof(keys));

        keys = GetKeys(keys);
        List<byte[]> result = [];
        
        await foreach (var item in MGetAsync(keys, _options.MultiGetBatchSize).WithCancellation(cancellationToken))
        {
            if (item != null)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public async Task<IEnumerable<byte[]>> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        return await GetAsync([key], cancellationToken);
    }

    public async Task<long> DelAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        var keys = GetKeys(key);
        var result = 0L;
        if (keys.Length == 0)
        {
            return result;
        }

        if (keys.Length <= _options.MultiGetBatchSize)
        {
            return await RedisHelper<RedisHelper>.DelAsync(keys);
        }

        var pageCount = Math.Ceiling((double)keys.Length / (double)_options.MultiGetBatchSize);
        for (var i = 1; i < pageCount; i++)
        {
            result += await RedisHelper<RedisHelper>.DelAsync(
                keys.Skip((i - 1) * _options.MultiGetBatchSize)
                    .Take(_options.MultiGetBatchSize)
                    .ToArray());
        }
        
        return result;
    }

    public Task<long> IncrByAsync(string key, long value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> RPushAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> RPopAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> LPushAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> LPopAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> LRangeAsync(string key, long start, long stop,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> LIndexAsync(string key, long index, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LTrimAsync(string key, long start, long stop, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> LRemAsync(string key, byte[] value, long count, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> RPopLPushAsync(string source, string destination, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> SAddAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> SAddAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> SMembersAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SIsMemberAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> SRemAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> SRemAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> SDiffAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> SInterAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> SUnionAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HSetAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> HGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, byte[]>> HGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> HKeysAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> HIncrByAsync(string key, string field, long value = 1,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HSetNxAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<byte[]>> HMGetAsync(string key, string[] fields,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HMSetAsync(string key, KeyValuePair<string, byte[]> keyValuePair,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HDelAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> HIncrAsync(string key, string field, long value = 1,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expire, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExpireAsync(string key, int seconds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(string topic, Action<string, byte[]> callback,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> PublishAsync(string topic, byte[] message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> KeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> ScanAsync(long cursor, string pattern, int? count,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> ExistsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> LLenAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private string[] GetKeys(params string[]? keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        try
        {
            var array = keys
                .Select(s => !string.IsNullOrWhiteSpace(_options.Instance) ? _options.Instance + ":" + s : s).ToArray();
            var result = Array.Empty<string>();
            result = array.Aggregate(result, (current, text) => text.Contains('*')
                ? current.Union(GetKeysBatch(text, _options.ScanSize)).ToArray()
                : current.Union(new[] { text }).ToArray());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.ToString());
            throw;
        }
    }

    private static IEnumerable<string> GetKeysBatch(string keyPattern, int batchSize = 5)
    {
        var num = 0L;
        do
        {
            var result = RedisHelper<RedisHelper>.Scan(num, keyPattern, batchSize);
            var items = result.Items;
            foreach (var t in items)
            {
                yield return t;
            }

            num = result.Cursor;

        } while (num > 0);
    }

    private static async IAsyncEnumerable<byte[]?> MGetAsync(string[]? keys, int pageSize = 20)
    {
        if (keys == null || keys.Length == 0)
        {
            yield break;
        }

        if (keys.Length == 1)
        {
            yield return await RedisHelper<RedisHelper>.GetAsync<byte[]>(keys[0]);
            yield break;
        }

        var pageCount = Math.Ceiling(keys.Length / (double)pageSize);

        for (var i = 1; i <= pageCount; i++)
        {
            var array = await RedisHelper<RedisHelper>.MGetAsync<byte[]>(keys.Skip((i - 1) * pageSize).Take(pageSize)
                .ToArray());
            foreach (var j in array)
            {
                yield return j;
            }
        }
    }

    public CSRedisClientLock Lock(string name, int timeoutSeconds, bool authDelay = true)
        => RedisHelper<RedisHelper>.Lock(name, timeoutSeconds, authDelay);
}