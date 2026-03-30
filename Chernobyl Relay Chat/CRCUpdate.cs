using System;
using System.Threading.Tasks;
// TODO: Update checker disabled pending rewrite with a modern Octokit version.
// GitHubUpdate is broken with Octokit > 0.5 and is not compatible with .NET 8.

namespace Chernobyl_Relay_Chat
{
    class CRCUpdate
    {
        // Both methods are stubs until the update checker is rewritten against a current Octokit API.
        public static bool CheckFirstUpdate() => false;
        public static System.Threading.Tasks.Task<bool> CheckUpdate() => System.Threading.Tasks.Task.FromResult(false);
    }
}
