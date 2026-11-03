# Call of Chernobyl Relay Chat (CoCRC)
An IRC-based chat app for Call of Chernobyl and Call of the Zone. Features an independent client as well as in-game chat, automatic death messages, and compatibility with all other addons.
Uplifted to .NET 10 and Avalonia, as well as newer Nuget packages.
All credit goes to TKGP.

![CRC-Screenshot](https://user-images.githubusercontent.com/95293308/166155945-211684c7-6486-4ee0-bc8e-ae56914b62d4.jpg)

# Installing CoCRC
1. Download the appropriate build from the Releases section and extract it.
2. Copy the files inside the folder into your Call of Chernobyl directory.

The CI-published builds are self-contained and do not require a separate .NET runtime installation.

# Upgrading CoCRC

1. Download the new build.
2. Proceed with normal installation, replacing the old application files.


# Usage
Run Chernobyl Relay Chat.exe; the application must be running for in-game chat to work.  
On Linux, make `Chernobyl Relay Chat` executable and run it.
After connecting, click the Options button to change your name and other settings, then launch CoC.  
Once playing, press Enter (by default) to bring up the chat interface and Enter again to send your message, or Escape to close without sending.  
You may use text commands from the game or client by starting with a /. Use /commands to see all available commands.  

# Building / Troubleshooting 
Should you need to clone, or fork this repository and revive CRC, I recommend using Visual Studio, with the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
You can also build from the repository root with:

```shell
cd "Chernobyl Relay Chat"
dotnet build --configuration Debug
cd ..
```

One of the most annoying issues I've encountered when working on CRC is that after making changes and committing, Visual Studio will then proceed to build an old version of CRC.
For example, you've just changed the IRC channel from #crc_english to your own test channel, but when you build CRC you find you still connect to the old IRC channel.
Settings are stored in `ChernobylRelayChat/settings.json` under the user's application data directory and can override code defaults. In Visual Studio go to Build, and then Clean.

CRC checks for a newer release when connecting and displays a message in the chat if one is available.
To test the release check, run `dotnet run --project "tests/CRCReleaseChecker.Tests"`.
To test the interface, run `dotnet run --project "tests/CRCUi.Tests"`.
To test IRC locally, run `dotnet run --project "tests/CRCIrc.Tests"`. Disconnect cleanup currently fails because SmartIrc4net uses `Thread.Abort`, which is not supported on .NET 10. The application may close when the IRC connection is lost; restart CRC if this happens.

# Credits
Chernobyl Relay Chat: TKGP
Chernobyl Relay Chat Rebirth: itsAnchorpoint
CoCRC Testers and family: Tolik_Lucky_Bastard, ElekTrick, Pretov, Pogodemon, Manny, TwistedLoner, Dancher743 and everyone who uses CRC and helped me with testing and keeping the community alive.
Translation by Zweelee  
GitHub: Octokit
Max Hauser: semver  
Mirco Bauer: SmartIrc4Net  
nixx quality: GitHubUpdate  
  
av661194, OWL, XMODER: Russian translation

Join us on our Discord channel for announcements and support with CoCRC - https://discord.gg/deJxyjU3vB
