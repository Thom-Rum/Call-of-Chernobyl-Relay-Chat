using Semver;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chernobyl_Relay_Chat
{
    internal static class CRCReleaseChecker
    {
        internal const string ReleasesUrl = "https://github.com/Thom-Rum/Call-of-Chernobyl-Relay-Chat/releases/latest";
        private const string ApiUrl = "https://api.github.com/repos/Thom-Rum/Call-of-Chernobyl-Relay-Chat/releases/latest";

        internal static HttpClient CreateClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Chernobyl-Relay-Chat");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            return client;
        }

        internal static async Task<string?> GetNewerRelease(HttpClient client, string? currentVersion)
        {
            if (!SemVersion.TryParse(currentVersion, SemVersionStyles.Strict, out var current))
                throw new InvalidDataException("The application version is not a valid semantic version.");

            var release = await client.GetFromJsonAsync<Release>(ApiUrl);
            if (release?.TagName == null ||
                !SemVersion.TryParse(release.TagName, SemVersionStyles.Strict | SemVersionStyles.AllowV, out var latest))
                throw new InvalidDataException("GitHub returned an invalid release version.");

            return latest.ComparePrecedenceTo(current) > 0 ? release.TagName : null;
        }

        private sealed class Release
        {
            [JsonPropertyName("tag_name")]
            public string? TagName { get; set; }
        }
    }
}
