/*
 * UpDownBase.cs - Implementation of the
 *			"System.Windows.Forms.UpDownBase" class.
 *
 * Copyright (C) 2003 Free Software Foundation
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

using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.Themes;
using System.Diagnostics;

namespace System.Windows.Forms
{

[TODO]
public abstract class UpDownBase : ContainerControl
{
	internal enum ButtonID
	{
		None = 0,
		Up = 1,
		Down = 2
	};

	[TODO]
	internal class UpDownButtons : Control
	{
		private UpDownBase parent;
		private Rectangle upButton = Rectangle.Empty;
		private Rectangle downButton = Rectangle.Empty;
		private ButtonID captured;
		private ButtonID pressed;
		private Timer timer;
		private int mouseX;
		private int mouseY;
		
		private const int repeatDelay = 50;
		private const int startDelay = 300;

		internal UpDownButtons(UpDownBase parent) : base()
		{
			SetStyle(ControlStyles.Selectable, false);
			this.parent = parent;
			timer = new Timer();
			pressed = ButtonID.None;
			captured = ButtonID.None;
		}
 		
		private void BeginButtonPress(MouseEventArgs e)
		{
			int x = e.X;
			int y = e.Y;

			mouseX = x;
			mouseY = y;

			// Set focus to parent 
			if (!parent.ContainsFocus)
			{
				parent.Focus();
			}

			if (upButton.Contains(x,y))
			{
				pressed = ButtonID.Up;
			}
			else 
			{
				if (downButton.Contains(x,y))
				{
					pressed = ButtonID.Down;
				}
				else
				{
					pressed = ButtonID.None;
				}
			}
			base.OnMouseDown(e);
			if (pressed != ButtonID.None)
			{
				Invalidate();
				OnUpDown(new UpDownEventArgs((int)pressed));
				base.Capture = true;
				captured = pressed;
				StartTimer();
			}
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return base.CreateAccessibilityInstance();
		}
			
		private void EndButtonPress()
		{
			if (captured != ButtonID.None)
			{
				StopTimer();
				pressed = ButtonID.None;
				captured = pressed;
				base.Capture = false;
				Invalidate();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				BeginButtonPress(e);
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			int x = e.X;
			int y = e.Y;

			mouseX = e.X;
			mouseY = e.Y;
			if (captured != ButtonID.None)
			{
				if ((captured == ButtonID.Up) && (upButton.Contains(x, y)))
				{
					if (captured != pressed)
					{
						pressed = captured;
						StartTimer();
						Invalidate();
					}
				}
				else
				{
					if ((captured == ButtonID.Down) && (downButton.Contains(x, y)))
					{
						if (captured != pressed)
						{
							pressed = captured;
							StartTimer();
							Invalidate();
						}
					}
					else
					{
						if (pressed != ButtonID.None)
						{
							pressed = ButtonID.None;
							StopTimer();
							Invalidate();
						}
					}
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			EndButtonPress();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e.Graphics, e.ClipRectangle);
			base.OnPaint(e);
		}

		protected virtual void OnUpDown(UpDownEventArgs e)
		{
			if (UpDown != null)
			{
				UpDown(this, e);
			}
		}

		protected void StartTimer()
		{
			timer.Tick += new EventHandler(TimerHandler);
			timer.Interval = startDelay;
			timer.Start();
		}

		protected void StopTimer()
		{
			timer.Stop();
			timer.Tick -= new EventHandler(TimerHandler);
		}

		private void TimerHandler(object source, EventArgs e)
		{
			timer.Stop();
			timer.Interval = repeatDelay;
			OnUpDown(new UpDownEventArgs((int)pressed));
			timer.Start();
		}
		
		public event UpDownEventHandler UpDown;
	
		// Draw if visible and created
		private void Draw(Graphics g, Rectangle drawBounds)
		{
			if (!Visible || !IsHandleCreated) { return; }
			LayoutButtons();
			ThemeManager.MainPainter.DrawScrollBar(g,  ClientRectangle,
								   drawBounds,
								   ForeColor, BackColor,
								   true, Enabled,
								   Rectangle.Empty, Rectangle.Empty,
								   upButton, (pressed == ButtonID.Up),
								   downButton, (pressed == ButtonID.Down));
		}

		private void LayoutButtons()
		{
			Size s = ClientSize;

			upButton = new Rectangle(0, 0, s.Width, s.Height / 2);
			downButton = new Rectangle(0, s.Height - (s.Height / 2), s.Width, s.Height / 2);
		}

	}; // class UpDownButtons

	internal class UpDownEdit : TextBox
	{
		private UpDownBase parent;
		private bool doubleClickFired;

		internal UpDownEdit(UpDownBase parent) : base()
		{
			this.parent = parent;
			doubleClickFired = false;
		}
		
		[TODO]
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

		[TODO]
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
		}
		
		[TODO]
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
		}

		[TODO]
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
		}

		[TODO]
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (parent.InterceptArrowKeys)
			{
				if ((keyData & Keys.Alt) == 0)
				{
					Keys key = keyData & Keys.KeyCode;

					switch (key)
					{
						case Keys.Up:
							parent.UpButton();
							return true;
						case Keys.Down:
							parent.DownButton();
							return true;
					}
	
				}
			}
			return base.ProcessDialogKey(keyData);
		}
	}; // class UpDownEdit

	private const System.Windows.Forms.BorderStyle DefaultBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
	private const int DefaultButtonsWidth = 20;
	private LeftRightAlignment upDownAlign;
	private UpDownButtons upDownButtons;
	private UpDownEdit upDownEdit;
	private bool userEdit;
	private bool interceptArrowKeys;
	private HorizontalAlignment textAlignment;
	
	[TODO]
	public UpDownBase() : base()
	{
		textAlignment = HorizontalAlignment.Left;
		base.BorderStyleInternal = DefaultBorderStyle;
		upDownEdit = new UpDownEdit(this);
		upDownEdit.AutoSize = true;
		upDownEdit.ReadOnly = false;
		upDownEdit.BorderStyle = BorderStyle.None;
		upDownEdit.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
		upDownEdit.KeyPress += new KeyPressEventHandler(OnTextBoxKeyPress);
		upDownEdit.LostFocus += new EventHandler(OnTextBoxLostFocus);
		upDownEdit.Resize += new EventHandler(OnTextBoxResize);
		upDownEdit.TextChanged += new EventHandler(OnTextBoxTextChanged);
		upDownButtons = new UpDownButtons(this);
		upDownButtons.TabStop = false;
		upDownButtons.UpDown += new UpDownEventHandler(upDownButtons_UpDown);
		upDownAlign = LeftRightAlignment.Right;
		interceptArrowKeys = true;
		base.Size = ClientToBounds(new Size(upDownEdit.Width + DefaultButtonsWidth, upDownEdit.Height));
		Controls.AddRange(new Control[] {upDownButtons, upDownEdit});
	}

	public BorderStyle BorderStyle
	{
		get
		{
			return base.BorderStyleInternal;
		}
		set
		{
			if (base.BorderStyleInternal != value)
			{
				base.BorderStyleInternal = value;
			}
		}
	}

	public abstract void DownButton();

	public bool InterceptArrowKeys
	{
		get
		{
			return interceptArrowKeys;
		}
		set
		{
			interceptArrowKeys = value;
		}
	}

	protected virtual void OnChanged(object source, EventArgs e)
	{
	}

	[TODO]
	protected override void OnFontChanged(EventArgs e)
	{
		Size s;

		base.OnFontChanged(e);
		upDownEdit.Font = Font;
		s = upDownEdit.Size;
		Height = ClientToBounds(new Size(0, s.Height)).Height;
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		doLayout();
	}

	protected override void OnLayout(LayoutEventArgs e)
	{
		base.OnLayout(e);
		doLayout();
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if( e.Delta > 0 ) {
			this.UpButton();
		}
		else {
			this.DownButton();
		}
		base.OnMouseWheel(e);
	}

	protected virtual void OnTextBoxKeyDown(object source, KeyEventArgs e)
	{
		this.OnKeyDown(e);
		if (this.interceptArrowKeys)
		{
				if (e.KeyData == Keys.Up)
				{
						this.UpButton();
						e.Handled = true;
				}
				else if (e.KeyData == Keys.Down)
				{
						this.DownButton();
						e.Handled = true;
				}
		}
		if ((e.KeyCode == Keys.Return) && this.UserEdit)
		{
				this.ValidateEditText();
		}
	}
	
	protected virtual void ValidateEditText() 
	{
	}

	protected virtual void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
	{
		this.OnKeyPress(e);
	}

	protected virtual void OnTextBoxLostFocus(object source, EventArgs e)
	{
		if(this.UserEdit)
		{
			this.ValidateEditText();
		}
	}

	protected virtual void OnTextBoxResize(object source, EventArgs e)
	{
		Size s;

		s = upDownEdit.Size;
		s = ClientToBounds(new Size(s.Width + DefaultButtonsWidth, s.Height));
		this.Size = s;
		doLayout();
	}

	[TODO]
	protected virtual void OnTextBoxTextChanged(object source, EventArgs e)
	{
		userEdit = true;
		this.OnTextChanged(e);
		this.OnChanged(source, new EventArgs());
	}

	[DefaultValue(false)]	
	public bool ReadOnly
	{
		get
		{
			return upDownEdit.ReadOnly;
		}
		set
		{
			upDownEdit.ReadOnly = value;
		}
	}

	[TODO]
	public void Select(int start, int length)
	{
		upDownEdit.Select(start, length);
	}

	[TODO]
	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
		doLayout();
	}

	[TODO]
	public override string Text
	{
		get
		{
			return upDownEdit.Text;
		}
		set
		{
			upDownEdit.Text = value;
		}	
	}

	public abstract void UpButton();

	protected abstract void UpdateEditText();

	public HorizontalAlignment TextAlign
	{
		get
		{
			return textAlignment;
		}
		set
		{
			if(textAlignment != value)
			{
				textAlignment = value;
				doLayout();
				Invalidate();
			}
		}
	}

	public LeftRightAlignment UpDownAlign
	{
		get
		{
			return upDownAlign;
		}
		set
		{
			if (upDownAlign != value)
			{
				upDownAlign = value;
				doLayout();
				Invalidate();
			}
		}
	}

	internal UpDownButtons UpDownButtonsInternal
	{
		get
		{
			return upDownButtons;
		}
	}

	protected bool UserEdit
	{
		get
		{
			return userEdit;
		}
		set
		{
			userEdit = value;
		}
	}

	[TODO]
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			upDownEdit.Dispose();
			upDownButtons.Dispose();
		}
		base.Dispose(disposing);		
	}

	public new event EventHandler BackgroundImageChanged;

	public new event EventHandler MouseEnter;

	public new event EventHandler MouseHover;

	public new event EventHandler MouseLeave;

	public new event MouseEventHandler MouseMove;

	public event UpDownEventHandler UpDown;

	private void upDownButtons_UpDown(Object source, UpDownEventArgs e)
	{
		if (e.ButtonID == (int)(ButtonID.Up))
		{
			UpButton();
		}
		else
		{
			if (e.ButtonID == (int)(ButtonID.Down))
			{
				DownButton();
			}
		}
	}

	private void doLayout()
	{
		Size s = base.ClientSize;

		if (s.Width > DefaultButtonsWidth)
		{
			upDownButtons.Size = new Size(DefaultButtonsWidth, s.Height);
			upDownEdit.Size = new Size(s.Width - DefaultButtonsWidth, s.Height);
			// this should work but doesn't
			if ((upDownAlign == LeftRightAlignment.Left) ^ (base.RightToLeft == RightToLeft.Yes))
			{
				upDownButtons.Location = new Point(0, 0); 
				upDownEdit.Location = new Point(DefaultButtonsWidth, 0); 
			}
			else
			{
				upDownButtons.Location = new Point(s.Width - DefaultButtonsWidth, 0); 
				upDownEdit.Location = new Point(0, 0); 
			}
		}
		else
		{
			upDownButtons.Size = new Size(DefaultButtonsWidth, s.Height);
			upDownButtons.Location = new Point(0, 0); 
			upDownEdit.Size = new Size(0, 0); 
			upDownEdit.Location = new Point(0, 0); 
		}
	}

}; // class UpDownBase
	
}; // namespace System.Windows.Forms

