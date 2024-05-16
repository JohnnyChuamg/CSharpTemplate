namespace Infrastructure.Contracts.ResultContracts;

public interface IResult
{
    public ResultCode Code { get; set; }

    public string? Message { get; set; }
}

public interface IResult<T> : IResult
{
    public T Content { get; set; }
    
    public Paging? Paging { get; set; }
}