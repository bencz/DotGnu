/*
 * DataGridTextBoxColumn.cs - Implementation of "System.Windows.Forms.DataGridTextBoxColumn" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Free Software Foundation, Inc.
 * Copyright (C) 2005  Boris Manojlovic.
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
	using System.ComponentModel;
	using System.Drawing;
	
	public class DataGridTextBoxColumn : DataGridColumnStyle
	{
		[TODO]
		public DataGridTextBoxColumn()
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridTextBoxColumn(PropertyDescriptor prop)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridTextBoxColumn(PropertyDescriptor prop, String format)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridTextBoxColumn(PropertyDescriptor prop, String format, bool isDefault)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		protected internal override void Abort(int rowNum)
		{
			throw new NotImplementedException("Abort");
		}

		[TODO]
		protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
		{
			throw new NotImplementedException("Commit");
		}

		[TODO]
		protected internal override void ConcedeFocus()
		{
			throw new NotImplementedException("ConcedeFocus");
		}

		[TODO]
		protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, String instantText, bool cellIsVisible)
		{
			throw new NotImplementedException("Edit");
		}

		[TODO]
		protected void EndEdit()
		{
			throw new NotImplementedException("EndEdit");
		}

		[TODO]
		protected internal override void EnterNullValue()
		{
			throw new NotImplementedException("EnterNullValue");
		}

		[TODO]
		protected internal override int GetMinimumHeight()
		{
			throw new NotImplementedException("GetMinimumHeight");
		}

		[TODO]
		protected internal override int GetPreferredHeight(Graphics g, System.Object value)
		{
			throw new NotImplementedException("GetPreferredHeight");
		}

		[TODO]
		protected internal override System.Drawing.Size GetPreferredSize(Graphics g, Object value)
		{
			throw new NotImplementedException("GetPreferredSize");
		}

		[TODO]
		protected void HideEditBox()
		{
			throw new NotImplementedException("HideEditBox");
		}

		[TODO]
		protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected void PaintText(Graphics g, Rectangle bounds, String text, bool alignToRight)
		{
			throw new NotImplementedException("PaintText");
		}

		[TODO]
		protected void PaintText(Graphics g,Rectangle textBounds, String text, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			throw new NotImplementedException("PaintText");
		}

		[TODO]
		protected internal override void ReleaseHostedControl()
		{
			throw new NotImplementedException("ReleaseHostedControl");
		}

		[TODO]
		protected override void SetDataGridInColumn(DataGrid value)
		{
			throw new NotImplementedException("SetDataGridInColumn");
		}

		[TODO]
		protected internal override void UpdateUI(CurrencyManager source, int rowNum,String instantText)
		{
			throw new NotImplementedException("UpdateUI");
		}

		[TODO]
		public String Format 
		{
 			get
			{
				throw new NotImplementedException("Format");
			}

 			set
			{
				throw new NotImplementedException("Format");
			}

 		}

		[TODO]
		public System.IFormatProvider FormatInfo 
		{
 			get
			{
				throw new NotImplementedException("FormatInfo");
			}

 			set
			{
				throw new NotImplementedException("FormatInfo");
			}

 		}

		[TODO]
		public override PropertyDescriptor PropertyDescriptor 
		{
 			set
			{
				throw new NotImplementedException("PropertyDescriptor");
			}

 		}

		[TODO]
		public override bool ReadOnly 
		{
 			get
			{
				throw new NotImplementedException("ReadOnly");
			}

 			set
			{
				throw new NotImplementedException("ReadOnly");
			}

 		}

		[TODO]
		public virtual System.Windows.Forms.TextBox TextBox 
		{
 			get
			{
				throw new NotImplementedException("TextBox");
			}

 		}

	}
}//namespace
