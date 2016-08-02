/*
 * MetafileHeader.cs - Implementation of the
 *			"System.Drawing.Imaging.MetafileHeader" class.
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

public sealed class MetafileHeader
{
	// Internal state.

	// Constructor.
	internal MetafileHeader() {}

	// Get this object's properties.
	[TODO]
	public Rectangle Bounds
			{
				get
				{
					// TODO
					return Rectangle.Empty;
				}
			}
	[TODO]
	public float DpiX
			{
				get
				{
					// TODO
					return 0.0f;
				}
			}
	[TODO]
	public float DpiY
			{
				get
				{
					// TODO
					return 0.0f;
				}
			}
	[TODO]
	public int EmfPlusHeaderSize
			{
				get
				{
					// TODO
					return 0;
				}
			}
	[TODO]
	public int LogicalDpiX
			{
				get
				{
					// TODO
					return 0;
				}
			}
	[TODO]
	public int LogicalDpiY
			{
				get
				{
					// TODO
					return 0;
				}
			}
	[TODO]
	public int MetafileSize
			{
				get
				{
					// TODO
					return 0;
				}
			}
	[TODO]
	public MetafileType Type
			{
				get
				{
					// TODO
					return MetafileType.Invalid;
				}
			}
	[TODO]
	public int Version
			{
				get
				{
					// TODO
					return 0;
				}
			}
	[TODO]
	public MetaHeader WmfHeader
			{
				get
				{
					// TODO
					return null;
				}
			}

	// Determine if the metafile is device dependent.
	[TODO]
	public bool IsDisplay()
			{
				// TODO
				return false;
			}

	// Determine if this is an "Emf" metafile.
	public bool IsEmf()
			{
				return (Type == MetafileType.Emf);
			}

	// Determine if this is an "Emf" or "EmfPlus" metafile.
	public bool IsEmfOrEmfPlus()
			{
				MetafileType type = Type;
				return (type == MetafileType.Emf ||
						type == MetafileType.EmfPlusOnly ||
						type == MetafileType.EmfPlusDual);
			}

	// Determine if this is an "EmfPlus" metafile.
	public bool IsEmfPlus()
			{
				MetafileType type = Type;
				return (type == MetafileType.EmfPlusOnly ||
						type == MetafileType.EmfPlusDual);
			}

	// Determine if this is an "EmfPlusDual" metafile.
	public bool IsEmfPlusDual()
			{
				return (Type == MetafileType.EmfPlusDual);
			}

	// Determine if this is an "EmfPlusOnly" metafile.
	public bool IsEmfPlusOnly()
			{
				return (Type == MetafileType.EmfPlusOnly);
			}

	// Determine if this is a "Wmf" metafile.
	public bool IsWmf()
			{
				MetafileType type = Type;
				return (type == MetafileType.Wmf ||
						type == MetafileType.WmfPlaceable);
			}

	// Determine if this is a "WmfPlaceable" metafile.
	public bool IsWmfPlaceable()
			{
				return (Type == MetafileType.WmfPlaceable);
			}

}; // class MetafileHeader

}; // namespace System.Drawing.Imaging
