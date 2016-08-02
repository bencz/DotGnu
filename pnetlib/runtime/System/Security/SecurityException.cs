/*
 * SecurityException.cs - Implementation of the
 *		"System.Security.SecurityException" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System;
using System.Runtime.Serialization;

public class SecurityException : SystemException
{
#if CONFIG_PERMISSIONS
	// Internal state.
	private String permissionState;
	private Type permissionType;
	private String grantedSet;
	private String refusedSet;
#endif

	// Constructors.
	public SecurityException()
			: base(_("Exception_Security")) {}
	public SecurityException(String msg)
			: base(msg) {}
	public SecurityException(String msg, Exception inner)
			: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected SecurityException(SerializationInfo info,
								StreamingContext context)
			: base(info, context)
			{
				permissionState = info.GetString("PermissionState");
			}
#endif
#if CONFIG_PERMISSIONS
	public SecurityException(String message, Type type)
			: base(message)
			{
				this.permissionType = type;
			}
	public SecurityException(String message, Type type, String state)
			: base(message)
			{
				this.permissionType = type;
				this.permissionState = state;
			}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_Security");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x8013150a;
				}
			}

#if CONFIG_PERMISSIONS

	// Get the permission type.
	public Type PermissionType
			{
				get
				{
					return permissionType;
				}
			}

	// Get the permission state.
	public String PermissionState
			{
				get
				{
					return permissionState;
				}
			}

	// Get the granted permission set.
	public String GrantedSet
			{
				get
				{
					return grantedSet;
				}
			}

	// Get the refused permission set.
	public String RefusedSet
			{
				get
				{
					return refusedSet;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString();
			}

#endif // CONFIG_PERMISSIONS

#if CONFIG_SERIALIZATION

	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
			#if CONFIG_PERMISSIONS
				info.AddValue("PermissionState", permissionState);
			#endif
			}

#endif

}; // class SecurityException

}; // namespace System.Security
