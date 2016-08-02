/*
 * RegionInfo.cs - Implementation of "System.Globalization.RegionInfo".
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

namespace System.Globalization
{

#if !ECMA_COMPAT

using System;
using System.Text;

public class RegionInfo
{
	// Internal state.  Points to a "RegionInfo" object that
	// is allocated by the I18N routines.
	private RegionInfo regionInfo;

	// The current region.  Other implementations make this global static,
	// but technically that is incorrect if the culture is thread-specific.
	// We want the current region to track the current culture at all times.
	[ThreadStatic] internal static RegionInfo currentRegion;

	// Constructors.
	public RegionInfo(int culture)
			{
				// The I18N routines use -1 to tell this class
				// not to perform a recursive region lookup.
				if(culture != -1)
				{
					regionInfo = (RegionInfo)
						(Encoding.InvokeI18N("GetRegion", culture));
					if(regionInfo == null)
					{
						throw new ArgumentException(_("Arg_InvalidRegion"));
					}
				}
			}
	public RegionInfo(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				regionInfo = (RegionInfo)
					(Encoding.InvokeI18N("GetRegion", name));
				if(regionInfo == null)
				{
					throw new ArgumentException(_("Arg_InvalidRegion"));
				}
			}

	// Get the region for the current culture.
	public static RegionInfo CurrentRegion
			{
				get
				{
					if(currentRegion == null)
					{
						currentRegion =
							new RegionInfo(CultureInfo.CurrentCulture.LCID);
					}
					return currentRegion;
				}
			}

	// Get the region properties.
	public virtual String CurrencySymbol
			{
				get
				{
					return regionInfo.CurrencySymbol;
				}
			}
	public virtual String DisplayName
			{
				get
				{
					return regionInfo.DisplayName;
				}
			}
	public virtual String EnglishName
			{
				get
				{
					return regionInfo.EnglishName;
				}
			}
	public virtual bool IsMetric
			{
				get
				{
					return regionInfo.IsMetric;
				}
			}
	public virtual String ISOCurrencySymbol
			{
				get
				{
					return regionInfo.ISOCurrencySymbol;
				}
			}
	public virtual String Name
			{
				get
				{
					return regionInfo.Name;
				}
			}
	public virtual String ThreeLetterISORegionName
			{
				get
				{
					return regionInfo.ThreeLetterISORegionName;
				}
			}
	public virtual String ThreeLetterWindowsRegionName
			{
				get
				{
					return regionInfo.ThreeLetterWindowsRegionName;
				}
			}
	public virtual String TwoLetterISORegionName
			{
				get
				{
					return regionInfo.TwoLetterISORegionName;
				}
			}
#if CONFIG_FRAMEWORK_2_0
	public virtual String CurrencyEnglishName
			{
				get
				{
					return regionInfo.CurrencyEnglishName;
				}
			}
	public virtual String CurrencyNativeName
			{
				get
				{
					return regionInfo.CurrencyNativeName;
				}
			}
	public virtual int GeoId
			{
				get
				{
					return regionInfo.GeoId;
				}
			}
#endif

	// Determine if two region information objects are equal.
	public override bool Equals(Object obj)
			{
				RegionInfo other = (obj as RegionInfo);
				if(other != null)
				{
					return regionInfo.Equals(other.regionInfo);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return regionInfo.GetHashCode();
			}

	// Convert this region into a string.
	public override String ToString()
			{
				return ThreeLetterISORegionName;
			}

}; // class RegionInfo

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
