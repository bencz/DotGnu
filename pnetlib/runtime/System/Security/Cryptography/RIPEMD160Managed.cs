/*
 * RIPEMD160Managed.cs - Implementation of the
 *		"System.Security.Cryptography.RIPEMD160Managed" class.
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

// Note: we use the runtime engine's implementation rather than
// write a version in C#.  This allows the system engine to be
// configured without RIPEMD160 on smaller platforms.

public class RIPEMD160Managed : RIPEMD160
{
	// Internal state from the runtime engine.
	private IntPtr state;

	// Constructor.
	public RIPEMD160Managed()
			: base()
			{
				try
				{
					state = CryptoMethods.HashNew(CryptoMethods.RIPEMD160);
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not have RIPEMD160 support.
					throw new CryptographicException
						(_("Crypto_NoProvider"), "RIPEMD160");
				}
			}

	// Destructor.
	~RIPEMD160Managed()
			{
				Dispose(false);
			}

	// Initialize the hash algorithm.
	public override void Initialize()
			{
				lock(this)
				{
					if(state != IntPtr.Zero)
					{
						CryptoMethods.HashReset(state);
					}
				}
			}

	// Dispose this object.
	protected override void Dispose(bool disposing)
			{
				if(disposing)
				{
					lock(this)
					{
						if(state != IntPtr.Zero)
						{
							CryptoMethods.HashFree(state);
							state = IntPtr.Zero;
						}
					}
				}
				else if(state != IntPtr.Zero)
				{
					CryptoMethods.HashFree(state);
					state = IntPtr.Zero;
				}
			}

	// Write data to the underlying hash algorithm.
	protected override void HashCore(byte[] array, int ibStart, int cbSize)
			{
				// Validate the parameters.
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(ibStart < 0 || ibStart > array.Length)
				{
					throw new ArgumentOutOfRangeException
						("ibStart", _("ArgRange_Array"));
				}
				else if(cbSize < 0 || (array.Length - ibStart) < cbSize)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}

				// Bail out if the runtime engine does not have a provider.
				if(state == IntPtr.Zero)
				{
					throw new CryptographicException
						(_("Crypto_NoProvider"), "RIPEMD160");
				}

				// Update the RIPEMD160 state.
				lock(this)
				{
					CryptoMethods.HashUpdate(state, array, ibStart, cbSize);
				}
			}

	// Finalize the hash and return the final hash value.
	protected override byte[] HashFinal()
			{
				// Bail out if the runtime engine does not have a provider.
				if(state == IntPtr.Zero)
				{
					throw new CryptographicException
						(_("Crypto_NoProvider"), "RIPEMD160");
				}

				// Compute the hash and return it.
				byte[] hash = new byte [64];
				lock(this)
				{
					CryptoMethods.HashFinal(state, hash);
				}
				return hash;
			}

}; // class RIPEMD160Managed

#endif // CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

}; // namespace System.Security.Cryptography
