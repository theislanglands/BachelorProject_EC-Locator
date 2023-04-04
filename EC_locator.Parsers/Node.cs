using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;
using Location = EC_locator.Core.Models.Location;

namespace EC_locator.Parsers;

public abstract class Node

{
    protected readonly bool Verbose;

    protected Node(IOptions<VerboseOptions> settingsOptions)
    {
        Verbose = settingsOptions.Value.Verbose;
    }
    public abstract void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times);
}