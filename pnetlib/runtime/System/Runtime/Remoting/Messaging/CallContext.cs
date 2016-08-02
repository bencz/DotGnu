/*
 * CallContext.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.CallContext" class.
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

#if CONFIG_REMOTING

[Serializable]
public sealed class CallContext
{

	// We cannot instantiate this class.
	private CallContext() {}

	// Get the logical call context for the current thread.
	[TODO]
	private static LogicalCallContext LogicalCallContext
			{
				get
				{
					// TODO
					return new LogicalCallContext();
				}
			}

	// Free the contents of a named data slot.
	public static void FreeNamedDataSlot(String name)
			{
				LogicalCallContext.FreeNamedDataSlot(name);
			}

	// Get the data in a particular named data slot.
	public static Object GetData(String name)
			{
				return LogicalCallContext.GetData(name);
			}

	// Get the headers that were sent with the method call.
	public static Header[] GetHeaders()
			{
				return LogicalCallContext.GetHeaders();
			}

	// Set the contents of a named data slot.
	public static void SetData(String name, Object value)
			{
				LogicalCallContext.SetData(name, value);
			}

	// Set the headers to be sent along with the method call.
	public static void SetHeaders(Header[] headers)
			{
				LogicalCallContext.SetHeaders(headers);
			}

}; // class CallContext

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
