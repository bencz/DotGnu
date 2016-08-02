/*
 * AccessControlException.cs - Implementation of the
 *		"System.Security.AccessControl.AccessControlException" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Security.AccessControl
{

#if CONFIG_ACCESS_CONTROL

using System;
using System.Runtime.Serialization;

public sealed class AccessControlException : SystemException
{
	// Constructors.
	public AccessControlException()
			: base(_("Exception_AccessControl")) {}
	public AccessControlException(String msg)
			: base(msg) {}
	public AccessControlException(String msg, Exception inner)
			: base(msg, inner) {}
	public AccessControlException(int errorCode)
			: base(_("Exception_AccessControl"))
			{
			#if !ECMA_COMPAT
				HResult = errorCode;
			#endif
			}
	public AccessControlException(int errorCode, String message)
			: base(message)
			{
			#if !ECMA_COMPAT
				HResult = errorCode;
			#endif
			}
#if CONFIG_SERIALIZATION
	internal AccessControlException(SerializationInfo info,
								    StreamingContext context)
			: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_AccessControl");
				}
			}

}; // class AccessControlException

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
