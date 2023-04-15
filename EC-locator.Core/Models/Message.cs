using System.Text;
using DateTime = System.DateTime;

namespace EC_locator.Core.Models;

public class Message : IComparable
{
    public string Content { get; set; }
    public string UserId { get; set; }
    public DateTime TimeStamp { get; set; }

    public Message(string content, string userId, DateTime timeStamp)
    {
        Content = content;
        UserId = userId;
        TimeStamp = timeStamp;
    }

    public Message()
    {
    }
    
    public override string ToString()
    {
        StringBuilder presentation = new StringBuilder();
        presentation.Append($"Content: {Content}\n");
        presentation.Append($"UserId: {UserId}\n");
        presentation.Append($"TimeStamp: {TimeStamp}\n");
        return presentation.ToString();
    }

    public int CompareTo(object? obj)
    {
        Message? otherMessage = obj as Message;
        
        if (otherMessage == null)
        {
            return 1;
        }

        Console.WriteLine(this.TimeStamp);
        Console.WriteLine(otherMessage.TimeStamp);
        Console.WriteLine(DateTime.Compare(this.TimeStamp, otherMessage.TimeStamp));
        
        return DateTime.Compare(this.TimeStamp, otherMessage.TimeStamp); 
    }
}