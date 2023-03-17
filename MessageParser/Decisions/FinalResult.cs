using EC_locator.Core.Models;

namespace Parser.Decisions;

public class FinalResult : Decision
{
    public override void Evaluate(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        Console.WriteLine("slut prut");
    }
}