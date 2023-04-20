namespace EC_locator.Core.Models;

public class CalendarEvent
{
    public string Subject { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public CalendarEvent(string subject, DateTime startTime, DateTime endTime)
    {
        Subject = subject;
        StartTime = startTime;
        EndTime = endTime;
    }

    public override string ToString()
    {
        return $"{Subject} ({StartTime} - {EndTime})";
    }
}