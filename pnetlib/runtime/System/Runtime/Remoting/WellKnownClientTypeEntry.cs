/*
 * WellKnownClientTypeEntry.cs - Implementation of the
 *			"System.Runtime.Remoting.WellKnownClientTypeEntry" class.
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

public class WellKnownClientTypeEntry : TypeEntry
{
	// Internal state.
	private String applicationUrl;
	private String objectUrl;

	// Constructor.
	public WellKnownClientTypeEntry(Type type, String objectUrl)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				if(objectUrl == null)
				{
					throw new ArgumentNullException("objectUrl");
				}
				actualType = type;
				TypeName = type.FullName;
				AssemblyName = type.Assembly.FullName;
				this.objectUrl = objectUrl;
			}
	public WellKnownClientTypeEntry
				(String typeName, String assemblyName, String objectUrl)
			{
				if(typeName == null)
				{
					throw new ArgumentNullException("typeName");
				}
				if(assemblyName == null)
				{
					throw new ArgumentNullException("assemblyName");
				}
				if(objectUrl == null)
				{
					throw new ArgumentNullException("objectUrl");
				}
				TypeName = typeName;
				AssemblyName = assemblyName;
				this.objectUrl = objectUrl;
			}

	// Get or set the application URL for this type entry.
	public String ApplicationUrl
			{
				get
				{
					return applicationUrl;
				}
				set
				{
					applicationUrl = value;
				}
			}

	// Get the object type that corresponds to this client type entry.
	public Type ObjectType
			{
				get
				{
					return GetObjectType();
				}
			}

	// Get the object URL for this type entry.
	public String ObjectUrl
			{
				get
				{
					return objectUrl;
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
				builder.Append("'; url=");
				builder.Append(objectUrl);
				if(applicationUrl != null)
				{
					builder.Append("; appUrl=");
					builder.Append(applicationUrl);
				}
				return builder.ToString();
			}

}; // class WellKnownClientTypeEntry

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
