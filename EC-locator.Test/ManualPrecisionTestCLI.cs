using System.Text;
using EC_locator.Core.Interfaces;

namespace EC_locator.Test;

public class ManualPrecisionTestCLI
{
    private StringBuilder result;
    private DateOnly? _startDate;
    private DateOnly? _endDate;
    private ITeamsRepository tr;

    public ManualPrecisionTestCLI(ITeamsRepository tr)
    {
        this.tr = tr;
    }

    public void RunTest()
    {
        Console.WriteLine("-- Test precision of prediction tool -- \n");
        
        // setPeriod();
        _startDate = new DateOnly(2023, 4, 5);
        _endDate = new DateOnly(2023, 4, 15);
        Console.WriteLine(_startDate);
        Console.WriteLine(_endDate);
        
        // Fetch messages
        var messages = tr.FetchAllMessagesAsync((DateOnly) _startDate, (DateOnly) _endDate).Result;

        // Iterate through each one,
        foreach (var message in messages)
        {
            Console.WriteLine(message);
        }
        

        
        
        
        // -- use message parser,
        // -- present result
        // -- ask if correct
        // -- store result
        // present result
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