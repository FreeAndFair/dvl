namespace AdminApplication {
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	using SmallTuba.Utility;

	internal static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main() {
			if (Debug.ConsoleOutput) {
				Win32.AllocConsole();
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Controller controller = new Controller();
			controller.Run();
		}

		public class Win32 {
			[DllImport("kernel32.dll")]
			public static extern Boolean AllocConsole();
		}
	}
}

