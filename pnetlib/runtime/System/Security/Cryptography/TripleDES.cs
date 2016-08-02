/*
 * TripleDES.cs - Implementation of the
 *		"System.Security.Cryptography.TripleDES" class.
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

public abstract class TripleDES : SymmetricAlgorithm
{

	// Constructor.
	public TripleDES()
			: base()
			{
				KeySizeValue = 192;
				BlockSizeValue = 64;
				FeedbackSizeValue = 64;
				LegalBlockSizesValue = new KeySizes [1];
				LegalBlockSizesValue[0] = new KeySizes(64, 64, 64);
				LegalKeySizesValue = new KeySizes [1];
				LegalKeySizesValue[0] = new KeySizes(128, 192, 64);
			}

	// Create a new instance of the TripleDES algorithm.
	public new static TripleDES Create()
			{
				return (TripleDES)(CryptoConfig.CreateFromName
						(CryptoConfig.TripleDESDefault, null));
			}
	public new static TripleDES Create(String algName)
			{
				return (TripleDES)(CryptoConfig.CreateFromName(algName, null));
			}

	// Determine if a TripleDES key value is "weak".
	public static bool IsWeakKey(byte[] rgbKey)
			{
				if(rgbKey == null ||
				   (rgbKey.Length != 16 && rgbKey.Length != 24))
				{
					throw new CryptographicException
						(_("Crypto_InvalidKeySize"),
						 ((rgbKey == null) ? 0 : rgbKey.Length).ToString());
				}
				if(rgbKey.Length == 16)
				{
					return CryptoMethods.SameKey(rgbKey, 0, rgbKey, 8);
				}
				else
				{
					return CryptoMethods.SameKey(rgbKey, 0, rgbKey, 8) ||
					       CryptoMethods.SameKey(rgbKey, 8, rgbKey, 16);
				}
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
					else if(value.Length != 16 && value.Length != 24)
					{
						throw new CryptographicException
							(_("Crypto_InvalidKeySize"),
							 value.Length.ToString());
					}
					else if(IsWeakKey(value))
					{
						throw new CryptographicException
							(_("Crypto_WeakKey"));
					}
					KeySizeValue = value.Length;
					KeyValue = value;
				}
			}

}; // class TripleDES

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
