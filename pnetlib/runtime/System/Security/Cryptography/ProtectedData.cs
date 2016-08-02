/*
 * ProtectedData.cs - Implementation of the
 *		"System.Security.Cryptography.ProtectedData" class.
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

// The Microsoft version of this class uses master keys, encrypted by
// the user's password, to protect the data.  Access to the plaintext of
// the user's password is not possible in most systems, for good reason.
//
// We use a simpler scrambling algorihm, based on AES, that uses a
// fixed key based on the user's login identifier and the machine name.
// This provides some casual protection, but no real security against a
// real adversary.
//
// Key management needs to be done carefully, and Microsoft's approach to
// it is flat out *wrong*.  Anyone with access to the memory of the machine
// can discover the user's password with ease by merely installing a trojan,
// or engaging in some form of social engineering (e.g. pop up a dialog box
// and just ask the user for the password because they are conditioned to
// provide it on demand).
//
// Use of this API for any purpose is highly discouraged.

public sealed class ProtectedData
{
	// Cannot instantiate this class.
	private ProtectedData() {}

	// Get the encryption key to use to protect memory for a scope.
	private static byte[] GetScopeKey(MemoryProtectionScope scope, byte[] salt)
			{
				String key;
				PasswordDeriveBytes derive;
				if(scope == MemoryProtectionScope.SameLogon)
				{
					key = Environment.UserName;
				}
				else
				{
					key = Environment.UserName + "/" + Environment.MachineName;
				}
				if(salt == null)
				{
					salt = new byte [16];
				}
				derive = new PasswordDeriveBytes(key, salt);
				return derive.CryptDeriveKey("Rijndael", "SHA1", 16, null);
			}

	// Protect a block of memory.
	internal static void Protect
				(byte[] userData, byte[] optionalEntropy,
				 MemoryProtectionScope scope, byte[] output)
			{
				// Get the key to use.
				byte[] key = GetScopeKey(scope, optionalEntropy);

				// Encrypt the block of memory using AES.
				Rijndael alg = new RijndaelManaged();
				alg.Mode = CipherMode.CFB;
				ICryptoTransform transform = alg.CreateEncryptor(key, null);
				transform.TransformBlock
					(userData, 0, userData.Length, output, 0);
				transform.Dispose();
				alg.Clear();
				Array.Clear(key, 0, key.Length);
			}
	public static byte[] Protect
				(byte[] userData, byte[] optionalEntropy,
				 MemoryProtectionScope scope)
			{
				// Validate the parameters.
				if(userData == null)
				{
					throw new ArgumentNullException("userData");
				}

				// Protect the data and return it.
				byte[] output = new byte [userData.Length];
				Protect(userData, optionalEntropy, scope, output);
				return output;
			}

	// Unprotect a block of memory.
	internal static void Unprotect
				(byte[] encryptedData, byte[] optionalEntropy,
				 MemoryProtectionScope scope, byte[] output)
			{
				// Get the key to use.
				byte[] key = GetScopeKey(scope, optionalEntropy);

				// Decrypt the block of memory using AES.
				Rijndael alg = new RijndaelManaged();
				alg.Mode = CipherMode.CFB;
				ICryptoTransform transform = alg.CreateDecryptor(key, null);
				transform.TransformBlock
					(encryptedData, 0, encryptedData.Length, output, 0);
				transform.Dispose();
				alg.Clear();
				Array.Clear(key, 0, key.Length);
			}
	public static byte[] Unprotect
				(byte[] encryptedData, byte[] optionalEntropy,
				 MemoryProtectionScope scope)
			{
				// Validate the parameters.
				if(encryptedData == null)
				{
					throw new ArgumentNullException("encryptedData");
				}

				// Unprotect the data and return it.
				byte[] output = new byte [encryptedData.Length];
				Unprotect(encryptedData, optionalEntropy, scope, output);
				return output;
			}

}; // class ProtectedData

#endif // CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

}; // namespace System.Security.Cryptography
