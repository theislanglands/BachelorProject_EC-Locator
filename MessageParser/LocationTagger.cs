using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;

namespace Parser;

public class LocationTagger
{
    private LocatorRepository? _locatorRepository;
    private readonly bool _verbose;
    private readonly object _workStartDefault;
    private Settings _settings;
    private readonly TimeOnly _workEndDefault;
    private readonly Location _defaultLocation;

    public LocationTagger()
    {
        _locatorRepository= new LocatorRepository();
        _settings = Settings.GetInstance();
        _workStartDefault = _settings.WorkStartDefault;
        _workEndDefault = _settings.WorkEndDefault;
        _defaultLocation = _settings.DefaultLocation;
        _verbose = _settings.Verbose;
    }

    public SortedList<int, Location> IdentifyLocations(string message)                                    
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
}