namespace Infrastructure.Contracts.ResultContracts;

public class QueryPage
{
    private int _offset;
    private int _limit = 30;

    public int Offset
    {
        get => _offset;
        set => _offset = value >= 0 ? value : 0;
    }

    public int Limit
    {
        get => _limit;
        set => _limit = value < 1 ? 30 : value;
    }
}