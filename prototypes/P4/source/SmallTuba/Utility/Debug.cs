namespace SmallTuba.Utility {
	using System;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// Static debug class used to determine whether
	/// the project is currenctly in debug-mode.
	/// 
	/// Used to modify the calls to external data
	/// sources for correct unit-test response
	/// information and to decide whether to output
	/// debug information such as exceptions.
	/// </summary>
	public class Debug {
		// Used to determine whether to use test versions of
		// the external data sources.
		public static bool ExternalDataSources;

		// Used to determine whether console output should be
		// turned on or not.
		public static bool ConsoleOutput;

		public static void WriteLine(string output) {
			if (ConsoleOutput) { 
				Console.WriteLine(output);
			}
		}
	}
}
