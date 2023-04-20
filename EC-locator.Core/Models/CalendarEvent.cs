namespace EC_locator.Core.Models;

public class CalendarEvent
{
    public string Subject { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAllDay { get; set; }


    public CalendarEvent(string subject, DateTime startTime, DateTime endTime, bool isAllDay)
    {
        Subject = subject;
        StartTime = startTime;
        EndTime = endTime;
        IsAllDay = isAllDay;

    }

    public override string ToString()
    {
        if (!IsAllDay)
        {
            return $"{Subject} ({StartTime:H:mm} - {EndTime:H:mm})";
        }
        return $"{Subject} (All Day)";
    }
}