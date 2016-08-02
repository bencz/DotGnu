/*
 * dynlib.c - Dynamic library support routines.
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

#include <stdio.h>
#include "il_system.h"
#ifdef IL_WIN32_PLATFORM
	#include <windows.h>
	#ifndef IL_WIN32_NATIVE
		#ifdef HAVE_SYS_CYGWIN_H
			#include <sys/cygwin.h>
		#endif
	#endif
#else

#ifdef __BEOS__ 
	#include <be/kernel/image.h>
	#include <OS.h>
#endif

#ifdef HAVE_DLFCN_H
	#include <dlfcn.h>
#endif
#endif

/*
 * Define this for additional debug messages.
 */
/*#define IL_DYNLIB_DEBUG 1*/

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(__APPLE__) && defined(__MACH__)	/* MacOS X */

#include <mach-o/dyld.h>

void *ILDynLibraryOpen(const char *name)
{
	NSObjectFileImage file;
	NSObjectFileImageReturnCode result;
	NSModule module;
	void *image;
	const char *msg;

	/* Attempt to open the dylib file */
	result = NSCreateObjectFileImageFromFile(name, &file);
	if(result == NSObjectFileImageInappropriateFile)
	{
		/* May be an image, and not a bundle */
		image = (void *)NSAddImage(name, NSADDIMAGE_OPTION_RETURN_ON_ERROR);
		if(image)
		{
			return image;
		}
	}
	if(result != NSObjectFileImageSuccess)
	{
		switch(result)
		{
			case NSObjectFileImageFailure:
				msg = " (NSObjectFileImageFailure)"; break;
			case NSObjectFileImageInappropriateFile:
				msg = " (NSObjectFileImageInappropriateFile)"; break;
			case NSObjectFileImageArch:
				msg = " (NSObjectFileImageArch)"; break;
			case NSObjectFileImageFormat:
				msg = " (NSObjectFileImageFormat)"; break;
			case NSObjectFileImageAccess:
				msg = " (NSObjectFileImageAccess)"; break;
			default:
				msg = ""; break;
		}
	#ifdef IL_DYNLIB_DEBUG
		fprintf(stderr, "%s: could not load dynamic library%s\n", name, msg);
	#endif
		return 0;
	}

	/* Link the module dependencies */
	module = NSLinkModule(file, name,
						  NSLINKMODULE_OPTION_BINDNOW |
						  NSLINKMODULE_OPTION_PRIVATE |
						  NSLINKMODULE_OPTION_RETURN_ON_ERROR);
	return (void *)module;
}

void  ILDynLibraryClose(void *handle)
{
	if((((struct mach_header *)handle)->magic == MH_MAGIC) ||
	   (((struct mach_header *)handle)->magic == MH_CIGAM))
	{
		/* Cannot remove dynamic images once they've been loaded */
		return;
	}
	NSUnLinkModule((NSModule)handle, NSUNLINKMODULE_OPTION_NONE);
}

static void *GetSymbol(void *handle, const char *symbol)
{
	NSSymbol sym;

	/* We have to use a different lookup approach for images and modules */
	if((((struct mach_header *)handle)->magic == MH_MAGIC) ||
	   (((struct mach_header *)handle)->magic == MH_CIGAM))
	{
		if(NSIsSymbolNameDefinedInImage((struct mach_header *)handle, symbol))
		{
			sym = NSLookupSymbolInImage((struct mach_header *)handle, symbol,
						NSLOOKUPSYMBOLINIMAGE_OPTION_BIND |
						NSLOOKUPSYMBOLINIMAGE_OPTION_RETURN_ON_ERROR);
		}
		else
		{
			sym = 0;
		}
	}
	else
	{
		sym = NSLookupSymbolInModule((NSModule)handle, symbol);
	}

	/* Did we find the symbol? */
	if(sym == 0)
	{
		return 0;
	}

	/* Convert the symbol into the address that we require */
	return (void *)NSAddressOfSymbol(sym);
}

void *ILDynLibraryGetSymbol(void *handle, const char *symbol)
{
	void *value = GetSymbol(handle, (char *)symbol);
	char *newName;
	if(value)
	{
		return value;
	}
	newName = (char *)ILMalloc(strlen(symbol) + 2);
	if(newName)
	{
		/* Try again with '_' prepended to the name */
		newName[0] = '_';
		strcpy(newName + 1, symbol);
		value = GetSymbol(handle, newName);
		if(value)
		{
			ILFree(newName);
			return value;
		}
		ILFree(newName);
	}
#ifdef IL_DYNLIB_DEBUG
	fprintf(stderr, "%s: could not find the specified symbol\n", symbol);
#endif
	return 0;
}

#elif defined(__BEOS__)

void *ILDynLibraryOpen(const char *name)
{
	image_id handle;
	const char *error;
	handle = load_add_on(name);
	if(!handle)
	{
		/* If the name does not start with "lib" and does not
		   contain a path, then prepend "lib" and try again */
		if(strncmp(name, "lib", 3) != 0)
		{
			error = name;
			while(*error != '\0' && *error != '/' && *error != '\\')
			{
				++error;
			}
			if(*error == '\0')
			{
				/* Try adding "lib" to the start */
				char *temp = (char *)ILMalloc(strlen(name) + 4);
				if(temp)
				{
					strcpy(temp, "lib");
					strcat(temp, name);
					handle = load_add_on(temp);
					ILFree(temp);
					if(handle >= B_NO_ERROR) /* errors are < B_NO_ERROR */
					{
						return handle;
					}
				}

				/* Reload the original error state */
				handle = load_add_on(name);
			}
		}

		/* Report the error */
	#ifdef IL_DYNLIB_DEBUG
		error = strerror(handle); 
		fprintf(stderr, "%s: \n", name,
				(error ? error : "could not load dynamic library"));
	#endif
		return 0;
	}
	else
	{
		return handle;
	}
}

