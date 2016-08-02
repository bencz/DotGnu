/*
 * XslCompiledTransform.cs - Implementation of "System.Xml.Xsl.XslCompiledTransform" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Klaus.T.
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
#if CONFIG_FRAMEWORK_2_0
#if CONFIG_XPATH && CONFIG_XSL

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.CodeDom.Compiler;
using System.Security.Policy;

namespace System.Xml.Xsl
{
	public sealed class XslCompiledTransform
	{
		private Boolean debugEnabled;

		public XslCompiledTransform()
		{
			debugEnabled = false;
		}

		public XslCompiledTransform(Boolean enableDebug)
		{
			debugEnabled = enableDebug;
		}

		[TODO]
		public XmlWriterSettings OutputSettings
		{
			get
			{
				// derieved from the xsl::output element of a compiled
				// stylesheet.
				return null;
			}
		}

		[TODO]
		public TempFileCollection TemporaryFiles
		{
			get
			{
				// is null if debugging is not enabled and Load was not
				// successfully called
				return null;
			}
		}

		[TODO]
		public void Load(IXPathNavigable stylesheet)
		{
			throw new NotImplementedException("Load");
		}

		[TODO]
		public void Load(String stylesheetUri)
		{
			throw new NotImplementedException("Load(String)");
		}

		[TODO]
		public void Load(XmlReader stylesheet)
		{
			throw new NotImplementedException("Load(XmlReader)");
		}

		[TODO]
		public void Load(IXPathNavigable stylesheet, XsltSettings settings,
							XmlResolver stylesheetResolver)
		{
			throw new NotImplementedException("Load(IXPathNavigable, XsltSettings, XmlResolver)");
		}

		[TODO]
		public void Load(String stylesheetUri, XsltSettings settings,
							XmlResolver stylesheetResolver)
		{
			throw new NotImplementedException("Load(String, XsltSettings, XmlResolver)");
		}

		[TODO]
		public void Load(XmlReader stylesheet, XsltSettings settings,
							XmlResolver stylesheetResolver)
		{
			throw new NotImplementedException("Load(XmlReader, XsltSettings, XmlResolver)");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XmlWriter results)
		{
			throw new NotImplementedException("Transform(IXPathNavigable, XmlWriter)");
		}

		[TODO]
		public void Transform(String inputUri, String resultsFile)
		{
			if(inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if(resultsFile == null)
			{
				throw new ArgumentNullException("resultsFile");
			}
			throw new NotImplementedException("Transform(String, String)");
		}

		[TODO]
		public void Transform(String inputUri, XmlWriter results)
		{
			if(inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(String, XmlWriter)");
		}

		[TODO]
		public void Transform(XmlReader input, XmlWriter results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(XmlReader, XmlWriter)");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList arguments,
								Stream results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(IXPathNavigable, XsltArgumentList, Stream)");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList arguments,
								TextWriter results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(IXPathNavigable, XsltArgumentList, TextWriter)");
		}

		[TODO]
		public void Transform(IXPathNavigable input, XsltArgumentList arguments,
								XmlWriter results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(IXPathNavigable, XsltArgumentList, XmlWriter)");
		}

		[TODO]
		public void Transform(String inputUri, XsltArgumentList arguments,
								Stream results)
		{
			if(inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(String, XsltArgumentList, Stream)");
		}

		[TODO]
		public void Transform(String inputUri, XsltArgumentList arguments,
								TextWriter results)
		{
			if(inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(String, XsltArgumentList, TextWriter)");
		}

		[TODO]
		public void Transform(String inputUri, XsltArgumentList arguments,
								XmlWriter results)
		{
			if(inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(String, XsltArgumentList, XmlWriter)");
		}

		[TODO]
		public void Transform(XmlReader input, XsltArgumentList arguments,
								Stream results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(XmlReader, XsltArgumentList, Stream)");
		}

		[TODO]
		public void Transform(XmlReader input, XsltArgumentList arguments,
								TextWriter results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(XmlReader, XsltArgumentList, TextWriter)");
		}

		[TODO]
		public void Transform(XmlReader input, XsltArgumentList arguments,
								XmlWriter results)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(XmlReader, XsltArgumentList, XmlWriter)");
		}

		[TODO]
		public void Transform(XmlReader input, XsltArgumentList arguments,
								XmlWriter results, XmlResolver documentResolver)
		{
			if(input == null)
			{
				throw new ArgumentNullException("input");
			}
			if(results == null)
			{
				throw new ArgumentNullException("results");
			}
			throw new NotImplementedException("Transform(XmlReader, XsltArgumentList, XmlWriter, XmlResolver)");
		}
	}
} //namespace
#endif // CONFIG_XPATH && CONFIG_XSL
#endif // CONFIG_FRAMEWORK_2_0
#endif // !ECMA_COMPAT
