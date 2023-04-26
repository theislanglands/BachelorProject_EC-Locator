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
    private readonly string _connectionString;

    public LocatorRepository(IOptions<LocatorRepositoryOptions> databaseSettings, IOptions<VerboseOptions> verboseSettings)
    {
        _host = databaseSettings.Value.Host;
        _userId = databaseSettings.Value.UserId;
        _password = databaseSettings.Value.Password;
        _connectionString = databaseSettings.Value.ConnectionStringOW;
        _verbose = verboseSettings.Value.Verbose;
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

    public void TestConnection()
    {
        
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
            // ILL KEYWORDS
            {  "seng", "ill" }, {  "dynen" ,"ill" }, {  "på langs" ,"ill" }, {  "vandret" ,"ill" }, {  "lægger mig" ,"ill" },
            
            {  "syg" ,"ill" }, {  "ikke frisk" ,"ill" }, {  "ikke på toppen" ,"ill" }, {  "skidt" ,"ill" }, {  "helbred" ,"ill" },
            
            {  "influenza" ,"ill" }, {  "feber" ,"ill" }, {  "forkølelse" ,"ill" }, {  "svimmel" ,"ill" }, {  "kvalme" ,"ill" }, 
            {  "ondt i hovedet" ,"ill" }, {  "migræne" ,"ill" }, {  "toilet" ,"ill" },
            
            // KIDS ILL KEYWORDS
            {  "den lille" ,"kidsIll" }, {  "de små" ,"kidsIll" }, {  "familie" ,"kidsIll" },
            {  "børn" ,"kidsIll" }, {  "barn" ,"kidsIll" },
            {  "pige" ,"kidsIll" }, {  "dreng" ,"kidsIll" },
            {  "unger" ,"kidsIll" }, {  "søn" ,"kidsIll" }, {  "datter" ,"kidsIll" },
            {  "Felix" ,"kidsIll" },
            {  "Otto" ,"kidsIll" },
            
            // MEETING KEYWORDS
            { "møde", "meeting" },
            
            // HOME KEYWORDS
            { "hjem", "home" },
            { "ikke på kontoret", "home" },
            
            // OFFICE KEYWORDS
            { "kommer ind", "office" }, { "kommer jeg ind", "office" }, { "inde", "office" }, { "ind forbi", "office" },
            { "komme ind", "office" },
            
            { "retur", "office"},
            { "er tilbage", "office"},
            
            { "på kontoret", "office" },
            { "på arbejdet", "office" },
            { "kommer i firmaet", "office" },
            { "konnes", "office" }, 

            // OFF KEYWORDS
            { "fri", "off" },
            { "off", "off" },
            { "går fra", "off" },
            { "stopper", "off" },
            { "smutter", "off" },
            { "holder", "off" },
            { "lukker ned", "off" },
            { "starter", "off" },

            // REMOTE KEYWORDS
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
    
    
    private void OpenConnection()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        /*
        builder.DataSource = _host; 
        builder.UserID = _userId;            
        builder.Password = _password;     
        builder.InitialCatalog = "Keywords";
        builder.TrustServerCertificate = true;
        */
        builder.ConnectionString = _connectionString;
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