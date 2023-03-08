
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

        SortedList<int, Location> listLocation = PrintLocation(message);
        
        // Printing list
        foreach (var location in listLocation)
        {
            Console.WriteLine($"Location {location.Value.Place} found at index {location.Key}");
        }
        
    }

    public void GetLocations()
    {
        // Getting locations and their index
        
        // Getting any time indicators between indexes
    }


    public SortedList<int, Location> PrintLocation(string message)                                    
    {
        // get dictionary mapping keywords to location
        Dictionary<string, string> locationWordsDictionary= _locatorRepository.GetLocationKeyWordsDictionary();
        message = message.ToLower();
        
        // holding found location and the index located in the message
        Dictionary<string, int> foundLocations = new Dictionary<string, int>();
        
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
        SortedList<int, Location> listLocation = new SortedList<int, Location>();

        foreach (var loc in foundLocations)
        {
            Location location = new Location();
            location.Place = loc.Key;
            listLocation.Add(loc.Value, location);
        }

        return listLocation;
    }

    public void LocateTime()
    {
        
    }
}