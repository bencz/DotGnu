/*
 * path.c - Find standard paths for configuration and library files.
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

#include "il_system.h"
#ifdef IL_WIN32_PLATFORM
	#include <windows.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_WIN32_PLATFORM

/*
 * Determine the appropriate directory separator to use.
 */
#ifdef IL_WIN32_CYGWIN	
	#define	DIR_SEP		'/'
#else
	#define	DIR_SEP		'\\'
#endif

/*
 * Determine if the tail part of a path equals a particular string,
 * taking case and separator differences into account.
 */
static int TailEquals(const char *path, int pathlen, const char *tail)
{
	int taillen = strlen(tail);
	int ch1, ch2;
	if(taillen >= pathlen)
	{
		return 0;
	}
	path += pathlen - taillen;
	while(taillen > 0)
	{
		ch1 = *path++;
		if(ch1 >= 'A' && ch1 <= 'Z')
		{
			ch1 = ch1 - 'A' + 'a';
		}
		else if(ch1 == '\\')
		{
			ch1 = '/';
		}
		ch2 = *tail++;
		if(ch1 != ch2)
		{
			return 0;
		}
		--taillen;
	}
	return 1;
}

/*
 * Get the Win32 base path of the installation.  e.g. if the program
 * is installed in "C:\PNET\BIN", then the base path is "C:\PNET".
 * This allows us to locate our files without using registry settings
 * or hard-wired paths.
 */
static char *GetBasePath(void)
{
	char *env;
	char moduleName[1024];
	int len;

	/* See if the "PNET_BASE" environment variable is set */
	env = getenv("PNET_BASE");
	if(env && *env != '\0')
	{
		return ILDupString(env);
	}

	/* Get the pathname of the current module */
	if(GetModuleFileName(NULL, moduleName, sizeof(moduleName)) == 0)
	{
		return 0;
	}

	/* Strip the module name down to its directory */
	len = strlen(moduleName);
	while(len > 0 && moduleName[len - 1] != '/' && moduleName[len - 1] != '\\')
	{
		--len;
	}
	if(len > 1 && moduleName[len - 2] != '/' && moduleName[len - 2] != '\\' &&
	   moduleName[len - 2] != ':')
	{
		--len;
	}

	/* How much do we need to remove to get back to the base path? */
	if(TailEquals(moduleName, len, "/lib/cscc/plugins"))
	{
		len -= 17;
	}
	else if(TailEquals(moduleName, len, "/lib/cscc/lib"))
	{
		len -= 13;
	}
	else if(TailEquals(moduleName, len, "/lib"))
	{
		len -= 4;
	}
	else if(TailEquals(moduleName, len, "/bin"))
	{
		len -= 4;
	}
	moduleName[len] = '\0';

	/* Return the base path to the caller */
	return ILDupString(moduleName);
}

/*
 * Get a file or directory, relative to the installation base path.
 */
static char *GetFileInBasePath(const char *tail1, const char *tail2)
{
	char *base;
	char *temp;
	int baselen;
	int len;

	/* Get the base path for the program installation */
	base = GetBasePath();
	if(!base)
	{
		return 0;
	}
	baselen = strlen(base);

	/* Allocate additional space for the rest of the path */
	len = baselen + (tail1 ? strlen(tail1) : 0) + (tail2 ? strlen(tail2) : 0);
	temp = (char *)ILRealloc(base, len + 1);
	if(!temp)
	{
		ILFree(base);
		return 0;
	}
	base = temp;

	/* Construct the final path, normalizing path separators as we go */
	len = baselen;
	while(tail1 != 0 && *tail1 != '\0')
	{
		if(*tail1 == '/')
		{
			base[len++] = DIR_SEP;
		}
		else
		{
			base[len++] = *tail1;
		}
		++tail1;
	}
	while(tail2 != 0 && *tail2 != '\0')
	{
		if(*tail2 == '/')
		{
			base[len++] = DIR_SEP;
		}
		else
		{
			base[len++] = *tail2;
		}
		++tail2;
	}
	base[len] = '\0';
	return base;
}

char *ILGetStandardLibraryPath(const char *tail)
{
	if(tail)
	{
		return GetFileInBasePath("/lib/", tail);
	}
	else
	{
		return GetFileInBasePath("/lib", 0);
	}
}

