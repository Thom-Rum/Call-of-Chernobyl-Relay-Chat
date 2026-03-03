using System;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Chernobyl_Relay_Chat
{
    static class Program
    {
        // Font: Graffiti1CTT Regular, as used in S.T.A.L.K.E.R.: Shadow of Chernobyl (GSC Game World, 2007).
        // Bundled with that game and used here for aesthetic consistency.
        // All rights remain with the original authors / GSC Game World.
        public const float FontSize = 10.25f;
        private static readonly PrivateFontCollection privateFonts = new PrivateFontCollection();
        public static FontFamily GraffitiFamily { get; private set; }
        public static Font AppFont { get; private set; }

        private static void LoadFont()
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Chernobyl_Relay_Chat.Assets.Graffiti1CTT Regular.ttf"))
                {
                    if (stream == null) return;
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
                    Marshal.Copy(data, 0, ptr, data.Length);
                    privateFonts.AddMemoryFont(ptr, data.Length);
                    Marshal.FreeCoTaskMem(ptr);
                    if (privateFonts.Families.Length > 0)
                        GraffitiFamily = privateFonts.Families[0];
                }
            }
            catch { }

            AppFont = GraffitiFamily != null
                ? new Font(GraffitiFamily, FontSize, FontStyle.Regular, GraphicsUnit.Point)
                : new Font(SystemFonts.DefaultFont.FontFamily, FontSize, FontStyle.Regular, GraphicsUnit.Point);
        }
        // GUID from AssemblyInfo.cs
        private static readonly Mutex mutex = new Mutex(false, "64eb5dda-2131-47fc-a32c-fbc64f440d8a");

        private static Thread displayThread, clientThread;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadFont();

#if !DEBUG
            // Prevent multiple instances
            if (!mutex.WaitOne(TimeSpan.FromSeconds(0), false))
                return;
#endif

            CRCStrings.Load();
            bool optionsLoaded = CRCOptions.Load();
            if (!optionsLoaded)
            {
                // No point localizing this since localization may not even be loaded
                MessageBox.Show("CRC was unable to access the registry, which is needed to preserve settings.\r\n"
                    + "Please try running the application As Administrator.\r\n\r\n"
                    + "CRC не смог получить доступ к реестру, который необходим для сохранения настроек.\r\n"
                    + "Попробуйте запустить приложение с правами администратора.",
                    CRCStrings.Localize("crc_name"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        //    if (CRCUpdate.CheckFirstUpdate())
        //        return;

            displayThread = new Thread(CRCDisplay.Start);
            displayThread.Start();
            clientThread = new Thread(CRCClient.Start);
            clientThread.Start();

            clientThread.Join();
            displayThread.Join();
            CRCOptions.Save();
        }
    }
}
