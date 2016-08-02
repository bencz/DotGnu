/*
 * QDataStream.cs - Qt-like data stream interface.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Dcop
{

using System;
using System.IO;
using System.Text;

// This class provides a counterpart for the "QDataStream" class in the
// "Qt" widget set, to allow DCOP messages to be serialized and de-serialized.
// It has just enough functionality to do DCOP from C#, so it isn't a
// general-purpose replacement for the C++ version of "QDataStream".

public class QDataStream
{
	// Internal state.
	private Stream stream;

	// Constructor.
	public QDataStream(Stream stream)
			{
				if(stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				this.stream = stream;
			}
	public QDataStream(byte[] buffer, int offset, int count)
			{
				stream = new MemoryStream(buffer, offset, count, false);
			}

	// Factory
	public static QDataStream Marshal(Stream stream, DcopFunction fun, Object[] parameters)
	{
		if(stream == null)
		{
			throw new ArgumentNullException("stream", "Argument cannot be null");
		}

		if(fun == null)
		{
			throw new ArgumentNullException("fun", "Argument cannot be null");
		}

		QDataStream s = new QDataStream(stream);
		try
		{
			for(int i = 0; i < fun.Length; i++)
			{
				switch (fun[i])
				{
					case "bool": s.Write((bool)parameters[i]); break;
					// FIXME: this assumes that sizeof(our int) == sizeof(dcop client int)
					// ASSUME makes ASS of yoU and ME :(
					case "int": s.Write((int)parameters[i]); break;

					case "Q_UINT32": s.Write((uint)parameters[i]); break; // Much better here! 32 bit is already 32 bit.
					case "QString": s.WriteUnicode(parameters[i] as string); break; // FIXME: is this correct?
					case "QStringList": s.WriteUnicode(parameters[i] as string[]); break;
					case "QCString": s.Write(parameters[i] as string); break; // FIXME: this again requires testing
					case "QCStringList": s.Write(parameters[i] as string[]); break;
//					case "QValueList<QCString>": s.WriteStringList(parameters[i] as string[]); break;
//					case "QValueList<DCOPRef>": s.WriteDcopRefList(parameters[i] as DcopRef[]); break;
				}
			}
		}
		catch (InvalidCastException ice)
		{
			throw new DcopNamingException("Failed to cast parameters", ice);
		}
		return s;
	}

	public Object ReadObject(string objType)
	{
		if((objType == null) || (objType.Length == 0))
		{
			return null;
		}

		try
		{
			// Simpe types
			switch(objType)
			{
				case "void":
				case "ASYNC": // FIXME: This is just BS, but for now...
					return null;
				case "int":
					return (int)ReadInt32(); // FIXME: This is just plain wrong
				case "Q_UINT32":
					return (uint)ReadUInt32();
				case "QString":
					return (string)ReadUnicodeString();
				case "QStringList":
					return (string[])ReadUnicodeStringList();
				case "QCString":
					return (string)ReadString();
				case "QCStringList":
					return (string[])ReadStringList();
				case "DCOPRef":
					return (DcopRef)ReadDcopRef();
				case "serviceResult":
					return (ServiceResult)ReadServiceResult();
			}
			// QSTL
			if(objType.StartsWith("QValueList<") && objType.EndsWith(">"))
			{
				string newType = objType.Substring(objType.IndexOf('<') + 1, objType.Length - (objType.IndexOf('<') + 2));
				return (Object[])ReadValueList(newType);
			}
		}
		catch(InvalidCastException ice)
		{ // FIXME: is not it redundant?
			throw new DcopNamingException("Failed to cast parameters", ice);
		}
		return null;
	}

	// Get the base stream that underlies this data stream.
	public Stream BaseStream
			{
				get
				{
					return stream;
				}
			}

	// Write values to a QDataStream.
	public void Write(bool value)
			{
				stream.WriteByte((byte)(value ? 1 : 0));
			}
	public void Write(byte value)
			{
				stream.WriteByte(value);
			}
	[CLSCompliant(false)]
	public void Write(sbyte value)
			{
				stream.WriteByte((byte)value);
			}
	public void Write(short value)
			{
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	[CLSCompliant(false)]
	public void Write(ushort value)
			{
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	public void Write(char value)
			{
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	public void Write(int value)
			{
				stream.WriteByte((byte)(value >> 24));
				stream.WriteByte((byte)(value >> 16));
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	[CLSCompliant(false)]
	public void Write(uint value)
			{
				stream.WriteByte((byte)(value >> 24));
				stream.WriteByte((byte)(value >> 16));
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	public void Write(long value)
			{
				stream.WriteByte((byte)(value >> 56));
				stream.WriteByte((byte)(value >> 48));
				stream.WriteByte((byte)(value >> 40));
				stream.WriteByte((byte)(value >> 32));
				stream.WriteByte((byte)(value >> 24));
				stream.WriteByte((byte)(value >> 16));
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	[CLSCompliant(false)]
	public void Write(ulong value)
			{
				stream.WriteByte((byte)(value >> 56));
				stream.WriteByte((byte)(value >> 48));
				stream.WriteByte((byte)(value >> 40));
				stream.WriteByte((byte)(value >> 32));
				stream.WriteByte((byte)(value >> 24));
				stream.WriteByte((byte)(value >> 16));
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}
	public void WriteUnicode(String value)
			{
				if(value == null)
				{
					Write(-1);
				}
				else
				{
					Write(value.Length * 2);
					foreach(char ch in value)
					{
						stream.WriteByte((byte)(ch >> 8));
						stream.WriteByte((byte)ch);
					}
				}
			}
	public void Write(String value)
			{
				if(value == null)
				{
					Write(0);
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(value);
					Write(bytes, 0, bytes.Length, true);
				}
			}
	public void Write(string[] value)
			{
				if(value == null)
				{
					Write((int)0);
				}
				else
				{
					Write((int)value.Length);
					for(int i = 0; i < value.Length; i++)
					{
						Write(value[i]);
					}
				}
			}
	public void WriteUnicode(string[] value)
			{
				if(value == null)
				{
					Write((int)0);
				}
				else
				{
					Write((int)value.Length);
					for(int i = 0; i < value.Length; i++)
					{
						WriteUnicode(value[i]);
					}
				}
			}
	public void Write(IQDataStreamable value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				else
				{
					value.Write(this);
				}
			}
	public void Write(byte[] buffer, int offset, int count, bool addNull)
			{
				if(addNull)
				{
					Write(count + 1);
				}
				else
				{
					Write(count);
				}
				stream.Write(buffer, offset, count);
				if(addNull)
				{
					stream.WriteByte(0);
				}
			}
	public void Write(byte[] buffer, int offset, int count)
			{
				Write(buffer, offset, count, false);
			}
	public void Write(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				Write(buffer, 0, buffer.Length, false);
			}
	public void WriteRaw(byte[] buffer, int offset, int count)
			{
				stream.Write(buffer, offset, count);
			}
	public void WriteRaw(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				WriteRaw(buffer, 0, buffer.Length);
			}
	public void Write(Object value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				else if(value is IQDataStreamable)
				{
					Write((IQDataStreamable)value);
				}
				else if(value is bool)
				{
					Write((bool)value);
				}
				else if(value is byte)
				{
					Write((byte)value);
				}
				else if(value is sbyte)
				{
					Write((sbyte)value);
				}
				else if(value is short)
				{
					Write((short)value);
				}
				else if(value is ushort)
				{
					Write((ushort)value);
				}
				else if(value is int)
				{
					Write((int)value);
				}
				else if(value is uint)
				{
					Write((uint)value);
				}
				else if(value is long)
				{
					Write((long)value);
				}
				else if(value is ulong)
				{
					Write((ulong)value);
				}
				else if(value is String)
				{
					Write((String)value);
				}
				else if(value is byte[])
				{
					Write((byte[])value);
				}
				else
				{
					// Don't know how to serialize this type of value.
					throw new ArgumentException();
				}
			}

	// Read values from a QDataStream.
	public bool ReadBoolean()
			{
				return (stream.ReadByte() > 0);
			}
	public byte ReadByte()
			{
				return (byte)(stream.ReadByte());
			}
	[CLSCompliant(false)]
	public sbyte ReadSByte()
			{
				return (sbyte)(stream.ReadByte());
			}
	public short ReadInt16()
			{
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();
				return (short)((b1 << 8) | b2);
			}
	[CLSCompliant(false)]
	public ushort ReadUInt16()
			{
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();
				return (ushort)((b1 << 8) | b2);
			}
	public char ReadChar()
			{
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();
				return (char)((b1 << 8) | b2);
			}
	public int ReadInt32()
			{
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();
				int b3 = stream.ReadByte();
				int b4 = stream.ReadByte();
				return ((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
			}
	[CLSCompliant(false)]
	public uint ReadUInt32()
			{
				int b1 = stream.ReadByte();
				int b2 = stream.ReadByte();
				int b3 = stream.ReadByte();
				int b4 = stream.ReadByte();
				return (uint)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
			}
	public long ReadInt64()
			{
				int high = ReadInt32();
				uint low = ReadUInt32();
				return (((long)high) << 32) | (long)low;
			}
	[CLSCompliant(false)]
	public ulong ReadUInt64()
			{
				uint high = ReadUInt32();
				uint low = ReadUInt32();
				return (((ulong)high) << 32) | (ulong)low;
			}
	public String ReadString()
			{
				int len = ReadInt32();
				if(len < 0)
				{
					return null;
				}
				else if(len == 0)
				{
					return String.Empty;
				}
				byte[] buf = new byte [len - 1];
				stream.Read(buf, 0, len - 1);
				stream.ReadByte();
				return Encoding.UTF8.GetString(buf); // FIXME: will this work for native charsets?
			}
	public string[] ReadStringList()
			{
				int len = ReadInt32();
				if(len < 0)
				{
					return null;
				}
				string[] list = new string[len];
				for (int i = 0; i < len; i++)
				{
					list[i] = ReadString();
				}
				return list;
			}

	public String ReadUnicodeString()
			{
				int len = ReadInt32();
				if(len < 0)
				{
					return null;
				}
				else if((uint)len == 0xffffffff)
				{
					return null;
				}
				else if(len == 0)
				{
					return String.Empty;
				}
				char[] buf = new char [len];
				int posn, b1, b2;
				for(posn = 0; posn < len; ++posn)
				{
					// FIXME: Endianness
					b1 = stream.ReadByte();
					b2 = stream.ReadByte();
					buf[posn] = (char)((b1 << 8) | b2);
				}
				stream.ReadByte();
				return new String(buf);
			}
	public string[] ReadUnicodeStringList()
			{
				int len = ReadInt32();
				if(len < 0)
				{
					return null;
				}
				string[] list = new string[len];
				for (int i = 0; i < len; i++)
				{
					list[i] = ReadUnicodeString();
				}
				return list;
			}
	public DcopRef ReadDcopRef()
			{
				string app = ReadString();
				string obj = ReadString();
				string type = ReadString();
				return new DcopRef(app, obj);
			}

	public ServiceResult ReadServiceResult()
			{
				ServiceResult val = new ServiceResult();
				val.Result = ReadInt32();
				val.DcopName = ReadString();
				val.ErrorMessage = ReadUnicodeString();
				// Will not touch pid now
				return val;
			}
	public Object[] ReadValueList(string type)
			{
				int length = ReadInt32();
				Object[] res = new Object[length];
				for(int i = 0; i < length; i++)
				{
					res[i] = ReadObject(type);
				}
				return res;
			}

	public void Read(IQDataStreamable obj)
			{
				if(obj != null)
				{
					obj.Read(this);
				}
			}

}; // class QDataStream

}; // namespace Xsharp.Dcop
