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
            {  "sengeliggende", "ill" }, {  "sengen", "ill" }, {  "i seng", "ill" },
            {  "dynen" ,"ill" }, {  "på langs" ,"ill" }, {  "vandret" ,"ill" }, {  "lægger mig" ,"ill" },

            {  "syg" ,"ill" }, {  "ikke frisk" ,"ill" }, {  "ikke på toppen" ,"ill" }, {  "skidt" ,"ill" }, {  "helbred" ,"ill" },
            
            {  "influenza" ,"ill" }, {  "feber" ,"ill" }, {  "forkølelse" ,"ill" }, {  "svimmel" ,"ill" }, {  "kvalme" ,"ill" }, 
            {  "migræne" ,"ill" }, {  "toilet" ,"ill" },
            //{  "ondt i hovedet" ,"ill" }
            
            // KIDS ILL KEYWORDS
            {  "den lille" ,"kidsIll" }, {  " de små" ,"kidsIll" }, {  "familie" ,"kidsIll" },
            {  "børn" ,"kidsIll" }, {  "barn" ,"kidsIll" },
            {  "pige" ,"kidsIll" }, {  "dreng" ,"kidsIll" },
            {  "unger" ,"kidsIll" }, {  "søn " ,"kidsIll" }, {  "datter" ,"kidsIll" },
            {  "Felix" ,"kidsIll" },
            {  "Otto" ,"kidsIll" },
            {  "Noah" ,"kidsIll" },
            {  "Isaac" ,"kidsIll" },
            
            // MEETING KEYWORDS
            { "møde ", "meeting" },
            { "mødet", "meeting" },
            
            // HOME KEYWORDS
            { "hjem", "home" },
            { "ikke på kontoret", "home" },
            
            // OFFICE KEYWORDS
            { "kommer ind", "office" }, { "kommer jeg ind", "office" }, { " er inde ", "office" }, { "ind forbi", "office" },
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
            { "for i dag", "off" },
            // 

            // REMOTE KEYWORDS
            { "tager ud til", "remote" },
            { "tager ned til", "remote" },
            { "hos ", "remote" },
            
            // NEW KEYWORDS
            
            // tilføjet til test
            { "ikke blevet meget bedre", "ill" },
            { "sløj", "ill" },
            { "hovedpine", "ill" },
            { "krads hals", "ill" },
            { "elendighed", "ill" },
            { "maveproblemer", "ill" },
            { "lægebesøg", "ill" },
            { "smerter", "ill" },
            { "kastet op", "ill" },
            { "kaste op", "ill" },
            { "opkast", "ill" },
            
            { "tak for i dag", "off" },
            { "God weekend", "off" },
            { "søvn", "off" },
            
            
            { "kører på", "remote" },
            { "er ved", "remote" },
            
            {  "Viggo" ,"kidsIll" },
            {  "hende" ,"kidsIll" },
            
            
            // SKAL TESTES
            {  "1 årig" ,"kidsIll" },
            { "kører ind til", "remote" },
            { "smadret", "ill"},
            { "ikke ovenpå", "ill"},
            { "halsen", "ill"},
            { "tandpine", "ill"},
            { "lagt ned", "ill"},
            { "fået det værre", "ill"},
            
            // 1/8 til 1/9
            { "møder", "office"},
            { "lukker øjnene", "ill"},
            { "hoved driller", "ill"},
            { "er mødt", "office"},
            
            { "på pinden", "office"},
            { "afspadserer", "off"},
            
            // 1/9 - 1/10
            { "ikke helt på toppen", "ill"},
            { "hårdt ramt", "ill"},
            { "ondt", "ill"}, // // rigtig ondt
            { "på toppen", "ill"}, // ikke er på toppen
            { "home", "home" },
            { "sat til", "ill"},
            { "snotter", "ill"},
            { "hoster", "ill"},
            { "ikke helt frisk", "ill"},
            { "smittet", "ill"},
            
            // 1-11 - 1/12
            { "drager", "remote"},
            { "kaster stadig op", "ill"},
            { "snøvsen", "ill"},
            { "covid", "ill"},
            { "trukket nitten", "ill"},
            { "hals", "ill"},
            { "\"5560\"", "home"},
            { "gemakker", "home"},
            { "nedlagt med Corona", "ill"},
            { "fået det noget værre", "ill"},
            { "positiv corona", "ill"},
            { "hænger med mulen", "ill"},
            { "langt nede", "ill"},
            
            // 1-1 - 15/1
            { "ikke rask", "ill"},
            { "Saga", "kidsIll"},
   
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
        
        timeKeywords.Add("formiddag", new TimeOnly(11,15));
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