using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Threading;

namespace Chernobyl_Relay_Chat
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Load settings; on first run, sensible defaults are used.
                // Language / channel can be changed any time via Options.
                CRCOptions.Load();
                if (string.IsNullOrEmpty(CRCOptions.Name))
                    CRCOptions.Name = CRCStrings.RandomIrcName(CRCOptions.GetFaction());

                var clientDisplay = new ClientDisplay();
                CRCDisplay.SetWindow(clientDisplay);
                desktop.MainWindow = clientDisplay;

                // IRC client runs on its own background thread (client.Listen() is blocking)
                var clientThread = new Thread(CRCClient.Start)
                {
                    IsBackground = true,
                    Name = "IRC Client"
                };
                clientThread.Start();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
