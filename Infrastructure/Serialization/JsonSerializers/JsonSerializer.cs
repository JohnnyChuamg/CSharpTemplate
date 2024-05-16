using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Serialization.JsonSerializers;

public class JsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOption = new();
    private readonly SerializerOption _option = new();
    private readonly ILogger<JsonSerializer> _logger;

    public JsonSerializer(ILogger<JsonSerializer> logger)
    {
        _logger = logger;
        SetOption(_option);
    }

    public void SetOption(SerializerOption option)
    {
        ArgumentNullException.ThrowIfNull(option);
        SetOption(delegate(SerializerOption opt)
        {
            opt.AllowTrailingCommas = option.AllowTrailingCommas;
            opt.DictionaryKeyPolicy = option.DictionaryKeyPolicy;
            opt.IgnoreNullValues = option.IgnoreNullValues;
            opt.IgnoreReadOnlyProperties = option.IgnoreReadOnlyProperties;
            opt.PropertyNameCaseInsensitive = option.PropertyNameCaseInsensitive;
            opt.PropertyNamingPolicy = option.PropertyNamingPolicy;
            opt.WriteIndented = option.WriteIndented;
        });
    }

    public void SetOption(Action<SerializerOption> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        predicate(_option);
        _jsonSerializerOption.AllowTrailingCommas = _option.AllowTrailingCommas;
        var jsonSerialOpt = _jsonSerializerOption;
        if (_option.DictionaryKeyPolicy != 0)
        {
            throw new NotSupportedException();
        }

        var jsonNamingPolicy = jsonSerialOpt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        _jsonSerializerOption.DefaultIgnoreCondition = _option.IgnoreNullValues
            ? JsonIgnoreCondition.WhenWritingNull
            : JsonIgnoreCondition.Never;
        _jsonSerializerOption.IgnoreReadOnlyProperties = _option.IgnoreReadOnlyProperties;
        _jsonSerializerOption.PropertyNameCaseInsensitive = _option.PropertyNameCaseInsensitive;
        jsonNamingPolicy = jsonSerialOpt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        if (_option.PropertyNamingPolicy != 0)
        {
            throw new NotSupportedException();
        }

        _jsonSerializerOption.WriteIndented = _option.WriteIndented;

    }

    public object? Deserialize(string data)
        => System.Text.Json.JsonSerializer.Deserialize<object>(data, _jsonSerializerOption);

    public object? Deserialize(ReadOnlySpan<byte> data)
        => System.Text.Json.JsonSerializer.Deserialize<object>(data, _jsonSerializerOption);

    public T? Deserialize<T>(string data)
        => System.Text.Json.JsonSerializer.Deserialize<T>(data, _jsonSerializerOption);

    public T? Deserialize<T>(ReadOnlySpan<byte> data)
        => System.Text.Json.JsonSerializer.Deserialize<T>(data, _jsonSerializerOption);

    public async Task<object?> DeserializeAsync(string data, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return await System.Text.Json.JsonSerializer.DeserializeAsync<object?>(memoryStream, _jsonSerializerOption,
            cancellationToken);
    }


    public async Task<object?> DeserializeAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream(data);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return await System.Text.Json.JsonSerializer.DeserializeAsync<object?>(memoryStream, _jsonSerializerOption,
            cancellationToken);
    }

    public async Task<T?> DeserializeAsync<T>(string data, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(memoryStream, _jsonSerializerOption,
            cancellationToken);
    }

    public async Task<T?> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream(data);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(memoryStream, _jsonSerializerOption,
            cancellationToken);
    }

    public bool TryDeserialize(string data, out object? result)
    {
        try
        {
            result = Deserialize(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);
        }

        result = null;
        return false;
    }

    public bool TryDeserialize(ReadOnlySpan<byte> data, out object? result)
    {
        try
        {
            result = Deserialize(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);
        }

        result = null;
        return false;
    }

    public bool TryDeserialize<T>(string data, out T? result)
    {
        try
        {
            result = Deserialize<T>(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);
        }

        result = default;
        return false;
    }

    public bool TryDeserialize<T>(ReadOnlySpan<byte> data, out T? result)
    {
        try
        {
            result = Deserialize<T>(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);
        }

        result = default;
        return false;
    }

    public string Serialize<T>(T data)
        => System.Text.Json.JsonSerializer.Serialize(data, _jsonSerializerOption);

    public byte[] SerializeToBytes<T>(T data)
        => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data, _jsonSerializerOption);

    public async Task<string> SerializeAsync<T>(T data, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, data, _jsonSerializerOption, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(stream).ReadToEndAsync(cancellationToken);
    }

    public async Task<byte[]> SerializeToBytesAsync<T>(T data, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();
        await System.Text.Json.JsonSerializer.SerializeAsync(stream, data, _jsonSerializerOption, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }

    public bool TrySerialize<T>(T data, out string? result)
    {
        try
        {
            result = Serialize(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);
        }

        result = null;
        return false;
    }

    public bool TrySerializeToBytes<T>(T data, out byte[]? result)
    {
        try
        {
            result = SerializeToBytes(data);
            return false;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, e.Message);
        }

        result = null;
        return false;
    }
}