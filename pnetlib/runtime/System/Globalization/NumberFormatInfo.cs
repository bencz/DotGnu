/*
 * NumberFormatInfo.cs - Implementation of the
 *        "System.Globalization.NumberFormatInfo" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

using System;

public sealed class NumberFormatInfo : ICloneable, IFormatProvider
{
	// Internal state.
	private static NumberFormatInfo invariantInfo;
	private int currencyDecimalDigits;
	private String currencyDecimalSeparator;
	private int[] currencyGroupSizes;
	private int[] numberGroupSizes;
	private int[] percentGroupSizes;
	private String currencyGroupSeparator;
	private String currencySymbol;
	private String nanSymbol;
	private int currencyPositivePattern;
	private int currencyNegativePattern;
	private int numberNegativePattern;
	private int percentPositivePattern;
	private int percentNegativePattern;
	private String positiveInfinitySymbol;
	private String negativeInfinitySymbol;
	private String positiveSign;
	private String negativeSign;
	private int numberDecimalDigits;
	private String numberDecimalSeparator;
	private String numberGroupSeparator;
	private int percentDecimalDigits;
	private String percentDecimalSeparator;
	private String percentGroupSeparator;
	private String percentSymbol;
	private String perMilleSymbol;
	private bool readOnly;

	// Properties.
	public static NumberFormatInfo InvariantInfo
			{
				get
				{
					lock(typeof(NumberFormatInfo))
					{
						if(invariantInfo == null)
						{
							invariantInfo = new NumberFormatInfo();
							invariantInfo.readOnly = true;
						}
						return invariantInfo;
					}
				}
			}
	public static NumberFormatInfo CurrentInfo
			{
				get
				{
					// return CultureInfo.CurrentCulture.NumberFormat;
					return System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
				}
			}
	public int CurrencyDecimalDigits
			{
				get
				{
					return currencyDecimalDigits;
				}
				set
				{
					if(value < 0 || value > 99)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To99"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencyDecimalDigits = value;
				}
			}
	public String CurrencyDecimalSeparator
			{
				get
				{
					return currencyDecimalSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencyDecimalSeparator = value;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return readOnly;
				}
			}
	public int[] CurrencyGroupSizes
			{
				get
				{
					return currencyGroupSizes;
				}
				set
				{
					ValidateGroupSizes(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencyGroupSizes = value;
				}
			}
	public int[] NumberGroupSizes
			{
				get
				{
					return numberGroupSizes;
				}
				set
				{
					ValidateGroupSizes(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					numberGroupSizes = value;
				}
			}
	public int[] PercentGroupSizes
			{
				get
				{
					return percentGroupSizes;
				}
				set
				{
					ValidateGroupSizes(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentGroupSizes = value;
				}
			}
	public String CurrencyGroupSeparator
			{
				get
				{
					return currencyGroupSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencyGroupSeparator = value;
				}
			}
	public String CurrencySymbol
			{
				get
				{
					return currencySymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencySymbol = value;
				}
			}
	public String NaNSymbol
			{
				get
				{
					return nanSymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					nanSymbol = value;
				}
			}
	public int CurrencyPositivePattern
			{
				get
				{
					return currencyPositivePattern;
				}
				set
				{
					if(value < 0 || value > 3)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To3"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					currencyPositivePattern = value;
				}
			}
	public int CurrencyNegativePattern
			{
				get
				{
					return currencyNegativePattern;
				}
				set
				{
					if(value < 0 || value > 15)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To15"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException();
					}
					currencyNegativePattern = value;
				}
			}
	public int NumberNegativePattern
			{
				get
				{
					return numberNegativePattern;
				}
				set
				{
					if(value < 0 || value > 4)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To4"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					numberNegativePattern = value;
				}
			}
	public int PercentPositivePattern
			{
				get
				{
					return percentPositivePattern;
				}
				set
				{
					if(value < 0 || value > 2)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To2"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentPositivePattern = value;
				}
			}
	public int PercentNegativePattern
			{
				get
				{
					return percentNegativePattern;
				}
				set
				{
					if(value < 0 || value > 2)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To2"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentNegativePattern = value;
				}
			}
	public String PositiveInfinitySymbol
			{
				get
				{
					return positiveInfinitySymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					positiveInfinitySymbol = value;
				}
			}
	public String NegativeInfinitySymbol
			{
				get
				{
					return negativeInfinitySymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					negativeInfinitySymbol = value;
				}
			}
	public String PositiveSign
			{
				get
				{
					return positiveSign;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					positiveSign = value;
				}
			}
	public String NegativeSign
			{
				get
				{
					return negativeSign;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					negativeSign = value;
				}
			}
	public int NumberDecimalDigits
			{
				get
				{
					return numberDecimalDigits;
				}
				set
				{
					if(value < 0 || value > 99)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To99"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					numberDecimalDigits = value;
				}
			}
	public String NumberDecimalSeparator
			{
				get
				{
					return numberDecimalSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					numberDecimalSeparator = value;
				}
			}
	public String NumberGroupSeparator
			{
				get
				{
					return numberGroupSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					numberGroupSeparator = value;
				}
			}
	public int PercentDecimalDigits
			{
				get
				{
					return percentDecimalDigits;
				}
				set
				{
					if(value < 0 || value > 99)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_Value0To99"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentDecimalDigits = value;
				}
			}
	public String PercentDecimalSeparator
			{
				get
				{
					return percentDecimalSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentDecimalSeparator = value;
				}
			}
	public String PercentGroupSeparator
			{
				get
				{
					return percentGroupSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentGroupSeparator = value;
				}
			}
	public String PercentSymbol
			{
				get
				{
					return percentSymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					percentSymbol = value;
				}
			}
	public String PerMilleSymbol
			{
				get
				{
					return perMilleSymbol;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					perMilleSymbol = value;
				}
			}

	// Construct the object with default invariant properties.
	public NumberFormatInfo()
			{
				currencyDecimalDigits = 2;
				currencyDecimalSeparator = ".";
				currencyGroupSizes = new int[1];
				currencyGroupSizes[0] = 3;
				numberGroupSizes = new int[1];
				numberGroupSizes[0] = 3;
				percentGroupSizes = new int[1];
				percentGroupSizes[0] = 3;
				currencyGroupSeparator = ",";
				currencySymbol = "\u00a4";
				nanSymbol = "NaN";
				currencyPositivePattern = 0;
				currencyNegativePattern = 0;
				numberNegativePattern = 1;
				percentPositivePattern = 0;
				percentNegativePattern = 0;
				positiveInfinitySymbol = "Infinity";
				negativeInfinitySymbol = "-Infinity";
				positiveSign = "+";
				negativeSign = "-";
				numberDecimalDigits = 2;
				numberDecimalSeparator = ".";
				numberGroupSeparator = ",";
				percentDecimalDigits = 2;
				percentDecimalSeparator = ".";
				percentGroupSeparator = ",";
				percentSymbol = "%";
				perMilleSymbol = "\u2030";
				readOnly = false;
			}

	// Implementation of the ICloneable interface.
	public Object Clone()
			{
				NumberFormatInfo numberFormat = (NumberFormatInfo)MemberwiseClone();
				numberFormat.readOnly = false;
				return numberFormat;
			}

	// Implementation of the IFormatProvider interface.
	public Object GetFormat(Type formatType)
			{
				if(formatType == typeof(NumberFormatInfo))
				{
					return this;
				}
				else
				{
					return CurrentInfo;
				}
			}

	// Get the number format information associated with "provider".
#if ECMA_COMPAT
	internal
#else
	public
#endif
	static NumberFormatInfo GetInstance(IFormatProvider provider)
			{
				if(provider != null)
				{
					Object obj = provider.GetFormat(typeof(NumberFormatInfo));
					if(obj != null)
					{
						return (NumberFormatInfo)obj;
					}
				}
				return CurrentInfo;
			}

	// Convert a number format info object into a read-only version.
	public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
			{
				if(nfi == null)
				{
					throw new ArgumentNullException("nfi");
				}
				else if(nfi.IsReadOnly)
				{
					return nfi;
				}
				else
				{
					NumberFormatInfo newNfi = (NumberFormatInfo)(nfi.Clone());
					newNfi.readOnly = true;
					return newNfi;
				}
			}

	// Validate a number group size array.
	private void ValidateGroupSizes(int[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				int posn;
				for(posn = 0; posn < (value.Length - 1); ++posn)
				{
					if(value[posn] < 1 || value[posn] > 9)
					{
						throw new ArgumentOutOfRangeException
							("value[" + posn.ToString() + "]",
							 _("Arg_Value1To9"));
					}
				}
				if(value.Length < 1 || value[value.Length - 1] < 0 ||
				   value[value.Length - 1] > 9)
				{
					throw new ArgumentOutOfRangeException
						("value[" + (value.Length - 1).ToString() + "]",
						 _("Arg_Value0To9"));
				}
			}

}; // class NumberFormatInfo

}; // namespace System.Globalization
