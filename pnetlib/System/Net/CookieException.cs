/*
 * CookieException.cs - Implementation of the
 *			"System.Net.CookieException" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Net
{

#if !ECMA_COMPAT

using System.Runtime.Serialization;

[Serializable]
public class CookieException : FormatException
{
	// Constructors.
	public CookieException()
		: base(S._("Exception_Cookie")) {}
	internal CookieException(String msg)
		: base(msg) {}
	internal CookieException(String msg, Exception inner)
		: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected CookieException(SerializationInfo info, StreamingContext context)
		: base(info, context) {}
#endif

}; // class CookieException

#endif // !ECMA_COMPAT

}; // namespace System.Net
