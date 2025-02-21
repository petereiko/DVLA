using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLA.WindowsApplication
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SplashScreen splashScreen = new SplashScreen();
            Thread splashThread = new Thread(new ThreadStart(() =>
            {
                Application.Run(splashScreen);
            }));

            splashThread.Start();

            // Simulate application loading
            Thread.Sleep(3000); // Adjust the time as needed

            // Start main form
            splashScreen.Invoke(new Action(() => splashScreen.Close()));
            splashThread.Join();

            Application.Run(new LoginForm());
        }
    }
}
