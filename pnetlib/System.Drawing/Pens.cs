/*
 * Pens.cs - Implementation of the "System.Drawing.Pens" class.
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

public sealed class Pens
{
	// Internal state.
	private static Pen[] standardPens;

	// This class cannot be instantiated.
	private Pens() {}

	// Get or create a standard pen.
	private static Pen GetOrCreatePen(KnownColor color)
			{
				lock(typeof(Pens))
				{
					if(standardPens == null)
					{
						standardPens = new Pen
							[((int)(KnownColor.YellowGreen)) -
							 ((int)(KnownColor.Transparent)) + 1];
					}
					int index = ((int)color) - ((int)(KnownColor.Transparent));
					if(standardPens[index] == null)
					{
						standardPens[index]
							= new Pen(new Color(color));
					}
					return standardPens[index];
				}
			}

	// Pen objects for the standard colors.
	public static Pen Transparent
			{
				get
				{
					return GetOrCreatePen(KnownColor.Transparent);
				}
			}
	public static Pen AliceBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.AliceBlue);
				}
			}
	public static Pen AntiqueWhite
			{
				get
				{
					return GetOrCreatePen(KnownColor.AntiqueWhite);
				}
			}
	public static Pen Aqua
			{
				get
				{
					return GetOrCreatePen(KnownColor.Aqua);
				}
			}
	public static Pen Aquamarine
			{
				get
				{
					return GetOrCreatePen(KnownColor.Aquamarine);
				}
			}
	public static Pen Azure
			{
				get
				{
					return GetOrCreatePen(KnownColor.Azure);
				}
			}
	public static Pen Beige
			{
				get
				{
					return GetOrCreatePen(KnownColor.Beige);
				}
			}
	public static Pen Bisque
			{
				get
				{
					return GetOrCreatePen(KnownColor.Bisque);
				}
			}
	public static Pen Black
			{
				get
				{
					return GetOrCreatePen(KnownColor.Black);
				}
			}
	public static Pen BlanchedAlmond
			{
				get
				{
					return GetOrCreatePen(KnownColor.BlanchedAlmond);
				}
			}
	public static Pen Blue
			{
				get
				{
					return GetOrCreatePen(KnownColor.Blue);
				}
			}
	public static Pen BlueViolet
			{
				get
				{
					return GetOrCreatePen(KnownColor.BlueViolet);
				}
			}
	public static Pen Brown
			{
				get
				{
					return GetOrCreatePen(KnownColor.Brown);
				}
			}
	public static Pen BurlyWood
			{
				get
				{
					return GetOrCreatePen(KnownColor.BurlyWood);
				}
			}
	public static Pen CadetBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.CadetBlue);
				}
			}
	public static Pen Chartreuse
			{
				get
				{
					return GetOrCreatePen(KnownColor.Chartreuse);
				}
			}
	public static Pen Chocolate
			{
				get
				{
					return GetOrCreatePen(KnownColor.Chocolate);
				}
			}
	public static Pen Coral
			{
				get
				{
					return GetOrCreatePen(KnownColor.Coral);
				}
			}
	public static Pen CornflowerBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.CornflowerBlue);
				}
			}
	public static Pen Cornsilk
			{
				get
				{
					return GetOrCreatePen(KnownColor.Cornsilk);
				}
			}
	public static Pen Crimson
			{
				get
				{
					return GetOrCreatePen(KnownColor.Crimson);
				}
			}
	public static Pen Cyan
			{
				get
				{
					return GetOrCreatePen(KnownColor.Cyan);
				}
			}
	public static Pen DarkBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkBlue);
				}
			}
	public static Pen DarkCyan
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkCyan);
				}
			}
	public static Pen DarkGoldenrod
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkGoldenrod);
				}
			}
	public static Pen DarkGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkGray);
				}
			}
	public static Pen DarkGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkGreen);
				}
			}
	public static Pen DarkKhaki
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkKhaki);
				}
			}
	public static Pen DarkMagenta
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkMagenta);
				}
			}
	public static Pen DarkOliveGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkOliveGreen);
				}
			}
	public static Pen DarkOrange
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkOrange);
				}
			}
	public static Pen DarkOrchid
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkOrchid);
				}
			}
	public static Pen DarkRed
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkRed);
				}
			}
	public static Pen DarkSalmon
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkSalmon);
				}
			}
	public static Pen DarkSeaGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkSeaGreen);
				}
			}
	public static Pen DarkSlateBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkSlateBlue);
				}
			}
	public static Pen DarkSlateGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkSlateGray);
				}
			}
	public static Pen DarkTurquoise
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkTurquoise);
				}
			}
	public static Pen DarkViolet
			{
				get
				{
					return GetOrCreatePen(KnownColor.DarkViolet);
				}
			}
	public static Pen DeepPink
			{
				get
				{
					return GetOrCreatePen(KnownColor.DeepPink);
				}
			}
	public static Pen DeepSkyBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.DeepSkyBlue);
				}
			}
	public static Pen DimGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.DimGray);
				}
			}
	public static Pen DodgerBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.DodgerBlue);
				}
			}
	public static Pen Firebrick
			{
				get
				{
					return GetOrCreatePen(KnownColor.Firebrick);
				}
			}
	public static Pen FloralWhite
			{
				get
				{
					return GetOrCreatePen(KnownColor.FloralWhite);
				}
			}
	public static Pen ForestGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.ForestGreen);
				}
			}
	public static Pen Fuchsia
			{
				get
				{
					return GetOrCreatePen(KnownColor.Fuchsia);
				}
			}
	public static Pen Gainsboro
			{
				get
				{
					return GetOrCreatePen(KnownColor.Gainsboro);
				}
			}
	public static Pen GhostWhite
			{
				get
				{
					return GetOrCreatePen(KnownColor.GhostWhite);
				}
			}
	public static Pen Gold
			{
				get
				{
					return GetOrCreatePen(KnownColor.Gold);
				}
			}
	public static Pen Goldenrod
			{
				get
				{
					return GetOrCreatePen(KnownColor.Goldenrod);
				}
			}
	public static Pen Gray
			{
				get
				{
					return GetOrCreatePen(KnownColor.Gray);
				}
			}
	public static Pen Green
			{
				get
				{
					return GetOrCreatePen(KnownColor.Green);
				}
			}
	public static Pen GreenYellow
			{
				get
				{
					return GetOrCreatePen(KnownColor.GreenYellow);
				}
			}
	public static Pen Honeydew
			{
				get
				{
					return GetOrCreatePen(KnownColor.Honeydew);
				}
			}
	public static Pen HotPink
			{
				get
				{
					return GetOrCreatePen(KnownColor.HotPink);
				}
			}
	public static Pen IndianRed
			{
				get
				{
					return GetOrCreatePen(KnownColor.IndianRed);
				}
			}
	public static Pen Indigo
			{
				get
				{
					return GetOrCreatePen(KnownColor.Indigo);
				}
			}
	public static Pen Ivory
			{
				get
				{
					return GetOrCreatePen(KnownColor.Ivory);
				}
			}
	public static Pen Khaki
			{
				get
				{
					return GetOrCreatePen(KnownColor.Khaki);
				}
			}
	public static Pen Lavender
			{
				get
				{
					return GetOrCreatePen(KnownColor.Lavender);
				}
			}
	public static Pen LavenderBlush
			{
				get
				{
					return GetOrCreatePen(KnownColor.LavenderBlush);
				}
			}
	public static Pen LawnGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.LawnGreen);
				}
			}
	public static Pen LemonChiffon
			{
				get
				{
					return GetOrCreatePen(KnownColor.LemonChiffon);
				}
			}
	public static Pen LightBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightBlue);
				}
			}
	public static Pen LightCoral
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightCoral);
				}
			}
	public static Pen LightCyan
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightCyan);
				}
			}
	public static Pen LightGoldenrodYellow
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightGoldenrodYellow);
				}
			}
	public static Pen LightGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightGray);
				}
			}
	public static Pen LightGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightGreen);
				}
			}
	public static Pen LightPink
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightPink);
				}
			}
	public static Pen LightSalmon
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightSalmon);
				}
			}
	public static Pen LightSeaGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightSeaGreen);
				}
			}
	public static Pen LightSkyBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightSkyBlue);
				}
			}
	public static Pen LightSlateGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightSlateGray);
				}
			}
	public static Pen LightSteelBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightSteelBlue);
				}
			}
	public static Pen LightYellow
			{
				get
				{
					return GetOrCreatePen(KnownColor.LightYellow);
				}
			}
	public static Pen Lime
			{
				get
				{
					return GetOrCreatePen(KnownColor.Lime);
				}
			}
	public static Pen LimeGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.LimeGreen);
				}
			}
	public static Pen Linen
			{
				get
				{
					return GetOrCreatePen(KnownColor.Linen);
				}
			}
	public static Pen Magenta
			{
				get
				{
					return GetOrCreatePen(KnownColor.Magenta);
				}
			}
	public static Pen Maroon
			{
				get
				{
					return GetOrCreatePen(KnownColor.Maroon);
				}
			}
	public static Pen MediumAquamarine
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumAquamarine);
				}
			}
	public static Pen MediumBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumBlue);
				}
			}
	public static Pen MediumOrchid
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumOrchid);
				}
			}
	public static Pen MediumPurple
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumPurple);
				}
			}
	public static Pen MediumSeaGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumSeaGreen);
				}
			}
	public static Pen MediumSlateBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumSlateBlue);
				}
			}
	public static Pen MediumSpringGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumSpringGreen);
				}
			}
	public static Pen MediumTurquoise
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumTurquoise);
				}
			}
	public static Pen MediumVioletRed
			{
				get
				{
					return GetOrCreatePen(KnownColor.MediumVioletRed);
				}
			}
	public static Pen MidnightBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.MidnightBlue);
				}
			}
	public static Pen MintCream
			{
				get
				{
					return GetOrCreatePen(KnownColor.MintCream);
				}
			}
	public static Pen MistyRose
			{
				get
				{
					return GetOrCreatePen(KnownColor.MistyRose);
				}
			}
	public static Pen Moccasin
			{
				get
				{
					return GetOrCreatePen(KnownColor.Moccasin);
				}
			}
	public static Pen NavajoWhite
			{
				get
				{
					return GetOrCreatePen(KnownColor.NavajoWhite);
				}
			}
	public static Pen Navy
			{
				get
				{
					return GetOrCreatePen(KnownColor.Navy);
				}
			}
	public static Pen OldLace
			{
				get
				{
					return GetOrCreatePen(KnownColor.OldLace);
				}
			}
	public static Pen Olive
			{
				get
				{
					return GetOrCreatePen(KnownColor.Olive);
				}
			}
	public static Pen OliveDrab
			{
				get
				{
					return GetOrCreatePen(KnownColor.OliveDrab);
				}
			}
	public static Pen Orange
			{
				get
				{
					return GetOrCreatePen(KnownColor.Orange);
				}
			}
	public static Pen OrangeRed
			{
				get
				{
					return GetOrCreatePen(KnownColor.OrangeRed);
				}
			}
	public static Pen Orchid
			{
				get
				{
					return GetOrCreatePen(KnownColor.Orchid);
				}
			}
	public static Pen PaleGoldenrod
			{
				get
				{
					return GetOrCreatePen(KnownColor.PaleGoldenrod);
				}
			}
	public static Pen PaleGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.PaleGreen);
				}
			}
	public static Pen PaleTurquoise
			{
				get
				{
					return GetOrCreatePen(KnownColor.PaleTurquoise);
				}
			}
	public static Pen PaleVioletRed
			{
				get
				{
					return GetOrCreatePen(KnownColor.PaleVioletRed);
				}
			}
	public static Pen PapayaWhip
			{
				get
				{
					return GetOrCreatePen(KnownColor.PapayaWhip);
				}
			}
	public static Pen PeachPuff
			{
				get
				{
					return GetOrCreatePen(KnownColor.PeachPuff);
				}
			}
	public static Pen Peru
			{
				get
				{
					return GetOrCreatePen(KnownColor.Peru);
				}
			}
	public static Pen Pink
			{
				get
				{
					return GetOrCreatePen(KnownColor.Pink);
				}
			}
	public static Pen Plum
			{
				get
				{
					return GetOrCreatePen(KnownColor.Plum);
				}
			}
	public static Pen PowderBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.PowderBlue);
				}
			}
	public static Pen Purple
			{
				get
				{
					return GetOrCreatePen(KnownColor.Purple);
				}
			}
	public static Pen Red
			{
				get
				{
					return GetOrCreatePen(KnownColor.Red);
				}
			}
	public static Pen RosyBrown
			{
				get
				{
					return GetOrCreatePen(KnownColor.RosyBrown);
				}
			}
	public static Pen RoyalBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.RoyalBlue);
				}
			}
	public static Pen SaddleBrown
			{
				get
				{
					return GetOrCreatePen(KnownColor.SaddleBrown);
				}
			}
	public static Pen Salmon
			{
				get
				{
					return GetOrCreatePen(KnownColor.Salmon);
				}
			}
	public static Pen SandyBrown
			{
				get
				{
					return GetOrCreatePen(KnownColor.SandyBrown);
				}
			}
	public static Pen SeaGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.SeaGreen);
				}
			}
	public static Pen SeaShell
			{
				get
				{
					return GetOrCreatePen(KnownColor.SeaShell);
				}
			}
	public static Pen Sienna
			{
				get
				{
					return GetOrCreatePen(KnownColor.Sienna);
				}
			}
	public static Pen Silver
			{
				get
				{
					return GetOrCreatePen(KnownColor.Silver);
				}
			}
	public static Pen SkyBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.SkyBlue);
				}
			}
	public static Pen SlateBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.SlateBlue);
				}
			}
	public static Pen SlateGray
			{
				get
				{
					return GetOrCreatePen(KnownColor.SlateGray);
				}
			}
	public static Pen Snow
			{
				get
				{
					return GetOrCreatePen(KnownColor.Snow);
				}
			}
	public static Pen SpringGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.SpringGreen);
				}
			}
	public static Pen SteelBlue
			{
				get
				{
					return GetOrCreatePen(KnownColor.SteelBlue);
				}
			}
	public static Pen Tan
			{
				get
				{
					return GetOrCreatePen(KnownColor.Tan);
				}
			}
	public static Pen Teal
			{
				get
				{
					return GetOrCreatePen(KnownColor.Teal);
				}
			}
	public static Pen Thistle
			{
				get
				{
					return GetOrCreatePen(KnownColor.Thistle);
				}
			}
	public static Pen Tomato
			{
				get
				{
					return GetOrCreatePen(KnownColor.Tomato);
				}
			}
	public static Pen Turquoise
			{
				get
				{
					return GetOrCreatePen(KnownColor.Turquoise);
				}
			}
	public static Pen Violet
			{
				get
				{
					return GetOrCreatePen(KnownColor.Violet);
				}
			}
	public static Pen Wheat
			{
				get
				{
					return GetOrCreatePen(KnownColor.Wheat);
				}
			}
	public static Pen White
			{
				get
				{
					return GetOrCreatePen(KnownColor.White);
				}
			}
	public static Pen WhiteSmoke
			{
				get
				{
					return GetOrCreatePen(KnownColor.WhiteSmoke);
				}
			}
	public static Pen Yellow
			{
				get
				{
					return GetOrCreatePen(KnownColor.Yellow);
				}
			}
	public static Pen YellowGreen
			{
				get
				{
					return GetOrCreatePen(KnownColor.YellowGreen);
				}
			}

}; // class Pens
		
}; // namespace System.Drawing
