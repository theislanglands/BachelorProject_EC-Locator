using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace EC_locator.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly bool _verbose;
    private IGraphHelper _graphHelper;
    // CALENDAR EVENTS ALL-DAY

    public CalendarRepository(IGraphHelper graphHelper, IOptions<VerboseOptions> settingsOptions)
    {
        _graphHelper = graphHelper;
        _verbose = settingsOptions.Value.Verbose;
    }

    public async Task<List<User>> GetCalendarEvents()
    {
        // anders
        //var employeeId = "2cf3e351-6ca8-4fda-999c-14a8b048b899";
        // brian
        //var employeeId = "2d3cfcdf-542d-43f5-a4b1-6f58387604eb";
        if (_verbose)
        {
            Console.WriteLine("fetching calendar events");
        }

        // theis
        var employeeId = "6e5ee9cb-11cb-405d-aaa8-60c3768340c3";
        await _graphHelper.getCalendarEventsAsync(employeeId);
        return null;
    }
}