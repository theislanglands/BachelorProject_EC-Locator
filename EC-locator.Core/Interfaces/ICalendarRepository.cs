using Microsoft.Graph;

namespace EC_locator.Core.Interfaces;

public interface ICalendarRepository
{
    Task<List<User>> GetCalendarEvents();
}