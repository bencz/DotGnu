/*
 * IToolkitEventSink.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitEventSink" class.
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

namespace System.Drawing.Toolkit
{

// This interface is implemented by higher-level controls
// that need to receive events from an IToolkitWindow.

[NonStandardExtra]
public interface IToolkitEventSink
{
	// Event that is emitted for an expose on this window.
	void ToolkitExpose(Graphics graphics);

	// Event that is emitted when the mouse enters this window.
	void ToolkitMouseEnter();

	// Event that is emitted when the mouse leaves this window.
	void ToolkitMouseLeave();

	// Event that is emitted when the focus enters this window.
	void ToolkitFocusEnter();

	// Event that is emitted when the focus leaves this window.
	void ToolkitFocusLeave();

	// Event that is emitted when the primary focus enters this window.
	// This is only called on top-level windows.
	void ToolkitPrimaryFocusEnter();

	// Event that is emitted when the primary focus leaves this window.
	// This is only called on top-level windows.
	void ToolkitPrimaryFocusLeave();

	// Event that is emitted for a key down event.
	bool ToolkitKeyDown(ToolkitKeys key);

	// Event that is emitted for a key up event.
	bool ToolkitKeyUp(ToolkitKeys key);

	// Event that is emitted for a key character event.
	bool ToolkitKeyChar(char charCode);

	// Event that is emitted for a mouse down event.
	void ToolkitMouseDown
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		 int clicks, int x, int y, int delta);

	// Event that is emitted for a mouse up event.
	void ToolkitMouseUp
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		 int clicks, int x, int y, int delta);

	// Event that is emitted for a mouse hover event.
	void ToolkitMouseHover
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		 int clicks, int x, int y, int delta);

	// Event that is emitted for a mouse move event.
	void ToolkitMouseMove
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		 int clicks, int x, int y, int delta);

	// Event that is emitted for a mouse wheel event.
	void ToolkitMouseWheel
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		 int clicks, int x, int y, int delta);

	// Event that is emitted when the window is moved by
	// external means (e.g. the user dragging the window).
	void ToolkitExternalMove(int x, int y);

	// Event that is emitted when the window is resized by
	// external means (e.g. the user resizing the window).
	void ToolkitExternalResize(int width, int height);

	// Event that is emitted when the close button on a window
	// is selected by the user.
	void ToolkitClose();

	// Event that is emitted when the help button on a window
	// is selected by the user.
	void ToolkitHelp();

	// Event that is emitted when the window state changes.
	// The argument is the "int" version of a "FormWindowState" value.
	void ToolkitStateChanged(int state);

	// Event that is emitted when the active MDI child window changes.
	// The "child" parameter is null if a window has been deactivated.
	void ToolkitMdiActivate(IToolkitWindow child);

	// Event that is emitted when a custom messages comes in. This
	// custom message is sent by another thread, telling the controlling
	// thread to Invoke a delegate (which is eventually pointed to by this
	// IntPtr)
	void ToolkitBeginInvoke(IntPtr i_gch);

}; // interface IToolkitEventSink

}; // namespace System.Drawing.Toolkit
