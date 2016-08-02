/*
 * XmlSchemaObject.cs - Implementation of "System.Xml.Schema.XmlSchemaObject" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * contributed by Gopal.V 
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
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaObject
	{
		[TODO]
		protected XmlSchemaObject()
		{
			 throw new NotImplementedException(".ctor");
		}

		[TODO]
		public int LineNumber 
		{
 			get
			{
				throw new NotImplementedException("LineNumber");
			}
 			set
			{
				throw new NotImplementedException("LineNumber");
			}
 		}

		[TODO]
		public int LinePosition 
		{
 			get
			{
				throw new NotImplementedException("LinePosition");
			}
 			set
			{
				throw new NotImplementedException("LinePosition");
			}
 		}

		[TODO]
		public XmlSerializerNamespaces Namespaces 
		{
 			get
			{
				throw new NotImplementedException("Namespaces");
			}
 			set
			{
				throw new NotImplementedException("Namespaces");
			}
 		}

		[TODO]
		public String SourceUri 
		{
 			get
			{
				throw new NotImplementedException("SourceUri");
			}
 			set
			{
				throw new NotImplementedException("SourceUri");
			}
 		}

	}
}//namespace
