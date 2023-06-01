using System.Reflection.Metadata;
using API;
using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using EC_locator.Core.Utilities;
using EC_locator.Locator;
using EC_locator.Parsers;
using EC_locator.Test;
using EC_locator.Repositories;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

HeartBeatManager heartBeatManager;
var builder = WebApplication.CreateBuilder(args);
InitializeHeartBeatManager();
// heartBeatManager.StartHeartBeat();

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

// heartBeatManager.StartHeartBeat();
app.Run();

void InitializeHeartBeatManager()
{
    var apiUrl = builder.Configuration.GetSection("HeartBeat").GetValue<string>("URI");
    var rate = builder.Configuration.GetSection("HeartBeat").GetValue<string>("TIMESPAN(D:H:M:S)");
    heartBeatManager = new HeartBeatManager(apiUrl, rate);
}

// configurations for app settings
void ConfigureSettingsOptions(IServiceCollection serviceCollection)
{
    serviceCollection.Configure<VerboseOptions>(builder.Configuration);
    serviceCollection.Configure<GraphHelperOptions>(builder.Configuration.GetSection("AzureAd"));
    serviceCollection.Configure<LocatorRepositoryOptions>(builder.Configuration.GetSection("MSSQL"));
    serviceCollection.Configure<DefaultLocationOptions>(builder.Configuration.GetSection("DefaultLocation"));
    serviceCollection.Configure<TeamsOptions>(builder.Configuration.GetSection("TestTeamsChannel"));
    serviceCollection.Configure<UsersOptions>(builder.Configuration);
}

// Application Services
void ConfigureLocatorServices(IServiceCollection services)
{
    services.AddSingleton<IMessageParser, MessageParser>();
    services.AddSingleton<IEmployeeLocator, EmployeeLocator>();
    
    // ADDING REPOSITORIES
    services.AddSingleton<ICalendarRepository, CalendarRepository>();

    
    if (builder.Configuration.GetValue<bool>("UseLocalTeamsRepo"))
    {
        // FOR TESTING ONLY
        services.AddSingleton<ITeamsRepository, TeamsRepositoryLocal>();
    } 
    else 
    {
        services.AddSingleton<ITeamsRepository, TeamsRepository>();
    }

    if (builder.Configuration.GetValue<bool>("UseDatabase"))
    {
        services.AddSingleton<ILocatorRepository, LocatorRepository>();
    } 
    else 
    {
        // USE HARDCODED KEYWORDS - bypassing database
        services.AddSingleton<ILocatorRepository, LocatorRepositoryLocal>();
    }
    
    // message parser services
    services.AddSingleton<ILocationTagger, LocationTagger>();
    services.AddSingleton<ITimeTagger, TimeTagger>();
    services.AddSingleton<ITimeAndLocationConnector, TimeAndLocationConnector>();
    
    // Teams repository services
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
    
    
    // TODO generate example responses
    // https://mattfrear.com/2016/01/25/generating-swagger-example-requests-with-swashbuckle/
    // https://code-maze.com/swagger-ui-asp-net-core-web-api/
    
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "EC-locator API",
            Description = "USE the EC-locator API calls for retrieving the employees of Ecreo and their current location",
            /*
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = new Uri("https://example.com/license")
            }
            */
        });
    });
    
    
    
}



