/*
 * PrinterSettings.cs - Implementation of the
 *			"System.Drawing.Printing.PrinterSettings" class.
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
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections;

#if !ECMA_COMPAT
[Serializable]
[ComVisible(false)]
#endif
public class PrinterSettings : ICloneable
{
	// Internal state.
	private bool collate;
	private short copies;
	[NonSerializedAttribute]
	private PageSettings defaultPageSettings;
	private Duplex duplex;
	private int fromPage;
	private int landscapeAngle;
	private int maximumCopies;
	private int maximumPage;
	private int minimumPage;
	[NonSerializedAttribute]
	private String printerName;
	private PrintRange printRange;
	private bool printToFile;
	private int toPage;
	private IToolkitPrinter toolkitPrinter;

	// Constructor.
	public PrinterSettings()
			{
				collate = false;
				copies = 1;
				duplex = Duplex.Default;
				fromPage = 0;
				landscapeAngle = 0;
				maximumCopies = 1;
				maximumPage = 9999;
				minimumPage = 0;
				printerName = null;
				printRange = PrintRange.AllPages;
				printToFile = false;
				toPage = 0;
				toolkitPrinter = null;
			}

	// Get or set this object's properties.
	public bool CanDuplex
			{
				get
				{
					return ToolkitPrinter.CanDuplex;
				}
			}
	public bool Collate
			{
				get
				{
					return collate;
				}
				set
				{
					collate = value;
				}
			}
	public short Copies
			{
				get
				{
					return copies;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					copies = value;
				}
			}
	public PageSettings DefaultPageSettings
			{
				get
				{
					if(defaultPageSettings == null)
					{
						LoadDefaults();
					}
					return defaultPageSettings;
				}
			}
	public Duplex Duplex
			{
				get
				{
					return duplex;
				}
				set
				{
					duplex = value;
				}
			}
	public int FromPage
			{
				get
				{
					return fromPage;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					fromPage = value;
				}
			}
	public bool IsDefaultPrinter
			{
				get
				{
					return (printerName == null);
				}
			}
	public bool IsPlotter
			{
				get
				{
					return ToolkitPrinter.IsPlotter;
				}
			}
	public bool IsValid
			{
				get
				{
					lock(this)
					{
						if(toolkitPrinter != null)
						{
							return true;
						}
						toolkitPrinter =
							ToolkitManager.PrintingSystem.GetPrinter
								(printerName);
						return (toolkitPrinter != null);
					}
				}
			}
	public int LandscapeAngle
			{
				get
				{
					return landscapeAngle;
				}
				set
				{
					landscapeAngle = value;
				}
			}
	public int MaximumCopies
			{
				get
				{
					return maximumCopies;
				}
			}
	public int MaximumPage
			{
				get
				{
					return maximumPage;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					maximumPage = value;
				}
			}
	public int MinimumPage
			{
				get
				{
					return minimumPage;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					minimumPage = value;
				}
			}
	public String PrinterName
			{
				get
				{
					if(printerName == null)
					{
						return ToolkitManager.PrintingSystem.DefaultPrinterName;
					}
					return printerName;
				}
				set
				{
					lock(this)
					{
						printerName = value;
						toolkitPrinter = null;
					}
				}
			}
	public PrintRange PrintRange
			{
				get
				{
					return printRange;
				}
				set
				{
					printRange = value;
				}
			}
	public bool PrintToFile
			{
				get
				{
					return printToFile;
				}
				set
				{
					printToFile = true;
				}
			}
	public bool SupportsColor
			{
				get
				{
					return ToolkitPrinter.SupportsColor;
				}
			}
	public int ToPage
			{
				get
				{
					return toPage;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					toPage = value;
				}
			}

	// Get the toolkit version of a printer.
	internal IToolkitPrinter ToolkitPrinter
			{
				get
				{
					lock(this)
					{
						if(toolkitPrinter == null)
						{
							toolkitPrinter =
								ToolkitManager.PrintingSystem.GetPrinter
									(printerName);
							if(toolkitPrinter == null)
							{
								throw new InvalidPrinterException(this);
							}
						}
						return toolkitPrinter;
					}
				}
			}

	// Get a list of all installed printers on this system.
	public static StringCollection InstalledPrinters
			{
				get
				{
					return new StringCollection
						(ToolkitManager.PrintingSystem.InstalledPrinters);
				}
			}

	// Get a list of all paper sizes that are supported by this printer.
	public PaperSizeCollection PaperSizes
			{
				get
				{
					return new PaperSizeCollection(ToolkitPrinter.PaperSizes);
				}
			}

	// Get a list of all paper sources that are supported by this printer.
	public PaperSourceCollection PaperSources
			{
				get
				{
					return new PaperSourceCollection
						(ToolkitPrinter.PaperSources);
				}
			}

	// Get a list of all resolutions that are supported by this printer.
	public PrinterResolutionCollection PrinterResolutions
			{
				get
				{
					return new PrinterResolutionCollection
						(ToolkitPrinter.PrinterResolutions);
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return MemberwiseClone();
			}

	// Create a graphics object that can be used to perform text measurement.
	public Graphics CreateMeasurementGraphics()
			{
				return ToolkitManager.CreateGraphics
					(ToolkitPrinter.CreateMeasurementGraphics());
			}

#if !ECMA_COMPAT

	// Get the Win32-specific DEVMODE information for these printer settings.
	public IntPtr GetHdevmode()
			{
				return GetHdevmode(null);
			}
	public IntPtr GetHdevmode(PageSettings pageSettings)
			{
				throw new Win32Exception
					(0, S._("NotSupp_CannotGetDevMode"));
			}

	// Get the Win32-specific DEVNAMES information for these printer settings.
	public IntPtr GetHdevnames()
			{
				throw new Win32Exception
					(0, S._("NotSupp_CannotGetDevNames"));
			}

	// Set the DEVMODE information from a Win32-specific structure.
	public void SetHdevmode(IntPtr hdevmode)
			{
				// Not used in this implementation.
			}

	// Set the DEVNAMES information from a Win32-specific structure.
	public void SetHdevnames(IntPtr hdevnames)
			{
				// Not used in this implementation.
			}

#endif

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[PrinterSettings ");
				builder.Append(PrinterName);
				builder.Append(" Copies=");
				builder.Append(Copies.ToString());
				builder.Append(" Collate=");
				builder.Append(Collate.ToString());
				builder.Append(" Duplex=");
				builder.Append(Duplex.ToString());
				builder.Append(" FromPage=");
				builder.Append(FromPage.ToString());
				builder.Append(" LandscapeAngle=");
				builder.Append(LandscapeAngle.ToString());
				builder.Append(" MaximumCopies=");
				builder.Append(MaximumCopies.ToString());
				builder.Append(" OutputPort=");
				builder.Append(ToolkitPrinter.OutputPort.ToString());
				builder.Append(" ToPage=");
				builder.Append(ToPage.ToString());
				builder.Append(']');
				return builder.ToString();
			}

	// Load the default page settings for the printer.
	private void LoadDefaults()
			{
				// Initialize the defaults object.
				PageSettings defaults = new PageSettings();
				defaults.Color = false;
				defaults.Landscape = false;
				defaults.PaperSize = new PaperSize(PaperKind.Letter);
				defaults.PaperSource =
					new PaperSource(PaperSourceKind.AutomaticFeed, null);
				defaults.PrinterResolution =
					new PrinterResolution
						(PrinterResolutionKind.Medium, 300, 300);
				defaultPageSettings = defaults;

				// Load printer-specific overrides from the system.
				ToolkitPrinter.LoadDefaults(defaults);
			}

	// Collection of paper sizes for a printer.
	public class PaperSizeCollection : ICollection, IEnumerable
	{
		// Internal state.
		private PaperSize[] array;

		// Constructor.
		public PaperSizeCollection(PaperSize[] array)
				{
					this.array = array;
				}

		// Get a specific element within this collection.
		public virtual PaperSize this[int index]
				{
					get
					{
						return array[index];
					}
				}

		// Implement the ICollection interface.
		void ICollection.CopyTo(Array array, int index)
				{
					this.array.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return array.Length;
					}
				}
		bool ICollection.IsSynchronized
				{
					get
					{
						return false;
					}
				}
		Object ICollection.SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return array.GetEnumerator();
				}

	}; // class PaperSizeCollection

	// Collection of paper sources for a printer.
	public class PaperSourceCollection : ICollection, IEnumerable
	{
		// Internal state.
		private PaperSource[] array;

		// Constructor.
		public PaperSourceCollection(PaperSource[] array)
				{
					this.array = array;
				}

		// Get a specific element within this collection.
		public virtual PaperSource this[int index]
				{
					get
					{
						return array[index];
					}
				}

		// Implement the ICollection interface.
		void ICollection.CopyTo(Array array, int index)
				{
					this.array.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return array.Length;
					}
				}
		bool ICollection.IsSynchronized
				{
					get
					{
						return false;
					}
				}
		Object ICollection.SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return array.GetEnumerator();
				}

	}; // class PaperSourceCollection

	// Collection of printer resolutions for a printer.
	public class PrinterResolutionCollection : ICollection, IEnumerable
	{
		// Internal state.
		private PrinterResolution[] array;

		// Constructor.
		public PrinterResolutionCollection(PrinterResolution[] array)
				{
					this.array = array;
				}

		// Get a specific element within this collection.
		public virtual PrinterResolution this[int index]
				{
					get
					{
						return array[index];
					}
				}

		// Implement the ICollection interface.
		void ICollection.CopyTo(Array array, int index)
				{
					this.array.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return array.Length;
					}
				}
		bool ICollection.IsSynchronized
				{
					get
					{
						return false;
					}
				}
		Object ICollection.SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return array.GetEnumerator();
				}

	}; // class PrinterResolutionCollection

	// String collection for printer names.
	public class StringCollection : ICollection, IEnumerable
	{
		// Internal state.
		private String[] array;

		// Constructor.
		public StringCollection(String[] array)
				{
					this.array = array;
				}

		// Get a specific element within this collection.
		public virtual String this[int index]
				{
					get
					{
						return array[index];
					}
				}

		// Implement the ICollection interface.
		void ICollection.CopyTo(Array array, int index)
				{
					this.array.CopyTo(array, index);
				}
		public int Count
				{
					get
					{
						return array.Length;
					}
				}
		bool ICollection.IsSynchronized
				{
					get
					{
						return false;
					}
				}
		Object ICollection.SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return array.GetEnumerator();
				}

	}; // class StringCollection

}; // class PrinterSettings

}; // namespace System.Drawing.Printing
