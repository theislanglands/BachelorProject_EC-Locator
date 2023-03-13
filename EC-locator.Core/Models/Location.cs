using System.Text;

namespace EC_locator.Core.Models;

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
    
    public Location(string place)
    {
        Place = place ?? throw new ArgumentNullException(nameof(place));
    }

    public override string ToString()
    {
        StringBuilder presentation = new StringBuilder();
        presentation.Append($"Location: {Place}, ");
        presentation.Append($"Start time: {Start}, End time: {End} ");
        return presentation.ToString();
    }
}
