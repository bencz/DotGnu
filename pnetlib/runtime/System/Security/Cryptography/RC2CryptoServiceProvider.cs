/*
 * RC2CryptoServiceProvider.cs - Implementation of the
 *		"System.Security.Cryptography.RC2CryptoServiceProvider" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
using Platform;

public sealed class RC2CryptoServiceProvider : RC2
{
	// Constructor.
	public RC2CryptoServiceProvider()
			: base()
			{
				if(!CryptoMethods.AlgorithmSupported(CryptoMethods.RC2))
				{
					throw new CryptographicException
						(_("Crypto_NoProvider"), "RC2");
				}
			}

	// Create a RC2 decryptor object.
	public override ICryptoTransform CreateDecryptor
				(byte[] rgbKey, byte[] rgbIV)
			{
				ValidateCreate(rgbKey, rgbIV);
				return new CryptoAPITransform
						(CryptoMethods.RC2, rgbIV, rgbKey,
						 BlockSizeValue, FeedbackSizeValue,
						 ModeValue, PaddingValue, false);
			}

	// Create a RC2 encryptor object.
	public override ICryptoTransform CreateEncryptor
				(byte[] rgbKey, byte[] rgbIV)
			{
				ValidateCreate(rgbKey, rgbIV);
				return new CryptoAPITransform
						(CryptoMethods.RC2, rgbIV, rgbKey,
						 BlockSizeValue, FeedbackSizeValue,
						 ModeValue, PaddingValue, true);
			}

	// Generate a random initialization vector.
	public override void GenerateIV()
			{
				byte[] iv = new byte [8];
				CryptoMethods.GenerateRandom(iv, 0, 8);
				IVValue = iv;
			}

	// Generate a random key value.
	public override void GenerateKey()
			{
				byte[] key = new byte [16];
				CryptoMethods.GenerateRandom(key, 0, 16);
				if(KeyValue != null)
				{
					// Clear the previous key value.
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				KeyValue = key;
			}

	// Get or set the effective key size.
	public override int EffectiveKeySize
			{
				get
				{
					return KeySizeValue;
				}
				set
				{
					// We cannot change the effective key size for
					// the RC2CryptoServiceProvider implementation.
					if(value != KeySizeValue)
					{
						throw new CryptographicException
							(_("Crypto_InvalidKeySize"), value.ToString());
					}
				}
			}

}; // class RC2CryptoServiceProvider

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
