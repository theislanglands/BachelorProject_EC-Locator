using System.Collections;
using EC_locator.Core;
using EC_locator.Repositories;
using EC_locator.Core.Interfaces;
using Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
using EC_locator.Core.Models;
using Location = EC_locator.Core.Models.Location;


// testing message parser
// TODO update interfaces


// initialize Settings singleton for global access to environment variables
var builder = WebApplication.CreateBuilder(args);

// Initializing global settings
Settings _settings = Settings.GetInstance();
initSettings();


MessageParser messageParser = new MessageParser();
TeamsRepository tr = new TeamsRepository();

TestMessageParser();

void TestMessageParser()
{
    string[] messages = tr.GetMessages("sample3", new DateOnly());

    foreach (string message in messages)
    {
        Console.WriteLine($"\n{message}");

        var locations = messageParser.GetLocations(message);

        foreach (var location in locations)
        {
            Console.WriteLine(location);
        }
    }

    Environment.Exit(1);
}




// TESTING OF TEAMS REPO

//await TestGettingUsersFromTeamsRepo();


async Task TestGettingUsersFromTeamsRepo()
{
    ArrayList users = await tr.GetUsersAsync();
    foreach (Microsoft.Graph.User user in users)
    {
        Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
        Console.WriteLine($"  ID: {user.Id}");
        Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}"); 
    }
    Environment.Exit(1);
}

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Problemer med API
await tr.ListMessagesAsync();
System.Environment.Exit(1);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



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
