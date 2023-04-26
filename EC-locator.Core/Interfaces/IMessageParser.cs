using EC_locator.Core.Models;

namespace EC_locator.Core.Interfaces;

public interface IMessageParser
{
    bool ContainsTomorrow(string message);
    List<Location> GetLocations(Message msg);
}