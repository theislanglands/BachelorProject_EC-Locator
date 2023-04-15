using System.Text.Json;
using System.Text.Json.Serialization;
using EC_locator.Core;
using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using EC_locator.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using EC_locator.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
[EnableCors]
public class LocationController
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMessageParser _messageParser;
    private readonly bool _verbose;
    
    public LocationController(IMessageParser messageParser, ITeamsRepository teamsRepository, IOptions<VerboseOptions> settingsOptions)
    {
        _messageParser = messageParser;
        _teamsRepository = teamsRepository;
        _verbose = settingsOptions.Value.Verbose;
    }
    
    [HttpGet("{employeeId}")]
    public async Task<string> GetCurrentLocationAsync(string employeeId)
    {
        string latestMessage;
        Location? foundLocation;
        // employeeId = "all";
        
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // var messages = _teamsRepository.GetMessages(employeeId, DateOnly.FromDateTime(DateTime.Now));
        
        // fetch messages from today
        var messages = await _teamsRepository.GetMessagesAsync(employeeId);
        
        if (messages == null)
        {
            LocationReturn locationReturn = new LocationReturn
            {
                Place = "no location found",
                LocationEndTime = "NA",
                TeamMessage = "no messages found"
            };
            return JsonSerializer.Serialize(locationReturn, jsonOptions);
        }
        
        // only Look at latest message
        messages.Sort();
        latestMessage = messages.First().Content;
        
        
        // latestMessage = SelectRandomMessage(messages); // (FOR TESTING)

        // parse message to locations
        var locations = _messageParser.GetLocations(latestMessage);
        
        // find current time
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        currentTime = new TimeOnly(11, 27); // FOR TESTING

        if (_verbose)
        {
            Console.WriteLine("\n- LOCATION CONTROLLER -");
            Console.WriteLine($"EmployeeID: {employeeId}");
            Console.WriteLine($"message {latestMessage}");
            Console.WriteLine($"Current Time {currentTime}\n");
        }
        
        foundLocation = FindLocation(locations, currentTime);
        
        if (foundLocation != null)
        {
            LocationReturn locationReturn = new LocationReturn
            {
                Place = foundLocation.Place,
                LocationEndTime = foundLocation.End.Value.ToString(),
                TeamMessage = latestMessage
                
            };
            return JsonSerializer.Serialize(locationReturn, jsonOptions);

        }
        else
        {
            LocationReturn locationReturn = new LocationReturn
            {
                Place = "no location found",
                LocationEndTime = "NA",
                TeamMessage = "no location matching current time"

            };
            return JsonSerializer.Serialize(locationReturn, jsonOptions);
        }
    }

    private Location FindLocation(List<Location> locations, TimeOnly time)
    {
        Location foundLocation = null;
        // find the location matching time
        foreach (var location in locations)
        {
            if (_verbose)
            {
                Console.WriteLine(location);
                Console.WriteLine($"location start-time <= currenttime: {location.Start < time} ");
                Console.WriteLine($"current time < location-endtime: {time < location.End} ");
            }

            // see if current time is after startTime and beforeEndTime
            if (location.Start <= time && time < location.End)
            {
                foundLocation = location;
                if (_verbose)
                {
                    Console.WriteLine($"\nfound location {foundLocation}");
                }

                break;
            }
        }

        
        if (foundLocation == null)
        {
            // no loctions found
            if (_verbose)
            {
                Console.WriteLine($"\nno location matching current time");
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