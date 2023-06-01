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
            {  "sengeliggende", "ill" }, {  "sengen", "ill" }, {  "i seng", "ill" }, {  "sengedag", "ill" },
            {  "dynen" ,"ill" }, {  "på langs" ,"ill" }, {  "vandret" ,"ill" }, {  "lægger mig" ,"ill" },
            { "lukker øjnene", "ill"},

            {  "syg" ,"ill" }, {  "ikke frisk" ,"ill" }, {  "ikke på toppen" ,"ill" }, {  "skidt" ,"ill" }, {  "helbred" ,"ill" },
            { "sløj", "ill" }, { "ikke blevet meget bedre", "ill" },{ "elendighed", "ill" }, { "smadret", "ill"}, { "ikke ovenpå", "ill"},
            { "lagt ned", "ill"},  { "fået det værre", "ill"},{ "ikke helt frisk", "ill"}, { "smittet", "ill"},{ "ikke helt på toppen", "ill"},
            { "hårdt ramt", "ill"}, { "på toppen", "ill"},{ "hænger med mulen", "ill"}, { "langt nede", "ill"}, { "ikke rask", "ill"},
            { "snøvsen", "ill"}, { "trukket nitten", "ill"}, { "fået det noget værre", "ill"},
            
            {  "influenza" ,"ill" }, {  "feber" ,"ill" }, {  "forkølelse" ,"ill" }, {  "svimmel" ,"ill" }, {  "kvalme" ,"ill" }, 
            {  "migræne" ,"ill" }, {  "toilet" ,"ill" }, {  "skoldkop" ,"ill" },{ "hovedpine", "ill" }, { "krads hals", "ill" },
            { "maveproblemer", "ill" }, { "smerter", "ill" }, { "kastet op", "ill" }, { "kaste op", "ill" }, { "opkast", "ill" },
            { "kaster stadig op", "ill"}, { "halsen", "ill"}, { "tandpine", "ill"}, { "hoved driller", "ill"},{ "snotter", "ill"}, 
            { "hoster", "ill"},{ "ondt", "ill"}, { "sat til", "ill"},{ "covid", "ill"},{ "nedlagt med Corona", "ill"},{ "positiv corona", "ill"},
            { "hals", "ill"},
            
            { "lægebesøg", "ill" },
            
            // KIDS ILL KEYWORDS
            {  "den lille" ,"kidsIll" }, {  " de små" ,"kidsIll" }, {  "familie" ,"kidsIll" },
            {  "børn" ,"kidsIll" }, {  "barn" ,"kidsIll" }, {  "1 årig" ,"kidsIll" },
            {  "pige" ,"kidsIll" }, {  "dreng" ,"kidsIll" }, {  "hende" ,"kidsIll" },
            {  "unger" ,"kidsIll" }, {  "søn " ,"kidsIll" }, {  "datter" ,"kidsIll" },
            {  "Felix" ,"kidsIll" }, {  "Otto" ,"kidsIll" }, {  "Noah" ,"kidsIll" }, {  "Isaac" ,"kidsIll" }, {  "Viggo" ,"kidsIll" }, { "Saga", "kidsIll"},
            
            // MEETING KEYWORDS
            { "møde ", "meeting" },
            { "møder", "meeting" },
            
            // HOME KEYWORDS
            { "hjem", "home" },{ "home", "home" },
            { "ikke på kontoret", "home" },
            { "aarup", "home" }, { "\"5560\"", "home"},
            { "gemakker", "home"},
            
            // OFFICE KEYWORDS
            { "kommer ind", "office" }, { "kommer jeg ind", "office" }, { " er inde ", "office" }, { "ind forbi", "office" },
            { "komme ind", "office" },
            { "retur", "office"}, { "er tilbage", "office"}, 
            { "at være der", "office"}, { "på kontoret", "office" }, { "på arbejdet", "office" },
            { "kommer i firmaet", "office" }, { "konnes", "office" }, { "er mødt", "office"}, { "på pinden", "office"},
            
            //NYE
            { "kontu", "office" },

            // OFF KEYWORDS
            { "fri", "off" }, { "off", "off" }, { "afspadserer", "off"},
            { "holder", "off" }, { "lukker ned", "off" },{ "for i dag", "off" }, { "går fra", "off" }, { "stopper", "off" }, { "smutter", "off" },
            { "tak for i dag", "off" }, { "God weekend", "off" },
            { "starter", "off" },
            { "søvn", "off" },
            
            // REMOTE KEYWORDS
            { "tager ud til", "remote" },
            { "tager ned til", "remote" },
            { "hos ", "remote" },
            { "kører på", "remote" },
            { "er ved", "remote" },
            { "kører ind til", "remote" },
            { "drager", "remote"},
            
            // nye
            { "kører til", "remote" }
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
        
        timeKeywords.Add("formiddag", new TimeOnly(12,00));
        timeKeywords.Add("eftermiddag", new TimeOnly(12,0));
        timeKeywords.Add("frokost", new TimeOnly(11, 15));
        timeKeywords.Add("middag", new TimeOnly(12,00));
        //timeKeywords.Add("aften", new TimeOnly(16,00));

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