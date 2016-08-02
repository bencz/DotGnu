/*
 * StringDictionary.cs - Implementation of
 *		"System.Collections.Specialized.StringDictionary".
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Specialized
{

#if !ECMA_COMPAT

using System;
using System.Collections;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

#if CONFIG_COMPONENT_MODEL_DESIGN
[DesignerSerializer("System.Diagnostics.Design.StringDictionaryCodeDomSerializer, System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
#endif
public class StringDictionary : IEnumerable
{
	// Internal state.
	private Hashtable hash;

	// Constructor.
	public StringDictionary()
			{
				hash = new Hashtable();
			}

	// Get the number of key-value pairs in the dictionary.
	public virtual int Count
			{
				get
				{
					return hash.Count;
				}
			}

	// Determine if this string dictionary is synchronized.
	public virtual bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

	// Get or set an item in this string dictionary.
	public virtual String this[String key]
			{
				get
				{
					return (String)(hash[key.ToLower(CultureInfo.InvariantCulture)]);
				}
				set
				{
					hash[key.ToLower(CultureInfo.InvariantCulture)] = value;
				}
			}

	// Get the list of all keys in this string dictionary.
	public virtual ICollection Keys
			{
				get
				{
					return hash.Keys;
				}
			}

	// Get the synchronization root for this string dictionary.
	public virtual Object SyncRoot
			{
				get
				{
					return hash;
				}
			}

	// Get the list of all values in this string dictionary.
	public virtual ICollection Values
			{
				get
				{
					return hash.Values;
				}
			}

	// Add an entry to this string dictionary.
	public virtual void Add(String key, String value)
			{
				hash.Add(key.ToLower(CultureInfo.InvariantCulture), value);
			}

	// Remove all entries from the string dictionary.
	public virtual void Clear()
			{
				hash.Clear();
			}

	// Determine if this string dictionary contains a specific key.
	public virtual bool ContainsKey(String key)
			{
				return hash.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
			}

	// Determine if this string dictionary contains a specific value.
	public virtual bool ContainsValue(String value)
			{
				return hash.ContainsValue(value);
			}

	// Copy all of the members in this string dictionary to an array.
	public virtual void CopyTo(Array array, int index)
			{
				hash.CopyTo(array, index);
			}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
			{
				return hash.GetEnumerator();
			}

	// Remove a member with a specific key from this string dictionary.
	public virtual void Remove(String key)
			{
				hash.Remove(key.ToLower(CultureInfo.InvariantCulture));
			}

}; // class StringDictionary

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
