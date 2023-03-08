
using EClocator.Core.Interfaces;
using EC_locator.Repositories;
using EClocator.Core.Models;

namespace Parser;

public class MessageParser : IMessageParser
{
    private static LocatorRepository _locatorRepository;
    
    public MessageParser()
    {
       _locatorRepository= new LocatorRepository();  
    }

    public void PrintLocations(string message)
    {
        Console.WriteLine(message);
        
        SortedList<int, Location> listOfLocations = IdentifyLocations(message);
        foreach (var location in listOfLocations)
        {
            Console.WriteLine($"Location: {location.Value.Place}, at index {location.Key}");
        }

        var times = IdentifyNumericTime(message);
        foreach (var timeOnly in times)
        {
            Console.WriteLine($"Time:{timeOnly.Value}, at index {timeOnly.Key}");
        }
        
        times = IdentifyKeywordsTime(message);
        foreach (var timeOnly in times)
        {
            Console.WriteLine($"Time:{timeOnly.Value}, at index {timeOnly.Key}");
        }
    }

    public void GetLocations(string message)
    {
        SortedList<int, Location> listOfLocations = IdentifyLocations(message);
        
        
        // run through message and locate time indicators with index
        // 1. look for numbers
        // 2. look for keywords
        // 3. look for før / efter, indtil
        
        // connect times and 
    }

    public Dictionary<int, TimeOnly> IdentifyKeywordsTime(string message)
    {
        var timeKeywords = _locatorRepository.GetTimeKeywords();
        var identifiedTimeOnIndex = new Dictionary<int, TimeOnly>();

        foreach (var timeKeyword in timeKeywords)
        {
            if (message.Contains(timeKeyword.Key))
            {
                var foundAtIndex = message.IndexOf(timeKeyword.Key, StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"Found {timeKeyword.Key} at index {foundAtIndex} meaning {timeKeyword.Value}");
                identifiedTimeOnIndex.Add(foundAtIndex, timeKeyword.Value);
            }
        }
        
        return identifiedTimeOnIndex;
    }

    private Dictionary<int, TimeOnly> IdentifyNumericTime(string message)
    {
        int foundAtIndex = 0;
        string number = "";
        //                       index in string, identified time
        var identifiedTimeOnIndex = new Dictionary<int, TimeOnly>();
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