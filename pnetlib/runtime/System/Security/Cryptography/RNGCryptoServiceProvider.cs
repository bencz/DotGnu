/*
 * RNGCryptoServiceProvider.cs - Implementation of the
 *		"System.Security.Cryptography.RNGCryptoServiceProvider" class.
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

public sealed class RNGCryptoServiceProvider : RandomNumberGenerator
{
	// Constructors.  We ignore the supplied parameters, and use
	// the runtime engine's random number generator instead.
	public RNGCryptoServiceProvider() {}
	public RNGCryptoServiceProvider(byte[] rgb) {}
	public RNGCryptoServiceProvider(CspParameters cspParams) {}
	public RNGCryptoServiceProvider(String str) {}

	// Destructor.  Nothing to do in this implementation.
	~RNGCryptoServiceProvider() {}

	// Get random data.
	public override void GetBytes(byte[] data)
			{
				CryptoMethods.GenerateRandom(data, 0, data.Length);
			}

	// Get non-zero random data.
	public override void GetNonZeroBytes(byte[] data)
			{
				int index;

				// Get the initial random data.
				CryptoMethods.GenerateRandom(data, 0, data.Length);

				// Replace zero bytes with new random data.
				for(index = 0; index < data.Length; ++index)
				{
					while(data[index] == 0)
					{
						CryptoMethods.GenerateRandom(data, index, 1);
					}
				}
			}

}; // class RNGCryptoServiceProvider

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
