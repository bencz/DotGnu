/*
 * ArgumentOutOfRangeException.cs - Implementation of the
 *		"System.ArgumentOutOfRangeException" class.
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

using System.Runtime.Serialization;

public class ArgumentOutOfRangeException : ArgumentException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Standard error message for out of range exceptions.
	private static String preloadedMessage = _("Arg_OutOfRange");

	// Internal state.
	private Object actualValue;

	// Constructors.
	public ArgumentOutOfRangeException()
			: base(preloadedMessage) {}
	public ArgumentOutOfRangeException(String paramName)
			: base(preloadedMessage, paramName) {}
	public ArgumentOutOfRangeException(String paramName, String msg)
			: base(msg, paramName) {}
	public ArgumentOutOfRangeException(String paramName,
									   Object _actualValue, String msg)
			: base(msg, paramName) { actualValue = _actualValue; }
#if CONFIG_SERIALIZATION
	protected ArgumentOutOfRangeException(SerializationInfo info,
										  StreamingContext context)
			: base(info, context)
			{
				actualValue = info.GetValue("ActualValue", typeof(Object));
			}
#endif

	// Properties.
	public virtual Object ActualValue
			{
				get
				{
					return actualValue;
				}
			}
#if !ECMA_COMPAT
	public override String Message
			{
				get
				{
					String msg = base.Message;
					if(actualValue == null)
					{
						return msg;
					}
					else
					{
						return msg + Environment.NewLine +
							   String.Format
							   		(_("Arg_OutOfRangeValue"),
									 actualValue.ToString());
					}
				}
			}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return preloadedMessage;
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131502;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("ActualValue", actualValue, typeof(Object));
			}
#endif

}; // class ArgumentOutOfRangeException

}; // namespace System
