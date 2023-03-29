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
        _locationTags = IdentifyLocations(message);
        _timeTags = IdentifyTimes(message);
        
        // Modifying and connecting times and locations according to linguistic meanings
        ModifyLocationsFound();
        AddTimesToLocations();
        
        return _locationsFound;
    }
    
    private SortedList<int, Location> IdentifyLocations(string message)                                    
    {
        // get dictionary mapping keywords to location
        Dictionary<string, string> locationWordsDictionary= _locatorRepository.GetLocationKeyWordsDictionary();
        // holding found location and the index located in the message
        var foundLocations = new SortedList<int, string>();
        
        
        // Dictionary<string, int> foundLocations = new Dictionary<string, int>();
        message = message.ToLower();    
        
        
        // run through each word to identify keyword belonging to each category
        foreach (var locationWord in locationWordsDictionary)
        {
            if (message.Contains(locationWord.Key))
            {
                // getting index of keyword in message
                int indexOfKeyWord = message.IndexOf(locationWord.Key, StringComparison.OrdinalIgnoreCase);
                
                // getting index of a found location containing the Value Location word
                
                // Adding Location if not already found
                if (!foundLocations.ContainsValue(locationWord.Value))
                {
                    foundLocations.Add(indexOfKeyWord, locationWord.Value);
                }

                else
                {
                    int indexOfFoundLocation = foundLocations.IndexOfValue(locationWord.Value);
                    if (foundLocations.GetKeyAtIndex(indexOfFoundLocation) <= indexOfKeyWord) {
                        // delete found location and create a new one!
                        foundLocations.RemoveAt(indexOfFoundLocation);
                        foundLocations.Add(indexOfKeyWord, locationWord.Value);
                    }
                }
            }
        }
        
        
        // Check if a negation keyword is present
        // TODO: get negation keywords from repo
        
        string negationKeyWord = "ikke ";
        if (message.Contains(negationKeyWord))
        {
            // Identify index of negation keyword
            int indexOfNegationWord = message.IndexOf(negationKeyWord, StringComparison.OrdinalIgnoreCase);
            

            var locationsToRemove = new List<int>();
            // Check if negation index is between two locations
            for (int i = 0; i < foundLocations.Count-1; i++)
            {
                if (foundLocations.GetKeyAtIndex(i) <= indexOfNegationWord && indexOfNegationWord < foundLocations.GetKeyAtIndex(i+1))
                {
                    // remove location after negation word
                    for (int j = indexOfNegationWord+1; j < foundLocations.Last().Key+1; j++) {
                        if (foundLocations.ContainsKey(j))
                        {
                            if (_verbose)
                            {
                                Console.WriteLine($"Negation Keyword found at index {j} - ignoring location: {foundLocations[j]}");
                            }

                            locationsToRemove.Add((j));
                            break;
                        }
                    }
                }
            }
            foreach (var key in locationsToRemove)
            {
                foundLocations.Remove(key);
            }
        }
        
        // creating a list of Locations objects sorted after index of location in message
        SortedList<int, Location> listOfLocations = new SortedList<int, Location>();

        foreach (var loc in foundLocations)
        {
            Location location = new Location();
            location.Place = loc.Value;
            listOfLocations.Add(loc.Key, location);
        }
        
        if (_verbose)
        {
            foreach (var location in listOfLocations)
            {
                Console.WriteLine($"location found {location.Value.Place} at index {location.Key}");
            }
        }
        
        //return foundLocations;
        return listOfLocations;
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
}