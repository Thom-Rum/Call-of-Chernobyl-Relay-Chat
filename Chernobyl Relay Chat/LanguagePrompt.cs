using Avalonia.Controls;

namespace Chernobyl_Relay_Chat
{
    public partial class LanguagePrompt : Window
    {
        /// <summary>Set by the button the user clicks: "eng", "ukr", or "rus".</summary>
        public string Result { get; private set; } = "eng";

        public LanguagePrompt()
        {
            InitializeComponent();
            buttonEnglish.Click += (_, _) => { Result = "eng"; Close(); };
            buttonSlavik.Click  += (_, _) => { Result = "ukr"; Close(); };
            buttonRussian.Click += (_, _) => { Result = "rus"; Close(); };
        }
    }
}
