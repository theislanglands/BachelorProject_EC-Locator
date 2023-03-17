using EC_locator.Core.Models;

namespace Parser;

public abstract class Decision


{
    public abstract void Evaluate(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times);
}