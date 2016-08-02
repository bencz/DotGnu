/*
 * ArgumentNullException.cs - Implementation of the
 *		"System.ArgumentNullException" class.
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

public class ArgumentNullException : ArgumentException
{
	// Standard error message for null exceptions.
	private static String preloadedMessage = _("Arg_NotNull");
	private static String preloadedNameMessage = _("Arg_NotNullName");

	// Constructors.
	public ArgumentNullException()
		: base(preloadedMessage) {}
	public ArgumentNullException(String paramName)
		: base(preloadedNameMessage, paramName) {}
	public ArgumentNullException(String paramName, String msg)
		: base(msg, paramName) {}
#if CONFIG_SERIALIZATION
	protected ArgumentNullException(SerializationInfo info,
									StreamingContext context)
		: base(info, context) {}
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
					return 0x80004003;
				}
			}

}; // class ArgumentNullException

}; // namespace System
