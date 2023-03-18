using EC_locator.Core.Models;

namespace Parser;

public class Action : Node

{
    public string Title { get; set; }
    public Node GoTo { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> PerformAction { get; set; }
    public override void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        Console.WriteLine($"\t- Action: {this.Title}");
        this.PerformAction(_locations, _times);
        GoTo.Perform(_locations, _times);
    }
}