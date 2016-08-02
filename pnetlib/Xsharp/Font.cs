/*
 * Font.cs - Font object for X#.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp
{

using System;
using System.IO;
using System.Text;
using System.Reflection;
using Xsharp.Types;

/// <summary>
/// <para>The <see cref="T:Xsharp.Font"/> class encapsulates
/// the operations on an X font.</para>
/// </summary>
public class Font
{
	// Internal class that keeps track of displays and fontsets.
	internal class FontInfo
	{
		public FontInfo next;
		public FontExtents extents;
		public Display dpy;
		public IntPtr fontSet;

	} // class FontInfo

	// Information about a registered font.
	private class RegisteredFont
	{
		public RegisteredFont next;
		public String family;
		public int pointSize;
		public FontStyle style;
		public byte[] data;
		public IntPtr loadedData;

	} // class RegisteredFont

	// Internal state.
	internal String family;
	internal int pointSize;
	internal FontStyle style;
	internal String xname;
	internal FontInfo infoList;
	private static RegisteredFont registeredFonts;

	/// <summary>
	/// <para>The family name for the default sans-serif font.</para>
	/// </summary>
	public static readonly String DefaultSansSerif = "defaultsans";

	/// <summary>
	/// <para>The family name for the usual X sans-serif font.</para>
	/// </summary>
	public static readonly String SansSerif = "helvetica";

	/// <summary>
	/// <para>The family name for the default serif font.</para>
	/// </summary>
	public static readonly String Serif = "times";

	/// <summary>
	/// <para>The family name for the default fixed-width font.</para>
	/// </summary>
	public static readonly String Fixed = "courier";

	// Constructors.
	private Font(String family, int pointSize, FontStyle style)
			{
				if(family != null)
					this.family = family;
				else
					this.family = SansSerif;
				if(pointSize < 0 || pointSize > 10000)
					this.pointSize = 120;
				else
					this.pointSize = pointSize;
				this.style = style;
				this.xname = null;
				this.infoList = null;
			}
	private Font(String name, bool unused)
			{
				this.family = null;
				this.pointSize = 0;
				this.style = FontStyle.Normal;
				this.xname = name;
				this.infoList = null;
			}

	/// <summary>
	/// <para>Get the family name of this font.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The family name, or <see langword="null"/> if this font
	/// was created from an XLFD name.</para>
	/// </value>
	public String Family
			{
				get
				{
					return family;
				}
			}

	/// <summary>
	/// <para>Get the point size of this font.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The point size.</para>
	/// </value>
	public int PointSize
			{
				get
				{
					return pointSize;
				}
			}

	/// <summary>
	/// <para>Get the extra styles that are associated with this font.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The extra styles.</para>
	/// </value>
	public FontStyle Style
			{
				get
				{
					return style;
				}
			}

	/// <summary>
	/// <para>Get the XLFD name of this font.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The XLFD name, or <see langword="null"/> if this font
	/// was not created from an XLFD name.</para>
	/// </value>
	public String XLFD
			{
				get
				{
					return xname;
				}
			}

	/// <summary>
	/// <para>Determine if two font descriptions are equal.</para>
	/// </summary>
	///
	/// <param name="obj">
	/// <para>The font object to compare against.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the two fonts are equal;
	/// <see langword="false"/> otherwise.</para>
	/// </returns>
	public override bool Equals(Object obj)
			{
				Font other = (obj as Font);
				if(other != null)
				{
					if(this == other)
					{
						return true;
					}
					else
					{
						return (this.family == other.family &&
						        this.pointSize == other.pointSize &&
								this.style == other.style &&
								this.xname == other.xname);
					}
				}
				else
				{
					return false;
				}
			}

	/// <summary>
	/// <para>Get the hash code for a font.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns the hash code.</para>
	/// </returns>
	public override int GetHashCode()
			{
				return (((family == null) ? xname.GetHashCode()
										  : family.GetHashCode()) +
						pointSize + (int)style);
			}

	// Determine if we appear to be running in a Latin1 locale.
	// We can optimize font handling a little if we are.
	private static bool IsLatin1()
			{
			#if !ECMA_COMPAT
				int codePage = Encoding.Default.WindowsCodePage;
			#else
				int codePage = Encoding.Default.GetHashCode();
			#endif
				if(codePage == 1252 || codePage == 28591)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	/// <summary>
	/// <para>Constructs a new instance of <see cref="T:Xsharp.Font"/>.
	/// </para>
	/// </summary>
	///
	/// <param name="family">
	/// <para>The name of the font family, or <see langword="null"/> to
	/// use the default sans-serif font.</para>
	/// </param>
	///
	/// <param name="pointSize">
	/// <para>The point size (120 is typically "normal height").</para>
	/// </param>
	///
	/// <param name="style">
	/// <para>Additional styles to apply to the font.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The font object that corresponds to the given parameters.</para>
	/// </returns>
	public static Font CreateFont
				(String family, int pointSize, FontStyle style)
			{
				// Determine if we are running in a Latin1 locale.
				bool latin1 = IsLatin1();

				// Do we have a registered font for this name, size, and style?
				RegisteredFont font;
				lock(typeof(Font))
				{
					font = registeredFonts;
					while(font != null)
					{
						if(font.family == family &&
						   font.pointSize == NormalizePointSize(pointSize) &&
						   font.style == (style & ~(FontStyle.Underlined |
						   							FontStyle.StrikeOut)))
						{
							break;
						}
						font = font.next;
					}
				}
				if(latin1 && font != null)
				{
					// Create the font image if necessary.
					if(font.loadedData == IntPtr.Zero)
					{
						font.loadedData = Xlib.XSharpPCFCreateImage
							(font.data, (uint)(font.data.Length));
					}

					// Create a PCF font based on a registered font image.
					if(font.loadedData != IntPtr.Zero)
					{
						return new PCFFont
							(family, pointSize, style, font.loadedData);
					}
				}

				// Search for a regular X font that matches the conditions.
				if(family == DefaultSansSerif)
				{
					family = SansSerif;
				}
				if(Xlib.XSharpUseXft() != 0)
				{
					return new XftFont(family, pointSize, style);
				}
				else if(latin1)
				{
					return new FontStructFont(family, pointSize, style);
				}
				else
				{
					return new Font(family, pointSize, style);
				}
			}

	/// <summary>
	/// <para>Constructs a new instance of <see cref="T:Xsharp.Font"/>
	/// with a default point size and style.</para>
	/// </summary>
	///
	/// <param name="family">
	/// <para>The name of the font family, or <see langword="null"/> to
	/// use the default sans-serif font.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The font object that corresponds to the given parameters.</para>
	/// </returns>
	public static Font CreateFont(String family)
			{
				return CreateFont(family, 120, FontStyle.Normal);
			}

	/// <summary>
	/// <para>Constructs a new instance of <see cref="T:Xsharp.Font"/>
	/// with a default style.</para>
	/// </summary>
	///
	/// <param name="family">
	/// <para>The name of the font family, or <see langword="null"/> to
	/// use the default sans-serif font.</para>
	/// </param>
	///
	/// <param name="pointSize">
	/// <para>The point size (120 is typically "normal height").</para>
	/// </param>
	///
	/// <returns>
	/// <para>The font object that corresponds to the given parameters.</para>
	/// </returns>
	public static Font CreateFont(String family, int pointSize)
			{
				return CreateFont(family, pointSize, FontStyle.Normal);
			}

	/// <summary>
	/// <para>Construct a font from an XLFD name.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The XLFD name of the font.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The font object that corresponds to
	/// <paramref name="name"/>.</para>
	/// </returns>
	public static Font CreateFromXLFD(String name)
			{
				if(name == null)
				{
					return CreateFont(null);
				}
				else
				{
					return new Font(name, true);
				}
			}

	/// <summary>
	/// <para>Register font data with a particular set of font
	/// parameters.</para>
	/// </summary>
	///
	/// <param name="family">
	/// <para>The name of the font family, or <see langword="null"/> to
	/// use the default sans-serif font.</para>
	/// </param>
	///
	/// <param name="pointSize">
	/// <para>The point size (120 is typically "normal height").</para>
	/// </param>
	///
	/// <param name="style">
	/// <para>Additional styles to apply to the font.</para>
	/// </param>
	///
	/// <param name="data">
	/// <para>The font data.  This will normally be uncompressed
	/// PCF font data.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>Registering a style will also register the underlined and
	/// strikeout versions of the font, which are synthesized.</para>
	/// </remarks>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="family"/> or <paramref name="data"/>
	/// is <see langword="null"/>.</para>
	/// </exception>
	public static void RegisterFont
				(String family, int pointSize, FontStyle style, byte[] data)
			{
				if(family == null)
				{
					throw new ArgumentNullException("family");
				}
				if(data == null)
				{
					throw new ArgumentNullException("data");
				}
				RegisteredFont font = new RegisteredFont();
				font.family = family;
				font.pointSize = pointSize;
				font.style = style;
				font.data = data;
				font.loadedData = IntPtr.Zero;
				lock(typeof(Font))
				{
					font.next = registeredFonts;
					registeredFonts = font;
				}
			}

	/// <summary>
	/// <para>Register font data with a particular set of font
	/// parameters.  The font data is obtained from a manifest resource
	/// within an assembly.</para>
	/// </summary>
	///
	/// <param name="family">
	/// <para>The name of the font family, or <see langword="null"/> to
	/// use the default sans-serif font.</para>
	/// </param>
	///
	/// <param name="pointSize">
	/// <para>The point size (120 is typically "normal height").</para>
	/// </param>
	///
	/// <param name="style">
	/// <para>Additional styles to apply to the font.</para>
	/// </param>
	///
	/// <param name="assembly">
	/// <para>The assembly containing the font data.</para>
	/// </param>
	///
	/// <param name="resourceName">
	/// <para>The name of the resource containing the font data.  This will
	/// normally be uncompressed PCF font data.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>Registering a style will also register the underlined and
	/// strikeout versions of the font, which are synthesized.</para>
	/// </remarks>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="family"/>, <paramref name="assembly"/>,
	/// or <paramref name="resourceName"/> is <see langword="null"/>, or
	/// if the resource does not exist in the assembly.</para>
	/// </exception>
	public static void RegisterFont
				(String family, int pointSize, FontStyle style,
				 Assembly assembly, String resourceName)
			{
				Stream stream;
				byte[] data;

				// Validate the parameters.
				if(family == null)
				{
					throw new ArgumentNullException("family");
				}
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				if(resourceName == null)
				{
					throw new ArgumentNullException("resourceName");
				}

				// Load the font data from the resource.
				stream = assembly.GetManifestResourceStream(resourceName);
				if(stream == null)
				{
					throw new ArgumentNullException("resourceName");
				}
				try
				{
					data = new byte [(int)(stream.Length)];
					stream.Read(data, 0, data.Length);
				}
				finally
				{
					stream.Close();
				}

				// Register the font with the data that we just loaded.
				RegisterFont(family, pointSize, style, data);
			}

	/// <summary>
	/// <para>Measure the width, ascent, and descent of a string,
	/// to calculate its extents when drawn on a graphics context
	/// using this font.</para>
	/// </summary>
	///
	/// <param name="graphics">
	/// <para>The graphics context to measure with.</para>
	/// </param>
	///
	/// <param name="str">
	/// <para>The string to be measured.</para>
	/// </param>
	///
	/// <param name="index">
	/// <para>The starting index in <paramref name="str"/> of the first
	/// character to be measured.</para>
	/// </param>
	///
	/// <param name="count">
	/// <para>The number of characters <paramref name="str"/>
	/// to be measured.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the string, in pixels.</para>
	/// </param>
	///
	/// <param name="ascent">
	/// <para>The ascent of the string, in pixels.</para>
	/// </param>
	///
	/// <param name="descent">
	/// <para>The descent of the string, in pixels.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="graphics"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public virtual void MeasureString
				(Graphics graphics, String str, int index, int count,
				 out int width, out int ascent, out int descent)
			{
				// Validate the parameters.
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				if(str == null || count == 0)
				{
					width = 0;
					ascent = 0;
					descent = 0;
					return;
				}

				// Extract the substring to be measured.
				// TODO: make this more efficient by avoiding the data copy.
				str = str.Substring(index, count);

				// Get the font set to use to measure the string.
				IntPtr fontSet = GetFontSet(graphics.dpy);
				if(fontSet == IntPtr.Zero)
				{
					width = 0;
					ascent = 0;
					descent = 0;
					return;
				}

				// Get the text extents and decode them into useful values.
				XRectangle overall_ink;
				XRectangle overall_logical;
				try
				{
					IntPtr display = graphics.dpy.Lock();
					Xlib.XSharpTextExtentsSet
						(display, fontSet, str,
						 out overall_ink, out overall_logical);
				}
				finally
				{
					graphics.dpy.Unlock();
				}
				width = overall_logical.width;
				ascent = -(overall_logical.y);
				descent = overall_logical.height + overall_logical.y;

				// Increase the descent to account for underlining.
				// We always draw the underline on pixel below
				// the font base line.
				if((style & FontStyle.Underlined) != 0)
				{
					if(descent < 2)
					{
						descent = 2;
					}
				}
			}

	/// <summary>
	/// <para>Draw a string at a particular position on a
	/// specified graphics context.</para>
	/// </summary>
	///
	/// <param name="graphics">
	/// <para>The graphics context to draw on.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the position to start drawing text.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the position to start drawing text.</para>
	/// </param>
	///
	/// <param name="str">
	/// <para>The string to be drawn.</para>
	/// </param>
	///
	/// <param name="index">
	/// <para>The starting index in <paramref name="str"/> of the first
	/// character to be measured.</para>
	/// </param>
	///
	/// <param name="count">
	/// <para>The number of characters <paramref name="str"/>
	/// to be measured.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="graphics"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public virtual void DrawString
				(Graphics graphics, int x, int y,
				 String str, int index, int count)
			{
				// Validate the parameters.
				if(x < -32768 || x > 32767 || y < -32768 || y > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				if(str == null || count == 0)
				{
					return;
				}

				// Extract the substring to be measured.
				// TODO: make this more efficient by avoiding the data copy.
				str = str.Substring(index, count);

				// Get the font set to use for the font.
				IntPtr fontSet = GetFontSet(graphics.dpy);
				if(fontSet == IntPtr.Zero)
				{
					return;
				}

				// Draw the string using the specified font set.
				try
				{
					IntPtr display = graphics.dpy.Lock();
					Xlib.XSharpDrawStringSet
							(display, graphics.drawableHandle, graphics.gc,
							 fontSet, x, y, str, (int)style);
				}
				finally
				{
					graphics.dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get extent information for this font, when drawing
	/// onto a particular graphics context.</para>
	/// </summary>
	///
	/// <param name="graphics">
	/// <para>The graphics context to get the extent information for.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns the extent information.</para>
	/// </returns>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="graphics"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public virtual FontExtents GetFontExtents(Graphics graphics)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				FontExtents extents = null;
				GetFontSet(graphics.dpy, out extents);
				return extents;
			}

	// Normalize a point size to make it match a nearby X font size so
	// that we don't get unsightly stretching.
	internal static int NormalizePointSize(int pointSize)
			{
				// No need for normalization if using Xft
				if (Xlib.XSharpUseXft () != 0)
				{
				    return pointSize;
				}
				
				if(pointSize < 90)
				{
					return 80;
				}
				else if(pointSize < 110)
				{
					return 100;
				}
				else if(pointSize < 130)
				{
					return 120;
				}
				else if(pointSize < 160)
				{
					return 140;
				}
				else if(pointSize < 210)
				{
					return 180;
				}
				else if(pointSize <= 240)
				{
					return 240;
				}
				else
				{
					return pointSize;
				}
			}

	// Create a native "font set" structure.
	internal virtual IntPtr CreateFontSet
				(IntPtr display, out int ascent, out int descent,
				 out int maxWidth)
			{
				XRectangle max_ink;
				XRectangle max_logical;
				IntPtr fontSet;

				// Create the raw X font set structure.
				fontSet = CreateFontSetOrStruct
					(display, family,
					 NormalizePointSize(pointSize), (int)style, false);

				// Get the extent information for the font.
				Xlib.XSharpFontExtentsSet
					(fontSet, out max_ink, out max_logical);

				// Convert the extent information into values that make sense.
				ascent = -(max_logical.y);
				descent = max_logical.height + max_logical.y;
				maxWidth = max_logical.width;

				// Increase the descent to account for underlining.
				// We always draw the underline one pixel below
				// the font base line.
				if((style & FontStyle.Underlined) != 0)
				{
					if(descent < 2)
					{
						descent = 2;
					}
				}

				// Return the font set structure to the caller.
				return fontSet;
			}

	// Get the XFontSet structure for this font on a particular display.
	internal IntPtr GetFontSet(Display dpy)
			{
				FontExtents extents;
				return GetFontSet(dpy, out extents);
			}
	internal IntPtr GetFontSet(Display dpy, out FontExtents extents)
			{
				lock(typeof(Font))
				{
					// Map this object to the one that actually stores
					// the font set information.
					Font font = (Font)(dpy.fonts[this]);
					if(font == null)
					{
						font = this;
						dpy.fonts[this] = this;
					}

					// Search for existing font set information.
					FontInfo info = font.infoList;
					while(info != null)
					{
						if(info.dpy == dpy)
						{
							extents = info.extents;
							return info.fontSet;
						}
						info = info.next;
					}

					// Create a new font set.
					IntPtr fontSet;
					int ascent, descent, maxWidth;
					try
					{
						IntPtr display = dpy.Lock();
						fontSet = CreateFontSet
							(display, out ascent, out descent, out maxWidth);
						if(fontSet == IntPtr.Zero)
						{
							extents = null;
							return IntPtr.Zero;
						}
					}
					finally
					{
						dpy.Unlock();
					}

					// Associate the font set with the display.
					info = new FontInfo();
					info.next = font.infoList;
					info.extents = new FontExtents(ascent, descent, maxWidth);
					info.dpy = dpy;
					info.fontSet = fontSet;
					font.infoList = info;

					// Return the font set to the caller.
					extents = info.extents;
					return fontSet;
				}
			}

	// Free a native "font set" structure.
	internal virtual void FreeFontSet(IntPtr display, IntPtr fontSet)
			{
				Xlib.XSharpFreeFontSet(display, fontSet);
			}

	// Disassociate this font from a particular display.
	internal void Disassociate(Display dpy)
			{
				lock(typeof(Font))
				{
					FontInfo info, prev;
					info = infoList;
					prev = null;
					while(info != null && info.dpy != dpy)
					{
						prev = info;
						info = info.next;
					}
					if(info != null)
					{
						if(prev != null)
						{
							prev.next = info.next;
						}
						else
						{
							infoList = info.next;
						}
						FreeFontSet(dpy.dpy, info.fontSet);
					}
				}
			}

	// Family lists for searching for useful X fonts.  This is necessary
	// because some people deliberately uninstall the traditional X fonts,
	// even though that is a very silly thing to do.  We may need to extend
	// this list in the future.
	private static readonly String[] SansSerifFamilies =
			{"helvetica", "lucida", "luxi sans"};
	private static readonly String[] SerifFamilies =
			{"times", "luxi serif"};
	private static readonly String[] FixedFamilies =
			{"courier", "lucida console", "lucidatypewriter", "luxi mono"};

	// Comma-separated versions of the above fallbacks, for Xft.
	private const String SansSerifFallbacks =
			"helvetica,lucida,luxi sans";
	private const String SerifFallbacks =
			"times,luxi serif";
	private const String FixedFallbacks =
			"courier,lucida console,lucidatypewriter,luxi mono";


	// Create a font set or font struct, with fallback names.
	private static IntPtr CreateFontSetOrStruct
				(IntPtr display, String[] families, int pointSize,
				 int style, bool isFontStruct)
			{
				int posn;
				IntPtr fontSet;

				// Try all of the candidate fonts in order.
				for(posn = 0; posn < families.Length; ++posn)
				{
					if(isFontStruct)
					{
						fontSet = Xlib.XSharpCreateFontStruct
							(display, families[posn], pointSize, style | 0x40);
					}
					else
					{
						fontSet = Xlib.XSharpCreateFontSet
							(display, families[posn], pointSize, style | 0x40);
					}
					if(fontSet != IntPtr.Zero)
					{
						return fontSet;
					}
				}

				// Let XsharpSupport choose a default based on the first font.
				if(isFontStruct)
				{
					fontSet = Xlib.XSharpCreateFontStruct
						(display, families[0], pointSize, style);
				}
				else
				{
					fontSet = Xlib.XSharpCreateFontSet
						(display, families[0], pointSize, style);
				}
				return fontSet;
			}
	private static IntPtr CreateFontSetOrStruct
				(IntPtr display, String family, int pointSize,
				 int style, bool isFontStruct)
			{
				String fallback = GetFallbackFamily(family);
				if(fallback != null)
				{
					IntPtr fontSet = TryCreateFontSetOrStruct
						(display, family, pointSize, style, isFontStruct);
					if(fontSet != IntPtr.Zero) { return fontSet; }
					family = fallback;
				}

				String[] families = GetFallbackFamilies(family);
				if(families != null)
				{
					return CreateFontSetOrStruct
						(display, families, pointSize, style, isFontStruct);
				}
				else if(isFontStruct)
				{
					return Xlib.XSharpCreateFontStruct
						(display, family, pointSize, style);
				}
				else
				{
					return Xlib.XSharpCreateFontSet
						(display, family, pointSize, style);
				}
			}

	// Try to create a font set or struct.
	private static IntPtr TryCreateFontSetOrStruct
				(IntPtr display, String family, int pointSize,
				 int style, bool isFontStruct)
			{
				if(isFontStruct)
				{
					return Xlib.XSharpCreateFontStruct
						(display, family, pointSize, style | 0x40);
				}
				else
				{
					return Xlib.XSharpCreateFontSet
						(display, family, pointSize, style | 0x40);
				}
			}

	// Get a fallback family name for typical Windows families.
	private static String GetFallbackFamily(String name)
			{
				if(String.Compare(name, "Times", true) == 0 ||
				   String.Compare(name, "Times New Roman", true) == 0)
				{
					return Serif;
				}
				else if(String.Compare(name, "Microsoft Sans Serif", true) == 0)
				{
					return DefaultSansSerif;
				}
				else if(String.Compare(name, "Helvetica", true) == 0 ||
				        String.Compare(name, "Helv", true) == 0 ||
				        String.Compare(name, "Arial", true) == 0 ||
						(name.Length >= 6 &&
				        	String.Compare(name, 0, "Arial ", 0, 6, true) == 0))
				{
					return SansSerif;
				}
				else if(String.Compare(name, "Courier", true) == 0 ||
				        String.Compare(name, "Courier New", true) == 0)
				{
					return Fixed;
				}
				else
				{
					return null;
				}
			}

	// Get fallback families for typical X families.
	private static String[] GetFallbackFamilies(String family)
			{
				if(family == null)
				{
					return null;
				}
				else if(family == SansSerif || family == DefaultSansSerif)
				{
					return SansSerifFamilies;
				}
				else if(family == Serif)
				{
					return SerifFamilies;
				}
				else if(family == Fixed)
				{
					return FixedFamilies;
				}
				else
				{
					return null;
				}
			}

	// Get fallback families for typical X families.
	private static String GetFallbackFamiliesCSV(String family)
			{
				if(family == null)
				{
					return null;
				}
				else if(family == SansSerif || family == DefaultSansSerif)
				{
					return SansSerifFallbacks;
				}
				else if(family == Serif)
				{
					return SerifFallbacks;
				}
				else if(family == Fixed)
				{
					return FixedFallbacks;
				}
				else
				{
					return null;
				}
			}

	// Font class that uses XFontStruct values instead of XFontSet values.
	private class FontStructFont : Font
	{
		// Constructor.
		public FontStructFont(String family, int pointSize, FontStyle style)
				: base(family, pointSize, style) {}

		// Create a native "font set" structure.
		internal override IntPtr CreateFontSet
					(IntPtr display, out int ascent, out int descent,
					 out int maxWidth)
				{
					XRectangle max_ink;
					XRectangle max_logical;
					IntPtr fontSet;

					// Create the raw X font set structure.
					fontSet = CreateFontSetOrStruct
						(display, family,
						 NormalizePointSize(pointSize), (int)style, true);

					// Get the extent information for the font.
					Xlib.XSharpFontExtentsStruct
						(fontSet, out max_ink, out max_logical);

					// Convert the extent info into values that make sense.
					ascent = -(max_logical.y);
					descent = max_logical.height + max_logical.y;
					maxWidth = max_logical.width;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}

					// Return the font set structure to the caller.
					return fontSet;
				}

		// Free a native "font set" structure.
		internal override void FreeFontSet(IntPtr display, IntPtr fontSet)
				{
					Xlib.XSharpFreeFontStruct(display, fontSet);
				}

		// Override the font drawing primitives.
		public override void MeasureString
					(Graphics graphics, String str, int index, int count,
					 out int width, out int ascent, out int descent)
				{
					// Validate the parameters.
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count <= 0)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}
					if(index < 0 || index >= str.Length ||
					   count > (str.Length - index))
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Get the font set to use to measure the string.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Get the text extents and decode them into useful values.
					XRectangle overall_ink;
					XRectangle overall_logical;
					try
					{
						IntPtr display = graphics.dpy.Lock();
						Xlib.XSharpTextExtentsStruct
							(display, fontSet, str, index, count,
							 out overall_ink, out overall_logical);
					}
					finally
					{
						graphics.dpy.Unlock();
					}
					width = overall_logical.width;
					ascent = -(overall_logical.y);
					descent = overall_logical.height + overall_logical.y;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}
				}
		public override void DrawString
					(Graphics graphics, int x, int y,
					 String str, int index, int count)
				{
					// Validate the parameters.
					if(x < -32768 || x > 32767 || y < -32768 || y > 32767)
					{
						throw new XException(S._("X_PointCoordRange"));
					}
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count <= 0)
					{
						return;
					}
					if(index < 0 || index >= str.Length ||
					   count > (str.Length - index))
					{
						return;
					}

					// Get the font set to use for the font.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						return;
					}

					// Draw the string using the specified font set.
					try
					{
						IntPtr display = graphics.dpy.Lock();
						Xlib.XSharpDrawStringStruct
								(display, graphics.drawableHandle, graphics.gc,
								 fontSet, x, y, str, index, count,
								 (int)style);
					}
					finally
					{
						graphics.dpy.Unlock();
					}
				}

	} // class FontStructFont

	// Font class that uses Xft to perform the rendering operations.
	private class XftFont : Font
	{
		// Constructor.
		public XftFont(String family, int pointSize, FontStyle style)
				: base(family, pointSize, style) {}

		// Create a native "font set" structure.
		internal override IntPtr CreateFontSet
					(IntPtr display, out int ascent, out int descent,
					 out int maxWidth)
				{
					XRectangle max_ink;
					XRectangle max_logical;
					IntPtr fontSet;

					// Check for fallback families.
					String fallbacks = GetFallbackFamiliesCSV
						(GetFallbackFamily(family));

					// Create the raw X font set structure.
					fontSet = Xlib.XSharpCreateFontXft
						(display, family, fallbacks, pointSize, (int)style);

					// Get the extent information for the font.
					Xlib.XSharpFontExtentsXft
						(fontSet, out max_ink, out max_logical);

					// Convert the extent info into values that make sense.
					ascent = -(max_logical.y);
					descent = max_logical.height + max_logical.y;
					maxWidth = max_logical.width;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}

					// Return the font set structure to the caller.
					return fontSet;
				}

		// Free a native "font set" structure.
		internal override void FreeFontSet(IntPtr display, IntPtr fontSet)
				{
					Xlib.XSharpFreeFontXft(display, fontSet);
				}

		// Override the font drawing primitives.
		public override void MeasureString
					(Graphics graphics, String str, int index, int count,
					 out int width, out int ascent, out int descent)
				{
					// Validate the parameters.
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count == 0)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Extract the substring to be measured.
					// TODO: make this more efficient by avoiding the data copy.
					str = str.Substring(index, count);

					// Get the font set to use to measure the string.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Get the text extents and decode them into useful values.
					XRectangle overall_ink;
					XRectangle overall_logical;
					try
					{
						IntPtr display = graphics.dpy.Lock();
						Xlib.XSharpTextExtentsXft
							(display, fontSet, str,
							 out overall_ink, out overall_logical);
					}
					finally
					{
						graphics.dpy.Unlock();
					}
					width = overall_logical.x + overall_logical.width;
					ascent = -(overall_logical.y);
					descent = overall_logical.height + overall_logical.y;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}
				}
		public override void DrawString
					(Graphics graphics, int x, int y,
					 String str, int index, int count)
				{
					// Validate the parameters.
					if(x < -32768 || x > 32767 || y < -32768 || y > 32767)
					{
						throw new XException(S._("X_PointCoordRange"));
					}
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count == 0)
					{
						return;
					}

					// Extract the substring to be measured.
					// TODO: make this more efficient by avoiding the data copy.
					str = str.Substring(index, count);

					// Get the font set to use for the font.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						return;
					}

					// Draw the string using the specified font set.
					try
					{
						IntPtr display = graphics.dpy.Lock();
						if(graphics.clipRegion == null)
						{
							Xlib.XSharpDrawStringXft
									(display, graphics.drawableHandle, graphics.gc,
									 fontSet, x, y, str, (int)style,
									 IntPtr.Zero, graphics.Foreground.value);
						}
						else
						{
							Xlib.XSharpDrawStringXft
									(display, graphics.drawableHandle, graphics.gc,
									 fontSet, x, y, str, (int)style,
									 graphics.clipRegion.GetRegion(), graphics.Foreground.value);
						}
					}
					finally
					{
						graphics.dpy.Unlock();
					}
				}

	} // class XftFont

	// Font class that uses a client-side PCF font renderer.
	private class PCFFont : Font
	{
		// Internal state.
		private IntPtr loadedData;

		// Constructor.
		public PCFFont(String family, int pointSize, FontStyle style,
					   IntPtr loadedData)
				: base(family, pointSize, style)
				{
					this.loadedData = loadedData;
				}

		// Create a native "font set" structure.
		internal override IntPtr CreateFontSet
					(IntPtr display, out int ascent, out int descent,
					 out int maxWidth)
				{
					XRectangle max_ink;
					XRectangle max_logical;
					IntPtr fontSet;

					// Create the raw X font set structure.
					fontSet = Xlib.XSharpPCFCreate(display, loadedData);

					// Get the extent information for the font.
					Xlib.XSharpFontExtentsPCF
						(fontSet, out max_ink, out max_logical);

					// Convert the extent info into values that make sense.
					ascent = -(max_logical.y);
					descent = max_logical.height + max_logical.y;
					maxWidth = max_logical.width;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}

					// Return the font set structure to the caller.
					return fontSet;
				}

		// Free a native "font set" structure.
		internal override void FreeFontSet(IntPtr display, IntPtr fontSet)
				{
					Xlib.XSharpPCFDestroy(display, fontSet);
				}

		// Override the font drawing primitives.
		public override void MeasureString
					(Graphics graphics, String str, int index, int count,
					 out int width, out int ascent, out int descent)
				{
					// Validate the parameters.
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count <= 0)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}
					if(index < 0 || index >= str.Length ||
					   count > (str.Length - index))
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Get the font set to use to measure the string.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						width = 0;
						ascent = 0;
						descent = 0;
						return;
					}

					// Get the text extents and decode them into useful values.
					XRectangle overall_ink;
					XRectangle overall_logical;
					try
					{
						IntPtr display = graphics.dpy.Lock();
						Xlib.XSharpTextExtentsPCF
							(display, fontSet, str, index, count,
							 out overall_ink, out overall_logical);
					}
					finally
					{
						graphics.dpy.Unlock();
					}
					width = overall_logical.width;
					ascent = -(overall_logical.y);
					descent = overall_logical.height + overall_logical.y;

					// Increase the descent to account for underlining.
					// We always draw the underline one pixel below
					// the font base line.
					if((style & FontStyle.Underlined) != 0)
					{
						if(descent < 2)
						{
							descent = 2;
						}
					}
				}
		public override void DrawString
					(Graphics graphics, int x, int y,
					 String str, int index, int count)
				{
					// Validate the parameters.
					if(x < -32768 || x > 32767 || y < -32768 || y > 32767)
					{
						throw new XException(S._("X_PointCoordRange"));
					}
					if(graphics == null)
					{
						throw new ArgumentNullException("graphics");
					}
					if(str == null || count <= 0)
					{
						return;
					}
					if(index < 0 || index >= str.Length ||
					   count > (str.Length - index))
					{
						return;
					}

					// Get the font set to use for the font.
					IntPtr fontSet = GetFontSet(graphics.dpy);
					if(fontSet == IntPtr.Zero)
					{
						return;
					}

					// Draw the string using the specified font set.
					try
					{
						IntPtr display = graphics.dpy.Lock();
						Xlib.XSharpDrawStringPCF
								(display, graphics.drawableHandle, graphics.gc,
								 fontSet, x, y, str, index, count,
								 (int)style);
					}
					finally
					{
						graphics.dpy.Unlock();
					}
				}

	} // class PCFFont

}; // class Font

}; // namespace Xsharp
