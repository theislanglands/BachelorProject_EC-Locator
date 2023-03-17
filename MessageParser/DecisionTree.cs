using EC_locator.Core.Models;
using Microsoft.IdentityModel.Tokens;
using Parser.Decisions;

namespace Parser;

public class DecisionTree : Node
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


    public override void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        Console.WriteLine("in decision tree");
        
     
        
        
        // Action 1
        var insertUndefined = new Action
        {
            Title = "Inserting Undefined at location 0",
            PerformAction = (_locations, _times) =>
            {
                Console.WriteLine("in action");
                _locations.Add(0, new Location("undefined"));
                Console.WriteLine(_locations[0]);
                return true;
            },
            
            GoTo = new FinalResult(),
        };
        
        // Decision 2
        var noTimesAndMultipleLocations = new DecisionQuery
        {
            Title = "Does message contain multiple Locations and no Times?",
            Test = (_locations, _times) =>
            {
                // is no time indication present and more than 1 location
                if (_times.IsNullOrEmpty())
                {
                    if (_locations.Count > 1)
                    {
                        return true;
                    }
                }

                return false;
            },
 
            Positive = insertUndefined,
            Negative = new FinalResult()
        };
        
        
        // Action 2
        var insertIll = new Action
        {
            Title = "Inserting Ill at location 0 and deleting other locations",
            PerformAction = (_locations, _times) =>
            {
                Console.WriteLine("in action query");
                _locations.Clear();
                _locations.Add(0, new Location("ill"));
                return true;
            },
            
            GoTo = noTimesAndMultipleLocations
        };
        
        // Decision 1
        var isIll = new DecisionQuery
        {
            Title = "Is location Ill identified",
            Test = (_locations, _times) =>
            {
                foreach (var location in _locations)
                {
                    if (location.Value.Place.Equals("ill"))
                    {
                        return true;
                    }
                }

                return false;
            },
            
            Positive = insertIll,
            Negative = new FinalResult()
        };
        
        
        
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
            
            Positive = isIll,
            Negative = insertUndefined
        };
        
        trunk.Perform(_locations, _times);
    }
}