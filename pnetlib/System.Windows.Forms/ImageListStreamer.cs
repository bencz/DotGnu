/*
 * ImageListStreamer.cs - Implementation of the
 *				"System.Windows.Forms.ImageListStreamer" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Free Software Foundation, Inc.
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

namespace System.Windows.Forms
{

#if CONFIG_SERIALIZATION

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;

[Serializable]
public class ImageListStreamer : ISerializable
{
											/*  M      S      F      t */
	private static byte[] magicBytes = new byte[] { 0x4d , 0x53 , 0x46 , 0x74 };
	internal Image[] images = null;

	[TODO]
	internal ImageListStreamer(ImageList imageList)
	{
		return;
	}

	[TODO]
	private ImageListStreamer(SerializationInfo info, StreamingContext context)
	{
		byte [] data = (byte[])info.GetValue("Data", typeof(byte[]));
		MemoryStream ms = new MemoryStream(data, false); // readonly stream
		foreach(byte b in magicBytes)
		{
			if(ms.ReadByte() != b)
			{
				// Handle I18n
				throw new FormatException("invalid signature"); 
			}
		}
		Stream encodedStream = new RunLengthEncodedStream(ms);
		BinaryReader reader = new BinaryReader(encodedStream);
		try
		{
			ushort magic 	 = reader.ReadUInt16();
			ushort version 	 = reader.ReadUInt16();
			ushort currImage = reader.ReadUInt16();
			ushort maxImage	 = reader.ReadUInt16();
			ushort grow 	 = reader.ReadUInt16();
			ushort width	 = reader.ReadUInt16();
			ushort height	 = reader.ReadUInt16();
			uint bkcolor	 = reader.ReadUInt32();
			ushort flags	 = reader.ReadUInt16();
			short []overlays = new short [4];
			for(int i = 0; i < 4 ; i++)
			{
				overlays[i]  = reader.ReadInt16();
			}
			Image imagelist = Image.FromStream(reader.BaseStream); 
			if(imagelist is Bitmap)
			{
				int rowStep = imagelist.Width / width;
				images = new Image[currImage];
				for(int i = 0 ; i < currImage ; i++)
				{
					Rectangle imageArea = new Rectangle(
											(i % rowStep) / width,
											(i/rowStep) * height, 
											width,
											height);
					PixelFormat format = imagelist.PixelFormat;
					if((imagelist.PixelFormat & PixelFormat.Indexed) != 0)
					{
						// ImageCollection will reformat back to Indexed
						// if needed
						format = PixelFormat.Format24bppRgb;
					}
					images[i] = (imagelist as Bitmap).Clone(imageArea, 
															format);
				}
			}
			else
			{
				images = null;
			}
		}
		finally
		{
			reader.Close();
			encodedStream.Close();
			ms.Close();
		}
	}

	[TODO]
	public virtual void GetObjectData(SerializationInfo si, StreamingContext context)
	{
		return;
	}

	internal Image[] Images
	{
		get
		{
			return images;
		}
	}

	[TODO]
	// Move into DotGNU.Images
	private sealed class RunLengthEncodedStream : Stream
	{
		// Constructor.
		private Stream underlying = null;
		private int runByte = -1;
		private int runLength = 0;
		
		public RunLengthEncodedStream(Stream underlying) 
		{
			// create with a stream starting at the RLE data 
			// start and read along :)
			if(underlying == null)
			{
				throw new ArgumentNullException("underlying");
			}
			this.underlying = underlying;
		}

		public override void Flush() 
		{
			if(CanWrite)
			{
				underlying.Flush();
			}
		}

		[TODO]
		public override int Read(byte[] buffer, int offset, int count)
		{
			int len = 0;
			if(buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			else if(offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException
					("offset");
			}
			else if(count < 0)
			{
				throw new ArgumentOutOfRangeException
					("count");
			}
			else if((buffer.Length - offset) < count)
			{
				// Handle I18n
				throw new ArgumentException("Invalid array range");
			}

			len = count;
			while(len != 0)
			{
				if(runLength == 0)
				{
					// this is to ensure that runLength set by
					// previous reads are considered
					runLength = underlying.ReadByte();
					runByte = underlying.ReadByte();
				}
				if(runByte == -1 || runLength == -1)
				{
					break;
				}
				buffer[offset++] = (byte)runByte; 
				runLength--;
				len--;
			}
			
			// if len == count means EOF
			return (len != count) ? (count - len) : -1;
		}
		public override int ReadByte() 
		{ 		
			if(runLength > 0)
			{
				runLength--;
				return runByte;
			}

			runLength = underlying.ReadByte();
			runByte = underlying.ReadByte();
			runLength--; // decrement for one read
			return runByte;
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek");
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException("SetLength");
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			// maybe someday, not today
			throw new NotSupportedException("Write");
		}
		public override void WriteByte(byte value) 
		{
			// maybe someday, not today
			throw new NotSupportedException("WriteByte");
		}

		public override void Close()
		{
		}

		public override bool CanRead 
		{ 
			get 
			{
				return true; 
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
				return false;
			} 
		}
		
		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}

}; // class ImageListStreamer

#endif // CONFIG_SERIALIZATION

}; // namespace System.Windows.Forms
