/*
 * csant_dir.c - Directory walking and name manipulation tools.
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

#include "csant_defs.h"
#include "il_regex.h"
#include <dirent.h>
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifndef IL_WIN32_NATIVE
#ifdef HAVE_SYS_CYGWIN_H
#include <sys/cygwin.h>
#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Directory walking structure.
 */
struct _tagCSAntDir
{
	ILDir  *dirInfo;
	int     useRegex;
	char   *name;
	regex_t regexState;
};

CSAntDir *CSAntDirOpen(const char *pathname, const char *fileRegex)
{
	ILDir *dirInfo;
	CSAntDir *dir;
	char *regex;
	char *out;
	char lastch;
	char *newPathname;

	/* Attempt to open the directory, after normalizing the path */
	newPathname = CSAntDirCombine(pathname, 0);
	dirInfo = ILOpenDir(newPathname);
	ILFree(newPathname);
	if(!dirInfo)
	{
		return 0;
	}

	/* Allocate space for the directory walker */
	dir = (CSAntDir *)ILMalloc(sizeof(CSAntDir));
	if(!dir)
	{
		CSAntOutOfMemory();
	}
	dir->dirInfo = dirInfo;
	dir->name = 0;

	/* Convert the regular expression from file form into grep form */
	if(fileRegex)
	{
		regex = (char *)ILMalloc(strlen(fileRegex) * 4 + 20);
		if(!regex)
		{
			CSAntOutOfMemory();
		}
		out = regex;
		*out++ = '^';
		while(*fileRegex != '\0')
		{
			lastch = *fileRegex++;
			if(lastch == '?')
			{
				*out++ = '.';
			}
			else if(lastch == '*')
			{
				*out++ = '.';
				*out++ = '*';
			}
			else if(lastch == '[')
			{
				*out++ = '[';
				while(*fileRegex != '\0')
				{
					lastch = *fileRegex++;
					if(lastch == ']')
					{
						break;
					}
					*out++ = lastch;
				}
				*out++ = ']';
			}
			else if(lastch == '.' || lastch == '^' || lastch == '$' ||
					lastch == ']' || lastch == '\\')
			{
				*out++ = '\\';
				*out++ = lastch;
			}
			else
			{
				*out++ = lastch;
			}
		}
		*out++ = '$';
		*out = '\0';
	}
	else
	{
		regex = 0;
	}

	/* Compile the regular expression */
	if(regex)
	{
		if(IL_regcomp(&(dir->regexState), regex, REG_EXTENDED | REG_NOSUB) != 0)
		{
			fprintf(stderr, "Invalid regular expression\n");
			exit(1);
		}
		ILFree(regex);
		dir->useRegex = 1;
	}
	else
	{
		dir->useRegex = 0;
	}

	/* Ready to go */
	return dir;
}

void CSAntDirClose(CSAntDir *dir)
{
	ILCloseDir(dir->dirInfo);
	if(dir->name)
	{
		ILFree(dir->name);
	}
	if(dir->useRegex)
	{
		IL_regfree(&(dir->regexState));
	}
	ILFree(dir);
}

const char *CSAntDirNext(CSAntDir *dir)
{
	ILDirEnt *entry;
	const char *name;

	for(;;)
	{
		/* Read the next raw entry from the operating system */
		entry = ILReadDir(dir->dirInfo);
		if(!entry)
		{
			break;
		}

		/* Always ignore "." and ".." */
		name = ILDirEntName(entry);
		if(!strcmp(name, ".") || !strcmp(name, ".."))
		{
			ILFree(entry);
			continue;
		}

		/* Ignore the name if the regular expression does not match */
		if(dir->useRegex)
		{
			if(IL_regexec(&(dir->regexState), name, 0, 0, 0) != 0)
			{
				ILFree(entry);
				continue;
			}
		}

		/* We have a match */
		if(dir->name)
		{
			ILFree(dir->name);
		}
		dir->name = ILDupString(name);
		ILFree(entry);
		return dir->name;
	}
	return 0;
}

char *CSAntDirCombine(const char *pathname, const char *filename)
{
	char *name;
	char *temp;
	#ifdef IL_WIN32_NATIVE
	if(filename && ((filename[0] == '/') ||
		(strlen(filename) >= 2 && ((filename[0] >= 'a' && filename[0] <= 'z') ||
		(filename[0] >= 'A' && filename[0] <= 'Z')) && filename[1] == ':')))
	#else
	if(filename && filename[0] == '/')
	#endif
	{
		name = ILDupString(filename);
		if(!name)
		{
			CSAntOutOfMemory();
		}
	}
	else
	{
		name = (char *)ILMalloc(strlen(pathname) +
							    (filename ? strlen(filename) + 1 : 0) + 1);
		if(!name)
		{
			CSAntOutOfMemory();
		}
		strcpy(name, pathname);
		if(filename)
		{
			strcat(name, "/");
			strcat(name, filename);
		}
	}
	temp = name;
	while(*temp != '\0')
	{
		if(*temp == '/' || *temp == '\\')
		{
		#ifdef IL_WIN32_NATIVE
			*temp = '\\';
		#else
			*temp = '/';
		#endif
		}
		++temp;
	}
	return name;
}

char *CSAntDirCombineWin32(const char *pathname, const char *filename,
						   const char *extension)
{
	/* Combine the path components */
	char *name;
	if(filename &&
	   (filename[0] == '/' || filename[0] == '\\' ||
	    (filename[0] != '\0' && filename[1] == ':')))
	{
		name = (char *)ILMalloc(strlen(filename) +
							    (extension ? strlen(extension) : 0) + 1);
		if(!name)
		{
			CSAntOutOfMemory();
		}
		strcpy(name, filename);
		if(extension)
		{
			strcat(name, extension);
		}
	}
	else
	{
		name = (char *)ILMalloc(strlen(pathname) +
							    (filename ? strlen(filename) + 1 : 0) +
							    (extension ? strlen(extension) : 0) + 1);
		if(!name)
		{
			CSAntOutOfMemory();
		}
		strcpy(name, pathname);
		if(filename)
		{
			strcat(name, "/");
			strcat(name, filename);
			if(extension)
			{
				strcat(name, extension);
			}
		}
	}

#if defined(HAVE_SYS_CYGWIN_H) && \
    defined(HAVE_CYGWIN_CONV_TO_WIN32_PATH)

	/* Use the Cygwin-supplied function to convert the path */
	{
		char buf[4096];
		char *temp;
		if(cygwin_conv_to_win32_path(name, buf) == 0)
		{
			temp = ILDupString(buf);
			if(!temp)
			{
				CSAntOutOfMemory();
			}
			ILFree(name);
			return temp;
		}
	}

#endif

	/* Simple method - just convert slashes into backslashes */
	{
		char *temp;
		temp = name;
		while(*temp != '\0')
		{
			if(*temp == '/' || *temp == '\\')
			{
				*temp = '\\';
			}
			++temp;
		}
	}
	return name;
}

#ifdef	__cplusplus
};
#endif
