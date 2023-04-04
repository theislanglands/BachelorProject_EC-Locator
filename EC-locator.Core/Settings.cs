using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;

namespace EC_locator.Core
{
    


// singleton holding Azure settings
public sealed class Settings : ISettings
{
    // AZURE ACCESS
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }
    
    // MESSAGE ACCESS
    public string? TeamId { get; set; }
    public string? ChannelID { get; set; }

    // PRINT IN TERMINAL
    public bool Verbose { get; set; }
    
    // Default Values for location and times
    public TimeOnly WorkStartDefault { get; set; }
    public TimeOnly WorkEndDefault { get; set; }
    public Location DefaultLocation { get; set; }

    private static Settings _instance;
    
    public Settings()
    {

    }

    // Singleton instance
    public static Settings GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Settings();
        }
        
        return _instance;
    }

    Settings ISettings.GetInstance()
    {
        return GetInstance();
    }
}

}