// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Repositories;

using EC_locator.Core;
using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;

public class GraphHelper : IGraphHelper
{
    private static GraphServiceClient? _graphClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tenantId;

    // App-ony auth token credential
    private static ClientSecretCredential? _clientSecretCredential;

    // GraphClient for access to MS graph
    public GraphHelper(IOptions<GraphHelperOptions> settingsOptions)
    {
        _clientId = settingsOptions.Value.ClientId;
        _clientSecret = settingsOptions.Value.ClientSecret;
        _tenantId = settingsOptions.Value.TenantId;
    }

    private void EnsureGraphForAppOnlyAuth()
    {
        // Ensure settings has been initialized
        if (_clientId == null || _clientSecret == null || _tenantId == null)
        {
            throw new System.NullReferenceException("MS Graph Settings not initialized");
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
            throw new System.NullReferenceException("Graph has not been initialized ");

        // Define the filter criteria to exclude room resources
        List<Option> options = new List<Option>
        {
            new QueryOption("$filter", "userType ne 'Room'")
        };
        
        var fetchedUsers = _graphClient.Users
            .Request()
            .Select(u => new
            {
                // Only request specific properties
                u.DisplayName,
                u.Id,
                u.Mail,
                u.UserType
            })
            .OrderBy("DisplayName")
            .GetAsync();
        
        return fetchedUsers;
    }

    public Task<IChannelMessagesCollectionPage> getMessagesAsync()
    {
        EnsureGraphForAppOnlyAuth();

        // Ensure client isn't null
        _ = _graphClient ??
            throw new System.NullReferenceException("Graph has not been initialized ");

        string? teamID = Settings.GetInstance().TeamId;
        string? channelID = Settings.GetInstance().ChannelID;

        var messages = _graphClient.Teams[teamID].Channels[channelID].Messages
            .Request()
            .Select(m => new
            {
                // Only request specific properties
                m.Id
            })
            .Top(3)
            .GetAsync();

        return messages;
    }
    
    public Task<IChannelMessagesCollectionPage> getCalendarEventsAsync(string employeeId)
    {
        EnsureGraphForAppOnlyAuth();
        _ = _graphClient ??
            throw new System.NullReferenceException("Graph has not been initialized ");
        
        var start = "2022-11-20T10:00:00.0000000"; //for debug only
        var end = "2024-11-20T23:00:00.0000000"; 
        
        List<Option> options = new List<Option>
        {
            new QueryOption("startDateTime", start),
            new QueryOption("endDateTime", end),
            new QueryOption("orderby", "start/dateTime")
        };

        var events = _graphClient.Users[employeeId].CalendarView
            .Request(options)
            .GetAsync()
            .Result;
        
        foreach (var ev in events)
        {
            Console.WriteLine($"subject: {ev.Subject}");
            Console.WriteLine($"body: {ev.Body}");
            Console.WriteLine($"type: {ev.Type}");
            Console.WriteLine($"body pw: {ev.BodyPreview}");

        }

        Console.WriteLine("Exiting program");
        Environment.Exit(1);
        
        return null;
    }
}

  
