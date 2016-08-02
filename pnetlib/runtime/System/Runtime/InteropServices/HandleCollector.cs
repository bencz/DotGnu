/*
 * HandleCollector.cs - Implementation of the
 *			"System.Runtime.InteropServices.HandleCollector" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

using System.Threading;

#if CONFIG_FRAMEWORK_1_2

public sealed class HandleCollector
{
	// Internal state.
	private String name;
	private int initialThreshold;
	private int maximumThreshold;
	private int count;

	// Constructors.
	public HandleCollector(String name, int initialThreshold)
			: this(name, initialThreshold, Int32.MaxValue) {}
	public HandleCollector
				(String name, int initialThreshold, int maximumThreshold)
			{
				if(initialThreshold < 0)
				{
					throw new ArgumentOutOfRangeException
						("initialThreshold", _("ArgRange_NonNegative"));
				}
				if(maximumThreshold < 0)
				{
					throw new ArgumentOutOfRangeException
						("maximumThreshold", _("ArgRange_NonNegative"));
				}
				this.name = name;
				this.initialThreshold = initialThreshold;
				this.maximumThreshold = maximumThreshold;
				this.count = 0;
			}

	// Get this object's properties.
	public int Count
			{
				get
				{
					return count;
				}
			}
	public int InitialThreshold
			{
				get
				{
					return initialThreshold;
				}
			}
	public int MaximumThreshold
			{
				get
				{
					return maximumThreshold;
				}
			}

	// Add one to the count.
	public void Add()
			{
				Interlocked.Increment(ref count);
				GC.Collect();
			}

	// Remove one from the count.
	public void Remove()
			{
				Interlocked.Decrement(ref count);
			}

}; // class HandleCollector

#endif // CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices
