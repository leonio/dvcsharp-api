using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

public class ListUsersCommand : BaseBadCommand
{
    public static readonly string KeyAllUsers = "AllUsers";

    public override async Task<bool> ExecuteAsync(
        Url baseUrl,
        BadApiSession session,
        CancellationToken token = default)
    {
        // api/users
        var results = await session.WithAuthenticatedEndpoint("users").GetJsonListAsync(token);
        var users = new List<ApiUser>();

        foreach (var user in results)
        {
            users.Add(
                new ApiUser
                {
                    Id = (int)user.id,
                    Name = user.name,
                    Email = user.email,
                    Role = user.role,
                    LastUpdated = user.updatedAt
                });
                
            Console.WriteLine("User {0} - name:{1}, email:{2}, role:{3}, lastUpdated:{4}", user.id, user.name, user.email, user.role, user.updatedAt);
        }

        session.SessionData[KeyAllUsers] = users;
        return true;
    }
}