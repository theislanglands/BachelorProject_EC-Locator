using EC_locator.Core.Models;

namespace Parser;

public class DecisionQuery : Node
{
    public string Title { get; set; }
    public Node Positive { get; set; }
    public Node Negative { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> Test { get; set; }
    
    public override void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        bool result = this.Test(_locations, _times);
        string resultAsString = result ? "yes" : "no";

        Console.WriteLine($"\t- Decision: {this.Title}? {resultAsString}");

        if (result) this.Positive.Perform(_locations, _times);
        else this.Negative.Perform(_locations, _times);
    }
}