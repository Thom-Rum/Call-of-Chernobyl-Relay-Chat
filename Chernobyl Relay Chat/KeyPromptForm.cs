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
            // Use PhysicalKey (scan-code based, layout-independent) so that keys
            // like '#' on UK or punctuation on AZERTY map to the correct DIK code
            // regardless of the user's active keyboard layout.
            if (physicalKeyToDik.TryGetValue(e.PhysicalKey, out string? dik))
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

        private static readonly Dictionary<PhysicalKey, string> physicalKeyToDik = new()
        {
            // Letters – physical position = DIK scan code, independent of layout labels
            [PhysicalKey.A] = "A",  [PhysicalKey.B] = "B",  [PhysicalKey.C] = "C",
            [PhysicalKey.D] = "D",  [PhysicalKey.E] = "E",  [PhysicalKey.F] = "F",
            [PhysicalKey.G] = "G",  [PhysicalKey.H] = "H",  [PhysicalKey.I] = "I",
            [PhysicalKey.J] = "J",  [PhysicalKey.K] = "K",  [PhysicalKey.L] = "L",
            [PhysicalKey.M] = "M",  [PhysicalKey.N] = "N",  [PhysicalKey.O] = "O",
            [PhysicalKey.P] = "P",  [PhysicalKey.Q] = "Q",  [PhysicalKey.R] = "R",
            [PhysicalKey.S] = "S",  [PhysicalKey.T] = "T",  [PhysicalKey.U] = "U",
            [PhysicalKey.V] = "V",  [PhysicalKey.W] = "W",  [PhysicalKey.X] = "X",
            [PhysicalKey.Y] = "Y",  [PhysicalKey.Z] = "Z",
            // Function keys
            [PhysicalKey.F1]  = "F1",  [PhysicalKey.F2]  = "F2",  [PhysicalKey.F3]  = "F3",
            [PhysicalKey.F4]  = "F4",  [PhysicalKey.F5]  = "F5",  [PhysicalKey.F6]  = "F6",
            [PhysicalKey.F7]  = "F7",  [PhysicalKey.F8]  = "F8",  [PhysicalKey.F9]  = "F9",
            [PhysicalKey.F10] = "F10", [PhysicalKey.F11] = "F11", [PhysicalKey.F12] = "F12",
            // Number row
            [PhysicalKey.Digit0] = "0", [PhysicalKey.Digit1] = "1", [PhysicalKey.Digit2] = "2",
            [PhysicalKey.Digit3] = "3", [PhysicalKey.Digit4] = "4", [PhysicalKey.Digit5] = "5",
            [PhysicalKey.Digit6] = "6", [PhysicalKey.Digit7] = "7", [PhysicalKey.Digit8] = "8",
            [PhysicalKey.Digit9] = "9",
            // Number row symbols
            [PhysicalKey.Backquote] = "GRAVE",
            [PhysicalKey.Minus]     = "MINUS",
            [PhysicalKey.Equal]     = "EQUALS",
            // Numpad
            [PhysicalKey.NumPad0]        = "NUMPAD0",
            [PhysicalKey.NumPad1]        = "NUMPAD1",
            [PhysicalKey.NumPad2]        = "NUMPAD2",
            [PhysicalKey.NumPad3]        = "NUMPAD3",
            [PhysicalKey.NumPad4]        = "NUMPAD4",
            [PhysicalKey.NumPad5]        = "NUMPAD5",
            [PhysicalKey.NumPad6]        = "NUMPAD6",
            [PhysicalKey.NumPad7]        = "NUMPAD7",
            [PhysicalKey.NumPad8]        = "NUMPAD8",
            [PhysicalKey.NumPad9]        = "NUMPAD9",
            [PhysicalKey.NumPadDecimal]  = "DECIMAL",
            [PhysicalKey.NumPadAdd]      = "ADD",
            [PhysicalKey.NumPadSubtract] = "SUBTRACT",
            [PhysicalKey.NumPadMultiply] = "MULTIPLY",
            [PhysicalKey.NumPadDivide]   = "DIVIDE",
            // Control keys
            [PhysicalKey.Enter]     = "RETURN",
            [PhysicalKey.Escape]    = "ESCAPE",
            [PhysicalKey.Tab]       = "TAB",
            [PhysicalKey.Space]     = "SPACE",
            [PhysicalKey.Backspace] = "BACKSPACE",
            // Punctuation (physical positions – layout-independent)
            [PhysicalKey.Slash]         = "SLASH",
            [PhysicalKey.Quote]         = "APOSTROPHE",
            [PhysicalKey.Backslash]     = "BACKSLASH",
            [PhysicalKey.IntlBackslash] = "OEM_102",   // ISO extra key: UK \|, EU <>
            [PhysicalKey.BracketLeft]   = "LBRACKET",
            [PhysicalKey.BracketRight]  = "RBRACKET",
            [PhysicalKey.Semicolon]     = "SEMICOLON",
            [PhysicalKey.Comma]         = "COMMA",
            [PhysicalKey.Period]        = "PERIOD",
            // Navigation
            [PhysicalKey.Delete]     = "DELETE",
            [PhysicalKey.End]        = "END",
            [PhysicalKey.Home]       = "HOME",
            [PhysicalKey.Insert]     = "INSERT",
            [PhysicalKey.ArrowLeft]  = "LEFT",
            [PhysicalKey.ArrowRight] = "RIGHT",
            [PhysicalKey.ArrowUp]    = "UP",
            [PhysicalKey.ArrowDown]  = "DOWN",
        };
    }
}

