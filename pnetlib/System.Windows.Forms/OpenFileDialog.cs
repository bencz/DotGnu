/*
 * OpenFileDialog.cs - Implementation of the
 *			"System.Windows.Forms.OpenFileDialog" class.
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
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[DesignerAttribute(
	"System.Windows.Forms.Design.OpenFileDialogDesigner, System.Design")]
#endif
public sealed class OpenFileDialog : FileDialog
{
	// Internal state.
	private bool multiselect;
	internal bool readOnlyChecked;
	private bool showReadOnly;

	// Constructor
	public OpenFileDialog() {}

	// Get or set this object's properties.

	#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
	#endif
	public override bool CheckFileExists
			{
				get
				{
					return base.CheckFileExists;
				}
				set
				{
					base.CheckFileExists = value;
				}
			}
	#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	#endif
	public bool Multiselect
			{
				get
				{
					return multiselect;
				}
				set
				{
					multiselect = value;
				}
			}
	#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	#endif
	public bool ReadOnlyChecked
			{
				get
				{
					return readOnlyChecked;
				}
				set
				{
					if(readOnlyChecked != value)
					{
						readOnlyChecked = value;
						if(form != null)
						{
							form.UpdateReadOnly();
						}
					}
				}
			}
	#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	#endif
	public bool ShowReadOnly
			{
				get
				{
					return showReadOnly;
				}
				set
				{
					if(showReadOnly != value)
					{
						showReadOnly = value;
						if(form != null)
						{
							form.UpdateReadOnly();
						}
					}
				}
			}
	internal override String DefaultTitle
			{
				get
				{
					return S._("SWF_FileDialog_OpenTitle", "Open");
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
				return new FileStream(filename, FileMode.Open,
									  FileAccess.Read, FileShare.Read);
			}

	// Reset the contents of the dialog box.
	internal override void ResetInternal()
			{
				base.ResetInternal();
				checkFileExists = true;
				multiselect = false;
				readOnlyChecked = false;
				showReadOnly = false;
			}
	public override void Reset()
			{
				base.Reset();
			}

}; // class OpenFileDialog

}; // namespace System.Windows.Forms
