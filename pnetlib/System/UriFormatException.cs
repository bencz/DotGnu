/*
 * UriFormatException.cs - Implementation of the
 *		"System.UriFormatException" class.
 *
 * Copyright (C) 2001, 2003  Free Software Foundation, Inc.
 *
 * Contributed by Stephen Compall <rushing@sigecom.net>
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

public class UriFormatException : FormatException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Constructors.
	public UriFormatException()
		: base(S._("Exception_UriFormat"))
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131537);
		#endif
		}
	public UriFormatException(String msg)
		: base(msg)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131537);
		#endif
		}

#if CONFIG_SERIALIZATION

	// De-serialize this object.
	protected UriFormatException(SerializationInfo info,
								 StreamingContext context)
		: base(info, context) {}

	// Get the serialization data for this object.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

#endif // CONFIG_SERIALIZATION

}; // class UriFormatException

}; // namespace System
