/*
 * DataGridColumnStyle.cs - Implementation of "System.Windows.Forms.DataGridColumnStyle" 
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
	using System.Drawing;
	using System.Text;
	using System.ComponentModel;
	using System.Reflection;


	public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
	{
		[TODO]
		public DataGridColumnStyle()
		{
			throw new NotImplementedException(".ctor");
		}

		[TODO]
		public DataGridColumnStyle(System.ComponentModel.PropertyDescriptor prop)
		{
			throw new NotImplementedException(".ctor");
		}

		protected internal abstract void Abort(int rowNum);

		[TODO]
		protected void BeginUpdate()
		{
			throw new NotImplementedException("BeginUpdate");
		}

		[TODO]
		protected void CheckValidDataSource(CurrencyManager value)
		{
			throw new NotImplementedException("CheckValidDataSource");
		}

		[TODO]
		public virtual void ColumnStartedEditing(Control editingControl)
		{
			throw new NotImplementedException("ColumnStartedEditing");
		}

		protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

		[TODO]
		protected internal virtual void ConcedeFocus()
		{
			throw new NotImplementedException("ConcedeFocus");
		}

		[TODO]
		protected virtual AccessibleObject CreateHeaderAccessibleObject()
		{
			throw new NotImplementedException("CreateHeaderAccessibleObject");
		}

		[TODO]
		protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly)
		{
			throw new NotImplementedException("Edit");
		}

		[TODO]
		protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, System.String instantText)
		{
			throw new NotImplementedException("Edit");
		}

		protected internal abstract void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, System.String instantText, bool cellIsVisible);

		[TODO]
		protected void EndUpdate()
		{
			throw new NotImplementedException("EndUpdate");
		}

		[TODO]
		protected internal virtual void EnterNullValue()
		{
			throw new NotImplementedException("EnterNullValue");
		}

		[TODO]
		protected internal virtual System.Object GetColumnValueAtRow(CurrencyManager source, int rowNum)
		{
			throw new NotImplementedException("GetColumnValueAtRow");
		}

		protected internal abstract int GetMinimumHeight();

		protected internal abstract int GetPreferredHeight(Graphics g, System.Object value);

		protected internal abstract System.Drawing.Size GetPreferredSize(System.Drawing.Graphics g, System.Object value);

		[TODO]
		protected virtual void Invalidate()
		{
			throw new NotImplementedException("Invalidate");
		}

		protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

		protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

		[TODO]
		protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			throw new NotImplementedException("Paint");
		}

		[TODO]
		protected internal virtual void ReleaseHostedControl()
		{
			throw new NotImplementedException("ReleaseHostedControl");
		}

		[TODO]
		public void ResetHeaderText()
		{
			throw new NotImplementedException("ResetHeaderText");
		}

		[TODO]
		protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, System.Object value)
		{
			throw new NotImplementedException("SetColumnValueAtRow");
		}

		[TODO]
		protected virtual void SetDataGrid(System.Windows.Forms.DataGrid value)
		{
			throw new NotImplementedException("SetDataGrid");
		}

		[TODO]
		protected virtual void SetDataGridInColumn(DataGrid value)
		{
			throw new NotImplementedException("SetDataGridInColumn");
		}

		[TODO]
		protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, System.String instantText)
		{
			throw new NotImplementedException("UpdateUI");
		}

		public event EventHandler AlignmentChanged
		{
			add
			{
				Events.AddHandler(EventId.AlignmentChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.AlignmentChanged,value);
			}
		}
		
			
		public event EventHandler FontChanged
		{
			add
			{
				Events.AddHandler(EventId.FontChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.FontChanged,value);
			}
		}

		public event EventHandler HeaderTextChanged
		{
			add
			{
				Events.AddHandler(EventId.HeaderTextChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.HeaderTextChanged,value);
			}
		}

		public event EventHandler MappingNameChanged
		{
			add
			{
				Events.AddHandler(EventId.MappingNameChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.MappingNameChanged,value);
			}
		}

		public event EventHandler NullTextChanged
		{
			add
			{
				Events.AddHandler(EventId.NullTextChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.NullTextChanged,value);
			}
		}

		public event EventHandler PropertyDescriptorChanged
		{
			add
			{
				Events.AddHandler(EventId.PropertyDescriptorChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.PropertyDescriptorChanged,value);
			}
		}

		public event EventHandler ReadOnlyChanged
		{
			add
			{
				Events.AddHandler(EventId.ReadOnlyChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.ReadOnlyChanged,value);
			}
		}

		public event EventHandler WidthChanged
		{
			add
			{
				Events.AddHandler(EventId.WidthChanged,value);
			}
			remove
			{
				Events.RemoveHandler(EventId.WidthChanged,value);
			}
		}

		[TODO]
		public virtual HorizontalAlignment Alignment 
		{
 			get
			{
				throw new NotImplementedException("Alignment");
			}

 			set
			{
				throw new NotImplementedException("Alignment");
			}

 		}

		[TODO]
		public virtual DataGridTableStyle DataGridTableStyle 
		{
 			get
			{
				throw new NotImplementedException("DataGridTableStyle");
			}

 		}

		[TODO]
		protected int FontHeight 
		{
 			get
			{
				throw new NotImplementedException("FontHeight");
			}

 		}

		[TODO]
		public AccessibleObject HeaderAccessibleObject 
		{
 			get
			{
				throw new NotImplementedException("HeaderAccessibleObject");
			}

 		}

		[TODO]
		public virtual System.String HeaderText 
		{
 			get
			{
				throw new NotImplementedException("HeaderText");
			}

 			set
			{
				throw new NotImplementedException("HeaderText");
			}

 		}

		[TODO]
		public System.String MappingName 
		{
 			get
			{
				throw new NotImplementedException("MappingName");
			}

 			set
			{
				throw new NotImplementedException("MappingName");
			}

 		}

		[TODO]
		public virtual System.String NullText 
		{
 			get
			{
				throw new NotImplementedException("NullText");
			}

 			set
			{
				throw new NotImplementedException("NullText");
			}

 		}

		[TODO]
		public virtual System.ComponentModel.PropertyDescriptor PropertyDescriptor 
		{
 			get
			{
				throw new NotImplementedException("PropertyDescriptor");
			}

 			set
			{
				throw new NotImplementedException("PropertyDescriptor");
			}

 		}

		[TODO]
		public virtual bool ReadOnly 
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
		public virtual int Width 
		{
 			get
			{
				throw new NotImplementedException("Width");
			}

 			set
			{
				throw new NotImplementedException("Width");
			}

 		}

	}
}//namespace
