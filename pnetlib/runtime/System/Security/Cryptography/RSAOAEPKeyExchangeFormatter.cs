/*
 * RSAOAEPKeyExchangeFormatter.cs - Implementation of the
 *		"System.Security.Cryptography.RSAOAEPKeyExchangeFormatter" class.
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

public class RSAOAEPKeyExchangeFormatter : AsymmetricKeyExchangeFormatter
{
	// Internal state.
	private RSACryptoServiceProvider keyContainer;
	private byte[] padding;
	private RandomNumberGenerator rng;

	// Constructors.
	public RSAOAEPKeyExchangeFormatter()
			{
				keyContainer = null;
			}
	public RSAOAEPKeyExchangeFormatter(AsymmetricAlgorithm key)
			{
				SetKey(key);
			}

	// Get or set the random padding for the key exchange process.
	public byte[] Parameter
			{
				get
				{
					if(padding != null)
					{
						return (byte[])(padding.Clone());
					}
					return null;
				}
				set
				{
					if(value == null)
					{
						padding = null;
					}
					else
					{
						padding = (byte[])(value.Clone());
					}
				}
			}

	// Get the key exchange parameters.
	public override String Parameters
			{
				get
				{
					// For compatibility with Microsoft implementations.
					return "<enc:KeyEncryptionMethods enc:Algorithm=" +
					       "\"http:/www.microsoft/xml/security/algorithms" +
						   "/PKCS1-OAEP-KeyEx\" xmlns:enc=\"http://" +
					       "www.microsoft.com/xml/security/encryption/v1.0\"/>";
				}
			}

	// Get or set the random number generator to be used.
	public RandomNumberGenerator Rng
			{
				get
				{
					return rng;
				}
				set
				{
					rng = value;
				}
			}

	// Create key exchange material from key data.
	public override byte[] CreateKeyExchange(byte[] data)
			{
				if(keyContainer == null)
				{
					throw new CryptographicException
						(_("Crypto_MissingKey"));
				}
				if(rng == null)
				{
					rng = new RNGCryptoServiceProvider();
				}
				return keyContainer.EncryptOAEP(data, padding, rng);
			}
	public override byte[] CreateKeyExchange(byte[] data, Type symAlgType)
			{
				return CreateKeyExchange(data);
			}

	// Set the private key to use for encryption.
	public override void SetKey(AsymmetricAlgorithm key)
			{
				if(!(key is RSACryptoServiceProvider))
				{
					throw new CryptographicException
						(_("Crypto_NeedsRSA"));
				}
				keyContainer = (RSACryptoServiceProvider)key;
			}

}; // class RSAOAEPKeyExchangeFormatter

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
