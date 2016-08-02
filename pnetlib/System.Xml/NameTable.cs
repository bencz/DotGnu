/*
 * NameTable.cs - Implementation of the "System.Xml.NameTable" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

using System;
using System.Collections;

public class NameTable : XmlNameTable
{
	// Internal state.
	private NameHashtable table;
	
	// Constructor.
	public NameTable()
			{
				table = new NameHashtable();
			}
	
	// Add a string to the table if it doesn't already exist.
	public override String Add(String key)
			{
				// Validate the parameters.
				if(key == null)
				{
					throw new ArgumentNullException("key");		
				}

				// If the key is zero length, then always return String.Empty.
				if(key.Length == 0)
				{
					return String.Empty;
				}
		 
				// Retrieve the current string if the name is already present.
				String current = (String)(table[key]);
				if(current != null)
				{
					return current;
				}

				// Add the new string to the table.
				table[key] = key;
				return key;
			}
	
	// Add a string to the table from an array.
	public override String Add(char[] key, int start, int len)
			{
				// Validate the parameters.
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				else if(len < 0)
				{
					throw new ArgumentOutOfRangeException
						("len", S._("ArgRange_StringRange"));
				}
				else if((start < 0) || (start + len > key.Length))
				{
					throw new IndexOutOfRangeException
						(S._("Arg_InvalidArrayRange"));
				}

				// If the key is zero length, then always return String.Empty.
				if(len == 0)
				{
					return String.Empty;
				}
		 
				// Retrieve the current string if the name is already present.
				String current = table.Lookup(key, start, len);
				if(current != null)
				{
					return current;
				}

				// Add the new string to the table.
				String skey = new String(key, start, len);
				table[skey] = skey;
				return skey;
			}

	// Get a string from the table by name.
	public override String Get(String value)
			{
				// Validate the parameters.
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}

				// If the string has zero length, then return String.Empty.
				if(value.Length == 0)
				{
					return String.Empty;
				}

				// Get the current value from the table.
				return (String)(table[value]);
			}
	
	// Get a string from the table by array name.
	public override String Get(char[] key, int start, int len)
			{
				// Validate the parameters.
				if(key == null)
				{
					throw new ArgumentNullException("value");
				}
				else if(len < 0)
				{
					throw new ArgumentOutOfRangeException
						("len", S._("ArgRange_StringRange"));
				}
				else if((start < 0) || (start + len > key.Length))
				{
					throw new IndexOutOfRangeException
						(S._("Arg_InvalidArrayRange"));
				}

				// If the key is zero length, then always return String.Empty.
				if(len == 0)
				{
					return String.Empty;
				}
		 
		 		// Retrieve the current value from the table.
				return table.Lookup(key, start, len);
			}

	// Hash table specialization for doing lookups in different ways.
	private class NameHashtable : Hashtable
	{
		// Internal state.
		private char[] arrayKey;
		private int arrayOffset;
		private int arrayLength;

		// Constructor.
		public NameHashtable() : base(128) {} // avoid expanding of hashtable

		// Do a lookup based on a character array.
		public String Lookup(char[] key, int offset, int length)
				{
					arrayKey = key;
					arrayOffset = offset;
					arrayLength = length;
					String result = (String)(this[key]);
					arrayKey = null;
					return result;
				}

		// Get the hash value for a key.
		protected override int GetHash(Object key)
				{
					if(arrayKey == null)
					{
						return base.GetHash(key);
					}
					else
					{
						// This must match the hash algorithm for
						// "String.GetHashCode()" in the engine.
						int hash = 0;
						int posn;
						for(posn = 0; posn < arrayLength; ++posn)
						{
							hash = (hash << 5) + hash +
								   (int)(arrayKey[arrayOffset + posn]);
						}
						return hash;
					}
				}

		// Determine if two keys are equal.
		protected override bool KeyEquals(Object _item, Object key)
				{
					if(arrayKey == null)
					{
						return ((String)_item) == ((String)key);
					}
					else
					{
						String item = ((String)_item);
						if(item.Length != arrayLength)
						{
							return false;
						}
						int posn;
						for(posn = 0; posn < arrayLength; ++posn)
						{
							if(item[posn] != arrayKey[arrayOffset + posn])
							{
								return false;
							}
						}
						return true;
					}
				}

	}; // class NameHashtable

}; // class NameTable

}; // namespace System.Xml
