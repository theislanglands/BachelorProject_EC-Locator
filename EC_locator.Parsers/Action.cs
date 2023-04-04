using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class Action : Node

{
    public string Title { get; set; }
    public Node GoTo { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool> PerformAction { get; set; }
    public Action(IOptions<VerboseOptions> settingsOptions) : base(settingsOptions)
    {
    }
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        if (Verbose)
        {
            Console.WriteLine($"\t- Action: {this.Title}");
        }

        this.PerformAction(locations, times);
        GoTo.Perform(locations, times);
    }

    
}