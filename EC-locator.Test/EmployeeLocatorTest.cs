using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using EC_locator.Core.Utilities;
using EC_locator.Locator;
using Microsoft.Extensions.Options;

namespace EC_locator.Test;

[TestFixture]
public class EmployeeLocatorTest
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IMessageParser _messageParser;
    private readonly ICalendarRepository _calendarRepository;
    private IOptions<VerboseOptions> verboseOptions;
    private IOptions<DefaultLocationOptions> locationOptions;
    
    private IEmployeeLocator _employeeLocator;

    
    [SetUp]
    public void Setup()
    {
        verboseOptions = Options.Create(new VerboseOptions { Verbose = true, UseDatabase = false});
        locationOptions = Options.Create(new DefaultLocationOptions {
            DefaultWorkStart = "9:00",
            DefaultWorkEnd = "16:00",
            DefaultLocation = "office"
        });
       
    }
    
    [Test]
    public void GetCurrentLocation_CurrentTimeWeekdayBeforeDefaultWorkingHours_ReturnsOff()
    {
        var defaultStartHour = int.Parse(locationOptions.Value.DefaultWorkStart.Split(":")[0]);
        var defaultEndHour = int.Parse(locationOptions.Value.DefaultWorkEnd.Split(":")[0]);
        
        // Datetime with fixed time - 1 hour before default start time
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,5, defaultStartHour,0,0).AddHours(-1));
        _employeeLocator = new EmployeeLocator(_messageParser, _teamsRepository, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo("off"), $"should return off when before default work hours");
    }
    
    [Test]
    public void GetCurrentLocation_CurrentTimeWeekdayAfterDefaultWorkingHours_ReturnsOff()
    {
        //ARRANGE
        var defaultEndHour = int.Parse(locationOptions.Value.DefaultWorkEnd.Split(":")[0]);
        
        // Datetime with fixed time - 1 hour before after default start time on a weekday
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,5, defaultEndHour,0,0).AddHours(+1));

        _employeeLocator = new EmployeeLocator(_messageParser, _teamsRepository, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo("off"), $"should return off when after default work hours");
    }
    
    
    [Test]
    public void GetCurrentLocation_CurrentDayIsWeekendInsideDefaultWorkingHours_ReturnsOff()
    {
        //ARRANGE
        var defaultEndHour = int.Parse(locationOptions.Value.DefaultWorkEnd.Split(":")[0]);
        
        // Datetime with fixed time - 1 hour before after default end time on a weekend
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,8, defaultEndHour,0,0).AddHours(-1));

        _employeeLocator = new EmployeeLocator(_messageParser, _teamsRepository, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo("off"), $"method GetCurrentLocation() should return off when called in a weekend");
    }
    
    [Test]
    public void GetCurrentLocation_NoTeamsMessagesFound_ReturnsDefault()
    {
        // ARRANGE
        
        // ACT 
        
        // ASSERT
    }
    
    [Test]
    public void GetCurrentLocation_NoLocationsInMessage_ReturnsUndefined()
    {
        // ARRANGE
        
        // ACT 
        
        // ASSERT
    }
    
    
    [Test]
    public void GetCurrentLocation_NoLocationsInMessageMatchCurrentTime_ReturnsUndefined()
    {
        // ARRANGE
        
        // ACT 
        
        // ASSERT
    }
    
    [Test]
    public void GetCurrentLocation_LocationMatchingCurrentTime_ReturnsCorrectLocation()
    {
        // ARRANGE
        
        // ACT 
        
        // ASSERT
    }
}