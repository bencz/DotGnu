/*
 * UnitySerializationHolder.cs - Implementation of the
 *			"System.UnitySerializationHolder" class.
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

namespace System
{

#if CONFIG_SERIALIZATION

using System.Runtime.Serialization;
using System.Reflection;
using System.Private;

// This class is used as a proxy to stand in for singleton objects
// like "System.DBNull", "System.Reflection.Missing", and for special
// types like "Module", "Assembly", etc.  This class must be named
// "System.UnitySerializationHolder" for compatibility with other
// serialization implementations.

internal class UnitySerializationHolder : ISerializable, IObjectReference
{
	// Special constants for the unity types.
	public enum UnityType
	{
		Empty		= 1,
		DBNull		= 2,
		Missing		= 3,
		ClrType		= 4,
		Module		= 5,
		Assembly	= 6

	}; // enum UnityType

	// Internal state.
	private UnityType type;
	private String data;
	private String assembly;

	// Constructor.
	public UnitySerializationHolder(SerializationInfo info,
									StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				type = (UnityType)(info.GetInt32("UnityType"));
				data = info.GetString("Data");
				assembly = info.GetString("AssemblyName");
			}

	// Serialize a unity object.
	public static void Serialize(SerializationInfo info, UnityType type,
								 String data, Assembly assembly)
			{
				info.SetType(typeof(UnitySerializationHolder));
				info.AddValue("data", data);
				info.AddValue("UnityType", (int)type);
				if(assembly != null)
				{
					info.AddValue("AssemblyName", assembly.FullName);
				}
				else
				{
					info.AddValue("AssemblyName", String.Empty);
				}
			}

	// Implement the ISerializable interface.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				// This method should never be called.
				throw new NotSupportedException();
			}

	// Implement the IObjectReference interface.
	public Object GetRealObject(StreamingContext context)
			{
				Assembly assem;
				switch(type)
				{
					case UnityType.Empty:		return Empty.Value;
					case UnityType.DBNull:		return DBNull.Value;
					case UnityType.Missing:		return Missing.Value;

					case UnityType.ClrType:
					{
						if(data == null || data.Length == 0 ||
						   assembly == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						if(assembly == String.Empty)
						{
							return Type.GetType(data);
						}
						assem = FormatterServices.GetAssemblyByName(assembly);
						if(assem == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						Type clrType =
							FormatterServices.GetTypeFromAssembly(assem, data);
						if(clrType != null)
						{
							return clrType;
						}
						throw new SerializationException
							(_("Serialize_StateMissing"));
					}
					// Not reached.

					case UnityType.Module:
					{
						assem = FormatterServices.GetAssemblyByName(assembly);
						if(assem == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						try
						{
							Module module = assem.GetModule(data);
							if(module == null)
							{
								throw new SerializationException
									(_("Serialize_StateMissing"));
							}
							return module;
						}
						catch(Exception)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
					}
					// Not reached.

					case UnityType.Assembly:
					{
						assem = FormatterServices.GetAssemblyByName(data);
						if(assem == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						return assem;
					}
					// Not reached.

					default:
						throw new ArgumentException
							(_("Arg_InvalidUnityObject"));
				}
			}

}; // class UnitySerializationHolder

#endif // CONFIG_SERIALIZATION

}; // namespace System
