/*
 * WellKnownServiceTypeEntry.cs - Implementation of the
 *			"System.Runtime.Remoting.WellKnownServiceTypeEntry" class.
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

namespace System.Runtime.Remoting
{

#if CONFIG_REMOTING

using System.Text;
using System.Runtime.Remoting.Contexts;

public class WellKnownServiceTypeEntry : TypeEntry
{
	// Internal state.
	private IContextAttribute[] contextAttributes;
	private String objectUri;
	private WellKnownObjectMode mode;

	// Constructor.
	public WellKnownServiceTypeEntry(Type type, String objectUri,
									 WellKnownObjectMode mode)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				if(objectUri == null)
				{
					throw new ArgumentNullException("objectUri");
				}
				actualType = type;
				TypeName = type.FullName;
				AssemblyName = type.Assembly.FullName;
				this.objectUri = objectUri;
				this.mode = mode;
			}
	public WellKnownServiceTypeEntry
				(String typeName, String assemblyName, String objectUri,
				 WellKnownObjectMode mode)
			{
				if(typeName == null)
				{
					throw new ArgumentNullException("typeName");
				}
				if(assemblyName == null)
				{
					throw new ArgumentNullException("assemblyName");
				}
				if(objectUri == null)
				{
					throw new ArgumentNullException("objectUri");
				}
				TypeName = typeName;
				AssemblyName = assemblyName;
				this.objectUri = objectUri;
				this.mode = mode;
			}

	// Get or set the context attributes.
	public IContextAttribute[] ContextAttributes
			{
				get
				{
					return contextAttributes;
				}
				set
				{
					contextAttributes = value;
				}
			}

	// Get the service object mode.
	public WellKnownObjectMode Mode
			{
				get
				{
					return mode;
				}
			}

	// Get the object type that corresponds to this service type entry.
	public Type ObjectType
			{
				get
				{
					return GetObjectType();
				}
			}

	// Get the object URL for this type entry.
	public String ObjectUri
			{
				get
				{
					return objectUri;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("type='");
				builder.Append(TypeName);
				builder.Append(", ");
				builder.Append(AssemblyName);
				builder.Append("'; objectUri=");
				builder.Append(objectUri);
				builder.Append("; mode=");
				builder.Append(mode.ToString());
				return builder.ToString();
			}

}; // class WellKnownServiceTypeEntry

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
