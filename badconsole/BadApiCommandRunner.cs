using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

public class BadApiCommandRunner
{
    private readonly Url _targetUrl;
    private readonly BadApiSession _session;

    public BadApiCommandRunner(Url targetUrl)
    {
        _targetUrl = targetUrl ?? throw new System.ArgumentNullException(nameof(targetUrl));
        _session = new BadApiSession(_targetUrl);
    }

    public async Task<BadApiSession> InitSessionAsync(CancellationToken token = default)
    {
        // POST: api/registrations 
        //   public string name { get; set; }
        //   public string email { get; set; }
        //   public string password { get; set; }
        //   public string passwordConfirmation { get; set; }
        var randomId = new Random().Next(1000,2000);
        var userid = string.Format("user_{0}", randomId);
        var password = string.Format("p4ssw0rd@{0}", randomId);
        var email = string.Format("user_{0}@sharklasers.com", randomId);

        var result = await _session.GetEndpoint("registrations")
            .PostJsonAsync(
                new
                {
                    name = userid,
                    email = email,
                    password = password,
                    passwordConfirmation = password
                },
                token);

        var response = await result.GetStringAsync();
        if (result?.StatusCode == 200)
        {
            Console.WriteLine("Initialized user: {0} - success with response: {1}", userid, response);    
        }
        else
        {
            Console.WriteLine("Failed to initialized user: {0} - ", userid, response);
        }

        // POST: api/authorizations
        // public string email { get; set; }
        // public string password { get; set; }
        var loginResult = await _session.GetEndpoint("authorizations")
            .PostJsonAsync(
                new
                {
                    email = email,
                    password = password
                });

        if (loginResult?.StatusCode == 200)
        {
            var json = await loginResult.GetJsonAsync();
            Console.WriteLine("Logged on: {0} - success. Role: {1}, Token: {2}", userid, json.role, json.accessToken);    
            
            _session.Establish(userid, email, json.role, json.accessToken);
        }
        else
        {
            Console.WriteLine("Failed to initialized user: {0} - ", userid, response);
        }

        return _session;
    }
}