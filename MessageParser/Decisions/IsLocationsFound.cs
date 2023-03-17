using EC_locator.Core.Models;

namespace Parser.Decisions;

public class IsLocationsFound : DecisionQuery
{
    public IsLocationsFound()
    {
        Title = "Is Locations Found";
        Positive = new FinalResult();
        Negative = new FinalResult();
        Test = Func;
    }

    private bool Func(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        Console.WriteLine("test in is location found");
        return true;
        if (locations.Count == 0)
        {
            //if (verbose)
            {
                Console.WriteLine("unable to decide - no locations identified - adding undefined at 0");
            }
            
            locations.Add(0, new Location("undefined"));
        }
        return true;
    }
}
