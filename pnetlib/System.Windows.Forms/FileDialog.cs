/*
 * FileDialog.cs - Implementation of the
 *			"System.Windows.Forms.FileDialog" class.
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
using System.ComponentModel;

public abstract class FileDialog : CommonDialog
{
	// Internal state.
	private bool addExtension;
	internal bool checkFileExists;
	private bool checkPathExists;
	private bool dereferenceLinks;
	private String defaultExt;
	private String fileName;
	private String[] fileNames;
	private String filter;
	private int filterIndex;
	private bool filterIndexSet;
	private String[] filterNames;
	private String[] filterPatterns;
	private String initialDirectory;
	private bool restoreDirectory;
	private bool showHelp;
	private bool validateNames;
	private String title;
	internal FileDialogForm form;

	// Event identifier for the "FileOk" event.
	protected static readonly object EventFileOk = new Object();

	// Constructor.
	internal FileDialog()
			{
				// Make sure that the dialog fields have their default values.
				Reset();
			}

	// Get or set this object's properties.
	public bool AddExtension
			{
				get
				{
					return addExtension;
				}
				set
				{
					addExtension = value;
				}
			}
	public virtual bool CheckFileExists
			{
				get
				{
					return checkFileExists;
				}
				set
				{
					checkFileExists = value;
				}
			}
	public bool CheckPathExists
			{
				get
				{
					return checkPathExists;
				}
				set
				{
					checkPathExists = value;
				}
			}
	public String DefaultExt
			{
				get
				{
					return defaultExt;
				}
				set
				{
					if(value == null)
					{
						value = String.Empty;
					}
					defaultExt = value;
					if(value.Length > 0 && value[0] == '.' &&
					   filterNames.Length > 1 && !filterIndexSet)
					{
						SetDefaultFilterIndex();
						if(form != null)
						{
							form.RefreshAll();
						}
					}
				}
			}
	public bool DereferenceLinks
			{
				get
				{
					return dereferenceLinks;
				}
				set
				{
					if(dereferenceLinks != value)
					{
						dereferenceLinks = value;
						if(form != null)
						{
							form.Reload();
						}
					}
				}
			}
	public String FileName
			{
				get
				{
					return fileName;
				}
				set
				{
					if(value == null || value.Length == 0)
					{
						value = String.Empty;
					}
					else
					{
						value = Path.GetFullPath(value);
					}
					if(fileName != value)
					{
						fileName = value;
						fileNames = null;
						if(form != null)
						{
							form.ChangeFilename(value);
						}
					}
				}
			}
	public String[] FileNames
			{
				get
				{
					if(fileNames == null)
					{
						if(fileName != null && fileName.Length > 0)
						{
							fileNames = new String [] {fileName};
							return fileNames;
						}
						else
						{
							return new String [0];
						}
					}
					else
					{
						return fileNames;
					}
				}
			}
	public String Filter
			{
				get
				{
					return filter;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					SplitFilter(value);
					filter = value;
					if(form != null)
					{
						form.RefreshAll();
					}
				}
			}
	public int FilterIndex
			{
				get
				{
					return filterIndex;
				}
				set
				{
					if(value < 1 || value > filterNames.Length)
					{
						throw new ArgumentOutOfRangeException
							(S._("SWF_FileDialog_FilterIndex"));
					}
					filterIndex = value;
					filterIndexSet = true;
					if(form != null)
					{
						form.RefreshAll();
					}
				}
			}
	public String InitialDirectory
			{
				get
				{
					return initialDirectory;
				}
				set
				{
					initialDirectory = value;
				}
			}
	protected virtual IntPtr Instance
			{
				get
				{
					// Not used in this implementation.
					return IntPtr.Zero;
				}
			}
	protected int Options
			{
				get
				{
					// Not used in this implementation.
					return 0;
				}
			}
	public bool RestoreDirectory
			{
				get
				{
					return restoreDirectory;
				}
				set
				{
					restoreDirectory = value;
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
					if(showHelp != value)
					{
						showHelp = value;
						if(form != null)
						{
							form.HelpButton = value;
						}
					}
				}
			}
	public String Title
			{
				get
				{
					if(title == null || title == String.Empty)
					{
						return DefaultTitle;
					}
					return title;
				}
				set
				{
					if(title != value)
					{
						title = value;
						if(form != null)
						{
							if(value == null || value == String.Empty)
							{
								form.Text = DefaultTitle;
							}
							else
							{
								form.Text = value;
							}
						}
					}
				}
			}
	internal virtual String DefaultTitle
			{
				get
				{
					// Overridden by subclasses to supply "Open" or "Save As".
					return String.Empty;
				}
			}
	public bool ValidateNames
			{
				get
				{
					return validateNames;
				}
				set
				{
					validateNames = value;
				}
			}
	internal virtual String OkButtonName
			{
				get
				{
					return S._("SWF_FileDialog_OpenButton", "Open");
				}
			}

	// Hook procedure - not used in this implementation.
	protected override IntPtr HookProc(IntPtr hWnd, int msg,
									   IntPtr wparam, IntPtr lparam)
			{
				return IntPtr.Zero;
			}

	// Raise the "FileOk" event.
	protected void OnFileOk(CancelEventArgs e)
			{
				if(FileOk != null)
				{
					FileOk(this, e);
				}
			}

	// Determine if the current file is ok when the user clicks Open or Save.
	private bool IsFileOk()
			{
				CancelEventArgs e = new CancelEventArgs();
				OnFileOk(e);
				return !(e.Cancel);
			}

	// Split a filter string into a list of names and extensions.
	private void SplitFilter(String filter)
			{
				// Split the filter string into its components and
				// then check that we have an even number of components.
				String[] split = filter.Split('|');
				if(split.Length == 0 || (split.Length % 2) != 0)
				{
					throw new ArgumentException
						(S._("SWF_FileDialog_Filter"));
				}

				// Create the filter name and pattern arrays.
				int len = split.Length / 2;
				int posn;
				String[] names = new String [len];
				String[] patterns = new String [len];
				for(posn = 0; posn < len; ++posn)
				{
					names[posn] = split[posn * 2];
					patterns[posn] = split[posn * 2 + 1].ToLower();
					if(patterns[posn] == String.Empty)
					{
						throw new ArgumentException
							(S._("SWF_FileDialog_Filter"));
					}
				}
				filterNames = names;
				filterPatterns = patterns;
				if(filterIndex > len)
				{
					filterIndex = 1;
				}

				// Set the default filter index if "DefaultExt" is non-empty.
				if(defaultExt.Length > 0 && defaultExt[0] == '.' &&
				   filterNames.Length > 1 && !filterIndexSet)
				{
					SetDefaultFilterIndex();
				}
			}

	// Set the filter index based on the default extension.
	private void SetDefaultFilterIndex()
			{
				int index = GetWildcardFilterIndex("*" + defaultExt);
				if(index != 0)
				{
					filterIndex = index;
				}
			}

	// Get the filter index corresponding to a particular wildcard pattern.
	// Returns zero if there is no matching filter.
	private int GetWildcardFilterIndex(String pattern)
			{
				// Convert Unix-style wildcards into DOS-style wildcards.
				if(pattern == "*")
				{
					pattern = "*.*";
				}
				pattern = pattern.ToLower();

				// Find the pattern that matches.
				int index, posn;
				String filter;
				for(index = 0; index < filterPatterns.Length; ++index)
				{
					filter = filterPatterns[index];
					posn = filter.IndexOf(pattern);
					if(posn != -1)
					{
						if(posn == 0 || filter[posn - 1] == ';')
						{
							if((posn + pattern.Length) == filter.Length)
							{
								return index + 1;
							}
							else if((posn + pattern.Length) < filter.Length &&
									filter[posn + pattern.Length] == ';')
							{
								return index + 1;
							}
						}
					}
				}
				return 0;
			}

	// Get the current filter pattern.
	private String GetFilterPattern()
			{
				if(filterIndex >= 1 && filterIndex <= filterPatterns.Length)
				{
					return filterPatterns[filterIndex - 1];
				}
				else
				{
					return "*.*";
				}
			}

	// Internal version of "Reset".
	internal virtual void ResetInternal()
			{
				addExtension = true;
				checkFileExists = false;
				checkPathExists = true;
				dereferenceLinks = true;
				defaultExt = String.Empty;
				fileName = String.Empty;
				fileNames = null;
				String all = S._("SWF_FileDialog_AllFiles", "All files (*.*)");
				filter = all + "|*.*";
				filterNames = new String [] {all};
				filterPatterns = new String [] {"*.*"};
				filterIndex = 1;
				filterIndexSet = false;
				initialDirectory = String.Empty;
				restoreDirectory = false;
				showHelp = false;
				validateNames = true;
				title = String.Empty;
			}

	// Reset the dialog box controls to their default values.
	public override void Reset()
			{
				ResetInternal();
				if(form != null)
				{
					form.RefreshAll();
				}
			}

	// Run the dialog box.
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

				// Construct the file dialog form.
				form = new FileDialogForm(this);

				// Run the dialog and get its result.
				DialogResult result;
				try
				{
					result = form.ShowDialog(owner);
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
				return base.ToString() + ": Title: " + Title +
					   ", FileName: " + FileName;
			}

	// Event that is raised to check that a file is OK.
	public event CancelEventHandler FileOk;

	// Icon codes.
	private const short IconCode_Directory = 0;
	private const short IconCode_Drive     = 1;
	private const short IconCode_File      = 2;
	private const short IconCode_Link      = 3;
	private const short IconCode_App       = 4;
	private const short IconCode_Dll       = 5;
	private const short IconCode_Text      = 6;
	private const short IconCode_Num       = 7;

	// Icon sizes and item spacing.
	private const int IconWidth				= 16;
	private const int IconHeight			= 16;
	private const int IconSpacing			= 4;
	private const int ColumnSpacing			= 32;
	private const int TextSelectOverlap		= 2;

	// Special characters that may appear as directory separators.
	private static readonly char[] directorySeparators =
			{'/', '\\', ':',
			 Path.DirectorySeparatorChar,
			 Path.AltDirectorySeparatorChar,
			 Path.VolumeSeparatorChar};

	// Special characters that indicate wildcards.
	private static readonly char[] wildcards = {'*', '?'};

	// Information that is stored for a filesystem entry.
	private sealed class FilesystemEntry : IComparable
	{
		public String name;				// Name to display in dialog box.
		public String fullName;			// Full pathname of the entry.
		public bool isDirectory;		// True if a directory.
		public bool isSymlink;			// True if a symbolic link.
		public short iconCode;			// Icon to display with the item.

		// Compare two entries.  Directories always sort before files.
		public int CompareTo(Object obj)
				{
					FilesystemEntry other = (obj as FilesystemEntry);
					if(other != null)
					{
						if(isDirectory)
						{
							if(!(other.isDirectory))
							{
								return -1;
							}
						}
						else if(other.isDirectory)
						{
							return 1;
						}
						return String.Compare(name, other.name, true);
					}
					else
					{
						return 1;
					}
				}

		// Resolve this entry as a symbolic link.
		public void ResolveSymlinks()
				{
				#if __CSCC__
					int count = 20;
					String path = fullName;
					String link;
					do
					{
						try
						{
							link = SymbolicLinks.ReadLink(path);
							if(link == null)
							{
								// We've found the link destination.
								fullName = path;
								if(Directory.Exists(path))
								{
									isDirectory = true;
									iconCode = IconCode_Directory;
								}
								else
								{
									iconCode = IconCode_File;
								}
								return;
							}
							else
							{
								path = Path.GetDirectoryName(path);
								path = Path.Combine(path, link);
							}
						}
						catch(Exception)
						{
							// The path doesn't exist, so the link is
							// pointing at nothing.  Bail out without
							// resolving it.
							return;
						}
					}
					while(--count > 0);
				#endif
				}

		// Set the icon code based on the file's extension.
		public void SetIconCode()
				{
					if(iconCode != IconCode_File)
					{
						return;
					}
					switch(Path.GetExtension(name).ToLower())
					{
						case ".txt": case ".text": case ".c": case ".cs":
						case ".h": case ".cc": case ".cpp": case ".hpp":
						case ".doc": case ".rtf": case ".xml": case ".texi":
						case ".tex": case ".html":
						{
							iconCode = IconCode_Text;
						}
						break;

						case ".exe":
						{
							iconCode = IconCode_App;
						}
						break;

						case ".dll":
						{
							iconCode = IconCode_Dll;
						}
						break;

						case ".out":
						{
							if(String.Compare(name, "a.out", true) == 0)
							{
								iconCode = IconCode_App;
							}
						}
						break;

						case "":
						{
							if(String.Compare(name, "README", true) == 0)
							{
								iconCode = IconCode_Text;
							}
						}
						break;
					}
				}

		// Determine if this entry's name starts with a particular character.
		// The "ch" value is assumed to be lower case.
		public bool StartsWith(char ch)
				{
					if(name.Length > 0 && Char.ToLower(name[0]) == ch)
					{
						return true;
					}
					else
					{
						return false;
					}
				}

	}; // class FilesystemEntry

	// Determine if we appear to be running on Windows.
	private static bool IsWindows()
			{
			#if !ECMA_COMPAT
				return (Environment.OSVersion.Platform != PlatformID.Unix);
			#else
				return (Path.DirectorySeparatorChar == '\\');
			#endif
			}

	// Determine if a pathname ends in ".lnk".
	private static bool EndsInLnk(String pathname)
			{
				int len = pathname.Length;
				if(len < 4)
				{
					return false;
				}
				if(pathname[len - 4] == '.' &&
				   (pathname[len - 3] == 'l' || pathname[len - 3] == 'L') &&
				   (pathname[len - 2] == 'n' || pathname[len - 2] == 'N') &&
				   (pathname[len - 1] == 'k' || pathname[len - 1] == 'K'))
				{
					return true;
				}
				return false;
			}

	// Scan a directory and collect up all of the filesystem entries.
	private static FilesystemEntry[] ScanDirectory
				(String directory, String pattern, bool derefLinks)
			{
				String[] dirs;
				String[] files;
				String[] links;
				FilesystemEntry[] entries;

				// Convert the directory name into a full pathname.
				directory = Path.GetFullPath(directory);

				// Get all sub-directories in the specified directory,
				// irrespective of whether they match the pattern or not.
				try
				{
					dirs = Directory.GetDirectories(directory);
				}
				catch(Exception)
				{
					// An error occurred while trying to scan the directory,
					// so return an empty list of entries.
					return new FilesystemEntry [0];
				}

				// Get all files and Windows shortcut link files that match
				// the pattern in the directory.
				if(pattern == null || pattern == "*" || pattern == "*.*")
				{
					files = Directory.GetFiles(directory);
					links = null;
				}
				else if(!EndsInLnk(pattern) && IsWindows())
				{
					files = Directory.GetFiles(directory, pattern);
					links = Directory.GetFiles(directory, pattern + ".lnk");
				}
				else
				{
					files = Directory.GetFiles(directory, pattern);
					links = null;
				}

				// Combine the three lists and populate the information.
				entries = new FilesystemEntry
					[dirs.Length + files.Length +
					 (links != null ? links.Length : 0)];
				int posn = 0;
				FilesystemEntry entry;
				bool resolveSymlinks = false;
				foreach(String dir in dirs)
				{
					entry = new FilesystemEntry();
					entry.name = Path.GetFileName(dir);
					entry.fullName = dir;
					entry.isDirectory = true;
					entry.isSymlink = false;
					entry.iconCode = IconCode_Directory;
					entries[posn++] = entry;
				}
				foreach(String file in files)
				{
					entry = new FilesystemEntry();
					entry.name = Path.GetFileName(file);
					entry.fullName = file;
					entry.isDirectory = false;
					entry.iconCode = IconCode_File;
				#if __CSCC__
					entry.isSymlink = SymbolicLinks.IsSymbolicLink(file);
					if(entry.isSymlink)
					{
						resolveSymlinks = true;
						entry.iconCode = IconCode_Link;
						if(EndsInLnk(file) && IsWindows())
						{
							// Strip ".lnk" from the end of the filename.
							entry.name = Path.GetFileNameWithoutExtension(file);
						}
					}
				#else
					entry.isSymlink = false;
				#endif
					entries[posn++] = entry;
					entry.SetIconCode();
				}
				if(links != null)
				{
					// We have an extra list of files that end in ".lnk".
					foreach(String link in links)
					{
						entry = new FilesystemEntry();
						entry.name = Path.GetFileNameWithoutExtension(link);
						entry.fullName = link;
						entry.isDirectory = false;
						entry.isSymlink = true;
						entry.iconCode = IconCode_Link;
						entries[posn++] = entry;
						resolveSymlinks = true;
					}
				}

				// Resolve symbolic links to the underlying file or directory.
				if(resolveSymlinks && derefLinks)
				{
					for(posn = 0; posn < entries.Length; ++posn)
					{
						entry = entries[posn];
						if(entry.isSymlink)
						{
							entry.ResolveSymlinks();
							entry.SetIconCode();
						}
					}
				}

				// Sort the entry list and return it.
				Array.Sort(entries);
				return entries;
			}

	// Determine if a path is a directory, and handle the symlink case.
	private static bool IsDirectory(String path)
			{
				if(Directory.Exists(path))
				{
					return true;
				}
				else if(!File.Exists(path))
				{
					return false;
				}
			#if __CSCC__
				if(!SymbolicLinks.IsSymbolicLink(path))
				{
					return false;
				}
				int count = 20;
				String link;
				do
				{
					try
					{
						link = SymbolicLinks.ReadLink(path);
					}
					catch(Exception)
					{
						link = null;
					}
					if(link == null)
					{
						return Directory.Exists(path);
					}
					else
					{
						path = Path.GetDirectoryName(path);
						path = Path.Combine(path, link);
					}
				}
				while(--count > 0);
			#endif
				return false;
			}

	// List box like control that manages a group of file icons.
	private sealed class FileIconListBox : Control
	{
		// Internal state.
		private FileDialogForm dialog;
		private HScrollBar scrollBar;
		private FilesystemEntry[] entries;
		private int numEntries;
		private int columns;
		private int rows;
		private int leftMostColumn;
		private int columnWidth;
		private int textHeight;
		private int rowHeight;
		private int selected;
		private bool suppressUpdates;
		private Icon[] icons;

		// Constructor.
		public FileIconListBox(FileDialogForm dialog)
				{
					// Set the appropriate style properties for this control.
					this.dialog = dialog;
					ForeColor = SystemColors.WindowText;
					BackColor = SystemColors.Window;
					BorderStyleInternal = BorderStyle.Fixed3D;
					ResizeRedraw = true;
					TabStop = true;

					// Create the horizontal scroll bar and position it.
					scrollBar = new HScrollBar();
					scrollBar.ForeColor = SystemColors.ControlText;
					scrollBar.BackColor = SystemColors.Control;
					scrollBar.Visible = false;
					scrollBar.Dock = DockStyle.Bottom;
					scrollBar.TabStop = false;
					scrollBar.ValueChanged += new EventHandler(ViewScrolled);
					Controls.Add(scrollBar);

					// Load the icons.  We need icons for drives and links.
					icons = new Icon [IconCode_Num];
					icons[0] = new Icon(typeof(FileDialog), "small_folder.ico");
					icons[1] = new Icon(typeof(FileDialog), "small_folder.ico");
					icons[2] = new Icon
						(typeof(FileDialog), "small_document.ico");
					icons[3] = new Icon
						(typeof(FileDialog), "small_document.ico");
					icons[4] = new Icon
						(typeof(FileDialog), "small_application.ico");
					icons[5] = new Icon
						(typeof(FileDialog), "small_dll.ico");
					icons[6] = new Icon
						(typeof(FileDialog), "small_text.ico");
				}

		// Destroy the handle associated with the control.
		protected override void DestroyHandle()
				{
					base.DestroyHandle();
					if(icons != null)
					{
						foreach(Icon icon in icons)
						{
							icon.Dispose();
						}
						icons = null;
					}
				}

		// Get or set the entries in this icon box.
		public FilesystemEntry[] Entries
				{
					get
					{
						return entries;
					}
					set
					{
						entries = value;
						if(entries != null)
						{
							numEntries = entries.Length;
						}
						else
						{
							numEntries = 0;
						}
						LayoutControl();
						Invalidate();
					}
				}

		// Lay out the control after a change in contents.
		private void LayoutControl()
				{
					// Bail out early if the form is not visible
					if(!Visible)
					{
						return;
					}

					// Bail out early if there are no entries to display.
					if(numEntries == 0)
					{
						rows = 1;
						columns = 1;
						columnWidth = 1;
						textHeight = 1;
						rowHeight = 1;
						leftMostColumn = 0;
						selected = -1;
						scrollBar.Visible = false;
						return;
					}

					// Measure all of the entries and get the maximum
					// column width and row height values.
					SizeF size;
					Font font = Font;
					int width;
					using(Graphics graphics = CreateGraphics())
					{
						textHeight = (int)(font.GetHeight(graphics));
						rowHeight = textHeight;
						if(rowHeight < (IconHeight + 1))
						{
							rowHeight = IconHeight + 1;
						}
						columnWidth = IconWidth + IconSpacing + ColumnSpacing;
						foreach(FilesystemEntry entry in entries)
						{
							size = graphics.MeasureString(entry.name, font);
							width = (IconWidth + IconSpacing + ColumnSpacing) +
									(int)(size.Width);
							if(width > columnWidth)
							{
								columnWidth = width;
							}
						}
					}

					// Determine if the entire list will fit in the
					// client area or if we need to add a scroll bar.
					Rectangle client = ClientRectangle;
					if(columnWidth > client.Width)
					{
						columnWidth = client.Width;
					}
					if(columnWidth < 1)
					{
						columnWidth = 1;
					}
					rows = client.Height / rowHeight;
					if(rows < 1)
					{
						rows = 1;
					}
					columns = (numEntries + rows - 1) / rows;
					int visibleColumns =
						(client.Width + columnWidth - 1) / columnWidth;
					leftMostColumn = 0;
					selected = -1;
					if(columns <= visibleColumns)
					{
						// We won't need a scroll bar to display the contents.
						scrollBar.Visible = false;
					}
					else
					{
						// We need a scroll bar to display the contents.
						rows = (client.Height - scrollBar.Height) / rowHeight;
						if(rows < 1)
						{
							rows = 1;
						}
						columns = (numEntries + rows - 1) / rows;
						scrollBar.Value = 0;
						scrollBar.Maximum = columns - 1;
						scrollBar.SmallChange = 1;
						if((columns - 1) <= visibleColumns)
						{
							scrollBar.LargeChange = 1;
						}
						else
						{
							scrollBar.LargeChange = visibleColumns;
						}
						scrollBar.Visible = true;
					}
				}

		// Handle a layout request from the parent class.
		protected override void OnLayout(LayoutEventArgs e)
				{
					base.OnLayout(e);
					LayoutControl();
				}

		// Draw a particular entry at a specified location.
		private void DrawEntry(Graphics graphics, int entry,
							   int x, int y, Font font,
							   bool clearBackground)
				{
					// Bail out if the entry index is out of range.
					if(entry < 0 || entry >= numEntries)
					{
						return;
					}

					// Compute the bounding box of the entry's text area.
					String name = entries[entry].name;
					SizeF size = graphics.MeasureString(name, font);
					int width = ((int)(size.Width)) + TextSelectOverlap * 2;
					int height = textHeight + 2;
					if(height > rowHeight)
					{
						height = rowHeight;
					}
					Rectangle textBounds = new Rectangle
						(x + (IconWidth + IconSpacing - TextSelectOverlap),
						 y + (rowHeight - height) / 2, width, height);

					// Draw the icon to the left of the text.
					graphics.DrawIcon
						(icons[entries[entry].iconCode],
						 x, y + (rowHeight - IconHeight) / 2);

					// Fill the background behind the text and get
					// the foreground color to draw the text with.
					Brush foreground;
					if(selected == entry)
					{
						if(Focused)
						{
							graphics.FillRectangle
								(SystemBrushes.Highlight, textBounds);
							foreground = SystemBrushes.HighlightText;
						}
						else
						{
							graphics.FillRectangle
								(SystemBrushes.Control, textBounds);
							foreground = SystemBrushes.ControlText;
						}
					}
					else if(clearBackground)
					{
						graphics.FillRectangle
							(SystemBrushes.Window, textBounds);
						foreground = SystemBrushes.WindowText;
					}
					else
					{
						foreground = SystemBrushes.WindowText;
					}

					// Draw the text within the specified text bounds.
					graphics.DrawString
							(name, font, foreground,
							 (float)(textBounds.X + TextSelectOverlap),
							 (float)(y + (rowHeight - textHeight) / 2));
				}

		// Draw a particular entry at its proper location.
		private void DrawEntry(int entry)
				{
					// Bail out if the entry index is out of range.
					if(entry < 0 || entry >= numEntries)
					{
						return;
					}

					// Compute the location of the entry.
					int row, column;
					int x, y;
					column = (entry / rows);
					row = entry - column * rows;
					x = (column - leftMostColumn) * columnWidth;
					y = row * rowHeight;

					// Draw the entry.
					using(Graphics graphics = CreateGraphics())
					{
						DrawEntry(graphics, entry, x, y, Font, true);
					}
				}

		// Handle a value changed event from the horizontal scroll bar.
		private void ViewScrolled(Object sender, EventArgs e)
				{
					int value = scrollBar.Value;
					if(!suppressUpdates && leftMostColumn != value)
					{
						SetLeftMostColumn(value);
					}
				}

		// Set the left-most displayed column.
		private void SetLeftMostColumn(int column)
				{
					leftMostColumn = column;
					Invalidate();
				}

		// Change the selection.
		private void ChangeSelection(int entry)
				{
					if(entry < 0 || entry >= numEntries)
					{
						entry = -1;
					}
					if(selected != entry)
					{
						// Redraw the selected entry.
						int oldSelected = selected;
						selected = entry;
						DrawEntry(oldSelected);
						DrawEntry(entry);

						// Scroll the entry into view if necessary.
						if(entry != -1)
						{
							int column = (entry / rows);
							int visibleColumns =
								(ClientSize.Width / columnWidth);
							if(visibleColumns < 1)
							{
								visibleColumns = 1;
							}
							if(column < leftMostColumn)
							{
								SetLeftMostColumn(column);
							}
							else if(column >= (leftMostColumn + visibleColumns))
							{
								SetLeftMostColumn(column - visibleColumns + 1);
							}
							suppressUpdates = true;
							scrollBar.Value = column;
							suppressUpdates = false;
						}

						// Update the text box to contain the filename.
						if(entry != -1)
						{
							if(!(entries[entry].isDirectory))
							{
								dialog.UpdateName(entries[entry].name);
							}
						}
					}
				}

		// Handle a paint request from the parent class.
		protected override void OnPaint(PaintEventArgs e)
				{
					Graphics graphics = e.Graphics;
					int row, column;
					int visibleColumns =
						(columnWidth != 0 ?
							((ClientSize.Width + columnWidth - 1)
								/ columnWidth) : 0);
					Font font = Font;
					for(row = 0; row < rows; ++row)
					{
						for(column = leftMostColumn;
							column < columns &&
							column < (leftMostColumn + visibleColumns);
							++column)
						{
							DrawEntry(graphics, column * rows + row,
									  (column - leftMostColumn) * columnWidth,
									  row * rowHeight, font, false);
						}
					}
				}

		// Handle a "focus enter" event.
		protected override void OnEnter(EventArgs e)
				{
					base.OnEnter(e);
					if(selected == -1)
					{
						// Set the focus to the first entry by default.
						selected = 0;
					}
					DrawEntry(selected);
				}

		// Handle a "focus leave" event.
		protected override void OnLeave(EventArgs e)
				{
					base.OnLeave(e);
					if(selected != -1)
					{
						DrawEntry(selected);
					}
				}

		// Handle a mouse down event.
		protected override void OnMouseDown(MouseEventArgs e)
				{
					base.OnMouseDown(e);
					if(e.Button == MouseButtons.Left)
					{
						int row = e.Y / rowHeight;
						int column = (e.X / columnWidth) + leftMostColumn;
						int entry = -1;
						if(row >= 0 && row < rows &&
						   column >= 0 && column < columns)
						{
							entry = column * rows + row;
							if(entry >= numEntries)
							{
								entry = -1;
							}
						}
						ChangeSelection(entry);
					}
				}

		// Handle a double click event.
		protected override void OnDoubleClick(EventArgs e)
				{
					base.OnDoubleClick(e);
					if((MouseButtons & MouseButtons.Left) != 0)
					{
						ActivateSelected();
					}
				}

		// Determine if a character is recognized by a control as an input char.
		protected override bool IsInputChar(char c)
				{
					if(Char.IsLetterOrDigit(c))
					{
						return false;
					}
					else
					{
						return base.IsInputChar(c);
					}
				}

		// Process a dialog character.  We use letters and digits to
		// select the next file starting with that character.
		protected override bool ProcessDialogChar(char charCode)
				{
					if(Char.IsLetterOrDigit(charCode))
					{
						int current = selected;
						int count = numEntries;
						charCode = Char.ToLower(charCode);
						while(count > 0)
						{
							++current;
							if(current >= numEntries)
							{
								current = 0;
							}
							if(entries[current].StartsWith(charCode))
							{
								ChangeSelection(current);
								break;
							}
							--count;
						}
						return true;
					}
					else
					{
						return base.ProcessDialogChar(charCode);
					}
				}

		// Process a dialog key.
		protected override bool ProcessDialogKey(Keys keyData)
				{
					int next = selected;
					int visibleColumns;
					switch(keyData)
					{
						case Keys.Left:
						{
							if(next >= rows)
							{
								next -= rows;
							}
						}
						break;

						case Keys.Right:
						{
							if(next == -1)
							{
								next = 0;
							}
							else
							{
								next += rows;
								if(next >= numEntries)
								{
									next -= rows;
								}
							}
						}
						break;

						case Keys.Up:
						{
							if(next > 0)
							{
								--next;
							}
						}
						break;

						case Keys.Down:
						{
							++next;
							if(next >= numEntries)
							{
								next = numEntries - 1;
							}
						}
						break;

						case Keys.Home:
						{
							next = 0;
						}
						break;

						case Keys.End:
						{
							next = numEntries - 1;
						}
						break;

						case Keys.PageUp:
						{
							visibleColumns =
								(columnWidth != 0 ?
									((ClientSize.Width + columnWidth - 1)
										/ columnWidth) : 0);
							next -= rows * visibleColumns;
							if(next < 0)
							{
								next = 0;
							}
						}
						break;

						case Keys.PageDown:
						{
							visibleColumns =
								(columnWidth != 0 ?
									((ClientSize.Width + columnWidth - 1)
										/ columnWidth) : 0);
							next += rows * visibleColumns;
							if(next >= numEntries)
							{
								next = numEntries - 1;
							}
						}
						break;

						case Keys.Enter:
						{
							ActivateSelected();
							return true;
						}
						// Not reached.

						default: return base.ProcessDialogKey(keyData);
					}
					ChangeSelection(next);
					return true;
				}

		// Activate the selected item.
		private void ActivateSelected()
				{
					// Bail out if nothing is selected.
					if(selected < 0 || selected >= numEntries)
					{
						return;
					}

					// Get the selected entry and check its type.
					FilesystemEntry entry = entries[selected];
					if(entry.isDirectory)
					{
						// This is a sub-directory which we should enter.
						dialog.ChangeDirectory(entry.fullName);
					}
					else
					{
						// The user has selected a file: accept the dialog.
						dialog.AcceptDialog(null, null);
					}
				}

	}; // class FileIconListBox

	// Form that defines the UI of a file dialog.
	internal sealed class FileDialogForm : Form
	{
		// Internal state.
		private FileDialog fileDialogParent;
		private VBoxLayout vbox;
		private HBoxLayout hbox;
		private GridLayout grid;
		private FileIconListBox listBox;
		private ComboBox directory;
		private Button upButton;
		private Button newDirButton;
		private Label nameLabel;
		private TextBox name;
		private Label typeLabel;
		private ComboBox type;
		private Button okButton;
		private Button cancelButton;
		private String currentDirectory;
		private String pattern;
		private CheckBox readOnly;
		private bool manualUserTextChange;
		private Image upIcon;
		private Image newIcon;

		// Constructor.
		public FileDialogForm(FileDialog fileDialogParent)
				{
					// Record the parent for later access.
					this.fileDialogParent = fileDialogParent;

					// Construct the layout boxes for the file dialog.
					vbox = new VBoxLayout();
					vbox.Dock = DockStyle.Fill;
					hbox = new HBoxLayout();
					listBox = new FileIconListBox(this);
					grid = new GridLayout(3, 2);
					grid.StretchColumn = 1;
					vbox.Controls.Add(hbox);
					vbox.Controls.Add(listBox);
					vbox.Controls.Add(grid);
					vbox.StretchControl = listBox;

					// Add the top line (directory name and up button).
					upIcon = new Bitmap(typeof(FileDialog), "small_up.ico");
					newIcon = new Bitmap(typeof(FileDialog), "small_new.ico");
					directory = new ComboBox();
					upButton = new Button();
					upButton.FlatStyle = FlatStyle.Popup;
					upButton.Image = upIcon;
					upButton.Size = new Size(23, 23);
					newDirButton = new Button();
					newDirButton.FlatStyle = FlatStyle.Popup;
					newDirButton.Image = newIcon;
					newDirButton.Size = new Size(23, 23);
				#if ECMA_COMPAT
					// Cannot create directories in ECMA-compat mode!
					newDirButton.Visible = false;
				#endif
					hbox.StretchControl = directory;
					hbox.Controls.Add(directory);
					hbox.Controls.Add(upButton);
					hbox.Controls.Add(newDirButton);

					// The second line is "listBox", already created above.

					// Add the third line (file name entry fields).
					nameLabel = new Label();
					nameLabel.Text = S._("SWF_FileDialog_FileName",
										 "File name:");
					name = new TextBox();
					okButton = new Button();
					okButton.Text = fileDialogParent.OkButtonName;
					grid.SetControl(0, 0, nameLabel);
					grid.SetControl(1, 0, name);
					grid.SetControl(2, 0, okButton);

					// Add the fourth line (file type entry fields).
					typeLabel = new Label();
					typeLabel.Text = S._("SWF_FileDialog_FilesOfType",
										 "Files of type:");
					type = new ComboBox();
					cancelButton = new Button();
					cancelButton.Text = S._("SWF_MessageBox_Cancel", "Cancel");
					grid.SetControl(0, 1, typeLabel);
					grid.SetControl(1, 1, type);
					grid.SetControl(2, 1, cancelButton);

					// Add the fifth line (read-only checkbox).
					if(fileDialogParent is OpenFileDialog)
					{
						readOnly = new CheckBox();
						readOnly.Text = S._("SWF_FileDialog_ReadOnly",
											"Open as read-only");
						readOnly.Checked =
							((OpenFileDialog)fileDialogParent).ReadOnlyChecked;
						readOnly.Visible =
							((OpenFileDialog)fileDialogParent).ShowReadOnly;
						readOnly.CheckStateChanged +=
							new EventHandler(ReadOnlyStateChanged);
						vbox.Controls.Add(readOnly);
					}

					// Add the top-level vbox to the dialog and set the size.
					Controls.Add(vbox);
					Size size = vbox.RecommendedSize;
					if(size.Width < 500)
					{
						size.Width = 500;
					}
					if(size.Height < 300)
					{
						size.Height = 300;
					}
					ClientSize = size;
					MinimumSize = size;
					MinimizeBox = false;
					ShowInTaskbar = false;

					// Hook up interesting events.
					upButton.Click += new EventHandler(UpOneLevel);
					newDirButton.Click += new EventHandler(NewFolder);
					directory.SelectedIndexChanged +=
						new EventHandler(DirectorySelectionChanged);
					name.TextChanged += new EventHandler(NameTextChanged);
					okButton.Click += new EventHandler(AcceptDialog);
					cancelButton.Click += new EventHandler(CancelDialog);
					type.SelectedIndexChanged +=
						new EventHandler(TypeSelectionChanged);

					// Match the requested settings from the dialog parent.
					RefreshAll();

					// Scan the initial directory.
					String dir = fileDialogParent.InitialDirectory;
					pattern = fileDialogParent.GetFilterPattern();
					if(dir == null || dir.Length == 0)
					{
						dir = Directory.GetCurrentDirectory();
						String filename = fileDialogParent.fileName;
						if(filename != null && filename.Length > 0)
						{
							// Use the previous filename as a starting point.
							if(IsDirectory(filename) ||
							   filename == Path.GetPathRoot(filename))
							{
								dir = filename;
							}
							else
							{
								dir = Path.GetDirectoryName(filename);
							}
						}
					}
					else
					{
						dir = Path.GetFullPath(dir);
					}
					if(!ChangeDirectory(dir))
					{
						ChangeDirectory(Directory.GetCurrentDirectory());
					}
				}

		// Dispose of this dialog.
		public void DisposeDialog()
				{
					if(upIcon != null)
					{
						upIcon.Dispose();
						newIcon.Dispose();
						upIcon = null;
						newIcon = null;
					}
					Dispose(true);
				}

		// Change to a new directory.
		public bool ChangeDirectory(String dir)
				{
					// Bail out if the directory does not exist.
					if(!IsDirectory(dir))
					{
						return false;
					}

					// Record the current directory.
					currentDirectory = dir;

					// Change the process directory if necessary.
					if(!(fileDialogParent.RestoreDirectory))
					{
						try
						{
							Directory.SetCurrentDirectory(currentDirectory);
						}
						catch(Exception)
						{
							// Ignore errors.
						}
					}

					// Scan the current directory and display the entries.
					FilesystemEntry[] entries;
					entries = ScanDirectory
						(dir, pattern, fileDialogParent.DereferenceLinks);
					listBox.Entries = entries;

					// Update the directory combo box with the list.
					directory.BeginUpdate();
					ComboBox.ObjectCollection items = directory.Items;
					items.Clear();
					String d = currentDirectory;
					while(d != Path.GetPathRoot(d))
					{
						items.Add(Path.GetFileName(d));
						d = Path.GetDirectoryName(d);
					}
					items.Add(d);
					directory.Text = currentDirectory;
					directory.EndUpdate();
					return true;
				}

		// Reload the current view to match the current settings.
		public void Reload()
				{
					ChangeDirectory(currentDirectory);
				}

		// Go up one level.
		public void UpOneLevel()
				{
					if(currentDirectory != Path.GetPathRoot(currentDirectory))
					{
						ChangeDirectory
							(Path.GetDirectoryName(currentDirectory));
					}
				}
		private void UpOneLevel(Object sender, EventArgs e)
				{
					UpOneLevel();
				}

		// Create a new folder.
		public void NewFolder()
				{
					NewFolderForm form = new NewFolderForm(currentDirectory);
					bool ok = false;
					try
					{
						if(form.ShowDialog(this) == DialogResult.OK)
						{
							ok = true;
						}
					}
					finally
					{
						form.DisposeDialog();
					}
					if(ok)
					{
						String dir = form.FolderName;
						if(dir != null)
						{
							ChangeDirectory(dir);
						}
					}
				}
		private void NewFolder(Object sender, EventArgs e)
				{
					NewFolder();
				}

		// Go to the home directory.
		public void Home()
				{
					String home = Environment.GetEnvironmentVariable("HOME");
					if(home != null && home.Length > 0)
					{
						ChangeDirectory(home);
					}
				}
		private void Home(Object sender, EventArgs e)
				{
					Home();
				}

		// Process a dialog key.
		protected override bool ProcessDialogKey(Keys keyData)
				{
					switch(keyData)
					{
						case Keys.F4:
						{
							directory.Focus();
							directory.DroppedDown = true;
						}
						break;

						case Keys.F5:
						{
							Reload();
						}
						break;

						case (Keys.Alt | Keys.Home):
						{
							Home();
						}
						break;

						case Keys.Enter:
						{
							AcceptDialog(null, null);
						}
						break;

						case Keys.Escape:
						{
							CancelDialog(null, null);
						}
						break;

						default: return base.ProcessDialogKey(keyData);
					}
					return true;
				}

		// Process a change of directory selection in a combo box.
		private void DirectorySelectionChanged(Object sender, EventArgs e)
				{
					int index = directory.SelectedIndex;
					String d = currentDirectory;
					while(d != Path.GetPathRoot(d))
					{
						if(index <= 0)
						{
							break;
						}
						--index;
						d = Path.GetDirectoryName(d);
					}
					if(d != currentDirectory)
					{
						ChangeDirectory(d);
					}
				}

		// Process a change of type selection in a combo box.
		private void TypeSelectionChanged(Object sender, EventArgs e)
				{
					int index = type.SelectedIndex;
					if(index >= 0 &&
					   index < fileDialogParent.filterNames.Length &&
					   pattern != fileDialogParent.filterPatterns[index] &&
					   currentDirectory != null)
					{
						pattern = fileDialogParent.filterPatterns[index];
						type.Text = fileDialogParent.filterNames[index];
						Reload();
					}
				}

		// Process a help request on the form.
		protected override void OnHelpRequested(HelpEventArgs e)
				{
					base.OnHelpRequested(e);
					fileDialogParent.EmitHelpRequest(e);
				}

		// Process a change of state on the read-only checkbox.
		private void ReadOnlyStateChanged(Object sender, EventArgs e)
				{
					((OpenFileDialog)fileDialogParent).readOnlyChecked =
						readOnly.Checked;
				}

		// Refresh all fields to match those in the dialog parent.
		public void RefreshAll()
				{
					Text = fileDialogParent.Title;
					HelpButton = fileDialogParent.ShowHelp;
					type.BeginUpdate();
					ComboBox.ObjectCollection items = type.Items;
					foreach(String name in fileDialogParent.filterNames)
					{
						items.Add(name);
					}
					type.EndUpdate();
					type.Text = fileDialogParent.filterNames
						[fileDialogParent.filterIndex - 1];
					pattern = fileDialogParent.GetFilterPattern();
					UpdateReadOnly();
				}

		// Update the read-only checkbox state information.
		public void UpdateReadOnly()
				{
					if(fileDialogParent is OpenFileDialog)
					{
						readOnly.Checked =
							((OpenFileDialog)fileDialogParent).ReadOnlyChecked;
						readOnly.Visible =
							((OpenFileDialog)fileDialogParent).ShowReadOnly;
					}
				}

		// Update the name in the text box.
		public void UpdateName(String filename)
				{
					name.Text = filename;
					manualUserTextChange = false;
				}

		// Notice a change to the text in the text box, for distinguishing
		// between changes made by selecting files and manual user entry.
		private void NameTextChanged(Object sender, EventArgs e)
				{
					manualUserTextChange = true;
				}

		// Handle a "focus enter" event.
		protected override void OnEnter(EventArgs e)
				{
					// Set the focus to the name text box immediately
					// if the top-level form gets the focus.
					name.Focus();
				}

		// Handle the "accept" button on this dialog.
		public void AcceptDialog(Object sender, EventArgs e)
				{
					String filename = name.Text.Trim();
					if(filename.IndexOfAny(directorySeparators) != -1)
					{
						// Contains a directory specification.
						if(filename.Length >= 2 &&
						   filename[0] == '~' &&
						   (filename[1] == '/' || filename[1] == '\\'))
						{
							// Filename of the form "~/xyzzy", which we
							// convert into a user home directory reference.
							String home = Environment.GetEnvironmentVariable
								("HOME");
							if(home != null && home.Length > 0)
							{
								filename = Path.Combine
									(home, filename.Substring(2));
							}
						}
						filename = Path.Combine(currentDirectory, filename);
						if(filename == Path.GetPathRoot(filename))
						{
							// Move to a root directory.
							ChangeDirectory(filename);
							name.Text = String.Empty;
						}
						else
						{
							// Split the filename into directory and base
							// components.  Then, change to the specified
							// directory, and re-accept the dialog with
							// the base name.
							if(ChangeDirectory(Path.GetDirectoryName(filename)))
							{
								name.Text = Path.GetFileName(filename);
								AcceptDialog(sender, e);
							}
						}
					}
					else if(filename.IndexOfAny(wildcards) != -1)
					{
						// Contains a wildcard specification.
						pattern = filename;
						int index = fileDialogParent.GetWildcardFilterIndex
							(pattern);
						if(index != 0)
						{
							type.Text = fileDialogParent.filterNames[index - 1];
						}
						else
						{
							type.Text = pattern;
						}
						Reload();
						name.Text = String.Empty;
					}
					else if(filename == "..")
					{
						// Move up to the parent directory.
						UpOneLevel();
						name.Text = String.Empty;
					}
					else if(filename == ".")
					{
						// Reload the current level.
						Reload();
						name.Text = String.Empty;
					}
					else if(filename == "~")
					{
						// Go to the home directory.
						Home();
						name.Text = String.Empty;
					}
					else if(filename.Length > 0)
					{
						// Ordinary filename, relative to current directory.
						filename = Path.Combine(currentDirectory, filename);
						if(IsDirectory(filename))
						{
							// This is a directory.
							ChangeDirectory(filename);
							name.Text = "";
						}
						else if(FilenameAcceptable(ref filename))
						{
							// We finally have a result, so exit the dialog.
							fileDialogParent.fileName = filename;
							fileDialogParent.fileNames = null;
							if(fileDialogParent.IsFileOk())
							{
								DialogResult = DialogResult.OK;
							}
						}
					}
				}

		// Handle the "cancel" button on this dialog.
		private void CancelDialog(Object sender, EventArgs e)
				{
					DialogResult = DialogResult.Cancel;
				}

		// Handle a "closing" event on this form.
		protected override void OnClosing(CancelEventArgs e)
				{
					base.OnClosing(e);
					e.Cancel = true;
					DialogResult = DialogResult.Cancel;
				}

		// Change the filename in the text box to a new value.
		public void ChangeFilename(String filename)
				{
					if(IsDirectory(filename) ||
					   filename == Path.GetPathRoot(filename))
					{
						ChangeDirectory(filename);
					}
					else
					{
						ChangeDirectory(Path.GetDirectoryName(filename));
						name.Text = Path.GetFileName(filename);
					}
				}

		// Determine if a filename is acceptable according to the dialog rules.
		// The filename may be modified to include a default extension.
		private bool FilenameAcceptable(ref String filename)
				{
					// Add a default extension if necessary.
					if(fileDialogParent.AddExtension &&
					   !Path.HasExtension(filename) &&
					   !File.Exists(filename))
					{
						int index = fileDialogParent.GetWildcardFilterIndex
							(pattern);
						String extension = fileDialogParent.defaultExt;
						String filterPattern;
						if(index != 0)
						{
							filterPattern =
								fileDialogParent.filterPatterns[index - 1];
						}
						else
						{
							filterPattern = String.Empty;
						}
						if(fileDialogParent.CheckFileExists)
						{
							// Look for any extension for which the file
							// actually exists in the filesystem.
							String[] patterns = filterPattern.Split(';');
							foreach(String patt in patterns)
							{
								if(patt != "*.*" && patt.StartsWith("*."))
								{
									String temp = filename + patt.Substring(1);
									if(File.Exists(temp))
									{
										extension = patt.Substring(1);
										break;
									}
								}
							}
						}
						else
						{
							// Only look at the first pattern.
							index = filterPattern.IndexOf(';');
							if(index != -1)
							{
								filterPattern = filterPattern.Substring
									(0, index);
							}
							if(filterPattern != "*.*" &&
							   filterPattern.StartsWith("*."))
							{
								extension = filterPattern.Substring(1);
							}
						}
						filename += extension;
					}

					// Check for file and path existence.
					if(fileDialogParent.CheckFileExists)
					{
						if(!File.Exists(filename))
						{
							MessageBox.Show
								(String.Format
									(S._("SWF_FileDialog_FileNotFound"),
									 filename),
								 fileDialogParent.Title,
								 MessageBoxButtons.OK,
								 MessageBoxIcon.Exclamation);
							return false;
						}
					}
					else if(fileDialogParent.CheckPathExists)
					{
						if(!IsDirectory(Path.GetDirectoryName(filename)))
						{
							MessageBox.Show
								(String.Format
									(S._("SWF_FileDialog_PathNotFound"),
									 filename),
								 fileDialogParent.Title,
								 MessageBoxButtons.OK,
								 MessageBoxIcon.Exclamation);
							return false;
						}
					}

					// Check for create and overwrite.
					if(fileDialogParent is SaveFileDialog)
					{
						if(((SaveFileDialog)fileDialogParent).OverwritePrompt)
						{
							if(File.Exists(filename))
							{
								if(MessageBox.Show
									(String.Format
										(S._("SWF_FileDialog_Overwrite"),
										 filename),
									 fileDialogParent.Title,
									 MessageBoxButtons.YesNo,
									 MessageBoxIcon.Question,
									 MessageBoxDefaultButton.Button2)
									 		!= DialogResult.Yes)
								{
									return false;
								}
							}
						}
						if(((SaveFileDialog)fileDialogParent).CreatePrompt)
						{
							if(!File.Exists(filename))
							{
								if(MessageBox.Show
									(String.Format
										(S._("SWF_FileDialog_Create"),
										 filename),
									 fileDialogParent.Title,
									 MessageBoxButtons.YesNo,
									 MessageBoxIcon.Question)
									 		!= DialogResult.Yes)
								{
									return false;
								}
							}
						}
					}

					// If we get here, then the filename is acceptable.
					return true;
				}

	}; // class FileDialogForm

	// Form class for the "New Folder" dialog box.
	private sealed class NewFolderForm : Form
	{
		// Internal state.
		private Control iconControl;
		private Icon icon;
		private Label textLabel;
		private Button button1;
		private Button button2;
		private VBoxLayout vbox;
		private VBoxLayout vbox2;
		private HBoxLayout hbox;
		private TextBox textBox;
		private ButtonBoxLayout buttonBox;
		private String currentDirectory;
		private String folderName;

		// Constructor.
		public NewFolderForm(String currentDirectory)
				{
					// Record the current directory to create the folder in.
					this.currentDirectory = currentDirectory;

					// Set the dialog box's caption.
					Text = S._("SWF_FileDialog_NewFolder");

					// Make the borders suitable for a dialog box.
					FormBorderStyle = FormBorderStyle.FixedDialog;
					MinimizeBox = false;
					ShowInTaskbar = false;

					// Create the layout areas.
					vbox = new VBoxLayout();
					hbox = new HBoxLayout();
					vbox2 = new VBoxLayout();
					buttonBox = new ButtonBoxLayout();
					vbox.Controls.Add(hbox);
					vbox.Controls.Add(buttonBox);
					vbox.StretchControl = hbox;
					buttonBox.UniformSize = true;
					vbox.Dock = DockStyle.Fill;
					Controls.Add(vbox);

					// Create a control to display the message box icon.
					this.icon = new Icon(typeof(MessageBox), "question.ico");
					iconControl = new Control();
					iconControl.ClientSize = this.icon.Size;
					iconControl.TabStop = false;
					hbox.Controls.Add(iconControl);
					hbox.Controls.Add(vbox2);

					// Create the label containing the message text.
					textLabel = new Label();
					textLabel.TextAlign = ContentAlignment.MiddleLeft;
					textLabel.TabStop = false;
					textLabel.Text = S._("SWF_FileDialog_NewFolderName");
					vbox2.Controls.Add(textLabel);

					// Create the text box for the folder name entry.
					textBox = new TextBox();
					vbox2.Controls.Add(textBox);

					// Create the buttons.
					button1 = new Button();
					button2 = new Button();
					button1.Text = S._("SWF_MessageBox_OK", "OK");
					button2.Text = S._("SWF_MessageBox_Cancel", "Cancel");
					buttonBox.Controls.Add(button1);
					buttonBox.Controls.Add(button2);
					AcceptButton = button1;
					CancelButton = button2;

					// Hook up the events for the form.
					button1.Click += new EventHandler(Button1Clicked);
					button2.Click += new EventHandler(Button2Clicked);
					Closing += new CancelEventHandler(CloseRequested);
					iconControl.Paint += new PaintEventHandler(PaintIcon);

					// Set the initial message box size to the vbox's
					// recommended size.
					Size size = vbox.RecommendedSize;
					if(size.Width < 350)
					{
						size.Width = 350;
					}
					ClientSize = size;
					MinimumSize = size;
					MaximumSize = size;
				}

		// Detect when button 1 is clicked.
		private void Button1Clicked(Object sender, EventArgs args)
				{
					// Get the text in the box and bail out if it is empty.
					String name = textBox.Text.Trim();
					if(name == String.Empty)
					{
						return;
					}

					// Verify that the folder does not contain directory
					// separator characters.
					if(name.IndexOfAny(directorySeparators) != -1)
					{
						MessageBox.Show
							(S._("SWF_FileDialog_InvalidChar"),
							 Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}

					// Check for pre-existence of the directory.
					name = Path.Combine(currentDirectory, name);
					if(File.Exists(name) || Directory.Exists(name))
					{
						MessageBox.Show
							(String.Format
								(S._("SWF_FileDialog_Exists"), name),
							 Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}

				#if !ECMA_COMPAT
					// Try to create the specified directory.
					try
					{
						Directory.CreateDirectory(name);
					}
					catch(Exception)
					{
						MessageBox.Show
							(String.Format
								(S._("SWF_FileDialog_CannotMkdir"), name),
							 Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
				#endif

					// Quit the dialog with the "OK" signal.
					folderName = name;
					DialogResult = DialogResult.OK;
				}

		// Detect when button 2 is clicked.
		private void Button2Clicked(Object sender, EventArgs args)
				{
					DialogResult = DialogResult.Cancel;
				}

		// Handle the "Closing" event on the form.
		private void CloseRequested(Object sender, CancelEventArgs args)
				{
					DialogResult = DialogResult.Cancel;
				}

		// Paint the icon control.
		private void PaintIcon(Object sender, PaintEventArgs args)
				{
					Graphics g = args.Graphics;
					g.DrawIcon(icon, 0, 0);
				}

		// Dispose of this dialog.
		public void DisposeDialog()
				{
					if(icon != null)
					{
						icon.Dispose();
						icon = null;
					}
					Dispose(true);
				}

		// Handle a "focus enter" event.
		protected override void OnEnter(EventArgs e)
				{
					// Set the focus to the name text box immediately
					// if the top-level form gets the focus.
					textBox.Focus();
				}

		// Get the name of the folder that was just created.
		public String FolderName
				{
					get
					{
						return folderName;
					}
				}

	}; // class NewFolderForm

}; // class FileDialog

}; // namespace System.Windows.Forms
