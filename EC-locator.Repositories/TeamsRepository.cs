using System.Collections;

namespace EC_locator.Repositories;

using EClocator.Core.Interfaces;

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
            "Jeg er på kontoret inden frokost. Er på hjemmefra"
        }; 
        return messages;
        
        Console.WriteLine("in GetMeaasages");

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