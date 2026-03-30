using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Chernobyl_Relay_Chat
{
    public partial class OptionsForm : Window
    {
        public OptionsForm()
        {
            InitializeComponent();

            Title = CRCStrings.Localize("options_title");
            buttonOK.Content     = CRCStrings.Localize("options_ok");
            buttonCancel.Content = CRCStrings.Localize("options_cancel");

            tabPageClient.Header = CRCStrings.Localize("options_tab_client");
            tabPageGame.Header   = CRCStrings.Localize("options_tab_game");

            labelLanguage.Text          = CRCStrings.Localize("options_language");
            labelChannel.Text           = CRCStrings.Localize("options_channel");
            radioButtonFactionAuto.Content   = CRCStrings.Localize("options_auto_faction");
            radioButtonFactionManual.Content = CRCStrings.Localize("options_manual_faction");
            labelName.Text              = CRCStrings.Localize("options_name");
            buttonRandom.Content        = CRCStrings.Localize("options_name_random");
            checkBoxTimestamps.Content  = CRCStrings.Localize("options_timestamps");
            checkBoxDeathSend.Content   = CRCStrings.Localize("options_send_deaths");
            checkBoxDeathReceive.Content = CRCStrings.Localize("options_receive_deaths");
            labelDeathInterval.Text     = CRCStrings.Localize("options_death_interval");
            labelNewsDuration.Text      = CRCStrings.Localize("options_news_duration");
            labelChatKey.Text           = CRCStrings.Localize("options_chat_key");
            buttonChatKey.Content       = CRCStrings.Localize("options_chat_key_change");
            checkBoxNewsSound.Content   = CRCStrings.Localize("options_news_sound");
            checkBoxCloseChat.Content   = CRCStrings.Localize("options_close_chat");

            // Mutual exclusion for radio buttons (no GroupName in AXAML)
            radioButtonFactionAuto.IsCheckedChanged += (_, _) =>
            {
                if (radioButtonFactionAuto.IsChecked == true)
                    radioButtonFactionManual.IsChecked = false;
                comboBoxFaction.IsEnabled = radioButtonFactionManual.IsChecked == true;
            };
            radioButtonFactionManual.IsCheckedChanged += (_, _) =>
            {
                if (radioButtonFactionManual.IsChecked == true)
                    radioButtonFactionAuto.IsChecked = false;
                comboBoxFaction.IsEnabled = radioButtonFactionManual.IsChecked == true;
            };
            checkBoxDeathReceive.IsCheckedChanged += (_, _) =>
                numericUpDownDeath.IsEnabled = checkBoxDeathReceive.IsChecked == true;

            buttonOK.Click     += ButtonOK_Click;
            buttonCancel.Click += (_, _) => Close();
            buttonRandom.Click += ButtonRandom_Click;
            buttonChatKey.Click += async (_, _) =>
            {
                var kp = new KeyPromptForm();
                await kp.ShowDialog(this);
                if (kp.Result != null)
                    textBoxChatKey.Text = kp.Result;
            };

            // Load current settings
            comboBoxLanguage.SelectedIndex = languageToIndex.GetValueOrDefault(CRCOptions.Language, 0);
            comboBoxChannel.SelectedIndex  = channelToIndex.GetValueOrDefault(CRCOptions.Channel, 0);
            radioButtonFactionAuto.IsChecked   = CRCOptions.AutoFaction;
            radioButtonFactionManual.IsChecked = !CRCOptions.AutoFaction;
            comboBoxFaction.SelectedIndex = factionToIndex.GetValueOrDefault(CRCOptions.ManualFaction, 5);
            comboBoxFaction.IsEnabled = !CRCOptions.AutoFaction;
            textBoxName.Text            = CRCOptions.Name;
            checkBoxTimestamps.IsChecked = CRCOptions.ShowTimestamps;
            checkBoxDeathSend.IsChecked  = CRCOptions.SendDeath;
            checkBoxDeathReceive.IsChecked = CRCOptions.ReceiveDeath;
            numericUpDownDeath.Value    = CRCOptions.DeathInterval;
            numericUpDownDeath.IsEnabled = CRCOptions.ReceiveDeath;
            numericUpDownNewsDuration.Value = CRCOptions.NewsDuration;
            textBoxChatKey.Text         = CRCOptions.ChatKey;
            checkBoxNewsSound.IsChecked = CRCOptions.NewsSound;
            checkBoxCloseChat.IsChecked = CRCOptions.CloseChat;
        }

        private void ButtonOK_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string name = (textBoxName.Text ?? "").Replace(' ', '_');
            string? error = CRCStrings.ValidateNick(name);
            if (error != null)
            {
                // No modal dialog in cross-platform Avalonia without extra packages;
                // show as info in the chat window and keep options open
                CRCDisplay.AddInformation(error);
                return;
            }

            string lang = indexToLanguage[comboBoxLanguage.SelectedIndex];
            if (lang != CRCOptions.Language)
            {
                CRCOptions.Language = lang;
                CRCDisplay.AddInformation(CRCStrings.Localize("options_language_restart"));
            }

            CRCOptions.Channel      = indexToChannel.GetValueOrDefault(comboBoxChannel.SelectedIndex, "#cocrc_english");
            CRCOptions.AutoFaction  = radioButtonFactionAuto.IsChecked == true;
            CRCOptions.ManualFaction = indexToFaction.GetValueOrDefault(comboBoxFaction.SelectedIndex, "actor_stalker");
            CRCOptions.Name         = name;
            CRCOptions.ShowTimestamps = checkBoxTimestamps.IsChecked == true;
            CRCOptions.SendDeath    = checkBoxDeathSend.IsChecked == true;
            CRCOptions.ReceiveDeath = checkBoxDeathReceive.IsChecked == true;
            CRCOptions.DeathInterval = (int)(numericUpDownDeath.Value ?? 90);
            CRCOptions.NewsDuration = (int)(numericUpDownNewsDuration.Value ?? 10);
            CRCOptions.ChatKey      = textBoxChatKey.Text ?? "RETURN";
            CRCOptions.NewsSound    = checkBoxNewsSound.IsChecked == true;
            CRCOptions.CloseChat    = checkBoxCloseChat.IsChecked == true;

            CRCOptions.Save();
            CRCClient.UpdateSettings();
            CRCGame.UpdateSettings();
            Close();
        }

        private void ButtonRandom_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string faction = radioButtonFactionAuto.IsChecked == true
                ? CRCOptions.GameFaction
                : indexToFaction.GetValueOrDefault(comboBoxFaction.SelectedIndex, "actor_stalker");
            textBoxName.Text = CRCStrings.RandomIrcName(faction);
        }

        // ── index ↔ key dictionaries ──────────────────────────────────────────

        private readonly Dictionary<string, int> languageToIndex = new() { ["eng"] = 0, ["ukr"] = 1, ["rus"] = 2 };
        private readonly Dictionary<int, string> indexToLanguage = new() { [0] = "eng", [1] = "ukr", [2] = "rus" };

        private readonly Dictionary<string, int> channelToIndex = new()
        {
            ["#cocrc_english"] = 0, ["#cocrc_english_rp"] = 1, ["#cocrc_slavik"] = 2,
        };
        private readonly Dictionary<int, string> indexToChannel = new()
        {
            [0] = "#cocrc_english", [1] = "#cocrc_english_rp", [2] = "#cocrc_slavik",
        };

        private readonly Dictionary<string, int> factionToIndex = new()
        {
            ["actor_bandit"] = 0, ["actor_csky"] = 1, ["actor_dolg"] = 2, ["actor_ecolog"] = 3,
            ["actor_freedom"] = 4, ["actor_stalker"] = 5, ["actor_killer"] = 6,
            ["actor_army"] = 7, ["actor_monolith"] = 8, ["actor_renegade"] = 9,
        };
        private readonly Dictionary<int, string> indexToFaction = new()
        {
            [0] = "actor_bandit", [1] = "actor_csky", [2] = "actor_dolg", [3] = "actor_ecolog",
            [4] = "actor_freedom", [5] = "actor_stalker", [6] = "actor_killer",
            [7] = "actor_army", [8] = "actor_monolith", [9] = "actor_renegade",
        };
    }
}
