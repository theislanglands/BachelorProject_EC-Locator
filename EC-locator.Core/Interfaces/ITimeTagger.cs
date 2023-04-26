using EC_locator.Core.Models;

namespace EC_locator.Parsers;

public interface ITimeTagger
{
    SortedList<int, TimeOnly> GetTags(Message message);
}