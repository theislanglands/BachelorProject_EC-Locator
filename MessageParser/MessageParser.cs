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
    
    private string message;
    
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
        this.message = message;
        _locationsFound = new List<Location>();
        
        // Identifying location and times tags in message
        _locationTags = IdentifyLocations(message);
        _timeTags = IdentifyTimes(message);
        
        //ModifyTimesAndLocations();
        ModifyTimesAndLocationDecisionTree();
        AddTimesToLocations();
        return _locationsFound;
    }

    private void ModifyTimesAndLocationDecisionTree()
    {
        DecisionTree dt = new DecisionTree();
        dt.Perform(_locationTags, _timeTags);
    }

    private void ModifyTimesAndLocations()
    {
        
        // checking if no locations are found  - DONE
        if (_locationTags.Count == 0)
        {
            if (_verbose)
            {
                Console.WriteLine("unable to decide - no locations identified - adding undefined at 0");
            }
            
            _locationTags.Add(0, new Location("undefined"));
        }

        // checking if location contains "ill" then return Location ill and all day - DONE
        foreach (var location in _locationTags)
        {
            if (location.Value.Place.Equals("ill"))
            {
                if (_verbose)
                {
                    Console.WriteLine("Ill detected - deleting all other location");
                }
                
                _locationTags.Clear();
                _locationTags.Add(0, new Location(_workStartDefault, _workEndDefault, "ill"));
                break;
                //_locationsFound.Add(new Location(_workStartDefault, _workEndDefault, "ill"));
                // return;
            }
        }
        
        // is no time indication present and more than 1 location
        if (_timeTags.IsNullOrEmpty())
        {
            if (_locationTags.Count > 1)
            {
                if (_verbose)
                {
                    Console.WriteLine("no time indication and more than one location found - unable to define");
                }
                
                _locationTags.Clear();
                _locationTags.Add(0, new Location("undefined"));
                
                //_locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
                // return;
            }
        }
        
        // is the first index recorded a time tag -> then insert office as location at 0
        
        if (_timeTags.Count > 0 && _locationTags.Keys[0] > _timeTags.Keys[0])
        {
            if (_verbose)
            {
                Console.WriteLine("starts with Times keys without location");
            }

            if (_locationTags.Values[0].Place.Equals("office"))
            {
                if (_verbose)
                {
                    Console.WriteLine("- setting start time to first location and end to default");
                }
                
                // insert home before 
                _locationTags.Add(0, new Location("home"));
                /*
                _locations.Values[0].Start = _times.Values[0];
                _locations.Values[0].End = _workEndDefault;
                _locationsFound.Add(_locations.Values[0]);
                return;
                */
            }
            else
            {
                if (_verbose)
                {
                    Console.WriteLine("- - adding default location");
                }
                // inserting default location (office) at index 0
                _locationTags.Add(0, _defaultLocation);
            }
        }

        // is there one location and one time recorded => insert home at 0
        if (_locationTags.Count == 1 && _timeTags.Count == 1)
        {
            if (_verbose)
            {
                Console.WriteLine("one location and one time");
            }
            
            if (!_locationTags.Values[0].Place.Equals("home"))
            {
                {
                    if (_verbose)
                    {
                        Console.WriteLine(" - adding home at beginning");
                    }

                    _locationTags.Add(0, new Location("home"));
                }
            }
        }
        
        // checking if number of locations tags 2 higher than number of times tags?
        if (_locationTags.Count - 1 > _timeTags.Count)
        {
            if (_verbose)
            {
                Console.WriteLine("number of location tags i 2 more than times-tags");
            }
            // check if a location is a meeting
            int locationToRemove = -1;
            foreach (var location in _locationTags)
            {
                if (location.Value.Place.Equals("meeting"))
                {
                    for (int i = 0; i < _timeTags.Count; i++)  
                    {   
                        // identifying to consecutive locations
                        if (_locationTags.Keys[i] < _timeTags.Keys[i] && _locationTags.Keys[i + 1] < _timeTags.Keys[i])
                        {
                            // either location i or i+1 is a meeting - remove the one that's not.
                            for (int j = i; j < i + 2; j++)
                            {
                                if (!_locationTags.Values[j].Place.Equals("meeting"))
                                {
                                    // removing the location
                                    locationToRemove = j;
                                }
                            }
                        }
                    }
                }
            }

            if (locationToRemove == -1)
            {
                if (_verbose)
                {
                    Console.WriteLine("not able to remove unnecessary location found - unable to define");
                }
                
                _locationTags.Clear();
                _locationTags.Add(0, new Location("undefined"));
                //_locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
                //return;
            }

            if (_verbose)
            {
                Console.WriteLine("moving location not a meeting");
            }
            _locationTags.Remove(_locationTags.Keys[locationToRemove]);
        }
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
                if (_locationTags.Values[i].Start == null)
                {
                    _locationTags.Values[i].Start = _workStartDefault;
                }
                
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
            if (_timeTags.Count > 0 && message[.._timeTags.Keys[^1]].Contains(stopIndicator))
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
            if (_timeTags.Count > 0 && message[.._timeTags.Keys[0]].Contains(startIndicator))
            {
                return (_locationTags.Count.Equals(_timeTags.Count));
            }
        }

        return false;
    }


    private SortedList<int, TimeOnly> IdentifyKeywordsTime(string message)
    {
        var timeKeywords = _locatorRepository.GetTimeKeywords();
        var identifiedTimeOnIndex = new SortedList<int, TimeOnly>();

        foreach (var timeKeyword in timeKeywords)
        {
            if (message.Contains(timeKeyword.Key))
            {
                var foundAtIndex = message.IndexOf(timeKeyword.Key, StringComparison.OrdinalIgnoreCase);
                identifiedTimeOnIndex.Add(foundAtIndex, timeKeyword.Value);
            }
        }
        
        return identifiedTimeOnIndex;
    }

    private SortedList<int, TimeOnly> IdentifyNumericTime(string message)
    {
        // index in string, identified time
        var identifiedTimeOnIndex = new SortedList<int, TimeOnly>();
     
        int foundAtIndex = 0;
        string number = "";
        
        for (int i = 0; i < message.Length; i++)
        {
            if (char.IsDigit(message[i]))
            {
                number += message[i];
                foundAtIndex = i;
                while (i < message.Length-1 && (char.IsDigit(message[i + 1]) || message[i + 1].Equals('.') || message[i + 1].Equals(':')))
                {
                    i++;
                    if (message[i].Equals('.') || message[i].Equals(':')) {
                        continue;
                    }
                    number += message[i];
                }
            }
            
            if (!number.Equals(""))
            {
                TimeOnly foundTime = ParseToTimeOnly(number);
                
                // see if a message contains a minute indicator between start and found index
                var minuteIndicators = _locatorRepository.GetMinuteIndicators();
                
                foreach (var minuteIndicator in minuteIndicators)
                {

                    if (message[..foundAtIndex].Contains(minuteIndicator.Key))
                    {
                        if (_verbose)
                        {
                            Console.Write($"minute indicator \"{minuteIndicator.Key}\" found before time - {foundTime} - correcting time with {minuteIndicator.Value} minutes");
                        }
                        foundTime = foundTime.AddMinutes(minuteIndicator.Value);
                        
                        if (_verbose)
                        {
                            Console.WriteLine($" - corrected time: {foundTime}");
                        }
                    }
                }
                
                identifiedTimeOnIndex.Add(foundAtIndex, foundTime);
                number = "";
            }
        }
        
        
        
        return identifiedTimeOnIndex;
    }

    private SortedList<int, TimeOnly> IdentifyTimes(string message)
    {
        var identifiedTimes = IdentifyNumericTime(message);
        foreach (var item in IdentifyKeywordsTime(message))
        {
            identifiedTimes.Add(item.Key, item.Value);
        }
        if (_verbose)
        {
            foreach (var time in identifiedTimes)
            {
                Console.WriteLine($"time found {time.Value} at index {time.Key}");
            }
        }
            
        return identifiedTimes;
    }

    private static TimeOnly ParseToTimeOnly(string number)
    {
        string hour;
        string minutes;

        if (number.Length > 4)
        {
            Console.WriteLine("unable to define time more than 4 characters");
        }

        if (number.Length == 4)
        {
            hour = number.Substring(0, 2);
            minutes = number.Substring(2, 2);
        }
                
        else if (number.Length == 3)
        {
            hour = number.Substring(0, 1);
            minutes = number.Substring(1, 2);
        }

        else
        {
            hour = number;
            minutes = "00";
        }

        return new TimeOnly(int.Parse(hour), int.Parse(minutes));
    }

    private SortedList<int, Location> IdentifyLocations(string message)                                    
    {
        // get dictionary mapping keywords to location
        Dictionary<string, string> locationWordsDictionary= _locatorRepository.GetLocationKeyWordsDictionary();
        // holding found location and the index located in the message
        Dictionary<string, int> foundLocations = new Dictionary<string, int>();
        message = message.ToLower();    
        
        // run through each word to identify keyword belonging to each category
        foreach (var locationWord in locationWordsDictionary)
        {
            if (message.Contains(locationWord.Key))
            {
                int indexOfKeyWord = message.IndexOf(locationWord.Key, StringComparison.OrdinalIgnoreCase);
                // Console.WriteLine($"Found: {locationWord.Key}   =>   location: {locationWord.Value}  =>  Index: {indexOfWord}");
                
                // Adding Location if not already found, otherwise update index if higher
                if (!foundLocations.ContainsKey(locationWord.Value))
                {
                    foundLocations.Add(locationWord.Value, indexOfKeyWord);
                }
                else if (foundLocations[locationWord.Value] <= indexOfKeyWord){
                        foundLocations[locationWord.Value] = indexOfKeyWord;
                }
            }
        }
        
        // creating a list of Locations objects sorted after index of location in message
        SortedList<int, Location> listOfLocations = new SortedList<int, Location>();

        foreach (var loc in foundLocations)
        {
            Location location = new Location();
            location.Place = loc.Key;
            listOfLocations.Add(loc.Value, location);
        }
        
        if (_verbose)
        {
            foreach (var location in listOfLocations)
            {
                Console.WriteLine($"location found {location.Value.Place} at index {location.Key}");
            }
        }
        
        return listOfLocations;
    }
}