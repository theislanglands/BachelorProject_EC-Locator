using EC_locator.Core;
using EC_locator.Core.Models;
using EC_locator.Repositories;

namespace Parser;

public class TimeAndLocationConnector
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

        for (int i = 0; i < _locationTags.Count; i++)
        {
            if (i == 0)
            {
                // if first location haven't already a start time assigned => assign default
                _locationTags.Values[i].Start ??= Settings.GetInstance().WorkStartDefault;

                // check if first location has a start keyword
                if (HasStartIndicator())
                {
                    _locationTags.Values[i].Start = _timeTags.Values[0];
                    _locationTags.Values[i].End = _timeTags.Values[1];
                }
            }
            else
            {
                _locationTags.Values[i].Start = locationsFound[^1].End;
            }

            // SETTING END TIMES
            if (_locationTags.Values[i].End == null)
            {
                if (i == _locationTags.Count - 1)
                {
                    if (HasStopIndicator())
                    {
                        _locationTags.Values[i].End = _timeTags.Values[^1];
                    }
                    else
                    {
                        _locationTags.Values[i].End = Settings.GetInstance().WorkEndDefault;
                    }
                }
                else
                {
                    _locationTags.Values[i].End = _timeTags.Values[i];
                }
            }
            locationsFound.Add(_locationTags.Values[i]);
        }

        return locationsFound;
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
}