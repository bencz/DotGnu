/*
 * PKCS1MaskGenerationMethod.cs - Implementation of the
 *		"System.Security.Cryptography.PKCS1MaskGenerationMethod" class.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO

using System;

public class PKCS1MaskGenerationMethod : MaskGenerationMethod
{
	// Internal state.
	private String hashName;

	// Constructor.
	public PKCS1MaskGenerationMethod()
			{
				hashName = CryptoConfig.SHA1Default;
			}

	// Get or set the hash to use to generate the mask.
	public String HashName
			{
				get
				{
					return hashName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(!CryptoConfig.IsAlgorithm
							(value, typeof(HashAlgorithm)))
					{
						throw new CryptographicException
							(_("Crypto_NeedsHash"));
					}
					hashName = value;
				}
			}

	// Generate a mask using a specific set and byte count.
	//
	// This implementation is based on the description in PKCS #1 v2.1.
	// ftp://ftp.rsasecurity.com/pub/pkcs/pkcs-1/pkcs-1v2-1.pdf
	public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
			{
				// Validate the parameters.
				if(rgbSeed == null)
				{
					throw new ArgumentNullException("rgbSeed");
				}
				else if(cbReturn < 0)
				{
					throw new ArgumentOutOfRangeException
						("cbReturn", _("ArgRange_NonNegative"));
				}

				// Create the final mask buffer.
				byte[] mask = new byte [cbReturn];

				// Create the hash algorithm instance.
				HashAlgorithm alg = HashAlgorithm.Create(hashName);

				// Create the mask.
				byte[] numbuf = new byte [4];
				byte[] hash;
				int hashSize = alg.HashSize;
				int count = 0;
				int index = 0;
				while(index < cbReturn)
				{
					numbuf[0] = (byte)(count >> 24);
					numbuf[1] = (byte)(count >> 16);
					numbuf[2] = (byte)(count >> 8);
					numbuf[3] = (byte)count;
					alg.InternalHashCore(rgbSeed, 0, rgbSeed.Length);
					alg.InternalHashCore(numbuf, 0, 4);
					hash = alg.InternalHashFinal();
					if(hashSize <= (cbReturn - index))
					{
						Array.Copy(hash, 0, mask, index, hashSize);
					}
					else
					{
						Array.Copy(hash, 0, mask, index, cbReturn - index);
					}
					Array.Clear(hash, 0, hash.Length);
					alg.Initialize();
					++count;
					index += hashSize;
				}
				Array.Clear(numbuf, 0, numbuf.Length);

				// The mask has been generated.
				return null;
			}

}; // class PKCS1MaskGenerationMethod

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
