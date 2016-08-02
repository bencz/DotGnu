/*
 * UserPreferenceCategory.cs - Implementation of the
 *		Microsoft.Win32.UserPreferenceCategory class.
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

namespace Microsoft.Win32
{

#if CONFIG_WIN32_SPECIFICS

public enum UserPreferenceCategory
{
	Accessibility	= 1,
	Color			= 2,
	Desktop			= 3,
	General			= 4,
	Icon			= 5,
	Keyboard		= 6,
	Menu			= 7,
	Mouse			= 8,
	Policy			= 9,
	Power			= 10,
	Screensaver		= 11,
	Window			= 12,
	Locale			= 13

}; // enum UserPreferenceCategory

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
