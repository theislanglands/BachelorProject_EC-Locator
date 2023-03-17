using EC_locator.Core.Models;
using Parser.Decisions;

namespace Parser;

public class DecisionTree : Decision
{
    /*
    //Decision 1
    var criminalBranch = new DecisionQuery
    {
        Title = "Have a criminal record",
        Test = (client) => client.CriminalRecord,
        Positive = new DecisionResult { Result = false },
        Negative = moneyBranch
    };

    //Decision 0
    var trunk = new DecisionQuery
        {
            Title = "Want a loan",
            Test = (client) => client.IsLoanNeeded,
            Positive = criminalBranch,
            Negative = new DecisionResult { Result = false }
        };

        return trunk;
    */


    public override void Evaluate(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        Console.WriteLine("in decision tree");
        
        // Decision 0
        var trunk = new DecisionQuery
        {
            Title = "Is Location Found",
            Test = (_locations, _times) =>
            {
                if (_locations.Count == 0)
                {
                    return false;
                }

                return true;
            },
            
            Positive = new FinalResult(),
            Negative = new FinalResult()
        };
        
        trunk.Evaluate(_locations, _times);
    }
}