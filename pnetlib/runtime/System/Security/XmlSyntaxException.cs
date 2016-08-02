/*
 * XmlSyntaxException.cs - Implementation of the
 *		"System.Security.XmlSyntaxException" class.
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

namespace System.Security
{

#if !ECMA_COMPAT && (CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS)

using System;
using System.Runtime.Serialization;

public sealed class XmlSyntaxException : SystemException
{
	// Constructors.
	public XmlSyntaxException()
			: base(_("Exception_XmlSyntax")) {}
	public XmlSyntaxException(String msg)
			: base(msg) {}
	public XmlSyntaxException(String msg, Exception inner)
			: base(msg, inner) {}
	public XmlSyntaxException(int lineNumber)
			: base(String.Format(_("Exception_XmlSyntaxLine"), lineNumber)) {}
	public XmlSyntaxException(int lineNumber, String message)
			: base(String.Format(_("Exception_XmlSyntaxLineMsg"),
								 lineNumber, message)) {}
#if CONFIG_SERIALIZATION
	internal XmlSyntaxException(SerializationInfo info,
								StreamingContext context)
			: base(info, context) {}
#endif

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131418;
				}
			}

}; // class XmlSyntaxException

#endif // !ECMA_COMPAT && (CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS)

}; // namespace System.Security
