using EClocator.Core.Interfaces;

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
             {"syg", "sengedag"},
             {"syg", "ikke frisk"},
             {"syg", "vandret"},
             {"syg", "ikke på toppen"},
             {"syg", "dynen"},
             {"syg", "syg"},
             {"syg", "influenza"},
             {"syg", "ligger syg"},
             {"syg", "lagt syg"},
             {"syg", "feber"},
             {"syg", "sygdom"},
             {"syg", "forkølelse"},
             {"syg", "svimmel"},
             {"syg", "kvalme"},
             {"syg", "ondt i hovedet"},
             {"syg", "på langs"},
             {"syg", "syge"},
             {"syg", "helbred"},
             {"syg", "feberbarn"},
             {"syg", "lægger mig"},
             {"syg", "skidt"},
             {"syg", "under dynen"},
             
             {"møde", "møde"},

             {"hjemme", "hjemme"},
             {"hjemme", "hjemmefra"},
             {"hjemme", "på hjemmefra"},
             {"hjemme", "tager den hjemmefra"},
             {"hjemme", "tager jeg den hjemmefra"},
             {"hjemme", "hjemmekontoret"},
             {"hjemme", "hjemmeskansen"},
             {"hjemme", "ikke på kontoret"},
             {"hjemme", "er inde"},
             
             
             {"konter", "på kontoret"},
             {"konter", "inde"},
             {"konter", "ind"},
             {"konter", "på arbejdet"},
             {"konter", "ind forbi"},
             {"konter", "er inde"},
             {"konter", "er inde ved"},
             {"konter", "kommer ind"},
             {"konter", "kommer i firmaet"},
             
             {"fri", "holder fri"},
             {"fri", "fri"},
             {"fri", "holder"},
             {"fri", "fridag"},
             {"fri", "holder weekend"},
             {"fri", "off"},
             
             {"remote", "tager ud til"},
             {"remote", "er på"},
             {"remote", "er hos"}
         }; 
         return keywords;
    }
}