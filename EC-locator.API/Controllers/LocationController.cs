using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using Location = EC_locator.Core.Models;
using Message = EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;


namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
[EnableCors]
public class LocationController
{
    private readonly bool _verbose;
    private readonly IEmployeeLocator _employeeLocator;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public LocationController(IEmployeeLocator employeeLocator, IOptions<VerboseOptions> settingsOptions)
    {
        _verbose = settingsOptions.Value.Verbose;
        _employeeLocator = employeeLocator;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    // TODO check async and await if works!
    [HttpGet("{employeeId}")]
    public async Task<string> GetCurrentLocationAsync(string employeeId)
    {
        Location.Location? currentLocation = _employeeLocator.GetCurrentLocation(employeeId);
        Location.Message? latestMessage = _employeeLocator.GetLatestMessage(employeeId);
        List<CalendarEvent>? calendarEvents = _employeeLocator.GetCurrentCalendarEvents(employeeId);
        
        LocationReturn lr = new LocationReturn();
        
        if (_verbose)
        {
            Console.WriteLine("\n- LOCATION CONTROLLER -");
            Console.WriteLine($"EmployeeID: {employeeId}");
            Console.WriteLine($"Latest Message: {latestMessage}");
            Console.WriteLine($"Current Location: {currentLocation}");
            if (calendarEvents != null)
                foreach (var calendarEvent in calendarEvents)
                {
                    Console.WriteLine($"Calendar Event: {calendarEvent}");
                }
        }

        if (latestMessage == null)
        {
            lr.TeamMessage = "no messages found";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(latestMessage.Content);
            lr.TeamMessage = latestMessage.Content;
            if (latestMessage.Replies != null)
            {
                foreach (var reply in latestMessage.Replies)
                {
                    sb.AppendLine($" -{reply.ToString()}");
                }
            }
            lr.TeamMessage = sb.ToString();
        }
        
        if (currentLocation == null)
        {
            lr.Place = "no location found";
            lr.TeamMessage = "no locations identified";
        }
        
        // TODO: use default if nothing found doen't come through - neither do OFF!
        
        else
        {
            lr.Place = currentLocation.Place;

            if (currentLocation.Start != null)
            {
                lr.LocationStartTime = currentLocation.Start.Value.ToString();
            }

            if (currentLocation.End != null)
            {
                lr.LocationEndTime = currentLocation.End.Value.ToString();
            }

        }

        if (calendarEvents != null)
        {
            lr.CurrentCalendarEvents = calendarEvents;
            StringBuilder calendarInfo = new StringBuilder();
            foreach (var calendarEvent in calendarEvents)
            {
                calendarInfo.Append(calendarEvent);
                if (!calendarEvent.Equals(calendarEvents.Last()))
                {
                    calendarInfo.Append(", ");
                }
            }

            lr.CalenderInfo = calendarInfo.ToString();
        }
        else
        {
            lr.CalenderInfo = "no calendar events found";
        }


        return JsonSerializer.Serialize(lr, _jsonOptions);
    }

    class LocationReturn
    {
        public string? Place { get; set; }
        public string? LocationStartTime { get; set; }

        public string? LocationEndTime { get; set; }

        public string? TeamMessage { get; set; }
        public string? CalenderInfo { get; set; }
        public List<CalendarEvent>? CurrentCalendarEvents { get; set; }
    }
}