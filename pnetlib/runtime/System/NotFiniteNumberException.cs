/*
 * NotFiniteNumberException.cs - Implementation of the
 *			"System.NotFiniteNumberException" class.
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

#if CONFIG_EXTENDED_NUMERICS

using System.Runtime.Serialization;

public class NotFiniteNumberException : ArithmeticException
{
	// Internal state.
	private double number;

	// Constructors.
	public NotFiniteNumberException()
		: base(_("Exception_NotFinite"))
		{ number = 0.0; }
	public NotFiniteNumberException(double offendingNumber)
		: base(_("Exception_NotFinite"))
		{ number = offendingNumber; }
	public NotFiniteNumberException(String msg)
		: base(msg) { number = 0.0; }
	public NotFiniteNumberException(String msg, double offendingNumber)
		: base(msg) { number = offendingNumber; }
	public NotFiniteNumberException(String msg, double offendingNumber,
									Exception inner)
		: base(msg, inner) { number = offendingNumber; }
#if CONFIG_SERIALIZATION
	protected NotFiniteNumberException(SerializationInfo info,
									   StreamingContext context)
		: base(info, context)
		{
			number = info.GetDouble("OffendingNumber");
		}
#endif

	// Get the offending number that caused this exception.
	public double OffendingNumber
			{
				get
				{
					return number;
				}
			}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_NotFinite");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131528;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("OffendingNumber", number);
			}
#endif

}; // class NotFiniteNumberException

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System
