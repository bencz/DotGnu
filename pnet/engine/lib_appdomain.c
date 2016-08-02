/*
 * lib_appdomain.c - Internalcall methods for the "System.AppDomain" class.
 *
 * Copyright (C) 2003, 2008  Southern Storm Software, Pty Ltd.
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

#include "engine_private.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Convert an image into an assembly object.
 */
static ILObject *ImageToAssembly(ILExecThread *thread, ILImage *image)
{
	void *item;
	item = ILImageTokenInfo(image, (IL_META_TOKEN_ASSEMBLY | 1));
	if(item)
	{
		return _ILClrToObject(thread, item, "System.Reflection.Assembly");
	}
	/* TODO: if the image does not have an assembly manifest,
	   then look for the parent assembly */
	return 0;
}

/*
 * Check if the ILExecProcess is unloaded.
 */
static ILBool IsAppDomainUnloaded(ILExecThread *thread, ILExecProcess *process)
{
	if(process->state >= _IL_PROCESS_STATE_UNLOADING)
	{
		ILExecThreadThrowSystem(thread, "System.AppDomainUnloadedException", 0);
		return (ILBool)1;
	}
	return (ILBool)0;
}

/*
 * convert an ILString to a not managed ansi string.
 */
static char *_ToAnsiString(ILExecThread *thread, ILString *str)
{
	if(str)
	{
		ILUInt16 *buf = StringToBuffer(str);
		ILInt32 len = ((System_String *)str)->length;
		unsigned long size = ILAnsiGetByteCount(buf, (unsigned long)len);
		char *newStr = (char *)ILMalloc(size + 1);
		if(!newStr)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		ILAnsiGetBytes(buf, (unsigned long)len,
					   (unsigned char *)newStr, size);
		newStr[size] = '\0';
		return newStr;
	}
	else
	{
		return 0;
	}
}

/*
 * private static void AppendPrivatePathInternal(Object appDomain, String[] splitPaths);
 */
void _IL_AppDomain_AppendPrivatePathsInternal(ILExecThread *thread, ILObject *appDomain, System_Array *splitPaths)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if (splitPaths && (ArrayLength(splitPaths) > 0))
		{
			char **libraryDirs;
			int numLibraryDirs;
			char **newLibraryDirs;
			System_String **buffer = (System_String **)ArrayToBuffer(splitPaths);
			int i;  /* loop counter */

			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILContextGetLibraryDirs(((ILExecProcess *)appDomain)->context, &libraryDirs, &numLibraryDirs);

			/* allocate the array for the old + the new directories */
			if (!(newLibraryDirs = (char **)ILMalloc(sizeof(char *) * (numLibraryDirs + ArrayLength(splitPaths)))))
			{
				IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}
		
			/* copy over the old directories */
			for (i = 0; i < numLibraryDirs; i++)
			{
				newLibraryDirs[i] = libraryDirs[i];
			}

			/* now append the new dirs */
			for (i = 0; i < ArrayLength(splitPaths); i++)
			{
				if (buffer[i] && buffer[i]->length)
				{
					newLibraryDirs[numLibraryDirs] = ILStringToPathname(thread, (ILString *)buffer[i]);
					numLibraryDirs++;
				}
			}

			/* now set the new library dirs */
			ILContextSetLibraryDirs(((ILExecProcess *)appDomain)->context, newLibraryDirs, numLibraryDirs);

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
		}
	}
}

/*
 * private static void CreateAppDomain(ref Object appDomain);
 */
void _IL_AppDomain_CreateAppDomain(ILExecThread *thread, ILObject **appDomain)
{
#ifdef IL_CONFIG_APPDOMAINS
	ILExecProcess *process = ILExecProcessCreate(0, 0);
	
	if (process)
	{
		int error;
		ILImage *corlibImage;
		/* load the same corlib assembly as the default appdomain */
		/* first get the corlib image of the default appdomain */
		ILImage *image = ILContextGetSystem(ILExecEngineInstance()->defaultProcess->context);

		if((error = ILImageLoadFromFile(ILImageGetFileName(image),
										process->context,
										&corlibImage,
										process->loadFlags, 0)))
		{
			ILExecThreadThrowSystem(thread, "System.IO.FileNotFoundException", 0);
		}
		else
		{
			 _ILExecProcessLoadStandard(process, corlibImage);
			/* return the new process */
			*appDomain = (ILObject *)process;
		}
	}
	else
	{
		ILExecThreadThrowOutOfMemory(thread);
	}
#else
	/* we don't support multiple Appdomains so return the current process of the thread */
	*appDomain = (ILNativeInt)((void *)thread->process);
#endif
}

