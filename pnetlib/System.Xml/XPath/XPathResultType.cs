/*
 * XPathResultType.cs - Implementation of 
 * 					"System.Xml.XPath.XPathResultType" enum 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V 
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

#if CONFIG_XPATH

namespace System.Xml.XPath
{
#if ECMA_COMPAT
internal
#else
public 
#endif
enum XPathResultType
{
	Number = 0x00,
	String = 0x01,
	Boolean = 0x02,
	NodeSet = 0x03,
	Navigator = String,
	Any = 0x05,
	Error = 0x06
}
}//namespace

#endif // CONFIG_XPATH
