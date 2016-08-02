/*
 * SHA1Managed.cs - Implementation of the
 *		"System.Security.Cryptography.SHA1Managed" class.
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

// Note: in this implementation, there is no difference between
// SHA1CryptoServiceProvider and SHA1Managed.  Why deliberately
// implement a slow hash algorithm when a fast one is available?

public class SHA1Managed : SHA1
{
	// Internal state from the runtime engine.
	private IntPtr state;

	// Constructor.
	public SHA1Managed()
			: base()
			{
				try
				{
					state = CryptoMethods.HashNew(CryptoMethods.SHA1);
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not have SHA1 support.
					state = IntPtr.Zero;
				}
			}

	// Destructor.
	~SHA1Managed()
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
						(_("Crypto_NoProvider"), "SHA1");
				}

				// Update the SHA1 state.
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
						(_("Crypto_NoProvider"), "SHA1");
				}

				// Compute the hash and return it.
				byte[] hash = new byte [20];
				lock(this)
				{
					CryptoMethods.HashFinal(state, hash);
				}
				return hash;
			}

}; // class SHA1Managed

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
