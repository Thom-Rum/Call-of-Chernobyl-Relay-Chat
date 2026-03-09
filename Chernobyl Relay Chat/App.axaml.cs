using Avalonia;
using Avalonia.Controls;
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
                bool isFirstRun = !CRCOptions.Load();

                if (isFirstRun)
                {
                    // Keep the app alive while the language prompt is open; a bare
                    // OnLastWindowClose would shut down the moment the prompt closes
                    // before the main window is created.
                    desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    var prompt = new LanguagePrompt();
                    desktop.MainWindow = prompt;
                    prompt.Closed += (_, _) =>
                    {
                        CRCOptions.Language = prompt.Result;
                        CRCOptions.Channel  = prompt.Result == "eng" ? "#cocrc_english" : "#cocrc_slavik";
                        desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
                        LaunchMainApp(desktop);
                    };
                }
                else
                {
                    LaunchMainApp(desktop);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void LaunchMainApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (string.IsNullOrEmpty(CRCOptions.Name))
                CRCOptions.Name = CRCStrings.RandomIrcName(CRCOptions.GetFaction());

            var clientDisplay = new ClientDisplay();
            CRCDisplay.SetWindow(clientDisplay);
            desktop.MainWindow = clientDisplay;
            clientDisplay.Show();

            new Thread(CRCClient.Start) { IsBackground = true, Name = "IRC Client" }.Start();
        }
    }
}
