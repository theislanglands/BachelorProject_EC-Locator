using EC_locator.Core.Models;

namespace Parser;

public interface ITimeAndLocationConnector
{
    List<Location> AddTimeToLocations(SortedList<int, Location> locationTags, SortedList<int, TimeOnly> timeTags, string message);
}