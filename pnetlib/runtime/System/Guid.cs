/*
 * Guid.cs - Implementation of the "System.Guid" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if !ECMA_COMPAT

using System.Runtime.CompilerServices;
using System.Text;

#if CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
#endif
public struct Guid : IFormattable, IComparable
#if CONFIG_FRAMEWORK_2_0
	, IComparable<Guid>, IEquatable<Guid>
#endif
{
	// The empty GUID.
	public static readonly Guid Empty;

	// Internal state.
	private int a__;
	private short b__, c__;
	private byte d__, e__, f__, g__, h__, i__, j__, k__;

	// Create a new GUID, initialized to a unique random value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Guid NewGuid();

	// Helper methods for Guid parsing.
	private static int GetHex(String g, ref int posn, int length, int digits)
			{
				int value = 0;
				char ch;
				while(digits > 0)
				{
					if(posn >= length)
					{
						throw new FormatException(_("Format_GuidValue"));
					}
					ch = g[posn++];
					if(ch >= '0' && ch <= '9')
					{
						value = value * 16 + (int)(ch - '0');
					}
					else if(ch >= 'A' && ch <= 'F')
					{
						value = value * 16 + (int)(ch - 'A' + 10);
					}
					else if(ch >= 'a' && ch <= 'f')
					{
						value = value * 16 + (int)(ch - 'a' + 10);
					}
					else
					{
						throw new FormatException(_("Format_GuidValue"));
					}
					--digits;
				}
				return value;
			}
	private static int GetVarHex(String g, ref int posn, int length, int digits)
			{
				int value = 0;
				char ch;
				bool sawDigit = false;
				if((length - posn) <= 2 || g[posn] != '0' ||
				   (g[posn + 1] != 'x' && g[posn + 1] != 'X'))
				{
					throw new FormatException(_("Format_GuidValue"));
				}
				posn += 2;
				for(;;)
				{
					if(posn >= length)
					{
						break;
					}
					ch = g[posn++];
					if(ch >= '0' && ch <= '9')
					{
						value = value * 16 + (int)(ch - '0');
					}
					else if(ch >= 'A' && ch <= 'F')
					{
						value = value * 16 + (int)(ch - 'A' + 10);
					}
					else if(ch >= 'a' && ch <= 'f')
					{
						value = value * 16 + (int)(ch - 'a' + 10);
					}
					else
					{
						--posn;
						break;
					}
					sawDigit = true;
					--digits;
					if(digits < 0)
					{
						throw new FormatException(_("Format_GuidValue"));
					}
				}
				if(!sawDigit)
				{
					throw new FormatException(_("Format_GuidValue"));
				}
				return value;
			}
	private static void GetChar(String g, ref int posn, int length, char ch)
			{
				if(posn >= length || g[posn] != ch)
				{
					throw new FormatException(_("Format_GuidValue"));
				}
				++posn;
			}

	// Constructors.
	public Guid(String g)
			{
				if(g == null)
				{
					throw new ArgumentNullException("g");
				}
				int posn = 0;
				int length = g.Length;
				if(g[0] == '{')
				{
					++posn;
				}
				if((length - posn) >= 2 && g[posn] == '0' &&
					(g[posn + 1] == 'x' || g[posn + 1] == 'X'))
				{
					if(posn == 0)
					{
						throw new FormatException(_("Format_GuidValue"));
					}
					a__ = (int)GetVarHex(g, ref posn, length, 8);
					GetChar(g, ref posn, length, ',');
					b__ = (short)GetVarHex(g, ref posn, length, 4);
					GetChar(g, ref posn, length, ',');
					c__ = (short)GetVarHex(g, ref posn, length, 4);
					GetChar(g, ref posn, length, ',');
					GetChar(g, ref posn, length, '{');
					d__ = (byte)GetVarHex(g, ref posn, length, 2);
					if(posn < length && g[posn] == '}')
					{
						// The byte values must be individually bracketed.
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						e__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						f__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						g__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						h__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						i__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						j__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
						GetChar(g, ref posn, length, ',');
						GetChar(g, ref posn, length, '{');
						k__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
					}
					else
					{
						// The byte values are not individually bracketed.
						GetChar(g, ref posn, length, ',');
						e__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						f__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						g__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						h__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						i__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						j__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, ',');
						k__ = (byte)GetVarHex(g, ref posn, length, 2);
						GetChar(g, ref posn, length, '}');
					}
				}
				else
				{
					a__ = (int)GetHex(g, ref posn, length, 8);
					GetChar(g, ref posn, length, '-');
					b__ = (short)GetHex(g, ref posn, length, 4);
					GetChar(g, ref posn, length, '-');
					c__ = (short)GetHex(g, ref posn, length, 4);
					GetChar(g, ref posn, length, '-');
					d__ = (byte)GetHex(g, ref posn, length, 2);
					e__ = (byte)GetHex(g, ref posn, length, 2);
					GetChar(g, ref posn, length, '-');
					f__ = (byte)GetHex(g, ref posn, length, 2);
					g__ = (byte)GetHex(g, ref posn, length, 2);
					h__ = (byte)GetHex(g, ref posn, length, 2);
					i__ = (byte)GetHex(g, ref posn, length, 2);
					j__ = (byte)GetHex(g, ref posn, length, 2);
					k__ = (byte)GetHex(g, ref posn, length, 2);
				}
				if(g[0] == '{')
				{
					if(posn >= length || g[posn] != '}')
					{
						throw new FormatException(_("Format_GuidValue"));
					}
					++posn;
				}
				if(posn != length)
				{
					throw new FormatException(_("Format_GuidValue"));
				}
			}
	public Guid(byte[] b)
			{
				if(b == null)
				{
					throw new ArgumentNullException("b");
				}
				if(b.Length != 16)
				{
					throw new ArgumentException(_("Arg_GuidArray16"));
				}
				a__ = ((int)(b[0])) | (((int)(b[1])) << 8) |
					  (((int)(b[2])) << 16) | (((int)(b[3])) << 24);
				b__ = (short)(((int)(b[4])) | (((int)(b[5])) << 8));
				c__ = (short)(((int)(b[6])) | (((int)(b[7])) << 8));
				d__ = b[8];
				e__ = b[9];
				f__ = b[10];
				g__ = b[11];
				h__ = b[12];
				i__ = b[13];
				j__ = b[14];
				k__ = b[15];
			}
	public Guid(int a, short b, short c, byte[] d)
			{
				if(d == null)
				{
					throw new ArgumentNullException("d");
				}
				if(d.Length != 8)
				{
					throw new ArgumentException(_("Arg_GuidArray8"));
				}
				a__ = a;
				b__ = b;
				c__ = c;
				d__ = d[0];
				e__ = d[1];
				f__ = d[2];
				g__ = d[3];
				h__ = d[4];
				i__ = d[5];
				j__ = d[6];
				k__ = d[7];
			}
	public Guid(int a, short b, short c, byte d, byte e, byte f, byte g,
			    byte h, byte i, byte j, byte k)
			{
				a__ = a;
				b__ = b;
				c__ = c;
				d__ = d;
				e__ = e;
				f__ = f;
				g__ = g;
				h__ = h;
				i__ = i;
				j__ = j;
				k__ = k;
			}
	[CLSCompliant(false)]
	public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g,
			    byte h, byte i, byte j, byte k)
			{
				a__ = unchecked((int)a);
				b__ = unchecked((short)b);
				c__ = unchecked((short)c);
				d__ = d;
				e__ = e;
				f__ = f;
				g__ = g;
				h__ = h;
				i__ = i;
				j__ = j;
				k__ = k;
			}

	// Implement the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(!(value is Guid))
					{
						throw new ArgumentException(_("Arg_MustBeGuid"));
					}
					Guid temp = (Guid)value;
					if(((uint)a__) < ((uint)(temp.a__)))
					{
						return -1;
					}
					else if(((uint)a__) > ((uint)(temp.a__)))
					{
						return 1;
					}
					if(((ushort)b__) < ((ushort)(temp.b__)))
					{
						return -1;
					}
					else if(((ushort)b__) > ((ushort)(temp.b__)))
					{
						return 1;
					}
					if(((ushort)c__) < ((ushort)(temp.c__)))
					{
						return -1;
					}
					else if(((ushort)c__) > ((ushort)(temp.c__)))
					{
						return 1;
					}
					if(d__ < temp.d__)
					{
						return -1;
					}
					else if(d__ > temp.d__)
					{
						return 1;
					}
					if(e__ < temp.e__)
					{
						return -1;
					}
					else if(e__ > temp.e__)
					{
						return 1;
					}
					if(f__ < temp.f__)
					{
						return -1;
					}
					else if(f__ > temp.f__)
					{
						return 1;
					}
					if(g__ < temp.g__)
					{
						return -1;
					}
					else if(g__ > temp.g__)
					{
						return 1;
					}
					if(h__ < temp.h__)
					{
						return -1;
					}
					else if(h__ > temp.h__)
					{
						return 1;
					}
					if(i__ < temp.i__)
					{
						return -1;
					}
					else if(i__ > temp.i__)
					{
						return 1;
					}
					if(j__ < temp.j__)
					{
						return -1;
					}
					else if(j__ > temp.j__)
					{
						return 1;
					}
					if(k__ < temp.k__)
					{
						return -1;
					}
					else if(k__ > temp.k__)
					{
						return 1;
					}
					return 0;
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<Guid> interface.
	public int CompareTo(Guid value)
			{
				if(((uint)a__) < ((uint)(value.a__)))
				{
					return -1;
				}
				else if(((uint)a__) > ((uint)(value.a__)))
				{
					return 1;
				}
				if(((ushort)b__) < ((ushort)(value.b__)))
				{
					return -1;
				}
				else if(((ushort)b__) > ((ushort)(value.b__)))
				{
					return 1;
				}
				if(((ushort)c__) < ((ushort)(value.c__)))
				{
					return -1;
				}
				else if(((ushort)c__) > ((ushort)(value.c__)))
				{
					return 1;
				}
				if(d__ < value.d__)
				{
					return -1;
				}
				else if(d__ > value.d__)
				{
					return 1;
				}
				if(e__ < value.e__)
				{
					return -1;
				}
				else if(e__ > value.e__)
				{
					return 1;
				}
				if(f__ < value.f__)
				{
					return -1;
				}
				else if(f__ > value.f__)
				{
					return 1;
				}
				if(g__ < value.g__)
				{
					return -1;
				}
				else if(g__ > value.g__)
				{
					return 1;
				}
				if(h__ < value.h__)
				{
					return -1;
				}
				else if(h__ > value.h__)
				{
					return 1;
				}
				if(i__ < value.i__)
				{
					return -1;
				}
				else if(i__ > value.i__)
				{
					return 1;
				}
				if(j__ < value.j__)
				{
					return -1;
				}
				else if(j__ > value.j__)
				{
					return 1;
				}
				if(k__ < value.k__)
				{
					return -1;
				}
				else if(k__ > value.k__)
				{
					return 1;
				}
				return 0;
			}

#endif // CONFIG_FRAMEWORK_2_0

	// Determine if two Guid objects are equal.
	public override bool Equals(Object obj)
			{
				if(obj is Guid)
				{
					Guid temp = (Guid)obj;
					return (a__ == temp.a__ &&
					        b__ == temp.b__ &&
					        c__ == temp.c__ &&
					        d__ == temp.d__ &&
					        e__ == temp.e__ &&
					        f__ == temp.f__ &&
					        g__ == temp.g__ &&
					        h__ == temp.h__ &&
					        i__ == temp.i__ &&
					        j__ == temp.j__ &&
					        k__ == temp.k__);
				}
				else
				{
					return false;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IEquatable<Guid> interface.
	public bool Equals(Guid obj)
			{
					return (a__ == obj.a__ &&
					        b__ == obj.b__ &&
					        c__ == obj.c__ &&
					        d__ == obj.d__ &&
					        e__ == obj.e__ &&
					        f__ == obj.f__ &&
					        g__ == obj.g__ &&
					        h__ == obj.h__ &&
					        i__ == obj.i__ &&
					        j__ == obj.j__ &&
					        k__ == obj.k__);
			}

#endif // CONFIG_FRAMEWORK_2_0

	// Get a hash code for this Guid object.
	public override int GetHashCode()
			{
				return (a__ ^ ((((int)b__) << 16) | (int)(ushort)c__) ^
						((((int)f__) << 24) | k__));
			}

	// Convert this Guid into a byte array.
	public byte[] ToByteArray()
			{
				byte[] bytes = new byte [16];
				bytes[0]  = (byte)(a__);
				bytes[1]  = (byte)(a__ >> 8);
				bytes[2]  = (byte)(a__ >> 16);
				bytes[3]  = (byte)(a__ >> 24);
				bytes[4]  = (byte)(b__);
				bytes[5]  = (byte)(b__ >> 8);
				bytes[6]  = (byte)(c__);
				bytes[7]  = (byte)(c__ >> 8);
				bytes[8]  = d__;
				bytes[9]  = e__;
				bytes[10] = f__;
				bytes[11] = g__;
				bytes[12] = h__;
				bytes[13] = i__;
				bytes[14] = j__;
				bytes[15] = k__;
				return bytes;
			}

	// Add the hex representation of an integer to a string builder.
	private static void AddHex(StringBuilder builder, int value, int digits)
			{
				int hexdig;
				while(digits > 0)
				{
					--digits;
					hexdig = ((value >> (digits * 4)) & 0x0F);
					if(hexdig < 10)
					{
						builder.Append((char)('0' + hexdig));
					}
					else
					{
						builder.Append((char)('a' + hexdig - 10));
					}
				}
			}

	// Convert this Guid into a string.
	public override String ToString()
			{
				return ToString("D", null);
			}
	public String ToString(String format)
			{
				return ToString(format, null);
			}
	public String ToString(String format, IFormatProvider provider)
			{
				String start, end, sep;
				if(format == "B" || format == "b")
				{
					start = "{";
					end = "}";
					sep = "-";
				}
				else if(format == null || format == "" ||
				        format == "D" || format == "d")
				{
					start = "";
					end = "";
					sep = "-";
				}
				else if(format == "N" || format == "n")
				{
					start = "";
					end = "";
					sep = "";
				}
				else if(format == "P" || format == "p")
				{
					start = "(";
					end = ")";
					sep = "-";
				}
				else
				{
					throw new FormatException(_("Format_Guid"));
				}
				StringBuilder builder = new StringBuilder(38);
				builder.Append(start);
				AddHex(builder, a__, 8);
				builder.Append(sep);
				AddHex(builder, b__, 4);
				builder.Append(sep);
				AddHex(builder, c__, 4);
				builder.Append(sep);
				AddHex(builder, d__, 2);
				AddHex(builder, e__, 2);
				builder.Append(sep);
				AddHex(builder, f__, 2);
				AddHex(builder, g__, 2);
				AddHex(builder, h__, 2);
				AddHex(builder, i__, 2);
				AddHex(builder, j__, 2);
				AddHex(builder, k__, 2);
				builder.Append(end);
				return builder.ToString();
			}

	// Operators.
	public static bool operator==(Guid a, Guid b)
			{
				return (a.a__ == b.a__ &&
				        a.b__ == b.b__ &&
				        a.c__ == b.c__ &&
				        a.d__ == b.d__ &&
				        a.e__ == b.e__ &&
				        a.f__ == b.f__ &&
				        a.g__ == b.g__ &&
				        a.h__ == b.h__ &&
				        a.i__ == b.i__ &&
				        a.j__ == b.j__ &&
				        a.k__ == b.k__);
			}
	public static bool operator!=(Guid a, Guid b)
			{
				return (a.a__ != b.a__ ||
				        a.b__ != b.b__ ||
				        a.c__ != b.c__ ||
				        a.d__ != b.d__ ||
				        a.e__ != b.e__ ||
				        a.f__ != b.f__ ||
				        a.g__ != b.g__ ||
				        a.h__ != b.h__ ||
				        a.i__ != b.i__ ||
				        a.j__ != b.j__ ||
				        a.k__ != b.k__);
			}

}; // struct Guid

#endif // !ECMA_COMPAT

}; // namespace System
