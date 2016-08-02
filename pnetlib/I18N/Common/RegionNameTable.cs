/*
 * RegionNameTable.cs - Implementation of the
 *		"I18N.Common.RegionNameTable" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Globalization;

internal sealed class RegionNameTable
{

	// Array of all registered region names.
	private static RegionName[] regions;
	private static int numRegions;

	// Useful constants.
	private const int DefaultTableSize = 64;
	private const int TableExtendSize  = 16;

	// Add an item to the region name table.
	public static void Add(RegionName name)
			{
				if(numRegions < regions.Length)
				{
					regions[numRegions++] = name;
				}
				else
				{
					RegionName[] newRegions;
					newRegions = new RegionName
						[numRegions + TableExtendSize];
					Array.Copy(regions, newRegions, regions.Length);
					regions = newRegions;
					regions[numRegions++] = name;
				}
			}

	// Populate the region name table.  Note: just because
	// a region exists in this table doesn't mean that it is
	// actually supported by the rest of the system.
	public static void PopulateNameTable()
	{
		Add(new RegionName
			(0x0401, "SA", "SAU", "SAU",
			 true, "\u0631.\u0633.\u200F", "SAR", 2));
		Add(new RegionName
			(0x0402, "BG", "BGR", "BGR",
			 true, "\u043B\u0432", "BGL", 2));
		Add(new RegionName
			(0x0403, "ES", "ESP", "ESP",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0404, "TW", "TWN", "TWN",
			 true, "NT$", "TWD", 2));
		Add(new RegionName
			(0x0405, "CZ", "CZE", "CZE",
			 true, "K\u010D", "CZK", 2));
		Add(new RegionName
			(0x0406, "DK", "DNK", "DNK",
			 true, "kr", "DKK", 2));
		Add(new RegionName
			(0x0407, "DE", "DEU", "DEU",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0408, "GR", "GRC", "GRC",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0409, "US", "USA", "USA",
			 false, "$", "USD", 2));
		Add(new RegionName
			(0x040A, "ES", "ESP", "ESP",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x040B, "FI", "FIN", "FIN",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x040C, "FR", "FRA", "FRA",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x040D, "IL", "ISR", "ISR",
			 true, "\u20AA", "ILS", 2));
		Add(new RegionName
			(0x040E, "HU", "HUN", "HUN",
			 true, "Ft", "HUF", 2));
		Add(new RegionName
			(0x040F, "IS", "ISL", "ISL",
			 true, "kr.", "ISK", 2));
		Add(new RegionName
			(0x0410, "IT", "ITA", "ITA",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0411, "JP", "JPN", "JPN",
			 true, "\u00A5", "JPY", 0));
		Add(new RegionName
			(0x0412, "KR", "KOR", "KOR",
			 true, "\u20A9", "KRW", 0));
		Add(new RegionName
			(0x0413, "NL", "NLD", "NLD",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0414, "NO", "NOR", "NOR",
			 true, "kr", "NOK", 2));
		Add(new RegionName
			(0x0415, "PL", "POL", "POL",
			 true, "z\u0142", "PLN", 2));
		Add(new RegionName
			(0x0416, "BR", "BRA", "BRA",
			 true, "R$ ", "BRL", 2));
		Add(new RegionName
			(0x0418, "RO", "ROM", "ROM",
			 true, "lei", "ROL", 2));
		Add(new RegionName
			(0x0419, "RU", "RUS", "RUS",
			 true, "\u0440.", "RUR", 2));
		Add(new RegionName
			(0x041A, "HR", "HRV", "HRV",
			 true, "kn", "HRK", 2));
		Add(new RegionName
			(0x041B, "SK", "SVK", "SVK",
			 true, "Sk", "SKK", 2));
		Add(new RegionName
			(0x041C, "AL", "ALB", "ALB",
			 true, "Lek", "ALL", 2));
		Add(new RegionName
			(0x041D, "SE", "SWE", "SWE",
			 true, "kr", "SEK", 2));
		Add(new RegionName
			(0x041E, "TH", "THA", "THA",
			 true, "\u0E3F", "THB", 2));
		Add(new RegionName
			(0x041F, "TR", "TUR", "TUR",
			 true, "TL", "TRL", 0));
		Add(new RegionName
			(0x0420, "PK", "PAK", "PAK",
			 true, "Rs", "PKR", 2));
		Add(new RegionName
			(0x0421, "ID", "IDN", "IDN",
			 true, "Rp", "IDR", 2));
		Add(new RegionName
			(0x0422, "UA", "UKR", "UKR",
			 true, "\u0433\u0440\u043D.", "UAH", 2));
		Add(new RegionName
			(0x0423, "BY", "BLR", "BLR",
			 true, "\u0440.", "BYB", 2));
		Add(new RegionName
			(0x0424, "SI", "SVN", "SVN",
			 true, "SIT", "SIT", 2));
		Add(new RegionName
			(0x0425, "EE", "EST", "EST",
			 true, "kr", "EEK", 2));
		Add(new RegionName
			(0x0426, "LV", "LVA", "LVA",
			 true, "Ls", "LVL", 2));
		Add(new RegionName
			(0x0427, "LT", "LTU", "LTU",
			 true, "Lt", "LTL", 2));
		Add(new RegionName
			(0x0429, "IR", "IRN", "IRN",
			 true, "\u0631\u064A\u0627\u0644", "IRR", 2));
		Add(new RegionName
			(0x042A, "VN", "VNM", "VNM",
			 true, "\u20AB", "VND", 2));
		Add(new RegionName
			(0x042B, "AM", "ARM", "ARM",
			 true, "\u0564\u0580.", "AMD", 2));
		Add(new RegionName
			(0x042C, "AZ", "AZE", "AZE",
			 true, "man.", "AZM", 2));
		Add(new RegionName
			(0x042D, "ES", "ESP", "ESP",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x042F, "MK", "MKD", "MKD",
			 true, "\u0434\u0435\u043D.", "MKD", 2));
		Add(new RegionName
			(0x0436, "ZA", "ZAF", "ZAF",
			 true, "R", "ZAR", 2));
		Add(new RegionName
			(0x0437, "GE", "GEO", "GEO",
			 true, "Lari", "GEL", 2));
		Add(new RegionName
			(0x0438, "FO", "FRO", "FRO",
			 true, "kr", "DKK", 2));
		Add(new RegionName
			(0x0439, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x043E, "MY", "MYS", "MYS",
			 true, "R", "MYR", 2));
		Add(new RegionName
			(0x043F, "KZ", "KAZ", "KAZ",
			 true, "\u0422", "KZT", 2));
		Add(new RegionName
			(0x0440, "KG", "KGZ", "KGZ",
			 true, "\u0441\u043E\u043C", "KGS", 2));
		Add(new RegionName
			(0x0441, "KE", "KEN", "KEN",
			 false, "S", "KES", 2));
		Add(new RegionName
			(0x0443, "UZ", "UZB", "UZB",
			 true, "su'm", "UZS", 2));
		Add(new RegionName
			(0x0444, "TA", "TAT", "TAT",
			 true, "\u0440.", "RUR", 2));
		Add(new RegionName
			(0x0446, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x0447, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x0449, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x044A, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x044B, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x044E, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x044F, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x0450, "MN", "MNG", "MNG",
			 true, "\u20AE", "MNT", 2));
		Add(new RegionName
			(0x0456, "ES", "ESP", "ESP",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0457, "IN", "IND", "IND",
			 true, "\u0930\u0941", "INR", 2));
		Add(new RegionName
			(0x045A, "SY", "SYR", "SYR",
			 true, "\u0644.\u0633.\u200F", "SYP", 2));
		Add(new RegionName
			(0x0465, "MV", "MDV", "MDV",
			 true, "\u0783.", "MVR", 2));
		Add(new RegionName
			(0x0801, "IQ", "IRQ", "IRQ",
			 true, "\u062F.\u0639.\u200F", "IQD", 3));
		Add(new RegionName
			(0x0804, "CN", "CHN", "CHN",
			 true, "\uFFE5", "CNY", 2));
		Add(new RegionName
			(0x0807, "CH", "CHE", "CHE",
			 true, "SFr.", "CHF", 2));
		Add(new RegionName
			(0x0809, "GB", "GBR", "GBR",
			 true, "\u00A3", "GBP", 2));
		Add(new RegionName
			(0x080A, "MX", "MEX", "MEX",
			 true, "$", "MXN", 2));
		Add(new RegionName
			(0x080C, "BE", "BEL", "BEL",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0810, "CH", "CHE", "CHE",
			 true, "SFr.", "CHF", 2));
		Add(new RegionName
			(0x0813, "BE", "BEL", "BEL",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0814, "NO", "NOR", "NOR",
			 true, "kr", "NOK", 2));
		Add(new RegionName
			(0x0816, "PT", "PRT", "PRT",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x081A, "SP", "SPB", "SPB",
			 true, "Din.", "YUN", 2));
		Add(new RegionName
			(0x081D, "FI", "FIN", "FIN",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x082C, "AZ", "AZE", "AZE",
			 true, "man.", "AZM", 2));
		Add(new RegionName
			(0x083E, "BN", "BRN", "BRN",
			 true, "$", "BND", 2));
		Add(new RegionName
			(0x0843, "UZ", "UZB", "UZB",
			 true, "su'm", "UZS", 2));
		Add(new RegionName
			(0x0C01, "EG", "EGY", "EGY",
			 true, "\u062C.\u0645.\u200F", "EGP", 2));
		Add(new RegionName
			(0x0C04, "HK", "HKG", "HKG",
			 true, "HK$", "HKD", 2));
		Add(new RegionName
			(0x0C07, "AT", "AUT", "AUT",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0C09, "AU", "AUS", "AUS",
			 true, "$", "AUD", 2));
		Add(new RegionName
			(0x0C0A, "ES", "ESP", "ESP",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x0C0C, "CA", "CAN", "CAN",
			 true, "$", "CAD", 2));
		Add(new RegionName
			(0x0C1A, "SP", "SPB", "SPB",
			 true, "Din.", "YUN", 2));
		Add(new RegionName
			(0x1001, "LY", "LBY", "LBY",
			 true, "\u062F.\u0644.\u200F", "LYD", 3));
		Add(new RegionName
			(0x1004, "SG", "SGP", "SGP",
			 false, "$", "SGD", 2));
		Add(new RegionName
			(0x1007, "LU", "LUX", "LUX",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x1009, "CA", "CAN", "CAN",
			 true, "$", "CAD", 2));
		Add(new RegionName
			(0x100A, "GT", "GTM", "GTM",
			 true, "Q", "GTQ", 2));
		Add(new RegionName
			(0x100C, "CH", "CHE", "CHE",
			 true, "SFr.", "CHF", 2));
		Add(new RegionName
			(0x1401, "DZ", "DZA", "DZA",
			 true, "\u062F.\u062C.\u200F", "DZD", 2));
		Add(new RegionName
			(0x1404, "MO", "MAC", "MCO",
			 true, "P", "MOP", 2));
		Add(new RegionName
			(0x1407, "LI", "LIE", "LIE",
			 true, "CHF", "CHF", 2));
		Add(new RegionName
			(0x1409, "NZ", "NZL", "NZL",
			 true, "$", "NZD", 2));
		Add(new RegionName
			(0x140A, "CR", "CRI", "CRI",
			 true, "\u20A1", "CRC", 2));
		Add(new RegionName
			(0x140C, "LU", "LUX", "LUX",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x1801, "MA", "MAR", "MAR",
			 true, "\u062F.\u0645.\u200F", "MAD", 2));
		Add(new RegionName
			(0x1809, "IE", "IRL", "IRL",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x180A, "PA", "PAN", "PAN",
			 true, "B/.", "PAB", 2));
		Add(new RegionName
			(0x180C, "MC", "MCO", "MCO",
			 true, "\u20AC", "EUR", 2));
		Add(new RegionName
			(0x1C01, "TN", "TUN", "TUN",
			 true, "\u062F.\u062A.\u200F", "TND", 3));
		Add(new RegionName
			(0x1C09, "ZA", "ZAF", "ZAF",
			 true, "R", "ZAR", 2));
		Add(new RegionName
			(0x1C0A, "DO", "DOM", "DOM",
			 true, "RD$", "DOP", 2));
		Add(new RegionName
			(0x2001, "OM", "OMN", "OMN",
			 true, "\u0631.\u0639.\u200F", "OMR", 3));
		Add(new RegionName
			(0x2009, "JM", "JAM", "JAM",
			 false, "J$", "JMD", 2));
		Add(new RegionName
			(0x200A, "VE", "VEN", "VEN",
			 true, "Bs", "VEB", 2));
		Add(new RegionName
			(0x2401, "YE", "YEM", "YEM",
			 true, "\u0631.\u064A.\u200F", "YER", 2));
		Add(new RegionName
			(0x2409, "CB", "CAR", "CAR",
			 false, "$", "USD", 2));
		Add(new RegionName
			(0x240A, "CO", "COL", "COL",
			 true, "$", "COP", 2));
		Add(new RegionName
			(0x2801, "SY", "SYR", "SYR",
			 true, "\u0644.\u0633.\u200F", "SYP", 2));
		Add(new RegionName
			(0x2809, "BZ", "BLZ", "BLZ",
			 true, "BZ$", "BZD", 2));
		Add(new RegionName
			(0x280A, "PE", "PER", "PER",
			 true, "S/.", "PEN", 2));
		Add(new RegionName
			(0x2C01, "JO", "JOR", "JOR",
			 true, "\u062F.\u0627.\u200F", "JOD", 3));
		Add(new RegionName
			(0x2C09, "TT", "TTO", "TTO",
			 true, "TT$", "TTD", 0));
		Add(new RegionName
			(0x2C0A, "AR", "ARG", "ARG",
			 true, "$", "ARS", 2));
		Add(new RegionName
			(0x3001, "LB", "LBN", "LBN",
			 true, "\u0644.\u0644.\u200F", "LBP", 2));
		Add(new RegionName
			(0x3009, "ZW", "ZWE", "ZWE",
			 false, "Z$", "ZWD", 2));
		Add(new RegionName
			(0x300A, "EC", "ECU", "ECU",
			 true, "$", "USD", 2));
		Add(new RegionName
			(0x3401, "KW", "KWT", "KWT",
			 true, "\u062F.\u0643.\u200F", "KWD", 3));
		Add(new RegionName
			(0x3409, "PH", "PHL", "PHL",
			 false, "Php", "PHP", 2));
		Add(new RegionName
			(0x340A, "CL", "CHL", "CHL",
			 true, "$", "CLP", 0));
		Add(new RegionName
			(0x3801, "AE", "ARE", "ARE",
			 true, "\u062F.\u0625.\u200F", "AED", 2));
		Add(new RegionName
			(0x380A, "UY", "URY", "URY",
			 true, "$U", "UYU", 2));
		Add(new RegionName
			(0x3C01, "BH", "BHR", "BHR",
			 true, "\u062F.\u0628.\u200F", "BHD", 3));
		Add(new RegionName
			(0x3C0A, "PY", "PRY", "PRY",
			 true, "Gs", "PYG", 0));
		Add(new RegionName
			(0x4001, "QA", "QAT", "QAT",
			 true, "\u0631.\u0642.\u200F", "QAR", 2));
		Add(new RegionName
			(0x400A, "BO", "BOL", "BOL",
			 true, "$b", "BOB", 2));
		Add(new RegionName
			(0x440A, "SV", "SLV", "SLV",
			 true, "$", "USD", 2));
		Add(new RegionName
			(0x480A, "HN", "HND", "HND",
			 true, "L.", "HNL", 2));
		Add(new RegionName
			(0x4C0A, "NI", "NIC", "NIC",
			 true, "C$", "NIO", 2));
		Add(new RegionName
			(0x500A, "PR", "PRI", "PRI",
			 true, "$", "USD", 2));
	}

	// Create the region name table.
	public static void CreateNameTable()
			{
				lock(typeof(RegionNameTable))
				{
					// Return immediately if the name table already exists.
					if(regions != null)
					{
						return;
					}

					// Create a new region name table.
					regions = new RegionName [DefaultTableSize];
					numRegions = 0;

					// Populate the region name table.
					PopulateNameTable();
				}
			}

	// Get the name information for a specific region, by name.
	public static RegionName GetNameInfoByName(String name)
			{
				// Create the region name table.
				CreateNameTable();

				// Search for the name in the table.
				int posn = numRegions - 1;
				while(posn >= 0)
				{
					if(regions[posn].twoLetterISOName == name)
					{
						return regions[posn];
					}
					--posn;
				}

				// Could not find the region.
				return null;
			}

	// Get the name information for a specific region, by identifier.
	public static RegionName GetNameInfoByID(int regionID)
			{
				// Create the region name table.
				CreateNameTable();

				// Search for the name in the table.
				int posn = numRegions - 1;
				while(posn >= 0)
				{
					if(regions[posn].regionID == regionID)
					{
						return regions[posn];
					}
					--posn;
				}

				// Could not find the region.
				return null;
			}

	// Add currency information to a NumberFormatInfo object.
	public static void AddCurrencyInfo
				(NumberFormatInfo nfi, RootCulture culture)
			{
				String country = culture.Country;
				if(country != null)
				{
					RegionName region = GetNameInfoByName(country);
					if(region != null)
					{
						nfi.CurrencySymbol = region.currencySymbol;
						nfi.CurrencyDecimalDigits = region.currencyDigits;
					}
				}
			}

}; // class RegionNameTable

}; // namespace I18N.Common
