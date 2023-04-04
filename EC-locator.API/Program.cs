using System.Collections;
using EC_locator.Core;
using EC_locator.Repositories;
using EC_locator.Core.Interfaces;
using Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
using Location = EC_locator.Core.Models.Location;

// TODO update interfaces
var builder = WebApplication.CreateBuilder(args);

// set global environment variables
initSettings();

// Add services to the container.
ConfigureLocatorServices(builder.Services);
ConfigureApiServices(builder.Services);

void ConfigureLocatorServices(IServiceCollection services)
{
    services.AddSingleton<IMessageParser, MessageParser>();
    services.AddSingleton<ITeamsRepository, TeamsRepository>();
    services.AddSingleton<ISettings, Settings>();
    
    // message parser services
    services.AddSingleton<ILocationTagger, LocationTagger>();
    services.AddSingleton<ITimeTagger, TimeTagger>();
    services.AddSingleton<ITimeAndLocationConnector, TimeAndLocationConnector>();
}

void ConfigureApiServices(IServiceCollection services)
{
    builder.Services.AddCors(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.AddPolicy(name: "CorsPolicy",
                policy =>
                {
                    policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                });
        }
        else
        {
            options.AddPolicy(name: "CorsPolicy",
                policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5174")
                        .WithMethods("GET", "POST", "OPTIONS");
                });
        }
    });
    
    
    services.AddControllers();
    //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

var app = builder.Build();

IMessageParser messageParser = app.Services.GetService<IMessageParser>();
ITeamsRepository tr = new TeamsRepository();
CalendarRepository cr = new CalendarRepository();
ILocatorRepository lr = new LocatorRepository();


/*
var test = lr.GetStopIndicatorKeywords();
tr.GetMessages("all", new DateOnly());
await TestGettingUsersFromTeamsRepo();
TestMessageParser();
TestTomorrow();
await tr.ListMessagesAsync();
await cr.GetCalendarEvents();
*/



// Configure the HTTP request pipeline.
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();





void TestTomorrow()
{
    string[] messages = tr.GetMessages("wip", new DateOnly());

    foreach (string message in messages)
    {
        Console.WriteLine($"\n{message}");
        Console.WriteLine($"Contains tomorrow {messageParser.ContainsTomorrow(message)}");
        /*
        var locations = messageParser.GetLocations(message);

        foreach (var location in locations)
        {
            Console.WriteLine(location);
        }
        */
    }

    Environment.Exit(1);
    
}

// TESTING MESSAGE PARSER
void TestMessageParser()
{
    string[] messages = tr.GetMessages("wip", new DateOnly());

    foreach (string message in messages)
    {
        Console.WriteLine($"\n{message}");
        

        var locations = messageParser.GetLocations(message);
        if (messageParser.ContainsTomorrow(message))
        {
            Console.WriteLine("Location(s) for tomorrow");
        }
        foreach (var location in locations)
        {
            Console.WriteLine(location);
        }
    }

    Environment.Exit(1);
}

// TESTING OF TEAMS REPO
async Task TestGettingUsersFromTeamsRepo()
{
    List<User> users = await tr.GetUsersAsync();
    foreach (User user in users)
    {
        Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
        Console.WriteLine($"  ID: {user.Id}");
        Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}"); 
    }
    Environment.Exit(1);
}

void initSettings()
{
    // AZURE SETTINGS
    
    Settings.GetInstance().ClientId = builder.Configuration.GetSection("AzureAd")["ClientId"];
    Settings.GetInstance().TenantId = builder.Configuration.GetSection("AzureAd")["TenantId"];
    Settings.GetInstance().ClientSecret = builder.Configuration.GetSection("AzureAd")["ClientSecret"];
    
    // TEAMS SETTINGS
    Settings.GetInstance().ChannelID = builder.Configuration.GetSection("TeamsChannel")["ChannelId"];
    Settings.GetInstance().TeamId = builder.Configuration.GetSection("TeamsChannel")["TeamId"];
    
    // VERBOSE SETTINGS
    Settings.GetInstance().Verbose = builder.Configuration.GetValue<bool>("Verbose");;
    
    // DEFAULT LOCATION AND TIME SETTINGS
    int hour;
    int minutes;
    
    // setting start time
    hour = builder.Configuration.GetSection("TimeDefaults").GetSection("WorkStart").GetValue<int>("Hour");
    minutes =  builder.Configuration.GetSection("TimeDefaults").GetSection("WorkStart").GetValue<int>("Minute");
    Settings.GetInstance().WorkStartDefault = new TimeOnly(hour, minutes);
    
    // setting end time
    hour = builder.Configuration.GetSection("TimeDefaults").GetSection("WorkEnd").GetValue<int>("Hour");
    minutes =  builder.Configuration.GetSection("TimeDefaults").GetSection("WorkEnd").GetValue<int>("Minute");
    Settings.GetInstance().WorkEndDefault = new TimeOnly(hour, minutes);
    
    // setting default Location
    Location location = new Location();
    location.Place = builder.Configuration.GetValue<string>("LocationDefault");
    Settings.GetInstance().DefaultLocation = location;
}
