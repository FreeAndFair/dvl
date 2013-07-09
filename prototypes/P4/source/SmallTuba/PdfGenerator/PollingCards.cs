namespace SmallTuba.PdfGenerator {
	using System.Diagnostics.Contracts;
	using System.IO;
	using PdfSharp.Drawing;
	using PdfSharp.Drawing.BarCodes;
	using PdfSharp.Drawing.Layout;
	using PdfSharp.Pdf;
	using SmallTuba.Entities;

	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// This class generates polling cards.
	/// The design of the polling card, is a rough copy of the official danish pollingcard for national election. 
	/// Each polling card is append as a page to a pdf document.
	/// The pdf document can be saved to a path supplied by the user of the class.
	/// </summary>
	public class PollingCards{
		private const int Width = 210, Height = 100, RightMargin = 35, LeftMargin =35;
		private PdfDocument document;
		private XForm template;

		/// <summary>
		/// May I have a new polling cards generator for this election?
		/// </summary>
		/// <param name="electionName">The name of the election</param>
		/// <param name="electionDate">The date of the election</param>
		/// <param name="electionTime">The timespan of the election</param>
		public PollingCards(string electionName, string electionDate, string electionTime){
			Contract.Requires(electionName != null);
			Contract.Requires(electionDate != null);
			Contract.Requires(electionTime != null);
			
			//Create the a new document
			document = new PdfDocument();

			//Create a template containing the non specific voter details
			template = new XForm(document,XUnit.FromMillimeter(Width), XUnit.FromMillimeter(Height));
			XGraphics gfx = XGraphics.FromForm(template);
			AddWatermark(gfx);
			DrawGraphics(gfx);
			ElectionDetails(gfx, electionName, electionDate, electionTime);
			Descriptions(gfx);
			
			//Release the XGraphics object
			gfx.Dispose();           
		}
		
		/// <summary>
		/// Create a polling card for this person!
		/// </summary>
		/// <param name="person">The voter</param>
		/// <param name="sender">Address of the sender</param>
		/// <param name="pollingVenue">Address of the polling venue</param>
		public void CreatePollingCard(Person person, Address sender, Address pollingVenue){
			Contract.Requires(person != null);

			//Add a new page to the document
			PdfPage page = document.AddPage();
			page.Width = XUnit.FromMillimeter(Width);
			page.Height = XUnit.FromMillimeter(Height);
			XGraphics gfx = XGraphics.FromPdfPage(page);

			//Draw the template
			gfx.DrawImage(template, 0, 0);

			//Draw the voter specific information on the polling card
			FromField(gfx, sender.Name, sender.Street, sender.City);
			ToField(gfx, person.FirstName + " " + person.LastName, person.Street, person.City);
			PollingTable(gfx, person.PollingTable);
			VotingNumber(gfx, person.VoterId.ToString());                 
			PollingVenue(gfx, pollingVenue.Name, pollingVenue.Street, pollingVenue.City);

			//Release the XGraphics object
			gfx.Dispose();
		}

		
		/// <summary>
		/// //Can you save all the polling card on this location on the harddrive?
		/// </summary>
		/// <param name="path">The location on the disk</param>
		public void SaveToDisk(string path){
			Contract.Requires(path != null);
			Contract.Ensures(File.Exists(path));

			document.Save(path);
		}

		/// <summary>
		/// Draws the Watermark
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		private void AddWatermark(XGraphics gfx){
			gfx.RotateTransform(-40);
			XFont font = new XFont("Arial Rounded MT Bold", 60, XFontStyle.Regular);
			XBrush brush = new XSolidBrush(XColor.FromArgb(70, 255, 0, 0));
			gfx.DrawString("VALGKORT", font, brush, -120, 250);
			gfx.RotateTransform(40);           
		}

		/// <summary>
		/// Draws the sender of the polling card
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="name">Name of the sender</param>
		/// <param name="street">Street of the sender</param>
		/// <param name="city">City of the sender</param>
		private void FromField(XGraphics gfx, string name, string street, string city){
			XFont font = new XFont("Lucida Console", 8, XFontStyle.Italic);
			XTextFormatter tf = new XTextFormatter(gfx);
			string adress = name + System.Environment.NewLine + street + System.Environment.NewLine + city;
			tf.DrawString(adress, font, XBrushes.Black, new XRect(310, 95, 100, 50));
		}

	   /// <summary>
	   /// Draws the receiver of the polling card
	   /// </summary>
	   /// <param name="gfx">XGraphics object</param>
	   /// <param name="name">Name of the receiver</param>
	   /// <param name="street">Street of the receiver</param>
	   /// <param name="city">City of the receiver</param>
		private void ToField(XGraphics gfx, string name, string street, string city){
			XFont font = new XFont("Lucida Console", 8, XFontStyle.Regular);
			XTextFormatter tf = new XTextFormatter(gfx);
			string adress = name + System.Environment.NewLine + street + System.Environment.NewLine + city;
			tf.DrawString(adress, font, XBrushes.Black, new XRect(310, 155, 100, 50));
		}

		/// <summary>
		/// Draws the statics descriptions of the values
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		private void Descriptions(XGraphics gfx){
			XFont font = new XFont("Arial", 5, XFontStyle.Regular);
			gfx.DrawString("Afstemningssted:", font, XBrushes.Black, 40, 90);
			gfx.DrawString("Valgbord:", font, XBrushes.Black, 40, 162);
			gfx.DrawString("Vælgernr.:", font, XBrushes.Black, 40, 192);
			gfx.DrawString("Afstemningstid:", font, XBrushes.Black, 40, 222);
			gfx.DrawString("Afsender:", font, XBrushes.Black, 305, 90);
			gfx.DrawString("Modtager:", font, XBrushes.Black, 305, 150);
		}

		/// <summary>
		/// Draws the main election details
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="electionName">The name of the election</param>
		/// <param name="electionDate">What date it is</param>
		/// <param name="electionTime">The timespan of the election</param>
		private void ElectionDetails(XGraphics gfx, string electionName, string electionDate, string electionTime){
			XFont font = new XFont("Arial", 12, XFontStyle.Bold);
			gfx.DrawString(electionName, font, XBrushes.Black, 35, 40);
			gfx.DrawString(electionDate, font, XBrushes.Black, 35, 55);

			ElectionTime(gfx, electionTime);

			XFont font2 = new XFont("Arial", 8, XFontStyle.BoldItalic);
			gfx.DrawString("Medbring kortet ved afstemningen", font2, XBrushes.Black, 35, 75);
			
		}

		/// <summary>
		/// Draws the address of the polling venue
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="name">The name of the polling venue</param>
		/// <param name="street">The street of the polling venue</param>
		/// <param name="city">The city of the polling venue</param>
		private void PollingVenue(XGraphics gfx, string name, string street, string city){
			XFont font = new XFont("Arial", 9, XFontStyle.Bold);
			XTextFormatter tf = new XTextFormatter(gfx);
			string adress = name + System.Environment.NewLine + street + System.Environment.NewLine + city;
			tf.DrawString(adress, font, XBrushes.Black, new XRect(45, 95, 100, 50));
		}

		/// <summary>
		/// Draws the polling table
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="table">The polling table number</param>
		private void PollingTable(XGraphics gfx, string table){
			XFont font = new XFont("Arial", 9, XFontStyle.Bold);
			gfx.DrawString(table, font, XBrushes.Black, 80, 162);
		}

		/// <summary>
		/// Draws the unique voter id
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="votingNumber">The unique voter id</param>
		private void VotingNumber(XGraphics gfx, string votingNumber){
			XFont font = new XFont("Arial", 9, XFontStyle.Bold);
			gfx.DrawString(votingNumber, font, XBrushes.Black, 80, 192);
			Barcode(gfx, votingNumber);
		}

		/// <summary>
		/// Draws the voter id as a barcode and a human readable string
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		/// <param name="votingNumber">The unique voter id </param>
		private void Barcode(XGraphics gfx, string votingNumber){
			//The barcode type
			BarCode barcode = new Code3of9Standard();
			barcode.Text = votingNumber;

			//Indicator to the barcode scanner where the barcode starts and ends
			barcode.StartChar = '*';
			barcode.EndChar = '*';

			//Draws the voter id as a barcode
			barcode.Size = (XSize)(new XPoint(120, 20));
			gfx.DrawBarCode(barcode, XBrushes.Black, new XPoint(310, 40));

			//Draws the voter id as a string
			XFont font = new XFont("Lucida Console", 7, XFontStyle.Regular);
			gfx.DrawString(votingNumber, font, XBrushes.Black, 310, 35);
		}

		/// <summary>
		/// Draws the timespan for the election
		/// </summary>
		/// <param name="gfx">xGraphics object</param>
		/// <param name="time">The timespan for the election</param>
		private void ElectionTime(XGraphics gfx, string time){
			XFont font = new XFont("Arial", 9, XFontStyle.Bold);
			gfx.DrawString(time, font, XBrushes.Black, 80, 222);
		}

		/// <summary>
		/// Draws the graphical lines one the polling card
		/// </summary>
		/// <param name="gfx">XGraphics object</param>
		private void DrawGraphics(XGraphics gfx){
			//the size and color of the lines
			XPen pen = new XPen(XColor.FromName("Black"), 0.5);
			
			//The rectangle around the polling venue address
			gfx.DrawRectangle(pen, LeftMargin, 80, 220, 60);

			//The rectangle around the polling table 
			gfx.DrawRectangle(pen, LeftMargin, 150, 220, 20);

			//The rectangle around the voter id
			gfx.DrawRectangle(pen, LeftMargin, 180, 220, 20);
			
			//The rectangle around the voting timespan
			gfx.DrawRectangle(pen, LeftMargin, 210, 220, 20);

			//The vertical separate line
			gfx.DrawLine(pen, 300, 20, 300, 250);

			//The horizontal separate linjes
			gfx.DrawLine(pen, LeftMargin, 250, gfx.PageSize.Width - RightMargin, 250);   
			gfx.DrawLine(pen, 300, 80, gfx.PageSize.Width - RightMargin, 80);
			gfx.DrawLine(pen, 300, 140, gfx.PageSize.Width - RightMargin, 140);

			//The crossed lines on the sender address
			gfx.DrawLine(pen, 300, 80, 450, 140);
			gfx.DrawLine(pen, 300, 140, 450, 80);
		}      
	}
}
