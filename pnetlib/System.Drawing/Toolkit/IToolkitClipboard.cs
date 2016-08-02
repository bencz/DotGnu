/*
 * IToolkitClipboard.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitClipboard" class.
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

/*

This class supports two kinds of clipboard: the regular Windows-style
clipboard, and the X selection.  The X selection is a special text-only
clipboard that is set when the user highlights text in an edit control
without explicitly pressing CTRL+C.  The clipboard usage rules are
documented at freedesktop.org, and may be summarised as follows:

- When the user highlights a piece of text, set the X selection, but
  do not alter the Windows clipboard.
- When the user hits CTRL+C to copy, then set the Windows clipboard,
  but do not alter the X selection.
- When the user hits the middle mouse button, then paste the contents
  of the X selection at the current cursor position, but do not paste
  the contents of the Windows clipboard.
- When the user hits CTRL+V to paste, then paste the contents of the
  Windows clipboard, but do not paste the contents of the X selection.
- If the user removes the highlight, then do not set the X selection
  to empty - leave it as-is.
- The "Copy" and "Paste" menu options are considered equivalent to
  CTRL+C and CTRL+V respectively.  "Cut" is handled in the same way
  as "Copy" where clipboard operations are concerned.

Deviating from these rules in any way will cause X users to get confused.
Where the above rules differ from freedesktop.org's, then freedesktop.org's
rules will be considered the correct ones.

*/

[NonStandardExtra]
public interface IToolkitClipboard
{
	// Register a format name and get its clipboard identifier.
	// Returns -1 if the system does not use clipboard identifiers.
	int RegisterFormat(String name);

	// Get the format name for a particular clipboard identifier.
	// Returns null if the clipboard identifier is not recognized.
	String GetFormat(int id);

	// Set the contents of the X selection.
	void SetSelection(String text, int index, int count);

	// Get the contents of the X selection.
	String GetSelection();

	// Start a clipboard set operation.  If "copy" is "true", then the
	// data should persist beyond the lifetime of the current application.
	void StartSet(bool copy);

	// Set data for a particular format on the clipboard.
	void SetData(String format, byte[] data);

	// Set plain text string data on the clipboard
	void SetStringData(String text, int index, int count);

	// End a clipboard set operation.
	void EndSet();

	// Get the data formats that are currently on the clipboard.
	String[] GetFormats();

	// Get the data associated with a particular format; null if none.
	byte[] GetData(String format);

	// Get plain text string data from the clipboard; null if none.
	String GetStringData();

}; // interface IToolkitClipboard

}; // namespace System.Drawing.Toolkit
