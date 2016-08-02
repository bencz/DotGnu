/*
 * PrinterUnitConvert.cs - Implementation of the
 *			"System.Drawing.Printing.PrinterUnitConvert" class.
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

namespace System.Drawing.Printing
{

public sealed class PrinterUnitConvert
{
	// Cannot instantiate this class.
	private PrinterUnitConvert() {}

	// Convert from one unit to another.
	public static double Convert
				(double value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
				switch(fromUnit)
				{
					case PrinterUnit.Display:
					{
						switch(toUnit)
						{
							case PrinterUnit.Display: break;

							case PrinterUnit.ThousandthsOfAnInch:
							{
								value /= 10.0;
							}
							break;

							case PrinterUnit.HundredthsOfAMillimeter:
							{
								value *= 0.00254;
							}
							break;

							case PrinterUnit.TenthsOfAMillimeter:
							{
								value *= 0.0254;
							}
							break;
						}
					}
					break;

					case PrinterUnit.ThousandthsOfAnInch:
					{
						switch(toUnit)
						{
							case PrinterUnit.Display:
							{
								value *= 10.0;
							}
							break;

							case PrinterUnit.ThousandthsOfAnInch: break;

							case PrinterUnit.HundredthsOfAMillimeter:
							{
								value *= 0.000254;
							}
							break;

							case PrinterUnit.TenthsOfAMillimeter:
							{
								value *= 0.00254;
							}
							break;
						}
					}
					break;

					case PrinterUnit.HundredthsOfAMillimeter:
					{
						switch(toUnit)
						{
							case PrinterUnit.Display:
							{
								value /= 0.00254;
							}
							break;

							case PrinterUnit.ThousandthsOfAnInch:
							{
								value /= 0.000254;
							}
							break;

							case PrinterUnit.HundredthsOfAMillimeter: break;

							case PrinterUnit.TenthsOfAMillimeter:
							{
								value /= 10.0;
							}
							break;
						}
					}
					break;

					case PrinterUnit.TenthsOfAMillimeter:
					{
						switch(toUnit)
						{
							case PrinterUnit.Display:
							{
								value = value / 0.0254;
							}
							break;

							case PrinterUnit.ThousandthsOfAnInch:
							{
								value = value / 0.00254;
							}
							break;

							case PrinterUnit.HundredthsOfAMillimeter:
							{
								value *= 10.0;
							}
							break;

							case PrinterUnit.TenthsOfAMillimeter: break;
						}
					}
					break;
				}
				return value;
			}
	public static int Convert
				(int value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
			#if CONFIG_EXTENDED_NUMERICS
				return (int)Math.Round
					(Convert((double)value, fromUnit, toUnit));
			#else
				return (int)
					(Convert((double)value, fromUnit, toUnit) + 0.5);
			#endif
			}
	public static Margins Convert
				(Margins value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
				return new Margins
					(Convert(value.Left, fromUnit, toUnit),
					 Convert(value.Right, fromUnit, toUnit),
					 Convert(value.Top, fromUnit, toUnit),
					 Convert(value.Bottom, fromUnit, toUnit));
			}
	public static Point Convert
				(Point value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
				return new Point(Convert(value.X, fromUnit, toUnit),
								 Convert(value.Y, fromUnit, toUnit));
			}
	public static Rectangle Convert
				(Rectangle value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
				return new Rectangle(Convert(value.X, fromUnit, toUnit),
								     Convert(value.Y, fromUnit, toUnit),
								     Convert(value.Width, fromUnit, toUnit),
								     Convert(value.Height, fromUnit, toUnit));
			}
	public static Size Convert
				(Size value, PrinterUnit fromUnit, PrinterUnit toUnit)
			{
				return new Size(Convert(value.Width, fromUnit, toUnit),
								Convert(value.Height, fromUnit, toUnit));
			}

}; // class PrinterUnitConvert

}; // namespace System.Drawing.Printing
