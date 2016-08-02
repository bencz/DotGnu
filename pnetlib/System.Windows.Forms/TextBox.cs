/*
 * TextBox.cs - Implementation of the
 *			"System.Windows.Forms.TextBox" class.
 *
 * Copyright (C) 2003 Neil Cawse
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

#if CONFIG_COMPONENT_MODEL
using System.ComponentModel;
using System.Reflection;
#endif

public class TextBox : TextBoxBase
{
	private bool acceptsReturn;
	private char passwordChar;
	private CharacterCasing characterCasing = CharacterCasing.Normal;
	private ScrollBars scrollBars;
	private HorizontalAlignment textAlign;

	private bool mouseDown;
	private Pen backPen;
	private bool inTextChangedEvent;
	private VScrollBar vScrollBar;
	private HScrollBar hScrollBar;
	private Region invalidateRegion;
	
	// The position and drawing information for each item
	private LayoutInfo layout;
	// Start of a selection
	private int selectionStartActual;
	// Length of the selection, could be negative
	private int selectionLengthActual;
	// A region of the text that is selected
	private Region selectedRegion;
	
	// Whether the flashing caret is currently hiding
	private bool caretHiding = true;
	// Position to draw caret
	private Rectangle caretBounds;
	private Pen caretPen;
	private int caretPosition = 0;

	// Maximum possible X/Y for a region
	private const int maxXY = 4194304;

	// XY offset of the view for text bigger than the text area
	private int xViewOffset;
	private int yViewOffset;

	// Height chosen, if not multiline could be different from actual
	private int chosenHeight;


	// Binding PropertyInfo for DataBinding of Text.
	private PropertyInfo bindInfo = null;
	
	// Variables to work around scrollbar visibility problem
	private bool showVScrollBar;
	private bool showHScrollBar;

	private int graphicsOffset;

	[TODO]
	public TextBox()
	{
		// Trap interesting events.  We do it this way rather
		// than override virtual methods so that the published
		// TextBox API is maintained.
		// Note: except for KeyPress events, for which the 
		// hooked up calls get priority over TextBox class.
		
#if CONFIG_COMPONENT_MODEL
		this.DataBindings.CollectionChanged +=new CollectionChangeEventHandler(HandleDataBindingCollectionChanged);
#endif
		
		textAlign = HorizontalAlignment.Left;

		BackColor = SystemColors.Window;
		ForeColor = SystemColors.WindowText;

		// Fix: Get this value from SystemInformation
		CaretSetPosition(0);

		// Cache the Pen - check what color this should be
		caretPen = new Pen(SystemColors.Highlight);
		Cursor = Cursors.IBeam;
		// We will erase the background.
		SetStyle(ControlStyles.Opaque, true);
		// Switch on double buffering.
		SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
	
		// we need this or no text is displayed after first start
		// this.CreateHandle(); // do we really need this ?????
		
	}

	public override Cursor Cursor
	{
		set
		{
			if( value == null ) base.Cursor = Cursors.IBeam;
			else                base.Cursor = value;
		}
	}
	
// Gets or sets a value indicating whether pressing ENTER in a multiline TextBox control creates a new line of text in the control or activates the default button for the form.
	public bool AcceptsReturn
	{
		get
		{
			return acceptsReturn;
		}
		set
		{
			acceptsReturn = value;
		}
	}

	// Handle this
	// Gets or sets whether the TextBox control modifies the case of characters as they are typed.
	public CharacterCasing CharacterCasing
	{
		get
		{
			return characterCasing;
		}
		set
		{
			characterCasing = value;
		}
	}

	[TODO]
	// Handle this
	protected override ImeMode DefaultImeMode
	{
		get
		{
			if(passwordChar != '\0')
			{
				return base.DefaultImeMode;
			}
			else
			{
				return ImeMode.Disable;
			}
		}
	}

	public char PasswordChar
	{
		get
		{
			return passwordChar;
		}
		set
		{
			passwordChar = value;
		}
	}

	[TODO]
	// Handle this
	public ScrollBars ScrollBars
	{
		get
		{
			return scrollBars;
		}
		set
		{
			if (scrollBars == value)
			{
				return;
			}
			scrollBars = value;
			layout = null;
		}
	}

	// Set up the scrollbars. Returns whether SetScrollBarPositions has to be called as well.
	private bool SetupScrollBars()
	{
		// Do we need to call SetScrollBarPositions ?
		bool callPositions = false;
		
		// Set vertical scrollbar
		// There is no vertical scrollbar if the textbox is not multiline
		if (Multiline && (scrollBars == ScrollBars.Both || scrollBars == ScrollBars.Vertical))
		{
			if (vScrollBar == null)
			{
				vScrollBar = new VScrollBar();
				vScrollBar.backColor = SystemColors.ScrollBar;
				vScrollBar.ValueChanged+=new EventHandler(vScrollBar_ValueChanged);
				vScrollBar.Maximum = 0;
				callPositions = true;
				Controls.Add(vScrollBar);
			}
		}
		else if (vScrollBar != null)
		{
			vScrollBar.ValueChanged-=new EventHandler(vScrollBar_ValueChanged);
			Controls.Remove(vScrollBar);
			vScrollBar = null;
		}
		// Set horizontal scrollbar
		// There is no hrizontal scrollbar if WordWrap is true
		if (!WordWrap && (scrollBars == ScrollBars.Both || scrollBars == ScrollBars.Horizontal))
		{
			if (hScrollBar == null)
			{
				hScrollBar = new HScrollBar();
				hScrollBar.backColor = SystemColors.ScrollBar;
				hScrollBar.ValueChanged+=new EventHandler(hScrollBar_ValueChanged);
				hScrollBar.Maximum = 0;
				callPositions = true;
				Controls.Add(hScrollBar);
			}
		}
		else if (hScrollBar != null)
		{
			hScrollBar.ValueChanged-=new EventHandler(hScrollBar_ValueChanged);
			Controls.Remove(hScrollBar);
			hScrollBar = null;
		}

		SetScrollBarVisibility ();
		
		return callPositions;
	}

	// Setup the positions of the scrollBar depending on the combination
	private void SetScrollBarPositions()
	{
		if (vScrollBar == null && hScrollBar == null)
		{
			return;
		}
		
		int width = ClientRectangle.Width;
		if (vScrollBar != null && showVScrollBar)
			{
			width -= vScrollBar.Width;
		}

		int height = ClientRectangle.Height;
		if (hScrollBar != null && showHScrollBar)
			{
			height -= hScrollBar.Height;
		}


		if (vScrollBar != null)
		{
			vScrollBar.Bounds = new Rectangle(ClientRectangle.Width - vScrollBar.Width, 0, vScrollBar.Width, height);
			
			int remainder = TextDrawArea.Height % Font.Height;
			int maximum = MaxTextDimensions.Height + remainder;
			
			if (maximum < TextDrawArea.Height)
			{
				maximum = TextDrawArea.Height;
			}
			vScrollBar.Enabled = (maximum != TextDrawArea.Height);
			vScrollBar.Maximum = maximum;
			vScrollBar.SmallChange = Font.Height;
			vScrollBar.LargeChange = TextDrawArea.Height + 1;
		}
		if (hScrollBar != null)
		{
			hScrollBar.Bounds = new Rectangle(0, ClientRectangle.Height - hScrollBar.Height, width, hScrollBar.Height);
			int maximum = MaxTextDimensions.Width;
			if (maximum < TextDrawArea.Width)
			{
				maximum = TextDrawArea.Width;
			}
			hScrollBar.Enabled = (maximum != TextDrawArea.Width);
			hScrollBar.Maximum = maximum;
			hScrollBar.SmallChange = 5;
			hScrollBar.LargeChange = TextDrawArea.Width + 1;
		}
	}

	// Set whether the scrollbars are visible.
	private void SetScrollBarVisibility()
	{
		if (vScrollBar == null && hScrollBar == null)
		{
			return;
		}
		
		if (vScrollBar != null)
		{
			showVScrollBar = (ClientRectangle.Width - vScrollBar.Width >= 5);
			vScrollBar.Visible = showVScrollBar;
		}

		if (hScrollBar != null)
		{
			showHScrollBar = (ClientRectangle.Height - hScrollBar.Height >= 5);
			hScrollBar.Visible = showHScrollBar;
		}
	}

	[TODO]
	private Size MaxTextDimensions
	{
		get
		{
			// Fix: handle case right to left
			int x = 0;
			int y = 0;
			if (Text.Length > 0)
			{
				y = layout.Items[Text.Length - 1].bounds.Bottom;
				x = layout.Items[Text.Length - 1].bounds.Right;
			}
			return new Size(x, y);
		}
	}

	private void vScrollBar_ValueChanged(object sender, EventArgs e)
	{
		YViewOffset = vScrollBar.Value;
	}

	private void hScrollBar_ValueChanged(object sender, EventArgs e)
	{
		XViewOffset = hScrollBar.Value;
	}

	public override int SelectionLength
	{
		get
		{
			return base.SelectionLength;
		}
		set
		{
			base.SelectionLength = value;
		}
	}

	public override String Text
	{
		get
		{
			string text = base.Text;
			if (text == null)
			{
				return string.Empty;
			}
			else
			{
				return text;
			}
		}
		set
		{
			if( base.Text == value ) {	// check if text has changed
				return;
			}
			
			if( value == null ) value = string.Empty;

			if( value.Length > 0 && value.IndexOfAny( new char [] { '\n', '\r' } ) >= 0 ) {	// check if chars are in use, before use of StringBuilder (performance)
			
				// Change all text endings of CR or LF into CRLF
				System.Text.StringBuilder sb = new System.Text.StringBuilder( value.Length);
				char cPrevious = (char)0;
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
					bool isNext = (i < value.Length - 1);
					char cNext = (char)0;
					if (isNext)
					{
						cNext = value[i+1];
					}
					if ((!isNext || cNext != '\n') && c == '\r')
					{
						sb.Append("\r\n");
					}
					else if (c=='\n' && cPrevious != '\r')
					{
						sb.Append("\r\n");
					}
					else
					{
						sb.Append(c);
					}
					cPrevious = c;
				}
				
				SetTextActual( sb.ToString());
			}
			else {
				SetTextActual( value );
			}
			
			// Set the position to the end
			SelectInternal(Text.Length, 0);
			if (!inTextChangedEvent)
			{
				if (IsHandleCreated)
				{
					CaretSetEndSelection();
					ResetView();
					InvalidateDirty();
				}
				OnTextChanged(EventArgs.Empty);
			}

		}
	}

	public override String[] Lines
	{
		get
		{
			int line = 0;
			int y = -1;
			// Find the number of lines
			for (int i = 0; i < Text.Length; i++)
			{
				int currentY = layout.Items[i].bounds.Y;
				if (currentY != y)
				{
					line++;
					y = currentY;
				}
			}
			if(Text.Length != 0 && line == 0)
			{
				line++; // At least one line
			}
			string[] lines = new string[line];
			int start = 0;
			int j = 0;
			line = 0;
			// Break into strings
			while(j < Text.Length)
			{
				if (Text[j] == '\r' && j < Text.Length - 1 && Text[j+1] == '\n') // Look for CRLF
				{
					lines[line++] = Text.Substring(start, j - start);
					j+=2; 
					start = j;
				}
				else
				{
					j+=1;
				}
			}

			if(start < Text.Length)
			{
				lines[line++] = Text.Substring(start);
			}
			else if(start != 0 && start == Text.Length)
			{
				//FIXME: blank lines at end of text should be ""
			}
			
			return lines;
		}
		set
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(value != null)
			{
				foreach(string s in value)
				{
					sb.Append(s);
					sb.Append("\r\n");
				}
			}
			SetTextActual( sb.ToString());
			ResetView();
			InvalidateDirty();
			OnTextChanged(EventArgs.Empty);
		}
	}

	public HorizontalAlignment TextAlign
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
				if (IsHandleCreated)
				{
					// Layout changes
					LayoutFromText(Text);
					SetScrollBarPositions();
					ResetView();
				}
				OnTextAlignChanged(EventArgs.Empty);
			}
		}
	}

	// Event that is emitted when the text alignment changes.
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

	// Determine if a key is recognized by a control as an input key.
	protected override bool IsInputKey(Keys keyData)
	{
		// If this is a multi-line control, then determine
		// if we need to recognize the "Return" key.  We ignore
		// Alt-Return, but recognize Shift-Return and Ctrl-Return.
		if(Multiline)
		{
			if((keyData & Keys.Alt) == 0)
			{
				if((keyData & Keys.KeyCode) == Keys.Enter)
				{
					return acceptsReturn;
				}
			}
		}
		return base.IsInputKey(keyData);
	}

	// need to check for caret OnGotFocus too, since OnEnter might not be exexuted
	// because OnEnter only gets executed if ActiveControl is set.
	protected override void OnGotFocus(EventArgs e) {
		base.OnGotFocus(e);
		if( this.Focused ) {
		// reset the caret position
			CaretSetPosition(caretPosition);
		
		// Perform the regular focus handling.
			CaretShow();
			InvalidateDirty();
			this.Invalidate();
		}
	}
	
	// Process when the control receives the focus
	protected override void OnEnter(EventArgs e)
	{
		// reset the caret position
		CaretSetPosition(caretPosition);
		
		// Perform the regular focus handling.
		base.OnEnter(e);
		CaretShow();
		InvalidateDirty();
	}

	// Process when the control loses the focus
	protected override void OnLeave(EventArgs e)
	{
		base.OnLeave (e);
		// Create a region containing the caret and all visible selected text
		Region update = new Region(caretBounds);
		for (int i = 0; i < Text.Length; i++)
		{
			if (layout.Items[i].selected)
			{
				Rectangle b = layout.Items[i].bounds;
				update.Union(new Region(new Rectangle(b.Left,b.Top,b.Width,b.Height + 1)));
			}
		}
		update.Translate(- XViewOffset, - YViewOffset);
		AddUpdate(update);
		caretHiding = true;
		InvalidateDirty();
		mouseDown = false;
		// We dont need to update any selection
		if( null != selectedRegion ) {
			selectedRegion.Dispose();
			selectedRegion = null;
		}
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged (e);
		if (!Multiline && AutoSize)
		{
			Height = ClientToBounds(Size.Empty).Height + Font.Height;
		}
	}

	// Raise the "TextAlignChanged" event.
	protected virtual void OnTextAlignChanged(EventArgs e)
	{
		EventHandler handler;
		handler = (EventHandler)(GetHandler(EventId.TextAlignChanged));
		if(handler != null)
		{
			handler(this, e);
		}
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		base.OnKeyPress(e);
		
		if(e.Handled == false)
		{
			HandleKeyPress(this, e);
		}
	}

	// Handle "KeyPress" events for the text box.
	private void HandleKeyPress(Object sender, KeyPressEventArgs e)
	{
		if (ReadOnly || e.Handled)
		{
			return;
		}
			
		char c = e.KeyChar;

		// Discard enter if not multiline
		if (c=='\r' && !Multiline)
		{
			return;
		}

		// Discard control characters
		if (c<' ' && c!='\r')
		{
			return;
		}

		if (MaxLength>0 && Text.Length >= MaxLength)
		{
			return;
		}
		
		if (GetInsertMode()==InsertMode.Overwrite && GetSelectionLength()==0)
		{
			int startPos = GetSelectionStart();
			int endPos = ComputeCharRightPos(startPos);
			if (endPos > startPos)
				SelectInternal(startPos, endPos-startPos);
		}

		string strInsert;
		if (c=='\r')
		{
			strInsert = "\r\n";
		}
		else
		{
			strInsert = c.ToString();
		}
		SetSelectionText( strInsert );
		SelectInternal(SelectionStart + strInsert.Length, 0);
		CaretSetPosition(SelectionStart);
		ScrollToCaretNoRedraw();
		InvalidateDirty();
		OnTextChanged(EventArgs.Empty);
		e.Handled = true;

	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		ProcessMouse(e);
	}
	
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		ProcessMouse(e);
	}
	
	protected override void OnDoubleClick(EventArgs e)
	{
		base.OnDoubleClick(e);
		Point pt = PointToClient(MousePosition);
		ProcessMouse(new MouseEventArgs(MouseButtons.Left, 2, pt.X, pt.Y, 0));
	}
	
	// Handle "Paint" events for the text box.
	// In our implementation NO painting happens outside of the paint event. This might change because it might not update fast enough
	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		Redraw(e.Graphics);
	}
	
	// Redraw a specific portion of the textbox
	private void Redraw(Graphics g)
	{
		if (layout == null)
		{
			LayoutFromText(Text, g);
			ResetView();
		}
		// Draw scrollbar corner if both are visible
		if (vScrollBar != null && hScrollBar != null && showVScrollBar && showHScrollBar)
		{
			g.FillRectangle(SystemBrushes.Control, hScrollBar.Right, vScrollBar.Bottom, vScrollBar.Width, hScrollBar.Height);
		}
		// Only allow updates in the TextDrawArea
		using (Region clip = g.Clip)
		{
			clip.Intersect(new Region(TextDrawArea));
			g.SetClip(clip, Drawing.Drawing2D.CombineMode.Replace);
		}

		bool focused = Focused;
		
		if(!Enabled || ReadOnly)
		{
			// if an other color is used for BackColor than SystemColors.Window
			// use the BackColor to fill, even the control is disabled.
			Color col;
			if(BackColor == SystemColors.Window)
			{
				col = SystemColors.Control;
			}
			else
			{
				col = BackColor;
			}
			
			//using (Brush disabledBackBrush = new SolidBrush(SystemColors.Control))
			using(Brush disabledBackBrush = new SolidBrush(col))
			{
				g.FillRegion(disabledBackBrush, g.Clip);
			}
		}
		else
		{
			using(Brush backBrush = new SolidBrush(BackColor))
			{
				g.FillRegion(backBrush, g.Clip);
			}
		}
		// Draw the background of the selected text
		if (focused && selectedRegion != null)
		{
			Region r = selectedRegion.Clone();
			r.Translate(- XViewOffset, - YViewOffset);
			using (Brush selectedBackBrush = new SolidBrush(SystemColors.Highlight))
			{
				g.FillRegion(selectedBackBrush, r);
			}
		}
		DrawText(g, focused);
		if (focused)
		{
			CaretDraw(g);
		}
	}

	// Handle the event when multiline is changed.
	protected override void OnMultilineChanged(EventArgs e)
	{
		base.OnMultilineChanged(e);
		layout = null;
		// Set back the actual chosen height
		// Will cause LayoutFromText to be called
		Height = chosenHeight;
	}

#if CONFIG_COMPONENT_MODEL	
	[TODO]
	private void HandleDataBindingCollectionChanged(object sender, CollectionChangeEventArgs E)
	{
		Binding binding = (Binding)E.Element;
		
		switch(E.Action)
		{
			case CollectionChangeAction.Add:
				binding.PullData();
				binding.PushData();
				break;
			case CollectionChangeAction.Refresh:
				// Fix: What do we do here?
				break;
			case CollectionChangeAction.Remove:
				// Fix: Not sure here either?
				break;
		}	
	}
#endif	
	
	private void LayoutFromText(String newText)
	{
		using (Graphics g = CreateGraphics())
		{
			LayoutFromText(newText, g);
		}
	}

	// Create the drawLayout from the text
	// All rendered in client coordinates.
	protected void LayoutFromText(String newText, Graphics g)
	{
		bool callSetScrollBarPositions;
		
		if (!IsHandleCreated)
			return;
		if (layout == null)
		{
			layout = new LayoutInfo();
			layout.Items = new LayoutInfo.Item[0];
		}

		// Optimization - only re-layout from the beginning of the last line modified.
		// Find posLine, the position of the beginning of the line we must start updating from.
		// yLine is the y coordinate of this point.
		int yLine = 1;
		int posLine = 0;

		for (int i = 0; i < layout.Items.Length; i++)
		{
			if (i > newText.Length - 1)
				break;
			
			if (layout.Items[i].bounds.Top != yLine)
			{
				posLine = i;
				yLine = layout.Items[i].bounds.Top;
			}
			if (newText[i] != Text[i])
				break;
		}

		// Set up the scrollbars and remember whether we need to call SetScrollBarPositions.
		// We need to do this here so TextDrawArea returns correct values.
		callSetScrollBarPositions = SetupScrollBars();

		graphicsOffset = g.MeasureCharacters("A", Font, new Rectangle(0,0, 100, 100), new StringFormat())[0].X;

		// We leave 1 pixel on the left and right for the caret
		// Multiline textboxes are infinite in the y direction
		// non multiline are infinite in the x direction and we scroll when needed
		// This is the area we need to lay the text into.
		Rectangle measureBounds;
		if (Multiline)
		{
			measureBounds = new Rectangle(1, yLine - 1, TextDrawArea.Width - 2, maxXY - (yLine - 1));
		}
		else
		{
			measureBounds = new Rectangle(1 - graphicsOffset, yLine - 1, maxXY + graphicsOffset - 1, TextDrawArea.Height - (yLine - 1));
		}

		string measureText = newText.Substring(posLine);

		// Convert the control settings to a StringFormat
		StringFormat format = new StringFormat();
		if (RightToLeft == RightToLeft.Yes)
		{
			format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
		}
		if (!Multiline)
		{
			format.FormatFlags |= StringFormatFlags.NoWrap;
		}
		if (textAlign == HorizontalAlignment.Left)
		{
			format.Alignment = StringAlignment.Near;
		}
		else if (textAlign == HorizontalAlignment.Right)
		{
			format.Alignment = StringAlignment.Far;
		}
		else if (textAlign == HorizontalAlignment.Center)
		{
			format.Alignment = StringAlignment.Center;
		}
	
		Rectangle[] bounds;
		if (measureText.Length == 0)
		{
			bounds = new Rectangle[0];
		}
		else if (passwordChar == 0)
		{
			bounds = g.MeasureCharacters(measureText, Font, measureBounds, format);
		}
		else
		{
			bounds = g.MeasureCharacters(new string(passwordChar, measureText.Length), Font, measureBounds, format);
		}
		LayoutInfo.Item[] newItems = new LayoutInfo.Item[newText.Length];
		// Copy in the previously measured items.
		Array.Copy(layout.Items, 0, newItems, 0, posLine);
		layout.Items = newItems;
		
		// Convert the MeasureCharacterRanges to LayoutInfo
		// MeasureCharacterRanges will return an empty rectangle for all characters
		// that are not visible. We need to figure out the positions of LF and spaces
		// that are swallowed
		Rectangle prevBounds;
		if (posLine == 0)
		{
			prevBounds = new Rectangle(CaretXFromAlign, 0, 0, Font.Height);
		}
		else
		{
			prevBounds = layout.Items[posLine - 1].bounds;
		}
		LayoutInfo.Item.CharType prevType = LayoutInfo.Item.CharType.VisibleChar;
		for (int i = posLine; i < newText.Length;i++)
		{
			LayoutInfo.Item item = new LayoutInfo.Item();
			char c = newText[i];
			Rectangle rect = bounds[i - posLine];
			if (c == '\r')
			{
				item.type = LayoutInfo.Item.CharType.CR;
				// Return. The bounds is to the right of the previous character.
				// If the previous character was also a linefeed, we move down a line
				// from the previous bounds.
				if (i > 0 && newText[i-1] == '\n')
				{
					rect = new Rectangle(CaretXFromAlign, prevBounds.Top + Font.Height, 0, Font.Height);
				}
				else
				{
					rect = new Rectangle(prevBounds.Right, prevBounds.Top, 2, prevBounds.Height);
				}
			}
			else if (c == '\n')
			{
				item.type = LayoutInfo.Item.CharType.LF;
				rect = new Rectangle(prevBounds.Right, prevBounds.Top, 2, prevBounds.Height);
				// give this LF a non-empty bounds so that it appears in a selection
			}
			else // c != CR, LF
			{
				item.type = LayoutInfo.Item.CharType.VisibleChar;
			}
			
			if (rect.IsEmpty)
			{
				item.type = LayoutInfo.Item.CharType.OutOfBoundsChar;
				// Look for spaces that are swallowed
				if (c == ' ' && prevType == LayoutInfo.Item.CharType.VisibleChar)
				{
					rect = new Rectangle(prevBounds.Right, prevBounds.Top, 0, prevBounds.Height);
				}
			}
			item.bounds = prevBounds = rect;
			layout.Items[i] = item;
			prevType = item.type;
		}
		
		// Set the positions of the scrollbars if SetupScrollBars told us to do so
		if (callSetScrollBarPositions)
		    SetScrollBarPositions ();
	}

	// Make sure the caret is visible
	protected override void ScrollToCaretInternal()
	{
		ScrollToCaretNoRedraw();
		InvalidateDirty();
	}

	// Called to recalculate the offsets if make sure bounds is visible.
	private void ScrollToCaretNoRedraw()
	{
		if (caretBounds.Top - YViewOffset < TextDrawArea.Top)
		{
			YViewOffset = caretBounds.Top;
		}
		else if (caretBounds.Bottom - YViewOffset > TextDrawArea.Bottom)
		{
			YViewOffset = caretBounds.Bottom - TextDrawArea.Bottom + 1;
		}
		if (caretBounds.Left- XViewOffset < TextDrawArea.Left)
		{
			XViewOffset = caretBounds.Left - 1;
		}
		else if (caretBounds.Right - XViewOffset > TextDrawArea.Right)
		{
			XViewOffset = caretBounds.Right - TextDrawArea.Right;
		}
	}

	// When changing text, bounds or alignment, we make sure the view includes the position of the first character.
	private void ResetView()
	{
		XViewOffset = 0;
		YViewOffset = 0;
		//Set caret to the beginning
		CaretSetPosition(0);
		if (!Multiline)
		{
			if (textAlign == HorizontalAlignment.Center)
			{
				XViewOffset = maxXY / 2 - TextDrawArea.Width / 2;
			}
			else if (textAlign == HorizontalAlignment.Right)
			{
				XViewOffset = maxXY - TextDrawArea.Width - graphicsOffset + 1;
			}
		}
		else
			ScrollToCaretNoRedraw();
				
	}

	protected class LayoutInfo : ICloneable
	{
		public Item[] Items;

		// The position and info for each draw item/ character
		public class Item
		{
			public enum CharType {OutOfBoundsChar, VisibleChar, CR, LF};
               
			public CharType type;
			public Rectangle bounds;
			public bool selected;
		}


		public object Clone()
		{
			LayoutInfo layout = new LayoutInfo();
			layout.Items = (Item[])Items.Clone();
			return layout;
		}

	}

	// Uses the current selection positions to set what is selected in drawLayout
	// Sets the update to redraw only the characters that have changed
	internal override void SelectInternal(int start, int length)
	{
		if (start == selectionStartActual && length == selectionLengthActual)
		{
			return;
		}
		if (!IsHandleCreated)
		{
			return;
		}
		Region newRegion = new Region(RectangleF.Empty);
		selectionStartActual = start;
		selectionLengthActual = length;
		for (int i = 0; i < Text.Length; i++) 
		{
			bool selected = (i>=GetSelectionStart() && i < GetSelectionStart() + GetSelectionLength());
			layout.Items[i].selected = selected;
			Rectangle b = layout.Items[i].bounds;
			b = new Rectangle(b.Left, b.Top, b.Width, b.Height + 1);
			if (selected)
			{
				newRegion.Union(b);
			}
		}
		// Find the region we need to redraw by Xoring with old
		if (selectedRegion != null)
		{
			Region redrawRegion = newRegion.Clone();
			redrawRegion.Xor(selectedRegion);
			redrawRegion.Translate(-XViewOffset, -YViewOffset);
			// Only allow updates in the TextDrawArea
			redrawRegion.Intersect(new Region(TextDrawArea));
			AddUpdate(redrawRegion);
		}
		else
		{
			AddUpdate(newRegion);
		}
		selectedRegion = newRegion;

	}

	protected override void SetTextInternal(string text)
	{

		SetTextActual(text);
		InvalidateDirty();
	}

	// Called to change the text. Sets the update to whats needed to but doesnt change the selection point or caret
	private void SetTextActual( string text)
	{
		switch( this.characterCasing ) 
		{
			case CharacterCasing.Upper :
				text = text.ToUpper();
				break;

			case CharacterCasing.Lower :
				text = text.ToLower();
				break;
		}

		if( !IsHandleCreated ) {
			// create handle here to be sure that LayoutInfo will be set correct.
			this.CreateHandle();
		}
		// Layout the new text. Compare with old layout, Creating a region for areas that must be updated.
		bool prevLayout = layout != null;
		LayoutInfo oldLayout = null;
		if (prevLayout)
		{
			oldLayout = (LayoutInfo)layout.Clone();
		}
		string oldText = Text;
		LayoutFromText(text);
		// We must not trigger the onTextChanged event yet else this controls text could be change in the event!
		(this as Control).text = text;
		SetScrollBarPositions();
		if (prevLayout)
		{
			try {
				Region update = new Region(RectangleF.Empty);
				int oldLen = oldText.Length;
				int newLen = text.Length;
				int len    = newLen;
						
				if (oldLen > len)
				{
					len = oldLen;
				}
				for (int i=0;i < len;i++)
				{
					if (i >= oldLen)
					{
						if( i < newLen ) update.Union( layout.Items[i].bounds);
					}
					else if (i >= newLen )
					{
						if( i < oldLen ) update.Union( oldLayout.Items[i].bounds);
					}
					else if ( (i < oldLen && i < newLen) && (Text[i] != oldText[i] || oldLayout.Items[i].bounds != layout.Items[i].bounds ) )
					{
						if( i < newLen ) {
							update.Union( layout.Items[i].bounds);
						}
						if( i < oldLen ) {
							update.Union( oldLayout.Items[i].bounds);
						}
					}
				}
				// Get the offset of the TextDrawArea
				update.Translate( - XViewOffset, - YViewOffset);
				AddUpdate(update);
			}
			catch {	// ignore exceptions here, because in some cases this could happen
			}
		}
	}

	// Get the length of the selection.
	internal override int GetSelectionLength()
	{
		if (selectionLengthActual < 0)
		{
			return -selectionLengthActual;
		}
		return selectionLengthActual;
	}

	// Get the start of the selection.
	// Our length could be negative
	internal override int GetSelectionStart()
	{
		if (selectionStartActual < selectionStartActual + selectionLengthActual)
		{
			return selectionStartActual;
		}
		return selectionStartActual + selectionLengthActual;
	}

	// Set the start of the selection and the caret position
	internal override void SetSelectionStart(int start)
	{
		SelectInternal(start, selectionLengthActual);
		CaretSetPosition(GetSelectionStart() + GetSelectionLength());
	}

	// Sets the end of the selection and the caret position
	internal override void SetSelectionLength(int length)
	{
		SelectInternal(selectionStartActual, length);
		CaretSetPosition(GetSelectionStart() + GetSelectionLength());
	}

	[TODO]
	internal override void OnToggleInsertMode()
	{
		// Fix: change caret appearance
	}
	
	// Caret navigation
	protected override void MoveCaret(CaretDirection dir, bool extend)
	{
		int startSel = GetSelectionStart();
		
		int newPos = startSel;
		if ((extend && selectionLengthActual>0) || (!extend && IsTowardsTextEnd(dir)))
		{
			newPos += GetSelectionLength();
		}
		
		switch (dir)
		{
		case CaretDirection.Left:
			newPos = ComputeCharLeftPos(newPos);
			break;
		case CaretDirection.Right:
			newPos = ComputeCharRightPos(newPos);
			break;
		case CaretDirection.WordLeft:
			newPos = ComputeWordLeftPos(newPos);
			break;
		case CaretDirection.WordRight:
			newPos = ComputeWordRightPos(newPos);
			break;
		case CaretDirection.LineStart:
			newPos = ComputeLineStartPos(newPos);
			break;
		case CaretDirection.LineEnd:
			newPos = ComputeLineEndPos(newPos);
			break;
		case CaretDirection.LineUp:
			if (caretBounds.Top >= caretBounds.Height )
			{
				newPos = ComputeLineOffset(newPos, -1);
			}
			break;
		case CaretDirection.LineDown:
			if (layout.Items.Length == 0)
			{
				return;
			}
			if (caretBounds.Top < layout.Items[layout.Items.Length - 1].bounds.Top)
			{
				newPos = ComputeLineOffset(newPos, 1);
			}
			break;
		case CaretDirection.PageUp:
			newPos = ComputeLineOffset(newPos, - (int) TextDrawArea.Height / Font.Height);
			break;
		case CaretDirection.PageDown:
			newPos = ComputeLineOffset(newPos, (int) TextDrawArea.Height / Font.Height);
			break;
		case CaretDirection.TextStart:
			newPos = 0;
			break;
		case CaretDirection.TextEnd:
			newPos = Text.Length;
			break;
		}
		
		if (extend)
		{
			UpdateSelectionInternal(newPos);
		}
		else
		{
			SelectInternal(newPos, 0);
			CaretSetPosition(newPos);
			ScrollToCaretNoRedraw();
			InvalidateDirty();
		}
	}
	
	protected override void DeleteTextOp(CaretDirection dir)
	{
		if (ReadOnly)
			return;

		switch (dir)
		{
			case CaretDirection.WordLeft:
			case CaretDirection.Left:
			{
				if (layout.Items.Length == 0)
					break;
				int widthText = layout.Items[layout.Items.Length - 1].bounds.Right;
				int bottomCaret = caretBounds.Bottom;

				if (GetSelectionLength()>0)
				{
					SetSelectionText("");
					SelectInternal(GetSelectionStart(), 0);
				}
				else
				{
					int startPos = GetSelectionStart();
					int newPos =
						dir==CaretDirection.Left
						?
						ComputeCharLeftPos(startPos)
						:
						ComputeWordLeftPos(startPos);
						
					if (newPos < startPos)
					{
						int nbCharsToDelete = startPos - newPos;
						SetTextActual(Text.Substring(0, GetSelectionStart() - nbCharsToDelete) + Text.Substring(GetSelectionStart()));
						SelectInternal(GetSelectionStart()-nbCharsToDelete, 0);
					}
				}
				OnTextChanged(EventArgs.Empty);

				// In the case of multiline we ensure that we recover any blank lines created by backspacing at the bottom (if the text is bigger than the view area).
				// In the case of non multiline, we recover any character space that is now there after deleting
				if (!Multiline && layout.Items.Length > 0)
				{
					if (textAlign == HorizontalAlignment.Center && XViewOffset > maxXY/2 - TextDrawArea.Width/2)
					{
						XViewOffset -= widthText - layout.Items[layout.Items.Length - 1].bounds.Right;
						int x = maxXY/2 - TextDrawArea.Width/2;
						if (x > xViewOffset)
						{
							XViewOffset = x;
						}
					}
					else if (textAlign == HorizontalAlignment.Left && XViewOffset > 0)
					{
						XViewOffset -= widthText - layout.Items[layout.Items.Length - 1].bounds.Right;
						if (xViewOffset < 0)
						{
							XViewOffset = 0;
						}
					}
				}
				else
				{
					//Move the caret first
					CaretSetEndSelection();
					if (bottomCaret != caretBounds.Bottom)
					{
						int yViewOffset = YViewOffset - bottomCaret + caretBounds.Bottom - 1;
						if (yViewOffset < 0)
							yViewOffset = 0;
						YViewOffset = yViewOffset;
					}
				}
				InvalidateDirty();
				break;
			}

			case CaretDirection.Right:
			case CaretDirection.WordRight:
			{
				if (GetSelectionLength()>0)
				{
					SetSelectionText("");
				}
				else
				{
					int startPos = GetSelectionStart();
					int newPos =
						dir==CaretDirection.Right
						?
						ComputeCharRightPos(startPos)
						:
						ComputeWordRightPos(startPos);
						
					if (newPos > startPos)
					{
						SetTextActual(Text.Substring(0, GetSelectionStart()) + Text.Substring(GetSelectionStart() + newPos - startPos));
					}
				}
				SelectInternal(GetSelectionStart(),0);
				InvalidateDirty();
				OnTextChanged(EventArgs.Empty);
				break;
			}
		}

		CaretSetEndSelection();
		ScrollToCaretNoRedraw();
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		chosenHeight = height;
		// If not Multiline then the control height is the font height
		if (!Multiline)
		{
			height = ClientToBounds(Size.Empty).Height + Font.Height + 1;
		}
		
		Rectangle oldRec = Bounds;
		
		if( oldRec.X == x && oldRec.Y == y && oldRec.Width == width && oldRec.Height == height ) { // no update needed if same bounds
			return;
		}
		
		base.SetBoundsCore (x, y, width, height, specified);
		if (!IsHandleCreated)
		{
			return;
		}
		
		// If the height or width changes then relayout the text
		if ((specified & BoundsSpecified.Height) != 0 | (specified & BoundsSpecified.Width) != 0)
		{
			layout = null;
			LayoutFromText(Text);
			SetScrollBarPositions();
			ResetView ();
			// Redraw
			InvalidateAll();
		}
	}

	// Paint the text using layout information
	private void DrawText(Graphics g, bool focused)
	{
		if (layout.Items.Length == 0)
		{
			return;
		}
		Font font = Font;
		int lineStart = 0;
		Brush prevFore = null;
		int x = 0;
		int y = int.MinValue;
		int i;
		using (SolidBrush foreBrush = new SolidBrush(ForeColor), selectedForeBrush = new SolidBrush(SystemColors.HighlightText))
		{
			int cleanI = 0;
			for (i=0; i < text.Length;i++) 
			{
				LayoutInfo.Item item = layout.Items[i];
				Rectangle bounds = item.bounds;
				bounds.Offset(-xViewOffset, -yViewOffset);
				if (item.type == LayoutInfo.Item.CharType.VisibleChar)
				{
					Brush fore;
					if (item.selected && focused)
					{
						fore = selectedForeBrush;
					}
					else
					{
						fore = foreBrush;
					}

					if (y >= Height)
					{
						return;
					}

					// Setup the first values.
					if (y == int.MinValue)
					{
						x = bounds.X - graphicsOffset;
						y = bounds.Y;
						prevFore = fore;
					}

					// Has the position of the line or the color of the text changed?
					bool drawLine = false;
					if (prevFore != fore)
					{
						drawLine = true;
						cleanI = i;
					}
					else if (bounds.Y != y)
					{
						drawLine = true;
					}

					if (drawLine)
					{
						// Don't bother if its off the screen.
						if (bounds.Bottom > 0)
						{
							String lineText;
							if (passwordChar != 0)
							{
								lineText = new String(passwordChar, cleanI - lineStart + 1);
							}
							else
							{
								lineText = text.Substring(lineStart, cleanI - lineStart + 1);
							}
							
							if (Enabled)
							{
								g.DrawString(lineText, font, prevFore, new Point (x, y));
							}
							else
							{
								if(BackColor != Color.Gray)
									foreBrush.Color = Color.Gray;
								g.DrawString(lineText, Font, foreBrush, new Point (x, y));
							}
						}
						lineStart = i;
						x = bounds.X - graphicsOffset;
						y = bounds.Y;
						prevFore = fore;
					}
					cleanI = i;

				}
			}
			// Do the very last line.
			if (i > lineStart)
			{
				String lineText;
				if (passwordChar != 0)
				{
					lineText = new String(passwordChar, cleanI - lineStart + 1);
				}
				else
				{
					lineText = text.Substring(lineStart, cleanI - lineStart + 1);
				}
						
				if (Enabled)
				{
					g.DrawString(lineText, Font, prevFore, new Point (x, y));
				}
				else
				{
					if(BackColor != Color.Gray)
						foreBrush.Color = Color.Gray;
					g.DrawString(lineText, Font, foreBrush, new Point (x, y));
				}
			}
		}
	}

	private int previousClosest = -1;

	// Handle all mouse processing
	private void ProcessMouse(MouseEventArgs e)
	{
		if (Enabled) 
		{
			if (mouseDown)
			{
				if (e.Button == MouseButtons.Left)
				{		
					Point pt = new Point(e.X,e.Y);
					pt.Offset(XViewOffset, YViewOffset);
					
					int closest = CaretGetPosition(pt);
					if (e.Clicks == 2)
					{
						SelectWord(closest);
					}
					else if (closest >= 0 && previousClosest != closest)
					{
						UpdateSelectionInternal(closest);
					}
					previousClosest = closest;
				}
				else 
				{
					mouseDown = false;
					Capture = false;
				}
					
			}
			else
			{
				if (e.Button == MouseButtons.Left) 
				{
					// We are clicking to move the caret
					Point pt = new Point(e.X,e.Y);
					previousClosest = -1;
					pt.Offset(XViewOffset, YViewOffset);
					int closest = CaretGetPosition(pt);
					if (closest >= 0)
					{
						if (e.Clicks == 2)
						{
							// Select the entire word
							int startPos = closest;
							for (; startPos > 0; --startPos)
							{
								if (!IsWordChar(Text[startPos-1]))
								{
									break;
								}
							}

							int endPos = closest;
							for (; endPos < Text.Length; ++endPos)
							{
								if (!IsWordChar(Text[endPos]))
								{
									break;
								}
							}
							
							SelectInternal(startPos, endPos-startPos);
						}
						else
						{
							SelectInternal(closest, 0);
							mouseDown = true;
							Capture = true;
						}
						CaretSetEndSelection();
						// If you click right at the end/beginning, make sure the caret is in view
						ScrollToCaretNoRedraw();
						CaretShow();
						InvalidateDirty();
					}
				}
			}
		}
		else
		{
			mouseDown = false;
		}
	}

	// Takes into account client offset and view offsets
	private void CaretDraw(Graphics g) 
	{
		if (caretHiding)
		{
			return;
		}
		Point p1 = CaretActualBounds.Location;
		Point p2 = p1 + new Size(0, caretBounds.Height - 1);
		g.DrawLine(caretPen, p1, p2);
	}

	// Get the default X Position of Caret based on alignment
	private int CaretXFromAlign
	{
		get
		{
			switch (TextAlign)
			{
				case(HorizontalAlignment.Left):
					return 1;
				case(HorizontalAlignment.Center):
					if (Multiline)
					{
						return TextDrawArea.Width/2;
					}
					else
					{
						return maxXY/2;
					}
				default: /*Right*/
					if (Multiline)
					{
						return TextDrawArea.Width;
					}
					else
					{
						return maxXY;
					}
			}
		}
	}

	// Set the caret bounds from a character position
	// Set update region
	internal override void CaretSetPosition( int position)
	{
		// remember Caret position
		caretPosition = position;

		if (!IsHandleCreated)
		{
			return;
		}
		Rectangle newBounds = Rectangle.Empty;
		int height = Font.Height;
		if (Text.Length == 0)
		{
			newBounds = new Rectangle(CaretXFromAlign, 1, 1, height);
		}
		else
		{
			if (position == Text.Length)
			{
				
				// If the last character is a linefeed, position ourselves at the
				// beginning of the following line. Otherwise, position ourselves
				// immediately to the right of the last character.
				LayoutInfo.Item item = layout.Items[position -1];
				newBounds = item.bounds;
				if (item.type == LayoutInfo.Item.CharType.LF)
				{
					newBounds = new Rectangle(CaretXFromAlign, newBounds.Top + height, 1, height);
				}
				else
				{
					newBounds = new Rectangle(newBounds.Right, newBounds.Top, 1, newBounds.Height + 1);
				}
			}
			else
			{
				newBounds = layout.Items[position].bounds;
				newBounds = new Rectangle(newBounds.Left, newBounds.Top, 1, newBounds.Height + 1);
			}
		}

		// This looks better.
		if (newBounds.X == 0)
			newBounds.X = 1;

		// When we change the caret position, find the region to update
		Region region = new Region(newBounds);
		if (!caretHiding)
		{
			region.Xor(caretBounds);
		}
		region.Translate(- XViewOffset, - YViewOffset);
		AddUpdate(region);
		caretBounds = newBounds;
		if (Focused)
		{
			CaretShow();
		}
		
	}

	// Get the caret position of the nearest point relative to pt in layout coordinates
	private int CaretGetPosition(Point pt)
	{
		int prevY = int.MinValue;
		int caretPosition = 0;
		Rectangle bounds = new Rectangle(0, 0, 0, Font.Height);
		Rectangle prevBounds = bounds;
		int i=0;
		for (; i<Text.Length;i++) 
		{
			if (layout.Items[i].type != LayoutInfo.Item.CharType.OutOfBoundsChar)
			{
				bounds = layout.Items[i].bounds;
				caretPosition = i;
				if (pt.Y < bounds.Bottom && pt.X < bounds.Left + bounds.Width/2 )
				{
					break;
				}
				// New Line
				if (bounds.Bottom > prevY)
				{
					if (pt.Y < prevY)
					{
						// Move position back to end of previous line
						caretPosition -= 2; // (CR, LF)
						break;
					}
					prevY = bounds.Bottom;
				}
				prevBounds = bounds;
			}
		}

		// CR's only get selected, so if this position is the beginning of a selection then select the LF as well
		if (caretPosition < (GetSelectionStart()+ GetSelectionLength()) && Text.Length > 1 && layout.Items[caretPosition].type == LayoutInfo.Item.CharType.LF && layout.Items[caretPosition - 1].type == LayoutInfo.Item.CharType.CR)
		{
			caretPosition--;
		}
	
		if (i == Text.Length)
		{
			// If the last character is a linefeed, set the caret to Text.Length only
			// if they clicked underneath the linefeed.
			caretPosition = i;
			if (Text.Length > 0 &&
				layout.Items[i-1].type == LayoutInfo.Item.CharType.LF &&
				pt.Y <= prevBounds.Bottom)
			{
				caretPosition -= 2; // (CR, LF)
			}
		}
		
		return caretPosition;
	}

	// Make sure the caret is not hiding, set update region
	private void CaretShow()
	{
		if (caretHiding)
		{
			AddUpdate(CaretActualBounds);
		}
		caretHiding = false;
	}

	// Reset the caret position to the end of the first visible line
	private void CaretReset()
	{
		int end = 0;
		if (Multiline)
		{
			end = Text.Length;
		}
		else
			// Position before the first linefeed
			for (; end < Text.Length; end++)
			{
				if (Text[end] == '\n')
				{
					break;
				}
			}

		SelectInternal(0, end);
		CaretSetPosition(end);
	}

	private Rectangle CaretActualBounds
	{
		get
		{
			return new Rectangle(caretBounds.Left - XViewOffset, caretBounds.Top - YViewOffset, caretBounds.Width, caretBounds.Height);
		}
	}

	// Change the selection and move the caret. Also make sure its visible
	private void UpdateSelectionInternal(int newPos)
	{
		// We need to select some text
		SelectInternal(selectionStartActual, newPos - selectionStartActual);
		// Caret is always at the end
		CaretSetPosition(newPos);
		ScrollToCaretNoRedraw();
		InvalidateDirty();
	}

	private void SelectWord(int pos)
	{
		if (text == null || pos >= text.Length)
		{
			return;
		}
		if (passwordChar != 0)
		{
			SelectInternal(0, text.Length);
		}
		else
		{
			char c = text[pos];
			bool isSpace = char.IsWhiteSpace(c);
			int startPos = pos;
			int endPos = pos;
			// Find the start of the section we want to select.
			for (startPos = pos; startPos > 0 && isSpace == char.IsWhiteSpace(text[startPos - 1]); startPos--)
			{
			}
			// Find the end of the section we want to select.
			for (endPos = pos; endPos < text.Length && isSpace == char.IsWhiteSpace(text[endPos]); endPos++)
			{
			}
			SelectInternal(startPos, endPos - startPos);
		}
		CaretSetPosition(pos);
		ScrollToCaretNoRedraw();
		InvalidateDirty();
	}

	private static bool IsWordChar(char ch)
	{
		return Char.IsLetterOrDigit(ch);
	}

	private static bool IsTowardsTextEnd(CaretDirection dir)
	{
		return
			dir == CaretDirection.Right
			||
			dir == CaretDirection.WordRight
			||
			dir == CaretDirection.LineEnd
			||
			dir == CaretDirection.LineDown
			||
			dir == CaretDirection.PageDown
			||
			dir == CaretDirection.TextEnd;
	}

	private int ComputeLineStartPos(int fromPos)
	{
		for (; fromPos > 0; --fromPos)
		{
			if (Text[fromPos-1] == '\n')
			{
				break;
			}
		}
		return fromPos;
	}

	private int ComputeLineEndPos(int fromPos)
	{
		for (; fromPos < Text.Length; ++fromPos)
		{
			if (Text[fromPos] == '\r')
			{
				break;
			}
		}
		return fromPos;
	}
	
	private int ComputeCharLeftPos(int fromPos)
	{
		int newPos = fromPos;
		if (newPos > 0) --newPos;
		if (newPos>0 && Text[newPos]=='\n')
		{
			--newPos;
		}
		return newPos;
	}
	
	private int ComputeCharRightPos(int fromPos)
	{
		int newPos = fromPos;
		if (newPos < Text.Length) ++newPos;
		if (newPos<Text.Length && Text[newPos]=='\n')
		{
			++newPos;
		}
		return newPos;
	}
	
	private int ComputeWordLeftPos(int fromPos)
	{
		if (fromPos == 0)
		{
			return 0;
		}
			
		fromPos = ComputeCharLeftPos(fromPos);

		for (; fromPos > 0; --fromPos)
		{
			if (!IsWordChar(Text[fromPos-1]))
			{
				break;
			}
		}

		return fromPos;
	}

	private int ComputeWordRightPos(int fromPos)
	{
		// Move past at least one non-word element
		// to right before the next word element.
		// If the character immediately to our
		// left is a word element, we move one
		// after the first non-word element to
		// our left. Otherwise, we move to the
		// second non-word element to our left.
		for (; fromPos < Text.Length; ++fromPos)
		{
			if (!IsWordChar(Text[fromPos]))
			{
				break;
			}
		}

		for (; fromPos < Text.Length; ++fromPos)
		{
			if (IsWordChar(Text[fromPos]))
			{
				break;
			}
		}

		return fromPos;
	}

	private int ComputeLineOffset(int fromPos, int nbLines)
	{
		int y = (caretBounds.Y + caretBounds.Bottom) / 2 + nbLines * caretBounds.Height;
		if (y > 0)
		{
			return CaretGetPosition(new Point( caretBounds.X, y));
		}
		else
		{
			return 0;
		}
	}

	// Add a region to include in the draw update
	private void AddUpdate(Region region)
	{
		if (invalidateRegion == null)
		{
			invalidateRegion = new Region();
			invalidateRegion.MakeEmpty();
		}
		invalidateRegion.Union(region);
	}

	// Add a rectangle to include in the draw update
	private void AddUpdate(Rectangle rectangle)
	{
		if (invalidateRegion == null)
		{
			invalidateRegion = new Region();
		}
		invalidateRegion.Union(rectangle);
	}

	protected override void OnTextChanged(EventArgs e)
	{
		inTextChangedEvent = true;
		base.OnTextChanged (e);
		/* Set Current Text to Binded DataSource */
		if( DataBindings["Text"] != null )
		{
			PropertyInfo setInfo;
			Binding b = DataBindings["Text"];
			b.UpdateSource(Text);
		}
		inTextChangedEvent = false;
	}

	internal override protected void OnWordWrapChanged( EventArgs e ) {
		layout = null;
	}
	

	private Rectangle TextDrawArea
	{
		get
		{
			Rectangle rect = ClientRectangle;
			if (vScrollBar != null && showVScrollBar)
			{
				rect.Width -= vScrollBar.Width;
			}
			if (hScrollBar != null && showHScrollBar)
			{
				rect.Height -= hScrollBar.Height;
			}
			return rect;
		}
	}

	private int XViewOffset
	{
		get
		{
			return xViewOffset;
		}
		set
		{
			if (value == xViewOffset)
			{
				return;
			}
			// Make sure the entire textbox is redrawn
			InvalidateAll();
			xViewOffset = value;
			if (hScrollBar != null)
			{
				hScrollBar.Value = xViewOffset;
			}
		}
	}

	private int YViewOffset
	{
		get
		{
			return yViewOffset;
		}
		set
		{
			if (value == yViewOffset)
			{
				return;
			}
			// Make sure the entire textbox is redrawn
			InvalidateAll();
			yViewOffset = value;
			if (vScrollBar != null && showVScrollBar)
			{
				vScrollBar.Value = yViewOffset;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (invalidateRegion != null)
		{
			invalidateRegion.Dispose();
			invalidateRegion = null;
		}
		if( selectedRegion != null ) {
			selectedRegion.Dispose();
			selectedRegion = null;
		}
		if (vScrollBar != null)
		{
			vScrollBar.ValueChanged-=new EventHandler(vScrollBar_ValueChanged);
			vScrollBar.Dispose();
			vScrollBar = null;
		}
		if (hScrollBar != null)
		{
			hScrollBar.ValueChanged-=new EventHandler(hScrollBar_ValueChanged);
			hScrollBar.Dispose();
			hScrollBar = null;
		}
		
		// remove event handler
		
#if CONFIG_COMPONENT_MODEL
		this.DataBindings.CollectionChanged -=new CollectionChangeEventHandler(HandleDataBindingCollectionChanged);
#endif
		
		base.Dispose (disposing);
	}

	private void InvalidateDirty()
	{
		if (invalidateRegion != null)
		{
			Invalidate(invalidateRegion);
			invalidateRegion.MakeEmpty();
		}
	}

	private void InvalidateAll()
	{
		Invalidate();
		if (invalidateRegion != null)
		{
			invalidateRegion.MakeEmpty();
		}
	}
	
	protected override void SetBorderStyle(BorderStyle borderStyle)
	{
		BorderStyleInternal = borderStyle;
	}
	
#if !CONFIG_COMPACT_FORMS

#endif // !CONFIG_COMPACT_FORMS

	
}; // class TextBox

}; // namespace System.Windows.Forms
