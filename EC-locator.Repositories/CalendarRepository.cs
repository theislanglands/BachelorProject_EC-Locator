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

    public async Task<string> GetCurrentCalendarEvent(string employeeId)
    {
        // anders
        // employeeId = "2cf3e351-6ca8-4fda-999c-14a8b048b899";
        // brian
        // employeeId = "2d3cfcdf-542d-43f5-a4b1-6f58387604eb";
        // theis
        employeeId = "6e5ee9cb-11cb-405d-aaa8-60c3768340c3";
        if (_verbose)
        {
            Console.WriteLine("fetching calendar event");
        }
        
        var calendarEvents = _graphHelper.getCalendarEventsAsync(employeeId).Result.CurrentPage;

        List<CalendarEvent> foundCalendarEvents = new();
        
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
                Console.WriteLine($"Found event:{subject} ({startTime} - {endTime})");
            }
            foundCalendarEvents.Add(new CalendarEvent(subject, startTime, endTime));
        }

        foreach (var ce in foundCalendarEvents)
        {
            Console.WriteLine($"now from DTO {ce.Subject}");
        }
        /*
        foreach (var ev in events.CurrentPage)
        {
            Console.WriteLine("");
            Console.WriteLine($"{ev.Subject} ({ev.Start.DateTime} - {ev.End.DateTime})");
        }
        */
        Environment.Exit(1);
        return null;
    }
}