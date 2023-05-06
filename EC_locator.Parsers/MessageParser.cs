using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
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
    
    public List<Location> GetLocations(Message message)
    {
        if (_verbose)
        {
            Console.WriteLine(" -- Parsing Message to Locations --");
        }
        
        // Getting location and time tags in message
        _locationTags = _locationTagger.GetTags(message);
        _timeTags = _timeTagger.GetTags(message);
        
        HandleReplies(message);

        // Modifying and connecting times and locations according to linguistic meanings
        ModifyFoundLocations();
        
        //Combining time and location tags into list of locations
        var locationsFound = _timeAndLocationConnector.AddTimeToLocations(_locationTags, _timeTags);
        
        return locationsFound;
    }
    
    public bool ContainsTomorrow(string message)
    {
        if (message.ToLower().Contains("i morgen") 
            || message.ToLower().Contains("imorgen")
            || message.ToLower().Contains("morrow"))
        {
            return true;
        }

        return false;
    }
    
    private void HandleReplies(Message message)
    {
        if (message.Replies == null) return;
        
        // Find replies that contains same user ID as message
        var selfReplies = message.Replies.Where(msg => msg.UserId.Equals(message.UserId)).ToList();
        
        if (selfReplies.Count == 0)
        {
            if (_verbose)
            {
                Console.WriteLine($"- replies only by other users found - ignored");
            }
            return;
        }

        // Analyse latest reply
        Message lastReply = selfReplies.Last();
        if (_verbose)
        {
            Console.WriteLine($"- reply on own message found: {lastReply.Content}");
        }
    
        // if reply message contains 1 time tag and no location tags AND original message has time tags = update last time in message
        if (_timeTagger.GetTags(lastReply).Count == 1 && _locationTagger.GetTags(lastReply).Count == 0 && _timeTags.Count != 0)
        {
            int keyOfLastTag = _timeTags.Last().Key;
            _timeTags[keyOfLastTag] = _timeTagger.GetTags(lastReply).Values[0];
            if (_verbose)
            {
                Console.WriteLine(
                    $"A reply found with an updated time - changing last time tag to {_timeTagger.GetTags(lastReply).Values[0]}");
            }

            return;
        }

        if (_locationTagger.GetTags(lastReply).Count == 0)
        {
            if (_verbose)
            {
                Console.WriteLine("No locations in self reply - reply ignored");
            }
            return;
        }
        
        // Replace tags from original message with the latest Reply
        _locationTags = _locationTagger.GetTags(lastReply);
        _timeTags = _timeTagger.GetTags(lastReply);
    }
    
    private void ModifyFoundLocations()
    {
        DecisionTree dt = new DecisionTree(_options);
        dt.Perform(_locationTags, _timeTags);
    }
}