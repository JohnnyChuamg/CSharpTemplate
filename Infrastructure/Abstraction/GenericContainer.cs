namespace Infrastructure.Abstraction;

public class GenericContainer<T> : IGenericContainer<T>
{
    public T? Content { get; set; }
}