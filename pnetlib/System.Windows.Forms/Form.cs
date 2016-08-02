/*
 * Form.cs - Implementation of the
 *			"System.Windows.Forms.Form" class.
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

namespace System.Windows.Forms
{

using System.Drawing;
using System.Drawing.Toolkit;
using System.ComponentModel;
using System.Collections;

public class Form : ContainerControl
{
	// Internal state.
	private IButtonControl acceptButton;
	private IButtonControl defaultButton;
	private IButtonControl cancelButton;
	private bool autoScale;
	private bool keyPreview;
	private bool topLevel;
	internal bool dialogResultIsSet;
	private Size autoScaleBaseSize;
	private DialogResult dialogResult;
	private FormBorderStyle borderStyle;
	private Icon icon;
	private ToolkitWindowFlags windowFlags;
	private Size maximumSize;
	private Size minimumSize;
	private Form[] mdiChildren;
	private Form[] ownedForms;
	private Form mdiParent;
	private Form owner;
	private MainMenu menu;
	private MainMenu mergedMenu;
	private double opacity;
	private SizeGripStyle sizeGripStyle;
	private FormStartPosition formStartPosition;
	private Color transparencyKey;
	private FormWindowState windowState;
	internal static Form activeForm;
	internal static ArrayList activeForms = new ArrayList();
	private bool showInTaskbar;
	private bool controlBox;
	private bool loaded; /* whether we have sent OnLoad or not */
	private MdiClient mdiClient;
	
	// Constructor.
	public Form()
			{
				visible = false;
				autoScale = true;
				topLevel = true;
				loaded=false;
				borderStyle = FormBorderStyle.Sizable;
				mdiChildren = new Form [0];
				ownedForms = new Form [0];
				opacity = 1.0;
				windowFlags = ToolkitWindowFlags.Default;
				formStartPosition = FormStartPosition.WindowsDefaultLocation;
				windowState = FormWindowState.Normal;
			}

	// Get or set this control's properties.
	private IToolkitTopLevelWindow ToolkitWindow
			{
				get
				{
					return (toolkitWindow as IToolkitTopLevelWindow);
				}
			}
	public IButtonControl AcceptButton
			{
				get
				{
					return acceptButton;
				}
				set
				{
					acceptButton = value;
				}
			}
	public static Form ActiveForm
			{
				get
				{
					return activeForm;
				}
			}
	public Form ActiveMdiChild
			{
				get
				{
					if(mdiClient != null)
					{
						return mdiClient.ActiveChild;
					}
					else
					{
						return null;
					}
				}
			}
	[TODO]
	public bool AllowTransparency
			{
				get
				{
					// Implement
					return false;
				}
				set
				{
					// Implement
				}
			}
	public bool AutoScale
			{
				get
				{
					return autoScale;
				}
				set
				{
					autoScale = value;
				}
			}
	public Size AutoScaleBaseSize
			{
				get
				{
					return autoScaleBaseSize;
				}
				set
				{
					autoScaleBaseSize = value;
				}
			}
	public override bool AutoScroll
			{
				get
				{
					return base.AutoScroll;
				}
				set
				{
					base.AutoScroll = value;
				}
			}
	public override Color BackColor
			{
				get
				{
					// The base implementation takes care of the default.
					return base.BackColor;
				}
				set
				{
					base.BackColor = value;
				}
			}
	public IButtonControl CancelButton
			{
				get
				{
					return cancelButton;
				}
				set
				{
					cancelButton = value;
				}
			}
	protected override CreateParams CreateParams
			{
				get
				{
					return base.CreateParams;
				}
			}
	protected override ImeMode DefaultImeMode
			{
				get
				{
					return ImeMode.NoControl;
				}
			}
	protected override Size DefaultSize
			{
				get
				{
					return new Size(300, 300);
				}
			}
	public Rectangle DesktopBounds
			{
				get
				{
					// Use the ordinary bounds.
					return Bounds;
				}
				set
				{
					Bounds = value;
				}
			}
	public Point DesktopLocation
			{
				get
				{
					// Use the ordinary location.
					return Location;
				}
				set
				{
					Location = value;
				}
			}
	public DialogResult DialogResult
			{
				get
				{
					return dialogResult;
				}
				set
				{
					dialogResult = value;
					dialogResultIsSet = true;
				}
			}
	public FormBorderStyle FormBorderStyle
			{
				get
				{
					return borderStyle;
				}
				set
				{
					if(borderStyle != value)
					{
						Size oldSize = ClientSize;
						borderStyle = value;
						SetWindowFlag(0, true);
						ClientSize = oldSize;
					}
				}
			}
	public bool HelpButton
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.Help);
				}
				set
				{
					SetWindowFlag(ToolkitWindowFlags.Help, value);
				}
			}
	public Icon Icon
			{
				get
				{
					return icon;
				}
				set
				{
					if(icon != value)
					{
						icon = value;
						if(ToolkitWindow != null)
						{
							ToolkitWindow.SetIcon(value);
						}
					}
				}
			}
	public bool IsMdiChild
			{
				get
				{
					return (MdiParent != null);
				}
			}
	public bool IsMdiContainer
			{
				get
				{
					return (mdiClient != null);
				}
				set
				{
					if(value && mdiClient == null)
					{
						// Convert the form into MDI mode.
						mdiClient = new MdiClient();
						Controls.Add(mdiClient);
					}
				}
			}
	public bool KeyPreview
			{
				get
				{
					return keyPreview;
				}
				set
				{
					keyPreview = value;
				}
			}
	public bool MaximizeBox
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.Maximize);
				}
				set
				{
					SetWindowFlag(ToolkitWindowFlags.Maximize, value);
				}
			}
	[TODO]
	protected Rectangle MaximizedBounds
			{
				get
				{
					// Implement
					return Rectangle.Empty;
				}
				set
				{
					// Implement
				}
			}
	public Size MaximumSize
			{
				get
				{
					return maximumSize;
				}
				set
				{
					if(value.Width < 0)
					{
						throw new ArgumentOutOfRangeException
							("value.Width", S._("SWF_NonNegative"));
					}
					if(value.Height < 0)
					{
						throw new ArgumentOutOfRangeException
							("value.Height", S._("SWF_NonNegative"));
					}
					if(maximumSize != value)
					{
						maximumSize = value;
						if(ToolkitWindow != null)
						{
							ToolkitWindow.SetMaximumSize(value);
						}
						if (maximumSize.Width > 0 && Width > maximumSize.Width)
							Width = maximumSize.Width;
						if (maximumSize.Height > 0 && Height > maximumSize.Height)
							Height = maximumSize.Height;
					}
				}
			}
	public Form[] MdiChildren
			{
				get
				{
					if(mdiClient != null)
					{
						return mdiClient.MdiChildren;
					}
					else
					{
						return new Form [0];
					}
				}
			}
	public Form MdiParent
			{
				get
				{
					return mdiParent;
				}
				set
				{
					if(mdiParent != null || value == null || Created)
					{
						return;
					}
					if(value.mdiClient == null)
					{
						return;
					}
					
					mdiParent = value;
					value.mdiClient.Controls.Add(this);
					Parent = value.mdiClient;
				}
			}
	public bool MinimizeBox
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.Minimize);
				}
				set
				{
					SetWindowFlag(ToolkitWindowFlags.Minimize, value);
				}
			}
	public Size MinimumSize
			{
				get
				{
					return minimumSize;
				}
				set
				{
					if(value.Width < 0)
					{
						throw new ArgumentOutOfRangeException
							("value.Width", S._("SWF_NonNegative"));
					}
					if(value.Height < 0)
					{
						throw new ArgumentOutOfRangeException
							("value.Height", S._("SWF_NonNegative"));
					}
					if(minimumSize != value)
					{
						minimumSize = value;
						if(ToolkitWindow != null)
						{
							ToolkitWindow.SetMinimumSize(value);
						}
						if (minimumSize.Width > 0 && Width < minimumSize.Width)
							Width = minimumSize.Width;
						if (minimumSize.Height > 0 && Height < minimumSize.Height)
							Height = minimumSize.Height;
					}
				}
			}
	public MainMenu Menu
			{
				get
				{
					return menu;
				}
				set
				{
					if(menu != value)
					{
						if(menu != null)
						{
							menu.RemoveFromForm();
							menu = null;
						}
						if(value != null)
						{
							Form other = value.GetForm();
							if(other != null)
							{
								other.Menu = null;
							}
						}
						// Get the ClientSize before we add the menu.
						Size clientSize = ClientSize;
						menu = value;
						if(menu != null)
						{
							menu.AddToForm(this);
						}
						// The ClientSize must be the original.
						ClientSize = clientSize;
					}
				}
			}
	public MainMenu MergedMenu
			{
				get
				{
					return mergedMenu;
				}
			}
	public bool Modal
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.Modal);
				}
			}
	public double Opacity
			{
				get
				{
					return opacity;
				}
				set
				{
					opacity = value;
					IToolkitTopLevelWindow window = (IToolkitTopLevelWindow)ToolkitWindow;
					if(window != null)
					{
						window.SetOpacity(opacity);
					}
				}
			}
	public Form[] OwnedForms
			{
				get
				{
					return ownedForms;
				}
			}
	[TODO]
	public Form Owner
			{
				get
				{
					return owner;
				}
				set
				{
					// Fix: update the owned child list
					owner = value;
				}
			}
	public bool ShowInTaskbar
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.ShowInTaskbar);
				}
				set
				{
					if (value != showInTaskbar)
					{
						showInTaskbar = value;
						SetWindowFlag(ToolkitWindowFlags.ShowInTaskbar, value);
					}
				}
			}
	public new Size Size
			{
				get
				{
					return base.Size;
				}
				set
				{
					base.Size = value;
				}
			}
	public SizeGripStyle SizeGripStyle
			{
				get
				{
					return sizeGripStyle;
				}
				set
				{
					sizeGripStyle = value;
				}
			}
	public FormStartPosition StartPosition
			{
				get
				{
					return formStartPosition;
				}
				set
				{
					formStartPosition = value;
				}
			}
	public bool TopLevel
			{
				get
				{
					return topLevel;
				}
				set
				{
					// recreate toolkitwindow, if exists
					if( value != topLevel ) {
						topLevel = value;
						if( null != toolkitWindow ) {
							Control [] copy = new Control[this.Controls.Count];
							this.Controls.CopyTo( copy, 0 );
							this.Controls.Clear();;
							toolkitWindow.Destroy();
							toolkitWindow = null;
							this.CreateHandle();
							this.Controls.AddRange( copy );
						}
					}
				}
			}
	public bool TopMost
			{
				get
				{
					return GetWindowFlag(ToolkitWindowFlags.TopMost);
				}
				set
				{
					SetWindowFlag(ToolkitWindowFlags.TopMost, value);
				}
			}
	public Color TransparencyKey
			{
				get
				{
					return transparencyKey;
				}
				set
				{
					transparencyKey = value;
				}
			}
	public FormWindowState WindowState
			{
				get
				{
					return windowState;
				}
				set
				{
					if(windowState != value)
					{
						windowState = value;
						if(ToolkitWindow != null)
						{
							if(value == FormWindowState.Normal)
							{
								ToolkitWindow.Restore();
							}
						#if !CONFIG_COMPACT_FORMS
							else if(value == FormWindowState.Minimized)
							{
								ToolkitWindow.Iconify();
							}
						#endif
							else if(value == FormWindowState.Maximized)
							{
								ToolkitWindow.Maximize();
							}
						}
					}
				}
			}

	internal override IToolkitWindow CreateToolkitWindow(IToolkitWindow parent)
	{
		// When a Form is reparented to a normal container control 
		// which does work on Win32 unfortunately.
		if(mdiParent == null && (!TopLevel))
		{
			return base.CreateToolkitWindow(parent);
		}

		CreateParams cp = CreateParams;

		// Create the window and set its initial caption.
		IToolkitTopLevelWindow window;
		if(mdiParent == null)
		{
			// use ControlToolkitManager to create the window thread safe
			window = ControlToolkitManager.CreateTopLevelWindow( this,
					cp.Width - ToolkitDrawSize.Width,
					cp.Height - ToolkitDrawSize.Height);
		}
		else
		{
			mdiParent.mdiClient.CreateControl();
			IToolkitMdiClient mdi =
					(mdiParent.mdiClient.toolkitWindow as
					IToolkitMdiClient);
			
			// use ControlToolkitManager to create the window thread safe
			window = ControlToolkitManager.CreateMdiChildWindow( this, mdi, 
					cp.X, cp.Y,
					cp.Width - ToolkitDrawSize.Width,
					cp.Height - ToolkitDrawSize.Height);
		}
		
		window.SetTitle(cp.Caption);
		// Win32 requires this because if the window is maximized, the windows size needs to be set.
		toolkitWindow = window;

		// Adjust the window hints to match our requirements.
		SetWindowFlags(window);
		if(icon != null)
		{
			window.SetIcon(icon);
		}
		window.SetMaximumSize(maximumSize);
		window.SetMinimumSize(minimumSize);

#if !CONFIG_COMPACT_FORMS
		if(windowState == FormWindowState.Minimized)
		{
			window.Iconify();
		}
		else
#endif
		if(windowState == FormWindowState.Maximized)
		{
			window.Maximize();
		}

		// Center the window on-screen if necessary.
		if(formStartPosition == FormStartPosition.CenterScreen)
		{
			Size screenSize = ToolkitManager.Toolkit.GetScreenSize();
			window.MoveResize
					((screenSize.Width - cp.Width) / 2,
					(screenSize.Height - cp.Height) / 2,
					window.Dimensions.Width, window.Dimensions.Height);
		}
		else if(formStartPosition == FormStartPosition.Manual)
		{
			window.MoveResize
					(
					cp.X,
			cp.Y,
			window.Dimensions.Width,
			window.Dimensions.Height
					);
		}
		if(opacity != 1.0)
		{
			window.SetOpacity(opacity);
		}
		return window;
}


	// Determine if this is a top-level control which cannot have parents.
	internal override bool IsTopLevel
			{
				get
				{
					return ((mdiParent == null) && TopLevel);
				}
			}
			

	// Get the current state of a window decoration flag.
	private bool GetWindowFlag(ToolkitWindowFlags flag)
			{
				return ((windowFlags & flag) == flag);
			}

	// Get the full set of window flags for this window.
	private ToolkitWindowFlags GetFullFlags()
			{
				ToolkitWindowFlags flags = windowFlags;
				switch(borderStyle)
				{
					case FormBorderStyle.None:
					{
						bool topMost = ((flags & ToolkitWindowFlags.TopMost) != 0);
						flags = ToolkitWindowFlags.ShowInTaskbar;
						if (topMost)
							flags |= ToolkitWindowFlags.TopMost;
					}
					break;

					case FormBorderStyle.Fixed3D:
					case FormBorderStyle.FixedSingle:
					{
						flags &= ~(ToolkitWindowFlags.Maximize |
								   ToolkitWindowFlags.ResizeHandles |
								   ToolkitWindowFlags.Resize);
					}
					break;

					case FormBorderStyle.FixedDialog:
					{
						flags &= ~(ToolkitWindowFlags.Maximize |
								   ToolkitWindowFlags.ResizeHandles |
								   ToolkitWindowFlags.Resize);
						flags |= ToolkitWindowFlags.Dialog;
					}
					break;

					case FormBorderStyle.FixedToolWindow:
					{
						flags &= ~(ToolkitWindowFlags.Maximize |
								   ToolkitWindowFlags.ResizeHandles |
								   ToolkitWindowFlags.Resize |
								   ToolkitWindowFlags.ShowInTaskbar);
						flags |= ToolkitWindowFlags.ToolWindow;
					}
					break;

					case FormBorderStyle.Sizable: break;

					case FormBorderStyle.SizableToolWindow:
					{
						flags &= ~(ToolkitWindowFlags.ShowInTaskbar);
						flags |= ToolkitWindowFlags.ToolWindow;
					}
					break;
				}
				if((flags & ToolkitWindowFlags.Modal) != 0)
				{
					flags |= ToolkitWindowFlags.Dialog;
				}
				return flags;
			}

	// Set the current state of the window decoration flags on a window.
	private void SetWindowFlags(IToolkitWindow window)
			{
				((IToolkitTopLevelWindow)window).SetWindowFlags(GetFullFlags());
			}

	// Set the current state of a window decoration flag.
	private void SetWindowFlag(ToolkitWindowFlags flag, bool value)
			{
				// Alter the flag setting.
				if(value)
				{
					windowFlags |= flag;
				}
				else
				{
					windowFlags &= ~flag;
				}

				// Pass the flags to the window manager.
				if(toolkitWindow != null)
				{
					SetWindowFlags(toolkitWindow);
				}
			}

	// Activate the form and give it focus.
	public void Activate()
			{
				BringToFront();
			}

	// Add an owned form to this form.
	public void AddOwnedForm(Form ownedForm)
			{
				if(ownedForm != null)
				{
					ownedForm.Owner = this;
				}
			}

	// Close this form.
	public void Close()
			{
				CloseRequest();
			}

	// Get the auto scale base size for a particular font.
	[TODO]
	public static SizeF GetAutoScaleSize(Font font)
			{
				return SizeF.Empty;
			}

	// Layout the MDI children of this form.
	public void LayoutMdi(MdiLayout value)
			{
				if(mdiClient != null)
				{
					mdiClient.LayoutMdi(value);
				}
			}

	// Remove an owned form from this form.
	public void RemoveOwnedForm(Form ownedForm)
			{
				if(ownedForm != null && ownedForm.Owner == this)
				{
					ownedForm.Owner = null;
				}
			}

