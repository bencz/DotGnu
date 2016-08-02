/*
 * Random.cs - Implementation of the "System.Random" class.
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

namespace System
{

#if CONFIG_EXTENDED_NUMERICS

public class Random
{
	// Current seed value for this instance.
	private int seed;

	// Constructors.
	public Random() : this(Environment.TickCount) {}
	public Random(int seed)
			{
				// Make sure that the seed value is positive.
				this.seed = unchecked(seed & 0x7FFFFFFF);
			}

	// Compute the next random number sample value.  This algorithm
	// is based on "rand()" from the FreeBSD sources.
#if !ECMA_COMPAT
	protected virtual double Sample()
#else
	private double Sample()
#endif
			{
				seed = unchecked((int)((((uint)seed) * 1103515245 + 12345) &
									   (uint)0x7FFFFFFF));
				return (((double)seed) / 2147483648.0);
			}

	// Get the next value in the random sequence.
	public virtual int Next()
			{
				return unchecked((int)(Sample() * 2147483648.0));
			}
	public virtual int Next(int maxValue)
			{
				if(maxValue < 0)
				{
					throw new ArgumentOutOfRangeException
						("maxValue", _("ArgRange_NonNegative"));
				}
				else if(maxValue != 0)
				{
					return unchecked((int)(Sample() * (double)maxValue));
				}
				else
				{
					return 0;
				}
			}
	public virtual int Next(int minValue, int maxValue)
			{
				uint range;
				if(minValue > maxValue)
				{
					throw new ArgumentOutOfRangeException
						("minValue", _("ArgRange_MinValueGtMaxValue"));
				}
				else if(minValue != maxValue)
				{
					// Use an unsigned integer for the range just in
					// case it is greater than 31 bits in size.
					range = unchecked((uint)(maxValue - minValue));
					return unchecked(((int)(Sample() * (double)range)) +
									 minValue);
				}
				else
				{
					return minValue;
				}
			}

	// Fill an array with random bytes.
	public virtual void NextBytes(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				int len = buffer.Length;
				int posn;
				for(posn = 0; posn < len; ++posn)
				{
					buffer[posn] = unchecked((byte)(Sample() * 256.0));
				}
			}

	// Get the next double quantity.
	public virtual double NextDouble()
			{
				return Sample();
			}

}; // class Random

#else // !CONFIG_EXTENDED_NUMERICS

// Use an integer-only implementation if we don't have floating-point.

public class Random
{
	// Current seed value for this instance.
	private int seed;

	// Constructors.
	public Random() : this(Environment.TickCount) {}
	public Random(int seed)
			{
				// Make sure that the seed value is positive.
				this.seed = unchecked(seed & 0x7FFFFFFF);
			}

	// Compute the next random number sample value.  This algorithm
	// is based on "rand()" from the FreeBSD sources.
	private int Sample()
			{
				seed = unchecked((int)((((uint)seed) * 1103515245 + 12345) &
									   (uint)0x7FFFFFFF));
				return seed;
			}

	// Get the next value in the random sequence.
	public virtual int Next()
			{
				return Sample();
			}
	public virtual int Next(int maxValue)
			{
				if(maxValue < 0)
				{
					throw new ArgumentOutOfRangeException
						("maxValue", _("ArgRange_NonNegative"));
				}
				else if(maxValue != 0)
				{
					return (int)(((long)(Sample()) * (long)maxValue) >> 31);
				}
				else
				{
					return 0;
				}
			}
	public virtual int Next(int minValue, int maxValue)
			{
				uint range;
				if(minValue > maxValue)
				{
					throw new ArgumentOutOfRangeException
						("minValue", _("ArgRange_MinValueGtMaxValue"));
				}
				else if(minValue != maxValue)
				{
					// Use an unsigned integer for the range just in
					// case it is greater than 31 bits in size.
					range = unchecked((uint)(maxValue - minValue));
					return (int)(((((ulong)(long)(Sample())) *
									(ulong)range)) >> 31) + minValue;
				}
				else
				{
					return minValue;
				}
			}

	// Fill an array with random bytes.
	public virtual void NextBytes(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				int len = buffer.Length;
				int posn;
				for(posn = 0; posn < len; ++posn)
				{
					buffer[posn] = unchecked((byte)(Sample() >> (31 - 8)));
				}
			}

}; // class Random

#endif // !CONFIG_EXTENDED_NUMERICS

}; // namespace System
