namespace EC_locator.Parsers;

public interface ITimeTagger
{
    SortedList<int, TimeOnly> GetTags(string message);
}