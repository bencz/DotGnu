/*
 * sysglobl.cs - Implementation of the "sysglobl.dll" assembly.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
using System.IO;
using System.Xml;

public enum CulturePrefix
{
	None		= 0,
	IANA		= 1,
	PrivateUse	= 2

}; // enum CulturePrefix

public sealed class CultureAndRegionInfoBuilder
{
	// Internal state.
	internal CultureInfo templateCulture;
	internal RegionInfo templateRegion;
	private Calendar[] availableCalendars;
	private CompareInfo compareInfo;
	private CultureInfo consoleFallbackUICulture;
	private String cultureName;
	private DateTimeFormatInfo dateTimeFormat;
	private bool isNeutralCulture;
	private int lcid;
#if CONFIG_FRAMEWORK_1_2
	private int keyboardLayoutID;
	private LineOrientation lineOrientation;
#endif
	private NumberFormatInfo numberFormat;
	private CultureInfo parent;
	private TextInfo textInfo;
#if CONFIG_REFLECTION
	private String cultureEnglishName;
	private String cultureNativeName;
	private String threeLetterISOLanguageName;
	private String threeLetterWindowsLanguageName;
	private String twoLetterISOLanguageName;
#endif
#if !ECMA_COMPAT
	internal int geoId;
	private bool isMetric;
	private String currencyEnglishName;
	private String currencyNativeName;
	private String currencySymbol;
	private String isoCurrencySymbol;
	private String regionEnglishName;
	private String regionName;
	private String regionNativeName;
	private String threeLetterISORegionName;
	private String threeLetterWindowsRegionName;
	private String twoLetterISORegionName;
#endif

	// Constructors.
#if !ECMA_COMPAT
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   RegionInfo templateRegion)
			: this(templateCulture, templateRegion,
				   String.Empty, String.Empty, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   RegionInfo templateRegion,
									   String language)
			: this(templateCulture, templateRegion,
				   language, String.Empty, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   RegionInfo templateRegion,
									   String language, String region)
			: this(templateCulture, templateRegion,
				   language, region, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   RegionInfo templateRegion,
									   String language, String region,
									   String suffix)
			: this(templateCulture, templateRegion,
				   language, region, suffix, CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   RegionInfo templateRegion,
									   String language, String region,
									   String suffix, CulturePrefix prefix)
#else
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   Object templateRegion)
			: this(templateCulture, templateRegion,
				   String.Empty, String.Empty, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   Object templateRegion,
									   String language)
			: this(templateCulture, templateRegion,
				   language, String.Empty, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   Object templateRegion,
									   String language, String region)
			: this(templateCulture, templateRegion,
				   language, region, String.Empty,
				   CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   Object templateRegion,
									   String language, String region,
									   String suffix)
			: this(templateCulture, templateRegion,
				   language, region, suffix, CulturePrefix.None) {}
	public CultureAndRegionInfoBuilder(CultureInfo templateCulture,
									   Object templateRegion,
									   String language, String region,
									   String suffix, CulturePrefix prefix)
#endif
			{
				if(templateCulture == null)
				{
					throw new ArgumentNullException("templateCulture");
				}
				if(templateRegion == null)
				{
					throw new ArgumentNullException("templateRegion");
				}

				// Copy the original property values out of the templates.
				availableCalendars = templateCulture.OptionalCalendars;
				consoleFallbackUICulture = templateCulture;
				cultureName = templateCulture.Name;
				dateTimeFormat = templateCulture.DateTimeFormat;
				isNeutralCulture = templateCulture.IsNeutralCulture;
			#if CONFIG_REFLECTION
				lcid = templateCulture.LCID;
			#else
				lcid = templateCulture.GetHashCode();
			#endif
				numberFormat = templateCulture.NumberFormat;
				parent = templateCulture.Parent;
				textInfo = templateCulture.TextInfo;
			#if CONFIG_FRAMEWORK_1_2
				keyboardLayoutID = templateCulture.KeyboardLayoutID;
				//lineOrientation = templateCulture.LineOrientation; // TODO
			#endif
			#if CONFIG_REFLECTION
				cultureEnglishName = templateCulture.EnglishName;
				cultureNativeName = templateCulture.NativeName;
				threeLetterISOLanguageName =
					templateCulture.ThreeLetterISOLanguageName;
				threeLetterWindowsLanguageName =
					templateCulture.ThreeLetterWindowsLanguageName;
				twoLetterISOLanguageName =
					templateCulture.TwoLetterISOLanguageName;
			#endif
			#if !ECMA_COMPAT
			#if CONFIG_FRAMEWORK_2_0
				geoId = templateRegion.GeoId;
			#else
				geoId = templateRegion.GetHashCode();
			#endif
			#if CONFIG_FRAMEWORK_2_0
				currencyEnglishName = templateRegion.CurrencyEnglishName;
				currencyNativeName = templateRegion.CurrencyNativeName;
			#endif
				currencySymbol = templateRegion.CurrencySymbol;
				isMetric = templateRegion.IsMetric;
				isoCurrencySymbol = templateRegion.ISOCurrencySymbol;
				regionEnglishName = templateRegion.EnglishName;
				regionName = templateRegion.Name;
				regionNativeName = templateRegion.DisplayName;
				threeLetterISORegionName =
					templateRegion.ThreeLetterISORegionName;
				threeLetterWindowsRegionName =
					templateRegion.ThreeLetterWindowsRegionName;
				twoLetterISORegionName =
					templateRegion.TwoLetterISORegionName;
			#endif

				// Override the names if necessary.
				String prefixValue;
				if(prefix == CulturePrefix.IANA)
				{
					prefixValue = "i-";
				}
				else if(prefix == CulturePrefix.PrivateUse)
				{
					prefixValue = "x-";
				}
				else
				{
					prefixValue = "";
				}
				if(language == null || language.Length == 0)
				{
					language = cultureName;
				}
				cultureName = prefixValue + language + suffix;
			#if CONFIG_REFLECTION
				cultureEnglishName = cultureName;
				cultureNativeName = cultureName;
			#endif
			#if !ECMA_COMPAT
				if(region == null || region.Length == 0)
				{
					region = regionName;
				}
				regionName = prefixValue + region + suffix;
				regionEnglishName = regionName;
				regionNativeName = regionName;
			#endif
			}

	// Get or set this object's culture properties.
	public Calendar[] AvailableCalendars
			{
				get
				{
					return availableCalendars;
				}
				set
				{
					availableCalendars = value;
				}
			}
	public CompareInfo CompareInfo
			{
				get
				{
					return compareInfo;
				}
				set
				{
					compareInfo = value;
				}
			}
	public CultureInfo ConsoleFallbackUICulture
			{
				get
				{
					return consoleFallbackUICulture;
				}
				set
				{
					consoleFallbackUICulture = value;
				}
			}
	public String CultureName
			{
				get
				{
					return cultureName;
				}
			}
#if CONFIG_REFLECTION
	public String CultureEnglishName
			{
				get
				{
					return cultureEnglishName;
				}
				set
				{
					cultureEnglishName = value;
				}
			}
	public String CultureNativeName
			{
				get
				{
					return cultureNativeName;
				}
				set
				{
					cultureNativeName = value;
				}
			}
#endif
	public DateTimeFormatInfo DateTimeFormat
			{
				get
				{
					return dateTimeFormat;
				}
				set
				{
					dateTimeFormat = value;
				}
			}
	public bool IsNeutralCulture
			{
				get
				{
					return isNeutralCulture;
				}
			}
	public int LCID
			{
				get
				{
					return lcid;
				}
			}
#if CONFIG_FRAMEWORK_1_2
	public int KeyboardLayoutID
			{
				get
				{
					return keyboardLayoutID;
				}
				set
				{
					keyboardLayoutID = value;
				}
			}
	public LineOrientation LineOrientation
			{
				get
				{
					return lineOrientation;
				}
				set
				{
					lineOrientation = value;
				}
			}
#endif
	public NumberFormatInfo NumberFormat
			{
				get
				{
					return numberFormat;
				}
				set
				{
					numberFormat = value;
				}
			}
	public CultureInfo Parent
			{
				get
				{
					return parent;
				}
				set
				{
					parent = value;
				}
			}
	public TextInfo TextInfo
			{
				get
				{
					return textInfo;
				}
				set
				{
					textInfo = value;
				}
			}
#if CONFIG_REFLECTION
	public String ThreeLetterISOLanguageName
			{
				get
				{
					return threeLetterISOLanguageName;
				}
				set
				{
					threeLetterISOLanguageName = value;
				}
			}
	public String ThreeLetterWindowsLanguageName
			{
				get
				{
					return threeLetterWindowsLanguageName;
				}
				set
				{
					threeLetterWindowsLanguageName = value;
				}
			}
	public String TwoLetterISOLanguageName
			{
				get
				{
					return twoLetterISOLanguageName;
				}
				set
				{
					twoLetterISOLanguageName = value;
				}
			}
#endif

#if !ECMA_COMPAT

	// Get or set this object's region properties.
	public String CurrencySymbol
			{
				get
				{
					return currencySymbol;
				}
				set
				{
					currencySymbol = value;
				}
			}
	public bool IsMetric
			{
				get
				{
					return isMetric;
				}
				set
				{
					isMetric = value;
				}
			}
	public String ISOCurrencySymbol
			{
				get
				{
					return isoCurrencySymbol;
				}
				set
				{
					isoCurrencySymbol = value;
				}
			}
	public String RegionEnglishName
			{
				get
				{
					return regionEnglishName;
				}
				set
				{
					regionEnglishName = value;
				}
			}
	public String RegionName
			{
				get
				{
					return regionName;
				}
			}
	public String RegionNativeName
			{
				get
				{
					return regionNativeName;
				}
				set
				{
					regionNativeName = value;
				}
			}
	public String ThreeLetterISORegionName
			{
				get
				{
					return threeLetterISORegionName;
				}
				set
				{
					threeLetterISORegionName = value;
				}
			}
	public String ThreeLetterWindowsRegionName
			{
				get
				{
					return threeLetterWindowsRegionName;
				}
				set
				{
					threeLetterWindowsRegionName = value;
				}
			}
	public String TwoLetterISORegionName
			{
				get
				{
					return twoLetterISORegionName;
				}
				set
				{
					twoLetterISORegionName = value;
				}
			}
#if CONFIG_FRAMEWORK_2_0
	public int GeoId
			{
				get
				{
					return geoId;
				}
				set
				{
					geoId = value;
				}
			}
	public String CurrencyEnglishName
			{
				get
				{
					return currencyEnglishName;
				}
				set
				{
					currencyEnglishName = value;
				}
			}
	public String CurrencyNativeName
			{
				get
				{
					return currencyNativeName;
				}
				set
				{
					currencyNativeName = value;
				}
			}
#endif

#endif // !ECMA_COMPAT

	// Create a user-defined culture from the current state of this builder.
	// We clone the builder first, to prevent later changes from affecting
	// the state of the user-defined culture.
	private CultureInfo CreateCulture()
			{
				CultureAndRegionInfoBuilder builder;
				builder = (CultureAndRegionInfoBuilder)MemberwiseClone();
				return new UserDefinedCultureInfo(builder);
			}

#if !ECMA_COMPAT

	// Create a user-defined region from the current state of this builder.
	// We clone the builder first, to prevent later changes from affecting
	// the state of the user-defined region.
	private RegionInfo CreateRegion()
			{
				CultureAndRegionInfoBuilder builder;
				builder = (CultureAndRegionInfoBuilder)MemberwiseClone();
				return new UserDefinedRegionInfo(builder);
			}

#endif // !ECMA_COMPAT

	// Register the culture that is represented by this builder.
	public void Register()
			{
			#if !ECMA_COMPAT
				_I18NRegistration.Register
					(CreateCulture(), ConsoleFallbackUICulture,
					 CreateRegion());
			#else
				_I18NRegistration.Register
					(CreateCulture(), ConsoleFallbackUICulture);
			#endif
			}
	public static void Register(CultureAndRegionInfoBuilder builder)
			{
				builder.Register();
			}
	public static void Register(String xmlFile)
			{
				// For compatibility with .NET SDK 2.0!
				throw new NotImplementedException();
			}

	// Save this culture to a stream.
	public void Save(String filename)
			{
				// For compatibility with .NET SDK 2.0!
				throw new NotImplementedException();
			}
	public void Save(Stream stream)
			{
				// For compatibility with .NET SDK 2.0!
				throw new NotImplementedException();
			}

	// Unregister a particular culture.
	public static void Unregister(String cultureName)
			{
				Unregister(cultureName, false);
			}
	public static void Unregister(String cultureName, bool force)
			{
				_I18NRegistration.Unregister(cultureName);
			}

}; // class CultureAndRegionInfoBuilder

internal class UserDefinedCultureInfo : CultureInfo
{
	// Internal state.
	private CultureAndRegionInfoBuilder builder;

	// Constructor.
	public UserDefinedCultureInfo(CultureAndRegionInfoBuilder builder)
#if CONFIG_REFLECTION
			: base(builder.templateCulture.LCID,
				   builder.templateCulture.UseUserOverride)
#else
			: base(builder.templateCulture.GetHashCode(),
				   builder.templateCulture.UseUserOverride)
#endif
			{
				this.builder = builder;
			}

	// Get the comparison rules that are used by the culture.
	public override CompareInfo CompareInfo
			{
				get
				{
					return builder.CompareInfo;
				}
			}

	// Get or set the date and time formatting information for this culture.
	public override DateTimeFormatInfo DateTimeFormat
			{
				get
				{
					return builder.DateTimeFormat;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					builder.DateTimeFormat = value;
				}
			}

#if CONFIG_REFLECTION

	// Get the display name for this culture.
	public override String DisplayName
			{
				get
				{
					return builder.CultureNativeName;
				}
			}

	// Get the English display name for this culture.
	public override String EnglishName
			{
				get
				{
					return builder.CultureEnglishName;
				}
			}

	// Determine if this is a neutral culture.
	public override bool IsNeutralCulture
			{
				get
				{
					return builder.IsNeutralCulture;
				}
			}

	// Get the culture identifier for this instance.
	public override int LCID
			{
				get
				{
					return builder.LCID;
				}
			}

	// Get the culture name.
	public override String Name
			{
				get
				{
					return builder.CultureName;
				}
			}

	// Get the culture's full name in the native language.
	public override String NativeName
			{
				get
				{
					return builder.CultureNativeName;
				}
			}

#else // !CONFIG_REFLECTION

	// Get the culture name.
	public override String Name
			{
				get
				{
					return builder.CultureName;
				}
			}

#endif // !CONFIG_REFLECTION

	// Get or set the number formatting information for this culture.
	public override NumberFormatInfo NumberFormat
			{
				get
				{
					return builder.NumberFormat;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					builder.NumberFormat = value;
				}
			}

	// Get the optional calendars for this instance.
	public override Calendar[] OptionalCalendars
			{
				get
				{
					return builder.AvailableCalendars;
				}
			}

	// Get the parent culture.
	public override CultureInfo Parent
			{
				get
				{
					return builder.Parent;
				}
			}

	// Get the text writing system associated with this culture.
	public override TextInfo TextInfo
			{
				get
				{
					return builder.TextInfo;
				}
			}

#if CONFIG_REFLECTION

	// Get the 3-letter ISO language name for this culture.
	public override String ThreeLetterISOLanguageName
			{
				get
				{
					return builder.ThreeLetterISOLanguageName;
				}
			}

	// Get the 3-letter Windows language name for this culture.
	public override String ThreeLetterWindowsLanguageName
			{
				get
				{
					return builder.ThreeLetterWindowsLanguageName;
				}
			}

	// Get the 2-letter ISO language name for this culture.
	public override String TwoLetterISOLanguageName
			{
				get
				{
					return builder.TwoLetterISOLanguageName;
				}
			}

#endif // CONFIG_REFLECTION

#if CONFIG_FRAMEWORK_1_2

	// Get the keyboard layout identifier for this culture.
	public override int KeyboardLayoutID
			{
				get
				{
					return builder.KeyboardLayoutID;
				}
			}

#endif // CONFIG_FRAMEWORK_1_2

	// Implementation of the ICloneable interface.
	public override Object Clone()
			{
				return new UserDefinedCultureInfo(builder);
			}

	// Determine if two culture objects are equal.
	public override bool Equals(Object obj)
			{
				UserDefinedCultureInfo other = (obj as UserDefinedCultureInfo);
				if(other != null)
				{
					return (other.Name == Name);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return builder.LCID;
			}

}; // class UserDefinedCultureInfo

#if !ECMA_COMPAT

internal class UserDefinedRegionInfo : RegionInfo
{
	// Internal state.
	private CultureAndRegionInfoBuilder builder;

	// Constructor.
	public UserDefinedRegionInfo(CultureAndRegionInfoBuilder builder)
			: base(-1)	// Don't try to recursively load an I18N handler.
			{
				this.builder = builder;
			}

	// Get the region properties.
	public override String CurrencySymbol
			{
				get
				{
					return builder.templateRegion.CurrencySymbol;
				}
			}
	public override String DisplayName
			{
				get
				{
					return builder.RegionNativeName;
				}
			}
	public override String EnglishName
			{
				get
				{
					return builder.RegionEnglishName;
				}
			}
	public override bool IsMetric
			{
				get
				{
					return builder.IsMetric;
				}
			}
	public override String ISOCurrencySymbol
			{
				get
				{
					return builder.ISOCurrencySymbol;
				}
			}
	public override String Name
			{
				get
				{
					return builder.RegionName;
				}
			}
	public override String ThreeLetterISORegionName
			{
				get
				{
					return builder.ThreeLetterISORegionName;
				}
			}
	public override String ThreeLetterWindowsRegionName
			{
				get
				{
					return builder.ThreeLetterWindowsRegionName;
				}
			}
	public override String TwoLetterISORegionName
			{
				get
				{
					return builder.TwoLetterISORegionName;
				}
			}
#if CONFIG_FRAMEWORK_2_0
	public override String CurrencyEnglishName
			{
				get
				{
					return builder.CurrencyEnglishName;
				}
			}
	public override String CurrencyNativeName
			{
				get
				{
					return builder.CurrencyNativeName;
				}
			}
	public override int GeoId
			{
				get
				{
					return builder.GeoId;
				}
			}
#endif

	// Determine if two region information objects are equal.
	public override bool Equals(Object obj)
			{
				UserDefinedRegionInfo other = (obj as UserDefinedRegionInfo);
				if(other != null)
				{
					return (other.builder.geoId == builder.geoId);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return builder.geoId;
			}

}; // class UserDefinedRegionInfo

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
