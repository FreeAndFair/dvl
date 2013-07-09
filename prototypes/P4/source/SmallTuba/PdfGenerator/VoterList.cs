namespace SmallTuba.PdfGenerator
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using PdfSharp;
	using PdfSharp.Drawing;
	using PdfSharp.Drawing.Layout;
	using PdfSharp.Pdf;
	using SmallTuba.Entities;

	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// This class generates voter lists.
	/// The number of rows is flexible, and the size of the font will automatically be adjusted.
	/// The list can be saved to the harddrive as a pdf file, when all the voters are added.
	/// </summary>
	public class VoterList{
		private const int TopMargin = 100, BottonMargin = 50, RightMargin = 50, LeftMargin = 50;
		private uint count, rows;
		private double nameFieldX, cprnrFieldX, voternrFieldX, rowDistance;
		private string electionName, electionDate, pollingTable;
		private XFont font;
		private XGraphics gfx;
		private PdfDocument document;

		/// <summary>
		/// May I have a new voting list for this election?
		/// </summary>
		/// <param name="rows">Numbers of rows on each page</param>
		/// <param name="electionName">The name of the election</param>
		/// <param name="electionDate">The date of the election</param>
		/// <param name="pollingTable">The polling table</param>
		public VoterList(uint rows, string electionName, string electionDate, string pollingTable){
			Contract.Requires(rows > 20);
			Contract.Requires(electionName != null);
			Contract.Requires(electionDate != null);
			Contract.Requires(pollingTable != null);

			this.rows = rows;
			this.electionName = electionName;
			this.electionDate = electionDate;
			this.pollingTable = pollingTable;
			this.document = new PdfDocument();
			this.AddPage();
			this.font = this.CreateFont();
		}

		/// <summary>
		/// Can you save the voting list to this location on the harddrive?
		/// </summary>
		/// <param name="person">The person</param>
		public void AddVoter(Person person){
			Contract.Requires(person != null);

			//Check for new page
			if (count == rows){
				this.AddPage();
			}
			count++;
			double positionY = TopMargin + (rowDistance * count) - 2;
			this.gfx.DrawString(person.FirstName + " " + person.LastName, font, XBrushes.Black, nameFieldX, positionY);
			this.gfx.DrawString(person.Cpr, font, XBrushes.Black, cprnrFieldX, positionY);
			this.gfx.DrawString(person.VoterId.ToString(), font, XBrushes.Black, voternrFieldX, positionY);
		}

		/// <summary>
		/// Can you save the voting list to this location on the harddrive?
		/// </summary>
		/// <param name="path">The location on the harddrive</param>
		public void SaveToDisk(String path){
			Contract.Requires(path != null);
			Contract.Ensures(File.Exists(path));

			this.AddPageNumbers();
			this.document.Save(path);
		}

		/// <summary>
		/// Creates a new font, where the font size is dependent of the numbers of rows on each page
		/// </summary>
		/// <returns>XFont object</returns>
		private XFont CreateFont(){
			int fontsize = 1;
			var font = new XFont("Arial", fontsize, XFontStyle.Regular);

			//Tests that the font size fits inside a row
			while (gfx.MeasureString("ABC", font).Height < rowDistance - 2 && fontsize <= 12){
				font = new XFont("Arial", fontsize, XFontStyle.Regular);
				fontsize++;
			}
			return font;
		}

		/// <summary>
		/// Creates a new new page and adds it to the pdf document.
		/// </summary>
		private void AddPage(){
			//Releases the Xgraphics object for the previous page
			if (gfx != null){
				gfx.Dispose();
			}
			PdfPage page = document.AddPage();

			//Size of the page
			page.Size = PageSize.A4;

			//Sets the Xgraphics associated with the current page 
			this.gfx = XGraphics.FromPdfPage(page);
			
			//Reset the row counter
			this.count = 0;

			//Draw the template
			this.DrawTemplate(page);
		}

		/// <summary>
		/// Draws the template on the page
		/// </summary>
		/// <param name="page">PdfPage</param>
		private void DrawTemplate(PdfPage page){
			XRect rect = this.DrawOuterFrame(page);
			this.DrawInnerFrame(rect);
			this.DrawColumnDescriptions(rect);
			this.SetDescriptionFieldsPostitions(rect);
			this.DrawElectionInformation();
		}

		/// <summary>
		/// Draws the outer lines on the page, and return a XRect used for the lines inside the outer frame
		/// </summary>
		/// <param name="page">PdfPage</param>
		/// <returns>XRect the size of the frame</returns>
		private XRect DrawOuterFrame(PdfPage page){
			XPen penBold = new XPen(XColor.FromName("Black"), 1.0);
			XRect rect = new XRect(LeftMargin, TopMargin, page.Width - RightMargin - LeftMargin, page.Height - BottonMargin - TopMargin);
			this.gfx.DrawRectangle(penBold, rect);
			return rect;
		}

		/// <summary>
		/// Draws the table lines inside the outerframe
		/// </summary>
		/// <param name="rect">XRect the size of the outer frame</param>
		private void DrawInnerFrame(XRect rect){
			XPen penRegular = new XPen(XColor.FromName("Black"), 0.5);
			
			//The vertical lines
			this.gfx.DrawLine(penRegular, rect.TopLeft.X + rect.Width / 2, rect.TopLeft.Y, rect.TopLeft.X + rect.Width / 2, rect.BottomLeft.Y);
			this.gfx.DrawLine(penRegular, rect.TopLeft.X + rect.Width * 0.75, rect.TopLeft.Y, rect.TopLeft.X + rect.Width * 0.75, rect.BottomLeft.Y);
			
			//The horizontal lines
			for (double i = rect.TopLeft.Y + (rect.Height / rows); i < rect.BottomLeft.Y; i += rect.Height / rows){
				this.gfx.DrawLine(penRegular, rect.TopLeft.X, i, rect.TopRight.X, i);
			}
		}

		/// <summary>
		/// Draws the descriptions on top of each column
		/// </summary>
		/// <param name="rect">XRect the size of the outer frame</param>
		private void DrawColumnDescriptions(XRect rect){
			XFont font = new XFont("Arial", 10, XFontStyle.Bold);
			this.gfx.DrawString("Navn", font, XBrushes.Black, 160, rect.TopLeft.Y - 2);
			this.gfx.DrawString("CPR nr.", font, XBrushes.Black, 340, rect.TopLeft.Y - 2);
			this.gfx.DrawString("Vælgernr.", font, XBrushes.Black, 460, rect.TopLeft.Y - 2);
		}

		/// <summary>
		/// Calculate where the first row in the table is for each column
		/// </summary>
		/// <param name="rect">XRect the size of the outer frame</param>
		private void SetDescriptionFieldsPostitions(XRect rect){
			this.rowDistance = rect.Height / rows;
			this.nameFieldX = LeftMargin + 3;
			this.cprnrFieldX = rect.TopLeft.X + rect.Width / 2 + 3;
			this.voternrFieldX = rect.TopLeft.X + rect.Width * 0.75 + 3;
		}

		/// <summary>
		/// Writes the page numbers on each page in the document
		/// </summary>
		private void AddPageNumbers(){
			this.gfx.Dispose();
			PdfPages pages = document.Pages;
			for (int i = 0; i < document.PageCount; i++){
				int pageNumber = i + 1;
				PdfPage page = pages[i];
				String text = "Side " + pageNumber + " af " + document.PageCount;
				XGraphics gfx = XGraphics.FromPdfPage(page);
				gfx.DrawString(text, font, XBrushes.Black, page.Width/2-(gfx.MeasureString(text, font).Width/2) , page.Height-BottonMargin/2);
			}
		}

		/// <summary>
		/// Draws informations about the election
		/// </summary>
		private void DrawElectionInformation(){
			XFont font = new XFont("Arial", 12, XFontStyle.Bold);
			XTextFormatter tf = new XTextFormatter(gfx);
			String text = electionName + System.Environment.NewLine + this.electionDate;
			tf.DrawString(text, font, XBrushes.Black, new XRect(LeftMargin,TopMargin/3,200,50));
			tf.DrawString("Bord "+ this.pollingTable.ToString(), font, XBrushes.Black, new XRect(gfx.PageSize.Width/2, TopMargin/3, 200, 50));
		}
	}
}