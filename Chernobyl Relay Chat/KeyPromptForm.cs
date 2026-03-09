using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.Generic;

namespace Chernobyl_Relay_Chat
{
    public partial class KeyPromptForm : Window
    {
        /// <summary>The DIK key name chosen, or null if the window was closed without a selection.</summary>
        public string? Result { get; private set; }

        public KeyPromptForm()
        {
            InitializeComponent();
            labelHelp.Text  = CRCStrings.Localize("keyprompt_help") + "\n\n";
            labelError.IsVisible = false;
            KeyDown += KeyPromptForm_KeyDown;
        }

        private void KeyPromptForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if ((key >= Key.A && key <= Key.Z) || (key >= Key.F1 && key <= Key.F12))
            {
                Result = key.ToString();
                Close();
            }
            else if (keyToDik.TryGetValue(key, out string? dik))
            {
                Result = dik;
                Close();
            }
            else
            {
                labelError.IsVisible = true;
                labelError.Text = CRCStrings.Localize("keyprompt_error");
            }
        }

        private static readonly Dictionary<Key, string> keyToDik = new()
        {
            [Key.OemTilde]       = "GRAVE",
            [Key.D0]             = "0",
            [Key.D1]             = "1",
            [Key.D2]             = "2",
            [Key.D3]             = "3",
            [Key.D4]             = "4",
            [Key.D5]             = "5",
            [Key.D6]             = "6",
            [Key.D7]             = "7",
            [Key.D8]             = "8",
            [Key.D9]             = "9",
            [Key.OemMinus]       = "MINUS",
            [Key.OemPlus]        = "EQUALS",
            [Key.NumPad0]        = "NUMPAD0",
            [Key.NumPad1]        = "NUMPAD1",
            [Key.NumPad2]        = "NUMPAD2",
            [Key.NumPad3]        = "NUMPAD3",
            [Key.NumPad4]        = "NUMPAD4",
            [Key.NumPad5]        = "NUMPAD5",
            [Key.NumPad6]        = "NUMPAD6",
            [Key.NumPad7]        = "NUMPAD7",
            [Key.NumPad8]        = "NUMPAD8",
            [Key.NumPad9]        = "NUMPAD9",
            [Key.Decimal]        = "DECIMAL",
            [Key.Add]            = "ADD",
            [Key.Subtract]       = "SUBTRACT",
            [Key.Multiply]       = "MULTIPLY",
            [Key.Divide]         = "DIVIDE",
            [Key.Enter]          = "RETURN",
            [Key.Return]         = "RETURN",
            [Key.Escape]         = "ESCAPE",
            [Key.Tab]            = "TAB",
            [Key.Space]          = "SPACE",
            [Key.OemQuestion]    = "SLASH",
            [Key.OemQuotes]      = "APOSTROPHE",
            [Key.Back]           = "BACKSPACE",
            [Key.OemBackslash]   = "BACKSLASH",
            [Key.OemOpenBrackets]  = "LBRACKET",
            [Key.OemCloseBrackets] = "RBRACKET",
            [Key.OemSemicolon]   = "SEMICOLON",
            [Key.OemComma]       = "COMMA",
            [Key.OemPeriod]      = "PERIOD",
            [Key.Delete]         = "DELETE",
            [Key.End]            = "END",
            [Key.Home]           = "HOME",
            [Key.Insert]         = "INSERT",
            [Key.Left]           = "LEFT",
            [Key.Right]          = "RIGHT",
            [Key.Up]             = "UP",
            [Key.Down]           = "DOWN",
        };
    }
}

