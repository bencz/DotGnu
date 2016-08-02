/*
 * setlocale.c - Set the current locale.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include <langinfo.h>

/*
 * Import the culture definitions from the C# library.
 */
__using__ System::Globalization::CultureInfo;
__using__ System::Runtime::InteropServices::Marshal;
__using__ System::String;

/*
 * Cached copy of the locale culture name.
 */
static __declspec(thread) char *locale_name;

char *
setlocale (int category, const char *locale)
{
  String *name;
  /* Note: we don't currently support changing the locale to something
     other than the C#-defined one.  It's easier this way, and is normally
	 all that anyone wants to use anyway.  Fix later if there is demand */
  if (locale_name)
    return locale_name;
  name = CultureInfo::CurrentCulture.Name;
  locale_name = (char *)Marshal::StringToHGlobalAnsi (name);
  return locale_name;
}
