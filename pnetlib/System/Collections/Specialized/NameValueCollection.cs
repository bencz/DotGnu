/*
 * NameValueCollection.cs - Implementation of
 *		"System.Collections.Specialized.NameValueCollection".
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

namespace System.Collections.Specialized
{

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

public class NameValueCollection : NameObjectCollectionBase
{
	// Internal state.
	private String[] allKeysResult;
	private String[] copyToResult;

	// Constructors.
	public NameValueCollection()
			: base(0, null, null)
			{
				// Nothing to do here.
			}
	public NameValueCollection(NameValueCollection col)
			: base(0, null, null) 
			{
				Add(col);
			}
	public NameValueCollection(IHashCodeProvider hashProvider,
							   IComparer comparer)
			: base(0, hashProvider, comparer)
			{
				// Nothing to do here.
			}
	public NameValueCollection(int capacity)
			: base(capacity, null, null)
			{
				// Nothing to do here.
			}
	public NameValueCollection(int capacity, NameValueCollection col)
			: base(capacity, null, null)
			{
				Add(col);
			}
	public NameValueCollection(int capacity, IHashCodeProvider hashProvider,
							   IComparer comparer)
			: base(capacity, hashProvider, comparer)
			{
				// Nothing to do here.
			}
#if CONFIG_SERIALIZATION
	protected NameValueCollection(SerializationInfo info,
								  StreamingContext context)
			: base(info, context) {}
#endif

	// Add a name/value pair to this collection.
	public virtual void Add(String name, String value)
			{
				if(IsReadOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				InvalidateCachedArrays();
				ArrayList strings = (ArrayList)(BaseGet(name));
				if(strings == null)
				{
					strings = new ArrayList(1);
					if(value != null)
					{
						strings.Add(value);
					}
					BaseAdd(name, strings);
				}
				else if(value != null)
				{
					strings.Add(value);
				}
			}

	// Add the contents of another name/value collection to this collection.
	public void Add(NameValueCollection c)
			{
				if(c == null)
				{
					throw new ArgumentNullException("c");
				}
				int count = c.Count;
				int posn;
				String name;
				ArrayList strings;
				for(posn = 0; posn < count; ++posn)
				{
					name = c.BaseGetKey(posn);
					strings = (ArrayList)(c.BaseGet(posn));
					foreach(String value in strings)
					{
						Add(name, value);
					}
				}
			}

	// Copy the strings in this collection to an array.
	public void CopyTo(Array array, int index)
			{
				if(copyToResult == null)
				{
					int count = Count;
					int posn;
					copyToResult = new String [count];
					for(posn = 0; posn < count; ++posn)
					{
						copyToResult[posn] = Get(posn);
					}
				}
				copyToResult.CopyTo(array, index);
			}

	// Clear the contents of this collection.
	public void Clear()
			{
				if(!IsReadOnly)
				{
					InvalidateCachedArrays();
				}
				BaseClear();
			}

	// Get a key at a particular index within this collection.
	public virtual String GetKey(int index)
			{
				return BaseGetKey(index);
			}

	// Collapse an array list of strings into a comma-separated value.
	private static String CollapseToString(ArrayList strings)
			{
				if(strings == null)
				{
					return null;
				}
				int count = strings.Count;
				if(count == 0)
				{
					return null;
				}
				else if(count == 1)
				{
					return (String)(strings[0]);
				}
				else
				{
					StringBuilder builder = new StringBuilder();
					builder.Append((String)(strings[0]));
					int posn;
					for(posn = 1; posn < count; ++posn)
					{
						builder.Append(',');
						builder.Append((String)(strings[posn]));
					}
					return builder.ToString();
				}
			}

	// Collapse an array list of strings into an array.
	private static String[] CollapseToArray(ArrayList strings)
			{
				if (strings == null)
					return null;
				String[] result = new String [strings.Count];
				strings.CopyTo(result, 0);
				return result;
			}

	// Get a value at a particular index within this collection.
	public virtual String Get(int index)
			{
				return CollapseToString((ArrayList)(BaseGet(index)));
			}

	// Get an array of values at a particular index within this collection.
	public virtual String[] GetValues(int index)
			{
				return CollapseToArray((ArrayList)(BaseGet(index)));
			}

	// Get the value associcated with a particular name.
	public virtual String Get(String name)
			{
				return CollapseToString((ArrayList)(BaseGet(name)));
			}

	// Get the array of values associcated with a particular name.
	public virtual String[] GetValues(String name)
			{
				return CollapseToArray((ArrayList)(BaseGet(name)));
			}

	// Determine if the collection has keys that are not null.
	public bool HasKeys()
			{
				return BaseHasKeys();
			}

	// Invalidate cached arrays within this collection.
	protected void InvalidateCachedArrays()
			{
				allKeysResult = null;
				copyToResult = null;
			}

	// Remove an entry with a specified name from this collection.
	public virtual void Remove(String name)
			{
				if(!IsReadOnly)
				{
					InvalidateCachedArrays();
				}
				BaseRemove(name);
			}

	// Set the value associated with a specified name in this collection.
	public virtual void Set(String name, String value)
			{
				if(!IsReadOnly)
				{
					InvalidateCachedArrays();
				}
				ArrayList strings = new ArrayList(1);
				if(value != null)
				{
					strings.Add(value);
				}
				BaseSet(name, strings);
			}

	// Get a list of all keys in this collection.
	public virtual String[] AllKeys
			{
				get
				{
					if(allKeysResult == null)
					{
						allKeysResult = BaseGetAllKeys();
					}
					return allKeysResult;
				}
			}

	// Get or set a specific item within this collection by name.
	public String this[String name]
			{
				get
				{
					return Get(name);
				}
				set
				{
					Set(name, value);
				}
			}

	// Get a specific item within this collection by index.
	public String this[int index]
			{
				get
				{
					return Get(index);
				}
			}

}; // class NameValueCollection

}; // namespace System.Collections.Specialized
