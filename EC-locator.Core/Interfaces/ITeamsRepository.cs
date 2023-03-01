namespace EClocator.Core.Interfaces;

public interface ITeamsRepository
{
    string[] GetMessages(string employeeID, DateOnly date);
}