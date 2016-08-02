/*
 * ProgressBar.cs - Implementation of "System.Windows.Forms.ProgressBar" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V <gopalv82@symonds.net> 
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
using System.Windows.Forms.Themes;

namespace System.Windows.Forms
{
	public sealed class ProgressBar: Control
	{
		
		private int min=0,max=100,value=0;
		private int step=10;
		private int range=100;

		public ProgressBar() : base("ProgressBar")
			{
				SetStyle(ControlStyles.Selectable, false);
			}

		/* NOTE: not really sure how the stuff is drawn , but
		*  this sure looks like it :-)
		*/
		private void Draw(Graphics graphics)
			{
				if(!Visible || !IsHandleCreated) return;

				Size clientSize = ClientSize;
				int x = 0;
				int y = 0;
				int width=clientSize.Width;
				int height=clientSize.Height;
				int steps=range/step;

				using(Brush brush=CreateBackgroundBrush())
				{
					graphics.FillRectangle(brush,x,y,width,height);
				}
				
				ThemeManager.MainPainter.DrawProgressBar(graphics,
				                                         x, y,
				                                         clientSize.Width,
				                                         clientSize.Height, 
				                                         steps, step,
				                                         value, this.Enabled);
			}

		protected override void OnPaint(PaintEventArgs args)
			{
				Draw(args.Graphics);
				base.OnPaint(args);
			}

		public void Increment(int value)
			{
				Redraw(value);
				Value=this.value+value;
			}

		// Redraw the area of the progress bar that has changed. "value" is the new value.
		// This needs to be changed if the drawing code is changed.
		private void Redraw(int value)
			{
				// Starting position, width and height.
				int width = ClientSize.Width - 4;
				int height = ClientSize.Height - 4;
				int y = 2;
				int x = 2;
				int steps = range/step;

				int xSpacing=2;
				int ySpacing=2;
				width -= (steps-1) * xSpacing;
				int blockWidth = width / steps;
				int blockHeight = height - ySpacing - 1;
				
				// Get the x positions of the two values.
				int x1 = x;
				int v = (int)Math.Ceiling((double)value/step);
				x += (blockWidth+xSpacing) * v;
				v = (int)Math.Ceiling((double)this.value/step);
				x1 += (blockWidth+xSpacing) * v;

				// Invalidate the area.
				if (x < x1)
				{
					Invalidate(new Rectangle(x, y, x1-x, blockHeight));
				}
				else if (x > x1)
				{
					Invalidate(new Rectangle(x1, y, x-x1, blockHeight));
				}
			}

		public void PerformStep()
			{
				value=(value + (step - (value % step)));
				Invalidate();
			}

		protected override Size DefaultSize 
			{
 				get
				{
					return new Size(138,20);
				}
 			}	

		public int Maximum 
			{
 				get
				{
					return max;
				}

 				set
				{
					if(value < min)
					{
						throw new ArgumentOutOfRangeException("Maximum");
					}
					max=value;
					range=max-min;
					Invalidate();
				}
	 		}

		public int Minimum 
			{
 				get
				{
					return min;
				}
	
 				set
				{
					if(value > max)
					{
						throw new ArgumentOutOfRangeException("Minimum");
					}
					min=value;
					range=max-min;
					Invalidate();
				}

 			}

		public int Step 
			{
	 			get
				{
					return step;
				}

	 			set
				{
					if(step <=0)
					{
						throw new ArgumentOutOfRangeException("Step");
					}
					step=value;
					Invalidate();
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
					if(value > max || value < min)
					{
						throw new ArgumentOutOfRangeException("Value");
					}
					Redraw(value);
					this.value=value;
				}

	 		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
			{
				base.SetBoundsCore (x, y, width, height, specified);
				Invalidate();
			}

		public override String ToString()
			{
				return base.ToString() + ", Minimum: " + min.ToString() + ", Maximum: " +
								 max.ToString() + ", Value: " + value.ToString();
			}
	}
}//namespace
