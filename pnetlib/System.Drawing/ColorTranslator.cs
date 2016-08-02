/*
 * ColorTranslator.cs - Implementation of the
 *			"System.Drawing.Printing.ColorTranslator" class.
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

namespace System.Drawing
{

public sealed class ColorTranslator
{
	// Convert OLE system color indicators into KnownColor values.
	private static readonly KnownColor[] oleKnownColors = {
				KnownColor.ScrollBar,
				KnownColor.Desktop,
				KnownColor.ActiveCaption,
				KnownColor.InactiveCaption,
				KnownColor.Menu,
				KnownColor.Window,
				KnownColor.WindowFrame,
				KnownColor.MenuText,
				KnownColor.WindowText,
				KnownColor.ActiveCaptionText,
				KnownColor.ActiveBorder,
				KnownColor.InactiveBorder,
				KnownColor.AppWorkspace,
				KnownColor.Highlight,
				KnownColor.HighlightText,
				KnownColor.Control,
				KnownColor.ControlDark,
				KnownColor.GrayText,
				KnownColor.ControlText,
				KnownColor.InactiveCaptionText,
				KnownColor.ControlLightLight,
				KnownColor.ControlDarkDark,
				KnownColor.ControlLight,
				KnownColor.InfoText,
				KnownColor.Info,
			};

	// Convert KnownColor values into OLE system color indicators.
	private static readonly uint[] oleSystemColors = {
				0x8000000a, 0x80000002, 0x80000009, 0x8000000c,
				0x8000000f, 0x80000010, 0x80000015, 0x80000016,
				0x80000014, 0x80000012, 0x80000001, 0x80000011,
				0x8000000d, 0x8000000e, 0x8000000d, 0x8000000b,
				0x80000003, 0x80000013, 0x80000018, 0x80000017,
				0x80000004, 0x80000007, 0x80000000, 0x80000005,
				0x80000006, 0x80000008
			};

	// This class cannot be instantiated.
	private ColorTranslator() {}

	// Transform a HTML color name into a "Color" value.
	public static Color FromHtml(String htmlColor)
			{
				// 1: is "#numeric" or "textual"
				// 1a: if numeric then remove #
				//		convert hex(char) to dec(int)
				//		Feed Color.FromArgb()
				// 1b: if textual then lookup the Enum to find (no case)
				//			the exact value
				//		take the "KnownValue" field
				//		Feed Color.FromKnownValue() (then force a resolve if needed)

				Color ret = Color.Empty; // default.

				if(htmlColor == null)
				{
					// throw exception?
					// No: For compatibility
					return Color.Empty;
				}
				if(htmlColor.Length == 0)
				{
					// throw exception?
					// No: For compatibility
					return Color.Empty;
				}
				if(htmlColor.Substring(0,1) == "#")
				{
					/// If color is #123, the result is #112233 (4 bit to 8 bit channel)
					/// If color is #1 or #12 or #1234 or #12345
					///		the result is #000001 or #000012 or #001234 or #012345 (prepend "0")
					/// If color is a KnownColor (doesn't start with "#"), get Color.FromName.
					int res=0; // res = result from #RGB
					if(htmlColor.Length == 4) // #123
					{
						res = Int32.Parse(htmlColor.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier);
						res = (((res & 0xF00)<<8) * 0x11) + (((res & 0x0F0)<<4) * 0x11) + ((res & 0x00F) * 0x11);
						// Explanation:
						// #123
						// (res & 0xF00) ->       0x100
						// ((0x100) << 8) ->    0x10000
						// (0x10000) * 0x11 -> 0x110000+
						// (res & 0x0F0) ->       0x020
						// ((0x20) << 4) ->      0x0200
						// (0x0200) * 0x11 ->    0x2200+
						// (res & 0x00F) ->         0x3
						// (0x3) * 0x11) ->        0x33=
						// ============ ->     0x112233
					}
					else if(htmlColor.Length != 7)
					{
						// throw BadLength exception?
						// No: For compatibility
						// Add Leading 0...
						string tmp = "0000000";
						tmp = tmp.Substring(htmlColor.Length);
						htmlColor = "#" + tmp + htmlColor.Substring(1);
						res = Int32.Parse(htmlColor.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier);
					}
					ret = Color.FromArgb(((res & 0x00FF0000)>>16),((res & 0x0000FF00)>>8),((res & 0x000000FF)));
				}
				else
				{
					// Is a KnownColor...
					ret = Color.FromName(htmlColor);
					// If it's not a KnownColor (bleu instead of blue)
					//		ret become Color.Empty.
				}

				return ret;
			}

	// Transform an OLE color value into a "Color" value.
	public static Color FromOle(int oleColor)
			{
				return FromWin32(oleColor);
			}

	// Transform a Win32 color value into a "Color" value.
	// This also understands system color indicators, for compatibility.
	public static Color FromWin32(int win32Color)
			{
				if((win32Color & unchecked((int)0xFF000000))
						!= unchecked((int)0x80000000))
				{
					return Color.FromArgb(win32Color & 0xFF,
								(win32Color >> 8) & 0xFF,
								(win32Color >> 16) & 0xFF);
				}
				else
				{
					int sysColor = ((win32Color >> 24) & 0xFF);
					if(sysColor >= 0 && sysColor < oleKnownColors.Length)
					{
						return new Color(oleKnownColors[sysColor]);
					}
					else
					{
						return Color.Empty;
					}
				}
			}

	// Convert a "Color" value into a HTML color name.
	public static String ToHtml(Color color)
			{
				String ret = "";
				int value = (int)(color.ToKnownColor());
				if(color.IsKnownColor)//(value>=1) && (value<=167))
				{
					//int value = (int)(color.ToKnownColor());
					if(value >= 1 && value <= oleSystemColors.Length)
					{
						switch(value)
						{
							case (int)KnownColor.ActiveCaptionText:
								ret = "captiontext";
								break;
							case (int)KnownColor.Control:
								ret = "buttonface";
								break;
							case (int)KnownColor.ControlDark:
								ret = "buttonshadow";
								break;
							case (int)KnownColor.ControlDarkDark:
								ret = "threeddarkshadow";
								break;
							case (int)KnownColor.ControlLight:
								ret = "buttonface";
								break;
							case (int)KnownColor.ControlLightLight:
								ret = "buttonhighlight";
								break;
							case (int)KnownColor.ControlText:
								ret = "buttontext";
								break;
							case (int)KnownColor.Desktop:
								ret = "background";
								break;
							case (int)KnownColor.HotTrack:
								ret = "highlight";
								break;
							case (int)KnownColor.Info:
								ret = "infobackground";
								break;
							//case (int)KnownColor.LightGray: ret = "LightGrey";break; // -> MS BUG.
							// This last case cause a compare error but the good result is
							// LightGray. So, never replace LightGray by LightGrey.
							default:
								ret = color.Name.ToLower();
								break;
						}
					}
					else
					{
						ret = color.Name;
					}
				}
				else if(color.Name != "0")
				{
					ret = "#" + color.Name.Substring(2).ToUpper();
					/// GET R,G,B -> Format to hexa RRGGBB prepend '#'
				}
				return ret;
			}

	// Convert a "Color" value into an OLE color value.
	public static int ToOle(Color color)
			{
				if(color.IsKnownColor)
				{
					int value = (int)(color.ToKnownColor());
					if(value >= 1 && value <= oleSystemColors.Length)
					{
						return (int)(oleSystemColors[value - 1]);
					}
				}
				return ToWin32(color);
			}

	// Convert a "Color" value into a Win32 color value.
	// This does not support system color indicators, for compatibility.
	public static int ToWin32(Color color)
			{
				return (color.R | (color.G << 8) | (color.B << 16));
			}

}; // class ColorTranslator

}; // namespace System.Drawing

