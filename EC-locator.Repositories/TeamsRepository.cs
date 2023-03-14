using System.Collections;

namespace EC_locator.Repositories;

using EC_locator.Core.Interfaces;

public class TeamsRepository : ITeamsRepository
{
    public async Task ListUsersAsync()
    {
        try
        {
            var userPage = await GraphHelper.GetUsersAsync();

            // Output each users's details
            foreach (var user in userPage.CurrentPage)
            {
                Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
                Console.WriteLine($"  ID: {user.Id}");
                Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}");
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // userPage.NextPageRequest.GetAsync();
            var moreAvailable = userPage.NextPageRequest != null;

            Console.WriteLine($"\nMore users available? {moreAvailable}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
        }
    }
    
    public async Task ListMessagesAsync()
    {
        try
        {
            var messagePage = await GraphHelper.getMessagesAsync();

            // Output message details
            foreach (var message in messagePage.CurrentPage)
            {
                Console.WriteLine($"User: {message.Body.Content ?? "NO CONTENT"}");
                Console.WriteLine($"  ID: {message.Id}");
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // userPage.NextPageRequest.GetAsync();
            var moreAvailable = messagePage.NextPageRequest != null;

            Console.WriteLine($"\nMore messages available? {moreAvailable}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }
    }
    

    public string[] GetMessages(string employeeID, DateOnly date)
    {
        if (employeeID.Equals("sample1"))
        {
            // all correct
            string[] messages =
            {
                "Jeg bliver hjemme indtil jeg kan aflevere min cykel til service klokken 10, og så kommer jeg ind.",
                "Jeg smutter til tandlæge her klokken 12. Arbejder muligvis hjemmefra efter.",
                "Jeg tager lige en time mere fra hjemmekontoret. Er inde ca. kl 10",
                "Er til møde ved Alumeco indtil 11.30 i morgen og arbejder hjemme fra derefter.",
                "Jeg er slet ikke på toppen, så jeg bliver hjemme i dag",
                "Kommer ind på kontoret omkring kl. 11",
                "Jeg starter lige hjemme og er på kontoret til frokost",
                "Jeg er på hjemmefra i dag.",
                
            };
            return messages;
        }

        if (employeeID.Equals("sample2"))
        {
            // all correct
            string[] messages =
            {
                "Kommer på kontoret omkring kl. 11",
                "Godmorgen. Jeg er på hjemmekontoret idag",
                "Stadig ikke på toppen, men arbejder det jeg kan",
                "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge'",
                "Godmorgen, jeg starter ud hjemme og kommer ind omkring kl 10",
                "Jeg er på hjemmefra i dag"
            };
            return messages;
        }
        
        if (employeeID.Equals("sample3"))
        {
            // all correct
            string[] messages =
            {
                "tager en dag under dynen",
                "Godmorgen. Jeg er på hjemmekontoret idag",
                "Morn - det bliver endnu en dag på hjemmekontoret - dels pga. bentøjet og dels for at få ro til at forberede Popermo til næste uge'",
                "Er hjemme med syge piger, så er lidt on/off hele dagen",
                "Thomas, Gorm og jeg tager ned til Nørgaard Mikkelsen til møde, forventer at være retur 10.30",
                "Starter til møde hos NM. Er tilbage lidt over 10.",
                "Morn - jeg starter hos lægen og kører på Popermo efterfølgende",
                "Jeg har krammet toilettet hele natten, så jeg er hjemme, og sover forhåbentligt",
                "Er ikke på toppen - Er on/off i dag",
                "Godmorgen. Jeg starter hjemme, men forventer at være på kontoret kl 10. Vi ses ✌",
                "Er inde 9:15"
            };
            return messages;
        }
        
        if (employeeID.Equals("outliers"))
        {
            string[] messages =
            {
                "Jeg er på kontoret inden frokost. Er på hjemmefra",
                "I morgen arbejder jeg hjemmefra og stopper 11.30",
                "Jeg starter ud hjemme 9.30 og er på kontoret til frokost",
                "Jeg tager lige en time eller to hjemmefra, her til morgen ",
                "Jeg er på kontoret cirka 09.30",
                "Lynet skal lige have en gang service, så er først på pinden 9.15-9.30"
            };
            return messages;
        }
        
        if (employeeID.Equals("un-precise"))
        {
            string[] messages =
            {
                "Skal lige hente Noah og køre ham ud til de gamle. Han har lidt ondt i maven. Kommer på konnes igen efter.",
            };
            return messages;
        }
        
        return null;
    }
    
    
    
    public async Task<ArrayList> GetUsersAsync()
    {
        ArrayList returnArray = new ArrayList();
        try
        {
            
            var userPage = await GraphHelper.GetUsersAsync();

            // Output each users's details
            foreach (var user in userPage.CurrentPage)
            {
                returnArray.Add(user);
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // userPage.NextPageRequest.GetAsync();
            var moreAvailable = userPage.NextPageRequest != null;

            // Console.WriteLine($"\nMore users available? {moreAvailable}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
        }

        return returnArray;
    }
    
    public async Task<ArrayList> GetMessagesAsync()
    {
        ArrayList returnArray = new ArrayList();
        try
        {
            
            var userPage = await GraphHelper.GetUsersAsync();

            // Output each users's details
            foreach (var user in userPage.CurrentPage)
            {

                returnArray.Add(user);
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // userPage.NextPageRequest.GetAsync();
            var moreAvailable = userPage.NextPageRequest != null;

            Console.WriteLine($"\nMore users available? {moreAvailable}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
        }

        return returnArray;
    }
}