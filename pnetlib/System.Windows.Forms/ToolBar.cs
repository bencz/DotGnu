/*
 * ToolBar.cs - Implementation of the
 *			"System.Windows.Forms.ToolBar" class.
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

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms.Themes;

public class ToolBar : Control
{
	// Variables
	private ToolBarAppearance appearance = ToolBarAppearance.Normal;
	private bool autoSize = true;
	private BorderStyle borderStyle = BorderStyle.None;
	internal ToolBarButtonCollection buttons;
	private Size buttonSize = Size.Empty;
	private Size cachedAutoButtonSize = Size.Empty;
	private Size establishedTextBounds = Size.Empty;
	private bool divider = true;
	private bool dropDownArrows = true;
	private ImageList imageList = null;
	private bool showToolTips = false;
	private ToolBarTextAlign textAlign = ToolBarTextAlign.Underneath;
	private bool wrappable = true;
	private Size preferredSize = Size.Empty;
	private Size staticSize;
	private int[] wrapData = new int[1] { 1 };

	private int mouseDownClick = -1;
	private int mouseDownDrop = -1;
	private int mouseHoverClick = -1;
	private int mouseHoverDrop = -1;
	private int limitWidth = -1;
	private int limitHeight = -1;

	// used in calculating button and toolbar sizes
	private static readonly int separatorSize = 6; // just a guess
	private static readonly int dropDownWidth = 20; // another guess
	private static readonly int dividerHeight = 2; // yet another guess



	// Constructor
	public ToolBar() : base()
	{
		base.Dock = DockStyle.Top;
		base.TabStop = false;
		buttons = new ToolBarButtonCollection(this);
		staticSize = DefaultSize;
	}



	// Properties
#if !CONFIG_COMPACT_FORMS
	public ToolBarAppearance Appearance
	{
		get { return appearance; }
		set
		{
			if (value == appearance) { return; }
			appearance = value;
			// if buttons aren't auto-sized, keep current size info
			if (buttonSize.IsEmpty)
			{
				cachedAutoButtonSize = Size.Empty;
				preferredSize = Size.Empty;
				AdjustSize();
			}
			if (appearance == ToolBarAppearance.Normal)
			{
				mouseHoverClick = -1;
				mouseHoverDrop = -1;
			}
			Redraw();
		}
	}
	public bool AutoSize
	{
		get { return autoSize; }
		set
		{
			if (value == autoSize) { return; }
			autoSize = value;
			if (autoSize)
			{
				AdjustSize();
				Redraw();
			}
			else
			{
				staticSize = Size;
			}
		}
	}
	public override Color BackColor
	{
		get { return base.BackColor; }
		set
		{
			if (value == base.BackColor) { return; }
			base.BackColor = value;
			Redraw();
		}
	}
	public override Image BackgroundImage
	{
		get { return base.BackgroundImage; }
		set
		{
			if (value == base.BackgroundImage) { return; }
			base.BackgroundImage = value;
			Redraw();
		}
	}
	public BorderStyle BorderStyle
	{
		get { return borderStyle; }
		set
		{
			if (value == borderStyle) { return; }
			borderStyle = value;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	}
#endif
	public ToolBarButtonCollection Buttons
	{
		get { return buttons; }
	}
#if !CONFIG_COMPACT_FORMS
	public
#else
	internal
#endif
	Size ButtonSize
	{
		get
		{
		#if !CONFIG_COMPACT_FORMS
			if (buttonSize.IsEmpty)
			{
		#endif
				if (IsHandleCreated && buttons.Count > 0 && Visible)
				{
					if (cachedAutoButtonSize.IsEmpty)
					{
						using (Graphics g = CreateGraphics())
						{
							RecacheAutoButtonSize(g);
						}
					}
					return cachedAutoButtonSize;
				}
				else
				{
					if (TextAlign == ToolBarTextAlign.Right)
					{
						return new Size(39,36);
					}
					else
					{
						return new Size(24,22);
					}
				}
		#if !CONFIG_COMPACT_FORMS
			}
			else
			{
				return buttonSize;
			}
		#endif
		}
	#if !CONFIG_COMPACT_FORMS
		set
		{
			if (value.Width < 0 || value.Height < 0)
			{
				throw new ArgumentException(/* TODO */);
			}
			if (value == buttonSize) { return; }
			buttonSize = value;
			cachedAutoButtonSize = Size.Empty;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	#endif
	}
	protected override CreateParams CreateParams
	{
		get { return base.CreateParams; }
	}
	protected override ImeMode DefaultImeMode
	{
		get { return ImeMode.Disable; }
	}
	protected override Size DefaultSize
	{
		get { return new Size(100,22); }
	}
