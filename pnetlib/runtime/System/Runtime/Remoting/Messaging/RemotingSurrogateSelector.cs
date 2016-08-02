/*
 * RemotingSurrogateSelector.cs - Implementation of the
 *		"System.Runtime.Remoting.Messaging.RemotingSurrogateSelector" class.
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

using System.Runtime.Serialization;

public class RemotingSurrogateSelector : ISurrogateSelector
{
	// Internal state.
	private MessageSurrogateFilter filter;
	private ISurrogateSelector next;
	private Object rootObject;

	// Constructor.
	[TODO]
	public RemotingSurrogateSelector()
			{
				// TODO
			}

	// Get or set the message surrogate filter.
	public MessageSurrogateFilter Filter
			{
				get
				{
					return filter;
				}
				set
				{
					filter = value;
				}
			}

	// Implement the ISurrogateSelector interface.
	[TODO]
	public virtual void ChainSelector(ISurrogateSelector selector)
			{
				// TODO
			}
	public virtual ISurrogateSelector GetNextSelector()
			{
				return next;
			}
	[TODO]
	public virtual ISerializationSurrogate GetSurrogate
				(Type type, StreamingContext context,
				 out ISurrogateSelector selector)
			{
				// TODO
				selector = null;
				return null;
			}

	// Get the root of the object graph.
	public Object GetRootObject()
			{
				return rootObject;
			}

	// Set the root of the object graph.
	public void SetRootObject(Object obj)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				rootObject = obj;
			}

	// Tell this selector to use the SOAP format.
	[TODO]
	public virtual void UseSoapFormat()
			{
				// TODO
			}

}; // class RemotingSurrogateSelector

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
