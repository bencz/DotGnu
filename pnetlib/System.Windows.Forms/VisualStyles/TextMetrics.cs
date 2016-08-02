/*
 * TextMetrics.cs - Implementation of the
 *			"System.Windows.Forms.VisualStyles.TextMetrics" class.
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

#if CONFIG_FRAMEWORK_2_0
namespace System.Windows.Forms.VisualStyles
{

public struct TextMetrics
{
	private int ascent;
	private int aveCharWidth;
	private int descent;
	private int digitizedAspectX;
	private int digitizedAspectY;
	private int externalLeading;
	private int height;
	private int internalLeading;
	private int maxCharWidth;
	private int overhang;
	private int weight;
	private char breakChar;
	private char defaultChar;
	private char firstChar;
	private char lastChar;
	private bool italic;
	private bool struckOut;
	private bool underlined;
	private TextMetricsPitchAndFamilyValues pitchAndFamiy;
	private TextMetricsCharacterSet charSet;

	public int Ascent
	{
		get
		{
			return ascent;
		}
		set
		{
			ascent = value;
		}
	}

	public int AverageCharWidth
	{
		get
		{
			return aveCharWidth;
		}
		set
		{
			aveCharWidth = value;
		}
	}

	public char BreakChar
	{
		get
		{
			return breakChar;
		}
		set
		{
			breakChar = value;
		}
	}

	public TextMetricsCharacterSet CharSet
	{
		get
		{
			return charSet;
		}
		set
		{
			charSet = value;
		}
	}

	public char DefaultChar
	{
		get
		{
			return defaultChar;
		}
		set
		{
			defaultChar = value;
		}
	}

	public int Descent
	{
		get
		{
			return descent;
		}
		set
		{
			descent = value;
		}
	}

	public int DigitizedAspectX
	{
		get
		{
			return digitizedAspectX;
		}
		set
		{
			digitizedAspectX = value;
		}
	}

	public int DigitizedAspectY
	{
		get
		{
			return digitizedAspectY;
		}
		set
		{
			digitizedAspectY = value;
		}
	}

	public int ExternalLeading
	{
		get
		{
			return externalLeading;
		}
		set
		{
			externalLeading = value;
		}
	}

	public char FirstChar
	{
		get
		{
			return firstChar;
		}
		set
		{
			firstChar = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int InternalLeading
	{
		get
		{
			return internalLeading;
		}
		set
		{
			internalLeading = value;
		}
	}

	public bool Italic
	{
		get
		{
			return italic;
		}
		set
		{
			italic = value;
		}
	}

	public char LastChar
	{
		get
		{
			return lastChar;
		}
		set
		{
			lastChar = value;
		}
	}

	public int MaxCharWidth
	{
		get
		{
			return maxCharWidth;
		}
		set
		{
			maxCharWidth = value;
		}
	}

	public int Overhang
	{
		get
		{
			return overhang;
		}
		set
		{
			overhang = value;
		}
	}

	public TextMetricsPitchAndFamilyValues PitchAndFamily
	{
		get
		{
			return pitchAndFamiy;
		}
		set
		{
			pitchAndFamiy = value;
		}
	}

	public bool StruckOut
	{
		get
		{
			return struckOut;
		}
		set
		{
			struckOut = value;
		}
	}

	public bool Underlined
	{
		get
		{
			return underlined;
		}
		set
		{
			underlined = value;
		}
	}

	public int Weight
	{
		get
		{
			return weight;
		}
		set
		{
			weight = value;
		}
	}
}; // struct TextMetrics

}; // namespace System.Windows.Forms.VisualStyles
#endif // CONFIG_FRAMEWORK_2_0

