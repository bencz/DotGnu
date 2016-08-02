/*
 * SynchronizationAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Contexts.SynchronizationAttribute" class.
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

namespace System.Runtime.Remoting.Contexts
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Activation;

[Serializable]
[AttributeUsage(AttributeTargets.Class)]
public class SynchronizationAttribute
	: ContextAttribute, IContributeServerContextSink,
	  IContributeClientContextSink
{
	// Internal state.
	private int flag;
	private bool reEntrant;
	private bool locked;
	private Object savedProp;

	// Public flag values.
	public const int NOT_SUPPORTED	= 0x0001;
	public const int SUPPORTED		= 0x0002;
	public const int REQUIRED		= 0x0004;
	public const int REQUIRES_NEW	= 0x0008;

	// Constructors.
	public SynchronizationAttribute() : this(REQUIRED, false) {}
	public SynchronizationAttribute(bool reEntrant)
			: this(REQUIRED, reEntrant) {}
	public SynchronizationAttribute(int flag) : this(flag, false) {}
	public SynchronizationAttribute(int flag, bool reEntrant)
			: base("Synchronization")
			{
				if(flag != NOT_SUPPORTED && flag != SUPPORTED &&
				   flag != REQUIRED && flag != REQUIRES_NEW)
				{
					throw new ArgumentException
						(_("Arg_SynchronizationFlag"));
				}
				this.flag = flag;
				this.reEntrant = reEntrant;
			}

	// Determine if this attribute is re-entrant.
	public virtual bool IsReEntrant
			{
				get
				{
					return reEntrant;
				}
			}

	// Get or set the context lock.
	public virtual bool Locked
			{
				get
				{
					return locked;
				}
				set
				{
					locked = value;
				}
			}

	// Create a client context sink and prepend it to a given chain.
	public virtual IMessageSink GetClientContextSink(IMessageSink nextSink)
			{
				return new PassThroughSink(this, nextSink);
			}

	// Get the properties for a new construction context.
	public override void GetPropertiesForNewContext
				(IConstructionCallMessage ctorMsg)
			{
				if(ctorMsg != null)
				{
					if(flag == REQUIRED)
					{
						if(savedProp != null)
						{
							ctorMsg.ContextProperties.Add(savedProp);
						}
						else
						{
							ctorMsg.ContextProperties.Add(this);
						}
					}
					else if(flag == REQUIRES_NEW)
					{
						ctorMsg.ContextProperties.Add(this);
					}
				}
			}

	// Get the server context sink.
	public virtual IMessageSink GetServerContextSink(IMessageSink nextSink)
			{
				return new PassThroughSink(this, nextSink);
			}

	// Determine if a context is OK with respect to this attribute.
	public override bool IsContextOK
				(Context ctx, IConstructionCallMessage msg)
			{
				if(ctx == null)
				{
					throw new ArgumentNullException("ctx");
				}
				if(msg == null)
				{
					throw new ArgumentNullException("msg");
				}
				if(flag == NOT_SUPPORTED)
				{
					if(ctx.GetProperty("Synchronization") != null)
					{
						return false;
					}
				}
				else if(flag == REQUIRED)
				{
					Object prop = ctx.GetProperty("Synchronization");
					if(prop == null)
					{
						return false;
					}
					savedProp = prop;
				}
				else if(flag != REQUIRES_NEW)
				{
					return true;
				}
				return false;
			}

	// Pass-through sink.
	private class PassThroughSink : IMessageSink
	{
		// Internal state.
		private SynchronizationAttribute attr;
		private IMessageSink nextSink;

		// Constructor.
		public PassThroughSink
					(SynchronizationAttribute attr, IMessageSink nextSink)
				{
					this.attr = attr;
					this.nextSink = nextSink;
				}

		// Implement the IMessageSink interface.
		public IMessageSink NextSink
				{
					get
					{
						return nextSink;
					}
				}
		public IMessageCtrl AsyncProcessMessage
						(IMessage msg, IMessageSink replySink)
				{
					return nextSink.AsyncProcessMessage(msg, replySink);
				}
		public IMessage SyncProcessMessage(IMessage msg)
				{
					return nextSink.SyncProcessMessage(msg);
				}

	}; // class PassThroughSink

}; // class SynchronizationAttribute

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Contexts
