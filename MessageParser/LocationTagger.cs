using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;

namespace Parser;

public class LocationTagger
{
    private readonly LocatorRepository? _locatorRepository;
    private readonly bool _verbose;

    public LocationTagger()
    {
        _locatorRepository= new LocatorRepository();
        _verbose = Settings.GetInstance().Verbose;
    }

    public SortedList<int, Location> GetTags(string message)                                    
    {
        // index, found place located in the message
        
        message = message.ToLower();    
        var foundLocations = FindLocations(message);
        HandleNegationKeywords(message, foundLocations);

        var listOfLocations = LocationsAsList(foundLocations);

        if (_verbose)
        {
            foreach (var location in listOfLocations)
            {
                Console.WriteLine($"location found {location.Value.Place} at index {location.Key}");
            }
        }
        
        return listOfLocations;
    }

    // mapping keywords to location

    private SortedList<int, string> FindLocations(string message)
    {
        var foundLocations = new SortedList<int, string>();
 
        var locationWordsDictionary = _locatorRepository.GetLocationKeyWordsDictionary();
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
                    if (foundLocations.GetKeyAtIndex(indexOfFoundLocation) <= indexOfKeyWord)
                    {
                        // delete found location and create a new one!
                        foundLocations.RemoveAt(indexOfFoundLocation);
                        foundLocations.Add(indexOfKeyWord, locationWord.Value);
                    }
                }
            }
        }

        return foundLocations;
    }

    private SortedList<int, Location> LocationsAsList(SortedList<int, string> foundLocations)
    {
        // creating a list of Locations objects sorted after index of location in message
        SortedList<int, Location> listOfLocations = new SortedList<int, Location>();

        foreach (var loc in foundLocations)
        {
            Location location = new Location(loc.Value);
            listOfLocations.Add(loc.Key, location);
        }

        return listOfLocations;
    }

    // Check if a negation keyword is present TODO: get negation keywords from repo
    private void HandleNegationKeywords(string message, SortedList<int, string> foundLocations)
    {
        string negationKeyWord = "ikke ";
        if (message.Contains(negationKeyWord))
        {
            // Identify index of negation keyword
            int indexOfNegationWord = message.IndexOf(negationKeyWord, StringComparison.OrdinalIgnoreCase);


            var locationsToRemove = new List<int>();
            // Check if negation index is between two locations
            for (int i = 0; i < foundLocations.Count - 1; i++)
            {
                if (foundLocations.GetKeyAtIndex(i) <= indexOfNegationWord &&
                    indexOfNegationWord < foundLocations.GetKeyAtIndex(i + 1))
                {
                    // remove location after negation word
                    for (int j = indexOfNegationWord + 1; j < foundLocations.Last().Key + 1; j++)
                    {
                        if (foundLocations.ContainsKey(j))
                        {
                            if (_verbose)
                            {
                                Console.WriteLine(
                                    $"Negation Keyword found at index {j} - ignoring location: {foundLocations[j]}");
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
    }
}