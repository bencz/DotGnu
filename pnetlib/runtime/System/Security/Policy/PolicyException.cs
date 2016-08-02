/*
 * PolicyException.cs - Implementation of the
 *		"System.Security.PolicyException" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS

using System;
using System.Runtime.Serialization;

public class PolicyException : SystemException
{
	// Constructors.
	public PolicyException()
			: base(_("Exception_Policy")) {}
	public PolicyException(String msg)
			: base(msg) {}
	public PolicyException(String msg, Exception inner)
			: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected PolicyException(SerializationInfo info,
							  StreamingContext context)
			: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_Policy");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131416;
				}
			}

}; // class PolicyException

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
