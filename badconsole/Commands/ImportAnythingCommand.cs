using System;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ImportAnythingCommand : BaseBadCommand
{
    public override async Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default)
    {
        // api/imports
        // no authorization, just push xml with Entities/Entity[@Type] deserializing a type
        // possibility of remote code execution dependon the xml types we can get into the container
        //... (i'm sure there is more nasty stuff here, but gotta get the type onto the container)
        // inspired from  https://github.com/VulnerableGhost/.Net-Sterilized--Deserialization-Exploitation/blob/master/BH_US_12_Forshaw_Are_You_My_Type_WP.pdf

        const string xmlFormat = "<?xml version=\"1.0\"?>"
            + "<Entities>"
            +    "<Entity Type=\"System.Int16\"><short>{0}</short></Entity>"
            + "</Entities>";

        var stringContent = SqlInt16.MaxValue.ToString();
        var xmlPayload = string.Format(xmlFormat, stringContent);
        var content = new StringContent(xmlPayload, Encoding.UTF8, "application/xml");
        var results = await session.WithAuthenticatedEndpoint("imports").SendAsync(HttpMethod.Post, content, token);

        // should be 200 OK with the contents of the /Entities/Entity
        Console.WriteLine("MaxInt for SqlInt16 was: {0}, and content: {1}", results.StatusCode, await results.GetStringAsync());
        
        // now attempt overflow with a bigger number
        stringContent = int.MaxValue.ToString();
        xmlPayload = string.Format(xmlFormat, stringContent);
        content = new StringContent(xmlPayload, Encoding.UTF8, "application/xml");
        try
        {
            results = await session.WithAuthenticatedEndpoint("imports").SendAsync(HttpMethod.Post, content, token);
        }
        catch (Flurl.Http.FlurlHttpException ex)
        {
            // and here we get a stacktrace...
            Console.WriteLine("API exceptioned out with expected overflow: {0}", await ex.GetResponseStringAsync());
        }

        return true;
    }
}