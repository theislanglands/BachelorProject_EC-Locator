using Location = EC_locator.Core.Models.Location;

namespace Parser;

public abstract class Node

{
    public abstract void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times);
}