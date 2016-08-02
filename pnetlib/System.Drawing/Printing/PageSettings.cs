/*
 * PageSettings.cs - Implementation of the
 *			"System.Drawing.Printing.PageSettings" class.
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

using System.Runtime.InteropServices;
using System.Text;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public class PageSettings : ICloneable
{
	// Internal state.
	private PrinterSettings printerSettings;
	private bool color;
	private bool colorSet;
	private bool landscape;
	private bool landscapeSet;
	private Margins margins;
	private PaperSize paperSize;
	private PaperSource paperSource;
	private PrinterResolution printerResolution;

	// Constructors.
	public PageSettings() : this(null) {}
	public PageSettings(PrinterSettings printerSettings)
			{
				if(printerSettings != null)
				{
					this.printerSettings = printerSettings;
				}
				else
				{
					this.printerSettings = new PrinterSettings();
				}
				margins = new Margins();
			}

	// Get or set this object's properties.
	public Rectangle Bounds
			{
				get
				{
					PaperSize size = PaperSize;
					if(Landscape)
					{
						return new Rectangle(0, 0, size.Height, size.Width);
					}
					else
					{
						return new Rectangle(0, 0, size.Width, size.Height);
					}
				}
			}
	public bool Color
			{
				get
				{
					if(colorSet)
					{
						return color;
					}
					else
					{
						return PrinterSettings.DefaultPageSettings.Color;
					}
				}
				set
				{
					color = value;
					colorSet = true;
				}
			}
	public bool Landscape
			{
				get
				{
					if(landscapeSet)
					{
						return landscape;
					}
					else
					{
						return PrinterSettings.DefaultPageSettings.Landscape;
					}
				}
				set
				{
					landscape = value;
					landscapeSet = true;
				}
			}
	public Margins Margins
			{
				get
				{
					return margins;
				}
				set
				{
					if(value != null)
					{
						margins = value;
					}
					else
					{
						margins = new Margins();
					}
				}
			}
	public PaperSize PaperSize
			{
				get
				{
					if(paperSize != null)
					{
						return paperSize;
					}
					else
					{
						return PrinterSettings.DefaultPageSettings.PaperSize;
					}
				}
				set
				{
					paperSize = value;
				}
			}
	public PaperSource PaperSource
			{
				get
				{
					if(paperSource != null)
					{
						return paperSource;
					}
					else
					{
						return PrinterSettings.DefaultPageSettings.PaperSource;
					}
				}
				set
				{
					paperSource = value;
				}
			}
	public PrinterResolution PrinterResolution
			{
				get
				{
					if(printerResolution != null)
					{
						return printerResolution;
					}
					else
					{
						return PrinterSettings.DefaultPageSettings.
									PrinterResolution;
					}
				}
				set
				{
					printerResolution = value;
				}
			}
	public PrinterSettings PrinterSettings
			{
				get
				{
					return printerSettings;
				}
				set
				{
					if(value != null)
					{
						printerSettings = value;
					}
					else
					{
						printerSettings = new PrinterSettings();
					}
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return MemberwiseClone();
			}

	// Copy the settings to a Win32 HDEVMODE structure.
	public void CopyToHdevmode(IntPtr hdevmode)
			{
				// Not used in this implementation.
			}

	// Set the settings in this object from a Win32 HDEVMODE structure.
	public void SetHdevmode(IntPtr hdevmode)
			{
				// Not used in this implementation.
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[PageSettings: Color=");
				builder.Append(Color.ToString());
				builder.Append(", Landscape=");
				builder.Append(Landscape.ToString());
				builder.Append(", Margins=");
				builder.Append(Margins.ToString());
				builder.Append(", PaperSize=");
				builder.Append(PaperSize.ToString());
				builder.Append(", PaperSource=");
				builder.Append(PaperSource.ToString());
				builder.Append(", PrinterResolution=");
				builder.Append(PrinterResolution.ToString());
				builder.Append(']');
				return builder.ToString();
			}

}; // class PageSettings

}; // namespace System.Drawing.Printing
