/*
 * RadioButton.cs - Implementation of the
 *			"System.Windows.Forms.RadioButton" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Contributions from Mario Luca Bernardi
 * Contributions from Simon Guindon
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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Themes;

public class RadioButton : ButtonBase
{
	// Internal state.
	private Appearance appearance;
	private ContentAlignment checkAlign;
	private Rectangle content;
	private bool autoCheck;
	private bool isChecked;
	private bool needsLayout;
	private int checkX;
	private int checkY;

	private const int checkSize = 13;

	// Constructor.
	public RadioButton() : base()
			{
				appearance = Appearance.Normal;
				checkAlign = ContentAlignment.MiddleLeft;
				TextAlign = ContentAlignment.MiddleLeft;
				entered = false;
				pressed = false;
				autoCheck = true;
				isChecked = false;
				needsLayout = true;
			}

#if !CONFIG_COMPACT_FORMS
	// Get or set the appearance of this radio button control.
	public Appearance Appearance
			{
				get { return appearance; }
				set
				{
					if (appearance != value)
					{
						if (!Enum.IsDefined(typeof(Appearance), value))
						{
						#if CONFIG_COMPONENT_MODEL
							throw new InvalidEnumArgumentException
								("Appearance", (int)value, typeof(Appearance));
						#else
							throw new ArgumentException
								("Appearance = "+(int)value);
						#endif
						}
						appearance = value;
						needsLayout = true;
						Redraw();
						OnAppearanceChanged(EventArgs.Empty);
					}
				}
			}

	// Get or set the "auto check" style for this radio button.
	public bool AutoCheck
			{
				get { return autoCheck; }
				set
				{
					if (autoCheck != value)
					{
						autoCheck = value;
					}
				}
			}

	// Get or set alignment of the check box on this radio button control.
	public ContentAlignment CheckAlign
			{
				get { return checkAlign; }
				set
				{
					if (checkAlign != value)
					{
						if (!Enum.IsDefined(typeof(ContentAlignment), value))
						{
						#if CONFIG_COMPONENT_MODEL
							throw new InvalidEnumArgumentException
								("CheckAlign", (int)value,
								 typeof(ContentAlignment));
						#else
							throw new ArgumentException
								("CheckAlign = "+(int)value);
						#endif
						}
						checkAlign = value;
						needsLayout = true;
						Redraw();
					}
				}
			}
#endif

	// Disable other radio buttons in the same group as this one.
	private void DisableOthersInGroup()
			{
				// Nothing to do if we aren't an "auto check" button.
				if(!autoCheck)
				{
					return;
				}

				// Remove tab stop indications from all other buttons.
				if(Parent != null)
				{
					foreach(Control child1 in Parent.Controls)
					{
						RadioButton rb1 = (child1 as RadioButton);
						if(rb1 != null && rb1 != this)
						{
							if(rb1.autoCheck)
							{
								rb1.TabStop = false;
							}
						}
					}
				}

				// Set the tab stop indication on this button.
				TabStop = isChecked;

				// Reset all other buttons in the group.
				if(isChecked && Parent != null)
				{
					foreach(Control child2 in Parent.Controls)
					{
						RadioButton rb2 = (child2 as RadioButton);
						if(rb2 != null && rb2 != this)
						{
							if(rb2.autoCheck)
							{
								rb2.Checked = false;
							}
						}
					}
				}
			}

	// Get or set the checked state as a simple boolean value.
	public bool Checked
			{
				get
				{
					return isChecked;
				}
				set
				{
					// Bail out if the value doesn't change.
					if(isChecked == value)
					{
						return;
					}

					// Record the value and redraw this radio button.
					isChecked = value;
					RedrawIfChanged();

					// Disable the other radio buttons in the group.
					DisableOthersInGroup();

					// Notify event listeners of the change.
					OnCheckedChanged(EventArgs.Empty);
				}
			}

#if !CONFIG_COMPACT_FORMS
	// Gets the required creation parameters when the control handle is
	// created.
	protected override CreateParams CreateParams
			{
				get { return base.CreateParams; }
			}
#endif

#if !CONFIG_COMPACT_FORMS
	// Gets the default size of the control.
	protected
#else
	internal
#endif
	override Size DefaultSize
			{
				get
				{
					return new Size(104, 24);
				}
			}

	// Get or set the tab stop indication.
	public new bool TabStop
			{
				get
				{
					return base.TabStop;
				}
				set
				{
					base.TabStop = value;
				}
			}

#if !CONFIG_COMPACT_FORMS
	// Gets or sets the alignment of the text on the RadioButton control.
	public
#else
	internal
#endif
	override ContentAlignment TextAlign
			{
				get { return base.TextAlign; }
				set { base.TextAlign = value; }
			}

	// Calculate the current state of the button for its visual appearance.
	internal override ButtonState CalculateState()
			{
				ButtonState state = ButtonState.Normal;

				if (FlatStyle == FlatStyle.Flat ||
				    (FlatStyle == FlatStyle.Popup && !entered))
				{
					state |= ButtonState.Flat;
				}

				if (isChecked)
				{
					state |= ButtonState.Checked;
					if (appearance == Appearance.Button)
					{
						state |= ButtonState.Pushed;
					}
				}

				if (Enabled)
				{
					if (entered && pressed)
					{
						state |= ButtonState.Pushed;
					}
					if(hasFocus)
					{
						// Special flag that indicates a focus rectangle.
						state |= (ButtonState)0x20000;
					}
				}
				else
				{
					state |= ButtonState.Inactive;
				}
				return state;
			}

#if !CONFIG_COMPACT_FORMS
	protected override AccessibleObject CreateAccessibilityInstance()
			{
				return base.CreateAccessibilityInstance();
			}
#endif

	// Draw the button in its current state on a Graphics surface.
	internal override void Draw(Graphics graphics)
			{
				// calculate ButtonState needed by DrawRadioButton
				ButtonState state = CalculateState();
				StringFormat format = GetStringFormat();

				if (needsLayout) { LayoutElements(); }

				if (appearance == Appearance.Button)
				{
					ThemeManager.MainPainter.DrawButton
						(graphics, 0, 0, content.Width, content.Height,
						 state, ForeColor, BackColor, false);
				}
				else
				{
					using (Brush bg = CreateBackgroundBrush())
					{
						ThemeManager.MainPainter.DrawRadioButton
							(graphics, checkX, checkY, checkSize, checkSize,
							 state, ForeColor, BackColor, bg);
					}
				}

				// TODO: image drawing

				Rectangle rect = content;
				if (appearance == Appearance.Button)
				{
					int x = content.X;
					int y = content.Y;
					int width = content.Width;
					int height = content.Height;
					x += 2;
					y += 2;
					width -= 4;
					height -= 4;
					if ((state & ButtonState.Pushed) != 0)
					{
						++x;
						++x;
					}
					rect = new Rectangle(x, y, width, height);
				}
				RectangleF layout = rect;
				Font font = Font;
				if((TextAlign & (ContentAlignment.MiddleLeft |
								 ContentAlignment.MiddleCenter |
								 ContentAlignment.MiddleRight)) != 0)
				{
					layout.Offset(0.0f, GetDescent(graphics, font) / 2.0f);
				}
				if (Enabled)
				{
					using (Brush brush = new SolidBrush(ForeColor))
					{
						graphics.DrawString(Text, font, brush, layout, format);
					}
				}
				else
				{
					ControlPaint.DrawStringDisabled(graphics, Text, font, BackColor, layout, format);
				}
			}

	// Layout the elements of this radio button control.
	private void LayoutElements()
			{
				needsLayout = false;

				Size clientSize = ClientSize;
				int width = clientSize.Width;
				int height = clientSize.Height;

				if (appearance == Appearance.Button)
				{
					checkX = 0;
					checkY = 0;
					content = new Rectangle(0, 0, width, height);
				}
				else
				{
					switch (checkAlign)
					{
						case ContentAlignment.TopLeft:
						{
							checkX = 0;
							checkY = 0;
							content = new Rectangle(checkSize+3, 0,
							                        width-checkSize-3, height);
						}
						break;

						case ContentAlignment.TopCenter:
						{
							checkX = (width/2) - (checkSize/2);
							checkY = 0;
							content = new Rectangle(0, checkSize+3,
							                        width, height-checkSize-3);
						}
						break;

						case ContentAlignment.TopRight:
						{
							checkX = width - checkSize - 1;
							checkY = 0;
							content = new Rectangle(0, 0, width-checkX, height);
						}
						break;

						case ContentAlignment.MiddleLeft:
						{
							checkX = 0;
							checkY = (height/2) - (checkSize/2);
							content = new Rectangle(checkSize+3, 0,
							                        width-checkSize-3, height);
						}
						break;

						case ContentAlignment.MiddleCenter:
						{
							// for this alignment, the text is placed under
							// the check box of the radio button
							checkX = (width/2) - (checkSize/2);
							checkY = (height/2) - (checkSize/2);
							content = new Rectangle(0, 0, width, height);
						}
						break;

						case ContentAlignment.MiddleRight:
						{
							checkX = width - checkSize - 1;
							checkY = (height/2) - (checkSize/2);
							content = new Rectangle(0, 0, width-checkX, height);
						}
						break;

						case ContentAlignment.BottomLeft:
						{
							checkX = 0;
							checkY = height - checkSize - 1;
							content = new Rectangle(checkSize+3, 0,
							                        width-checkSize-3, height);
						}
						break;

						case ContentAlignment.BottomCenter:
						{
							checkX = (width/2) - (checkSize/2);
							checkY = height - checkSize - 1;
							content = new Rectangle(0, 0,
							                        width, height-checkSize-3);
						}
						break;

						case ContentAlignment.BottomRight:
						{
							checkX = width - checkSize - 1;
							checkY = height - checkSize - 1;
							content = new Rectangle(0, 0, width-checkX, height);
						}
						break;
					}
				}
			}

	// Raises the AppearanceChanged event.
	protected virtual void OnAppearanceChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.AppearanceChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Raises the CheckedChanged event.
	protected virtual void OnCheckedChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.CheckedChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Raises the Click event.
	protected override void OnClick(EventArgs e)
			{
				if(autoCheck)
				{
					Checked = true;
				}
				base.OnClick(e);
			}

	// Process a focus enter event.
	protected override void OnEnter(EventArgs e)
			{
			#if false	// TODO: uncomment once "MouseButtons" works
				if(MouseButtons == MouseButtons.None)
				{
					OnClick(e);
				}
			#endif
				base.OnEnter(e);
			}

	// Raises the HandleCreated event.
	protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
			}

	// Raises the MouseUp event.
	protected override void OnMouseUp(MouseEventArgs e)
			{
				if(button == e.Button)
				{
					bool clicked = (entered && pressed);
					pressed = false;
					if(clicked)
					{
						OnClick(EventArgs.Empty);
					}
				}
				base.OnMouseUp(e);
			}

	// Clicks this radio button.
	public void PerformClick()
			{
				OnClick(EventArgs.Empty);
			}

	// Processes a mnemonic character.
	protected override bool ProcessMnemonic(char charCode)
			{
				if(IsMnemonic(charCode, Text))
				{
					if(CanSelect)
					{
						if(Focused)
						{
							OnClick(EventArgs.Empty);
							return true;
						}
						else
						{
							// Bring the focus here, which will activate us.
							Focus();
						}
					}
				}
				return false;
			}

	// Convert this object into a string.
	public override string ToString()
			{
				return GetType().FullName.ToString() + ", Checked: " + Checked.ToString();
			}

	// Occurs when the value of the Appearance property changes.
	public event EventHandler AppearanceChanged
			{
				add
				{
					AddHandler(EventId.AppearanceChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.AppearanceChanged, value);
				}
			}

	// Occurs when the value of the Checked property changes.
	public event EventHandler CheckedChanged
			{
				add
				{
					AddHandler(EventId.CheckedChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.CheckedChanged, value);
				}
			}

}; // class RadioButton

}; // namespace System.Windows.Forms
