/*
 * Clipboard.cs - Widget class that manages an X clipboard.
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

namespace Xsharp
{

using System;
using System.Text;
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Clipboard"/> class manages clipboards
/// within the X system.</para>
/// </summary>
public class Clipboard : InputOutputWidget
{
	// Internal state.
	private XAtom name;
	private XAtom targets;
	private String[] formats;
	private byte[][] values;

	/// <summary>
	/// <para>Construct a new clipboard, associated with a particular
	/// screen and selection name.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen to attach the clipboard to, or <see langword="null"/>
	/// to use the default screen.</para>
	/// </param>
	///
	/// <param name="name">
	/// <para>The name of the selection to use for clipboard access.
	/// This is usually <c>PRIMARY</c> for the X selection or
	/// <c>CLIPBOARD</c> for the explicit copy/cut/paste clipboard.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="name"/> parameter is
	/// <see langword="null"/>.</para>
	/// </exception>
	public Clipboard(Screen screen, String name)
			: base(TopLevelWindow.GetRoot(screen), -1, -1, 1, 1)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				try
				{
					IntPtr display = dpy.Lock();
					this.name = Xlib.XInternAtom
						(display, name, XBool.False);
					this.targets = Xlib.XInternAtom
						(display, "TARGETS", XBool.False);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Determine if we currently own the clipboard.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if this process currently
	/// has ownership of the clipboard.</para>
	/// </value>
	public bool OwnsClipboard
			{
				get
				{
					try
					{
						IntPtr display = dpy.Lock();
						XWindow handle = GetWidgetHandle();
						if(Xlib.XGetSelectionOwner (display, name) == handle)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get the list of formats that are currently stored on the
	/// clipboard.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns an array of MIME types corresponding to the types
	/// of values on the clipboard.</para>
	/// </returns>
	public String[] GetFormats()
			{
				// Bail out early if we own the clipboard ourselves.
				if(OwnsClipboard)
				{
					return formats;
				}

				// TODO

				// Could not get the format list.
				return new String [0];
			}

	/// <summary>
	/// <para>Get clipboard data in a particular MIME format.</para>
	/// </summary>
	///
	/// <param name="format">
	/// <para>The MIME format to request data for, or <see langword="null"/>
	/// for plain text.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The format's data, or <see langword="null"/> if data is
	/// not available in the specified format.</para>
	/// </returns>
	public byte[] GetData(String format)
			{
				int posn;

				// If no format is supplied, then assume "text/plain".
				if(format == null)
				{
					format = "text/plain";
				}

				// Search our list of values if we have the clipboard.
				if(OwnsClipboard)
				{
					if(formats == null)
					{
						return null;
					}
					for(posn = 0; posn < formats.Length; ++posn)
					{
						if(formats[posn] == format)
						{
							return values[posn];
						}
					}
					return null;
				}

				// Ask the current clipboard owner for the data.
				// TODO

				// Could not get the data in the requested format.
				return null;
			}

	/// <summary>
	/// <para>Get clipboard data in string format.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>The string data on the clipboard, or <see langword="null"/>
	/// if there is no string data on the clipboard.</para>
	/// </returns>
	///
	/// <remarks>
	/// <para>This is a convenience routine to fetch data in
	/// <c>text/plain</c> format.</para>
	/// </remarks>
	public String GetStringData()
			{
				byte[] data = GetData("text/plain");
				if(data != null)
				{
					return Encoding.UTF8.GetString(data);
				}
				return null;
			}

	/// <summary>
	/// <para>Set the contents of the clipboard to a specific set of
	/// formats and values.</para>
	/// </summary>
	///
	/// <param name="formats">
	/// <para>The data formats that make up the data that is being placed
	/// onto the clipboard.</para>
	/// </param>
	///
	/// <param name="values">
	/// <para>The data values that make up the data that is being placed
	/// onto the clipboard.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Either <paramref name="formats"/> or <paramref name="values"/>
	/// is <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentException">
	/// <para>The lengths of <paramref name="formats"/> and
	/// <paramref name="values"/> do not match.</para>
	/// </exception>
	public void SetData(String[] formats, byte[][] values)
			{
				// Validate the parameters.
				if(formats == null)
				{
					throw new ArgumentNullException("formats");
				}
				if(values == null)
				{
					throw new ArgumentNullException("values");
				}
				if(formats.Length != values.Length)
				{
					throw new ArgumentException
						(S._("X_MismatchedClipboardLists"));
				}

				// Make a copy of the values that we were supplied.
				this.formats = formats;
				this.values = values;

				// Grab the selection for ourselves, since we're now the owner.
				try
				{
					IntPtr display = dpy.Lock();
					Xlib.XSetSelectionOwner
						(display, name, GetWidgetHandle(), dpy.knownEventTime);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				switch((EventType)(xevent.xany.type__))
				{
					case EventType.SelectionClear:
					{
						// We have lost ownership of the selection.
						formats = null;
						values = null;
					}
					break;

					case EventType.SelectionRequest:
					{
						// Bail out if we received a request from ourselves!
						if(xevent.xselectionrequest.requestor ==
								GetWidgetHandle())
						{
							break;
						}

						// Build the SelectionNotify event for the reply.
						XEvent response = new XEvent();
						response.xany.type = (int)(EventType.SelectionNotify);
						response.xselection.requestor =
							xevent.xselectionrequest.requestor;
						response.xselection.selection =
							xevent.xselectionrequest.selection;
						response.xselection.target =
							xevent.xselectionrequest.target;
						response.xselection.property = XAtom.Zero;
						response.xselection.time =
							xevent.xselectionrequest.time;

						// TODO

						// Transmit the response back to the requestor.
						try
						{
							IntPtr display = dpy.Lock();
							Xlib.XSendEvent
								(display,
								 xevent.xselectionrequest.requestor,
								 XBool.False,
								 (int)(EventMask.NoEventMask),
								 ref response);
						}
						finally
						{
							dpy.Unlock();
						}
					}
					break;
				}
				base.DispatchEvent(ref xevent);
			}

} // class Clipboard

} // namespace Xsharp
