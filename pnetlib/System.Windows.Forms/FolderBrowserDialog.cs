/*
 * FolderBrowserDialog.cs - Implementation of the
 *			"System.Windows.Forms.FolderBrowserDialog" class.
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
using System.Drawing;
using System.ComponentModel;

public sealed class FolderBrowserDialog : CommonDialog
{
	// Internal state.
	private FolderBrowserDialogForm form;
	private String description;
	private String selectedPath;
#if !ECMA_COMPAT
	private Environment.SpecialFolder rootFolder;
#endif
	private bool showHelp;
	private bool showNewFolderButton;

	// Constructor.
	public FolderBrowserDialog()
			{
				// Make sure that the dialog fields have their default values.
				Reset();
			}

	// Get or set this object's properties.
	public String Description
			{
				get
				{
					return description;
				}
				set
				{
					if(description != value)
					{
						if(value == null)
						{
							value = String.Empty;
						}
						description = value;
						if(form != null)
						{
							form.UpdateDialog();
						}
					}
				}
			}
#if !ECMA_COMPAT
	public Environment.SpecialFolder RootFolder
			{
				get
				{
					return rootFolder;
				}
				set
				{
					if(rootFolder != value)
					{
						rootFolder = value;
						if(form != null)
						{
							form.UpdateDialog();
						}
					}
				}
			}
#endif
	public String SelectedPath
			{
				get
				{
					return selectedPath;
				}
				set
				{
					if(selectedPath != value)
					{
						if(value == null)
						{
							value = String.Empty;
						}
						selectedPath = value;
						if(form != null)
						{
							form.UpdateDialog();
						}
					}
				}
			}
	public bool ShowHelp
			{
				get
				{
					return showHelp;
				}
				set
				{
					if(showHelp != value)
					{
						showHelp = value;
						if(form != null)
						{
							form.UpdateDialog();
						}
					}
				}
			}
	public bool ShowNewFolderButton
			{
				get
				{
					return showNewFolderButton;
				}
				set
				{
					if(showNewFolderButton != value)
					{
						showNewFolderButton = value;
						if(form != null)
						{
							form.UpdateDialog();
						}
					}
				}
			}

	// Reset the dialog box controls to their default values.
	public override void Reset()
			{
				description = String.Empty;
			#if !ECMA_COMPAT
				rootFolder = Environment.SpecialFolder.Desktop;
			#endif
				selectedPath = String.Empty;
				showHelp = false;
				showNewFolderButton = true;
				if(form != null)
				{
					form.UpdateDialog();
				}
			}

	// Run the dialog box, with a particular parent owner.
	protected override bool RunDialog(IntPtr hWndOwner)
			{
				// This version is not used in this implementation.
				return false;
			}
	internal override DialogResult RunDialog(IWin32Window owner)
			{
				// If the dialog is already visible, then bail out.
				if(form != null)
				{
					return DialogResult.Cancel;
				}

				// Construct the folder browser dialog form.
				form = new FolderBrowserDialogForm(this);

				// Run the dialog and get its result.
				DialogResult result;
				try
				{
					result = form.ShowDialog(owner);
				}
				finally
				{
					form.DisposeDialog();
					form = null;
				}

				// Return the final dialog result to the caller.
				return result;
			}

	// Form that represents the folder browser dialog.
	private class FolderBrowserDialogForm : Form
	{
		// Internal state.
		private FolderBrowserDialog dialog;

		// Constructor.
		public FolderBrowserDialogForm(FolderBrowserDialog dialog)
				{
					this.dialog = dialog;
					// TODO: create the form
				}

		// Dispose of this dialog.
		public void DisposeDialog()
				{
					Dispose(true);
				}

		// Update the contents of the dialog to match the properties.
		public void UpdateDialog()
				{
					// TODO
				}

	}; // class FolderBrowserDialogForm

}; // class FolderBrowserDialog

}; // namespace System.Windows.Forms
