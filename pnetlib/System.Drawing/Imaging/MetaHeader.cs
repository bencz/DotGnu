/*
 * MetaHeader.cs - Implementation of the
 *			"System.Drawing.Imaging.MetaHeader" class.
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

public sealed class MetaHeader
{
	// Internal state.
	private short headerSize;
	private short noObjects;
	private short noParameters;
	private short type;
	private short version;
	private int maxRecord;
	private int size;

	// Constructor.
	public MetaHeader() {}

	// Get or set this object's properties.
	public short HeaderSize
			{
				get
				{
					return headerSize;
				}
				set
				{
					headerSize = value;
				}
			}
	public int MaxRecord
			{
				get
				{
					return maxRecord;
				}
				set
				{
					maxRecord = value;
				}
			}
	public short NoObjects
			{
				get
				{
					return noObjects;
				}
				set
				{
					noObjects = value;
				}
			}
	public short NoParameters
			{
				get
				{
					return noParameters;
				}
				set
				{
					noParameters = value;
				}
			}
	public int Size
			{
				get
				{
					return size;
				}
				set
				{
					size = value;
				}
			}
	public short Type
			{
				get
				{
					return type;
				}
				set
				{
					type = value;
				}
			}
	public short Version
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

}; // class MetaHeader

}; // namespace System.Drawing.Imaging