#if !CONFIG_COMPACT_FORMS

	// Set the desktop bounds of this form.
	public void SetDesktopBounds(int x, int y, int width, int height)
			{
				Rectangle workingArea = SystemInformation.WorkingArea;
				SetBoundsCore(workingArea.X + x, workingArea.Y + y,
							  width, height, BoundsSpecified.All);
			}

	// Set the desktop location of this form.
	public void SetDesktopLocation(int x, int y)
			{
				Rectangle workingArea = SystemInformation.WorkingArea;
				Location = new Point(workingArea.X + x, workingArea.Y + y);
			}

#endif // !CONFIG_COMPACT_FORMS

	// Show this form as a modal dialog.
	private DialogResult ShowDialog(Form owner)
			{
				// Bail out if this dialog is already displayed modally.
				if(Modal)
				{
					return DialogResult.None;
				}

				// Reset the dialog result.
				dialogResult = DialogResult.None;
				dialogResultIsSet = false;

				// Mark this form as modal.
				SetWindowFlag(ToolkitWindowFlags.Modal, true);
				try
				{
					// Create the control.  We must do this before
					// we set the owner or make the form visible.
					CreateControl();

					// Set the dialog owner.
					IToolkitTopLevelWindow toolkitWindow = ToolkitWindow;
					if(toolkitWindow != null)
					{
						if(owner != null)
						{
							toolkitWindow.SetDialogOwner(owner.ToolkitWindow);
						}
						else
						{
							toolkitWindow.SetDialogOwner(null);
						}
					}

					// Make the form visible.
					Visible = true;
					Activate();

					// Enter a message loop until the dialog result is set.
					Application.InnerMessageLoop(this);
					
					if( toolkitWindow != null ) {
						DestroyHandle(); // close handle
					}
				}
				finally
				{
					// Make sure that the form is not visible.
					Visible = false;
					// The form is no longer modal.
					SetWindowFlag(ToolkitWindowFlags.Modal, false);
				}
				
				// Return the dialog result.
				return dialogResult;
			}
	public DialogResult ShowDialog()
			{
				return ShowDialog(Owner);
			}
	public DialogResult ShowDialog(IWin32Window owner)
			{
				return ShowDialog(owner as Form);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", Text: " + Text;
			}

	// Event that is emitted when this form is activated.
	public event EventHandler Activated
			{
				add
				{
					AddHandler(EventId.Activated, value);
				}
				remove
				{
					RemoveHandler(EventId.Activated, value);
				}
			}

	// Event that is emitted when this form is closed.
	public event EventHandler Closed
			{
				add
				{
					AddHandler(EventId.Closed, value);
				}
				remove
				{
					RemoveHandler(EventId.Closed, value);
				}
			}

	// Event that is emitted when this form is closing.
	public event CancelEventHandler Closing
			{
				add
				{
					AddHandler(EventId.Closing, value);
				}
				remove
				{
					RemoveHandler(EventId.Closing, value);
				}
			}

	// Event that is emitted when this form is deactivated.
	public event EventHandler Deactivate
			{
				add
				{
					AddHandler(EventId.Deactivate, value);
				}
				remove
				{
					RemoveHandler(EventId.Deactivate, value);
				}
			}

	// Event that is emitted when the input language of a form is changed.
	public event InputLanguageChangedEventHandler InputLanguageChanged
			{
				add
				{
					AddHandler(EventId.InputLanguageChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.InputLanguageChanged, value);
				}
			}

	// Event that is emitted when the input language of a form is changing.
	public event InputLanguageChangingEventHandler InputLanguageChanging
			{
				add
				{
					AddHandler(EventId.InputLanguageChanging, value);
				}
				remove
				{
					RemoveHandler(EventId.InputLanguageChanging, value);
				}
			}

	// Event that is emitted when this form is first loaded.
	public event EventHandler Load
			{
				add
				{
					AddHandler(EventId.Load, value);
				}
				remove
				{
					RemoveHandler(EventId.Load, value);
				}
			}

	// Event that is emitted when the maximized bounds change.
	public event EventHandler MaximizedBoundsChanged
			{
				add
				{
					AddHandler(EventId.MaximizedBoundsChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.MaximizedBoundsChanged, value);
				}
			}

	// Event that is emitted when the maximum size changes.
	public event EventHandler MaximumSizeChanged
			{
				add
				{
					AddHandler(EventId.MaximumSizeChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.MaximumSizeChanged, value);
				}
			}

	// Event that is emitted when an MDI child is activated.
	public event EventHandler MdiChildActivate
			{
				add
				{
					AddHandler(EventId.MdiChildActivate, value);
				}
				remove
				{
					RemoveHandler(EventId.MdiChildActivate, value);
				}
			}

	// Event that is emitted at the end of processing a menu item.
	public event EventHandler MenuComplete
			{
				add
				{
					AddHandler(EventId.MenuComplete, value);
				}
				remove
				{
					RemoveHandler(EventId.MenuComplete, value);
				}
			}

	// Event that is emitted at the start of processing a menu item.
	public event EventHandler MenuStart
			{
				add
				{
					AddHandler(EventId.MenuStart, value);
				}
				remove
				{
					RemoveHandler(EventId.MenuStart, value);
				}
			}

	// Event that is emitted when the minimum size changes.
	public event EventHandler MinimumSizeChanged
			{
				add
				{
					AddHandler(EventId.MinimumSizeChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.MinimumSizeChanged, value);
				}
			}

	protected void CenterToScreen()
			{
				// Nothing to do here -- Not to be used by App developers
			}
	
	// Create a new control collection for this instance.
	protected override Control.ControlCollection CreateControlsInstance()
			{
				return new ControlCollection(this);
			}

	// Create the handle for this control.
	protected override void CreateHandle()
			{
				// Let the base class do the work.
				base.CreateHandle();
			}

	// Dispose of this control.
	protected override void Dispose(bool disposing)
			{
				acceptButton = null;
				defaultButton = null;
				cancelButton = null;
				mdiClient = null;
				if( null != owner ) {
					this.RemoveOwnedForm(owner);
					owner = null;
				}
				if( null != ownedForms ) {
					int iCount = ownedForms.Length;
					for( int i = iCount-1; i >= 0; i-- ) {
						if( null != ownedForms[i] ) {
							ownedForms[i].Dispose();
						}
					}
				}

				if( null != menu ) {
					menu.ownerForm = null;
					menu = null;
				}

				if( null != mergedMenu ) {
					if( mergedMenu.ownerForm == this || mergedMenu.ownerForm == null ) {
						mergedMenu.Dispose();
					}
					mergedMenu = null;
				}
				
				if( activeForm == this ) activeForm = null;

				base.Dispose(disposing);
			}

	// Emit the "Activated" event.
	protected virtual void OnActivated(EventArgs e)
			{
				// This form is currently the active one.
				activeForm = this;

				// Dispatch the event to everyone who is listening.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Activated));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "Closed" event.
	protected virtual void OnClosed(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Closed));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "Closing" event.
	protected virtual void OnClosing(CancelEventArgs e)
			{
				CancelEventHandler handler;
				handler = (CancelEventHandler)(GetHandler(EventId.Closing));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Handle initial control creation.
	protected override void OnCreateControl()
			{
				base.OnCreateControl();
				if(loaded == false && toolkitWindow.IsMapped)
				{
					loaded = true;
					OnLoad(EventArgs.Empty);
				}
				if( menu != null ) {
					// Workaround for fixing that menu is dispayed correct when form is shown.
					Height = Height+1;
					Height = Height-1;
				}
			}

	// Emit the "Deactivate" event.
	protected virtual void OnDeactivate(EventArgs e)
			{
				// None of the application's forms are currently active.
				activeForm = null;

				// Dispatch the event to everyone who is listening.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Deactivate));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Override the "FontChanged" event.
	protected override void OnFontChanged(EventArgs e)
			{
				base.OnFontChanged(e);
			}

	// Override the "HandleCreated" event.
	protected override void OnHandleCreated(EventArgs e)
			{
				// we have to add a reference to the form if the form is created and shown without a reference
				// to avoid the form beeing collected, even it is shown.
				if( !activeForms.Contains(this) ) activeForms.Add(this);
				base.OnHandleCreated(e);
			}

	// Override the "HandleDestroyed" event.
	protected override void OnHandleDestroyed(EventArgs e)
			{
				activeForms.Remove(this);
				base.OnHandleDestroyed(e);
			}

	// Emit the "InputLanguageChanged" event.
	protected virtual void OnInputLanguageChanged
				(InputLanguageChangedEventArgs e)
			{
				InputLanguageChangedEventHandler handler;
				handler = (InputLanguageChangedEventHandler)
					(GetHandler(EventId.InputLanguageChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "InputLanguageChanging" event.
	protected virtual void OnInputLanguageChanging
				(InputLanguageChangingEventArgs e)
			{
				InputLanguageChangingEventHandler handler;
				handler = (InputLanguageChangingEventHandler)
					(GetHandler(EventId.InputLanguageChanging));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "Load" event.
	protected virtual void OnLoad(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Load));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MaximizedBoundsChanged" event.
	protected virtual void OnMaximizedBoundsChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.MaximizedBoundsChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MaximumSizeChanged" event.
	protected virtual void OnMaximumSizeChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.MaximumSizeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MdiChildActivate" event.
	protected virtual void OnMdiChildActivate(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MdiChildActivate));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MenuComplete" event.
	protected virtual void OnMenuComplete(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MenuComplete));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MenuStart" event.
	protected virtual void OnMenuStart(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MenuStart));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "MinimumSizeChanged" event.
	protected virtual void OnMinimumSizeChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.MinimumSizeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Override the "Paint" event.
	protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				if (menu != null)
					menu.OnPaint();
			}

	// Override the "PrimaryEnter" event.
	internal override void OnPrimaryEnter(EventArgs e)
			{
				OnActivated(e);
			}

	// Override the "PrimaryLeave" event.
	internal override void OnPrimaryLeave(EventArgs e)
			{
				OnDeactivate(e);
			}

	// Override the "Resize" event.
	protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);
			}

	// Override the "StyleChanged" event.
	protected override void OnStyleChanged(EventArgs e)
			{
				base.OnStyleChanged(e);
			}

	// Override the "TextChanged" event.
	protected override void OnTextChanged(EventArgs e)
			{
				if(ToolkitWindow != null)
				{
					ToolkitWindow.SetTitle(Text);
				}
				base.OnTextChanged(e);
			}

	// Override the "VisibleChanged" event.
	protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if(loaded == false && 
						toolkitWindow != null && toolkitWindow.IsMapped)
				{
					loaded = true;
					OnLoad(EventArgs.Empty);
				}
			}

	// Process a command key.
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
			{
				if (base.ProcessCmdKey(ref msg, keyData))
					return true;
				if (menu != null)
					return menu.ProcessCmdKey(ref msg, keyData);
				return false;
			}

	// Process a dialog key.
	protected override bool ProcessDialogKey(Keys keyData)
			{
				if ((keyData & (Keys.Control | Keys.Alt)) == 0)
				{
					Keys key = keyData & Keys.KeyCode;
					if (key == Keys.Return)
					{
						if (acceptButton != null)
						{
							acceptButton.PerformClick();
							return true;
						}
					}
					else if (key == Keys.Escape)
					{
						if (cancelButton != null)
						{
							cancelButton.PerformClick();
							return true;
						}
					}
				}
				return base.ProcessDialogKey(keyData);
			}

	// Preview a keyboard message.
	protected override bool ProcessKeyPreview(ref Message msg)
			{
				if(keyPreview)
				{
					// The form wants first crack at the key message.
					if(ProcessKeyEventArgs(ref msg))
					{
						return true;
					}
				}
				return base.ProcessKeyPreview(ref msg);
			}

	// Process the tab key.
	protected override bool ProcessTabKey(bool forward)
			{
				return SelectNextControl(ActiveControl, forward, true, true, true);
			}

	protected override bool ProcessDialogChar(char charCode)
			{
				if (GetTopLevel() && ProcessMnemonic(charCode))
					return true; 
				return base.ProcessDialogChar(charCode); 
			}

	// Inner core of "Scale".
	[TODO]
	protected override void ScaleCore(float dx, float dy)
			{
				base.ScaleCore(dx, dy);
			}

	// Select this control.
	protected override void Select(bool directed, bool forward)
			{
				if (directed)
					base.SelectNextControl(null, forward, true, true, false);
			
				if (TopLevel)
					toolkitWindow.Focus();

				Form parent = ParentForm;
				if (parent != null)
					parent.ActiveControl = this;
			}

	protected override void UpdateDefaultButton()
			{
				// Find the bottom active control.
				ContainerControl c = this;
				while (true)
				{
					ContainerControl nextActive = c.ActiveControl as ContainerControl;
					if (nextActive == null)
					{
						break;
					}
					c = nextActive;
				}

				IButtonControl newDefaultButton = c as IButtonControl;
				if (c == null)
				{
					newDefaultButton = acceptButton;
				}

				if (newDefaultButton != defaultButton)
				{
					// Notify the previous button that it is not the default.
					if (defaultButton != null)
					{
						defaultButton.NotifyDefault(false);
					}
					defaultButton = newDefaultButton;
					if (defaultButton != null)
					{
						defaultButton.NotifyDefault(true);
					}
				}
			} 

	// Inner core of "SetBounds".
	protected override void SetBoundsCore
				(int x, int y, int width, int height,
				 BoundsSpecified specified)
			{
				base.SetBoundsCore(x, y, width, height, specified);
			}

	public override Point ClientOrigin
			{
				get 
				{
					if (menu != null)
						return ToolkitDrawOrigin + new Size(0, SystemInformation.MenuHeight);
					else
						return ToolkitDrawOrigin;
				}
			}

	protected override Point ToolkitDrawOrigin
			{
				get
				{
					int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
					if(FormBorderStyle != FormBorderStyle.None)
					{
						ToolkitManager.Toolkit.GetWindowAdjust
							(out leftAdjust, out topAdjust,
							out rightAdjust, out bottomAdjust, GetFullFlags());
						return new Point(leftAdjust, topAdjust);
					}
					else
					{
						return Point.Empty;
					}
				}
			}

	protected override Size ToolkitDrawSize
			{
				get
				{
					if(FormBorderStyle != FormBorderStyle.None)
					{
						int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
						ToolkitManager.Toolkit.GetWindowAdjust
							(out leftAdjust, out topAdjust,
							out rightAdjust, out bottomAdjust, GetFullFlags());
						return new Size(leftAdjust + rightAdjust, topAdjust + bottomAdjust);
					}
					else
					{
						return Size.Empty;
					}
				}
			}
	
	// Convert a client size into a window bounds size.
	internal override Size ClientToBounds(Size size)
			{
				int leftAdjust = 0, topAdjust = 0, rightAdjust = 0, bottomAdjust = 0;

				if (FormBorderStyle != FormBorderStyle.None)
				{
					ToolkitManager.Toolkit.GetWindowAdjust
						(out leftAdjust, out topAdjust,
						out rightAdjust, out bottomAdjust, GetFullFlags());
				}
				
				if (Menu != null)
					topAdjust += SystemInformation.MenuHeight;
				return new Size(size.Width + leftAdjust + rightAdjust,
					size.Height + topAdjust + bottomAdjust);
			}

	// Inner core of setting the visibility state.
	protected override void SetVisibleCore(bool value)
			{
				base.SetVisibleCore(value);
			}

	// Close request received from "Control.ToolkitClose".
	internal override void CloseRequest()
			{
				if( IsDisposed ) return;	// irgnore CloseRequest, if form was destroyed
				
				CancelEventArgs args = new CancelEventArgs();
				OnClosing(args);
				if(!(args.Cancel))
				{
					dialogResultIsSet = true;	// must be set here, or Application.InnerMessageLoop won't end
					OnClosed(EventArgs.Empty);
					if( !Modal) Dispose();
				}
			}

	// Window state change request received.
	internal override void WindowStateChanged(FormWindowState state)
			{
				windowState = state;
			}

	// Event that is called when an MDI child is activated or deactivated.
	internal override void MdiActivate(IToolkitWindow child)
			{
				if(mdiClient != null)
				{
					mdiClient.Activate(child);
				}
				OnMdiChildActivate(EventArgs.Empty);
			}

