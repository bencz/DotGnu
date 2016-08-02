/*
 * XmlCharacterData.cs - Implementation of the
 *		"System.Xml.XmlCharacterData" class.
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
using System.Text;

#if ECMA_COMPAT
internal
#else
public
#endif
abstract class XmlCharacterData : XmlLinkedNode
{
	// Internal state.  May be either "String" or "StringBuilder".
	private Object data;

	// Constructors.
	internal XmlCharacterData(XmlNode parent, String data)
			: base(parent)
			{
				this.data = data;
			}
	protected internal XmlCharacterData(String data, XmlDocument doc)
			: base(doc)
			{
				this.data = data;
			}

	// Get or set the data in this node.
	public virtual String Data
			{
				get
				{
					return GetString();
				}
				set
				{
					XmlNodeChangedEventArgs args;
					args = EmitBefore(XmlNodeChangedAction.Change);
					data = value;
					EmitAfter(args);
				}
			}

	// Get or set the inner text for this node.
	public override String InnerText
			{
				get
				{
					return Value;
				}
				set
				{
					Value = value;
				}
			}

	// Get the length of the character data.
	public virtual int Length
			{
				get
				{
					if(data == null)
					{
						return 0;
					}
					else if(data is StringBuilder)
					{
						return ((StringBuilder)data).Length;
					}
					else
					{
						return ((String)data).Length;
					}
				}
			}

	// Get or set the value of this node.
	public override String Value
			{
				get
				{
					return Data;
				}
				set
				{
					Data = value;
				}
			}

	// Get the underlying data as a string builder.
	private StringBuilder GetBuilder()
			{
				StringBuilder builder;
				if(data == null)
				{
					builder = new StringBuilder();
				}
				else if(data is StringBuilder)
				{
					return (StringBuilder)data;
				}
				else
				{
					builder = new StringBuilder((String)data);
				}
				data = builder;
				return builder;
			}

	// Get the underlying data as a string.
	private String GetString()
			{
				if(data == null)
				{
					return String.Empty;
				}
				else if(data is StringBuilder)
				{
					data = ((StringBuilder)data).ToString();
					return (String)data;
				}
				else
				{
					return (String)data;
				}
			}

	// Append data to this node.
	public virtual void AppendData(String strData)
			{
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Change);
				GetBuilder().Append(strData);
				EmitAfter(args);
			}

	// Delete data from this node.
	public virtual void DeleteData(int offset, int count)
			{
				// Clamp the range to the actual data bounds.
				int length = Length;
				if(offset < 0)
				{
					count += offset;
					offset = 0;
				}
				else if(offset >= length)
				{
					offset = length;
					count = 0;
				}
				if((length - offset) < count)
				{
					count = length - offset;
				}
				if(count < 0)
				{
					count = 0;
				}

				// Delete the character range.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Change);
				GetBuilder().Remove(offset, count);
				EmitAfter(args);
			}

	// Insert data into this node.
	public virtual void InsertData(int offset, String strData)
			{
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Change);
				GetBuilder().Insert(offset, strData);
				EmitAfter(args);
			}

	// Replace the data at a particular position within this node.
	public virtual void ReplaceData(int offset, int count, String strData)
			{
				// Clamp the range to the actual data bounds.
				int length = Length;
				if(offset < 0)
				{
					count += offset;
					offset = 0;
				}
				else if(offset >= length)
				{
					offset = length;
					count = 0;
				}
				if((length - offset) < count)
				{
					count = length - offset;
				}
				if(count < 0)
				{
					count = 0;
				}

				// Replace the range with the supplied string.
				XmlNodeChangedEventArgs args;
				args = EmitBefore(XmlNodeChangedAction.Change);
				GetBuilder().Remove(offset, count);
				GetBuilder().Insert(offset, strData);
				EmitAfter(args);
			}

	// Retrieve a substring.
	public virtual String Substring(int offset, int count)
			{
				// Clamp the range to the actual data bounds.
				int length = Length;
				if(offset < 0)
				{
					count += offset;
					offset = 0;
				}
				else if(offset >= length)
				{
					offset = length;
					count = 0;
				}
				if((length - offset) < count)
				{
					count = length - offset;
				}
				if(count < 0)
				{
					count = 0;
				}

				// Get the substring.
				return GetString().Substring(offset, count);
			}

}; // class XmlCharacterData

}; // namespace System.Xml
