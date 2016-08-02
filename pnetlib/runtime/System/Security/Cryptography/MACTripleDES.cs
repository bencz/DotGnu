/*
 * MACTripleDES.cs - Implementation of the
 *		"System.Security.Cryptography.MACTripleDES" class.
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
using System.IO;
using Platform;

public class MACTripleDES : KeyedHashAlgorithm
{
	// Internal state.
	private TripleDES alg;
	private CryptoStream stream;
	private byte[] block;

	// Constructors.
	public MACTripleDES()
			{
				HashSizeValue = 64;
				KeyValue = new byte [24];
				CryptoMethods.GenerateRandom(KeyValue, 0, 24);
				SetupAlgorithm(CryptoConfig.TripleDESDefault);
			}
	public MACTripleDES(byte[] rgbKey)
			{
				HashSizeValue = 64;
				if(rgbKey == null)
				{
					throw new ArgumentNullException("rgbKey");
				}
				KeyValue = rgbKey;
				SetupAlgorithm(CryptoConfig.TripleDESDefault);
			}
	public MACTripleDES(String strTripleDES, byte[] rgbKey)
			{
				HashSizeValue = 64;
				if(rgbKey == null)
				{
					throw new ArgumentNullException("rgbKey");
				}
				KeyValue = rgbKey;
				SetupAlgorithm(strTripleDES);
			}

	// Destructor.
	~MACTripleDES()
			{
				Dispose(false);
			}

	// Dispose this object.
	protected override void Dispose(bool disposing)
			{
				if(stream != null)
				{
					stream.Close();
					stream = null;
				}
				if(block != null)
				{
					Array.Clear(block, 0, block.Length);
				}
				if(alg != null)
				{
					((IDisposable)alg).Dispose();
				}
				base.Dispose(disposing);
			}

	// Set up the TripleDES algorithm instance.
	private void SetupAlgorithm(String name)
			{
				// Create the algorithm instance.
				alg = TripleDES.Create(name);

				// Set the starting initialization vector to all-zeroes.
				alg.IV = new byte [8];

				// Set the padding mode to zeroes.
				alg.Padding = PaddingMode.Zeros;

				// We don't have a stream yet.
				stream = null;

				// Allocate the temporary block for the final hash value.
				block = new byte [8];
			}

	// Initialize the hash algorithm.
	public override void Initialize()
			{
				if(stream != null)
				{
					stream.Close();
					stream = null;
					Array.Clear(block, 0, block.Length);
				}
			}

	// Prepare the hash for the initial call to "HashCore" or "HashFinal".
	private void Prepare()
			{
				if(stream == null)
				{
					stream = new CryptoStream
						(new FinalBlockStream(block),
						 alg.CreateEncryptor(KeyValue, alg.IV),
						 CryptoStreamMode.Write);
				}
			}

	// Write data to the underlying hash algorithm.
	protected override void HashCore(byte[] array, int ibStart, int cbSize)
			{
				Prepare();
				stream.Write(array, ibStart, cbSize);
			}

	// Finalize the hash and return the final hash value.
	protected override byte[] HashFinal()
			{
				Prepare();
				stream.Close();
				stream = null;
				byte[] hash = (byte[])(block.Clone());
				Array.Clear(block, 0, block.Length);
				return hash;
			}

	// A stream class that retrieves the final 8 bytes that are written to it.
	private sealed class FinalBlockStream : Stream
	{
		// Internal state.
		private byte[] block;
		private int blockSize;

		// Constructor.
		public FinalBlockStream(byte[] block)
			: base()
			{
				this.block = block;
				blockSize = 0;
			}

		// Write data to this stream.
		public override void Write(byte[] buffer, int offset, int count)
			{
				if(count >= 8)
				{
					Array.Copy(buffer, offset + count - 8, block, 0, 8);
					blockSize = 8;
				}
				else if(count <= (8 - blockSize))
				{
					Array.Copy(block, offset, block, blockSize, count);
					blockSize += count;
				}
				else if(count > 0)
				{
					Array.Copy(block, count, block, 0, 8 - count);
					Array.Copy(buffer, offset, block, 8 - count, count);
					blockSize = 8;
				}
			}

		// Stub out unnecessary methods and properties.
		public override void Close() {}
		public override void Flush() {}
		public override int Read(byte[] buffer, int offset, int count)
			{ return 0; }
		public override long Seek(long offset, SeekOrigin origin)
			{ return 0; }
		public override void SetLength(long length) {}
		public override bool CanRead { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override long Length { get { return 0; } }
		public override long Position { get { return 0; } set {} }

	}; // class FinalBlockStream

}; // class MACTripleDES

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
