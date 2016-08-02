/*
 * strings.c - System manipulation routines.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Note: this uses an English-style case comparison routine.
 * This is deliberate.  "ILStrICmp" is used to compare program
 * identifiers and the like and we don't want that comparison
 * to be affected by the locale.
 */
int ILStrICmp(const char *str1, const char *str2)
{
	char ch1;
	char ch2;
	while(*str1 != '\0' && *str2 != '\0')
	{
		ch1 = *str1++;
		if(ch1 >= 'A' && ch1 <= 'Z')
		{
			ch1 = (ch1 - 'A' + 'a');
		}
		ch2 = *str2++;
		if(ch2 >= 'A' && ch2 <= 'Z')
		{
			ch2 = (ch2 - 'A' + 'a');
		}
		if(ch1 < ch2)
		{
			return -1;
		}
		else if(ch1 > ch2)
		{
			return 1;
		}
	}
	if(*str1 != '\0')
	{
		return 1;
	}
	else if(*str2 != '\0')
	{
		return -1;
	}
	else
	{
		return 0;
	}
}

int ILStrNICmp(const char *str1, const char *str2, int len)
{
	char ch1;
	char ch2;
	while(len > 0 && *str1 != '\0' && *str2 != '\0')
	{
		ch1 = *str1++;
		if(ch1 >= 'A' && ch1 <= 'Z')
		{
			ch1 = (ch1 - 'A' + 'a');
		}
		ch2 = *str2++;
		if(ch2 >= 'A' && ch2 <= 'Z')
		{
			ch2 = (ch2 - 'A' + 'a');
		}
		if(ch1 < ch2)
		{
			return -1;
		}
		else if(ch1 > ch2)
		{
			return 1;
		}
		--len;
	}
	if(!len)
	{
		return 0;
	}
	if(*str1 != '\0')
	{
		return 1;
	}
	else if(*str2 != '\0')
	{
		return -1;
	}
	else
	{
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
