/*
 * Hash.cs - Implementation of the
 *		"System.Security.Policy.Hash" class.
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

namespace System.Security.Policy
{

#if CONFIG_CRYPTO && CONFIG_POLICY_OBJECTS

using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;

[Serializable]
public sealed class Hash
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Internal state.
	private Assembly assembly;
	private byte[] md5;
	private byte[] sha1;
	private byte[] dataToHash;

	// Constructor.
	public Hash(Assembly assembly)
			{
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				this.assembly = assembly;
			}
#if CONFIG_SERIALIZATION
	internal Hash(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				dataToHash = (byte[])info.GetValue("RawData", typeof(byte[]));
			}
#endif

	// Get the MD5 hash value for the assembly.
	public byte[] MD5
			{
				get
				{
					if(md5 == null)
					{
						md5 = GenerateHash(Cryptography.MD5.Create());
					}
					return md5;
				}
			}

	// Get the SHA1 hash value for the assembly.
	public byte[] SHA1
			{
				get
				{
					if(sha1 == null)
					{
						sha1 = GenerateHash(Cryptography.SHA1.Create());
					}
					return sha1;
				}
			}

	// Get the raw data to be hashed.
	private byte[] RawData
			{
				get
				{
					// Strong names not supported in this implementation.
					throw new NotSupportedException();
				}
			}

	// Generate the hash value for this assembly using a given algorith.
	public byte[] GenerateHash(HashAlgorithm hashAlg)
			{
				if(hashAlg == null)
				{
					throw new ArgumentNullException("hashAlg");
				}
				byte[] rawData = RawData;
				if(rawData == null)
				{
					return null;
				}
				hashAlg.Initialize();
				return hashAlg.ComputeHash(rawData);
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerialization interface.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("RawData", RawData, typeof(byte[]));
			}

#endif

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element;
				element = new SecurityElement("System.Security.Policy.Hash");
				element.AddAttribute("version", "1");
				byte[] rawData = RawData;
				if(rawData != null && rawData.Length != 0)
				{
					element.AddChild(new SecurityElement
						("RawData", StrongNamePublicKeyBlob.ToHex(rawData)));
				}
				return element.ToString();
			}

}; // class Hash

#endif // CONFIG_CRYPTO && CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
