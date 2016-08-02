/*
 * Encoder.cs - Implementation of the "System.Text.Encoder" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Text
{

using System;

[Serializable]
public abstract class Encoder
{

	// Constructor.
	protected Encoder() {}

	// Get the number of bytes needed to encode a buffer.
	public abstract int GetByteCount(char[] chars, int index,
									 int count, bool flush);

	// Get the bytes that result from decoding a buffer.
	public abstract int GetBytes(char[] chars, int charIndex, int charCount,
								 byte[] bytes, int byteIndex, bool flush);

}; // class Encoder

}; // namespace System.Text
