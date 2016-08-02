/*
 * ToBase64Transform.cs - Implementation of the
 *		"System.Security.Cryptography.ToBase64Transform" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

public class ToBase64Transform : ICryptoTransform, IDisposable
{
	// Constructor.
	public ToBase64Transform() {}

	// Destructor.
	~ToBase64Transform()
			{
				Dispose(false);
			}

	// Determine if we can reuse this transform object.
	public virtual bool CanReuseTransform
			{
				get
				{
					return true;
				}
			}

	// Determine if this transformation can process multiple blocks.
	public bool CanTransformMultipleBlocks
			{
				get
				{
					return true;
				}
			}

	// Get the input block size.
	public int InputBlockSize
			{
				get
				{
					return 3;
				}
			}

	// Get the output block size.
	public int OutputBlockSize
			{
				get
				{
					return 4;
				}
			}

	// Clear the state of this object.
	public void Clear()
			{
				((IDisposable)this).Dispose();
			}

	// Dispose the state of this object.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do here.
			}

	// Transform a block of input data.
	public int TransformBlock(byte[] inputBuffer, int inputOffset,
							  int inputCount, byte[] outputBuffer,
							  int outputOffset)
			{
				int offset = outputOffset;
				int b1, b2, b3;
				String base64 = Convert.base64Chars;
				while(inputCount >= 3)
				{
					// Fetch the next three bytes to be encoded.
					b1 = inputBuffer[inputOffset];
					b2 = inputBuffer[inputOffset + 1];
					b3 = inputBuffer[inputOffset + 2];
					inputOffset += 3;
					inputCount -= 3;

					// Encode the bytes as ASCII characters.
					outputBuffer[offset++] = (byte)(base64[b1 >> 2]);
					outputBuffer[offset++] =
						(byte)(base64[((b1 & 0x03) << 4) | (b2 >> 4)]);
					outputBuffer[offset++] =
						(byte)(base64[((b2 & 0x0F) << 2) | (b3 >> 6)]);
					outputBuffer[offset++] = (byte)(base64[b3 & 0x3F]);
				}
				return offset - outputOffset;
			}

	// Transform the final block of input data.
	public byte[] TransformFinalBlock(byte[] inputBuffer,
									  int inputOffset, int inputCount)
			{
				// Allocate a buffer to hold the final block's translation.
				byte[] outputBuffer = new byte [((inputCount + 2) / 3) * 4];

				// Translate any full blocks that are present.
				int offset = 0;
				int b1, b2, b3;
				String base64 = Convert.base64Chars;
				while(inputCount >= 3)
				{
					// Fetch the next three bytes to be encoded.
					b1 = inputBuffer[inputOffset];
					b2 = inputBuffer[inputOffset + 1];
					b3 = inputBuffer[inputOffset + 2];
					inputOffset += 3;
					inputCount -= 3;

					// Encode the bytes as ASCII characters.
					outputBuffer[offset++] = (byte)(base64[b1 >> 2]);
					outputBuffer[offset++] =
						(byte)(base64[((b1 & 0x03) << 4) | (b2 >> 4)]);
					outputBuffer[offset++] =
						(byte)(base64[((b2 & 0x0F) << 2) | (b3 >> 6)]);
					outputBuffer[offset++] = (byte)(base64[b3 & 0x3F]);
				}

				// Translate the final partial block.
				if(inputCount == 1)
				{
					b1 = inputBuffer[inputOffset];
					outputBuffer[offset++] = (byte)(base64[b1 >> 2]);
					outputBuffer[offset++] = (byte)(base64[(b1 & 0x03) << 4]);
					outputBuffer[offset++] = (byte)'=';
					outputBuffer[offset++] = (byte)'=';
				}
				else if(inputCount == 2)
				{
					b1 = inputBuffer[inputOffset];
					b2 = inputBuffer[inputOffset + 1];
					outputBuffer[offset++] = (byte)(base64[b1 >> 2]);
					outputBuffer[offset++] =
						(byte)(base64[((b1 & 0x03) << 4) | (b2 >> 4)]);
					outputBuffer[offset++] = (byte)(base64[(b2 & 0x0F) << 2]);
					outputBuffer[offset++] = (byte)'=';
				}

				// Return the translated buffer to the caller.
				return outputBuffer;
			}

}; // class ToBase64Transform

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
