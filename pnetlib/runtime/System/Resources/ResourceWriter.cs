/*
 * ResourceWriter.cs - Implementation of the
 *		"System.Resources.ResourceWriter" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

#if CONFIG_RUNTIME_INFRA

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

#if ECMA_COMPAT
internal
#else
public
#endif
sealed class ResourceWriter : IDisposable, IResourceWriter
{
	// Internal state.
	private Stream stream;
	private bool generateDone;
	private Hashtable table;
	private Hashtable ignoreCaseNames;
	private ArrayList types;
	private TextInfo info;

	// Constructors.
	public ResourceWriter(Stream stream)
			{
				if(stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				else if(!stream.CanWrite)
				{
					throw new ArgumentException
						(_("IO_NotSupp_Write"), "stream");
				}
				this.stream = stream;
				generateDone = false;
				table = new Hashtable();
				ignoreCaseNames = new Hashtable();
				types = new ArrayList();
				info = CultureInfo.InvariantCulture.TextInfo;
			}
	public ResourceWriter(String fileName)
			: this(new FileStream(fileName, FileMode.Create,
								  FileAccess.Write))
			{
				// Nothing to do here.
			}

	// Add a value to this writer.
	private void AddValue(String name, Object value)
			{
				// See if we already have the name.
				String lowerName = info.ToLower(name);
				if(ignoreCaseNames.Contains(lowerName))
				{
					throw new ArgumentException
						(_("Arg_ResourceAlreadyPresent"), "value");
				}

				// Add the name to "table".
				table.Add(name, value);

				// Add the lower-case name to "ignoreCaseNames".
				ignoreCaseNames.Add(lowerName, String.Empty);

				// Add the value's type to the type list.
				if(value != null && !types.Contains(value.GetType()))
				{
					types.Add(value.GetType());
				}
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				Close();
			}

	// Implement the IResourceWriter interface.
	public void AddResource(String name, byte[] value)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(generateDone)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceWriterClosed"));
				}
				else
				{
					AddValue(name, value);
				}
			}
	public void AddResource(String name, Object value)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(generateDone)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceWriterClosed"));
				}
				else
				{
					AddValue(name, value);
				}
			}
	public void AddResource(String name, String value)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(generateDone)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceWriterClosed"));
				}
				else
				{
					AddValue(name, value);
				}
			}
	public void Close()
			{
				if(stream != null)
				{
					if(!generateDone)
					{
						Generate();
					}
					stream.Close();
					stream = null;
				}
			}
	public void Generate()
			{
				if(generateDone)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceWriterClosed"));
				}
				BinaryWriter bw = new BinaryWriter(stream);
				try
				{
					// Write the resource file header.
					bw.Write(ResourceManager.MagicNumber);
					bw.Write(ResourceManager.HeaderVersionNumber);
					MemoryStream mem = new MemoryStream();
					BinaryWriter membw = new BinaryWriter(mem);
					membw.Write("System.Resources.ResourceReader, mscorlib");
					membw.Write
						("System.Resources.RuntimeResourceSet, mscorlib");
					membw.Flush();
					bw.Write((int)(mem.Length));
					bw.Write(mem.GetBuffer(), 0, (int)(mem.Length));

					// Write the resource set header.
					bw.Write(1);			// Resource set version number.
					bw.Write(table.Count);	// Number of resources.

					// Create streams for the name and value sections.
					MemoryStream names = new MemoryStream();
					BinaryWriter namesWriter = new BinaryWriter(names);
					MemoryStream values = new MemoryStream();
					BinaryWriter valuesWriter = new BinaryWriter(values);
					int[] nameHash = new int [table.Count];
					int[] namePosition = new int [table.Count];
					int posn = 0;

					// Process all values in the resource set.
					IDictionaryEnumerator e = table.GetEnumerator();
					while(e.MoveNext())
					{
						// Hash the name and record the name position.
						nameHash[posn] = ResourceReader.Hash((String)(e.Key));
						namePosition[posn] = (int)(names.Position);
						++posn;

						// Write out the name and value index.
						WriteKey(namesWriter, (String)(e.Key));
						namesWriter.Write((int)(values.Position));

						// Write the type table index to the value section.
						Object value = e.Value;
						if(value == null)
						{
							valuesWriter.Write7BitEncoded(-1);
						}
						else
						{
							valuesWriter.Write7BitEncoded
								(types.IndexOf(value.GetType()));
						}

						// Write the value to the value section.
						if(value != null)
						{
							WriteValue(values, valuesWriter, value);
						}
					}

					// Write the type table.
					bw.Write(types.Count);
					foreach(Type t in types)
					{
						bw.Write(t.AssemblyQualifiedName);
					}

					// Align on an 8-byte boundary.
					bw.Flush();
					while((bw.BaseStream.Position & 7) != 0)
					{
						bw.Write((byte)0);
						bw.Flush();
					}

					// Sort the name hash and write it out.
					Array.Sort(nameHash, namePosition);
					foreach(int hash in nameHash)
					{
						bw.Write(hash);
					}
					foreach(int pos in namePosition)
					{
						bw.Write(pos);
					}

					// Write the offset of the value section.
					bw.Flush();
					namesWriter.Flush();
					valuesWriter.Flush();
					bw.Write((int)(bw.BaseStream.Position + names.Length + 4));

					// Write the name and value sections.
					bw.Write(names.GetBuffer(), 0, (int)(names.Length));
					bw.Write(values.GetBuffer(), 0, (int)(values.Length));
					names.Close();
					values.Close();
				}
				finally
				{
					generateDone = true;
					bw.Flush();
					((IDisposable)bw).Dispose();
				}
			}

	// Write a resource value to a stream.
	private void WriteValue(MemoryStream stream, BinaryWriter writer,
							Object value)
			{
				Type type = value.GetType();
				if(type == typeof(String))
				{
					writer.Write((String)value);
				}
				else if(type == typeof(Byte))
				{
					writer.Write((byte)value);
				}
				else if(type == typeof(SByte))
				{
					writer.Write((sbyte)value);
				}
				else if(type == typeof(Int16))
				{
					writer.Write((short)value);
				}
				else if(type == typeof(UInt16))
				{
					writer.Write((ushort)value);
				}
				else if(type == typeof(Int32))
				{
					writer.Write((int)value);
				}
				else if(type == typeof(UInt32))
				{
					writer.Write((uint)value);
				}
				else if(type == typeof(Int64))
				{
					writer.Write((long)value);
				}
				else if(type == typeof(UInt64))
				{
					writer.Write((ulong)value);
				}
			#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(Single))
				{
					writer.Write((float)value);
				}
				else if(type == typeof(Double))
				{
					writer.Write((double)value);
				}
				else if(type == typeof(Decimal))
				{
					int[] bits = Decimal.GetBits((Decimal)value);
					writer.Write(bits[0]);
					writer.Write(bits[1]);
					writer.Write(bits[2]);
					writer.Write(bits[3]);
				}
			#endif
				else if(type == typeof(DateTime))
				{
					writer.Write(((DateTime)value).Ticks);
				}
				else if(type == typeof(TimeSpan))
				{
					writer.Write(((TimeSpan)value).Ticks);
				}
				else
				{
				#if CONFIG_SERIALIZATION
					// Serialize the value with a binary formatter.
					writer.Flush();
					BinaryFormatter formatter;
					formatter = new BinaryFormatter
						(null, new StreamingContext
								(StreamingContextStates.File |
								 StreamingContextStates.Persistence));
					formatter.Serialize(stream, value);
				#endif
				}
			}

	// Write a key value.
	private static void WriteKey(BinaryWriter writer, String key)
			{
				writer.Write7BitEncoded(key.Length * 2);
				foreach(char ch in key)
				{
					writer.Write((ushort)ch);
				}
			}

}; // class ResourceWriter

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Resources
