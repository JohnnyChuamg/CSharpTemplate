using Infrastructure.Diagnostics.Health;

namespace Infrastructure.Caching.Redis;

public interface IRedis : IHealthPing
{
    string Instance { get; }

    Task<bool> SetAsync(string key, byte[] value, CacheOption? options = null,
        CancellationToken cancellationToken = default);

    Task<bool> SetNxAsync(string key, byte[] value, CacheOption? option = null,
        CancellationToken cancellationToken = default);

    Task<bool> MSetAsync(KeyValuePair<string, byte[]>[] keyValuePairs, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<byte[]>> GetAsync(string[] keys, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> GetAsync(string key, CancellationToken cancellationToken = default);

    Task<long> DelAsync(string key, CancellationToken cancellationToken = default);

    Task<long> IncrByAsync(string key, long value, CancellationToken cancellationToken = default);

    Task<long> RPushAsync(string key, byte[] value, CancellationToken cancellationToken = default);

    Task<byte[]> RPopAsync(string key, CancellationToken cancellationToken = default);

    Task<long> LPushAsync(string key, byte[] value, CancellationToken cancellationToken = default);

    Task<byte[]> LPopAsync(string key, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> LRangeAsync(string key, long start, long stop,
        CancellationToken cancellationToken = default);

    Task<byte[]> LIndexAsync(string key, long index, CancellationToken cancellationToken = default);

    Task<bool> LTrimAsync(string key, long start, long stop, CancellationToken cancellationToken = default);

    Task<long> LRemAsync(string key, byte[] value, long count, CancellationToken cancellationToken = default);

    Task<byte[]> RPopLPushAsync(string source, string destination, CancellationToken cancellationToken = default);

    Task<long> SAddAsync(string key, byte[] member, CancellationToken cancellationToken = default);

    Task<long> SAddAsync(string key, byte[][] members, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> SMembersAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> SIsMemberAsync(string key, byte[] member, CancellationToken cancellationToken = default);

    Task<long> SRemAsync(string key, byte[] member, CancellationToken cancellationToken = default);

    Task<long> SRemAsync(string key, byte[][] members, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> SDiffAsync(string[] keys, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> SInterAsync(string[] keys, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> SUnionAsync(string[] keys, CancellationToken cancellationToken = default);

    Task<bool> HSetAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default);

    Task<byte[]> HGetAsync(string key, string field, CancellationToken cancellationToken = default);

    Task<Dictionary<string, byte[]>> HGetAllAsync(string key, CancellationToken cancellationToken = default);

    Task<string[]> HKeysAsync(string key, CancellationToken cancellationToken = default);

    Task<long> HIncrByAsync(string key, string field, long value = 1L, CancellationToken cancellationToken = default);

    Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default);

    Task<bool> HSetNxAsync(string key, string field, byte[] value, CancellationToken cancellationToken = default);

    Task<IEnumerable<byte[]>> HMGetAsync(string key, string[] fields, CancellationToken cancellationToken = default);

    Task<bool> HMSetAsync(string key, KeyValuePair<string, byte[]> keyValuePair,
        CancellationToken cancellationToken = default);

    Task<long> HDelAsync(string key, string field, CancellationToken cancellationToken = default);

    Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default);

    Task<long> HIncrAsync(string key, string field, long value = 1L, CancellationToken cancellationToken = default);

    Task<bool> ExpireAsync(string key, TimeSpan expire, CancellationToken cancellationToken = default);

    Task<bool> ExpireAsync(string key, int seconds, CancellationToken cancellationToken = default);

    Task SubscribeAsync(string topic, Action<string, byte[]> callback, CancellationToken cancellationToken = default);

    Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);

    Task<long> PublishAsync(string topic, byte[] message, CancellationToken cancellationToken = default);

    Task<string[]> KeysAsync(string pattern, CancellationToken cancellationToken = default);

    Task<string[]> ScanAsync(long cursor, string pattern, int? count, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task<long> ExistsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    Task<long> LLenAsync(string key, CancellationToken cancellationToken = default);
}