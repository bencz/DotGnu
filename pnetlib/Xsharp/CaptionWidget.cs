/*
 * CaptionWidget.cs - Widget that applies a caption to another window.
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

namespace Xsharp
{

using System;
using System.Reflection;
using Xsharp.Events;
using DotGNU.Images;
using OpenSystem.Platform.X11;

internal class CaptionWidget : InputOutputWidget
{
	// Internal state.
	private TopLevelWindow child;
	private static Font captionFont;
	private int captionHeight;
	private CaptionFlags flags;
	private HitTest clickMode;
	private int startX, startY;
	private int startWidth, startHeight;
	private int startRootX, startRootY;
	private Xsharp.Image gradient;
	private int restoreX, restoreY, restoreWidth, restoreHeight;

	// Operations that are passed up from the child.
	internal enum Operation
	{
		Destroy,
		FirstMap,
		Map,
		Unmap,
		Raise,
		Lower,
		Iconify,
		Deiconify,
		Maximize,
		Restore,
		Title,
		Decorations,
		SetMinimumSize,
		SetMaximumSize,
		SetIcon,

	} // enum Operation

	// Flags that may be set on this caption widget.
	internal enum CaptionFlags
	{
		Active			= (1 << 0),
		HasClose		= (1 << 1),
		HasMaximize		= (1 << 2),
		HasRestore		= (1 << 3),
		HasMinimize		= (1 << 4),
		HasHelp			= (1 << 5),
		ClosePressed	= (1 << 6),
		MaximizePressed	= (1 << 7),
		RestorePressed	= (1 << 8),
		MinimizePressed	= (1 << 9),
		HelpPressed		= (1 << 10),
		Grabbed			= (1 << 11),
		InMoveResize	= (1 << 12),

	} // enum CaptionFlags

	// Region codes that result from hit testing.
	private enum HitTest
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Top,
		Bottom,
		Left,
		Right,
		Caption,
		Close,
		Maximize,
		Restore,
		Minimize,
		Help,
		Client,
		Outside,
		Move

	} // enum HitTest

	// Useful constants.
	private const int FrameBorderSize   = 4;
	private const int CaptionExtra      = 4;
	private const int CaptionMinimum    = 18;
	private const int CornerSize        = 20;
	private const int AbsoluteMinWidth  = 100;
	private const int AbsoluteMinHeight = 50;
	private const int MinimizedWidth    = 160;

	// Construct a new caption widget underneath "parent", which
	// encapsulates the given "child" widget.
	public CaptionWidget(Widget parent, String name, int x, int y,
						 int width, int height, Type type)
			: base(parent, x, y,
				   width + FrameBorderSize * 2,
				   height + GetCaptionHeight(parent) + FrameBorderSize,
				   new Color(StandardColor.Foreground),
				   new Color(StandardColor.Background))
			{
				// Don't automatically map the child when it is created.
				AutoMapChildren = false;

				// Calculate the size of the caption, including the border.
				captionHeight = GetCaptionHeight(parent);

				// The caption widget is not focusable.
				Focusable = false;

				// Create the "top-level" window object for the child.
				ConstructorInfo ctor = type.GetConstructor
					(new Type [] {typeof(Widget), typeof(String),
								  typeof(int), typeof(int),
								  typeof(int), typeof(int)});
				child = (TopLevelWindow)(ctor.Invoke
					(new Object[] {this, name, FrameBorderSize, captionHeight,
					 			   width, height}));
				child.reparented = true;

				// Set the default flags.
				flags = CaptionFlags.HasClose |
						CaptionFlags.HasMaximize |
						CaptionFlags.HasMinimize;
				clickMode = HitTest.Outside;

				// Perform an initial move/resize to position the child
				// window properly within the MDI client area.
				MoveResize(this.x, this.y, this.width, this.height);

				// Make sure that we have the inactive grab.
				MakeInactive();
			}

	// Get the child window that is encapsulated by this caption widget.
	public TopLevelWindow Child
			{
				get
				{
					return child;
				}
			}

	// Get the caption font.
	private static Font GetCaptionFont()
			{
				lock(typeof(CaptionWidget))
				{
					if(captionFont == null)
					{
						captionFont = Font.CreateFont
							(Font.DefaultSansSerif, 120, FontStyle.Bold);
					}
					return captionFont;
				}
			}

	// Get the size of the top-most border on a caption widget,
	// including the title bar display area.
	private static int GetCaptionHeight(Widget widget)
			{
				Font font = GetCaptionFont();
				int height;
				using(Graphics graphics = new Graphics(widget))
				{
					FontExtents extents = font.GetFontExtents(graphics);
					height = extents.Ascent + extents.Descent;
				}
				height += CaptionExtra;
				if(height < CaptionMinimum)
				{
					height = CaptionMinimum;
				}
				return height + FrameBorderSize;
			}

	// Draw a caption button.  Returns the width of the button.
	private static int DrawCaptionButton
					(Graphics graphics, Rectangle rect,
					 int subtract, bool pressed, bool draw,
					 XPixmap buttonPixmap)
			{
				int buttonSize = rect.height - 4;
				int x = rect.x + rect.width - subtract - buttonSize;
				int y = rect.y + 2;
				if(draw)
				{
					if(pressed)
					{
						graphics.DrawEffect(x, y, buttonSize, buttonSize,
											Effect.CaptionButtonIndented);
						++x;
						++y;
					}
					else
					{
						graphics.DrawEffect(x, y, buttonSize, buttonSize,
											Effect.CaptionButtonRaised);
					}
					x += (buttonSize - 9) / 2;
					y += (buttonSize - 9) / 2;
					graphics.DrawBitmap(x, y, 9, 9, buttonPixmap);
				}
				return buttonSize;
			}

	// Draw the caption buttons in their current state.  Returns the
	// number of pixels on the right of the caption that are occupied
	// by the buttons.
	private static int DrawCaptionButtons
				(Graphics graphics, Rectangle rect,
				 CaptionFlags flags, CaptionFlags buttonsToDraw)
			{
				int subtract = 2;
				if((flags & CaptionFlags.HasClose) != 0)
				{
					subtract += DrawCaptionButton
						(graphics, rect, subtract,
						 ((flags & CaptionFlags.ClosePressed) != 0),
						 ((buttonsToDraw & CaptionFlags.HasClose) != 0),
						 graphics.dpy.bitmaps.Close);
					if((flags & (CaptionFlags.HasMaximize |
								 CaptionFlags.HasRestore |
								 CaptionFlags.HasMinimize)) != 0)
					{
						// Leave a gap between the close button and the others.
						subtract += 2;
					}
				}
				if((flags & CaptionFlags.HasMaximize) != 0)
				{
					subtract += DrawCaptionButton
						(graphics, rect, subtract,
						 ((flags & CaptionFlags.MaximizePressed) != 0),
						 ((buttonsToDraw & CaptionFlags.HasMaximize) != 0),
						 graphics.dpy.bitmaps.Maximize);
				}
				if((flags & CaptionFlags.HasRestore) != 0)
				{
					subtract += DrawCaptionButton
						(graphics, rect, subtract,
						 ((flags & CaptionFlags.RestorePressed) != 0),
						 ((buttonsToDraw & CaptionFlags.HasRestore) != 0),
						 graphics.dpy.bitmaps.Restore);
				}
				if((flags & CaptionFlags.HasMinimize) != 0)
				{
					subtract += DrawCaptionButton
						(graphics, rect, subtract,
						 ((flags & CaptionFlags.MinimizePressed) != 0),
						 ((buttonsToDraw & CaptionFlags.HasMinimize) != 0),
						 graphics.dpy.bitmaps.Minimize);
				}
				if((flags & CaptionFlags.HasHelp) != 0)
				{
					// Leave a gap between the help button and the others.
					subtract += 2;
					subtract += DrawCaptionButton
						(graphics, rect, subtract,
						 ((flags & CaptionFlags.HelpPressed) != 0),
						 ((buttonsToDraw & CaptionFlags.HasHelp) != 0),
						 graphics.dpy.bitmaps.Help);
				}
				if(subtract > 2)
				{
					// Leave a gap between the buttons and the text.
					return subtract + 2;
				}
				else
				{
					// There are no buttons, so no need for an extra gap.
					return 2;
				}
			}

	// Create a gradient for the background of a title bar.
	private DotGNU.Images.Image CreateGradient
				(int width, int height, Color startColor, Color endColor)
			{
				if(startColor.Index != StandardColor.RGB)
				{
					startColor = screen.ToColor(startColor.Index);
				}
				if(endColor.Index != StandardColor.RGB)
				{
					endColor = screen.ToColor(endColor.Index);
				}
				int startR = startColor.Red;
				int startG = startColor.Green;
				int startB = startColor.Blue;
				int lenR = endColor.Red - startR;
				int lenG = endColor.Green - startG;
				int lenB = endColor.Blue - startB;
				DotGNU.Images.Image image = new DotGNU.Images.Image
					(width, height, PixelFormat.Format24bppRgb);
				Frame frame = image.AddFrame();
				int x, y, red, green, blue;
				for(y = 0; y < height; ++y)
				{
					for(x = 0; x < width; ++x)
					{
						red = startR + lenR * x / width;
						green = startG + lenG * x / width;
						blue = startB + lenB * x / width;
						frame.SetPixel(x, y, (red << 16) + (green << 8) + blue);
					}
				}
				return image;
			}

	// Paint this widget in response to an "Expose" event.
	protected override void OnPaint(Graphics graphics)
			{
				// Draw the thick 3D border around the outside first.
				graphics.DrawEffect(0, 0, width, height, Effect.Raised);

				// Get the rectangle containing the caption area.
				Rectangle rect = new Rectangle
					(FrameBorderSize, FrameBorderSize,
					 width - FrameBorderSize * 2,
					 captionHeight - FrameBorderSize);

				// If the rectangle does not overlap the expose region,
				// then there is no point drawing the main caption area.
				if(!graphics.ExposeRegion.Overlaps(rect))
				{
					return;
				}

				// Get the colors to use for the foreground and background.
				Color foreground, background, endBackground;
				if((flags & CaptionFlags.Active) != 0)
				{
					foreground = new Color(StandardColor.HighlightForeground);
					background = new Color(StandardColor.HighlightBackground);
					endBackground =
						new Color(StandardColor.HighlightEndBackground);
				}
				else
				{
					foreground = new Color(StandardColor.Background);
					background = new Color(StandardColor.BottomShadow);
					endBackground = new Color(StandardColor.EndBackground);
				}

				// Create a gradient for the title bar, if necessary.
				if(gradient != null &&
				   (gradient.Width != rect.width ||
				    gradient.Height != rect.height))
				{
					// The size has changed and we need a new gradient.
					gradient.Dispose();
					gradient = null;
				}
				if(gradient == null && screen.DefaultDepth >= 15)
				{
					DotGNU.Images.Image image = CreateGradient
						(rect.width, rect.height, background, endBackground);
					gradient = new Xsharp.Image(screen, image.GetFrame(0));
					image.Dispose();
				}

				// Clear the caption background.
				if(gradient == null)
				{
					graphics.Foreground = background;
					graphics.SetFillSolid();
					graphics.FillRectangle(rect);
				}
				else
				{
					graphics.SetFillTiled(gradient.Pixmap, rect.x, rect.y);
					graphics.FillRectangle(rect);
					graphics.SetFillSolid();
				}

				// Draw the caption buttons and then subtract that
				// region off the caption rectangle so we don't get
				// bleed through when we draw the caption text.
				rect.width -= DrawCaptionButtons
					(graphics, rect, flags, (CaptionFlags)(~0));

				// Bail out if the rectangle is too small for the text.
				if(rect.width <= 2)
				{
					return;
				}

				// Position the caption text.
				Font font = GetCaptionFont();
				FontExtents extents = font.GetFontExtents(graphics);
				int textY = (rect.height - extents.Ascent) / 2;
				textY += rect.y + extents.Ascent;

				// Draw the caption text, clipped to the caption area
				// so that it won't overwrite the buttons on the right.
				using(Region region = new Region(graphics.ExposeRegion))
				{
					region.Intersect(rect);
					graphics.SetClipRegion(region);
					graphics.Foreground = foreground;
					graphics.DrawString(rect.x + 2, textY, child.Name, font);
				}
			}

	// Change the state of a caption button.  Returns true if the
	// button was pressed before we changed its state.
	private bool ChangeButtonState(HitTest hitTest, bool pressed)
			{
				// Determine what change we need to apply.
				CaptionFlags buttonsToDraw = (CaptionFlags)0;
				CaptionFlags pressedState = (CaptionFlags)0;
				CaptionFlags origFlags = flags;
				switch(hitTest)
				{
					case HitTest.Close:
					{
						buttonsToDraw = CaptionFlags.HasClose;
						pressedState = CaptionFlags.ClosePressed;
					}
					break;

					case HitTest.Maximize:
					{
						buttonsToDraw = CaptionFlags.HasMaximize;
						pressedState = CaptionFlags.MaximizePressed;
					}
					break;

					case HitTest.Minimize:
					{
						buttonsToDraw = CaptionFlags.HasMinimize;
						pressedState = CaptionFlags.MinimizePressed;
					}
					break;

					case HitTest.Restore:
					{
						buttonsToDraw = CaptionFlags.HasRestore;
						pressedState = CaptionFlags.RestorePressed;
					}
					break;

					case HitTest.Help:
					{
						buttonsToDraw = CaptionFlags.HasHelp;
						pressedState = CaptionFlags.HelpPressed;
					}
					break;
				}
				if(pressed)
				{
					flags |= pressedState;
				}
				else
				{
					flags &= ~pressedState;
				}

				// Redraw the caption buttons to match the state change.
				if(flags != origFlags)
				{
					Rectangle rect = new Rectangle
						(FrameBorderSize, FrameBorderSize,
						 width - FrameBorderSize * 2,
						 captionHeight - FrameBorderSize);
					using(Graphics graphics = new Graphics(this))
					{
						DrawCaptionButtons
							(graphics, rect, flags, buttonsToDraw);
					}
				}
				return ((origFlags & pressedState) != 0);
			}

	// Handle a button press.
	protected void OnButtonPress(int x, int y, int x_root, int y_root,
								 ButtonName button, ModifierMask modifiers)
			{
				// Ignore button presses in a click mode because they
				// will be for something other than the select button.
				if(clickMode != HitTest.Outside)
				{
					return;
				}

				// Adjust the cursor to match the current position.
				HitTest hitTest = SetCursor(x, y);

				// Bail out if we are not processing the select button.
				if(!IsSelect(button))
				{
					return;
				}

				// Enter the appropriate click mode.
				if(hitTest != HitTest.Outside && hitTest != HitTest.Client)
				{
					clickMode = hitTest;
					startX = x;
					startY = y;
					startWidth = width;
					startHeight = height;
					startRootX = x_root;
					startRootY = y_root;
				}

				// Depress a button if one was pressed.
				switch(hitTest)
				{
					case HitTest.Close:
					case HitTest.Maximize:
					case HitTest.Minimize:
					case HitTest.Restore:
					case HitTest.Help:
					{
						ChangeButtonState(hitTest, true);
					}
					break;
				}
			}

	// Handle a button release.
	protected void OnButtonRelease(int x, int y, int x_root, int y_root,
								   ButtonName button, ModifierMask modifiers)
			{
				// Set the cursor and bail out if we aren't in a click mode.
				if(clickMode == HitTest.Outside)
				{
					SetCursor(x, y);
					return;
				}

				// Bail out if it wasn't the select button that was released.
				if(!IsSelect(button))
				{
					return;
				}

				// Get the area currently occupied by the cursor.
				HitTest hitTest = PerformHitTest(x, y);

				// Process the button release according to the click mode.
				switch(clickMode)
				{
					case HitTest.Close:
					case HitTest.Maximize:
					case HitTest.Minimize:
					case HitTest.Restore:
					case HitTest.Help:
					{
						if(ChangeButtonState(clickMode, false) &&
						   clickMode == hitTest)
						{
							// A caption button has been clicked.
							switch(clickMode)
							{
								case HitTest.Close:
									child.Close();
									break;
								case HitTest.Maximize:
									if(child.IsIconic)
									{
										child.Deiconify();
									}
									child.Maximize();
									break;
								case HitTest.Minimize:
									child.Iconify();
									break;
								case HitTest.Restore:
									if(child.IsIconic)
									{
										child.Deiconify();
									}
									else
									{
										child.Restore();
									}
									break;
								case HitTest.Help:
									child.Help();
									break;
							}
						}
					}
					break;
				}

				// Exit the click mode and return to normal.
				clickMode = HitTest.Outside;
				SetCursor(x, y);
			}

	// Handle a button double-click.
	protected void OnButtonDoubleClick
				(int x, int y, int x_root, int y_root,
				 ButtonName button, ModifierMask modifiers)
			{
				HitTest hitTest = SetCursor(x, y);
				if(IsSelect(button))
				{
					if(hitTest == HitTest.Caption)
					{
						// Double-clicking the caption de-iconifies, maximizes,
						// or restores.
						if(child.IsIconic)
						{
							child.Deiconify();
						}
						else if((flags & CaptionFlags.HasMaximize) != 0)
						{
							child.Maximize();
						}
						else if((flags & CaptionFlags.HasRestore) != 0)
						{
							child.Restore();
						}
					}
					else
					{
						// Double-click elsewhere is the same as single-click.
						OnButtonPress(x, y, x_root, y_root, button, modifiers);
					}
				}
			}

	// Handle pointer motion events.
	protected void OnPointerMotion
				(int x, int y, int x_root, int y_root, ModifierMask modifiers)
			{
				// Set the cursor and bail out if we aren't in a click mode.
				if(clickMode == HitTest.Outside)
				{
					SetCursor(x, y);
					return;
				}

				// Get the area currently occupied by the cursor.
				HitTest hitTest = PerformHitTest(x, y);

				// Get the root co-ordinates of the MDI client window
				// and restrict (x_root, y_root) to its boundaries.
				Widget mdiClient = Parent;
				int clientLeft = mdiClient.X;
				int clientTop = mdiClient.Y;
				Widget parent = mdiClient.Parent;
				while(parent != null)
				{
					clientLeft += parent.X;
					clientTop += parent.Y;
					parent = parent.Parent;
				}
				int clientRight = clientLeft + mdiClient.Width;
				int clientBottom = clientTop + mdiClient.Height;
				if(x_root < clientLeft)
				{
					x_root = clientLeft;
				}
				else if(x_root >= clientRight)
				{
					x_root = clientRight - 1;
				}
				if(y_root < clientTop)
				{
					y_root = clientTop;
				}
				else if(y_root >= clientBottom)
				{
					y_root = clientBottom - 1;
				}

				// Get the amount by which the mouse pointer has moved
				// since we started the drag operation.
				int diffx = x_root - startRootX;
				int diffy = y_root - startRootY;

				// Get the root co-ordinates of the window pre-drag.
				int left, top, right, bottom;
				left = startRootX - startX;
				top = startRootY - startY;
				right = left + startWidth;
				bottom = top + startHeight;

				// Process the pointer motion according to the click mode.
				switch(clickMode)
				{
					case HitTest.TopLeft:
					{
						left += diffx;
						top += diffy;
						if((right - left) < MinWidth)
						{
							left = right - MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							left = right - MaxWidth;
						}
						if((bottom - top) < MinHeight)
						{
							top = bottom - MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							top = bottom - MaxHeight;
						}
					}
					break;

					case HitTest.TopRight:
					{
						right += diffx;
						top += diffy;
						if((right - left) < MinWidth)
						{
							right = left + MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							right = left + MaxWidth;
						}
						if((bottom - top) < MinHeight)
						{
							top = bottom - MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							top = bottom - MaxHeight;
						}
					}
					break;

					case HitTest.BottomLeft:
					{
						left += diffx;
						bottom += diffy;
						if((right - left) < MinWidth)
						{
							left = right - MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							left = right - MaxWidth;
						}
						if((bottom - top) < MinHeight)
						{
							bottom = top + MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							bottom = top + MaxHeight;
						}
					}
					break;

					case HitTest.BottomRight:
					{
						right += diffx;
						bottom += diffy;
						if((right - left) < MinWidth)
						{
							right = left + MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							right = left + MaxWidth;
						}
						if((bottom - top) < MinHeight)
						{
							bottom = top + MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							bottom = top + MaxHeight;
						}
					}
					break;

					case HitTest.Top:
					{
						top += diffy;
						if((bottom - top) < MinHeight)
						{
							top = bottom - MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							top = bottom - MaxHeight;
						}
					}
					break;

					case HitTest.Bottom:
					{
						bottom += diffy;
						if((bottom - top) < MinHeight)
						{
							bottom = top + MinHeight;
						}
						else if((bottom - top) > MaxHeight)
						{
							bottom = top + MaxHeight;
						}
					}
					break;

					case HitTest.Left:
					{
						left += diffx;
						if((right - left) < MinWidth)
						{
							left = right - MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							left = right - MaxWidth;
						}
					}
					break;

					case HitTest.Right:
					{
						right += diffx;
						if((right - left) < MinWidth)
						{
							right = left + MinWidth;
						}
						else if((right - left) > MaxWidth)
						{
							right = left + MaxWidth;
						}
					}
					break;

					case HitTest.Close:
					case HitTest.Maximize:
					case HitTest.Minimize:
					case HitTest.Restore:
					case HitTest.Help:
					{
						// Change the button state if we have left or
						// re-entered the caption button we started with.
						ChangeButtonState(clickMode, clickMode == hitTest);
					}
					return;

					case HitTest.Caption:
					{
						// If the mouse has moved sufficiently away from
						// the starting position, then switch into move mode.
						if(diffx < -4 || diffx > 4 || diffy < -4 || diffy > 4)
						{
							// We cannot enter move mode if maximized.
							if(!(child.IsMaximized))
							{
								clickMode = HitTest.Move;
								goto case HitTest.Move;
							}
						}
					}
					return;

					case HitTest.Move:
					{
						// Move the window to a new location.
						left += diffx;
						right += diffx;
						top += diffy;
						bottom += diffy;
					}
					break;
				}

				// Convert root co-ordinates back into normal co-ordinates.
				left -= clientLeft;
				right -= clientLeft;
				top -= clientTop;
				bottom -= clientTop;

				// Move the window to its new location.
				MoveResize(left, top, right - left, bottom - top);
			}

	// Handle pointer enters.
	protected override void OnEnter(Widget child, int x, int y,
								   ModifierMask modifiers,
								   CrossingMode mode,
								   CrossingDetail detail)
			{
				if(clickMode == HitTest.Outside)
				{
					SetCursor(x, y);
				}
			}

	// Handle pointer leaves.
	protected override void OnLeave(Widget child, int x, int y,
								   ModifierMask modifiers,
								   CrossingMode mode,
								   CrossingDetail detail)
			{
				if(clickMode == HitTest.Outside)
				{
					Cursor = null;
				}
			}

	// Raise this widget.
	public override void Raise()
			{
				base.Raise();
				SelectActive();
			}

	// Lower this widget.
	public override void Lower()
			{
				base.Lower();
				SelectActive();
			}

	// Determine if the child window is resizable.
	private bool IsResizable()
			{
				if(child.IsIconic)
				{
					return false;
				}
				else if((child.Functions & MotifFunctions.All) != 0)
				{
					return ((child.Functions & MotifFunctions.Resize) == 0);
				}
				else
				{
					return ((child.Functions & MotifFunctions.Resize) != 0);
				}
			}

	// Perform a hit test on a set of co-ordinates to determine
	// what part of the window the co-ordinates are over.
	private HitTest PerformHitTest(int x, int y)
			{
				// Check for co-ordinates that are completely outside.
				if(x < 0 || x >= width || y < 0 || y >= height)
				{
					return HitTest.Outside;
				}

				// Hit-test the borders.
				if(x < FrameBorderSize)
				{
					if(!IsResizable())
					{
						return HitTest.Outside;
					}
					else if(y < CornerSize)
					{
						return HitTest.TopLeft;
					}
					else if(y >= (height - CornerSize))
					{
						return HitTest.BottomLeft;
					}
					else
					{
						return HitTest.Left;
					}
				}
				else if(x >= (width - FrameBorderSize))
				{
					if(!IsResizable())
					{
						return HitTest.Outside;
					}
					else if(y < CornerSize)
					{
						return HitTest.TopRight;
					}
					else if(y >= (height - CornerSize))
					{
						return HitTest.BottomRight;
					}
					else
					{
						return HitTest.Right;
					}
				}
				else if(y < FrameBorderSize)
				{
					if(!IsResizable())
					{
						return HitTest.Outside;
					}
					else if(x < CornerSize)
					{
						return HitTest.TopLeft;
					}
					else if(x >= (width - CornerSize))
					{
						return HitTest.TopRight;
					}
					else
					{
						return HitTest.Top;
					}
				}
				else if(y >= (height - FrameBorderSize))
				{
					if(!IsResizable())
					{
						return HitTest.Outside;
					}
					else if(x < CornerSize)
					{
						return HitTest.BottomLeft;
					}
					else if(x >= (width - CornerSize))
					{
						return HitTest.BottomRight;
					}
					else
					{
						return HitTest.Bottom;
					}
				}

				// Bail out if we are obviously in the client area.
				if(y >= captionHeight)
				{
					return HitTest.Client;
				}

				// See if we are over one of the caption buttons.
				int buttonSize = (captionHeight - FrameBorderSize - 4);
				int right = width - FrameBorderSize - 2;
				if(y < (FrameBorderSize + 2) ||
				   y >= (FrameBorderSize + 2 + buttonSize))
				{
					return HitTest.Caption;
				}
				if((flags & CaptionFlags.HasClose) != 0)
				{
					if(x < right && x >= (right - buttonSize))
					{
						return HitTest.Close;
					}
					right -= buttonSize;
					if((flags & (CaptionFlags.HasMaximize |
								 CaptionFlags.HasRestore |
								 CaptionFlags.HasMinimize)) != 0)
					{
						// Leave a gap between the close button and the others.
						right -= 2;
					}
				}
				if((flags & CaptionFlags.HasMaximize) != 0)
				{
					if(x < right && x >= (right - buttonSize))
					{
						return HitTest.Maximize;
					}
					right -= buttonSize;
				}
				if((flags & CaptionFlags.HasRestore) != 0)
				{
					if(x < right && x >= (right - buttonSize))
					{
						return HitTest.Restore;
					}
					right -= buttonSize;
				}
				if((flags & CaptionFlags.HasMinimize) != 0)
				{
					if(x < right && x >= (right - buttonSize))
					{
						return HitTest.Minimize;
					}
					right -= buttonSize;
				}
				if((flags & CaptionFlags.HasHelp) != 0)
				{
					// Leave a gap between the help button and the others.
					right -= 2;
					if(x < right && x >= (right - buttonSize))
					{
						return HitTest.Help;
					}
				}

				// Whatever is left must be the caption.
				return HitTest.Caption;
			}

	// Perform a hit test and set the cursor to the appropriate value.
	private HitTest SetCursor(int x, int y)
			{
				HitTest test = PerformHitTest(x, y);
				CursorType cursor;
				switch(test)
				{
					case HitTest.TopLeft:
						cursor = CursorType.XC_top_left_corner;
						break;
					case HitTest.TopRight:
						cursor = CursorType.XC_top_right_corner;
						break;
					case HitTest.BottomLeft:
						cursor = CursorType.XC_bottom_left_corner;
						break;
					case HitTest.BottomRight:
						cursor = CursorType.XC_bottom_right_corner;
						break;
					case HitTest.Top:
						cursor = CursorType.XC_top_side;
						break;
					case HitTest.Bottom:
						cursor = CursorType.XC_bottom_side;
						break;
					case HitTest.Left:
						cursor = CursorType.XC_left_side;
						break;
					case HitTest.Right:
						cursor = CursorType.XC_right_side;
						break;
					default:
						cursor = CursorType.XC_inherit_parent;
						break;
				}
				Cursor = new Cursor(cursor);
				return test;
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				ButtonName button;
				XTime time;
	
				switch((EventType)(xevent.xany.type__))
				{
					case EventType.ButtonPress:
					{
						// If we have a passive grab in force, then raise
						// this window and replay the event normally.
						if((flags & CaptionFlags.Grabbed) != 0)
						{
							Raise();
							try
							{
								IntPtr display = dpy.Lock();
								Xlib.XAllowEvents
									(display, 2 /* ReplayPointer */,
									 dpy.knownEventTime);
							}
							finally
							{
								dpy.Unlock();
							}
							return;
						}

						// Process button events.
						button = xevent.xbutton.button;
						time = xevent.xbutton.time;
						if(lastClickButton == button &&
						   lastClickTime != XTime.CurrentTime &&
						   (time - lastClickTime) < 500)
						{
							OnButtonDoubleClick(xevent.xbutton.x,
								        		xevent.xbutton.y,
												xevent.xbutton.x_root,
												xevent.xbutton.y_root,
												button, xevent.xbutton.state);
							time = XTime.CurrentTime;
						}
						else
						{
							OnButtonPress(xevent.xbutton.x,
								  		  xevent.xbutton.y,
								  		  xevent.xbutton.x_root,
								  		  xevent.xbutton.y_root,
										  button, xevent.xbutton.state);
						}
						lastClickTime = time;
						lastClickButton = button;
					}
					break;

					case EventType.ButtonRelease:
					{
						// Dispatch a button release event.
						button = xevent.xbutton.button;
						OnButtonRelease(xevent.xbutton.x,
										xevent.xbutton.y,
										xevent.xbutton.x_root,
										xevent.xbutton.y_root,
										button, xevent.xbutton.state);
					}
					break;

					case EventType.MotionNotify:
					{
						// Dispatch a pointer motion event.
						OnPointerMotion(xevent.xmotion.x,
								   	    xevent.xmotion.y,
								   	    xevent.xmotion.x_root,
								   	    xevent.xmotion.y_root,
								   	    xevent.xmotion.state);
					}
					break;

					default:
					{
						base.DispatchEvent(ref xevent);
					}
					break;
				}
			}

	// Determine if the child window has a particular Motif function.
	private bool HasFunction(MotifFunctions function)
			{
				if((child.Functions & MotifFunctions.All) != 0)
				{
					return ((child.Functions & function) == 0);
				}
				else
				{
					return ((child.Functions & function) != 0);
				}
			}

	// Update the window decorations to match the values on the child.
	private void UpdateDecorations()
			{
				CaptionFlags addFlags = (CaptionFlags)0;
				CaptionFlags removeFlags = (CaptionFlags)0;
				CaptionFlags newFlags;
				if(HasFunction(MotifFunctions.Close))
				{
					addFlags |= CaptionFlags.HasClose;
				}
				else
				{
					removeFlags |= CaptionFlags.HasClose |
								   CaptionFlags.ClosePressed;
				}
				if(!(child.IsIconic))
				{
					if(HasFunction(MotifFunctions.Resize) &&
					   HasFunction(MotifFunctions.Maximize))
					{
						if(child.IsMaximized)
						{
							addFlags |= CaptionFlags.HasRestore;
							removeFlags |= CaptionFlags.HasMaximize |
										   CaptionFlags.MaximizePressed;
						}
						else
						{
							addFlags |= CaptionFlags.HasMaximize;
							removeFlags |= CaptionFlags.HasRestore |
										   CaptionFlags.RestorePressed;
						}
					}
					else
					{
						removeFlags |= CaptionFlags.HasMaximize |
									   CaptionFlags.MaximizePressed |
									   CaptionFlags.HasRestore |
									   CaptionFlags.RestorePressed;
					}
					if(HasFunction(MotifFunctions.Minimize))
					{
						addFlags |= CaptionFlags.HasMinimize;
					}
					else
					{
						removeFlags |= CaptionFlags.HasMinimize |
									   CaptionFlags.MinimizePressed;
					}
				}
				else
				{
					if(HasFunction(MotifFunctions.Maximize))
					{
						addFlags |= CaptionFlags.HasRestore |
									CaptionFlags.HasMaximize;
						removeFlags |= CaptionFlags.HasMinimize |
									   CaptionFlags.MinimizePressed;
					}
					else
					{
						addFlags |= CaptionFlags.HasRestore;
						removeFlags |= CaptionFlags.HasMaximize |
									   CaptionFlags.MaximizePressed |
									   CaptionFlags.HasMinimize |
									   CaptionFlags.MinimizePressed;
					}
				}
				if((child.OtherHints & OtherHints.HelpButton) != 0)
				{
					addFlags |= CaptionFlags.HasHelp;
				}
				else
				{
					removeFlags |= CaptionFlags.HasHelp |
								   CaptionFlags.HelpPressed;
				}
				newFlags = ((flags & ~removeFlags) | addFlags);
				if(newFlags != flags)
				{
					flags = newFlags;
					Repaint();
				}
			}

	// Select an active caption widget in the MDI client.
	private void SelectActive()
			{
				((MdiClientWidget)Parent).SelectActive();
			}

	// Process an operation that was passed up from an MDI child to its
	// surrounding caption widget.  Returns true if the operation should
	// be passed onto the ordinary window manager.
	internal bool CaptionOperation(Operation operation)
			{
				IntPtr display;
				switch(operation)
				{
					case Operation.Destroy:
					{
						// Unmap and destroy this caption widget.  This will
						// recursively destroy the child also.
						Unmap();
						SelectActive();
						Destroy();

						// Detach us from our parent because we don't want
						// the MDI client to know about us any more.
						Detach(true);
					}
					break;

					case Operation.FirstMap:
					case Operation.Map:
					{
						// Map the child window to the screen.
						try
						{
							display = dpy.Lock();
							Xlib.XMapWindow(display, child.GetWidgetHandle());
						}
						finally
						{
							dpy.Unlock();
						}
						Raise();
						Map();
						SelectActive();
					}
					break;

					case Operation.Unmap:
					{
						// Withdraw the child window from the screen.
						Unmap();
						try
						{
							display = dpy.Lock();
							Xlib.XUnmapWindow(display, child.GetWidgetHandle());
						}
						finally
						{
							dpy.Unlock();
						}
						SelectActive();
					}
					break;

					case Operation.Raise:
					{
						Raise();
					}
					break;

					case Operation.Lower:
					{
						Lower();
					}
					break;

					case Operation.Iconify:
					{
						MinimizeChild();
					}
					break;

					case Operation.Deiconify:
					{
						UnminimizeChild();
					}
					break;

					case Operation.Maximize:
					{
						((MdiClientWidget)Parent).MaximizeAll(this);
					}
					break;

					case Operation.Restore:
					{
						((MdiClientWidget)Parent).RestoreAll(this);
					}
					break;

					case Operation.Title:
					{
						Repaint();
					}
					break;

					case Operation.Decorations:
					{
						UpdateDecorations();
					}
					break;

					case Operation.SetMinimumSize: break;
					case Operation.SetMaximumSize: break;
					case Operation.SetIcon: break;

					default: return true;
				}
				return false;
			}

	// Make this caption widget inactive.
	internal void MakeInactive()
			{
				// Add a passive grab so that we can intercept button
				// press events before the child window gets them
				// and cause the window to be raised first.
				if((flags & CaptionFlags.Grabbed) == 0)
				{
					try
					{
						IntPtr display = dpy.Lock();
						Xlib.XGrabButton
							(display, 0 /* AnyButton */,
							 (1 << 15) /* AnyModifier */,
							 GetWidgetHandle(), XBool.False,
							 (uint)(EventMask.ButtonPressMask),
							 0 /* GrabModeSync */, 1 /* GrabModeAsync */,
							 XWindow.Zero, XCursor.Zero);
					}
					finally
					{
						dpy.Unlock();
					}
					flags |= CaptionFlags.Grabbed;
				}

				// Change the visual aspect of this window to inactive.
				flags &= ~CaptionFlags.Active;
				if(gradient != null)
				{
					gradient.Dispose();
					gradient = null;
				}
				Repaint();
			}

	// Make this caption widget active.
	internal void MakeActive()
			{
				// If we have a passive grab active, then remove it.
				if((flags & CaptionFlags.Grabbed) != 0)
				{
					try
					{
						IntPtr display = dpy.Lock();
						Xlib.XUngrabButton
							(display, 0 /* AnyButton */,
							 (1 << 15) /* AnyModifier */,
							 GetWidgetHandle());

						// We might have a frozen mouse pointer, so allow
						// events to proceed just in case.
						Xlib.XAllowEvents
							(display, 2 /* ReplayPointer */,
							 dpy.knownEventTime);
					}
					finally
					{
						dpy.Unlock();
					}
					flags &= ~CaptionFlags.Grabbed;
				}

				// Change the visual aspect of this window to active.
				flags |= CaptionFlags.Active;
				if(gradient != null)
				{
					gradient.Dispose();
					gradient = null;
				}
				Repaint();
			}

	// Process a minimize event on the child.
	private void MinimizeChild()
			{
				// Bail out if the child is already iconic.
				if(child.iconic)
				{
					return;
				}

				// Mark the child as iconic.
				child.iconic = true;

				// Save the restore position if we aren't maximized.
				if(!(child.maximized))
				{
					restoreX = x;
					restoreY = y;
					restoreWidth = width;
					restoreHeight = height;
				}

				// Resize the caption widget to the minimized size.
				Rectangle rect = new Rectangle
					(0, 0, MinimizedWidth, captionHeight + FrameBorderSize);
				((MdiClientWidget)Parent).PlaceMinimizedRectangle
					(ref rect, this);
				MoveResize(rect.x, rect.y, rect.width, rect.height);

				// Unmap the child window.
				try
				{
					IntPtr display = dpy.Lock();
					Xlib.XUnmapWindow(display, child.GetWidgetHandle());
				}
				finally
				{
					dpy.Unlock();
				}

				// Lower this window and select a new active window.
				Lower();

				// Update the decorations to match the new flags.
				UpdateDecorations();
			}

	// Process an unminimize event on the child.
	private void UnminimizeChild()
			{
				// Bail out if the child is not iconic.
				if(!(child.iconic))
				{
					return;
				}

				// Mark the child as no longer iconic.
				child.iconic = false;

				// Map the child window.
				try
				{
					IntPtr display = dpy.Lock();
					Xlib.XMapWindow(display, child.GetWidgetHandle());
				}
				finally
				{
					dpy.Unlock();
				}

				// Resize the window to the previous location.
				if(child.maximized)
				{
					MaximizeChild
						(false, ((MdiClientWidget)Parent).HasControls());
				}
				else
				{
					MoveResize(restoreX, restoreY, restoreWidth, restoreHeight);
				}

				// Raise this window and select it as the new active window.
				Raise();

				// Update the decorations to match the new flags.
				UpdateDecorations();
			}

	// Maximize this child window.
	internal void MaximizeChild(bool notify, bool controls)
			{
				// Record the current window size if necessary.
				if(!(child.maximized) && !(child.iconic))
				{
					restoreX = x;
					restoreY = y;
					restoreWidth = width;
					restoreHeight = height;
				}

				// Resize the window to the full MDI client area.
				if(!(child.iconic))
				{
					int mdiWidth = Parent.Width;
					int mdiHeight = Parent.Height;
					if(controls)
					{
						// We have controls, so shift the borders out of view.
						MoveResize(-FrameBorderSize, -captionHeight,
								   mdiWidth + FrameBorderSize * 2,
								   mdiHeight + captionHeight + FrameBorderSize);
					}
					else
					{
						// Keep the caption area visible so that the user has
						// some way to perform window operations.
						MoveResize(-FrameBorderSize, -FrameBorderSize,
								   mdiWidth + FrameBorderSize * 2,
								   mdiHeight + FrameBorderSize * 2);
					}
				}

				// Update the decorations to match the maximized state.
				// Note: the child may already be maximized if we had to
				// "re-maximize" it after a resize on the MDI client window.
				if(!(child.maximized))
				{
					child.maximized = true;
					UpdateDecorations();
					if(notify)
					{
						child.MaximizeChanged();
					}
				}
			}

	// Restore this child window.
	internal void RestoreChild(bool notify)
			{
				// Resize the window back to its original size.
				if(!(child.iconic))
				{
					MoveResize(restoreX, restoreY, restoreWidth, restoreHeight);
				}

				// Update the decorations to match the restored state.
				if(child.maximized)
				{
					child.maximized = false;
					UpdateDecorations();
					if(notify)
					{
						child.MaximizeChanged();
					}
				}
			}

	// Get the minimum width and height for this window.
	private int MinWidth
			{
				get
				{
					int value = child.minWidth + FrameBorderSize * 2;
					if(value < AbsoluteMinWidth)
					{
						value = AbsoluteMinWidth;
					}
					return value;
				}
			}
	private int MinHeight
			{
				get
				{
					int value = child.minHeight + captionHeight +
								FrameBorderSize;
					if(value < AbsoluteMinHeight)
					{
						value = AbsoluteMinHeight;
					}
					return value;
				}
			}

	// Get the maximum width and height for this window.
	private int MaxWidth
			{
				get
				{
					int value = child.maxWidth;
					if(value != 0)
					{
						return value + FrameBorderSize * 2;
					}
					else
					{
						return 32767;
					}
				}
			}
	private int MaxHeight
			{
				get
				{
					int value = child.maxHeight;
					if(value != 0)
					{
						return value + captionHeight + FrameBorderSize;
					}
					else
					{
						return 32767;
					}
				}
			}

	// Adjust the child window to match the parent.
	public override void MoveResize(int x, int y, int width, int height)
			{
				flags |= CaptionFlags.InMoveResize;
				try
				{
					base.MoveResize(x, y, width, height);
					if(!(child.iconic))
					{
						// Transmit the request to the X server.
						try
						{
							IntPtr display = dpy.Lock();
							Xlib.XMoveResizeWindow
								(display, child.GetWidgetHandle(),
								 FrameBorderSize, captionHeight,
								 (uint)(width - FrameBorderSize * 2),
								 (uint)(height - FrameBorderSize
								 			- captionHeight));
						}
						finally
						{
							dpy.Unlock();
						}
	
						// Transmit "OnMove" and "OnResize" events to the child.
						child.HandleMoveResize
							(x + FrameBorderSize, y + captionHeight,
							 width - FrameBorderSize * 2,
							 height - FrameBorderSize - captionHeight);
					}
				}
				finally
				{
					flags &= ~(CaptionFlags.InMoveResize);
				}
			}

	// Perform a move/resize operation that was requested by the child.
	internal void PerformChildResize(int x, int y, int width, int height)
			{
				if((flags & CaptionFlags.InMoveResize) != 0)
				{
					// Ignore the request if we are recursively re-entered.
					// This can happen when a user-initiatied resize causes
					// events to be emitted and another resize occurs.
					return;
				}
				if(child.iconic)
				{
					// Adjust the restored size, but leave the current alone.
					restoreX = x - FrameBorderSize;
					restoreY = y - captionHeight;
					restoreWidth = width + FrameBorderSize * 2;
					restoreHeight = height + captionHeight + FrameBorderSize;
				}
				else if(!(child.maximized))
				{
					// Change the current size to match the request.
					MoveResize(x - FrameBorderSize, y - captionHeight,
					           width + FrameBorderSize * 2,
							   height + captionHeight + FrameBorderSize);
				}
			}

} // class CaptionWidget

} // namespace Xsharp
