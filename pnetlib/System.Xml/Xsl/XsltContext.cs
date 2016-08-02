/*
 * XsltContext.cs - Implementation of "System.Xml.Xsl.XsltContext" 
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

#if !ECMA_COMPAT

#if CONFIG_XSL && CONFIG_XPATH

using System;
using System.Xml;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public abstract class XsltContext: XmlNamespaceManager
	{
		[TODO]
		public XsltContext() : this(new NameTable())
		{
			throw new NotImplementedException(".ctor");
		}

		public XsltContext(NameTable table) : base(table)
		{
		}

		public abstract int CompareDocument(String baseUri, 
		String nextbaseUri);

		public abstract bool PreserveWhitespace(XPathNavigator nav);

		public abstract IXsltContextFunction ResolveFunction(String prefix, 
								String name, XPathResultType[] ArgTypes);

		public abstract IXsltContextVariable ResolveVariable(String prefix, 
								String name);

		public abstract bool Whitespace { get; }

	}
}//namespace
#endif // CONFIG_XPATH && CONFIG_XSL
#endif // !ECMA_COMPAT
