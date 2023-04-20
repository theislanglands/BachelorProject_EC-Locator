using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Extensions;

namespace EC_locator.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly bool _verbose;
    private readonly IGraphHelper _graphHelper;
    
    public CalendarRepository(IGraphHelper graphHelper, IOptions<VerboseOptions> settingsOptions)
    {
        _graphHelper = graphHelper;
        _verbose = settingsOptions.Value.Verbose;
    }

    public async Task<List<CalendarEvent>?> GetCurrentCalendarEventsAsync(string employeeId)
    {
        if (_verbose)
        {
            Console.WriteLine("fetching calendar event");
        }

        IList<Event>? calendarEvents = null;
        try
        {
            calendarEvents = _graphHelper.getCalendarEventsAsync(employeeId).Result.CurrentPage;
        }
        catch (System.AggregateException ex)
        {
            Console.WriteLine("ex: user not found");
            return null;
        }

        if (calendarEvents == null)
        {
            Console.WriteLine("no calendar events");
            return null;
        }

        List<CalendarEvent>? foundCalendarEvents = new();
        
        for (int i = 0; i< calendarEvents.Count; i++)
        {
            string subject = calendarEvents[i].Subject;
            
            DateTime startTime, endTime;
            if (calendarEvents[i].IsAllDay.GetValueOrDefault())
            {
                startTime = calendarEvents[i].Start.ToDateTime();
                endTime = calendarEvents[i].End.ToDateTime();
            }
            else
            {
                startTime = calendarEvents[i].Start.ToDateTime().ToLocalTime();
                endTime = calendarEvents[i].End.ToDateTime().ToLocalTime();
            }
            
            if (_verbose)
            {
                Console.WriteLine($"Found event: {subject} ({startTime} - {endTime})");
            }
            
            foundCalendarEvents.Add(new CalendarEvent(subject, startTime, endTime));
        }
        

        if (foundCalendarEvents.Count != 0)
        {
            return foundCalendarEvents;
        }

        return null;
        
    }
}