/*
 * SaveFileDialog.cs - Implementation of the
 *			"System.Windows.Forms.SaveFileDialog" class.
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

using System.IO;

public sealed class SaveFileDialog : FileDialog
{
	// Internal state.
	private bool createPrompt;
	private bool overwritePrompt;

	// Constructor
	public SaveFileDialog() {}

	// Get or set this object's properties.
	public bool CreatePrompt
			{
				get
				{
					return createPrompt;
				}
				set
				{
					createPrompt = value;
				}
			}
	public bool OverwritePrompt
			{
				get
				{
					return overwritePrompt;
				}
				set
				{
					overwritePrompt = value;
				}
			}
	internal override String DefaultTitle
			{
				get
				{
					return S._("SWF_FileDialog_SaveAsTitle", "Save As");
				}
			}
	internal override String OkButtonName
			{
				get
				{
					return S._("SWF_MessageBox_SaveButton", "Save");
				}
			}

	// Open the file specified by this dialog box.
	public Stream OpenFile()
			{
				String filename = FileName;
				if(filename == null || filename.Length == 0)
				{
					throw new ArgumentNullException("FileName");
				}
				return new FileStream(filename, FileMode.Create,
									  FileAccess.ReadWrite);
			}

	// Reset the contents of the dialog box.
	internal override void ResetInternal()
			{
				base.ResetInternal();
				createPrompt = false;
				overwritePrompt = true;
			}
	public override void Reset()
			{
				base.Reset();
			}

}; // class SaveFileDialog

}; // namespace System.Windows.Forms
