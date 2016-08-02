/*
 * DataFormats.cs - Implementation of the
 *			"System.Windows.Forms.DataFormats" class.
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

using System.Drawing.Toolkit;

public class DataFormats
{
	// Internal state.
	private static Format formats = null;

	// Cannot instantiate this class.
	private DataFormats() {}

	// Information about a registered clipboard format
	public class Format
	{
		// Internal state.
		private int id;
		private String name;
		private Format next;

		// Constructor.
		public Format(int id, String name, Format next)
				{
					this.id = id;
					this.name = name;
					this.next = next;
				}
		public Format(String name, Format next)
				{
					this.id = AllocID(name);
					this.name = name;
					this.next = next;
				}

		// Properties.
		public int Id
				{
					get
					{
						return id;
					}
				}
		public String Name
				{
					get
					{
						return name;
					}
				}
		internal Format Next
				{
					get
					{
						return next;
					}
				}

	}; // class Format

	// Standard format names.
	public static readonly String Bitmap = "Bitmap";
	public static readonly String CommaSeparatedValue = "Csv";
	public static readonly String Dib = "DeviceIndependentBitmap";
	public static readonly String Dif = "DataInterchangeFormat";
	public static readonly String EnhancedMetafile = "EnhancedMetafile";
	public static readonly String FileDrop = "FileDrop";
	public static readonly String Html = "HTML Format";
	public static readonly String Locale = "Locale";
	public static readonly String MetafilePict = "MetaFilePict";
	public static readonly String OemText = "OEMText";
	public static readonly String Palette = "Palette";
	public static readonly String PenData = "PenData";
	public static readonly String Riff = "RiffAudio";
	public static readonly String Rtf = "Rich Text Format";
	public static readonly String Serializable
			= "WindowsForms10PersistentObject";
	public static readonly String StringFormat = typeof(String).FullName;
	public static readonly String SymbolicLink = "SymbolicLink";
	public static readonly String Text = "Text";
	public static readonly String Tiff = "TaggedImageFileFormat";
	public static readonly String UnicodeText = "UnicodeText";
	public static readonly String WaveAudio = "WaveAudio";

	// Load the standard formats into the list.
	private static void LoadStandardFormats()
			{
				// Standard formats with fixed identifiers.
				formats = new Format(1, Text, formats);
				formats = new Format(2, Bitmap, formats);
				formats = new Format(3, MetafilePict, formats);
				formats = new Format(4, SymbolicLink, formats);
				formats = new Format(5, Dif, formats);
				formats = new Format(6, Tiff, formats);
				formats = new Format(7, OemText, formats);
				formats = new Format(8, Dib, formats);
				formats = new Format(9, Palette, formats);
				formats = new Format(10, PenData, formats);
				formats = new Format(11, Riff, formats);
				formats = new Format(12, WaveAudio, formats);
				formats = new Format(13, UnicodeText, formats);
				formats = new Format(14, EnhancedMetafile, formats);
				formats = new Format(15, FileDrop, formats);
				formats = new Format(16, Locale, formats);

				// Other formats that need identifiers allocated for them.
				formats = new Format(CommaSeparatedValue, formats);
				formats = new Format(Html, formats);
				formats = new Format(Rtf, formats);
				formats = new Format(Serializable, formats);
				formats = new Format(StringFormat, formats);
			}

	// Get the name of an existing format identifier.
	private static String GetFormatName(int id)
			{
				IToolkitClipboard clipboard =
					ToolkitManager.Toolkit.GetClipboard();
				if(clipboard != null)
				{
					String format = clipboard.GetFormat(id);
					if(format != null)
					{
						return format;
					}
				}
				return "Format" + id.ToString();
			}

	// Allocate a new format identifier.
	private static int AllocID(String format)
			{
				IToolkitClipboard clipboard =
					ToolkitManager.Toolkit.GetClipboard();
				if(clipboard != null)
				{
					int id = clipboard.RegisterFormat(format);
					if(id != -1)
					{
						return id;
					}
				}
				Format info = formats;
				int max = 0;
				while(info != null)
				{
					if(info.Id > max)
					{
						max = info.Id;
					}
					info = info.Next;
				}
				return max + 1;
			}

	// Get or register format information for a Windows clipboard ID.
	public static Format GetFormat(int id)
			{
				lock(typeof(DataFormats))
				{
					if(formats == null)
					{
						LoadStandardFormats();
					}
					Format info = formats;
					while(info != null)
					{
						if(info.Id == id)
						{
							return info;
						}
						info = info.Next;
					}
					info = new Format(id, GetFormatName(id), formats);
					formats = info;
					return info;
				}
			}

	// Get or register format information for a Windows clipboard name.
	public static Format GetFormat(String format)
			{
				if(format == null)
				{
					throw new ArgumentNullException("format");
				}
				lock(typeof(DataFormats))
				{
					if(formats == null)
					{
						LoadStandardFormats();
					}
					Format info = formats;
					while(info != null)
					{
						if(info.Name == format)
						{
							return info;
						}
						info = info.Next;
					}
					info = new Format(AllocID(format), format, formats);
					formats = info;
					return info;
				}
			}

}; // class DataFormats

}; // namespace System.Windows.Forms
