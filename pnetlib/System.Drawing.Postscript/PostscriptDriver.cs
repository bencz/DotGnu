/*
 * PostscriptDriver.cs - Implementation of the
 *			"System.Drawing.Toolkit.PostscriptDriver" class.
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
using System.Drawing.Toolkit;

public sealed class PostscriptDriver
{
	[TODO]
	// Create a graphics object for performing text measurement.
	public static IToolkitGraphics CreateMeasurementGraphics
				(String printerName)
			{
				return null;
			}

	// Create a print session object for a particular document.
	public static IToolkitPrintSession GetSession
				(String printerName, PrintDocument document)
			{
				return new PostscriptPrintSession(printerName, document);
			}

}; // class PostscriptDriver

}; // namespace System.Drawing.Postscript