#if !CONFIG_COMPACT_FORMS
	public bool Divider
	{
		get { return divider; }
		set
		{
			if (value == divider) { return; }
			divider = value;
			if (!preferredSize.IsEmpty)
			{
				// if it's cached, just +/- the divider height
				if (divider)
				{
					preferredSize.Height += dividerHeight;
				}
				else
				{
					preferredSize.Height -= dividerHeight;
				}
			}
			AdjustSize();
			Redraw();
		}
	}
	public override DockStyle Dock
	{
		get { return base.Dock; }
		set
		{
			if (value == base.Dock) { return; }
			base.Dock = value;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	}
	public bool DropDownArrows
	{
		get { return dropDownArrows; }
		set
		{
			if (value == dropDownArrows) { return; }
			dropDownArrows = value;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	}
	public override Color ForeColor
	{
		get { return base.ForeColor; }
		set
		{
			if (value == base.ForeColor) { return; }
			base.ForeColor = value;
			Redraw();
		}
	}
#endif
	public ImageList ImageList
	{
		get { return imageList; }
		set
		{
			if (value == imageList) { return; }

			if (imageList != null)
			{
				imageList.RecreateHandle -= new EventHandler(ImageListHandler);
			}

			Size oldImageSize = ImageSize;
			imageList = value;
			Size newImageSize = ImageSize;

			if (imageList != null)
			{
				imageList.RecreateHandle += new EventHandler(ImageListHandler);
			}

			// if the button size isn't auto-adjusted or the image
			// sizes are the same then there's no need to adjust size
			if (buttonSize.IsEmpty && oldImageSize != newImageSize)
			{
				cachedAutoButtonSize = Size.Empty;
				preferredSize = Size.Empty;
				AdjustSize();
			}

			Redraw();
		}
	}
#if !CONFIG_COMPACT_FORMS
	public
#else
	internal
#endif
	Size ImageSize
	{
		get
		{
			if (imageList == null)
			{
				return Size.Empty;
			}
			else
			{
				return imageList.ImageSize;
			}
		}
	}
#if !CONFIG_COMPACT_FORMS
	public new ImeMode ImeMode
	{
		get { return base.ImeMode; }
		set { base.ImeMode = value; }
	}
#endif
	private Size PreferredSize
	{
		get
		{
			if (!preferredSize.IsEmpty) { return preferredSize; }
			DockStyle dock = Dock;
			if (limitWidth == -1)
			{
				limitWidth = Parent == null ? 0 : Parent.Width;
			}
			if (limitHeight == -1)
			{
				limitHeight = Parent == null ? 0 : Parent.Height;
			}
			if (dock == DockStyle.Left || dock == DockStyle.Right)
			{
				preferredSize = CalculateSizeV(limitHeight);

				if (preferredSize.Height < limitHeight)
				{
					preferredSize.Height = limitHeight;
				}
			}
			else if (dock == DockStyle.Top || dock == DockStyle.Bottom)
			{
				preferredSize = CalculateSizeH(limitWidth);

				if (preferredSize.Width < limitWidth)
				{
					preferredSize.Width = limitWidth;
				}
			}
			else // dock == DockStyle.None
			{
				preferredSize = CalculateSizeH(limitWidth);
			}
			return preferredSize;
		}
	}
#if !CONFIG_COMPACT_FORMS
	[TODO]
	public override RightToLeft RightToLeft
	{
		get { return base.RightToLeft; }
		set
		{
			if (value == base.RightToLeft) { return; }
			base.RightToLeft = value;
			Redraw();
		}
	}
	public bool ShowToolTips
	{
		get { return showToolTips; }
		set
		{
			if (value == showToolTips) { return; }
			showToolTips = value;
		}
	}
	private static StringFormat StringFormat
	{
		get
		{
			StringFormat format = new StringFormat(StringFormat.GenericDefault);
			format.Trimming = StringTrimming.EllipsisWord;
			format.FormatFlags |= StringFormatFlags.NoWrap;
			return format;
		}
	}
	public new bool TabStop
	{
		get { return base.TabStop; }
		set { base.TabStop = value; }
	}
	[TODO]
	public override string Text
	{
		get { return base.Text; }
		set { base.Text = value; /* TODO? */ }
	}
	public ToolBarTextAlign TextAlign
	{
		get { return textAlign; }
		set
		{
			if (value == textAlign) { return; }
			textAlign = value;
			// if buttons aren't auto-sized, keep current size info
			if (buttonSize.IsEmpty)
			{
				cachedAutoButtonSize = Size.Empty;
				preferredSize = Size.Empty;
				AdjustSize();
			}
			Redraw();
		}
	}
#endif
	public bool Wrappable
	{
		get { return wrappable; }
		set
		{
			if (value == wrappable) { return; }
			wrappable = value;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	}



	// Methods
	private void AdjustSize()
	{
		if (!Visible || !IsHandleCreated) { return; }
		if (autoSize)
		{
			Size = PreferredSize;
		}
		else
		{
			Size = staticSize;
			CalculateStaticWrapData();
		}
		staticSize = Size;
	}
	private static ButtonState CalculateButtonState(bool flat,
	                                                bool click,
	                                                bool hover,
	                                                bool partial,
	                                                bool pushed)
	{
		ButtonState state = 0;
		if (flat && !hover && !click && !pushed)
		{
			state |= ButtonState.Flat;
		}
		if (click || pushed)
		{
			state |= ButtonState.Pushed;
		}
		if (partial)
		{
			state |= ButtonState.Inactive;
		}
		return state;
	}
	private Size CalculateSizeH(int limitWidth)
	{
		wrapData = new int[buttons.groups];
		limitWidth = StripLimitWidth(limitWidth);
		if (buttons.Count > 0)
		{
			int width = 0;
			int height = 0;

			// figure out the width of the button groups
			// not including separators
			int[] widths = new int[buttons.groups];
			int lastSep = -1;
			int count = buttons.Count;
			int bWidth = ButtonSize.Width;
			ToolBarButtonStyle dropStyle = ToolBarButtonStyle.DropDownButton;
			for (int i = 0, j = 0; i < count; ++i)
			{
				ToolBarButton b = buttons[i];
				if (!b.Visible) { continue; }
				if (b.groupID > lastSep)
				{
					++j;
					lastSep = b.groupID;
				}
				else
				{
					widths[j] += bWidth;
					if (dropDownArrows && b.Style == dropStyle)
					{
						widths[j] += dropDownWidth;
					}
				}
			}
			count = widths.Length;
			if (wrappable && count > 1)
			{
				int level = 0;
				int[] levelWidths = new int[count];
				levelWidths[0] = widths[0];
				for (int i = 1; i < count; ++i)
				{
					// fit as many groups on a level as
					// possible before wrapping to the next
					int tmp = levelWidths[level] +
					          separatorSize +
					          widths[i];
					if (tmp <= limitWidth)
					{
						levelWidths[level] = tmp;
						wrapData[i] = level;
					}
					else
					{
						width = width > levelWidths[level] ?
						        width : levelWidths[level];
						++level;
						levelWidths[level] = widths[i];
					}
				}
				width = width > levelWidths[level] ?
				        width : levelWidths[level];
				height = ButtonSize.Height*(level+1);
			}
			else
			{
				// there's always at least one group
				width = widths[0];
				for (int i = 0; i < count; ++i)
				{
					width += separatorSize + widths[i];
					wrapData[i] = 0;
				}
				height = ButtonSize.Height;
			}
			return CalculateSizeHelper(width,height);
		}
		else // buttons.Count == 0
		{
			// if there are no buttons, show a blank area
			// the size of one button
			wrapData[0] = 1;
			Size size = ButtonSize;
			return CalculateSizeHelper(size.Width,size.Height);
		}
	}
	private Size CalculateSizeV(int limitHeight)
	{
		wrapData = new int[buttons.groups];
		limitHeight = StripLimitHeight(limitHeight);
		if (buttons.Count > 0)
		{
			int width = 0;
			int height = 0;

			// figure out the height of the button groups
			// not including separators, and which groups have
			// drop down menus (which requires width adjustments)
			int[] heights = new int[buttons.groups];
			bool[] hasDropDowns = new bool[buttons.groups];
			int lastSep = -1;
			int count = buttons.Count;
			int bHeight = ButtonSize.Height;
			int bWidth = ButtonSize.Width;
			ToolBarButtonStyle dropStyle = ToolBarButtonStyle.DropDownButton;
			for (int i = 0, j = 0; i < count; ++i)
			{
				ToolBarButton b = buttons[i];
				if (!b.Visible) { continue; }
				if (b.groupID > lastSep)
				{
					++j;
					lastSep = b.groupID;
				}
				else
				{
					heights[j] += bHeight;
					if (dropDownArrows && b.Style == dropStyle)
					{
						hasDropDowns[j] = true;
					}
				}
			}
			count = heights.Length;
			if (wrappable && count > 1)
			{
				int level = 0;
				int[] levelHeights = new int[count];
				bool extraWidth = hasDropDowns[0];
				levelHeights[0] = heights[0];
				for (int i = 1; i < count; ++i)
				{
					// fit as many groups on a level as
					// possible before wrapping to the next
					int tmp = levelHeights[level] +
					          separatorSize +
					          heights[i];
					if (tmp <= limitHeight)
					{
						levelHeights[level] = tmp;
						extraWidth |= hasDropDowns[i];
						wrapData[i] = level;
					}
					else
					{
						height = height > levelHeights[level] ?
						         height : levelHeights[level];
						width += bWidth;
						if (extraWidth)
						{
							width += dropDownWidth;
						}
						extraWidth = false;
						++level;
						levelHeights[level] = heights[i];
					}
				}
				height = height > levelHeights[level] ?
				         height : levelHeights[level];
				width += bWidth;
				if (extraWidth)
				{
					width += dropDownWidth;
				}
			}
			else
			{
				// there's always at least one group
				height = heights[0];
				bool extraWidth = hasDropDowns[0];
				for (int i = 0; i < count; ++i)
				{
					height += separatorSize + heights[i];
					extraWidth |= hasDropDowns[i];
					wrapData[i] = 0;
				}
				width = bWidth;
				if (extraWidth)
				{
					width += dropDownWidth;
				}
			}
			return CalculateSizeHelper(width,height);
		}
		else // buttons.Count == 0
		{
			// if there are no buttons, show a blank area
			// the size of one button
			wrapData[0] = 1;
			Size size = ButtonSize;
			return CalculateSizeHelper(size.Width,size.Height);
		}
	}
	private Size CalculateSizeHelper(int width, int height)
	{
		if (borderStyle == BorderStyle.FixedSingle)
		{
			Size s = SystemInformation.BorderSize;
			width += s.Width*2;
			height += s.Height*2;
		}
		else if (borderStyle == BorderStyle.Fixed3D)
		{
			Size s = SystemInformation.Border3DSize;
			width += s.Width*2;
			height += s.Height*2;
		}

		if (divider) { height += dividerHeight; }

		width += 4; // 4px padding
		height += 4; // 4px padding
		return new Size(width,height);
	}
	private void CalculateStaticWrapData()
	{
		// reset wrapping data for current size
		DockStyle dock = base.Dock;
		if (dock == DockStyle.Left || dock == DockStyle.Right)
		{
			CalculateSizeH(staticSize.Width);
		}
		else
		{
			CalculateSizeV(staticSize.Height);
		}
	}
#if !CONFIG_COMPACT_FORMS
	protected override void CreateHandle()
	{
		base.CreateHandle();
	}
#endif
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (imageList != null)
			{
				imageList.RecreateHandle -= new EventHandler(ImageListHandler);
#if CONFIG_COMPONENT_MODEL
				imageList.Dispose();
#else
				imageList.Dispose(disposing);
#endif
			}
		}
		base.Dispose(disposing);
	}
	private void Draw(Graphics g)
	{
		if (!Visible || !IsHandleCreated) { return; }

		Size clientSize = ClientSize;
		int x = 0;
		int y = 0;
		int width = clientSize.Width;
		int height = clientSize.Height;

		using (Brush brush = CreateBackgroundBrush())
		{
			g.FillRectangle(brush,x,y,width,height);
		}

		bool vertical = LayoutButtons(x,y);

		// draw divider
		if (divider)
		{
			ControlPaint.DrawBorder3D(g,x,y,width,height,
			                          Border3DStyle.RaisedInner,
			                          Border3DSide.Top);
			y += dividerHeight;
			height -= dividerHeight;
		}

		// draw toolbar border
		BorderStyle style = BorderStyle;
		if (style == BorderStyle.Fixed3D)
		{
			ControlPaint.DrawBorder3D(g,x,y,width,height,Border3DStyle.SunkenInner);
		}
		else if (style == BorderStyle.FixedSingle)
		{
			// draw a single line around the toolbar, using the
			// forecolor, along the middle of the border's size
			Pen pen = new Pen(ForeColor);
			Size tmp = SystemInformation.BorderSize;
			int rectX = x+(tmp.Width/2);
			int rectY = y+(tmp.Height/2);
			int rectWidth = width-tmp.Width;
			int rectHeight = height-tmp.Height;
			g.DrawRectangle(pen,rectX,rectY,rectWidth,rectHeight);
		}

		// draw buttons
		for (int i = 0; i < buttons.Count; ++i)
		{
			ToolBarButton b = buttons[i];
			Rectangle view = b.viewRectangle;
			if (b.Visible && !view.IsEmpty)
			{
				DrawButton(i,g,vertical);
			}
		}
	}
	private void DrawButton(int index, Graphics g, bool vertical)
	{
		ToolBarButton b = buttons[index];
		Rectangle view = b.viewRectangle;
		int viewX = view.X;
		int viewY = view.Y;
		int viewWidth = view.Width;
		int viewHeight = view.Height;
		int dropX = view.X;
		int dropY = view.Y;
		int dropWidth = view.Width;
		int dropHeight = view.Height;
		bool drops = (dropDownArrows && b.Style == ToolBarButtonStyle.DropDownButton);
		bool flat = (appearance == ToolBarAppearance.Flat);

		using (Brush brush = CreateBackgroundBrush())
		{
			g.FillRectangle(brush,viewX,viewY,viewWidth,viewHeight);
			if (drops)
			{
				g.FillRectangle(brush,dropX,dropY,dropWidth,dropHeight);
			}
		}
		if (b.Style == ToolBarButtonStyle.Separator)
		{
			if (flat)
			{
				Pen pen = new Pen(ControlPaint.Dark(BackColor,0.3f));
				int x1,x2,y1,y2;
				if (vertical)
				{
					// vertical layouts have horizontal seps
					x1 = viewX+4; // left edge
					y1 = viewY+((viewHeight-2)/2); // middle y
					x2 = viewX+viewWidth-4; // right edge
					y2 = y1; // middle y
				}
				else
				{
					// horizontal layouts have vertical seps
					x1 = viewX+((viewWidth-2)/2); // center x
					y1 = viewY+4; // top edge
					x2 = x1; // center x
					y2 = viewY+viewHeight-4; // bottom edge
				}
				g.DrawLine(pen,x1,y1,x2,y2);
				pen = new Pen(ControlPaint.Light(BackColor,0.3f));
				if (vertical)
				{
					++y1;
					++y2;
				}
				else
				{
					++x1;
					++x2;
				}
				g.DrawLine(pen,x1,y1,x2,y2);
			}
			return;
		}

		bool viewHover = (index == mouseHoverClick);
		bool viewClick = (index == mouseDownClick);
		bool dropHover = (index == mouseHoverDrop);
		bool dropClick = (index == mouseDownDrop);
		bool enabled = b.Enabled;
		bool partial = (b.Style == ToolBarButtonStyle.ToggleButton);
		bool pushed = partial && b.Pushed;
		partial &= b.PartialPush;

		// if the button is a drop down button, but the drop down is
		// triggered from the viewRectangle, instead of the
		// dropRectangle, set viewClick to true for proper drawing
		viewClick |= (dropClick && !drops);


		ButtonState state = CalculateButtonState(flat,viewClick,viewHover,
		                                         partial,pushed);
		ThemeManager.MainPainter.DrawButton(g,
		                                    viewX, viewY,
		                                    viewWidth, viewHeight,
		                                    state,
		                                    ForeColor, BackColor, false);
		// adjust bounds to exclude button border
		viewX += 2;
		viewY += 2;
		viewWidth -= 4;
		viewHeight -= 4;
		if (drops)
		{
			state = CalculateButtonState(flat,dropClick,dropHover,
			                             false,false);
			ThemeManager.MainPainter.DrawScrollButton(g,
			                                          dropX, dropY,
			                                          dropWidth, dropHeight,
			                                          ScrollButton.Down, state,
			                                          ForeColor, BackColor);
			// adjust bounds to exclude button border
			dropX += 2;
			dropY += 2;
			dropWidth -= 4;
			dropHeight -= 4;
			// TODO - draw drop down arrow
		}

	#if !CONFIG_COMPACT_FORMS
		bool centerX = (textAlign == ToolBarTextAlign.Underneath);
		bool centerY = !centerX;
	#else
		bool centerX = true;
		bool centerY = true;
	#endif
		Size imageSize = ImageSize;
		if (imageSize.IsEmpty) { imageSize = new Size(1,1); }

		// TODO - intersect the clip region of g with the rect available
		//        for drawing the button interior so the Draw* operations
		//        don't draw outside the button interior
		int x = 0;
		int y = 0;
		int width = imageSize.Width;
		int height = imageSize.Height;

		if (centerX)
		{
			x += (viewWidth-width)/2;
		}
		if (centerY)
		{
			y += (viewHeight-height)/2;
		}
		if (viewClick)
		{
			++x;
			++y;
		}
		x += viewX; // translate to toolbar coordinates
		y += viewY; // translate to toolbar coordinates
		int imageIndex = b.ImageIndex;
		Image image = null;

		if (imageIndex != -1 && imageList != null)
		{
			ImageList.ImageCollection images = imageList.Images;
			if (images != null)
			{
				image = images[imageIndex];
			}
		}
		if (image != null)
		{
			if (enabled)
			{
				g.DrawImage(image,x,y,width,height);
			}
			else
			{
				// TODO - force scaling to required width and height
				ControlPaint.DrawImageDisabled(g,image,x,y,BackColor);
			}
		}
	#if !CONFIG_COMPACT_FORMS
		String text = b.Text;
		if (text == null || text == String.Empty)
		{
			return;
		}
		StringFormat format = StringFormat;
		if (textAlign == ToolBarTextAlign.Underneath)
		{
			x = viewX;
			y += height;
			if (viewClick)
			{
				++x;
			}
			width = viewWidth;
			height = (viewHeight-(y-viewY));
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Far;
		}
		else
		{
			x += width;
			y = viewY;
			if (viewClick)
			{
				++y;
			}
			width = (viewWidth-(x-viewX));
			height = viewHeight;
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
		}
		RectangleF bounds = new RectangleF(x,y,width,height);
		if (enabled)
		{
			using (Brush brush = new SolidBrush(ForeColor))
			{
				g.DrawString(text,Font,brush,bounds,format);
			}
		}
		else
		{
			ControlPaint.DrawStringDisabled(g,text,Font,BackColor,bounds,format);
		}
		// Remove the following and the program hangs, for some reason as
		// yet unknown, when pnet is configured for multi-threaded mode.
		// (i.e. --enable-threads=none fixes the problem)
		//Console.WriteLine();//TRACE
	#endif
	}
	private void ImageListHandler(Object sender, EventArgs e)
	{
		if (buttonSize.IsEmpty)
		{
			cachedAutoButtonSize = Size.Empty;
			preferredSize = Size.Empty;
			AdjustSize();
		}
		Redraw();
	}
	private bool LayoutButtons(int x, int y)
	{
		if (borderStyle == BorderStyle.FixedSingle)
		{
			Size s = SystemInformation.BorderSize;
			x = s.Width;
			y = s.Height;
		}
		else if (borderStyle == BorderStyle.Fixed3D)
		{
			Size s = SystemInformation.Border3DSize;
			x = s.Width;
			y = s.Height;
		}

		if (divider) { y += dividerHeight; }

		x += 2; // padding - keep synched with CalculateSizeHelper
		y += 2; // padding - keep synched with CalculateSizeHelper

		DockStyle dock = Dock;
		Size bSize = ButtonSize;
		bool vertical = (dock == DockStyle.Left || dock == DockStyle.Right);
		bool levelHasDropDown = false;
		int bWidth = bSize.Width;
		int bHeight = bSize.Height;

		for (int i = 0, level = 0, group = 0; i < buttons.Count; ++i)
		{
			ToolBarButton b = buttons[i];
			if (!b.Visible) { continue; }
			if (b.Style == ToolBarButtonStyle.Separator)
			{
				++group;
				if (wrapData[group] != level)
				{
					++level;
					if (vertical)
					{
						x += bWidth;
						if (levelHasDropDown)
						{
							x += dropDownWidth;
						}
					}
					else
					{
						y += bHeight;
					}
					levelHasDropDown = false;
					b.viewRectangle = Rectangle.Empty;
				}
				else if (vertical)
				{
					b.viewRectangle = new Rectangle(x,y,bWidth,separatorSize);
					y += separatorSize;
				}
				else
				{
					b.viewRectangle = new Rectangle(x,y,separatorSize,bHeight);
					x += separatorSize;
				}
				b.dropRectangle = Rectangle.Empty;
				continue;
			}
			b.viewRectangle = new Rectangle(x,y,bWidth,bHeight);
			if (dropDownArrows && b.Style == ToolBarButtonStyle.DropDownButton)
			{
				b.dropRectangle = new Rectangle(x+bWidth,y,dropDownWidth,bHeight);
				levelHasDropDown = true;
				if (vertical)
				{
					y += bHeight;
				}
				else
				{
					x += bWidth + dropDownWidth;
				}
			}
			else
			{
				if (vertical)
				{
					y += bHeight;
				}
				else
				{
					x += bWidth;
				}
				b.dropRectangle = Rectangle.Empty;
			}
		}
		return vertical; // just a convenience ;)
	}
	protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e)
	{
		ToolBarButtonClickEventHandler handler;
		handler = ((ToolBarButtonClickEventHandler)
		           (GetHandler(EventId.ButtonClick)));
		if (handler != null)
		{
			handler(this, e);
		}
	}
	protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e)
	{
		ToolBarButtonClickEventHandler handler;
		handler = ((ToolBarButtonClickEventHandler)
		           (GetHandler(EventId.ButtonDropDown)));
		if (handler != null)
		{
			handler(this, e);
		}
		ToolBarButton b = e.Button;
		ContextMenu menu = (b.DropDownMenu as ContextMenu);
		if (menu != null)
		{
			Rectangle rect;
			if (dropDownArrows)
			{
				rect = b.dropRectangle;
			}
			else
			{
				rect = b.viewRectangle;
			}
			int x = rect.X; // left
			int y = rect.Y+rect.Height; // bottom
			menu.Show(this,new Point(x,y));
		}
	}
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		establishedTextBounds = Size.Empty;
		cachedAutoButtonSize = Size.Empty;
		if (buttonSize.IsEmpty)
		{
			preferredSize = Size.Empty;
			AdjustSize();
		}
		Redraw();
	}
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}
	protected override void OnMouseDown(MouseEventArgs e)
	{
		int x = e.X;
		int y = e.Y;
		for (int i = 0; i < buttons.Count; ++i)
		{
			ToolBarButton b = buttons[i];
			switch (b.Contains(x,y,dropDownArrows))
			{
				case 1:
				{
					mouseDownClick = i;
					RedrawButton(i);
					base.OnMouseDown(e);
					return;
				}
				case 2:
				{
					mouseDownDrop = i;
					RedrawButton(i);
					base.OnMouseDown(e);
					return;
				}
				case -1:
				{
					// it's a separator
					ResetMouseStates();
					base.OnMouseDown(e);
					return;
				}
			}
		}
		base.OnMouseDown(e);
	}
	protected override void OnMouseLeave(EventArgs e)
	{
		ResetMouseStates();
		base.OnMouseLeave(e);
	}
	protected override void OnMouseMove(MouseEventArgs e)
	{
		int x = e.X;
		int y = e.Y;
		bool flat = (appearance == ToolBarAppearance.Flat);
		if (mouseDownClick != -1)
		{
			ToolBarButton b = buttons[mouseDownClick];
			if (b.Contains(x,y,dropDownArrows) != 1)
			{
				int tmp = mouseDownClick;
				mouseDownClick = -1;
				RedrawButton(tmp);
			}
		}
		else if (mouseDownDrop != -1)
		{
			ToolBarButton b = buttons[mouseDownDrop];
			if (b.Contains(x,y,dropDownArrows) != 2)
			{
				int tmp = mouseDownDrop;
				mouseDownDrop = -1;
				RedrawButton(tmp);
			}
		}
		else if (mouseHoverClick != -1)
		{
			ToolBarButton b = buttons[mouseHoverClick];
			if (b.Contains(x,y,dropDownArrows) != 1)
			{
				int tmp = mouseHoverClick;
				mouseHoverClick = -1;
				RedrawButton(tmp);
			}
		}
		else if (mouseHoverDrop != -1)
		{
			ToolBarButton b = buttons[mouseHoverDrop];
			if (b.Contains(x,y,dropDownArrows) != 2)
			{
				int tmp = mouseHoverDrop;
				mouseHoverDrop = -1;
				RedrawButton(tmp);
			}
		}
		if (flat)
		{
			for (int i = 0; i < buttons.Count; ++i)
			{
				ToolBarButton b = buttons[i];
				switch (b.Contains(x,y,dropDownArrows))
				{
					case 1:
					{
						if (flat && mouseHoverClick != i)
						{
							int tmp = mouseHoverClick;
							mouseHoverClick = i;
							if (tmp != -1)
							{
								RedrawButton(tmp);
							}
							RedrawButton(i);
						}
						base.OnMouseMove(e);
						return;
					}
					case 2:
					{
						if (flat && mouseHoverDrop != i)
						{
							int tmp = mouseHoverDrop;
							mouseHoverDrop = i;
							if (tmp != -1)
							{
								RedrawButton(tmp);
							}
							RedrawButton(i);
						}
						base.OnMouseMove(e);
						return;
					}
				}
			}
		}
		base.OnMouseMove(e);
	}
	protected override void OnMouseUp(MouseEventArgs e)
	{
		int x = e.X;
		int y = e.Y;
		if (mouseDownClick != -1)
		{
			int tmp = mouseDownClick;
			mouseDownClick = -1;
			RedrawButton(tmp);
			if (buttons[tmp].Contains(x,y,dropDownArrows) == 1)
			{
				base.OnMouseUp(e);
				OnButtonClick(new ToolBarButtonClickEventArgs(buttons[tmp]));
				return;
			}
		}
		else if (mouseDownDrop != -1)
		{
			int tmp = mouseDownDrop;
			mouseDownDrop = -1;
			RedrawButton(tmp);
			if (buttons[tmp].Contains(x,y,dropDownArrows) == 2)
			{
				base.OnMouseUp(e);
				OnButtonDropDown(new ToolBarButtonClickEventArgs(buttons[tmp]));
				return;
			}
		}
		base.OnMouseUp(e);
	}
	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		AdjustSize();
		Draw(e.Graphics);
	}
	protected override void OnResize(EventArgs e)
	{
		preferredSize = Size.Empty;
		AdjustSize();
		Redraw();
		base.OnResize(e);
	}
	private void RecacheAutoButtonSize(Graphics g)
	{
		int width;
		int height;
		Size tmp = ImageSize;
		if (tmp.IsEmpty)
		{
			width = 1;
			height = 1;
		}
		else
		{
			width = tmp.Width;
			height = tmp.Height;
		}

	#if !CONFIG_COMPACT_FORMS
		if (establishedTextBounds.IsEmpty)
		{
			float maxWidth = 0;
			float maxHeight = 0;
			Font font = Font;
			SizeF textBounds = new SizeF(font.Height*12,font.Height);
			StringFormat format = StringFormat;
			SizeF size;
			String text;
			for (int i = 0; i < buttons.Count; ++i)
			{
				ToolBarButton b = buttons[i];
				if (!b.Visible ||
				    b.Style == ToolBarButtonStyle.Separator)
				{
					continue;
				}
				text = buttons[i].Text;
				size = g.MeasureString(text,font,textBounds,format);
				// recalculate the max width and height
				maxWidth = maxWidth > size.Width ?
				           maxWidth : size.Width;
				maxHeight = maxHeight > size.Height ?
				            maxHeight : size.Height;
			}
			// add the ceiling of the max text size to the total
		#if CONFIG_EXTENDED_NUMERICS
			int tmpW = (int)(Math.Ceiling(maxWidth));
			int tmpH = (int)(Math.Ceiling(maxHeight));
		#else
			int tmpW = (int)(maxWidth+0.99f);
			int tmpH = (int)(maxWidth+0.99f);
		#endif
			establishedTextBounds = new Size(tmpW,tmpH);
		}
		width += establishedTextBounds.Width;
		height += establishedTextBounds.Height;
	#endif
		// adjust for button borders (4px) and click shift (1px)
		width += 5;
		height += 5;

		cachedAutoButtonSize = new Size(width,height);
	}
	private void Redraw()
	{
		if (!Visible || !IsHandleCreated) { return; }

		using (Graphics graphics = CreateGraphics())
		{
			Draw(graphics);
		}
	}
	private void RedrawButton(int index)
	{
		if (!Visible || !IsHandleCreated || !buttons[index].Visible)
		{
			return;
		}

		DockStyle dock = Dock;
		bool vertical = (dock == DockStyle.Left || dock == DockStyle.Right);
		using (Graphics g = CreateGraphics())
		{
			
			DrawButton(index,g,vertical);
		}
	}
	private void ResetMouseStates()
	{
		if (mouseDownClick != -1)
		{
			int tmp = mouseDownClick;
			mouseDownClick = -1;
			RedrawButton(tmp);
		}
		if (mouseDownDrop != -1)
		{
			int tmp = mouseDownDrop;
			mouseDownDrop = -1;
			RedrawButton(tmp);
		}
		if (appearance == ToolBarAppearance.Flat)
		{
			if (mouseHoverClick != -1)
			{
				int tmp = mouseHoverClick;
				mouseHoverClick = -1;
				RedrawButton(tmp);
			}
			if (mouseHoverDrop != -1)
			{
				int tmp = mouseHoverDrop;
				mouseHoverDrop = -1;
				RedrawButton(tmp);
			}
		}
	}
