/*
 * MissingMethodException.cs - Implementation of the
 *		"System.MissingMethodException" class.
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

namespace System
{

#if CONFIG_RUNTIME_INFRA

using System.Runtime.Serialization;

public class MissingMethodException : MissingMemberException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Constructors.
	public MissingMethodException()
		: base(_("Exception_MethodMissing")) {}
	public MissingMethodException(String msg)
		: base(msg) {}
	public MissingMethodException(String msg, Exception inner)
		: base(msg, inner) {}
#if !ECMA_COMPAT
	public MissingMethodException(String className, String methodName)
			: base(className, methodName) {}
#endif
#if CONFIG_SERIALIZATION
	protected MissingMethodException(SerializationInfo info,
									 StreamingContext context)
			: base(info, context) {}
#endif

#if !ECMA_COMPAT
	// Get the message string for this exception.
	public override String Message
			{
				get
				{
					return base.Message;
				}
			}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_MethodMissing");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131513;
				}
			}

}; // class MissingMethodException

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
