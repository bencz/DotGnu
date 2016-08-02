/*
 * PostscriptPrintSession.cs - Implementation of the
 *			"System.Drawing.Toolkit.PostscriptPrintSession" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Drawing.Toolkit
{

using System.IO;
using System.Text;
using System.Drawing.Printing;

internal sealed class PostscriptPrintSession : IToolkitPrintSession
{
	// Internal state.
	private String printerName;
	private PrintDocument document;
	private TextWriter writer;
	private Encoding encoding;
	private IToolkit toolkit;
	private bool onPage;
	private int pageNum;

	// Constructor.
	public PostscriptPrintSession(String printerName, PrintDocument document)
			{
				this.printerName = printerName;
				this.document = document;
				this.writer = null;
				this.encoding = Encoding.Default;
				this.toolkit = new PostscriptToolkit();
				this.onPage = false;
				this.pageNum = 1;
			}

	// Get or set the document that is associated with this session.
	public PrintDocument Document
			{
				get
				{
					return document;
				}
				set
				{
					document = value;
				}
			}

	// Start the printing session.
	public void StartPrint(PrintEventArgs e)
			{
				// TODO: other output types, especially pipe-to-lpr.
				writer = new StreamWriter("output.ps");

				// Write the Postscript header to the stream.
				writer.WriteLine("%!PS-Adobe-3.0");
				// TODO: bounding box
				writer.WriteLine
					("%%Creator: Portable.NET System.Drawing.Postscript");
				writer.WriteLine("%%DocumentData: Clean8Bit");
				writer.WriteLine("%%DocumentPaperSizes: {0}",
					document.DefaultPageSettings.PaperSize.Kind.ToString());
				if(document.DefaultPageSettings.Landscape)
				{
					writer.WriteLine("%%Orientation: Landscape");
				}
				else
				{
					writer.WriteLine("%%Orientation: Portrait");
				}
				writer.WriteLine("%%Pages: (atend)");
				writer.WriteLine("%%PageOrder: Ascend");
				if(document.DocumentName != "document")
				{
					writer.WriteLine("%%Title: {0}", document.DocumentName);
				}
				writer.WriteLine("%%EndComments");

				// Write the prolog definitions.
				writer.WriteLine("%%BeginProlog");
				foreach(String line in prolog)
				{
					writer.WriteLine(line);
				}
				writer.WriteLine("%%EndProlog");
			}

	// End the printing session.
	public void EndPrint(PrintEventArgs e)
			{
				// Finalize the last page.
				EndPage(null);

				// Output the Postscript trailer.
				writer.WriteLine("%%Trailer");
				writer.WriteLine("%%Pages: {0}", pageNum - 1);
				writer.WriteLine("%%EOF");

				// Close the output stream.
				writer.Close();
				writer = null;
			}

	// Start printing a page, and return a "Graphics" object for it.
	public Graphics StartPage(PrintPageEventArgs e)
			{
				// Make sure that the previous page was closed.
				EndPage(e);

				// Output the page header information.
				writer.WriteLine("%%Page: {0} {0}", pageNum, pageNum);
				writer.WriteLine("%%BeginPageSetup");

				// Save the current VM state, so that we can
				// discard temporary definitions at the end
				// of the page.
				writer.WriteLine("/pagelevel save def");

				// Flip the co-ordinate system so that the top-left
				// margin position on the page is the origin.
				double temp;
				if(e.PageSettings.Landscape)
				{
					// TODO: rotate the co-ordinate system for landscape mode.
				}
				else
				{
					temp = (e.PageBounds.Height * 72.0) / 100.0;
				#if CONFIG_EXTENDED_NUMERICS
					writer.WriteLine("0 {0} translate 1 -1 scale", temp);
				#else
					writer.WriteLine("0 {0} translate 1 -1 scale", (int)temp);
				#endif
				}

				// Mark the end of the page header information.
				writer.WriteLine("%%EndPageSetup");

				// We are now on a page.
				onPage = true;

				// Save the graphics state.  It will be restored and
				// re-saved every time a pen or brush is changed.
				writer.WriteLine("gsave");

				// Create a "Graphics" object for this page and return it.
				return ToolkitManager.CreateGraphics
					(new PostscriptGraphics(toolkit, writer, this));
			}

	// End printing of the current page.
	public void EndPage(PrintPageEventArgs e)
			{
				if(onPage)
				{
					// Restore the VM state at the start of the page.
					writer.WriteLine("grestore pagelevel restore");

					// Show the page.
					writer.WriteLine("showpage");

					// We are no longer processing a page.
					onPage = false;

					// Increase the page count.
					++pageNum;
				}
			}

	// Get the preview page information for all pages.  This is only
	// used for preview sessions.  Normal printers should return null.
	public PreviewPageInfo[] GetPreviewPageInfo()
			{
				// Not used.
				return null;
			}

	// Get or set the anti-alias flag for preview operations.
	public bool UseAntiAlias
			{
				get
				{
					// Not used.
					return false;
				}
				set
				{
					// Not used.
				}
			}

	// Write a string value to the PostScript stream while escaping
	// characters that may cause problems in the printer otherwise.
	internal void Write(String str)
			{
				// See if we can short-cut the conversion process
				// for simple strings with no funky characters.
				int posn = 0;
				char ch;
				while(posn < str.Length)
				{
					ch = str[posn];
					if(ch < 0x20 || ch > 0x7E)
					{
						break;
					}
					else if(ch == '(' || ch == ')' || ch == '\\')
					{
						break;
					}
					++posn;
				}
				if(posn >= str.Length)
				{
					writer.Write('(');
					writer.Write(str);
					writer.Write(')');
					writer.Write(' ');
					return;
				}

				// Convert the string into the final encoding and write it.
				byte[] bytes = encoding.GetBytes(str);
				writer.Write('(');
				foreach(byte b in bytes)
				{
					if(b < 0x20 || b > 0x7E)
					{
						writer.Write('\\');
						writer.Write((char)('0' + ((b >> 6) & 0x03)));
						writer.Write((char)('0' + ((b >> 3) & 0x07)));
						writer.Write((char)('0' + (b & 0x07)));
					}
					else if(b == ((int)'(') ||
							b == ((int)')') ||
							b == ((int)'\\'))
					{
						writer.Write('\\');
						writer.Write((char)b);
					}
					else
					{
						writer.Write((char)b);
					}
				}
				writer.Write(')');
				writer.Write(' ');
			}

	[TODO]
	// The PostScript prolog to add to every output stream.
	private static readonly String[] prolog = {
	};

}; // class PostscriptPrintSession

}; // namespace System.Drawing.Postscript
