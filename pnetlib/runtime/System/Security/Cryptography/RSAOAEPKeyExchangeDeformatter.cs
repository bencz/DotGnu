/*
 * RSAOAEPKeyExchangeDeformatter.cs - Implementation of the
 *		"System.Security.Cryptography.RSAOAEPKeyExchangeDeformatter" class.
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

public class RSAOAEPKeyExchangeDeformatter
	: AsymmetricKeyExchangeDeformatter
{
	// Internal state.
	private RSACryptoServiceProvider keyContainer;

	// Constructors.
	public RSAOAEPKeyExchangeDeformatter()
			{
				keyContainer = null;
			}
	public RSAOAEPKeyExchangeDeformatter(AsymmetricAlgorithm key)
			{
				SetKey(key);
			}

	// Get or set the key exchange parameters.
	public override String Parameters
			{
				get
				{
					// OAEP does not have any parameters.
					return null;
				}
				set
				{
					// OAEP does not have any parameters.
				}
			}

	// Decrypt key exchange material to get the key.
	public override byte[] DecryptKeyExchange(byte[] rgb)
			{
				if(keyContainer == null)
				{
					throw new CryptographicException
						(_("Crypto_MissingKey"));
				}
				return keyContainer.DecryptOAEP(rgb);
			}

	// Set the private key to use for decryption.
	public override void SetKey(AsymmetricAlgorithm key)
			{
				if(!(key is RSACryptoServiceProvider))
				{
					throw new CryptographicException
						(_("Crypto_NeedsRSA"));
				}
				keyContainer = (RSACryptoServiceProvider)key;
			}

}; // class RSAOAEPKeyExchangeDeformatter

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