void  ILDynLibraryClose(void *handle)
{
	/* TODO */
	unload_add_on(handle);
}

void *ILDynLibraryGetSymbol(void *handle, const char *symbol)
{
	void *value;
	const char * error;
	int b_error;

	b_error = get_image_symbol((image_id)handle, (char *)symbol, 
								B_SYMBOL_TYPE_ANY, (void**) &value);

	if(b_error == B_OK)
	{
		return value;
	}
#ifdef IL_DYNLIB_DEBUG
	error = strerror(handle); 
	fprintf(stderr, "%s: %s\n", symbol, error);
#endif
	return 0;
}

#elif defined(IL_WIN32_PLATFORM)	/* Native Win32 or Cygwin */

void *ILDynLibraryOpen(const char *name)
{
	void *libHandle;
	char *newName = 0;

#if defined(IL_WIN32_CYGWIN) && defined(HAVE_SYS_CYGWIN_H) && \
    defined(HAVE_CYGWIN_CONV_TO_WIN32_PATH)

	/* Use Cygwin to expand the path */
	{
		char buf[4096];
		if(cygwin_conv_to_win32_path(name, buf) == 0)
		{
			newName = ILDupString(buf);
			if(!newName)
			{
				return 0;
			}
		}
	}

#endif

	/* Attempt to load the library */
	libHandle = (void *)LoadLibrary((newName ? newName : name));
	if(libHandle == 0)
	{
	#ifdef IL_DYNLIB_DEBUG
		fprintf(stderr, "%s: could not load dynamic library\n",
				(newName ? newName : name));
	#endif
		if(newName)
		{
			ILFree(newName);
		}
		return 0;
	}
	if(newName)
	{
		ILFree(newName);
	}
	return libHandle;
}

void ILDynLibraryClose(void *handle)
{
	FreeLibrary((HINSTANCE)handle);
}

void *ILDynLibraryGetSymbol(void *handle, const char *symbol)
{
	void *procAddr;
	procAddr = (void *)GetProcAddress((HINSTANCE)handle, symbol);
	if(procAddr == 0)
	{
	#ifdef IL_DYNLIB_DEBUG
		fprintf(stderr, "%s: could not resolve symbol", symbol);
	#endif
		return 0;
	}
	return procAddr;
}

#elif defined(HAVE_DLFCN_H) && defined(HAVE_DLOPEN)

void *ILDynLibraryOpen(const char *name)
{
	void *handle;
	const char *error;
	handle = dlopen(name, RTLD_LAZY | RTLD_GLOBAL);
	if(!handle)
	{
		/* If the name does not start with "lib" and does not
		   contain a path, then prepend "lib" and try again */
		if(strncmp(name, "lib", 3) != 0)
		{
			error = name;
			while(*error != '\0' && *error != '/' && *error != '\\')
			{
				++error;
			}
			if(*error == '\0')
			{
				/* Try adding "lib" to the start */
				char *temp = (char *)ILMalloc(strlen(name) + 4);
				if(temp)
				{
					strcpy(temp, "lib");
					strcat(temp, name);
					handle = dlopen(temp, RTLD_LAZY | RTLD_GLOBAL);
					ILFree(temp);
					if(handle)
					{
						return handle;
					}
				}

				/* Reload the original error state */
				handle = dlopen(name, RTLD_LAZY | RTLD_GLOBAL);
			}
		}

		/* Report the error */
	#ifdef IL_DYNLIB_DEBUG
		error = dlerror();
		fprintf(stderr, "%s: %s\n", name,
				(error ? error : "could not load dynamic library"));
	#endif
		return 0;
	}
	else
	{
		return handle;
	}
}

void  ILDynLibraryClose(void *handle)
{
	dlclose(handle);
}

void *ILDynLibraryGetSymbol(void *handle, const char *symbol)
{
	/* call dlerror prior to resolving the symbol to clear any pending
	   errors. */
	const char *error = dlerror();
	void *value = dlsym(handle, (char *)symbol);
	if(value != 0)
	{
		/* In this case we definitely found the symbol. */
		return value;
	}
	if(!(error = dlerror()))
	{
		/* No error. The symbol is actually NULL */
		return 0;
	}
	else
	{
		/* There occured an error during resolving the symbol */
		char *newName = (char *)ILMalloc(strlen(symbol) + 2);
		if(newName)
		{
			/* Try again with '_' prepended to the name in case
			   we are running on a system with a busted "dlsym" */
			newName[0] = '_';
			strcpy(newName + 1, symbol);
			value = dlsym(handle, newName);
			if(value != 0)
			{
				/* So we found the symbol with preceding underscore. */
				ILFree(newName);
				return value;
			}
			error = dlerror();
			if(error == 0)
			{
				/* This symbol is NULL. */
				ILFree(newName);
				return value;
			}
			ILFree(newName);
		}
	}
#ifdef IL_DYNLIB_DEBUG
	fprintf(stderr, "%s: %s\n", symbol, error);
#endif
	return 0;
}

#else	/* No dynamic library support */

void *ILDynLibraryOpen(const char *name)
{
#ifndef REDUCED_STDIO
#ifdef IL_DYNLIB_DEBUG
	fprintf(stderr, "%s: dynamic libraries are not available\n", name);
#endif
#endif
	return 0;
}

void  ILDynLibraryClose(void *handle)
{
}

void *ILDynLibraryGetSymbol(void *handle, const char *symbol)
{
	return 0;
}

#endif	/* No dynamic library support */

#ifdef	__cplusplus
};
#endif
