/*
 * _I18NRegistration.cs - Implementation of the
 *		"System.Globalization._I18NRegistration" class.
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

// This class exists to allow "sysglobl.dll" to register and unregister
// cultures that it has built manually based on user definitions.

[CLSCompliant(false)]
[NonStandardExtra]
public sealed class _I18NRegistration
{
	// Cannot construct an instance of this class.
	private _I18NRegistration() {}

	// Register a culture and region with the current thread.
#if !ECMA_COMPAT
	public static void Register(CultureInfo culture, CultureInfo uiCulture,
							    RegionInfo region)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				if(uiCulture == null)
				{
					throw new ArgumentNullException("uiCulture");
				}
				if(region == null)
				{
					throw new ArgumentNullException("region");
				}
				CultureInfo.SetCurrentCulture(culture);
				CultureInfo.SetCurrentUICulture(uiCulture);
				RegionInfo.currentRegion = region;
			}
#else
	public static void Register(CultureInfo culture, CultureInfo uiCulture)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				if(uiCulture == null)
				{
					throw new ArgumentNullException("uiCulture");
				}
				CultureInfo.SetCurrentCulture(culture);
				CultureInfo.SetCurrentUICulture(uiCulture);
			}
#endif

	// Unregister the thread's current culture if it is called "name".
	public static void Unregister(String name)
			{
				if(CultureInfo.CurrentCulture.Name == name)
				{
					// Set the culture to null, which will force a
					// regular culture object to be created upon the
					// next request for CultureInfo.
					CultureInfo.SetCurrentCulture(null);
					CultureInfo.SetCurrentUICulture(null);
				}
			}

}; // class _I18NRegistration

}; // namespace System.Globalization
