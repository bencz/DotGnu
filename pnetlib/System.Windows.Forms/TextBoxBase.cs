/*
 * TextBoxBase.cs - Implementation of the
 *			"System.Windows.Forms.TextBoxBase" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Free Software Foundation, Inc.
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
using System.Text;
using System.Drawing;

public abstract class TextBoxBase : Control
{
	internal enum InsertMode {Insert, Overwrite};
	
	// Internal state.
	private bool acceptsTab;
	private bool autoSize;
	private bool hideSelection;
	private bool modified;
	private bool multiline;
	private bool readOnly;
	private InsertMode insertMode;
	private bool wordWrap;
	private int maxLength;

	// Constructor.
	internal TextBoxBase()
			{
				acceptsTab = false;
				autoSize = true;
				hideSelection = true;
				modified = false;
				multiline = false;
				readOnly = false;
				insertMode = InsertMode.Insert;
				wordWrap = true;
				BorderStyleInternal = BorderStyle.Fixed3D;
				maxLength = 32767;
			}

	protected override void Dispose(bool disposing)
	{
		// remove event handler
		base.Dispose(disposing);
	}
			
	// Get or set this object's properties.
	public bool AcceptsTab
			{
				get
				{
					return acceptsTab;
				}
				set
				{
					if(acceptsTab != value)
					{
						acceptsTab = value;
						OnAcceptsTabChanged(EventArgs.Empty);
					}
				}
			}

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
	
	public BorderStyle BorderStyle
			{
				get
				{
					return BorderStyleInternal;
				}
				set
				{
					if(BorderStyleInternal != value)
					{
						SetBorderStyle(value);
						OnBorderStyleChanged(EventArgs.Empty);
					}
				}
			}

	protected abstract void SetBorderStyle(BorderStyle borderStyle);

	[TODO]
	public bool CanUndo
			{ 
				get
				{
					// Fix: Check whether
					// Anything exists in the stack
					return false;
				}
			}

	protected override Size DefaultSize
			{
				get
				{
					return new Size(100, PreferredHeight);
				}
			}
	
	public bool HideSelection
			{
				get
				{
					return hideSelection;
				}
				set
				{
					if(hideSelection != value)
					{
						hideSelection = value;
						OnHideSelectionChanged(EventArgs.Empty);
					}
				}
			}

	public abstract String[] Lines
			{ get; set; }

	public virtual int MaxLength
			{
				get
				{
					return maxLength;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("SWF_NonNegative"));
					}
					maxLength = value;
				}
			}

	public bool Modified
			{
				get
				{
					return modified;
				}
				set
				{
					if(modified != value)
					{
						modified = value;
						OnModifiedChanged(EventArgs.Empty);
					}
				}
			}

	public virtual bool Multiline
			{
				get
				{
					return multiline;
				}
				set
				{
					if(multiline != value)
					{
						multiline = value;
						OnMultilineChanged(EventArgs.Empty);
					}
				}
			}

	public int PreferredHeight
			{
				get
				{
					if(BorderStyleInternal == BorderStyle.None)
					{
						return FontHeight;
					}
					else
					{
						return FontHeight + 3 +
							   SystemInformation.BorderSize.Height * 4;
					}
				}
			}

	public bool ReadOnly
			{
				get
				{
					return readOnly;
				}
				set
				{
					if(readOnly != value)
					{
						readOnly = value;
						OnReadOnlyChanged(EventArgs.Empty);
					}
				}
			}

	public virtual String SelectedText
			{
				get
				{
					int start = SelectionStart;
					int length = SelectionLength;
					if(start >= 0)
					{
						return Text.Substring(start, length);
					}
					else
					{
						return null;
					}
				}
				set
				{
					SetSelectionText(value);
					ClearUndo();
					SelectInternal(SelectionStart + value.Length, 0);
					CaretSetPosition(SelectionStart);
					ScrollToCaretInternal();
					OnTextChanged(EventArgs.Empty);
				}
			}

	public virtual int SelectionLength
			{
				get
				{
					return GetSelectionLength();
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("SWF_NonNegative"));
					}
					SetSelectionLength(value);
				}
			}

	public virtual int SelectionStart
			{
				get
				{
					return GetSelectionStart();
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("SWF_NonNegative"));
					}
					SetSelectionStart(value);
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
	public virtual int TextLength
			{
				get
				{
					String text = Text;
					if(text != null)
					{
						return text.Length;
					}
					else
					{
						return 0;
					}
				}
			}
	public bool WordWrap
			{
				get
				{
					return wordWrap;
				}
				set
				{
					if (wordWrap == value)
						return;
					wordWrap = value;
					OnWordWrapChanged(EventArgs.Empty);
				}
			}

	// Append text to this control.
	public void AppendText(String text)
			{
				Text += text;
			}

	// Clear all text from this control.
	public void Clear()
			{
				Text = null;
			}

	// Clear undo information for this control.
	[TODO]
	public void ClearUndo()
			{
				return;
			}

	// Copy the current selection to the clipboard.
	[TODO]
	public void Copy()
			{
				return;
			}

	// Create the handle for this control.
	protected override void CreateHandle()
			{
				// Let the base class do the work.
				base.CreateHandle();
			}

	// Cut the current selection to the clipboard.
	[TODO]
	public void Cut()
			{
				return;
			}

	// Determine if a key is recognized by a control as an input key.
	protected override bool IsInputKey(Keys keyData)
			{
				if((keyData & Keys.Alt) == 0)
				{
					Keys code = (keyData & Keys.KeyCode);
					if(code == Keys.Tab)
					{
						if(!Multiline || !acceptsTab)
						{
							return false;
						}
						if((keyData & Keys.Control) != 0)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					else if(code == Keys.Escape)
					{
						if(Multiline)
						{
							return false;
						}
					}
					else if(code >= Keys.Prior && code <= Keys.Home)
					{
						return true;
					}
				}
				return base.IsInputKey(keyData);
			}

	// Paste the clipboard and replace the current selection.
	[TODO]
	public void Paste()
			{
				return;
			}

	protected override bool ProcessDialogKey(Keys keyData)
			{
				if ((keyData & Keys.Alt) == 0)
				{
					Keys key = keyData & Keys.KeyCode;
					bool shiftKey = (keyData & Keys.Shift) != 0;
					bool controlKey = (keyData & Keys.Control) != 0;

					switch (key)
					{
						case Keys.Left:
							MoveCaret(controlKey ? CaretDirection.WordLeft : CaretDirection.Left, shiftKey);
							return true;
						case Keys.Right:
							MoveCaret(controlKey ? CaretDirection.WordRight : CaretDirection.Right, shiftKey);
							return true;
						case Keys.Up:
							MoveCaret(CaretDirection.LineUp, shiftKey);
							return true;
						case Keys.Down:
							MoveCaret(CaretDirection.LineDown, shiftKey);
							return true;
					}
	
				}
				return base.ProcessDialogKey(keyData);
			}

	// Scroll the text box to make the caret visible.
	public void ScrollToCaret()
			{
				ScrollToCaretInternal();
			}

	// Move the selection.
	public void Select(int start, int length)
			{
				if(start < 0)
				{
					throw new ArgumentException
						(S._("SWF_NonNegative"), "start");
				}
				if(length < 0)
				{
					throw new ArgumentException
						(S._("SWF_NonNegative"), "length");
				}
				SelectInternal( start, length);
				CaretSetEndSelection();
				ScrollToCaret();
			}

	// Sets the caret position to the end of the selection
	internal void CaretSetEndSelection()
	{
		// Even if we select backwards, the Caret will be at the end
		CaretSetPosition(GetSelectionStart()+ GetSelectionLength());
	}

	// Set the caret bounds from a character position
	// Set update region
	internal abstract void CaretSetPosition( int position);

	// Select all text in the control.
	public void SelectAll()
			{
				Select(0, TextLength);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				String text = Text;
				if(text != null && text.Length > 40)
				{
					text = text.Substring(0, 40) + " ...";
				}
				return base.ToString() + ", Text: " + text;
			}

	// Undo the last editing operation.
	[TODO]
	public void Undo()
			{
				return;
			}

	// Events that may be emitted by this control.
	public event EventHandler AcceptsTabChanged
			{
				add
				{
					AddHandler(EventId.AcceptsTabChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.AcceptsTabChanged, value);
				}
			}

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

	public new event EventHandler Click
			{
				add
				{
					AddHandler(EventId.TextBoxClick, value);
				}
				remove
				{
					RemoveHandler(EventId.TextBoxClick, value);
				}
			}

	public event EventHandler HideSelectionChanged
			{
				add
				{
					AddHandler(EventId.HideSelectionChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.HideSelectionChanged, value);
				}
			}

	public event EventHandler ModifiedChanged
			{
				add
				{
					AddHandler(EventId.ModifiedChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ModifiedChanged, value);
				}
			}

	public event EventHandler MultilineChanged
			{
				add
				{
					AddHandler(EventId.MultilineChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.MultilineChanged, value);
				}
			}

	public event EventHandler ReadOnlyChanged
			{
				add
				{
					AddHandler(EventId.ReadOnlyChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ReadOnlyChanged, value);
				}
			}

			
	internal virtual protected void OnWordWrapChanged( EventArgs e ) {
	}

	// Caret Navigation
	protected enum CaretDirection
	{
		Left, Right, WordLeft, WordRight, LineStart, LineEnd,
		LineUp, LineDown, PageUp, PageDown, TextStart, TextEnd
	};
	
	protected abstract void MoveCaret(CaretDirection dir, bool extend);

	// Deletes text (i.e. backspace, delete keys)
	protected abstract void DeleteTextOp(CaretDirection dir);

	// Dispatch events from this control.
	protected virtual void OnAcceptsTabChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.AcceptsTabChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected virtual void OnAutoSizeChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.AutoSizeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected virtual void OnBorderStyleChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.BorderStyleChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected override void OnFontChanged(EventArgs e)
			{
				base.OnFontChanged(e);
			}

	protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);
			}

	protected override void OnHandleDestroyed(EventArgs e)
			{
				base.OnHandleDestroyed(e);
			}

	protected virtual void OnHideSelectionChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.HideSelectionChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected virtual void OnModifiedChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.ModifiedChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected virtual void OnMultilineChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.MultilineChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	protected virtual void OnReadOnlyChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.ReadOnlyChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Set the selection text to a new value.
	internal virtual void SetSelectionText(String value)
			{
				int start = SelectionStart;
				int length = SelectionLength;
				String text = Text;
				int vlength;
				String newText;
				if(start >= 0)
				{
					if(value == null)
					{
						value = String.Empty;
					}
					vlength = value.Length;
					newText = text.Substring(0, start) + value +
							  text.Substring(start + length);
					SetTextInternal(newText);
				}
			}

	// Get the length of the selection.
	abstract internal int GetSelectionLength();

	// Set the length of the selection.
	abstract internal void SetSelectionLength(int value);

	// Get the start of the selection.
	abstract internal int GetSelectionStart();

	// Set the start of the selection.
	abstract internal void SetSelectionStart(int value);

	// Change the text selection
	abstract internal void SelectInternal( int start, int length);

	abstract protected void SetTextInternal( string text);

	// Make sure the caret is visible
	abstract protected void ScrollToCaretInternal();

	// Toggle insert/overwrite mode
	internal InsertMode GetInsertMode()
	{
		return insertMode;
	}
	
	abstract internal void OnToggleInsertMode();

#if !CONFIG_COMPACT_FORMS

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if( e.Handled ) return; 
		
		bool extendSel = (ModifierKeys & Keys.Shift) != 0;
		bool controlKey = (ModifierKeys & Keys.Control) != 0;
		
		switch (e.KeyCode)
		{
			case Keys.PageUp:
				MoveCaret(CaretDirection.PageUp, extendSel);
				e.Handled = true;
				break;
			case Keys.PageDown:
				MoveCaret(CaretDirection.PageDown, extendSel);
				e.Handled = true;
				break;
			case Keys.Home:
				MoveCaret(controlKey ? CaretDirection.TextStart : CaretDirection.LineStart, extendSel);
				e.Handled = true;
				break;
			case Keys.End:
				MoveCaret(controlKey ? CaretDirection.TextEnd : CaretDirection.LineEnd, extendSel);
				e.Handled = true;
				break;
			case Keys.C:
				if (controlKey)
				{
					Copy();
					e.Handled = true;
				}
				break;
			case Keys.X:
				if (controlKey)
				{
					Cut();
					e.Handled = true;
				}
				break;
			case Keys.V:
				if (controlKey)
				{
					Paste();
					e.Handled = true;
				}
				break;
			case Keys.A:
				if (controlKey)
				{
					SelectAll();
					e.Handled = true;
				}
				break;
			case Keys.Z:
				if (controlKey)
				{
					Undo();
					e.Handled = true;
				}
				break;
			case Keys.Back:
				DeleteTextOp(controlKey ? CaretDirection.WordLeft : CaretDirection.Left);
				e.Handled = true;
				break;
			case Keys.Delete:
				DeleteTextOp(controlKey ? CaretDirection.WordRight : CaretDirection.Right);
				e.Handled = true;
				break;
			case Keys.Insert:
				insertMode = insertMode==InsertMode.Insert ? InsertMode.Overwrite : InsertMode.Insert;
				OnToggleInsertMode();
				e.Handled = true;
				break;
		}
	}














// NOTE: the following classes constitute the model/view backend for the text
//       box... the text tree is the view and the text buffer is the model...
//       the text layout class provides a way to plug in different layout
//       behavior into the view (e.g. wrapping or non-wrapping)... further
//       model/view functionality (as will be needed for the rich text box)
//       should be built on top of these as needed... the text tree is
//       designed to be the main entry point for all modifications to the
//       actual character data, however this isn't set in stone... also note
//       that there is still plenty TODO in the text tree to provide the
//       needed interface for it to act as said entry point - Rich

#region    // TextTree
	internal class TextTree
	{
		// Internal state.
		private ITextMark  caret;
		private ITextMark  selection;
		private TextBuffer buffer;
		private TextLayout layout;
		private TextGroup  root;


		// Constructor.
		public TextTree(TextBuffer buffer, TextLayout layout)
				{
					// set the buffer for this text tree
					this.buffer = buffer;

					// set the layout for this text tree
					this.layout = layout;

					// set the root for this text tree
					root = new TextGroup();

					// insert a new line into the buffer
					buffer.Insert(0, '\n');

					// create the first line
					TextLine line = new TextLine
						(buffer.MarkPosition(0, true),
						 buffer.MarkPosition(1, true));

					// insert the first line into the tree
					root.InsertChild(null, line);

					// update the metrics information
					root.UpdateMetrics(layout, true);

					// create the caret position
					caret = buffer.MarkPosition(0, false);

					// create the selection position
					selection = buffer.MarkPosition(0, false);
				}


		// Get the character count for this tree.
		public int CharCount
				{
					get { return (root.CharCount - 1); }
				}

		// Get or set the text layout.
		public TextLayout Layout
				{
					get { return layout; }
					set
					{
						layout = value;
						root.UpdateMetrics(layout, true);
					}
				}

		// Get the line count for this tree.
		public int LineCount
				{
					get { return root.LineCount; }
				}

		// Get the selection length.
		public int SelectionLength
				{
					get
					{
						// get the caret offset
						int caretOffset = caret.Offset;

						// get the selection offset
						int selectionOffset = selection.Offset;

						// return the negative length on backwards selections
						if(caretOffset > selectionOffset)
						{
							return (caretOffset - selectionOffset);
						}

						// return the length
						return (selectionOffset - caretOffset);
					}
					set
					{
						// get the caret offset
						int caretOffset = caret.Offset;

						// get the selection offset
						int selectionOffset = selection.Offset;

						// set the value to the new end of the selection
						if(caretOffset > selectionOffset)
						{
							value += selectionOffset;
						}
						else
						{
							value += caretOffset;
						}

						// enforce upper and lower bounds limits on value
						if(value < 0)
						{
							value = 0;
						}
						else
						{
							// get the character count
							int charCount = CharCount;

							// set the value to the end position if past it
							if(value > charCount)
							{
								value = charCount;
							}
						}

						// move the end depending on direction of selection
						if(caret.Offset > selection.Offset)
						{
							// move the caret
							caret.Move(value);
						}
						else
						{
							// move the selection
							selection.Move(value);
						}
					}
				}

		// Get the selection start.
		public int SelectionStart
				{
					get
					{
						// get the caret offset
						int caretOffset = caret.Offset;

						// get the selection offset
						int selectionOffset = selection.Offset;

						// return the selection offset if it's lower
						if(caretOffset > selectionOffset)
						{
							return selectionOffset;
						}

						// return the caret offset
						return caretOffset;
					}
					set
					{
						// enforce upper and lower bounds limits on value
						if(value < 0)
						{
							value = 0;
						}
						else
						{
							// get the character count
							int charCount = CharCount;

							// set the value to the end position if past it
							if(value > charCount)
							{
								value = charCount;
							}
						}

						// move the start depending on direction of selection
						if(caret.Offset > selection.Offset)
						{
							// move the selection
							selection.Move(value);
						}
						else
						{
							// move the caret
							caret.Move(value);
						}
					}
				}


		// Clear the tree of all character data.
		public void Clear()
				{
					// get the character count
					int charCount = CharCount;

					// bail out now if there's nothing to remove
					if(charCount == 0) { return; }

					// remove the character data from the buffer
					buffer.Remove(0, charCount);

					// update the tree structure
					RemovalUpdate(0, charCount);
				}

		// Get the currently selected text.
		public String GetSelected()
				{
					// return the selection to caret if selection is lower
					if(caret.Offset > selection.Offset)
					{
						return buffer[selection.Offset, caret.Offset];
					}

					// return caret to selection
					return buffer[caret.Offset, selection.Offset];
				}

		// Find a line by a y position.
		public TextLine FindLineByY(int y, out int actualY)
				{
					// set the default actual y position
					actualY = 0;

					// get the maximum y position
					int maxY = root.Size.Height;

					// enforce the upper y limit
					if(y > maxY) { y = maxY; }

					// enforce the lower y limit
					if(y < 0) { y = 0; }

					// find the line
					TextLine line = root.FindLineByY(y, ref actualY);

					// zero the actual y position if there was no match
					if(line == null) { actualY = 0; }

					// return the matching line
					return line;
				}

		// Find a line by a line number.
		public TextLine FindLineByLineNumber(int num)
				{
					// bail out now if there's no such line
					if(num >= root.LineCount) { return null; }

					// find and return the line
					return root.FindLineByLineNumber(num);
				}

		// Find a line by a character offset.
		public TextLine FindLineByCharOffset(int offset, out int actualOffset)
				{
					// set the default actual offset position
					actualOffset = 0;

					// bail out now if there's no such line
					if(offset >= (root.CharCount - 1)) { return null; }

					// find the line
					TextLine line = root.FindLineByCharOffset
						(offset, ref actualOffset);

					// zero the actual offset if there was no match
					if(line == null) { actualOffset = 0; }

					// return the matching line
					return line;
				}

		// Find a character offset by an x,y position.
		public int FindCharOffsetByXY(int x, int y)
				{
					// get the maximum y position
					int maxY = root.Size.Height;

					// enforce the upper y limit
					if(y > maxY) { return CharCount; }

					// enforce the lower y limit
					if(y < 0) { return 0; }

					// set the default actual y position
					int actualY = 0;

					// find the line
					TextLine line = root.FindLineByY(y, ref actualY);

					// return the last position if no match was found
					if(line == null) { return CharCount; }

					// get the offset into the line of the x,y position
					int lineOffset = layout.FindOffsetByXY
						(line.StartOffset, line.EndOffset, x, (y - actualY));

					// return the absolute character offset of the x,y position
					return line.StartOffset + lineOffset;
				}

		// Update after an insertion.
		public void InsertionUpdate(int offset, int length)
				{
					// create new lines and modify existing ones, as needed
					TextLine.InsertionUpdate(this, buffer, offset, length);

					// update the metrics information
					root.UpdateMetrics(layout, false);
				}

		// Update after a removal.
		public void RemovalUpdate(int offset, int length)
				{
					// remove and modify lines, as needed
					TextLine.RemovalUpdate(this, buffer, offset, length);

					// update the metrics information
					root.UpdateMetrics(layout, false);
				}















	#region    // TextNode
		internal abstract class TextNode
		{
			// Internal state.
			internal bool     valid;
			internal int      level;
			internal int      lineCount;
			internal int      charCount;
			internal TextNode parent;
			internal TextNode prev;
			internal TextNode next;
			internal Size     size;


			// Constructor.
			protected TextNode()
					{
						valid = false;
						level = 0;
						lineCount = 0;
						charCount = 0;
						parent = null;
						prev = null;
						next = null;
						size = Size.Empty;
					}


			// Get the character count for this node.
			public int CharCount
					{
						get { return charCount; }
					}

			// Get the child count for this node.
			public virtual int ChildCount
					{
						get { return 0; }
					}

			// Get the first child of this node.
			public virtual TextNode FirstChild
					{
						get { return null; }
					}

			// Get the last child of this node.
			public virtual TextNode LastChild
					{
						get { return null; }
					}

			// Get the last line of this node.
			public virtual TextLine LastLine
					{
						get { return null; }
					}

			// Get the parent of this node.
			public TextNode Parent
					{
						get { return parent; }
					}

			// Get the previous sibling of this node.
			public TextNode PrevSibling
					{
						get { return prev; }
					}

			// Get the next sibling of this node.
			public TextNode NextSibling
					{
						get { return next; }
					}

			// Get the level of this node.
			public int Level
					{
						get { return level; }
					}

			// Get the line count for this node.
			public int LineCount
					{
						get { return lineCount; }
					}

			// Get the size of this node.
			public Size Size
					{
						get { return size; }
					}

			// Get the valid flag for the size of this node.
			public bool Valid
					{
						get { return valid; }
					}


			// Find the next node.
			public TextNode FindNext()
					{
						// return the next node if we have it
						if(next != null) { return next; }

						// get the current node
						TextNode node = parent;

						// find the next node with a next node
						while(node != null && node.next == null)
						{
							node = node.parent;
						}

						// return null if this is the last node on this level
						if(node == null) { return null; }

						// get the parent to search down from
						node = node.next;

						// move down the tree until this level is reached
						while(node.level > level) { node = node.FirstChild; }

						// return the next node at this level
						return node.FirstChild;
					}

			// Find a line by a character offset.
			public virtual TextLine FindLineByCharOffset
						(int offset, ref int actualOffset)
					{
						throw new InvalidOperationException();
					}

			// Find a line by a line number.
			public virtual TextLine FindLineByLineNumber(int num)
					{
						throw new InvalidOperationException();
					}

			// Find a line by a y position.
			public virtual TextLine FindLineByY(int y, ref int actualY)
					{
						throw new InvalidOperationException();
					}

			// Insert a child into the given position.
			public virtual void InsertChild(TextNode prev, TextNode child)
					{
						throw new InvalidOperationException();
					}

			// Insert children into the given position.
			public virtual void InsertChildren(TextNode prev, TextNode first)
					{
						throw new InvalidOperationException();
					}

			// Insert children into the given position.
			public virtual void InsertChildren
						(TextNode prev, TextNode first, int count)
					{
						throw new InvalidOperationException();
					}

			// Invalidate this node and all its ancestors.
			public void Invalidate()
					{
						if(valid)
						{
							// flag that the metrics information is invalid
							valid = false;

							// clear the metrics information
							size = Size.Empty;

							// invalidate up the tree
							if(parent != null) { parent.Invalidate(); }
						}
					}

			// Rebalance the tree at this node.
			public virtual void Rebalance(TextTree tree)
					{
						throw new InvalidOperationException();
					}

			// Remove the given child from this node.
			public virtual void RemoveChild(TextNode child)
					{
						throw new InvalidOperationException();
					}

			// Remove children from this node.
			public virtual void RemoveChildren(TextNode first)
					{
						throw new InvalidOperationException();
					}

			// Remove children from this node.
			public virtual void RemoveChildren(TextNode first, int count)
					{
						throw new InvalidOperationException();
					}

			// Update the character count of this node.
			protected void UpdateCharCount(int delta)
					{
						if(delta != 0)
						{
							// add the delta to the current count
							charCount += delta;

							// update up the tree
							if(parent != null)
							{
								parent.UpdateCharCount(delta);
							}
						}
					}

			// Update the line count of this node.
			protected void UpdateLineCount(int delta)
					{
						if(delta != 0)
						{
							// add the delta to the current count
							lineCount += delta;

							// update up the tree
							if(parent != null)
							{
								parent.UpdateLineCount(delta);
							}
						}
					}

			// Update the metrics of this node.
			public abstract void UpdateMetrics(TextLayout layout, bool force);

		}; // class TextNode
	#endregion // TextNode

	#region    // TextLine
		internal sealed class TextLine : TextNode
		{
			// Internal state.
			private ITextMark start;
			private ITextMark end;


			// Constructor.
			public TextLine(ITextMark start, ITextMark end)
					: base()
					{
						// set the start position for this line
						this.start = start;

						// set the end position for this line
						this.end = end;

						// set the character count
						charCount = (end.Offset - start.Offset);

						// set the line count for this line
						lineCount = 1;
					}


			// Get the character offset of this line.
			public int CharOffset
					{
						get
						{
							// initialize the offset
							int offset = 0;

							// set the current parent node
							TextNode parent = this.parent;

							// set the current node
							TextNode node = this;

							// count up the tree
							while(parent != null)
							{
								// set the current count node
								TextNode curr = node.prev;

								// count through the previous nodes
								while(curr != null)
								{
									// add the character count to the offset
									offset += curr.charCount;

									// set the current count node to the previous
									curr = curr.prev;
								}

								// set the current node to the current parent
								node = parent;

								// set the current parent to the parent's parent
								parent = parent.parent;
							}

							// return the offset
							return offset;
						}
					}

			// Get the end offset of this line.
			public int EndOffset
					{
						get { return end.Offset; }
					}

			// Get the line number of this line.
			public int LineNumber
					{
						get
						{
							// initialize the line number
							int num = 0;

							// set the current parent node
							TextNode parent = this.parent;

							// set the current node
							TextNode node = this;

							// count up the tree
							while(parent != null)
							{
								// set the current count node
								TextNode curr = node.prev;

								// count through the previous nodes
								while(curr != null)
								{
									// add the line count to the line number
									num += curr.lineCount;

									// set the current count node to the previous
									curr = curr.prev;
								}

								// set the current node to the current parent
								node = parent;

								// set the current parent to the parent's parent
								parent = parent.parent;
							}

							// return the line number
							return num;
						}
					}

			// Get the start offset of this line.
			public int StartOffset
					{
						get { return start.Offset; }
					}

			// Get the y position of this line.
			public int YPosition
					{
						get
						{
							// initialize the y position
							int y = 0;

							// set the current parent node
							TextNode parent = this.parent;

							// set the current node
							TextNode node = this;

							// count up the tree
							while(parent != null)
							{
								// set the current count node
								TextNode curr = node.prev;

								// count through the previous nodes
								while(curr != null)
								{
									// add the height to the y position
									y += curr.size.Height;

									// set the current count node to the previous
									curr = curr.prev;
								}

								// set the current node to the current parent
								node = parent;

								// set the current parent to the parent's parent
								parent = parent.parent;
							}

							// return the y position
							return y;
						}
					}


			// Update after an insertion.
			public static unsafe void InsertionUpdate
						(TextTree tree, TextBuffer buffer,
						 int offset, int length)
					{
						// NOTE: this is here to keep the method calls down to
						//       a minimum... technically, this belongs in the
						//       tree, but it's more efficient to do this here,
						//       where we can access the line/node fields
						//       directly

						// declare the start offset of the insertion line
						int lineStart;

						// find the insertion line
						TextLine line = tree.FindLineByCharOffset
							(offset, out lineStart);

						// get the parent of the insertion line
						TextNode parent = line.Parent;

						// get the end offset of the insertion line
						int lineEnd = line.end.Offset;

						// create a text slice
						TextSlice slice = new TextSlice();

						// get the inserted data into the slice
						buffer.GetSlice(offset, length, slice);

						// get the current line start position
						int currStart = lineStart;

						// get the current line end position
						int currEnd = (offset + 1);

						// set the default new line list
						TextLine first = null;

						// set the default previous new line
						TextLine prev = null;

						// find and create new lines, quickly
						fixed(char* start = &slice.chars[slice.start])
						{
							char* curr = start;
							// get the insertion end position
							char* end = (curr + slice.length);

							// find and create new lines
							while(curr != end)
							{
								if(*curr == '\n')
								{
									if(currStart == lineStart)
									{
										// shorten the insertion line
										line.end.Move(currEnd);
									}
									else
									{
										// create a new line
										TextLine newline = new TextLine
											(buffer.MarkPosition(currStart, true),
											 buffer.MarkPosition(currEnd, true));

										// update list links
										if(first == null)
										{
											// set the current line as the first
											first = newline;
										}
										else
										{
											// set the current line's prev
											newline.prev = prev;

											// set the previous line's next
											prev.next = newline;
										}

										// set the previous line to the current
										prev = newline;
									}

									// update the current line start position
									currStart = currEnd;
								}

								// increment the current input pointer
								++curr;

								// increment the current line end position
								++currEnd;
							}
						}

						// insert new lines and rebalance parent, if needed
						if(first != null)
						{
							// add any remaining characters to new line
							if(currEnd < lineEnd)
							{
								// create a new line
								TextLine newline = new TextLine
									(buffer.MarkPosition(currStart, true),
									 buffer.MarkPosition(currEnd, true));

								// set the current line's prev reference
								newline.prev = prev;

								// set the previous line's next reference
								prev.next = newline;
							}

							// insert the new lines
							parent.InsertChildren(line, first);

							// rebalance, starting at the new lines' parent
							parent.Rebalance(tree);
						}
					}

			// Layout this line.
			public void Layout(TextLayout layout)
					{
						layout.LayoutLine
							(start.Offset, end.Offset, ref size, ref valid);
					}

			// Update after a removal.
			public static bool RemovalUpdate
						(TextTree tree, TextBuffer buffer,
						 int offset, int length)
					{
						// NOTE: this is here to keep the method calls down to
						//       a minimum... technically, this belongs in the
						//       tree, but it's more efficient to do this here,
						//       where we can access the line/node fields
						//       directly

						// set the default return value
						bool retval = false;

						// declare the start offset of the found lines
						int garbage;

						// get the first line affected by the removal
						TextLine startLine = tree.FindLineByCharOffset
							(offset, out garbage);

						// get the last line affected by the removal
						TextLine endLine = tree.FindLineByCharOffset
							((offset + length + 1), out garbage);

						// handle end of buffer case
						if(endLine == null)
						{
							// get the character count of the buffer
							int bufCount = buffer.CharCount;

							// insert an extra line at the end of the buffer
							buffer.Insert(bufCount, '\n');

							// find the last line of the tree
							endLine = tree.root.LastLine;

							// create the new line node
							TextLine newLine = new TextLine
								(buffer.MarkPosition(bufCount, true),
								 buffer.MarkPosition((bufCount + 1), true));

							// add the new line to the last line's parent
							endLine.parent.InsertChild(endLine, newLine);

							// rebalance the parent
							endLine.parent.Rebalance(tree);

							// set the end line to the new line
							endLine = newLine;

							// flag that an insertion was performed
							retval = true;
						}

						// handle single line case
						if(startLine == endLine)
						{
							// force a character count update for the line
							startLine.UpdateCharCount();

							// invalidate the line
							startLine.Invalidate();

							// we're done
							return retval;
						}

						// merge the content of the two lines into the first
						startLine.end.Move(endLine.EndOffset);

						// get the start line's next sibling
						TextNode first = startLine.next;

						// set the current line to the start's next sibling
						TextLine currLine = (TextLine)first;

						// get the start line's parent
						TextNode startParent = startLine.parent;

						// get the end line's parent
						TextNode endParent = endLine.parent;

						// handle single parent case
						if(startParent == endParent)
						{
							// set the default removal count
							int rmCount = 1;

							// delete the end line's start position
							endLine.start.Delete();

							// delete the end line's end position
							endLine.end.Delete();

							// count and delete the lines to be removed
							while(currLine != endLine)
							{
								// delete the current line's start position
								currLine.start.Delete();

								// delete the current line's end position
								currLine.end.Delete();

								// move to the next line
								currLine = (TextLine)currLine.next;

								// increment the removal count
								++rmCount;
							}

							// remove the deleted lines from their parent
							startParent.RemoveChildren(first, rmCount);

							// rebalance the parent
							startParent.Rebalance(tree);

							// we're done
							return retval;
						}

						// delete the lines to be removed from the start parent
						while(currLine != null)
						{
							// delete the current line's start position
							currLine.start.Delete();

							// delete the current line's end position
							currLine.end.Delete();

							// move to the next line
							currLine = (TextLine)currLine.next;
						}

						// remove the deleted lines from their parent
						if(first != null) { startParent.RemoveChildren(first); }

						// get the current parent
						TextNode currParent = startParent.FindNext();

						// remove lines and groups as needed
						while(currParent != endParent)
						{
							// get the first child of the current parent
							first = currParent.FirstChild;

							// get the current line
							currLine = (TextLine)first;

							// set the default before next line
							TextLine nextLine = null;

							// delete the lines to be removed
							while(currLine != null)
							{
								// delete the current line's start position
								currLine.start.Delete();

								// delete the current line's end position
								currLine.end.Delete();

								// get the before next line
								nextLine = currLine;

								// set the current line to the next
								currLine = (TextLine)currLine.next;
							}

							// find the actual next line
							nextLine = (TextLine)nextLine.FindNext();

							// remove the deleted lines from their parent
							currParent.RemoveChildren(first);

							// remove empty groups up the tree
							while(currParent.ChildCount == 0)
							{
								// get the current parent's parent
								TextNode parent = currParent.parent;

								// remove the current parent
								parent.RemoveChild(currParent);

								// move up the tree
								currParent = parent;
							}

							// set the next parent to the next line's parent
							currParent = nextLine.parent;
						}

						// delete and remove lines from the end parent
						{
							// set the default removal count
							int rmCount = 1;

							// get the first child of the end line's parent
							first = endParent.FirstChild;

							// get the current line
							currLine = (TextLine)first;

							// delete the end line's start position
							endLine.start.Delete();

							// delete the end line's end position
							endLine.end.Delete();

							// count and delete the lines to be removed
							while(currLine != endLine)
							{
								// delete the current line's start position
								currLine.start.Delete();

								// delete the current line's end position
								currLine.end.Delete();

								// move to the next line
								currLine = (TextLine)currLine.next;

								// increment the removal count
								++rmCount;
							}

							// remove the deleted lines from their parent
							endParent.RemoveChildren(first, rmCount);
						}

						// rebalance the end line's parent
						endParent.Rebalance(tree);

						// rebalance the start line's parent
						startLine.parent.Rebalance(tree);

						// return the inserted flag
						return retval;
					}

			// Update the character count for this line.
			public void UpdateCharCount()
					{
						// get the new character count
						int newCharCount = (end.Offset - start.Offset);

						// update the character count if it has changed
						if(charCount != newCharCount)
						{
							UpdateCharCount(newCharCount - charCount);
						}
					}

			// Update the metrics of this line.
			public override void UpdateMetrics(TextLayout layout, bool force)
					{
						if(force || !valid)
						{
							// get the metrics information for this line
							size = layout.GetLineMetrics
								(start.Offset, end.Offset);

							// flag that the metrics information is now valid
							valid = true;
						}
					}

		}; // class TextLine
	#endregion // TextLine

	#region    // TextGroup
		internal sealed class TextGroup : TextNode
		{
			// Internal state.
			private int      childCount;
			private TextNode first;
			private TextNode last;

			// maximum number of children a group can have
			private const int MAX_CHILD_COUNT = 12;

			// minimum number of children a group can have (must be ceil(max/2))
			private const int MIN_CHILD_COUNT = 6;


			// Constructor.
			public TextGroup()
					: base()
					{
						childCount = 0;
						first = null;
						last = null;
						level = 1;
					}

			// Get the child count for this node.
			public override int ChildCount
					{
						get { return childCount; }
					}

			// Get the first child of this node.
			public override TextNode FirstChild
					{
						get { return first; }
					}

			// Get the last child of this node.
			public override TextNode LastChild
					{
						get { return last; }
					}

			// Get the last line of this node.
			public override TextLine LastLine
					{
						get
						{
							// simply return the line if we have it
							if(level == 1) { return (TextLine)last; }

							// recurse down
							return last.LastLine;
						}
					}


			// Find a line by a character offset.
			public override TextLine FindLineByCharOffset
						(int offset, ref int actualOffset)
					{
						// set the current character offset
						int currOffset = 0;

						// set the current child to the first
						TextNode child = first;

						// find and return the matching line
						while(child != null)
						{
							// return the line if it matches
							if(offset <= (currOffset + child.charCount))
							{
								// return the line if that's what we have
								if(child.level == 0)
								{
									// return the line
									return (TextLine)child;
								}

								// search the group for the line
								return child.FindLineByCharOffset
									((offset - currOffset), ref actualOffset);
							}

							// update the current character offset
							currOffset += child.charCount;

							// update the actual character offset
							actualOffset += child.charCount;

							// set the current child to the next
							child = child.next;
						}

						// return null since there's no matching line
						return null;
					}

			// Find a line by a line number.
			public override TextLine FindLineByLineNumber(int num)
					{
						// set the current line number
						int currNum = 0;

						// set the current child to the first
						TextNode child = first;

						// find and return the matching line
						while(child != null)
						{
							// return the line if it matches
							if(num <= (currNum + child.lineCount))
							{
								// return the line if that's what we have
								if(child.level == 0)
								{
									// return the line
									return (TextLine)child;
								}

								// search the group for the line
								return child.FindLineByLineNumber(num - currNum);
							}

							// update the current line number
							currNum += child.lineCount;

							// set the current child to the next
							child = child.next;
						}

						// return null since there's no matching line
						return null;
					}

			// Find a line by a y position.
			public override TextLine FindLineByY(int y, ref int actualY)
					{
						// set the current y
						int currY = 0;

						// set the current child to the first
						TextNode child = first;

						// find and return the matching line
						while(child != null)
						{
							// get the height for the child
							int height = child.size.Height;

							// return the line if it matches
							if(y < (currY + height))
							{
								// return the line if that's what we have
								if(child.level == 0)
								{
									// return the line
									return (TextLine)child;
								}

								// search the group for the line
								return child.FindLineByY
									((y - currY), ref actualY);
							}

							// update the current y position
							currY += height;

							// update the actual y position
							actualY += height;

							// set the current child to the next
							child = child.next;
						}

						// return null since there's no matching line
						return null;
					}

			// Insert a child into the given position.
			public override void InsertChild(TextNode prev, TextNode child)
					{
						// remove the child from its old parent
						if(child.parent != null)
						{
							child.parent.RemoveChild(child);
						}

						// insert the child after the given previous child
						if(prev == null)
						{
							if(first == null)
							{
								// reset the last child of this node
								last = child;
							}
							else
							{
								// reset the child's next reference
								child.next = first;

								// reset the first child's prev reference
								first.prev = child;
							}

							// reset the first child of this node
							first = child;
						}
						else if(prev.parent == this)
						{
							// get the previous sibling's next reference
							TextNode next = prev.next;

							// reset the child's prev reference
							child.prev = prev;

							// reset the previous sibling's next reference
							prev.next = child;

							if(next == null)
							{
								// reset the last child of this node
								last = child;
							}
							else
							{
								// reset the child's next reference
								child.next = next;

								// reset the next sibling's prev reference
								next.prev = child;
							}
						}
						else
						{
							// internal error
							throw new InvalidOperationException();
						}

						// reset the child's parent field
						child.parent = this;

						// update child count
						++childCount;

						// update aggregate line/char information for this node
						UpdateCharCount(child.charCount);
						UpdateLineCount(child.lineCount);

						// invalidate this node and its ancestors, if needed
						if(valid) { Invalidate(); }
					}

			// Insert children into the given position.
			public override void InsertChildren(TextNode prev, TextNode first)
					{
						// remove the children from their old parent
						if(first.parent != null)
						{
							first.parent.RemoveChildren(first);
						}

						// set up the character count delta
						int charCountDelta = 0;

						// set up the line count delta
						int lineCountDelta = 0;

						// set up the last new child
						TextNode last = null;

						// set up the current new child
						TextNode child = first;

						// set the parent of the children and get delta information
						while(child != null)
						{
							// reset the child's parent to this group
							child.parent = this;

							// add the child's character count to the delta
							charCountDelta += child.charCount;

							// add the child's character count to the delta
							lineCountDelta += child.lineCount;

							// update the child count
							++childCount;

							// set the last new child to the current
							last = child;

							// set the current new child to the next
							child = child.next;
						}

						// insert the new children after the given previous child
						if(prev == null)
						{
							if(this.first == null)
							{
								// reset the last child of this node
								this.last = last;
							}
							else
							{
								// reset the last new child's next reference
								last.next = this.first;

								// reset the old first child's prev reference
								this.first.prev = last;
							}

							// reset the first child of this node
							this.first = first;
						}
						else if(prev.parent == this)
						{
							// get the previous sibling's next reference
							TextNode next = prev.next;

							// reset the first new child's prev reference
							first.prev = prev;

							// reset the previous sibling's next reference
							prev.next = first;

							if(next == null)
							{
								// reset the last child of this node
								this.last = last;
							}
							else
							{
								// reset the last new child's next reference
								last.next = next;

								// reset the next sibling's prev reference
								next.prev = last;
							}
						}
						else
						{
							// internal error
							throw new InvalidOperationException();
						}

						// update aggregate line/char information for this node
						UpdateCharCount(charCountDelta);
						UpdateLineCount(lineCountDelta);

						// invalidate this node and its ancestors, if needed
						if(valid) { Invalidate(); }
					}

			// Insert children into the given position.
			public override void InsertChildren
						(TextNode prev, TextNode first, int count)
					{
						// remove the children from their old parent
						if(first.parent != null)
						{
							first.parent.RemoveChildren(first, count);
						}
						else
						{
							// set up the last new child
							TextNode last = null;

							// set up the current new child
							TextNode child = first;

							// find the last new child
							for(int i = 0; i < count; ++i)
							{
								// set the last new child to the current
								last = child;

								// set the current new child to the next
								child = child.next;
							}

							// reset the before first's next reference
							if(first.prev != null) { first.prev.next = last.next; }

							// reset the after last's prev reference
							if(last.next != null) { last.next.prev = first.prev; }

							// reset the first new child's prev reference
							first.prev = null;

							// reset the last new child's next reference
							last.next = null;
						}

						// insert the new children into the given position
						InsertChildren(prev, first);
					}

			// Rebalance the tree at this node.
			public override void Rebalance(TextTree tree)
					{
						// get this node
						TextGroup node = this;

						// rebalance up the tree
						while(node != null)
						{
							// enforce upper bound on child count
							while(node.childCount > MAX_CHILD_COUNT)
							{
								// create a new root before splitting current root
								if(node.parent == null)
								{
									// create the new root node
									TextGroup root = new TextGroup();

									// set the new root's level
									root.level = (node.level + 1);

									// insert the old root under the new root
									root.InsertChild(null, node);

									// set the root of the tree to the new root
									tree.root = root;
								}

								// create the new group for the excess children
								TextGroup group = new TextGroup();

								// set the new group's level
								group.level = node.level;

								// get the first child of the node to be split
								TextNode child = node.first;

								// find the first excess child
								for(int i = 1; i < MIN_CHILD_COUNT; ++i)
								{
									child = child.next;
								}

								// insert first excess child under the new group
								group.InsertChildren(null, child.next);

								// insert group under the current node's parent
								node.parent.InsertChild(node, group);

								// set the current node to the new group
								node = group;
							}

							// enforce lower bound on child count
							while(node.childCount < MIN_CHILD_COUNT)
							{
								// handle the root node case
								if(node.parent == null)
								{
									// root needs at least two children or one line
									if((node.childCount == 1) && (node.level > 1))
									{
										// set the root to its only child
										tree.root = (TextGroup)node.first;

										// remove new root from the old root
										node.RemoveChild(tree.root);
									}

									// we're done rebalancing
									return;
								}

								// rebalance the parent, if too few siblings
								if(node.parent.ChildCount < 2)
								{
									// rebalance the parent
									node.parent.Rebalance(tree);

									// try the current node again
									continue;
								}

								// get the next sibling of the current node
								TextGroup next = (TextGroup)node.next;

								// use previous and swap, if no next sibling
								if(next == null)
								{
									// set the next node to the current node
									next = node;

									// reset current node to previous sibling
									node = (TextGroup)node.prev;
								}

								// calculate the total child count for the nodes
								int fullCount = node.childCount + next.childCount;

								// merge the nodes or split up the children
								if(fullCount <= MAX_CHILD_COUNT)
								{
									// merge the children under a single node
									node.InsertChildren(node.last, next.first);

									// remove the empty node from the tree
									node.parent.RemoveChild(next);

									// continue rebalancing at this node, if needed
									if(fullCount < MIN_CHILD_COUNT) { continue; }
								}
								else
								{
									// calculate the first node's new child count
									int halfCount = (fullCount / 2);

									// move children around as needed
									if(node.childCount < halfCount)
									{
										// move the needed children to the first node
										node.InsertChildren
											(node.last, next.first,
											 (halfCount - node.childCount));
									}
									else if(node.childCount > halfCount)
									{
										// get the first child of the first node
										TextNode child = node.first;

										// get the first of the children to move
										for(int i = 1; i < halfCount; ++i)
										{
											child = child.next;
										}

										// move extra children to the second node
										next.InsertChildren(null, child);
									}
								}
							}

							// set the current node to its parent
							node = (TextGroup)node.parent;
						}
					}

			// Remove the given child from this node.
			public override void RemoveChild(TextNode child)
					{
						// ensure the child is valid
						if(child == null || child.parent != this)
						{
							throw new InvalidOperationException();
						}

						// get the previous sibling of the child
						TextNode prev = child.prev;

						// get the next sibling of the child
						TextNode next = child.next;

						// reset the previous sibling's next reference
						if(prev != null) { prev.next = next; }

						// reset the next sibling's prev reference
						if(next != null) { next.prev = prev; }

						// reset the first child of this node, if needed
						if(first == child) { first = next; }

						// reset the last child of this node, if needed
						if(last == child) { last = prev; }

						// reset the child's references
						child.parent = null;
						child.prev = null;
						child.next = null;

						// update child count
						--childCount;

						// update aggregate line/char information for this node
						UpdateCharCount(-child.charCount);
						UpdateLineCount(-child.lineCount);

						// invalidate this node and its ancestors, if needed
						if(valid) { Invalidate(); }
					}

			// Remove children from this node.
			public override void RemoveChildren(TextNode first)
					{
						// ensure the child is valid
						if(first == null || first.parent != this)
						{
							throw new InvalidOperationException();
						}

						// set up the character count delta
						int charCountDelta = 0;

						// set up the line count delta
						int lineCountDelta = 0;

						// set up the last removal child
						TextNode last = null;

						// set up the current removal child
						TextNode child = first;

						// set the parent of the children and get delta information
						while(child != null)
						{
							// reset the child's parent to null
							child.parent = null;

							// subtract child's character count from the delta
							charCountDelta -= child.charCount;

							// subtract child's character count from the delta
							lineCountDelta -= child.lineCount;

							// update the child count
							--childCount;

							// set the last removal child to the current
							last = child;

							// set the current removal child to the next
							child = child.next;
						}

						// remove the children from their current position
						if(first.prev == null)
						{
							this.first = null;
							this.last = null;
						}
						else
						{
							// get the first removal child's prev reference
							TextNode prev = first.prev;

							// reset the first removal child's prev reference
							first.prev = null;

							// reset the previous sibling's next reference
							prev.next = null;

							// reset the last child of this node
							this.last = prev;

							// reset the first child of this node
							if(prev.prev == null) { this.first = prev; }
						}

						// update aggregate line/char information for this node
						UpdateCharCount(charCountDelta);
						UpdateLineCount(lineCountDelta);

						// invalidate this node and its ancestors, if needed
						if(valid) { Invalidate(); }
					}

			// Remove children from this node.
			public override void RemoveChildren(TextNode first, int count)
					{
						// ensure the child is valid
						if(first == null || first.parent != this)
						{
							throw new InvalidOperationException();
						}

						// set up the character count delta
						int charCountDelta = 0;

						// set up the line count delta
						int lineCountDelta = 0;

						// set up the last removal child
						TextNode last = null;

						// set up the current removal child
						TextNode child = first;

						// set the parent of the children and get delta information
						for(int i = 0; i < count; ++i)
						{
							// reset the child's parent to null
							child.parent = null;

							// subtract the child's character count from the delta
							charCountDelta -= child.charCount;

							// subtract the child's character count from the delta
							lineCountDelta -= child.lineCount;

							// set the last removal child to the current
							last = child;

							// set the current removal child to the next
							child = child.next;
						}

						// reset the before first's next reference
						if(first.prev == null)
						{
							this.first = last.next;
						}
						else
						{
							first.prev.next = last.next;
						}

						// reset the after last's prev reference
						if(last.next == null)
						{
							this.last = first.prev;
						}
						else
						{
							last.next.prev = first.prev;
						}

						// reset the first removal child's prev reference
						first.prev = null;

						// reset the last removal child's next reference
						last.next = null;

						// update the child count
						childCount -= count;

						// update aggregate line/char information for this node
						UpdateCharCount(charCountDelta);
						UpdateLineCount(lineCountDelta);

						// invalidate this node and its ancestors, if needed
						if(valid) { Invalidate(); }
					}

			// Update the metrics of this node.
			public override void UpdateMetrics(TextLayout layout, bool force)
					{
						if(force || !valid)
						{
							// clear the metrics information
							size = Size.Empty;

							// get the first child of this group
							TextNode child = first;

							// update child metrics and add to group's metrics
							while(child != null)
							{
								// update the child's metrics information
								child.UpdateMetrics(layout, force);

								// add the metrics information
								size = layout.AddMetrics(size, child.size);

								// move to the next child
								child = child.next;
							}

							// flag that the metrics information is now valid
							valid = true;
						}
					}

		}; // class TextGroup
	#endregion // TextGroup

	}; // class TextTree
#endregion // TextTree

#region    // GapVector
	internal abstract class GapVector
	{
		// Internal state.
		protected Array array;
		protected int gapStart;
		protected int gapEnd;


		// Constructor.
		public GapVector(int initialCapacity)
				{
					// create the array
					array = Array.CreateInstance(ItemType, initialCapacity);

					// set the gap start
					gapStart = 0;

					// set the gap end
					gapEnd = initialCapacity;
				}


		// Get the type of the stored items.
		protected abstract Type ItemType { get; }


		// Expand the size of the gap to the given size.
		protected virtual void ExpandGap(int newGapSize)
				{
					// calculate the old gap size
					int gapSize = (gapEnd - gapStart);

					// expand the gap, if needed
					if(newGapSize > gapSize)
					{
						// get the old array length
						int len = array.Length;

						// calculate the minimum new array length
						int minLen = (((len + (newGapSize - gapSize)) + 31) & ~31);

						// calculate a possible new array length
						int newLen = (len * 2);

						// use the larger new array length
						if(newLen < minLen) { newLen = minLen; }

						// create the new array
						Array newArray = Array.CreateInstance(ItemType, newLen);

						// calculate the new gap end
						int newGapEnd = (gapEnd + (newLen - len));

						// copy the items before the gap to the new array
						Array.Copy(array, 0, newArray, 0, gapStart);

						// copy the items after the gap to the new array
						Array.Copy(array, gapEnd, newArray, gapEnd, (len - gapEnd));

						// reset the gap end to the new gap end
						gapEnd = newGapEnd;

						// reset the array to the new array
						array = newArray;
					}
				}

		// Shift the gap by the given delta.
		protected virtual void ShiftGap(int delta)
				{
					// bail out now if there's no shifting to be done
					if(delta == 0) { return; }

					// calculate the new gap information
					int newGapStart = gapStart + delta;
					int newGapEnd = gapEnd + delta;

					// shift the gap based on the delta
					if(delta > 0)
					{
						Array.Copy(array, gapEnd, array, gapStart, delta);
					}
					else
					{
						Array.Copy(array, newGapStart, array, newGapEnd, -delta);
					}

					// update the gap start/end information
					gapStart = newGapStart;
					gapEnd = newGapEnd;
				}

	}; // class GapVector
#endregion // GapVector

#region    // TextSlice
	internal sealed class TextSlice
	{
		// Publicly-accessible state.
		public char[] chars;
		public int    start;
		public int    length;


		public override String ToString()
				{
					return new String(chars, start, length);
				}

	}; // class TextSlice
#endregion // TextSlice

#region    // ITextMark
	internal interface ITextMark
	{
		// Get the text buffer containing the marked position.
		TextBuffer Buffer { get; }

		// Get the character offset of the marked position.
		int Offset { get; }

		// Determine if this mark has been deleted.
		bool Deleted { get; }

		// Determine if this mark has left gravity.
		bool LeftGravity { get; }

		// Delete this mark from its owning buffer.
		void Delete();

		// Move this mark to a new position.
		void Move(int newPos);

	}; // interface ITextMark
#endregion // ITextMark

#region    // TextBuffer
	internal sealed class TextBuffer : GapVector
	{
		// Internal state.
		private TextMarks leftMarks;
		private TextMarks rightMarks;

		// NOTE: marks with left gravity which are attached to the gap end
		//       character are positioned at the gap start, so the offset
		//       calculations and adjustments required are slightly different
		//       from those of marks with right gravity... right gravity marks
		//       attached to the gap end character are simply placed at the gap
		//       end... unless dealing with the gap start/end edge cases the
		//       marks should be treated the same... it is the edge case
		//       handling which gives them their gravity


		// Constructor.
		public TextBuffer()
				: base(256)
				{
					// add the extra character (simplifies things)
					((char[])array)[--gapEnd] = '\n';

					// create the left marks list
					leftMarks = new TextMarks(8);

					// create the right marks list
					rightMarks = new TextMarks(8);
				}


		// Get the character at the given position.
		public char this[int pos]
				{
					get
					{
						// move the position past the gap if needed
						if(pos < gapStart) { pos += (gapEnd - gapStart); }

						// return the character
						return ((char[])array)[pos];
					}
				}

		// Get a string with the given length, from the given position.
		public String this[int pos, int length]
				{
					get
					{
						// create a text slice
						TextSlice slice = new TextSlice();

						// get the slice data for the given range
						GetSlice(pos, length, slice);

						// create a new string from the slice
						return slice.ToString();
					}
				}

		// Get the character count for this buffer.
		public int CharCount
				{
					get { return array.Length - ((gapEnd - gapStart) + 1); }
				}

		// Get the type of the stored items.
		protected override Type ItemType
				{
					get { return typeof(char); }
				}


		// Build a text slice into the given string builder.
		public void BuildSlice(int pos, int length, StringBuilder sb)
				{
					// get the character count
					int charCount = CharCount;

					// enforce position bounds limits
					if(pos < 0 || pos >= charCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// enforce length bounds limits
					if(length <= 0 || (pos + length) >= charCount)
					{
						throw new ArgumentOutOfRangeException("length");
					}

					// get the character data
					char[] chars = (char[])array;

					// build the slice into the builder
					if((pos + length) <= gapStart)
					{
						// append the character data
						sb.Append(chars, pos, length);
					}
					else if(pos >= gapStart)
					{
						// append the character data
						sb.Append(chars, (pos + (gapEnd - gapStart)), length);
					}
					else
					{
						// get the number of characters to copy before the gap
						int before = (gapStart - pos);

						// get the number of characters to copy after the gap
						int after = (length - before);

						// append the before gap characters to the builder
						sb.Append(chars, pos, before);

						// append the after gap characters to the builder
						sb.Append(chars, gapEnd, after);
					}
				}
		public void BuildSlice(int pos, int length, StringBuilder sb, int index)
				{
					// get the character count
					int charCount = CharCount;

					// enforce position bounds limits
					if(pos < 0 || pos >= charCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// enforce length bounds limits
					if(length <= 0 || (pos + length) >= charCount)
					{
						throw new ArgumentOutOfRangeException("length");
					}

					// get the character data
					char[] chars = (char[])array;

					// build the slice into the builder
					if((pos + length) <= gapStart)
					{
						// insert the character data
						sb.Insert(index, chars, pos, length);
					}
					else if(pos >= gapStart)
					{
						// insert the character data
						sb.Insert
							(index, chars, (pos + (gapEnd - gapStart)), length);
					}
					else
					{
						// get the number of characters to copy before the gap
						int before = (gapStart - pos);

						// get the number of characters to copy after the gap
						int after = (length - before);

						// insert the before gap characters into the builder
						sb.Insert(index, chars, pos, before);

						// insert the after gap characters into the builder
						sb.Insert((index + before), chars, gapEnd, after);
					}
				}

		// Get a text slice with the given length, from the given position.
		public void GetSlice(int pos, int length, TextSlice slice)
				{
					// get the character count
					int charCount = CharCount;

					// enforce position bounds limits
					if(pos < 0 || pos >= charCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// enforce length bounds limits
					if(length <= 0 || (pos + length) >= charCount)
					{
						throw new ArgumentOutOfRangeException("length");
					}

					// set the slice length
					slice.length = length;

					// set the slice character array and start position
					if((pos + length) <= gapStart)
					{
						// set the slice character array
						slice.chars = (char[])array;

						// set the slice start position
						slice.start = pos;
					}
					else if(pos >= gapStart)
					{
						// set the slice character array
						slice.chars = (char[])array;

						// set the slice start position
						slice.start = (pos + (gapEnd - gapStart));
					}
					else
					{
						// create the slice character array
						slice.chars = new char[length];

						// set the slice start position
						slice.start = 0;

						// get the number of characters to copy before the gap
						int before = (gapStart - pos);

						// get the number of characters to copy after the gap
						int after = (length - before);

						// copy the before gap characters to the slice array
						Array.Copy(array, pos, slice.chars, 0, before);

						// copy the after gap characters to the slice array
						Array.Copy(array, gapEnd, slice.chars, before, after);
					}
				}

		// Insert the given character data at the given position.
		public UndoInfo Insert(int pos, String str)
				{
					// get the characters from the string
					char[] c = str.ToCharArray();

					// perform the insertion and return the undo information
					return Insert(pos, c, 0, c.Length);
				}
		public UndoInfo Insert(int pos, String str, int start, int length)
				{
					// get the characters from the string
					char[] c = str.ToCharArray(start, length);

					// perform the insertion and return the undo information
					return Insert(pos, c, 0, c.Length);
				}
		public unsafe UndoInfo Insert
					(int pos, char[] chars, int start, int length)
				{
					// enforce position bounds limits
					if(pos < 0 || pos >= CharCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// adjust the gap as needed
					ExpandGap(length);
					ShiftGap(gapStart - pos);

					// copy data and normalize line ending characters, quickly
					fixed(char* startInput = &(chars[start]),
					      startOutput = &(((char[])array)[gapStart]))
					{
						// save a copy of the output start position
						// use pointer aliases for pointer arithmetic. csc doesn't like pointer arithmetic
						// done on pointers declared within the fixed expression
						char* input = startInput;
						char* output = startOutput;
						char* outputStart = output;

						// get the input end position
						char* end = (input + length);

						// copy the character data and normalize newlines
						while(input != end)
						{
							if(*input == '\r')
							{
								// skip a character if at a CRLF sequence
								if((input + 1) != end && *(input + 1) == '\n')
								{
									++input;
								}

								// output the newline character
								*output = '\n';
							}
							else
							{
								// output the current character
								*output = *input;
							}

							// move to the next output position
							++output;

							// move to the next input position
							++input;
						}

						// set the length to the actual length
						length = (int)(output - outputStart);
					}

					// update the gap start and marks
					gapStart += length;

				#if false
					// return the undo information
					return new InsertUndo(pos, length);
				#else
					return null;
				#endif
				}
		public UndoInfo Insert(int pos, char c)
				{
					// enforce position bounds limits
					if(pos < 0 || pos >= CharCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// adjust the gap as needed
					ExpandGap(1);
					ShiftGap(gapStart - pos);

					// normalize line ending characters
					if(c == '\r') { c = '\n'; }

					// add the new character data to the buffer
					((char[])array)[gapStart++] = c;

				#if false
					// return the undo information
					return new InsertUndo(pos, 1);
				#else
					return null;
				#endif
				}

		// Mark a position in the buffer.
		public ITextMark MarkPosition(int pos, bool leftGravity)
				{
					// enforce position bounds limits
					if(pos < 0 || pos > CharCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					if(leftGravity)
					{
						// adjust the mark index for the gap
						if(pos > gapStart) { pos += (gapEnd - gapStart); }

						// create the new mark
						LeftMark mark = new LeftMark(this, pos);

						// find the insertion point
						pos = leftMarks.MarkInsertionSearch(pos);

						// insert the new mark
						leftMarks.Insert(pos, mark);

						// create and return the new position marker
						return mark;
					}
					else
					{
						// adjust the mark index for the gap
						if(pos >= gapStart) { pos += (gapEnd - gapStart); }

						// create the new mark
						RightMark mark = new RightMark(this, pos);

						// find the insertion point
						pos = rightMarks.MarkInsertionSearch(pos);

						// insert the new mark
						rightMarks.Insert(pos, mark);

						// create and return the new position marker
						return mark;
					}
				}

    	// Remove the given number of characters at the given position.
		public UndoInfo Remove(int pos, int length)
				{
					// get the character count
					int charCount = CharCount;

					// enforce position bounds limits
					if(pos < 0 || pos >= charCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					// enforce length bounds limits
					if(length <= 0 || (pos + length) > charCount)
					{
						throw new ArgumentOutOfRangeException("length");
					}

				#if false
					// get the undo information
					UndoInfo undo = new RemoveUndo(pos, this[pos, length]);
				#endif

					// shift the gap to the removal position
					ShiftGap(gapStart - pos);

					// perform the removal
					gapEnd += length;

					// shift the left marks at the gap end to the gap start
					leftMarks.AdjustMarksMove
						((gapStart + 1), (gapEnd + 1), gapStart);

				#if false
					// return the undo information
					return undo;
				#else
					return null;
				#endif
				}

		// Expand the size of the gap to the given size.
		protected override void ExpandGap(int newGapSize)
				{
					// save the old gap end information
					int oldGapEnd = gapEnd;

					// expand the gap
					base.ExpandGap(newGapSize);

					// get the change in gap size
					int delta = (gapEnd - oldGapEnd);

					// shift the left marks in the adjustment range by delta
					leftMarks.AdjustMarks(oldGapEnd, delta);

					// shift the right marks in the adjustment range by delta
					rightMarks.AdjustMarks(oldGapEnd, delta);
				}

		// Shift the gap by the given delta.
		protected override void ShiftGap(int delta)
				{
					// bail out now if there's no shifting to be done
					if(delta == 0) { return; }

					// save the old gap start information
					int oldGapStart = gapStart;

					// save the old gap end information
					int oldGapEnd = gapEnd;

					// shift the gap
					base.ShiftGap(delta);

					// get the size of the gap
					int gapSize = (gapEnd - gapStart);

					// update the position markers
					if(delta < 0)
					{
						// shift the left marks in the adjustment range up
						leftMarks.AdjustMarks(gapStart, oldGapEnd, gapSize);

						// shift the right marks in the adjustment range up
						rightMarks.AdjustMarks(gapStart, oldGapEnd, gapSize);
					}
					else
					{
						// shift the left marks in the adjustment range down
						leftMarks.AdjustMarks(oldGapStart, gapEnd, -gapSize);

						// shift the right marks in the adjustment range down
						rightMarks.AdjustMarks(oldGapStart, gapEnd, -gapSize);
					}

					// shift the left marks at the gap end to the gap start
					leftMarks.AdjustMarksMove
						((gapStart + 1), (gapEnd + 1), gapStart);

					// shift the right marks at the gap start to the gap end
					rightMarks.AdjustMarksMove(gapStart, gapEnd, gapEnd);
				}

		// Move a mark.
		private void MoveMark(TextMark mark, int pos)
				{
					// enforce position bounds limits
					if(pos < 0 || pos > CharCount)
					{
						throw new ArgumentOutOfRangeException("pos");
					}

					if(mark.LeftGravity)
					{
						// adjust the mark index for the gap
						if(pos > gapStart) { pos += (gapEnd - gapStart); }

						// move the mark
						leftMarks.MoveMark(mark, pos);
					}
					else
					{
						// adjust the mark index for the gap
						if(pos >= gapStart) { pos += (gapEnd - gapStart); }

						// move the mark
						rightMarks.MoveMark(mark, pos);
					}
				}

		// Remove a mark.
		private void RemoveMark(TextMark mark)
				{
					if(mark.LeftGravity)
					{
						// remove the mark
						leftMarks.Remove(leftMarks.MarkSearch(mark), 1);
					}
					else
					{
						// remove the mark
						rightMarks.Remove(rightMarks.MarkSearch(mark), 1);
					}
				}
























	#region    // TextMark
		private abstract class TextMark : ITextMark
		{
			// Internal state.
			public int        index;
			public TextBuffer buffer;


			// Constructor.
			protected TextMark(TextBuffer buffer, int index)
					{
						this.buffer = buffer;
						this.index = index;
					}


			// Get the character offset of the marked position.
			public abstract int Offset { get; }

			// Get the text buffer containing the marked position.
			public TextBuffer Buffer
					{
						get { return buffer; }
					}

			// Determine if this mark has been deleted.
			public bool Deleted
					{
						get { return (buffer == null); }
					}

			// Determine if this mark has left gravity.
			public abstract bool LeftGravity { get; }


			// Delete this mark from its owning buffer.
			public void Delete()
					{
						buffer.RemoveMark(this);
						buffer = null;
						index = -1;
					}

			// Move this mark to a new position.
			public void Move(int pos)
					{
						buffer.MoveMark(this, pos);
					}

		}; // class TextMark
	#endregion // TextMark

	#region    // LeftMark
		private sealed class LeftMark : TextMark
		{
			// Constructor.
			public LeftMark(TextBuffer buffer, int index)
					: base(buffer, index)
					{
						// nothing to do here
					}


			// Get the character offset of the marked position.
			public override int Offset
					{
						get
						{
							// return the index if it matches the offset
							if(index <= buffer.gapStart) { return index; }

							// return the adjusted index
							return (index - (buffer.gapEnd - buffer.gapStart));
						}
					}

			// Determine if this mark has left gravity.
			public override bool LeftGravity
					{
						get { return true; }
					}

		}; // class LeftMark
	#endregion // LeftMark

	#region    // RightMark
		private sealed class RightMark : TextMark
		{
			// Constructor.
			public RightMark(TextBuffer buffer, int index)
					: base(buffer, index)
					{
						// nothing to do here
					}


			// Get the character offset of the marked position.
			public override int Offset
					{
						get
						{
							// return the index if it matches the offset
							if(index < buffer.gapStart) { return index; }

							// return the adjusted index
							return (index - (buffer.gapEnd - buffer.gapStart));
						}
					}

			// Determine if this mark has left gravity.
			public override bool LeftGravity
					{
						get { return false; }
					}

		}; // class RightMark
	#endregion // RightMark

	#region    // TextMarks
		private sealed class TextMarks : GapVector
		{
			// Constructor.
			public TextMarks(int initialCapacity)
					: base(initialCapacity)
					{
						// nothing to do here
					}


			// Get the type of the stored items.
			protected override Type ItemType
					{
						get { return typeof(TextMark); }
					}

			// Get the mark at the given position.
			public TextMark this[int pos]
					{
						get
						{
							// move the position past the gap if needed
							if(pos < gapStart) { pos += (gapEnd - gapStart); }

							// return the mark
							return ((TextMark[])array)[pos];
						}
					}

			// Get the mark count.
			public int MarkCount
					{
						get { return array.Length - (gapEnd - gapStart); }
					}


			// Adjust the marks at or after the start by the given delta.
			public void AdjustMarks(int start, int delta)
					{
						// get the adjustment position
						int i = MarkAdjustmentSearch(start);

						// get the mark array
						TextMark[] marks = (TextMark[])array;

						// shift the marks in the adjustment range below the gap
						while(i < gapStart) { marks[i++].index += delta; }

						// move past the gap
						i += (gapEnd - gapStart);

						// get the length of the array
						int len = marks.Length;

						// shift the marks in the adjustment range above the gap
						while(i < len) { marks[i++].index += delta; }
					}

			// Adjust the marks in the given range by the given delta.
			public void AdjustMarks(int start, int end, int delta)
					{
						// get the adjustment position
						int i = MarkAdjustmentSearch(start);

						// get the mark array
						TextMark[] marks = (TextMark[])array;

						// shift the marks in the adjustment range below the gap
						while(i < gapStart)
						{
							// get the current mark
							TextMark mark = marks[i++];

							// return if we're past the adjustment range
							if(mark.index >= end) { return; }

							// shift the mark by the delta
							mark.index += delta;
						}

						// move past the gap
						i += (gapEnd - gapStart);

						// get the length of the array
						int len = marks.Length;

						// shift the marks in the adjustment range above the gap
						while(i < len)
						{
							// get the current mark
							TextMark mark = marks[i++];

							// return if we're past the adjustment range
							if(mark.index >= end) { return; }

							// shift the mark by the delta
							mark.index += delta;
						}
					}

			// Adjust the marks in the given range to the new position.
			public void AdjustMarksMove(int start, int end, int newPos)
					{
						// NOTE: this method assumes the move won't unsort
						//       the marks... use with caution... also note
						//       that for left marks which use an end of
						//       gapEnd plus one, overflow causing minus
						//       one will still work as intended ;)

						// get the adjustment position
						int i = MarkAdjustmentSearch(start);

						// get the mark array
						TextMark[] marks = (TextMark[])array;

						// move the marks in the adjustment range below the gap
						while(i < gapStart)
						{
							// get the current mark
							TextMark mark = marks[i++];

							// return if we're past the adjustment range
							if(mark.index >= end) { return; }

							// move the mark to the new position
							mark.index = newPos;
						}

						// move past the gap
						i += (gapEnd - gapStart);

						// get the length of the array
						int len = marks.Length;

						// move the marks in the adjustment range above the gap
						while(i < len)
						{
							// get the current mark
							TextMark mark = marks[i++];

							// return if we're past the adjustment range
							if(mark.index >= end) { return; }

							// move the mark to the new position
							mark.index = newPos;
						}
					}

			// Insert a mark at the given position.
			public void Insert(int pos, TextMark mark)
					{
						// enforce position bounds limits
						if(pos < 0 || pos >= MarkCount)
						{
							throw new ArgumentOutOfRangeException("pos");
						}

						// adjust the gap as needed
						ExpandGap(1);
						ShiftGap(gapStart - pos);

						// add the new mark to the buffer
						((TextMark[])array)[gapStart++] = mark;
					}

			// Search for a mark adjustment position.
			public int MarkAdjustmentSearch(int pos)
					{
						// search for the mark position
						int index = MarkSearch(pos);

						// return the complement of the index if it's the target
						if(index < 0) { return ~index; }

						// find and return the adjustment position
						for(--index; index >= 0; --index)
						{
							// get the current mark
							TextMark mark = this[index];

							// return the index if the mark doesn't match
							if(mark.index != pos) { return (index + 1); }
						}

						// return the adjustment position
						return 0;
					}

			// Search for a mark insertion position.
			public int MarkInsertionSearch(int pos)
					{
						// search for the mark position
						int index = MarkSearch(pos);

						// return the complement of the index if it's the target
						if(index < 0) { return ~index; }

						// get the marks count
						int markCount = MarkCount;

						// find and return the optimal (last) insertion position
						for(++index; index < markCount; ++index)
						{
							// get the current mark
							TextMark mark = this[index];

							// return the index if the mark doesn't match
							if(mark.index != pos) { return (index - 1); }
						}

						// return the insertion position
						return markCount;
					}

			// Search for the position of the given mark.
			public int MarkSearch(TextMark mark)
					{
						// search for the mark position
						int index = MarkSearch(mark.index);

						// return minus one if there's no match
						if(index < 0) { return -1; }

						// return the index if it's a match
						if(this[index] == mark) { return index; }

						// get the marks count
						int markCount = MarkCount;

						// find and return the removal position, forward search
						for(int i = (index + 1); i < markCount; ++i)
						{
							// get the current mark
							TextMark curr = this[i];

							// compare the current mark against the removal mark
							if(curr == mark) { return i; }

							// break if there are no more forward candidates
							if(curr.index != mark.index) { break; }
						}

						// find and return the removal position, backward search
						for(--index; index >= 0; --index)
						{
							// get the current mark
							TextMark curr = this[index];

							// compare the current mark against the removal mark
							if(curr == mark) { return index; }

							// break if there are no more backward candidates
							if(curr.index != mark.index) { break; }
						}

						// return minus one if there's no match
						return -1;
					}

			// Search for a mark position for the given mark index.
			public int MarkSearch(int pos)
					{
						// set up the boundary indices
						int left = 0;
						int right = (MarkCount - 1);

						// subdivide and search until boundaries meet
						while(left <= right)
						{
							// calculate the middle position
							int middle = ((left + right) / 2);

							// get the middle item
							TextMark mark = this[middle];

							// continue search or return depending on comparison
							if(mark.index == pos)
							{
								return middle;
							}
							else if(mark.index < pos)
							{
								right = (middle - 1);
							}
							else
							{
								left = (middle + 1);
							}
						}

						// return the complement of the next index
						return ~left;
					}

			// Move a mark to a new position.
			public void MoveMark(TextMark mark, int newPos)
					{
						// bail out now if there's no movement required
						if(mark.index == newPos) { return; }

						// get the mark position
						int i = MarkSearch(mark);

						// remove the mark from the old position
						Remove(i, 1);

						// get the mark insertion index
						i = MarkInsertionSearch(newPos);

						// insert the mark at the new position
						Insert(i, mark);

						// set the mark index to the new position
						mark.index = newPos;
					}

			// Remove the given number of marks at the given position.
			public void Remove(int pos, int length)
					{
						// get the mark count
						int markCount = MarkCount;

						// enforce position bounds limits
						if(pos < 0 || pos >= markCount)
						{
							throw new ArgumentOutOfRangeException("pos");
						}

						// enforce length bounds limits
						if(length <= 0 || (pos + length) >= markCount)
						{
							throw new ArgumentOutOfRangeException("length");
						}

						// shift the gap to the removal position
						ShiftGap(gapStart - (pos + length));

						// perform the removal
						gapStart -= length;
					}

		}; // class TextMarks
	#endregion // TextMarks

	}; // class TextBuffer
#endregion // TextBuffer

#region    // UndoInfo
	[TODO]
	internal abstract class UndoInfo
	{
		// Implement

	}; // class UndoInfo
#endregion // UndoInfo

#region    // TextLayout
	internal abstract class TextLayout
	{
		// Internal state.
		protected TextBuffer buffer;


		// Add a child's metrics information to that of its parent.
		public abstract Size AddMetrics(Size size, Size childSize);

		// Find an offset into a line from a relative x,y position.
		public abstract int FindOffsetByXY
			(int startOffset, int endOffset, int x, int y);

		// Get the metrics information for a line of text.
		public abstract Size GetLineMetrics(int startOffset, int endOffset);

		// Layout a line of text.
		public abstract void LayoutLine
			(int startOffset, int endOffset, ref Size size, ref bool valid);

	}; // class TextLayout
#endregion // TextLayout

}; // class TextBoxBase

}; // namespace System.Windows.Forms
