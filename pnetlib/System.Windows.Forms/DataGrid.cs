/*
 * DataGrid.cs - Implementation of "System.Windows.Forms.DataGrid" 
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
#if CONFIG_COMPONENT_MODEL
        using System.ComponentModel;
#endif
        using System.Drawing;
        public class DataGrid : Control, IDataGridEditingService
#if CONFIG_COMPONENT_MODEL
                , ISupportInitialize
#endif
	{
		internal int xOffset = 0;

		public DataGridColumnStyle gridColumn;
		public System.Drawing.Rectangle bounds;

		private HScrollBar hScrollBar;
		private VScrollBar vScrollBar;

		private BorderStyle borderStyle;
		
		private bool initializing;
		private bool captionVisible = true;
		private bool columnHeadersVisible = true;
		private bool rowHeadersVisible = true;
		private bool allowNavigation = true;
		private bool allowSorting = true;

		private String dataMember= "";
		private String captionText = "";

		private int rowHeight;
		private int columnWidth;
		private int rows;
		private int columns;
		private int row;
		private int col;
		private int numEntries = 1;
		private int textHeight;
		private int leftMostColumn;
		private int selected;
		private int columnSpacing;
		private	int currentRowIndex;
		private int preferredColumnWidth = 75;
		private int preferredRowHeight = 16;
		private int rowHeaderWidth = 35;

		private Font captionFont;
		private Font headerFont;

		private Color dgforeColor; 
		private Color backgroundColor;
		private Color headerBackColor;
		private Color dgbackColor;
		private Color headerForeColor;
		private Color captionBackColor;
		private Color captionForeColor; 
		private Color gridLineColor;
		private Color alternatingBackColor;
		
		private Image backgroundImage;
		
		private System.Object dataSource;
		
		private TextBox textBox;

		private DataGridCell currentCell;
		private DataGridCell editCell;
		
		private GridTableStylesCollection gridTableStylesCollection;
		// Constructor
		public DataGrid()
		{
//			Console.WriteLine("DataGrid.constr");

			SetStyle(ControlStyles.ResizeRedraw,true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			initializing = true;
			gridTableStylesCollection= new GridTableStylesCollection();
			gridTableStylesCollection.Add(new DataGridTableStyle());
			dgforeColor = SystemColors.WindowText;
			backgroundColor = SystemColors.Window;
			headerForeColor = SystemColors.WindowText;
			hScrollBar = new HScrollBar();
			hScrollBar.Visible = false;
			hScrollBar.Dock = DockStyle.Bottom;
			hScrollBar.TabStop = false;
			hScrollBar.ValueChanged += new EventHandler(GridHScrolled);
			vScrollBar = new VScrollBar();
			vScrollBar.Visible = false;
			vScrollBar.Dock = DockStyle.Right;
			vScrollBar.TabStop = false;
			vScrollBar.ValueChanged += new EventHandler(GridVScrolled);
			Controls.Add(vScrollBar);
			Controls.Add(hScrollBar);
		}

		[TODO]
		public virtual bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
		{
			throw new NotImplementedException("BeginEdit");
		}
		[TODO]
		public virtual void BeginInit()
		{
//			Console.WriteLine("BeginInit()");
		}

		[TODO]
		protected virtual void CancelEditing()
		{
			throw new NotImplementedException("CancelEditing");
		}

		[TODO]
		public void Collapse(int row)
		{
			throw new NotImplementedException("Collapse");
		}

		[TODO]
		protected internal virtual void ColumnStartedEditing(System.Drawing.Rectangle bounds)
		{
			throw new NotImplementedException("ColumnStartedEditing");
		}

		[TODO]
		protected internal virtual void ColumnStartedEditing(Control editingControl)
		{
			throw new NotImplementedException("ColumnStartedEditing");
		}

		[TODO]
		protected override AccessibleObject CreateAccessibilityInstance()
		{
			throw new NotImplementedException("CreateAccessibilityInstance");
		}

		[TODO]
		protected virtual DataGridColumnStyle CreateGridColumn(System.ComponentModel.PropertyDescriptor prop, bool isDefault)
		{
			throw new NotImplementedException("CreateGridColumn");
		}

		[TODO]
		protected virtual DataGridColumnStyle CreateGridColumn(System.ComponentModel.PropertyDescriptor prop)
		{
			throw new NotImplementedException("CreateGridColumn");
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		[TODO]
		public virtual bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
		{
			throw new NotImplementedException("EndEdit");
		}

		[TODO]
		public virtual void EndInit()
		{
//			Console.WriteLine("EndInit()");
		}

		[TODO]
		public void Expand(int row)
		{
			throw new NotImplementedException("Expand");
		}

		[TODO]
		public Rectangle GetCellBounds(int row, int col)
		{
			throw new NotImplementedException("GetCellBounds");
		}

		[TODO]
		public Rectangle GetCellBounds(DataGridCell dgc)
		{
			throw new NotImplementedException("GetCellBounds");
		}

		[TODO]
		public Rectangle GetCurrentCellBounds()
		{
			throw new NotImplementedException("GetCurrentCellBounds");
		}

		[TODO]
		protected virtual System.String GetOutputTextDelimiter()
		{
			throw new NotImplementedException("GetOutputTextDelimiter");
		}

		[TODO]
		protected virtual void GridHScrolled(System.Object sender, EventArgs e)
		{
			//throw new NotImplementedException("GridHScrolled");
//			Console.Write("HS");
		}

		[TODO]
		protected virtual void GridVScrolled(System.Object sender, EventArgs e)
		{
			//throw new NotImplementedException("GridVScrolled");
//			Console.Write("VS");
		}
		[TODO]
		public HitTestInfo HitTest(int x, int y)
		{
			throw new NotImplementedException("HitTest");
		}

		[TODO]
		public HitTestInfo HitTest(System.Drawing.Point position)
		{
			throw new NotImplementedException("HitTest");
		}

		[TODO]
		public bool IsExpanded(int rowNumber)
		{
			throw new NotImplementedException("IsExpanded");
		}

		[TODO]
		public bool IsSelected(int row)
		{
			throw new NotImplementedException("IsSelected");
		}

		[TODO]
		public void NavigateBack()
		{
			throw new NotImplementedException("NavigateBack");
		}

		[TODO]
		public void NavigateTo(int rowNumber, System.String relationName)
		{
			throw new NotImplementedException("NavigateTo");
		}

		[TODO]
		protected virtual void OnAllowNavigationChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnAllowNavigationChanged");
		}

		[TODO]
		protected void OnBackButtonClicked(System.Object sender, System.EventArgs e)
		{
			throw new NotImplementedException("OnBackButtonClicked");
		}

		[TODO]
		protected override void OnBackColorChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnBackColorChanged");
		}

		[TODO]
		protected virtual void OnBackgroundColorChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnBackgroundColorChanged");
		}

		[TODO]
		protected override void OnBindingContextChanged(System.EventArgs e)
		{
			base.OnBindingContextChanged(e);
			//throw new NotImplementedException("OnBindingContextChanged");
		}

		[TODO]
		protected virtual void OnBorderStyleChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnBorderStyleChanged");
		}

		[TODO]
		protected virtual void OnCaptionVisibleChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnCaptionVisibleChanged");
		}

		[TODO]
		protected virtual void OnCurrentCellChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnCurrentCellChanged");
		}

		[TODO]
		protected virtual void OnDataSourceChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnDataSourceChanged");
		}

		[TODO]
		protected override void OnEnter(System.EventArgs e)
		{
			throw new NotImplementedException("OnEnter");
		}

		[TODO]
		protected virtual void OnFlatModeChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnFlatModeChanged");
		}

		[TODO]
		protected override void OnFontChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnFontChanged");
		}

		[TODO]
		protected override void OnForeColorChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnForeColorChanged");
		}

		// OnHandleCreated - Just calling base.OnHandleCreated...
		protected override void OnHandleCreated(System.EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		// OnHandleDestroyed - Just calling base.OnHandleDestroyed....
		protected override void OnHandleDestroyed(System.EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		[TODO]
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs ke)
		{
//			Console.WriteLine("OnKeyDown");
		}

		[TODO]
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs kpe)
		{
//			Console.WriteLine("OnKeyPress");
		}

		internal void Draw()
		{
			if(!Visible || !IsHandleCreated) { return; }

			
				Rectangle clientRectangle = ClientRectangle;
				int drawableHeight = clientRectangle.Height;
//				Console.WriteLine(drawableHeight);
				int drawableWidth = clientRectangle.Width;
//				Console.WriteLine(drawableWidth);

			using(Graphics g = CreateGraphics())
			{
//				Console.Write("D");
				// Need to fill bg...
				SolidBrush b = new SolidBrush(backgroundColor);
				g.FillRectangle(b, 0, 0, Width, Height);
				System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
				Size s = ClientSize;
				//Rectangle rect = new Rectangle(drawableWidth + xOffset, 1, vScrollBar.Width, clientRectangle.Height);
				Rectangle rect = new Rectangle(0, 0, hScrollBar.Width - vScrollBar.Width,vScrollBar.Height);
				hScrollBar.Visible = false;
				vScrollBar.Visible = false;
				//g.FillRectangle(myBrush, rect);
				Pen	Black = new Pen(Color.Black);
				g.DrawRectangle(Black, rect);
				Pen	txtPen = new Pen(Color.Black);
				g.DrawLine(txtPen,vScrollBar.Width/2,vScrollBar.Width, 0, vScrollBar.Width);
				//g.ExcludeClip(rect);
			}
		}
		
		protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent)
		{
//			Console.Write("OL");
			Draw();
			base.OnLayout(levent);

		}

		[TODO]
		protected override void OnLeave(System.EventArgs e)
		{
			throw new NotImplementedException("OnLeave");
		}

		[TODO]
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.backgroundColor = Color.Beige;
//			Console.Write("M");
		}

		[TODO]
		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave(e);
			//Console.WriteLine("OnMouseLeave");
			//throw new NotImplementedException("OnMouseLeave");
		}

		[TODO]
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			//Console.WriteLine("OnMouseMove");
			//throw new NotImplementedException("OnMouseMove");
		}

		[TODO]
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			//Console.WriteLine("OnMouseUp");
			//throw new NotImplementedException("OnMouseUp");
		}

		[TODO]
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			//Console.WriteLine("OnMouseWheel");
			//throw new NotImplementedException("OnMouseWheel");
		}

		[TODO]
		protected void OnNavigate(System.Windows.Forms.NavigateEventArgs e)
		{
			throw new NotImplementedException("OnNavigate");
		}

		[TODO]
		protected override void OnPaint(PaintEventArgs pe)
		{
			Draw();
			base.OnPaint(pe);
//			Console.Write("P");
			//throw new NotImplementedException("OnPaint");
		}

		[TODO]
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs ebe)
		{
			
			base.OnPaintBackground (ebe);
			using (Brush back = new SolidBrush(backgroundColor)) 
			{
				ebe.Graphics.FillRectangle(back, 0, 0, Width, Height);
			}
			//throw new NotImplementedException("OnPaintBackground");
		}

		[TODO]
		protected virtual void OnParentRowsLabelStyleChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnParentRowsLabelStyleChanged");
		}

		[TODO]
		protected virtual void OnParentRowsVisibleChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnParentRowsVisibleChanged");
		}

		[TODO]
		protected virtual void OnReadOnlyChanged(System.EventArgs e)
		{
			throw new NotImplementedException("OnReadOnlyChanged");
		}

		[TODO]
		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);
		}

		[TODO]
		protected void OnRowHeaderClick(System.EventArgs e)
		{
			throw new NotImplementedException("OnRowHeaderClick");
		}

		[TODO]
		protected void OnScroll(System.EventArgs e)
		{
			throw new NotImplementedException("OnScroll");
		}

		[TODO]
		protected void OnShowParentDetailsButtonClicked(System.Object sender, System.EventArgs e)
		{
			throw new NotImplementedException("OnShowParentDetailsButtonClicked");
		}

		[TODO]
		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
		{
			return false;
		}

		[TODO]
		protected bool ProcessGridKey(System.Windows.Forms.KeyEventArgs ke)
		{
			throw new NotImplementedException("ProcessGridKey");
		}

		[TODO]
		protected override bool ProcessKeyPreview(ref Message m)
		{
			throw new NotImplementedException("ProcessKeyPreview");
		}

		[TODO]
		protected bool ProcessTabKey(System.Windows.Forms.Keys keyData)
		{
			throw new NotImplementedException("ProcessTabKey");
		}

		[TODO]
		public void ResetAlternatingBackColor()
		{
			throw new NotImplementedException("ResetAlternatingBackColor");
		}

		[TODO]
		public override void ResetBackColor()
		{
			throw new NotImplementedException("ResetBackColor");
		}

		[TODO]
		public override void ResetForeColor()
		{
			throw new NotImplementedException("ResetForeColor");
		}

		[TODO]
		public void ResetGridLineColor()
		{
			throw new NotImplementedException("ResetGridLineColor");
		}

		[TODO]
		public void ResetHeaderBackColor()
		{
			throw new NotImplementedException("ResetHeaderBackColor");
		}

		[TODO]
		public void ResetHeaderFont()
		{
			throw new NotImplementedException("ResetHeaderFont");
		}

		[TODO]
		public void ResetHeaderForeColor()
		{
			throw new NotImplementedException("ResetHeaderForeColor");
		}

		[TODO]
		public void ResetLinkColor()
		{
			throw new NotImplementedException("ResetLinkColor");
		}

		[TODO]
		public void ResetLinkHoverColor()
		{
			throw new NotImplementedException("ResetLinkHoverColor");
		}

		[TODO]
		protected void ResetSelection()
		{
			throw new NotImplementedException("ResetSelection");
		}

		[TODO]
		public void ResetSelectionBackColor()
		{
			throw new NotImplementedException("ResetSelectionBackColor");
		}

		[TODO]
		public void ResetSelectionForeColor()
		{
			throw new NotImplementedException("ResetSelectionForeColor");
		}

		[TODO]
		public void Select(int row)
		{
			throw new NotImplementedException("Select");
		}

		[TODO]
		public void SetDataBinding(System.Object dataSource, System.String dataMember)
		{
			/*if(( dataSource.GetType() != "System.Data.DataSet" )|| ( dataSource.GetType() != "")) {
				throw new ArgumentException("ArgumentException");
			}*/
			switch(dataSource.GetType().ToString()) 
			{
				case "djoka":
//					Console.WriteLine("asd");
					break;
				case "sdasdas":
//					Console.WriteLine("asdasdasd");
					break;
				default:
//					Console.WriteLine("asdasd");
					break;
			}
			this.DataSource = dataSource;
			this.DataMember = dataMember;

