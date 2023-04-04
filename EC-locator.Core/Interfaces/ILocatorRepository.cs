namespace EC_locator.Core.Interfaces;

public interface ILocatorRepository
{
    public List<string> GetStopIndicatorKeywords();
    public List<string> GetStartIndicatorKeywords();
    public Dictionary<string, double> GetMinuteIndicators();
    public Dictionary<string, TimeOnly> GetTimeKeywords();
    public Dictionary<string, string> GetLocationKeywords();
}