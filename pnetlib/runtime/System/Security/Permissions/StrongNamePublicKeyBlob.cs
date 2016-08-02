/*
 * StrongNamePublicKeyBlob.cs - Implementation of the
 *		"System.Security.Permissions.StrongNamePublicKeyBlob" class.
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

namespace System.Security.Permissions
{

#if CONFIG_POLICY_OBJECTS && !ECMA_COMPAT

using System;
using System.Text;
using System.Security;

public sealed class StrongNamePublicKeyBlob
{

	// Internal state.
	private byte[] blob;

	// Constructor.
	public StrongNamePublicKeyBlob(byte[] publicKey)
			{
				if(publicKey == null)
				{
					throw new ArgumentNullException("publicKey");
				}
				else if(publicKey.Length == 0)
				{
					throw new ArgumentException(_("Arg_PublicKeyBlob"));
				}
				else
				{
					blob = (byte[])(publicKey.Clone());
				}
			}
	internal StrongNamePublicKeyBlob(String publicKey)
			{
				blob = FromHex(publicKey);
			}

	// Convert a hex string into a byte buffer.
	internal static byte[] FromHex(String s)
			{
				byte[] blob;
				if(s != null)
				{
					blob = new byte [s.Length / 2];
					int posn;
					int value;
					char ch;
					for(posn = 0; posn < s.Length; posn += 2)
					{
						ch = s[posn];
						if(ch >= '0' && ch <= '9')
						{
							value = (ch - '0') << 4;
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							value = (ch - 'A' + 10) << 4;
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							value = (ch - 'a' + 10) << 4;
						}
						else
						{
							throw new ArgumentException(_("Arg_PublicKeyBlob"));
						}
						ch = s[posn + 1];
						if(ch >= '0' && ch <= '9')
						{
							value += (ch - '0');
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							value += (ch - 'A' + 10);
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							value += (ch - 'a' + 10);
						}
						else
						{
							throw new ArgumentException(_("Arg_PublicKeyBlob"));
						}
						blob[posn / 2] = (byte)value;
					}
				}
				else
				{
					blob = new byte [0];
				}
				return blob;
			}

	// Convert a byte buffer into a hex string.
	internal static String ToHex(byte[] buffer)
			{
				if(buffer == null)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder();
				int posn;
				for(posn = 0; posn < buffer.Length; ++posn)
				{
					BitConverter.AppendHex(builder, buffer[posn]);
				}
				return builder.ToString();
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				StrongNamePublicKeyBlob key = (obj as StrongNamePublicKeyBlob);
				if(key != null && blob.Length == key.blob.Length)
				{
					int posn;
					for(posn = 0; posn < blob.Length; ++posn)
					{
						if(blob[posn] != key.blob[posn])
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				int hash = 0;
				int posn;
				for(posn = 0; posn < blob.Length; ++posn)
				{
					hash = (hash << 5) + hash + blob[posn];
				}
				return hash;
			}

	// Convert this public key into a string.
	public override String ToString()
			{
				return ToHex(blob);
			}

}; // class StrongNamePublicKeyBlob

#endif // CONFIG_POLICY_OBJECTS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
