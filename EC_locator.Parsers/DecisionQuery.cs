using EC_locator.Core.Models;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

namespace EC_locator.Parsers;

public class DecisionQuery : Node
{
  
    public string? Title { get; set; }
    public Node? Positive { get; set; }
    public Node? Negative { get; set; }
    public Func<SortedList<int, Location>, SortedList<int, TimeOnly>, bool>? Test { get; set; }

    public DecisionQuery(IOptions<VerboseOptions> settingsOptions) : base(settingsOptions)
    {
        
    }
    public override void Perform(SortedList<int, Location> locations, SortedList<int, TimeOnly> times)
    {
        bool result = Test(locations, times);
        string resultAsString = result ? "yes" : "no";

        if (Verbose)
        {
            Console.WriteLine($"\t- Decision: {this.Title}? {resultAsString}");
        }

        if (result) Positive.Perform(locations, times);
        else Negative.Perform(locations, times);
    }
}

