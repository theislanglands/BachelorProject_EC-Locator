using EC_locator.Core.Interfaces;
using EC_locator.Core.Models;

namespace EC_locator.Repositories;

public class LocatorRepository : ILocatorRepository
{
    public Dictionary<string, string> GetLocationKeyWordsDictionary()
    {   
        string[,] keywords =
        {
            { "sengedag", "ill" },
            {  "ikke frisk" ,"ill" },
            {  "vandret" ,"ill" },
            {  "ikke på toppen" ,"ill" },
            {  "dynen" ,"ill" },
            {  "syg" ,"ill" },
            {  "influenza" ,"ill" },
            {  "ligger syg" ,"ill" },
            {  "lagt syg" ,"ill" },
            {  "feber" ,"ill" },
            {  "sygdom" ,"ill" },
            {  "forkølelse" ,"ill" },
            {  "svimmel" ,"ill" },
            {  "kvalme" ,"ill" },
            {  "ondt i hovedet" ,"ill" },
            {  "på langs" ,"ill" },
            {  "syge" ,"ill" },
            {  "helbred" ,"ill" },
            {  "feberbarn" ,"ill" },
            {  "lægger mig" ,"ill" },
            {  "skidt" ,"ill" },
            {  "under dynen" ,"ill" },
            {  "toilet" ,"ill" },

            { "møde", "meeting" },
            
            { "hjem", "home" },
            { "hjemme", "home" },
            { "hjemmefra", "home" },
            { "på hjemmefra", "home" },
            { "tager den hjemmefra", "home" },
            { "tager jeg den hjemmefra", "home" },
            { "hjemmekontoret", "home" },
            { "hjemmeskansen", "home" },
            { "ikke på kontoret", "home" },
            { "kommer på kontoret", "home" },
            
            { "kommer ind", "office" },
            { "retur", "office"},
            {"er tilbage", "office"},
            { "på kontoret", "office" },
            { "inde", "office" },
            { "på arbejdet", "office" },
            { "ind forbi", "office" },
            { "er inde", "office" },
            { "er inde ved", "office" },
            { "kommer jeg ind", "office" },
            { "kommer i firmaet", "office" },
            { "konnes", "office" }, 

            { "holder fri", "day-off" },
            { "fri", "day-off" },
            { "holder fridag", "day-off" },
            { "fridag", "day-off" },
            { "holder weekend", "day-off" },
            { "off", "day-off" },

            { "tager ud til", "remote" },
            { "tager ned til", "remote" },
            { "er hos", "remote" },
            { "ved", "remote" }
        };
        Dictionary<string, string> keywordsDic = new Dictionary<string, string>();
        for (int i = 0; i < keywords.GetLength(0); i++)
        {
            keywordsDic.Add(keywords[i,0], keywords[i,1]);
        }

        return keywordsDic;
    }

    public Dictionary<string, TimeDefinition> GetTimeDefinitionKeywords()
    {
        Dictionary<string, TimeDefinition> timeKeywords = new Dictionary<string, TimeDefinition>();
        
        timeKeywords.Add("formiddag", new TimeDefinition("formiddag", new TimeOnly(9,0), new TimeOnly(12, 0)));
        timeKeywords.Add("eftermiddag", new TimeDefinition("eftermiddag", new TimeOnly(12,0), new TimeOnly(16, 0)));
        timeKeywords.Add("frokost", new TimeDefinition("frokost", new TimeOnly(11,15), new TimeOnly(11, 15)));
        timeKeywords.Add("middag", new TimeDefinition("middag", new TimeOnly(12,00), new TimeOnly(12, 00)));
        timeKeywords.Add("aften", new TimeDefinition("aften", new TimeOnly(16,00), new TimeOnly(21, 00)));

        return timeKeywords;
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
    
    public List<string> GetStartIndicatorKeywords()
    {
        var timeKeywords = new List<string>();
        
        timeKeywords.Add("starter");

        return timeKeywords;
    }
    
    public List<string> GetStopIndicatorKeywords()
    {
        var timeKeywords = new List<string>();
        
        timeKeywords.Add("stopper");
        timeKeywords.Add("holder");
        
        return timeKeywords;
    }
    
    
    // NOT USED
    public string[] GetSplitterKeywords()
    {
        string[] keywords =
        {
            "fra kl",
            "indtil",
            "forventer",
            "i morgen",
            "kommer",
            "derefter",
            "er væk",
            "smutter",
            "stopper ved",
            "kører",
        };
        return keywords;
    }
}