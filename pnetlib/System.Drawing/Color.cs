/*
 * Color.cs - Implementation of the "System.Drawing.Color" class.
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

using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Toolkit;
using System.Text;

#if !ECMA_COMPAT
[Serializable]
[ComVisible(true)]
#endif
#if CONFIG_COMPONENT_MODEL_DESIGN
[TypeConverter(typeof(ColorConverter))]
[Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design",
		typeof(UITypeEditor))]
#endif
public struct Color
{
	// The empty color.
	public static readonly Color Empty;

	// Internal state.
	private uint value;
	private short knownColor;
	private bool resolved;

	// Constructors.
	internal Color(KnownColor knownColor)
			{
				this.value = 0;
				this.knownColor = (short)knownColor;
				this.resolved = false;
			}
	private Color(uint value)
			{
				this.value = value;
				this.knownColor = (short)0;
				this.resolved = true;
			}

	// Get the color components.
	public byte R
			{
				get
				{
					if(!resolved)
					{
						Resolve();
					}
					return (byte)(value >> 16);
				}
			}
	public byte G
			{
				get
				{
					if(!resolved)
					{
						Resolve();
					}
					return (byte)(value >> 8);
				}
			}
	public byte B
			{
				get
				{
					if(!resolved)
					{
						Resolve();
					}
					return (byte)value;
				}
			}
	public byte A
			{
				get
				{
					if(!resolved)
					{
						Resolve();
					}
					return (byte)(value >> 24);
				}
			}

	// Determine if this is the empty color value.
	public bool IsEmpty
			{
				get
				{
					return (knownColor == 0 && !resolved);
				}
			}

	// Determine if this is a known color value.
	public bool IsKnownColor
			{
				get
				{
					return (knownColor != 0);
				}
			}

	// Determine if this is a named color value.
	public bool IsNamedColor
			{
				get
				{
					// Because we only support known color names,
					// this is identical in behaviour to "IsKnownColor".
					return (knownColor != 0);
				}
			}

	// Determine if this is a system color value.
	public bool IsSystemColor
			{
				get
				{
					return (knownColor >= (int)(KnownColor.ActiveBorder) &&
							knownColor <= (int)(KnownColor.WindowText));
				}
			}

	// Get the color's name.
	public String Name
			{
				get
				{
					if(knownColor != 0)
					{
						return ((KnownColor)knownColor).ToString();
					}
					else
					{
						return String.Format("{0:x}", value);
					}
				}
			}

	// Determine if this object is equal to another.
	public override bool Equals(Object obj)
			{
				if(obj is Color)
				{
					Color other = (Color)obj;
					if(other.knownColor != 0)
					{
						return (other.knownColor == knownColor);
					}
					else if(knownColor != 0)
					{
						return false;
					}
					else
					{
						return (other.value == value);
					}
				}
				else
				{
					return false;
				}
			}

	// Convert an ARGB value into a color.
	public static Color FromArgb(int argb)
			{
				return new Color((uint)argb);
			}
	public static Color FromArgb(int alpha, Color baseColor)
			{
				if(alpha < 0 || alpha > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "alpha");
				}
				if(!(baseColor.resolved))
				{
					baseColor.Resolve();
				}
				return new Color((baseColor.value & 0x00FFFFFF) |
								 (uint)(alpha << 24));
			}
	public static Color FromArgb(int red, int green, int blue)
			{
				if(red < 0 || red > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "red");
				}
				if(green < 0 || green > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "green");
				}
				if(blue < 0 || blue > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "blue");
				}
				return new Color(((uint)((red << 16) | (green << 8) | blue)) |
								 0xFF000000);
			}
	public static Color FromArgb(int alpha, int red, int green, int blue)
			{
				if(alpha < 0 || alpha > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "alpha");
				}
				if(red < 0 || red > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "red");
				}
				if(green < 0 || green > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "green");
				}
				if(blue < 0 || blue > 255)
				{
					throw new ArgumentException
						(S._("Arg_ColorComponent"), "blue");
				}
				return new Color((uint)((alpha << 24) | (red << 16) |
									    (green << 8) | blue));
			}

	// Convert a known color value into a color value.
	public static Color FromKnownColor(KnownColor color)
			{
				return new Color(color);
			}

	// Convert a color name into a color value.
	public static Color FromName(String name)
			{
				try
				{
					KnownColor value;
					value = (KnownColor)
						(Enum.Parse(typeof(KnownColor), name, true));
					return new Color(value);
				}
				catch(ArgumentException)
				{
					// Unknown color name.
					return Empty;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				if(!resolved)
				{
					Resolve();
				}
				return (int)value;
			}

	// Get the HSV values from this color.
	public float GetHue()
			{
				// Calculate the minimum and maximum components.
				int red = R;
				int green = G;
				int blue = B;
				if (red == green && green == blue)
					return 0.0f;
				int min, max;
				min = red;
				if(min > green)
				{
					min = green;
				}
				if(min > blue)
				{
					min = blue;
				}
				max = red;
				if(max < green)
				{
					max = green;
				}
				if(max < blue)
				{
					max = blue;
				}
				
				float hue;
				if(red == max)
				{
					hue = (float)(green - blue) / (float)(max - min);
				}
				else if(green == max)
				{
					hue = 2.0f + (float)(blue - red) / (float)(max - min);
				}
				else
				{
					hue = 4.0f + (float)(red - green) / (float)(max - min);
				}
				hue *= 60.0f;
				if(hue < 0.0f)
				{
					hue += 360.0f;
				}
				return hue;
			}
	public float GetSaturation()
			{
				int red = R;
				int green = G;
				int blue = B;
				int min, max;
				min = red;
				if(min > green)
				{
					min = green;
				}
				if(min > blue)
				{
					min = blue;
				}
				max = red;
				if(max < green)
				{
					max = green;
				}
				if(max < blue)
				{
					max = blue;
				}
				if(max == min)
				{
					return 0.0f;
				}
				else
				{
					if (max + min < 256)
						return (float)(max - min)/(float)(max + min);
					else
						return (float)(max - min) / (float)( 510 - max - min);
				}
			}
	public float GetBrightness()
			{
				// The brightness is the average of the maximum and minimum of the
				// components.
				int red = R;
				int min  = R;
				int green = G;
				int blue = B;
				if(red < green)
				{
					red = green;
				}
				if(red < blue)
				{
					red = blue;
				}
				if (min > green)
				{
					min = green;
				}
				if (min > blue)
				{
					min = blue;
				}
				return (float)(red + min) / 510.0f;
			}

	// Get the ARGB value for a color.
	public int ToArgb()
			{
				if(!resolved)
				{
					Resolve();
				}
				return (int)value;
			}

	// Get the known color value within this structure.
	public KnownColor ToKnownColor()
			{
				return (KnownColor)knownColor;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("Color [");
				if(knownColor != 0)
				{
					builder.Append(((KnownColor)knownColor).ToString());
				}
				else
				{
					builder.Append("A=");
					builder.Append(A);
					builder.Append(", R=");
					builder.Append(R);
					builder.Append(", G=");
					builder.Append(G);
					builder.Append(", B=");
					builder.Append(B);
				}
				builder.Append(']');
				return builder.ToString();
			}

	// Equality and inequality testing.
	public static bool operator==(Color left, Color right)
			{
				return left.Equals(right);
			}
	public static bool operator!=(Color left, Color right)
			{
				return !(left.Equals(right));
			}

	// Resolve a color to its ARGB value.
	private void Resolve()
			{
				KnownColor known = (KnownColor)knownColor;
				if(known >= KnownColor.ActiveBorder &&
				   known <= KnownColor.WindowText)
				{
					int rgb = ToolkitManager.Toolkit.ResolveSystemColor(known);
					if(rgb != -1)
					{
						value = ((uint)rgb) | ((uint)0xFF000000);
						// We don't set "resolved" to true, because the system
						// color may change between now and the next call.
					}
					else
					{
						value = specificColors[((int)known)];
						resolved = true;
					}
				}
				else if(known >= KnownColor.Transparent &&
						known <= KnownColor.YellowGreen)
				{
					// A known color that has a fixed value.
					value = specificColors[((int)known)];
					resolved = true;
				}
			}

	// Builtin color table.
	private static readonly uint[] specificColors = {
		// Placeholder for KnownColor value of zero.
		0x00000000,

		// Special colors - may be overridden by profiles.
		0xFFD4D0C8, 
		0xFF0A246A, 
		0xFFFFFFFF, 
		0xFF808080, 
		0xFFD4D0C8, 
		0xFF808080, 
		0xFF404040, 
		0xFFD4D0C8, 
		0xFFFFFFFF, 
		0xFF000000, 
		0xFF3A6EA5, 
		0xFF808080, 
		0xFF0A246A, 
		0xFFFFFFFF, 
		0xFF0000FF, 
		0xFFD4D0C8, 
		0xFF808080, 
		0xFFD4D0C8, 
		0xFFFFFFE1, 
		0xFF000000, 
		0xFFD4D0C8, 
		0xFF000000, 
		0xFFD4D0C8, 
		0xFFFFFFFF, 
		0xFF000000, 
		0xFF000000, 

		// Specific colors with fixed values.
		0x00FFFFFF, 
		0xFFF0F8FF, 
		0xFFFAEBD7, 
		0xFF00FFFF, 
		0xFF7FFFD4, 
		0xFFF0FFFF, 
		0xFFF5F5DC, 
		0xFFFFE4C4, 
		0xFF000000, 
		0xFFFFEBCD, 
		0xFF0000FF, 
		0xFF8A2BE2, 
		0xFFA52A2A, 
		0xFFDEB887, 
		0xFF5F9EA0, 
		0xFF7FFF00, 
		0xFFD2691E, 
		0xFFFF7F50, 
		0xFF6495ED, 
		0xFFFFF8DC, 
		0xFFDC143C, 
		0xFF00FFFF, 
		0xFF00008B, 
		0xFF008B8B, 
		0xFFB8860B, 
		0xFFA9A9A9, 
		0xFF006400, 
		0xFFBDB76B, 
		0xFF8B008B, 
		0xFF556B2F, 
		0xFFFF8C00, 
		0xFF9932CC, 
		0xFF8B0000, 
		0xFFE9967A, 
		0xFF8FBC8B, 
		0xFF483D8B, 
		0xFF2F4F4F, 
		0xFF00CED1, 
		0xFF9400D3, 
		0xFFFF1493, 
		0xFF00BFFF, 
		0xFF696969, 
		0xFF1E90FF, 
		0xFFB22222, 
		0xFFFFFAF0, 
		0xFF228B22, 
		0xFFFF00FF, 
		0xFFDCDCDC, 
		0xFFF8F8FF, 
		0xFFFFD700, 
		0xFFDAA520, 
		0xFF808080, 
		0xFF008000, 
		0xFFADFF2F, 
		0xFFF0FFF0, 
		0xFFFF69B4, 
		0xFFCD5C5C, 
		0xFF4B0082, 
		0xFFFFFFF0, 
		0xFFF0E68C, 
		0xFFE6E6FA, 
		0xFFFFF0F5, 
		0xFF7CFC00, 
		0xFFFFFACD, 
		0xFFADD8E6, 
		0xFFF08080, 
		0xFFE0FFFF, 
		0xFFFAFAD2, 
		0xFFD3D3D3, 
		0xFF90EE90, 
		0xFFFFB6C1, 
		0xFFFFA07A, 
		0xFF20B2AA, 
		0xFF87CEFA, 
		0xFF778899, 
		0xFFB0C4DE, 
		0xFFFFFFE0, 
		0xFF00FF00, 
		0xFF32CD32, 
		0xFFFAF0E6, 
		0xFFFF00FF, 
		0xFF800000, 
		0xFF66CDAA, 
		0xFF0000CD, 
		0xFFBA55D3, 
		0xFF9370DB, 
		0xFF3CB371, 
		0xFF7B68EE, 
		0xFF00FA9A, 
		0xFF48D1CC, 
		0xFFC71585, 
		0xFF191970, 
		0xFFF5FFFA, 
		0xFFFFE4E1, 
		0xFFFFE4B5, 
		0xFFFFDEAD, 
		0xFF000080, 
		0xFFFDF5E6, 
		0xFF808000, 
		0xFF6B8E23, 
		0xFFFFA500, 
		0xFFFF4500, 
		0xFFDA70D6, 
		0xFFEEE8AA, 
		0xFF98FB98, 
		0xFFAFEEEE, 
		0xFFDB7093, 
		0xFFFFEFD5, 
		0xFFFFDAB9, 
		0xFFCD853F, 
		0xFFFFC0CB, 
		0xFFDDA0DD, 
		0xFFB0E0E6, 
		0xFF800080, 
		0xFFFF0000, 
		0xFFBC8F8F, 
		0xFF4169E1, 
		0xFF8B4513, 
		0xFFFA8072, 
		0xFFF4A460, 
		0xFF2E8B57, 
		0xFFFFF5EE, 
		0xFFA0522D, 
		0xFFC0C0C0, 
		0xFF87CEEB, 
		0xFF6A5ACD, 
		0xFF708090, 
		0xFFFFFAFA, 
		0xFF00FF7F, 
		0xFF4682B4, 
		0xFFD2B48C, 
		0xFF008080, 
		0xFFD8BFD8, 
		0xFFFF6347, 
		0xFF40E0D0, 
		0xFFEE82EE, 
		0xFFF5DEB3, 
		0xFFFFFFFF, 
		0xFFF5F5F5, 
		0xFFFFFF00, 
		0xFF9ACD32, 
	};

	// Pre-defined colors as static properties.
	public static Color Transparent
			{
				get
				{
					return new Color(KnownColor.Transparent);
				}
			}
	public static Color AliceBlue
			{
				get
				{
					return new Color(KnownColor.AliceBlue);
				}
			}
	public static Color AntiqueWhite
			{
				get
				{
					return new Color(KnownColor.AntiqueWhite);
				}
			}
	public static Color Aqua
			{
				get
				{
					return new Color(KnownColor.Aqua);
				}
			}
	public static Color Aquamarine
			{
				get
				{
					return new Color(KnownColor.Aquamarine);
				}
			}
	public static Color Azure
			{
				get
				{
					return new Color(KnownColor.Azure);
				}
			}
	public static Color Beige
			{
				get
				{
					return new Color(KnownColor.Beige);
				}
			}
	public static Color Bisque
			{
				get
				{
					return new Color(KnownColor.Bisque);
				}
			}
	public static Color Black
			{
				get
				{
					return new Color(KnownColor.Black);
				}
			}
	public static Color BlanchedAlmond
			{
				get
				{
					return new Color(KnownColor.BlanchedAlmond);
				}
			}
	public static Color Blue
			{
				get
				{
					return new Color(KnownColor.Blue);
				}
			}
	public static Color BlueViolet
			{
				get
				{
					return new Color(KnownColor.BlueViolet);
				}
			}
	public static Color Brown
			{
				get
				{
					return new Color(KnownColor.Brown);
				}
			}
	public static Color BurlyWood
			{
				get
				{
					return new Color(KnownColor.BurlyWood);
				}
			}
	public static Color CadetBlue
			{
				get
				{
					return new Color(KnownColor.CadetBlue);
				}
			}
	public static Color Chartreuse
			{
				get
				{
					return new Color(KnownColor.Chartreuse);
				}
			}
	public static Color Chocolate
			{
				get
				{
					return new Color(KnownColor.Chocolate);
				}
			}
	public static Color Coral
			{
				get
				{
					return new Color(KnownColor.Coral);
				}
			}
	public static Color CornflowerBlue
			{
				get
				{
					return new Color(KnownColor.CornflowerBlue);
				}
			}
	public static Color Cornsilk
			{
				get
				{
					return new Color(KnownColor.Cornsilk);
				}
			}
	public static Color Crimson
			{
				get
				{
					return new Color(KnownColor.Crimson);
				}
			}
	public static Color Cyan
			{
				get
				{
					return new Color(KnownColor.Cyan);
				}
			}
	public static Color DarkBlue
			{
				get
				{
					return new Color(KnownColor.DarkBlue);
				}
			}
	public static Color DarkCyan
			{
				get
				{
					return new Color(KnownColor.DarkCyan);
				}
			}
	public static Color DarkGoldenrod
			{
				get
				{
					return new Color(KnownColor.DarkGoldenrod);
				}
			}
	public static Color DarkGray
			{
				get
				{
					return new Color(KnownColor.DarkGray);
				}
			}
	public static Color DarkGreen
			{
				get
				{
					return new Color(KnownColor.DarkGreen);
				}
			}
	public static Color DarkKhaki
			{
				get
				{
					return new Color(KnownColor.DarkKhaki);
				}
			}
	public static Color DarkMagenta
			{
				get
				{
					return new Color(KnownColor.DarkMagenta);
				}
			}
	public static Color DarkOliveGreen
			{
				get
				{
					return new Color(KnownColor.DarkOliveGreen);
				}
			}
	public static Color DarkOrange
			{
				get
				{
					return new Color(KnownColor.DarkOrange);
				}
			}
	public static Color DarkOrchid
			{
				get
				{
					return new Color(KnownColor.DarkOrchid);
				}
			}
	public static Color DarkRed
			{
				get
				{
					return new Color(KnownColor.DarkRed);
				}
			}
	public static Color DarkSalmon
			{
				get
				{
					return new Color(KnownColor.DarkSalmon);
				}
			}
	public static Color DarkSeaGreen
			{
				get
				{
					return new Color(KnownColor.DarkSeaGreen);
				}
			}
	public static Color DarkSlateBlue
			{
				get
				{
					return new Color(KnownColor.DarkSlateBlue);
				}
			}
	public static Color DarkSlateGray
			{
				get
				{
					return new Color(KnownColor.DarkSlateGray);
				}
			}
	public static Color DarkTurquoise
			{
				get
				{
					return new Color(KnownColor.DarkTurquoise);
				}
			}
	public static Color DarkViolet
			{
				get
				{
					return new Color(KnownColor.DarkViolet);
				}
			}
	public static Color DeepPink
			{
				get
				{
					return new Color(KnownColor.DeepPink);
				}
			}
	public static Color DeepSkyBlue
			{
				get
				{
					return new Color(KnownColor.DeepSkyBlue);
				}
			}
	public static Color DimGray
			{
				get
				{
					return new Color(KnownColor.DimGray);
				}
			}
	public static Color DodgerBlue
			{
				get
				{
					return new Color(KnownColor.DodgerBlue);
				}
			}
	public static Color Firebrick
			{
				get
				{
					return new Color(KnownColor.Firebrick);
				}
			}
	public static Color FloralWhite
			{
				get
				{
					return new Color(KnownColor.FloralWhite);
				}
			}
	public static Color ForestGreen
			{
				get
				{
					return new Color(KnownColor.ForestGreen);
				}
			}
	public static Color Fuchsia
			{
				get
				{
					return new Color(KnownColor.Fuchsia);
				}
			}
	public static Color Gainsboro
			{
				get
				{
					return new Color(KnownColor.Gainsboro);
				}
			}
	public static Color GhostWhite
			{
				get
				{
					return new Color(KnownColor.GhostWhite);
				}
			}
	public static Color Gold
			{
				get
				{
					return new Color(KnownColor.Gold);
				}
			}
	public static Color Goldenrod
			{
				get
				{
					return new Color(KnownColor.Goldenrod);
				}
			}
	public static Color Gray
			{
				get
				{
					return new Color(KnownColor.Gray);
				}
			}
	public static Color Green
			{
				get
				{
					return new Color(KnownColor.Green);
				}
			}
	public static Color GreenYellow
			{
				get
				{
					return new Color(KnownColor.GreenYellow);
				}
			}
	public static Color Honeydew
			{
				get
				{
					return new Color(KnownColor.Honeydew);
				}
			}
	public static Color HotPink
			{
				get
				{
					return new Color(KnownColor.HotPink);
				}
			}
	public static Color IndianRed
			{
				get
				{
					return new Color(KnownColor.IndianRed);
				}
			}
	public static Color Indigo
			{
				get
				{
					return new Color(KnownColor.Indigo);
				}
			}
	public static Color Ivory
			{
				get
				{
					return new Color(KnownColor.Ivory);
				}
			}
	public static Color Khaki
			{
				get
				{
					return new Color(KnownColor.Khaki);
				}
			}
	public static Color Lavender
			{
				get
				{
					return new Color(KnownColor.Lavender);
				}
			}
	public static Color LavenderBlush
			{
				get
				{
					return new Color(KnownColor.LavenderBlush);
				}
			}
	public static Color LawnGreen
			{
				get
				{
					return new Color(KnownColor.LawnGreen);
				}
			}
	public static Color LemonChiffon
			{
				get
				{
					return new Color(KnownColor.LemonChiffon);
				}
			}
	public static Color LightBlue
			{
				get
				{
					return new Color(KnownColor.LightBlue);
				}
			}
	public static Color LightCoral
			{
				get
				{
					return new Color(KnownColor.LightCoral);
				}
			}
	public static Color LightCyan
			{
				get
				{
					return new Color(KnownColor.LightCyan);
				}
			}
	public static Color LightGoldenrodYellow
			{
				get
				{
					return new Color(KnownColor.LightGoldenrodYellow);
				}
			}
	public static Color LightGray
			{
				get
				{
					return new Color(KnownColor.LightGray);
				}
			}
	public static Color LightGreen
			{
				get
				{
					return new Color(KnownColor.LightGreen);
				}
			}
	public static Color LightPink
			{
				get
				{
					return new Color(KnownColor.LightPink);
				}
			}
	public static Color LightSalmon
			{
				get
				{
					return new Color(KnownColor.LightSalmon);
				}
			}
	public static Color LightSeaGreen
			{
				get
				{
					return new Color(KnownColor.LightSeaGreen);
				}
			}
	public static Color LightSkyBlue
			{
				get
				{
					return new Color(KnownColor.LightSkyBlue);
				}
			}
	public static Color LightSlateGray
			{
				get
				{
					return new Color(KnownColor.LightSlateGray);
				}
			}
	public static Color LightSteelBlue
			{
				get
				{
					return new Color(KnownColor.LightSteelBlue);
				}
			}
	public static Color LightYellow
			{
				get
				{
					return new Color(KnownColor.LightYellow);
				}
			}
	public static Color Lime
			{
				get
				{
					return new Color(KnownColor.Lime);
				}
			}
	public static Color LimeGreen
			{
				get
				{
					return new Color(KnownColor.LimeGreen);
				}
			}
	public static Color Linen
			{
				get
				{
					return new Color(KnownColor.Linen);
				}
			}
	public static Color Magenta
			{
				get
				{
					return new Color(KnownColor.Magenta);
				}
			}
	public static Color Maroon
			{
				get
				{
					return new Color(KnownColor.Maroon);
				}
			}
	public static Color MediumAquamarine
			{
				get
				{
					return new Color(KnownColor.MediumAquamarine);
				}
			}
	public static Color MediumBlue
			{
				get
				{
					return new Color(KnownColor.MediumBlue);
				}
			}
	public static Color MediumOrchid
			{
				get
				{
					return new Color(KnownColor.MediumOrchid);
				}
			}
	public static Color MediumPurple
			{
				get
				{
					return new Color(KnownColor.MediumPurple);
				}
			}
	public static Color MediumSeaGreen
			{
				get
				{
					return new Color(KnownColor.MediumSeaGreen);
				}
			}
	public static Color MediumSlateBlue
			{
				get
				{
					return new Color(KnownColor.MediumSlateBlue);
				}
			}
	public static Color MediumSpringGreen
			{
				get
				{
					return new Color(KnownColor.MediumSpringGreen);
				}
			}
	public static Color MediumTurquoise
			{
				get
				{
					return new Color(KnownColor.MediumTurquoise);
				}
			}
	public static Color MediumVioletRed
			{
				get
				{
					return new Color(KnownColor.MediumVioletRed);
				}
			}
	public static Color MidnightBlue
			{
				get
				{
					return new Color(KnownColor.MidnightBlue);
				}
			}
	public static Color MintCream
			{
				get
				{
					return new Color(KnownColor.MintCream);
				}
			}
	public static Color MistyRose
			{
				get
				{
					return new Color(KnownColor.MistyRose);
				}
			}
	public static Color Moccasin
			{
				get
				{
					return new Color(KnownColor.Moccasin);
				}
			}
	public static Color NavajoWhite
			{
				get
				{
					return new Color(KnownColor.NavajoWhite);
				}
			}
	public static Color Navy
			{
				get
				{
					return new Color(KnownColor.Navy);
				}
			}
	public static Color OldLace
			{
				get
				{
					return new Color(KnownColor.OldLace);
				}
			}
	public static Color Olive
			{
				get
				{
					return new Color(KnownColor.Olive);
				}
			}
	public static Color OliveDrab
			{
				get
				{
					return new Color(KnownColor.OliveDrab);
				}
			}
	public static Color Orange
			{
				get
				{
					return new Color(KnownColor.Orange);
				}
			}
	public static Color OrangeRed
			{
				get
				{
					return new Color(KnownColor.OrangeRed);
				}
			}
	public static Color Orchid
			{
				get
				{
					return new Color(KnownColor.Orchid);
				}
			}
	public static Color PaleGoldenrod
			{
				get
				{
					return new Color(KnownColor.PaleGoldenrod);
				}
			}
	public static Color PaleGreen
			{
				get
				{
					return new Color(KnownColor.PaleGreen);
				}
			}
	public static Color PaleTurquoise
			{
				get
				{
					return new Color(KnownColor.PaleTurquoise);
				}
			}
	public static Color PaleVioletRed
			{
				get
				{
					return new Color(KnownColor.PaleVioletRed);
				}
			}
	public static Color PapayaWhip
			{
				get
				{
					return new Color(KnownColor.PapayaWhip);
				}
			}
	public static Color PeachPuff
			{
				get
				{
					return new Color(KnownColor.PeachPuff);
				}
			}
	public static Color Peru
			{
				get
				{
					return new Color(KnownColor.Peru);
				}
			}
	public static Color Pink
			{
				get
				{
					return new Color(KnownColor.Pink);
				}
			}
	public static Color Plum
			{
				get
				{
					return new Color(KnownColor.Plum);
				}
			}
	public static Color PowderBlue
			{
				get
				{
					return new Color(KnownColor.PowderBlue);
				}
			}
	public static Color Purple
			{
				get
				{
					return new Color(KnownColor.Purple);
				}
			}
	public static Color Red
			{
				get
				{
					return new Color(KnownColor.Red);
				}
			}
	public static Color RosyBrown
			{
				get
				{
					return new Color(KnownColor.RosyBrown);
				}
			}
	public static Color RoyalBlue
			{
				get
				{
					return new Color(KnownColor.RoyalBlue);
				}
			}
	public static Color SaddleBrown
			{
				get
				{
					return new Color(KnownColor.SaddleBrown);
				}
			}
	public static Color Salmon
			{
				get
				{
					return new Color(KnownColor.Salmon);
				}
			}
	public static Color SandyBrown
			{
				get
				{
					return new Color(KnownColor.SandyBrown);
				}
			}
	public static Color SeaGreen
			{
				get
				{
					return new Color(KnownColor.SeaGreen);
				}
			}
	public static Color SeaShell
			{
				get
				{
					return new Color(KnownColor.SeaShell);
				}
			}
	public static Color Sienna
			{
				get
				{
					return new Color(KnownColor.Sienna);
				}
			}
	public static Color Silver
			{
				get
				{
					return new Color(KnownColor.Silver);
				}
			}
	public static Color SkyBlue
			{
				get
				{
					return new Color(KnownColor.SkyBlue);
				}
			}
	public static Color SlateBlue
			{
				get
				{
					return new Color(KnownColor.SlateBlue);
				}
			}
	public static Color SlateGray
			{
				get
				{
					return new Color(KnownColor.SlateGray);
				}
			}
	public static Color Snow
			{
				get
				{
					return new Color(KnownColor.Snow);
				}
			}
	public static Color SpringGreen
			{
				get
				{
					return new Color(KnownColor.SpringGreen);
				}
			}
	public static Color SteelBlue
			{
				get
				{
					return new Color(KnownColor.SteelBlue);
				}
			}
	public static Color Tan
			{
				get
				{
					return new Color(KnownColor.Tan);
				}
			}
	public static Color Teal
			{
				get
				{
					return new Color(KnownColor.Teal);
				}
			}
	public static Color Thistle
			{
				get
				{
					return new Color(KnownColor.Thistle);
				}
			}
	public static Color Tomato
			{
				get
				{
					return new Color(KnownColor.Tomato);
				}
			}
	public static Color Turquoise
			{
				get
				{
					return new Color(KnownColor.Turquoise);
				}
			}
	public static Color Violet
			{
				get
				{
					return new Color(KnownColor.Violet);
				}
			}
	public static Color Wheat
			{
				get
				{
					return new Color(KnownColor.Wheat);
				}
			}
	public static Color White
			{
				get
				{
					return new Color(KnownColor.White);
				}
			}
	public static Color WhiteSmoke
			{
				get
				{
					return new Color(KnownColor.WhiteSmoke);
				}
			}
	public static Color Yellow
			{
				get
				{
					return new Color(KnownColor.Yellow);
				}
			}
	public static Color YellowGreen
			{
				get
				{
					return new Color(KnownColor.YellowGreen);
				}
			}

}; // struct Color
		
}; // namespace System.Drawing
