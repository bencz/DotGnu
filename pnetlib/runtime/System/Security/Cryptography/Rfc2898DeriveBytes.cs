/*
 * Rfc2898DeriveBytes.cs - Implementation of the
 *		"System.Security.Cryptography.Rfc2898DeriveBytes" class.
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

using System;
using Platform;
using System.Text;

// Both "PasswordDeriveBytes" and "Rfc2898DeriveBytes" implement
// PKCS #5 v2.0.  The only difference between the two classes is
// that this one uses SHA1 as the fixed hash algorithm.  Because
// of this, we just wrap this class around "PasswordDeriveBytes".

public class Rfc2898DeriveBytes : DeriveBytes
{
	// Internal state.
	private PasswordDeriveBytes db;

	// Constructors.
	public Rfc2898DeriveBytes(String password, int saltSize)
			: this(password, GenerateSalt(saltSize), 1000) {}
	public Rfc2898DeriveBytes(String password, int saltSize, int iterations)
			: this(password, GenerateSalt(saltSize), iterations) {}
	public Rfc2898DeriveBytes(String password, byte[] salt)
			: this(password, salt, 1000) {}
	public Rfc2898DeriveBytes(String password, byte[] salt, int iterations)
			{
				if(password == null)
				{
					throw new ArgumentNullException("password");
				}
				if(salt == null)
				{
					throw new ArgumentNullException("salt");
				}
				if(salt.Length < 8)
				{
					throw new ArgumentException(_("Crypto_SaltSize"));
				}
				db = new PasswordDeriveBytes
					(password, salt, "SHA1", iterations);
			}

	// Generate a random salt of a particular size.
	private static byte[] GenerateSalt(int saltSize)
			{
				if(saltSize < 0)
				{
					throw new ArgumentOutOfRangeException
						("saltSize", _("ArgRange_NonNegative"));
				}
				byte[] salt = new byte [saltSize];
				CryptoMethods.GenerateRandom(salt, 0, saltSize);
				return salt;
			}

	// Get or set the iteration count.
	public int IterationCount
			{
				get
				{
					return db.IterationCount;
				}
				set
				{
					if(value < 1)
					{
						throw new ArgumentOutOfRangeException
							("value", _("ArgRange_PositiveNonZero"));
					}
					db.iterations = value;
				}
			}

	// Get or set the salt.
	public byte[] Salt
			{
				get
				{
					return db.Salt;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(value.Length < 8)
					{
						throw new ArgumentException(_("Crypto_SaltSize"));
					}
					db.rgbSalt = (byte[])(value.Clone());
				}
			}

	// Get the pseudo-random key bytes.
	public override byte[] GetBytes(int cb)
			{
				if(cb < 1)
				{
					throw new ArgumentOutOfRangeException
						("cb", _("ArgRange_PositiveNonZero"));
				}
				return db.GetBytes(cb);
			}

	// Reset the state.
	public override void Reset()
			{
				db.Reset();
			}

}; // class Rfc2898DeriveBytes

#endif // CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

}; // namespace System.Security.Cryptography
