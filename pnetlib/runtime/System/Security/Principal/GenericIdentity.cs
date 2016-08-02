/*
 * GenericIdentity.cs - Implementation of the
 *		"System.Security.Principal.GenericIdentity" class.
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

namespace System.Security.Principal
{

#if CONFIG_POLICY_OBJECTS

[Serializable]
public class GenericIdentity : IIdentity
{
	// Internal state.
	private String name;
	private String type;

	// Constructor.
	public GenericIdentity(String name) : this(name, String.Empty) {}
	public GenericIdentity(String name, String type)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				this.name = name;
				this.type = type;
			}

	// Get the type of authentication used.
	public virtual String AuthenticationType
			{
				get
				{
					return type;
				}
			}

	// Determine if we have been authenticated.
	public virtual bool IsAuthenticated
			{
				get
				{
					return (name != String.Empty);
				}
			}

	// Get the name associated with this identity.
	public virtual String Name
			{
				get
				{
					return name;
				}
			}

}; // class GenericIdentity

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Principal
