/*
 * IToolkitPrinter.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitPrinter" class.
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
public interface IToolkitPrinter
{
	// Determine if this printer supports duplex operation.
	bool CanDuplex { get; }

	// Determine if this printer is a plotter.
	bool IsPlotter { get; }

	// Get the output port for the printer.  String.Empty if none.
	String OutputPort { get; }

	// Get the paper sizes supported by this printer.
	PaperSize[] PaperSizes { get; }

	// Get the paper sources supported by this printer.
	PaperSource[] PaperSources { get; }

	// Get the resolutions supported by this printer.
	PrinterResolution[] PrinterResolutions { get; }

	// Determine if this printer supports color.
	bool SupportsColor { get; }

	// Load the default page settings from the printer.
	void LoadDefaults(PageSettings defaults);

	// Create an "IToolkitGraphics" object that can be used
	// to perform text measurement for this printer.
	IToolkitGraphics CreateMeasurementGraphics();

	// Get a session handler for this printer to print a document.
	IToolkitPrintSession GetSession(PrintDocument document);

}; // interface IToolkitPrinter

}; // namespace System.Drawing.Toolkit
