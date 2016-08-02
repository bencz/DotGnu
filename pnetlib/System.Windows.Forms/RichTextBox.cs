/*
 * RichTextBox.cs - Implementation of the
 *		"System.Windows.Forms.RichTextBox" class.
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
#if !CONFIG_COMPACT_FORMS
using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;

public class RichTextBox: TextBoxBase
{

	private bool autoWordSelection;
	private int bulletIndent;
	private bool detectUrls;
	private int rightMargin;
	private RichTextBoxScrollBars scrollBars;
	private float zoomFactor;

	public event ContentsResizedEventHandler ContentsResized;
	public event EventHandler HScroll;
	public event EventHandler ImeChange;
	public event LinkClickedEventHandler LinkClicked;
	public event EventHandler Protected;
	public event EventHandler SelectionChanged;
	public event EventHandler VScroll;

	[TODO]
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

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public override bool AutoSize
	{
		get
		{
			return base.AutoSize;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool AutoWordSelection
	{
		get
		{
			return autoWordSelection;
		}
		set
		{
			autoWordSelection = value;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(0)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public int BulletIndent
	{
		get
		{
			return bulletIndent;
		}
		set
		{
			bulletIndent = value;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool CanRedo
	{
		get
		{
			return false;
		}
	}

	[TODO]
	public bool CanPaste(DataFormats.Format clipFormat)
	{
		throw new NotImplementedException("CanPaste");
	}

	[TODO]
	internal override void CaretSetPosition(int position)
	{
	}

	[TODO]
	protected override void DeleteTextOp(CaretDirection dir)
	{
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool DetectUrls
	{
		get
		{
			return detectUrls;
		}
		set
		{
			detectUrls = value;
		}
	}

	[TODO]
	public int Find(char[] characterSet)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(char[] characterSet, int start)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(char[] characterSet, int start, int end)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(String str)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(String str, RichTextBoxFinds options)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(String str, int start, RichTextBoxFinds options)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public int Find(String str, int start, int end, RichTextBoxFinds options)
	{
		throw new NotImplementedException("Find");
	}

	[TODO]
	public char GetCharFromPosition(Point pt)
	{
		throw new NotImplementedException("GetCharFromPosition");
	}

	[TODO]
	public int GetCharIndexFromPosition(Point pt)
	{
		throw new NotImplementedException("GetCharIndexFromPosition");
	}

	[TODO]
	public int GetLineFromCharIndex(int index)
	{
		throw new NotImplementedException("GetLineFromCharIndex");
	}

	[TODO]
	public Point GetPositionFromCharIndex(int index)
	{
		throw new NotImplementedException("GetPositionFromCharIndex");
	}

	[TODO]
	internal override int GetSelectionLength()
	{
		return 0;
	}

	[TODO]
	internal override int GetSelectionStart()
	{
		return 0;
	}

	[TODO]
	public override String[] Lines
	{
		get
		{
			return new String[0];
		}
		set
		{
		}
	}

	[TODO]
	public void LoadFile(Stream data, RichTextBoxStreamType fileType)
	{
		throw new NotImplementedException("LoadFile");
	}

	[TODO]
	public void LoadFile(String path)
	{
		throw new NotImplementedException("LoadFile");
	}

	[TODO]
	public void LoadFile(String path, RichTextBoxStreamType fileType)
	{
		throw new NotImplementedException("LoadFile");
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(2147483647)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public override int MaxLength
	{
		get
		{
			return base.MaxLength;
		}
		set
		{
			base.MaxLength = value;
		}
	}

	[TODO]
	protected override void MoveCaret(CaretDirection dir, bool extend)
	{
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public override bool Multiline
	{
		get
		{
			return base.Multiline;
		}
		set
		{
			base.Multiline = value;
		}
	}

	[TODO]
	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	[TODO]
	protected virtual void OnContentsResized(ContentsResizedEventArgs e)
	{
		if (ContentsResized != null)
		{
			ContentsResized(this, e);
		}
	}

	[TODO]
	protected override void OnContextMenuChanged(EventArgs e)
	{
		base.OnContextMenuChanged(e);
	}

	[TODO]
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	[TODO]
	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	[TODO]
	protected virtual void OnHScroll(EventArgs e)
	{
		if (HScroll != null)
		{
			HScroll(this, e);
		}
	}

	[TODO]
	protected virtual void OnImeChange(EventArgs e)
	{
		if (ImeChange != null)
		{
			ImeChange(this, e);
		}
	}

	[TODO]
	protected virtual void OnLinkClicked(LinkClickedEventArgs e)
	{
		if (LinkClicked != null)
		{
			LinkClicked(this, e);
		}
	}

	[TODO]
	protected virtual void OnProtected(EventArgs e)
	{
		if (Protected != null)
		{
			Protected(this, e);
		}
	}

	[TODO]
	protected override void OnRightToLeftChanged(EventArgs e)
	{
		base.OnRightToLeftChanged(e);
	}

	[TODO]
	protected virtual void OnSelectionChanged(EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	[TODO]
	protected override void OnSystemColorsChanged(EventArgs e)
	{
		base.OnSystemColorsChanged(e);
	}

	[TODO]
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
	}

	[TODO]
	internal override void OnToggleInsertMode()
	{
	}

	[TODO]
	protected virtual void OnVScroll(EventArgs e)
	{
		if (VScroll != null)
		{
			VScroll(this, e);
		}
	}

	[TODO]
	public void Paste(DataFormats.Format clipFormat)
	{
		throw new NotImplementedException("Paste");
	}

	[TODO]
	public void Redo()
	{
		throw new NotImplementedException("Redo");
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public String RedoActionName
	{
		get
		{
			return String.Empty;
		}
	}

	[TODO]
	public RichTextBox() : base()
	{
		autoWordSelection = false;
		bulletIndent = 0;
		detectUrls = true;
		rightMargin = 0;
		scrollBars = RichTextBoxScrollBars.Both;
		zoomFactor = 1;
		base.MaxLength = 2147483647;
		base.Multiline = true;
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(0)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public int RightMargin
	{
		get
		{
			return rightMargin;
		}
		set
		{
			rightMargin = value;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(null)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public String Rtf
	{
		get
		{
			return String.Empty;
		}
		set
		{
		}
	}

	[TODO]
	public void SaveFile(Stream data, RichTextBoxStreamType fileType)
	{
		throw new NotImplementedException("SaveFile");
	}

	[TODO]
	public void SaveFile(String path)
	{
		throw new NotImplementedException("SaveFile");
	}

	[TODO]
	public void SaveFile(String path, RichTextBoxStreamType fileType)
	{
		throw new NotImplementedException("SaveFile");
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(RichTextBoxScrollBars.Both)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public RichTextBoxScrollBars ScrollBars
	{
		get
		{
			return scrollBars;
		}
		set
		{
			scrollBars = value;
		}
	}

	[TODO]
	protected override void ScrollToCaretInternal()
	{
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue("")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public String SelectedRtf
	{
		get
		{
			return String.Empty;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue("")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public override String SelectedText
	{
		get
		{
			return base.SelectedText;
		}
		set
		{
			base.SelectedText = value;
		}
	}

	[TODO]
	internal override void SelectInternal(int start, int length)
	{
	}

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(HorizontalAlignment.Left)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[TODO]
	public HorizontalAlignment SelectionAlignment
	{
		get
		{
			return HorizontalAlignment.Left;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool SelectionBullet
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public int SelectionCharOffset
	{
		get
		{
			return 0;
		}
		set
		{
			if (value < -2000 || value > 2000)
			{
				throw new ArgumentException("value");
			}
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public Color SelectionColor
	{
		get
		{
			return Color.Black;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public Font SelectionFont
	{
		get
		{
			return base.Font;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(0)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public int SelectionHangingIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(0)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public int SelectionIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
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

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool SelectionProtected
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(0)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public int SelectionRightIndent
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public int[] SelectionTabs
	{
		get
		{
			return new int[0];
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public RichTextBoxSelectionTypes SelectionType
	{
		get
		{
			return RichTextBoxSelectionTypes.Empty;
		}
	}

	[TODO]
	protected override void SetBorderStyle(BorderStyle borderStyle)
	{
		BorderStyleInternal = borderStyle;
	}

	[TODO]
	internal override void SetSelectionLength(int length)
	{
	}

	[TODO]
	internal override void SetSelectionStart(int start)
	{
	}

	[TODO]
	protected override void SetTextInternal(string text)
	{
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public bool ShowSelectionMargin
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
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

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public override int TextLength
	{
		get
		{
			return base.TextLength;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public String UndoActionName
	{
		get
		{
			return String.Empty;
		}
	}

	[TODO]
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(1)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public float ZoomFactor
	{
		get
		{
			return zoomFactor;
		}
		set
		{
			zoomFactor = value;
		}
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}
}; // class RichTextBox
#endif
}; // namespace System.Windows.Forms
