/*
 * UnixPrinter.cs - Implementation of the
 *			"System.Drawing.Toolkit.UnixPrinter" class.
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

using System.Reflection;
using System.Drawing.Printing;

internal sealed class UnixPrinter : IToolkitPrinter
{
	// Internal state.
	private String name;
#if CONFIG_REFLECTION
	private static Assembly driver;
	private static Type driverType;
#endif

	// Constructor.
	public UnixPrinter(String name)
			{
				this.name = name;
			}

	// Determine if this printer supports duplex operation.
	public bool CanDuplex
			{
				get
				{
					return false;
				}
			}

	// Determine if this printer is a plotter.
	public bool IsPlotter
			{
				get
				{
					return false;
				}
			}

	// Get the output port for the printer.  String.Empty if none.
	public String OutputPort
			{
				get
				{
					return String.Empty;
				}
			}

	// Get the paper sizes supported by this printer.
	public PaperSize[] PaperSizes
			{
				get
				{
					return new PaperSize[] {
						ToolkitManager.GetStandardPaperSize
							(PaperKind.Letter),
						ToolkitManager.GetStandardPaperSize
							(PaperKind.A4),
						ToolkitManager.GetStandardPaperSize
							(PaperKind.Executive),
						ToolkitManager.GetStandardPaperSize
							(PaperKind.Legal),
					};
				}
			}

	// Get the paper sources supported by this printer.
	public PaperSource[] PaperSources
			{
				get
				{
					return new PaperSource[] {
						ToolkitManager.GetStandardPaperSource
							(PaperSourceKind.Upper)
					};
				}
			}

	// Get the resolutions supported by this printer.
	public PrinterResolution[] PrinterResolutions
			{
				get
				{
					return new PrinterResolution[] {
						ToolkitManager.GetStandardPrinterResolution
							(PrinterResolutionKind.Medium, 300, 300)
					};
				}
			}

	// Determine if this printer supports color.
	public bool SupportsColor
			{
				get
				{
					return false;
				}
			}

	// Load the default page settings from the printer.
	public void LoadDefaults(PageSettings defaults)
			{
				// Nothing to do here: the standard defaults are good enough.
			}

	// Call the print driver (currently, System.Drawing.Postscript).
	private static Object CallDriver(String name, Object[] args)
			{
			#if CONFIG_REFLECTION
				lock(typeof(UnixPrinter))
				{
					if(driver == null)
					{
						driver = Assembly.Load("System.Drawing.Postscript");
					}
					if(driverType == null)
					{
						driverType = driver.GetType
							("System.Drawing.Toolkit.PostscriptDriver");
						if(driverType == null)
						{
							throw new NotSupportedException();
						}
					}
					return driverType.InvokeMember
								(name,
								 BindingFlags.InvokeMethod |
								 	BindingFlags.Static |
									BindingFlags.Public,
								 null, null, args, null, null, null);
				}
			#else
				throw new NotSupportedException();
			#endif
			}

	// Create an "IToolkitGraphics" object that can be used
	// to perform text measurement for this printer.
	public IToolkitGraphics CreateMeasurementGraphics()
			{
				return (IToolkitGraphics)CallDriver
					("CreateMeasurementGraphics",
					 new Object[] {name});
			}

	// Get a session handler for this printer to print a document.
	public IToolkitPrintSession GetSession(PrintDocument document)
			{
				return (IToolkitPrintSession)CallDriver
					("GetSession", new Object[] {name, document});
			}

}; // class UnixPrinter

}; // namespace System.Drawing.Toolkit
