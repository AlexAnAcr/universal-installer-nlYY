using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FTPDataUpdater
{
    static class Program
    {
        public static FTP ftp_client; public static byte answer; public static string reserve_str;
        public static readonly string[] SUPPORT_VERSIONS = new string[] { "1,0" };
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex nmp = new System.Threading.Mutex(true, "{ec141b4c-53b6-4438-b676-e1fec91e87a4}", out bool runned);
            if (runned)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Start_app());
            }
            else
            {
                MessageBox.Show("Одна копия программы уже запущена!", "Ошибка");
            }
        }
        class Start_app : ApplicationContext
        {
            public Start_app()
            {
                while (true)
                {
                    answer = 0;
                    Form form = new Login();
                    form.ShowDialog();
                    if (answer == 0) Environment.Exit(0);
                    answer = 0;
                    form = new Main_form();
                    form.ShowDialog();
                    if (answer == 0) Environment.Exit(0);
                }
            }
        }
    }
}
