namespace EClocator.Core.Models;

public class Location
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Place { get; set; }

    public Location(DateTime start, DateTime end, string place)
    {
        Start = start;
        End = end;
        Place = place ?? throw new ArgumentNullException(nameof(place));
    }

    public Location()
    {
    }
}
