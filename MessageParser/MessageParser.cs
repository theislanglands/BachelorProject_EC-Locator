using System.Collections;
using EC_locator.Core;
using EC_locator.Core.Interfaces;
using EC_locator.Repositories;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Location = EC_locator.Core.Models.Location;

namespace Parser;

public class MessageParser : IMessageParser
{
    private Settings _settings;
    
    // holding default values
    private TimeOnly _workStartDefault;
    private TimeOnly _workEndDefault;
    private readonly Location _defaultLocation;
    
    private static LocatorRepository? _locatorRepository;
    private readonly bool _verbose;
 
    // holding identified tags and their index found in message
    private SortedList<int, Location>? _locationTags;
    private SortedList<int, TimeOnly>? _timeTags;
    
    // interpreted locations found in message
    private List<Location> _locationsFound;

    private LocationTagger _locationTagger = new LocationTagger();
    private TimeTagger _timeTagger = new TimeTagger();
    
    private string _message;
    
    public MessageParser()
    {
       _locatorRepository= new LocatorRepository();
       _settings = Settings.GetInstance();
       _workStartDefault = _settings.WorkStartDefault;
       _workEndDefault = _settings.WorkEndDefault;
       _defaultLocation = _settings.DefaultLocation;
       _verbose = _settings.Verbose;
    }
    
    public List<Location> GetLocations(string message)
    {
        this._message = message;
        _locationsFound = new List<Location>();
        
        // Identifying location and times tags in message
        _locationTags = _locationTagger.GetTags(message);
        _timeTags = _timeTagger.IdentifyTimes(message);
        
        // Modifying and connecting times and locations according to linguistic meanings
        ModifyLocationsFound();
        AddTimesToLocations();
        
        return _locationsFound;
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
    
    private void AddTimesToLocations()
    {
        // adding times to locations - Algorithm
        if (_verbose)
        {
            Console.WriteLine("running times algorithm");
        }

        for (int i = 0; i < _locationTags.Count; i++)
        {
            if (i == 0)
            {
                // if first location haven't allready a start time assigned => assign default
                _locationTags.Values[i].Start ??= _workStartDefault;

                // check if first location has a start keyword
                if (HasStartIndicator())
                {
                    _locationTags.Values[i].Start = _timeTags.Values[0];
                    _locationTags.Values[i].End = _timeTags.Values[1];
                }
            }
            else
            {
                _locationTags.Values[i].Start = _locationsFound[^1].End;
            }

            // SETTING END TIMES
            if (_locationTags.Values[i].End == null)
            {
                if (i == _locationTags.Count - 1)
                {
                    if (HasStopIndicator())
                    {
                        _locationTags.Values[i].End = _timeTags.Values[^1];

                    }
                    else
                    {
                        _locationTags.Values[i].End = _workEndDefault;
                    }
                }
                else
                {
                    _locationTags.Values[i].End = _timeTags.Values[i];
                }
            }
            _locationsFound.Add(_locationTags.Values[i]);
        }
    }

    private bool HasStopIndicator()
    {
        foreach (var stopIndicator in _locatorRepository.GetStopIndicatorKeywords())
        {
            if (_timeTags.Count > 0 && _message[.._timeTags.Keys[^1]].Contains(stopIndicator))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasStartIndicator()
    {
        foreach (var startIndicator in _locatorRepository.GetStartIndicatorKeywords())
        {
            if (_timeTags.Count > 0 && _message[.._timeTags.Keys[0]].Contains(startIndicator))
            {
                return (_locationTags.Count.Equals(_timeTags.Count));
            }
        }

        return false;
    }


    
}