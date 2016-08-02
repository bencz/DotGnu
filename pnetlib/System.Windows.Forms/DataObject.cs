/*
 * DataObject.cs - Implementation of the
 *			"System.Windows.Forms.DataObject" class.
 *
 * Copyright (C) 2003  Free Software Foundation, Inc.
 *
 * Contributions from Cecilio Pardo <cpardo@imayhem.com>
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

namespace System.Windows.Forms
{

using System;
using System.Collections;

[TODO]
//[ClassInterface(ClassInterfaceType.None)]
public class DataObject : IDataObject
{
	Hashtable objects = new Hashtable();

	public DataObject() {}

	public DataObject(Object o)
	{
		SetData(o);
	}

	public DataObject(String s, Object o)
	{
		SetData(s, o);
	}

	public virtual Object GetData(String s)
	{
		return GetData(s, true);
	}

	[TODO]
	public virtual Object GetData(Type t)
	{
		return null;
	}

	[TODO]
	// Handle: Format conversion
	public virtual Object GetData(String s, bool b)
	{
		if (s == null)
		{
			return null;
		}
		if (objects.ContainsKey(s))
		{
			return ((DataObjectElement)objects[s]).data;
		}
		return null;
	}

	public virtual bool GetDataPresent(String s)
	{
		return GetDataPresent(s, true);
	}

	[TODO]
	public virtual bool GetDataPresent(Type t)
	{
		return false;
	}

	[TODO]
	// Handle: Check if some other format is convertible
	public virtual bool GetDataPresent(String s, bool b)
	{
		return objects.ContainsKey(s);
	}

	public virtual String[] GetFormats()
	{
		return GetFormats(true);
	}

	[TODO]
	// Handle: Implement format convertibility
	public virtual String[] GetFormats(bool b)
	{
		String[] s;
		s = new String[objects.Count];
		int i = 0;
		foreach (DictionaryEntry d in objects)
		{
			s[i++] = d.Key.ToString();
		}
		return s;
	}

	[TODO]
	// Handle: This method has to find the format from the class
	public virtual void SetData(Object o)
	{
		SetData(DataFormats.StringFormat, o.ToString()); 
	}

	public virtual void SetData(String s, Object o)
	{
		SetData(s, true, o);
	}

	[TODO]
	public virtual void SetData(Type t, Object o)
	{
	}

	public virtual void SetData(String s, bool b, Object o)
	{
		if (s == null)
		{
			s = DataFormats.StringFormat;
		}
		objects.Remove(s);
		objects.Add(s, new DataObjectElement(s, o, true));
	}





















	internal class DataObjectElement
	{
		public String format;
		public Object data;
		public bool canConvert;

		public DataObjectElement(String format, Object data, bool canConvert)
		{
			this.format = format;
			this.data = data;
			this.canConvert = canConvert;
		}

	}; // class DataObjectElement

}; // class DataObject

}; // namespace System.Windows.Forms
