using EC_locator.Core;
using Location = EC_locator.Core.Models.Location;

namespace EC_locator.Parsers;

public abstract class Node

{
    protected bool Verbose = Settings.GetInstance().Verbose;
    public abstract void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times);
}