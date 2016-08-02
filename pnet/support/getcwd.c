/*
 * getcwd.c - Get the current working directory in a portable manner.
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

#ifndef _GNU_SOURCE
#define _GNU_SOURCE 1		/* Needed for "get_current_dir_name" */
#endif
#include <stdio.h>
#include "il_system.h"
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#if defined(WIN32) || defined(HAVE_DIRECT_H)
	#include <direct.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

char *ILGetCwd(void)
{
#if HAVE_GET_CURRENT_DIR_NAME
	char *dir = get_current_dir_name();
	char *newDir = ILDupString(dir);
	if(dir)
	{
		free(dir);
	}
	return newDir;
#elif HAVE_GETCWD
	char name[8192];
	return ILDupString(getcwd(name, sizeof(name)));
#elif HAVE_GETWD
	char name[8192];
	return ILDupString(getwd(name));
#else
	return 0;
#endif
}

#ifdef	__cplusplus
};
#endif
