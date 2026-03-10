using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
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
                try
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
                        prompt.Show();
                        prompt.Closed += (_, _) =>
                        {
                            try
                            {
                                CRCOptions.Language = prompt.Result;
                                CRCOptions.Channel  = prompt.Result == "eng" ? "#cocrc_english" : "#cocrc_slavik";
                                desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
                                LaunchMainApp(desktop);
                            }
                            catch (Exception ex)
                            {
                                LogCrash(ex);
                                desktop.Shutdown(1);
                            }
                        };
                    }
                    else
                    {
                        LaunchMainApp(desktop);
                    }
                }
                catch (Exception ex)
                {
                    LogCrash(ex);
                    desktop.Shutdown(1);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        internal static void LogCrash(Exception ex)
        {
            string msg = ex.ToString();
            Console.Error.WriteLine("[CRC CRASH] " + msg);
            try
            {
                // Write to cwd first, then fall back to home dir so the log is
                // always findable even if the working directory is read-only.
                string local = Path.Combine(AppContext.BaseDirectory, "crash.log");
                string home  = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "crc_crash.log");
                File.WriteAllText(local, msg);
                File.WriteAllText(home,  msg);
            }
            catch { }
        }

        private static void LaunchMainApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (string.IsNullOrEmpty(CRCOptions.Name))
                CRCOptions.Name = CRCStrings.RandomIrcName(CRCOptions.GetFaction());

            Console.WriteLine("[CRC] LaunchMainApp: creating ClientDisplay...");
            Console.Out.Flush();
            var clientDisplay = new ClientDisplay();
            Console.WriteLine("[CRC] LaunchMainApp: ClientDisplay created, setting window...");
            Console.Out.Flush();
            CRCDisplay.SetWindow(clientDisplay);
            desktop.MainWindow = clientDisplay;
            Console.WriteLine("[CRC] LaunchMainApp: calling Show()...");
            Console.Out.Flush();
            clientDisplay.Show();
            Console.WriteLine("[CRC] LaunchMainApp: Show() returned, starting IRC thread...");
            Console.Out.Flush();

            new Thread(CRCClient.Start) { IsBackground = true, Name = "IRC Client" }.Start();
        }
    }
}
