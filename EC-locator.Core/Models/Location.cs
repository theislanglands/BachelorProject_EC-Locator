using System.Text;

namespace EClocator.Core.Models;

public class Location
{
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public string Place { get; set; }

    public Location(TimeOnly start, TimeOnly end, string place)
    {
        Start = start;
        End = end;
        Place = place ?? throw new ArgumentNullException(nameof(place));
    }

    public Location()
    {
    }

    public override string ToString()
    {
        StringBuilder presentation = new StringBuilder();
        presentation.AppendLine($"Location {Place}");
        presentation.AppendLine($"Start time: {Start}, End time {End} ");
        return presentation.ToString();
    }
}
