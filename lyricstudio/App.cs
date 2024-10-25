using System;
using System.Windows;

namespace ti_Lyricstudio
{
    internal class App
    {
        [STAThread]
        public static void Main()
        {
            Application app = new();
            MainWindow window = new();

            app.Run(window);
        }
    }
}
