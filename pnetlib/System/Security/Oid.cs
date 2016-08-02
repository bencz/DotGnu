/*
 * Oid.cs - Implementation of the "System.Security.Oid" class.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
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

namespace System.Security
{

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

public sealed class Oid
{
	// Internal state.
	private String value;
	private String friendlyName;

	// Constructors.
	public Oid()
	{
	}

	public Oid(Oid oid)
	{
		if(oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		this.value = oid.value;
		this.friendlyName = oid.friendlyName;
	}

	public Oid(String oid)
	{
		this.value = oid;
	}

	public Oid(String value, String friendlyName)
	{
		this.value = value;
		this.friendlyName = friendlyName;
	}

	// Get this object's properties.
	public String FriendlyName
	{
		get
		{
			return this.friendlyName;
		}
		set
		{
			this.friendlyName = value;
		}
	}

	public String Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

}; // class Oid

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

}; // namespace System.Security
