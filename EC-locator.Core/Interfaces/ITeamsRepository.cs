
using System.Collections;
using Microsoft.Graph;
using Message = EC_locator.Core.Models.Message;

namespace EC_locator.Core.Interfaces;

public interface ITeamsRepository
{
    Task<List<User>> GetUsersAsync();

    Task<List<Message>> GetMessagesAsync(string employeeId, DateOnly date);

    // TODO: FOR TESTING - to be deleted
    
    // testing connection to messages
    Task ListMessagesAsync();
    
    // returns hardcoded messages
    string[] GetMessages(string employeeID, DateOnly date);
    
}