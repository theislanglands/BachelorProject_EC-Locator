using System.Reflection.Metadata;
using API;
using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using EC_locator.Core.Utilities;
using EC_locator.Locator;
using EC_locator.Parsers;
using EC_locator.Test;
using EC_locator.Repositories;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

var builder = WebApplication.CreateBuilder(args);

var apiUrl = builder.Configuration.GetSection("HeartBeat").GetValue<string>("URI");
var rate = builder.Configuration.GetSection("HeartBeat").GetValue<string>("TIMESPAN(D:H:M:S)");
HeartBeatManager heartBeatManager = new HeartBeatManager(apiUrl, rate);
//heartBeatManager.StartHeartBeat();

// Add services to the container.
ConfigureSettingsOptions(builder.Services);
ConfigureLocatorServices(builder.Services);
ConfigureApiServices(builder.Services);

var app = builder.Build();

/*
ManualPrecisionTestCLI mpt = new(app.Services.GetService<ITeamsRepository>(), app.Services.GetService<IMessageParser>());
mpt.RunTest();
Environment.Exit(1);
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
    
    // utility services
    services.AddSingleton<DateTimeProvider>();
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



