/*
 * ColorConverter.cs - Implementation of the
 *			"System.Drawing.Printing.ColorConverter" class.
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

#if CONFIG_COMPONENT_MODEL

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections;

public class ColorConverter : TypeConverter
{
	// Constructor.
	public ColorConverter() {}

	// Determine if we can convert from a given type to "Color".
	public override bool CanConvertFrom
				(ITypeDescriptorContext context, Type sourceType)
			{
				if(sourceType == typeof(String))
				{
					return true;
				}
				else
				{
					return base.CanConvertFrom(context, sourceType);
				}
			}

	// Determine if we can convert to a given type from "Color".
	public override bool CanConvertTo
				(ITypeDescriptorContext context, Type destinationType)
			{
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(destinationType == typeof(InstanceDescriptor))
				{
					return true;
				}
				else
			#endif
				{
					return base.CanConvertTo(context, destinationType);
				}
			}

	// Parse a number from a string.
	private static int ParseNumber(String str, ref int posn,
								   bool needComma, int invalid)
			{
				int value;
				char ch;

				// Process the comma between components if necessary.
				if(needComma)
				{
					while(posn < str.Length && Char.IsWhiteSpace(str[posn]))
					{
						++posn;
					}
					if(posn >= str.Length)
					{
						// There are no more components in the string.
						return -1;
					}
					if(str[posn] != ',')
					{
						return invalid;
					}
					while(posn < str.Length && Char.IsWhiteSpace(str[posn]))
					{
						++posn;
					}
					if(posn >= str.Length)
					{
						return invalid;
					}
				}

				// Extract the number and parse it.
				if(posn < (str.Length - 1) && str[posn] == '0' &&
				   (str[posn + 1] == 'x' || str[posn + 1] == 'X'))
				{
					// Parse a hexadecimal constant.
					posn += 2;
					if(posn >= str.Length)
					{
						return invalid;
					}
					value = 0;
					while(posn < str.Length)
					{
						ch = str[posn];
						if(ch >= '0' && ch <= '9')
						{
							value = value * 16 + (int)(ch - '0');
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							value = value * 16 + (int)(ch - 'A' + 10);
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							value = value * 16 + (int)(ch - 'a' + 10);
						}
						else
						{
							break;
						}
						++posn;
					}
					return value;
				}
				else if(posn < str.Length &&
						str[posn] >= '0' && str[posn] <= '9')
				{
					// Parse a decimal constant.
					value = (int)(str[posn] - '0');
					++posn;
					while(posn < str.Length &&
						  str[posn] >= '0' && str[posn] <= '9')
					{
						value = value * 10 + (int)(str[posn] - '0');
						++posn;
					}
					return value;
				}
				else
				{
					// Don't know what this is.
					return invalid;
				}
			}

	// Convert from a source type to "Color".
	public override Object ConvertFrom
				(ITypeDescriptorContext context,
				 CultureInfo culture, Object value)
			{
				// Pass control to the base class if we weren't given a string.
				if(!(value is String))
				{
					return base.ConvertFrom(context, culture, value);
				}

				// Extract the string and trim it.
				String str = ((String)value).Trim();
				if(str.Length == 0)
				{
					return Color.Empty;
				}

				// Try parsing as a named color.
				Color color = Color.FromName(str);
				if(!(color.IsEmpty))
				{
					return color;
				}

				if(str[0] == '#' && str.Length == 7)
				{
					// Web color 
					uint val = UInt32.Parse(str.Substring(1), NumberStyles.HexNumber);
					val |= 0xFF000000;
					return Color.FromArgb((int)val);
				}

				// Parse "[A,] R, G, B" components from the string.
				int[] numbers = new int [4];
				int posn = 0;
				numbers[0] = ParseNumber(str, ref posn, false, 256);
				numbers[1] = ParseNumber(str, ref posn, true, 256);
				numbers[2] = ParseNumber(str, ref posn, true, 256);
				numbers[3] = ParseNumber(str, ref posn, true, 256);
				if(numbers[0] == -1 || numbers[1] == -1 || numbers[2] == -1 ||
				   numbers[0] >= 256 || numbers[1] >= 256 ||
				   numbers[2] >= 256 || numbers[3] >= 256 ||
				   posn < str.Length)
				{
					throw new ArgumentException(S._("Arg_InvalidColor"));
				}
				if(numbers[3] == -1)
				{
					return Color.FromArgb((byte)(numbers[0]),
										  (byte)(numbers[1]),
										  (byte)(numbers[2]));
				}
				else
				{
					return Color.FromArgb((byte)(numbers[0]),
										  (byte)(numbers[1]),
										  (byte)(numbers[2]),
										  (byte)(numbers[3]));
				}
			}

	// Convert from "Color" to a destination type.
	[TODO]
	public override Object ConvertTo
				(ITypeDescriptorContext context,
				 CultureInfo culture, Object value,
				 Type destinationType)
			{
				Color color = (Color)value;
				if(destinationType == typeof(String))
				{
					if(color.IsKnownColor)
					{
						return color.ToKnownColor().ToString();
					}
					else if(color.A == 0xFF)
					{
						return String.Format("{0}, {1}, {2}",
											 (int)(color.R),
											 (int)(color.G),
											 (int)(color.B));
					}
					else
					{
						return String.Format("{0}, {1}, {2}, {3}",
											 (int)(color.A),
											 (int)(color.R),
											 (int)(color.G),
											 (int)(color.B));
					}
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				else if(destinationType == typeof(InstanceDescriptor))
				{
					// TODO
					return null;
				}
			#endif
				else
				{
					return base.ConvertTo
						(context, culture, value, destinationType);
				}
			}

	// Return a collection of standard values for this data type.
	[TODO]
	public override TypeConverter.StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				// TODO
				return null;
			}

	// Determine if "GetStandardValues" is supported.
	public override bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

}; // class ColorConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.Drawing
