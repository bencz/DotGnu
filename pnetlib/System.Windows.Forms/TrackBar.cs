/*
 * TrackBar.cs - Implementation of the
 *			"System.Windows.Forms.TrackBar" class.
 *
 * Copyright (C) 2003  Neil Cawse.
 * Copyright (C) 2004  Deryk Robosson.
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
	using System.Windows.Forms.Themes;

#if CONFIG_COMPONENT_MODEL
	using System.ComponentModel;
#endif
	using System.Drawing;
	public class TrackBar : Control
#if CONFIG_COMPONENT_MODEL
		, ISupportInitialize
#endif
	{
		private bool autoSize;
		private int largeChange;
		private int maximum;
		private int minimum;
		private Orientation orientation;
		private int smallChange;
		private int tickFrequency;
		private TickStyle tickStyle;
		private bool initializing;
		private int value;
		private int trackSize;
		private int minBarSize;
		private Rectangle barRect;
		private bool barDown;
		private bool trackDown;
		private Point mouseCoords;
		private bool valuechanged;
		private Timer timer;
		private const int repeatDelay = 50;
		private const int startDelay = 300;

		public event EventHandler Scroll;
		public event EventHandler ValueChanged;

		public TrackBar()
		{
			autoSize = true;
			largeChange = 5;
			maximum = 10;
			smallChange = 1;
			tickFrequency = 1;
			tickStyle = TickStyle.BottomRight;
			orientation = Orientation.Horizontal;
			minBarSize = 8;
			trackSize = 25;
			barRect = Rectangle.Empty;
			barDown = false;
			trackDown = false;
			mouseCoords = new Point(0, 0);
			valuechanged = false;
			timer = new Timer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Interval = 1;

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
			// set cursor
			this.Cursor = Cursors.Hand;
		}

		public override Cursor Cursor
		{
			set
			{
				if( value == null ) base.Cursor = Cursors.Hand;
				else                base.Cursor = value;
			}
		}
		
		public bool AutoSize
		{
			get
			{
				return autoSize;
			}

			set
			{
				if (autoSize != value)
				{
					autoSize = value;
				}
			}
		}

		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}

			set
			{
				base.BackgroundImage = value;
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
				return ImeMode.Disable;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(104, 42);
			}
		}

		public override Font Font
		{
			get
			{
				return base.Font;
			}

			set
			{
				base.Font = value;
			}
		}
		public override Color ForeColor
		{
			get
			{
				return SystemColors.WindowText;
			}

			set
			{
			}
		}

		public new ImeMode ImeMode
		{
			get
			{
				return base.ImeMode;
			}

			set
			{
				base.ImeMode = value;
			}
		}

		public int LargeChange
		{
			get
			{
				return largeChange;
			}

			set
			{
				if (largeChange != value)
				{
					largeChange = value;
					this.Invalidate();
				}
			}
		}

		public int Maximum
		{
			get
			{
				return maximum;
			}

			set
			{
				if (maximum != value)
				{
					if (value < minimum)
						minimum = value;
					SetRange(minimum, value);
				}
			}
		}

		public int Minimum
		{
			get
			{
				return minimum;
			}

			set
			{
				if (minimum != value)
				{
					if (value > maximum)
						maximum = value;
					SetRange(value, maximum);
				}
			}
		}

		public Orientation Orientation
		{
			get
			{
				return orientation;
			}

			set
			{
				if (orientation != value)
				{
					orientation = value;
					this.Invalidate();
				}
			}
		}

		public int SmallChange
		{
			get
			{
				return smallChange;
			}

			set
			{
				if (smallChange != value)
				{
					smallChange = value;
					this.Invalidate();
				}
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				base.Text = value;
			}
		}

		public TickStyle TickStyle
		{
			get
			{
				return tickStyle;
			}

			set
			{
				if (tickStyle != value)
				{
					tickStyle = value;
					this.Invalidate();
				}
			}
		}

		public int TickFrequency
		{
			get
			{
				return tickFrequency;
			}

			set
			{
				if (tickFrequency != value)
				{
					tickFrequency = value;
					this.Invalidate();
				}
			}
		}

		public int Value
		{
			get
			{
				return value;
			}

			set
			{
				if (this.value != value)
				{
					if (value < minimum || value > maximum)
					{
						throw new ArgumentException();
					}
					this.value = value;
					valuechanged = true;
					this.Invalidate();
					OnValueChanged(EventArgs.Empty);
				}
			}
		}

		private void CheckValue()
		{
			if (initializing)
				return;
			if (value < minimum)
				value = minimum;
			if (value > maximum)
				value = maximum;
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();
		}

		public virtual void BeginInit()
		{
			initializing = true;
		}

		public virtual void EndInit()
		{
			initializing = false;
			CheckValue();
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Down:
				case Keys.Left:
				{
					SmallDecrement();
					return true;
				}
				case Keys.Up:
				case Keys.Right:
				{
					SmallIncrement();
					return true;
				}
				case Keys.Home:
				{
					return true;
				}
				default:
				{
					return base.IsInputKey(keyData);
				}
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (autoSize)
			{
				if (orientation == Orientation.Horizontal)
				{
					Size = new Size (Width, 20);
				}
				else
				{
					Size = new Size (20, Height);
				}
			}
		}

		protected virtual void OnScroll(EventArgs e)
		{
			if (Scroll != null)
			{
				Scroll(this, e);
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(!Enabled) 
			{
				return;
			}

			if(e.Delta > 0) 
			{
				SmallDecrement();
			}
			else 
			{
				SmallIncrement();
			}

			base.OnMouseWheel(e);
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, e);
			}
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);
		}

		public void SetRange(int minValue, int maxValue)
		{
			if (minimum == minValue && maximum == maxValue)
			{
				return;
			}
			
			if (minValue > maxValue)
			{
				maxValue = minValue;
			}
			
			minimum = minValue;
			maximum = maxValue;
			if (value < minimum)
			{
				value = minimum;
			}
			if (value > maximum)
			{
				value = maximum;
			}
			this.Invalidate();
		}

		// Draw the trackbar
		private void Draw(Graphics g)
		{
			// Paint based on TickStyle(None, TopLeft, BottomRight, Both)
			// Also Orientation.Horizontal/Vertical
			if (!Visible || !IsHandleCreated)
			{
				return;
			}

			if(valuechanged == true) 
			{
				CalculatePosition();
			}
			else 
			{
				CalculateBar();
			}

			ThemeManager.MainPainter.DrawTrackBar(g,
						ClientRectangle,
						barRect, orientation, Enabled,
						tickFrequency, tickStyle);
		}

		// Calculate bar rectangle based upon mouse coordinates
		private void CalculateBar()
		{
			int x = 0;
			int y = 0;

			if(barDown) 
			{
				if(orientation == Orientation.Horizontal) 
				{
					if((mouseCoords.X + (barRect.Width / 2)) > ClientRectangle.Right) 
					{
						x = ClientRectangle.Right - barRect.Width - 1;
					}
					else if((mouseCoords.X - barRect.Width / 2) < ClientRectangle.Left) 
					{
						x = 0;
					}
					else 
					{
						x = mouseCoords.X - (barRect.Width / 2);
					}
					y = ClientRectangle.Top;
				}
				else if(orientation == Orientation.Vertical) 
				{
					x = ClientRectangle.Left;
					if((mouseCoords.Y - (barRect.Height / 2)) > ClientRectangle.Bottom) 
					{
						y = ClientRectangle.Bottom - barRect.Height - 1;
					}
					else if((mouseCoords.Y - (barRect.Height / 2)) < ClientRectangle.Top) 
					{
						y = 0;
					}
					else 
					{
						y = mouseCoords.Y - (barRect.Height / 2);
					}
				}
			}

			if(orientation == Orientation.Horizontal) 
			{
				barRect = new Rectangle(
						x, ClientRectangle.Y,
						ClientRectangle.Width / 20, ClientRectangle.Height);
			}
			else if(orientation == Orientation.Vertical) 
			{
				barRect = new Rectangle(
						ClientRectangle.X, y,
						ClientRectangle.Width, ClientRectangle.Height / 14);
			}
			else 
			{
				barRect = Rectangle.Empty;
			}
		}

		// calculate a coordinate position from a supplied value
		private void CalculatePosition()
		{
			double percent = 0;

			if(orientation == Orientation.Horizontal) 
			{
				if(value == 0) 
				{
					barRect.X = 0;
					return;
				}

				percent = ClientRectangle.Width * (value / 100.0);
				if(((int)percent - (barRect.Width / 2)) < ClientRectangle.X) 
				{
					barRect.X = 0;
				}
				else if(((int)percent - (barRect.Width / 2)) > ClientRectangle.Width) 
				{
					barRect.X = ClientRectangle.Width - barRect.Width - 1;
				}
				else 
				{
					barRect.X = (int)percent - (barRect.Width / 2);
				}
			}
			else if(orientation == Orientation.Vertical) 
			{
				if(value == 0) 
				{
					barRect.Y = 0;
					return;
				}
				percent = ClientRectangle.Height * (value / 100.0);
				if(((int)percent - (barRect.Height / 2)) < ClientRectangle.Y) 
				{
					barRect.Y = 0;
				}
				else if(((int)percent - (barRect.Height / 2)) > ClientRectangle.Height) 
				{
					barRect.Y = ClientRectangle.Height - barRect.Height - 1;
				}
				else 
				{
					barRect.Y = (int)percent - (barRect.Height / 2);
				}
			}

			valuechanged = false;
		}

		// Handy property from ScrollBar.cs
		private int ValueFromPosition
		{
			get 
			{
				int v;
				if(orientation == Orientation.Vertical)
				{
					int position = barRect.Y - ClientRectangle.Y;
					double percentage = (double) position / (ClientRectangle.Height - barRect.Height);
					v = (int)(percentage * (maximum - minimum + 1 - largeChange));
				}
				else
				{
					int position = barRect.X - ClientRectangle.X;
					double percentage = (double) position / (ClientRectangle.Width - barRect.Width);
					v = (int)(percentage * (maximum - minimum + 1 - largeChange));
				}

				v += minimum;
				if (v < minimum)
				{
					v = minimum;
				}
				if (v > maximum)
				{
					v = maximum;
				}
				return v;
			}
		}

		private void LargeIncrement ()
    	{
			int pos = value + LargeChange;

			if(pos < minimum) 
			{
				value = minimum;
			}
			else if(pos > maximum) 
			{
				value = maximum;
			}
			else 
			{
				value = pos;
			}

			this.Invalidate();
			OnScroll(new EventArgs ());
    	}

    	private void LargeDecrement ()
    	{
			int pos = value - LargeChange;

			if(pos < minimum) 
			{
				value = minimum;
			}
			else if(pos > maximum) 
			{
				value = maximum;
			}
			else 
			{
				value = pos;
			}

			this.Invalidate();
			OnScroll(new EventArgs ());
    	}

		private void SmallIncrement ()
    	{
			int pos = value + SmallChange;

			if(pos < minimum) 
			{
				value = minimum;
			}
			else if(pos > maximum) 
			{
				value = maximum;
			}
			else 
			{
				value = pos;
			}

			this.Invalidate();
			OnScroll(new EventArgs ());
    	}

    	private void SmallDecrement ()
    	{
			int pos = value - SmallChange;

			if(pos < minimum) 
			{
				value = minimum;
			}
			else if(pos > maximum) 
			{
				value = maximum;
			}
			else 
			{
				value = pos;
			}

			this.Invalidate();
			OnScroll(new EventArgs ());	
    	}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left) 
			{
				return;
			}

			if(barDown == false) 
			{
				mouseCoords = new Point(e.X, e.Y);

				if(barRect.Contains(mouseCoords)) 
				{
					barDown = true;
				}
				else if(ClientRectangle.Contains(mouseCoords))
				{
					trackDown = true;
					if(ClientRectangle.Contains(mouseCoords)) 
					{
						if(orientation == Orientation.Horizontal) 
						{
							if(e.X > barRect.X + barRect.Width) 
							{
								LargeIncrement();
							}
							else if(e.X < barRect.X) 
							{
								LargeDecrement();
							}
						}
						else 
						{
							if(e.Y > barRect.Y + barRect.Height) 
							{
								LargeIncrement();
							}
							else if(e.Y < barRect.Y) 
							{
								LargeDecrement();
							}
						}
					}
					timer.Interval = startDelay;
					timer.Start();
				}
			}
			base.OnMouseDown (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			mouseCoords = new Point(e.X, e.Y);

			if(barDown) 
			{
				if(ClientRectangle.Contains(new Point(e.X, e.Y))) 
				{
					if(ValueFromPosition != this.value) 
					{
						this.value = ValueFromPosition;
						OnValueChanged(new EventArgs());
					}
					this.Invalidate();
				}
			}
			else if(trackDown) 
			{
			}

			base.OnMouseMove (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseCoords = new Point(e.X, e.Y);

			if(barDown) 
			{
				barDown = false;
			}
			else if(trackDown) 
			{
				trackDown = false;
			}
			timer.Stop();

			base.OnMouseUp (e);
		}

		public override string ToString()
		{
			return base.ToString() + ", Minimum: " + minimum + ", Maximum: " + maximum + ", Value: " + value;
		}

#if !CONFIG_COMPACT_FORMS
		protected override void WndProc(ref Message m)
		{
			// Nothing to do
		}
#endif
		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e.Graphics);
			base.OnPaint (e);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			this.Invalidate();
		}
	}

}