#if !CONFIG_COMPACT_FORMS
	protected override void SetBoundsCore(int x, int y,
	                                      int width, int height,
	                                      BoundsSpecified specified)
	{
		Size oldSize = Size;
		if ((specified & BoundsSpecified.Width) != 0)
		{
			limitWidth = width;
		}
		if ((specified & BoundsSpecified.Height) != 0)
		{
			limitHeight = height;
		}
		if (autoSize)
		{
			preferredSize = Size.Empty;
			Size size = PreferredSize;
			width = size.Width;
			height = size.Height;
		}
		base.SetBoundsCore(x, y, width, height, specified);
		staticSize = Size;
		if (oldSize != staticSize)
		{
			if (!autoSize)
			{
				CalculateStaticWrapData();
			}
			Redraw();
		}
	}
#endif
	// limit on height of toolbar internals (ie. minus borders and divider)
	private int StripLimitHeight(int limitHeight)
	{
		if (borderStyle == BorderStyle.FixedSingle)
		{
			limitHeight -= SystemInformation.BorderSize.Height*2;
		}
		else if (borderStyle == BorderStyle.Fixed3D)
		{
			limitHeight -= SystemInformation.Border3DSize.Height*2;
		}
		if (divider)
		{
			limitHeight -= dividerHeight;
		}
		return limitHeight;
	}
	// limit on width of toolbar internals (ie. minus borders)
	private int StripLimitWidth(int limitWidth)
	{
		if (borderStyle == BorderStyle.FixedSingle)
		{
			limitWidth -= SystemInformation.BorderSize.Width*2;
		}
		else if (borderStyle == BorderStyle.Fixed3D)
		{
			limitWidth -= SystemInformation.Border3DSize.Width*2;
		}
		return limitWidth;
	}
	internal void TBBCUpdate(bool trivial)
	{
		// trivial is for grouping changes only (MakeGroup/KillGroup)
		if (!trivial)
		{
			establishedTextBounds = Size.Empty;
			cachedAutoButtonSize = Size.Empty;
		}
		preferredSize = Size.Empty;
		AdjustSize();
		Redraw();
	}
	internal void TBBUpdate(int index, bool trivial)
	{
		// trivial is for minor state changes (enabled/disabled, etc.)
		// anything that might change sizing data, like style changes
		// is considered non-trivial and such data is recalculated
		if (trivial)
		{
			RedrawButton(index);
		}
		else
		{
			establishedTextBounds = Size.Empty;
			cachedAutoButtonSize = Size.Empty;
			preferredSize = Size.Empty;
			AdjustSize();
			Redraw();
		}
	}

	public override string ToString()
	{
		string ret = base.ToString() + ", Buttons.Count: " + buttons.Count.ToString();
		if(buttons.Count > 0)
		{
			for(int cnt = 0; cnt < buttons.Count; cnt++)
			{
				ret += ", Buttons[" + cnt.ToString() + "]: ToolBarButton: " + buttons[cnt].Text + ", Style: " + buttons[cnt].Style.ToString();
			}
		}
		return ret;
	}
