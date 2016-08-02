/*
 * ButtonBase.cs - Implementation of the
 *			"System.Windows.Forms.ButtonBase" class.
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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Themes;

public abstract class ButtonBase : Control
{
	// Internal state.
	private FlatStyle flatStyle;
	private Image image;
	private ContentAlignment imageAlign;
	private int imageIndex;
	private ImageList imageList;
	private ContentAlignment textAlign;
	private bool isDefault;
	internal bool entered;
	internal bool pressed;
	internal bool hasFocus;
	internal MouseButtons button;
	private StringFormat format;
	private ButtonState prevState;

	// Contructor.
	protected ButtonBase()
			{
				flatStyle = FlatStyle.Standard;
				imageAlign = ContentAlignment.MiddleCenter;
				imageIndex = -1;
				textAlign = ContentAlignment.MiddleCenter;
				prevState = (ButtonState)(-1);
				format = new StringFormat();
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				SetStringFormat();
				SetStyle(ControlStyles.ResizeRedraw, true);
			}

	// Get or set this control's properties.
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(FlatStyle.Standard)]
#endif
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
						Redraw();
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
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
						Redraw();
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(ContentAlignment.MiddleCenter)]
#endif
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
						Redraw();
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if (CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS) && CONFIG_COMPONENT_MODEL_DESIGN
	[DefaultValue(-1)]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design", typeof(UITypeEditor))]
#endif
#if !CONFIG_COMPACT_FORMS || CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[TypeConverter(typeof(ImageIndexConverter))]
#endif
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
						Redraw();
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
						Redraw();
					}
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
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
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(ContentAlignment.MiddleCenter)]
#endif
#if !CONFIG_COMPACT_FORMS
	public
#else
	internal
#endif
	virtual ContentAlignment TextAlign
			{
				get
				{
					return textAlign;
				}
				set
				{
					if(textAlign != value)
					{
						textAlign = value;
						SetStringFormat();
						Redraw();
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
					return new Size(75, 23);
				}
			}
	protected bool IsDefault
			{
				get
				{
					return isDefault;
				}
				set
				{
					if(isDefault != value)
					{
						isDefault = value;
						Redraw();
					}
				}
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

	// Get the correct string format, based on text alignment and RTL info.
	internal StringFormat GetStringFormat()
			{
				return format;
			}

	// Set the correct string format, based on text alignment and RTL info.
	private void SetStringFormat()
			{
				format.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
				
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
			}

	// Calculate the current state of the button for its visual appearance.
	internal virtual ButtonState CalculateState()
			{
				ButtonState state = ButtonState.Normal;
				if(Enabled)
				{
					if(entered && pressed)
					{
						state |= ButtonState.Pushed;
						if(flatStyle == FlatStyle.Flat)
						{
							state |= ButtonState.Flat;
						}
					}
					else if(entered)
					{
						if(flatStyle == FlatStyle.Flat)
						{
							state |= ButtonState.Flat;
						}
					}
					else
					{
						if(flatStyle == FlatStyle.Flat ||
						   flatStyle == FlatStyle.Popup)
						{
							state |= ButtonState.Flat;
						}
					}
					if(hasFocus)
					{
						// Special flag that indicates a focus rectangle.
						state |= (ButtonState)0x20000;
					}
				}
				else
				{
					state |= ButtonState.Inactive;
					if(flatStyle == FlatStyle.Flat ||
					   flatStyle == FlatStyle.Popup)
					{
						state |= ButtonState.Flat;
					}
				}
				return state;
			}

	// Get the descent in pixels for a font.
	internal static float GetDescent(Graphics graphics, Font font)
			{
				FontFamily family = font.FontFamily;
				int ascent = family.GetCellAscent(font.Style);
				int descent = family.GetCellDescent(font.Style);
				return font.GetHeight(graphics) * descent / (ascent + descent);
			}

	// Draw the button in its current state on a Graphics surface.
	internal virtual void Draw(Graphics graphics)
			{
				ButtonState state = CalculateState();
				Size clientSize = ClientSize;
				int x = 0;
				int y = 0;
				int width = clientSize.Width;
				int height = clientSize.Height;

				// Draw the border and background.
				ThemeManager.MainPainter.DrawButton
					(graphics, x, y, width, height, state,
					 ForeColor, BackColor, isDefault);

				// Draw the focus rectangle.
				if(hasFocus)
				{
					ControlPaint.DrawFocusRectangle
						(graphics, new Rectangle(x + 4, y + 4,
												 width - 8, height - 8),
						 ForeColor, BackColor);
				}

				// Draw the button image.
				Image image = this.image;
				if(image == null && imageList != null && imageIndex != -1)
				{
					image = imageList.Images[imageIndex];
				}
				if(image != null)
				{
					int imageX = x;
					int imageY = y;
					int imageWidth = image.Width;
					int imageHeight = image.Height;
					switch(imageAlign)
					{
						case ContentAlignment.TopLeft: break;

						case ContentAlignment.TopCenter:
						{
							imageX += (width - imageWidth) / 2;
						}
						break;

						case ContentAlignment.TopRight:
						{
							imageX += width - imageWidth;
						}
						break;

						case ContentAlignment.MiddleLeft:
						{
							imageY += (height - imageHeight) / 2;
						}
						break;

						case ContentAlignment.MiddleCenter:
						{
							imageX += (width - imageWidth) / 2;
							imageY += (height - imageHeight) / 2;
						}
						break;

						case ContentAlignment.MiddleRight:
						{
							imageX += width - imageWidth;
							imageY += (height - imageHeight) / 2;
						}
						break;

						case ContentAlignment.BottomLeft:
						{
							imageY += height - imageHeight;
						}
						break;

						case ContentAlignment.BottomCenter:
						{
							imageX += (width - imageWidth) / 2;
							imageY += height - imageHeight;
						}
						break;

						case ContentAlignment.BottomRight:
						{
							imageX += width - imageWidth;
							imageY += height - imageHeight;
						}
						break;
					}
					if(pressed)
					{
						++imageX;
						++imageY;
					}
					if(Enabled)
					{
						graphics.DrawImage(image, imageX, imageY);
					}
					else
					{
						ControlPaint.DrawImageDisabled
							(graphics, image, imageX, imageY, BackColor);
					}
				}

				// Get the text layout rectangle.
				RectangleF layout = new RectangleF
					(x + 2, y + 2, width - 4, height - 4);
				if(entered && pressed)
				{
					layout.Offset(1.0f, 1.0f);
				}

				// If we are using "middle" alignment, then shift the
				// text down to account for the descent.  This makes
				// the button "look better".
				Font font = Font;
				if((TextAlign & (ContentAlignment.MiddleLeft |
								 ContentAlignment.MiddleCenter |
								 ContentAlignment.MiddleRight)) != 0)
				{
					layout.Offset(0.0f, GetDescent(graphics, font) / 2.0f);
				}

				// Draw the text on the button.
				String text = Text;
				if(text != null && text != String.Empty)
				{
					if(Enabled)
					{
						Brush brush = new SolidBrush(ForeColor);
						graphics.DrawString(text, font, brush, layout, format);
						brush.Dispose();
					}
					else
					{
						ControlPaint.DrawStringDisabled
							(graphics, text, font, BackColor, layout, format);
					}
				}
			}

	// Redraw the button after a state change.
	internal void Redraw()
			{
				// Clear the previous state, for non-trivial draw detection.
				prevState = (ButtonState)(-1);

				// Redraw the button.
				Invalidate();
			}

	// Redraw if a non-trivial state change has occurred.
	internal void RedrawIfChanged()
			{
				ButtonState state = CalculateState();
				if(state != prevState)
				{
					Redraw();
					prevState = state;
				}
			}

	// Override events from the "Control" class.
	protected override void OnEnabledChanged(EventArgs e)
			{
				RedrawIfChanged();
				base.OnEnabledChanged(e);
			}
	protected override void OnEnter(EventArgs e)
			{
				hasFocus = true;
				RedrawIfChanged();
				base.OnEnter(e);
			}
	protected override void OnLeave(EventArgs e)
			{
				hasFocus = false;
				RedrawIfChanged();
				base.OnLeave (e);
			}
	protected override void OnKeyDown(KeyEventArgs e)
			{
				if(e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
				{
					if(Enabled)
					{
						OnClick(EventArgs.Empty);
						e.Handled = true;
					}
				}
				base.OnKeyDown(e);
			}
	protected override void OnKeyUp(KeyEventArgs e)
			{
				base.OnKeyUp(e);
			}

	protected override void OnMouseDown(MouseEventArgs e)
			{
				if(button == MouseButtons.None && e.Button == MouseButtons.Left)
				{
					button = e.Button;
					pressed = true;
					RedrawIfChanged();
				}
				base.OnMouseDown(e);
			}
	protected override void OnMouseEnter(EventArgs e)
			{
				entered = true;
				RedrawIfChanged();
				base.OnMouseEnter(e);
			}
	protected override void OnMouseLeave(EventArgs e)
			{
				entered = false;
				RedrawIfChanged();
				base.OnMouseLeave(e);
			}
	protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);
			}
	protected override void OnMouseUp(MouseEventArgs e)
			{
				if(button == e.Button)
				{
					button = MouseButtons.None;
					pressed = false;
					RedrawIfChanged();
				}
				base.OnMouseUp(e);
			}
	protected override void OnPaint(PaintEventArgs e)
			{
				prevState = CalculateState();
				Draw(e.Graphics);
				base.OnPaint(e);
			}
	protected override void OnParentChanged(EventArgs e)
			{
				Redraw();
				base.OnParentChanged(e);
			}
	protected override void OnTextChanged(EventArgs e)
			{
				Redraw();
				base.OnTextChanged(e);
			}
	protected override void OnRightToLeftChanged(EventArgs e)
			{
				SetStringFormat();
				Redraw();
				base.OnRightToLeftChanged(e);
			}
	protected override void OnVisibleChanged(EventArgs e)
			{
				if(!Visible)
				{
					entered = false;
					pressed = false;
					hasFocus = false;
					button = MouseButtons.None;
				}
				base.OnVisibleChanged(e);
			}

#if !CONFIG_COMPACT_FORMS

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

}; // class ButtonBase

}; // namespace System.Windows.Forms
