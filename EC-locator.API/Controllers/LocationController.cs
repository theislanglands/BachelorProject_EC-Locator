using System.Text.Json;
using System.Text.Json.Serialization;
using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Parser;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
[EnableCors]
public class LocationController
{
    // TODO: use ITeamsrepository interface instead 
    private TeamsRepository _teamsRepository = new TeamsRepository();
    private MessageParser _messageParser = new MessageParser();
    Settings _settings = Settings.GetInstance();

    [HttpGet("{employeeId}")]
    public async Task<string> GetCurrentLocationAsync(string employeeId)
    {
        string latestMessage;
        Location foundLocation;
        employeeId = "sample2";

        // fetch messages from today
        string[] messages = _teamsRepository.GetMessages(employeeId, new DateOnly());
        
        // only Look at latest message???
        // Select random message (For testing)
        latestMessage = SelectRandomMessage(messages);

        // parse message to locations
        var locations = _messageParser.GetLocations(latestMessage);
        
        // find current time
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        // currentTime = new TimeOnly(9, 59);

        if (_settings.Verbose)
        {
            Console.WriteLine("\n- LOCATION CONTROLLER -");
            Console.WriteLine($"EmployeeID: {employeeId}");
            Console.WriteLine($"message {latestMessage}");
            Console.WriteLine($"Current Time {currentTime}\n");
        }
        
        foundLocation = FindLocation(locations, currentTime);
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        LocationReturn locationReturn = new LocationReturn
        {
            Place = foundLocation.Place,
            LocationEndTime = foundLocation.End.Value.ToString(),
            TeamMessage = latestMessage
        };

        return JsonSerializer.Serialize(locationReturn, options);
    }

    private Location FindLocation(List<Location> locations, TimeOnly time)
    {
        Location foundLocation = new Location();
        // find the location matching time
        foreach (var location in locations)
        {
            if (_settings.Verbose)
            {
                Console.WriteLine(location);
                Console.WriteLine($"location.Start <= time: {location.Start < time} ");
                Console.WriteLine($"time < location.End: {time < location.End} ");
            }

            // see if current time is after startTime and beforeEndTime
            if (location.Start <= time && time < location.End)
            {
                foundLocation = location;
                if (_settings.Verbose)
                {
                    Console.WriteLine($"\nfound location {foundLocation}");
                }

                break;
            }
        }

        if (foundLocation.Start == null)
        {
            // no loctions found
            foundLocation = new Location("unable to find location in Location Controller");
            if (_settings.Verbose)
            {
                Console.WriteLine($"\nerror: {foundLocation}");
            }
        }

        return foundLocation;
    }

    private string SelectRandomMessage(string[] messages)
    {
        int randomIndex = new Random().Next(0, messages.Length);
        return messages[randomIndex];
    }

    class LocationReturn
    {
        public string Place { get; set; }
        public string LocationEndTime { get; set; }
        public string TeamMessage { get; set; }
        public string CalenderInfo { get; set; }
    }
}