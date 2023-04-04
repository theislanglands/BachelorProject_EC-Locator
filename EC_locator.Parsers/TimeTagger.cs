using EC_locator.Core;
using EC_locator.Core.Interfaces;
using EC_locator.Repositories;

namespace EC_locator.Parsers;

public class TimeTagger : ITimeTagger
{
    private readonly ILocatorRepository _locatorRepository;
    private readonly bool _verbose;

    public TimeTagger(ILocatorRepository locatorRepository)
    {
        var settings = Settings.GetInstance();
        _locatorRepository = locatorRepository;
        _verbose = settings.Verbose;
    }

    public SortedList<int, TimeOnly> GetTags(string message)
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
                                                  message[i + 1].Equals(':')))
                {
                    i++;
                    if (message[i].Equals('.') || message[i].Equals(':'))
                    {
                        continue;
                    }

                    number += message[i];
                }
            }

            if (!number.Equals(""))
            {
                TimeOnly foundTime = ParseToTimeOnly(number);
                foundTime = AddMinuteIndication(message, foundAtIndex, foundTime);
                timeOnIndex.Add(foundAtIndex, foundTime);
                number = "";
            }
        }

        return timeOnIndex;
    }
    
    // see if a message contains a minute indicator e.g "quarter past" between start and found index
    private TimeOnly AddMinuteIndication(string message, int foundAtIndex, TimeOnly foundTime)
    {
        var minuteIndicators = _locatorRepository.GetMinuteIndicators();

        foreach (var minuteIndicator in minuteIndicators)
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