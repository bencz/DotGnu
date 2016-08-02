/*
 * LogicalCallContext.cs - Implementation of the
 *		"System.Runtime.Remoting.Messaging.LogicalCallContext" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_SERIALIZATION

using System.Collections;
using System.Runtime.Serialization;

[Serializable]
public sealed class LogicalCallContext : ISerializable, ICloneable
{
	// Internal state.
	private Hashtable table;

	// Constructors.
	internal LogicalCallContext() {}
	internal LogicalCallContext(Hashtable clone)
			{
				table = clone;
			}
	[TODO]
	internal LogicalCallContext(SerializationInfo info,
								StreamingContext context)
			{
				// TODO
			}

	// Determine if this context contains information.
	public bool HasInfo
			{
				get
				{
					return (table != null && table.Count != 0);
				}
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				if(table != null)
				{
					return new LogicalCallContext((Hashtable)(table.Clone()));
				}
				else
				{
					return new LogicalCallContext();
				}
			}

	// Free a particular named data slot.
	public void FreeNamedDataSlot(String name)
			{
				if(table != null)
				{
					table.Remove(name);
				}
			}

	// Get the data in a particular named data slot.
	public Object GetData(String name)
			{
				if(table != null)
				{
					return table[name];
				}
				else
				{
					return null;
				}
			}

	// Implement the ISerializable interface.
	[TODO]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				// TODO
			}

	// Set the data in a particular named data slot.
	public void SetData(String name, Object value)
			{
				if(table != null)
				{
					table[name] = value;
				}
				else
				{
					table = new Hashtable();
					table[name] = value;
				}
			}

	// Get the headers that are stored in this logical call context.
	[TODO]
	internal Header[] GetHeaders()
			{
				// TODO
				return null;
			}

	// Set the headers that are stored in this logical call context.
	[TODO]
	internal void SetHeaders(Header[] headers)
			{
				// TODO
			}

}; // class LogicalCallContext

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Messaging
