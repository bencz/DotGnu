/*
 * Button.cs - Implementation of the
 *			"System.Windows.Forms.Button" class.
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

public class Button : ButtonBase, IButtonControl
{
	// Internal result.
	private DialogResult result;

	// Contructor.
	public Button()
			{
				// Turn off the standard click styles.
				SetStyle(ControlStyles.StandardClick |
						 ControlStyles.StandardDoubleClick, false);
			}

	// Get or set the dialog result associated with this button.
	public virtual DialogResult DialogResult
			{
				get
				{
					return result;
				}
				set
				{
					result = value;
				}
			}

	// Notify the button that it is now the default button.
	public virtual void NotifyDefault(bool value)
			{
				IsDefault = value;
			}

	// Perform a click on this control.
	public void PerformClick()
			{
				if(Visible && Enabled)
				{
					OnClick(EventArgs.Empty);
				}
			}

	// Process a button click.
	protected override void OnClick(EventArgs e)
			{
				// Notify the form of the dialog result if necessary.
				if(result != DialogResult.None)
				{
					Form form = (TopLevelControl as Form);
					if(form != null)
					{
						form.DialogResult = result;
					}
				}

				// Perform the default button click behaviour.
				base.OnClick(e);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", Text: " + Text;
			}

	// Get the create parameters.  Not used in this implementation.
	protected override CreateParams CreateParams
			{
				get
				{
					return base.CreateParams;
				}
			}

	// Handle the mouse up event to cause "Click" to be emitted.
	protected override void OnMouseUp(MouseEventArgs e)
			{
				if(button == e.Button && entered && pressed)
				{
					OnClick(EventArgs.Empty);
				}
				base.OnMouseUp(e);
			}

	// Process a key mnemonic.
	protected override bool ProcessMnemonic(char charCode)
			{
				if(IsMnemonic(charCode, Text))
				{
					Control hierarchy = Parent;
					while (hierarchy != null)
					{
						if (!hierarchy.Enabled || !hierarchy.Visible)
						{
							return base.ProcessMnemonic(charCode);
						}
						hierarchy = hierarchy.Parent;
					}

					PerformClick();
					return true;
				}
				else
				{
					return base.ProcessMnemonic(charCode);
				}
			}

	// Perform the default accessibility action for this control.
	internal override void DoDefaultAction()
			{
				PerformClick();
			}

}; // class Button

}; // namespace System.Windows.Forms
