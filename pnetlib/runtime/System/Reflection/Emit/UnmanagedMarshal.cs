/*
 * UnmanagedMarshal.cs - Implementation of the
 *		"System.Reflection.Emit.UnmanagedMarshal" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * 
 * Contributions from by Gopal.V <gopalv82@symonds.net> 
 *                       Rhys Weatherley <rweather@southern-storm.com.au>
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.Text;
using System.Runtime.InteropServices;

public sealed class UnmanagedMarshal
{
	// Internal state.
	private UnmanagedType type;
	private UnmanagedType baseType;
	private int elemCount;
	private string marshalCookie;
	private string marshalType;


	// Constructor.
	private UnmanagedMarshal(UnmanagedType type)
			{
				this.type = type;
			}
	private UnmanagedMarshal(UnmanagedType type, int elemCount)
			{
				this.type = type;
				this.elemCount = elemCount;
			}
	private UnmanagedMarshal(UnmanagedType type, UnmanagedType baseType)
			{
				this.type = type;
				this.elemCount = elemCount;
			}

	// Define a by-value array type.
	public static UnmanagedMarshal DefineByValArray(int elemCount)
			{
				return new UnmanagedMarshal
					(UnmanagedType.ByValArray, elemCount);
			}

	// Define a by-value TSTR type.
	public static UnmanagedMarshal DefineByValTStr(int elemCount)
			{
				return new UnmanagedMarshal
					(UnmanagedType.ByValTStr, elemCount);
			}

	// Define an LP array type.
	public static UnmanagedMarshal DefineLPArray(UnmanagedType elemType)
			{
				return new UnmanagedMarshal(UnmanagedType.LPArray, elemType);
			}

	// Define a safe array.
	public static UnmanagedMarshal DefineSafeArray(UnmanagedType elemType)
			{
				return new UnmanagedMarshal(UnmanagedType.SafeArray, elemType);
			}

	// Define a simple unmanaged marshalling behaviour.
	public static UnmanagedMarshal DefineUnmanagedMarshal
				(UnmanagedType unmanagedType)
			{
				// Must be a simple unmanaged type.
				if(unmanagedType == UnmanagedType.ByValArray ||
				   unmanagedType == UnmanagedType.ByValTStr ||
				   unmanagedType == UnmanagedType.LPArray ||
				   unmanagedType == UnmanagedType.SafeArray ||
				   unmanagedType == UnmanagedType.CustomMarshaler)
				{
					throw new ArgumentException
						(_("Emit_NotSimpleUnmanagedType"));
				}
				return new UnmanagedMarshal(unmanagedType);
			}

	// Define a custom-marshaler. This is an extension to MS.NET
	public static UnmanagedMarshal DefineCustom
			(Type typeRef, string cookie, string type, Guid guid)
		{
			UnmanagedMarshal custom =
				new UnmanagedMarshal(UnmanagedType.CustomMarshaler);

			if (type == null || type.Length == 0)
				custom.marshalType = typeRef.AssemblyQualifiedName;
			else
				custom.marshalType = type;
			// Should throw an exception if neither typeRef or type given

			custom.marshalCookie = cookie;

			// guid is ignored

			return custom;
		}

	// Get the base type for marshalling behaviour.
	public UnmanagedType BaseType 
			{
				get
				{
					if(type == UnmanagedType.LPArray ||
					   type == UnmanagedType.SafeArray)
					{
						return baseType;
					}
					else
					{
						throw new ArgumentException
							(_("Emit_NoUnmanagedBaseType"));
					}
				}
			}

	// Get the number of elements in an array type.
	public int ElementCount 
			{
				get
				{
					if(type == UnmanagedType.ByValArray ||
					   type == UnmanagedType.ByValTStr)
					{
						return elemCount;
					}
					else
					{
						throw new ArgumentException
							(_("Emit_NoUnmanagedElementCount"));
					}
				}
			}

	// Get the primary unmanaged type code for marshalling behaviour.
	public UnmanagedType GetUnmanagedType 
			{
				get
				{
					return type;
				}
			}

	// Get the GUID information for custom marshalling.
	public Guid IIDGuid 
			{
				get
				{
					if(type == UnmanagedType.CustomMarshaler)
					{
						// It is not actually possible to set the GUID
						// through any of the API's, so it will always
						// be the empty GUID.
						return Guid.Empty;
					}
					else
					{
						throw new ArgumentException(_("Emit_NotCustom"));
					}
				}
			}

	// Convert this object into an array of marshalling bytes.
	internal byte[] ToBytes()
			{
				byte[] bytes;
				switch(type)
				{
					case UnmanagedType.ByValArray:
					case UnmanagedType.ByValTStr:
					{
						if(elemCount < 0x80)
						{
							bytes = new byte [2];
							bytes[0] = (byte)type;
							bytes[1] = (byte)elemCount;
						}
						else if(elemCount < 0x4000)
						{
							bytes = new byte [3];
							bytes[0] = (byte)type;
							bytes[1] = (byte)((elemCount >> 8) | 0x80);
							bytes[2] = (byte)(elemCount & 0xFF);
						}
						else
						{
							bytes = new byte [5];
							bytes[0] = (byte)type;
							bytes[1] = (byte)((elemCount >> 24) | 0x80);
							bytes[2] = (byte)((elemCount >> 16) | 0x80);
							bytes[3] = (byte)((elemCount >> 8) | 0x80);
							bytes[4] = (byte)(elemCount & 0xFF);
						}
					}
					break;

					case UnmanagedType.LPArray:
					case UnmanagedType.SafeArray:
					{
						bytes = new byte [2];
						bytes[0] = (byte)type;
						bytes[1] = (byte)baseType;
					}
					break;

					case UnmanagedType.CustomMarshaler:
					{
						// At a minimum, need space for type and 4 length-bytes
						int size = 5;
						int marshalTypeSize = Encoding.UTF8.GetByteCount(marshalType);
						size += marshalTypeSize;
						if (marshalCookie != null && marshalCookie.Length > 0)
							size += Encoding.UTF8.GetByteCount(marshalCookie);

						bytes = new byte[size];
						bytes[0] = (byte)type;
						bytes[1] = 0; // Empty guid string
						bytes[2] = 0; // Empty native string
						bytes[3] = (byte) Encoding.UTF8.GetBytes(marshalType,
							0, marshalType.Length, bytes, 4);
						bytes[marshalTypeSize+4] = (byte) Encoding.UTF8.GetBytes(marshalCookie,
							0, marshalCookie.Length, bytes, marshalTypeSize + 5);
						break;
					}

					default:
					{
						bytes = new byte [1];
						bytes[0] = (byte)type;
					}
					break;
				}
				return bytes;
			}

}; // class UnmanagedMarshal

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
