/*
 * VBMath.cs - Implementation of the
 *			"Microsoft.VisualBasic.VBMath" class.
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

namespace Microsoft.VisualBasic
{

using System;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class VBMath
{
	// Internal state.
	private static int seed = (Environment.TickCount & 0x7FFFFFFF);

	// This class cannot be instantiated.
	private VBMath() {}

	// Get the next value in sequence.
	private static float Next(int prevSeed, bool update)
			{
				int value;
				value = unchecked((int)((((uint)prevSeed)
											* 1103515245 + 12345) &
									    (uint)0x7FFFFFFF));
				if(update)
				{
					seed = value;
				}
				return (float)((((double)value) / 2147483648.0));
			}

	// Get the previous value in the sequence.
	private static float Prev()
			{
				return (float)((((double)seed) / 2147483648.0));
			}

	// Initialize the random number generator.
	public static void Randomize()
			{
				lock(typeof(VBMath))
				{
					seed = (Environment.TickCount & 0x7FFFFFFF);
				}
			}
	public static void Randomize(double number)
			{
				lock(typeof(VBMath))
				{
					long value = BitConverter.DoubleToInt64Bits(number);
					value ^= value >> 32;
					seed = unchecked(((int)value) & 0x7FFFFFFF);
				}
			}

	// Generate a random number.
	public static float Rnd()
			{
				lock(typeof(VBMath))
				{
					return Next(seed, true);
				}
			}
	public static float Rnd(float number)
			{
				lock(typeof(VBMath))
				{
					if(number == 0.0f)
					{
						return Prev();
					}
					else if(number > 0.0f)
					{
						return Next(seed, true);
					}
					else
					{
						long value = BitConverter.DoubleToInt64Bits(number);
						value ^= value >> 32;
						int seed = unchecked(((int)value) & 0x7FFFFFFF);
						return Next(seed, false);
					}
				}
			}

}; // class VBMath

}; // namespace Microsoft.VisualBasic
