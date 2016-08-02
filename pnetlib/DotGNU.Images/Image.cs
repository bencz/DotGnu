/*
 * Image.cs - Implementation of the "DotGNU.Images.Image" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.Images
{

using System;
using System.IO;

public class Image : MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	private int width;
	private int height;
	internal PixelFormat pixelFormat;
	private int numFrames;
	private Frame[] frames;
	private String format;
	private int[] palette;
	private int transparentPixel;

	// Standard image formats.
	public const String Png = "png";
	public const String Jpeg = "jpeg";
	public const String Gif = "gif";
	public const String Tiff = "tiff";
	public const String Bmp = "bmp";
	public const String Icon = "icon";
	public const String Cursor = "cursor";
	public const String Exif = "exif";

	// Constructors.
	public Image()
			{
				this.width = 0;
				this.height = 0;
				this.pixelFormat = PixelFormat.Undefined;
				this.numFrames = 0;
				this.frames = null;
				this.format = null;
				this.palette = null;
				this.transparentPixel = -1;
			}

	public Image(int width, int height, PixelFormat pixelFormat)
			{
				this.width = width;
				this.height = height;
				this.pixelFormat = pixelFormat;
				this.numFrames = 0;
				this.frames = null;
				this.format = null;
				this.palette = null;
				this.transparentPixel = -1;
			}

	private Image(Image image, Frame thisFrameOnly) :
		this(image, image.PixelFormat)
			{
				if(thisFrameOnly != null)
				{
					this.numFrames = 1;
					this.frames = new Frame [1];
					this.frames[0] = thisFrameOnly.CloneFrame(this);
				}
				else
				{
					this.numFrames = image.numFrames;
					if(image.frames != null)
					{
						int frame;
						this.frames = new Frame [this.numFrames];
						for(frame = 0; frame < this.numFrames; ++frame)
						{
							this.frames[frame] =
								image.frames[frame].CloneFrame(this);
						}
					}
				}
				
			}

	private Image(Image image, PixelFormat format)
			{
				this.width = image.width;
				this.height = image.height;
				this.pixelFormat = image.pixelFormat;
				this.format = image.format;
				if(image.palette != null)
				{
					this.palette = (int[])(image.palette.Clone());
				}
				this.transparentPixel = image.transparentPixel;
			}

	// Destructor.
	~Image()
			{
				Dispose(false);
			}

	// Get or set the image's overall properties.  The individual frames
	// may have different properties from the ones stated here.
	public int Width
			{
				get
				{
					return width;
				}
				set
				{
					width = value;
				}
			}

	public int Height
			{
				get
				{
					return height;
				}
				set
				{
					height = value;
				}
			}

	public int NumFrames
			{
				get
				{
					return numFrames;
				}
			}

	public PixelFormat PixelFormat
			{
				get
				{
					return pixelFormat;
				}
				set
				{
					pixelFormat = value;
				}
			}

	public String LoadFormat
			{
				get
				{
					// Format the image was loaded in (e.g. "jpeg").
					// Returns a null value if created in-memory.
					return format;
				}
				set
				{
					format = value;
				}
			}

	public int[] Palette
			{
				get
				{
					// The palette for indexed images, null if an RGB image.
					return palette;
				}
				set
				{
					palette = value;
				}
			}

	public int TransparentPixel
			{
				get
				{
					// Index into "Palette" of the transparent pixel value.
					// Returns -1 if there is no transparent pixel specified.
					return transparentPixel;
				}
				set
				{
					transparentPixel = value;
				}
			}

	// Add a new frame to this image.
	public Frame AddFrame()
			{
				return AddFrame(width, height, pixelFormat);
			}

	public Frame AddFrame(int width, int height, PixelFormat pixelFormat)
			{
				Frame frame = new Frame(this, width, height, pixelFormat);
				frame.Palette = palette;
				frame.TransparentPixel = transparentPixel;
				return AddFrame(frame);
			}

	public Frame AddFrame(Frame frame)
			{
				if(frames == null)
				{
					frames = new Frame[] {frame};
					numFrames = 1;
				}
				else
				{
					Frame[] newFrames = new Frame [numFrames + 1];
					Array.Copy(frames, 0, newFrames, 0, numFrames);
					frames = newFrames;
					frames[numFrames] = frame;
					++numFrames;
				}
				return frame;
			}

	// Clone this object.
	public Object Clone()
			{
				return new Image(this, null);
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	protected virtual void Dispose(bool disposing)
			{
				if(frames != null)
				{
					int frame;
					for(frame = 0; frame < numFrames; ++frame)
					{
						frames[frame].Dispose();
					}
					frames = null;
					numFrames = 0;
				}
				palette = null;
				transparentPixel = -1;
			}

	// Get a particular frame within this image.
	public Frame GetFrame(int frame)
			{
				if(frame >= 0 && frame < numFrames && frames != null)
				{
					return frames[frame];
				}
				else
				{
					return null;
				}
			}

	public void SetFrame(int frame, Frame newFrame)
			{
				if(frame >= 0 && frame < numFrames && newFrame != null)
				{
					newFrame.NewImage(this);
					frames[frame] = newFrame;
				}
			}

	// Determine if it is possible to load a particular format.
	public static bool CanLoadFormat(String format)
			{
				return (format == Bmp || format == Icon ||
				        format == Cursor || format == Png ||
						format == Gif || format == Jpeg ||
						format == Exif);
			}

	// Determine if it is possible to save a particular format.
	public static bool CanSaveFormat(String format)
			{
				return (format == Bmp || format == Icon ||
				        format == Cursor || format == Png ||
						format == Gif || format == Jpeg);
			}

	// Load an image from a stream into this object.  This will
	// throw "FormatException" if the format could not be loaded.
	public void Load(String filename)
			{
				if(filename == null)
				{
					throw new ArgumentNullException("filename", "Argument cannot be null");
				}

				Stream stream = new FileStream
					(filename, FileMode.Open, FileAccess.Read);
				try
				{
					Load(stream);
				}
				finally
				{
					stream.Close();
				}
			}

	public void Load(Stream stream)
			{
				// Read the first 4 bytes from the stream to determine
				// what kind of image we are loading.
				byte[] magic = new byte [4];
				stream.Read(magic, 0, 4);
				if(magic[0] == (byte)'B' && magic[1] == (byte)'M')
				{
					// Windows bitmap image.
					BmpReader.Load(stream, this);
				}
				else if(magic[0] == 0 && magic[1] == 0 &&
						magic[2] == 1 && magic[3] == 0)
				{
					// Windows icon image.
					IconReader.Load(stream, this, false);
				}
				else if(magic[0] == 0 && magic[1] == 0 &&
						magic[2] == 2 && magic[3] == 0)
				{
					// Windows cursor image (same as icon, with hotspots).
					IconReader.Load(stream, this, true);
				}
				else if(magic[0] == 137 && magic[1] == 80 &&
						magic[2] == 78 && magic[3] == 71)
				{
					// PNG image.
					PngReader.Load(stream, this);
				}
				else if(magic[0] == (byte)'G' && magic[1] == (byte)'I' &&
						magic[2] == (byte)'F' && magic[3] == (byte)'8')
				{
					// GIF image.
					GifReader.Load(stream, this);
				}
				else if(magic[0] == (byte)0xFF && magic[1] == (byte)0xD8)
				{
					// JPEG or EXIF image.
					JpegReader.Load(stream, this, magic, 4);
				}
				else
				{
					// Don't know how to load this kind of file.
					throw new FormatException();
				}
			}

	// Save this image to a stream, in a particular format.
	// If the format is not specified, it defaults to "png".
	public void Save(String filename)
			{
				Stream stream = new FileStream
					(filename, FileMode.Create, FileAccess.Write);
				try
				{
					Save(stream, null);
				}
				finally
				{
					stream.Close();
				}
			}

	public void Save(String filename, String format)
			{
				Stream stream = new FileStream
					(filename, FileMode.Create, FileAccess.Write);
				try
				{
					Save(stream, format);
				}
				finally
				{
					stream.Close();
				}
			}

	public void Save(Stream stream)
			{
				Save(stream, null);
			}

	public void Save(Stream stream, String format)
			{
				// Select a default format for the save.
				if(format == null)
				{
					format = Png;
				}

				// Determine how to save the image.
				if(format == Bmp)
				{
					// Windows bitmap image.
					BmpWriter.Save(stream, this);
				}
				else if(format == Icon)
				{
					// Windows icon image.
					IconWriter.Save(stream, this, false);
				}
				else if(format == Cursor)
				{
					// Windows cursor image (same as icon, with hotspots).
					IconWriter.Save(stream, this, true);
				}
				else if(format == Png)
				{
					// PNG image.
					PngWriter.Save(stream, this);
				}
				else if(format == Gif)
				{
					// GIF image.  If the image is RGB, then we encode
					// as a PNG instead and hope that the image viewer
					// is smart enough to check the magic number before
					// decoding the image.
					if(GifWriter.IsGifEncodable(this))
					{
						GifWriter.Save(stream, this);
					}
					else
					{
						PngWriter.Save(stream, this);
					}
				}
				else if(format == Jpeg)
				{
					// JPEG image.
					JpegWriter.Save(stream, this);
				}
			}

	// Stretch this image to a new size.
	public Image Stretch(int width, int height)
			{
				Width = width;
				Height = height;
				Image newImage = null;
				for (int i = 0; i < frames.Length; i++)
				{
					if (i == 0)
						newImage = new  Image(this, frames[0].Scale(width, height));
					else
						newImage.AddFrame(frames[i].Scale(width, height));
				}
				return newImage;
			}

	// Create a new image that contains a copy of one frame from this image.
	public Image ImageFromFrame(int frame)
			{
				return new Image(this, GetFrame(frame));
			}

	public Image Reformat(PixelFormat newFormat)
			{
				Image newImage = new Image(this, newFormat);
				for (int i = 0; i < frames.Length; i++)
					newImage.AddFrame(frames[i].Reformat(newFormat));
				return newImage;
			}

}; // class Image

}; // namespace DotGNU.Images
