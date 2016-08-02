/*
 * StrongNameKeyPair.cs - Implementation of the
 *		"System.Reflection.StrongNameKeyPair" class.
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

namespace System.Reflection
{

#if !ECMA_COMPAT

using System;
using System.IO;

public class StrongNameKeyPair
{
	// Internal state.
	private byte[] publicKey;
	private byte[] keyPairArray;
	private String container;

	// Constructors.
	public StrongNameKeyPair(byte[] keyPairArray)
			{
				if(keyPairArray == null)
				{
					throw new ArgumentNullException("keyPairArray");
				}
				this.keyPairArray = (byte[])(keyPairArray.Clone());
			}
	public StrongNameKeyPair(FileStream keyPairFile)
			{
				if(keyPairFile == null)
				{
					throw new ArgumentNullException("keyPairFile");
				}
				int size = (int)(keyPairFile.Length);
				keyPairArray = new byte [size];
				keyPairFile.Read(keyPairArray, 0, size);
			}
	public StrongNameKeyPair(String keyPairContainer)
			{
				if(keyPairContainer == null)
				{
					throw new ArgumentNullException("keyPairContainer");
				}
				container = keyPairContainer;
			}

	// Convert a key pair array into just the public key portion.
	private static byte[] KeyPairToPublicKey(byte[] pair)
			{
				// Validate the header.
				if(pair.Length < 16)
				{
					return null;
				}
				if(pair[0]  != 0x07 ||	// 0x07 indicates a private key blob.
				   pair[1]  != 0x02 ||	// 0x02 is the version number.
				   pair[2]  != 0x00 ||	// Reserved
				   pair[3]  != 0x00 ||	// Reserved
				   pair[4]  != 0x00 ||	// Algorithm ID: 00 24 00 00
				   pair[5]  != 0x24 ||
				   pair[6]  != 0x00 ||
				   pair[7]  != 0x00 ||
				   pair[8]  != 0x52 ||	// Magic: "RSA2"
				   pair[9]  != 0x53 ||
				   pair[10] != 0x41 ||
				   pair[11] != 0x32)
				{
					return null;
				}

				// Extract the number of bits in the modulus.
				int numBits = pair[12] |
						     (pair[13] << 8) |
						     (pair[14] << 16) |
						     (pair[15] << 24);
				if((numBits % 8) != 0)
				{
					return null;
				}

				// Construct the public key array.
				byte[] key = new byte [(numBits / 8) + 32];
				int keySize = key.Length - 12;

				// Construct the public key blob.
				key[0]  = 0x00;			// Algorithm ID: 00 24 00 00
				key[1]  = 0x24;
				key[2]  = 0x00;
				key[3]  = 0x00;
				key[4]  = 0x04;			// Hash ID (SHA1): 04 80 00 00
				key[5]  = 0x80;
				key[6]  = 0x00;
				key[7]  = 0x00;
				key[8]  = (byte)(keySize & 0xFF);
				key[9]  = (byte)((keySize >> 8) & 0xFF);
				key[10] = (byte)((keySize >> 16) & 0xFF);
				key[11] = (byte)((keySize >> 24) & 0xFF);
				key[12] = 0x06;			// 0x06 indicates a public key blob.
				Array.Copy(pair, 1, key, 13, keySize - 1);
				key[23] = 0x31;			// "RSA2" -> "RSA1".
				return key;
			}

	// Get the public key portion of the key pair.
	public byte[] PublicKey
			{
				get
				{
					if(publicKey == null && keyPairArray != null)
					{
						// Extract the public key portion of the key pair.
						publicKey = KeyPairToPublicKey(keyPairArray);
					}
					return publicKey;
				}
			}

}; // class StrongNameKeyPair

#endif // !ECMA_COMPAT

}; // namespace System.Reflection
