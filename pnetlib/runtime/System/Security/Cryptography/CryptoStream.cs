/*
 * CryptoStream.cs - Implementation of the
 *		"System.Security.Cryptography.CryptoStream" class.
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
using System.IO;

public class CryptoStream : Stream, IDisposable
{
	// Internal state.
	private Stream stream;
	private ICryptoTransform transform;
	private CryptoStreamMode mode;
	private bool flushFinal;
	private int inBlockSize;
	private int inBufferSize;
	private int inBufferPosn;
	private byte[] inBuffer;
	private int outBlockSize;
	private int outBufferSize;
	private int outBufferPosn;
	private byte[] outBuffer;
	private bool sawEOF;

	// Constructor.
	public CryptoStream(Stream stream, ICryptoTransform transform,
						CryptoStreamMode mode)
			{
				this.stream = stream;
				this.transform = transform;
				this.mode = mode;
				this.flushFinal = false;
				if(mode == CryptoStreamMode.Read)
				{
					if(!(stream.CanRead))
					{
						throw new NotSupportedException
							(_("IO_NotSupp_Read"));
					}
				}
				else if(mode == CryptoStreamMode.Write)
				{
					if(!(stream.CanWrite))
					{
						throw new NotSupportedException
							(_("IO_NotSupp_Write"));
					}
				}
				else
				{
					throw new NotSupportedException
						(_("Crypto_InvalidStreamMode"));
				}
				inBlockSize = transform.InputBlockSize;
				outBlockSize = transform.OutputBlockSize;
				if(transform.CanTransformMultipleBlocks)
				{
					inBufferSize = 1024 - (1024 % inBlockSize);
					outBufferSize =
						(inBufferSize / inBlockSize) * outBlockSize;
				}
				else
				{
					// We need at least two blocks in the input buffer to
					// process padding blocks at the end of the stream.
					inBufferSize = transform.InputBlockSize * 2;
					outBufferSize = transform.OutputBlockSize;
				}
				inBuffer = new byte [inBufferSize];
				outBuffer = new byte [outBufferSize];
				inBufferPosn = 0;
				outBufferPosn = 0;
				sawEOF = false;
			}

	// Destructor.
	~CryptoStream()
			{
				Dispose(false);
			}

	// Stream properties.
	public override bool CanRead
			{
				get
				{
					return (mode == CryptoStreamMode.Read);
				}
			}
	public override bool CanSeek
			{
				get
				{
					return false;
				}
			}
	public override bool CanWrite
			{
				get
				{
					return (mode == CryptoStreamMode.Write);
				}
			}
	public override long Length
			{
				get
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}
			}
	public override long Position
			{
				get
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}
				set
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}
			}

	// Clear the resources used by this stream.
	public void Clear()
			{
				((IDisposable)this).Dispose();
			}

	// Dispose this stream.
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
				if(outBuffer != null)
				{
					Array.Clear(outBuffer, 0, outBuffer.Length);
				}
				if(transform != null)
				{
					transform.Dispose();
					transform = null;
				}
			}

	// Flush the final block to the output stream.
	public void FlushFinalBlock()
			{
				if(mode != CryptoStreamMode.Write || flushFinal)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}
				else if(stream == null)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
				byte[] buf = transform.TransformFinalBlock
						(inBuffer, 0, inBufferPosn);
				if(buf != null)
				{
					stream.Write(buf, 0, buf.Length);
					Array.Clear(buf, 0, buf.Length);
				}
				flushFinal = true;
			}

	// Override unsupported stream methods.
	public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException(_("IO_NotSupp_Seek"));
			}
	public override void SetLength(long value)
			{
				throw new NotSupportedException(_("IO_NotSupp_SetLength"));
			}

	// Close the stream.
	public override void Close()
			{
				// Bail out if the stream is already closed.
				if(stream == null)
				{
					return;
				}

				// Flush the final block in write mode.
				if(mode == CryptoStreamMode.Write && !flushFinal)
				{
					FlushFinalBlock();
				}

				// Close the underlying stream.
				stream.Close();
				stream = null;

				// Clear the buffers, in case they contained plaintext data.
				Array.Clear(inBuffer, 0, inBuffer.Length);
				Array.Clear(outBuffer, 0, outBuffer.Length);
			}

	// Flush the stream.
	public override void Flush()
			{
				if(mode != CryptoStreamMode.Write)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}
				else if(stream == null)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
			}

	// Read data from the stream.
	public override int Read(byte[] buffer, int offset, int count)
			{
				int total, len, processed;
				byte[] buf;
				if(mode != CryptoStreamMode.Read)
				{
					throw new NotSupportedException(_("IO_NotSupp_Read"));
				}
				else if(stream == null)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
				total = 0;
				while(count > 0)
				{
					// Copy output data to the user-supplied buffer.
					if(outBufferPosn > 0)
					{
						len = outBufferPosn;
						if(len > count)
						{
							len = count;
						}
						Array.Copy(outBuffer, 0, buffer, offset, len);
						offset += len;
						count -= len;
						total += len;
						if(outBufferPosn < len)
						{
							Array.Copy(outBuffer, outBufferPosn,
									   outBuffer, 0, outBufferPosn - len);
							outBufferPosn -= len;
						}
						else
						{
							outBufferPosn = 0;
						}
						continue;
					}

					// Read more data into the input buffer.
					if(inBufferPosn < inBufferSize && !sawEOF)
					{
						len = stream.Read(inBuffer, inBufferPosn,
										  inBufferSize - inBufferPosn);
						if(len <= 0)
						{
							sawEOF = true;
						}
					}

					// Transform as many blocks as we can.  We only
					// do this if we have more than 1 block, because
					// the final block may be handled specially for
					// adding or removing padding bytes at EOF.
					if(inBufferPosn > inBlockSize)
					{
						if((inBufferPosn % inBlockSize) == 0)
						{
							processed = inBufferPosn - inBlockSize;
						}
						else
						{
							processed = inBufferPosn -
								(inBufferPosn % inBlockSize);
						}
						len = transform.TransformBlock
							(inBuffer, 0, processed, outBuffer, 0);
						outBufferPosn = len;
						Array.Copy(inBuffer, processed, inBuffer, 0,
								   inBufferPosn - processed);
						inBufferPosn -= processed;
					}
					else if(sawEOF)
					{
						if(!flushFinal)
						{
							// Replace "outBuffer" with the final block.
							buf = transform.TransformFinalBlock
									(inBuffer, 0, inBufferPosn);
							inBufferPosn = 0;
							Array.Clear(outBuffer, 0, outBuffer.Length);
							outBuffer = buf;
							outBufferPosn = 0;
							outBufferSize = buf.Length;
							flushFinal = true;
						}
						else
						{
							// We've reached the read EOF position.
							break;
						}
					}
				}
				return total;
			}

	// Write data to the stream.
	public override void Write(byte[] buffer, int offset, int count)
			{
				int len, processed;
				if(mode != CryptoStreamMode.Write)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}
				else if(stream == null)
				{
					throw new ObjectDisposedException
						(null, _("IO_StreamClosed"));
				}
				while(count > 0)
				{
					// Copy the input data into "inBuffer".
					len = inBufferSize - inBufferPosn;
					if(len > count)
					{
						len = count;
					}
					Array.Copy(buffer, offset, inBuffer, inBufferPosn, len);
					inBufferPosn += len;
					offset += len;
					count -= len;

					// Transform as many blocks as we can.  We only
					// do this if we have more than 1 block, because
					// the final block may be handled specially for
					// adding or removing padding bytes at EOF.
					if(inBufferPosn > inBlockSize)
					{
						if((inBufferPosn % inBlockSize) == 0)
						{
							processed = inBufferPosn - inBlockSize;
						}
						else
						{
							processed = inBufferPosn -
								(inBufferPosn % inBlockSize);
						}
						len = transform.TransformBlock
							(inBuffer, 0, processed, outBuffer, 0);
						if(len > 0)
						{
							stream.Write(outBuffer, 0, len);
						}
						Array.Copy(inBuffer, processed, inBuffer, 0,
								   inBufferPosn - processed);
						inBufferPosn -= processed;
					}
				}
			}

}; // class CryptoStream

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