/*
 * private static void ClearPrivatePathInternal(Object appDomain);
 */
void _IL_AppDomain_ClearPrivatePathInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

		ILContextClearLibraryDirs(((ILExecProcess *)appDomain)->context);

		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}

/*
 * private static void ClearShadowCopyPathInternal(Object appDomain);
 */
void _IL_AppDomain_ClearShadowCopyPathInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

		ILContextClearShadowCopyDirs(((ILExecProcess *)appDomain)->context);

		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}

/*
 * private static void CurrentAppDomain(ref Object appDomain);
 */
void _IL_AppDomain_CurrentAppDomain(ILExecThread *thread, ILObject **appDomain)
{
	*appDomain = (ILObject *)(thread->process);
}

/*
 * private static Assembly[] GetAssembliesInternal(Object appDomain);
 */
System_Array *_IL_AppDomain_GetAssembliesInternal(ILExecThread *thread,
												  ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		ILContext *context;
		ILImage *image;
		ILInt32 num;
		System_Array *array;
		ILObject **buffer;
		ILImage **images;
		ILImage **ptr;

		IL_METADATA_RDLOCK((ILExecProcess *)appDomain);

		context = ((ILExecProcess *)appDomain)->context;

		/* count out the number of images */
		image = 0;
		num = 0;
		while((image = ILContextNextImage(context, image)) != 0)
		{
			++num;
		}

		/* create the image array */
		if (!(images = (ILImage **)ILMalloc(sizeof(ILImage *)*num)))
		{
			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}

		/* fill the image array */
		image = 0;
		ptr = images;
		while((image = ILContextNextImage(context, image)) != 0)
		{
			*ptr = image;
			++ptr;
		}

		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);

		/* create the assembly array */
		array = (System_Array *)ILExecThreadNew(thread,
	                                        "[oSystem.Reflection.Assembly;",
	                                        "(Ti)V", (ILVaInt)num);
		if(!array)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}

		/* fill the assembly array */
		ptr = images;
		buffer = (ILObject **)(ArrayToBuffer(array));
		while(num > 0)
		{
			*buffer = ImageToAssembly(thread, *ptr);
			++buffer;
			++ptr;
			--num;
		}

		/* cleanup */
		ILFree(images);

		/* return the assembly array */
		return array;
	}
	return (System_Array *)0;
}

/*
 * private static int GetIdInternal(Object appDomain);
 */
ILInt32 _IL_AppDomain_GetIdInternal(ILExecThread *thread, ILObject *appDomain)
{
#ifdef IL_CONFIG_APPDOMAINS
	return ((ILExecProcess *)appDomain)->id;
#else
	return 1;
#endif
}

/*
 * private static bool IsDefaultAppDomainInternal(Object appDomain);
 */
ILBool _IL_AppDomain_IsDefaultAppDomainInternal(ILExecThread *thread, ILObject *appDomain)
{
#ifdef IL_CONFIG_APPDOMAINS
	ILExecEngine *engine = ILExecEngineInstance();
	if (engine) /* this should always be true */
	{
		if ((ILExecProcess *)appDomain == engine->defaultProcess)
		{
			return (ILBool)1;
		}
		else
		{
			return (ILBool)0;
		}
	}
#endif
	return (ILBool)1;
}

/*
 * private static bool IsFinalizingForUnloadInternal(Object appDomain);
 */
ILBool _IL_AppDomain_IsFinalizingForUnloadInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(((ILExecProcess *)appDomain)->state == _IL_PROCESS_STATE_RUNNING_FINALIZERS)
	{
		return (ILBool)1;
	}
	else
	{
		return (ILBool)0;
	}
}

/*
 * private static String GetBaseDirectoryInternal(Object appDomain);
 */
ILString *_IL_AppDomain_GetBaseDirectoryInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		const char *baseDir;

		IL_METADATA_RDLOCK((ILExecProcess *)appDomain);

		baseDir = ILContextGetApplicationBaseDir(((ILExecProcess *)appDomain)->context);

		if(baseDir)
		{
			ILString *result;

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);

			result = ILStringCreate(thread, baseDir);
			if(!result)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			return result;
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
	return 0;
}

/*
 * private static void SetBaseDirectoryInternal(Object appDomain, String baseDirectory);
 */
void _IL_AppDomain_SetBaseDirectoryInternal(ILExecThread *thread, ILObject *appDomain, ILString *baseDirectory)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if(baseDirectory)
		{
			char *path = _ToAnsiString(thread, baseDirectory);

			if(!path)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}	
		
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILContextSetApplicationBaseDir(((ILExecProcess *)appDomain)->context, path);
		}
		else
		{
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILContextSetApplicationBaseDir(((ILExecProcess *)appDomain)->context, 0);
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}


