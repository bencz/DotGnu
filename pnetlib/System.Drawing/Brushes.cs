/*
 * Brushes.cs - Implementation of the "System.Drawing.Brushes" class.
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

public sealed class Brushes
{
	// Internal state.
	private static Brush[] standardBrushes;

	// This class cannot be instantiated.
	private Brushes() {}

	// Get or create a standard brush.
	private static Brush GetOrCreateBrush(KnownColor color)
			{
				lock(typeof(Brushes))
				{
					if(standardBrushes == null)
					{
						standardBrushes = new Brush
							[((int)(KnownColor.YellowGreen)) -
							 ((int)(KnownColor.Transparent)) + 1];
					}
					int index = ((int)color) - ((int)(KnownColor.Transparent));
					if(standardBrushes[index] == null)
					{
						standardBrushes[index]
							= new SolidBrush(new Color(color));
					}
					return standardBrushes[index];
				}
			}

	// Standard brush objects.
	public static Brush Transparent
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Transparent);
				}
			}
	public static Brush AliceBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.AliceBlue);
				}
			}
	public static Brush AntiqueWhite
			{
				get
				{
					return GetOrCreateBrush(KnownColor.AntiqueWhite);
				}
			}
	public static Brush Aqua
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Aqua);
				}
			}
	public static Brush Aquamarine
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Aquamarine);
				}
			}
	public static Brush Azure
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Azure);
				}
			}
	public static Brush Beige
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Beige);
				}
			}
	public static Brush Bisque
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Bisque);
				}
			}
	public static Brush Black
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Black);
				}
			}
	public static Brush BlanchedAlmond
			{
				get
				{
					return GetOrCreateBrush(KnownColor.BlanchedAlmond);
				}
			}
	public static Brush Blue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Blue);
				}
			}
	public static Brush BlueViolet
			{
				get
				{
					return GetOrCreateBrush(KnownColor.BlueViolet);
				}
			}
	public static Brush Brown
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Brown);
				}
			}
	public static Brush BurlyWood
			{
				get
				{
					return GetOrCreateBrush(KnownColor.BurlyWood);
				}
			}
	public static Brush CadetBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.CadetBlue);
				}
			}
	public static Brush Chartreuse
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Chartreuse);
				}
			}
	public static Brush Chocolate
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Chocolate);
				}
			}
	public static Brush Coral
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Coral);
				}
			}
	public static Brush CornflowerBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.CornflowerBlue);
				}
			}
	public static Brush Cornsilk
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Cornsilk);
				}
			}
	public static Brush Crimson
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Crimson);
				}
			}
	public static Brush Cyan
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Cyan);
				}
			}
	public static Brush DarkBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkBlue);
				}
			}
	public static Brush DarkCyan
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkCyan);
				}
			}
	public static Brush DarkGoldenrod
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkGoldenrod);
				}
			}
	public static Brush DarkGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkGray);
				}
			}
	public static Brush DarkGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkGreen);
				}
			}
	public static Brush DarkKhaki
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkKhaki);
				}
			}
	public static Brush DarkMagenta
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkMagenta);
				}
			}
	public static Brush DarkOliveGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkOliveGreen);
				}
			}
	public static Brush DarkOrange
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkOrange);
				}
			}
	public static Brush DarkOrchid
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkOrchid);
				}
			}
	public static Brush DarkRed
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkRed);
				}
			}
	public static Brush DarkSalmon
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkSalmon);
				}
			}
	public static Brush DarkSeaGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkSeaGreen);
				}
			}
	public static Brush DarkSlateBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkSlateBlue);
				}
			}
	public static Brush DarkSlateGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkSlateGray);
				}
			}
	public static Brush DarkTurquoise
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkTurquoise);
				}
			}
	public static Brush DarkViolet
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DarkViolet);
				}
			}
	public static Brush DeepPink
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DeepPink);
				}
			}
	public static Brush DeepSkyBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DeepSkyBlue);
				}
			}
	public static Brush DimGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DimGray);
				}
			}
	public static Brush DodgerBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.DodgerBlue);
				}
			}
	public static Brush Firebrick
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Firebrick);
				}
			}
	public static Brush FloralWhite
			{
				get
				{
					return GetOrCreateBrush(KnownColor.FloralWhite);
				}
			}
	public static Brush ForestGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ForestGreen);
				}
			}
	public static Brush Fuchsia
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Fuchsia);
				}
			}
	public static Brush Gainsboro
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Gainsboro);
				}
			}
	public static Brush GhostWhite
			{
				get
				{
					return GetOrCreateBrush(KnownColor.GhostWhite);
				}
			}
	public static Brush Gold
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Gold);
				}
			}
	public static Brush Goldenrod
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Goldenrod);
				}
			}
	public static Brush Gray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Gray);
				}
			}
	public static Brush Green
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Green);
				}
			}
	public static Brush GreenYellow
			{
				get
				{
					return GetOrCreateBrush(KnownColor.GreenYellow);
				}
			}
	public static Brush Honeydew
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Honeydew);
				}
			}
	public static Brush HotPink
			{
				get
				{
					return GetOrCreateBrush(KnownColor.HotPink);
				}
			}
	public static Brush IndianRed
			{
				get
				{
					return GetOrCreateBrush(KnownColor.IndianRed);
				}
			}
	public static Brush Indigo
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Indigo);
				}
			}
	public static Brush Ivory
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Ivory);
				}
			}
	public static Brush Khaki
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Khaki);
				}
			}
	public static Brush Lavender
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Lavender);
				}
			}
	public static Brush LavenderBlush
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LavenderBlush);
				}
			}
	public static Brush LawnGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LawnGreen);
				}
			}
	public static Brush LemonChiffon
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LemonChiffon);
				}
			}
	public static Brush LightBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightBlue);
				}
			}
	public static Brush LightCoral
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightCoral);
				}
			}
	public static Brush LightCyan
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightCyan);
				}
			}
	public static Brush LightGoldenrodYellow
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightGoldenrodYellow);
				}
			}
	public static Brush LightGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightGray);
				}
			}
	public static Brush LightGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightGreen);
				}
			}
	public static Brush LightPink
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightPink);
				}
			}
	public static Brush LightSalmon
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightSalmon);
				}
			}
	public static Brush LightSeaGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightSeaGreen);
				}
			}
	public static Brush LightSkyBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightSkyBlue);
				}
			}
	public static Brush LightSlateGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightSlateGray);
				}
			}
	public static Brush LightSteelBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightSteelBlue);
				}
			}
	public static Brush LightYellow
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LightYellow);
				}
			}
	public static Brush Lime
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Lime);
				}
			}
	public static Brush LimeGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.LimeGreen);
				}
			}
	public static Brush Linen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Linen);
				}
			}
	public static Brush Magenta
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Magenta);
				}
			}
	public static Brush Maroon
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Maroon);
				}
			}
	public static Brush MediumAquamarine
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumAquamarine);
				}
			}
	public static Brush MediumBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumBlue);
				}
			}
	public static Brush MediumOrchid
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumOrchid);
				}
			}
	public static Brush MediumPurple
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumPurple);
				}
			}
	public static Brush MediumSeaGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumSeaGreen);
				}
			}
	public static Brush MediumSlateBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumSlateBlue);
				}
			}
	public static Brush MediumSpringGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumSpringGreen);
				}
			}
	public static Brush MediumTurquoise
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumTurquoise);
				}
			}
	public static Brush MediumVioletRed
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MediumVioletRed);
				}
			}
	public static Brush MidnightBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MidnightBlue);
				}
			}
	public static Brush MintCream
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MintCream);
				}
			}
	public static Brush MistyRose
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MistyRose);
				}
			}
	public static Brush Moccasin
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Moccasin);
				}
			}
	public static Brush NavajoWhite
			{
				get
				{
					return GetOrCreateBrush(KnownColor.NavajoWhite);
				}
			}
	public static Brush Navy
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Navy);
				}
			}
	public static Brush OldLace
			{
				get
				{
					return GetOrCreateBrush(KnownColor.OldLace);
				}
			}
	public static Brush Olive
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Olive);
				}
			}
	public static Brush OliveDrab
			{
				get
				{
					return GetOrCreateBrush(KnownColor.OliveDrab);
				}
			}
	public static Brush Orange
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Orange);
				}
			}
	public static Brush OrangeRed
			{
				get
				{
					return GetOrCreateBrush(KnownColor.OrangeRed);
				}
			}
	public static Brush Orchid
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Orchid);
				}
			}
	public static Brush PaleGoldenrod
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PaleGoldenrod);
				}
			}
	public static Brush PaleGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PaleGreen);
				}
			}
	public static Brush PaleTurquoise
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PaleTurquoise);
				}
			}
	public static Brush PaleVioletRed
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PaleVioletRed);
				}
			}
	public static Brush PapayaWhip
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PapayaWhip);
				}
			}
	public static Brush PeachPuff
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PeachPuff);
				}
			}
	public static Brush Peru
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Peru);
				}
			}
	public static Brush Pink
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Pink);
				}
			}
	public static Brush Plum
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Plum);
				}
			}
	public static Brush PowderBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.PowderBlue);
				}
			}
	public static Brush Purple
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Purple);
				}
			}
	public static Brush Red
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Red);
				}
			}
	public static Brush RosyBrown
			{
				get
				{
					return GetOrCreateBrush(KnownColor.RosyBrown);
				}
			}
	public static Brush RoyalBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.RoyalBlue);
				}
			}
	public static Brush SaddleBrown
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SaddleBrown);
				}
			}
	public static Brush Salmon
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Salmon);
				}
			}
	public static Brush SandyBrown
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SandyBrown);
				}
			}
	public static Brush SeaGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SeaGreen);
				}
			}
	public static Brush SeaShell
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SeaShell);
				}
			}
	public static Brush Sienna
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Sienna);
				}
			}
	public static Brush Silver
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Silver);
				}
			}
	public static Brush SkyBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SkyBlue);
				}
			}
	public static Brush SlateBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SlateBlue);
				}
			}
	public static Brush SlateGray
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SlateGray);
				}
			}
	public static Brush Snow
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Snow);
				}
			}
	public static Brush SpringGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SpringGreen);
				}
			}
	public static Brush SteelBlue
			{
				get
				{
					return GetOrCreateBrush(KnownColor.SteelBlue);
				}
			}
	public static Brush Tan
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Tan);
				}
			}
	public static Brush Teal
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Teal);
				}
			}
	public static Brush Thistle
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Thistle);
				}
			}
	public static Brush Tomato
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Tomato);
				}
			}
	public static Brush Turquoise
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Turquoise);
				}
			}
	public static Brush Violet
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Violet);
				}
			}
	public static Brush Wheat
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Wheat);
				}
			}
	public static Brush White
			{
				get
				{
					return GetOrCreateBrush(KnownColor.White);
				}
			}
	public static Brush WhiteSmoke
			{
				get
				{
					return GetOrCreateBrush(KnownColor.WhiteSmoke);
				}
			}
	public static Brush Yellow
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Yellow);
				}
			}
	public static Brush YellowGreen
			{
				get
				{
					return GetOrCreateBrush(KnownColor.YellowGreen);
				}
			}

}; // class Brushes

}; // namespace System.Drawing
