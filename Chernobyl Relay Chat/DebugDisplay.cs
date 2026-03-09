using Avalonia.Controls;
using Avalonia.Threading;
using System.Text;

namespace Chernobyl_Relay_Chat
{
    public partial class DebugDisplay : Window
    {
        private readonly StringBuilder _rawSb = new StringBuilder();

        public DebugDisplay()
        {
            InitializeComponent();
        }

        public void AddRaw(string message)
        {
            if (_rawSb.Length > 0)
                _rawSb.Append('\n');
            _rawSb.Append(message);
            Dispatcher.UIThread.Post(() =>
            {
                textBoxRaw.Text = _rawSb.ToString();
                textBoxRaw.CaretIndex = textBoxRaw.Text?.Length ?? 0;
                scrollViewer.ScrollToEnd();
            });
        }
    }
}
