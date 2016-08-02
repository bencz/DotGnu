/*
 * ContainerControl.cs - Implementation of the
 *			"System.Windows.Forms.ContainerControl" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Neil Cawse.
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

/* Information on Keyboard handling
 * Key event processing always starts at the control that has the input focus (e.g. a text control). From there various methods are called upwards in the window hierarchy (from child to parent). This gives parent controls such as forms and panels an opportunity to handle and consume key events before they can reach the target control. There is only one method (ProcessMnemonic) that is called in the opposite direction (from parent to child). Each of the 8 methods returns a boolean value. If a method returns true the key event is considered handled (consumed) and processing of that particular key event will stop immediatly (no further processing methods will be called).

Key event processing actually happens in two phases: preprocessing and processing. The preprocessing phase starts with a call to PreProcessMessage, the processing phase with a call to ProcessKeyMessage. While the processing phase is very similar for all three types of key events (WM_KEYDOWN, WM_KEYUP and WM_CHAR) preprocessing differs completely for WM_KEYDOWN and WM_CHAR and there is no preprocessing phase for WM_KEYUP.

Here is what happens in detail:

Preprocessing Phase (WM_KEYDOWN):
  1. PreProcessMessage is called on the focused control.
     2. The focused control calls its own ProcessCmdKey method.
        3. ProcessCmdKey by default just returns the result of calling
           ProcessCmdKey on its parent. So effectivly this bubbles the event
           up the windows hierarchy until the top-most control is reached
           which - by default - simply returns false.
     4. The focused control calls IsInputKey on itself. If the result is true
        the PreProcessMessage terminates at this point with a return value of false.
     6. The focused control calls its own ProcessDialogKey method.
        7. Again, ProcessDialogKey is called from child to parent along the parent
           chaim until the top-most control is reached which usually returns false.
  7. PreProcessMessage returns.           
           
Preprocessing Phase (WM_CHAR):
  1. PreProcessMessage is called on the focused control.
     2. The focused control calls IsInputChar on itself. If the result is true
        the PreProcessMessage terminates at this point with a return value of false.
     3. The focused control calls its own ProcessDialogChar method.
        4. ProcessDialogChar is called from child to parent along the parent
           chain until the top-most control is reached which usually returns false.
           In addition, for each ContainerControl the following step is
           performed before the parent's ProcessDialogChar method is invoked:
           5. The container control calls its own ProcessMnemonic method.
              6. ProcessMnemonic is called for all child controls of the
                 container (which in turn might call ProcessMnemonic on their
                 children).
  7. PreProcessMessage returns.
   
Processing Phase (WM_KEYDOWN, WM_KEYUP and WM_CHAR):
  1. ProcessKeyMessage is called on the focused control.
     2. The focused control calls ProcessKeyPreview on the parent control.
        3. ProcessKeyPreview is called from child to parent along the parent
           chain until the top-most control is reached which usually returns false.
     4. The focused control calls ProcessKeyEventArgs on itself.
        5. ProcessKeyEventArgs calls one of the methods OnKeyDown, OnKeyUp
           or OnKeyPress depending on the type of key event.
           6. The On* method invokes all event handlers that have been registerd
              for the corresponding event.
   7. ProcessKeyMessage returns.
           
Note again that as soon as one method returns true processing will stop and the calling method will immediately return true as well. Also, if the preprocessing phase returns true the processing phase will be skipped.

You may have noticed that in preprocessing of WM_CHAR when there are several container controls in the parent chain the ProcessMnemonic method of child controls will be called unnecessarily often.
*/

namespace System.Windows.Forms
{

using System.Drawing;
using System.ComponentModel;

public class ContainerControl : ScrollableControl, IContainerControl
{
	// Internal state.
	private Control activeControl;
	private Control focusedControl;
	private Control unvalidatedControl;
	private bool validating = false;

