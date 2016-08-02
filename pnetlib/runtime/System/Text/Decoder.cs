/*
 * Decoder.cs - Implementation of the "System.Text.Decoder" class.
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
public abstract class Decoder
{

	// Constructor.
	protected Decoder() {}

	// Get the number of characters needed to decode a buffer.
	public abstract int GetCharCount(byte[] bytes, int index, int count);

	// Get the characters that result from decoding a buffer.
	public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount,
								 char[] chars, int charIndex);

}; // class Decoder

}; // namespace System.Text
