/*
 * FromBase64Transform.cs - Implementation of the
 *		"System.Security.Cryptography.FromBase64Transform" class.
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

public class FromBase64Transform : ICryptoTransform, IDisposable
{
	// Internal state.
	byte[] inBuffer;
	int inBufPosn;

	// Constructors.
	public FromBase64Transform()
			{
				inBuffer = new byte [4];
				inBufPosn = 0;
			}
	public FromBase64Transform(FromBase64TransformMode whitespaces)
			: this()
			{
				// This class ignores white space and non-base64 characters,
				// even if the constructor was called with "don't ignore".
			}

	// Destructor.
	~FromBase64Transform()
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
					return 1;
				}
			}

	// Get the output block size.
	public int OutputBlockSize
			{
				get
				{
					return 3;
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
				if(inBuffer != null)
				{
					Array.Clear(inBuffer, 0, inBuffer.Length);
				}
				inBufPosn = 0;
			}

	// Transform a block of input data.
	public int TransformBlock(byte[] inputBuffer, int inputOffset,
							  int inputCount, byte[] outputBuffer,
							  int outputOffset)
			{
				int offset = outputOffset;
				int value;
				while(inputCount > 0)
				{
					// Fetch the next 6-bit value from the input stream.
					value = Convert.base64Values[inputBuffer[inputOffset++]];
					--inputCount;
					if(value < 0)
					{
						continue;
					}

					// Add the value to the buffer and check for overflow.
					inBuffer[inBufPosn++] = (byte)value;
					if(inBufPosn >= 4)
					{
						outputBuffer[offset++] =
							(byte)((inBuffer[0] << 2) | (inBuffer[1] >> 4));
						outputBuffer[offset++] =
							(byte)((inBuffer[1] << 4) | (inBuffer[2] >> 2));
						outputBuffer[offset++] =
							(byte)((inBuffer[2] << 6) | inBuffer[3]);
						inBufPosn = 0;
					}
				}
				return offset - outputOffset;
			}

	// Transform the final block of input data.
	public byte[] TransformFinalBlock(byte[] inputBuffer,
									  int inputOffset, int inputCount)
			{
				// Allocate an output buffer that is at least as
				// big as we need to store the remaining data.
				byte[] outputBuffer =
					new byte [inputCount * 3 / 4 + 3];

				// Convert the contents of the input buffer.
				int offset = 0;
				int value;
				while(inputCount > 0)
				{
					// Fetch the next 6-bit value from the input stream.
					value = Convert.base64Values[inputBuffer[inputOffset++]];
					--inputCount;
					if(value < 0)
					{
						continue;
					}

					// Add the value to the buffer and check for overflow.
					inBuffer[inBufPosn++] = (byte)value;
					if(inBufPosn >= 4)
					{
						outputBuffer[offset++] =
							(byte)((inBuffer[0] << 2) | (inBuffer[1] >> 4));
						outputBuffer[offset++] =
							(byte)((inBuffer[1] << 4) | (inBuffer[2] >> 2));
						outputBuffer[offset++] =
							(byte)((inBuffer[2] << 6) | inBuffer[3]);
						inBufPosn = 0;
					}
				}

				// Output the left-over bytes.
				if(inBufPosn == 2)
				{
					outputBuffer[offset++] = (byte)(inBuffer[0] << 2);
				}
				else if(inBufPosn == 3)
				{
					outputBuffer[offset++] =
						(byte)((inBuffer[0] << 2) | (inBuffer[1] >> 4));
					outputBuffer[offset++] = (byte)(inBuffer[1] << 4);
				}
				inBufPosn = 0;

				// Shorten the array to its final size.
				if(offset != outputBuffer.Length)
				{
					byte[] newout = new byte [offset];
					Array.Copy(outputBuffer, 0, newout, 0, offset);
					Array.Clear(outputBuffer, 0, outputBuffer.Length);
					return newout;
				}
				else
				{
					return outputBuffer;
				}
			}

}; // class FromBase64Transform

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
