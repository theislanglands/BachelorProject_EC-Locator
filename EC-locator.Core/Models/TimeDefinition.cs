namespace EClocator.Core.Models;

public class TimeDefinition
{
    public string timeExpression;
    public TimeOnly? Start { get; set; }
    public TimeOnly? End { get; set; }

    public TimeDefinition(string timeExpression, TimeOnly start = default, TimeOnly end = default)
    {
        this.timeExpression = timeExpression;
        Start = start;
        End = end;
    }
    
    public TimeDefinition(string timeExpression)
    {
        this.timeExpression = timeExpression;
    }
}