
using System.Collections;
using Microsoft.Graph;
using Message = EC_locator.Core.Models.Message;

namespace EC_locator.Core.Interfaces;

public interface ITeamsRepository
{
    Task<List<User>> GetUsersAsync();
    Task<List<Message>?> GetRecentMessagesAsync(string employeeId);
    Task<List<Message>> FetchAllMessagesAsync(DateOnly fromDate, DateOnly toDate);
    
    
    // returns hardcoded messages
    string[] GetSamples(string employeeID);
    
    List<Message>? GetMessageSamples(string employeeId);
}