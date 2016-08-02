/*
 * GroupBox.cs - Implementation of "System.Windows.Forms.GroupBox" class
 *
 * Copyright (C) 2003  Free Software Foundation, Inc.
 *
 * Contributions from Cecilio Pardo <cpardo@imayhem.com>
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.Themes;

public class GroupBox : Control
{
	// Internal state.
	private FlatStyle flatStyle;
	private bool entered;

	// Constructor.
	public GroupBox()
			{
				base.TabStop = false;
				this.flatStyle = FlatStyle.Standard;
				this.entered = false;
				SetStyle(ControlStyles.ContainerControl, true);
				SetStyle(ControlStyles.Selectable, false);
			}

	// Properties.
	public override bool AllowDrop 
			{
				get { return base.AllowDrop; }
				set { base.AllowDrop = value; }
			}
	protected override CreateParams CreateParams
			{
				get { return base.CreateParams; }
			}
	protected override Size DefaultSize 
			{
				get { return new Size(200, 100); }
			}
	public override Rectangle DisplayRectangle
			{
				get
				{
					Size clientSize = ClientSize;
					int x = 0;
					int y = 0;
					int width = clientSize.Width;
					int height = clientSize.Height;
					int fontHeight = Font.Height;

					// Correct the rectangle for borders
					x += 2;
					y += fontHeight + 2;
					width -= 4;
					height -= fontHeight + 4;

					// Handle edge cases.
					width = (width < 0) ? 0 : width;
					height = (height < 0) ? 0 : height;

					return new Rectangle(x, y, width, height);
				}
			}
	public FlatStyle FlatStyle
			{
				get { return flatStyle; }
				set
				{
					if(flatStyle == value) { return; }

					if(!Enum.IsDefined(typeof(FlatStyle), value))
					{
					#if CONFIG_COMPONENT_MODEL
						throw new InvalidEnumArgumentException
							("FlatStyle", (int)value, typeof(FlatStyle));
					#else
						throw new ArgumentException
							("FlatStyle = "+(int)value);
					#endif
					}
					flatStyle = value;
					Invalidate();
				}
			}
	public new bool TabStop
			{
				get { return base.TabStop; }
				set { base.TabStop = value; }
			}
	public override String Text
			{
				get { return base.Text; }
				set
				{
					base.Text = value;
					Invalidate();
				}
			}

	// Methods.
	private void Draw(Graphics g)
			{
				IThemePainter themePainter;

				themePainter = ThemeManager.PainterForStyle(flatStyle);
				using(Brush bgBrush = CreateBackgroundBrush())
				{
					themePainter.DrawGroupBox
						(g, ClientRectangle, ForeColor, BackColor, bgBrush,
						 Enabled, entered, flatStyle, Text, Font,
						 GetStringFormat());
				}
			}
	private StringFormat GetStringFormat()
			{
				StringFormat format = new StringFormat();
				if(ShowKeyboardCues)
				{
					format.HotkeyPrefix = HotkeyPrefix.Show;
				}
				else
				{
					format.HotkeyPrefix = HotkeyPrefix.Hide;
				}
				if(RightToLeft == RightToLeft.Yes)
				{
					format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				}
				return format;
			}
	protected override void OnFontChanged(EventArgs e)
			{
				Invalidate();
				base.OnFontChanged(e);
			}
	protected override void OnMouseEnter(EventArgs e)
			{
				entered = true;
				Invalidate();
				base.OnMouseEnter(e);
			}
	protected override void OnMouseLeave(EventArgs e)
			{
				entered = false;
				Invalidate();
				base.OnMouseLeave(e);
			}
	protected override void OnPaint(PaintEventArgs e)
			{
				if(Visible && IsHandleCreated)
				{
					Draw(e.Graphics);
				}
				base.OnPaint(e);
			}
	protected override bool ProcessMnemonic(char charCode)
			{
				// check this control's text for the mnemonic
				if(!IsMnemonic(charCode, base.Text))
				{
					return false;
				}

				// make sure all the base controls are visible and enabled
				for(Control c = this; c != null; c = c.Parent)
				{
					if(!c.Visible || !c.Enabled)
					{
						return false;
					}
				}

				// focus on the first selectable child control
				SelectNextControl(null, true, true, true, false);

				// let the caller know that the mnemonic has been processed
				return true;
			}
	public override String ToString()
			{
				return base.ToString() + ", Text: " + Text;
			}
#if !CONFIG_COMPACT_FORMS
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}
#endif // !CONFIG_COMPACT_FORMS

	// Events.
	public new event EventHandler Click
			{
				add { base.Click += value; }
				remove { base.Click -= value; }
			}
	public new event EventHandler DoubleClick
			{
				add { base.DoubleClick += value; }
				remove { base.DoubleClick -= value; }
			}
	public new event KeyEventHandler KeyDown
			{
				add { base.KeyDown += value; }
				remove { base.KeyDown -= value; }
			}
	public new event KeyPressEventHandler KeyPress
			{
				add { base.KeyPress += value; }
				remove { base.KeyPress -= value; }
			}
	public new event KeyEventHandler KeyUp
			{
				add { base.KeyUp += value; }
				remove { base.KeyUp -= value; }
			}
	public new event MouseEventHandler MouseDown
			{
				add { base.MouseDown += value; }
				remove { base.MouseDown -= value; }
			}
	public new event EventHandler MouseEnter
			{
				add { base.MouseEnter += value; }
				remove { base.MouseEnter -= value; }
			}
	public new event EventHandler MouseLeave
			{
				add { base.MouseLeave += value; }
				remove { base.MouseLeave -= value; }
			}
	public new event MouseEventHandler MouseMove
			{
				add { base.MouseMove += value; }
				remove { base.MouseMove -= value; }
			}
	public new event MouseEventHandler MouseUp
			{
				add { base.MouseUp += value; }
				remove { base.MouseUp -= value; }
			}
	public new event EventHandler TabStopChanged
			{
				add { base.TabStopChanged += value; }
				remove { base.TabStopChanged -= value; }
			}

}; // class GroupBox

}; // namespace System.Windows.Forms
