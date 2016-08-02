/*
 * FontDialog.cs - Implementation of the
 *			"System.Windows.Forms.FontDialog" class.
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

using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

public class FontDialog : CommonDialog
{
	// Internal state.
	private FontDialogForm form;
	private bool allowScriptChange;
	private bool allowSimulations;
	private bool allowVectorFonts;
	private bool allowVerticalFonts;
	private bool fixedPitchOnly;
	private bool fontMustExist;
	private bool scriptsOnly;
	private bool showApply;
	private bool showColor;
	private bool showEffects;
	private bool showHelp;
	private Color color;
	private Font font;
	private int minSize;
	private int maxSize;

	// Constructor.
	public FontDialog()
			{
				// Make sure that the dialog fields have their default values.
				Reset();
			}

	// Get or set this object's properties.
	public bool AllowScriptChange
			{
				get
				{
					return allowScriptChange;
				}
				set
				{
					allowScriptChange = value;
				}
			}
	public bool AllowSimulations
			{
				get
				{
					return allowSimulations;
				}
				set
				{
					allowSimulations = value;
				}
			}
	public bool AllowVectorFonts
			{
				get
				{
					return allowVectorFonts;
				}
				set
				{
					allowVectorFonts = value;
				}
			}
	public bool AllowVerticalFonts
			{
				get
				{
					return allowVerticalFonts;
				}
				set
				{
					allowVerticalFonts = value;
				}
			}
	public Color Color
			{
				get
				{
					return color;
				}
				set
				{
					color = value;
				}
			}
	public bool FixedPitchOnly
			{
				get
				{
					return fixedPitchOnly;
				}
				set
				{
					fixedPitchOnly = value;
				}
			}
	public Font Font
			{
				get
				{
					return font;
				}
				set
				{
					font = value;
				}
			}
	public bool FontMustExist
			{
				get
				{
					return fontMustExist;
				}
				set
				{
					fontMustExist = value;
				}
			}
	public int MinSize
			{
				get
				{
					return minSize;
				}
				set
				{
					if(value < 0)
					{
						value = 0;
					}
					minSize = value;
				}
			}
	public int MaxSize
			{
				get
				{
					return maxSize;
				}
				set
				{
					if(value < 0)
					{
						value = 0;
					}
					maxSize = value;
				}
			}
	public bool ScriptsOnly
			{
				get
				{
					return scriptsOnly;
				}
				set
				{
					scriptsOnly = value;
				}
			}
	public bool ShowApply
			{
				get
				{
					return showApply;
				}
				set
				{
					showApply = value;
				}
			}
	public bool ShowColor
			{
				get
				{
					return showColor;
				}
				set
				{
					showColor = value;
				}
			}
	public bool ShowEffects
			{
				get
				{
					return showEffects;
				}
				set
				{
					showEffects = value;
				}
			}
	public bool ShowHelp
			{
				get
				{
					return showHelp;
				}
				set
				{
					showHelp = value;
				}
			}

	// Reset the dialog box controls to their default values.
	public override void Reset()
			{
				allowScriptChange = true;
				allowSimulations = true;
				allowVectorFonts = true;
				allowVerticalFonts = true;
				fixedPitchOnly = false;
				fontMustExist = false;
				scriptsOnly = false;
				showApply = false;
				showColor = false;
				showEffects = true;
				showHelp = false;
				color = Color.Black;
				font = null;
				minSize = 0;
				maxSize = 0;
				if(form != null)
				{
					form.UpdateDialog();
				}
			}

	// Run the dialog box, with a particular parent owner.
	protected override bool RunDialog(IntPtr hWndOwner)
			{
				// This version is not used in this implementation.
				return false;
			}
	internal override DialogResult RunDialog(IWin32Window owner)
			{
				// If the dialog is already visible, then bail out.
				if(form != null)
				{
					return DialogResult.Cancel;
				}

				// Construct the font dialog form.
				form = new FontDialogForm(this);

				// Run the dialog and get its result.
				DialogResult result;
				try
				{
					result = form.ShowDialog(owner);
					if(result != DialogResult.OK)
					{
						font = null;
					}
				}
				finally
				{
					form.DisposeDialog();
					form = null;
				}

				// Return the final dialog result to the caller.
				return result;
			}

	// Convert this object into a string.
	public override string ToString()
			{
				return base.ToString() + ", Font: " +
					   ((font != null) ? font.ToString() : "(none)");
			}

	// Event that is emitted when the user clicks the "Apply" button.
	public event EventHandler Apply;

	// Emit the "Apply" event.
	protected virtual void OnApply(EventArgs e)
			{
				if(Apply != null)
				{
					Apply(this, e);
				}
			}
	internal void EmitApply(EventArgs e)
			{
				OnApply(e);
			}

	// Hook procedure - not used in this implementation.
	protected override IntPtr HookProc(IntPtr hWnd, int msg,
									   IntPtr wparam, IntPtr lparam)
			{
				return IntPtr.Zero;
			}

	// Control that is used to fill empty space in layouts.
	private class EmptyControl : Control
	{
		// Constructor.
		public EmptyControl()
				{
					TabStop = false;
					SetStyle(ControlStyles.Selectable, false);
				}

	}; // class EmptyControl

	// Sizes to display in the size list.
	private static readonly int[] sizes =
		{8, 9, 10, 11, 12, 14, 16, 18, 20, 22,
		 24, 28, 30, 32, 34, 36, 38, 40};

	// Form that represents the font dialog.
	private class FontDialogForm : Form
	{
		// Internal state.
		private FontDialog dialog;
		private HBoxLayout hbox;		// Main layout for dialog.
		private VBoxLayout vbox1;		// Left-hand side.
		private VBoxLayout vbox2;		// Push buttons on right-hand side.
		private VBoxLayout vbox3;		// Effects controls.
		private GridLayout grid;		// Font name and size grid.
		private HBoxLayout hbox2;		// Grid plus effects.
		private Button okButton;
		private Button cancelButton;
		private Button applyButton;
		private Button helpButton;
		private TextBox name;
		private TextBox size;
		private ListBox nameList;
		private ListBox sizeList;
		private Control sample;
		private GroupBox effects;
		private CheckBox bold;
		private CheckBox italic;
		private CheckBox underline;
		private CheckBox strikeout;
		private Hashtable fonts;

		// Information about a font that we have cached.
		private class FontInfo
		{
			// Accessible state.
			public String family;
			public float size;
			public FontStyle style;
			public Font font;
			public bool disposable;

			// Constructors.
			public FontInfo(Font font, bool disposable)
					{
						if(font == null)
						{
							font = Control.DefaultFont;
						}
						this.family = font.Name;
						this.size = font.Size;
						this.style = font.Style;
						this.font = font;
						this.disposable = disposable;
					}
			public FontInfo(String family, float size, FontStyle style)
					{
						this.family = family;
						this.size = size;
						this.style = style;
						this.font = null;
						this.disposable = false;
					}

			// Dispose of this object if it isn't the selected font.
			public void Dispose(Font notThis)
					{
						if(disposable && font != null && font != notThis)
						{
							font.Dispose();
							font = null;
						}
					}

			// Determine if two objects are equal.
			public override bool Equals(Object obj)
					{
						FontInfo info = (obj as FontInfo);
						if(info != null)
						{
							return (family == info.family &&
									size == info.size &&
									style == info.style);
						}
						else
						{
							return false;
						}
					}

			// Get a hash code for this object.
			public override int GetHashCode()
					{
						return family.GetHashCode() + (int)size + (int)style;
					}

		}; // class FontInfo

		// Constructor.
		public FontDialogForm(FontDialog dialog)
				{
					// Record the parent for later access.
					this.dialog = dialog;

					// Create the initial font cache.
					fonts = new Hashtable();
					FontInfo info = new FontInfo(dialog.Font, false);
					fonts.Add(info, info);

					// Set the title.
					Text = S._("SWF_FontDialog_Title", "Font");

					// Construct the layout boxes for the font dialog.
					hbox = new HBoxLayout();
					hbox.Dock = DockStyle.Fill;
					vbox1 = new VBoxLayout();
					vbox2 = new VBoxLayout();
					vbox3 = new VBoxLayout();
					hbox2 = new HBoxLayout();
					grid = new GridLayout(2, 3);
					grid.StretchColumn = 0;
					effects = new GroupBox();
					effects.Text = S._("SWF_FontDialog_Effects", "Effects");
					effects.Width = 80;
					hbox.Controls.Add(vbox1);
					hbox.Controls.Add(vbox2);
					hbox.StretchControl = vbox1;
					hbox2.Controls.Add(grid);
					hbox2.Controls.Add(effects);
					hbox2.StretchControl = grid;
					vbox1.Controls.Add(hbox2);
					vbox1.StretchControl = hbox2;
					effects.Controls.Add(vbox3);
					vbox3.Dock = DockStyle.Fill;

					// Create the main display area.
					Label label;
					label = new Label();
					label.Text = S._("SWF_FontDialog_Name", "Font:");
					name = new TextBox();
					name.ReadOnly = true;
					nameList = new ListBox();
					grid.SetControl(0, 0, label);
					grid.SetControl(0, 1, name);
					grid.SetControl(0, 2, nameList);
					label = new Label();
					label.Text = S._("SWF_FontDialog_Size", "Size:");
					size = new TextBox();
					size.Width = 40;
					size.ReadOnly = true;
					sizeList = new ListBox();
					sizeList.Width = 40;
					grid.SetControl(1, 0, label);
					grid.SetControl(1, 1, size);
					grid.SetControl(1, 2, sizeList);

					// Create the buttons.
					okButton = new Button();
					okButton.Text = S._("SWF_MessageBox_OK", "OK");
					cancelButton = new Button();
					cancelButton.Text = S._("SWF_MessageBox_Cancel", "Cancel");
					applyButton = new Button();
					applyButton.Text = S._("SWF_MessageBox_Apply", "Apply");
					helpButton = new Button();
					helpButton.Text = S._("SWF_MessageBox_Help", "Help");
					vbox2.Controls.Add(okButton);
					vbox2.Controls.Add(cancelButton);
					vbox2.Controls.Add(applyButton);
					vbox2.Controls.Add(helpButton);
					vbox2.Controls.Add(new EmptyControl());
					AcceptButton = okButton;
					CancelButton = cancelButton;

					// Create the effects controls.
					bold = new CheckBox();
					bold.Text = S._("SWF_FontDialog_Bold", "Bold");
					italic = new CheckBox();
					italic.Text = S._("SWF_FontDialog_Italic", "Italic");
					underline = new CheckBox();
					underline.Text =
						S._("SWF_FontDialog_Underline", "Underline");
					strikeout = new CheckBox();
					strikeout.Text =
						S._("SWF_FontDialog_Strikeout", "Strikeout");
					Control spacing = new Control();
					vbox3.Spacing = 0;
					vbox3.Controls.Add(bold);
					vbox3.Controls.Add(italic);
					vbox3.Controls.Add(underline);
					vbox3.Controls.Add(strikeout);
					vbox3.Controls.Add(spacing);

					// Create the sample box.
					sample = new Control();
					sample.ForeColor = SystemColors.WindowText;
					sample.BackColor = SystemColors.Window;
					sample.BorderStyleInternal = BorderStyle.Fixed3D;
					sample.Height = 60;
					vbox1.Controls.Add(sample);

					// Add the top-level hbox to the dialog and set the size.
					Controls.Add(hbox);
					Size drawsize = hbox.RecommendedSize;
					if(drawsize.Width < 450)
					{
						drawsize.Width = 450;
					}
					if(drawsize.Height < 280)
					{
						drawsize.Height = 280;
					}
					ClientSize = drawsize;
					MinimumSize = drawsize;
					MinimizeBox = false;
					ShowInTaskbar = false;

					// Fill in the font names and sizes.
					nameList.BeginUpdate();
					foreach(FontFamily family in FontFamily.Families)
					{
						nameList.Items.Add(family.Name);
					}
					nameList.EndUpdate();
					sizeList.BeginUpdate();
					foreach(int value in sizes)
					{
						sizeList.Items.Add(value);
					}
					sizeList.EndUpdate();

					// Hook up interesting events.
					okButton.Click += new EventHandler(AcceptDialog);
					cancelButton.Click += new EventHandler(CancelDialog);
					applyButton.Click += new EventHandler(ApplyButtonClicked);
					helpButton.Click += new EventHandler(HelpButtonClicked);
					nameList.SelectedIndexChanged
						+= new EventHandler(NameIndexChanged);
					sizeList.SelectedIndexChanged
						+= new EventHandler(SizeIndexChanged);
					bold.CheckedChanged
						+= new EventHandler(FontStyleChanged);
					italic.CheckedChanged
						+= new EventHandler(FontStyleChanged);
					underline.CheckedChanged
						+= new EventHandler(FontStyleChanged);
					strikeout.CheckedChanged
						+= new EventHandler(FontStyleChanged);
					sample.Paint += new PaintEventHandler(PaintSample);

					// Match the requested settings from the dialog parent.
					UpdateDialog();
				}

		// Dispose of this dialog.
		public void DisposeDialog()
				{
					// Dispose the widget details.
					Dispose(true);

					// Dispose the font cache.
					IDictionaryEnumerator e = fonts.GetEnumerator();
					while(e.MoveNext())
					{
						((FontInfo)(e.Value)).Dispose(dialog.Font);
					}
				}

		// Update the dialog to match the "FontDialog" properties.
		public void UpdateDialog()
				{
					int index;
					applyButton.Visible = dialog.ShowApply;
					helpButton.Visible = dialog.ShowHelp;
					HelpButton = dialog.ShowHelp;
					Font font = dialog.Font;
					if(font == null)
					{
						// Use the default font to start with.
						font = this.Font;
					}
					index = nameList.Items.IndexOf(font.Name);
					if(index >= 0)
					{
						nameList.SelectedIndex = index;
					}
					else
					{
						name.Text = font.Name;
					}
					bold.Checked = ((font.Style & FontStyle.Bold) != 0);
					italic.Checked = ((font.Style & FontStyle.Italic) != 0);
					underline.Checked =
						((font.Style & FontStyle.Underline) != 0);
					strikeout.Checked =
						((font.Style & FontStyle.Strikeout) != 0);
					index = sizeList.Items.IndexOf((int)(font.Size));
					if(index >= 0)
					{
						sizeList.SelectedIndex = index;
					}
					else
					{
						size.Text = ((int)(font.Size)).ToString();
					}
					vbox3.SuspendLayout();
					underline.Visible = dialog.ShowEffects;
					strikeout.Visible = dialog.ShowEffects;
					vbox3.ResumeLayout();
					UpdateSample();
				}

		// Set the font in the return dialog object.
		private void SetFont()
				{
					FontStyle style = FontStyle.Regular;
					if(bold.Checked)
					{
						style |= FontStyle.Bold;
					}
					if(italic.Checked)
					{
						style |= FontStyle.Italic;
					}
					if(underline.Checked)
					{
						style |= FontStyle.Underline;
					}
					if(strikeout.Checked)
					{
						style |= FontStyle.Strikeout;
					}
					String family = name.Text.Trim();
					float fontSize;
					try
					{
					#if CONFIG_EXTENDED_NUMERICS
						fontSize = Single.Parse(size.Text);
					#else
						fontSize = Int32.Parse(size.Text);
					#endif
					}
					catch
					{
						fontSize = 0.0f;
					}

					// Sanity-check the values a little.
					if(family == null || family == String.Empty)
					{
						return;
					}
					if(fontSize <= 0.0f)
					{
						return;
					}

					// Look for a font that matches the requirements.
					FontInfo info = new FontInfo(family, fontSize, style);
					info = (FontInfo)(fonts[info]);
					if(info == null)
					{
						Font font = new Font(family, fontSize, style);
						info = new FontInfo(font, true);
						fonts[info] = info;
					}
					dialog.Font = info.font;
				}

		// Update the sample region.
		private void UpdateSample()
				{
					sample.Invalidate();
				}

		// The text to paint in the sample area.
		private const String sampleText = "ABCDEF abcdef 012345";

		// Paint the sample region.
		private void PaintSample(Object sender, PaintEventArgs e)
				{
					Graphics g = e.Graphics;
					Font font = dialog.Font;
					if(font == null)
					{
						font = Control.DefaultFont;
					}
					SizeF size = g.MeasureString(sampleText, font);
					Size clientSize = sample.ClientSize;
					g.DrawString(sampleText, font, SystemBrushes.WindowText,
								 (clientSize.Width - size.Width) / 2,
								 (clientSize.Height - size.Height) / 2);
				}

		// Process a help request on the form.
		protected override void OnHelpRequested(HelpEventArgs e)
				{
					base.OnHelpRequested(e);
					dialog.EmitHelpRequest(e);
				}

		// Handle the "accept" button on this dialog.
		public void AcceptDialog(Object sender, EventArgs e)
				{
					SetFont();
					DialogResult = DialogResult.OK;
				}

		// Handle the "cancel" button on this dialog.
		private void CancelDialog(Object sender, EventArgs e)
				{
					DialogResult = DialogResult.Cancel;
				}

		// Handle the "apply" button on this dialog.
		private void ApplyButtonClicked(Object sender, EventArgs e)
				{
					// Copy the font details into the dialog object.
					SetFont();

					// Make sure that the font cannot be disposed just
					// in case the surrounding application keeps a copy.
					FontInfo info = new FontInfo(dialog.Font, false);
					info = (FontInfo)(fonts[info]);
					if(info != null)
					{
						info.disposable = false;
					}

					// Emit the "Apply" signal.
					dialog.EmitApply(e);
				}

		// Handle the "help" button on this dialog.
		private void HelpButtonClicked(Object sender, EventArgs e)
				{
					dialog.EmitHelpRequest(e);
				}

		// Handle a change in style.
		private void FontStyleChanged(Object sender, EventArgs e)
				{
					SetFont();
					UpdateSample();
				}

		// Handle a "closing" event on this form.
		protected override void OnClosing(CancelEventArgs e)
				{
					base.OnClosing(e);
					e.Cancel = true;
					DialogResult = DialogResult.Cancel;
				}

		// Handle a change of index in the font name list.
		private void NameIndexChanged(Object sender, EventArgs e)
				{
					int index = nameList.SelectedIndex;
					if(index >= 0)
					{
						name.Text = ((IList)nameList.Items)[index].ToString();
					}
					else
					{
						name.Text = String.Empty;
					}
					SetFont();
					UpdateSample();
				}

		// Handle a change of index in the size list.
		private void SizeIndexChanged(Object sender, EventArgs e)
				{
					int index = sizeList.SelectedIndex;
					if(index >= 0)
					{
						size.Text = ((IList)sizeList.Items)[index].ToString();
					}
					SetFont();
					UpdateSample();
				}

	}; // class FontDialogForm

}; // class FontDialog

}; // namespace System.Windows.Forms
