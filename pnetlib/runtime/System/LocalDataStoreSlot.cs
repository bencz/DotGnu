/*
 * LocalDataStoreSlot.cs - Implementation of the
 *			"System.LocalDataStoreSlot" class.
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

namespace System
{

#if !ECMA_COMPAT

public sealed class LocalDataStoreSlot
{
	// Internal state.
	private LocalDataStoreSlot nextNamed;
	private String name;
	private int slotNum;
	private bool free;
	private bool isNamed;
	private static int nextSlotNum;
	private static LocalDataStoreSlot namedSlots;
	[ThreadStatic] private static Object[] data;

	// Constructor.
	internal LocalDataStoreSlot(bool isNamed, String name)
			{
				this.isNamed = isNamed;
				this.name = name;
				lock(typeof(LocalDataStoreSlot))
				{
					slotNum = nextSlotNum++;
				}
			}

	// Destructor.
	~LocalDataStoreSlot()
			{
				// Not used in this implementation.
			}

	// Get a data store slot with a specific name.
	internal static LocalDataStoreSlot GetNamed(String name)
			{
				lock(typeof(LocalDataStoreSlot))
				{
					LocalDataStoreSlot slot = namedSlots;
					while(slot != null)
					{
						if(slot.isNamed && slot.name == name)
						{
							return slot;
						}
						slot = slot.nextNamed;
					}
					slot = new LocalDataStoreSlot(true, name);
					slot.nextNamed = namedSlots;
					namedSlots = slot;
					return slot;
				}
			}

	// Free a data store slot with a specific name.
	internal static void FreeNamed(String name)
			{
				lock(typeof(LocalDataStoreSlot))
				{
					LocalDataStoreSlot slot = namedSlots;
					LocalDataStoreSlot prev = null;
					while(slot != null)
					{
						if(slot.isNamed && slot.name == name)
						{
							if(prev != null)
							{
								prev.nextNamed = slot.nextNamed;
							}
							else
							{
								namedSlots = slot.nextNamed;
							}
							slot.free = true;
							return;
						}
						prev = slot;
						slot = slot.nextNamed;
					}
				}
			}

	// Get or set the data in this slot for the current thread.
	internal Object Data
			{
				get
				{
					if(free)
					{
						return null;
					}
					Object[] tempData = data;
					if(tempData != null && slotNum < tempData.Length)
					{
						return tempData[slotNum];
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(free)
					{
						return;
					}
					Object[] tempData = data;
					if(tempData != null && slotNum < tempData.Length)
					{
						tempData[slotNum] = value;
					}
					else
					{
						Object[] tempData1 = new Object [(slotNum + 8) & ~7];
						if(data != null)
						{
							Array.Copy(data, 0, tempData1, 0, data.Length);
						}
						data = tempData1;
						tempData1[slotNum] = value;
					}
				}
			}

}; // class LocalDataStoreSlot

#endif // !ECMA_COMPAT

}; // namespace System