	// Constructor.
	public ContainerControl()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint, false);
		SetStyle(ControlStyles.ContainerControl, true);
	}

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public override BindingContext BindingContext
	{
		get
		{
			// Containers must always have a binding context.
			BindingContext context = base.BindingContext;
			if(context == null)
			{
				context = new BindingContext();
				base.BindingContext = context;
			}
			return context;
		}
		set
		{
			base.BindingContext = value;
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			return base.CreateParams;
		}
	}

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public virtual Control ActiveControl
	{
		get
		{
			return activeControl;
		}

		set
		{
			// Already set?
			if (activeControl == value && (value == null || value.Focused))
			{
				return;
			}

			// Can't set the active control to a control thats not a child of the container.
			if (value != null && !Contains(value))
			{
				throw new ArgumentException(S._("SWF_CouldNotActivate"));
			}
		
			bool activated;

			// Find outer container.
			ContainerControl parentContainer = this;
			if (value != null && value.Parent != null)
			{
				parentContainer = value.Parent.GetContainerControl() as ContainerControl;
			}

			if (parentContainer != null)
			{
				activated = parentContainer.ActivateControlInternal(value);
			}
			else
			{
				activated = SetActiveControl(value);
			}

			if (parentContainer != null && activated)
			{
				// Only focus if the control that currently has the focus falls in the same hierarchy as the control we are setting the focus to.
				ContainerControl outerContainer = this;

				while (outerContainer.Parent != null && outerContainer.Parent.GetContainerControl() is ContainerControl)
				{
					outerContainer = outerContainer.Parent.GetContainerControl() as ContainerControl;
				}
				if (outerContainer.ContainsFocus)
				{
					parentContainer.SetFocus(activeControl);
				}
			}
		}
	}

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public Form ParentForm
	{
		get
		{
			Control parent = Parent;
			if (parent != null)
			{
				return parent.FindForm();
			}
			if (this is Form)
			{
				return null;
			}
			else
			{
				return FindForm();
			}
		}
	}

	// Activate a specific control.
	public bool ActivateControl(Control active)
	{
		bool activated = ActivateControlInternal(active);
		//TODO: Scroll active into view if its not visible.
		return activated;
	}

	private bool ActivateControlInternal(Control active)
	{

		bool activated = true;
		bool activate = false;
		ContainerControl container;
		Control parent = base.Parent;
		if (parent != null)
		{
			container = parent.GetContainerControl() as ContainerControl;
			if (container != null && container.ActiveControl != this)
			{
				activate = true;
				if (!container.ActivateControlInternal(this))
				{
					return false; 
				}
			}
		}
		
		if (active != activeControl || activate)
		{
			if (active == this)
			{
				activated = SetActiveControl(null);
			}
			else
			{
				activated = SetActiveControl(active);
			}
		}
		return activated;
	}

