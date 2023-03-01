namespace EClocator.Core.Interfaces;

public interface ITeamsRepository
{
    string[] getMessages(string employeeID, DateOnly date);
}