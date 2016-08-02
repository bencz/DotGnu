/*
 * ImageList.cs - Implementation of the
 *			"System.Windows.Forms.ImageList" class.
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

using System.Collections;
#if CONFIG_COMPONENT_MODEL
using System.ComponentModel;
#endif
using System.Drawing;
using System.Drawing.Imaging;


[TODO]
public sealed class ImageList
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Variables
	private ColorDepth colorDepth = ColorDepth.Depth8Bit;
	private ImageCollection images;
	private Size imageSize = new Size(16,16);
#if CONFIG_SERIALIZATION
	private ImageListStreamer imageStream = null;
#endif
	private Color transparentColor = Color.Transparent;
	private Delegate rhHandler = null;

	// Constructor
	public ImageList() : base()
	{
		images = new ImageCollection(this);
	}
#if !CONFIG_COMPACT_FORMS && CONFIG_COMPONENT_MODEL
	public ImageList(IContainer container) : this() { container.Add(this); }
#endif

	// Properties
#if !CONFIG_COMPACT_FORMS
	public ColorDepth ColorDepth
	{
		get { return colorDepth; }
		set
		{
			colorDepth = value;
			OnRecreateHandle();
		}
	}
	public IntPtr Handle { get { return IntPtr.Zero; } }
	public bool HandleCreated { get { return true; } }
#endif
	public ImageCollection Images { get { return images; } }
	public Size ImageSize
	{
		get { return imageSize; }
		set
		{
			if (value.Height <= 0 || value.Width <= 0 ||
			    value.Height > 256 || value.Width > 256)
			{
				throw new ArgumentException(/* TODO */);
			}
			imageSize = value;
			OnRecreateHandle();
		}
	}
#if !CONFIG_COMPACT_FORMS
#if CONFIG_SERIALIZATION
	public ImageListStreamer ImageStream
	{
		get
		{
			if (imageStream == null)
			{
				imageStream = new ImageListStreamer(this);
			}
			return imageStream;
		}
		set
		{
			imageStream = value;
			images = new ImageCollection(this);
			foreach(Image img in imageStream.Images)
			{
				images.Add(img);
			}
			OnRecreateHandle();
		}
	}
#endif
	public Color TransparentColor
	{
		get { return transparentColor; }
		set { transparentColor = value; }
	}
#endif

	// Methods
#if CONFIG_COMPONENT_MODEL
	protected override void Dispose(bool disposing)
#else
	public void Dispose(bool disposing)
#endif
	{
		images.Dispose();
	}
#if !CONFIG_COMPACT_FORMS
	public void Draw(Graphics g, Point pt, int index)
	{
		Draw(g,pt.X,pt.Y,index);
	}
	public void Draw(Graphics g, int x, int y, int index)
	{
		g.DrawImage(images[index],x,y,imageSize.Width,imageSize.Height);
	}
	public void Draw(Graphics g, int x, int y, int width, int height, int index)
	{
		g.DrawImage(images[index],x,y,width,height);
	}
#endif
	private void OnRecreateHandle()
	{
		EventHandler handler = (rhHandler as EventHandler);
		if (handler != null)
		{
			handler(this,EventArgs.Empty);
		}
	}

	[TODO]
	public override string ToString()
	{
		// Handle
		return base.ToString();
	}

	// Events
#if !CONFIG_COMPACT_FORMS
	public
#else
	internal
