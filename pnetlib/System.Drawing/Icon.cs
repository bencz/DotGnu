/*
 * Icon.cs - Implementation of the "System.Drawing.Icon" class.
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

namespace System.Drawing
{

using System.IO;
using System.Security;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Toolkit;
using DotGNU.Images;

#if !ECMA_COMPAT
[Serializable]
[ComVisible(false)]
#endif
#if CONFIG_COMPONENT_MODEL
[TypeConverter(typeof(IconConverter))]
#endif
#if CONFIG_COMPONENT_MODEL_DESIGN
[Editor("System.Drawing.Design.IconEditor, System.Drawing.Design",
		typeof(UITypeEditor))]
#endif
public sealed class Icon : MarshalByRefObject, ICloneable, IDisposable
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	private DotGNU.Images.Image image;
	internal DotGNU.Images.Frame frame;
	private int frameNum;
	private IToolkitImage toolkitImage;

	// Constructors.
	public Icon(Stream stream)
			{
				Load(stream);
			}
	public Icon(Stream stream, int width, int height)
			{
				Load(stream);
				SelectFrame(width, height);
			}
	public Icon(String filename)
			{
				FileStream stream = new FileStream
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
	public Icon(Icon original, Size size)
			: this(original, size.Width, size.Height) {}
	public Icon(Icon original, int width, int height)
			: this(original)
			{
				SelectFrame(width, height);
			}
	public Icon(Type type, String resource)
			{
				Stream stream = Bitmap.GetManifestResourceStream
					(type, resource);
				if(stream == null)
				{
					throw new ArgumentException(S._("Arg_UnknownResource"));
				}
				try
				{
					Load(stream);
				}
				finally
				{
					stream.Close();
				}
			}
	private Icon(Icon cloneFrom)
			{
				if(cloneFrom == null)
				{
					throw new ArgumentNullException("cloneFrom");
				}
				image = (DotGNU.Images.Image)(cloneFrom.image.Clone());
				frameNum = cloneFrom.frameNum;
				frame = image.GetFrame(frameNum);
			}
#if CONFIG_SERIALIZATION
	internal Icon(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				byte[] data = (byte[])
					(info.GetValue("IconData", typeof(byte[])));
				Size size = (Size)(info.GetValue("IconSize", typeof(Size)));
				Load(new MemoryStream(data));
				SelectFrame(size.Width, size.Height);
			}
#endif

	// Destructor.
	~Icon()
			{
				Dispose();
			}

	// Load the icon contents from a stream, and then set the
	// current frame to the first one in the icon image.
	private void Load(Stream stream)
			{
				image = new DotGNU.Images.Image();
				image.Load(stream);
				frame = image.GetFrame(0);
				frameNum = 0;
			}

	// Select a particular frame from this icon.
	private void SelectFrame(int width, int height)
			{
				int index;
				Frame frame;
				for(index = 0; index < image.NumFrames; ++index)
				{
					frame = image.GetFrame(index);
					if(frame.Width == width && frame.Height == height)
					{
						this.frame = frame;
						frameNum = index;
						return;
					}
				}
				frame = image.GetFrame(0);
				frameNum = 0;
			}

	// Get this icon's properties.
	public IntPtr Handle
			{
				get
				{
					// Not used in this implementation.
					return IntPtr.Zero;
				}
			}
	public int Height
			{
				get
				{
					if(frame != null)
					{
						return frame.Height;
					}
					else
					{
						return 0;
					}
				}
			}
	public Size Size
			{
				get
				{
					if(frame != null)
					{
						return new Size(frame.Width, frame.Height);
					}
					else
					{
						return Size.Empty;
					}
				}
			}
	public int Width
			{
				get
				{
					if(frame != null)
					{
						return frame.Width;
					}
					else
					{
						return 0;
					}
				}
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				return new Icon(this);
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				if(toolkitImage != null)
				{
					toolkitImage.Dispose();
					toolkitImage = null;
				}
				if(image != null)
				{
					image.Dispose();
					image = null;
					frame = null;
					frameNum = 0;
				}
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerializable interface.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				if(image != null)
				{
					MemoryStream stream = new MemoryStream();
					Save(stream);
					info.AddValue("IconData", stream.ToArray(), typeof(byte[]));
					info.AddValue("IconSize", Size, typeof(Size));
				}
			}

#endif

	// Convert a native icon handle into an "Icon" object.
	public static Icon FromHandle(IntPtr handle)
			{
				throw new SecurityException();
			}

	// Save this icon to a specified stream.
	public void Save(Stream outputStream)
			{
				if(image != null)
				{
					image.Save(outputStream, DotGNU.Images.Image.Icon);
				}
			}

	// Convert this object into a bitmap.
	public Bitmap ToBitmap()
			{
				if(image != null)
				{
					return new Bitmap(image.ImageFromFrame(frameNum));
				}
				else
				{
					return null;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "Icon: " + Width + ", " + Height;
			}

	// Get the toolkit image underlying this icon.
	internal IToolkitImage GetToolkitImage(Graphics graphics)
			{
				if(toolkitImage != null)
				{
					return toolkitImage;
				}
				else if(image != null)
				{
					toolkitImage =
						graphics.ToolkitGraphics.Toolkit.CreateImage
							(image, frameNum);
					return toolkitImage;
				}
				else
				{
					return null;
				}
			}

}; // class Icon

}; // namespace System.Drawing
