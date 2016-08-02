/*
 * MessageBox.cs - Implementation of "System.Windows.Forms.MessageBox" class. 
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

using System;
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;

public class MessageBox
{
	// Internal state.
	private static Icon handIcon;
	private static Icon questionIcon;
	private static Icon exclamationIcon;
	private static Icon asteriskIcon;

	// Cannot instantiate this class.
	private MessageBox() {}

	// Show a message box and get the result code.
	public static DialogResult Show(String text)
			{
				return Show(null, text, String.Empty,
							MessageBoxButtons.OK,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(IWin32Window owner, String text)
			{
				return Show(owner, text, String.Empty,
							MessageBoxButtons.OK,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(String text, String caption)
			{
				return Show(null, text, caption,
							MessageBoxButtons.OK,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(IWin32Window owner, String text, 
									String caption)
			{
				return Show(owner, text, caption,
							MessageBoxButtons.OK,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(String text, String caption, 
									MessageBoxButtons buttons)
			{
				return Show(null, text, caption, buttons,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(IWin32Window owner, String text, 
									String caption, 
									MessageBoxButtons buttons)
			{
				return Show(owner, text, caption, buttons,
							MessageBoxIcon.None,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(String text, String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon)
			{
				return Show(null, text, caption, buttons, icon,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(IWin32Window owner, String text, 
									String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon)
			{
				return Show(owner, text, caption, buttons, icon,
							MessageBoxDefaultButton.Button1,
							(MessageBoxOptions)0);
			}
	public static DialogResult Show(String text, String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon, 
									MessageBoxDefaultButton defaultButton)
			{
				return Show(null, text, caption, buttons, icon,
							defaultButton, (MessageBoxOptions)0);
			}
	public static DialogResult Show(IWin32Window owner, String text, 
									String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon, 
									MessageBoxDefaultButton defaultButton)
			{
				return Show(owner, text, caption, buttons, icon,
							defaultButton, (MessageBoxOptions)0);
			}
	public static DialogResult Show(String text, String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon, 
									MessageBoxDefaultButton defaultButton,
									MessageBoxOptions options)
			{
				return Show(null, text, caption, buttons, icon,
							defaultButton, options);
			}
	public static DialogResult Show(IWin32Window owner, String text, 
									String caption, 
									MessageBoxButtons buttons, 
									MessageBoxIcon icon, 
									MessageBoxDefaultButton defaultButton,
									MessageBoxOptions options)
			{
				MessageBoxForm form = new MessageBoxForm
					(text, caption, buttons, icon,
					 defaultButton, options);
				DialogResult result = form.ShowDialog(owner as Form);
				form.DisposeDialog();
				return result;
			}

	// Load a particular message box icon from this assembly's resources.
	private static Icon LoadIcon(MessageBoxIcon icon)
			{
				lock(typeof(MessageBox))
				{
					Icon iconObject = null;
					try
					{
						switch(icon)
						{
							case MessageBoxIcon.Hand:
							{
								if(handIcon == null)
								{
									handIcon = System.Drawing.SystemIcons.Hand;
								}
								iconObject = handIcon;
							}
							break;
	
							case MessageBoxIcon.Question:
							{
								if(questionIcon == null)
								{
									questionIcon = System.Drawing.SystemIcons.Question;
								}
								iconObject = questionIcon;
							}
							break;
	
							case MessageBoxIcon.Exclamation:
							{
								if(exclamationIcon == null)
								{
									exclamationIcon = System.Drawing.SystemIcons.Exclamation;
								}
								iconObject = exclamationIcon;
							}
							break;
	
							case MessageBoxIcon.Asterisk:
							{
								if(asteriskIcon == null)
								{
									asteriskIcon = System.Drawing.SystemIcons.Asterisk;
								}
								iconObject = asteriskIcon;
							}
							break;
						}
					}
					catch
					{
						// Could not load the icon - ignore this condition.
					}
					return iconObject;
				}
			}

	// Form class for message box dialogs.
	private sealed class MessageBoxForm : Form
	{
		// Internal state.
		private MessageBoxButtons buttons;
		private Control iconControl;
		private Icon icon;
		private Label textLabel;
		private Button button1;
		private Button button2;
		private Button button3;
		private VBoxLayout vbox;
		private HBoxLayout hbox;
		private ButtonBoxLayout buttonBox;
		private bool hasCancel;

		// Constructor.
		public MessageBoxForm(String text, String caption, 
							  MessageBoxButtons buttons, 
							  MessageBoxIcon icon, 
							  MessageBoxDefaultButton defaultButton,
							  MessageBoxOptions options)
			{
				// Set the message box's caption.
				if(caption != null)
				{
					Text = caption;
				}
				else
				{
					Text = String.Empty;
				}

				// Make the borders suitable for a dialog box.
				FormBorderStyle = FormBorderStyle.FixedDialog;
				MinimizeBox = false;
				ShowInTaskbar = false;

				// Create the layout areas.
				vbox = new VBoxLayout();
				hbox = new HBoxLayout();
				buttonBox = new ButtonBoxLayout();
				vbox.Controls.Add(hbox);
				vbox.Controls.Add(buttonBox);
				vbox.StretchControl = hbox;
				buttonBox.UniformSize = true;
				vbox.Dock = DockStyle.Fill;
				Controls.Add(vbox);

				// Create a control to display the message box icon.
				this.icon = LoadIcon(icon);
				if(this.icon != null)
				{
					iconControl = new Control();
					iconControl.ClientSize = this.icon.Size;
					iconControl.TabStop = false;
					hbox.Controls.Add(iconControl);
				}

				// Create the label containing the message text.
				textLabel = new Label();
				textLabel.TextAlign = ContentAlignment.MiddleLeft;
				textLabel.TabStop = false;
				if(text != null)
				{
					textLabel.Text = text;
				}
				else
				{
					textLabel.Text = String.Empty;
				}
				hbox.Controls.Add(textLabel);

				// Determine the number and names of the message box buttons.
				this.buttons = buttons;
				switch(buttons)
				{
					case MessageBoxButtons.OK: default:
					{
						button1 = new Button();
						button1.Text = S._("SWF_MessageBox_OK", "OK");
						button2 = null;
						button3 = null;
						hasCancel = true;	// Can always cancel an OK dialog.
					}
					break;

					case MessageBoxButtons.OKCancel:
					{
						button1 = new Button();
						button2 = new Button();
						button1.Text = S._("SWF_MessageBox_OK", "OK");
						button2.Text = S._("SWF_MessageBox_Cancel", "Cancel");
						button3 = null;
						hasCancel = true;
					}
					break;

					case MessageBoxButtons.AbortRetryIgnore:
					{
						button1 = SetHotkeyPrefix(new Button());
						button2 = SetHotkeyPrefix(new Button());
						button3 = SetHotkeyPrefix(new Button());
						button1.Text = S._("SWF_MessageBox_Abort", "&Abort");
						button2.Text = S._("SWF_MessageBox_Retry", "&Retry");
						button3.Text = S._("SWF_MessageBox_Ignore", "&Ignore");
						hasCancel = false;
					}
					break;

					case MessageBoxButtons.YesNoCancel:
					{
						button1 = SetHotkeyPrefix(new Button());
						button2 = SetHotkeyPrefix(new Button());
						button3 = SetHotkeyPrefix(new Button());
						button1.Text = S._("SWF_MessageBox_Yes", "&Yes");
						button2.Text = S._("SWF_MessageBox_No", "&No");
						button3.Text = S._("SWF_MessageBox_Cancel", "Cancel");
						hasCancel = true;
					}
					break;

					case MessageBoxButtons.YesNo:
					{
						button1 = SetHotkeyPrefix(new Button());
						button2 = SetHotkeyPrefix(new Button());
						button1.Text = S._("SWF_MessageBox_Yes", "&Yes");
						button2.Text = S._("SWF_MessageBox_No", "&No");
						button3 = null;
						hasCancel = false;
					}
					break;

					case MessageBoxButtons.RetryCancel:
					{
						button1 = SetHotkeyPrefix(new Button());
						button2 = SetHotkeyPrefix(new Button());
						button1.Text = S._("SWF_MessageBox_Retry", "&Retry");
						button2.Text = S._("SWF_MessageBox_Cancel", "Cancel");
						button3 = null;
						hasCancel = true;
					}
					break;
				}

				// Add the buttons to the control.
				buttonBox.Controls.Add(button1);
				if(button2 != null)
				{
					buttonBox.Controls.Add(button2);
				}
				if(button3 != null)
				{
					buttonBox.Controls.Add(button3);
				}

				// Set the "Accept" and "Cancel" buttons.
				Button acceptButton = null;
				if(hasCancel)
				{
					if(button3 != null)
					{
						CancelButton = button3;
					}
					else if(button2 != null)
					{
						CancelButton = button2;
					}
				}
				switch(defaultButton)
				{
					case MessageBoxDefaultButton.Button1: default:
					{
						acceptButton = button1;
					}
					break;

					case MessageBoxDefaultButton.Button2:
					{
						if(button2 != null)
						{
							acceptButton = button2;
						}
						else
						{
							acceptButton = button1;
						}
					}
					break;

					case MessageBoxDefaultButton.Button3:
					{
						if(button3 != null)
						{
							acceptButton = button3;
						}
						else if(button2 != null)
						{
							acceptButton = button2;
						}
						else
						{
							acceptButton = button1;
						}
					}
					break;
				}
				AcceptButton = acceptButton;

				// Hook up the events for the form.
				button1.Click += new EventHandler(Button1Clicked);
				if(button2 != null)
				{
					button2.Click += new EventHandler(Button2Clicked);
				}
				if(button3 != null)
				{
					button3.Click += new EventHandler(Button3Clicked);
				}
				Closing += new CancelEventHandler(CloseRequested);
				if(iconControl != null)
				{
					iconControl.Paint += new PaintEventHandler(PaintIcon);
				}

				// Set the initial message box size to the vbox's recommended.
				ClientSize = vbox.RecommendedSize;
			}

		// Detect when button 1 is clicked.
		private void Button1Clicked(Object sender, EventArgs args)
			{
				switch(buttons)
				{
					case MessageBoxButtons.OK: default:
					case MessageBoxButtons.OKCancel:
						DialogResult = DialogResult.OK; break;

					case MessageBoxButtons.AbortRetryIgnore:
						DialogResult = DialogResult.Abort; break;

					case MessageBoxButtons.YesNoCancel:
					case MessageBoxButtons.YesNo:
						DialogResult = DialogResult.Yes; break;

					case MessageBoxButtons.RetryCancel:
						DialogResult = DialogResult.Retry; break;
				}
			}

		// Detect when button 2 is clicked.
		private void Button2Clicked(Object sender, EventArgs args)
			{
				switch(buttons)
				{
					case MessageBoxButtons.OKCancel:
						DialogResult = DialogResult.Cancel; break;

					case MessageBoxButtons.AbortRetryIgnore:
						DialogResult = DialogResult.Retry; break;

					case MessageBoxButtons.YesNoCancel:
					case MessageBoxButtons.YesNo:
						DialogResult = DialogResult.No; break;

					case MessageBoxButtons.RetryCancel:
						DialogResult = DialogResult.Cancel; break;
				}
			}

		// Detect when button 3 is clicked.
		private void Button3Clicked(Object sender, EventArgs args)
			{
				switch(buttons)
				{
					case MessageBoxButtons.AbortRetryIgnore:
						DialogResult = DialogResult.Ignore; break;

					case MessageBoxButtons.YesNoCancel:
						DialogResult = DialogResult.Cancel; break;
				}
			}

		// Handle the "Closing" event on the form.
		private void CloseRequested(Object sender, CancelEventArgs args)
			{
				if(hasCancel)
				{
					if(buttons == MessageBoxButtons.OK)
					{
						DialogResult = DialogResult.OK;
					}
					else
					{
						DialogResult = DialogResult.Cancel;
					}
				}
				else
				{
					// There is no "Cancel" button, so prevent the
					// dialog box from being closed until the user
					// selects one of the displayed buttons.
					args.Cancel = true;
				}
			}

		// Paint the icon control.
		private void PaintIcon(Object sender, PaintEventArgs args)
			{
				Graphics g = args.Graphics;
				g.DrawIcon(icon, 0, 0);
			}

		private static Button SetHotkeyPrefix(Button btn)
			{
				StringFormat format = btn.GetStringFormat();
				format.HotkeyPrefix = HotkeyPrefix.Show;
				return btn;
			}

		// Dispose of this dialog.
		public void DisposeDialog()
			{
				Dispose(true);
			}

	}; // class MessageBoxForm

}; // class MessageBox

}; // namespace System.Windows.Forms
