using EC_locator.Core.Models;

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
        throw new NotImplementedException();
    }
}