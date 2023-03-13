using EC_locator.Core.Interfaces;
using EC_locator.Repositories;
using EC_locator.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace Parser;

public class MessageParser : IMessageParser
{
    private static LocatorRepository? _locatorRepository;
    private bool _verbose = false;
    
    // holding default values
    private readonly TimeOnly _workStartDefault = new TimeOnly(9,0);
    private readonly TimeOnly _workEndDefault = new TimeOnly(16,0);
    private readonly Location _defaultLocation = new Location("office");
    
    // holding identified tags and their index found in message
    private SortedList<int, Location>? _locations;
    private SortedList<int, TimeOnly>? _times;
    
    // interpreted locations found in message
    private List<Location> _locationsFound;
    
    public MessageParser()
    {
       _locatorRepository= new LocatorRepository();
    }
    
    private void ConnectTimesAndLocations()
    {
        // checking if location contains "ill" then return Location ill and all day
        foreach (var location in _locations)
        {
            if (location.Value.Place.Equals("ill"))
            {
                Console.WriteLine("Jeg er syg og hjemme");
                _locationsFound.Add(new Location(_workStartDefault, _workEndDefault, "ill"));
                return;
            }
        }
        
        // is time indication present = all day location, or error
        if (_times.IsNullOrEmpty())
        {
            // If location count is 1 One location and no time = all day - using defaults
            if (_locations.Count == 1)
            {
                Console.WriteLine("no time and one location");
                _locations.Values[0].Start = _workStartDefault;
                _locations.Values[0].End = _workEndDefault;
                _locationsFound.Add(_locations.Values[0]);
                return;
            }
            
            if (_locations.Count > 1)
            {
                Console.WriteLine("no time indication and more than one location found - unable to define");
                _locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
                return;
            }
        }
        
        // is the first index recorded a time tag -> then insert office as location at 0
        if (_locations.Keys[0] > _times.Keys[0])
        {
            Console.WriteLine("starts with Times keys without location - adding default");
            // inserting default location at index 0
            _locations.Add(0, _defaultLocation);
        }
        
        // is there one location and one time recorded => insert home at 0
        if (_locations.Count == 1 && _times.Count == 1)
        {
            {
                Console.WriteLine("one location and one time");
                _locations.Add(0, new Location("home"));
            }
        }
        
        // checking if number of locations tags 2 higher than number of times tags?
        if (_locations.Count - 1 > _times.Count)
        {
            Console.WriteLine("number of location tags i 2 more than timestags");
            // check if a location is a meeting
            int locationToRemove = -1;
            foreach (var location in _locations)
            {
                if (location.Value.Place.Equals("meeting"))
                {
                    for (int i = 0; i < _times.Count; i++)  
                    {   
                        // identifying to consecutive locations
                        if (_locations.Keys[i] < _times.Keys[i] && _locations.Keys[i + 1] < _times.Keys[i])
                        {
                            // either location i or i+1 is a meeting - remove the one that's not.
                            for (int j = i; j < i + 2; j++)
                            {
                                if (!_locations.Values[j].Place.Equals("meeting"))
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
                if (_verbose) {Console.WriteLine("not able to remove unnessesacry location found - unable to define");}
                
                _locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
                return;
            }

            _locations.Remove(_locations.Keys[locationToRemove]);
            Console.WriteLine("moving location not a meeting");
        }
        
        // adding times to locations - Algorithm
        for (int i = 0; i < _locations.Count; i++)
        {
            if (i == 0)
            {
                _locations.Values[i].Start = _workStartDefault;
            }
            else
            {
                _locations.Values[i].Start = _times.Values[i - 1];
            }

            if (i == _locations.Count - 1)
            {
                _locations.Values[i].End = _workEndDefault;
            }
            else
            {
                _locations.Values[i].End = _times.Values[i];
            }

            _locationsFound.Add(_locations.Values[i]);
        }
    }
    
    public List<Location> GetLocations(string message)
    {
        _locationsFound = new List<Location>();
        
        _locations = IdentifyLocations(message);
        if (_verbose)
        {
            foreach (var location in _locations)
            {
                Console.WriteLine($"location found {location.Value.Place} at index {location.Key}");
            }
        }

        _times = IdentifyTimes(message);
        if (_verbose)
        {
            foreach (var time in _times)
            {
                Console.WriteLine($"time found {time.Value} at index {time.Key}");
            }
        }
        ConnectTimesAndLocations();

        return _locationsFound;
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
        int foundAtIndex = 0;
        string number = "";
        //                       index in string, identified time
        var identifiedTimeOnIndex = new SortedList<int, TimeOnly>();
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
                identifiedTimeOnIndex.Add(foundAtIndex, ConvertToTimeOnly(number));
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
            
        return identifiedTimes;
    }

    private TimeOnly ConvertToTimeOnly(string number)
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

        return listOfLocations;
    }

}