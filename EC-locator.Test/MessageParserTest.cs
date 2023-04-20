using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using EC_locator.Repositories;
using Microsoft.Extensions.Options;
using EC_locator.Parsers;
using Location = EC_locator.Core.Models.Location;

namespace EC_locator.Test;

[TestFixture]
public class MessageParserTest
{
    // TODO: create mock - only possible to mock interfaces!
    private ILocationTagger _locationTagger;
    private ITimeTagger _timeTagger;
    private ITimeAndLocationConnector _timeAndLocationConnector;
    private ILocatorRepository _locatorRepository;
    MessageParser _messageParser;

    private Dictionary<string, Location[]> _messageSamples;

    [SetUp]
    public void Setup()
    {
        // settings Options used in objects
        var verboseOptions = Options.Create(new VerboseOptions { Verbose = false, UseDatabase = false});
        var locatorRepositoryOptions = Options.Create(new LocatorRepositoryOptions
        {
            Host = "localhost",
            UserId = "sa",
            Password = "Secretpassword1!",
            ConnectionStringRW = "Server=tcp:ecreo01.database.windows.net,1433;" +
                                 "Initial Catalog=sqldb-eclocator-dev-001;" +
                                 "Persist Security Info=False;" +
                                 "User ID=eclocator-dev-ow;" +
                                 "Password=K@7KJ.Y3Vk!(BDAaaRczTWt@lC*Q;" +
                                 "MultipleActiveResultSets=False;" +
                                 "Encrypt=True;" +
                                 "TrustServerCertificate=False;" +
                                 "Connection Timeout=30;"
        });
        var locationOptions = Options.Create(new DefaultLocationOptions
        {
            DefaultWorkStart = "9:00",
            DefaultWorkEnd = "16:00",
            DefaultLocation = "office"
        });

        // Creating objects 
        _locatorRepository = new LocatorRepository(locatorRepositoryOptions, verboseOptions);
        _locationTagger = new LocationTagger(_locatorRepository, verboseOptions);
        _timeTagger = new TimeTagger(_locatorRepository, verboseOptions);
        _timeAndLocationConnector = new TimeAndLocationConnector(verboseOptions, locationOptions);
        _messageParser = new MessageParser(_locationTagger, _timeTagger, _timeAndLocationConnector, verboseOptions);
        
        _messageSamples = new Dictionary<string, Location[]>(); 
        AddHomeMessageSamples();
        AddHomeAndOfficeMessageSamples();
        AddIllMessageSamples();
        AddKidsIllMessageSamples();
        AddStartAndStopMessageSamples();
        AddMeetingAndRemoteMessageSamples();
        AddNegationMessageSamples();
    }
    
    [Test]
    public void MessageSample_ReturnsCorrectLocationObjects()
    {
        foreach (var message in _messageSamples)
        {
            TestLocation(message);
        }
    }

    private void TestLocation(KeyValuePair<string, Location[]> message)
    {
        var locations = _messageParser.GetLocations(message.Key);

        Assert.That(locations.Count, Is.EqualTo(message.Value.Length), 
            $"number of locations found not correct in message: \n{message.Key}");
        
        for (int i = 0; i < message.Value.Length; i++)
        {
            Assert.That(locations[i].Place, Is.EqualTo(message.Value[i].Place),
                $"wrong location identified in message: {message.Key}");
            Assert.That(locations[i].Start, Is.EqualTo(message.Value[i].Start),
                $"wrong start time identified in message: {message.Key}");
            Assert.That(locations[i].End, Is.EqualTo(message.Value[i].End),
                $"wrong end time identified in message: {message.Key}");
        }
    }

    private void AddMeetingAndRemoteMessageSamples()
    {
        _messageSamples.Add(
            "Thomas, Gorm og jeg tager ned til Nørgaard Mikkelsen til møde, forventer at være retur 10.30",
            new[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 30),
                    "meeting"),
                new Location(
                    new TimeOnly(10, 30),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Starter til møde hos NM. Er tilbage lidt over 10.",
            new[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 0),
                    "meeting"),
                new Location(
                    new TimeOnly(10, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Morn - jeg starter hos lægen og kører på Popermo efterfølgende",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "undefined"),
            });
        _messageSamples.Add(
            "Er til møde ved Alumeco indtil 11.30 i morgen og arbejder hjemme fra derefter.",
            new []
            {
                new Location(
                    new TimeOnly(9,0),
                    new TimeOnly(11, 30),
                    "meeting"),
                new Location(
                    new TimeOnly(11,30),
                    new TimeOnly(16, 0),
                    "home"),
            });
        _messageSamples.Add(
            "Jeg tager hjem og arbejder efter zoo mødet  Hovedet driller lidt i dag. ",
            new []
            {
                new Location(
                    new TimeOnly(9,0),
                    new TimeOnly(16, 0),
                    "undefined"),
            });
        _messageSamples.Add(
            "Jeg er i Nørresundby hele dagen i morgen hos Continia sammen med Martin, Simone og Jesper",
            new []
            {
                new Location(
                    new TimeOnly(9,0),
                    new TimeOnly(16, 0),
                    "remote"),
            });
    }

