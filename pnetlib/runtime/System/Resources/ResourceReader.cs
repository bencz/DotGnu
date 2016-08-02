/*
 * ResourceReader.cs - Implementation of the
 *		"System.Resources.ResourceReader" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

#if ECMA_COMPAT
internal
#else
public
#endif
sealed class ResourceReader : IEnumerable, IDisposable, IResourceReader
{
	// Internal state.
	private Stream stream;
	private int numStrings;
	private int[] nameHash;
	private int[] namePosn;
	private long nameStart;
	private long dataStart;
	internal Type[] types;

	// Constructors.
	public ResourceReader(Stream stream)
			{
				if(stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				else if(!stream.CanRead)
				{
					throw new ArgumentException
						(_("IO_NotSupp_Read"), "stream");
				}
				else if(!stream.CanSeek)
				{
					throw new ArgumentException
						(_("IO_NotSupp_Seek"), "stream");
				}
				this.stream = stream;
				if(!ReadHeader())
				{
					numStrings = 0;
				}
			}
	public ResourceReader(String fileName)
			: this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				// Nothing to do here.
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				if(stream == null)
				{
					throw new InvalidOperationException
						(_("IO_StreamClosed"));
				}
				return new ResourceEnumerator(this);
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Close();
			}

	// Implement the IResourceReader interface.
	public void Close()
			{
				if(stream != null)
				{
					stream.Close();
					stream = null;
				}
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				if(stream == null)
				{
					throw new InvalidOperationException
						(_("IO_StreamClosed"));
				}
				return new ResourceEnumerator(this);
			}

	// Read an unsigned integer value from a buffer.
	private static uint ReadUInt(byte[] buf, int offset)
			{
				return ((uint)(buf[offset])) |
				       (((uint)(buf[offset + 1])) << 8) |
				       (((uint)(buf[offset + 2])) << 16) |
				       (((uint)(buf[offset + 3])) << 24);
			}

	// Read a integer value byte by byte.  Returns -1 if invalid.
	private static int ReadInt(Stream stream)
			{
				byte [] data = new byte[4];
				if(stream.Read(data, 0, 4) != 4)
				{
					return -1;
				}
				int value = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
				return value;
			}

	// Read a compact length value from a stream.  Returns -1 if invalid.
	private static int ReadLength(Stream stream)
			{
				int byteval = stream.ReadByte();
				int value;
				if(byteval == -1)
				{
					return -1;
				}
				value = (byteval & 0x7F);
				int shift = 7;
				while((byteval & 0x80) != 0)
				{
					byteval = stream.ReadByte();
					if(byteval == -1)
					{
						return -1;
					}
					value |= ((byteval & 0x7F) << shift);
					shift += 7;
				}
				if(value < 0)
				{
					return -1;
				}
				else
				{
					return value;
				}
			}

	// Read a string value from a stream.  Returns null if invalid.
	private static String ReadString(Stream stream)
			{
				int length = ReadLength(stream);
				if(length < 0)
				{
					return null;
				}
				else if(length == 0)
				{
					return String.Empty;
				}
				byte[] buf = new byte [length];
				if(stream.Read(buf, 0, length) != length)
				{
					return null;
				}
				return Encoding.UTF8.GetString(buf, 0, length);
			}

	// Read a Unicode string value from a stream.  Returns null if invalid.
	private static String ReadUnicodeString(Stream stream)
			{
				int length = ReadLength(stream);
				if(length < 0)
				{
					return null;
				}
				byte[] buf = new byte [length];
				if(stream.Read(buf, 0, length) != length)
				{
					return null;
				}
				return Encoding.Unicode.GetString(buf, 0, length);
			}

	// Convert a string name into a type.
	private static Type StringToType(String name)
			{
				if(name == null)
				{
					return null;
				}
				if(name == "System.String" || name.StartsWith("System.String,"))
				{
					// This is the most common case that we will encounter.
					return typeof(String);
				}
			#if CONFIG_REFLECTION
				return Type.GetType(name);
			#else
				return null;
			#endif
			}

	// Read the resource stream header.  Returns false if
	// the header was invalid in some way.
	private bool ReadHeader()
			{
				byte[] header = new byte [12];
				uint numTypes;
				long start = stream.Position;
				int posn;

				// Read the primary part of the header and validate it.
				if(stream.Read(header, 0, 12) != 12 ||
				   ReadUInt(header, 0) !=
				   		(uint)(ResourceManager.MagicNumber) ||
				   ReadUInt(header, 4) !=
				   		(uint)(ResourceManager.HeaderVersionNumber))
				{
					return false;
				}

				// Skip past the class names.
				stream.Seek((long)(ReadUInt(header, 8)), SeekOrigin.Current);

				// Read the secondary part of the header.
				if(stream.Read(header, 0, 12) != 12 ||
				   ReadUInt(header, 0) != (uint)1)
				{
					return false;
				}
				numStrings = (int)(ReadUInt(header, 4));
				if(numStrings < 0)
				{
					return false;
				}

				// Read the type table.
				numTypes = ReadUInt(header, 8);
				types = new Type [(int)numTypes];
				posn = 0;
				while(numTypes > 0)
				{
					types[posn++] = StringToType(ReadString(stream));
					--numTypes;
				}

				// Align on an 8-byte boundary.
				long current = stream.Position - start;
				if((current % 8) != 0)
				{
					stream.Seek(8 - (current % 8), SeekOrigin.Current);
				}

				// Read the name hash table into memory.
				nameHash = new int [numStrings];
				for(posn = 0; posn < numStrings; ++posn)
				{
					nameHash[posn] = ReadInt(stream);
				}

				// Read the name position table into memory.
				namePosn = new int [numStrings];
				for(posn = 0; posn < numStrings; ++posn)
				{
					namePosn[posn] = ReadInt(stream);
				}

				// Read the offset of the data section.
				if(stream.Read(header, 0, 4) != 4)
				{
					return false;
				}
				dataStart = start + (long)(ReadUInt(header, 0));

				// We have found the start of the name section.
				nameStart = stream.Position;

				// Ready to go.
				return true;
			}

	// Hash a resource name to a resource hash value.
	internal static int Hash(String name)
			{
				int len = name.Length;
				int posn;
				int hash = 0x1505;
				for(posn = 0; posn < len; ++posn)
				{
					hash = ((hash << 5) + hash) ^ (int)(name[posn]);
				}
				return hash;
			}

#if CONFIG_EXTENDED_NUMERICS

	// Read floating-point values.
	private static float ReadSingle(Stream stream)
			{
				int value;
				byte[] buf = new byte [4];
				stream.Read(buf, 0, 4);
				if(BitConverter.IsLittleEndian)
				{
					value = ((int)(buf[0])) |
					        (((int)(buf[1])) << 8) |
					        (((int)(buf[2])) << 16) |
					        (((int)(buf[3])) << 24);
				}
				else
				{
					value = ((int)(buf[3])) |
					        (((int)(buf[2])) << 8) |
					        (((int)(buf[1])) << 16) |
					        (((int)(buf[0])) << 24);
				}
				return BitConverter.Int32BitsToFloat(value);
			}
	private static double ReadDouble(Stream stream)
			{
				long value;
				byte[] buf = new byte [8];
				stream.Read(buf, 0, 8);
				if(BitConverter.IsLittleEndian)
				{
					value = ((long)(buf[0])) |
						    (((long)(buf[1])) << 8) |
						    (((long)(buf[2])) << 16) |
						    (((long)(buf[3])) << 24) |
						    (((long)(buf[4])) << 32) |
						    (((long)(buf[5])) << 40) |
						    (((long)(buf[6])) << 48) |
						    (((long)(buf[7])) << 56);
				}
				else
				{
					value = ((long)(buf[7])) |
						    (((long)(buf[6])) << 8) |
						    (((long)(buf[5])) << 16) |
						    (((long)(buf[4])) << 24) |
						    (((long)(buf[3])) << 32) |
						    (((long)(buf[2])) << 40) |
						    (((long)(buf[1])) << 48) |
						    (((long)(buf[0])) << 56);
				}
				return BitConverter.Int64BitsToDouble(value);
			}

#endif // CONFIG_EXTENDED_NUMERICS

	// Read an object value of a particular type.
	private static Object ReadObject(Stream stream, Type type)
			{
				int byteval;
				long temp;

				// Handle the simple type cases first.
				if(type == null)
				{
					return null;
				}
				if(type == typeof(Byte))
				{
					return (byte)(stream.ReadByte());
				}
				else if(type == typeof(SByte))
				{
					return (sbyte)(stream.ReadByte());
				}
				else if(type == typeof(Int16))
				{
					byteval = stream.ReadByte();
					return (short)(byteval | (stream.ReadByte() << 8));
				}
				else if(type == typeof(UInt16))
				{
					byteval = stream.ReadByte();
					return (ushort)(byteval | (stream.ReadByte() << 8));
				}
				else if(type == typeof(Int32))
				{
					return ReadInt(stream);
				}
				else if(type == typeof(UInt32))
				{
					return (uint)(ReadInt(stream));
				}
				else if(type == typeof(Int64))
				{
					temp = (long)(ReadInt(stream));
					return (temp | (((long)ReadInt(stream)) << 32));
				}
				else if(type == typeof(UInt64))
				{
					temp = (long)(ReadInt(stream));
					return (ulong)(temp | (((long)ReadInt(stream)) << 32));
				}
			#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(Single))
				{
					return ReadSingle(stream);
				}
				else if(type == typeof(Double))
				{
					return ReadDouble(stream);
				}
				else if(type == typeof(Decimal))
				{
					int[] bits = new int [4];
					bits[0] = ReadInt(stream);
					bits[1] = ReadInt(stream);
					bits[2] = ReadInt(stream);
					bits[3] = ReadInt(stream);
					return new Decimal(bits);
				}
			#endif
				else if(type == typeof(DateTime))
				{
					temp = (long)(ReadInt(stream));
					return new DateTime(temp | (((long)ReadInt(stream)) << 32));
				}
				else if(type == typeof(TimeSpan))
				{
					temp = (long)(ReadInt(stream));
					return new TimeSpan(temp | (((long)ReadInt(stream)) << 32));
				}

			#if CONFIG_SERIALIZATION
				// De-serialize the value with a binary formatter.
				BinaryFormatter formatter;
				formatter = new BinaryFormatter
					(null, new StreamingContext
							(StreamingContextStates.File |
							 StreamingContextStates.Persistence));
				formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				Object obj = formatter.Deserialize(stream);
				if(obj != null && obj.GetType() == type)
				{
					return obj;
				}
			#endif

				// Don't know how to de-serialize, so return null.
				return null;
			}

	// Look up a resource object by name.  This is a short-cut
	// that "ResourceSet" can use to perform quicker lookups in
	// the common case of internal assembly string resources.
	internal Object GetObject(String name)
			{
				int hash = Hash(name);
				int left, right, middle;
				String test;
				int typeCode;

				// Search for the hash value using a binary search.
				left = 0;
				right = numStrings - 1;
				while(left <= right)
				{
					middle = left + ((right - left) / 2);
					if(hash < nameHash[middle])
					{
						right = middle - 1;
					}
					else if(hash > nameHash[middle])
					{
						left = middle + 1;
					}
					else
					{
						left = middle;
						break;
					}
				}
				if(left > right)
				{
					return null;
				}

				// Find the left-most and right-most extent of strings
				// that hash to the same value.
				right = left;
				while(left > 0 && nameHash[left - 1] == hash)
				{
					--left;
				}
				while(right < (numStrings - 1) && nameHash[right + 1] == hash)
				{
					++right;
				}

				// Scan all strings with the same hash for a name match.
				while(left <= right)
				{
					stream.Seek(nameStart + namePosn[left], SeekOrigin.Begin);
					test = ReadUnicodeString(stream);
					if(test != null && test.Equals(name))
					{
						// We have found a name match: fetch the value.
						int valuePosn = ReadInt(stream);
						if(valuePosn == -1)
						{
							return null;
						}
						stream.Seek(dataStart + valuePosn, SeekOrigin.Begin);
						typeCode = ReadLength(stream);
						if(typeCode < 0 || typeCode >= types.Length)
						{
							return null;
						}
						if(types[typeCode] == typeof(String))
						{
							// This is the most common case.
							return ReadString(stream);
						}
						else
						{
							return ReadObject(stream, types[typeCode]);
						}
					}
					++left;
				}

				// There are no strings with an equal name.
				return null;
			}

	// Private enumerator class for resource readers.
	private sealed class ResourceEnumerator
				: IEnumerator, IDictionaryEnumerator
	{
		// Internal state.
		private ResourceReader reader;
		private int posn;
		private String key;
		private Object value;

		// Constructor.
		public ResourceEnumerator(ResourceReader reader)
				{
					this.reader = reader;
					this.posn = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					int typeCode;
					Stream stream = reader.stream;
					if(stream == null)
					{
						throw new InvalidOperationException
							(_("IO_StreamClosed"));
					}
					++posn;
					if(posn < reader.numStrings)
					{
						stream.Seek(reader.nameStart + reader.namePosn[posn],
									SeekOrigin.Begin);
						key = ReadUnicodeString(stream);
						if(key == null)
						{
							throw new InvalidOperationException
								(_("IO_ReadFailed"));
						}
						int valuePosn = ReadInt(stream);
						if(valuePosn == -1)
						{
							throw new InvalidOperationException
								(_("IO_ReadFailed"));
						}
						stream.Seek(reader.dataStart + (long)valuePosn,
									SeekOrigin.Begin);
						typeCode = ReadLength(stream);
						if(typeCode < 0 || typeCode >= reader.types.Length)
						{
							value = null;
						}
						else if(reader.types[typeCode] == typeof(String))
						{
							// This is the most common case.
							value = ReadString(stream);
						}
						else
						{
							value = ReadObject(stream, reader.types[typeCode]);
						}
						if(value == null)
						{
							throw new InvalidOperationException
								(_("IO_ReadFailed"));
						}
						return true;
					}
					else
					{
						--posn;
						return false;
					}
				}
		public void Reset()
				{
					posn = -1;
				}
		public Object Current
				{
					get
					{
						return Entry;
					}
				}

		// Implement the IDictionaryEnumerator interface.
		public DictionaryEntry Entry
				{
					get
					{
						if(posn >= 0 && posn < reader.numStrings)
						{
							return new DictionaryEntry(key, value);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}
		public Object Key
				{
					get
					{
						if(posn >= 0 && posn < reader.numStrings)
						{
							return key;
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}
		public Object Value
				{
					get
					{
						if(posn >= 0 && posn < reader.numStrings)
						{
							return value;
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	}; // class ResourceEnumerator

}; // class ResourceReader

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Resources
