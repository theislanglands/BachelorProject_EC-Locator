using EC_locator.Repositories;
using EClocator.Core.Interfaces;
using Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;


// testing message parser
// IMessageParser messageParser = new MessageParser();
// messageParser.testParser();

var builder = WebApplication.CreateBuilder(args);

// initialize Settings singleton - used for Microsoft Graph Access
initSettings();

TeamsRepository tr = new TeamsRepository();
await tr.ListUsersAsync();

Environment.Exit(1);


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
