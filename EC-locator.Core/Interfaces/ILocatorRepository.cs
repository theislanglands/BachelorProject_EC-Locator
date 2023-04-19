namespace EC_locator.Core.Interfaces;

public interface ILocatorRepository
{
    public Dictionary<string, double> GetMinuteIndicators();
    public Dictionary<string, TimeOnly> GetTimeKeywords();
    public Dictionary<string, string> GetLocationKeywords();

    Dictionary<string, string> GetLocationKeywordsDB();
    Dictionary<string, double> GetMinuteIndicatorsDB();
    Dictionary<string, TimeOnly> GetTimeKeywordsDB();
    
}