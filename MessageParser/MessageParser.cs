
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

    public void SplitCount(string message)
    {
        string[] splitterWords = _locatorRepository.GetSplitterKeywords(); 
        
        // convert to lower case
        message = message.ToLower();
        
        // split into words
        string[] words = message.Split(' ');

        int splitcount = 0;
         //  iterate words and identify if contains a splitter
        foreach (var word in words) 
        {
            foreach (string splitterWord in splitterWords)
            {
                if (word == splitterWord)
                {
                    Console.WriteLine($"split word {splitterWord}");
                    splitcount++;
                }
            }
        }

        Console.WriteLine($"{message} - contains {splitcount + 1} locations");
    }

    public void LocationCount(string message)                                    
    {                                                                         
        // get array of words
        //string[,] locationWords = _locatorRepository.GetLocationKeyWords2();
        Dictionary<string, string> locationWordsDictionary= _locatorRepository.GetLocationKeyWordsDictionary();
        // convert to lower case                                              
        message = message.ToLower();

        Console.WriteLine(message);
        // run through each word to identify one keyword belonging to each
        
        Dictionary<string, int> foundLocations = new Dictionary<string, int>();
        
        foreach (var locationWord in locationWordsDictionary)
        {

            if (message.Contains(locationWord.Key))
            {
                int indexOfWord = message.IndexOf(locationWord.Key, StringComparison.OrdinalIgnoreCase);
                // Console.WriteLine($"Found: {locationWord.Key}   =>   location: {locationWord.Value}  =>  Index: {indexOfWord}");
                
                // checking if location is already localized
                if (foundLocations.ContainsKey(locationWord.Value))
                {
                    // if location is localized, compare index of newly found and update if bigger
                    if (foundLocations[locationWord.Value] < indexOfWord)
                    {
                        foundLocations[locationWord.Value] = indexOfWord;
                    }
                }
                else
                {
                    foundLocations.Add(locationWord.Value, indexOfWord);
                }
            }
        }
        
        // creating a list of locations sorted after index of location in message
        SortedList<int, Location> listLocation = new SortedList<int, Location>();

        foreach (var loc in foundLocations)
        {
            Location location = new Location();
            location.Place = loc.Key;
            listLocation.Add(loc.Value, location);
        }

        foreach (var location in listLocation)
        {
            Console.WriteLine($"Location {location.Value.Place} found at index {location.Key}");
        }

        /* creating location object
        Location location = new Location();
        location.Place = locationWord.Value;
*/

        //Console.WriteLine($"{message} - contains {locationCount} locations");
    }
}