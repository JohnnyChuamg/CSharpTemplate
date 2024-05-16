namespace Infrastructure.Serialization.JsonSerializers;

public interface IJsonSerializer : ISerializer
{
    void SetOption(SerializerOption option);
    void SetOption(Action<SerializerOption> predicate);
}