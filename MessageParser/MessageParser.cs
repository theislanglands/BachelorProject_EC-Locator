
using EClocator.Core.Interfaces;
using EC_locator.Repositories;

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
        string[,] locationWords = _locatorRepository.GetLocationKeywords();    
                                                                          
        // convert to lower case                                              
        message = message.ToLower();                                          
                                                                          
        // split into words                                                   
        string[] words = message.Split(' ');                                  
                                                                          
        int locationCount = 0;                                                   

        // run throug each category to identify one keyword beloning to each
        string category = "";
        foreach (var locationWord in locationWords)
        {
            if (locationWord == category) continue;
            
        }                                                                     
                                                                          
        Console.WriteLine($"{message} - contains {locationCount} locations");
    }                                                                         
}