namespace EC_locator.Core.Interfaces;

public interface ITeamsRepository
{
    string[] GetMessages(string employeeID, DateOnly date);
}