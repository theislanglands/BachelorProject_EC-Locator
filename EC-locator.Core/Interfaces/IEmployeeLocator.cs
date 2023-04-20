using EC_locator.Core.Models;

namespace EC_locator.Core.Interfaces;

public interface IEmployeeLocator
{
    Location? GetCurrentLocation(string employeeId);
    Message? GetLatestMessage(string employeeId);

    List<CalendarEvent>? GetCurrentCalendarEvents(string employeeId);
}