using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;

namespace Chernobyl_Relay_Chat
{
    class CRCDisplay
    {
        private static ClientDisplay? clientDisplay;

        // SoC-style amber-orange used for NPC/speaker names in Shadow of Chernobyl
        internal static readonly IBrush NickBrush = new SolidColorBrush(Color.FromRgb(206, 143, 63));

        /// <summary>Called by App.OnFrameworkInitializationCompleted to register the main window.</summary>
        public static void SetWindow(ClientDisplay window)
        {
            clientDisplay = window;
        }

        public static void Stop()
        {
            Dispatcher.UIThread.Post(() => clientDisplay?.Close());
        }

        public static void ShowError(string message)
        {
            // Route errors into the chat window; a modal dialog would require an owner reference.
            AddError(message);
        }

        public static void AddInformation(string message)
        {
            clientDisplay?.AddInformation(message);
        }

        public static void AddError(string message)
        {
            clientDisplay?.AddError(message);
        }

        public static void OnConnected()
        {
            clientDisplay?.Enable();
        }

        public static void UpdateUsers()
        {
            clientDisplay?.UpdateUsers(CRCClient.Users);
        }

        public static void OnHighlightMessage(string nick, string message)
        {
            clientDisplay?.AddHighlightMessage(nick, message);
        }

        public static void OnChannelMessage(string nick, string message)
        {
            clientDisplay?.AddMessage(nick, message, NickBrush);
        }

        public static void OnOwnChannelMessage(string nick, string message)
        {
            clientDisplay?.AddMessage(nick, message, NickBrush);
        }

        public static void OnQueryMessage(string from, string to, string message)
        {
            // Notification sound: Console.Beep() is cross-platform (no-op if no bell available)
            try { Console.Beep(); } catch { }
            clientDisplay?.AddMessage(from + " -> " + to, message, NickBrush);
        }

        public static void OnGotKicked()
        {
            clientDisplay?.Disable();
        }
    }
}
