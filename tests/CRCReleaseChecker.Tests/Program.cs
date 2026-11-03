using Chernobyl_Relay_Chat;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

await TestRelease("0.10.0", "v0.10.1", "v0.10.1");
await TestRelease("0.10.0", "v0.10.0", null);
await TestRelease("0.10.0+build123", "v0.10.0", null);
await TestRelease("0.10.0", "v0.9.4", null);
await TestRelease("0.10.0", "v0.10.0-rc.1", null);
await TestRelease("0.10.0-rc.1", "v0.10.0", "v0.10.0");
await TestRelease("0.9.9", "v0.10.0", "v0.10.0");
await TestRelease("0.11.0", "v0.10.0", null);
await TestRelease("0.11.0", "v0.11.0", null);
await TestRelease("0.11.0", "v0.11.1", "v0.11.1");
await ExpectFailure<InvalidDataException>("0.10.0", "{\"tag_name\":\"not-a-version\"}");
await ExpectFailure<InvalidDataException>("0.10.0", "{}");
await ExpectFailure<InvalidDataException>(null, "{\"tag_name\":\"v0.10.0\"}");
await ExpectFailure<JsonException>("0.10.0", "not-json");
await ExpectFailure<HttpRequestException>("0.10.0", "{}", HttpStatusCode.Forbidden);
Console.WriteLine("All release checker tests passed.");

if (args.Length == 1 && args[0] == "--live")
{
    using var client = CRCReleaseChecker.CreateClient();
    string? latest = await CRCReleaseChecker.GetNewerRelease(client, "0.0.0");
    if (latest == null)
        throw new Exception("The live API did not return a newer release.");
    Console.WriteLine("Live GitHub release check passed: " + latest);
}

static async Task TestRelease(string current, string tag, string? expected)
{
    using var client = CreateClient("{\"tag_name\":\"" + tag + "\"}");
    string? actual = await CRCReleaseChecker.GetNewerRelease(client, current);
    if (actual != expected)
        throw new Exception($"Current {current}, release {tag}: expected {expected ?? "<none>"}, got {actual ?? "<none>"}.");
}

static async Task ExpectFailure<T>(string? current, string json, HttpStatusCode status = HttpStatusCode.OK)
    where T : Exception
{
    using var client = CreateClient(json, status);
    try
    {
        await CRCReleaseChecker.GetNewerRelease(client, current);
    }
    catch (T)
    {
        return;
    }
    throw new Exception($"Expected {typeof(T).Name}.");
}

static HttpClient CreateClient(string json, HttpStatusCode status = HttpStatusCode.OK)
{
    return new HttpClient(new FakeHandler(json, status));
}

sealed class FakeHandler(string json, HttpStatusCode status) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri?.AbsoluteUri !=
            "https://api.github.com/repos/Thom-Rum/Call-of-Chernobyl-Relay-Chat/releases/latest")
            throw new Exception("Unexpected release API URL.");

        return Task.FromResult(new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}
