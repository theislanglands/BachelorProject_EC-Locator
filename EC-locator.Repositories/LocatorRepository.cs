using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EC_locator.Repositories;

public class LocatorRepository : ILocatorRepository
{
    private readonly bool _verbose;
    private SqlConnection connection;
    private readonly string _host, _userId, _password;

    public LocatorRepository(IOptions<LocatorRepositoryOptions> databaseSettings, IOptions<VerboseOptions> verboseSettings)
    {
        _host = databaseSettings.Value.Host;
        _userId = databaseSettings.Value.UserId;
        _password = databaseSettings.Value.Password;
        _verbose = verboseSettings.Value.Verbose;
    }
    
    public List<string> GetStopIndicatorKeywordsDB()
    {
        var keywords = new List<string>();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading Stop Indicator Keywords records");
            }
            string sql = "SELECT * FROM StopIndicatorKeywords";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                keywords.Add(
                    reader.GetString(0));
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read Stop Indicator Keywords: {ex.Message}");
        }
        
        CloseConnection();
        return keywords;
    }
    
    public List<string> GetStartIndicatorKeywordsDB()
    {
        var keywords = new List<string>();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading Start Indicator Keywords records");
            }
            string sql = "SELECT * FROM StartIndicatorKeywords";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                keywords.Add(
                    reader.GetString(0));
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read Start Indicator Keywords: {ex.Message}");
        }
        
        CloseConnection();
        return keywords;
    }
    
    public Dictionary<string, double> GetMinuteIndicatorsDB()
    {
        var keywords = new Dictionary<string, double>();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading Minute Indicator records");
            }
            string sql = "SELECT * FROM MinuteIndicatorKeywords";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                keywords.Add(
                    reader.GetString(0), 
                    reader.GetInt16(1));
            }
            reader.Close();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read Minute Indicator-keywords: {ex.Message}");
        }
        
        CloseConnection();
        return keywords;
    }
    
    public Dictionary<string, TimeOnly> GetTimeKeywordsDB()
    {
        Dictionary<string, TimeOnly> timeKeywords = new();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading TimeKeywords records");
            }
            string sql = "SELECT * FROM TimeKeywords";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                timeKeywords.Add(
                    reader.GetString(0), 
                    TimeOnly.Parse(reader.GetTimeSpan(1).ToString()));
            }
            reader.Close();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read time-keywords: {ex.Message}");
        }
        
        CloseConnection();
        return timeKeywords;
    }
    
    public List<string> getLocationsDB()
    {
        List<string> locations = new();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading location records");
            }
            string sql = "SELECT * FROM Location";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                locations.Add(reader.GetString(1));
            }
            reader.Close();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read records {ex.Message}");
        }
        
        CloseConnection();
        return locations;
    }
    
    public Dictionary<string, string> GetLocationKeywordsDB()
    {
        Dictionary<string, string> keywords = new Dictionary<string, string>();

        List<string> locations = new();
        OpenConnection();
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("reading location keywords");
            }
            
            string sql = "" +
                         "SELECT LocationKeywords.Keyword, Location.Name " +
                         "FROM Location " +
                         "INNER JOIN LocationKeywords " +
                         "ON Location.LocationID = LocationKeywords.Location";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                keywords.Add(reader.GetString(0), reader.GetString(1));
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to read location keywords: {ex.Message}");
        }
        
        CloseConnection();
        
        return keywords;
    }
    
    public Dictionary<string, string> GetLocationKeywords()
    {   
        string[,] keywords =
        {
            { "sengedag", "ill" },
            {  "ikke frisk" ,"ill" },
            {  "vandret" ,"ill" },
            {  "ikke på toppen" ,"ill" },
            {  "dynen" ,"ill" },
            {  "syg" ,"ill" },
            {  "influenza" ,"ill" },
            {  "ligger syg" ,"ill" },
            {  "lagt syg" ,"ill" },
            {  "feber" ,"ill" },
            {  "sygdom" ,"ill" },
            {  "forkølelse" ,"ill" },
            {  "svimmel" ,"ill" },
            {  "kvalme" ,"ill" },
            {  "ondt i hovedet" ,"ill" },
            {  "på langs" ,"ill" },
            {  "syge" ,"ill" },
            {  "helbred" ,"ill" },
            {  "feberbarn" ,"ill" },
            {  "lægger mig" ,"ill" },
            {  "skidt" ,"ill" },
            {  "under dynen" ,"ill" },
            {  "toilet" ,"ill" },
            {  "migræne" ,"ill" },
            
            {  "Otto" ,"KidsIll" },
            {  "den lille" ,"KidsIll" },
            {  "feberbørn" ,"KidsIll" },
            
            { "møde", "meeting" },
            
            { "hjem", "home" },
            { "hjemme", "home" },
            { "hjemmefra", "home" },
            { "på hjemmefra", "home" },
            { "tager den hjemmefra", "home" },
            { "tager jeg den hjemmefra", "home" },
            { "hjemmekontoret", "home" },
            { "hjemmeskansen", "home" },
            { "ikke på kontoret", "home" },
            { "kommer på kontoret", "home" },
            
            { "kommer ind", "office" },
            { "retur", "office"},
            { "er tilbage", "office"},
            { "på kontoret", "office" },
            { "inde", "office" },
            { "på arbejdet", "office" },
            { "ind forbi", "office" },
            { "er inde", "office" },
            { "er inde ved", "office" },
            { "kommer jeg ind", "office" },
            { "kommer i firmaet", "office" },
            { "konnes", "office" }, 

            { "holder fri", "off" },
            { "fri", "off" },
            { "holder fridag", "off" },
            { "fridag", "off" },
            { "holder weekend", "off" },
            { "off", "off" },
            { "går fra ved", "off" },
            { "stopper", "off" },
            { "holder", "off" },
            { "starter", "off" },

            { "tager ud til", "remote" },
            { "tager ned til", "remote" },
            { "hos", "remote" },
        };
        
        // replacing string[] with dictionary
        Dictionary<string, string> keywordsDic = new Dictionary<string, string>();
        for (int i = 0; i < keywords.GetLength(0); i++)
        {
            keywordsDic.Add(keywords[i,0], keywords[i,1]);
        }

        return keywordsDic;
    }
    
    public Dictionary<string, TimeOnly> GetTimeKeywords()
    {
        var timeKeywords = new Dictionary<string, TimeOnly>();
        
        timeKeywords.Add("formiddag", new TimeOnly(9,0));
        timeKeywords.Add("eftermiddag", new TimeOnly(12,0));
        timeKeywords.Add("frokost", new TimeOnly(11, 15));
        timeKeywords.Add("middag", new TimeOnly(12,00));
        timeKeywords.Add("aften", new TimeOnly(16,00));

        return timeKeywords;
    }
    
    public Dictionary<string, double> GetMinuteIndicators()
    {
        var timeKeywords = new Dictionary<string, double>();
        
        timeKeywords.Add("kvart over", 15);
        timeKeywords.Add("kvart i", -15);
        timeKeywords.Add("halv", -30);
        
        // "forsinket" - add minutes to current time
        
        return timeKeywords;
    }
    
    public List<string> GetStartIndicatorKeywords()
    {
        var timeKeywords = new List<string>();
        
        timeKeywords.Add("starter");

        return timeKeywords;
    }
    
    public List<string> GetStopIndicatorKeywords()
    {
        var timeKeywords = new List<string>();
        
        timeKeywords.Add("stopper");
        timeKeywords.Add("holder");
        
        return timeKeywords;
    }
    

    
    
    // NOT USED
    public string[] GetSplitterKeywords()
    {
        string[] keywords =
        {
            "fra kl",
            "indtil",
            "forventer",
            "i morgen",
            "kommer",
            "derefter",
            "er væk",
            "smutter",
            "kører",
        };
        return keywords;
    }
    
    private void OpenConnection()
    {
        // TODO get from .env
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = _host; 
        builder.UserID = _userId;            
        builder.Password = _password;     
        builder.InitialCatalog = "Keywords";
        builder.TrustServerCertificate = true;
        connection = new SqlConnection(builder.ConnectionString);
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("connecting to database");
            }

            connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to connect: {ex.Message}");
        }
    }

    private void CloseConnection()
    {
        try
        {
            if (_verbose)
            {
                Console.WriteLine("Closing database connection");
            }
            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing database: {ex.Message}");
        }
    }
}