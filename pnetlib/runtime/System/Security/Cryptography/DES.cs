/*
 * DES.cs - Implementation of the "System.Security.Cryptography.DES" class.
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

public abstract class DES : SymmetricAlgorithm
{

	// Constructor.
	public DES()
			: base()
			{
				KeySizeValue = 64;
				BlockSizeValue = 64;
				FeedbackSizeValue = 64;
				LegalBlockSizesValue = new KeySizes [1];
				LegalBlockSizesValue[0] = new KeySizes(64, 64, 64);
				LegalKeySizesValue = new KeySizes [1];
				LegalKeySizesValue[0] = new KeySizes(64, 64, 64);
			}

	// Create a new instance of the DES algorithm.
	public new static DES Create()
			{
				return (DES)(CryptoConfig.CreateFromName
						(CryptoConfig.DESDefault, null));
			}
	public new static DES Create(String algName)
			{
				return (DES)(CryptoConfig.CreateFromName(algName, null));
			}

	// Determine if a DES key value is "semi-weak".
	public static bool IsSemiWeakKey(byte[] rgbKey)
			{
				if(rgbKey == null || rgbKey.Length != 8)
				{
					throw new CryptographicException
						(_("Crypto_InvalidKeySize"),
						 ((rgbKey == null) ? 0 : rgbKey.Length).ToString());
				}
				return CryptoMethods.IsSemiWeakKey(rgbKey, 0);
			}

	// Determine if a DES key value is "weak".
	public static bool IsWeakKey(byte[] rgbKey)
			{
				if(rgbKey == null || rgbKey.Length != 8)
				{
					throw new CryptographicException
						(_("Crypto_InvalidKeySize"),
						 ((rgbKey == null) ? 0 : rgbKey.Length).ToString());
				}
				return CryptoMethods.IsWeakKey(rgbKey, 0);
			}

	// Get or set the DES key value.
	public override byte[] Key
			{
				get
				{
					if(KeyValue == null)
					{
						GenerateKey();
					}
					return KeyValue;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if((value.Length * 8) != KeySizeValue)
					{
						throw new ArgumentException
							(String.Format(_("Crypto_InvalidKeySize"),
							 			   value.Length.ToString()),
							 "value");
					}
					else if(CryptoMethods.IsSemiWeakKey(value, 0) ||
							CryptoMethods.IsWeakKey(value, 0))
					{
						throw new CryptographicException
							(_("Crypto_WeakKey"));
					}
					KeyValue = value;
				}
			}

}; // class DES

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
