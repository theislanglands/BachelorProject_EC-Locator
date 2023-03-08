using EClocator.Core.Interfaces;
using EClocator.Core.Models;

namespace EC_locator.Repositories;

public class LocatorRepository : ILocatorRepository
{
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

    public string[,] GetLocationKeywords()
    {
        string[,] keywords =
        {
            { "syg", "sengedag" },
            { "syg", "ikke frisk" },
            { "syg", "vandret" },
            { "syg", "ikke på toppen" },
            { "syg", "dynen" },
            { "syg", "syg" },
            { "syg", "influenza" },
            { "syg", "ligger syg" },
            { "syg", "lagt syg" },
            { "syg", "feber" },
            { "syg", "sygdom" },
            { "syg", "forkølelse" },
            { "syg", "svimmel" },
            { "syg", "kvalme" },
            { "syg", "ondt i hovedet" },
            { "syg", "på langs" },
            { "syg", "syge" },
            { "syg", "helbred" },
            { "syg", "feberbarn" },
            { "syg", "lægger mig" },
            { "syg", "skidt" },
            { "syg", "under dynen" },

            { "møde", "møde" },

            { "hjemme", "hjemme" },
            { "hjemme", "hjemmefra" },
            { "hjemme", "på hjemmefra" },
            { "hjemme", "tager den hjemmefra" },
            { "hjemme", "tager jeg den hjemmefra" },
            { "hjemme", "hjemmekontoret" },
            { "hjemme", "hjemmeskansen" },
            { "hjemme", "ikke på kontoret" },


            { "konter", "på kontoret" },
            { "konter", "inde" },
            { "konter", "ind" },
            { "konter", "på arbejdet" },
            { "konter", "ind forbi" },
            { "konter", "er inde" },
            { "konter", "er inde ved" },
            { "konter", "kommer ind" },
            { "konter", "kommer i firmaet" },

            { "fri", "holder fri" },
            { "fri", "fri" },
            { "fri", "holder" },
            { "fri", "fridag" },
            { "fri", "holder weekend" },
            { "fri", "off" },

            { "remote", "tager ud til" },
            { "remote", "er på" },
            { "remote", "er hos" }
        };
        return keywords;
    }

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

            { "møde", "meeting" },

            { "hjemme", "home" },
            { "hjemmefra", "home" },
            { "på hjemmefra", "home" },
            { "tager den hjemmefra", "home" },
            { "tager jeg den hjemmefra", "home" },
            { "hjemmekontoret", "home" },
            { "hjemmeskansen", "home" },
            { "ikke på kontoret", "home" },

            { "på kontoret", "office" },
            { "inde", "office" },
            //{ "ind", "office" },
            { "på arbejdet", "office" },
            { "ind forbi", "office" },
            { "er inde", "office" },
            { "er inde ved", "office" },
            { "kommer ind", "office" },
            { "kommer jeg ind", "office" },
            { "kommer i firmaet", "office" },

            { "holder fri", "day-off" },
            { "fri", "day-off" },
            { "holder", "day-off" },
            { "fridag", "day-off" },
            { "holder weekend", "day-off" },
            { "off", "day-off" },

            { "tager ud til", "remote" },
            //{ "er på", "remote" },
            { "er hos", "remote" },
            { "ved", "remote" }
        };
        Dictionary<string, string> keywordsDic = new Dictionary<string, string>();
        for (int i = 0; i < keywords.GetLength(0); i++)
        {
            //Console.WriteLine($"key {keywords[i,0]} value {keywords[i,1]}");
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
}