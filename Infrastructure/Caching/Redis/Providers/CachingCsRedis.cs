using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

using CSRedis;
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
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
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
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        key = GetKeys(key)[0];
        var result = await RedisHelper<RedisHelper>.SetNxAsync(key, value);
        if (option?.AbsoluteExpiration.HasValue ?? false)
        {
            await RedisHelper<RedisHelper>.ExpireAsync(key,
                (int)option.AbsoluteExpiration.Value.Subtract(DateTimeOffset.UtcNow).TotalSeconds);
        }

        return result;
    }

    public Task<bool> MSetAsync(KeyValuePair<string, byte[]>[] keyValuePairs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (keyValuePairs.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(keyValuePairs));
        var data = (from keyValuePair in keyValuePairs
            let key = GetKeys(keyValuePair.Key)[0]
            let value = keyValuePair.Value
            select new KeyValuePair<string, byte[]>(key, value)).ToArray();
        return RedisHelper<RedisHelper>.MSetAsync(data);
    }

    public async Task<IEnumerable<byte[]>> GetAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (keys.Length == 0)
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(keys));

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

    public Task<IEnumerable<byte[]>> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        return GetAsync([key], cancellationToken);
    }

    public async Task<long> DelAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
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
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value == 0L)
        {
            throw new ArgumentNullException(nameof(value));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.IncrByAsync(key, value);
    }

    public Task<long> RPushAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.RPushAsync(key, value);
    }

    public Task<byte[]> RPopAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.RPopAsync<byte[]>(key);
    }

    public Task<long> LPushAsync(string key, byte[] value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.LPushAsync(key, value);
    }

    public Task<byte[]> LPopAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.LPopAsync<byte[]>(key);
    }

    public async Task<IEnumerable<byte[]>> LRangeAsync(string key, long start, long stop,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (stop == 0L)
        {
            throw new ArgumentNullException(nameof(stop));
        }

        if (stop == start)
        {
            throw new ArgumentException("stop == start", nameof(stop));
        }

        key = GetKeys(key)[0];

        return await RedisHelper<RedisHelper>.LRangeAsync<byte[]>(key, start, stop);
    }

    public Task<byte[]> LIndexAsync(string key, long index, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.LIndexAsync<byte[]>(key, index);
    }

    public Task<bool> LTrimAsync(string key, long start, long stop, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (stop == 0L)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(stop));
        }

        if (stop == start)
        {
            throw new ArgumentException("stop == start", nameof(stop));
        }

        return RedisHelper<RedisHelper>.LTrimAsync(key, start, stop);
    }

    public Task<long> LRemAsync(string key, byte[] value, long count, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (value.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        return RedisHelper<RedisHelper>.LRemAsync(key, count, value);
    }

    public Task<byte[]> RPopLPushAsync(string source, string destination, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(source))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(source));
        if (string.IsNullOrWhiteSpace(destination))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(destination));
        source = GetKeys(source)[0];
        destination = GetKeys(source)[0];
        return RedisHelper<RedisHelper>.RPopLPushAsync<byte[]>(source, destination);
    }

    public Task<long> SAddAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        if (member.Length == 0)
            ArgumentException.ThrowIfNullOrEmpty(nameof(member));
        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.SAddAsync(key, member);
    }

    public Task<long> SAddAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (members.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(members));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.SAddAsync(key, members);
    }

    public async Task<IEnumerable<byte[]>> SMembersAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
        }

        key = GetKeys(key)[0];
        return await RedisHelper<RedisHelper>.SMembersAsync<byte[]>(key);
    }

    public Task<bool> SIsMemberAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (member.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(member));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.SIsMemberAsync(key, member);
    }

    public Task<long> SRemAsync(string key, byte[] member, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (member.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(member));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.SRemAsync(key, member);
    }

    public Task<long> SRemAsync(string key, byte[][] members, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (members.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(members));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.SRemAsync(key, members);
    }

    public async Task<IEnumerable<byte[]>> SDiffAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (keys.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(keys));
        }

        keys = GetKeys(keys);
        return await RedisHelper<RedisHelper>.SDiffAsync<byte[]>(keys);
    }

    public async Task<IEnumerable<byte[]>> SInterAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (keys.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(keys));
        }

        keys = GetKeys(keys);
        return await RedisHelper<RedisHelper>.SInterAsync<byte[]>(keys);
    }

    public async Task<IEnumerable<byte[]>> SUnionAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (keys.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(keys));
        }

        keys = GetKeys(keys);
        return await RedisHelper<RedisHelper>.SUnionAsync<byte[]>(keys);
    }

    public Task<bool> HSetAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        if (value.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        }

        key = GetKeys(key)[0];

        return RedisHelper<RedisHelper>.HSetAsync(key, field, value);
    }

    public Task<byte[]> HGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        key = GetKeys(key)[0];
        return RedisHelper<RedisHelper>.HGetAsync<byte[]>(key, field);
    }

    public Task<Dictionary<string, byte[]>> HGetAllAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.HGetAllAsync<byte[]>(key);
    }

    public Task<string[]> HKeysAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.HKeysAsync(key);
    }

    public Task<long> HIncrByAsync(string key, string field, long value = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        return RedisHelper<RedisHelper>.HIncrByAsync(key, field, value);
    }

    public Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        return RedisHelper<RedisHelper>.HExistsAsync(key, field);
    }

    public Task<bool> HSetNxAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        if (value.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(value));
        }

        return RedisHelper<RedisHelper>.HSetNxAsync(key, field, value);
    }

    public async Task<IEnumerable<byte[]>> HMGetAsync(string key, string[] fields,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else if (key.IndexOf('*', StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("key does not support wildcard", nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (fields.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(fields));
        }

        return await RedisHelper<RedisHelper>.HMGetAsync<byte[]>(key, fields);
    }

    public Task<bool> HMSetAsync(string key, KeyValuePair<string, byte[]> keyValuePair,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else if (key.IndexOf('*', StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("key does not support wildcard", nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.HMSetAsync(key, keyValuePair);
    }

    public Task<long> HDelAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else if (key.IndexOf('*', StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("key does not support wildcard", nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        return RedisHelper<RedisHelper>.HDelAsync(key, field);
    }

    public Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else if (key.IndexOf('*', StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("key does not support wildcard", nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (fields.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(fields));
        }

        return RedisHelper<RedisHelper>.HDelAsync(key, fields);
    }

    public Task<long> HIncrAsync(string key, string field, long value = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else if (key.IndexOf('*', StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("key does not support wildcard", nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        if (string.IsNullOrWhiteSpace(field))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(field));
        }

        return RedisHelper<RedisHelper>.HIncrByAsync(key, field, value);
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expire, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.ExpireAsync(key, expire);
    }

    public Task<bool> ExpireAsync(string key, int seconds, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.ExpireAsync(key, seconds);
    }

    public async Task SubscribeAsync(string topic, Action<string, byte[]> callback,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(topic))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(topic));
        }

        topic = GetKeys(topic)[0];

        var result = await Task.FromResult(RedisHelper<RedisHelper>.Subscribe((topic,
            delegate(CSRedisClient.SubscribeMessageEventArgs args)
            {
                callback(args.Channel, Convert.FromBase64String(args.Body));
            })));

        Subscribes.AddOrUpdate(topic, result, delegate(string key, CSRedisClient.SubscribeObject value)
        {
            if (value != result)
            {
                _logger.LogWarning("Duplicate topic are not allowed: " + topic);
            }

            return value;
        });
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(topic))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(topic));
        }

        topic = GetKeys(topic)[0];
        Subscribes.TryRemove(topic, out var result);
        if (result is { IsUnsubscribed: false })
        {
            await Task.Run(delegate { result.Unsubscribe(); }, cancellationToken);
        }
    }

    public Task<long> PublishAsync(string topic, byte[] message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(topic))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(topic));
        }

        if (message.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(message));
        }

        topic = GetKeys(topic)[0];

        return RedisHelper<RedisHelper>.PublishAsync(topic,
            Convert.ToBase64String(message, Base64FormattingOptions.None));
    }

    public Task<string[]> KeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(pattern))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(pattern));
        }

        return Task.FromResult(GetKeys(pattern));
    }

    public async Task<string[]> ScanAsync(long cursor, string pattern, int? count,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(pattern))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(pattern));
        }
        else
        {
            pattern = GetKeys(pattern)[0];
        }

        return (await RedisHelper<RedisHelper>.ScanAsync(cursor, pattern, count)).Items;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }

        key = GetKeys(key)[0];

        return RedisHelper<RedisHelper>.ExistsAsync(key);
    }

    public Task<long> ExistsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumerable = keys.ToArray();
        if (enumerable.Length == 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(keys));
        }

        enumerable = GetKeys(enumerable);
        return RedisHelper<RedisHelper>.ExistsAsync(enumerable);
    }

    public Task<long> LLenAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(key))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(key));
        }
        else
        {
            key = GetKeys(key)[0];
        }

        return RedisHelper<RedisHelper>.LLenAsync(key);
    }

    private string[] GetKeys(params string[]? keys)
    {
        if (keys == null || keys.Length == 0)
        {
            ArgumentNullException.ThrowIfNull(keys);
        }

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