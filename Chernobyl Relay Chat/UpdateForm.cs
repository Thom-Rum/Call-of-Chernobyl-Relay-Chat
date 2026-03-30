using Avalonia.Controls;

namespace Chernobyl_Relay_Chat
{
    public partial class UpdateForm : Window
    {
        public UpdateForm(bool gamedataRequired, string releaseNotes)
        {
            InitializeComponent();
            labelDescription.Text = CRCStrings.Localize("update_description");
            labelGamedata.Text    = gamedataRequired ? CRCStrings.Localize("update_gamedata") : "";
            labelReleaseNotes.Text = CRCStrings.Localize("update_notes");
            buttonDownload.Content = CRCStrings.Localize("update_download");
            buttonClose.Content    = CRCStrings.Localize("update_close");
            // Strip basic HTML tags for plain-text display
            textNotes.Text = System.Text.RegularExpressions.Regex.Replace(releaseNotes, "<[^>]+>", "");
            buttonClose.Click    += (_, _) => Close();
            buttonDownload.Click += (_, _) =>
            {
                // TODO: trigger download once update checker is rewritten
                Close();
            };
        }
    }
}
