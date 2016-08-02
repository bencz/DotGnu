/*
 * Rijndael.cs - Implementation of the
 *		"System.Security.Cryptography.Rijndael" class.
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

public abstract class Rijndael : SymmetricAlgorithm
{

	// Constructor.
	public Rijndael()
			: base()
			{
				KeySizeValue = 128;
				BlockSizeValue = 128;
				FeedbackSizeValue = 128;
				LegalBlockSizesValue = new KeySizes [1];
				LegalBlockSizesValue[0] = new KeySizes(128, 128, 128);
				LegalKeySizesValue = new KeySizes [1];
				LegalKeySizesValue[0] = new KeySizes(128, 256, 64);
			}

	// Create a new instance of the Rijndael algorithm.
	public new static Rijndael Create()
			{
				return (Rijndael)(CryptoConfig.CreateFromName
						(CryptoConfig.RijndaelDefault, null));
			}
	public new static Rijndael Create(String algName)
			{
				return (Rijndael)(CryptoConfig.CreateFromName(algName, null));
			}

}; // class Rijndael

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
