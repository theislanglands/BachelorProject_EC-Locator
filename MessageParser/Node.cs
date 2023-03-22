using EC_locator.Core;
using Location = EC_locator.Core.Models.Location;

namespace Parser;

public abstract class Node


{
    public bool Verbose = Settings.GetInstance().Verbose;
    public abstract void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times);
}