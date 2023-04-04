using EC_locator.Core;
using EC_locator.Core.Interfaces;
using Location = EC_locator.Core.Models.Location;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Parser;

public class MessageParser : IMessageParser
{
    private readonly bool _verbose;
 
    // index of identified tags in message
    private SortedList<int, Location>? _locationTags;
    private SortedList<int, TimeOnly>? _timeTags;
    
    private readonly LocationTagger _locationTagger = new();
    private readonly TimeTagger _timeTagger = new();
    private readonly TimeAndLocationConnector _timeAndLocationConnector = new();
    
    public MessageParser()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices(
                services =>
                {
                    services.AddSingleton<ISettings, Settings>();
                })
            
            .Build();
        var settings = host.Services.GetRequiredService<ISettings>();
        _verbose = settings.GetInstance().Verbose;
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
        var locationsFound = _timeAndLocationConnector.AddTimeToLocations(_locationTags, _timeTags, message);
        
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
        DecisionTree dt = new DecisionTree();
        dt.Perform(_locationTags, _timeTags);
    }
}