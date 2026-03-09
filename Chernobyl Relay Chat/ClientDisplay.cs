using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace Chernobyl_Relay_Chat
{
    public partial class ClientDisplay : Window, ICRCSendable
    {
        private readonly Timer _gameCheckTimer  = new Timer(1000);
        private readonly Timer _gameUpdateTimer = new Timer(100);
        private readonly Timer _updateCheckTimer = new Timer(300_000); // 5 min

        public ClientDisplay()
        {
            InitializeComponent();

            Title = CRCStrings.Localize("crc_name");
            buttonSend.Content    = CRCStrings.Localize("display_send");
            buttonOptions.Content = CRCStrings.Localize("display_options");

            buttonSend.Click    += (_, _) => SendMessage();
            buttonOptions.Click += (_, _) => new OptionsForm().Show(this);
            textBoxInput.KeyDown += TextBoxInput_KeyDown;

            // Ensure the Inlines collection is ready before any text is appended
            chatDisplay.Inlines ??= new InlineCollection();

            // Restore saved window geometry
            if (CRCOptions.DisplayWidth > 0 && CRCOptions.DisplayHeight > 0)
            {
                Width    = CRCOptions.DisplayWidth;
                Height   = CRCOptions.DisplayHeight;
                Position = new Avalonia.PixelPoint(CRCOptions.DisplayLocationX, CRCOptions.DisplayLocationY);
            }

            Closing += ClientDisplay_Closing;

            _gameCheckTimer.Elapsed  += (_, _) => CRCGame.GameCheck();
            _gameUpdateTimer.Elapsed += (_, _) => CRCGame.GameUpdate();
            _updateCheckTimer.Elapsed += async (_, _) => await CRCUpdate.CheckUpdate();
            _updateCheckTimer.AutoReset = false;

            _gameCheckTimer.Start();
            _gameUpdateTimer.Start();
            _updateCheckTimer.Start();

            AddInformation(CRCStrings.Localize("display_connecting"));
        }

        private void ClientDisplay_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _gameCheckTimer.Stop();
            _gameUpdateTimer.Stop();
            _updateCheckTimer.Stop();

            CRCOptions.DisplayLocationX = Position.X;
            CRCOptions.DisplayLocationY = Position.Y;
            CRCOptions.DisplayWidth  = (int)Width;
            CRCOptions.DisplayHeight = (int)Height;

            CRCClient.Stop();
        }

        private void TextBoxInput_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
                SendMessage();
        }

        private void SendMessage()
        {
            string trimmed = textBoxInput.Text?.Trim() ?? "";
            if (trimmed.Length > 0)
            {
                if (trimmed[0] == '/')
                    CRCCommands.ProcessCommand(trimmed, this);
                else
                    CRCClient.Send(trimmed);
                textBoxInput.Text = "";
            }
        }

        public void Enable()
        {
            Dispatcher.UIThread.Post(() =>
            {
                buttonSend.IsEnabled    = true;
                buttonOptions.IsEnabled = true;
            });
        }

        public void Disable()
        {
            Dispatcher.UIThread.Post(() =>
            {
                buttonSend.IsEnabled    = false;
                buttonOptions.IsEnabled = false;
            });
        }

        private void AddLinePrefix()
        {
            if (chatDisplay.Inlines?.Count > 0)
                chatDisplay.Inlines.Add(new LineBreak());
            if (CRCOptions.ShowTimestamps)
            {
                chatDisplay.Inlines!.Add(new Run
                {
                    Text       = DateTime.Now.ToString("HH:mm:ss "),
                    FontFamily = new FontFamily("Courier New,Courier,monospace"),
                    Foreground = Brushes.DimGray
                });
            }
        }

        public void AddLine(string line, IBrush brush)
        {
            Dispatcher.UIThread.Post(() =>
            {
                AddLinePrefix();
                chatDisplay.Inlines!.Add(new Run { Text = line, Foreground = brush });
                chatScrollViewer.ScrollToEnd();
            });
        }

        public void AddInformation(string message) => AddLine(message, Brushes.DodgerBlue);
        public void AddError(string message)       => AddLine(message, Brushes.OrangeRed);

        public void AddMessage(string nick, string message, IBrush nickBrush)
        {
            Dispatcher.UIThread.Post(() =>
            {
                AddLinePrefix();
                chatDisplay.Inlines!.Add(new Run
                {
                    Text       = nick + ": ",
                    FontWeight = FontWeight.Bold,
                    Foreground = nickBrush
                });
                chatDisplay.Inlines.Add(new Run { Text = message, Foreground = Brushes.White });
                chatScrollViewer.ScrollToEnd();
            });
        }

        public void AddHighlightMessage(string nick, string message)
        {
            // TODO: per-line background colour (not trivial with TextBlock Inlines)
            AddMessage("[!] " + nick, message, Brushes.Yellow);
        }

        public void UpdateUsers(List<string> users)
        {
            Dispatcher.UIThread.Post(() =>
                textBoxUsers.Text = string.Join("\n", users));
        }
    }
}


