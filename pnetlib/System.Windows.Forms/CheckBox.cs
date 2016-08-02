/*
 * CheckBox.cs - Implementation of the
 *			"System.Windows.Forms.CheckBox" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace System.Windows.Forms
{

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
[DefaultProperty("Checked")]
[DefaultEvent("CheckedChanged")]
#endif
public class CheckBox : ButtonBase
{
	// Internal result.
	private Appearance appearance;
	private ContentAlignment checkAlign;
	private bool autoCheck;
	private bool threeState;
	private CheckState state;
	private const int normalCheckSize = 13;
	private const int flatCheckSize = 13;

	// Contructor.
	public CheckBox()
			{
				Appearance = Appearance.Normal;
				checkAlign = ContentAlignment.MiddleLeft;
				TextAlign = ContentAlignment.MiddleLeft;
				autoCheck = true;
				SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
			}

	// Calculate the current state of the button for its visual appearance.
	internal override ButtonState CalculateState()
			{
				ButtonState checkState;
				if(state == CheckState.Unchecked)
				{
					checkState = ButtonState.Normal;
				}
				else if(state == CheckState.Checked)
				{
					checkState = ButtonState.Checked;
				}
				else //state == CheckState.Indeterminate
				{
					// Special flag for "IThemePainter.DrawCheckBox".
					checkState = ButtonState.Checked | (ButtonState)0x10000;
				}
				if(pressed && entered)
				{
					checkState |= ButtonState.Pushed;
				}
				if(!Enabled)
				{
					checkState |= ButtonState.Inactive;
				}
				if(FlatStyle == FlatStyle.Flat)
				{
					checkState |= ButtonState.Flat;
				}
				if(hasFocus)
				{
					// Special flag that indicates a focus rectangle.
					checkState |= (ButtonState)0x20000;
				}
				return checkState;
			}

	// Draw the contents of this check box.
	internal override void Draw(Graphics graphics)
			{
				DrawBox(graphics);
				DrawText(graphics);
			}

	private void DrawBox(Graphics graphics)
			{
				int checkX = 0;
				int checkY = 0;

				int checkSize;
				if(FlatStyle == FlatStyle.Flat)
				{
					checkSize = flatCheckSize;
				}
				else
				{
					checkSize = normalCheckSize;
				}
		
				switch (checkAlign)
				{
					case ContentAlignment.BottomCenter:
						checkX = (Width / 2) - (checkSize / 2);
						checkY = Height - checkSize - 1;
						break;
					case ContentAlignment.BottomLeft:
						checkX = 0;
						checkY = Height - checkSize - 1;
						break;
					case ContentAlignment.BottomRight:
						checkX = Width - checkSize - 1;
						checkY = Height - checkSize - 1;
						break;
					case ContentAlignment.MiddleCenter:
						checkX = (Width / 2) - (checkSize / 2);
						checkY = (Height / 2) - (checkSize / 2);
						break;
					case ContentAlignment.MiddleLeft:
						checkX = 0;
						checkY = (Height / 2) - (checkSize / 2);
						break;
					case ContentAlignment.MiddleRight:
						checkX = Width - checkSize - 1;
						checkY = (Height / 2) - (checkSize / 2);
						break;
					case ContentAlignment.TopCenter:
						checkX = (Width / 2) - (checkSize / 2);
						checkY = 0;
						break;
					case ContentAlignment.TopLeft:
						checkX = 0;
						checkY = 0;
						break;
					case ContentAlignment.TopRight:
						checkX = Width - checkSize - 1;
						checkY = 0;
						break;
				}
				ButtonState checkState = CalculateState();
				ControlPaint.DrawCheckBox
					(graphics, checkX, checkY,
					 checkSize, checkSize, checkState);
			}

	private void DrawText(Graphics graphics)
			{
				int x = 0;
				int y = 2;
				int width = Width - 2;
				int height = Height - 4;
		
				int checkSize;
				if(FlatStyle == FlatStyle.Flat)
				{
					checkSize = flatCheckSize;
				}
				else
				{
					checkSize = normalCheckSize;
				}
		
				SizeF textSize = graphics.MeasureString(Text, Font);
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Near;
				format.LineAlignment = StringAlignment.Far;
				format.HotkeyPrefix = HotkeyPrefix.Show;

				switch (checkAlign)
				{
					case ContentAlignment.BottomCenter:
						height -= checkSize + 3;
						break;
					case ContentAlignment.BottomLeft:
						x = checkSize + 3;
						width -= x;
						break;
					case ContentAlignment.BottomRight:
						width -= checkSize;
						break;
					case ContentAlignment.MiddleCenter:
						break;
					case ContentAlignment.MiddleLeft:
						x = checkSize + 3;
						width -= x;
						break;
					case ContentAlignment.MiddleRight:
						width -= checkSize;
						break;
					case ContentAlignment.TopCenter:
						y = checkSize + 3;
						height -= y;
						break;
					case ContentAlignment.TopLeft:
						x = checkSize + 3;
						width -= x;
						break;
					case ContentAlignment.TopRight:
						width -= checkSize;
						break;
				}

				switch (TextAlign)
				{
					case ContentAlignment.BottomCenter:
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomLeft:
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.BottomRight:
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Far;
						break;
					case ContentAlignment.MiddleCenter:
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleLeft:
						format.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.MiddleRight:
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Center;
						break;
					case ContentAlignment.TopCenter:
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopLeft:
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Near;
						break;
					case ContentAlignment.TopRight:
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Near;
						break;
				}
				RectangleF rect = new RectangleF(x, y, width, height);
				String text = Text;
				Font font = Font;
				if((TextAlign & (ContentAlignment.MiddleLeft |
								 ContentAlignment.MiddleCenter |
								 ContentAlignment.MiddleRight)) != 0)
				{
					rect.Offset(0.0f, GetDescent(graphics, font) / 2.0f);
				}
				if(text != null && text != String.Empty)
				{
					if(Enabled)
					{
						Brush brush = new SolidBrush(ForeColor);
						graphics.DrawString(text, font, brush, rect, format);
						brush.Dispose();
					}
					else
					{
						ControlPaint.DrawStringDisabled
							(graphics, text, font, BackColor, rect, format);
					}
				}
			}

	// Gets or sets the check box appearance.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(Appearance.Normal)]
#endif
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public Appearance Appearance 
			{
				get
				{
					return appearance; 
				}
				set
				{
					if(appearance != value)
					{
						appearance = value;
						Redraw();
						OnAppearanceChanged(EventArgs.Empty);
					}
				}
			}

	// Get or set the "auto check" style for this check box.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif
	public bool AutoCheck
			{
				get
				{
					return autoCheck;
				}
				set
				{
					autoCheck = value;
				}
			}

	// Gets or set alignment of a check box on a check box control.
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(ContentAlignment.MiddleLeft)]
#endif
	public ContentAlignment CheckAlign 
			{
				get
				{
					return checkAlign;
				}
				set
				{
					if(checkAlign != value)
					{
						checkAlign = value;
						Invalidate();
					}
				}
			}

	// Get or set the checked state as a simple boolean value.
#if CONFIG_COMPONENT_MODEL 
	[Bindable(true)]
	[RefreshProperties(RefreshProperties.All)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public bool Checked
			{
				get
				{
					return (CheckState != CheckState.Unchecked);
				}
				set
				{
					if(value)
					{
						CheckState = CheckState.Checked;
					}
					else
					{
						CheckState = CheckState.Unchecked;
					}
				}
			}

	// Get or set the check box state.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(CheckState.Unchecked)]
#endif
#if CONFIG_COMPONENT_MODEL
	[RefreshProperties(RefreshProperties.All)]
	[Bindable(true)]
#endif
	public CheckState CheckState
			{
				get
				{
					return state;
				}
				set
				{
					if(state != value)
					{
						bool checkedBefore = (state != CheckState.Unchecked);
						state = value;
						bool checkedAfter = (state != CheckState.Unchecked);
						RedrawIfChanged();
						if(checkedBefore != checkedAfter)
						{
							OnCheckedChanged(EventArgs.Empty);
						}
						OnCheckStateChanged(EventArgs.Empty);
					}
				}
			}

	// Gets or sets the site of the control.
#if CONFIG_COMPONENT_MODEL
	public override ISite Site 
			{
				get 
				{
					return base.Site;
				}
				set
				{
					base.Site = value;
				}
			}
#endif

	// Gets or sets the alignment of the text on the checkbox control.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(ContentAlignment.MiddleLeft)]
#endif
#if CONFIG_COMPONENT_MODEL 
	[Localizable(true)]
#endif
	public override ContentAlignment TextAlign 
			{
				get
				{
					return base.TextAlign;
				}
				set
				{
					base.TextAlign = value;
					Invalidate();
				}
			}

	// Get or set the "three state" style for this check box.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public bool ThreeState
			{
				get
				{
					return threeState;
				}
				set
				{
					threeState = value;
				}
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

	// Event that is emitted when the check state changes.
	public event EventHandler CheckStateChanged
			{
				add
				{
					AddHandler(EventId.CheckStateChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.CheckStateChanged, value);
				}
			}

	// Gets the required creation parameters when the control handle is created.
	protected override CreateParams CreateParams 
			{
				get
				{
					return base.CreateParams;
				}
			}

	// Gets the default size of the control.	
	protected override Size DefaultSize 
			{
				get
				{
					return new Size(104, 24);
				}
			}

	// Create an accessibility object.
	protected override AccessibleObject CreateAccessibilityInstance()
			{
				return base.CreateAccessibilityInstance();
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

	// Emit the CheckStateChanged event.
	protected virtual void OnCheckStateChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.CheckStateChanged));
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
					CheckState newState;
					if(state == CheckState.Unchecked)
					{
						newState = CheckState.Checked;
					}
					else if(state == CheckState.Checked)
					{
						if(threeState)
						{
							newState = CheckState.Indeterminate;
						}
						else
						{
							newState = CheckState.Unchecked;
						}
					}
					else
					{
						newState = CheckState.Unchecked;
					}
					CheckState = newState;
				}
				base.OnClick(e);
			}

	// Raises the HandleCreated event.
	protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
			}

	// Raises the MouseUp event.
	protected override void OnMouseUp(MouseEventArgs mevent)
			{
				if(button == mevent.Button)
				{
					bool clicked = (entered && pressed);
					base.OnMouseUp(mevent);
					if(clicked)
					{
						OnClick(EventArgs.Empty);
					}
				}
				else
				{
					base.OnMouseUp(mevent);
				}
			}

	// Processes a mnemonic character.
	protected override bool ProcessMnemonic(char charCode)
			{
				if(IsMnemonic(charCode, Text))
				{
					if(CanSelect)
					{
						OnClick(EventArgs.Empty);
						return true;
					}
				}
				return false;
			}

	public override String ToString()
			{
				return base.ToString() + ", CheckState: " +
					(state == CheckState.Unchecked ? "0" : "1");
			}
}; // class CheckBox

}; // namespace System.Windows.Forms
