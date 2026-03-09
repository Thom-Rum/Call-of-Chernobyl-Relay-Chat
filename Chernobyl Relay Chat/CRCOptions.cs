using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Chernobyl_Relay_Chat
{
    class CRCOptions
    {
        //irc.gamesurge.net
        //irc.slashnet.org
        public const string Server = "irc.gamesurge.net";
        public static readonly string InPath = Path.Combine("..", "gamedata", "configs", "crc_input.txt");
        public static readonly string OutPath = Path.Combine("..", "gamedata", "configs", "crc_output.txt");

        public static string Language = "eng";
        public static string Channel = "#cocrc_english";

        // Window geometry (replaces System.Drawing.Point/Size)
        public static int DisplayLocationX;
        public static int DisplayLocationY;
        public static int DisplayWidth;
        public static int DisplayHeight;

        public static bool AutoFaction = true;
        public static string GameFaction = "actor_stalker";
        public static string ManualFaction = "actor_stalker";
        public static string? Name;
        public static bool SendDeath = true;
        public static bool ReceiveDeath = true;
        public static int DeathInterval = 90;
        public static bool ShowTimestamps = true;

        public static int NewsDuration = 10;
        public static string ChatKey = "RETURN";
        public static bool NewsSound = true;
        public static bool CloseChat = true;

        private static readonly Dictionary<string, string> defaultChannel = new Dictionary<string, string>()
        {
            ["eng"] = "#cocrc_english",
            ["ukr"] = "#cocrc_slavik",
            ["rus"] = "#cocrc_slavik",
        };

        private static string SettingsPath
        {
            get
            {
                string configDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ChernobylRelayChat");
                Directory.CreateDirectory(configDir);
                return Path.Combine(configDir, "settings.json");
            }
        }

        public static string ChannelProxy()
        {
            return Channel;
        }

        public static string GetFaction()
        {
            if (AutoFaction)
                return GameFaction;
            else
                return ManualFaction;
        }

        /// <summary>
        /// Loads settings from JSON. Returns false if this is a first run (no settings file).
        /// </summary>
        public static bool Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                    return false;

                string json = File.ReadAllText(SettingsPath);
                var data = JsonSerializer.Deserialize<SettingsData>(json);
                if (data == null) return false;

                Language = data.Language ?? "eng";
                Channel = data.Channel ?? defaultChannel.GetValueOrDefault(Language, "#cocrc_english");
                DisplayLocationX = data.DisplayLocationX;
                DisplayLocationY = data.DisplayLocationY;
                DisplayWidth = data.DisplayWidth;
                DisplayHeight = data.DisplayHeight;
                AutoFaction = data.AutoFaction;
                GameFaction = data.GameFaction ?? "actor_stalker";
                ManualFaction = data.ManualFaction ?? "actor_stalker";
                Name = data.Name ?? CRCStrings.RandomIrcName(GetFaction());
                SendDeath = data.SendDeath;
                ReceiveDeath = data.ReceiveDeath;
                DeathInterval = data.DeathInterval > 0 ? data.DeathInterval : 90;
                ShowTimestamps = data.ShowTimestamps;
                NewsDuration = data.NewsDuration > 0 ? data.NewsDuration : 10;
                ChatKey = data.ChatKey ?? "RETURN";
                NewsSound = data.NewsSound;
                CloseChat = data.CloseChat;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Save()
        {
            var data = new SettingsData
            {
                Language = Language,
                Channel = Channel,
                DisplayLocationX = DisplayLocationX,
                DisplayLocationY = DisplayLocationY,
                DisplayWidth = DisplayWidth,
                DisplayHeight = DisplayHeight,
                AutoFaction = AutoFaction,
                GameFaction = GameFaction,
                ManualFaction = ManualFaction,
                Name = Name,
                SendDeath = SendDeath,
                ReceiveDeath = ReceiveDeath,
                DeathInterval = DeathInterval,
                ShowTimestamps = ShowTimestamps,
                NewsDuration = NewsDuration,
                ChatKey = ChatKey,
                NewsSound = NewsSound,
                CloseChat = CloseChat,
            };
            File.WriteAllText(SettingsPath,
                JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        }

        private class SettingsData
        {
            public string? Language { get; set; }
            public string? Channel { get; set; }
            public int DisplayLocationX { get; set; }
            public int DisplayLocationY { get; set; }
            public int DisplayWidth { get; set; }
            public int DisplayHeight { get; set; }
            public bool AutoFaction { get; set; } = true;
            public string? GameFaction { get; set; }
            public string? ManualFaction { get; set; }
            public string? Name { get; set; }
            public bool SendDeath { get; set; } = true;
            public bool ReceiveDeath { get; set; } = true;
            public int DeathInterval { get; set; } = 90;
            public bool ShowTimestamps { get; set; } = true;
            public int NewsDuration { get; set; } = 10;
            public string? ChatKey { get; set; }
            public bool NewsSound { get; set; } = true;
            public bool CloseChat { get; set; } = true;
        }
    }
}
