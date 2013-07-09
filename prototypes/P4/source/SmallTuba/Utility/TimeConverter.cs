namespace SmallTuba.Utility {
	using System;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// Source code and idea is taken from the given url:
	/// 
	/// http://codeclimber.net.nz/archive/2007/07/10/convert-a-unix-timestamp-to-a-.net-datetime.aspx
	/// </summary>
	public class TimeConverter {
		public static DateTime ConvertFromUnixTimestamp(double timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

			return origin.AddSeconds(timestamp);
		}


		public static int ConvertToUnixTimestamp(DateTime date) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			TimeSpan diff = date - origin;

			return (int)Math.Floor(diff.TotalSeconds);
		}
	}
}
