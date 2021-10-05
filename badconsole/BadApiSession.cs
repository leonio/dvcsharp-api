using System;
using System.Collections.Generic;
using Flurl;
using Flurl.Http;

public class BadApiSession
{
    public const string KeyAccessToken = "AccessToken";
    public const string KeyRole = "Role";
    public const string KeyEmail = "Email";
    public const string KeyUser = "User";
    private readonly Url _baseUrl;

    public BadApiSession(Url baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public void Establish(string userName, string email, string role, string accessToken)
    {
        UserName = userName;
        Email = email;
        Role = role;
        AccessToken = accessToken;

        SessionData[KeyAccessToken] = accessToken;
        SessionData[KeyRole] = role;
        SessionData[KeyEmail] = email; 
        SessionData[KeyUser] = userName;

        IsEstablished = true;
    }

    public IFlurlRequest WithAuthenticatedEndpoint(string uri)
    {
        return GetEndpoint(uri).WithHeader("Authorization", $"Bearer {AccessToken}");
    }

    public string GetEndpoint(string controllerAction) => string.Format("{0}/api/{1}", _baseUrl, controllerAction);


    public Dictionary<string, object> SessionData { get; } = new Dictionary<string, object>();
    public bool IsEstablished { get; private set; }
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string Role { get; private set; }
    public string AccessToken { get; private set; }
}