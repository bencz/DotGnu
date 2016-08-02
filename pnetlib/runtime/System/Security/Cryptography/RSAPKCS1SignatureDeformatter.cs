/*
 * RSAPKCS1SignatureDeformatter.cs - Implementation of the
 *		"System.Security.Cryptography.RSAPKCS1SignatureDeformatter" class.
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

public class RSAPKCS1SignatureDeformatter : AsymmetricSignatureDeformatter
{
	// Internal state.
	private RSACryptoServiceProvider keyContainer;
	private String hashAlgorithm;

	// Constructors.
	public RSAPKCS1SignatureDeformatter()
			{
				keyContainer = null;
			}
	public RSAPKCS1SignatureDeformatter(AsymmetricAlgorithm key)
			{
				SetKey(key);
			}

	// Set the hash algorithm.
	public override void SetHashAlgorithm(string strName)
			{
				hashAlgorithm = strName;
			}

	// Set the key to use to verify the signature.
	public override void SetKey(AsymmetricAlgorithm key)
			{
				if(!(key is RSACryptoServiceProvider))
				{
					throw new CryptographicException
						(_("Crypto_NeedsRSA"));
				}
				keyContainer = (RSACryptoServiceProvider)key;
			}

	// Verify the signature for a specific hash value.
	public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
			{
				if(keyContainer == null)
				{
					throw new CryptographicUnexpectedOperationException
						(_("Crypto_MissingKey"));
				}
				if(hashAlgorithm == null)
				{
					throw new CryptographicUnexpectedOperationException
						(_("Crypto_PKCS1Hash"));
				}
				return keyContainer.VerifyHash
					(rgbHash, hashAlgorithm, rgbSignature);
			}

}; // class RSAPKCS1SignatureDeformatter

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