#if !CONFIG_COMPACT_FORMS
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}
#endif
	public event ToolBarButtonClickEventHandler ButtonClick
	{
		add { AddHandler(EventId.ButtonClick,value); }
		remove { RemoveHandler(EventId.ButtonClick,value); }
	}
	public event ToolBarButtonClickEventHandler ButtonDropDown
	{
		add { AddHandler(EventId.ButtonDropDown,value); }
		remove { RemoveHandler(EventId.ButtonDropDown,value); }
	}























	public class ToolBarButtonCollection : IList
	{
		// Variables
		private ToolBar owner;
		private ToolBarButton[] buttons = new ToolBarButton[8];
		private int count = 0;
		private int capacity = 8;
		internal int groups = 1;
		private int generation = 0;



		// Constructor
	#if !CONFIG_COMPACT_FORMS
		public
	#else
		internal
	#endif
		ToolBarButtonCollection(ToolBar owner) : base()
		{
			this.owner = owner;
		}



		// Properties
		public virtual int Count { get { return count; } }
		public virtual bool IsReadOnly { get { return false; } }
	#if !CONFIG_COMPACT_FORMS
		public virtual ToolBarButton this[int index]
		{
			get
			{
				if (index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException(/* TODO */);
				}
				return buttons[index];
			}
			set
			{
				if (index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException(/* TODO */);
				}
				if (value == null)
				{
					throw new ArgumentNullException(/* TODO */);
				}
				if (value == buttons[index]) { return; }
				if (value.parent != null)
				{
					throw new ArgumentException(/* TODO */);
				}
				ToolBarButton b = buttons[index];
				buttons[index] = value;
				value.parent = owner;
				value.tbbcIndex = index;
				value.groupID = b.groupID;
				b.Reset();

				int flags = 0;
				if (b.Style == ToolBarButtonStyle.Separator)
				{
					flags |= 1;
				}
				if (value.Style == ToolBarButtonStyle.Separator)
				{
					flags |= 2;
				}
				if (flags == 1)
				{
					KillGroupQuiet(index);
				}
				else if (flags == 2)
				{
					MakeGroupQuiet(index);
				}

				++generation;
				owner.TBBCUpdate(false);
			}
		}
	#endif



		// Methods
	#if !CONFIG_COMPACT_FORMS
		public int Add(string text)
		{
			EnsureCapacity(count+1);
			ToolBarButton button = new ToolBarButton(text);
			button.parent = owner;
			button.tbbcIndex = count;
			buttons[count] = button;
			++count;

			if (count > 1)
			{
				button.groupID = buttons[count-2].groupID;
			}
			else
			{
				button.groupID = -1;
			}

			owner.TBBCUpdate(false);
			++generation;
			return count-1;
		}
	#endif
		public int Add(ToolBarButton button)
		{
			if (button == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (button.parent != null)
			{
				throw new ArgumentException(/* TODO */);
			}
			EnsureCapacity(count+1);
			button.parent = owner;
			button.tbbcIndex = count;
			buttons[count] = button;
			++count;

			if (count > 1)
			{
				button.groupID = buttons[count-2].groupID;
			}
			else
			{
				button.groupID = -1;
			}

			if (button.Style == ToolBarButtonStyle.Separator)
			{
				MakeGroupQuiet(count-1);
			}

			++generation;
			owner.TBBCUpdate(false);
			return count-1;
		}
	#if !CONFIG_COMPACT_FORMS
		public void AddRange(ToolBarButton[] buttons)
		{
			EnsureCapacity(count+buttons.Length);
			int lastSep = -1;
			if (count > 0)
			{
				lastSep = this.buttons[count-1].groupID;
			}
			for (int i = 0; i < buttons.Length; ++i)
			{
				ToolBarButton button = buttons[i];
				if (button == null)
				{
					throw new ArgumentNullException(/* TODO */);
				}
				if (button.parent != null)
				{
					throw new ArgumentException(/* TODO */);
				}
				button.parent = owner;
				button.tbbcIndex = count;
				button.groupID = lastSep;
				this.buttons[count] = button;
				++count;
				if (button.Style == ToolBarButtonStyle.Separator)
				{
					MakeGroupQuiet(count-1);
					lastSep = button.groupID;
				}
			}

			++generation;
			owner.TBBCUpdate(false);
		}
	#endif
		public virtual void Clear()
		{
			for (int i = 0; i < count; ++i)
			{
				buttons[i].Reset();
				buttons[i] = null;
			}
			count = 0;
			groups = 1;

			++generation;
			owner.TBBCUpdate(false);
		}
		public bool Contains(ToolBarButton button)
		{
			return (IndexOf(button) != -1);
		}
		private void EnsureCapacity(int required)
		{
			if (required > capacity)
			{
				// the +31 &~31 stuff is taken from ArrayList
				// it calculates the smallest mutliple of 32
				// which meets the required capacity
				// if doubling is insufficient
				int newCapacity = ((required + 31) & ~31);
				int newCapacity2 = capacity * 2;
				if (newCapacity2 > newCapacity)
				{
					newCapacity = newCapacity2;
				}
				ToolBarButton[] tmp = new ToolBarButton[newCapacity];
				Array.Copy(buttons,0,tmp,0,count);
				buttons = tmp;
			}
		}
		public virtual IEnumerator GetEnumerator()
		{
			return new TBBCEnumerator(this);
		}
		public int IndexOf(ToolBarButton button)
		{
			for (int i = 0; i < count; ++i)
			{
				if (buttons[i] == button) { return i; }
			}
			return -1;
		}
		public void Insert(int index, ToolBarButton button)
		{
			if (index > count)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}
			if (button == null)
			{
				throw new ArgumentNullException(/* TODO */);
			}
			if (button.parent != null)
			{
				throw new ArgumentException(/* TODO */);
			}
			EnsureCapacity(count+1);
			button.parent = owner;
			button.tbbcIndex = index;

			if (count > 0)
			{
				button.groupID = buttons[count-1].groupID;
			}
			else
			{
				button.groupID = -1;
			}

			for (int i = count; i > index; --i)
			{
				buttons[i] = buttons[i-1];
				buttons[i].tbbcIndex = i;
				if (buttons[i].groupID != button.groupID)
				{
					buttons[i].groupID++;
				}
			}
			buttons[index] = button;
			++count;

			if (button.Style == ToolBarButtonStyle.Separator)
			{
				MakeGroupQuiet(count-1);
			}

			++generation;
			owner.TBBCUpdate(false);
		}
		internal void KillGroupQuiet(int index)
		{
			int lastSep = -1;
			if (index > 0)
			{
				lastSep = buttons[index-1].groupID;
			}
			for (int i = index; i < count; ++i)
			{
				if (buttons[i].groupID == index)
				{
					buttons[i].groupID = lastSep;
				}
				else
				{
					return;
				}
			}
			groups--;
		}
		internal void MakeGroupQuiet(int index)
		{
			int lastSep = buttons[index].groupID;
			for (int i = index; i < count; ++i)
			{
				if (buttons[i].groupID == lastSep)
				{
					buttons[i].groupID = index;
				}
				else
				{
					return;
				}
			}
			groups++;
		}
		public void Remove(ToolBarButton button)
		{
			if (button == null) { return; }
			if (button.parent != owner) { return; }

			int index = button.tbbcIndex;
			if (buttons[index] != button)
			{
				// someone's been playing around with
				// the public constructor (ie. stupid spec.)
				return;
			}
			RemoveAt(index);
		}
		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= count)
			{
				throw new ArgumentOutOfRangeException(/* TODO */);
			}

			ToolBarButton button = buttons[index];

			if (button.Style == ToolBarButtonStyle.Separator)
			{
				KillGroupQuiet(index);
			}

			int lastSep = button.groupID;
			for (int i = index; i+1 < count; ++i)
			{
				buttons[i] = buttons[i+1];
				buttons[i].tbbcIndex = i;
				if (buttons[i].groupID != lastSep)
				{
					buttons[i].groupID--;
				}
			}

			button.Reset();

			--count;
			buttons[count] = null;

			++generation;
			owner.TBBCUpdate(false);
		}

		// Properties (Explicit IList Implementation)
		bool IList.IsFixedSize { get { return false; } }
		object IList.this[int index]
		{
			get { return this[index]; }
			set
			{
				ToolBarButton b = (value as ToolBarButton);
				if (b == null)
				{
					throw new ArgumentException(/* TODO */);
				}
				this[index] = b;
			}
		}

		// Methods (Explicit IList Implementation)
		int IList.Add(object o)
		{
			ToolBarButton b = (o as ToolBarButton);
			if (b == null)
			{
				throw new ArgumentException(/* TODO */);
			}
			return Add(b);
		}
		bool IList.Contains(object o)
		{
			ToolBarButton b = (o as ToolBarButton);
			if (b == null) { return false; }
			return Contains(b);
		}
		int IList.IndexOf(object o)
		{
			ToolBarButton b = (o as ToolBarButton);
			if (b == null) { return -1; }
			return IndexOf(b);
		}
		void IList.Insert(int index, object o)
		{
			ToolBarButton b = (o as ToolBarButton);
			if (b == null)
			{
				throw new ArgumentException(/* TODO */);
			}
			Insert(index,b);
		}
		void IList.Remove(object o)
		{
			ToolBarButton b = (o as ToolBarButton);
			if (b == null)
			{
				throw new ArgumentException(/* TODO */);
			}
			Remove(b);
		}

		// Properties (Explicit ICollection Implementation)
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}
		object ICollection.SyncRoot
		{
			get { return this; }
		}

		// Methods (Explicit ICollection Implementation)
		void ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(buttons,0,array,index,count);
		}















		private class TBBCEnumerator : IEnumerator
		{
			private ToolBarButtonCollection tbbc;
			private int position;
			private int count;
			private int generation;

			public TBBCEnumerator(ToolBarButtonCollection tbbc)
			{
				this.tbbc = tbbc;
				position = -1;
				count = tbbc.Count;
				generation = tbbc.generation;
			}

			public bool MoveNext()
			{
				if (generation != tbbc.generation)
				{
					throw new InvalidOperationException
					(
						S._("Invalid_CollectionModified")
					);
				}
				++position;
				return (position < count);
			}

			public void Reset()
			{
				position = -1;
			}

			public Object Current
			{
				get
				{
					if(generation != tbbc.generation)
					{
						throw new InvalidOperationException
						(
							S._("Invalid_CollectionModified")
						);
					}
					else if(position < 0 || position >= count)
					{
						throw new InvalidOperationException
						(
							S._("Invalid_BadEnumeratorPosition")
						);
					}
					return tbbc[position];
				}
			}

		}; // class TBBCEnumerator

	}; // class ToolBarButtonCollection

}; // class ToolBar

}; // namespace System.Windows.Forms
