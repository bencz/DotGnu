/*
 * ResXResourceReader.cs - Implementation of the
 *			"System.Resources.ResXResourceReader" class. 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Neil Cawse.
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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

public class ResXResourceReader : IResourceReader, IEnumerable, IDisposable
{
	// Internal state.
	private Stream stream;
	private TextReader reader;
	private String fileName;
	private Hashtable loadedData;
	private String headerMimeType;
	private String headerReader;
	private String headerWriter;
	private String headerVersion;
	private BinaryFormatter binaryFormatter;
#if CONFIG_COMPONENT_MODEL_DESIGN
	private ITypeResolutionService typeResolver;
#endif

	// Constructors.
	private ResXResourceReader()
			{
				binaryFormatter = new BinaryFormatter();
			}
	public ResXResourceReader(Stream stream) : this()
			{
				this.stream = stream;
			}
	public ResXResourceReader(String fileName) : this()
			{
				this.fileName = fileName;
			}
	public ResXResourceReader(TextReader reader) : this()
			{
				this.reader = reader;
			}
#if CONFIG_COMPONENT_MODEL_DESIGN
	public ResXResourceReader(Stream stream,
							  ITypeResolutionService typeResolver)
			: this(stream)
			{
				this.typeResolver = typeResolver;
			}
	public ResXResourceReader(String fileName,
							  ITypeResolutionService typeResolver)
			: this(fileName)
			{
				this.typeResolver = typeResolver;
			}
	public ResXResourceReader(TextReader reader,
							  ITypeResolutionService typeResolver)
			: this(reader)
			{
				this.typeResolver = typeResolver;
			}
#endif

	// Destructor.
	~ResXResourceReader()
			{
				Dispose(false);
			}

	// Implement the IResourceReader interface.
	public void Close()
			{
				((IDisposable)this).Dispose();
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				if(loadedData == null)
				{
					loadedData = new Hashtable();
					XmlTextReader xmlReader;
					TextReader fileReader = null;
					if(fileName != null)
					{
						fileReader = new StreamReader(fileName);
						xmlReader = new XmlTextReader(fileReader);
					}
					else if(stream != null)
					{
						xmlReader = new XmlTextReader(stream);
					}
					else
					{
						xmlReader = new XmlTextReader(reader);
					}
					try
					{
						Load(xmlReader);
					}
					finally
					{
						xmlReader.Close();
						if(fileReader != null)
						{
							fileReader.Close();
						}
					}
				}
				return loadedData.GetEnumerator();
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

	// Dispose of this resource reader.
	protected virtual void Dispose(bool disposing)
			{
				if(stream != null)
				{
					stream.Close();
					stream = null;
				}
				if(reader != null)
				{
					reader.Close();
					reader = null;
				}
			}

	// Load resources from a file contents string.
	public static ResXResourceReader FromFileContents(String fileContents)
			{
				return new ResXResourceReader
					(new StringReader(fileContents));
			}
#if CONFIG_COMPONENT_MODEL_DESIGN
	public static ResXResourceReader FromFileContents
				(String fileContents, ITypeResolutionService typeResolver)
			{
				return new ResXResourceReader
					(new StringReader(fileContents), typeResolver);
			}
#endif

	// Load the contents of an XML resource stream.
	private void Load(XmlTextReader reader)
			{
				reader.WhitespaceHandling = WhitespaceHandling.None;
				
				// Create NameTable
				reader.NameTable.Add("resheader");
				reader.NameTable.Add("resmimetype");
				reader.NameTable.Add("version");
				reader.NameTable.Add("reader");
				reader.NameTable.Add("writer");

				reader.NameTable.Add("data");
				reader.NameTable.Add("name");
				reader.NameTable.Add("type");
				reader.NameTable.Add("mimeType");
				reader.NameTable.Add("value");
				
				reader.NameTable.Add(ResXResourceWriter.BinSerializedObjectMimeType);
				reader.NameTable.Add(ResXResourceWriter.SoapSerializedObjectMimeType);

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						// Deal with the header
						if (reader.LocalName == "resheader")
						{
							switch (reader["name"])
							{
								case "resmimetype":
									headerMimeType = reader.ReadElementString();
									break;
								case "version":
									headerVersion = reader.ReadElementString();
									break;
								case "reader":
									headerReader = reader.ReadElementString();
									break;
								case "writer":
									headerWriter = reader.ReadElementString();
									break;
							}
						}
						// Deal with the data
						else if (reader.LocalName == "data")
						{
							String name = reader["name"];
							String type = reader["type"];
							String mimeType = reader["mimeType"];
							String value;
							if (reader.NodeType == XmlNodeType.Element)
								value = reader.ReadElementString();
							else
								value = reader.Value;
							if (mimeType != null)
								LoadMime(name, type, mimeType, value);
							else if (type != null)
								LoadType(name, type, value);
							else
								loadedData[name] = value;
						}
					}
				}
			}

	private void LoadMime(String name, String type, String mimeType, String value)
	{
		if (mimeType == ResXResourceWriter.ByteArraySerializedObjectMimeType)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(FindType(type));
			if (converter.CanConvertFrom(typeof(byte[])))
				loadedData[name] = converter.ConvertFrom(Convert.FromBase64String(value));
		}
		else
			throw new NotSupportedException(mimeType);
	}

	private void LoadType(String name, String typeName, String value)
	{
		Type type = FindType(typeName);
		if (type == typeof(ResXNullRef))
			loadedData[name] = null;
		else if (typeName.StartsWith("System.Byte[]"))
			loadedData[name] = Convert.FromBase64String(value);
		else
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter.CanConvertFrom(typeof(String)))
				loadedData[name] = converter.ConvertFromInvariantString(value);
		}
	}

	private Type FindType(string typeName)
	{
#if CONFIG_COMPONENT_MODEL_DESIGN
		if (typeResolver == null)
			return  Type.GetType(typeName);
		else
#endif
			return typeResolver.GetType(typeName);
	}

}; // class ResXResourceReader

#endif // !ECMA_COMPAT

}; // namespace System.Resources
