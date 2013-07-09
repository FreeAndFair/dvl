namespace Central
{
    using System;
    using System.Windows.Forms;


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Launch 'Central' client.
            new Central.Main();
        }
    }
}
