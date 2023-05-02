using System.Collections;
using EC_locator.Core;
using EC_locator.Repositories;
using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Parsers;
using Location = EC_locator.Core.Models.Location;
using EC_locator.Core.SettingsOptions;
using EC_locator.Locator;
using EC_locator.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Message = EC_locator.Core.Models.Message;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureSettingsOptions(builder.Services);
ConfigureLocatorServices(builder.Services);
ConfigureApiServices(builder.Services);

var app = builder.Build();

ManualPrecisionTestCLI mpt = new(app.Services.GetService<ITeamsRepository>(), app.Services.GetService<IMessageParser>());
mpt.RunTest();
Environment.Exit(1);

var messageParser = app.Services.GetService<IMessageParser>();
var tr = app.Services.GetService<ITeamsRepository>();
var cr = app.Services.GetService<ICalendarRepository>();
var lr = app.Services.GetService<ILocatorRepository>();
var el = app.Services.GetService<IEmployeeLocator>();


var AndersId = "2cf3e351-6ca8-4fda-999c-14a8b048b899";
var BrianId = "2d3cfcdf-542d-43f5-a4b1-6f58387604eb";
var TheisId = "6e5ee9cb-11cb-405d-aaa8-60c3768340c3";
var RuneId = "5907407f-ca28-4ff6-92d7-4b05d64a017c";



/*
el.GetCurrentLocation(TheisId);

*/

//TestRetrivingCalendarEvents();
// TestMessageParser();

// await TestGettingUsersFromTeamsRepo();
// TestTomorrow();
// await tr.ListMessagesAsync();

/*
var messages = await tr.GetMessagesAsync(RuneId);
foreach (var message in messages)
{
    Console.WriteLine(message);
}


*/
// await cr.GetCalendarEvents();




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


// configurations for app settings
void ConfigureSettingsOptions(IServiceCollection serviceCollection)
{
    serviceCollection.Configure<VerboseOptions>(builder.Configuration);
    serviceCollection.Configure<GraphHelperOptions>(builder.Configuration.GetSection("AzureAd"));
    serviceCollection.Configure<LocatorRepositoryOptions>(builder.Configuration.GetSection("MSSQL"));
    serviceCollection.Configure<DefaultLocationOptions>(builder.Configuration.GetSection("DefaultLocation"));
    serviceCollection.Configure<TeamsOptions>(builder.Configuration.GetSection("TeamsChannel"));
    serviceCollection.Configure<UsersOptions>(builder.Configuration);
}

// Application Services
void ConfigureLocatorServices(IServiceCollection services)
{
    services.AddSingleton<IMessageParser, MessageParser>();
    services.AddSingleton<ITeamsRepository, TeamsRepository>();
    services.AddSingleton<ICalendarRepository, CalendarRepository>();
    services.AddSingleton<IEmployeeLocator, EmployeeLocator>();
    
    if (builder.Configuration.GetValue<bool>("UseDatabase"))
    {
        services.AddSingleton<ILocatorRepository, LocatorRepository>();
    } 
    else 
    {
        services.AddSingleton<ILocatorRepository, LocatorRepositoryLocal>();
    }
    
    // message parser services
    services.AddSingleton<ILocationTagger, LocationTagger>();
    services.AddSingleton<ITimeTagger, TimeTagger>();
    services.AddSingleton<ITimeAndLocationConnector, TimeAndLocationConnector>();
    
    // repository services
    services.AddSingleton<IGraphHelper, GraphHelper>();
}

// Api configuration
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

void TestTomorrow()
{
    string[] messages = tr.GetSamples("wip");

    foreach (string message in messages)
    {
        Console.WriteLine($"\n{message}");
        Console.WriteLine($"Contains tomorrow {messageParser.ContainsTomorrow(message)}");
    }

    Environment.Exit(1);
}

// TESTING MESSAGE PARSER
void TestMessageParser()
{
    string[] messages = tr.GetSamples("wip");

    foreach (string message in messages)
    {
        Console.WriteLine($"\n{message}");
        Message msg = new Message()
        {
            Content = message,
            TimeStamp = DateTime.Now,
            UserId = "a user",
            Replies = null
        };
        
        var locations = messageParser.GetLocations(msg);
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

void TestRetrivingCalendarEvents()
{
    var AndersId = "2cf3e351-6ca8-4fda-999c-14a8b048b899";
    var BrianId = "2d3cfcdf-542d-43f5-a4b1-6f58387604eb";
    var TheisId = "6e5ee9cb-11cb-405d-aaa8-60c3768340c3";

    var evts = cr.GetCurrentCalendarEventsAsync(TheisId).Result;

    if (evts == null)
    {
        Console.WriteLine("no events found");
    }
    else {
        foreach (var ce in evts)
        {
            Console.WriteLine($"{ce}");
        }
    }
    Environment.Exit(1);
}
