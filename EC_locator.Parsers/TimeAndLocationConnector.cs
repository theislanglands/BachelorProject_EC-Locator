using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class TimeAndLocationConnector : ITimeAndLocationConnector
{
    private readonly bool _verbose;
    private readonly TimeOnly _workStartDefault;
    private readonly TimeOnly _workEndDefault;
    
    private SortedList<int, Location> _locationTags;
    private SortedList<int, TimeOnly> _timeTags;

    public TimeAndLocationConnector(IOptions<VerboseOptions> settingsOptions, IOptions<DefaultLocationOptions> locationOptions)
    {
        string[] workStart = locationOptions.Value.DefaultWorkStart.Split(':');
        string[] workEnd = locationOptions.Value.DefaultWorkEnd.Split(':');
        _workStartDefault = new TimeOnly(int.Parse(workStart[0]),int.Parse(workStart[1]));
        _workEndDefault = new TimeOnly(int.Parse(workEnd[0]), int.Parse(workEnd[1]));
        _verbose = settingsOptions.Value.Verbose;
    }

    public List<Location> AddTimeToLocations(SortedList<int, Location> locationTags, SortedList<int, TimeOnly> timeTags)
    {
        _locationTags = locationTags;
        _timeTags = timeTags;
        var locationsFound = new List<Location>();
        
        if (_verbose)
        {
            Console.WriteLine("Adding times to locations");
        }
        
        // adding times to location tags
        for (int locationIndex = 0; locationIndex < _locationTags.Count; locationIndex++)
        {
            SetStartTime(locationIndex);
            SetEndTime(locationIndex);
            locationsFound.Add(_locationTags.Values[locationIndex]);
        }

        return locationsFound;
    }
    
    private void SetStartTime(int locationIndex)
    {
        // if first location use default work start
        if (locationIndex == 0)
        {
            _locationTags.Values[locationIndex].Start = _workStartDefault;
        }
        // if not first location, set start time to end time of previous location
        else
        {
            _locationTags.Values[locationIndex].Start = _locationTags.Values[locationIndex-1].End;
        }
    }
    
    private void SetEndTime(int locationIndex)
    {
        // run if end time has not been set
        if (_locationTags.Values[locationIndex].End == null)
        {
            // if last location has and end-indicator, set to last time, otherwise default
            if (locationIndex == _locationTags.Count - 1)
            {
                _locationTags.Values[locationIndex].End = _workEndDefault;
            }
            else
            {
                _locationTags.Values[locationIndex].End = _timeTags.Values[locationIndex];
            }
        }
    }
}