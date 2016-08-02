/*
 * XPathDocument.cs - Implementation of "System.Xml.XPath.XPathDocument" class 
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

using System;
using System.IO;
using System.Xml;

#if CONFIG_XPATH

namespace System.Xml.XPath
{
#if ECMA_COMPAT
internal
#else
public
#endif
class XPathDocument : Object, IXPathNavigable
{
	[TODO]
	public XPathDocument(XmlReader reader, XmlSpace space)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public XPathDocument(XmlReader reader)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public XPathDocument(TextReader reader)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public XPathDocument(Stream stream)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public XPathDocument(String uri)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public XPathDocument(String uri, XmlSpace space)
	{
		 throw new NotImplementedException("ctor");
	}

	[TODO]
	public virtual XPathNavigator CreateNavigator()
	{
		 throw new NotImplementedException("CreateNavigator");
	}
}
}//namespace

#endif // CONFIG_XPATH
