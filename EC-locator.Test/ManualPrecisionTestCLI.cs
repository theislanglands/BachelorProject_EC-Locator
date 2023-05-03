using System.Text;
using EC_locator.Core.Interfaces;

namespace EC_locator.Test;

public class ManualPrecisionTestCLI
{
    private StringBuilder result;
    private DateOnly? _startDate;
    private DateOnly? _endDate;
    private ITeamsRepository tr;
    private IMessageParser _mp;

    public ManualPrecisionTestCLI(ITeamsRepository tr, IMessageParser mp)
    {
        this.tr = tr;
        _mp = mp;
        result = new();
    }

    public void RunTest()
    {
        Console.WriteLine("-- Test precision of prediction tool -- \n");

        int correctPredictions = 0;

            // setPeriod();
        _startDate = new DateOnly(2023, 1, 15);
        _endDate = new DateOnly(2023, 2, 1);


        result.AppendLine($"-- Result of message analysis from {_startDate} to {_endDate}  -- \n");
        // Fetch messages
        var messages = tr.FetchAllMessagesAsync((DateOnly) _startDate, (DateOnly) _endDate).Result;

        // Iterate through each one,
        foreach (var message in messages)
        {
            bool correct = false;
            result.AppendLine($"Message: {message.Content}");
            Console.WriteLine($"{message.Content}");
            if (message.Replies != null)
            {
                foreach (var reply in message.Replies)
                {
                    Console.WriteLine($"- {reply.Content}");
                    result.AppendLine($"Reply: - {reply.Content}");
                }
            }

            var locations = _mp.GetLocations(message);
            
            foreach (var location in locations)
            {
                Console.WriteLine(location);
                result.AppendLine(location.ToString());
            }

            
            while (true)
            {
                Console.WriteLine("Are locations identified correct? Y/N");
                var answer = Console.ReadLine();
                if (answer.ToLower().Equals("y"))
                {
                    correctPredictions++;
                    correct = true;
                    break;
                } else if (answer.ToLower().Equals("n"))
                {
                    correct = false;
                    break;
                }
                else
                {
                    Console.WriteLine("answer not accepted\n");
                }
            }
            
            result.AppendLine($"Correct prediction?: {correct}");
            result.AppendLine();
            Console.Clear();
        }

        result.AppendLine($"\n{correctPredictions} out of {messages.Count} predicted correct");
        result.AppendLine($"prediction precision of {(correctPredictions * 100)/ messages.Count} %");
        Console.Clear();
        Console.WriteLine("-- result -- ");
        
        Console.WriteLine(result.ToString());
        
    }

    private void setPeriod()
    {
        bool periodSet = false;
        while (!periodSet) 
        {
            while (_startDate == null)
            {
                Console.Write("\nSet start date (DD:MM:YR): ");
                string? start = Console.ReadLine();
                _startDate = parseToDate(start);
                if (_startDate == null)
                {
                    Console.WriteLine("\n- Wrong input");
                }
            }

            while (_endDate == null)
            {
                Console.Write("\nSet end date (DD:MM:YR): ");
                string? start = Console.ReadLine();
                _endDate = parseToDate(start);
                if (_endDate == null)
                {
                    Console.WriteLine("\n- Wrong input");
                }
            }
            
            if (_startDate <= _endDate)
            {
                periodSet = true;
            }
        }
    }

    private static DateOnly? parseToDate(string? start)
    {
        // verify input is usable
        if (start == null)
        {
            return null;
        }
        
        if (start.Length != 8 && start.Length != 6)
        {
            return null;
        }

        // REMOVING ":"
        if (start.Length == 8)
        {
            start = start.Replace(":", "");
        }

        if (start.Length != 6)
        {
            return null;
        }

        try
        {
            var day = int.Parse((start.Substring(0, 2)));
            var mth = int.Parse((start.Substring(2, 2)));
            var yr = 2000 + int.Parse((start.Substring(4, 2)));
            return new DateOnly(yr, mth, day);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine("fejl i parsing");
            Console.WriteLine(e);
            return null;
        }
    }
}