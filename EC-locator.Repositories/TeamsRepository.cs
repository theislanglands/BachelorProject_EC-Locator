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
        if (_verbose)
        {
            Console.WriteLine("Fetching messages from MS Graph");
        }
        
        List<Message>? foundMessages = new();
        var date = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
        
        try
        {
            //var msg = FetchAllMessagesAsync(date, DateOnly.FromDateTime(DateTime.Now));
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
}