/*
 * KeySizes.cs - Implementation of the
 *		"System.Security.Cryptography.KeySizes" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO

using System;

public sealed class KeySizes
{
	// Internal state.
	private int minSize, maxSize, skipSize;

	// Constructor.
	public KeySizes(int minSize, int maxSize, int skipSize)
			{
				this.minSize = minSize;
				this.maxSize = maxSize;
				this.skipSize = skipSize;
			}

	// Access the members.
	public int MinSize  { get { return minSize; } }
	public int MaxSize  { get { return maxSize; } }
	public int SkipSize { get { return skipSize; } }

	// Validate a key size value.
	internal static bool Validate(KeySizes[] sizes, int value)
			{
				if(sizes == null)
				{
					return false;
				}
				foreach(KeySizes size in sizes)
				{
					if(value >= size.MinSize && value <= size.MaxSize &&
					   ((value - size.MinSize) % size.SkipSize) == 0)
					{
						return true;
					}
				}
				return false;
			}

	// Validate a key size and throw an exception if invalid.
	internal static void Validate(KeySizes[] sizes, int value, String resource)
			{
				if(!Validate(sizes, value))
				{
					throw new CryptographicException
						(_(resource), value.ToString());
				}
			}

}; // class KeySizes

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
