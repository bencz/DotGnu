/*
 * Clipboard.cs - Implementation of the
 *			"System.Windows.Forms.Clipboard" class.
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

using System.Drawing.Toolkit;

public sealed class Clipboard
{
	// Internal state.
	private static IDataObject obj;

	// Cannot instantiate this class.
	private Clipboard() {}

	[TODO]
	// Get the current clipboard contents.
	public static IDataObject GetDataObject()
			{
				return obj;
			}

	// Set the current clipboard contents.
	public static void SetDataObject(Object data)
			{
				SetDataObject(data, false);
			}
	[TODO]
	public static void SetDataObject(Object data, bool copy)
			{
				if(data == null)
				{
					throw new ArgumentNullException("data");
				}
				// Fix
				if(data is IDataObject)
				{
					obj = (IDataObject)data;
				}
				else
				{
					obj = new DataObject(data);
				}
			}

	// Get the contents of the X selection, or null if none.  See the
	// comments in "IToolkitClipboard.cs" for a description of the correct
	// usage of the X selection.
	internal static String GetSelection()
			{
				IToolkitClipboard clipboard =
					ToolkitManager.Toolkit.GetClipboard();
				if(clipboard != null)
				{
					return clipboard.GetSelection();
				}
				else
				{
					return null;
				}
			}

	// Set the contents of the X selection.
	internal static void SetSelection(String text, int index, int count)
			{
				IToolkitClipboard clipboard =
					ToolkitManager.Toolkit.GetClipboard();
				if(clipboard != null)
				{
					clipboard.SetSelection(text, index, count);
				}
			}

}; // class Clipboard

}; // namespace System.Windows.Forms
