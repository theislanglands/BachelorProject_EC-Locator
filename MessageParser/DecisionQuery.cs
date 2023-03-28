using EC_locator.Core.Models;

namespace Parser;

public class DecisionQuery : Node
{
    public string Title { get; set; }
    public Node Positive { get; set; }
    public Node Negative { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> Test { get; set; }
    
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        bool result = this.Test(locations, times);
        string resultAsString = result ? "yes" : "no";

        if (Verbose)
        {
            Console.WriteLine($"\t- Decision: {this.Title}? {resultAsString}");
        }

        if (result) this.Positive.Perform(locations, times);
        else this.Negative.Perform(locations, times);
    }
}