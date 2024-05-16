namespace Infrastructure.Serialization;

public interface ISerializer
{
    object? Deserialize(string data);
    object? Deserialize(ReadOnlySpan<byte> data);
    T? Deserialize<T>(string data);
    T? Deserialize<T>(ReadOnlySpan<byte> data);
    Task<object?> DeserializeAsync(string data, CancellationToken cancellationToken = default);
    Task<object?> DeserializeAsync(byte[] data, CancellationToken cancellationToken = default);
    Task<T?> DeserializeAsync<T>(string data, CancellationToken cancellationToken = default);
    Task<T?> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default);
    bool TryDeserialize(string data, out object? result);
    bool TryDeserialize(ReadOnlySpan<byte> data, out object? result);
    bool TryDeserialize<T>(string data, out T? result);
    bool TryDeserialize<T>(ReadOnlySpan<byte> data, out T? result);
    string Serialize<T>(T data);
    byte[] SerializeToBytes<T>(T data);
    Task<string> SerializeAsync<T>(T data, CancellationToken cancellationToken = default);
    Task<byte[]> SerializeToBytesAsync<T>(T data, CancellationToken cancellationToken = default);
    bool TrySerialize<T>(T data, out string? result);
    bool TrySerializeToBytes<T>(T data, out byte[]? result);
}