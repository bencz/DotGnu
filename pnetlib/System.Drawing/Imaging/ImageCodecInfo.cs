/*
 * ImageCodecInfo.cs - Implementation of the
 *			"System.Drawing.Imaging.ImageCodecInfo" class.
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

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class ImageCodecInfo
{
	// Internal state.
#if !ECMA_COMPAT
	private Guid clsid;
	private Guid formatId;
#endif
	private String codecName;
	private String dllName;
	private String filenameExtension;
	private ImageCodecFlags flags;
	private String formatDescription;
	private String mimeType;
	private byte[][] signatureMasks;
	private byte[][] signaturePatterns;
	private int version;

	// Constructor.
	internal ImageCodecInfo() {}

	// Get or set this object's properties.
#if !ECMA_COMPAT
	public Guid Clsid
			{
				get
				{
					return clsid;
				}
				set
				{
					clsid = value;
				}
			}
#endif
	public String CodecName
			{
				get
				{
					return codecName;
				}
				set
				{
					codecName = value;
				}
			}
	public String DllName
			{
				get
				{
					return dllName;
				}
				set
				{
					dllName = value;
				}
			}
	public String FilenameExtension
			{
				get
				{
					return filenameExtension;
				}
				set
				{
					filenameExtension = value;
				}
			}
	public ImageCodecFlags Flags
			{
				get
				{
					return flags;
				}
				set
				{
					flags = value;
				}
			}
	public String FormatDescription
			{
				get
				{
					return formatDescription;
				}
				set
				{
					formatDescription = value;
				}
			}
#if !ECMA_COMPAT
	public Guid FormatID
			{
				get
				{
					return formatId;
				}
				set
				{
					formatId = value;
				}
			}
#endif
	public String MimeType
			{
				get
				{
					return mimeType;
				}
				set
				{
					mimeType = value;
				}
			}
	[CLSCompliant(false)]
	public byte[][] SignatureMasks
			{
				get
				{
					return signatureMasks;
				}
				set
				{
					signatureMasks = value;
				}
			}
	[CLSCompliant(false)]
	public byte[][] SignaturePatterns
			{
				get
				{
					return signaturePatterns;
				}
				set
				{
					signaturePatterns = value;
				}
			}
	public int Version
			{
				get
				{
					return version;
				}
				set
				{
					version = value;
				}
			}

	// Find all image decoders.
	[TODO]
	public static ImageCodecInfo[] GetImageDecoders()
			{
				// TODO
				return null;
			}

	// Find all image encoders.
	[TODO]
	public static ImageCodecInfo[] GetImageEncoders()
			{
				// TODO
				return null;
			}

}; // class ImageCodecInfo

}; // namespace System.Drawing.Imaging
