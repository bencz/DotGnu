/*
 * RegionData.cs - Implementation of the "I18N.Common.RegionData" class.
 *
 * Copyright (c) 2002, 2004  Southern Storm Software, Pty Ltd
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

#if !ECMA_COMPAT

using System;
using System.Globalization;

internal class RegionData : RegionInfo
{
	// Internal state.
	private RegionName regionName;

	// Constructor.
	public RegionData(RegionName regionName)
			: base(-1)	// -1 suppresses the region lookup in the base class.
			{
				this.regionName = regionName;
			}

	// Get the region properties.
	public override String CurrencySymbol
			{
				get
				{
					return regionName.currencySymbol;
				}
			}
	public override String DisplayName
			{
				get
				{
					// Use the current culture to determine the display name.
					return (Manager.GetCurrentCulture()).ResolveCountry(Name);
				}
			}
	public override String EnglishName
			{
				get
				{
					return (new CNen()).ResolveCountry(Name);
				}
			}
	public override bool IsMetric
			{
				get
				{
					return regionName.isMetric;
				}
			}
	public override String ISOCurrencySymbol
			{
				get
				{
					return regionName.isoCurrencySymbol;
				}
			}
	public override String Name
			{
				get
				{
					return regionName.twoLetterISOName;
				}
			}
	public override String ThreeLetterISORegionName
			{
				get
				{
					return regionName.threeLetterISOName;
				}
			}
	public override String ThreeLetterWindowsRegionName
			{
				get
				{
					return regionName.threeLetterWindowsName;
				}
			}
	public override String TwoLetterISORegionName
			{
				get
				{
					return regionName.twoLetterISOName;
				}
			}
#if CONFIG_FRAMEWORK_2_0
	public override String CurrencyEnglishName
			{
				get
				{
					return regionName.currencyEnglishName;
				}
			}
	public override String CurrencyNativeName
			{
				get
				{
					return regionName.currencyNativeName;
				}
			}
	public override int GeoId
			{
				get
				{
					return regionName.regionID;
				}
			}
#endif

	// Determine if two region information objects are equal.
	public override bool Equals(Object obj)
			{
				RegionData other = (obj as RegionData);
				if(other != null)
				{
					return (regionName.regionID ==
							other.regionName.regionID);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return regionName.regionID;
			}

}; // class RegionData

#endif // !ECMA_COMPAT

}; // namespace I18N.Common
