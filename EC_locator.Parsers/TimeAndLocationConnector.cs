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

    private List<Location> _locationsFound;
    private SortedList<int, Location> _locationTags;
    private SortedList<int, TimeOnly> _timeTags;

    public TimeAndLocationConnector(IOptions<VerboseOptions> settingsOptions,
        IOptions<DefaultLocationOptions> locationOptions)
    {
        string[] workStart = locationOptions.Value.DefaultWorkStart.Split(':');
        string[] workEnd = locationOptions.Value.DefaultWorkEnd.Split(':');
        _workStartDefault = new TimeOnly(int.Parse(workStart[0]), int.Parse(workStart[1]));
        _workEndDefault = new TimeOnly(int.Parse(workEnd[0]), int.Parse(workEnd[1]));
        _verbose = settingsOptions.Value.Verbose;
    }

    public List<Location> AddTimeToLocations(SortedList<int, Location> locationTags, SortedList<int, TimeOnly> timeTags)
    {
        _locationTags = locationTags;
        _timeTags = timeTags;
        _locationsFound = new List<Location>();

        if (_verbose)
        {
            Console.WriteLine("Adding times to locations");
        }

        // adding times to location tags
        for (int locationIndex = 0; locationIndex < _locationTags.Count; locationIndex++)
        {
            SetStartTime(locationIndex);
            SetEndTime(locationIndex);
            _locationsFound.Add(_locationTags.Values[locationIndex]);
        }
        
        RemoveNonConsecutiveLocations();
        
        if (_locationsFound.Count == 0)
        {
            _locationsFound.Add(new Location(_workStartDefault, _workEndDefault,"undefined"));
        }
        
        return _locationsFound;
    }

    private void RemoveNonConsecutiveLocations()
    {
        List<Location> locationToDelete = new();
        foreach (var location in _locationsFound)
        {
            if (location.Start >= location.End)
            {
                locationToDelete.Add(location);
            }
        }

        if (_verbose && locationToDelete.Count != 0)
        {
            Console.WriteLine("- Deleting non-consecutive locations");
        }

        foreach (var location in locationToDelete)
        {
            _locationsFound.Remove(location);
        }
    }

    private void SetStartTime(int locationIndex)
    {
        
        // if first location use default work start otherwise end time of previous locations
        if (locationIndex == 0)
        {
            _locationTags.Values[locationIndex].Start = _workStartDefault;
        }
        else
        {
            _locationTags.Values[locationIndex].Start = _locationTags.Values[locationIndex - 1].End;
        }
    }

    private void SetEndTime(int locationIndex)
    {
        // set to endtime to start of next locations, if last the default
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