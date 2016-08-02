/*
 * SHA256.cs - Implementation of the
 *		"System.Security.Cryptography.SHA256" class.
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

public abstract class SHA256 : HashAlgorithm
{
	// Constructor.
	public SHA256()
			{
				HashSizeValue = 256;
			}

	// Create a new instance of the "SHA256" class.
	public new static SHA256 Create()
			{
				return (SHA256)(CryptoConfig.CreateFromName
						(CryptoConfig.SHA256Default, null));
			}
	public new static SHA256 Create(String algName)
			{
				return (SHA256)(CryptoConfig.CreateFromName(algName, null));
			}

}; // class SHA256

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
