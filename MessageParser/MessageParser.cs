
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
        int locationCount = 0;

        Console.WriteLine(message);
        // run through each word to identify one keyword belonging to each
        foreach (var locationWord in locationWordsDictionary)
        {
            //Console.WriteLine($"{locationWord.Key} keyword betyder {locationWord.Value}");
            if (message.Contains(locationWord.Key))
            {
                int indexOfWord = message.IndexOf(locationWord.Key, StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"Found: {locationWord.Key}   =>   location: {locationWord.Value}  =>  Index: {indexOfWord}");
                Location location = new Location();
                location.Place = locationWord.Value;
            }
        }

        //Console.WriteLine($"{message} - contains {locationCount} locations");
    }
}