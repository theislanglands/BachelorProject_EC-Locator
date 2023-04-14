using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using EC_locator.Parsers.Decisions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EC_locator.Parsers;

public class DecisionTree : Node
{
    private readonly IOptions<VerboseOptions> _options; 
    public DecisionTree(IOptions<VerboseOptions> settingsOptions) : base(settingsOptions)
    {
        _options = settingsOptions;
    }
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        // Action 1
        var insertUndefined = new Action(_options)
        {
            Title = "Inserting Undefined at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("undefined"));
                return true;
            },
            
            GoTo = new FinalResult(_options),
        };
        
        var deleteLocationNotMeeting = new Action(_options)
        {
            Title = "Deleting location not a meeting",
            PerformAction = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    // identifying to consecutive locations
                    for (int i = 0; i < times.Count; i++)
                    {
                        if (locations.Keys[i] < times.Keys[i] && locations.Keys[i + 1] < times.Keys[i])
                        {
                            // check if one af the locations is a meeting and remove the one that's not.
                            for (int j = i; j < i + 2; j++)
                            {
                                if (!locations.Values[j].Place.Equals("meeting"))
                                {
                                    locations.Remove(locations.Keys[j]);
                                    return true;
                                }
                            }
                        }
                    }
                }
                return true;
            },
            
            GoTo = new FinalResult(_options)
        };
        
        
        // Decision 8
        var isMeetingPresent = new DecisionQuery(_options)
        {
            Title = "Is meeting present as one of the additional locations",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("meeting"))
                    {
                        for (int i = 0; i < times.Count; i++)
                        {
                            // identifying to consecutive locations
                            if (locations.Keys[i] < times.Keys[i] && locations.Keys[i + 1] < times.Keys[i])
                            {
                                // either location i or i+1 is a meeting - remove the one that's not.
                                for (int j = i; j < i + 2; j++)
                                {
                                    if (!locations.Values[j].Place.Equals("meeting"))
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
        var isLocationCountTwoHigherThanTime = new DecisionQuery(_options)
        {
            Title = "Is location count two higher than time count",
            Test = (locations, times) =>
            {
                if (locations.Count - 1 > times.Count)
                {
                    return true;
                }

                return false;
            },
 
            Positive = isMeetingPresent,
            Negative = new FinalResult(_options)
        };
        
        // Action 5
        var insertHomeA5= new Action(_options)
        {
            Title = "Inserting home at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Add(0, new Location("home"));
                return true;
            },
            
            GoTo = isLocationCountTwoHigherThanTime
        };
        
        
        // Decision 6
        var isFirstLocationNotHome = new DecisionQuery(_options)
        {
            Title = "Is first location different from home",
            Test = (locations, times) =>
            {
                if (!locations.Values[0].Place.Equals("home"))
                {
                    return true;
                }

                return false;
            },
 
            Positive = insertHomeA5,
            Negative = isLocationCountTwoHigherThanTime
        };
        
        // Decision 5
        var oneLocationOneTime = new DecisionQuery(_options)
        {
            Title = "Is there one location and one time",
            Test = (locations, times) =>
            {
                if (locations.Count == 1 && times.Count == 1)
                {
                    return true;
                }

                return false;
            },
 
            Positive = isFirstLocationNotHome,
            Negative = isLocationCountTwoHigherThanTime
        };
        
        // Action 4
        var insertHome = new Action(_options)
        {
            Title = "Inserting home at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Add(0, new Location("home"));
                return true;
            },
            
            GoTo = oneLocationOneTime,
        };
        
        // Action 3
        var insertOffice = new Action(_options)
        {
            Title = "Inserting office at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Add(0, new Location("office"));
                return true;
            },
            
            GoTo = oneLocationOneTime,
        };
        
        // Decision 4
        var isFirstLocationOffice = new DecisionQuery(_options)
        {
            Title = "Is the first location=office",
            Test = (locations, times) =>
            {
                // is no time indication present and more than 1 location
                if (locations.Values[0].Place.Equals("office"))
                {
                    return true;
                }

                return false;
            },
 
            Positive = insertHome,
            Negative = insertOffice
        };
        
        
        // Decision 3
        var isFirstIndexTimeKeyword = new DecisionQuery(_options)
        {
            Title = "Is the first index found a time keyword",
            Test = (locations, times) =>
            {
                // is no time indication present and more than 1 location
                if (times.Count > 0 && locations.Keys[0] > times.Keys[0])
                {
                    return true;
                }

                return false;
            },
 
            Positive = isFirstLocationOffice,
            Negative = oneLocationOneTime
        };
        
        // Decision 3
        var noTimesAndMultipleLocations = new DecisionQuery(_options)
        {
            Title = "Does message contain multiple Locations and no Times",
            Test = (locations, times) =>
            {
                // is no time indication present and more than 1 location
                if (times.IsNullOrEmpty())
                {
                    if (locations.Count > 1)
                    {
                        return true;
                    }
                }

                return false;
            },
 
            Positive = insertUndefined,
            Negative = isFirstIndexTimeKeyword
        };
        
        // Action 1a
        var deleteOffLocation = new Action(_options)
        {
            Title = "Deleting off-location",
            PerformAction = (locations, times) =>
            {
                int locationToDelete = -1;
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("off"))
                    {
                        locationToDelete = location.Key;
                        break;
                    }
                }

                if (locationToDelete != -1)
                {
                    locations.Remove(locationToDelete);
                }

                return true;
            },
            
            GoTo = noTimesAndMultipleLocations
        };
        
        // Decision 2a
        var isOffFollowedByTwoLocations = new DecisionQuery(_options)
        {
            Title = "Is an off-location followed by two locations before times-tag",
            Test = (locations, times) =>
            {
                // Is a location place "off"?
                for (int index = 0; index < locations.Count; index++)
                {
                    if (!locations.Values[index].Place.Equals("off")) continue;
                    
                    // does "off" has two sucessors?
                    if (index >= locations.Count - 2) continue;
                    
                    foreach (var timeTag in times)
                    {
                        // is there a time tag between the two locations
                        if (locations.Keys[index + 1] < timeTag.Key && timeTag.Key < locations.Keys[index + 2])
                        {
                            return false;
                        }
                    }

                    return true;
                }
                return false;
            },
 
            Positive = deleteOffLocation,
            Negative = noTimesAndMultipleLocations
        };
        
        
        
        // Action 2
        var insertIll = new Action(_options)
        {
            Title = "Inserting Ill at location 0 and deleting other locations",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("ill"));
                return true;
            },
            
            GoTo = new FinalResult(_options)
        };
        
        // Action 2b
        var insertKidsIll = new Action(_options)
        {
            Title = "Inserting Kids Ill at location 0 and deleting other locations",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("KidsIll"));
                return true;
            },
            
            GoTo = new FinalResult(_options)
        };
        
        // Action 2a
        var insertOfficeAtEnd = new Action(_options)
        {
            Title = "Inserting Office at last location",
            PerformAction = (locations, times) =>
            {
                var key = times.Keys.Last() + 1;
                locations.Add(key, new Location("office"));
                return true;
            },
            
            GoTo = insertHomeA5
        };
        
        // Action 2c
        var deleteKids= new Action(_options)
        {
            Title = "Deleting kids-tag",
            PerformAction = (locations, times) =>
            {
                List<int> locationsToDelete = new();
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("KidsIll"))
                    {
                        locationsToDelete.Add(location.Key);
                    }
                }

                if (locationsToDelete.Count != 0)
                {
                    foreach (var location in locationsToDelete)
                    {
                        locations.Remove(location);
                    }
                }

                return true;
            },
            
            GoTo = isOffFollowedByTwoLocations
        };
        
        // Decision 2a2
        var isKidsIll = new DecisionQuery(_options)
        {
            Title = "Is kids- and Ill keyword identified",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("KidsIll"))
                    {
                        return true;
                    }
                }

                return false;
            },
            
            Positive = insertKidsIll,
            Negative = insertIll
        };
        
        var isKidsPresent = new DecisionQuery(_options)
        {
            Title = "Is kids-keyword identified",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("KidsIll"))
                    {
                        return true;
                    }
                }

                return false;
            },
            
            Positive = deleteKids,
            Negative = isOffFollowedByTwoLocations
        };
        
        
        // Decision 2
        var isIll = new DecisionQuery(_options)
        {
            Title = "Is location Ill identified",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("ill"))
                    {
                        return true;
                    }
                }

                return false;
            },
            
            Positive = isKidsIll,
            Negative = isKidsPresent
        };
        
        // Decision 1a
        var oneTimeNoLocations = new DecisionQuery(_options)
        {
            Title = "is there one time and no locations",
            Test = (locations, times) =>
            {
                if (times.Count == 1 && locations.Count == 0) 
                {
                    return true;
                }

                return false;
            },
            
            Positive = insertOfficeAtEnd,
            Negative = insertUndefined
        };
        
        

        // Decision 1
        var trunk = new DecisionQuery(_options)
        {
            Title = "Is Location Found",
            Test = (locations, times) =>
            {
                if (locations.Count == 0)
                {
                    return false;
                }

                return true;
            },
            
            Positive = isIll,
            Negative = oneTimeNoLocations
        };
        
        trunk.Perform(locations, times);
    }
}