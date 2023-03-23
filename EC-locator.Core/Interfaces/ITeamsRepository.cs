
using Microsoft.Graph;

namespace EC_locator.Core.Interfaces;

public interface ITeamsRepository
{
    string[] GetMessages(string employeeID, DateOnly date);

    Task<List<User>> GetUsersAsync();
}