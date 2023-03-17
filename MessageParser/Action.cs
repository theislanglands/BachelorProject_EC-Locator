using EC_locator.Core.Models;

namespace Parser;

public abstract class Action

{
    public abstract void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times);
}