using EC_locator.Core.Models;
using Microsoft.IdentityModel.Tokens;
using Parser.Decisions;

namespace Parser;

public class DecisionTree : Node
{
    public override void Perform(SortedList<int, Location> _locations, SortedList<int, TimeOnly> _times)
    {
        // Action 1
        var insertUndefined = new Action
        {
            Title = "Inserting Undefined at location 0",
            PerformAction = (_locations, _times) =>
            {
                _locations.Clear();
                _locations.Add(0, new Location("undefined"));
                return true;
            },
            
            GoTo = new FinalResult(),
        };
        
        // Action 5
        var deleteLocationNotMeeting = new Action
        {
            Title = "Deleting location not a meeting",
            PerformAction = (_locations, _times) =>
            {
                int locationToRemove = -1;
                foreach (var location in _locations)
                {
                    // identifying to consecutive locations
                    for (int i = 0; i < _times.Count; i++)
                    {
                        if (_locations.Keys[i] < _times.Keys[i] && _locations.Keys[i + 1] < _times.Keys[i])
                        {
                            // check if one af the locations is a meeting and remove the one that's not.
                            for (int j = i; j < i + 2; j++)
                            {
                                if (!_locations.Values[j].Place.Equals("meeting"))
                                {
                                    _locations.Remove(_locations.Keys[j]);
                                    return true;
                                }
                            }
                        }
                    }
                    /*
                    if (locationToRemove == -1)
                    {
                        _locations.Clear();
                        _locations.Add(0, new Location("undefined"));
                    }
                    
                    {
                        //Console.WriteLine("moving location not a meeting");
                        _locations.Remove(_locations.Keys[locationToRemove]);
                    }
                    */
                }
                return true;
            },
            
            GoTo = new FinalResult()
            
            
        };
        
        
        // Decision 8
        var isMeetingPresent = new DecisionQuery
        {
            Title = "Is meeting present as one of the additional locations",
            Test = (_locations, _times) =>
            {
                foreach (var location in _locations)
                {
                    if (location.Value.Place.Equals("meeting"))
                    {
                        for (int i = 0; i < _times.Count; i++)
                        {
                            // identifying to consecutive locations
                            if (_locations.Keys[i] < _times.Keys[i] && _locations.Keys[i + 1] < _times.Keys[i])
                            {
                                // either location i or i+1 is a meeting - remove the one that's not.
                                for (int j = i; j < i + 2; j++)
                                {
                                    if (!_locations.Values[j].Place.Equals("meeting"))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                
                return false;
            },
 
            Positive = deleteLocationNotMeeting,
            Negative = insertUndefined
        };
        
        // Decision 7
        var isLocationCountTwoHigherThanTime = new DecisionQuery
        {
            Title = "Is location count two higher than time count",
            Test = (_locations, _times) =>
            {
                if (_locations.Count - 1 > _times.Count)
                {
                    return true;
                }

                return false;
            },
 
            Positive = isMeetingPresent,
            Negative = new FinalResult()
        };
        
        // Action 5
        var insertHomeA5= new Action
        {
            Title = "Inserting home at location 0",
            PerformAction = (_locations, _times) =>
            {
                _locations.Add(0, new Location("home"));
                return true;
            },
            
            GoTo = isLocationCountTwoHigherThanTime
        };
        
        
        // Decision 6
        var isFirstLocationNotHome = new DecisionQuery
        {
            Title = "Is first location different from home",
            Test = (_locations, _times) =>
            {
                if (!_locations.Values[0].Place.Equals("home"))
                {
                    return true;
                }

                return false;
            },
 
            Positive = insertHomeA5,
            Negative = isLocationCountTwoHigherThanTime
        };
        
        // Decision 5
        var oneLocationOneTime = new DecisionQuery
        {
            Title = "Is there one location and one time",
            Test = (_locations, _times) =>
            {
                // is no time indication present and more than 1 location
                if (_locations.Count == 1 && _times.Count == 1)
                {
                    return true;
                }

                return false;
            },
 
            Positive = isFirstLocationNotHome,
            Negative = isLocationCountTwoHigherThanTime
        };
        
        // Action 4
        var insertHome = new Action
        {
            Title = "Inserting home at location 0",
            PerformAction = (_locations, _times) =>
            {
                _locations.Add(0, new Location("home"));
                return true;
            },
            
            GoTo = oneLocationOneTime,
        };
        
        // Action 3
        var insertOffice = new Action
        {
            Title = "Inserting office at location 0",
            PerformAction = (_locations, _times) =>
            {
                _locations.Add(0, new Location("office"));
                return true;
            },
            
            GoTo = oneLocationOneTime,
        };
        
        // Decision 4
        var isFirstLocationOffice = new DecisionQuery
        {
            Title = "Is the first location=office",
            Test = (_locations, _times) =>
            {
                // is no time indication present and more than 1 location
                if (_locations.Values[0].Place.Equals("office"))
                {
                    return true;
                }

                return false;
            },
 
            Positive = insertHome,
            Negative = insertOffice
        };
        
        
        // Decision 3
        var isFirstIndexTimeKeyword = new DecisionQuery
        {
            Title = "Is the first index found a time keyword",
            Test = (_locations, _times) =>
            {
                // is no time indication present and more than 1 location
                if (_times.Count > 0 && _locations.Keys[0] > _times.Keys[0])
                {
                    return true;
                }

                return false;
            },
 
            Positive = isFirstLocationOffice,
            Negative = oneLocationOneTime
        };
        
        // Decision 2
        var noTimesAndMultipleLocations = new DecisionQuery
        {
            Title = "Does message contain multiple Locations and no Times",
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
            Negative = isFirstIndexTimeKeyword
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
            Negative = noTimesAndMultipleLocations
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