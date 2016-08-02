/*
 * ImageFormat.cs - Implementation of the
 *			"System.Drawing.Imaging.ImageFormat" class.
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

namespace System.Drawing.Imaging
{

#if !ECMA_COMPAT

public sealed class ImageFormat
{
	// Internal state.
	private Guid guid;
	private static readonly ImageFormat bmp =
			new ImageFormat
				(new Guid("{b96b3cab-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat emf =
			new ImageFormat
				(new Guid("{b96b3cac-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat exif =
			new ImageFormat
				(new Guid("{b96b3cb2-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat gif =
			new ImageFormat
				(new Guid("{b96b3cb0-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat icon =
			new ImageFormat
				(new Guid("{b96b3cb5-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat jpeg =
			new ImageFormat
				(new Guid("{b96b3cae-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat memoryBmp =
			new ImageFormat
				(new Guid("{b96b3caa-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat png =
			new ImageFormat
				(new Guid("{b96b3caf-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat tiff =
			new ImageFormat
				(new Guid("{b96b3cb1-0728-11d3-9d7b-0000f81ef32e}"));
	private static readonly ImageFormat wmf =
			new ImageFormat
				(new Guid("{b96b3cad-0728-11d3-9d7b-0000f81ef32e}"));

	// Constructor.
	public ImageFormat(Guid guid)
			{
				this.guid = guid;
			}

	// Get the GUID for this image format.
	public Guid Guid
			{
				get
				{
					return guid;
				}
			}

	// Standard image formats.
	public static ImageFormat Bmp
			{
				get
				{
					return bmp;
				}
			}
	public static ImageFormat Emf
			{
				get
				{
					return emf;
				}
			}
	public static ImageFormat Exif
			{
				get
				{
					return exif;
				}
			}
	public static ImageFormat Gif
			{
				get
				{
					return gif;
				}
			}
	public static ImageFormat Icon
			{
				get
				{
					return icon;
				}
			}
	public static ImageFormat Jpeg
			{
				get
				{
					return jpeg;
				}
			}
	public static ImageFormat MemoryBmp
			{
				get
				{
					return memoryBmp;
				}
			}
	public static ImageFormat Png
			{
				get
				{
					return png;
				}
			}
	public static ImageFormat Tiff
			{
				get
				{
					return tiff;
				}
			}
	public static ImageFormat Wmf
			{
				get
				{
					return wmf;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				ImageFormat other = (obj as ImageFormat);
				if(other != null)
				{
					return (other.guid.Equals(guid));
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return guid.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(this == bmp)
				{
					return "Bmp";
				}
				else if(this == emf)
				{
					return "Emf";
				}
				else if(this == exif)
				{
					return "Exif";
				}
				else if(this == gif)
				{
					return "Gif";
				}
				else if(this == icon)
				{
					return "Icon";
				}
				else if(this == jpeg)
				{
					return "Jpeg";
				}
				else if(this == memoryBmp)
				{
					return "MemoryBMP";
				}
				else if(this == png)
				{
					return "Png";
				}
				else if(this == tiff)
				{
					return "Tiff";
				}
				else if(this == wmf)
				{
					return "Wmf";
				}
				else
				{
					return "[ImageFormat: " + guid.ToString() + "]";
				}
			}

}; // class ImageFormat

#endif // !ECMA_COMPAT

}; // namespace System.Drawing.Imaging