#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif // CONFIG_COMPONENT_MODEL
	protected override void AdjustFormScrollbars(bool displayScrollbars)
	{
		AdjustFormScrollbars(displayScrollbars);
		ScrollControlIntoView(activeControl);
	}

	protected virtual void UpdateDefaultButton()
	{  
	} 

	private bool SetActiveControl(Control value)
	{
		if (activeControl == value)
		{
			return true;
		}
		activeControl = value;

		if (!validating && unvalidatedControl == null)
		{
			// Set the unvalidatedControl to outermost active control.
			// Start with the focused control.
			unvalidatedControl = focusedControl;
			while (true)
			{
				ContainerControl c = unvalidatedControl as ContainerControl;
				if (c == null || c.activeControl == null)
				{
					break;
				}
				unvalidatedControl = c.activeControl;
			}
		}

		// Go through the hierarchy, doing the leave and enter events, as we change active control.
		Control currentFocusedControl = focusedControl;
		ReDo:
		while (activeControl != currentFocusedControl)
		{
			if (currentFocusedControl == null || IsParent(currentFocusedControl, activeControl))
			{
				Control currentActiveControl = activeControl;
				while (true)
				{
					Control parent = currentActiveControl.Parent;
					// Find which is lower in the hierarchy (which comes first), the active Control or the focused Control.
					// Set the focus to this if validated.
					if (parent == this || parent == currentFocusedControl)
					{
						Control prevFocusedControl = focusedControl = currentFocusedControl;
						ValidateEnter(currentActiveControl);
						if (focusedControl != prevFocusedControl)
						{
							currentFocusedControl = focusedControl;
						}
						else
						{
							currentFocusedControl = currentActiveControl;
							try
							{
								currentFocusedControl.DoEnter();
							}
							catch (Exception e)
							{
								Application.OnThreadException(e);
							}
						}
						goto ReDo;
					}
					currentActiveControl = currentActiveControl.Parent;
				}
			}
			
			// Find innermost focused container.
			
			ContainerControl innerMostFocusedContainer = this;
			while (innerMostFocusedContainer.focusedControl is ContainerControl)
			{
				innerMostFocusedContainer = innerMostFocusedContainer.focusedControl as ContainerControl;
			}

			// Reset the focusedControl and activeControl of the innermost container.
			Control highestResetControl = null;
			if (innerMostFocusedContainer.focusedControl == null)
			{
				// The container has the focus set.
				currentFocusedControl = innerMostFocusedContainer;
				if (innerMostFocusedContainer.Parent != null)
				{
					// Remove the active control and focused control of this containers container.
					ContainerControl parentContainer = innerMostFocusedContainer.Parent.GetContainerControl() as ContainerControl;
					highestResetControl = parentContainer;
					if (parentContainer != null && parentContainer != this)
					{
						parentContainer.activeControl = null;
						parentContainer.focusedControl = null;
					}
				}
			}
			else
			{
				// There is a focused control of the innermost container.
				currentFocusedControl = innerMostFocusedContainer.focusedControl;
				highestResetControl = innerMostFocusedContainer;
				if (innerMostFocusedContainer != this)
				{
					innerMostFocusedContainer.focusedControl = null;
					if (innerMostFocusedContainer.Parent == null || !(innerMostFocusedContainer.Parent is MdiClient))
					{
						innerMostFocusedContainer.activeControl = null;
					}
				}
			}

			// Do all the leave events up the hierarchy.
			do
			{
				Control prevFocusedControl = currentFocusedControl;
				if (currentFocusedControl != null)
				{
					currentFocusedControl = currentFocusedControl.Parent;
				}
				if (currentFocusedControl == this)
				{
					currentFocusedControl = null;
				}
				if (prevFocusedControl != null)
				{
					try
					{
						prevFocusedControl.DoLeave();
					}
					catch (Exception e)
					{
						Application.OnThreadException(e);
					}
				}
			}
			while (currentFocusedControl != null && currentFocusedControl != highestResetControl && !IsParent(currentFocusedControl, activeControl));
		}

		focusedControl = activeControl;
		if (activeControl != null)
		{
			ValidateEnter(activeControl);
		}
		
		if (activeControl == value)
		{
			Form form = FindForm();
			if (form != null)
			{
				form.UpdateDefaultButton();
			}
		}

		return (activeControl == value);
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		OnBindingContextChanged(EventArgs.Empty);
	}

