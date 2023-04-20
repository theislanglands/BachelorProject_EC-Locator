using EC_locator.Core.Models;
using Microsoft.Graph;

namespace EC_locator.Core.Interfaces;

public interface ICalendarRepository
{
    Task<List<CalendarEvent>?> GetCurrentCalendarEventsAsync(string employeeId);
}