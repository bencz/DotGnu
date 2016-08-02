/*
 * DictionaryEntry.cs - Implementation of the
 *			"System.Collections.DictionaryEntry" class.
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

namespace System.Collections
{

using System;

public struct DictionaryEntry
{
	// Instance fields.
	private Object key_;
	private Object value_;

	// Construct a dictionary entry.
	public DictionaryEntry(Object key, Object value)
	{
		if(key == null)
		{
			throw new ArgumentNullException("key");
		}
		key_ = key;
		value_ = value;
	}

	// DictionaryEntry properties.
	public Object Key
	{
		get
		{
			return key_;
		}
		set
		{
			if(value == null)
			{
				throw new ArgumentNullException("value");
			}
			key_ = value;
		}
	}
	public Object Value
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

}; // struct DictionaryEntry

}; // namespace System.Collections
