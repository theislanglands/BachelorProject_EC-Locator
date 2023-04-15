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
    private readonly IEmployeeLocator _employeeLocator;

    private readonly JsonSerializerOptions _jsonOptions;
    
    public LocationController(IMessageParser messageParser, ITeamsRepository teamsRepository, IOptions<VerboseOptions> settingsOptions)
    {
        _messageParser = messageParser;
        _teamsRepository = teamsRepository;
        _verbose = settingsOptions.Value.Verbose;
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    // TODO check async and awit if works!
    [HttpGet("{employeeId}")]
    public async Task<string> GetCurrentLocationAsync(string employeeId)
    {
        Location? currentLocation = _employeeLocator.GetCurrentLocation(employeeId);
        Message? latestMessage = _employeeLocator.GetLatestMessage(employeeId);
        LocationReturn lr = new LocationReturn();
        
        if (_verbose)
        {
            Console.WriteLine("\n- LOCATION CONTROLLER -");
            Console.WriteLine($"EmployeeID: {employeeId}");
            Console.WriteLine($"Latest Message {latestMessage}");
            Console.WriteLine($"Current Location {currentLocation}");
        }
        
        if (latestMessage == null)
        {
            lr.Place = "no location found";
            lr.TeamMessage = "no messages found";
        } 
        else if (currentLocation == null)
        {
            lr.Place = "no location found";
            lr.TeamMessage = "no locations identified";
        }
        else
        {
            lr.Place = currentLocation.Place;
            lr.LocationStartTime = currentLocation.Start.Value.ToString();
            lr.LocationEndTime = currentLocation.End.Value.ToString();
            lr.TeamMessage = latestMessage.Content;
        }

        return JsonSerializer.Serialize(lr, _jsonOptions);
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
        public string LocationStartTime { get; set; }

        public string TeamMessage { get; set; }
        public string CalenderInfo { get; set; }
    }
}