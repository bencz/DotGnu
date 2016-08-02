/*
 * EventHandlerList.cs - Implementation of the
 *		"System.ComponentModule.EventHandlerList" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;

public sealed class EventHandlerList : IDisposable
{
	// Internal storage for the list.
	private EventHandlerEntry list;

	// Constructor.
	public EventHandlerList()
			{
				list = null;
			}

	// Get or set a delegate on this list, by key.
	public Delegate this[Object key]
			{
				get
				{
					EventHandlerEntry entry = list;
					while(entry != null)
					{
						if(entry.key == key)
						{
							return entry.value;
						}
						entry = entry.next;
					}
					return null;
				}
				set
				{
					EventHandlerEntry entry = list;
					while(entry != null)
					{
						if(entry.key == key)
						{
							entry.value = value;
							return;
						}
						entry = entry.next;
					}
					list = new EventHandlerEntry(key, value, list);
				}
			}

	// Add a handler to this list.
	public void AddHandler(Object key, Delegate value)
			{
				EventHandlerEntry entry = list;
				while(entry != null)
				{
					if(entry.key == key)
					{
						entry.value = Delegate.Combine(entry.value, value);
						return;
					}
					entry = entry.next;
				}
				list = new EventHandlerEntry(key, value, list);
			}

	// Remove a handler from this list.
	public void RemoveHandler(Object key, Delegate value)
			{
				EventHandlerEntry entry = list;
				EventHandlerEntry prev = null;
				while(entry != null)
				{
					if(entry.key == key)
					{
						entry.value = Delegate.Remove(entry.value, value);
						if(entry.value == null)
						{
							if(prev != null)
							{
								prev.next = entry.next;
							}
							else
							{
								list = entry.next;
							}
						}
						return;
					}
					prev = entry;
					entry = entry.next;
				}
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				list = null;
			}

	// Information that is stored for an event handler entry.
	private class EventHandlerEntry
	{
		// Internal state.
		public Object key;
		public Delegate value;
		public EventHandlerEntry next;

		// Constructor.
		public EventHandlerEntry(Object key, Delegate value,
								 EventHandlerEntry next)
				{
					this.key = key;
					this.value = value;
					this.next = next;
				}

	}; // class EventHandlerEntry

}; // class EventHandlerList

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
