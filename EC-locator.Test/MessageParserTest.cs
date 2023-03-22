using EC_locator.Core.Models;
using Parser;

namespace EC_locator.Test;

[TestFixture]
public class MessageParserTest
{
    MessageParser mp;

    private Dictionary<string, Location[]> messageSamples;

    [SetUp]
    public void Setup()
    {
        mp = new MessageParser();
        messageSamples = new Dictionary<string, Location[]>(); 
        AddHomeMessageSamples();
        AddHomeAndOfficeMessageSamples();
        AddIllMessageSamples(); 
        AddStartAndStopMessageSamples();
        AddMeetingAndRemoteMessageSamples();
    }

    private void AddMeetingAndRemoteMessageSamples()
    {
        messageSamples.Add(
            "Thomas, Gorm og jeg tager ned til Nørgaard Mikkelsen til møde, forventer at være retur 10.30",
            new Location[]
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
        messageSamples.Add(
            "Starter til møde hos NM. Er tilbage lidt over 10.",
            new Location[]
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
        messageSamples.Add(
            "Morn - jeg starter hos lægen og kører på Popermo efterfølgende",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "remote"),
            });
        messageSamples.Add(
            "Er til møde ved Alumeco indtil 11.30 i morgen og arbejder hjemme fra derefter.",
            new Location[]
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
        messageSamples.Add(
            "Jeg tager hjem og arbejder efter zoo mødet  Hovedet driller lidt i dag. ",
            new Location[]
            {
                new Location(
                    new TimeOnly(9,0),
                    new TimeOnly(16, 0),
                    "undefined"),
            });
    }

    private void AddHomeAndOfficeMessageSamples()
    {
        messageSamples.Add(
            "0930 på kontoret",
            new Location[]
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
        messageSamples.Add(
            "Jeg bliver hjemme indtil jeg kan aflevere min cykel til service klokken 10, og så kommer jeg ind.",
            new Location[]
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
        messageSamples.Add(
            "Jeg smutter til tandlæge her klokken 12. Arbejder muligvis hjemmefra efter.",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(12, 0),
                    "office"),
                new Location(
                    new TimeOnly(12, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        messageSamples.Add(
            "Jeg tager lige en time mere fra hjemmekontoret. Er inde ca. kl 10",
            new Location[]
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
        messageSamples.Add(
            "Kommer ind på kontoret omkring kl. 11",
            new Location[]
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
        messageSamples.Add(
            "Godmorgen, jeg starter ud hjemme og kommer ind omkring kl 10",
            new Location[]
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
        messageSamples.Add(
            "Godmorgen. Jeg starter hjemme, men forventer at være på kontoret kl 10. Vi ses ✌",
            new Location[]
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
        messageSamples.Add(
            "Er inde 9:15",
            new Location[]
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
        messageSamples.Add(
            "Jeg er på kontoret cirka 09.30",
            new Location[]
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
    }


    private void AddHomeMessageSamples()
    {
        messageSamples.Add(
            "Jeg er på hjemmefra i dag",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        messageSamples.Add(
            "Godmorgen. Jeg er på hjemmekontoret idag",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        messageSamples.Add(
            "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
        messageSamples.Add(
            "Jeg er på hjemmefra i dag.",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "home")
            });
    }

    private void AddStartAndStopMessageSamples()
    {
        messageSamples.Add(
            "Jeg starter lige hjemme og er på kontoret til frokost",
            new Location[]
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
        messageSamples.Add(
            "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 30),
                    new TimeOnly(11, 15),
                    "home"),
                new Location(
                    new TimeOnly(11, 15),
                    new TimeOnly(16, 0),
                    "office")
            });
        messageSamples.Add(
            "I morgen arbejder jeg hjemmefra og stopper 11.30",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 00),
                    new TimeOnly(11, 30),
                    "home"),
            });
        messageSamples.Add(
            "Vejret gjorde lige det helt lidt mere bøvlet her til morgen. Jeg er inde omkring kvart over 9...",
            new Location[]
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
    }


    private void AddIllMessageSamples()
    {
        messageSamples.Add(
            "tager en dag under dynen",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Er hjemme med syge piger, så er lidt on/off hele dagen",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Jeg har krammet toilettet hele natten, så jeg er hjemme, og sover forhåbentligt",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Er ikke på toppen - Er on/off i dag",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Stadig ikke på toppen, men arbejder det jeg kan",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Er helt smadret - bliver under dynen, og ser om jeg kan arbejde senere",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
        messageSamples.Add(
            "Jeg er slet ikke på toppen, så jeg bliver hjemme i dag",
            new Location[]
            {
                new Location(
                    new TimeOnly(9, 0),
                    new TimeOnly(16, 0),
                    "ill")
            });
    }

    [Test]
    public void TestLocations()
    {
        foreach (var message in messageSamples)
        {
            TestLocation(message);
        }
    }

    private void TestLocation(KeyValuePair<string, Location[]> message)
    {
        var locations = mp.GetLocations(message.Key);

        Assert.That(locations.Count, Is.EqualTo(message.Value.Length), 
            "number of locations found not correct");
        
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
}