/*
 * private static String GetFriendlyNameInternal(Object appDomain);
 */
ILString *_IL_AppDomain_GetFriendlyNameInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		char *friendlyName;

		IL_METADATA_RDLOCK((ILExecProcess *)appDomain);

		friendlyName = _ILExecProcessGetFriendlyName(((ILExecProcess *)appDomain));

		if(friendlyName)
		{
			ILString *result;

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);

			result = ILStringCreate(thread, friendlyName);
			if(!result)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			return result;
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
	return 0;
}

/*
 * private static void SetFriendlyNameInternal(Object appDomain, String friendlyName);
 */
void _IL_AppDomain_SetFriendlyNameInternal(ILExecThread *thread, ILObject *appDomain, ILString *friendlyName)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if(friendlyName)
		{
			char *name = _ToAnsiString(thread, friendlyName);

			if(!name)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}	
		
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILExecProcessSetFriendlyName((ILExecProcess *)appDomain, name);
		}
		else
		{
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILExecProcessSetFriendlyName((ILExecProcess *)appDomain, 0);
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}

/*
 * private static String GetRelativeSearchPathInternal(Object appDomain);
 */
ILString *_IL_AppDomain_GetRelativeSearchPathInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		const char *appRelativePath;

		IL_METADATA_RDLOCK((ILExecProcess *)appDomain);

		appRelativePath = ILContextGetRelativeSearchDir(((ILExecProcess *)appDomain)->context);

		if(appRelativePath)
		{
			ILString *result;

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);

			result = ILStringCreate(thread, appRelativePath);
			if(!result)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			return result;
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
	return 0;
}

/*
 * private static void SetRelativeSearchPathInternal(Object appDomain, String appRelativePath);
 */
void _IL_AppDomain_SetRelativeSearchPathInternal(ILExecThread *thread,
												 ILObject *appDomain,
												 ILString *appRelativePath)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if(appRelativePath)
		{
			char *path = _ToAnsiString(thread, appRelativePath);

			if(!path)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}	
		
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILContextSetRelativeSearchDir(((ILExecProcess *)appDomain)->context, path);
		}
		else
		{
			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			ILContextSetRelativeSearchDir(((ILExecProcess *)appDomain)->context, 0);
		}
		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}

/*
 * private static bool GetShadowCopyFilesInternal(Object appDomain);
 */
ILBool _IL_AppDomain_GetShadowCopyFilesInternal(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if(ILContextGetShadowCopyFiles(((ILExecProcess *)appDomain)->context))
		{
			return (ILBool)1;
		}
	}
	return (ILBool)0;
}

/*
 * private static void SetShadowCopyFilesInternal(Object appDomain, ILBool shadowCopyFiles);
 */
void _IL_AppDomain_SetShadowCopyFilesInternal(ILExecThread *thread,
											  ILObject *appDomain,
											  ILBool shadowCopyFiles)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

		ILContextSetShadowCopyFiles(((ILExecProcess *)appDomain)->context, (int)shadowCopyFiles);

		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
	}
}

/*
 * private static void SetShadowCopyPathInternal(Object appDomain, String[] splitPaths);
 */
void _IL_AppDomain_SetShadowCopyPathInternal(ILExecThread *thread,
											 ILObject *appDomain,
											 System_Array *splitPaths)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if (splitPaths && (ArrayLength(splitPaths) > 0))
		{
			char **shadowCopyDirs;
			int numShadowCopyDirs = 0;
			System_String **buffer = (System_String **)ArrayToBuffer(splitPaths);
			int i;  /* loop counter */

			/* allocate the array for the directories */
			if (!(shadowCopyDirs = (char **)ILMalloc(sizeof(char *) * (ArrayLength(splitPaths)))))
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}
		
			/* now copy the dirs */
			for (i = 0; i < ArrayLength(splitPaths); i++)
			{
				if (buffer[i] && buffer[i]->length)
				{
					shadowCopyDirs[numShadowCopyDirs] = _ToAnsiString(thread, (ILString *)buffer[i]);
					numShadowCopyDirs++;
				}
			}

			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			/* now set the new shadow copy dirs */
			ILContextSetShadowCopyDirs(((ILExecProcess *)appDomain)->context, shadowCopyDirs, numShadowCopyDirs);

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
		}
		else
		{
			_IL_AppDomain_ClearShadowCopyPathInternal(thread, appDomain);
		}
	}
}


/*
 * Load errors.  These must be kept in sync with "pnetlib".
 */
