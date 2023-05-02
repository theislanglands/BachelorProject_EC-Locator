using System.Text;

namespace EC_locator.Test;

public static class ManualPrecisionTestCLI
{
    private static StringBuilder result;
    private static DateOnly? startDate = null;
    private static DateOnly? endDate = null;

    
    public static void runTest()
    {
        Console.WriteLine("-- Test precision of prediction tool -- \n");
        
        // set period
        while (startDate == null)
        {
            Console.WriteLine("Set startdate (DD:MM:YR) ");
            string? start = Console.ReadLine();
            startDate = parseToDate(start);
            if (startDate == null)
            {
                Console.WriteLine("- Wrong input\n");
            }
        }

        // var theDate = new DateOnly(2015, 10, 21);

        Console.WriteLine(startDate);
        return;
        Console.WriteLine("Set enddate: ");
        string? end = Console.ReadLine();
        
        
        
        // Fetch messages
        // Iterate through each one,
        // -- use message parser,
        // -- present result
        // -- ask if correct
        // -- store result
        // present result
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
        catch (Exception e)
        {
            Console.WriteLine("fejl i parsing");
            Console.WriteLine(e);
            return null;
        }
    }
}