/*
 * BufferedStream.cs - Implementation of "System.IO.BufferedStream" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * contributed by Gopal.V
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

using System;

namespace System.IO
{
#if !ECMA_COMPAT

	public sealed class BufferedStream: Stream
	{
		private Stream stream;
		private int    		bufferSize;
		private byte[] 		inBuffer;
		private int    		inBufferPosn;
		private int    		inBufferLen;
		private byte[] 		outBuffer;
		private int    		outBufferPosn;
		private int    		outBufferLen;
		private bool		sawEOF;

		private bool		readMode;

		// Default and minimum buffer sizes to use for streams.
		private const int STREAM_BUFSIZ = 1024;
		private const int MIN_BUFSIZ    = 128;
		
		public BufferedStream(Stream stream) : this(stream,STREAM_BUFSIZ)
		{
		}

		public BufferedStream(Stream stream, int bufferSize)
		{
				// Validate the parameters.
			if(stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if(bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException
					("bufferSize", _("ArgRange_BufferSize"));
			}
			if(bufferSize < MIN_BUFSIZ)
			{
				bufferSize = MIN_BUFSIZ;
			}

			this.stream = stream;
			this.bufferSize = bufferSize;
			this.inBuffer = new byte [bufferSize];
			this.inBufferPosn = 0;
			this.inBufferLen = 0;
			this.outBuffer = new byte [bufferSize];
			this.outBufferPosn = 0;
			this.outBufferLen = 0;
			this.sawEOF = false;
			this.readMode=false;
		}

		public override void Close()
		{
			Flush();
			stream.Close();
		}

		public override void Flush()
		{
			if(readMode)
			{
				/* data will be lost if I cannot seek back ? */
				if(CanSeek && stream.Position != this.Position)
				{
					stream.Seek(inBufferLen-inBufferPosn,SeekOrigin.Current);
					inBufferPosn=0;
					inBufferLen=0;
				}
			}
			else
			{
				if((!CanSeek) || stream.Position == this.Position)
				{
					if((outBufferLen - outBufferPosn) !=0)
					{
						stream.Write(outBuffer,outBufferPosn,
										(outBufferLen-outBufferPosn));
						stream.Flush();
						outBufferLen=0;
						outBufferPosn=0;
					}
				}
				else
				{
					/* a dirty write .. by design all reads are flushed
					   before writing , so this is a bug or mistake by
					   the application dev */
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}
			}
			stream.Flush();
		}

		private void FillBuffer()
		{
			int len;
			
			/* this is lifted off StreamReader */
			while(inBufferPosn >= inBufferLen && !sawEOF)
			{
				// Move the previous left-over buffer contents down.
				if((inBufferLen - inBufferPosn) < bufferSize)
				{
					if(inBufferPosn < inBufferLen)
					{
						Array.InternalCopy
							(inBuffer, inBufferPosn,
						     inBuffer, 0, inBufferLen - inBufferPosn);
						inBufferLen -= inBufferPosn;
					}
					else
					{
						inBufferLen = 0;
					}
					inBufferPosn = 0;
					// Read new bytes into the buffer.
					if(stream == null)
					{
						throw new IOException(_("IO_StreamClosed"));
					}
					len = stream.Read(inBuffer, inBufferPosn,
									  bufferSize - inBufferPosn);
					if(len <= 0)
					{
						sawEOF = true;
					}
					else
					{
						inBufferLen += len;
					}
				}
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			// Validate the parameters.
			if(buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if(offset < 0)
			{
				throw new ArgumentOutOfRangeException
					("offset", _("ArgRange_Array"));
			}
			if(count < 0)
			{
				throw new ArgumentOutOfRangeException
					("count", _("ArgRange_Array"));
			}
			if((buffer.Length - offset) < count)
			{
				throw new ArgumentException
					(_("Arg_InvalidArrayRange"));
			}
			if(!readMode)
			{
				Flush(); /* flush previous writes for avoiding dirty stuff */
				readMode=true;
			}
			// Read data from the input stream into the buffer.
			int len = 0;
			int templen;
			while(count > 0)
			{
				// Re-fill the buffer
				if(inBufferPosn >= inBufferLen)
				{
					FillBuffer();
					if(inBufferPosn >= inBufferLen)
					{
						break;
					}
				}
				// Copy data to the result buffer.
				templen = inBufferLen - inBufferPosn;
				if(templen > count)
				{
					templen = count;
				}
				Array.InternalCopy(inBuffer, inBufferPosn,
						   buffer, offset, templen);
				inBufferPosn += templen;
				offset += templen;
				count -= templen;
				len += templen;
			}
			return len;
		}

		public override int ReadByte()
		{
			/* note: I've taken the easy way out here */
			byte[] buf = new byte[1];
			if(Read(buf,0,1) == 1)
			{
				return buf[0];
			}
			return -1;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			Flush();
			return stream.Seek(offset,origin);
		}

		public override void SetLength(long value)
		{
			Flush();
			stream.SetLength(value);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if(array == null)
			{
				throw new ArgumentNullException("array");
			}
			if(offset < 0)
			{
				throw new ArgumentOutOfRangeException
					("offset", _("ArgRange_Array"));
			}
			if(count < 0)
			{
				throw new ArgumentOutOfRangeException
					("count", _("ArgRange_Array"));
			}
			if((array.Length - offset) < count)
			{
				throw new ArgumentException
					(_("Arg_InvalidArrayRange"));
			}

			if(readMode)
			{
				Flush(); /* seek back if possible */
				readMode=false;
			}

			int spaceInBuffer=bufferSize-outBufferLen;
			/* simple cases first ;) */
			if(spaceInBuffer > count)
			{
				Array.InternalCopy(array, offset, outBuffer,outBufferLen, count);
				outBufferLen+=count;
				return;
			}
			// so we have more data than we can store directly , try making
			// room by moving to the front.
			if(outBufferPosn < outBufferLen)
			{
				Array.InternalCopy(outBuffer, outBufferPosn, outBuffer, 0, 
							outBufferLen-outBufferPosn);
				outBufferLen -= outBufferPosn;	
				outBufferPosn = 0;
			}
			else
			{
				/* empty buffer */
				outBufferLen=0;
				outBufferPosn=0;
			}
			spaceInBuffer=bufferSize-outBufferLen;
			if(spaceInBuffer > count)
			{
				Array.InternalCopy(array, offset, outBuffer, outBufferLen, count);
				outBufferLen+=count;
				return;
			}
			// now we know that the data is too large to fit in the 
			// buffer in anyway, so flush
			Flush();
			// calculate what will be left in the buffer
			int keptData= count % bufferSize;
			if(keptData!=0)
			{
				stream.Write(array,offset,count-keptData);
				Array.InternalCopy(array, offset + count - keptData, 
							outBuffer, outBufferPosn, keptData);
				outBufferLen+=keptData;
			}
			else
			{
				/* we have got full chunks to write , yay ! 
				   and we have a Flushed buffer 
				 */
				stream.Write(array,offset,count);
			}
		}

		public override void WriteByte(byte value)
		{
			byte []buf=new byte[1];
			buf[0]=value;
		
			Write(buf,0,1);
		}

		public override bool CanRead 
		{
 			get
			{
				return stream.CanRead;
			}
 		}

		public override bool CanSeek 
		{
 			get
			{
				return stream.CanSeek;
			}
 		}

		public override bool CanWrite 
		{
 			get
			{
				return stream.CanWrite;
			}
 		}

		public override long Length 
		{
 			get
			{
				long streamLength = stream.Length;

				if(readMode)
				{
					long inLength = (long)(inBufferPosn + inBufferLen);
			
					return inLength > streamLength ? inLength : streamLength;
				}
				else
				{
					long outLength = (long)(outBufferPosn + outBufferLen);

					return outLength > streamLength ? outLength : streamLength;
				}
			}
 		}

		public override long Position 
		{
 			get
			{
				/* note : virtual position ? */
				return stream.Position - (inBufferLen-inBufferPosn);
			}
 			set
			{
				Flush();
				stream.Position = value;
			}
 		}

	}
#endif
}//namespace