#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif // CONFIG_COMPONENT_MODEL
	protected override void OnControlRemoved(ControlEventArgs e)
	{
		Control control = e.Control;
		// Make sure the unvalidated control and active control are cleaned up.
		if (control == unvalidatedControl || control.Contains(unvalidatedControl))
		{
			unvalidatedControl = null;
		}
		if (control == activeControl || control.Contains(activeControl))
		{
			ActiveControl = null;
		}
		base.OnControlRemoved(e);
	}

	// Brubbel
	internal virtual void AfterControlRemoved(Control control)
	{
		// Select next control, if control was active and removed.
		if (control == activeControl || control.Contains(activeControl))
		{
			if( base.SelectNextControl( control, true, true, true, true ) ) {
				this.SetFocus( activeControl );
			}
			else {
				this.SetActiveControl( null );
			}
		}
		else if( (this.activeControl == null) && (this.Parent != null) ) {
			ContainerControl container = Parent.GetContainerControl() as ContainerControl;
			if( null != container && container.ActiveControl == this ) {
				Form form = base.FindForm();
				if( null != form ) {
					form.SelectNextControl( this, true, true, true, true );
				}
				
			}
		}
		else {
		}
		
		// Make sure the unvalidated control and active control are cleaned up.
		if (control == unvalidatedControl || control.Contains(unvalidatedControl))
		{
			unvalidatedControl = null;
		}
	}
	
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif // CONFIG_COMPONENT_MODEL
	protected override bool ProcessDialogChar(char charCode)
	{
		if (GetTopLevel())
			{
				if(charCode != ' ' && ProcessMnemonic(charCode))
				{
					return true;
				}
			}
		return base.ProcessDialogChar(charCode);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if ((keyData & (Keys.Alt | Keys.Control)) == 0)
		{
			Keys key = keyData & Keys.KeyCode;
			if (key == Keys.Tab)
			{
				if (ProcessTabKey((keyData & Keys.Shift) == 0))
				{
					return true;
				}
			}
			else if (key == Keys.Left || key == Keys.Up || key == Keys.Right || key == Keys.Down)
			{
				Control control;
				if (activeControl != null)
				{
					control = activeControl.Parent;
				}
				else
				{
					control = this;
				}
				bool forward = (key == Keys.Right || key == Keys.Down);
				if (control.SelectNextControl(activeControl, forward , false, false, true))
				{
					return true;
				}
			}
		}
		return base.ProcessDialogKey(keyData);

	}

	protected override bool ProcessMnemonic(char charCode)
	{

		if (Controls.Count == 0)
		{
			return false; 
		}
		Control active = ActiveControl;
		// Find the bottom most active control or the container if there isn't one.
		while (true)
		{
			ContainerControl container = active as ContainerControl;
			if (container == null)
			{
				break;
			}
			Control newActiveControl = container.ActiveControl;
			if (newActiveControl == null)
			{
				break;
			}
			active = newActiveControl;
		}

		// Process the mnemonics if needed.
		bool back = false;
		Control mnemonicControl = active;

		do
		{
			mnemonicControl = GetNextControl(mnemonicControl, true);
			if (mnemonicControl == null)
			{
				if (back)
				{
					break;
				}
				back = true;
			}
			else
			{
				if (mnemonicControl.ProcessMnemonicInternal(charCode))
				{
					return true;
				}
			}
		}
		while (mnemonicControl != active);
		return false;
	}

	private new void ScrollControlIntoView(Control control)
	{
		// Find scrollable parent.
		while (true)
		{
			control = control.Parent;
			if (control == null)
			{
				return;
			}
			ScrollableControl scrollableControl = control as ScrollableControl;
			if (scrollableControl != null)
			{
				scrollableControl.ScrollControlIntoView(activeControl);
				continue;
			}
		}
	}

	protected override void Select(bool directed, bool forward)
	{
		if (Parent != null)
		{
			IContainerControl container = Parent.GetContainerControl();
			if (container != null)
			{
				container.ActiveControl = this;
				if (directed && container.ActiveControl == this)
				{
					SelectNextControl(null, forward, true, true, false);		 
				}
			}
		}
		if (directed)
		{
			SelectNextControl(null, forward, true, true, false);
		}
	}

	private void ValidateEnter(Control control)
	{
		if (unvalidatedControl == null || control.CausesValidation)
		{
			return;
		}
		while (control != null && !IsParent(control, unvalidatedControl))
		{
			control = control.Parent;
		}
		ValidateHierarchy(control);
	}

	public bool Validate()
	{
		if (unvalidatedControl == null)
		{
			if (focusedControl is ContainerControl && focusedControl.CausesValidation)
			{
				(focusedControl as ContainerControl).Validate();
			}
			else
			{
				unvalidatedControl = focusedControl;
			}
		}
		return ValidateHierarchy(null);
	}

	// Validate from control back up to unvalidated or the currently focused control.
	private bool ValidateHierarchy(Control control)
	{
		if (unvalidatedControl == null)
		{
			unvalidatedControl = focusedControl;
			return true;
		}
		if (validating)
		{
			return false;
		}
		if (control == null)
		{
			control = this;
		}
		if (!IsParent(control, unvalidatedControl))
		{
			return false;
		}
		validating = true;
		bool cancelValidate = false;
		Control currentControl = unvalidatedControl;
		// Validate starts off uncancelled.
		if (activeControl != null)
		{
			activeControl.DoValidationCancel(false);
		}
		try
		{
			while(currentControl != control)
			{
				// Do the validating and validated events if needed.
				if (currentControl.CausesValidation)
				{
					bool cancelled = false;
					try
					{
						cancelled = currentControl.DoValidating();
					}
					catch (Exception)
					{
						cancelValidate = true;
						throw;
					}
					if (cancelled)
					{
						cancelValidate = true;
						break;
					}
					try
					{
						currentControl.DoValidated();
					}
					catch (Exception e)
					{
						Application.OnThreadException(e);
					}
				}
				// Move up the hierarchy.
				currentControl = currentControl.Parent;
			}
			if (cancelValidate)
			{
				if (activeControl != null)
				{
					activeControl.DoValidationCancel(true);
					if (activeControl is ContainerControl)
					{
						ContainerControl containerControl = activeControl as ContainerControl;
						if (containerControl.focusedControl != null)
						{
							containerControl.focusedControl.DoValidationCancel(true);
						}
						containerControl.ResetControlsRecursive();
					}
				}
				ActiveControl = unvalidatedControl;
			}
		}
		finally
		{
			unvalidatedControl = null;
			validating = false;
		}
		return !cancelValidate;
	}

	internal void ResetControlsRecursive()
	{
		if (activeControl is ContainerControl)
		{
			(activeControl as ContainerControl).ResetControlsRecursive();
		}
		focusedControl = null;
		activeControl = null;
	}

	private bool IsParent(Control parent, Control control)
	{
		while (control != null)
		{
			if (parent == control)
			{
				return true;
			}
			control = control.Parent;
		}
		return false;
	}

	// Dispose of this control.
	protected override void Dispose(bool disposing)
	{
		activeControl = null;
		focusedControl = null;
		unvalidatedControl = null;
	
		base.Dispose(disposing);
	}

	// Process the tab key.
	protected virtual bool ProcessTabKey(bool forward)
	{
		return SelectNextControl(activeControl, forward, true, true, false);
	}

	private void SetFocus(Control control)
	{
		if (control != null && control.toolkitWindow != null && control.Visible)
		{
			control.toolkitWindow.Focus();
			return; 
		}

		// Find the first visible container.
		ContainerControl container = this;
		Control parent;
		while (container != null && !container.Visible)
		{
			parent = container.Parent;
			if (parent == null)
			{
				if (container.Visible)
				{
					container.toolkitWindow.Focus();
				}
				break;
			}
			container = parent.GetContainerControl() as ContainerControl;
		}
	}

	protected override void OnGotFocus(EventArgs e)
	{
		if (ActiveControl != null)
		{
			if (!ActiveControl.Visible)
			{
				base.OnGotFocus(EventArgs.Empty);
			}
			SetFocus(activeControl);
			return;
		}
		if (Parent != null)
		{
			IContainerControl container = Parent.GetContainerControl();
			if (container != null)
			{
				if (!container.ActivateControl(this))
				{
					return;
				}
			}
		}
		base.OnGotFocus (e);
	}


#if !CONFIG_COMPACT_FORMS

	// Process a message.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif // CONFIG_COMPONENT_MODEL
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

#endif // !CONFIG_COMPACT_FORMS

}; // class ContainerControl

}; // namespace System.Windows.Forms
