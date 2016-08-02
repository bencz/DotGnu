/*
 * XslTransform.cs - Implementation of "System.Xml.Xsl.XslTransform" 
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

#if CONFIG_XPATH && CONFIG_XSL

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Security.Policy;

namespace System.Xml.Xsl
{
	public sealed class XslTransform
	{
		[TODO]
		public XslTransform()
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		~XslTransform()
		{
			throw new NotImplementedException("Finalize");
		}

		[TODO]
		public void Load(IXPathNavigable stylesheet)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(String url)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XmlReader stylesheet)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XPathNavigator stylesheet)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(IXPathNavigable stylesheet, XmlResolver resolver)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(String url, XmlResolver resolver)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XmlReader stylesheet, XmlResolver resolver)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XPathNavigator stylesheet, XmlResolver resolver)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(IXPathNavigable stylesheet, XmlResolver resolver,
								Evidence evidence)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XmlReader stylesheet, XmlResolver resolver,
							Evidence evidence)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(XPathNavigator stylesheet, XmlResolver resolver,
								Evidence evidence)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(String inputfile, String outputfile)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(String inputfile, String outputfile,
							XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public XmlReader Transform(XPathNavigator input, XsltArgumentList args)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
								Stream output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
					Stream output, XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
								TextWriter output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
					TextWriter output, XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args,
								XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
								XmlWriter output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList args, 
					XmlWriter output, XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args, 
								Stream output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args, 
					Stream output, XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args, 
								TextWriter output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args,
					TextWriter output, XmlResolver resolver) 
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public XmlReader Transform(XPathNavigator input, XsltArgumentList args, 
								XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args, 
								XmlWriter output)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public void Transform(XPathNavigator input, XsltArgumentList args,
					XmlWriter output, XmlResolver resolver)
		{
			throw new NotImplementedException("Transform");
		}

		[TODO]
		public XmlResolver XmlResolver 
		{
 			set
			{
				throw new NotImplementedException("XmlResolver");
			}
 		}
	}
}//namespace
#endif // CONFIG_XPATH && CONFIG_XSL
#endif // !ECMA_COMPAT
