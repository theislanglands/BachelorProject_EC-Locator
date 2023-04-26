namespace EC_locator.Repositories;
using EC_locator.Core.Interfaces;
using EC_locator.Core.SettingsOptions;
using Microsoft.Extensions.Options;

public class LocatorRepositoryLocal : ILocatorRepository
{
    private readonly bool _verbose;
    
    public LocatorRepositoryLocal(IOptions<VerboseOptions> verboseSettings)
    {
        _verbose = verboseSettings.Value.Verbose;
    }
    
    public Dictionary<string, string> GetLocationKeywords()
    {   
        string[,] keywords =
        {
            // ILL KEYWORDS
            {  "seng", "ill" }, {  "dynen" ,"ill" }, {  "på langs" ,"ill" }, {  "vandret" ,"ill" }, {  "lægger mig" ,"ill" },
            
            {  "syg" ,"ill" }, {  "ikke frisk" ,"ill" }, {  "ikke på toppen" ,"ill" }, {  "skidt" ,"ill" }, {  "helbred" ,"ill" },
            
            {  "influenza" ,"ill" }, {  "feber" ,"ill" }, {  "forkølelse" ,"ill" }, {  "svimmel" ,"ill" }, {  "kvalme" ,"ill" }, 
            {  "ondt i hovedet" ,"ill" }, {  "migræne" ,"ill" }, {  "toilet" ,"ill" },
            
            // KIDS ILL KEYWORDS
            {  "den lille" ,"kidsIll" }, {  "de små" ,"kidsIll" }, {  "familie" ,"kidsIll" },
            {  "børn" ,"kidsIll" }, {  "barn" ,"kidsIll" },
            {  "pige" ,"kidsIll" }, {  "dreng" ,"kidsIll" },
            {  "unger" ,"kidsIll" }, {  "søn" ,"kidsIll" }, {  "datter" ,"kidsIll" },
            {  "Felix" ,"kidsIll" },
            {  "Otto" ,"kidsIll" },
            
            // MEETING KEYWORDS
            { "møde", "meeting" },
            
            // HOME KEYWORDS
            { "hjem", "home" },
            { "ikke på kontoret", "home" },
            
            // OFFICE KEYWORDS
            { "kommer ind", "office" }, { "kommer jeg ind", "office" }, { "inde", "office" }, { "ind forbi", "office" },
            { "komme ind", "office" },
            
            { "retur", "office"},
            { "er tilbage", "office"},
            
            { "på kontoret", "office" },
            { "på arbejdet", "office" },
            { "kommer i firmaet", "office" },
            { "konnes", "office" }, 

            // OFF KEYWORDS
            { "fri", "off" },
            { "off", "off" },
            { "går fra", "off" },
            { "stopper", "off" },
            { "smutter", "off" },
            { "holder", "off" },
            { "lukker ned", "off" },
            { "starter", "off" },

            // REMOTE KEYWORDS
            { "tager ud til", "remote" },
            { "tager ned til", "remote" },
            { "hos", "remote" },
        };
        
        // replacing string[] with dictionary
        Dictionary<string, string> keywordsDic = new Dictionary<string, string>();
        for (int i = 0; i < keywords.GetLength(0); i++)
        {
            keywordsDic.Add(keywords[i,0], keywords[i,1]);
        }

        return keywordsDic;
    }
    
    public Dictionary<string, TimeOnly> GetTimeKeywords()
    {
        var timeKeywords = new Dictionary<string, TimeOnly>();
        
        timeKeywords.Add("formiddag", new TimeOnly(9,0));
        timeKeywords.Add("eftermiddag", new TimeOnly(12,0));
        timeKeywords.Add("frokost", new TimeOnly(11, 15));
        timeKeywords.Add("middag", new TimeOnly(12,00));
        timeKeywords.Add("aften", new TimeOnly(16,00));

        return timeKeywords;
    }
    
    public Dictionary<string, double> GetMinuteIndicators()
    {
        var timeKeywords = new Dictionary<string, double>();
        
        timeKeywords.Add("kvart over", 15);
        timeKeywords.Add("kvart i", -15);
        timeKeywords.Add("halv", -30);
        
        // "forsinket" - add minutes to current time
        
        return timeKeywords;
    }
}