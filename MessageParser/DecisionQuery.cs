using EC_locator.Core.Models;

namespace Parser;

public class DecisionQuery : Decision
{
    public string Title { get; set; }
    public Decision Positive { get; set; }
    public Decision Negative { get; set; }
    // public Func<Client, bool> Test { get; set; }


    public override void Evaluate(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        throw new NotImplementedException();
    }
}