/*
 * nl_langinfo.c - Get information about the current language.
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
#include <stdlib.h>

/*
 * Import the culture definitions from the C# library.
 */
__using__ System::Globalization::CultureInfo;
__using__ System::Globalization::DateTimeFormatInfo;
__using__ System::Globalization::NumberFormatInfo;
__using__ System::Runtime::InteropServices::Marshal;
__using__ System::Text::Encoding;
__using__ System::String;
__using__ System::DayOfWeek;

/*
 * Table of loaded values for the current thread.
 */
typedef struct nl_loaded nl_loaded;
struct nl_loaded
  {
    nl_item item;
    char *value;
    nl_loaded *next;
  };
static __declspec(thread) nl_loaded *loaded;

char *
nl_langinfo (nl_item item)
{
  nl_loaded *current;
  char *value;
  String *svalue;
  CultureInfo *culture;

  /* See if we already have a cached value for this item */
  current = loaded;
  while (current != 0)
    {
      if (current->item == item)
        return current->value;
      current = current->next;
    }

  /* Fetch the value from the thread's current culture */
  value = 0;
  svalue = 0;
  culture = CultureInfo::CurrentCulture;
  switch (item)
    {
      case CODESET:
      	svalue = Encoding::Default.BodyName;
        break;

      case ABDAY_1: case ABDAY_2: case ABDAY_3: case ABDAY_4:
      case ABDAY_5: case ABDAY_6: case ABDAY_7:
      	svalue = culture.DateTimeFormat.GetAbbreviatedDayName
		((DayOfWeek)(item - ABDAY_1));
	break;

      case DAY_1: case DAY_2: case DAY_3: case DAY_4:
      case DAY_5: case DAY_6: case DAY_7:
      	svalue = culture.DateTimeFormat.GetDayName
		((DayOfWeek)(item - DAY_1));
	break;

      case ABMON_1: case ABMON_2: case ABMON_3: case ABMON_4:
      case ABMON_5: case ABMON_6: case ABMON_7: case ABMON_8:
      case ABMON_9: case ABMON_10: case ABMON_11: case ABMON_12:
      	svalue = culture.DateTimeFormat.GetAbbreviatedMonthName
		((int)(item - ABMON_1 + 1));
	break;

      case MON_1: case MON_2: case MON_3: case MON_4:
      case MON_5: case MON_6: case MON_7: case MON_8:
      case MON_9: case MON_10: case MON_11: case MON_12:
      	svalue = culture.DateTimeFormat.GetMonthName
		((int)(item - MON_1 + 1));
	break;

      case AM_STR:
      	svalue = culture.DateTimeFormat.AMDesignator;
	break;

      case PM_STR:
      	svalue = culture.DateTimeFormat.PMDesignator;
	break;

      /* TODO: date and time formats */

      case __INT_CURR_SYMBOL:
      	svalue = S"\u00A4";
      	break;

      case __CURRENCY_SYMBOL:
      	svalue = culture.NumberFormat.CurrencySymbol;
	break;

      case __MON_DECIMAL_POINT:
      	svalue = culture.NumberFormat.CurrencyDecimalSeparator;
	break;

      case __MON_THOUSANDS_SEP:
      	svalue = culture.NumberFormat.CurrencyGroupSeparator;
	break;

      case __POSITIVE_SIGN:
      	svalue = culture.NumberFormat.PositiveSign;
	break;

      case __NEGATIVE_SIGN:
      	svalue = culture.NumberFormat.NegativeSign;
	break;

      case __DECIMAL_POINT:
      	svalue = culture.NumberFormat.NumberDecimalSeparator;
	break;

      case __THOUSANDS_SEP:
      	svalue = culture.NumberFormat.NumberGroupSeparator;
	break;

      /* TODO: number and monetary order flags */
    }
  if (svalue)
    {
      value = (char *)Marshal::StringToHGlobalAnsi (svalue);
    }
  if (!value)
    {
      value = "";
    }

  /* Cache the value and then return it */
  if (value)
    {
      current = (nl_loaded *)malloc (sizeof(nl_loaded));
      if (current)
        {
	  current->item = item;
	  current->value = value;
	  current->next = loaded;
	  loaded = current;
	}
    }
  return value;
}
