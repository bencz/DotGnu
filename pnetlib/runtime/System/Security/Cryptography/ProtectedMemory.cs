/*
 * ProtectedMemory.cs - Implementation of the
 *		"System.Security.Cryptography.ProtectedMemory" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#if CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

// Read the comments in "ProtectedData.cs" before using this class.

public sealed class ProtectedMemory
{
	// Cannot instantiate this class.
	private ProtectedMemory() {}

	// Protect a block of memory.
	public static void Protect
				(byte[] userData, MemoryProtectionScope scope)
			{
				if(userData == null)
				{
					throw new ArgumentNullException("userData");
				}
				if((userData.Length % 16) != 0)
				{
					throw new CryptographicException(_("Crypto_MultOf16"));
				}
				ProtectedData.Protect(userData, null, scope, userData);
			}

	// Unprotect a block of memory.
	public static void Unprotect
				(byte[] encryptedData, MemoryProtectionScope scope)
			{
				if(encryptedData == null)
				{
					throw new ArgumentNullException("encryptedData");
				}
				if((encryptedData.Length % 16) != 0)
				{
					throw new CryptographicException(_("Crypto_MultOf16"));
				}
				ProtectedData.Unprotect
					(encryptedData, null, scope, encryptedData);
			}

}; // class ProtectedMemory

#endif // CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

}; // namespace System.Security.Cryptography
