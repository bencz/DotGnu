/*
 * PictureBox.cs - Implementation of the "System.Windows.Forms.PictureBox" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Free Software Foundation, Inc.
 * 
 * Contributions from Simon Guindon (simon@nureality.ca)
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
	using System.Drawing.Imaging;
	using System.Windows.Forms;

	public class PictureBox : Control
	{
		private Image image;
		private PictureBoxSizeMode sizeMode;

		public PictureBox()
		{
			sizeMode = PictureBoxSizeMode.Normal;
			TabStop = false;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, false);
		}

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

		public BorderStyle BorderStyle
		{
			get
			{
				return BorderStyleInternal;
			}
			set
			{
				if (value == BorderStyleInternal)
					return;
				BorderStyleInternal = value;
				Invalidate();
			}
		}

		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
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
				return base.DefaultImeMode;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(100, 50);
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

		public Image Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
				if (sizeMode == PictureBoxSizeMode.AutoSize)
					SetSize();
				Invalidate();
			}
		}

		private void SetSize()
		{
				if (image != null)
					ClientSize = image.Size; 
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

		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
			}
		}

		public PictureBoxSizeMode SizeMode
		{
			get
			{
				return sizeMode;
			}
			set
			{
				sizeMode = value;
				if (sizeMode == PictureBoxSizeMode.AutoSize)
					SetSize();
				Invalidate();
			}
		}

		public new int TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex = value;
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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Draw(e.Graphics);
		}

		private void Draw(Graphics g)
		{
			if (image != null)
			{
				int imageX = 0;
				int imageY = 0;
				int imageWidth = image.Width;
				int imageHeight = image.Height;
				if (sizeMode == PictureBoxSizeMode.CenterImage )
				{
					Size client = base.ClientSize;
					imageX = (client.Width - imageWidth) / 2;
					imageY = (client.Height - imageHeight) / 2;
				}
				else if (sizeMode == PictureBoxSizeMode.StretchImage)
				{
					Size client = ClientSize;
					imageWidth = client.Width;
					imageHeight = client.Height;
				}			
				g.DrawImage(image, imageX, imageY, imageWidth, imageHeight);
			}
		}
	
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (!IsHandleCreated)
				return;
			if (sizeMode == PictureBoxSizeMode.StretchImage || sizeMode == PictureBoxSizeMode.CenterImage)
				Invalidate();
		}

		protected virtual void OnSizeModeChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)GetHandler(EventId.SizeModeChanged);
			if (handler != null)
				handler(this,e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if (sizeMode == PictureBoxSizeMode.AutoSize)
			{
				if (image == null)
				{
					width = base.Width;
					height = base.Height;
				}
				else
				{
					width = image.Width;
					height = image.Height;
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}

		public override string ToString()
		{
			return base.ToString() + ", SizeMode:" + sizeMode.ToString("G");
		}

		public new event EventHandler CausesValidationChanged
		{
			add
			{
				base.CausesValidationChanged += value;
			}
			remove
			{
				base.CausesValidationChanged -= value;
			}
		}

		public new event EventHandler Enter
		{
			add
			{
				base.Enter += value;
			}
			remove
			{
				base.Enter -= value;
			}
		}

		public new event EventHandler FontChanged
		{
			add
			{
				base.FontChanged += value;
			}
			remove
			{
				base.FontChanged -= value;
			}
		}

		public new event EventHandler ForeColorChanged
		{
			add
			{
				base.ForeColorChanged += value;
			}
			remove
			{
				base.ForeColorChanged -= value;
			}
		}

		public new event EventHandler ImeModeChanged
		{
			add
			{
				base.ImeModeChanged += value;
			}
			remove
			{
				base.ImeModeChanged -= value;
			}
		}

		public new event KeyEventHandler KeyDown
		{
			add
			{
				base.KeyDown += value;
			}
			remove
			{
				base.KeyDown -= value;
			}
		}

		public new event KeyPressEventHandler KeyPress
		{
			add
			{
				base.KeyPress += value;
			}
			remove
			{
				base.KeyPress -= value;
			}
		}

		public new event KeyEventHandler KeyUp
		{
			add
			{
				base.KeyUp += value;
			}
			remove
			{
				base.KeyUp -= value;
			}
		}

		public new event EventHandler Leave
		{
			add
			{
				base.Leave += value;
			}
			remove
			{
				base.Leave -= value;
			}
		}

		public new event EventHandler RightToLeftChanged
		{
			add
			{
				base.RightToLeftChanged += value;
			}
			remove
			{
				base.RightToLeftChanged -= value;
			}
		}

		public event EventHandler SizeModeChanged
		{
			add
			{
				AddHandler(EventId.BackColorChanged, value);
			}
			remove
			{
				RemoveHandler(EventId.BackColorChanged, value);
			}
		}

		public new event EventHandler TabIndexChanged
		{
			add
			{
				base.TabIndexChanged += value;
			}
			remove
			{
				base.TabIndexChanged -= value;
			}
		}

		public new event EventHandler TabStopChanged
		{
			add
			{
				base.TabStopChanged += value;
			}
			remove
			{
				base.TabStopChanged -= value;
			}
		}

		public new event EventHandler TextChanged
		{
			add
			{
				base.TextChanged += value;
			}
			remove
			{
				base.TextChanged -= value;
			}
		}

	}; // class PictureBox

}; // namespace System.Windows.Forms
