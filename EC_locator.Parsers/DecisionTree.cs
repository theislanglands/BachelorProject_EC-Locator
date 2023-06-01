using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using EC_locator.Parsers.Decisions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EC_locator.Parsers;

public class DecisionTree
{
    private readonly IOptions<VerboseOptions> _options; 
    public DecisionTree(IOptions<VerboseOptions> settingsOptions)
    {
        _options = settingsOptions;
    }
    public void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        // BUILDING TREE AND CALLS RECURSIVELY - START AT BOTTOM OF METHOD!
        
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
            
            GoTo = new FinalNode(_options),
        };
        
        // Action 10
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
            
            GoTo = new FinalNode(_options)
        };
        
        // Decision 15
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
        
        // Decision 14
        var twoLocationsHigherThanTime = new DecisionQuery(_options)
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
            Negative = new FinalNode(_options)
        };
        
        // Action 8
        var insertHome = new Action(_options)
        {
            Title = "Inserting home at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Add(0, new Location("home"));
                return true;
            },
            
            GoTo = twoLocationsHigherThanTime,
        };
        
        // Decision 13
        var firstLocationNotHome = new DecisionQuery(_options)
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
            Positive = insertHome,
            Negative = twoLocationsHigherThanTime
        };
        
        // Action 9
        var insertOffice = new Action(_options)
        {
            Title = "Inserting office at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Add(0, new Location("office"));
                return true;
            },
            GoTo = twoLocationsHigherThanTime,
        };
        
        // Decision 12
        var isFirstLocationOffice = new DecisionQuery(_options)
        {
            Title = "Is the first location different from office",
            Test = (locations, times) =>
            {
                // is no time indication present and more than 1 location
                if (locations.Values[0].Place.Equals("office"))
                {
                    return false;
                }
                return true;
            },
            Negative = insertHome,
            Positive = insertOffice
        };
        
        // Decision 11
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
            Positive = firstLocationNotHome,
            Negative = twoLocationsHigherThanTime
        };
        
        // Decision 10
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
        
        // Action 7
        var insertMeeting= new Action(_options)
        {
            Title = "Inserting meeting at location 0",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("meeting"));
                return true;
            },
            
            GoTo = new FinalNode(_options)
        };
        
        // Decision 9
        var twoLocationsOneIsMeeting = new DecisionQuery(_options)
        {
            Title = "Does message contain two Locations and one of them is meeting",
            Test = (locations, times) =>
            {
                if (locations.Count != 2)
                {
                    return false;
                }

                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("meeting"))
                    {
                        return true;
                    }
                }
                return false;
            },
 
            Positive = insertMeeting,
            Negative = insertUndefined
        };
        
        // Decision 8
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
 
            Positive = twoLocationsOneIsMeeting,
            Negative = isFirstIndexTimeKeyword
        };
        
        // Action 6
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
        
        // Decision 7
        var isOffFollowedByTwoLocations = new DecisionQuery(_options)
        {
            Title = "Is an off-location followed by two locations before a time tag",
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
        
        // Action 5
        var deleteKids= new Action(_options)
        {
            Title = "Deleting kids-tag",
            PerformAction = (locations, times) =>
            {
                List<int> locationsToDelete = new();
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("kidsIll"))
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
        
        // Decision 6
        var moreThanOneLocation = new DecisionQuery(_options)
        {
            Title = "Are there more than one location",
            Test = (locations, times) =>
            {
                return locations.Count > 1;
            },
            
            Positive = deleteKids,
            Negative = insertUndefined
        };
        
        // Action 4
        var insertKidsIll = new Action(_options)
        {
            Title = "Inserting Kids Ill at location 0 and deleting other locations",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("KidsIll"));
                return true;
            },
            
            GoTo = new FinalNode(_options)
        };
        
        // Action 3
        var insertIll = new Action(_options)
        {
            Title = "Inserting Ill at location 0 and deleting other locations",
            PerformAction = (locations, times) =>
            {
                locations.Clear();
                locations.Add(0, new Location("ill"));
                return true;
            },
            
            GoTo = new FinalNode(_options)
        };
        
        // Decision 5
        var areKidsIll = new DecisionQuery(_options)
        {
            Title = "Is kids- and Ill keyword identified",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("kidsIll"))
                    {
                        return true;
                    }
                }

                return false;
            },
            
            Positive = insertKidsIll,
            Negative = insertIll
        };

        // Decision 4
        var isKidsPresent = new DecisionQuery(_options)
        {
            Title = "Is location kidsIll identified",
            Test = (locations, times) =>
            {
                foreach (var location in locations)
                {
                    if (location.Value.Place.Equals("kidsIll"))
                    {
                        return true;
                    }
                }

                return false;
            },

            Positive = moreThanOneLocation,
            Negative = isOffFollowedByTwoLocations
        };
            
        // Decision 3
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
            
            Positive = areKidsIll,
            Negative = isKidsPresent
        };
        
        // Action 2
        var insertHomeAndOffice = new Action(_options)
        {
            Title = "Inserting Office at last location",
            PerformAction = (locations, times) =>
            {
                var key = times.Keys.Last() + 1;
                locations.Add(0, new Location("home"));
                locations.Add(key, new Location("office"));
                return true;
            },
            
            GoTo = new FinalNode(_options)
        };
        
        // Decision 2
        var oneTimeNoLocations = new DecisionQuery(_options)
        {
            Title = "Is there one time and no locations",
            Test = (locations, times) =>
            {
                if (times.Count == 1 && locations.Count == 0) 
                {
                    return true;
                }

                return false;
            },
            
            Positive = insertHomeAndOffice,
            Negative = insertUndefined
        };

        // Decision 1
        var root = new DecisionQuery(_options)
        {
            Title = "Are locations found",
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
        
        root.Perform(locations, times);
    }
}