/*
 * Metafile.cs - Implementation of the
 *			"System.Drawing.Imaging.Metafile" class.
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

using System.IO;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[Serializable]
[ComVisible(false)]
#endif
public sealed class Metafile : Image
{
	// Internal state.

	// Main constructor variants.
	[TODO]
	public Metafile(IntPtr henhmetafile, bool deleteEmf)
			{
				// TODO
			}
	[TODO]
	public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader,
					bool deleteWmf)
			{
				// TODO
			}
	[TODO]
	public Metafile(IntPtr referenceHdc, EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(IntPtr referenceHdc, Rectangle frameRect,
					MetafileFrameUnit frameUnit, EmfType type,
					String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(IntPtr referenceHdc, RectangleF frameRect,
					MetafileFrameUnit frameUnit, EmfType type,
					String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(Stream stream, IntPtr referenceHdc,
					EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(Stream stream, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit,
					EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(Stream stream, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit,
					EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(String fileName, IntPtr referenceHdc,
					EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(String fileName, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit,
					EmfType type, String description)
			{
				// TODO
			}
	[TODO]
	public Metafile(String fileName, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit,
					EmfType type, String description)
			{
				// TODO
			}

	// Convenience wrappers for the main constructors.
	public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
			: this(hmetafile, wmfHeader, false) {}
	public Metafile(IntPtr referenceHdc, EmfType emfType)
			: this(referenceHdc, emfType, null) {}
	public Metafile(IntPtr referenceHdc, Rectangle frameRect)
			: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible,
			       EmfType.EmfPlusDual, null) {}
	public Metafile(IntPtr referenceHdc, RectangleF frameRect)
			: this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible,
			       EmfType.EmfPlusDual, null) {}
	public Metafile(IntPtr referenceHdc, Rectangle frameRect,
					MetafileFrameUnit frameUnit)
			: this(referenceHdc, frameRect, frameUnit,
			       EmfType.EmfPlusDual, null) {}
	public Metafile(IntPtr referenceHdc, RectangleF frameRect,
					MetafileFrameUnit frameUnit)
			: this(referenceHdc, frameRect, frameUnit,
			       EmfType.EmfPlusDual, null) {}
	public Metafile(IntPtr referenceHdc, Rectangle frameRect,
					MetafileFrameUnit frameUnit, EmfType type)
			: this(referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(IntPtr referenceHdc, RectangleF frameRect,
					MetafileFrameUnit frameUnit, EmfType type)
			: this(referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(Stream stream)
			: this(stream, IntPtr.Zero, EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc)
			: this(stream, referenceHdc, EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc, EmfType type)
			: this(stream, referenceHdc, type, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect)
			: this(stream, referenceHdc, frameRect,
				   MetafileFrameUnit.GdiCompatible,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect)
			: this(stream, referenceHdc, frameRect,
				   MetafileFrameUnit.GdiCompatible,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit)
			: this(stream, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit)
			: this(stream, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit,
					EmfType type)
			: this(stream, referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(Stream stream, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit,
					EmfType type)
			: this(stream, referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(String fileName)
			: this(fileName, IntPtr.Zero, EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc)
			: this(fileName, referenceHdc, EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc, EmfType type)
			: this(fileName, referenceHdc, type, null) {}
	public Metafile(String fileName, IntPtr referenceHdc, Rectangle frameRect)
			: this(fileName, referenceHdc, frameRect,
				   MetafileFrameUnit.GdiCompatible,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc, RectangleF frameRect)
			: this(fileName, referenceHdc, frameRect,
				   MetafileFrameUnit.GdiCompatible,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit)
			: this(fileName, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit)
			: this(fileName, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, null) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit,
					EmfType type)
			: this(fileName, referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit,
					EmfType type)
			: this(fileName, referenceHdc, frameRect, frameUnit, type, null) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					Rectangle frameRect, MetafileFrameUnit frameUnit,
					String description)
			: this(fileName, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, description) {}
	public Metafile(String fileName, IntPtr referenceHdc,
					RectangleF frameRect, MetafileFrameUnit frameUnit,
					String description)
			: this(fileName, referenceHdc, frameRect, frameUnit,
				   EmfType.EmfPlusDual, description) {}

	// Detach this Metafile object.  Used by "GetMetafileHeader" wrappers.
	[TODO]
	private void Detach()
			{
				// TODO
			}

	// Get the Windows enhanced metafile pointer.
	[TODO]
	public IntPtr GetHenhmetafile()
			{
				// TODO
				return IntPtr.Zero;
			}

	// Get metafile header information.
	[TODO]
	public MetafileHeader GetMetafileHeader()
			{
				// TODO
				return null;
			}

	// Convenience wrappers for "GetMetafileHeader".
	public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
			{
				Metafile file = new Metafile(henhmetafile, false);
				MetafileHeader header = file.GetMetafileHeader();
				file.Detach();
				return header;
			}
	public static MetafileHeader GetMetafileHeader(Stream stream)
			{
				Metafile file = new Metafile(stream);
				MetafileHeader header = file.GetMetafileHeader();
				file.Detach();
				return header;
			}
	public static MetafileHeader GetMetafileHeader(String fileName)
			{
				Metafile file = new Metafile(fileName);
				MetafileHeader header = file.GetMetafileHeader();
				file.Detach();
				return header;
			}
	public static MetafileHeader GetMetafileHeader
				(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
			{
				Metafile file = new Metafile(hmetafile, wmfHeader);
				MetafileHeader header = file.GetMetafileHeader();
				file.Detach();
				return header;
			}

	// Play a metafile record.
	[TODO]
	public void PlayRecord(EmfPlusRecordType recordType,
						   int flags, int dataSize, byte[] data)
			{
				// TODO
			}

}; // class Metafile

}; // namespace System.Drawing.Imaging
