using Avalonia.Controls;
using Avalonia.Threading;
using System.Text;

namespace Chernobyl_Relay_Chat
{
    public partial class DebugDisplay : Window
    {
        private readonly StringBuilder _rawSb = new StringBuilder();
        private readonly object _lock = new object();

        public DebugDisplay()
        {
            InitializeComponent();
        }

        public void AddRaw(string message)
        {
            string text;
            lock (_lock)
            {
                if (_rawSb.Length > 0)
                    _rawSb.Append('\n');
                _rawSb.Append(message);
                text = _rawSb.ToString();
            }
            Dispatcher.UIThread.Post(() =>
            {
                textBoxRaw.Text = text;
                textBoxRaw.CaretIndex = textBoxRaw.Text?.Length ?? 0;
                scrollViewer.ScrollToEnd();
            });
        }
    }
}
