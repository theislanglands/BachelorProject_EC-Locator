using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;
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
        // Workday within default workinghours
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,5, 14,0,0));
        
        var teamsRepositoryMock = new Mock<ITeamsRepository>();
        teamsRepositoryMock.Setup(
            x => x.GetRecentMessagesAsync("test")).Returns(Task.FromResult<List<Message>?>(null));
        
        _employeeLocator = new EmployeeLocator(_messageParser, teamsRepositoryMock.Object, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo(locationOptions.Value.DefaultLocation), "When no messages found, default location is expected");
        var startTime = location.Start.GetValueOrDefault().ToString("H\\:mm");
        Assert.That(startTime, Is.EqualTo(locationOptions.Value.DefaultWorkStart), "Default Work start time not correct");
        var endTime = location.End.GetValueOrDefault().ToString("H\\:mm");
        Assert.That(endTime, Is.EqualTo(locationOptions.Value.DefaultWorkEnd), "Default Work end time not correct");
    }
    
    
    [Test]
    public void GetCurrentLocation_NoLocationsInMessageMatchCurrentTime_ReturnsUndefined()
    {
        // ARRANGE
        // Workday within default workinghours
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,5, 15,0,0));

        Message latestMessage = new()
        {
            Content = "test",
            UserId = "test",
            TimeStamp = dateTimeProvider.Now
        };
        
        var teamsRepositoryMock = new Mock<ITeamsRepository>();
        teamsRepositoryMock.Setup(
            x => x.GetRecentMessagesAsync("test")).ReturnsAsync(new List<Message>{latestMessage});
        
        var locationList = new List<Location>
        {
            new()
            {
                Start = new TimeOnly(10,0,0),
                End = new TimeOnly(12,0,0),
                Place = "office"
            },
            new()
            {
                Start = new TimeOnly(12,0,0),
                End = new TimeOnly(14,0,0),
                Place = "home"
            }
        };

        var messageParserMock = new Mock<IMessageParser>();
        messageParserMock.Setup(x => x.GetLocations(latestMessage)).Returns(locationList);
        
        _employeeLocator = new EmployeeLocator(messageParserMock.Object, teamsRepositoryMock.Object, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo("undefined"), "No location matching current time should return undefined");
    }
    
    [Test]
    public void GetCurrentLocation_LocationMatchingCurrentTime_ReturnsCorrectLocation()
    {
        // ARRANGE
        // Workday within default workinghours
        var dateTimeProvider = new DateTimeProvider(new DateTime(2023,1,5, 12,0,0));

        Message latestMessage = new()
        {
            Content = "test",
            UserId = "test",
            TimeStamp = dateTimeProvider.Now
        };
        
        var teamsRepositoryMock = new Mock<ITeamsRepository>();
        teamsRepositoryMock.Setup(
            x => x.GetRecentMessagesAsync("test")).ReturnsAsync(new List<Message>{latestMessage});
        
        var locationList = new List<Location>
        {
            new()
            {
                Start = new TimeOnly(10,0,0),
                End = new TimeOnly(12,0,0),
                Place = "home"
            },
            new()
            {
                Start = new TimeOnly(12,0,0),
                End = new TimeOnly(14,0,0),
                Place = "remote"
            }
        };

        var messageParserMock = new Mock<IMessageParser>();
        messageParserMock.Setup(x => x.GetLocations(latestMessage)).Returns(locationList);
        
        _employeeLocator = new EmployeeLocator(messageParserMock.Object, teamsRepositoryMock.Object, _calendarRepository, dateTimeProvider, verboseOptions,
            locationOptions);
        
        // ACT 
        var location = _employeeLocator.GetCurrentLocation("test");
        
        // ASSERT
        Assert.That(location.Place, Is.EqualTo("remote"), "Location remote - should match current time");
    }
}