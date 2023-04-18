using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers.Decisions;

public class FinalNode : Node
{
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
    
    }

    public FinalNode(IOptions<VerboseOptions> settingsOptions) : base(settingsOptions)
    {
    }
}