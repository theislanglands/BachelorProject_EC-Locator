using System.Text;
using DateTime = System.DateTime;

namespace EC_locator.Core.Models;

public class Message : IComparable
{
    public string Content { get; set; }
    public string UserId { get; set; }
    public DateTime TimeStamp { get; set; }

    public List<Message>? Replies { get; set; }

    public Message(string content, string userId, DateTime timeStamp, List<Message>? replies)
    {
        Content = content;
        UserId = userId;
        TimeStamp = timeStamp;
        Replies = replies;
    }

    public Message()
    {
    }
    
    public override string ToString()
    {
        StringBuilder presentation = new StringBuilder();
        presentation.Append($"{Content}\n");
        presentation.Append($"UserId: {UserId}\n");
        presentation.Append($"TimeStamp: {TimeStamp}\n");

        if (Replies != null)
        {
            presentation.Append("\n  -- Replies --\n");
            foreach (var reply in Replies)
            {
                presentation.AppendLine(reply.ToString());
            }
        }

        return presentation.ToString();
    }

    public int CompareTo(object? obj)
    {
        Message? otherMessage = obj as Message;
        
        if (otherMessage == null)
        {
            return -1;
        }
        
        return DateTime.Compare(TimeStamp, otherMessage.TimeStamp); 
    }
}