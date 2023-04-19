using Microsoft.Graph;

namespace EC_locator.Repositories;

public interface IGraphHelper
{
    Task<IGraphServiceUsersCollectionPage> GetUsersAsync();
    //Task<IChannelMessagesCollectionPage> getMessagesAsync();
    Task<IUserCalendarViewCollectionPage> getCalendarEventsAsync(string employeeId);

    Task<IChatMessageDeltaCollectionPage> GetMessagesAsync(DateOnly date);
}