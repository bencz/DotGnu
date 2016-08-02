/*
 * expand.c - Expand filenames to full pathnames.
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

#include <stdio.h>
#include "il_system.h"
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#if defined(IL_WIN32_NATIVE) || defined(IL_WIN32_CYGWIN)
	#include <windows.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

char *ILExpandFilename(const char *filename, char *searchPath)
{
	char *env;
	char *path;
	char *newPath;
	int len;
#if defined(IL_WIN32_NATIVE) || defined(IL_WIN32_CYGWIN)
	char fullName[MAX_PATH];
	LPSTR filePart;
	DWORD pathLen;
#ifdef IL_WIN32_CYGWIN
	int len2;
#endif
#else
	int len2;
#endif

	/* Should we do a path search? */
	if(searchPath && strchr(filename, '/') == 0 && strchr(filename, '\\') == 0)
	{
		env = searchPath;
		if(env)
		{
			while(*env != '\0')
			{
			#ifndef IL_WIN32_NATIVE
				if(*env == ':')
			#else
				if(*env == ';')
			#endif
				{
					++env;
				}
				else
				{
					len = 1;
				#ifndef IL_WIN32_NATIVE
					while(env[len] != '\0' && env[len] != ':')
				#else
					while(env[len] != '\0' && env[len] != ';')
				#endif
					{
						++len;
					}
					path = (char *)ILMalloc(len + strlen(filename) + 2);
					if(path)
					{
						strncpy(path, env, len);
					#ifndef IL_WIN32_NATIVE
						path[len++] = '/';
					#else
						path[len++] = '\\';
					#endif
						strcpy(path + len, filename);
						if(ILFileExists(path, &newPath))
						{
							if(newPath)
							{
								ILFree(path);
								return newPath;
							}
							return path;
						}
						ILFree(path);
					}
					env += len;
				}
			}
		}
	}

#ifndef IL_WIN32_NATIVE
	/* Get the starting directory for name resolution */
	if(*filename == '/')
	{
		path = ILDupString("/");
		++filename;
	}
	else
	{
#ifdef IL_WIN32_CYGWIN
		if((strlen(filename) > 1) && 
			((filename[0] >= 'a' && filename[0] <= 'z') ||
			(filename[0] >= 'A' && filename[0] <= 'Z')) &&
			(filename[1] == ':'))
		{
			/* File starts with a drive spec. so expand the filename using Win32 functions */
			filePart = 0;
			pathLen = GetFullPathName(filename, sizeof(fullName), fullName, &filePart);
			if(!pathLen)
			{
				/* Something is wrong with the filename, so just return it as-is */
				return ILDupString(filename);
			}
			else if(pathLen < sizeof(fullName))
			{
				return ILDupString(fullName);
			}
			newPath = (char *)ILMalloc(pathLen + 1);
			pathLen = GetFullPathName(filename, pathLen + 1, newPath, &filePart);
			newPath[pathLen] = '\0';
			return newPath;
		}
		else
		{
			path = ILGetCwd();
		}
#else		
		path = ILGetCwd();
#endif
	}
	if(!path)
	{
		return 0;
	}

	/* Process all of the filename components */
	while(*filename != '\0')
	{
		if(*filename == '.' && (filename[1] == '/' || filename[1] == '\\'))
		{
			/* Same directory: skip this component */
			filename += 2;
		}
		if(*filename == '.' && filename[1] == '\0')
		{
			/* Same directory: skip this component */
			++filename;
		}
		else if(*filename == '.' && filename[1] == '.' &&
		        (filename[2] == '/' || filename[2] == '\\' ||
				 filename[2] == '\0'))
		{
			/* Parent directory: strip the last component from "path" */
			len = strlen(path);
			while(len > 0 && path[len - 1] != '/' && path[len - 1] != '\\')
			{
				--len;
			}
			if(len > 0)
			{
				--len;
			}
			path[len] = '\0';
			if(filename[2] == '\0')
			{
				filename += 2;
			}
			else
			{
				filename += 3;
			}
		}
		else if(*filename == '/' || *filename == '\\')
		{
			/* Skip spurious directory separators */
			++filename;
		}
		else
		{
			/* Append this component to "path" */
			len = 1;
			while(filename[len] != '\0' && filename[len] != '/' &&
			      filename[len] != '\\')
			{
				++len;
			}
			len2 = strlen(path);
			newPath = (char *)ILMalloc(len2 + len + 2);
			if(!newPath)
			{
				ILFree(path);
				return 0;
			}
			strcpy(newPath, path);
			if(len2 <= 0 || (newPath[len2 - 1] != '/' &&
							 newPath[len2 - 1] != '\\'))
			{
				newPath[len2++] = '/';
			}
			ILMemCpy(newPath + len2, filename, len);
			newPath[len2 + len] = '\0';
			filename += len;
			ILFree(path);
			path = newPath;
		}
	}

	/* Done */
	return path;

#else /* IL_WIN32_NATIVE */

	/* Expand the filename using Win32 functions */
	filePart = 0;
	pathLen = GetFullPathName(filename, sizeof(fullName), fullName, &filePart);
	if(!pathLen)
	{
		/* Something is wrong with the filename, so just return it as-is */
		return ILDupString(filename);
	}
	else if(pathLen < sizeof(fullName))
	{
		return ILDupString(fullName);
	}
	newPath = (char *)ILMalloc(pathLen + 1);
	pathLen = GetFullPathName(filename, pathLen + 1, newPath, &filePart);
	newPath[pathLen] = '\0';
	return newPath;

#endif /* IL_WIN32_NATIVE */
}

#ifdef	__cplusplus
};
#endif
