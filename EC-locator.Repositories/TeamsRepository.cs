﻿using System.Collections;
using Microsoft.Graph;
using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Repositories;

public class TeamsRepository : ITeamsRepository
{
    private readonly IGraphHelper _graphHelper;
    private readonly bool _verbose;
    
    public TeamsRepository(IGraphHelper graphHelper, IOptions<VerboseOptions> settingsOptions)
    {
        _graphHelper = graphHelper;
        _verbose = settingsOptions.Value.Verbose;
    }
    
    public async Task<List<User>> GetUsersAsync()
    {
        List<string> excludedEmails = new List<string>
        {
            "zookort2@ecreo.dk",
            "zookort1@ecreo.dk",
            "webmuseet@ecreo.dk",
            "support@ecreo.dk",
            "stortmodelokale@ecreo.dk",
            "roundtable@ecreo.dk",
            "projekter@ecreo.dk",
            "leasymail@ecreo.dk",
            "kantinen@ecreo.dk",
            "it@ecreo.dk",
            "fod.kal@ecreo.dk",
            "backup.email@ecreo.dk",
            "bogholder@ecreo.dk"
        };

        List<User> users = new();
        try
        {
            var userPage = await _graphHelper.GetUsersAsync();

            // adding fetched users to list containing @ecroe.dk
            foreach (var user in userPage.CurrentPage)
            {
                if (user.Mail.ToLower().EndsWith("@ecreo.dk"))
                {
                    if (!excludedEmails.Contains(user.Mail))
                    {
                        users.Add(user);
                    }
                }
            }

            // If NextPageRequest is not null, there are more user available on the server
            // Access the next page: userPage.NextPageRequest.GetAsync();
            var moreAvailable = userPage.NextPageRequest != null;
            if (_verbose)
            {
                Console.WriteLine($"\nMore users available? {moreAvailable}");
            }

        }
        catch (Exception ex)
        {
            if (_verbose)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
            }
        }

