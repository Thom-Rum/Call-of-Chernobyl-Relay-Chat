using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chernobyl_Relay_Chat
{
    class CRCUpdate
    {
        private static readonly HttpClient client = CRCReleaseChecker.CreateClient();

        public static async Task CheckUpdate()
        {
            try
            {
                string? currentVersion = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                string? latestVersion = await CRCReleaseChecker.GetNewerRelease(client, currentVersion);
                if (latestVersion != null)
                {
                    string message = string.Format(CRCStrings.Localize("update_available"),
                        latestVersion, CRCReleaseChecker.ReleasesUrl);
                    CRCDisplay.AddInformation(message);
                    CRCGame.OnUpdate(message);
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException ||
                                       ex is JsonException || ex is InvalidDataException)
            {
                Console.Error.WriteLine("[CRC] Update check failed: " + ex);
                CRCDisplay.AddError(CRCStrings.Localize("update_check_failed"));
            }
        }
    }
}
