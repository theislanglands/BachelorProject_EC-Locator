using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;

namespace Parser;

public class TimeAndLocationConnector : ITimeAndLocationConnector
{
    private static LocatorRepository? _locatorRepository;
    private readonly bool _verbose;
 
    private SortedList<int, Location>? _locationTags;
    private SortedList<int, TimeOnly>? _timeTags;
    private string _message;

    public TimeAndLocationConnector()
    {
        _verbose = Settings.GetInstance().Verbose;
        _locatorRepository = new LocatorRepository();
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
                _locationTags.Values[locationIndex].End = _timeTags.Values.Count == 1 ? Settings.GetInstance().WorkStartDefault : _timeTags.Values[1];
            }
            else
            {
                _locationTags.Values[locationIndex].Start = Settings.GetInstance().WorkStartDefault;
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
            // if last location has and end-indicater, set to last time, otherwise default
            if (locationIndex == _locationTags.Count - 1)
            {
                if (HasStopIndicator())
                {
                    _locationTags.Values[locationIndex].End = _timeTags.Values[^1];
                }
                else
                {
                    _locationTags.Values[locationIndex].End = Settings.GetInstance().WorkEndDefault;
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
            if (_timeTags.Count > 0 && _message[.._timeTags.Keys[0]].Contains(startIndicator))
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
            if (_timeTags.Count > 0 && _message[.._timeTags.Keys[^1]].Contains(stopIndicator))
            {
                return true;
            }
        }
        return false;
    }
}