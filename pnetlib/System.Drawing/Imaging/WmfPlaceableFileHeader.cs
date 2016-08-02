/*
 * WmfPlaceableFileHeader.cs - Implementation of the
 *			"System.Drawing.Imaging.WmfPlaceableFileHeader" class.
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

public sealed class WmfPlaceableFileHeader
{
	// Internal state.
	private short bboxBottom;
	private short bboxLeft;
	private short bboxRight;
	private short bboxTop;
	private short checksum;
	private short hmf;
	private short inch;
	private int key;
	private int reserved;

	// Constructor.
	public WmfPlaceableFileHeader()
			{
				key = unchecked((int)0x9AC6CDD7);
			}

	// Get or set this object's properties.
	public short BboxBottom
			{
				get
				{
					return bboxBottom;
				}
				set
				{
					bboxBottom = value;
				}
			}
	public short BboxLeft
			{
				get
				{
					return bboxLeft;
				}
				set
				{
					bboxLeft = value;
				}
			}
	public short BboxRight
			{
				get
				{
					return bboxRight;
				}
				set
				{
					bboxRight = value;
				}
			}
	public short BboxTop
			{
				get
				{
					return bboxTop;
				}
				set
				{
					bboxTop = value;
				}
			}
	public short Checksum
			{
				get
				{
					return checksum;
				}
				set
				{
					checksum = value;
				}
			}
	public short Hmf
			{
				get
				{
					return hmf;
				}
				set
				{
					hmf = value;
				}
			}
	public short Inch
			{
				get
				{
					return inch;
				}
				set
				{
					inch = value;
				}
			}
	public int Key
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}
	public int Reserved
			{
				get
				{
					return reserved;
				}
				set
				{
					reserved = value;
				}
			}

}; // class WmfPlaceableFileHeader

}; // namespace System.Drawing.Imaging
