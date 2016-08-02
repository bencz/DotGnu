/*
 * ResXResourceWriter.cs - Implementation of the
 *			"System.Resources.ResXResourceWriter" class. 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003 Neil Cawse.
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

namespace System.Resources
{

#if !ECMA_COMPAT && CONFIG_SERIALIZATION

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
#if CONFIG_COMPONENT_MODEL
using System.ComponentModel;
#endif

public class ResXResourceWriter : IResourceWriter, IDisposable
{
	// Internal state.
	private Stream stream;
	private String fileName;
	private TextWriter writer;
	private XmlTextWriter xml;
	private bool setup;
	private bool generated;

	// Version of the resource format.
	public static readonly String Version = "1.3";

	public readonly static string BinSerializedObjectMimeType = "application/x-microsoft.net.object.binary.base64";
	public readonly static string ResMimeType = "text/microsoft-resx";
	public readonly static string ByteArraySerializedObjectMimeType = "application/x-microsoft.net.object.bytearray.base64";
	public readonly static string DefaultSerializedObjectMimeType = BinSerializedObjectMimeType;
	public readonly static string SoapSerializedObjectMimeType = "application/x-microsoft.net.object.soap.base64";
	public readonly static string ResourceSchema =
		"\r\n"+
		"    <!-- \r\n    Microsoft ResX Schema Version " + Version + "\r\n"+
		"    -->"+
		"	  <xsd:schema id=\"root\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">"+
		"    <xsd:element name=\"root\" msdata:IsDataSet=\"true\">"+
		"      <xsd:complexType>"+
		"        <xsd:choice maxOccurs=\"unbounded\">"+
		"          <xsd:element name=\"data\">"+
		"            <xsd:complexType>"+
		"              <xsd:sequence>"+
		"                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />"+
		"                <xsd:element name=\"comment\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"2\" />"+
		"              </xsd:sequence>"+
		"              <xsd:attribute name=\"name\" type=\"xsd:string\" msdata:Ordinal=\"1\" />"+
		"              <xsd:attribute name=\"type\" type=\"xsd:string\" msdata:Ordinal=\"3\" />"+
		"              <xsd:attribute name=\"mimetype\" type=\"xsd:string\" msdata:Ordinal=\"4\" />"+
		"            </xsd:complexType>"+
		"          </xsd:element>"+
		"          <xsd:element name=\"resheader\">"+
		"            <xsd:complexType>"+
		"              <xsd:sequence>"+
		"                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />"+
		"              </xsd:sequence>"+
		"              <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" />"+
		"            </xsd:complexType>"+
		"          </xsd:element>"+
		"        </xsd:choice>"+
		"      </xsd:complexType>"+
		"    </xsd:element>"+
		"  </xsd:schema>";

	private BinaryFormatter binaryFormatter = new BinaryFormatter();

	// Constructors.
	public ResXResourceWriter(Stream stream)
			{
				this.stream = stream;
			}
	public ResXResourceWriter(String fileName)
			{
				this.fileName = fileName;
			}
	public ResXResourceWriter(TextWriter writer)
			{
				this.writer = writer;
			}

	// Destructor.
	~ResXResourceWriter()
			{
				Dispose(false);
			}

		public void AddResource(String name, Object value)
			{
				if (!setup)
					Setup();
				if (value == null)
				{
					AddResourceNull(name);
					return;
				}
				Type type = value.GetType();
				if (type == typeof(string))
				{
					AddResource(name, value as String);
					return;
				}
				if (type == typeof(byte[]))
				{
					AddResource(name, (byte[])value);
					return;
				}
				

				xml.WriteStartElement("data");
				xml.WriteAttributeString("name", name);
#if CONFIG_COMPONENT_MODEL
					
				TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
				if (typeConverter.CanConvertTo(typeof(string)) && typeConverter.CanConvertFrom(typeof(string)))
				{
					xml.WriteAttributeString("type", type.AssemblyQualifiedName);
					xml.WriteStartElement("value");
					xml.WriteString(typeConverter.ConvertToInvariantString(value));
				}
				else if (typeConverter.CanConvertTo(typeof(byte[])) && typeConverter.CanConvertFrom(typeof(byte[])))
				{
					string s = FormatString((byte[])typeConverter.ConvertTo(value, typeof(byte[])));
					xml.WriteAttributeString("type", type.AssemblyQualifiedName);
					xml.WriteAttributeString("mimetype", ByteArraySerializedObjectMimeType);
					xml.WriteStartElement("value");
					xml.WriteString(s);
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					binaryFormatter.Serialize(memoryStream, value);
					string s = FormatString(memoryStream.ToArray());
					xml.WriteAttributeString("mimetype", DefaultSerializedObjectMimeType);
					xml.WriteStartElement("value");
					xml.WriteString(s);
				}
				xml.WriteEndElement();
				xml.WriteEndElement();
#endif
			}

	private void AddResourceNull(String name)
			{
				xml.WriteStartElement("data");
				xml.WriteAttributeString("name", name);
				xml.WriteStartElement("value");
				xml.WriteString(String.Empty);
				xml.WriteAttributeString("type", typeof(ResXNullRef).AssemblyQualifiedName);
				xml.WriteEndElement();
				xml.WriteEndElement();
			}
	
	public void AddResource(String name, String value)
			{
				if (!setup)
					Setup();

				xml.WriteStartElement("data");
				xml.WriteAttributeString("name", name);
				xml.WriteStartElement("value");
				xml.WriteString(value);
				xml.WriteEndElement();
				xml.WriteEndElement();
			}
	
	public void AddResource(String name, byte[] value)
			{
				if (!setup)
					Setup();
				
				xml.WriteStartElement("data");
				xml.WriteAttributeString("name", name);
				xml.WriteAttributeString("type", typeof(byte[]).AssemblyQualifiedName);
				xml.WriteStartElement("value");
				xml.WriteString(FormatString(value));
				xml.WriteEndElement();
				xml.WriteEndElement();
			}

	private void Setup()
	{
		if (writer != null)
		{
			writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			xml = new XmlTextWriter(writer);
			xml.Formatting = Formatting.Indented;
		}
		else
		{
			if (fileName == null)
				xml = new XmlTextWriter(stream, Encoding.UTF8);
			else
				xml = new XmlTextWriter(fileName, Encoding.UTF8);
			xml.Formatting = Formatting.Indented;
			xml.WriteStartDocument();
		}
			
		xml.WriteStartElement("root");
		XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(ResourceSchema));
		xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
		xml.WriteNode(xmlTextReader, true);
		xml.WriteStartElement("resheader");
		xml.WriteAttributeString("name", "resmimetype");
		xml.WriteStartElement("value");
		xml.WriteString(ResMimeType);
		xml.WriteEndElement();
		xml.WriteEndElement();
		xml.WriteStartElement("resheader");
		xml.WriteAttributeString("name", "version");
		xml.WriteStartElement("value");
		xml.WriteString(Version);
		xml.WriteEndElement();
		xml.WriteEndElement();
		xml.WriteStartElement("resheader");
		xml.WriteAttributeString("name", "reader");
		xml.WriteStartElement("value");
		xml.WriteString(typeof(ResXResourceReader).AssemblyQualifiedName);
		xml.WriteEndElement();
		xml.WriteEndElement();
		xml.WriteStartElement("resheader");
		xml.WriteAttributeString("name", "writer");
		xml.WriteStartElement("value");
		xml.WriteString(typeof(ResXResourceWriter).AssemblyQualifiedName);
		xml.WriteEndElement();
		xml.WriteEndElement();
		setup = true;
	}

	private static string FormatString(byte[] data)
	{
		string s = Convert.ToBase64String(data);
		if (s.Length <= 80)
			return s;
		StringBuilder stringBuilder = new StringBuilder(s.Length * 110 / 80);
		int i = 0;
		for (; i < s.Length - 80; i += 80)
		{
			stringBuilder.Append("\r\n        ");
			stringBuilder.Append(s, i, 80);
		}
		stringBuilder.Append("\r\n        ");
		stringBuilder.Append(s, i, s.Length - i);
		stringBuilder.Append("\r\n");
		return stringBuilder.ToString();
	}
	
	public void Close()
			{
				Dispose();
			}
	
	public void Generate()
			{
				if (!generated)
				{
					generated = true;
					xml.WriteEndElement();
					xml.Flush();
				}
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (!generated)
						Generate();
					if (xml!= null)
						xml.Close();
					// Not sure, does the writer close the stream?
					if (stream != null)
						stream.Close();
					if (writer != null)
						writer.Close();
				}
			}

}; // class ResXResourceWriter

#endif // !ECMA_COMPAT

}; // namespace System.Resources
