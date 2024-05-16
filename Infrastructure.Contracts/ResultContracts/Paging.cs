namespace Infrastructure.Contracts.ResultContracts;

public class Paging
{
    private const int DEFAULT_LIMIT = 30;
    private int _offset;
    private int _limit = DEFAULT_LIMIT;

    public Paging()
    {
    }

    public Paging(int offset, int limit)
    {
        _offset = offset;
        _limit = limit;
    }

    public Paging(int offset, int limit, long count)
    {
        _offset = offset;
        _limit = limit;
        Count = count;
    }

    public long Count { get; set; }

    public int Offset
    {
        get => _offset;
        set => _offset = value >= 0 ? value : 0;
    }

    public int Limit
    {
        get => _limit;
        set => _limit = value < 1 ? DEFAULT_LIMIT : value;
    }

    public int Page
    {
        get
        {
            if (Count == 0L || Limit == 0) return 0;
            return (int)Math.Ceiling(Count / (double)Limit);
        }
    }
}