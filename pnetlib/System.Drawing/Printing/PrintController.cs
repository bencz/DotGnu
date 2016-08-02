/*
 * PrintController.cs - Implementation of the
 *			"System.Drawing.Printing.PrintController" class.
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

public abstract class PrintController
{
	// Constructor.
	public PrintController() {}

	// Event that is emitted at the end of a page.
	public virtual void OnEndPage
				(PrintDocument document, PrintPageEventArgs e)
			{
				// Nothing to do here.
			}

	// Event that is emitted at the end of the print process.
	public virtual void OnEndPrint
				(PrintDocument document, PrintEventArgs e)
			{
				// Nothing to do here.
			}

	// Event that is emitted at the start of a page.
	public virtual Graphics OnStartPage
				(PrintDocument document, PrintPageEventArgs e)
			{
				// Nothing to do here.
				return null;
			}

	// Event that is emitted at the start of the print process.
	public virtual void OnStartPrint
				(PrintDocument document, PrintEventArgs e)
			{
				// Nothing to do here.
			}

}; // class PrintController

}; // namespace System.Drawing.Printing
