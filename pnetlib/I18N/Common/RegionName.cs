/*
 * RegionName.cs - Implementation of the "I18N.Common.RegionName" class.
 *
 * Copyright (C) 2002, 2004  Southern Storm Software, Pty Ltd.
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

namespace I18N.Common
{

using System;

internal sealed class RegionName
{

	// Accessible internal state.
	public int	  regionID;
	public String twoLetterISOName;
	public String threeLetterISOName;
	public String threeLetterWindowsName;
	public bool   isMetric;
	public String currencySymbol;
	public String isoCurrencySymbol;
	public int    currencyDigits;
	public String currencyEnglishName;
	public String currencyNativeName;

	// Construct a "RegionName" instance.
	public RegionName(int regionID,
					  String twoLetterISOName, String threeLetterISOName,
					  String threeLetterWindowsName,
					  bool isMetric, String currencySymbol,
					  String isoCurrencySymbol, int currencyDigits)
			{
				this.regionID               = regionID;
				this.twoLetterISOName       = twoLetterISOName;
				this.threeLetterISOName     = threeLetterISOName;
				this.threeLetterWindowsName = threeLetterWindowsName;
				this.isMetric				= isMetric;
				this.currencySymbol			= currencySymbol;
				this.isoCurrencySymbol		= isoCurrencySymbol;
				this.currencyDigits			= currencyDigits;
				this.currencyEnglishName    = currencySymbol;	// TODO
				this.currencyNativeName     = currencySymbol;	// TODO
			}

}; // class RegionName

}; // namespace I18N.Common
