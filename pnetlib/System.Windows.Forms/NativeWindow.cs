/*
 * NativeWindow.cs - Implementation of the
 *			"System.Windows.Forms.NativeWindow" class.
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

#if !CONFIG_COMPACT_FORMS

// Not used in this implementation - this is incredibly
// insecure and non-portable.

public class NativeWindow : MarshalByRefObject
{
	// Internal state.
	private IntPtr handle;

	// Constructors.
	public NativeWindow() {}
	private NativeWindow(IntPtr handle)
			{
				this.handle = handle;
			}

	// Destructor.
	~NativeWindow() {}

	// Get the handle for this window.
	public IntPtr Handle
			{
				get
				{
					return handle;
				}
			}

	// Assign a handle to this window.
	public void AssignHandle(IntPtr handle)
			{
				this.handle = handle;
				OnHandleChange();
			}

	// Create a window handle with the specified parameters.
	public virtual void CreateHandle(CreateParams cp)
			{
				// Not used in this implementation.
			}

	// Invoke the default window procedure for
	public void DefWndProc(ref Message m)
			{
				// Not used in this implementation.
			}

	// Destroy the window and its handle.
	public virtual void DestroyHandle()
			{
				handle = IntPtr.Zero;
				OnHandleChange();
			}

	// Get the NativeWindow object for a specific handle.
	public static NativeWindow FromHandle(IntPtr handle)
			{
				return new NativeWindow(handle);
			}

	// Release the handle associated with this window.
	public virtual void ReleaseHandle()
			{
				handle = IntPtr.Zero;
				OnHandleChange();
			}

	// Notify subclasses of a handle change.
	protected virtual void OnHandleChange()
			{
				// Nothing to do here.
			}

	// Notify subclasses that an exception occured while dispatching
	// a message for this window.
	protected virtual void OnThreadException(Exception e)
			{
				// Nothing to do here.
			}

	// Window procedure for this native window.
	protected virtual void WndProc(ref Message m)
			{
				// Not used in this implementation.
			}

}; // class NativeWindow

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
