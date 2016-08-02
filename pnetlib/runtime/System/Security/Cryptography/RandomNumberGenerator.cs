/*
 * RandomNumberGenerator.cs - Implementation of the
 *		"System.Security.Cryptography.RandomNumberGenerator" class.
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

public abstract class RandomNumberGenerator
{
	// Constructor.
	public RandomNumberGenerator() {}

	// Create an instance of the default random number generator.
	public static RandomNumberGenerator Create()
			{
				return (RandomNumberGenerator)
					(CryptoConfig.CreateFromName
						(CryptoConfig.RNGDefault, null));
			}

	// Create an instance of a specific random number generator.
	public static RandomNumberGenerator Create(String rngName)
			{
				return (RandomNumberGenerator)
					(CryptoConfig.CreateFromName(rngName, null));
			}

	// Get random data.
	public abstract void GetBytes(byte[] data);

	// Get non-zero random data.
	public abstract void GetNonZeroBytes(byte[] data);

}; // class RandomNumberGenerator

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
