/*
 * StringFormat.cs - Implementation of the "System.Drawing.StringFormat" class.
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

using System.Drawing.Toolkit;
using System.Drawing.Text;

public sealed class StringFormat
	: MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	private StringFormatFlags options;
	private int language;
	private StringAlignment alignment;
	private StringDigitSubstitute digitMethod;
	private HotkeyPrefix hotkeyPrefix;
	private StringAlignment lineAlignment;
	private StringTrimming trimming;
	internal CharacterRange[] ranges;
	private float firstTabOffset;
	private float[] tabStops;
	internal bool IsTypographic;

	// Constructors.
	public StringFormat()
			{
				this.trimming = StringTrimming.Character;
			}
	public StringFormat(StringFormat format)
			{
				if(format == null)
				{
					throw new ArgumentNullException("format");
				}
				this.options = format.options;
				this.language = format.language;
				this.alignment = format.alignment;
				this.digitMethod = format.digitMethod;
				this.hotkeyPrefix = format.hotkeyPrefix;
				this.lineAlignment = format.lineAlignment;
				this.trimming = format.trimming;
				this.ranges = format.ranges;
				this.firstTabOffset = format.firstTabOffset;
				this.tabStops = format.tabStops;
				this.IsTypographic = format.IsTypographic;
			}
	public StringFormat(StringFormatFlags options)
			{
				this.options = options;
				this.trimming = StringTrimming.Character;
				
			}
	public StringFormat(StringFormatFlags options, int language)
			{
				this.options = options;
				this.language = language;
				this.trimming = StringTrimming.Character;
			}
	private StringFormat(bool typographic)
			{
				if(typographic)
				{
					this.options = (StringFormatFlags.LineLimit |
					                StringFormatFlags.NoClip);
					this.IsTypographic = true;
				}
				else
				{
					this.trimming = StringTrimming.Character;
				}
			}


	// Get or set this object's properties.
	public StringAlignment Alignment
			{
				get
				{
					return alignment;
				}
				set
				{
					alignment = value;
				}
			}
	public int DigitSubstitutionLanguage
			{
				get
				{
					return language;
				}
			}
	public StringDigitSubstitute DigitSubstitutionMethod
			{
				get
				{
					return digitMethod;
				}
				set
				{
					digitMethod = value;
				}
			}
	public StringFormatFlags FormatFlags
			{
				get
				{
					return options;
				}
				set
				{
					options = value;
				}
			}
	public HotkeyPrefix HotkeyPrefix
			{
				get
				{
					return hotkeyPrefix;
				}
				set
				{
					hotkeyPrefix = value;
				}
			}
	public StringAlignment LineAlignment
			{
				get
				{
					return lineAlignment;
				}
				set
				{
					lineAlignment = value;
				}
			}
	public StringTrimming Trimming
			{
				get
				{
					return trimming;
				}
				set
				{
					trimming = value;
				}
			}

	// Get the generic default string format.
	public static StringFormat GenericDefault
			{
				get { return new StringFormat(false); }
			}

	// Get the generic typographic string format.
	public static StringFormat GenericTypographic
			{
				get { return new StringFormat(true); }
			}


	// Clone this object.
	public Object Clone()
			{
				return new StringFormat(this);
			}

	// Dispose of this object.
	public void Dispose()
			{
				// Nothing to do here in this implementation.
			}

	// Get tab stop information for this format.
	public float[] GetTabStops(out float firstTabOffset)
			{
			#if CONFIG_EXTENDED_NUMERICS
				if(this.tabStops == null)
				{
					this.firstTabOffset = 8.0f;
					this.tabStops = new float [] {8.0f};
				}
			#endif
				firstTabOffset = this.firstTabOffset;
				return this.tabStops;
			}

	// Set the digit substitution properties.
	public void SetDigitSubstitution
				(int language, StringDigitSubstitute substitute)
			{
				this.language = language;
				this.digitMethod = digitMethod;
			}

	// Set the measurable character ranges.
	public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
			{
				this.ranges = ranges;
			}

	// Set the tab stops for this format.
	public void SetTabStops(float firstTabOffset, float[] tabStops)
			{
				this.firstTabOffset = firstTabOffset;
				this.tabStops = tabStops;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "[StringFormat, FormatFlags=" +
					   options.ToString() + "]";
			}

}; // class StringFormat

}; // namespace System.Drawing
