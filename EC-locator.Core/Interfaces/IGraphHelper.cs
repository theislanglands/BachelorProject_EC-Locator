using Microsoft.Graph;

namespace EC_locator.Repositories;

public interface IGraphHelper
{
    Task<IGraphServiceUsersCollectionPage> GetUsersAsync();
    Task<IChannelMessagesCollectionPage> getMessagesAsync();
    Task<IChannelMessagesCollectionPage> getCalendarEventsAsync(string employeeId);
}