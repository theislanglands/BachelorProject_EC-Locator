using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class TimeAndLocationConnector : ITimeAndLocationConnector
{
    private readonly ILocatorRepository _locatorRepository;
    private readonly bool _verbose;
    private readonly TimeOnly _workStartDefault;
    private readonly TimeOnly _workEndDefault;
    
    private SortedList<int, Location> _locationTags;
    private SortedList<int, TimeOnly> _timeTags;
    private string? _message;

    public TimeAndLocationConnector(ILocatorRepository locatorRepository, IOptions<VerboseOptions> settingsOptions, IOptions<DefaultLocationOptions> locationOptions)
    {
        _locatorRepository = locatorRepository;
        string[] workStart = locationOptions.Value.DefaultWorkStart.Split(':');
        string[] workEnd = locationOptions.Value.DefaultWorkEnd.Split(':');
        _workStartDefault = new TimeOnly(int.Parse(workStart[0]),int.Parse(workStart[1]));
        _workEndDefault = new TimeOnly(int.Parse(workEnd[0]), int.Parse(workEnd[1]));
        _verbose = settingsOptions.Value.Verbose;
    }

    public List<Location> AddTimeToLocations(SortedList<int, Location> locationTags, SortedList<int, TimeOnly> timeTags, string message)
    {
        _locationTags = locationTags;
        _timeTags = timeTags;
        _message = message;
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
        // if first location has a start keyword, assign first time tag as start, otherwise default
        if (locationIndex == 0)
        {
            if (HasStartIndicator())
            {
                _locationTags.Values[locationIndex].Start = _timeTags.Values[0];
                _locationTags.Values[locationIndex].End = _timeTags.Values.Count == 1 ? _workStartDefault : _timeTags.Values[1];
            }
            else
            {
                _locationTags.Values[locationIndex].Start = _workStartDefault;
            }
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
                if (HasStopIndicator())
                {
                    _locationTags.Values[locationIndex].End = _timeTags.Values[^1];
                }
                else
                {
                    _locationTags.Values[locationIndex].End = _workEndDefault;
                }
            }
            else
            {
                _locationTags.Values[locationIndex].End = _timeTags.Values[locationIndex];
            }
        }
    }

    private bool HasStartIndicator()
    {
        foreach (var startIndicator in _locatorRepository.GetStartIndicatorKeywords())
        {
            if (_message != null && _timeTags.Count > 0 && _message[.._timeTags.Keys[0]].Contains(startIndicator))
            {
                return (_locationTags.Count.Equals(_timeTags.Count));
            }
        }
        return false;
    }
    private bool HasStopIndicator()
    {
        foreach (var stopIndicator in _locatorRepository.GetStopIndicatorKeywords())
        {
            if (_message != null && _timeTags.Count > 0 && _message[.._timeTags.Keys[^1]].Contains(stopIndicator))
            {
                return true;
            }
        }
        return false;
    }
}