using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class TimeTagger : ITimeTagger
{
    private readonly bool _verbose;
    private readonly Dictionary<string, TimeOnly> _timeKeywords;
    private readonly Dictionary<string, double> _minuteIndicators;
    private SortedList<int, TimeOnly> identifiedTimes;

    public TimeTagger(ILocatorRepository locatorRepository, IOptions<VerboseOptions> settingsOptions)
    {
        _verbose = settingsOptions.Value.Verbose;
        _timeKeywords = locatorRepository.GetTimeKeywords();
        _minuteIndicators = locatorRepository.GetMinuteIndicators();
    }

    public SortedList<int, TimeOnly> GetTags(Message message)
    {
        identifiedTimes = IdentifyNumericTime(message.Content);
        
        foreach (var item in IdentifyKeywordsTime(message.Content))
        {
            identifiedTimes.Add(item.Key, item.Value);
        }
        
        ProneNonChronologicalTimeTags();


        if (_verbose)
        {
            foreach (var time in identifiedTimes)
            {
                Console.WriteLine($"time found {time.Value} at index {time.Key}");
            }
        }
        
        return identifiedTimes;
    }

    private void ProneNonChronologicalTimeTags()
    {
        // Prone time tags that are not chronological!
        if (identifiedTimes.Count != 0)
        {
            TimeOnly previousTime = new TimeOnly(0, 0);
            List<int> tagsToDelete = new List<int>();

            foreach (var timeTag in identifiedTimes)
            {
                if (timeTag.Value <= previousTime)
                {
                    tagsToDelete.Add(timeTag.Key);
                }

                previousTime = timeTag.Value;
            }

            foreach (var timeTag in tagsToDelete)
            {
                if (_verbose)
                {
                    Console.WriteLine("proning time tags, for non chronological tags");
                }

                identifiedTimes.Remove(timeTag);
            }
        }
    }

    private SortedList<int, TimeOnly> IdentifyKeywordsTime(string message)
    {
        var identifiedTimeOnIndex = new SortedList<int, TimeOnly>();

        foreach (var timeKeyword in _timeKeywords)
        {
            if (message.Contains(timeKeyword.Key))
            {
                var foundAtIndex = message.IndexOf(timeKeyword.Key, StringComparison.OrdinalIgnoreCase) + 1;
                identifiedTimeOnIndex.Add(foundAtIndex, timeKeyword.Value);
            }
        }

        return identifiedTimeOnIndex;
    }

    private SortedList<int, TimeOnly> IdentifyNumericTime(string message)
    {
        // index in string, identified time
        var timeOnIndex = new SortedList<int, TimeOnly>();

        int foundAtIndex = 0;
        string number = "";

        for (int i = 0; i < message.Length; i++)
        {
            if (char.IsDigit(message[i]))
            {
                number += message[i];
                foundAtIndex = i;
                while (i < message.Length - 1 && (char.IsDigit(message[i + 1]) || 
                                                  message[i + 1].Equals('.') ||
                                                  message[i + 1].Equals(' ') ||
                                                  message[i + 1].Equals(':')))
                {
                    i++;
                    if (message[i].Equals('.') || message[i].Equals(':')|| message[i].Equals(' '))
                    {
                        continue;
                    }

                    number += message[i];
                }
            }

            if (!number.Equals(""))
            {
                var foundTime = ParseToTimeOnly(number);

                if (foundTime != null)
                {
                    foundTime = AddMinuteIndication(message, foundAtIndex, (TimeOnly) foundTime);
                    timeOnIndex.Add(foundAtIndex + 1, (TimeOnly) foundTime);
                    number = "";
                }
            }
        }

        return timeOnIndex;
    }
    
    // check if a message contains a minute indicator e.g "quarter past" between start and found index
    private TimeOnly AddMinuteIndication(string message, int foundAtIndex, TimeOnly foundTime)
    {
        foreach (var minuteIndicator in _minuteIndicators)
        {
            if (message[..foundAtIndex].Contains(minuteIndicator.Key))
            {
                if (_verbose)
                {
                    Console.Write(
                        $"minute indicator \"{minuteIndicator.Key}\" found before time - {foundTime} - correcting time with {minuteIndicator.Value} minutes");
                }

                foundTime = foundTime.AddMinutes(minuteIndicator.Value);

                if (_verbose)
                {
                    Console.WriteLine($" - corrected time: {foundTime}");
                }
            }
        }

        return foundTime;
    }

    // Parses from string to TimeOnly object
    private TimeOnly? ParseToTimeOnly(string number) 
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

        int hrs = int.Parse(hour);
        int mnt = int.Parse(minutes);

        TimeOnly? returnTime = null;

        try
        {
            returnTime = new TimeOnly(hrs, mnt);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            if (_verbose)
            {
                Console.WriteLine($"not able to parse identified number {number} into a time");
            }
        }

        return returnTime;
    }
}