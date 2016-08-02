/*
 * LongJmp.cs - Support code for setjmp/longjmp operations.
 *
 * This file is part of the Portable.NET "C language support" library.
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;

// An exception class that is used to implement "longjmp".
public sealed class LongJmpException : Exception
{
	// Internal state.
	private int marker;
	private int value;
	private static int nextMarker = 0;

	// Constructor.
	public LongJmpException(int marker, int value)
			: base()
			{
				this.marker = marker;
				this.value = value;
			}

	// Get the marker value from this exception object.
	public int Marker
			{
				get
				{
					return marker;
				}
			}

	// Get the longjmp value from this exception object.
	public int Value
			{
				get
				{
					return value;
				}
			}

	// Get a range of unique marker values for the calling method frame.
	public static int GetMarkers(int range)
			{
				lock(typeof(LongJmpException))
				{
					int marker = nextMarker;
					nextMarker += range;
					return marker;
				}
			}

} // class LongJmpException

} // namespace OpenSystem.C
