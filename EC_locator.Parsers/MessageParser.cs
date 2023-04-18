using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using Location = EC_locator.Core.Models.Location;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class MessageParser : IMessageParser
{
    private readonly bool _verbose;
    private readonly IOptions<VerboseOptions> _options;
    private readonly ILocationTagger _locationTagger;
    private readonly ITimeTagger _timeTagger;
    private readonly ITimeAndLocationConnector _timeAndLocationConnector;
 
    // index of identified tags in message
    private SortedList<int, Location>? _locationTags;
    private SortedList<int, TimeOnly>? _timeTags;

    public MessageParser(ILocationTagger locationTagger, ITimeTagger timeTagger, ITimeAndLocationConnector timeAndLocationConnector, IOptions<VerboseOptions> settingsOptions)
    {
        _locationTagger = locationTagger;
        _timeTagger = timeTagger;
        _timeAndLocationConnector = timeAndLocationConnector;
        _verbose = settingsOptions.Value.Verbose;
        _options = settingsOptions;
    }
    
    public List<Location> GetLocations(string message)
    {
        if (_verbose)
        {
            Console.WriteLine(" -- Parsing Message to Locations --");
        }
        
        // Getting location and time tags in message
        _locationTags = _locationTagger.GetTags(message);
        _timeTags = _timeTagger.GetTags(message);
        
        // Modifying and connecting times and locations according to linguistic meanings
        ModifyLocationsFound();
        
        //Combining time and location tags into list of locations
        var locationsFound = _timeAndLocationConnector.AddTimeToLocations(_locationTags, _timeTags);
        
        return locationsFound;
    }

    public bool ContainsTomorrow(string message)
    {
        if (message.Contains("i morgen"))
        {
            return true;
        }

        return false;
    }
    
    private void ModifyLocationsFound()
    {
        // TODO inject dependency!
        DecisionTree dt = new DecisionTree(_options);
        dt.Perform(_locationTags, _timeTags);
    }
}