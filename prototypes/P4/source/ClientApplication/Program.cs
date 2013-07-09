namespace ClientApplication {
	using System;


	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The entry point of the client application
	/// </summary>
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			/*if (Debug.ConsoleOutput) {
				Win32.AllocConsole();
			}*/

			Controller controller = new Controller();
			controller.Run();
		}
	}

	/*
	/// <summary>
	/// Helper class for enabling the console
	/// </summary>
	public class Win32 {
		/// <summary>
		/// Enables the console
		/// </summary>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		public static extern Boolean AllocConsole();
	}
	*/
}