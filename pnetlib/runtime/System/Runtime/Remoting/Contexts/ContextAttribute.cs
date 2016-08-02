/*
 * ContextAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Contexts.ContextAttribute" class.
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

using System.Runtime.Remoting.Activation;

[Serializable]
[AttributeUsage(AttributeTargets.Class)]
public class ContextAttribute : Attribute, IContextAttribute, IContextProperty
{
	// Accessible state.
	protected String AttributeName;

	// Constructor.
	public ContextAttribute(String name)
			{
				this.AttributeName = name;
			}

	// Get the name of this property.
	public virtual String Name
			{
				get
				{
					return AttributeName;
				}
			}

	// Determine if two context attributes are equal.
	public override bool Equals(Object obj)
			{
				ContextAttribute other = (obj as ContextAttribute);
				if(other != null)
				{
					return (AttributeName == other.AttributeName);
				}
				else
				{
					return false;
				}
			}

	// Freeze this property within a specific context.
	public virtual void Freeze(Context newContext)
			{
				// Nothing to do here.
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return AttributeName.GetHashCode();
			}

	// Get the properties for a new construction context.
	public virtual void GetPropertiesForNewContext
				(IConstructionCallMessage ctorMsg)
			{
				if(ctorMsg == null)
				{
					throw new ArgumentNullException("ctorMsg");
				}
				ctorMsg.ContextProperties.Add(this);
			}

	// Determine if a context is OK with respect to this attribute.
	public virtual bool IsContextOK
				(Context ctx, IConstructionCallMessage ctorMsg)
			{
				if(ctx == null)
				{
					throw new ArgumentNullException("ctx");
				}
				if(ctorMsg == null)
				{
					throw new ArgumentNullException("ctorMsg");
				}
				if(!ctorMsg.ActivationType.IsContextful)
				{
					return true;
				}
				Object value = ctx.GetProperty(AttributeName);
				return (value != null && this.Equals(value));
			}

	// Determine if a new context is OK for this property.
	public virtual bool IsNewContextOK(Context newCtx)
			{
				// Nothing to do here except to say "yes".
				return true;
			}

}; // class ContextAttribute

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Contexts
