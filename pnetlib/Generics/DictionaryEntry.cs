/*
 * DictionaryEntry.cs - Generic dictionary entries.
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

namespace Generics
{

using System;

public struct DictionaryEntry<KeyT, ValueT>
{
	// Instance fields.  "key_" is an Object so that we can detect null values.
	private Object key_;
	private ValueT value_;

	// Construct a dictionary entry.
	public DictionaryEntry(KeyT key, ValueT value)
			{
				key_ = key;
				if(key_ == null)
				{
					throw new ArgumentNullException("key");
				}
				value_ = value;
			}

	// DictionaryEntry properties.
	public KeyT Key
			{
				get
				{
					return (KeyT)key_;
				}
				set
				{
					key_ = value;
					if(key_ == null)
					{
						throw new ArgumentNullException("value");
					}
				}
			}
	public ValueT Value
			{
				get
				{
					return value_;
				}
				set
				{
					value_ = value;
				}
			}

}; // struct DictionaryEntry<KeyT, ValueT>

}; // namespace Generics
