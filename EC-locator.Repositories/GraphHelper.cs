// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Repositories;

using Azure.Identity;
using Microsoft.Graph;

public class GraphHelper : IGraphHelper
{
    private static GraphServiceClient? _graphClient;
    private readonly string _clientId, _clientSecret, _tenantId;
    private readonly string _teamId, _channelId;
    
    // App-ony auth token credential
    private static ClientSecretCredential? _clientSecretCredential;

    // GraphClient for access to MS graph
    public GraphHelper(IOptions<GraphHelperOptions> settingsOptions, IOptions<TeamsOptions> teamsSettings)
    {
        _clientId = settingsOptions.Value.ClientId;
        _clientSecret = settingsOptions.Value.ClientSecret;
        _tenantId = settingsOptions.Value.TenantId;
        _teamId = teamsSettings.Value.TeamId;
        _channelId = teamsSettings.Value.ChannelId;
    }

    private void EnsureGraphForAppOnlyAuth()
    {
        // Ensure settings has been initialized
        if (_clientId == null || _clientSecret == null || _tenantId == null)
        {
            throw new NullReferenceException("MS Graph Settings not initialized");
        }

        if (_clientSecretCredential == null)
        {
            _clientSecretCredential = new ClientSecretCredential(
                _tenantId, _clientId, _clientSecret);
        }

        if (_graphClient == null)
        {
            _graphClient = new GraphServiceClient(_clientSecretCredential,
                // scopes configured on the app registration
                new[] { "https://graph.microsoft.com/.default" });
        }
    }

    // Get Users
    public Task<IGraphServiceUsersCollectionPage> GetUsersAsync()
    {
        EnsureGraphForAppOnlyAuth();
        _ = _graphClient ??
            throw new NullReferenceException("Graph has not been initialized ");

        // Define the filter criteria to exclude room resources
        List<Option> options = new List<Option>
        {
            new QueryOption("$filter", "userType ne 'Room'")
        };
        
        var fetchedUsers = _graphClient.Users
            .Request()
            .Select(u => new
            {
                u.DisplayName,
                u.Id,
                u.Mail,
                u.UserType
            })
            .OrderBy("DisplayName")
            .GetAsync();
        
        return fetchedUsers;
    }
    
    // returning messages from the specified day and onwords
    
    public Task<IChatMessageDeltaCollectionPage> GetMessagesAsync(DateOnly date)
    {
        EnsureGraphForAppOnlyAuth();
        _ = _graphClient ??
            throw new NullReferenceException("Graph has not been initialized ");

        string dateString = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "Z";
        
        var messages = _graphClient
            .Teams[_teamId]
            .Channels[_channelId]
            .Messages
            .Delta()
            .Request()
            .Filter($"lastModifiedDateTime gt {dateString}")
            .GetAsync();

        return messages;
    }
    
    public Task<IUserCalendarViewCollectionPage> getCalendarEventsAsync(string employeeId)
    {
        EnsureGraphForAppOnlyAuth();
        _ = _graphClient ??
            throw new NullReferenceException("Graph has not been initialized ");
        
        var start = "2023-04-19T13:59:00.0000000"; //for debug only
        var end = "2026-04-20T23:00:00.0000000";

        // Set the start and end times for the current window of events
        DateTimeOffset startDateTime = DateTimeOffset.UtcNow;
        start = startDateTime.ToString("o"); // TIME RIGHT NOW
        //Console.WriteLine(start);
        DateTimeOffset endDateTime = startDateTime.AddDays(1);
        start = endDateTime.ToString("o"); // TIME RIGHT NOW + 1 day
        
        List<Option> options = new List<Option>
        {
            new QueryOption("startDateTime", start),
            new QueryOption("endDateTime", end),
            new QueryOption("orderby", "start/dateTime")
        };

        var events = _graphClient.Users[employeeId].CalendarView
            .Request(options)
            .Top(1)
            .GetAsync();
            
        
       

        //Console.WriteLine("Exiting program");
        //Environment.Exit(1);

        return events;
    }
}

  
