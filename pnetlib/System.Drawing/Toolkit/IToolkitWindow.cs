/*
 * IToolkitWindow.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitWindow" class.
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

using DotGNU.Images;

// This interface provides a primitive windowing facility for
// managing rectangular regions in a stack.  It can be used to
// create owner-draw widgets and the like.

[NonStandardExtra]
public interface IToolkitWindow : IDisposable
{
	// Get the toolkit that owns this window.
	IToolkit Toolkit { get; }

	// Get the toolkit parent window.
	IToolkitWindow Parent { get; }

	// Get the current dimensions of this window.
	Rectangle Dimensions { get; }

	// Get or set the mapped state of the window.
	bool IsMapped { get; set; }

	// Determine if this window currently has the input focus.
	bool Focused { get; }

	// Get or set the mouse capture on this window.
	bool Capture { get; set; }

	// Set the focus to this window.
	void Focus();

	// Destroy this window and all of its children.
	void Destroy();

	// Move or resize this window.
	void MoveResize(int x, int y, int width, int height);

	// Raise this window respective to its siblings.
	void Raise();

	// Lower this window respective to its siblings.
	void Lower();

	// Reparent this window to underneath a new parent.
	void Reparent(IToolkitWindow parent, int x, int y);

	// Get a toolkit graphics object for this window.
	IToolkitGraphics GetGraphics();

	// Set the foreground of the window to a solid color.
	void SetForeground(Color color);

	// Set the background of the window to a solid color.
	void SetBackground(Color color);

	// Move this window to above one of its siblings.
	void MoveToAbove(IToolkitWindow sibling);

	// Move this window to below one of its siblings.
	void MoveToBelow(IToolkitWindow sibling);

	// Get the HWND for this window.  IntPtr.Zero if not supported.
	IntPtr GetHwnd();

	// Invalidate this window.
	void Invalidate();

	// Invalidate a rectangle within this window.
	void Invalidate(int x, int y, int width, int height);

	// Force an update of all invalidated regions.
	void Update();

	// Set the cursor.  The toolkit may ignore "frame" if it already
	// has a system-defined association for "cursorType".  Setting
	// "cursorType" to "ToolkitCursorType.InheritParent" will reset
	// the cursor to be the same as the parent window's.
	void SetCursor(ToolkitCursorType cursorType, Frame frame);

	void SendBeginInvoke(IntPtr i_gch);

}; // interface IToolkitWindow

}; // namespace System.Drawing.Toolkit
