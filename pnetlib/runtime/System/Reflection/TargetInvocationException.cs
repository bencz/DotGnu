/*
 * TargetInvocationException.cs - Implementation of the
 *			"System.Reflection.TargetInvocationException" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

using System;
using System.Runtime.Serialization;

public sealed class TargetInvocationException : ApplicationException
{

	// Constructors.
	private TargetInvocationException()
		: base(_("Exception_TargetInvoke")) {}
	private TargetInvocationException(String msg)
		: base(msg) {}
	public TargetInvocationException(String msg, Exception inner)
		: base(msg, inner) {}
	public TargetInvocationException(Exception inner)
		: base(_("Exception_TargetInvoke"), inner) {}
#if CONFIG_SERIALIZATION
	internal TargetInvocationException(SerializationInfo info,
									   StreamingContext context)
		: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_TargetInvoke");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131604;
				}
			}

}; // class TargetInvocationException

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