char *ILGetStandardDataPath(const char *tail)
{
	if(tail)
	{
		return GetFileInBasePath("/share/", tail);
	}
	else
	{
		return GetFileInBasePath("/share", 0);
	}
}

char *ILGetStandardProgramPath(void)
{
	return GetFileInBasePath("/bin", 0);
}

#else	/* !IL_WIN32_PLATFORM */

#define	DIR_SEP		'/'

char *ILGetStandardLibraryPath(const char *tail)
{
	if(tail)
	{
		char *path = (char *)ILMalloc(strlen(CSCC_LIB_PREFIX) +
									  strlen(tail) + 2);
		if(path)
		{
			strcpy(path, CSCC_LIB_PREFIX);
			strcat(path, "/");
			strcat(path, tail);
		}
		return path;
	}
	else
	{
		return ILDupString(CSCC_LIB_PREFIX);
	}
}

char *ILGetStandardDataPath(const char *tail)
{
	if(tail)
	{
		char *path = (char *)ILMalloc(strlen(CSCC_DATA_PREFIX) +
									  strlen(tail) + 2);
		if(path)
		{
			strcpy(path, CSCC_DATA_PREFIX);
			strcat(path, "/");
			strcat(path, tail);
		}
		return path;
	}
	else
	{
		return ILDupString(CSCC_DATA_PREFIX);
	}
}

char *ILGetStandardProgramPath(void)
{
	return ILDupString(CSCC_BIN_PREFIX);
}

#endif	/* !IL_WIN32_PLATFORM */

char *ILSearchPath(const char *path, const char *name, int isExe)
{
#if defined(__palmos__)
	return 0;
#else
	int separator;
	int len, temp;
	char *fullname;
	char *newExePath;

	/* Use the "PATH" environment variable if "path" is NULL */
	if(!path)
	{
		path = getenv("PATH");
		if(!path)
		{
			return 0;
		}
	}

	/* Attempt to discover the correct path separator to use */
#ifdef IL_WIN32_PLATFORM
	if(strchr(path, ';') != 0)
	{
		/* The path already uses ';', so that is probably the separator */
		separator = ';';
	}
	else
	{
		/* Deal with the ambiguity between ':' used as a separator
		   and ':' used to specify a drive letter */
		if(((path[0] >= 'A' && path[0] <= 'Z') ||
		    (path[0] >= 'a' && path[0] <= 'z')) && path[1] == ':')
		{
			/* The path is probably one directory, starting
			   with a drive letter */
			separator = ';';
		}
		else
		{
			/* The path is probably Cygwin-like, using ':' to separate */
			separator = ':';
		}
	}
#else
	separator = ':';
#endif

	/* Scan the path for the requested name */
	while(*path != '\0')
	{
		/* Skip path separators */
		if(*path == separator || *path == ' ' || *path == '\t' ||
		   *path == '\r' || *path == '\n')
		{
			++path;
			continue;
		}

		/* Extract the next directory from the path */
		len = 1;
		while(path[len] != separator && path[len] != '\0')
		{
			++len;
		}
		while(len > 0 && (path[len - 1] == ' ' || path[len - 1] == '\t' ||
						  path[len - 1] == '\r' || path[len - 1] == '\n'))
		{
			/* Strip whitespace from the end of the path, but not its middle */
			--len;
		}

		/* Build the full pathname and then probe for it */
		fullname = (char *)ILMalloc(len + strlen(name) + 2);
		if(fullname)
		{
			ILMemCpy(fullname, path, len);
			temp = len;
			if(path[len - 1] != '/' && path[len - 1] != '\\')
			{
				fullname[temp++] = DIR_SEP;
			}
			strcpy(fullname + temp, name);
			if(isExe)
			{
				if(ILFileExists(fullname, &newExePath))
				{
					if(newExePath)
					{
						ILFree(fullname);
						return newExePath;
					}
					else
					{
						return fullname;
					}
				}
			}
			else if(ILFileExists(fullname, (char **)0))
			{
				return fullname;
			}
			ILFree(fullname);
		}

		/* Advance to the next directory in the path */
		path += len;
	}

	/* If we get here, then we could not locate the name */
	return 0;
#endif
}

const char *ILGetPlatformName(void)
{
#ifdef CSCC_HOST_TRIPLET
	return CSCC_HOST_TRIPLET;
#else
	return 0;
#endif
}

#ifdef	__cplusplus
};
#endif
