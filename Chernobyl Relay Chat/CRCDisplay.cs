using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;

namespace Chernobyl_Relay_Chat
{
    class CRCDisplay
    {
        private static ClientDisplay? clientDisplay;

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
            clientDisplay?.AddMessage(nick, message, Brushes.Black);
        }

        public static void OnOwnChannelMessage(string nick, string message)
        {
            clientDisplay?.AddMessage(nick, message, Brushes.Gray);
        }

        public static void OnQueryMessage(string from, string to, string message)
        {
            // Notification sound: Console.Beep() is cross-platform (no-op if no bell available)
            try { Console.Beep(); } catch { }
            clientDisplay?.AddMessage(from + " -> " + to, message, Brushes.DeepPink);
        }

        public static void OnGotKicked()
        {
            clientDisplay?.Disable();
        }
    }
}
