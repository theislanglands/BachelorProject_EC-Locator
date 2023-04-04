namespace Parser;

public interface ITimeTagger
{
    SortedList<int, TimeOnly> GetTags(string message);
}