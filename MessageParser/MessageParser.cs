
using System.Collections;
using EClocator.Core.Interfaces;
using EC_locator.Repositories;
using EClocator.Core.Models;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Location = EClocator.Core.Models.Location;

namespace Parser;

public class MessageParser : IMessageParser
{
    private static LocatorRepository _locatorRepository;
    private TimeDefinition _timeDefinition;
    private TimeOnly _workStartDefault = new TimeOnly(9,0);
    private TimeOnly _workEndDefault = new TimeOnly(16,0);
    
    SortedList<int, Location> locations;
    SortedList<int, TimeOnly> times;
    List<Location> locationsFound;
    private Location _defaultLocation = new Location("office");
    
    public MessageParser()
    {
       _locatorRepository= new LocatorRepository();
       _timeDefinition = new TimeDefinition("formiddag", new TimeOnly(9), new TimeOnly(12));
    }

    public void PrintLocations(string message)
    {
        Console.WriteLine($"\n{message}");
        locationsFound = new List<Location>();
        locations = IdentifyLocations(message);
        times = IdentifyTimes(message);
        
        /*
        foreach (var location in locations)
        {
            Console.WriteLine($"Location: {location.Value.Place}, at index {location.Key}");
        }
        
        foreach (var timeOnly in times)
        {
            Console.WriteLine($"Time:{timeOnly.Value}, at index {timeOnly.Key}");
        }
        */
        
        // Adding times to locations
        ConnectTimesAndLocations(message);
        foreach (var location in locationsFound)
        {
            Console.WriteLine(location);
        }
    }

    private void ConnectTimesAndLocations(string message)
    {
        // checking if contains "ill" then return Location ill and all day
        foreach (var location in locations)
        {
            if (location.Value.Place.Equals("ill"))
            {
                Console.WriteLine("Jeg er syg og hjemme");
                locationsFound.Add(new Location(_workStartDefault, _workEndDefault, "ill"));
                return;
            }
        }
        
        // if no times information = all day location, or error
        if (times.IsNullOrEmpty())
        {
            // One location and no time = all day - using defaults
            if (locations.Count == 1)
            {
                Console.WriteLine("no time and one location");
                locations.Values[0].Start = _workStartDefault;
                locations.Values[0].End = _workEndDefault;
                locationsFound.Add(locations.Values[0]);
                return;
            }

            if (locations.Count > 1)
            {
                Console.WriteLine("no time indication and more than one location found - unable to define");
                locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
                return;
            }
        }
        
        // check if the first statement is a timekey -> then insert office as location
        if (locations.Keys[0] > times.Keys[0])
        {
            Console.WriteLine("starts with Times keys without location - adding default");
            // inserting default location at index 0
            locations.Add(0, _defaultLocation);
        }
        
        // adding times to locations
        if (locations.Count - 1 > times.Count)
        {
            Console.WriteLine("--Special case--)");
            Console.WriteLine("number of locations i 2 more than times - unable to identify - prioritize locations");
            return;
        }

        for (int i = 0; i < locations.Count; i++)
        {
            if (i == 0)
            {
                locations.Values[i].Start = _workStartDefault;
            }
            else
            {
                locations.Values[i].Start = times.Values[i - 1];;
            }

            if (i == locations.Count - 1)
            {
                locations.Values[i].End = _workEndDefault;
            }
            else
            {
                locations.Values[i].End = times.Values[i];
            }

            locationsFound.Add(locations.Values[i]);
        }
    }
    
    public List<Location> GetLocations(string message)
    {
        locationsFound = new List<Location>();
        
        locations = IdentifyLocations(message);
        times = IdentifyTimes(message);
        ConnectTimesAndLocations(message);

        return locationsFound;
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
                // Console.WriteLine(ConvertToTimeOnly(number).ToString());
                number = "";
            }
        }

        return identifiedTimeOnIndex;
    }

    private SortedList<int, TimeOnly> IdentifyTimes(string message)
    {
        var identifiedTimes = IdentifyNumericTime(message);
        //var identifiedKeywordTimes = IdentifyKeywordsTime(message);
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

    public void LocateTime()
    {
        
    }
}