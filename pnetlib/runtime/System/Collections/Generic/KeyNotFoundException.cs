/*
 * KeyNotFoundException.cs - Implementation of the
 *		"System.Collections.Generic.KeyNotFoundException" class.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Generic
{

#if CONFIG_FRAMEWORK_2_0

using System;

#if CONFIG_SERIALIZATION && !ECMA_COMPAT
using System.Runtime.Serialization;
#endif

#if !ECMA_COMPAT
[Serializable]
#endif
public class KeyNotFoundException : SystemException
#if CONFIG_SERIALIZATION && !ECMA_COMPAT
	, ISerializable
#endif
{
	// Standard error message for null exceptions.
	private static String preloadedMessage = _("Key_NotFound");

	// Constructors.
	public KeyNotFoundException()
		: base(preloadedMessage) {}
	public KeyNotFoundException(String message)
		: base(message) {}
	public KeyNotFoundException(String message, Exception innerException)
		: base(message, innerException) {}
#if CONFIG_SERIALIZATION  && !ECMA_COMPAT
	protected KeyNotFoundException(SerializationInfo info,
								   StreamingContext context)
			: base(info, context) {}
#endif

}; // class KeyNotFoundException

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Collections.Generic
