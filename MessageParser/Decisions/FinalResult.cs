using EC_locator.Core.Models;

namespace Parser.Decisions;

public class FinalResult : Node
{
    public override void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        Console.WriteLine("slut prut");
        foreach (var location in _locations)
        {
            Console.WriteLine(location);
        }
    }
}