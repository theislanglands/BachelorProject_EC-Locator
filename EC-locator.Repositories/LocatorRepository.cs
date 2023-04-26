using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EC_locator.Repositories;

public class LocatorRepository : ILocatorRepository
{
    private readonly bool _verbose;
    private SqlConnection _connection;
    private readonly string _connectionString;

    public LocatorRepository(IOptions<LocatorRepositoryOptions> databaseSettings, IOptions<VerboseOptions> verboseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionStringOW;
        _verbose = verboseSettings.Value.Verbose;
    }
    
    public Dictionary<string, double> GetMinuteIndicators()
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
            SqlCommand cmd = new SqlCommand(sql, _connection);
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
    
    public Dictionary<string, TimeOnly> GetTimeKeywords()
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
            SqlCommand cmd = new SqlCommand(sql, _connection);
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
    
    public List<string> GetLocations()
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
            SqlCommand cmd = new SqlCommand(sql, _connection);
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
    
    public Dictionary<string, string> GetLocationKeywords()
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
            SqlCommand cmd = new SqlCommand(sql, _connection);
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
    
    private void OpenConnection()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.ConnectionString = _connectionString;
        _connection = new SqlConnection(builder.ConnectionString);
        
        try
        {
            if (_verbose)
            {
                Console.WriteLine("connecting to database");
            }

            _connection.Open();
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
            _connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing database: {ex.Message}");
        }
    }
}