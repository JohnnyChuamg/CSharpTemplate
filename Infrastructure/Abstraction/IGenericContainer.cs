namespace Infrastructure.Abstraction;

public interface IGenericContainer<T>
{
    T? Content { get; set; }
}