        return users;
    }
    
    public async Task ListMessagesAsync()
    {
        Console.WriteLine("In list messages async");
        try
        {
            var messages = await _graphHelper.getMessagesAsync();

            // Output message details
            foreach (var message in messages.CurrentPage)
            {
                //Console.WriteLine($"User: {message.Body.Content ?? "NO CONTENT"}");
                Console.WriteLine("\n-- Message in TR --");
                Console.WriteLine($"Message ID: {message.Id}");
                Console.WriteLine($"Content type: {message.Body.ContentType.Value}");
                if (message.Body.ContentType.Value.ToString().Equals("Html"))
                {
                    Console.WriteLine("-- CONTAINS HTML -- ");
                    message.Body.Content = "replaced text";
                }

                Console.WriteLine($"Message content: {message.Body.Content}");
                // Console.WriteLine($"Message replies: {message.Replies.Count}"); // Can be null! if no replies
                Console.WriteLine($"Message lastEditedDateTime: {message.LastModifiedDateTime}");
                Console.WriteLine($"Message From.user.Id: {message.From.User.Id}");
            }
            
            // If NextPageRequest is not null, there are more user available on the server
            // Access the next page: userPage.NextPageRequest.GetAsync();
            var moreAvailable = messages.NextPageRequest != null;
            if (_verbose)
            {
                Console.WriteLine($"\nMore messages available? {moreAvailable}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }
        Environment.Exit(1);
    }

    private string ParseHtmlToText(string html)
    {
        return "hej";
    }

    public string[] GetMessages(string employeeId, DateOnly date)
    {
        if (employeeId.Equals("all"))
        {
            var concatenatedMessages = this.GetMessages("sample1", new DateOnly()).ToList();
            concatenatedMessages.AddRange(this.GetMessages("sample3", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("ill", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("is_first_location_office", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("minute indicators", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("startKeywords", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("stopKeywords", new DateOnly()).ToList());
            concatenatedMessages.AddRange(this.GetMessages("negation", new DateOnly()).ToList());

            
            Console.WriteLine();
            foreach (var message in concatenatedMessages.ToArray())
            {
                Console.WriteLine(message);
            }
            
            return concatenatedMessages.ToArray();
        }

        if (employeeId.Equals("sample1"))
        {
            string[] messages =
            {
                "Jeg er på hjemmefra i dag",
                "Godmorgen. Jeg er på hjemmekontoret idag",
                "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge",
                "Jeg er på hjemmefra i dag.",
            };
            return messages;
        }
        
        if (employeeId.Equals("sample2"))
        {
            // hjemmefra og kontor - tilføjet til test!
            string[] messages =
            {
                "Jeg bliver hjemme indtil jeg kan aflevere min cykel til service klokken 10, og så kommer jeg ind.",
                "Jeg smutter til tandlæge her klokken 12. Arbejder muligvis hjemmefra efter.",
                "Jeg tager lige en time mere fra hjemmekontoret. Er inde ca. kl 10",
                "Kommer ind på kontoret omkring kl. 11",
                "Godmorgen, jeg starter ud hjemme og kommer ind omkring kl 10",
                "Godmorgen. Jeg starter hjemme, men forventer at være på kontoret kl 10. Vi ses ✌",
                "Er inde 9:15",
                "Jeg er på kontoret cirka 09.30",
            };
            return messages;
        }

        if (employeeId.Equals("sample3"))
        {
            // Diverse - Meeting, Undefined
            string[] messages =
            {
                "Thomas, Gorm og jeg tager ned til Nørgaard Mikkelsen til møde, forventer at være retur 10.30",
                "Starter til møde hos NM. Er tilbage lidt over 10.",
                "Morn - jeg starter hos lægen og kører på Popermo efterfølgende",
                "Er til møde ved Alumeco indtil 11.30 i morgen og arbejder hjemme fra derefter.",
                "Jeg tager hjem og arbejder efter zoo mødet  Hovedet driller lidt i dag. ",
            };
            return messages;
        }
        
        if (employeeId.Equals("ill"))
        {
            // syg - Tilføjet til unit test
            string[] messages =
            {
                "tager en dag under dynen",
                "Er hjemme med syge piger, så er lidt on/off hele dagen",
                "Jeg har krammet toilettet hele natten, så jeg er hjemme, og sover forhåbentligt",
                "Er ikke på toppen - Er on/off i dag",
                "Stadig ikke på toppen, men arbejder det jeg kan",
                "Er helt smadret - bliver under dynen, og ser om jeg kan arbejde senere",
                "Jeg er slet ikke på toppen, så jeg bliver hjemme i dag",
                "jeg er syg i dag",
            };
            return messages;
        }
        
        if (employeeId.Equals("is_first_location_office"))
        {
            // tilføjet til test
            string[] messages =
            {
                "0930 på kontoret"
            };
            return messages;
        }
        
        if (employeeId.Equals("minute indicators"))
        {
            // tilføjet til test
            string[] messages =
            {
                "Vejret gjorde lige det helt lidt mere bøvlet her til morgen. Jeg er inde omkring kvart over 9...",
            };
            return messages;
        }
        
        if (employeeId.Equals("startKeywords"))
        {
            // tilføjet til test
            string[] messages =
            {
                "Jeg starter lige hjemme og er på kontoret til frokost",
                "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
            };
            return messages;
        }
        
        if (employeeId.Equals("stopKeywords"))
        {
            // tilføjet til test
            string[] messages =
            {
                "I morgen arbejder jeg hjemmefra og stopper 11.30",
            };
            return messages;
        }
        
        if (employeeId.Equals("wip"))
        {
            string[] messages =
            {
                "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
                
            };
            return messages;
        }
        
        if (employeeId.Equals("negation"))
        {
            // added to test
            string[] messages =
            {
                "Kommer ikke på kontoret", // ikke negering

            };
            return messages;
        }
        
        // SE PÅ DEM HER!
        if (employeeId.Equals("outliers"))
        {
            string[] messages =
            {
                "Jeg er på kontoret inden frokost. Er på hjemmefra", // omvendt rækkefølge - kan ikke håndtere -> false prediction
                "Jeg tager lige en time eller to hjemmefra, her til morgen", // utvetydig -> kan ikke finde kontor
                
                "Lynet skal lige have en gang service, så er først på pinden 9.15-9.30", // kan ikke specificere tidspunkt - to tidspunkter efter hinanden - slet første?
                "Otto er desværre blevet syg, så jeg holder hjemmefronten indtil backup ankommer. Er på kontoret inden 11", // Syg, men på kontoret
                
                "Jeg holder for i dag", // HOLDER = holder fri fra time=now -> location "home" fra NU af!
                "Jeg er forresten stadig på hjemmefra - er måske på kontoret en af de kommende dage", // -> undefined no time keyword - Maybe "Future" keywords!

            };
            return messages;
        }
        
        if (employeeId.Equals("un-precise"))
        {
            string[] messages =
            {
                "Skal lige hente Noah og køre ham ud til de gamle. Han har lidt ondt i maven. Kommer på konnes igen efter.",
            };
            return messages;
        }

        return null;
    }
    
    public async Task<ArrayList> GetMessagesAsync()
    {
        ArrayList returnArray = new ArrayList();
        try
        {
            
            var userPage = await _graphHelper.GetUsersAsync();

            // Output each users's details
            foreach (var user in userPage.CurrentPage)
            {
                returnArray.Add(user);
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // userPage.NextPageRequest.GetAsync();
            var moreAvailable = userPage.NextPageRequest != null;

            Console.WriteLine($"\nMore users available? {moreAvailable}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
        }

        return returnArray;
    }
}