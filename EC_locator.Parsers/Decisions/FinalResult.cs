using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers.Decisions;

public class FinalResult : Node
{
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
    
    }

    public FinalResult(IOptions<VerboseOptions> settingsOptions) : base(settingsOptions)
    {
    }
}