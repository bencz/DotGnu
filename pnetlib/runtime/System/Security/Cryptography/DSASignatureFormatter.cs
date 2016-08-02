/*
 * DSASignatureFormatter.cs - Implementation of the
 *		"System.Security.Cryptography.DSASignatureFormatter" class.
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

public class DSASignatureFormatter : AsymmetricSignatureFormatter
{
	// Internal state.
	private DSA keyContainer;

	// Constructors.
	public DSASignatureFormatter()
			{
				keyContainer = null;
			}
	public DSASignatureFormatter(AsymmetricAlgorithm key)
			{
				SetKey(key);
			}

	// Set the hash algorithm.
	public override void SetHashAlgorithm(string strName)
			{
				if(strName == null)
				{
					throw new ArgumentNullException("strName");
				}
				if(!CryptoConfig.IsAlgorithm(strName, typeof(SHA1)))
				{
					throw new CryptographicException
						(_("Crypto_DSANeedsSHA1"));
				}
			}

	// Set the key to use to compute the signature.
	public override void SetKey(AsymmetricAlgorithm key)
			{
				if(!(key is DSA))
				{
					throw new CryptographicException
						(_("Crypto_NeedsDSA"));
				}
				keyContainer = (DSA)key;
			}

	// Create a signature for a specified hash value.
	public override byte[] CreateSignature(byte[] rgbHash)
			{
				if(keyContainer == null)
				{
					throw new CryptographicException
						(_("Crypto_MissingKey"));
				}
				return keyContainer.CreateSignature(rgbHash);
			}

}; // class DSASignatureFormatter

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
