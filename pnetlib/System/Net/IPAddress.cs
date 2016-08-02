/*
 * IPAddress.cs - Implementation of the "System.Net.IPAddress" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
 *
 * Contributions by Gerard Toonstra <toonstra@ntlworld.com>
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

namespace System.Net
{

using System;
using System.Text;
using System.Net.Sockets;
using System.Globalization;
using System.Runtime.CompilerServices;

// Note: the IPv6 stuff isn't ECMA-compatible, strictly speaking,
// but it is very useful to have it available in ECMA environments.

public class IPAddress
{
	// Internal state.
	private AddressFamily family;
	private long value;
	internal ushort[] ipv6;

	// Predefined addresses.
	public static readonly IPAddress Any = new IPAddress(0x0000000000000000);
	public static readonly IPAddress Broadcast =
			new IPAddress((long)(uint)HostToNetworkOrder
							(unchecked((int)0xFFFFFFFF)));
	public static readonly IPAddress Loopback =
			new IPAddress((long)(uint)HostToNetworkOrder(0x7F000001));
	public static readonly IPAddress None = Broadcast;
	public static readonly IPAddress IPv6Any =
			new IPAddress(new byte [16], 0);
	public static readonly IPAddress IPv6Loopback =
			new IPAddress(new byte[] {0, 0, 0, 0, 0, 0, 0, 0,
									  0, 0, 0, 0, 0, 0, 0, 1}, 0);
	public static readonly IPAddress IPv6None =
			new IPAddress(new byte [16], 0);

	// Constructors.
	public IPAddress(long newAddress)
			{
				if((newAddress < 0) || (newAddress > 0x00000000FFFFFFFF))
				{
					throw new ArgumentOutOfRangeException
						("newAddress", S._("Arg_OutOfRange") + " " + newAddress.ToString("x"));
				}
				this.family = AddressFamily.InterNetwork;
				this.value = newAddress;
				this.ipv6 = null;
			}
	public IPAddress(byte[] address) : this(address, 0) {}
	public IPAddress(byte[] address, long scopeid)
			{
				if(address == null)
				{
					throw new ArgumentNullException("address");
				}
				else if(address.Length != 16)
				{
					throw new ArgumentException
						(S._("Arg_InvalidIPv6Address"));
				}
				if(scopeid < 0 || scopeid > (long)(UInt32.MaxValue))
				{
					throw new ArgumentException
						(S._("Arg_InvalidIPv6Scope"));
				}
				this.family = AddressFamily.InterNetworkV6;
				this.value = scopeid;
				this.ipv6 = new ushort [8];
				int posn;
				for(posn = 0; posn < 8; ++posn)
				{
					this.ipv6[posn] =
						(ushort)((address[posn * 2] << 8) |
								 (address[posn * 2 + 1]));
				}
			}
	private IPAddress(ushort[] address, long scopeid)
			{
				this.family = AddressFamily.InterNetworkV6;
				this.value = scopeid;
				this.ipv6=new ushort[8];
				Array.Copy(address,this.ipv6,8);
			}
	
	// Determine if two objects are equal.
	public override bool Equals(Object comparand)
			{
				IPAddress other = (comparand as IPAddress);
				if(other != null)
				{
					if(family != other.family || value != other.value)
					{
						return false;
					}
					if(ipv6 != null)
					{
						int posn;
						for(posn = 0; posn < 8; ++posn)
						{
							if(ipv6[posn] != other.ipv6[posn])
							{
								return false;
							}
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get the bytes of this address.
	public byte[] GetAddressBytes()
			{
				byte[] buf;
				if(family == AddressFamily.InterNetwork)
				{
					int host = NetworkToHostOrder((int)value);
					buf = new byte [4];
					buf[0] = (byte)(host >> 24);
					buf[1] = (byte)(host >> 16);
					buf[2] = (byte)(host >> 8);
					buf[3] = (byte)host;
					return buf;
				}
				else
				{
					buf = new byte [16];
					int posn;
					for(posn = 0; posn < 8; ++posn)
					{
						buf[posn * 2] = (byte)(ipv6[posn] >> 8);
						buf[posn * 2 + 1] = (byte)(ipv6[posn]);
					}
					return buf;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode() 
			{
				if(family == AddressFamily.InterNetwork)
				{
					return unchecked(((int)value) & 0x7FFFFFFF);
				}
				else
				{
					int hash = 0;
					int posn;
					for(posn = 0; posn < 8; ++posn)
					{
						hash = (hash << 5) + hash + ipv6[posn];
					}
					return unchecked((hash ^ (int)value) & 0x7FFFFFFF);
				}
			}

	// Convert from host to network order.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long HostToNetworkOrder(long host);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int HostToNetworkOrder(int host);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static short HostToNetworkOrder(short host);

	// Determine if an address corresponds to the loopback interface.
	public static bool IsLoopback(IPAddress address)
			{
				if(address.family == AddressFamily.InterNetwork)
				{
					long mask = (long)(uint)HostToNetworkOrder(0x7F000000);
					return ((address.value & mask) == mask);
				}
				else
				{
					return address.Equals(IPv6Loopback);
				}
			}

	// Convert from network to host order.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long NetworkToHostOrder(long network);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int NetworkToHostOrder(int network);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static short NetworkToHostOrder(short network);

	private static IPAddress ParseIPv4(String ipString)
			{
				int parsed;
				String[]  tokenizedString;
				int quadA;
				int quadB;
				int quadC;
				int quadD;
				
				// this only takes char[]. not String
				tokenizedString = ipString.Split(new char[]{'.'}, 4);

				if (tokenizedString.Length < 4)
				{
					throw new FormatException(S._("Format_IP"));
				}

				try
				{
					quadA = Byte.Parse(tokenizedString[0]);
					quadB = Byte.Parse(tokenizedString[1]);
					quadC = Byte.Parse(tokenizedString[2]);
					quadD = Byte.Parse(tokenizedString[3]);
				}
				catch(OverflowException)
				{
					throw new FormatException(S._("Format_IP"));
				}

				parsed=	((quadD)) ;
				parsed|=((quadC << 8)  & 0xFF00);
				parsed|=((quadB << 16) & 0xFF0000);
				parsed|=((quadA << 24) & unchecked((int)0xFF000000));

				return new IPAddress((long)(uint)HostToNetworkOrder(parsed));
			}
	
	// IPv6 Parsing
	
	private static ushort[] ParseHex(String input)
			{
				if(input==String.Empty)
				{
					return new ushort[0];
				}
				String [] toks = input.Split(':');
				ushort [] retval = new ushort[toks.Length];
				for(int i=0; i< toks.Length; i++)
				{
					try
					{
						retval[i] =
							UInt16.Parse(toks[i],NumberStyles.HexNumber);
					}
					catch(OverflowException)
					{
						throw new FormatException(S._("Format_IP"));
					}
				}
				return retval;
			}
		
	private static ushort[] ParseNoCompress(String input)
			{
				if(input==String.Empty)
				{
					return new ushort[0];
				}
				int ipv4Start=input.IndexOf(".");
				int lastPart = input.LastIndexOf(":");
				String ip4 = null;
				ushort [] retval;
				if(ipv4Start!=-1 && lastPart!=-1)
				{
					// F00F::0:13.2.3.4
					if(lastPart > ipv4Start)
					{
						throw new FormatException(S._("Format_IP"));
					}
					ip4 = input.Substring(input.LastIndexOf(":")+1);
					input = input.Substring(0, input.LastIndexOf(":"));
				}
				else if(ipv4Start!=-1 && lastPart == -1)
				{
					// F00F::13.2.2.3
					ip4 = input.Substring(input.LastIndexOf(":")+1);
					input = String.Empty;
				}
				ushort[] hex4 = ParseHex(input);
				
				retval = new ushort[hex4.Length + ((ip4!=null) ? 2 : 0)];
				
				Array.Copy(hex4,retval,hex4.Length);

				if(ip4!=null)
				{
					long ipValue = ParseIPv4(ip4).value; 
					retval [hex4.Length] = (ushort) 
										(((int) (ipValue & 0xff) << 8) + 
										((int) ((ipValue >> 8) & 0xff)));
												
					retval [hex4.Length + 1] = (ushort) 
									(((int) ((ipValue >> 16) & 0xff) << 8)
									+ ((int) ((ipValue >> 24) & 0xff)));
				}
				return retval;
			}
	
	private static IPAddress ParseIPv6(String input)
			{
				long scopeid=0;
				bool zeroCompress, multipleCompress ;

				if(input==null)
				{
					throw new ArgumentNullException("input");
				}
				if(input.IndexOf('/')!=-1)
				{
					String prefix=input.Substring(input.IndexOf('/')+1);
					try
					{
						scopeid=Int64.Parse(prefix);
					}
					catch
					{
						throw new FormatException("Invalid prefix");
					}
					input=input.Substring(0,input.IndexOf('/'));
				}
				
				zeroCompress = (input.IndexOf("::") != -1);
				multipleCompress = 
							(input.IndexOf("::") != input.LastIndexOf("::"));
				if(!zeroCompress)
				{
					ushort[] retval=ParseNoCompress(input);
					if(retval.Length != 8)
					{
						throw new FormatException(S._("Format_IP"));
					}
					return new IPAddress(retval,scopeid);
				}
				else if(!multipleCompress)
				{
					String part1=input.Substring(0,input.IndexOf("::"));
					String part2=input.Substring(input.IndexOf("::")+2);

					// Parse the two peices independently 
					ushort[] retval1=ParseNoCompress(part1);
					ushort[] retval2=ParseNoCompress(part2);
					
					if((retval1.Length + retval2.Length) >= 8)
						throw new FormatException(S._("Format_IP"));

					ushort[] retval = new ushort[8]{0,0,0,0,0,0,0,0};

					// fill in the peices from either end , leaving the
					// zero block to remain between.

					Array.Copy(retval1, retval, retval1.Length);
					Array.Copy(retval2, 0, retval, 8-retval2.Length, 
													retval2.Length);
					return new IPAddress(retval,scopeid);
				}
				throw new FormatException(S._("Format_IP"));
			}

	public static IPAddress Parse(String ipString)
			{
				if (ipString == null)
				{
					throw new ArgumentNullException
						("ipString", S._("Arg_NotNull"));
				}
				if(ipString.IndexOf(":")!=-1)
				{
					return ParseIPv6(ipString);
				}
				else
				{
					return ParseIPv4(ipString);
				}
			}	
	
	public override string ToString()
			{
				if(family==AddressFamily.InterNetworkV6)
				{
					StringBuilder sb=new StringBuilder(8*5);
					for(int i=0;i<7;i++)
					{
						sb.Append(ipv6[i].ToString("X4"));
						sb.Append('.');
					}
					sb.Append(ipv6[7].ToString("X4"));
					return sb.ToString();
				}
				else
				{
					int host = NetworkToHostOrder((int)value);
					return ((host >> 24) & 0xFF).ToString() + "." +
						   ((host >> 16) & 0xFF).ToString() + "." +
						   ((host >> 8) & 0xFF).ToString() + "." +
						   (host & 0xFF).ToString();
				}
			}

	// Get or set the IPv4 address.
	[Obsolete("IPAddress.Address is address family dependant, use Equals method for comparison.")]
	public long Address
			{
				get
				{
					if(family == AddressFamily.InterNetwork)
					{
						return value;
					}
					else
					{
						throw new SocketException();
					}
				}
				set
				{
					if(family != AddressFamily.InterNetwork)
					{
						throw new SocketException();
					}
					if((value < 0) || (value > 0x00000000FFFFFFFF))
					{
						throw new ArgumentOutOfRangeException
							("newAddress", S._("Arg_OutOfRange"));
					}
					this.value = value;
				}
			}

	// Get or set the IPv6 scope identifier.
	public long ScopeId
			{
				get
				{
					if(family == AddressFamily.InterNetworkV6)
					{
						return value;
					}
					else
					{
						throw new SocketException();
					}
				}
				set
				{
					if(family != AddressFamily.InterNetworkV6)
					{
						throw new SocketException();
					}
					if((value < 0) || (value > 0x00000000FFFFFFFF))
					{
						throw new ArgumentOutOfRangeException
							("newAddress", S._("Arg_OutOfRange"));
					}
					this.value = value;
				}
			}

	// Get the address family of this IP address.
	public AddressFamily AddressFamily
			{
				get
				{
					return family;
				}
			}
	
}; // class IPAddress

}; // namespace System.Net
