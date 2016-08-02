/*
 * ColorDialog.cs - Implementation of the
 *			"System.Windows.Forms.ColorDialog" class.
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

[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
[DefaultProperty("Color")]
#endif
public class ColorDialog : CommonDialog
{
	// Internal state.
	private ColorDialogForm form;
	private bool allowFullOpen;
	private bool anyColor;
	private bool fullOpen;
	private bool showHelp;
	private bool solidColorOnly;
	private Color color;
	private int[] customColors;

	// Constructor.
	public ColorDialog()
			{
				// Make sure that the dialog fields have their default values.
				Reset();
			}

	// Get or set this object's properties.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif
	public virtual bool AllowFullOpen
			{
				get
				{
					return allowFullOpen;
				}
				set
				{
					allowFullOpen = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public virtual bool AnyColor
			{
				get
				{
					return anyColor;
				}
				set
				{
					anyColor = value;
				}
			}
	public Color Color
			{
				get
				{
					return color;
				}
				set
				{
					if(color != value)
					{
						color = value;
						if(form != null)
						{
							form.ChangeColor();
						}
					}
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int[] CustomColors
			{
				get
				{
					return customColors;
				}
				set
				{
					customColors = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public virtual bool FullOpen
			{
				get
				{
					return fullOpen;
				}
				set
				{
					fullOpen = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public virtual bool ShowHelp
			{
				get
				{
					return showHelp;
				}
				set
				{
					showHelp = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public virtual bool SolidColorOnly
			{
				get
				{
					return solidColorOnly;
				}
				set
				{
					solidColorOnly = value;
				}
			}

	// Reset the dialog box controls to their default values.
	public override void Reset()
			{
				allowFullOpen = true;
				anyColor = false;
				fullOpen = false;
				solidColorOnly = false;
				color = Color.Black;
				customColors = null;
				if(form != null)
				{
					form.ChangeColor();
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

				// Construct the color dialog form.
				form = new ColorDialogForm(this);

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

	// Convert this object into a string.
	public override string ToString()
			{
				return base.ToString() + ", Color: " + Color.ToString();
			}

	// Form that represents the color dialog.
	private class ColorDialogForm : Form
	{
		// Internal state.
		private ColorDialog dialog;

		[TODO]
		// Constructor.
		public ColorDialogForm(ColorDialog dialog)
				{
					this.dialog = dialog;
					// create the form
				}

		// Dispose of this dialog.
		public void DisposeDialog()
				{
					Dispose(true);
				}

		[TODO]
		// Change the color that is displayed in the dialog.
		public void ChangeColor()
				{
					return;
				}

	}; // class ColorDialogForm

}; // class ColorDialog

}; // namespace System.Windows.Forms
