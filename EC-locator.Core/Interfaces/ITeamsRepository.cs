
using System.Collections;
using Microsoft.Graph;

namespace EC_locator.Core.Interfaces;

public interface ITeamsRepository
{
    Task<List<User>> GetUsersAsync();

    Task<ArrayList> GetMessagesAsync();

    // TODO: FOR TESTING - to be deleted
    
    // testing conncection to messages
    Task ListMessagesAsync();
    
    // returns hardcoded messages
    string[] GetMessages(string employeeID, DateOnly date);
    
}