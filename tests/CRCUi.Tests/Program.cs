using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Chernobyl_Relay_Chat;
using System;
using System.Linq;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var gameEncoding = Encoding.GetEncoding(1251);
byte[] cyrillic = Convert.FromHexString("CFF0E8E2E5F2");
if (gameEncoding.GetString(cyrillic) != "Привет" ||
    !gameEncoding.GetBytes("Привет").SequenceEqual(cyrillic))
    throw new Exception("Windows-1251 game text encoding failed.");

bool native = args.Length == 1 && args[0] == "--native";
var builder = AppBuilder.Configure<App>();
if (native)
    builder.UsePlatformDetect();
else
    builder.UseHeadless(new AvaloniaHeadlessPlatformOptions());
builder.SetupWithoutStarting();
CRCStrings.Load();

var main = new ClientDisplay();
main.Show();
Dispatcher.UIThread.RunJobs();
var send = Required<Button>(main, "buttonSend");
var optionsButton = Required<Button>(main, "buttonOptions");
if (send.IsEnabled || optionsButton.IsEnabled)
    throw new Exception("Chat controls should be disabled before connecting.");

main.Enable();
main.AddInformation("UI smoke test");
Dispatcher.UIThread.RunJobs();
if (!send.IsEnabled || !optionsButton.IsEnabled)
    throw new Exception("Chat controls were not enabled.");
var chat = Required<SelectableTextBlock>(main, "chatDisplay");
if (chat.Inlines == null || !chat.Inlines.OfType<Run>().Any(run => run.Text == "UI smoke test"))
    throw new Exception("Chat information was not displayed.");

var options = new OptionsForm();
options.Show(main);
Dispatcher.UIThread.RunJobs();
var autoFaction = Required<RadioButton>(options, "radioButtonFactionAuto");
var manualFaction = Required<RadioButton>(options, "radioButtonFactionManual");
var factions = Required<ComboBox>(options, "comboBoxFaction");
manualFaction.IsChecked = true;
if (autoFaction.IsChecked == true || !factions.IsEnabled)
    throw new Exception("Manual faction selection did not enable the faction list.");
autoFaction.IsChecked = true;
if (manualFaction.IsChecked == true || factions.IsEnabled)
    throw new Exception("Automatic faction selection did not disable the faction list.");
options.Close();

var keyPrompt = new KeyPromptForm();
keyPrompt.Show(main);
if (native)
{
    keyPrompt.RaiseEvent(new KeyEventArgs
    {
        RoutedEvent = InputElement.KeyDownEvent,
        Key = Key.A,
        PhysicalKey = PhysicalKey.A
    });
}
else
{
    keyPrompt.KeyPress(Key.A, RawInputModifiers.None, PhysicalKey.A, "a");
}
Dispatcher.UIThread.RunJobs();
if (keyPrompt.Result != "A")
    throw new Exception("Physical key selection failed.");

main.Close();
Console.WriteLine(native ? "All native UI tests passed." : "All headless UI tests passed.");

static T Required<T>(Window window, string name) where T : Control =>
    window.FindControl<T>(name) ?? throw new Exception($"Missing control: {name}");
