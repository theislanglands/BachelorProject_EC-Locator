using EC_locator.Core.Models;
using Parser;

namespace EC_locator.Test;

[TestFixture]
public class MessageParserTest
{
    
    MessageParser mp;

    private Dictionary<string, Location[]> messageSamples;
    // message, location[]

    [SetUp]
    public void Setup()
    {
        mp = new MessageParser();
        messageSamples = new Dictionary<string, Location[]>();
        // AddHomeMessageSamples();
        // AddHomeAndOfficeMessageSamples();
        // AddIllMessageSamples(); 
        AddStartAndStopMessageSamples();

    }
    
  

    private void AddHomeMessageSamples()
    {
        messageSamples.Add(
            "Jeg er på hjemmefra i dag",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "home") });
        messageSamples.Add(
            "Godmorgen. Jeg er på hjemmekontoret idag",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "home") });
        messageSamples.Add(
            "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "home") });
        messageSamples.Add(
            "Jeg er på hjemmefra i dag.",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "home") });
    }
    
    private void AddStartAndStopMessageSamples()
    {
        messageSamples.Add(
            "Jeg starter lige hjemme og er på kontoret til frokost",
            new Location[] { 
                new Location(
                    new TimeOnly(9, 0), 
                    new TimeOnly(11, 15), 
                    "home"),
                new Location(
                    new TimeOnly(11, 15), 
                    new TimeOnly(16, 0), 
                    "office") });
        messageSamples.Add(
            "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
            new Location[] { 
                new Location(
                    new TimeOnly(9, 30), 
                    new TimeOnly(11, 15), 
                    "home"),
                new Location(
                    new TimeOnly(11, 15), 
                    new TimeOnly(16, 0), 
                    "office") });
        messageSamples.Add(
            "I morgen arbejder jeg hjemmefra og stopper 11.30",
            new Location[] { 
                new Location(
                    new TimeOnly(9, 00), 
                    new TimeOnly(11, 30), 
                    "home"),
                });
    }
    
    private void AddIllMessageSamples()
    {
        messageSamples.Add(
            "tager en dag under dynen",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Er hjemme med syge piger, så er lidt on/off hele dagen",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Jeg har krammet toilettet hele natten, så jeg er hjemme, og sover forhåbentligt",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Er ikke på toppen - Er on/off i dag",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Stadig ikke på toppen, men arbejder det jeg kan",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Er helt smadret - bliver under dynen, og ser om jeg kan arbejde senere",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
        messageSamples.Add(
            "Jeg er slet ikke på toppen, så jeg bliver hjemme i dag",
            new Location[] { new Location(
                new TimeOnly(9, 0), 
                new TimeOnly(16, 0), 
                "ill") });
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

            for (int i = 0; i < message.Value.Length; i++)
            {
                Assert.That(locations[i].Place, Is.EqualTo(message.Value[i].Place), $"wrong location identified in message: {message.Key}");
                Assert.That(locations[i].Start, Is.EqualTo(message.Value[i].Start), $"wrong start time identified in message: {message.Key}");
                Assert.That(locations[i].End, Is.EqualTo(message.Value[i].End), $"wrong end time identified in message: {message.Key}");
            }
    }
}