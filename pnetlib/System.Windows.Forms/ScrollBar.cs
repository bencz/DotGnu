/*
 * ScrollBar.cs - Implementation of the "System.Windows.Forms.ScrollBar" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Free Software Foundation, Inc.
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
using System.Windows.Forms.Themes;

public abstract class ScrollBar : Control
{
	// Fields
	private int minimum = 0;
	private int maximum = 100;
	private int value = 0;
	private int largeChange = 10;
	private int smallChange = 1;
	private Rectangle bar = Rectangle.Empty;
	private Rectangle track = Rectangle.Empty;
	private Rectangle increment = Rectangle.Empty;
	private Rectangle decrement = Rectangle.Empty;
	private bool incDown = false;
	private bool decDown = false;
	private bool trackDown = false;
	private int barDown = -1;
	private int minBarDown;
	private int maxBarDown;
	private bool keyDown = false;
	private Timer timer;
	private int mouseX;
	private int mouseY;
	private Timer keyTimer;
	internal bool vertical;

	private const int repeatDelay = 50;
	private const int startDelay = 300;

	private Timer idleTimer;
	private MouseEventArgs idleMouse;

	private const int minThumbSize = 8;

	// Constructors
	public ScrollBar() : base()
	{
		base.TabStop = false;
		timer = new Timer();
		keyTimer = new Timer(); // keep key and mouse events from stepping on each others toes
		idleTimer = new Timer();
		idleTimer.Tick += new EventHandler(idleTimer_Tick);
		idleTimer.Interval = 1;
		BackColor = SystemColors.ScrollBar;
	}

	// Properties
	public override Color BackColor
	{
		get { return base.BackColor; }
		set
		{
			if (value == base.backColor) { return; }
			base.BackColor = value;
			Invalidate();
		}
	}

	public override Image BackgroundImage
	{
		get { return base.BackgroundImage; }
		set
		{
			if (value == base.BackgroundImage) { return; }
			base.BackgroundImage = value;
			Invalidate();
		}
	}

	protected override CreateParams CreateParams
	{
		get { return base.CreateParams; }
	}

	// Get or set whether the Decrement button is down, start the timer
	private bool DecrementDown
	{
		get { return decDown; }
		set
		{
			if (value == decDown) { return; }
			decDown = value;
			Invalidate(decrement);
			if (value)
			{
				timer.Tick += new EventHandler(Decrement);
				timer.Interval = startDelay;
				timer.Start();
			}
			else
			{
				timer.Stop();
				timer.Tick -= new EventHandler(Decrement);
			}
		}
	}

	protected override ImeMode DefaultImeMode
	{
		get { return ImeMode.Disable; }
	}

	public override Font Font
	{
		get { return base.Font; }
		set { base.Font = value; }
	}

	public override Color ForeColor
	{
		get { return base.ForeColor; }
		set
		{
			if (value == base.foreColor) { return; }
			base.ForeColor = value;
			Invalidate();
		}
	}

	public new ImeMode ImeMode
	{
		get { return base.ImeMode; }
		set { base.ImeMode = value; }
	}

	// Get or set whether the Increment button is down, start the timer
	private bool IncrementDown
	{
		get { return incDown; }
		set
		{
			if (value == incDown) { return; }
			incDown = value;
			Invalidate(increment);
			if (value)
			{
				timer.Tick += new EventHandler(Increment);
				timer.Interval = startDelay;
				timer.Start();
			}
			else
			{
				timer.Stop();
				timer.Tick -= new EventHandler(Increment);
			}
		}
	}

	// Value for large scroll jumps
	public int LargeChange
	{
		get { return largeChange; }
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			
			if(value == largeChange)
				return;
			
			largeChange = value;
			if (largeChange > maximum - minimum)
				largeChange = (maximum - minimum + 1);// /2
			
			LayoutScrollBar();
			Invalidate();
		}
	}

	// Maximum value of scroll
	public int Maximum
	{
		get { return maximum; }
		set
		{
			if (value < minimum)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			if (value == maximum) { return; }
			maximum = value;
			if (value > maximum)
				Value = maximum;
			LayoutScrollBar();
			Invalidate();
		}
	}

	// Minimum value of scroll
	public int Minimum
	{
		get { return minimum; }
		set
		{
			if (value > maximum)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			if (value == minimum) { return; }
			minimum = value;
			if (value < minimum)
				Value = minimum;
			LayoutScrollBar();
			Invalidate();
		}
	}

	public int SmallChange
	{
		get { return smallChange; }
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			smallChange = value;
		}
	}

	public override string Text
	{
		get { return base.Text; }
		set { base.Text = value; }
	}

	public int Value
	{
		get { return value; }
		set
		{
			if (value > maximum || value < minimum)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			if (value == this.value) { return; }

			Rectangle oldBounds = bar;
			this.value = value;
			LayoutScrollBar();
			RedrawTrackBar(oldBounds);
			OnValueChanged(new EventArgs());
		}
	}

	// Called when decrement button is pushed and called from timer
	private void Decrement(Object sender, EventArgs e)
	{
		timer.Interval = repeatDelay;

		int newValue = value - smallChange;
		if (newValue < minimum)
			newValue = minimum;

		Rectangle oldBounds = bar;
		GenerateManualScrollEvents( newValue, ScrollEventType.SmallDecrement);
		RedrawTrackBar(oldBounds);
	}

	// Called when the trackbar is clicked to decrement big and called from the timer
	private void DecrementBig(Object sender, EventArgs e)
	{
		timer.Interval = repeatDelay;

		int newValue = value - largeChange;
		if (newValue < minimum)
			newValue = minimum;

		if (trackDown)
		{
			if (vertical)
			{
				if (mouseY > bar.Y)
				{
					TrackPressed(false,false);
					return;
				}
			}
			else
			{
				if (mouseX > bar.X)
				{
					TrackPressed(false,false);
					return;
				}
			}
		}
		Rectangle oldBounds = bar;
		GenerateManualScrollEvents(newValue, ScrollEventType.LargeDecrement);
		RedrawTrackBar(oldBounds);
	}

	// Draw if visible and created
	private void Draw(Graphics g, Rectangle drawBounds)
	{
		if (!Visible || !IsHandleCreated) { return; }
		ThemeManager.MainPainter.DrawScrollBar(g,
											   ClientRectangle,
											   drawBounds,
											   ForeColor,BackColor,
											   vertical,Enabled,
											   bar, track,
											   decrement,decDown,
											   increment,incDown);
	}

	// Called when the increment button is pressed and called from the timer
	private void Increment(Object sender, EventArgs e)
	{
		timer.Interval = repeatDelay;

		int v = value + smallChange;
		int v1 = maximum - largeChange + 1;
		if (v1 < minimum)
			v1 = minimum;
		if (v > v1)
			v = v1;
		Rectangle oldBounds = bar;
		GenerateManualScrollEvents(v, ScrollEventType.SmallIncrement);
		RedrawTrackBar(oldBounds);
	}

	// Called when the trackbar is clicked to increment big and called from the timer
	private void IncrementBig(Object sender, EventArgs e)
	{
		timer.Interval = repeatDelay;

		int newValue = value + largeChange;
		int maxValue = maximum - largeChange + 1;
		if (maxValue < minimum)
			maxValue = minimum;
		if (newValue > maxValue)
			newValue = maxValue;

		if (trackDown)
		{
			if (vertical)
			{
				if (mouseY - bar.Height < bar.Y)
				{
					TrackPressed(false,false);
					return;
				}
			}
			else
			{
				if (mouseX - bar.Width < bar.X)
				{
					TrackPressed(false,false);
					return;
				}
			}
			
		}
		Rectangle oldBounds = bar;
		GenerateManualScrollEvents(newValue, ScrollEventType.LargeIncrement); 
		RedrawTrackBar(oldBounds);
	}

	// Sets up the layout rectangles for a HScrollBar's elements
	private void LayoutElementsH(Size s)
	{
		LayoutElementsV(new Size(s.Height, s.Width));
		decrement = SwapRectValues(decrement);
		increment = SwapRectValues(increment);
		track = SwapRectValues(track);
		bar = SwapRectValues(bar);
		if (RightToLeft == RightToLeft.Yes)
		{
			int offset = bar.X - track.X;
			int guiMax = track.Width - bar.Width*2/3;

			bar.X = guiMax - offset;
			
		}
	}

	// Sets up the layout rectangles for a VScrollBar's elements
	// Windows XP decrement and increment buttons width is just 2/3 of the initially used
	// thus we fix this with a 2/3 coefficient and other experimentally found coefficients 
	// -- the only reason of this hack is that it makes our bar look good
	private void LayoutElementsV(Size s)
	{
		int trackHeight, thumbHeight, thumbPos, zeroMax, zeroVal;
		double percentage;
		
		// Track
		trackHeight = s.Height - (s.Width * 4/3);
		if (trackHeight < 0)
			trackHeight = 0;
		
		// Decrement and increment buttons
		// Is there enough room to fit both buttons at their
		// preferred size?
		
		if(s.Height >= (s.Width * 5/3) + 2)   // 5/3 is just an experimental coefficient, no logical reason
		{
	
			
			track  = new Rectangle(0, s.Width * 2/3, s.Width, s.Height - (s.Width * 4/3));
		
			decrement = new Rectangle(0, 0, s.Width, s.Width * 2/3);
			increment = new Rectangle(0, s.Width * 2/3 + trackHeight, s.Width, (s.Width) * 2/3);
		}
		else
		{
			// No.  Split what's left.
		    	track  = new Rectangle(0, s.Height / 3 + 1, s.Width, s.Height / 3 - 1);

			decrement = new Rectangle(0, 0, s.Width, s.Height / 3 );
			increment = new Rectangle(0, s.Height * 2/3, s.Width, s.Height / 3);
		}
		
		// Thumb.
		zeroMax = maximum - minimum - largeChange + 1;
		if (zeroMax == 0)
		{
			bar = Rectangle.Empty;
			return;
		}
		zeroVal = value - minimum;
		percentage = (double) (largeChange) / (maximum - minimum + 1);
		thumbHeight = (int) (percentage * trackHeight);
		if (thumbHeight > trackHeight)
			thumbHeight = trackHeight;
		if (thumbHeight < minThumbSize)
			thumbHeight = minThumbSize - 2;
		percentage = (double) zeroVal / zeroMax;
		thumbPos = (int)(percentage * (trackHeight - thumbHeight - 3));
		if (thumbPos > (trackHeight - thumbHeight - 2))
		{       thumbPos = (trackHeight - thumbHeight - 2);
			
			
		}

		// Out of range
		if (thumbPos<=0)
			thumbPos = -1;
                
		// Out of range. 3/4 is just an experimental coefficient that makes our bar look good		
		if(s.Height  >= s.Width * 5/3 + 2)  // 5/3 is just an experimental coefficient, no logical reason		
	     		bar = new Rectangle(0, s.Width * 3/4 + thumbPos, s.Width, thumbHeight + 1); 
		else  bar = new Rectangle(0, 0, 0, 0);
	
	}

	protected override void OnEnabledChanged(EventArgs e)
	{
		Invalidate();
		base.OnEnabledChanged(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (keyDown) { return; }

		switch (e.KeyCode)
		{
			case Keys.PageUp:
			{
				DecrementBig(null,null);
				ScrollKeyPressed(true,-2);
			}
			break;

			case Keys.PageDown:
			{
				IncrementBig(null,null);
				ScrollKeyPressed(true,2);
			}
			break;

			case Keys.Home:
			{
				Value = 0;
			}
			break;

			case Keys.End:
			{
				Value = maximum - largeChange + 1;
			}
			break;

			case Keys.Up:
			case Keys.Left:
			{
				Decrement(null,null);
				ScrollKeyPressed(true,-1);
			}
			break;

			case Keys.Down:
			case Keys.Right:
			{
				Increment(null,null);
				ScrollKeyPressed(true,1);
			}
			break;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (!keyDown) { return; }

		barDown = -1;

		switch (e.KeyCode)
		{
			case Keys.PageUp:
			case Keys.Home:
			{
				ScrollKeyPressed(false,2);
			}
			break;

			case Keys.PageDown:
			case Keys.End:
			{
				ScrollKeyPressed(false,-2);
			}
			break;

			case Keys.Up:
			case Keys.Left:
			{
				ScrollKeyPressed(false,1);
			}
			break;

			case Keys.Down:
			case Keys.Right:
			{
				ScrollKeyPressed(false,-1);
			}
			break;
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
			return;
		Capture = true;
		int x = e.X;
		int y = e.Y;

		mouseX = x;
		mouseY = y;

		if (increment.Contains(x,y))
		{
			Increment(null,null);
			IncrementDown = true;
		}
		else if (decrement.Contains(x,y))
		{
			Decrement(null,null);
			DecrementDown = true;
		}
		else if (bar.Contains(x,y))
		{
			// Set the position of the mouse as it starts dragging the bar
			// Calculate the min and max allowable positions
			// This is MS behavior
			if (vertical)
			{
				barDown = y;
				minBarDown = y - bar.Y + track.Y;
				maxBarDown = track.Bottom - (bar.Bottom - y);
			}
			else
			{
				barDown = x;
				minBarDown = x - bar.X + track.X;
				maxBarDown = track.Right - (bar.Right - x);
			}
			ScrollEventArgs se = new ScrollEventArgs(ScrollEventType.ThumbTrack, value);
			OnScroll(se);
			if (value != se.NewValue)
				Value = se.NewValue;
		}
		else if (track.Contains(x,y))
		{
			if (vertical)
			{
				if (y >= bar.Bottom)
				{
					IncrementBig(null,null);
					TrackPressed(true,true);
				}
				else // y <= bar.Top
				{
					DecrementBig(null,null);
					TrackPressed(true,false);
				}
			}
			else
			{
				bool plus = (x >= bar.Right);
				plus ^= (RightToLeft == RightToLeft.Yes);
				if (plus)
				{
					IncrementBig(null,null);
					TrackPressed(true,true);
				}
				else
				{
					DecrementBig(null,null);
					TrackPressed(true,false);
				}
			}
		}
		base.OnMouseDown(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (incDown)
		{
			IncrementDown = false;
			Invalidate(increment);
		}
		else if (decDown)
		{
			DecrementDown = false;
			Invalidate(decrement);
		}
		else if (trackDown)
		{
			TrackPressed(false,false);
		}

		base.OnMouseLeave(e);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter (e);
	}


	protected override void OnMouseMove(MouseEventArgs e)
	{
		idleMouse = e;
		// Do the actual event when the events have caught up.
		idleTimer.Start();
	}

	private void OnMouseMoveActual(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		mouseX = e.X;
		mouseY = e.Y;

		if (incDown)
		{
			IncrementDown = increment.Contains(e.X, e.Y);
		}
		else if (decDown)
		{
			DecrementDown = decrement.Contains(e.X, e.Y);
		}
		else if (barDown != -1)
		{
			Rectangle oldBounds = bar;
			if (vertical)
			{
				int guiMax = track.Y + track.Height - bar.Height;
				int newPos = bar.Y + e.Y - barDown;
				if (newPos < track.Y)
				{
					newPos = track.Y;
					barDown = minBarDown;
				}
				else if (newPos > guiMax)
				{
					newPos = guiMax;
					barDown = maxBarDown;
				}
				else
					barDown = e.Y;
				if (bar.Y == newPos)
					return;
				bar.Y = newPos;
			}
			else
			{
				int guiMax = track.X + track.Width - bar.Width;
				int newPos = bar.X + e.X - barDown;
				if (newPos < track.X)
				{
					newPos = track.X;
					barDown = minBarDown;
				}
				else if (newPos > guiMax)
				{
					newPos = guiMax;
					barDown = maxBarDown;
				}
				else
					barDown = e.X;
				if (bar.X == newPos)
					return;
				bar.X = newPos;
			}
			// Only generate the events if the bar has moved signficantly enough to change value
			int newValue = ValueFromPosition;
			if (newValue != value)
			{
				// Generate the events
				ScrollEventArgs se = new ScrollEventArgs(ScrollEventType.ThumbTrack, newValue);
				OnScroll(se);
				value = se.NewValue;
				// Did the event alter the value?
				if (newValue != se.NewValue)
					LayoutScrollBar();
				OnValueChanged(new EventArgs());
			}

			RedrawTrackBar( oldBounds);
		}
	}

	// When the trackbar moves, to prevent flickering, we only redraw what we have to
	private void RedrawTrackBar( Rectangle oldBounds)
	{
		Region region = new Region(oldBounds);
		region.Union(bar);
		Invalidate(region);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		Capture = false;
		mouseX = e.X;
		mouseY = e.Y;
		ScrollEventArgs se;

		if (incDown)
		{
			IncrementDown = false;
		}
		else if (decDown)
		{
			DecrementDown = false;
		}
		else if (trackDown)
		{
			TrackPressed(false,false);
		}
		else if (barDown != -1)
		{
			barDown = -1;
			se = new ScrollEventArgs(ScrollEventType.ThumbPosition, value);
			OnScroll(se);
			Value = se.NewValue;
		}
		se = new ScrollEventArgs(ScrollEventType.EndScroll, value);
		OnScroll(se);
		Value = se.NewValue;
		base.OnMouseUp(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Draw(e.Graphics, e.ClipRectangle);
		base.OnPaint(e);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		bool modified = (x != Left || y != Top || width != Width || height != Height);
		base.SetBoundsCore (x, y, width, height, specified);
		if (modified && IsHandleCreated)
		{
			LayoutScrollBar();
			Invalidate();
		}
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl ();
		LayoutScrollBar();
	}


	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged (e);
		if (IsHandleCreated)
			LayoutScrollBar();
	}


	// Generate the events when the scrollbar is incremented or decremented
	private void GenerateManualScrollEvents(int newValue, ScrollEventType type)
	{
		ScrollEventArgs se = new ScrollEventArgs(type, newValue);
		OnScroll(se);
		// This should be checked. If the value is set back to the previous value, I assume
		// this prevents an OnValueChanged event.
		if (se.NewValue != value)
		{
			value = se.NewValue;
			LayoutScrollBar();
			OnValueChanged(new EventArgs());
		}
	}

	protected virtual void OnScroll(ScrollEventArgs e)
	{
		ScrollEventHandler handler;
		handler = (ScrollEventHandler)(GetHandler(EventId.Scroll));
		if (handler != null)
		{
			handler(this,e);
		}
	}

	protected virtual void OnValueChanged(EventArgs e)
	{
		EventHandler handler;
		handler = (EventHandler)(GetHandler(EventId.ValueChanged));
		if (handler != null)
		{
			handler(this,e);
		}
	}

	private void ScrollKeyPressed(bool pressed, int amount)
	{
		if (pressed == keyDown) { return; }
		keyDown = pressed;
		if (pressed)
		{
			switch (amount)
			{
				case 2:
				{
					timer.Tick += new EventHandler(IncrementBig);
				}
				break;

				case -2:
				{
					timer.Tick += new EventHandler(DecrementBig);
				}
				break;

				case 1:
				{
					timer.Tick += new EventHandler(Increment);
				}
				break;

				case -1:
				{
					timer.Tick += new EventHandler(Decrement);
				}
				break;
			}
			timer.Interval = startDelay;
			timer.Start();
		}
		else
		{
			timer.Stop();
			timer.Tick -= new EventHandler(IncrementBig);
			timer.Tick -= new EventHandler(DecrementBig);
			timer.Tick -= new EventHandler(Increment);
			timer.Tick -= new EventHandler(Decrement);
		}
	}

	// Set the position based on "value" & layout the elements
	private void LayoutScrollBar()
	{
		if(vertical)
			LayoutElementsV(ClientSize);
		else
			LayoutElementsH(ClientSize);
	}

	// Calculate "value" from the bar position
	private int ValueFromPosition
	{
		get
		{
			int v;
			if(vertical)
			{
				int position = bar.Y - track.Y;
				double percentage = (double) position / (track.Height - bar.Height);
				v = (int)(percentage * (maximum - minimum + 1 - largeChange));
			}
			else
			{
				int position = bar.X - track.X;
				double percentage = (double) position / (track.Width - bar.Width);
				v = (int)(percentage * (maximum - minimum + 1 - largeChange));
			
				if(RightToLeft == RightToLeft.Yes)
				{
					int guiMax = (maximum - largeChange - minimum);
					v = guiMax - v;
				}
			}
			v += minimum;
			if (v < minimum)
				v = minimum;
			if (v > maximum)
				v = maximum;
			return v;
		}
	}

	private static Rectangle SwapRectValues(Rectangle rect)
	{
		return new Rectangle(rect.Y,rect.X,rect.Height,rect.Width);
	}

	public override string ToString()
	{
		return base.ToString() + ", Minimum: " + minimum + ", Maximum: " +
			maximum + ", Value: " + value;
	}

	private void TrackPressed(bool pressed, bool plus)
	{
		if (pressed == trackDown) { return; }
		trackDown = pressed;
		if (pressed)
		{
			if (plus)
			{
				timer.Tick += new EventHandler(IncrementBig);
			}
			else
			{
				timer.Tick += new EventHandler(DecrementBig);
			}
			timer.Interval = startDelay;
			timer.Start();
		}
		else
		{
			timer.Stop();
			timer.Tick -= new EventHandler(IncrementBig);
			timer.Tick -= new EventHandler(DecrementBig);
		}
	}

#if !CONFIG_COMPACT_FORMS
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}
#endif // !CONFIG_COMPACT_FORMS

	public event ScrollEventHandler Scroll
	{
		add { AddHandler(EventId.Scroll,value); }
		remove { RemoveHandler(EventId.Scroll,value); }
	}

	public event EventHandler ValueChanged
	{
		add { AddHandler(EventId.ValueChanged,value); }
		remove { RemoveHandler(EventId.ValueChanged,value); }
	}

	private void idleTimer_Tick(object sender, EventArgs e)
	{
		idleTimer.Stop();
		OnMouseMoveActual(idleMouse);
	}
}; // class ScrollBar

}; // namespace System.Windows.Forms
