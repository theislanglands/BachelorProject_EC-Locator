using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;

namespace Parser;

public class TimeTagger
{
    private Settings _settings;

    private TimeOnly _workStartDefault;
    private TimeOnly _workEndDefault;
    private readonly Location _defaultLocation;
    
    private static LocatorRepository? _locatorRepository;
    private readonly bool _verbose;
    
    public TimeTagger()
    {
        _locatorRepository= new LocatorRepository();
        _settings = Settings.GetInstance();
        _workStartDefault = _settings.WorkStartDefault;
        _workEndDefault = _settings.WorkEndDefault;
        _defaultLocation = _settings.DefaultLocation;
        _verbose = _settings.Verbose;
    }
    
    public SortedList<int, TimeOnly> IdentifyKeywordsTime(string message)
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

    public SortedList<int, TimeOnly> IdentifyTimes(string message)
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