//			Console.WriteLine(dataSource.GetType());
//			Console.WriteLine(dataMember.ToString());
			//throw new NotImplementedException("SetDataBinding");
		}

		[TODO]
		protected virtual bool ShouldSerializeAlternatingBackColor()
		{
			throw new NotImplementedException("ShouldSerializeAlternatingBackColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeBackgroundColor()
		{
			throw new NotImplementedException("ShouldSerializeBackgroundColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeCaptionBackColor()
		{
			throw new NotImplementedException("ShouldSerializeCaptionBackColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeCaptionForeColor()
		{
			throw new NotImplementedException("ShouldSerializeCaptionForeColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeGridLineColor()
		{
			throw new NotImplementedException("ShouldSerializeGridLineColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeHeaderBackColor()
		{
			throw new NotImplementedException("ShouldSerializeHeaderBackColor");
		}

		[TODO]
		protected bool ShouldSerializeHeaderFont()
		{
			throw new NotImplementedException("ShouldSerializeHeaderFont");
		}

		[TODO]
		protected virtual bool ShouldSerializeHeaderForeColor()
		{
			throw new NotImplementedException("ShouldSerializeHeaderForeColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeLinkHoverColor()
		{
			throw new NotImplementedException("ShouldSerializeLinkHoverColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeParentRowsBackColor()
		{
			throw new NotImplementedException("ShouldSerializeParentRowsBackColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeParentRowsForeColor()
		{
			throw new NotImplementedException("ShouldSerializeParentRowsForeColor");
		}

		[TODO]
		protected bool ShouldSerializePreferredRowHeight()
		{
			throw new NotImplementedException("ShouldSerializePreferredRowHeight");
		}

		[TODO]
		protected bool ShouldSerializeSelectionBackColor()
		{
			throw new NotImplementedException("ShouldSerializeSelectionBackColor");
		}

		[TODO]
		protected virtual bool ShouldSerializeSelectionForeColor()
		{
			throw new NotImplementedException("ShouldSerializeSelectionForeColor");
		}

		[TODO]
		public void SubObjectsSiteChange(bool site)
		{
			throw new NotImplementedException("SubObjectsSiteChange");
		}

		[TODO]
		public void UnSelect(int row)
		{
			throw new NotImplementedException("UnSelect");
		}
/////////////////////////////////////////////////////////////////////////

		public event EventHandler AllowNavigationChanged
		{
				add
				{
					AddHandler(EventId.AllowNavigationChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.AllowNavigationChanged,value);
				}
		}

		public event EventHandler BackButtonClick
		{
				add
				{
					AddHandler(EventId.BackButtonClick,value);
				}
				remove
				{
					RemoveHandler(EventId.BackButtonClick,value);
				}
		}

		public event EventHandler BackgroundColorChanged
		{
				add
				{
					AddHandler(EventId.BackgroundColorChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.BackgroundColorChanged,value);
				}
		}

		public new event EventHandler BackgroundImageChanged
		{
				add
				{
					base.BackgroundImageChanged += value;
				}
				remove
				{
					base.BackgroundImageChanged -= value;
				}
		}

		public event EventHandler BorderStyleChanged
		{
				add
				{
					AddHandler(EventId.BorderStyleChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.BorderStyleChanged,value);
				}
		}

		public event EventHandler CaptionVisibleChanged
		{
				add
				{
					AddHandler(EventId.CaptionVisibleChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.CaptionVisibleChanged,value);
				}
		}

		public event EventHandler CurrentCellChanged
		{
				add
				{
					AddHandler(EventId.CurrentCellChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.CurrentCellChanged,value);
				}
		}

		public new event EventHandler CursorChanged
		{
				add
				{
					base.CursorChanged += value;
				}
				remove
				{
					base.CursorChanged -= value;
				}
		}

		public event EventHandler DataSourceChanged
		{
				add
				{
					AddHandler(EventId.DataSourceChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.DataSourceChanged,value);
				}
		}

		public event EventHandler FlatModeChanged
		{
				add
				{
					AddHandler(EventId.FlatModeChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.FlatModeChanged,value);
				}
		}

		public event EventHandler Navigate
		{
				add
				{
					AddHandler(EventId.Navigate,value);
				}
				remove
				{
					RemoveHandler(EventId.Navigate,value);
				}
		}

		public event EventHandler ParentRowsLabelStyleChanged
		{
				add
				{
					AddHandler(EventId.ParentRowsLabelStyleChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.ParentRowsLabelStyleChanged,value);
				}
		}

		public event EventHandler ParentRowsVisibleChanged
		{
				add
				{
					AddHandler(EventId.ParentRowsVisibleChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.ParentRowsVisibleChanged,value);
				}
		}

		public event EventHandler ReadOnlyChanged
		{
				add
				{
					AddHandler(EventId.ReadOnlyChanged,value);
				}
				remove
				{
					RemoveHandler(EventId.ReadOnlyChanged,value);
				}
		}

		public event EventHandler RowHeaderClick
		{
				add
				{
					AddHandler(EventId.RowHeaderClick,value);
				}
				remove
				{
					RemoveHandler(EventId.RowHeaderClick,value);
				}
		}

		public event EventHandler Scroll
		{
				add
				{
					AddHandler(EventId.Scroll,value);
				}
				remove
				{
					RemoveHandler(EventId.Scroll,value);
				}
		}

		public event EventHandler ShowParentDetailsButtonClick
		{
				add
				{
					AddHandler(EventId.ShowParentDetailsButtonClick,value);
				}
				remove
				{
					RemoveHandler(EventId.ShowParentDetailsButtonClick,value);
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

/////////////////////////////////////


		[TODO]
		public bool AllowNavigation 
		{
 			get
			{
				return this.allowNavigation;
			}

 			set
			{
 				if(this.allowNavigation != value) {
 					this.allowNavigation = value;
 					// OnAllowNavigationChanged event should be fired
 				}
			}

 		}

		[TODO]
		public bool AllowSorting 
		{
 			get
			{
				return this.allowSorting;
			}

 			set
			{
				this.allowSorting = value;
			}

 		}

		[TODO]
		public System.Drawing.Color AlternatingBackColor 
		{
 			get
			{
				return this.alternatingBackColor;
			}

 			set
			{
				this.alternatingBackColor = value;
			}

 		}

		public override System.Drawing.Color BackColor 
		{
 			get
			{
				return this.dgbackColor;
			}

 			set
			{
				this.dgbackColor = value;
			}

 		}

		[TODO]
		public System.Drawing.Color BackgroundColor 
		{
 			get
			{
				return this.backgroundColor;
			}

 			set
			{
				this.backgroundColor = value;
			}

 		}

		[TODO]
		public override System.Drawing.Image BackgroundImage 
		{
 			get
			{
				return this.backgroundImage;
			}

 			set
			{
				this.backgroundImage = value;
			}

 		}

		[TODO]
		public BorderStyle BorderStyle 
		{
 			get
			{
				return this.borderStyle;
			}

 			set
			{
				this.borderStyle = value;
			}

 		}

		[TODO]
		public System.Drawing.Color CaptionBackColor 
		{
 			get
			{
				return this.captionBackColor;
			}

 			set
			{
				this.captionBackColor = value;
			}

 		}

		[TODO]
		public System.Drawing.Font CaptionFont 
		{
 			get
			{
				return this.captionFont;
			}

 			set
			{
				this.captionFont = value;
			}

 		}

		[TODO]
		public System.Drawing.Color CaptionForeColor 
		{
 			get
			{
				return this.captionForeColor;
			}

 			set
			{
				this.captionForeColor = value;
			}

 		}

		[TODO]
		public System.String CaptionText
		{
 			get
			{
				return this.captionText;
			}

 			set
			{
				this.captionText = value;
			}

 		}

		[TODO]
		public bool CaptionVisible
		{
 			get
			{
				return this.captionVisible;
			}

 			set
			{
				this.captionVisible = value;
			}

 		}

		[TODO]
		public bool ColumnHeadersVisible 
		{
 			get
			{
				return this.columnHeadersVisible;
			}

 			set
			{
				this.columnHeadersVisible = value;
			}

 		}

		[TODO]
		public DataGridCell CurrentCell 
		{
 			get
			{
				throw new NotImplementedException("CurrentCell");
			}

 			set
			{
				throw new NotImplementedException("CurrentCell");
			}

 		}

		[TODO]
		public int CurrentRowIndex 
		{
 			get
			{
				return this.currentRowIndex;
			}

 			set
			{
				this.currentRowIndex = value;
			}

 		}

		[TODO]
		public override Cursor Cursor 
		{
 			get
			{
				throw new NotImplementedException("Cursor");
			}

 			set
			{
				throw new NotImplementedException("Cursor");
			}

 		}

		public String DataMember 
		{
 			get
			{
				return this.dataMember;
			}

 			set
			{
				this.dataMember = value;
			}

 		}

		[TODO]
		public System.Object DataSource 
		{
 			get
			{
				return this.dataSource;
			}

 			set
			{
				this.dataSource = value;
			}

 		}

		protected override System.Drawing.Size DefaultSize 
		{
 			get
			{
				return new System.Drawing.Size(130, 80);
			}

 		}

		[TODO]
		public int FirstVisibleColumn 
		{
 			get
			{
				throw new NotImplementedException("FirstVisibleColumn");
			}

 		}

		[TODO]
		public bool FlatMode 
		{
 			get
			{
				throw new NotImplementedException("FlatMode");
			}

 			set
			{
				throw new NotImplementedException("FlatMode");
			}

 		}

		[TODO]
		public override System.Drawing.Color ForeColor 
		{
 			get
			{
				return this.dgforeColor;
			}

 			set
			{
				this.dgforeColor = value;
			}

 		}

		[TODO]
		public System.Drawing.Color GridLineColor 
		{
 			get
			{
				return this.gridLineColor;
			}

 			set
			{
				this.gridLineColor = value;
			}

 		}

		[TODO]
		public DataGridLineStyle GridLineStyle 
		{
 			get
			{
				throw new NotImplementedException("GridLineStyle");
			}

 			set
			{
				throw new NotImplementedException("GridLineStyle");
			}

 		}

		[TODO]
		public System.Drawing.Color HeaderBackColor 
		{
 			get
			{
				return this.headerBackColor;
			}

 			set
			{
				this.headerBackColor = value;
			}

 		}

		[TODO]
		public System.Drawing.Font HeaderFont 
		{
 			get
			{
				return this.headerFont;
			}

 			set
			{
				this.headerFont = value;
			}

 		}

		public System.Drawing.Color HeaderForeColor 
		{
 			get
			{
				return headerForeColor;
			}

 			set
			{
				headerForeColor = value;
			}

 		}

		[TODO]
		protected ScrollBar HorizScrollBar 
		{
 			get
			{
				throw new NotImplementedException("HorizScrollBar");
			}

 		}

		[TODO]
		public System.Object this[int rowIndex, int columnIndex] 
		{
 			get
			{
				throw new NotImplementedException("Item");
			}

 			set
			{
				throw new NotImplementedException("Item");
			}

 		}

		[TODO]
		public System.Object this[DataGridCell cell] 
		{
 			get
			{
				throw new NotImplementedException("Item");
			}

 			set
			{
				throw new NotImplementedException("Item");
			}

 		}

		[TODO]
		public System.Drawing.Color LinkColor 
		{
 			get
			{
				throw new NotImplementedException("LinkColor");
			}

 			set
			{
				throw new NotImplementedException("LinkColor");
			}

 		}

		[TODO]
		public System.Drawing.Color LinkHoverColor 
		{
 			get
			{
				throw new NotImplementedException("LinkHoverColor");
			}

 			set
			{
				throw new NotImplementedException("LinkHoverColor");
			}

 		}

		[TODO]
		protected internal CurrencyManager ListManager 
		{
 			get
			{
				throw new NotImplementedException("ListManager");
			}

 			set
			{
				throw new NotImplementedException("ListManager");
			}

 		}

		[TODO]
		public System.Drawing.Color ParentRowsBackColor 
		{
 			get
			{
				throw new NotImplementedException("ParentRowsBackColor");
			}

 			set
			{
				throw new NotImplementedException("ParentRowsBackColor");
			}

 		}

		[TODO]
		public System.Drawing.Color ParentRowsForeColor 
		{
 			get
			{
				throw new NotImplementedException("ParentRowsForeColor");
			}

 			set
			{
				throw new NotImplementedException("ParentRowsForeColor");
			}

 		}

		[TODO]
		public DataGridParentRowsLabelStyle ParentRowsLabelStyle 
		{
 			get
			{
				throw new NotImplementedException("ParentRowsLabelStyle");
			}

 			set
			{
				throw new NotImplementedException("ParentRowsLabelStyle");
			}

 		}

		[TODO]
		public bool ParentRowsVisible 
		{
 			get
			{
				throw new NotImplementedException("ParentRowsVisible");
			}

 			set
			{
				throw new NotImplementedException("ParentRowsVisible");
			}

 		}

		[TODO]
		public int PreferredColumnWidth 
		{
 			get
			{
				return this.preferredColumnWidth;
			}

 			set
			{
				this.preferredColumnWidth = value;
			}

 		}

		[TODO]
		public int PreferredRowHeight 
		{
 			get
			{
				return this.preferredRowHeight;
			}

 			set
			{
				this.preferredRowHeight = value;
			}

 		}

		[TODO]
		public bool ReadOnly 
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
		public int RowHeaderWidth 
		{
 			get
			{
				return this.rowHeaderWidth;
			}

 			set
			{
				this.rowHeaderWidth = value;
			}

 		}

		[TODO]
		public bool RowHeadersVisible 
		{
 			get
			{
				return this.rowHeadersVisible;
			}

 			set
			{
				this.rowHeadersVisible = value;
			}

 		}

		[TODO]
		public System.Drawing.Color SelectionBackColor 
		{
 			get
			{
				throw new NotImplementedException("SelectionBackColor");
			}

 			set
			{
				throw new NotImplementedException("SelectionBackColor");
			}

 		}

		[TODO]
		public System.Drawing.Color SelectionForeColor 
		{
 			get
			{
				throw new NotImplementedException("SelectionForeColor");
			}

 			set
			{
				throw new NotImplementedException("SelectionForeColor");
			}

 		}

		[TODO]
		public override System.ComponentModel.ISite Site 
		{
 			get
			{
				throw new NotImplementedException("Site");
			}

 			set
			{
				throw new NotImplementedException("Site");
			}

 		}

		[TODO]
		public GridTableStylesCollection TableStyles 
		{
 			get
			{	
				return this.gridTableStylesCollection;
			}

 		}

		public override System.String Text
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

		[TODO]
		protected ScrollBar VertScrollBar 
		{
 			get
			{
				throw new NotImplementedException("VertScrollBar");
			}

 		}

		[TODO]
		public int VisibleColumnCount 
		{
 			get
			{
				throw new NotImplementedException("VisibleColumnCount");
			}

 		}

		[TODO]
		public int VisibleRowCount 
		{
 			get
			{
				throw new NotImplementedException("VisibleRowCount");
			}

 		}

 		[Flags]
		[Serializable]
		public enum HitTestType
		{
			Caption			= 32,
			Cell			= 1,
			ColumnHeader	= 2,
			ColumnResize	= 8,
			None			= 0,
			ParentRows		= 64,
			RowHeader		= 4,
			RowResize		= 16
		}

		public sealed class HitTestInfo
		{
			internal int column;
			internal int row;
		
			public static readonly DataGrid.HitTestInfo Nowhere = null;

			[TODO]
			public int Column 
			{
				get 
				{
					throw new NotImplementedException("Column");
				}
			}
		
			[TODO]
			public int Row
			{
				get
				{
					throw new NotImplementedException("Row");
				}
			}
			
			[TODO]
			public DataGrid.HitTestType Type 
			{
				get
				{
					throw new NotImplementedException("Type");
				}
			}
		
			[TODO]
			public override bool Equals( object value)
			{
				throw new NotImplementedException("Equals");
			}
		
			[TODO]
			public override int GetHashCode()
			{
				throw new NotImplementedException("GetHashCode");
			}
		
			[TODO]
			public override string ToString()
			{
				throw new NotImplementedException("ToString");
			}
		} //HitTestInfo
	}

}//namespace System.Windows.Forms

