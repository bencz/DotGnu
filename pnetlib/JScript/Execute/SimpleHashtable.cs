/*
 * SimpleHashtable.cs - Simple hash table class.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Collections;

// In this implementation, we just wrap up a "Hashtable".  That's simple.

public sealed class SimpleHashtable
{
	// Internal state.
	private Hashtable table;

	// Constructor.
	public SimpleHashtable(uint threshold)
			{
				table = new Hashtable((int)threshold);
			}

	// Get an enumerator for the hash table.
	public IDictionaryEnumerator GetEnumerator()
			{
				return table.GetEnumerator();
			}

	// Remove an entry from the hash table.
	public void Remove(Object key)
			{
				table.Remove(key);
			}

	// Indexer for the hash table.
	public Object this[Object key]
			{
				get
				{
					return table[key];
				}
				set
				{
					table[key] = value;
				}
			}

}; // class SimpleHashtable

}; // namespace Microsoft.JScript
