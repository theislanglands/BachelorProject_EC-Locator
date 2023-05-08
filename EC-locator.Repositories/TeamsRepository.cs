using System.Text.RegularExpressions;

using Microsoft.Graph;
using Microsoft.Extensions.Options;

using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using Message = EC_locator.Core.Models.Message;

namespace EC_locator.Repositories;

public class TeamsRepository : ITeamsRepository
{
    private readonly IGraphHelper _graphHelper;
    private readonly bool _verbose;
    private readonly string[] _excludedUsers;

    public TeamsRepository(IGraphHelper graphHelper, IOptions<VerboseOptions> settingsOptions,
        IOptions<UsersOptions> usersOptions)
    {
        _graphHelper = graphHelper;
        _verbose = settingsOptions.Value.Verbose;
        _excludedUsers = usersOptions.Value.ExcludedUsers;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        List<User> users = new();
        try
        {
            var userPage = await _graphHelper.GetUsersAsync();

            // adding fetched users to list containing @ecroe.dk
            foreach (var user in userPage.CurrentPage)
            {
                if (user.Mail.ToLower().EndsWith("@ecreo.dk"))
                {
                    if (!_excludedUsers.Contains(user.Mail))
                    {
                        users.Add(user);
                    }
                }
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

    public async Task<List<Message>?> GetRecentMessagesAsync(string employeeId)
    {
        List<Message>? foundMessages = new();
        var date = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

        if (_verbose)
        {
            Console.WriteLine("Fetching messages from MS Graph");
        }

        try
        {
            var messages = await _graphHelper.GetMessagesAsync(date);

            foreach (var message in messages.CurrentPage)
            {
                // Check if message sender match the employee ID 
                if (!message.From.User.Id.Equals(employeeId))
                {
                    continue;
                }

                // check if content is html and convert to plain text             
                if (message.Body.ContentType.Value.ToString().Equals("Html"))
                {
                    message.Body.Content = ParseHtmlToText(message.Body.Content);
                }

                List<Message>? replies = null;

                // check if message contains replies
                if (message.Replies.Count != 0)
                {
                    replies = new();
                    foreach (var reply in message.Replies.CurrentPage)
                    {
                        replies.Add(new Message()
                        {
                            Content = ParseHtmlToText(reply.Body.Content),
                            TimeStamp = reply.LastModifiedDateTime.Value.DateTime,
                            UserId = reply.From.User.Id
                        });
                    }

                    replies.Sort();
                }

                DateTime timeStamp;
                if (message.LastEditedDateTime != null)
                {
                    timeStamp = message.LastEditedDateTime.Value.DateTime;
                }
                else
                {
                    timeStamp = message.CreatedDateTime.Value.DateTime;

                }

                Console.WriteLine(timeStamp);
    
                // adding to found messages
                foundMessages.Add(new Message()
                {
                    Content = message.Body.Content,
                    TimeStamp = timeStamp,
                    UserId = employeeId,
                    Replies = replies
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }

        if (foundMessages.Count != 0)
        {
            return foundMessages;
        }

        return null;
    }
    
    public async Task<List<Message>> FetchAllMessagesAsync(DateOnly fromDate, DateOnly toDate)
    {
        List<Message> fetchedMessages = new();
        if (_verbose)
        {
            Console.WriteLine($"Fetching all messages from {fromDate} to {toDate}");
        }

        try
        {
            bool moreMessages = true;
            var messages = await _graphHelper.GetMessagesAsync(fromDate);

            while (moreMessages)
            {
                // Output message details
                foreach (var message in messages.CurrentPage)
                {
                    if (message.Body.Content == null)
                    {
                        continue;
                    }

                    Message fetchedMessage = new();
                    
                    fetchedMessage.Content = ParseHtmlToText(message.Body.Content);
                    fetchedMessage.TimeStamp = message.LastModifiedDateTime.Value.LocalDateTime;
                    fetchedMessage.UserId = message.From.User.Id;

                    if (message.Replies.Count != 0)
                    {
                        Message replyMessage = new();
                        foreach (var reply in message.Replies.CurrentPage)
                        {
                            if (reply.Body.Content == null)
                            {
                                continue;
                            }

                            replyMessage.Content = ParseHtmlToText(reply.Body.Content);
                            replyMessage.TimeStamp = reply.LastModifiedDateTime.Value.LocalDateTime;
                            replyMessage.UserId = reply.From.User.Id;

                            if (reply.From.User.Id.Equals(message.From.User.Id))
                            {
                                if (fetchedMessage.Replies == null)
                                {
                                    fetchedMessage.Replies = new();
                                }

                                if (!fetchedMessage.Replies.Contains(replyMessage))
                                {
                                    fetchedMessage.Replies.Add(replyMessage);
                                }

                            }
                        }

                        // stop fetching if desired date has been reached
                        if (DateOnly.FromDateTime(fetchedMessage.TimeStamp) > toDate)
                        {
                            moreMessages = false;
                            break;
                        }

                        fetchedMessages.Add(fetchedMessage);
                    }
                }

                // fetching next page
                if (messages.NextPageRequest != null && moreMessages)
                {
                    messages = await messages.NextPageRequest.GetAsync();
                }
                else
                {
                    moreMessages = false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }

        return fetchedMessages;
    }

    private string ParseHtmlToText(string html)
    {
        string plainText;
        
        // remove tags and entities
        plainText = Regex.Replace(html, "<.*?>|&.*?;", string.Empty);
        
        // remove, tabs, newline and carriage return
        plainText = Regex.Replace(plainText, "(\t|\r|\n)+", string.Empty);
        
        return plainText;
    }
    

    public List<Message>? GetMessageSamples(string employeeId)
    {
        string sampleCode = "wip";

        List<Message> messages = new();
        var samples = GetSamples(sampleCode);
        int i = 1;
        foreach (var sample in samples)
        {
            messages.Add(new Message(sample, employeeId, DateTime.Now.AddMinutes(i), new List<Message>()));
            i++;
        }

        if (messages.Count == 0)
        {
            return null;
        }

        return messages;
    }

    public string[] GetSamples(string employeeId)
    {
        if (employeeId.Equals("all"))
        {
            var concatenatedMessages = this.GetSamples("sample1").ToList();
            concatenatedMessages.AddRange(this.GetSamples("sample3").ToList());
            concatenatedMessages.AddRange(this.GetSamples("ill").ToList());
            concatenatedMessages.AddRange(this.GetSamples("is_first_location_office").ToList());
            concatenatedMessages.AddRange(this.GetSamples("minute indicators").ToList());
            concatenatedMessages.AddRange(this.GetSamples("startKeywords").ToList());
            concatenatedMessages.AddRange(this.GetSamples("stopKeywords").ToList());
            concatenatedMessages.AddRange(this.GetSamples("negation").ToList());
            concatenatedMessages.AddRange(this.GetSamples("undefined").ToList());

            if (_verbose)
            {
                Console.WriteLine();
                foreach (var message in concatenatedMessages.ToArray())
                {
                    Console.WriteLine(message);
                }
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
                "Morn. Jeg er ved Popermo i dag",
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
                // off - home - office - 11_15 = > delete off

                "Godmorgen. Jeg starter hjemme, men forventer at være på kontoret kl 10. Vi ses ✌",
                // off - home - office - 10 = > delete off

                "Er inde 9:15",
                "Jeg er på kontoret cirka 09.30",
                "Er hjemmefra med Otto indtil backup kommer Jeg er inde inden frokost",
                "0920", // home 9- office start 920 -> time with no location => insert office
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
                // off - meeting - Office - 11_15 = > delete off

                "Morn - jeg starter hos lægen og kører på Popermo efterfølgende",
                // off - remote - No-time!

                "Er til møde ved Alumeco indtil 11.30 i morgen og arbejder hjemme fra derefter.",
                "Jeg tager hjem og arbejder efter zoo mødet  Hovedet driller lidt i dag. ",
                "Jeg er i Nørresundby hele dagen i morgen hos Continia sammen med Martin, Simone og Jesper",
            };
            return messages;
        }

        if (employeeId.Equals("ill"))
        {
            // syg & børn syge - Tilføjet til unit test
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
                "Det er som om min forkølelse er blusset op igen, så jeg er nok først på senere.",

                "Den lille er stadigvæk syg, arbejde det jeg kan ind i mellem",
                "Felix er desværre syg med feber så tager den hjemmefra, så meget det er muligt 🤒",
                "Otto er desværre blevet syg, så jeg holder hjemmefronten indtil backup ankommer. Er på kontoret inden 11", // Syg, men på kontoret
                "Jeg tager en fridag i dag med Viggo på sygehuset, er på telefonen hvis der er noget",
                "Jeg gennemgår lige nogle øv ting her for tiden, med en masse lægebesøg og smerter. Så jeg er ikke lige så aktiv på kontoret de dage her."

                
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
                "Godmorgen, jeg har ikke rigtig sovet pga maven, ser om jeg ka få indhentet lidt søvn her til formiddag. Er på hjemmefra senest ved middagstid",

            };
            return messages;
        }

        if (employeeId.Equals("startKeywords"))
        {
            // tilføjet til test
            string[] messages =
            {
                "Jeg starter lige hjemme og er på kontoret til frokost",
                // off - home - Office - 11_15
                "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
                // off - home - 9:30 - Office - 11_15

                // hvis off er efterfulgt af to locations uden en time => delete
            };
            return messages;
        }

        if (employeeId.Equals("stopKeywords"))
        {
            // tilføjet til test
            string[] messages =
            {
                "I morgen arbejder jeg hjemmefra og stopper 11.30",
                "Arbejder hjemme i dag og går fra ved frokosttid. God påske ",
                "Jeg holder weekend ved 14 tiden God påske til jer der går på ferie",
                "Lukker ned kl. 14"
            };
            return messages;
        }

        if (employeeId.Equals("wip"))
        {
            string[] messages =
            {
                "Vi bringer en rettelse - jeg skal ikke til demo hos Popermo, så jeg tager den fra hjemmekontoret"
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

        if (employeeId.Equals("undefined"))
        {
            // added to test
            string[] messages =
            {
                "prut fis kanon",
                "Morn - jeg er startet hjemmefra og kører på Popermo efterfølgende. God weekend til alle jeg ikke når at hilse på "
            };
            return messages;
        }

        if (employeeId.Equals("tomorrow"))
        {
            // added to test
            string[] messages =
            {
                "Jeg er på hjemmefra i morgen.",
                "Jeg er først på kontoret omkring kl 10 i morgen",
            };
            return messages;
        }

        // SE PÅ DEM HER!
        if (employeeId.Equals("outliers"))
        {
            string[] messages =
            {
                "Jeg døjer stadig med øjenmigræne hvilket gør at det slører for mine øjne. Der er gode og dårlige timer. Jeg håber at komme ind på kontoret til formiddag",
                "Jeg holder for i dag", // HOLDER = holder fri fra time=now -> location "home" fra NU af!
                "Jeg er på kontoret inden frokost. Er på hjemmefra", // omvendt rækkefølge - kan ikke håndtere -> false prediction
                "Jeg tager lige en time eller to hjemmefra, her til morgen", // utvetydig -> kan ikke finde kontor
                "Lynet skal lige have en gang service, så er først på pinden 9.15-9.30", // kan ikke specificere tidspunkt - to tidspunkter efter hinanden - slet første?
                "Otto er desværre blevet syg, så jeg holder hjemmefronten indtil backup ankommer. Er på kontoret inden 11", // Syg, men på kontoret
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
}