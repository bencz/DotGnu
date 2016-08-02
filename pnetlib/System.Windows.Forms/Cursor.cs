/*
 * Cursor.cs - Implementation of the "System.Windows.Forms.Cursor" class.
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

namespace System.Windows.Forms
{
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Toolkit;
using System.Security;
using System.Runtime.Serialization;

// Behind the scenes a user-defined cursor is just an icon.  The image
// loading routines in "DotGNU.Images" can load any type of file, be it
// .cur, .bmp, .ico, etc, and use it as a cursor.

#if CONFIG_SERIALIZATION
[Serializable]
public class Cursor : IDisposable, ISerializable
#else
public class Cursor : IDisposable
#endif
{
	// Internal state.
	private ToolkitCursorType type;
	private Icon icon;
	private static Cursor desktopCursor;

	// Constructors.
	internal Cursor(ToolkitCursorType type) 
			{
				this.type = type;
				this.icon = null;
			}
	public Cursor(IntPtr handle)
			{
				// This form of cursor creation is dangerous and not supported.
				throw new SecurityException();
			}
	public Cursor(Stream stream)
			{
				this.type = ToolkitCursorType.UserDefined;
				this.icon = new Icon(stream);
			}
	public Cursor(String fileName)
			{
				this.type = ToolkitCursorType.UserDefined;
				this.icon = new Icon(fileName);
			}
	public Cursor(Type type, String resource)
			{
				this.type = ToolkitCursorType.UserDefined;
				this.icon = new Icon(type, resource);
			}
	internal Cursor(ToolkitCursorType cursorType, Type type, String resource)
			{
				this.type = cursorType;
				this.icon = new Icon(type, resource);
			}
#if CONFIG_SERIALIZATION
	[TODO]
	internal Cursor(SerializationInfo info, StreamingContext context)
			{
				return;
			}
#endif

	// Destructor.
	~Cursor()
			{
				Dispose();
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				if(icon != null)
				{
					icon.Dispose();
				}
				icon = null;
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerializable interface.
	[TODO]
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				return;
			}

#endif

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return (int)type;
			}

	// Get the cursor's handle.
	public IntPtr Handle
			{
				get
				{
					// We don't use handles in this implementation.
					return IntPtr.Zero;
				}
			}

	// Get or the cursor's clipping bounds.  This returns a dummy value
	// because the desktop cursor clip is incredibly security-sensitive.
	public static Rectangle Clip
			{
				get
				{
					return Screen.PrimaryScreen.Bounds;
				}
				set
				{
					// Nothing to do here.
				}
			}

	// Get or set the current desktop cursor.  This returns a dummy value
	// because the desktop cursor is incredibly security-sensitive.
	public static Cursor Current
			{
				get
				{
					lock(typeof(Cursor))
					{
						if(desktopCursor == null)
						{
							desktopCursor = Cursors.Arrow;
						}
						return desktopCursor;
					}
				}
				set
				{
					lock(typeof(Cursor))
					{
						desktopCursor = value;
					}
				}
			}

	// Get or set the current position of the desktop cursor.
	public static Point Position
			{
				get
				{
					return Control.MousePosition;
				}
				set
				{
					// Too dangerous, so not supported in this implementation.
				}
			}

	// Get the size of the cursor object.
	public Size Size
			{
				get
				{
					if(icon != null)
					{
						return icon.Size;
					}
					else
					{
						return SystemInformation.CursorSize;
					}
				}
			}

	// Make a copy of this cursor's handle.
	public IntPtr CopyHandle()
			{
				// Not used in this implementation.
				return IntPtr.Zero;
			}

	// Draw the cursor.
	public void Draw(Graphics g, Rectangle targetRect)
			{
				if(icon != null)
				{
					g.DrawIconUnstretched(icon, targetRect);
				}
			}
	public void DrawStretched(Graphics g, Rectangle targetRect)
			{
				if(icon != null)
				{
					g.DrawIcon(icon, targetRect);
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(object obj) 
			{
				Cursor other = (obj as Cursor);
				if(other != null)
				{
					if(other.type != type)
					{
						return false;
					}
					return (other.icon == icon);
				}
				else
				{
					return false;
				}
			}

	// Hide the desktop's cursor.
	public static void Hide()
			{
				// Nothing to do here: hiding the desktop cursor is
				// incredibly security sensitive.
			}

	// Show the desktop's cursor.
	public static void Show()
			{
				// Since we cannot hide the desktop cursor, there
				// is nothing that we need to do to show it again.
			}

	// Convert this cursor into a string.
	public override String ToString()
			{
				if(type == ToolkitCursorType.UserDefined && icon != null)
				{
					return "[Cursor: " + icon.Width + ", " + icon.Height + "]";
				}
				else
				{
					return "[Cursor: " + type + "]";
				}
			}

	// Equality and inequality operators on cursors.
	public static bool operator==(Cursor left, Cursor right)
			{
				if(((Object)left) != null)
				{
					return left.Equals(right);
				}
				else
				{
					return (((Object)right) == null);
				}
			}
	public static bool operator!=(Cursor left, Cursor right)
			{
				if(((Object)left) != null)
				{
					return !left.Equals(right);
				}
				else
				{
					return (((Object)right) != null);
				}
			}

	// Set this cursor on a toolkit window.
	internal void SetCursorOnWindow(IToolkitWindow window)
			{
				if(icon != null)
				{
					window.SetCursor(type, ToolkitManager.GetImageFrame(icon));
				}
				else
				{
					window.SetCursor(type, null);
				}
			}

}; // class Cursor

}; // namespace System.Windows.Forms