#define	LoadError_OK			0
#define	LoadError_InvalidName	1
#define	LoadError_FileNotFound	2
#define	LoadError_BadImage		3
#define	LoadError_Security		4

/*
 * private static Assembly LoadFromName(String name, out int error,
 *										Assembly parent);
 */
ILObject *_IL_AppDomain_LoadFromName(ILExecThread *thread,
									ILObject *appDomain,
									ILString *name,
									ILInt32 *error,
									ILObject *parent)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		ILProgramItem *item = (ILProgramItem *)_ILClrFromObject(thread, parent);
		ILImage *image = ((item != 0) ? ILProgramItem_Image(item) : 0);
		char *str = ILStringToUTF8(thread, name);
		if(image && str)
		{
			int len;
			int loadError;
			ILImage *newImage;
			len = strlen(str);
			if(len > 4 && str[len - 4] == '.' &&
				(str[len - 3] == 'd' || str[len - 3] == 'D') &&
				(str[len - 2] == 'l' || str[len - 2] == 'L') &&
		 		(str[len - 1] == 'l' || str[len - 1] == 'L'))
			{
				/* Remove ".dll", to get the assembly name */
				str[len - 4] = '\0';
			}
			loadError = ILImageLoadAssembly(str, ((ILExecProcess *)appDomain)->context,
										image, &newImage);
			if(loadError == 0)
			{
				return ImageToAssembly(thread, newImage);
			}
			else if(loadError == -1)
			{
				*error = LoadError_FileNotFound;
				return 0;
			}
			else if(loadError == IL_LOADERR_MEMORY)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return 0;
			}
			else
			{
				*error = LoadError_BadImage;
				return 0;
			}
		}
		else
		{
			*error = LoadError_FileNotFound;
			return 0;
		}
	}
	return 0;
}

/*
 * internal static Assembly LoadFromFile(String name, out int error,
 *										 Assembly parent);
 */
ILObject *_IL_AppDomain_LoadFromFile(ILExecThread *thread,
									ILObject *appDomain,
									ILString *name,
									ILInt32 *error,
									ILObject *parent)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		char *filename;
		ILImage *image;
		int loadError;

		/* Convert the name into a NUL-terminated filename string */
		filename = ILStringToAnsi(thread, name);
		if(!filename)
		{
			*error = LoadError_FileNotFound;
			return 0;
		}

		/* TODO: validate the pathname */
		if(*filename == '\0')
		{
			*error = LoadError_InvalidName;
			return 0;
		}

		/* TODO: check security permissions */

		/* Load from context if it exists already */

		image = ILContextGetFile(((ILExecProcess *)appDomain)->context, filename);

		if(image != NULL)
		{
			*error = LoadError_OK;
			return ImageToAssembly(thread, image);
		}

		/* Attempt to load the file */
		loadError = ILImageLoadFromFile(filename, ((ILExecProcess *)appDomain)->context,
									&image, IL_LOADFLAG_FORCE_32BIT, 0);
		if(loadError == 0)
		{
			*error = LoadError_OK;
			return ImageToAssembly(thread, image);
		}

		/* Convert the error code into something the C# library knows about */
		if(loadError == -1)
		{
			*error = LoadError_FileNotFound;
		}
		else if(loadError == IL_LOADERR_MEMORY)
		{
			*error = LoadError_FileNotFound;
			ILExecThreadThrowOutOfMemory(thread);
		}
		else
		{
			*error = LoadError_BadImage;
		}
	}
	return 0;
}

/*
 * internal static Assembly LoadFromBytes(byte[] bytes, out int error,
 *										  Assembly parent);
 */
ILObject *_IL_AppDomain_LoadFromBytes(ILExecThread *thread,
									 ILObject *appDomain,
									 System_Array *bytes,
									 ILInt32 *error,
									 ILObject *parent)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		ILImage *image;
		int loadError;

		/* Bail out if "bytes" is NULL (should be trapped higher
		   up the stack, but let's be paranoid anyway) */
		if(!bytes)
		{
			*error = LoadError_FileNotFound;
			return 0;
		}

		/* TODO: check security permissions */

		/* Attempt to load the image.  Note: we deliberately don't supply
			the "IL_LOADFLAG_IN_PLACE" flag because we don't want the user
			to be able to modify the IL binary after we have loaded it.
			Or worse, have the garbage collector throw the "bytes" array away */
		loadError = ILImageLoadFromMemory(ArrayToBuffer(bytes),
									  (unsigned long)(long)(ArrayLength(bytes)),
									  ((ILExecProcess *)appDomain)->context,
									  &image, IL_LOADFLAG_FORCE_32BIT, 0);
		if(loadError == 0)
		{
			*error = LoadError_OK;
			return ImageToAssembly(thread, image);
		}

		/* Convert the error code into something the C# library knows about */
		if(loadError == -1)
		{
			*error = LoadError_FileNotFound;
		}
		else if(loadError == IL_LOADERR_MEMORY)
		{
			*error = LoadError_FileNotFound;
			ILExecThreadThrowOutOfMemory(thread);
		}
		else
		{
			*error = LoadError_BadImage;
		}
	}
	return 0;
}

