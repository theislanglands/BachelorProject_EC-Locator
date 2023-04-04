using EC_locator.Core.Models;

namespace EC_locator.Parsers;

public class Action : Node

{
    public string Title { get; set; }
    public Node GoTo { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> PerformAction { get; set; }
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        if (Verbose)
        {
            Console.WriteLine($"\t- Action: {this.Title}");
        }

        this.PerformAction(locations, times);
        GoTo.Perform(locations, times);
    }
}