    private void AddHomeAndOfficeMessageSamples()
    {
        _messageSamples.Add(
            "0930 på kontoret",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(9, 30),
                    "home"),
                new Location(
                    new TimeOnly(9, 30),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "0930",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(9, 30),
                    "home"),
                new Location(
                    new TimeOnly(9, 30),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Jeg bliver hjemme indtil jeg kan aflevere min cykel til service klokken 10, og så kommer jeg ind.",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 0),
                    "home"),
                new Location(
                    new TimeOnly(10, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Jeg smutter til tandlæge her klokken 12. Arbejder muligvis hjemmefra efter.",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(12, 0),
                    "off"),
                new Location(
                    new TimeOnly(12, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        _messageSamples.Add(
            "Jeg tager lige en time mere fra hjemmekontoret. Er inde ca. kl 10",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 0),
                    "home"),
                new Location(
                    new TimeOnly(10, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Kommer ind på kontoret omkring kl. 11",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(11, 0),
                    "home"),
                new Location(
                    new TimeOnly(11, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Godmorgen, jeg starter ud hjemme og kommer ind omkring kl 10",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 0),
                    "home"),
                new Location(
                    new TimeOnly(10, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Godmorgen. Jeg starter hjemme, men forventer at være på kontoret kl 10. Vi ses ✌",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(10, 0),
                    "home"),
                new Location(
                    new TimeOnly(10, 0),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Er inde 9:15",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(9, 15),
                    "home"),
                new Location(
                    new TimeOnly(9, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Jeg er på kontoret cirka 09.30",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(9, 30),
                    "home"),
                new Location(
                    new TimeOnly(9, 30),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Er hjemmefra med Otto indtil backup kommer Jeg er inde inden frokost",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(11, 15),
                    "home"),
                new Location(
                    new TimeOnly(11, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
    }


    private void AddHomeMessageSamples()
    {
        _messageSamples.Add(
            "Jeg er på hjemmefra i dag",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        _messageSamples.Add(
            "Godmorgen. Jeg er på hjemmekontoret idag",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        _messageSamples.Add(
            "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        _messageSamples.Add(
            "Jeg er på hjemmefra i dag.",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
    }

    private void AddStartAndStopMessageSamples()
    {
        _messageSamples.Add(
            "Jeg starter lige hjemme og er på kontoret til frokost",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(11, 15),
                    "home"),
                new Location(
                    new TimeOnly(11, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(9, 30),
                    "off"),
                new Location(
                    new TimeOnly(9, 30),
                    new TimeOnly(11, 15),
                    "home"),
                new Location(
                    new TimeOnly(11, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "I morgen arbejder jeg hjemmefra og stopper 11.30",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(11, 30),
                    "home"),
                new Location(
                    new TimeOnly(11, 30),
                    new TimeOnly(16, 00),
                    "off"),
            });
        _messageSamples.Add(
            "Vejret gjorde lige det helt lidt mere bøvlet her til morgen. Jeg er inde omkring kvart over 9...",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(9, 15),
                    "home"),
                new Location(
                    new TimeOnly(9, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
        _messageSamples.Add(
            "Arbejder hjemme i dag og går fra ved frokosttid. God påske ",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(11, 15),
                    "home"),
                new Location(
                    new TimeOnly(11, 15),
                    new TimeOnly(16, 0),
                    "off")
            });
        _messageSamples.Add(
            "Jeg holder weekend ved 14 tiden God påske til jer der går på ferie",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(14, 00),
                    "home"),
                new Location(
                    new TimeOnly(14, 00),
                    new TimeOnly(16, 0),
                    "off")
            });
        _messageSamples.Add(
            "Lukker ned kl. 14",
            new []
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(14, 00),
                    "home"),
                new Location(
                    new TimeOnly(14, 00),
                    new TimeOnly(16, 0),
                    "off")
            });
    }

    private void AddKidsIllMessageSamples()
    {
        _messageSamples.Add(
            "Er hjemme med syge piger, så er lidt on/off hele dagen",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "KidsIll")
            });
        _messageSamples.Add(
            "Den lille er stadigvæk syg, arbejde det jeg kan ind i mellem",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "KidsIll")
            });
        _messageSamples.Add(
            "Felix er desværre syg med feber så tager den hjemmefra, så meget det er muligt",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "KidsIll")
            });
        _messageSamples.Add(
            "Otto er desværre blevet syg, så jeg holder hjemmefronten indtil backup ankommer. Er på kontoret inden 11",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "KidsIll")
            });
    }


    private void AddIllMessageSamples()
    {
        _messageSamples.Add(
            "tager en dag under dynen",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        
        _messageSamples.Add(
            "Jeg har krammet toilettet hele natten, så jeg er hjemme, og sover forhåbentligt",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "Er ikke på toppen - Er on/off i dag",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "Stadig ikke på toppen, men arbejder det jeg kan",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "Er helt smadret - bliver under dynen, og ser om jeg kan arbejde senere",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "Jeg er slet ikke på toppen, så jeg bliver hjemme i dag",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "jeg er syg i dag",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        _messageSamples.Add(
            "Det er som om min forkølelse er blusset op igen, så jeg er nok først på senere.",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
    }

    private void AddNegationMessageSamples()
    {
        _messageSamples.Add(
            "Kommer ikke på kontoret",
            new []
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
    }

}