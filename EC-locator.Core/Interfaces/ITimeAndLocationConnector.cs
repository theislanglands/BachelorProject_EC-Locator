using EC_locator.Core.Models;

namespace EC_locator.Parsers;

public interface ITimeAndLocationConnector
{
    List<Location> AddTimeToLocations(SortedList<int, Location> locationTags, SortedList<int, TimeOnly> timeTags, string message);
}