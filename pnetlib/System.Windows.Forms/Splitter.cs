/*
 * Splitter.cs - Implementation of the
 *			"System.Windows.Forms.Splitter" class.
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

namespace System.Windows.Forms
{

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[DefaultEvent("SplitterMoved")]
[DefaultProperty("Dock")]
[Designer("System.Windows.Forms.Design.SplitterDesigner, System.Design")]
#endif
public class Splitter : Control, IMessageFilter
{
	// Internal state.
	private int minSize;
	private int minExtra;
	private int thickness;
	private bool moving;
	private bool drawn;
	private Brush xorBrush;
	private Rectangle drawnRect;
	private int moveX, moveY;
	private int startMoveX, startMoveY;
	private int startX, startY;
	private int margin1, margin2;

	// Constructor.
	public Splitter()
			{
				minSize = 25;
				minExtra = 25;
				thickness = 3;
				moving = false;
				drawn = false;
				xorBrush = new HatchBrush(HatchStyle.Percent50,
										  Color.White, Color.Black);
				xorBrush = ToolkitManager.CreateXorBrush(xorBrush);
				Dock = DockStyle.Left;
				TabStop = false;
				SetStyle(ControlStyles.Selectable, false);
			}

	// Get or set this control's properties.
	public override bool AllowDrop
			{
				get
				{
					return base.AllowDrop;
				}
				set
				{
					base.AllowDrop = value;
				}
			}
	public override AnchorStyles Anchor
			{
				get
				{
					return AnchorStyles.None;
				}
				set
				{
					// Nothing to do here: anchoring is not supported.
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
	public BorderStyle BorderStyle
			{
				get
				{
					return BorderStyleInternal;
				}
				set
				{
					BorderStyleInternal = value;
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
					return new Size(3, 3);
				}
			}
	public override Cursor Cursor
		{
			set
			{
				if( value == null ) {
					switch( Dock ) {
						case DockStyle.Left : case DockStyle.Right :
							base.Cursor = Cursors.SizeWE;
							break;
						case DockStyle.Top : case DockStyle.Bottom :
							base.Cursor = Cursors.SizeNS;
							break;
						default:
							base.Cursor = value;
							break;
					}
				}
				else {
					base.Cursor = value;
				}
			}
		}
			
	public override DockStyle Dock
			{
				get
				{
					return base.Dock;
				}
				set
				{
					if(value == DockStyle.Left || value == DockStyle.Right)
					{
						base.Dock = value;
						Width = thickness;
						Cursor = Cursors.SizeWE;
					}
					else if(value == DockStyle.Top || value == DockStyle.Bottom)
					{
						base.Dock = value;
						Height = thickness;
						Cursor = Cursors.SizeNS;
					}
					else
					{
						throw new ArgumentException(S._("SWF_DockStyle"));
					}
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
					return base.ForeColor;
				}
				set
				{
					base.ForeColor = value;
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
	public int MinExtra
			{
				get
				{
					return minExtra;
				}
				set
				{
					if(minExtra != value)
					{
						if(value < 0)
						{
							value = 0;
						}
						minExtra = value;
					}
				}
			}
	public int MinSize
			{
				get
				{
					return minSize;
				}
				set
				{
					if(minSize != value)
					{
						if(value < 0)
						{
							value = 0;
						}
						minSize = value;
					}
				}
			}
	public int SplitPosition
			{
				get
				{
					Control adjusted = GetAdjustedControl();
					if(adjusted != null)
					{
						int size;
						switch(Dock)
						{
							case DockStyle.Left:
							case DockStyle.Right:
							default:
							{
								size = adjusted.Width;
							}
							break;

							case DockStyle.Top:
							case DockStyle.Bottom:
							{
								size = adjusted.Height;
							}
							break;
						}
						if(size < minSize)
						{
							size = minSize;
						}
						return size;
					}
					else
					{
						return -1;
					}
				}
				set
				{
					// Bail out if we are in the middle of a move operation.
					if(moving)
					{
						return;
					}

					// Get the control to be adjusted.
					Control adjusted = GetAdjustedControl();
					if(adjusted == null)
					{
						return;
					}

					// Range-check the size.
					if(value < minSize)
					{
						value = minSize;
					}

					// Adjust the bounds.
					switch(Dock)
					{
						case DockStyle.Left:
						case DockStyle.Right:
						{
							if(adjusted.Width == value)
							{
								return;
							}
							adjusted.Width = value;
						}
						break;

						case DockStyle.Top:
						case DockStyle.Bottom:
						{
							if(adjusted.Height == value)
							{
								return;
							}
							adjusted.Height = value;
						}
						break;
					}

					// Notify event handlers of the move.
					OnSplitterMoved
						(new SplitterEventArgs(Left, Top, Left, Top));
				}
			}
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
	public override String Text
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

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", MinExtra=" + MinExtra.ToString() +
					   ", MinSize=" + MinSize.ToString();
			}

	// Filter a message before it is dispatched.
	public bool PreFilterMessage(ref Message m)
			{
				// Not used in this implementation.
				return false;
			}

	// Event that is emitted when the splitter is moved.
	public event SplitterEventHandler SplitterMoved
			{
				add
				{
					AddHandler(EventId.SplitterMoved, value);
				}
				remove
				{
					RemoveHandler(EventId.SplitterMoved, value);
				}
			}

	// Event that is emitted when the splitter is moving.
	public event SplitterEventHandler SplitterMoving
			{
				add
				{
					AddHandler(EventId.SplitterMoving, value);
				}
				remove
				{
					RemoveHandler(EventId.SplitterMoving, value);
				}
			}

	// Draw the splitter at its current move position.
	private void DrawSplitter()
			{
				if(!drawn)
				{
					switch(Dock)
					{
						case DockStyle.Left:
						case DockStyle.Right:
						{
							drawnRect = new Rectangle
								(moveX, Top, thickness, Height);
						}
						break;

						case DockStyle.Top:
						case DockStyle.Bottom:
						{
							drawnRect = new Rectangle
								(Left, moveY, Width, thickness);
						}
						break;
					}
					using(Graphics graphics = Parent.CreateGraphics())
					{
						graphics.FillRectangle(xorBrush, drawnRect);
					}
					drawn = true;
				}
			}

	// Erase the splitter from its current move position.
	private void EraseSplitter()
			{
				if(drawn)
				{
					drawn = false;
					using(Graphics graphics = Parent.CreateGraphics())
					{
						graphics.FillRectangle(xorBrush, drawnRect);
					}
				}
			}

	// End a splitter move operation.
	private void EndMove()
			{
				// Erase the splitter and end the move operation.
				EraseSplitter();
				moving = false;
				Capture = false;

				// Actually move the position.
				switch(Dock)
				{
					case DockStyle.Left:
					{
						SplitPosition = moveX - margin1 + minSize;
					}
					break;

					case DockStyle.Right:
					{
						SplitPosition =
							Parent.ClientSize.Width -
								(moveX + thickness) -
								margin2 + minSize;
					}
					break;

					case DockStyle.Top:
					{
						SplitPosition = moveY - margin1 + minSize;
					}
					break;

					case DockStyle.Bottom:
					{
						SplitPosition =
							Parent.ClientSize.Height -
								(moveY + thickness) -
								margin2 + minSize;
					}
					break;
				}
			}

	// Handle a "KeyDown" event.
	protected override void OnKeyDown(KeyEventArgs e)
			{
				base.OnKeyDown(e);
				if(moving && e.KeyCode == Keys.Escape)
				{
					EndMove();
				}
			}

	// Handle a "MouseDown" event.
	protected override void OnMouseDown(MouseEventArgs e)
			{
				base.OnMouseDown(e);
				if(!moving && e.Button == MouseButtons.Left)
				{
					// Bail out if there is no control to adjust.
					Control adjusted = GetAdjustedControl();
					if(adjusted == null)
					{
						return;
					}

					// Record the starting position so that we know
					// how much to offset by when determining movements.
					startX = e.X;
					startY = e.Y;

					// Set the initial move position.
					moveX = Left;
					moveY = Top;
					startMoveX = moveX;
					startMoveY = moveY;

					// Determine the margins of the splitter move.
					switch(Dock)
					{
						case DockStyle.Left:
						{
							margin1 = adjusted.Left + minSize;
							margin2 = minExtra;
						}
						break;

						case DockStyle.Right:
						{
							margin1 = minExtra;
							margin2 = (Parent.ClientSize.Width -
									   adjusted.Right + minSize);
						}
						break;

						case DockStyle.Top:
						{
							margin1 = adjusted.Top + minSize;
							margin2 = minExtra;
						}
						break;

						case DockStyle.Bottom:
						{
							margin1 = minExtra;
							margin2 = (Parent.ClientSize.Height -
									   adjusted.Bottom + minSize);
						}
						break;
					}

					// Capture the mouse and draw the splitter.
					Capture = true;
					moving = true;
					DrawSplitter();
				}
			}

	// Handle a "MouseMove" event.
	protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
				if(moving)
				{
					int diffX = e.X - startX;
					int diffY = e.Y - startY;
					int newX = moveX;
					int newY = moveY;
					Size parentSize = Parent.ClientSize;
					switch(Dock)
					{
						case DockStyle.Left:
						case DockStyle.Right:
						{
							newX = startMoveX + diffX;
							if(newX < margin1)
							{
								newX = margin1;
							}
							else if((newX + thickness) >
										(parentSize.Width - margin2))
							{
								newX = parentSize.Width - margin2 - thickness;
							}
						}
						break;

						case DockStyle.Top:
						case DockStyle.Bottom:
						{
							newY = startMoveY + diffY;
							if(newY < margin1)
							{
								newY = margin1;
							}
							else if((newY + thickness) >
										(parentSize.Height - margin2))
							{
								newY = parentSize.Height - margin2 - thickness;
							}
						}
						break;
					}
					if(newX != moveX || newY != moveY)
					{
						// Redraw the splitter in its new position.
						EraseSplitter();
						moveX = newX;
						moveY = newY;
						DrawSplitter();

						// Emit the "SplitterMoving" event.
						OnSplitterMoving
							(new SplitterEventArgs
								(Left + e.X, Top + e.Y, moveX, moveY));
					}
				}
			}

	// Handle a "MouseUp" event.
	protected override void OnMouseUp(MouseEventArgs e)
			{
				base.OnMouseUp(e);
				if(moving && e.Button == MouseButtons.Left)
				{
					EndMove();
				}
			}

	// Emit the "SplitterMoved" event.
	protected virtual void OnSplitterMoved(SplitterEventArgs e)
			{
				SplitterEventHandler handler;
				handler = (SplitterEventHandler)
					(GetHandler(EventId.SplitterMoved));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Emit the "SplitterMoving" event.
	protected virtual void OnSplitterMoving(SplitterEventArgs e)
			{
				SplitterEventHandler handler;
				handler = (SplitterEventHandler)
					(GetHandler(EventId.SplitterMoving));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Inner core of "SetBounds".
	protected override void SetBoundsCore
				(int x, int y, int width, int height,
				 BoundsSpecified specified)
			{
				// Set the thickness based on the width or height.
				DockStyle dock = Dock;
				if(dock == DockStyle.Left || dock == DockStyle.Right)
				{
					if(width <= 0)
					{
						width = 3;
					}
					thickness = width;
				}
				else
				{
					if(height <= 0)
					{
						height = 3;
					}
					thickness = height;
				}

				// Set the bounds.
				base.SetBoundsCore(x, y, width, height, specified);
			}

	// Get the control that is being adjusted by this splitter.
	// Returns null if we cannot find an appropriate control.
	private Control GetAdjustedControl()
			{
				// No adjusted control if we don't have a parent.
				Control parent = Parent;
				if(parent == null)
				{
					return null;
				}

				// Look for a control that has a side adjoining this one.
				DockStyle dock = Dock;
				foreach(Control control in parent.Controls)
				{
					if(!control.Visible)
					{
						continue;
					}
					switch(dock)
					{
						case DockStyle.Left:
						{
							if(control.Right == Left)
							{
								return control;
							}
						}
						break;

						case DockStyle.Right:
						{
							if(control.Left == Right)
							{
								return control;
							}
						}
						break;

						case DockStyle.Top:
						{
							if(control.Bottom == Top)
							{
								return control;
							}
						}
						break;

						case DockStyle.Bottom:
						{
							if(control.Top == Bottom)
							{
								return control;
							}
						}
						break;
					}
				}

				// We could not find an appropriate control.
				return null;
			}

}; // class Splitter

}; // namespace System.Windows.Forms
