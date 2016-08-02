/*
 * HandleMap.cs - Quick hash table for mapping window handles.
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

namespace Xsharp
{

using System;
using OpenSystem.Platform.X11;

// This class is a lot more efficient than "Hashtable" when it comes
// to mapping window handles to widget objects.

internal class HandleMap
{
	// Internal state.
	private HandleInfo[] handles;

	// Size of the hash table: must be a power of two.  Handles are allocated
	// sequentially by Xlib, so there is little point in using a prime number
	// for the hash table size, and powers of two are more efficient.
	private const int HashSize = 1024;

	// Information about a handle in the main part of the table.
	private struct HandleInfo
	{
		public XWindow window;
		[NonSerializedAttribute]
		public Widget widget;
		[NonSerializedAttribute]
		public HandleOverflowInfo overflow;

	}; // struct HandleInfo

	// Information about a handle in the overflow part of the table.
	private class HandleOverflowInfo
	{
		public XWindow window;
		public Widget widget;
		public HandleOverflowInfo overflow;

	}; // class HandleOverflowInfo

	// Constructor.
	public HandleMap()
			{
				handles = new HandleInfo [HashSize];
			}

	// Get or set a member within this handle map.
	public Widget this[XWindow window]
			{
				get
				{
					// Look in the main part of the table.
					int hash = (((int)window) & (HashSize - 1));
					if(handles[hash].window == window)
					{
						return handles[hash].widget;
					}

					// Look in the overflow part of the table.
					HandleOverflowInfo info = handles[hash].overflow;
					while(info != null)
					{
						if(info.window == window)
						{
							return info.widget;
						}
						info = info.overflow;
					}

					// There is no widget registered with this handle.
					return null;
				}
				set
				{
					// Look in the main part of the table.
					int hash = (((int)window) & (HashSize - 1));
					if(handles[hash].window == window)
					{
						handles[hash].widget = value;
						return;
					}

					// Look in the overflow part of the table.
					HandleOverflowInfo info = handles[hash].overflow;
					while(info != null)
					{
						if(info.window == window)
						{
							info.widget = value;
							return;
						}
						info = info.overflow;
					}

					// Add to the main part of the table if it is empty.
					if(handles[hash].window == XWindow.Zero)
					{
						handles[hash].window = window;
						handles[hash].widget = value;
						return;
					}

					// Add an overflow entry to the hash table.
					info = new HandleOverflowInfo();
					info.window = window;
					info.widget = value;
					info.overflow = handles[hash].overflow;
					handles[hash].overflow = info;
				}
			}

	// Remove an element from this handle map.
	public void Remove(XWindow window)
			{
				// Look in the main part of the table.
				int hash = (((int)window) & (HashSize - 1));
				if(handles[hash].window == window)
				{
					handles[hash].window = XWindow.Zero;
					handles[hash].widget = null;
					return;
				}

				// Look in the overflow part of the table.
				HandleOverflowInfo info = handles[hash].overflow;
				while(info != null)
				{
					if(info.window == window)
					{
						info.window = XWindow.Zero;
						info.widget = null;
						return;
					}
					info = info.overflow;
				}
			}

} // class HandleMap

} // namespace Xsharp
