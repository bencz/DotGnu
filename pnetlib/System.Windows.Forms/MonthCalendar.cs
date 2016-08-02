/*
 * MonthCalendar.cs - Implementation of "System.Windows.Forms.MonthCalendar" 
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
using System.Collections;
using System.Globalization;
using System.Drawing.Toolkit;
using System.Windows.Forms.Themes;
namespace System.Windows.Forms
{

[TODO]
public class MonthCalendar : Control
{
	private Size calendarDims=new Size(1,1);
	
	private DateTime selectionStart;
	private DateTime selectionEnd;

	private bool showWeekNumbers=false;

	private StringFormat dayFormat;
	
	private Size cellSize = new Size(0,0);
	private Size numSize;
	private Size totalSize;
	private Size monthSize;
	private Size textSize;

	private SingleMonthCalendar[] cals=null;

	private int selectedIndex = 0;
	
	private DateRangeEventHandler changedEvent; 
	
	private DateTime[] boldedDates;
	private DateTime[] monthlyBoldedDates;
	private DateTime[] annuallyBoldedDates;

	public MonthCalendar()
	{
		selectionStart = DateTime.Today;
		selectionEnd = selectionStart;
		base.BorderStyleInternal = BorderStyle.FixedSingle;
		base.SetStyle(ControlStyles.ContainerControl, true);
		BackColor = Color.White;
		AddCalendars();
	}

	private void AddCalendars()
	{
		this.SuspendLayout();
		Controls.Clear();

		cals=new SingleMonthCalendar[calendarDims.Width*calendarDims.Height];

		for(int i=0;i<calendarDims.Height;i++)
		{
			for(int j=0;j<calendarDims.Width;j++)
			{
				SingleMonthCalendar cal = new SingleMonthCalendar(this,	i*calendarDims.Width+j);
				cal.Size = monthSize;
				cal.Location = ClientOrigin + new Size(monthSize.Width * j, monthSize.Height * i);
				cals[i*calendarDims.Width+j] = cal;
				Controls.Add(cal);
			}
		}

		this.ResumeLayout();
	}

	[TODO]
	public void SetCalendarDimensions(int x, int y)
	{
		// Fix: try to maintain x:y ratio
		if(x*y > 12)
		{
			x=3;
			y=4;
		}
		calendarDims=new Size(x,y);
		Size = DefaultSize;
		AddCalendars();
	}
	
	public void SetDate(DateTime date)
	{
		SetSelectionInternal(date, date, -1, false);
	}

	[TODO]
	internal void SetSelectionInternal(DateTime start, DateTime end, int index, bool all)
	{	
		bool changed = (selectionStart != start || selectionEnd != end);

		selectionStart=start;
		selectionEnd=end;

		// Fix: implement proper invalidate for modified cals only
		if(changed && (all || index==-1))
		{
			foreach(Control c in Controls)
			{
				c.Invalidate();
			}	
		}
		else if(index!=-1)
		{
			for(int i=0;i<cals.Length;i++)
			{
				if(i==selectedIndex || i==index || cals[i].HasSelectedItem)
				{
					cals[i].Invalidate(cals[index].DayRectangle);
				}
			}
		}

		if(index != -1)
		{
			selectedIndex = index;
		}
		
		if(changed)
		{
			OnDateChanged(new DateRangeEventArgs(start, end));
		}
	}

	public void SetSelectionRange(DateTime start, DateTime end)
	{
		SetSelectionInternal(start, end, -1, true);
	}

	protected virtual void OnDateChanged(DateRangeEventArgs e)
	{
		DateRangeEventHandler handler;
		handler = (DateRangeEventHandler)(GetHandler(EventId.DateChanged));
		
		if(handler != null)
		{
			handler(this, e);
		}
	}

	public event DateRangeEventHandler DateChanged
	{
		add 
		{
			AddHandler(EventId.DateChanged, value);
		}
		remove 
		{
			RemoveHandler(EventId.DateChanged, value); 
		}
	}
	

	public DateTime SelectionStart
	{
		get
		{
			return this.selectionStart;
		}
		set
		{
			this.selectionStart = value;
		}
	}

	public DateTime SelectionEnd
	{
		get
		{
			return this.selectionEnd;
		}
		set
		{
			this.selectionEnd = value;
		}
	}

	public bool ShowWeekNumbers
	{
		get
		{
			return showWeekNumbers;
		}
		set
		{
			showWeekNumbers = value;
		}
	}

	public DateTime[] BoldedDates
	{
		get
		{
			return boldedDates;
		}
		set
		{
			boldedDates = value;
		}
	}

	public DateTime[] MonthlyBoldedDates
	{
		get
		{
			return monthlyBoldedDates;
		}
		set
		{
			monthlyBoldedDates = value;
		}
	}
	
	public DateTime[] AnnuallyBoldedDates
	{
		get
		{
			return annuallyBoldedDates;
		}
		set
		{
			annuallyBoldedDates = value;
		}
	}

	protected override Size DefaultSize
	{
		get
		{
			return ClientToBounds(UpdateSize());
		}
	}
	
	private Size UpdateSize()
	{
		Font font = this.Font;
		int xdiv = 7;
		
		if(showWeekNumbers)
		{
			xdiv = 8; 
		}

		using(Graphics g = MonthCalendar.DefaultGraphics)
		{
			int textWidth = g.MeasureCharacters("w", Font, new Rectangle(0,0, 100, 100), new StringFormat())[0].Width;
			int textHeight = font.Height;

			textSize = new Size(textWidth, textHeight);	

			numSize = g.MeasureCharacters("9", Font, new Rectangle(0,0, 100, 100), new StringFormat())[0].Size;
		}

		cellSize = new Size(textSize.Width*3 + 1 , textSize.Height+2);

		monthSize = new Size(cellSize.Width*xdiv, cellSize.Height*10);	

		totalSize = new Size((cellSize.Width*xdiv*calendarDims.Width)+1, (cellSize.Height*10*calendarDims.Height)+1);	

		return totalSize;
	}
	
	private static Graphics DefaultGraphics
	{
		get
		{
			IToolkitGraphics toolkitGraphics = ToolkitManager.Toolkit.GetDefaultGraphics();
			return ToolkitManager.CreateGraphics(toolkitGraphics,Rectangle.Empty);
		}
	}


	[Flags]
	private enum CalendarFlags
	{
		None = 0x00,
		PrevButton = 0x01,
		NextButton = 0x02,
		ShowLastMonth = 0x04,
		ShowNextMonth = 0x08
	}


	// This should be the drawing peer of the MonthCalendar in fact
	private class SingleMonthCalendar : Control
	{
		MonthCalendar parent=null;
		private int index=0;
		private DateTime firstDateCell;
		private BitArray posHasDate;

		private int monthStartPos;
		private int monthEndPos;

		private bool hasSelectedItem = false; 
		
		/* all the drawing stuff to track here */
		private Rectangle titleRect;
		private Rectangle nextBtnRect;
		private Rectangle prevBtnRect;
		private Rectangle monthRect;
		private Rectangle yearRect;
		private Rectangle weekRect;
		private Rectangle dayRect;
		private Rectangle weeknumRect;
		private Rectangle todayRect;

		public SingleMonthCalendar(MonthCalendar parent, int index)
		{
			base.SetStyle(ControlStyles.UserPaint, true);
			this.parent = parent;
			this.index = index;
			this.posHasDate = new BitArray(42, false);
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			bool prev = (index==0);
			Size parentDims = parent.calendarDims;

			bool next = (index==parentDims.Width-1);
			DateTime d=parent.selectionStart;
			// Note: this is to prevent 31 Dec + 2 being 31 Feb :)
			d = new DateTime(d.Year, d.Month, 1);
			d = d.AddMonths(index-parent.selectedIndex);
			CalendarFlags flags = CalendarFlags.None;

			flags |= (index == 0) ? 
						(CalendarFlags.PrevButton | CalendarFlags.ShowLastMonth)
						: CalendarFlags.None ;
						
			flags |= (index == parentDims.Width-1) ? 
							CalendarFlags.NextButton : CalendarFlags.None;

							
			flags |= (index == parentDims.Width*parentDims.Height-1) ?  CalendarFlags.ShowNextMonth : CalendarFlags.None;

			DrawMonth(e.Graphics, d, flags);
		}

		private bool IsBolded(DateTime current)
		{
			if(parent.BoldedDates!=null)
			{
				foreach(DateTime d in parent.BoldedDates)
				{
					if(current == d) 
					{
						return true;
					}
				}
			}

			if(parent.MonthlyBoldedDates != null)
			{
				foreach(DateTime d in parent.MonthlyBoldedDates)
				{
					if(current.Day == d.Day) 
					{
						return true;
					}
				}
			}

			if(parent.MonthlyBoldedDates != null)
			{
				foreach(DateTime d in parent.MonthlyBoldedDates)
				{
					if(current.Day == d.Day && current.Month == d.Month) 
					{
						return true;
					}
				}
			}

			return false;
		}

		[TODO]
		// Fix: move this into theming API
		private void DrawMonth(Graphics g, DateTime monthDate, CalendarFlags flags)
		{
			UpdateRects(new Point(0,0), monthDate);
			DateTime today=DateTime.Today;
			Size cellSize=parent.cellSize;
			Size numSize=parent.numSize;
			Size textSize=parent.textSize;
			
			Brush enabledBrush, disabledBrush, lightBrush;
	
			enabledBrush = new SolidBrush(SystemColors.ControlText);
			disabledBrush = new SolidBrush(SystemColors.GrayText);
			lightBrush = new SolidBrush(SystemColors.ControlLight);
			
			DateTime startDate = new DateTime(monthDate.Year, monthDate.Month, 1);
			hasSelectedItem = false;
			monthStartPos = -1;
			monthEndPos = -1;
	
			// NOTE: why would someone have two different ways of DayOfWeek
			// and Day enums. So for today the weeks starts with sunday
	
			while(startDate.DayOfWeek != DayOfWeek.Sunday)
			{
				startDate = startDate.AddDays(-1);
			}
	
			firstDateCell = startDate;
							
			g.FillRectangle(lightBrush,titleRect);
	
			String monthString = monthDate.ToString("MMMM");
			String yearString = monthDate.ToString("yyyy");
				
			Font boldFont = new Font(Font, FontStyle.Bold);
				
			g.DrawString(monthString,
				boldFont,
				enabledBrush,
				monthRect.Left,
				monthRect.Top,	
				null);
					
			g.DrawString(yearString,
				boldFont,
				enabledBrush,
				yearRect.Left,
				yearRect.Top,	
				null);

			DateTime weekStart=firstDateCell;
			for(int i=0;i<7;i++)
			{
				int offset=(cellSize.Width-textSize.Width*3)/2;
				g.DrawString(weekStart.ToString("ddd"),
					Font,
					disabledBrush,
					titleRect.Left + cellSize.Width * i + offset,
					titleRect.Bottom,
					null);

				weekStart = weekStart.AddDays(1);
			}
			for(int i=0;i<6*7;i++)
			{
				int day=startDate.Day;
				/* Note: the difference between years cannot be more
					 * than 1 as a MAXIMUM of 7 days will be the difference
					 * between startDate and endDate */
				int monthRelation = (startDate.Year != monthDate.Year) ?
					startDate.Year - monthDate.Year :
					startDate.Month - monthDate.Month;
					
				Brush b = enabledBrush;
					
				if(monthRelation!=0) b = disabledBrush;

				if(monthRelation==0 && monthStartPos==-1) 
				{
					monthStartPos = i;
				}

				if(monthRelation==1 && monthEndPos==-1)
				{
					monthEndPos = i;
				}
	
				int offset= (cellSize.Width - (day > 9 ? 2 : 1) * numSize.Width)/ 2;
	
				Rectangle rect=new Rectangle(dayRect.Left+cellSize.Width * (i%7),
					dayRect.Top+ (i/7) * cellSize.Height,
					cellSize.Width,
					cellSize.Height);
	
				if(monthRelation==0 && startDate >= parent.selectionStart 
					&& startDate <= parent.selectionEnd)
				{
					hasSelectedItem = true;
					g.FillRectangle(lightBrush, rect);
				}
				if(monthRelation==0 && startDate == today)
				{
					using(Pen redPen = new Pen(Color.Red, 1.0f))
					{
						rect.Size+=new Size(-1,0);
						g.DrawRectangle(redPen, rect);
					}
				}
				if(monthRelation==0 
					||	((monthRelation < 0) && ((flags & CalendarFlags.ShowLastMonth) != 0)) 
					||  ((monthRelation > 0) &&	((flags & CalendarFlags.ShowNextMonth) != 0)))
				{
					Font font = IsBolded(startDate) ? boldFont : Font ;

					g.DrawString(day.ToString(),
						font,
						b,
						rect.Left + offset,
						rect.Top,	
						null);
					posHasDate[i]=true;
				}
				else
				{
					posHasDate[i]=false;
				}
				startDate=startDate.AddDays(1);
				
				boldFont.Dispose();

				if((flags & CalendarFlags.PrevButton) != 0)
				{
					ControlPaint.DrawScrollButton(g, 
						prevBtnRect, ScrollButton.Left, 
						ButtonState.Flat);
				}
	
				if((flags & CalendarFlags.NextButton) != 0)
				{
					ControlPaint.DrawScrollButton(g, 
						nextBtnRect, ScrollButton.Right, 
						ButtonState.Flat);
				}
			}
	
			lightBrush.Dispose();
			enabledBrush.Dispose();
			disabledBrush.Dispose();
		}

		private void UpdateRects(Point origin, DateTime monthDate)
		{
			int xdiv = 7;
	
			Size cellSize = parent.cellSize;
			Size numSize = parent.numSize;
			Size textSize = parent.textSize;

			if(parent.showWeekNumbers)
			{
				xdiv = 8; 
			}
			titleRect.Location = origin;
			titleRect.Size=new Size(cellSize.Width*xdiv-2, cellSize.Height*2);
	
			prevBtnRect= new Rectangle(titleRect.Left,
									titleRect.Top+cellSize.Height/2,
									cellSize.Width,
									cellSize.Height);
			
			nextBtnRect= new Rectangle(titleRect.Right-cellSize.Width,
									titleRect.Top+cellSize.Height/2,
									cellSize.Width,
									cellSize.Height);
			using(Graphics g = MonthCalendar.DefaultGraphics)
			{	
				Font boldFont = new Font(Font, FontStyle.Bold);
				String monthString = monthDate.ToString("MMMM"); 
				String yearString = monthDate.ToString("yyyy");
				
				int yearWidth = Size.Round(g.MeasureString(yearString, boldFont)).Width;
				int monthWidth = Size.Round(g.MeasureString(monthString, boldFont)).Width;
				int totalWidth = yearWidth + monthWidth + textSize.Width;

				int offset = (titleRect.Width - totalWidth)/2;
			
				monthRect = new Rectangle(titleRect.Left + offset,
						   				titleRect.Top + cellSize.Height/2 ,
										monthWidth,
										cellSize.Height);

				yearRect = new Rectangle(monthRect.Right + textSize.Width,
										titleRect.Top + cellSize.Height/2,
										yearWidth,
										cellSize.Height);
	
				dayRect = new Rectangle(titleRect.Left + cellSize.Width * (xdiv - 7),
									titleRect.Bottom + cellSize.Height,
									cellSize.Width * 7,
									cellSize.Height * 6);
				boldFont.Dispose();
			}
		}

		private static DateTime GetNewMonthView(DateTime d, int increment)
		{
			DateTime result = new DateTime(d.Year, d.Month, 1);
			result = result.AddMonths(increment);
			int maxDay = DateTime.DaysInMonth(result.Year, result.Month);
			if(d.Day > maxDay)
			{
				result = new DateTime(result.Year, result.Month,  maxDay);
				
			}	
			else
			{
				result = new DateTime(result.Year, result.Month, d.Day);
			}

			return result;
		}

		[TODO]
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Point p = new Point(e.X, e.Y);
			Size cellSize = parent.cellSize;
			Size parentDims = parent.calendarDims;
			int nextPage = parentDims.Width*parentDims.Height;

			// Fix: Handle long clicks with a timer like ScrollBar does	
			if(prevBtnRect.Contains(p))
			{
				parent.SetDate(GetNewMonthView(parent.selectionStart,-nextPage));
			}
			if(nextBtnRect.Contains(p))
			{
				parent.SetDate(GetNewMonthView(parent.selectionStart,nextPage));
			}
			if(dayRect.Contains(p))
			{
				int count= ((p.X - dayRect.Left) / cellSize.Width + 
								(p.Y - dayRect.Top) / cellSize.Height * 7 );
				if(posHasDate[count])
				{
					// Note: monthEndPos denotes the position where
					// the next month starts
					if(count >= monthStartPos && count < monthEndPos)
					{
						DateTime next = firstDateCell.AddDays(count);
						parent.SetSelectionInternal(next, next, this.index, false);
					}
					else
					{
						// refresh all dates
						DateTime next = firstDateCell.AddDays(count);
						parent.SetSelectionInternal(next, next, this.index, true);
					}
				}
			}
		}
	
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		public bool HasSelectedItem
		{
				get
				{
						return hasSelectedItem;
				}
		}

		public Rectangle DayRectangle
		{
				get
				{
						return dayRect;
				}
		}

	}// Nested class

} // Class
} // Namespace
