/*
 * IToolkitPrintSession.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitPrintSession" class.
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

using System.Drawing.Printing;

[NonStandardExtra]
public interface IToolkitPrintSession
{
	// Get or set the document that is associated with this session.
	PrintDocument Document { get; set; }

	// Start the printing session.
	void StartPrint(PrintEventArgs e);

	// End the printing session.
	void EndPrint(PrintEventArgs e);

	// Start printing a page, and return a "Graphics" object for it.
	Graphics StartPage(PrintPageEventArgs e);

	// End printing of the current page.
	void EndPage(PrintPageEventArgs e);

	// Get the preview page information for all pages.  This is only
	// used for preview sessions.  Normal printers should return null.
	PreviewPageInfo[] GetPreviewPageInfo();

	// Get or set the anti-alias flag for preview operations.
	bool UseAntiAlias { get; set; }

}; // interface IToolkitPrintSession

}; // namespace System.Drawing.Toolkit
