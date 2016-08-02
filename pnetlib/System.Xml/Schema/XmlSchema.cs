/*
 * XmlSchema.cs - Implementation of "XmlSchema" class
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

namespace System.Xml.Schema
{
	public class XmlSchema: XmlSchemaObject
	{
	
		public const String InstanceNamespace="http://www.w3.org/2001/XMLSchema-instance";
		public const String Namespace="http://www.w3.org/2001/XMLSchema";
		[TODO]
		public XmlSchema()
		{
			 throw new NotImplementedException(".ctor");
		}

		[TODO]
		public void Compile(ValidationEventHandler handler)
		{
			 throw new NotImplementedException("Compile");
		}

		[TODO]
		public static XmlSchema Read(TextReader reader, 
							ValidationEventHandler validationEventHandler)
		{
			 throw new NotImplementedException("Read");
		}

		[TODO]
		public static XmlSchema Read(Stream stream, 
							ValidationEventHandler validationEventHandler)
		{
			 throw new NotImplementedException("Read");
		}

		[TODO]
		public static XmlSchema Read(XmlReader rdr, 
							ValidationEventHandler validationEventHandler)
		{
			 throw new NotImplementedException("Read");
		}

		[TODO]
		public void Write(Stream stream)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public void Write(TextWriter writer)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public void Write(XmlWriter writer)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public void Write(Stream stream, XmlNamespaceManager namespaceManager)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public void Write(TextWriter writer, XmlNamespaceManager namespaceManager)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public void Write(XmlWriter writer, XmlNamespaceManager namespaceManager)
		{
			 throw new NotImplementedException("Write");
		}

		[TODO]
		public XmlSchemaForm AttributeFormDefault 
		{
 			get
			{
				throw new NotImplementedException("AttributeFormDefault");
			}
 			set
			{
				throw new NotImplementedException("AttributeFormDefault");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable AttributeGroups 
		{
 			get
			{
				throw new NotImplementedException("AttributeGroups");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable Attributes 
		{
 			get
			{
				throw new NotImplementedException("Attributes");
			}
 		}

		[TODO]
		public XmlSchemaDerivationMethod BlockDefault 
		{
 			get
			{
				throw new NotImplementedException("BlockDefault");
			}
 			set
			{
				throw new NotImplementedException("BlockDefault");
			}
 		}

		[TODO]
		public XmlSchemaForm ElementFormDefault 
		{
 			get
			{
				throw new NotImplementedException("ElementFormDefault");
			}
 			set
			{
				throw new NotImplementedException("ElementFormDefault");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable Elements 
		{
 			get
			{
				throw new NotImplementedException("Elements");
			}
 		}

		[TODO]
		public XmlSchemaDerivationMethod FinalDefault 
		{
 			get
			{
				throw new NotImplementedException("FinalDefault");
			}
 			set
			{
				throw new NotImplementedException("FinalDefault");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable Groups 
		{
 			get
			{
				throw new NotImplementedException("Groups");
			}
 		}

		[TODO]
		public String Id 
		{
 			get
			{
				throw new NotImplementedException("Id");
			}
 			set
			{
				throw new NotImplementedException("Id");
			}
 		}

		[TODO]
		public XmlSchemaObjectCollection Includes 
		{
 			get
			{
				throw new NotImplementedException("Includes");
			}
 		}

		[TODO]
		public bool IsCompiled 
		{
 			get
			{
				throw new NotImplementedException("IsCompiled");
			}
 		}

		[TODO]
		public XmlSchemaObjectCollection Items 
		{
 			get
			{
				throw new NotImplementedException("Items");
			}
 		}

		[TODO]
		public String Language 
		{
 			get
			{
				throw new NotImplementedException("Language");
			}
 			set
			{
				throw new NotImplementedException("Language");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable Notations 
		{
 			get
			{
				throw new NotImplementedException("Notations");
			}
 		}

		[TODO]
		public XmlSchemaObjectTable SchemaTypes 
		{
 			get
			{
				throw new NotImplementedException("SchemaTypes");
			}
 		}

		[TODO]
		public String TargetNamespace 
		{
 			get
			{
				throw new NotImplementedException("TargetNamespace");
			}
 			set
			{
				throw new NotImplementedException("TargetNamespace");
			}
 		}

		[TODO]
		public XmlAttribute[] UnhandledAttributes 
		{
 			get
			{
				throw new NotImplementedException("UnhandledAttributes");
			}
 			set
			{
				throw new NotImplementedException("UnhandledAttributes");
			}
 		}

		[TODO]
		public String Version 
		{
 			get
			{
				throw new NotImplementedException("Version");
			}
 			set
			{
				throw new NotImplementedException("Version");
			}
 		}

	}
}//namespace