#endif
	event EventHandler RecreateHandle
	{
		add
		{
			rhHandler = Delegate.Combine(rhHandler,value);
		}
		remove
		{
			rhHandler = Delegate.Remove(rhHandler,value);
		}
	}

	[TODO]
	public sealed class ImageCollection : IList
	{
		// Variables
		private ImageList owner;
		private ArrayList images = new ArrayList();

		// Constructor
		internal ImageCollection(ImageList owner) { this.owner = owner; }

		// Properties
		public int Count { get { return images.Count; } }
		public bool IsReadOnly { get { return false; } }
	#if !CONFIG_COMPACT_FORMS
		public bool Empty { get { return images.Count == 0; } }
		public Image this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException(/* TODO */);
				}
				return (Image) images[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException(/* TODO */);
				}
				if (value == null || !(value is Bitmap))
				{
					throw new ArgumentNullException(/* TODO */);
				}
				((Bitmap) value).MakeTransparent(owner.TransparentColor);
				images[index] = value;
			}
		}
	#endif

		// Methods
		public void Add(Icon icon)
		{
			Image image;

			if (icon == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if ((image = icon.ToBitmap()) == null)
			{
				throw new ArgumentException(/* TODO */);
			}
		#if !CONFIG_COMPACT_FORMS
			((Bitmap) image).MakeTransparent(owner.TransparentColor);
		#endif
			AddImage(image);
		}

		// Add the image as the correct size and depth
		private int AddImage(Image image)
		{
			PixelFormat format = FormatFromDepth(owner.ColorDepth);
			Image newImage;
			// Create the image we will write to.
			using (Image tempNewImage = image.Reformat(format))
			{
				Size size = owner.imageSize;
				newImage = tempNewImage.Resize(size.Width, size.Height);
			}
			return images.Add(newImage);
		}

		private PixelFormat FormatFromDepth(ColorDepth depth)
		{
			switch (depth)
			{
				case(ColorDepth.Depth4Bit):
					return PixelFormat.Format4bppIndexed;
				case (ColorDepth.Depth8Bit):
					return PixelFormat.Format8bppIndexed;
				case (ColorDepth.Depth16Bit):
					return PixelFormat.Format16bppRgb555;
				case (ColorDepth.Depth24Bit):
					return PixelFormat.Format24bppRgb;
				case (ColorDepth.Depth32Bit):
					return PixelFormat.Format32bppRgb;
				default:  // Never reached
					return 0;
			}
		}

		public void Add(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (!(image is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
		#if !CONFIG_COMPACT_FORMS
			((Bitmap) image).MakeTransparent(owner.TransparentColor);
		#endif
			AddImage(image);
		}
	#if !CONFIG_COMPACT_FORMS
		public int Add(Image image, Color transparentColor)
		{
			if (image == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (!(image is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
			((Bitmap) image).MakeTransparent(transparentColor);
			return AddImage(image);
		}
		public int AddStrip(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (!(image is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
			//if (The attempt to add the image failed.
			//    || The width of the image strip is zero or != to the existing image width.
			//    || The height of the image strip is != to the existing image height.)
			//	throw new Exception("TODO");
			//	return -1; // lol
			if (image.Width == 0 ||
			    image.Width%owner.ImageSize.Width != 0 ||
			    image.Height != owner.ImageSize.Height)
			{
				throw new Exception(/* TODO */);
			}
			// TODO
			return -1;
		}
	#endif
		public void Clear()
		{
			images.Clear();
		}
	#if !CONFIG_COMPACT_FORMS
		public bool Contains(Image image)
		{
			throw new NotSupportedException(/* TODO */);
		}
	#endif
		public IEnumerator GetEnumerator()
		{
			return images.GetEnumerator();
		}
	#if !CONFIG_COMPACT_FORMS
		public int IndexOf(Image image)
		{
			throw new NotSupportedException(/* TODO */);
		}

		public void Remove(Image image)
		{
			throw new NotSupportedException(/* TODO */);
		}
	#endif
		public void RemoveAt(int index)
		{
			if (index < 0 || index > Count)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			images.RemoveAt(index);
		}

		// Properties (Explicit IList Implementation)
		bool IList.IsFixedSize { get { return false; } }
	#if !CONFIG_COMPACT_FORMS
		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (Image) value; }
		}
	#endif

		// Methods (Explicit IList Implementation)
		int IList.Add(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (!(o is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
		#if !CONFIG_COMPACT_FORMS
			((Bitmap) o).MakeTransparent(owner.TransparentColor);
		#endif
			return images.Add(o);
		}
		bool IList.Contains(object o)
		{
			if (!(o is Bitmap)) { return false; }
			return images.Contains(o);
		}
		int IList.IndexOf(object o)
		{
			if (!(o is Bitmap)) { return -1; }
			return images.IndexOf(o);
		}
		void IList.Insert(int index, object o)
		{
			if (!(o is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
			images.Insert(index, o);
		}
		void IList.Remove(object o)
		{
			if (!(o is Bitmap))
			{
				throw new ArgumentException(/* TODO */);
			}
			images.Remove(o);
		}

		// Properties (Explicit ICollection Implementation)
		bool ICollection.IsSynchronized
		{
			get { return images.IsSynchronized; }
		}
		object ICollection.SyncRoot
		{
			get { return images.SyncRoot; }
		}

		// Methods (Explicit ICollection Implementation)
		void ICollection.CopyTo(Array array, int index)
		{
			images.CopyTo(array,index);
		}

		internal void Dispose()
		{
			if (images == null)
				return;
			for (int i = 0; i < images.Count; i++)
				(images[i] as Image).Dispose();
		}

	}; // class ImageList.ImageCollection

}; // class ImageList

}; // namespace System.Windows.Forms
