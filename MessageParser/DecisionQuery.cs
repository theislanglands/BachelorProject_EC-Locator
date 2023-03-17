using EC_locator.Core.Models;

namespace Parser;

public class DecisionQuery : Decision
{
    public string Title { get; set; }
    public Decision Positive { get; set; }
    public Decision Negative { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> Test { get; set; }
    
    public override void Evaluate(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        bool result = this.Test(_locations, _times);
        string resultAsString = result ? "yes" : "no";

        Console.WriteLine($"\t- {this.Title}? {resultAsString}");

        if (result) this.Positive.Evaluate(_locations, _times);
        else this.Negative.Evaluate(_locations, _times);
    }
}