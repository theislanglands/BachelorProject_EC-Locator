using System.Collections;
using EC_locator.Repositories;
using EClocator.Core.Interfaces;
using Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;


// testing message parser
// TODO update interfaces
MessageParser messageParser = new MessageParser();
TeamsRepository tr = new TeamsRepository();

string[] messages = tr.GetMessages("ert", new DateOnly());
foreach (string message in messages)
{
    messageParser.PrintLocations(message);
}


// messageParser.GetLocation("hej");
Environment.Exit(1);


var builder = WebApplication.CreateBuilder(args);

// initialize Settings singleton - used for Microsoft Graph Access
initSettings();


/*
// TESTING OF TEAMS REPO

await tr.ListUsersAsync();
await TestGettingUsersFromTeamsRepo();

// TODO: Problemer med API
await tr.ListMessagesAsync();
*/

async Task TestGettingUsersFromTeamsRepo()
{
    ArrayList users = await tr.GetUsersAsync();
    foreach (Microsoft.Graph.User user in users)
    {
        Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
        Console.WriteLine($"  ID: {user.Id}");
        Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}"); 
    }
}

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
    var clientId = builder.Configuration.GetSection("AzureAd")["ClientId"];
    var tenantId = builder.Configuration.GetSection("AzureAd")["TenantId"];
    var clientSecret = builder.Configuration.GetSection("AzureAd")["ClientSecret"];
    Settings.GetInstance().initForAppAuth(clientId, clientSecret, tenantId);
}
