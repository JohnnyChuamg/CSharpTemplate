namespace Infrastructure.Commons;

public class Snowflake
{
    private static long _machineId;
    private static long _datacenterId;
    private static long _sequence;
    private const long TWEPOCH = 687888001020L;
    private const long MAX_MACHINE_ID = 31L;
    private const long MAX_DATACENTER_ID = 31L;
    private const int MACHINE_ID_SHIFT = 12;
    private const int DATACENTER_ID_SHIFT = 17;
    private const int TIMESTAMP_LEFT_SHIFT = 22;
    private static long _lastTimestamp = -1L;
    private static readonly object SyncRoot = new();
    private static Snowflake? _snowflakes;
    public static Snowflake Instance => _snowflakes ??= new Snowflake();

    public Snowflake(long machineId = 0L, long datacenterId = 0L)
    {
        if (machineId is > MAX_MACHINE_ID or < 0)
        {
            throw new ArgumentException(nameof(machineId) + " Maximum value should be between 0 and " + MAX_MACHINE_ID);
        }

        if (datacenterId is > MAX_DATACENTER_ID or < 0)
        {
            throw new ArgumentException(nameof(datacenterId) + " Maximum value should be between 0 and " +
                                        MAX_MACHINE_ID);
        }

        _machineId = machineId;
        _datacenterId = datacenterId;
    }

    private static long GetTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static long GetNextTimestamp(long lastTimestamp)
    {
        var timestamp = GetTimestamp();
        if (timestamp <= lastTimestamp)
        {
            timestamp = GetTimestamp();
        }

        return timestamp;
    }

    public long Generate()
    {
        lock (SyncRoot)
        {
            var num = GetTimestamp();
            if (_lastTimestamp == num)
            {
                _sequence = (_sequence + 1) & 0xFFF;
                if (_sequence == 0L)
                {
                    num = GetNextTimestamp(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            if (num < _lastTimestamp)
            {
                throw new ArgumentException("timestamp");
            }

            _lastTimestamp = num;

            return (num - TWEPOCH << TIMESTAMP_LEFT_SHIFT)
                   | (_datacenterId << DATACENTER_ID_SHIFT)
                   | (_machineId << MACHINE_ID_SHIFT)
                   | _sequence;
        }
    }
    // private const int SEQUENCE_BITS = 12;
    // private const long MACHINE_ID_BITS = 5L;
    // private const long DATACENTER_ID_BITS = 5L;
    // private const long SEQUENCE_MASK = 4095L;
}