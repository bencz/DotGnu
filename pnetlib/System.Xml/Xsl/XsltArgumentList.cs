/*
 * XsltArgumentList.cs - Implementation of "System.Xml.Xsl.XsltArgumentList" 
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

using System;
using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public
#if !CONFIG_FRAMEWORK_2_0
	sealed
#endif
	class XsltArgumentList
	{
		private Hashtable extensions;
		private Hashtable parameters;

		public XsltArgumentList()
		{
			extensions = new Hashtable ();
			parameters = new Hashtable ();
		}

		public void AddExtensionObject(String namespaceUri, Object extension)
		{
			if(namespaceUri == null)
			{
				throw new ArgumentException("namespaceUri");
			}
			if(namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("namespaceUri");
			}
			if(extensions.Contains (namespaceUri))
			{
				throw new ArgumentException("namespaceUri");
			}
			
			extensions[namespaceUri] = extension;
		}

		public void AddParam(String name, String namespaceUri, Object parameter)
		{
			XmlQualifiedName qName;

			if(namespaceUri == null)
			{
				throw new ArgumentException("namespaceUri");
			}
			if(namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("namespaceUri");
			}
			if(name == null)
			{
				throw new ArgumentException("name");
			}

			qName = new XmlQualifiedName(name, namespaceUri);
			if(parameters.Contains(qName))
			{
				throw new ArgumentException("namespaceUri");
			}
			parameter = ValidateParam(parameter);
			parameters[qName] = parameter;
		}

		public void Clear()
		{
			extensions.Clear();
			parameters.Clear();
		}

		public Object GetExtensionObject(String namespaceUri)
		{
			return extensions[namespaceUri];
		}

		public Object GetParam(String name, String namespaceUri)
		{
			XmlQualifiedName qName;

			if(name == null)
			{
				throw (new ArgumentException("name"));
			}
			qName = new XmlQualifiedName(name, namespaceUri);
			return parameters[qName];
		}

		public Object RemoveExtensionObject(String namespaceUri)
		{
			Object extensionObject = this.GetExtensionObject(namespaceUri);
			extensions.Remove(namespaceUri);
			return extensionObject;
		}

		public Object RemoveParam(String name, String namespaceUri)
		{
			XmlQualifiedName qName = new XmlQualifiedName(name, namespaceUri);
			Object parameter = this.GetParam(name, namespaceUri);
			parameters.Remove(qName);
			return parameter;
		}

		private Object ValidateParam(Object parameter)
		{
			if(parameter is string)
			{
				return parameter;
			}
			if(parameter is bool)
			{
				return parameter;
			}
			if(parameter is double)
			{
				return parameter;
			}
#if CONFIG_XPATH
			if(parameter is XPathNavigator)
			{
				return parameter;
			}
			if(parameter is XPathNodeIterator)
			{
				return parameter;
			}
#endif // CONFIG_XPATH
			if(parameter is Int16)
			{
				return (double)(Int16)parameter;
			}
			if(parameter is UInt16)
			{
				return (double)(UInt16)parameter;
			}
			if(parameter is Int32)
			{
				return (double)(Int32)parameter;
			}
			if(parameter is Int64)
			{
				return (double)(Int64)parameter;
			}
			if(parameter is UInt64)
			{
				return (double)(UInt64)parameter;
			}
			if(parameter is Single)
			{
				return (double)(Single)parameter;
			}
			if(parameter is decimal)
			{
				return (double)(decimal)parameter;
			}
			return parameter.ToString();
		}
	}
}//namespace
#endif