/*
 * private static void UnloadAppDomain(Object appDomain);
 */
void _IL_AppDomain_UnloadAppDomain(ILExecThread *thread, ILObject *appDomain)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
#ifdef IL_CONFIG_APPDOMAINS
		if (appDomain)
		{
			ILExecEngine *engine = ILExecEngineInstance();
			if (engine)   /* this should always be true */
			{
				/* we aren't allowed to unload the default AppDomain */
				if ((ILExecProcess *)appDomain == engine->defaultProcess)
			{
					/* bail out */
					/* Throw an CannotUnloadAppDomainException */
					ILExecThreadThrowSystem(thread, 
										"System.CannotUnloadAppDomainException",
										(const char *)0);
					return;
				}
				/* if the current thread runs in the specified process */
				/* we need to start an other thread to do the Unload */
				if (thread->process == (ILExecProcess *)appDomain)
				{
					/* TODO */
				}
				ILExecProcessUnload((ILExecProcess *)appDomain);
			}
		}
#else
		/* we don't support multiple Appdomains so there is nothing to do */
#endif
	}
}

/*
 * private static void GetPrivateBinPaths(Object appDomain, ref String[] splitPaths);
 */
void _IL_AppDomainSetup_GetPrivateBinPaths(ILExecThread *thread,
										   ILObject *appDomain,
										   System_Array **splitPaths)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		char **libraryDirs = 0;
		int numLibraryDirs = 0;

		IL_METADATA_RDLOCK((ILExecProcess *)appDomain);

		/* now clear the library dirs */
		ILContextGetLibraryDirs(((ILExecProcess *)appDomain)->context, &libraryDirs, &numLibraryDirs);

		IL_METADATA_UNLOCK((ILExecProcess *)appDomain);

		if (numLibraryDirs > 0)
		{
			System_Array *array;
			ILString **buffer;
			int i; /* loop counter */

			/* create the assembly array */
			array = (System_Array *)ILExecThreadNew(thread,
												"[oSystem.String;",
												"(Ti)V", (ILVaInt)numLibraryDirs);
			if (!array)
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}

			buffer = ArrayToBuffer(array);

			for (i = 0; i < numLibraryDirs; i++)
			{
				buffer[i] = ILStringCreate(thread, libraryDirs[i]);
			}
			/* return the array */
			*splitPaths = array;
		}
		else
		{
			*splitPaths = 0;
		}
	}
}

/*
 * private static void SetPrivateBinPaths(Object appDomain, String[] splitPaths);
 */
void _IL_AppDomainSetup_SetPrivateBinPaths(ILExecThread *thread,
										   ILObject *appDomain,
										   System_Array *splitPaths)
{
	if(!IsAppDomainUnloaded(thread, (ILExecProcess *)appDomain))
	{
		if (splitPaths && (ArrayLength(splitPaths) > 0))
		{
			char **libraryDirs = 0;
			int numLibraryDirs = 0;
			System_String **buffer = (System_String **)ArrayToBuffer(splitPaths);
			int i;  /* loop counter */

			/* allocate the array for the the new directories */
			if (!(libraryDirs = (char **)ILMalloc(sizeof(char *) * ArrayLength(splitPaths))))
			{
				ILExecThreadThrowOutOfMemory(thread);
				return;
			}
		
			/* now append the dirs */
			for (i = 0; i < ArrayLength(splitPaths); i++)
			{
				if (buffer[i] && buffer[i]->length)
				{
					libraryDirs[numLibraryDirs] = _ToAnsiString(thread, (ILString *)buffer[i]);
					numLibraryDirs++;
				}
			}

			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			/* now set the new library dirs */
			ILContextSetLibraryDirs(((ILExecProcess *)appDomain)->context, libraryDirs, numLibraryDirs);

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
		}
		else
		{

			IL_METADATA_WRLOCK((ILExecProcess *)appDomain);

			/* now clear the library dirs */
			ILContextClearLibraryDirs(((ILExecProcess *)appDomain)->context);

			IL_METADATA_UNLOCK((ILExecProcess *)appDomain);
		}
	}
}


#ifdef	__cplusplus
};
#endif
