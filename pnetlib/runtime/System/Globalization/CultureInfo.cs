/*
 * CultureInfo.cs - Implementation of "System.Globalization.CultureInfo".
 *
 * Copyright (C) 2001, 2003, 2004  Southern Storm Software, Pty Ltd.
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
using System.Runtime.CompilerServices;
using System.Text;

// Note: if we don't have reflection, then we cannot load I18N handlers,
// and all "CultureInfo" objects act like the invariant culture.

public class CultureInfo : ICloneable, IFormatProvider
{

	// Cached culture objects.
	private static CultureInfo invariantCulture;
	[ThreadStatic] private static CultureInfo currentCulture;
	[ThreadStatic] private static CultureInfo currentUICulture;
#if CONFIG_REFLECTION
	private static CultureInfo globalCulture;
	private static bool gettingCurrent;
#endif

	// Internal state.
	private int         cultureID;
	private bool		readOnly;
	private Calendar	calendar;
	private Calendar[]	otherCalendars;
	private NumberFormatInfo numberFormat;
	private CompareInfo compareInfo;
	private DateTimeFormatInfo dateTimeFormat;
	private TextInfo	textInfo;
	private bool        userOverride;
#if CONFIG_REFLECTION
	private CultureInfo handler;

	// Culture identifier for "es-ES" with traditional sort rules.
	private const int TraditionalSpanish = 0x040A;
#endif

	// Constructors.
	public CultureInfo(int culture)
			: this(culture, true)
			{
				// Nothing to do here.
			}
	public CultureInfo(String name)
			: this(name, true)
			{
				// Nothing to do here.
			}
	public CultureInfo(int culture, bool useUserOverride)
			{
				if(culture < 0)
				{
					throw new ArgumentOutOfRangeException
						("culture", _("ArgRange_NonNegative"));
				}
				if((culture & 0x40000000) != 0)
				{
					// This flag is a special indication from the I18N
					// library that this object is a culture handler
					// and we should not recursively load another culture.
					this.cultureID = (culture & ~0x40000000);
					return;
				}
			#if CONFIG_REFLECTION
				if(culture == TraditionalSpanish)
				{
					cultureID   = culture;
					handler = GetCultureHandler(cultureID, useUserOverride);
				}
				else if(culture == 0x007F)
				{
					cultureID   = 0x007F;
				}
				else
				{
					cultureID   = culture;
					handler = GetCultureHandler(cultureID, useUserOverride);
				}
			#else
				cultureID = culture;
			#endif
				userOverride = useUserOverride;
			}
	public CultureInfo(String name, bool useUserOverride)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
			#if CONFIG_REFLECTION
				userOverride = useUserOverride;
				if( name == string.Empty ) {
					cultureID = 0x007F;
				}
				else {
					handler = GetCultureHandler(name, useUserOverride);
					cultureID = handler.LCID;
				}
			#else
				cultureID = 0x007F;
				userOverride = useUserOverride;
			#endif
			}
#if CONFIG_REFLECTION
	private CultureInfo(CultureInfo handler, bool useUserOverride)
			{
				this.handler = handler;
				this.userOverride = useUserOverride;
				this.cultureID = handler.LCID;
			}
#endif

	// Get the invariant culture object.
	public static CultureInfo InvariantCulture
			{
				get
				{
					lock(typeof(CultureInfo))
					{
						if(invariantCulture != null)
						{
							return invariantCulture;
						}
						invariantCulture = new CultureInfo(0x007F);
						invariantCulture.readOnly = true;
						return invariantCulture;
					}
				}
			}

#if CONFIG_REFLECTION

	// Get the current culture identifier from the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int InternalCultureID();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String InternalCultureName();

	// Get the global culture object for the running process.
	private static CultureInfo GlobalCulture
			{
				get
				{
					lock(typeof(CultureInfo))
					{
						if(globalCulture != null)
						{
							return globalCulture;
						}
						if(gettingCurrent)
						{
							// We were recursively called during initialization,
							// so return the invariant culture for now.
							return InvariantCulture;
						}
						gettingCurrent = true;
						int id = InternalCultureID();
						CultureInfo handler;
						if(id <= 0)
						{
							// Try getting by name instead, in case the
							// engine doesn't know about culture ID's.
							String name = InternalCultureName();
							if(name != null)
							{
								handler = GetCultureHandler(name, true);
							}
							else
							{
								handler = null;
							}
						}
						else if(id == 0x007F)
						{
							handler = null;
						}
						else
						{
							handler = GetCultureHandler(id, true);
						}
						if(handler == null)
						{
							// Could not find a handler, so use invariant.
							globalCulture = InvariantCulture;
							gettingCurrent = false;
							return globalCulture;
						}
						globalCulture = new CultureInfo(handler, true);
						globalCulture.readOnly = true;
						gettingCurrent = false;
						return globalCulture;
					}
				}
			}

#else // !CONFIG_REFLECTION

	// Get the global culture object for the running process.
	private static CultureInfo GlobalCulture
			{
				get
				{
					// The invariant culture is the only one in the system.
					return InvariantCulture;
				}
			}

#endif // !CONFIG_REFLECTION

	// Get the current culture object for the running thread.
	public static CultureInfo CurrentCulture
			{
				get
				{
					if(currentCulture == null)
					{
						currentCulture = GlobalCulture;
					}
					return currentCulture;
				}
			}

	// Set the current culture.
	internal static void SetCurrentCulture(CultureInfo value)
			{
				currentCulture = value;
			#if !ECMA_COMPAT
				RegionInfo.currentRegion = null;	// Recreate on next call.
			#endif
			}

	// Get the current UI culture object for the running thread.
	public static CultureInfo CurrentUICulture
			{
				get
				{
					if(currentUICulture == null)
					{
						currentUICulture = GlobalCulture;
					}
					return currentUICulture;
				}
			}

	// Set the current UI culture.
	internal static void SetCurrentUICulture(CultureInfo value)
			{
				currentUICulture = value;
			}

	// Get the installed UI culture object for the system.
	public static CultureInfo InstalledUICulture
			{
				get
				{
					return GlobalCulture;
				}
			}

#if !ECMA_COMPAT
	// Create a CultureInfo object for a specific culture.
	public static CultureInfo CreateSpecificCulture(String name)
			{
				CultureInfo culture = new CultureInfo(name);
				return culture;
			}

	// Get the list of all cultures supported by this system.
	public static CultureInfo[] GetCultures(CultureTypes types)
			{
				Object obj = Encoding.InvokeI18N("GetCultures", types);
				if(obj != null)
				{
					return (CultureInfo[])obj;
				}
				else if((types & CultureTypes.NeutralCultures) != 0)
				{
					CultureInfo[] cultures = new CultureInfo [1];
					cultures[0] = InvariantCulture;
					return cultures;
				}
				else
				{
					return new CultureInfo [0];
				}
			}

	// Wrap a culture information object to make it read-only.
	public static CultureInfo ReadOnly(CultureInfo ci)
			{
				if(ci == null)
				{
					throw new ArgumentNullException("ci");
				}
				if(ci.IsReadOnly)
				{
					return ci;
				}
				else
				{
					CultureInfo culture = (CultureInfo)(ci.MemberwiseClone());
					culture.readOnly = true;
					return culture;
				}
			}
#endif

	// Get the default calendar that is used by the culture.
	public virtual Calendar Calendar
			{
				get
				{
					lock(this)
					{
						if(calendar == null)
						{
						#if CONFIG_REFLECTION
							if(handler != null)
							{
								calendar = handler.Calendar;
								if(calendar == null)
								{
									calendar = new GregorianCalendar();
								}
							}
							else
						#endif
							{
								calendar = new GregorianCalendar();
							}
						}
						return calendar;
					}
				}
			}

	// Get the comparison rules that are used by the culture.
	public virtual CompareInfo CompareInfo
			{
				get
				{
					lock(this)
					{
						if(compareInfo == null)
						{
						#if CONFIG_REFLECTION
							if(handler != null)
							{
								compareInfo = handler.CompareInfo;
								if(compareInfo == null)
								{
									compareInfo =
										CompareInfo.InvariantCompareInfo;
								}
							}
							else
						#endif
							{
								compareInfo = CompareInfo.InvariantCompareInfo;
							}
						}
						return compareInfo;
					}
				}
			}

	// Get or set the date and time formatting information for this culture.
	public virtual DateTimeFormatInfo DateTimeFormat
			{
				get
				{
					if(dateTimeFormat == null)
					{
						if(cultureID == 0x007F && readOnly)
						{
							// This is the invariant culture, so always
							// return the invariant date time format info.
							return DateTimeFormatInfo.InvariantInfo;
						}
					#if CONFIG_REFLECTION
						else if(handler != null)
						{
							dateTimeFormat = handler.DateTimeFormat;
							if(dateTimeFormat == null)
							{
								dateTimeFormat = new DateTimeFormatInfo();
							}
						}
						else
					#endif
						{
							dateTimeFormat = new DateTimeFormatInfo();
						}
					}
					if(readOnly)
					{
						// Wrap up the date/time formatting information
						// to make it read-only like the culture.
						dateTimeFormat = DateTimeFormatInfo.ReadOnly
							(dateTimeFormat);
					}
					return dateTimeFormat;
				}
				set
				{
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					dateTimeFormat = value;
				}
			}

#if CONFIG_REFLECTION

	// Get the display name for this culture.
	public virtual String DisplayName
			{
				get
				{
					if(handler != null)
					{
						return handler.DisplayName;
					}
					return "Invariant Language (Invariant Country)";
				}
			}

	// Get the English display name for this culture.
	public virtual String EnglishName
			{
				get
				{
					if(handler != null)
					{
						return handler.EnglishName;
					}
					return "Invariant Language (Invariant Country)";
				}
			}

	// Determine if this is a neutral culture.
	public virtual bool IsNeutralCulture
			{
				get
				{
					return ((cultureID & ~0x3FF) == 0);
				}
			}

	// Get the culture identifier for this instance.
	public virtual int LCID
			{
				get
				{
					return cultureID;
				}
			}

	// Get the culture name.
	public virtual String Name
			{
				get
				{
					if(handler != null)
					{
						return handler.Name;
					}
					return "";
				}
			}

	// Get the culture's full name in the native language.
	public virtual String NativeName
			{
				get
				{
					if(handler != null)
					{
						return handler.NativeName;
					}
					return "Invariant Language (Invariant Country)";
				}
			}

#else // !CONFIG_REFLECTION

	// Get the culture name.
	public virtual String Name
			{
				get
				{
					// All cultures are invariant if no reflection.
					return "";
				}
			}

#endif // !CONFIG_REFLECTION

	// Determine if this culture instance is read-only.
	public bool IsReadOnly
			{
				get
				{
					return readOnly;
				}
			}

#if CONFIG_FRAMEWORK_1_2

	// Get the keyboard layout identifier for this culture.
	public virtual int KeyboardLayoutID
			{
				get
				{
					if(handler != null)
					{
						return handler.KeyboardLayoutID;
					}
					return 0x0C09;	// US 101 keyboard.
				}
			}

#endif // CONFIG_FRAMEWORK_1_2

	// Get or set the number formatting information for this culture.
	public virtual NumberFormatInfo NumberFormat
			{
				get
				{
					if(numberFormat == null)
					{
						if(cultureID == 0x007F && readOnly)
						{
							// This is the invariant culture, so always
							// return the invariant number format info.
							return NumberFormatInfo.InvariantInfo;
						}
					#if CONFIG_REFLECTION
						else if(handler != null)
						{
							numberFormat = handler.NumberFormat;
							if(numberFormat == null)
							{
								numberFormat = new NumberFormatInfo();
							}
						}
						else
					#endif
						{
							numberFormat = new NumberFormatInfo();
						}
					}
					if(readOnly)
					{
						// Wrap up the number formatting information
						// to make it read-only like the culture.
						numberFormat = NumberFormatInfo.ReadOnly
							(numberFormat);
					}
					return numberFormat;
				}
				set
				{
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					numberFormat = value;
				}
			}

	// Get the optional calendars for this instance.
	public virtual Calendar[] OptionalCalendars
			{
				get
				{
					lock(this)
					{
						if(otherCalendars == null)
						{
						#if CONFIG_REFLECTION
							if(handler != null)
							{
								otherCalendars = handler.OptionalCalendars;
							}
							else
						#endif
							{
								otherCalendars = new Calendar [0];
							}
						}
						return otherCalendars;
					}
				}
			}

	// Get the parent culture.
	public virtual CultureInfo Parent
			{
				get
				{
					if((cultureID & ~0x03FF) == 0)
					{
						return InvariantCulture;
					}
					else
					{
						return new CultureInfo(cultureID & 0x03FF);
					}
				}
			}

	// Get the text writing system associated with this culture.
	public virtual TextInfo TextInfo
			{
				get
				{
					lock(this)
					{
						if(textInfo == null)
						{
						#if CONFIG_REFLECTION
							if(handler != null)
							{
								textInfo = handler.TextInfo;
								if(textInfo == null)
								{
									textInfo = new TextInfo(cultureID);
								}
							}
							else
						#endif
							{
								textInfo = new TextInfo(cultureID);
							}
						}
						return textInfo;
					}
				}
			}

#if CONFIG_REFLECTION

	// Get the 3-letter ISO language name for this culture.
	public virtual String ThreeLetterISOLanguageName
			{
				get
				{
					if(handler != null)
					{
						return handler.ThreeLetterISOLanguageName;
					}
					return "IVL";
				}
			}

	// Get the 3-letter Windows language name for this culture.
	public virtual String ThreeLetterWindowsLanguageName
			{
				get
				{
					if(handler != null)
					{
						return handler.ThreeLetterWindowsLanguageName;
					}
					return "IVL";
				}
			}

	// Get the 2-letter ISO language name for this culture.
	public virtual String TwoLetterISOLanguageName
			{
				get
				{
					if(handler != null)
					{
						return handler.TwoLetterISOLanguageName;
					}
					return "iv";
				}
			}

#endif // CONFIG_REFLECTION

	// Determine if this culture is configured for user overrides.
	public bool UseUserOverride
			{
				get
				{
					return userOverride;
				}
			}

	// Implementation of the ICloneable interface.
	public virtual Object Clone()
			{
			#if !ECMA_COMPAT
				CultureInfo culture = (CultureInfo)(MemberwiseClone());
				culture.readOnly = false;
				// clone DateTimeFormat and NumberFormat if available
				if(dateTimeFormat != null)
				{
					culture.dateTimeFormat = (DateTimeFormatInfo)dateTimeFormat.Clone();
				}
				if(numberFormat != null)
				{
					culture.numberFormat = (NumberFormatInfo)numberFormat.Clone();
				}
				return culture;
			#else
				return MemberwiseClone();
			#endif
			}

	// Implementation of the IFormatProvider interface.
	public virtual Object GetFormat(Type formatType)
			{
				if(formatType ==
						typeof(System.Globalization.DateTimeFormatInfo))
				{
					return DateTimeFormat;
				}
				else if(formatType ==
							typeof(System.Globalization.NumberFormatInfo))
				{
					return NumberFormat;
				}
				else
				{
					return null;
				}
			}

	// Clear any cached culture data in this object.
	public void ClearCachedData()
			{
				// Nothing to do here.
			}

	// Determine if two culture objects are equal.
	public override bool Equals(Object obj)
			{
				CultureInfo other = (obj as CultureInfo);
				if(other != null)
				{
					return (other.cultureID == cultureID);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return cultureID;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return Name;
			}

#if CONFIG_REFLECTION

	// Get the culture handler for a specific culture.
	internal static CultureInfo GetCultureHandler
				(int culture, bool useUserOverride)
			{
				Object obj = Encoding.InvokeI18N
						("GetCulture", culture, useUserOverride);
				if(obj == null)
				{
					// Try the neutral culture instead.
					obj = Encoding.InvokeI18N
						("GetCulture", culture & 0x03FF, useUserOverride);
				}
				return (CultureInfo)obj;
			}
	internal static CultureInfo GetCultureHandler
				(String culture, bool useUserOverride)
			{
				Object obj = Encoding.InvokeI18N
						("GetCulture", culture, useUserOverride);
				return (CultureInfo)obj;
			}

#endif // CONFIG_REFLECTION

}; // class CultureInfo

}; // namespace System.Globalization
