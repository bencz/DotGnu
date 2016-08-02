/*
 * SerializationInfo.cs - Implementation of the
 *			"System.Runtime.Serialization.SerializationInfo" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

using System.Collections;

public sealed class SerializationInfo
{
	// Internal state.
	private IFormatterConverter converter;
	private String assemblyName;
	private String fullTypeName;
	internal ArrayList names;
	internal ArrayList values;
	internal ArrayList types;
	internal int generation;

	// Constructor.
	[CLSCompliant(false)]
	public SerializationInfo(Type type, IFormatterConverter converter)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				if(converter == null)
				{
					throw new ArgumentNullException("converter");
				}
				this.converter = converter;
				this.assemblyName = type.Assembly.FullName;
				this.fullTypeName = type.FullName;
				this.names = new ArrayList();
				this.values = new ArrayList();
				this.types = new ArrayList();
				this.generation = 0;
			}

	// Get or set the assembly name to serialize.
	public String AssemblyName
			{
				get
				{
					return assemblyName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					assemblyName = value;
				}
			}

	// Get or set the full name of the type to serialize.
	public String FullTypeName
			{
				get
				{
					return fullTypeName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					fullTypeName = value;
				}
			}

	// Get the number of members that have been added to this object.
	public int MemberCount
			{
				get
				{
					return values.Count;
				}
			}

	// Add values to this serialization information object.
	public void AddValue(String name, bool value)
			{
				AddValue(name, (Object)value, typeof(Boolean));
			}
	public void AddValue(String name, byte value)
			{
				AddValue(name, (Object)value, typeof(Byte));
			}
	[CLSCompliant(false)]
	public void AddValue(String name, sbyte value)
			{
				AddValue(name, (Object)value, typeof(SByte));
			}
	public void AddValue(String name, short value)
			{
				AddValue(name, (Object)value, typeof(Int16));
			}
	[CLSCompliant(false)]
	public void AddValue(String name, ushort value)
			{
				AddValue(name, (Object)value, typeof(UInt16));
			}
	public void AddValue(String name, char value)
			{
				AddValue(name, (Object)value, typeof(Char));
			}
	public void AddValue(String name, int value)
			{
				AddValue(name, (Object)value, typeof(Int32));
			}
	[CLSCompliant(false)]
	public void AddValue(String name, uint value)
			{
				AddValue(name, (Object)value, typeof(UInt32));
			}
	public void AddValue(String name, long value)
			{
				AddValue(name, (Object)value, typeof(Int64));
			}
	[CLSCompliant(false)]
	public void AddValue(String name, ulong value)
			{
				AddValue(name, (Object)value, typeof(UInt64));
			}
	public void AddValue(String name, float value)
			{
				AddValue(name, (Object)value, typeof(Single));
			}
	public void AddValue(String name, double value)
			{
				AddValue(name, (Object)value, typeof(Double));
			}
	public void AddValue(String name, DateTime value)
			{
				AddValue(name, (Object)value, typeof(DateTime));
			}
	public void AddValue(String name, Decimal value)
			{
				AddValue(name, (Object)value, typeof(Decimal));
			}
	internal void AddValue(String name, String value)
			{
				AddValue(name, (Object)value, typeof(String));
			}
	public void AddValue(String name, Object value)
			{
				AddValue(name, (Object)value, value.GetType());
			}
	public void AddValue(String name, Object value, Type type)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				if(value != null && !type.IsAssignableFrom(value.GetType()))
				{
					throw new SerializationException
						(_("Serialize_BadType"));
				}
				if(names.IndexOf(name) != -1)
				{
					throw new SerializationException
						(_("Serialize_AlreadyPresent"));
				}
				++generation;
				names.Add(name);
				values.Add(value);
				types.Add(type);
			}

	// Get a value from this serialization information object.
	public bool GetBoolean(String name)
			{
				return (bool)GetValue(name, typeof(Boolean));
			}
	public byte GetByte(String name)
			{
				return (byte)GetValue(name, typeof(Byte));
			}
	[CLSCompliant(false)]
	public sbyte GetSByte(String name)
			{
				return (sbyte)GetValue(name, typeof(SByte));
			}
	public short GetInt16(String name)
			{
				return (short)GetValue(name, typeof(Int16));
			}
	[CLSCompliant(false)]
	public ushort GetUInt16(String name)
			{
				return (ushort)GetValue(name, typeof(UInt16));
			}
	public char GetChar(String name)
			{
				return (char)GetValue(name, typeof(Char));
			}
	public int GetInt32(String name)
			{
				return (int)GetValue(name, typeof(Int32));
			}
	[CLSCompliant(false)]
	public uint GetUInt32(String name)
			{
				return (uint)GetValue(name, typeof(UInt32));
			}
	public long GetInt64(String name)
			{
				return (long)GetValue(name, typeof(Int64));
			}
	[CLSCompliant(false)]
	public ulong GetUInt64(String name)
			{
				return (ulong)GetValue(name, typeof(UInt64));
			}
	public float GetSingle(String name)
			{
				return (float)GetValue(name, typeof(Single));
			}
	public double GetDouble(String name)
			{
				return (double)GetValue(name, typeof(Double));
			}
	public DateTime GetDateTime(String name)
			{
				return (DateTime)GetValue(name, typeof(DateTime));
			}
	public Decimal GetDecimal(String name)
			{
				return (Decimal)GetValue(name, typeof(Decimal));
			}
	public String GetString(String name)
			{
				return (String)GetValue(name, typeof(String));
			}
	public Object GetValue(String name, Type type)
			{
				int index;
				Object value;
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				index = names.IndexOf(name);
				if(index != -1)
				{
					value = values[index];
					if(value != null)
					{
						if(type.IsAssignableFrom((Type)(types[index])))
						{
							return value;
						}
						else
						{
							return converter.Convert(value, type);
						}
					}
				}
				return null;
			}

	// Get values from this info object, while ignoring case
	// during name matches.
	internal String GetStringIgnoreCase(String name)
			{
				return (String)GetValueIgnoreCase(name, typeof(String));
			}
	internal Object GetValueIgnoreCase(String name, Type type)
			{
				int index;
				Object value;
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				for(index = 0; index < names.Count; ++index)
				{
					if(String.Compare((String)(names[index]), name, true) == 0)
					{
						value = values[index];
						if(value != null)
						{
							if(type.IsAssignableFrom((Type)(types[index])))
							{
								return value;
							}
							else
							{
								return converter.Convert(value, type);
							}
						}
					}
				}
				return null;
			}

	// Get an enumerator for iterating over this information object.
	public SerializationInfoEnumerator GetEnumerator()
			{
				return new SerializationInfoEnumerator(this);
			}

	// Set the serialization type associated with this object.
	public void SetType(Type type)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				assemblyName = type.Assembly.FullName;
				fullTypeName = type.FullName;
			}

}; // class SerializationInfo

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
