/*
 * ICryptoTransform.cs - Implementation of the
 *		"System.Security.Cryptography.ICryptoTransform" interface.
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

public interface ICryptoTransform : IDisposable
{

	// Determine if we can reuse this transform object.
	bool CanReuseTransform { get; }

	// Determine if multiple blocks can be transformed.
	bool CanTransformMultipleBlocks { get; }

	// Get the input block size.
	int InputBlockSize { get; }

	// Get the output block size.
	int OutputBlockSize { get; }

	// Transform an input block into an output block.
	int TransformBlock(byte[] inputBuffer, int inputOffset,
					   int inputCount, byte[] outputBuffer,
					   int outputOffset);

	// Transform the final input block into a final output block.
	byte[] TransformFinalBlock(byte[] inputBuffer,
							   int inputOffset,
							   int inputCount);

}; // interface ICryptoTransform

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
