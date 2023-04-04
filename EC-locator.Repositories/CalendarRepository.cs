using EC_locator.Core.Interfaces;

using System.Collections;
using EC_locator.Core;
using Microsoft.Graph;


namespace EC_locator.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private IGraphHelper _graphHelper;
    Settings _settings = Settings.GetInstance();
    // CALENDAR EVENTS ALL-DAY

    public CalendarRepository(IGraphHelper graphHelper)
    {
        _graphHelper = graphHelper;
    }

    public async Task<List<User>> GetCalendarEvents()
    {
        // anders
        //var employeeId = "2cf3e351-6ca8-4fda-999c-14a8b048b899";
        // brian
        //var employeeId = "2d3cfcdf-542d-43f5-a4b1-6f58387604eb";
        
        // theis
        var employeeId = "6e5ee9cb-11cb-405d-aaa8-60c3768340c3";
        await _graphHelper.getCalendarEventsAsync(employeeId);
        return null;
    }
}