/*
 * DESCryptoServiceProvider.cs - Implementation of the
 *		"System.Security.Cryptography.DESCryptoServiceProvider" class.
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
using Platform;

public sealed class DESCryptoServiceProvider : DES
{
	// Constructor.
	public DESCryptoServiceProvider()
			: base()
			{
				if(!CryptoMethods.AlgorithmSupported(CryptoMethods.DES))
				{
					throw new CryptographicException
						(_("Crypto_NoProvider"), "DES");
				}
			}

	// Create a DES decryptor object.
	public override ICryptoTransform CreateDecryptor
				(byte[] rgbKey, byte[] rgbIV)
			{
				ValidateCreate(rgbKey, rgbIV);
				return new CryptoAPITransform
						(CryptoMethods.DES, rgbIV, rgbKey,
						 BlockSizeValue, FeedbackSizeValue,
						 ModeValue, PaddingValue, false);
			}

	// Create a DES encryptor object.
	public override ICryptoTransform CreateEncryptor
				(byte[] rgbKey, byte[] rgbIV)
			{
				ValidateCreate(rgbKey, rgbIV);
				return new CryptoAPITransform
						(CryptoMethods.DES, rgbIV, rgbKey,
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
				byte[] key = new byte [8];
				do
				{
					CryptoMethods.GenerateRandom(key, 0, 8);
				}
				while(CryptoMethods.IsSemiWeakKey(key, 0) ||
				      CryptoMethods.IsWeakKey(key, 0));
				if(KeyValue != null)
				{
					// Clear the previous key value.
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				KeyValue = key;
			}

}; // class DESCryptoServiceProvider

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
