/*
 * CommonDialog.cs - Implementation of the
 *		"System.Windows.Forms.CommonDialog" class.
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

using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[ToolboxItemFilter("System.Windows.Forms")]
#endif
public abstract class CommonDialog
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Constructor.
	public CommonDialog() {}

	// Reset the dialog box controls to their default values.
	public abstract void Reset();

	// Run the dialog box, with a particular parent owner.
	// This method is not used in this implementation.
	protected abstract bool RunDialog(IntPtr hwndOwner);

	// Run the dialog box - internal version.
	internal virtual DialogResult RunDialog(IWin32Window owner)
			{
				return DialogResult.Cancel;
			}

	// Show the dialog box and wait for the answer.
	public DialogResult ShowDialog()
			{
				return RunDialog((IWin32Window)null);
			}
	public DialogResult ShowDialog(IWin32Window owner)
			{
				return RunDialog(owner);
			}

	// Emit the help request event.
	protected virtual void OnHelpRequest(EventArgs e)
			{
				if(HelpRequest != null)
				{
					HelpRequest(this, e);
				}
			}

	// Emit a help request.
	internal void EmitHelpRequest(EventArgs e)
			{
				OnHelpRequest(e);
			}

	// Event that is emitted when the help button is pressed in the dialog.
	public event EventHandler HelpRequest;

	// Hook procedure - not used in this implementation.
	protected virtual IntPtr HookProc(IntPtr hWnd, int msg,
									  IntPtr wparam, IntPtr lparam)
			{
				return IntPtr.Zero;
			}

	// Owner window procedure - not used in this implementation.
	protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg,
										  IntPtr wparam, IntPtr lparam)
			{
				return IntPtr.Zero;
			}

}; // class CommonDialog

}; // namespace System.Windows.Forms
