/*
 * StandardPrintController.cs - Implementation of the
 *			"System.Drawing.Printing.StandardPrintController" class.
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

namespace System.Drawing.Printing
{

using System.Drawing.Toolkit;

public class StandardPrintController : PrintController
{
	// Constructor.
	public StandardPrintController() {}

	// Get the printer session for a document.
	private static IToolkitPrintSession GetSession(PrintDocument document)
			{
				if(document.session == null)
				{
					document.session =
						document.PrinterSettings.ToolkitPrinter
							.GetSession(document);
					document.session.Document = document;
				}
				return document.session;
			}

	// Event that is emitted at the end of a page.
	public override void OnEndPage
				(PrintDocument document, PrintPageEventArgs e)
			{
				GetSession(document).EndPage(e);
			}

	// Event that is emitted at the end of the print process.
	public override void OnEndPrint
				(PrintDocument document, PrintEventArgs e)
			{
				IToolkitPrintSession session = GetSession(document);
				try
				{
					session.EndPrint(e);
				}
				finally
				{
					document.session = null;
				}
			}

	// Event that is emitted at the start of a page.
	public override Graphics OnStartPage
				(PrintDocument document, PrintPageEventArgs e)
			{
				return GetSession(document).StartPage(e);
			}

	// Event that is emitted at the start of the print process.
	public override void OnStartPrint
				(PrintDocument document, PrintEventArgs e)
			{
				IToolkitPrintSession session = GetSession(document);
				session.StartPrint(e);
			}

}; // class StandardPrintController

}; // namespace System.Drawing.Printing