#if !CONFIG_COMPACT_FORMS

	// Default window procedure for this control class.
	protected override void DefWndProc(ref Message msg)
			{
				base.DefWndProc(ref msg);
			}

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

	// Collection of child controls.
	public new class ControlCollection : Control.ControlCollection
	{
		// Internal state.
		private Form formOwner;

		// Constructor.
		public ControlCollection(Form owner) : base(owner)
				{
					this.formOwner = owner;
				}

		// Override the "Add" and "Remove" behavior.
		[TODO]
		public override void Add(Control control)
				{
					base.Add(control);
				}
		[TODO]
		public override void Remove(Control control)
				{
					base.Remove(control);
				}

	}; // class ControlCollection

	protected override void OnMouseDown(MouseEventArgs e)
			{
				// If the mouse is in the non client area,
				// it must be over the menu
				if (e.Y < 0 && menu != null)
					menu.OnMouseDown(e);
						
				base.OnMouseDown (e);
			}

	protected override void OnMouseLeave(EventArgs e)
			{
				// The menu needs to remove the highlighting
				if (menu != null)
					menu.OnMouseLeave();
				base.OnMouseLeave (e);
			}


	protected override void OnMouseMove(MouseEventArgs e)
			{
				// If the mouse is in the non client area,
				// it must be over the menu
				if (e.Y < 0 && menu != null)
					menu.OnMouseMove(e);
				base.OnMouseMove (e);
			}

	[TODO]
	public bool ControlBox
			{
				get
				{
					return controlBox;
				}
				set
				{
					if (value != controlBox)
					{
						controlBox = value;
						// Implement
					}
				}
			}

}; // class Form

}; // namespace System.Windows.Forms
