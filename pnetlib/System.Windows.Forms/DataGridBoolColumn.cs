/*
 * DataGridBoolColumn.cs - Implementation of "System.Windows.Forms.DataGridBoolColumn" 
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
	public class DataGridBoolColumn : DataGridColumnStyle
	{
		[TODO]
		public DataGridBoolColumn()
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridBoolColumn(System.ComponentModel.PropertyDescriptor prop)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridBoolColumn(System.ComponentModel.PropertyDescriptor prop, bool isDefault)
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		protected internal override void Abort(int rowNum)
		{
			throw new NotImplementedException("Abort");
		}

		[TODO]
		protected internal override bool Commit(System.Windows.Forms.CurrencyManager dataSource, int rowNum)
		{
			throw new NotImplementedException("Commit");
		}

		[TODO]
		protected internal override void ConcedeFocus()
		{
			throw new NotImplementedException("ConcedeFocus");
		}

		[TODO]
		protected internal override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, System.String instantText, bool cellIsVisible)
		{
			throw new NotImplementedException("Edit");
		}

		[TODO]
		protected internal override void EnterNullValue()
		{
			throw new NotImplementedException("EnterNullValue");
		}

		[TODO]
		protected internal override System.Object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager lm, int row)
		{
			throw new NotImplementedException("GetColumnValueAtRow");
		}

		[TODO]
		protected internal override int GetMinimumHeight()
		{
			throw new NotImplementedException("GetMinimumHeight");
		}

		[TODO]
		protected internal override int GetPreferredHeight(System.Drawing.Graphics g, System.Object value)
		{
			throw new NotImplementedException("GetPreferredHeight");
		}

		[TODO]
		protected internal override System.Drawing.Size GetPreferredSize(System.Drawing.Graphics g, System.Object value)
		{
			throw new NotImplementedException("GetPreferredSize");
		}

		[TODO]
		protected internal override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum, bool alignToRight)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Brush backBrush, System.Drawing.Brush foreBrush, bool alignToRight)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager lm, int row, System.Object value)
		{
			throw new NotImplementedException("SetColumnValueAtRow");
		}

		[TODO]
		public void add_AllowNullChanged(System.EventHandler value)
		{
			throw new NotImplementedException("add_AllowNullChanged");
		}

		[TODO]
		public void add_FalseValueChanged(System.EventHandler value)
		{
			throw new NotImplementedException("add_FalseValueChanged");
		}

		[TODO]
		public void add_TrueValueChanged(System.EventHandler value)
		{
			throw new NotImplementedException("add_TrueValueChanged");
		}

		[TODO]
		public void remove_AllowNullChanged(System.EventHandler value)
		{
			throw new NotImplementedException("remove_AllowNullChanged");
		}

		[TODO]
		public void remove_FalseValueChanged(System.EventHandler value)
		{
			throw new NotImplementedException("remove_FalseValueChanged");
		}

		[TODO]
		public void remove_TrueValueChanged(System.EventHandler value)
		{
			throw new NotImplementedException("remove_TrueValueChanged");
		}

		[TODO]
		public bool AllowNull 
		{
 			get
			{
				throw new NotImplementedException("AllowNull");
			}

 			set
			{
				throw new NotImplementedException("AllowNull");
			}

 		}

		[TODO]
		public System.Object FalseValue 
		{
 			get
			{
				throw new NotImplementedException("FalseValue");
			}

 			set
			{
				throw new NotImplementedException("FalseValue");
			}

 		}

		[TODO]
		public System.Object NullValue 
		{
 			get
			{
				throw new NotImplementedException("NullValue");
			}

 			set
			{
				throw new NotImplementedException("NullValue");
			}

 		}

		[TODO]
		public System.Object TrueValue 
		{
 			get
			{
				throw new NotImplementedException("TrueValue");
			}

 			set
			{
				throw new NotImplementedException("TrueValue");
			}

 		}

		public System.EventHandler AllowNullChanged;

		public System.EventHandler FalseValueChanged;

		public System.EventHandler TrueValueChanged;

	}
}//namespace
