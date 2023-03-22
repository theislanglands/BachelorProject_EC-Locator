namespace EC_locator.Core
{
    
}

// singleton holding Azure settings
public sealed class Settings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }
    
    public bool? Verbose { get; set; }
    
    // Singleton instance
    private static Settings _instance;
    
    public static Settings GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Settings();
        }
        
        return _instance;
    }

    public void initForAppAuth(string ClientId, string ClientSecret, string TenantId)
    {
        this.ClientId = ClientId;
        this.ClientSecret = ClientSecret;
        this.TenantId = TenantId;
    }
}