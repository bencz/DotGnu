/*
 * Label.cs - Implementation of the
 *			"System.Windows.Forms.Label" class.
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
using System.Drawing.Text;

public class Label : Control
{
	// Internal state.
	private bool autoSize;
	private bool useMnemonic;
	private bool renderTransparent;
	private FlatStyle flatStyle;
	private ContentAlignment alignment;
	private Image image;
	private ContentAlignment imageAlign;
	private int imageIndex;
	private ImageList imageList;
	private int preferredWidth;

	// Contructor.
	public Label()
			{
				SetStyle(ControlStyles.ResizeRedraw, true);
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				SetStyle(ControlStyles.Selectable, false);

				this.useMnemonic = true;
				this.flatStyle = FlatStyle.Standard;
				this.alignment = ContentAlignment.TopLeft;
				this.preferredWidth = -1;
				this.imageAlign = ContentAlignment.MiddleCenter;
				TabStop = false;
			}

	// Get or set this label's properties.
	public virtual bool AutoSize
			{
				get
				{
					return autoSize;
				}
				set
				{
					if(autoSize != value)
					{
						autoSize = value;
						OnAutoSizeChanged(EventArgs.Empty);
					}
				}
			}
	public virtual BorderStyle BorderStyle
			{
				get
				{
					return BorderStyleInternal;
				}
				set
				{
					if(BorderStyleInternal != value)
					{
						BorderStyleInternal = value;
						Invalidate();
					}
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
					if(AutoSize)
					{
						// The preferred width will be based on the text,
						// but when "DefaultSize" is called from the "Label"
						// constructor, the text isn't set yet.  So we
						// default to 100 for the width and use the font
						// for the preferred height.
						return new Size(100, PreferredHeight);
					}
					else
					{
						return new Size(100, 23);
					}
				}
			}
	public FlatStyle FlatStyle
			{
				get
				{
					return flatStyle;
				}
				set
				{
					if(flatStyle != value)
					{
						flatStyle = value;
						Recalculate();
					}
				}
			}
	public Image Image
			{
				get
				{
					return image;
				}
				set
				{
					if(image != value)
					{
						image = value;
						Recalculate();
					}
				}
			}
	public ContentAlignment ImageAlign
			{
				get
				{
					return imageAlign;
				}
				set
				{
					if(imageAlign != value)
					{
						imageAlign = value;
						Recalculate();
					}
				}
			}
	public int ImageIndex
			{
				get
				{
					return imageIndex;
				}
				set
				{
					if(imageIndex != value)
					{
						imageIndex = value;
						Recalculate();
					}
				}
			}
	public ImageList ImageList
			{
				get
				{
					return imageList;
				}
				set
				{
					if(imageList != value)
					{
						imageList = value;
						Recalculate();
					}
				}
			}
	public virtual int PreferredHeight
			{
				get
				{
					int height = FontHeight;

					if(BorderStyle == BorderStyle.None)
					{
						return height + 3;
					}
					else
					{
						return height + 6;
					}
				}
			}
	public virtual int PreferredWidth
			{
				get
				{
					// See if we have a cached width from last time.
					if(preferredWidth == -1)
					{
						preferredWidth = GetPreferredSize().Width;
					}
					return preferredWidth;
				}
			}
	// The actual size the text in the label would occupy.
	internal Size GetPreferredSize()
			{
				Size size;
				// Bail out if the text string is empty.
				String text = Text;
				if(text == null || text == String.Empty)
				{
					size = new Size(2, 0);
				}
				else
				{
					// Get a graphics object and measure the text.
					using (Graphics graphics = CreateGraphics())
					{
						// Measure using very large bounds and using the default StringFormat.
						SizeF size1 = graphics.MeasureString
							(text, Font, new SizeF(30000.0f, 30000.0f));
	#if CONFIG_EXTENDED_NUMERICS
						size = Size.Ceiling(size1);
	#else
						size = new Size((int)(size1.Width + 0.99f),(int)(size1.Height + 0.99f));
	#endif
					}
				}

				// Add one to account for a small discrepancy between the behaviour
				// of MeasureString and DrawString and 1 because we leave the first pixel blank when drawing.
				size.Width += 2;

				if(BorderStyle != BorderStyle.None)
				{
					size += new Size(4, 4);
				}
				return size;
			}
	protected virtual bool RenderTransparent
			{
				get
				{
					return renderTransparent;
				}
				set
				{
					if(renderTransparent != value)
					{
						renderTransparent = value;
						Invalidate();
					}
				}
			}
	public ContentAlignment TextAlign
			{
				get
				{
					return alignment;
				}
				set
				{
					if(alignment != value)
					{
						alignment = value;
						OnTextAlignChanged(EventArgs.Empty);
					}
				}
			}
	public bool UseMnemonic
			{
				get
				{
					return useMnemonic;
				}
				set
				{
					if(useMnemonic != value)
					{
						useMnemonic = value;
						Invalidate();
					}
				}
			}

	// Event that is emitted when the "AutoSize" property changes.
	public event EventHandler AutoSizeChanged
			{
				add
				{
					AddHandler(EventId.AutoSizeChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.AutoSizeChanged, value);
				}
			}

	// Event that is emitted when the "TextAlign" property changes.
	public event EventHandler TextAlignChanged
			{
				add
				{
					AddHandler(EventId.TextAlignChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.TextAlignChanged, value);
				}
			}

	// Calculate the image rendering bounds.
	protected Rectangle CalcImageRenderBounds
				(Image image, Rectangle r, ContentAlignment align)
			{
				int width = image.Width;
				int height = image.Height;
				Rectangle result = new Rectangle();
				switch(align)
				{
					case ContentAlignment.TopLeft:
					default:
					{
						result.X = r.X;
						result.Y = r.Y;
					}
					break;

					case ContentAlignment.TopCenter:
					{
						result.X = r.X + (r.Width - width) / 2;
						result.Y = r.Y;
					}
					break;

					case ContentAlignment.TopRight:
					{
						result.X = r.X + r.Width - width;
						result.Y = r.Y;
					}
					break;

					case ContentAlignment.MiddleLeft:
					{
						result.X = r.X;
						result.Y = r.Y + (r.Height - height) / 2;
					}
					break;

					case ContentAlignment.MiddleCenter:
					{
						result.X = r.X + (r.Width - width) / 2;
						result.Y = r.Y + (r.Height - height) / 2;
					}
					break;

					case ContentAlignment.MiddleRight:
					{
						result.X = r.X + r.Width - width;
						result.Y = r.Y + (r.Height - height) / 2;
					}
					break;

					case ContentAlignment.BottomLeft:
					{
						result.X = r.X;
						result.Y = r.Y + r.Height - height;
					}
					break;

					case ContentAlignment.BottomCenter:
					{
						result.X = r.X + (r.Width - width) / 2;
						result.Y = r.Y + r.Height - height;
					}
					break;

					case ContentAlignment.BottomRight:
					{
						result.X = r.X + r.Width - width;
						result.Y = r.Y + r.Height - height;
					}
					break;
				}
				result.Width = width;
				result.Height = height;
				return result;
			}

#if !CONFIG_COMPACT_FORMS

	// Create the accessibility object for this control.
	protected override AccessibleObject CreateAccessibilityInstance()
			{
				return base.CreateAccessibilityInstance();
			}

#endif

	// Dispose of this instance.
	protected override void Dispose(bool disposing)
			{
				// Nothing to do in this implementation.
				base.Dispose(disposing);
			}

	// Draw an image within the specified bounds.
	protected void DrawImage
				(Graphics g, Image image, Rectangle r, ContentAlignment align)
			{
				Rectangle bounds = CalcImageRenderBounds(image, r, align);
				g.DrawImage(image, bounds);
			}

	// Raise the "AutoSizeChanged" event.
	protected virtual void OnAutoSizeChanged(EventArgs e)
			{
				Invalidate();

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.AutoSizeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Override the "FontChanged" event.
	protected override void OnFontChanged(EventArgs e)
			{
				base.OnFontChanged(e);
				Recalculate();
			}

	// Override the "Paint" event.
	[TODO]
	protected override void OnPaint(PaintEventArgs e)
			{
				Draw(e.Graphics);
				base.OnPaint(e);
			}

	private void Draw(Graphics g)
			{
				// Fill the background if we aren't transparent.
				if(!renderTransparent)
				{
					using(Brush brush = CreateBackgroundBrush())
					{
						g.FillRectangle(brush, ClientRectangle);
					}
				}
				
				// Draw the image
				if(image != null)
				{
					this.DrawImage(g, image, base.ClientRectangle, this.imageAlign);
				}

				// Draw the text within the label.
				RectangleF layout = (RectangleF)ClientRectangle;
				StringFormat format = GetStringFormat();
				if(text != null && text != String.Empty)
				{
					layout.X += 1;
					if(Enabled)
					{
						Brush brush = new SolidBrush(ForeColor);
						g.DrawString(Text, Font, brush, layout, format);
						brush.Dispose();
					}
					else
					{
						ControlPaint.DrawStringDisabled
							(g, Text, Font, BackColor, layout, format);
					}
				}
			}

	// Override the "ParentChanged" event.
	protected override void OnParentChanged(EventArgs e)
			{
				base.OnParentChanged(e);
			}

	// Raise the "TextAlignChanged" event.
	protected virtual void OnTextAlignChanged(EventArgs e)
			{
				Invalidate();

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.TextAlignChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Override the "TextChanged" event.
	protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);
				Recalculate();
			}

	// Override the "VisibleChanged" event.
	protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
			}

	// Process a key mnemonic.  For labels, we pass the selection
	// focus onto the control that follows the label.
	protected override bool ProcessMnemonic(char charCode)
			{
				if(useMnemonic && IsMnemonic(charCode, Text))
				{

					Control parent = Parent;
					Control hierarchy = parent;
					while (hierarchy != null)
					{
						if (!hierarchy.Enabled || !hierarchy.Visible)
						{
							return base.ProcessMnemonic(charCode);
						}
						hierarchy = hierarchy.Parent;
					}

					if(parent != null)
					{
						if (parent.SelectNextControl(this, true, false, true, false) && !parent.ContainsFocus)
						{
							parent.Focus();
						}
						return true;
					}
				}
				return base.ProcessMnemonic(charCode);
			}

	// Inner core of "SetBounds".
	protected override void SetBoundsCore
				(int x, int y, int width, int height,
				 BoundsSpecified specified)
			{
				if(AutoSize)
				{
					width = PreferredWidth;
					height = PreferredHeight;
				}
				base.SetBoundsCore(x, y, width, height, specified);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", Text: " + Text;
			}

#if !CONFIG_COMPACT_FORMS

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

	// Get the string format to use to render the text.
	[TODO]
	private StringFormat GetStringFormat()
			{
				StringFormat format = new StringFormat();
				// TODO: adjust the format according to the label's properties.
				switch(RtlTranslateAlignment(TextAlign))
				{
					case ContentAlignment.TopLeft:
					{
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Near;
					}
					break;

					case ContentAlignment.TopCenter:
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Near;
					}
					break;

					case ContentAlignment.TopRight:
					{
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Near;
					}
					break;

					case ContentAlignment.MiddleLeft:
					{
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Center;
					}
					break;

					case ContentAlignment.MiddleCenter:
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;
					}
					break;

					case ContentAlignment.MiddleRight:
					{
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Center;
					}
					break;

					case ContentAlignment.BottomLeft:
					{
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Far;
					}
					break;

					case ContentAlignment.BottomCenter:
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Far;
					}
					break;

					case ContentAlignment.BottomRight:
					{
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Far;
					}
					break;
				}
				if(UseMnemonic)
				{
					format.HotkeyPrefix = HotkeyPrefix.Show;
				}
				return format;
			}

	// Recalculate the preferred size of the label and then redraw it.
	private void Recalculate()
			{
				// Recalculate the size and set it.
				preferredWidth = -1;
				if(AutoSize)
				{
					Size = new Size(PreferredWidth, PreferredHeight);
				}

				Invalidate();
			}

}; // class Label

}; // namespace System.Windows.Forms
