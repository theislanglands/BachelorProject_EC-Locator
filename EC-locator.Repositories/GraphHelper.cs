// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
namespace EC_locator.Repositories;

// using EClocator.Core;

using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;

class GraphHelper

{
// Settings object
    private static Settings? _settings = Settings.GetInstance();
    
// App-ony auth token credential
private static ClientSecretCredential? _clientSecretCredential;

// GraphClient for access to MS graph
private static GraphServiceClient? _graphClient;

private static void EnsureGraphForAppOnlyAuth()
{
// Ensure settings has been initialized
    if (_settings.ClientId == null || _settings.ClientSecret == null || _settings.TenantId == null)
    {
        throw new System.NullReferenceException("MS Graph Settings not initialized");
    }

    if (_clientSecretCredential == null)
{
    _clientSecretCredential = new ClientSecretCredential(
        _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
}

if (_graphClient == null)
{
    _graphClient = new GraphServiceClient(_clientSecretCredential,
        // scopes configured on the app registration
        new[] {"https://graph.microsoft.com/.default"});
}
}

// <GetUsersSnippet>
public static Task<IGraphServiceUsersCollectionPage> GetUsersAsync()
{
EnsureGraphForAppOnlyAuth();
// Ensure client isn't null
_ = _graphClient ??
    throw new System.NullReferenceException("Graph has not been initialized ");

return _graphClient.Users
    .Request()
    .Select(u => new
    {
        // Only request specific properties
        u.DisplayName,
        u.Id,
        u.Mail
    })
    // Get at most 5 results
    .Top(5)
    // Sort by display name
    .OrderBy("DisplayName")
    .GetAsync();
}
// </GetUsersSnippet>


#pragma warning disable CS1998
// <MakeGraphCallSnippet>
// This function serves as a playground for testing Graph snippets
// or other code
public async static Task MakeGraphCallAsync()
{
// INSERT YOUR CODE HERE
// Note: if using _appClient, be sure to call EnsureGraphForAppOnlyAuth
// before using it.
// EnsureGraphForAppOnlyAuth();
}
// </MakeGraphCallSnippet>


    public static Task<IChannelMessagesCollectionPage> getMessagesAsync()
    {
        EnsureGraphForAppOnlyAuth();
        
        // Ensure client isn't null
        _ = _graphClient ??
            throw new System.NullReferenceException("Graph has not been initialized ");

        string channelID = "19%3a5e10833a67a74fed84927e17db5c8d52%40thread.skype";
        string teamID = "fe5c4984-c3d0-4a17-b213-4e4c845e07ce&tenantId=b5cf4dd9-702b-4170-a901-60b49142a6c4";
        
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
    /*
    return _graphClient.Users
        .Request()
    .Select(u => new
    {
        // Only request specific properties
        u.DisplayName,
        u.Id,
        u.Mail
    })
    // Get at most 5 results
    .Top(5)
        // Sort by display name
        .OrderBy("DisplayName")
        .GetAsync();
        */

}
/*
.Select(m => new
    {
        // Only request specific properties
        m.Body.Content,
        m.Id
    })
    */
