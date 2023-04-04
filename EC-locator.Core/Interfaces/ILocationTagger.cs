using EC_locator.Core.Models;

namespace EC_locator.Core.Interfaces;

public interface ILocationTagger
{
    SortedList<int, Location> GetTags(string message);
}