namespace EC_locator.Core.Utilities;

public class DateTimeProvider
{
    private readonly DateTime? _dateTime;

    public DateTimeProvider()
    {
    }

    public DateTimeProvider(DateTime fixedDateTime)
        => _dateTime = fixedDateTime;

    public DateTime Now => _dateTime ?? DateTime.Now;
}