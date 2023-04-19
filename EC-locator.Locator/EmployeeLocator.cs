using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Locator;

public class EmployeeLocator : IEmployeeLocator
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMessageParser _messageParser;
    
    private TimeOnly _currentTime;
    private readonly bool _verbose;
    
    private readonly TimeOnly _workStartDefault, _workEndDefault;
    private readonly string _defaultLocation;

    public EmployeeLocator(IMessageParser messageParser, ITeamsRepository teamsRepository,IOptions<VerboseOptions> settingsOptions, IOptions<DefaultLocationOptions> locationOptions)
    {
        _messageParser = messageParser;
        _teamsRepository = teamsRepository;
        string[] workStart = locationOptions.Value.DefaultWorkStart.Split(':');
        string[] workEnd = locationOptions.Value.DefaultWorkEnd.Split(':');
        _workStartDefault = new TimeOnly(int.Parse(workStart[0]),int.Parse(workStart[1]));
        _workEndDefault = new TimeOnly(int.Parse(workEnd[0]), int.Parse(workEnd[1]));
        _defaultLocation = locationOptions.Value.DefaultLocation;
        _verbose = settingsOptions.Value.Verbose;
    }
    
    public Location GetCurrentLocation(string employeeId)
    {
        // FIND CURRENT TIME
        _currentTime = TimeOnly.FromDateTime(DateTime.Now);
        _currentTime = new TimeOnly(09, 27); // FOR TESTING
        
        // SEE IF OUTSIDE DEFAULT WORKING HOURS => return OFF work
        if (!IsInsideDefault(_currentTime) || IsWeekend())
        {
            if (_verbose)
            {
                Console.WriteLine(" => Outside default working hours");
            }

            return new Location("off");
        }
        
        // IF NO RELEVANT MESSAGE IN TEAMS CHANNEL => ASSUME DEFAULT LOCATION
        Message? latestMessage = GetLatestMessage(employeeId);
        if (latestMessage == null)
        {
            if (_verbose)
            {
                Console.WriteLine("no message found - using default location");
            }
            
            return new Location(_workStartDefault, _workEndDefault, _defaultLocation);
        }
        
        // PARSE MESSAGE TO LOCATIONS       
        List<Location>? locations = _messageParser.GetLocations(latestMessage.Content);

        if (locations == null)
        {
            if (_verbose)
            {
                Console.WriteLine("no locations found in message");
            }
            return new Location("undefined");
        }
        
        // MATCHING A LOCATION WITH CURRENT TIME
        Location? foundLocation = FindLocation(locations, _currentTime);

        if (foundLocation == null)
        {
            if (_verbose)
            {
                Console.WriteLine("no locations matching current time");
            }
            return new Location("undefined");
        }

        return foundLocation;
    }


    public Message? GetLatestMessage(string employeeId)
    {
        //var messages = _teamsRepository.GetMessagesAsync(employeeId).Result;
        var messages = _teamsRepository.GetMessageSamples(employeeId);
        
        if (messages != null)
        {
            // SORT MESSAGES AFTER DATE AND RETRIEVE THE MOST RECENT
            messages.Sort();
            Message latestMessage = messages.Last();
            if (_verbose)
            {
                Console.WriteLine($"Message(s) found: {latestMessage.Content}");
            }

            // HANDLING TOMORROW TAGS
            bool containsTomorrow = _messageParser.ContainsTomorrow(latestMessage.Content);
            
            if (latestMessage.TimeStamp.Date.Equals(DateTime.Now.Date) && !containsTomorrow)
            {
                if (_verbose)
                {
                    Console.WriteLine("- message is from today and doesn't contain tomorrow keyword");
                }
                return latestMessage;
            } 
            if (latestMessage.TimeStamp.Date.Equals(DateTime.Now.Date.AddDays(-1)) && containsTomorrow)
            {
                if (_verbose)
                {
                    Console.WriteLine("- message is from yesterday and contains tomorrow keyword");
                }
                return latestMessage;
            }

            if (_verbose)
            {
                Console.WriteLine($"- message not relevant");
            }
            return null;
        }

        if (_verbose)
        {
            Console.WriteLine("No messages found from today or yesterday");
        }
        return null;
    }

    // FIND LOCATION THAT MATCHES TIME!
    private Location? FindLocation(List<Location> locations, TimeOnly time)
    {
        Location foundLocation = null;
        // find the location matching time

        if (_verbose)
        {
            Console.WriteLine("matching current time with location");
        }

        foreach (var location in locations)
        {
            if (_verbose)
            {
                Console.WriteLine($"- {location}");
                Console.WriteLine($"- start-time  <=  current time?: {location.Start < time} ");
                Console.WriteLine($"- current time < end-time?: {time < location.End} ");
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
            if (_verbose)
            {
                Console.WriteLine($"\nno location matching current time");
            }
        }

        return foundLocation;
    }
    
    private bool IsInsideDefault(TimeOnly time)
    {
        if (_verbose)
        {
            Console.WriteLine($"Inside default working hours?: {_workStartDefault <= _currentTime && _currentTime <= _workEndDefault}");
        }

        return _workStartDefault <= _currentTime && _currentTime <= _workEndDefault;
    }

    private bool IsWeekend()
    {
        return false; // TODO: DELETE IT'S FOR TESTING

        if (_verbose)
        {
            Console.WriteLine($"It's weekend?: {DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday}");
        }
        
        return DateTime.Now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}