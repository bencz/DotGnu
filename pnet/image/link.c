/*
 * link.c - Dynamic linking support for IL images.
 *
 * Copyright (C) 2001, 2002, 2009  Southern Storm Software, Pty Ltd.
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

#include "program.h"
#include "il_serialize.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Various default path lists.
 */
#define	CSCC_LIB_PATH_DEFAULT	\
			"/usr/local/lib/cscc/lib:/usr/lib/cscc/lib"
#if defined(__x86_64) || defined(__x86_64__)
/* Note: x86_64 is a special case as it has 32 bit binaries
		 in /lib and 64 bit in /lib64*/
#define	LD_LIBRARY_PATH_DEFAULT	\
			("/usr/local/lib64:/usr/X11R6/lib64:/usr/lib64:" \
			"/usr/local/lib:/usr/X11R6/lib:/usr/lib")
#else
#define	LD_LIBRARY_PATH_DEFAULT	\
			"/usr/local/lib:/usr/X11R6/lib:/usr/lib"
#endif

/*
 * The cached system path, loaded from the environment.  The "important"
 * system path is searched before looking in the same directory as the
 * assembly, to prevent conflicts with foreign CLI's for core libraries.
 */
static char **systemPath = 0;
static int systemPathSize = 0;
static char **importantSystemPath = 0;
static int importantSystemPathSize = 0;

/*
 * Cached information from "pinvoke.map" files.
 */
typedef struct _tagILMapContents
{
	char *name;
	int nameLen;
	char *systemType;
	char *remappedName;
	struct _tagILMapContents *next;

} ILMapContents;
typedef struct _tagILMapDirectory
{
	char *directory;
	int directoryLen;
	ILMapContents *contents;
	struct _tagILMapDirectory *next;

} ILMapDirectory;
static ILMapDirectory *mapDirs = 0;
static ILMapDirectory *lastMapDir = 0;

/*
 * Add a pathname to "systemPaths".
 */
static void AddSystemPath(const char *path, int len, int importantPath)
{
	char **newPathList;
	if(importantPath)
	{
		newPathList = (char **)ILRealloc
			(importantSystemPath,
			 (importantSystemPathSize + 1) * sizeof(char *));
		if(!newPathList)
		{
			return;
		}
		importantSystemPath = newPathList;
		importantSystemPath[importantSystemPathSize] = ILDupNString(path, len);
		if(!(importantSystemPath[importantSystemPathSize]))
		{
			return;
		}
		++importantSystemPathSize;
	}
	else
	{
		newPathList = (char **)ILRealloc
			(systemPath, (systemPathSize + 1) * sizeof(char *));
		if(!newPathList)
		{
			return;
		}
		systemPath = newPathList;
		systemPath[systemPathSize] = ILDupNString(path, len);
		if(!(systemPath[systemPathSize]))
		{
			return;
		}
		++systemPathSize;
	}
}

/*
 * Split a pathname list and add it to the global list of system paths.
 */
static void SplitPathString(char *list, char *stdpath, char *defaultPath,
							int importantPath)
{
	int len;
	int separator;

	/* Use the default path if necessary */
	if(!list || *list == '\0')
	{
		/* If we have a standard path, then add that */
		if(stdpath)
		{
			AddSystemPath(stdpath, strlen(stdpath), importantPath);
			ILFree(stdpath);
			stdpath = 0;
		}

#ifdef IL_WIN32_NATIVE
		/* Ignore default paths, as they won't make any sense */
		return;
#else
		list = defaultPath;
		if(!list)
		{
			return;
		}
#endif
	}

	/* Garbage-collect the standard path */
	if(stdpath)
	{
		ILFree(stdpath);
	}

	/* Determine the path separator to use */
#ifdef IL_WIN32_PLATFORM
	if(strchr(list, ';') != 0)
	{
		/* The path already uses ';', so that is probably the separator */
		separator = ';';
	}
	else
	{
		/* Deal with the ambiguity between ':' used as a separator
		   and ':' used to specify a drive letter */
		if(((list[0] >= 'A' && list[0] <= 'Z') ||
		    (list[0] >= 'a' && list[0] <= 'z')) && list[1] == ':')
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

	/* Split the pathname list */
	while(*list != '\0')
	{
		/* Skip separators between directory pathnames */
		if(*list == separator || *list == ' ' || *list == '\t' ||
		   *list == '\r' || *list == '\n')
		{
			++list;
			continue;
		}

		/* Get the next directory pathname */
		len = 1;
		while(list[len] != '\0' && list[len] != separator)
		{
			++len;
		}
		while(list[len - 1] == ' ' || list[len - 1] == '\t' ||
		      list[len - 1] == '\r' || list[len - 1] == '\n')
		{
			--len;
		}

		/* Add the path to the global list */
		AddSystemPath(list, len, importantPath);

		/* Advance to the next path */
		list += len;
	}
}

/*
 * Load the system path.
 */
static void LoadSystemPath(void)
{
#if !defined(__palmos__)
	if(!systemPath && !importantSystemPath)
	{
		SplitPathString(getenv("CSCC_LIB_PATH"),
						ILGetStandardLibraryPath("cscc/lib"),
						CSCC_LIB_PATH_DEFAULT, 1);
		SplitPathString(getenv("MONO_PATH"),
						ILGetStandardLibraryPath(0), 0, 0);
		SplitPathString(getenv("LD_LIBRARY_PATH"), 0,
						LD_LIBRARY_PATH_DEFAULT, 0);
	#ifdef IL_WIN32_PLATFORM
		/* Win32: try looking in PATH also, since Win32 puts dll's there */
		SplitPathString(getenv("PATH"), 0, 0, 0);
	#endif
	}
#endif
}

/*
 * Test a specific pathname for a file.
 */
static char *TestPathForFile(const char *pathname, int pathlen,
							 const char *name, int namelen,
							 const char *prefix, const char *suffix,
							 int *retryLower, int forceLower)
{
	int fullLen;
	int posn;
	char *path;

	/* Determine the length of the full pathname */
	fullLen = pathlen + namelen + 1;
	if(pathlen)
	{
		++fullLen;
	}
	if(prefix)
	{
		fullLen += strlen(prefix);
	}
	if(suffix)
	{
		fullLen += strlen(suffix);
	}

	/* Build the full pathname */
	path = (char *)ILMalloc(fullLen + 3);
	if(!path)
	{
		return 0;
	}
	if(pathlen)
	{
		ILMemCpy(path, pathname, pathlen);
		if(path[pathlen - 1] != '/' && path[pathlen - 1] != '\\')
		{
			path[pathlen] = '/';
			posn = pathlen + 1;
		}
		else
		{
			posn = pathlen;
		}
	}
	else
	{
		posn = 0;
	}
	if(prefix)
	{
		strcpy(path + posn, prefix);
		posn += strlen(prefix);
	}
	if(forceLower)
	{
		while(namelen > 0)
		{
			if(*name >= 'A' && *name <= 'Z')
			{
				path[posn++] = (*name++ - 'A' + 'a');
			}
			else
			{
				path[posn++] = *name++;
			}
			--namelen;
		}
	}
	else if(retryLower)
	{
		*retryLower = 0;
		while(namelen > 0)
		{
			if(*name >= 'A' && *name <= 'Z')
			{
				*retryLower = 1;
			}
			path[posn++] = *name++;
			--namelen;
		}
	}
	else
	{
		ILMemCpy(path + posn, name, namelen);
		posn += namelen;
	}
	if(suffix)
	{
		strcpy(path + posn, suffix);
		posn += strlen(suffix);
	}
	path[posn] = '\0';

	/* Test for the file's presence */
	if(ILFileExists(path, 0))
	{
		return path;
	}

	/* If the suffix is ".dll", then try again with ".DLL" */
	if(suffix && !strcmp(suffix, ".dll"))
	{
		strcpy(path + posn - 3, "DLL");
		if(ILFileExists(path, 0))
		{
			return path;
		}

#ifdef IL_CONFIG_GZIP
		/* Try ".dll.gz" as well, in case the library was compressed */
		strcpy(path + posn - 3, "dll.gz");
		if(ILFileExists(path, 0))
		{
			return path;
		}
#endif
	}

	/* Not found */
	ILFree(path);
	return 0;
}

/*
 * Extract a field from a buffer.
 */
static char *ExtractField(char *buffer, int *_posn)
{
	int posn = *_posn;
	char *result;
	while(buffer[posn] != '\0' && buffer[posn] != '#' &&
		  (buffer[posn] == ' ' || buffer[posn] == '\t' ||
		   buffer[posn] == '\r' || buffer[posn] == '\n'))
	{
		++posn;
	}
	result = buffer + posn;
	while(buffer[posn] != '\0' && buffer[posn] != '#' &&
		  buffer[posn] != ' ' && buffer[posn] != '\t' &&
		  buffer[posn] != '\r' && buffer[posn] != '\n')
	{
		++posn;
	}
	if(buffer[posn] == '#')
	{
		buffer[posn] = '\0';
	}
	else if(buffer[posn] != '\0')
	{
		buffer[posn++] = '\0';
	}
	*_posn = posn;
	return result;
}

/*
 * Load the contents of a "pinvoke.map" file.
 */
static ILMapDirectory *LoadPInvokeMap(const char *pathname, int pathlen)
{
	ILMapDirectory *dir;
	char *mapFile;
	FILE *file;
	char buffer[BUFSIZ];
	char *field1;
	char *field2;
	char *field3;
	int posn;
	ILMapContents *contents;
	ILMapContents *last;

	/* Allocate space for the directory and initialize it */
	dir = (ILMapDirectory *)ILMalloc(sizeof(ILMapDirectory));
	if(!dir)
	{
		return 0;
	}
	dir->directory = ILDupNString(pathname, pathlen);
	if(!(dir->directory))
	{
		ILFree(dir);
		return 0;
	}
	dir->directoryLen = pathlen;
	dir->contents = 0;
	dir->next = 0;
	if(lastMapDir)
	{
		lastMapDir->next = dir;
	}
	else
	{
		mapDirs = dir;
	}
	lastMapDir = dir;

	/* Try to open the "pinvoke.map" file */
	if((mapFile = (char *)ILMalloc(pathlen + 13)) == 0)
	{
		return dir;
	}
	strncpy(mapFile, pathname, pathlen);
	strcpy(mapFile + pathlen, "/pinvoke.map");
	if((file = fopen(mapFile, "r")) == NULL)
	{
		ILFree(mapFile);
		return dir;
	}
	ILFree(mapFile);

	/* Read the lines into memory */
	last = 0;
	while(fgets(buffer, sizeof(buffer), file))
	{
		posn = 0;
		field1 = ExtractField(buffer, &posn);
		field2 = ExtractField(buffer, &posn);
		field3 = ExtractField(buffer, &posn);
		if(field1[0] != '\0' && field2[0] != '\0' && field3[0] != '\0')
		{
			contents = (ILMapContents *)ILMalloc(sizeof(ILMapContents));
			if(!contents)
			{
				break;
			}
			contents->name = ILDupString(field1);
			if(!(contents->name))
			{
				ILFree(contents);
				break;
			}
			contents->systemType = ILDupString(field2);
			if(!(contents->systemType))
			{
				ILFree(contents->name);
				ILFree(contents);
				break;
			}
			contents->remappedName = ILDupString(field3);
			if(!(contents->remappedName))
			{
				ILFree(contents->systemType);
				ILFree(contents->name);
				ILFree(contents);
				break;
			}
			contents->nameLen = strlen(field1);
			contents->next = 0;
			if(last)
			{
				last->next = contents;
			}
			else
			{
				dir->contents = contents;
			}
			last = contents;
		}
	}
	fclose(file);

	/* Finished */
	return dir;
}

/*
 * Test a specific pathname for a native library file.
 */
static char *TestPathForNativeLib(const char *pathname, int pathlen,
							      const char *name, int namelen,
							      const char *optPrefix, const char *suffix)
{
	char *fullName;
	int retryLower;
	ILMapDirectory *dir;

	/* Look for a "pinvoke.map" file in the directory */
	dir = mapDirs;
	while(dir != 0)
	{
		if(dir->directoryLen == pathlen &&
		   !ILMemCmp(dir->directory, pathname, pathlen))
		{
			break;
		}
		dir = dir->next;
	}
	if(!dir)
	{
		/* We haven't seen this directory before, so load "pinvoke.map".
		   If there is no "pinvoke.map", then cache the fact that it
		   isn't there so that we don't try to load it again */
		LoadPInvokeMap(pathname, pathlen);
	}

	/* Try looking for an exact match */
	fullName = TestPathForFile(pathname, pathlen, name, namelen,
							   (const char *)0, suffix,
							   &retryLower, 0);

	/* Try changing the name to lower case and retrying, to get
	   around Win32 vs Unix discrepancies in naming conventions */
	if(!fullName && retryLower)
	{
		fullName = TestPathForFile(pathname, pathlen, name, namelen,
								   (const char *)0, suffix, 0, 1);
	}

	/* Try adding the "lib" prefix to the name */
	if(!fullName && optPrefix)
	{
		fullName = TestPathForFile(pathname, pathlen, name, namelen,
								   optPrefix, suffix, &retryLower, 0);

		/* Try changing the name to lower case and adding the "lib" prefix */
		if(!fullName && retryLower)
		{
			fullName = TestPathForFile(pathname, pathlen, name, namelen,
									   optPrefix, suffix, 0, 1);
		}
	}

	/* Return the name that we found, or NULL if nothing worked */
	return fullName;
}

/*
 * Test for a "pnetlib.here" file in a specific directory.
 * This file marks the location of the standard pnetlib assemblies.
 */
static int TestPathForPnetlibHere(const char *pathname, int pathlen)
{
	char *path = TestPathForFile(pathname, pathlen, "pnetlib.here", 12,
								 0, 0, 0, 0);
	if(path)
	{
		ILFree(path);
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Find a particular assembly in a set of paths.  Returns
 * non-zero if the assembly has been located.
 */
static int FindAssemblyInPaths(const char **paths, unsigned long numPaths,
							   const char *name, int namelen, int isSystem,
							   char **path, char **firstPath,
							   const ILUInt16 *version)
{
	unsigned long posn;
	char versionBuffer[64];
	const char *versionPrefix;
	if(version && (version[0] != 0 || version[1] != 0 ||
				   version[2] != 0 || version[3] != 0))
	{
		sprintf(versionBuffer, "%d.%d.%d.%d/",
				(int)(version[0]), (int)(version[1]),
				(int)(version[2]), (int)(version[3]));
		versionPrefix = versionBuffer;
	}
	else
	{
		versionBuffer[0] = '\0';
		versionPrefix = 0;
	}
	for(posn = 0; posn < numPaths; ++posn)
	{
		*path = TestPathForFile(paths[posn], strlen(paths[posn]),
							    name, namelen, versionPrefix, ".dll", 0, 0);
		if(*path)
		{
			return 1;
		}
		if(versionPrefix)
		{
			/* Retry without the version directory prefix */
			*path = TestPathForFile(paths[posn], strlen(paths[posn]),
								    name, namelen, 0, ".dll", 0, 0);
			if(*path)
			{
				return 1;
			}
		}
	}
	return 0;
}

char *ILImageSearchPath(const char *name, const ILUInt16 *version,
						const char *parentAssemblyFile,
						const char **beforePaths, unsigned long numBeforePaths,
						const char **afterPaths, unsigned long numAfterPaths,
						int suppressStandardPaths, int *sameDir)
{
	int namelen;
	char *path;
	char *firstPath;
	int len;
	int isSystem;

	/* Bail out immediately if the name is invalid */
	if(!name)
	{
		return 0;
	}
	namelen = strlen(name);
	if(namelen >= 4 && !ILStrICmp(name + namelen - 4, ".dll"))
	{
		/* Strip ".dll" from the end of the name */
		namelen -= 4;
	}
	if(namelen == 0)
	{
		return 0;
	}

	/* Convert "corlib" into "mscorlib", in case we loaded a Mono app */
	if(namelen == 6 && !strncmp(name, "corlib", 6))
	{
		name = "mscorlib";
		namelen = 8;
	}

	/* Is this a system assembly that we must handle specially? */
	/* Note: system check is no longer necessary - remove eventually */
	isSystem = 0;

	/* Initialize flag variables for the search */
	if(sameDir)
	{
		*sameDir = 0;
	}
	firstPath = 0;

	/* Search the before path list */
	if(FindAssemblyInPaths(beforePaths, numBeforePaths, name, namelen,
						   isSystem, &path, &firstPath, version))
	{
		return path;
	}

	/* Search the "important" standard system paths */
	if(!suppressStandardPaths)
	{
		LoadSystemPath();
		if(FindAssemblyInPaths((const char **)importantSystemPath,
							   (unsigned long)importantSystemPathSize,
							   name, namelen, isSystem, &path, &firstPath,
							   version))
		{
			return path;
		}
	}

	/* Look in the same directory as the parent assembly */
	if(sameDir)
	{
		*sameDir = 1;
	}
	if(parentAssemblyFile)
	{
		len = strlen(parentAssemblyFile);
		while(len > 0 && parentAssemblyFile[len - 1] != '/' &&
		      parentAssemblyFile[len - 1] != '\\')
		{
			--len;
		}
		if(len > 1)
		{
			--len;
		}
		path = TestPathForFile(parentAssemblyFile, len,
							   name, namelen, 0, ".dll", 0, 0);
		if(path)
		{
			if(!isSystem || TestPathForPnetlibHere(parentAssemblyFile, len))
			{
				return path;
			}
			firstPath = path;
		}
	}
	if(sameDir)
	{
		*sameDir = 0;
	}

	/* Search the standard system paths */
	if(!suppressStandardPaths)
	{
		LoadSystemPath();
		if(FindAssemblyInPaths((const char **)systemPath,
							   (unsigned long)systemPathSize, name, namelen,
							   isSystem, &path, &firstPath, version))
		{
			return path;
		}
	}

	/* Search the after path list */
	if(FindAssemblyInPaths(afterPaths, numAfterPaths, name, namelen,
						   isSystem, &path, &firstPath, version))
	{
		return path;
	}

	/* Use the first match that we found, if any */
	return firstPath;
}

int _ILImageDynamicLink(ILImage *image, const char *filename, int flags)
{
	ILContext *context = ILImageToContext(image);
	ILAssembly *assem;
	ILFileDecl *file;
	char *pathname;
	int error;
	int result = 0;
	ILImage *newImage;
	int sameDir;
	int loadFlags;
	int len, retryLower;
	ILImage *linkImage;

	/* Scan the AssemblyRef table for the assemblies that we require */
	assem = 0;
	while((assem = (ILAssembly *)ILImageNextToken
				(image, IL_META_TOKEN_ASSEMBLY_REF, assem)) != 0)
	{
		/* Ignore this assembly reference if we already have it and
		   it isn't marked as "building" */
		linkImage = ILContextGetAssembly(image->context, assem->name);
		if(linkImage && linkImage->type != IL_IMAGETYPE_BUILDING)
		{
			continue;
		}

		/* Locate the assembly along the search path */
		pathname = ILImageSearchPath(assem->name, assem->version, filename,
									 (const char **)(context->libraryDirs),
							 		 context->numLibraryDirs,
									 0, 0, 0, &sameDir);
		if(!pathname)
		{
		#if IL_DEBUG_META
			fprintf(stderr, "could not locate the assembly %s/%u.%u.%u.%u\n",
					assem->name, (unsigned)(assem->version[0]),
					(unsigned)(assem->version[1]),
					(unsigned)(assem->version[2]),
					(unsigned)(assem->version[3]));
		#endif
			result = IL_LOADERR_UNRESOLVED;
			continue;
		}

		/* If the assembly was loaded from a system directory, then we
		   can assume that it is being loaded from a secure location */
		if(!sameDir)
		{
			loadFlags = flags & ~IL_LOADFLAG_INSECURE;
		}
		else
		{
			loadFlags = flags;
		}

		/* Load the image */
		error = ILImageLoadFromFile(pathname, image->context,
							        &newImage, flags, 0);
		ILFree(pathname);
		if(error != 0)
		{
			if(error == -1)
			{
				result = IL_LOADERR_UNRESOLVED;
			}
			else
			{
				result = error;
			}
		}
	}

	/* If we loaded the parent from an insecure source, then bail out
	   without attempting to load the module files */
	if((flags & IL_LOADFLAG_INSECURE) != 0 || !filename)
	{
		return result;
	}

	/* Strip the final component from the filename */
	len = strlen(filename);
	while(len > 0 && filename[len - 1] != '/' && filename[len - 1] != '\\')
	{
		--len;
	}
	if(len > 0)
	{
		--len;
	}

	/* Scan the File table for the external module files that we require */
	file = 0;
	while((file = (ILFileDecl *)ILImageNextToken
				(image, IL_META_TOKEN_FILE, file)) != 0)
	{
		/* Ignore this file if it does not contain metadata */
		if(!ILFileDecl_HasMetaData(file))
		{
			continue;
		}

		/* Ignore this file if we already have it */
		if(ILContextGetFile(image->context, file->name) != 0)
		{
			continue;
		}

		/* Ignore this file if its name contains a '/' or '\', because
		   files in other directories may be a security risk */
		if(ILMemChr(file->name, '/', strlen(file->name)) != 0 ||
		   ILMemChr(file->name, '\\', strlen(file->name)) != 0)
		{
			continue;
		}

		/* Get the full pathname of the referenced file */
		retryLower = 0;
		pathname = TestPathForFile(filename, len,
								   file->name, strlen(file->name),
								   0, 0, &retryLower, 0);
		if(!pathname && retryLower)
		{
			pathname = TestPathForFile(filename, len,
									   file->name, strlen(file->name),
									   0, 0, &retryLower, 1);
		}
		if(!pathname)
		{
		#if IL_DEBUG_META
			fprintf(stderr, "could not locate the file %s\n", file->name);
		#endif
			result = IL_LOADERR_UNRESOLVED;
			continue;
		}

		/* Load the image */
		error = ILImageLoadFromFile(pathname, image->context,
							        &newImage, flags, 0);
		ILFree(pathname);
		if(error != 0)
		{
			if(error == -1)
			{
				result = IL_LOADERR_UNRESOLVED;
			}
			else
			{
				result = error;
			}
		}
	}

	/* Done */
	return result;
}

int _ILImageDynamicLinkModule(ILImage *image, const char *filename,
							  const char *moduleName, int flags,
							  ILImage **newImage)
{
	char *pathname;
	int error;
	int len, retryLower;

	/* Clear the return image before we start */
	*newImage = 0;

	/* If we loaded the parent from an insecure source, then bail out
	   without attempting to load the module files */
	if((flags & IL_LOADFLAG_INSECURE) != 0 || !filename)
	{
		return 0;
	}

	/* Strip the final component from the filename */
	len = strlen(filename);
	while(len > 0 && filename[len - 1] != '/' && filename[len - 1] != '\\')
	{
		--len;
	}
	if(len > 0)
	{
		--len;
	}

	/* Ignore this module if we already have it */
	if(ILContextGetFile(image->context, moduleName) != 0)
	{
		return 0;
	}

	/* Ignore this module if its name contains a '/' or '\', because
	   files in other directories may be a security risk */
	if(ILMemChr(moduleName, '/', strlen(moduleName)) != 0 ||
	   ILMemChr(moduleName, '\\', strlen(moduleName)) != 0)
	{
		return 0;
	}

	/* Get the full pathname of the referenced file */
	retryLower = 0;
	pathname = TestPathForFile(filename, len,
							   moduleName, strlen(moduleName),
							   0, 0, &retryLower, 0);
	if(!pathname && retryLower)
	{
		pathname = TestPathForFile(filename, len,
								   moduleName, strlen(moduleName),
								   0, 0, &retryLower, 1);
	}
	if(!pathname)
	{
	#if IL_DEBUG_META
		fprintf(stderr, "could not locate the file %s\n", moduleName);
	#endif
		return IL_LOADERR_UNRESOLVED;
	}

	/* Load the image */
	error = ILImageLoadFromFile(pathname, image->context,
						        newImage, flags, 0);
	ILFree(pathname);
	if(error == -1)
	{
		return IL_LOADERR_UNRESOLVED;
	}
	else
	{
		return error;
	}
}

int ILImageLoadAssembly(const char *name, ILContext *context,
						ILImage *parentImage, ILImage **image)
{
	int len;
	char *pathname;
	int sameDir;
	int flags;
	int error;

	/* Bail out if the assembly name is invalid-looking */
	if(!name)
	{
		return -1;
	}
	len = strlen(name);
	while(len > 0 && name[len - 1] != '/' && name[len - 1] != '\\' &&
		  name[len - 1] != ':')
	{
		--len;
	}
	if(len > 0)
	{
		return IL_LOADERR_UNRESOLVED;
	}

	/* See if we already have an assembly with this name */
	*image = ILContextGetAssembly(context, name);
	if(*image != 0)
	{
		return 0;
	}

	/* Try to locate the assembly relative to its parent assembly */
	pathname = ILImageSearchPath(name, (ILUInt16 *)0, parentImage->filename,
							     (const char **)(context->libraryDirs),
							     context->numLibraryDirs, 0, 0, 0, &sameDir);
	if(!pathname)
	{
		return -1;
	}

	/* If the assembly was loaded from a system directory, then we
	   can assume that it is being loaded from a secure location */
	flags = IL_LOADFLAG_FORCE_32BIT;
	if(sameDir)
	{
		flags |= (parentImage->secure ? 0 : IL_LOADFLAG_INSECURE);
	}

	/* Load the image */
	error = ILImageLoadFromFile(pathname, context, image, flags, 0);
	ILFree(pathname);
	return error;
}

#ifdef IL_CONFIG_PINVOKE

#if defined(CSCC_HOST_TRIPLET) || defined(CSCC_HOST_ALIAS)

/*
 * Determine if a platform name matches a host name that
 * was discovered during the autoconfiguration process.
 */
static int MatchHostName(const char *host, const char *platform,
						 int platformLen)
{
	while(platformLen > 0 && *host != '\0')
	{
		if(*platform == '*')
		{
			/* If this is the last charcter, then we have a match */
			++platform;
			--platformLen;
			if(platformLen == 0)
			{
				return 1;
			}

			/* Search for matches on the next character */
			while(*host != '\0')
			{
				if(*host == *platform)
				{
					if(MatchHostName(host + 1, platform + 1, platformLen - 1))
					{
						return 1;
					}
				}
				++host;
			}
			return 0;
		}
		else
		{
			/* Perform a normal character match */
			if(*platform != *host)
			{
				return 0;
			}
			++platform;
			--platformLen;
			++host;
		}
	}
	return (platformLen == 0 && *host == '\0');
}

#endif	/* CSCC_HOST_TRIPLET || CSCC_HOST_ALIAS */

/*
 * Determine if we have a match against a "DllImportMap" attribute.
 */
static int DllMapMatch(const char *name,
					   const char *platform, int platformLen,
					   const char *oldName, int oldNameLen)
{
	/* Check for a name match first */
	if(strlen(name) != oldNameLen ||
	   ILMemCmp(name, oldName, oldNameLen) != 0)
	{
		return 0;
	}

	/* Match against either the host triplet or alias that
	   was discovered during the autoconfiguration process */
#ifdef CSCC_HOST_TRIPLET
	if(MatchHostName(CSCC_HOST_TRIPLET, platform, platformLen))
	{
		return 1;
	}
#endif
#ifdef CSCC_HOST_ALIAS
	if(MatchHostName(CSCC_HOST_ALIAS, platform, platformLen))
	{
		return 1;
	}
#endif

	/* Is the platform code a standard fallback? */
#ifndef IL_WIN32_PLATFORM
	if(platformLen == 17 &&
	   !ILMemCmp(platform, "std-shared-object", 17))
	{
		return 1;
	}
#else
	if(platformLen == 13 &&
	   !ILMemCmp(platform, "std-win32-dll", 13))
	{
		return 1;
	}
#endif

	/* The platform did not match */
	return 0;
}

/*
 * Search for a "DllImportMap" attribute on a program item.
 * Returns the length of the new name, or -1 if not found.
 */
static int SearchForDllMap(ILProgramItem *item, const char *name,
						   const char **remapName)
{
	ILAttribute *attr;
	ILMethod *method;
	const void *blob;
	ILUInt32 blobLen;
	ILSerializeReader *reader;
	const char *platform;
	int platformLen;
	const char *oldName;
	int oldNameLen;
	const char *newName;
	int newNameLen;

#ifndef IL_WIN32_PLATFORM
	/* Hack around a hard-wired library names in Gtk#.  This is temporary
	   until we can come up with a better solution that does not require
	   the user to manually edit configuration files */
	newNameLen = strlen(name);
	if(newNameLen > 4)
	{
		/* strip off the .dll suffix if present */
		if((name[newNameLen - 4] == '.') && (name[newNameLen - 3] == 'd') &&
			(name[newNameLen - 2] == 'l') && (name[newNameLen - 1] == 'l'))
		{
			newNameLen -= 4;
			/* now strip of the lib at the beginning if present */
			if(newNameLen > 3)
			{
				if((name[0] == 'l') && (name[1] == 'i') && (name[2] == 'b'))
				{
					int index
;
					newName = name + 3;
					newNameLen -= 3;

					/* now search for the first dash after the last dot */
					index = newNameLen -1;

					while(index >= 0)
					{
						if(newName[index] == '.')
						{
							/* found the dot */
							index ++;

							while(index < newNameLen)
							{
								if(newName[index] == '-')
								{
									newNameLen = index;
									break;
								}
								index++;
							}
							break;
						}
						index--;
					}
					if((newNameLen == 13) && 
						!strncmp(newName, "gtk-win32-2.0", 13))
					{
						*remapName = "gtk-x11-2.0";
					}
					else
					{
						if((newNameLen == 13) &&
							!strncmp(newName, "gdk-win32-2.0", 13))
						{
							*remapName = "gdk-x11-2.0";
						}
						else
						{
							*remapName = newName;
						}
					}
					return newNameLen;
				}
			}
		}
	}
#endif

	attr = 0;
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		method = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
		if(method != 0 && !strcmp(method->member.owner->className->name,
								  "DllImportMapAttribute"))
		{
			blob = ILAttributeGetValue(attr, &blobLen);
			if(blob &&
			   (reader = ILSerializeReaderInit(method, blob, blobLen)) != 0)
			{
				/* Get the parameter values from the attribute */
				if(ILSerializeReaderGetParamType(reader) ==
						IL_META_SERIALTYPE_STRING)
				{
					platformLen = ILSerializeReaderGetString
							(reader, &platform);
					if(platformLen != -1 &&
					   ILSerializeReaderGetParamType(reader) ==
							IL_META_SERIALTYPE_STRING)
					{
						oldNameLen = ILSerializeReaderGetString
								(reader, &oldName);
						if(oldNameLen != -1 &&
						   ILSerializeReaderGetParamType(reader) ==
								IL_META_SERIALTYPE_STRING)
						{
							newNameLen = ILSerializeReaderGetString
									(reader, &newName);
							if(newNameLen != -1 &&
							   DllMapMatch(name, platform, platformLen,
							   			   oldName, oldNameLen))
							{
								/* We have found a match */
								*remapName = newName;
								return newNameLen;
							}
						}
					}
				}
				ILSerializeReaderDestroy(reader);
			}
		}
	}

	/* The attribute is not present */
	return -1;
}

/*
 * Search the path for a native library.
 */
static char *PInvokeSearchPath(ILPInvoke *pinvoke, char *baseName,
							   const char *optPrefix, const char *suffix)
{
	const char *name;
	int namelen;
	int posn;
	char *fullName;
	ILContext *context;

	/* Look in the same directory as the assembly first,
	   in case the programmer has shipped the native library
	   along-side the assembly that imports it */
	if((name = pinvoke->member.programItem.image->filename) != 0)
	{
		/* Get the name of the directory containing the assembly */
		namelen = strlen(name);
		while(namelen > 0 && name[namelen - 1] != '/' &&
			  name[namelen - 1] != '\\')
		{
			--namelen;
		}
		if(namelen > 1)
		{
			--namelen;
		}

		/* Look for the native library */
		fullName = TestPathForNativeLib(name, namelen,
										baseName, strlen(baseName),
								        optPrefix, suffix);
		if(fullName)
		{
			ILFree(baseName);
			return fullName;
		}
	}

	/* Look in the user-specified library search directories */
	context = pinvoke->member.programItem.image->context;
	for(posn = 0; posn < context->numLibraryDirs; ++posn)
	{
		fullName = TestPathForNativeLib(context->libraryDirs[posn],
									    strlen(context->libraryDirs[posn]),
									    baseName, strlen(baseName),
										optPrefix, suffix);
		if(fullName)
		{
			ILFree(baseName);
			return fullName;
		}
	}

	/* Look on the standard system search path */
	LoadSystemPath();
	for(posn = 0; posn < importantSystemPathSize; ++posn)
	{
		fullName = TestPathForNativeLib(importantSystemPath[posn],
									    strlen(importantSystemPath[posn]),
									    baseName, strlen(baseName),
										optPrefix, suffix);
		if(fullName)
		{
			ILFree(baseName);
			return fullName;
		}
	}
	for(posn = 0; posn < systemPathSize; ++posn)
	{
		fullName = TestPathForNativeLib(systemPath[posn],
									    strlen(systemPath[posn]),
									    baseName, strlen(baseName),
										optPrefix, suffix);
		if(fullName)
		{
			ILFree(baseName);
			return fullName;
		}
	}

	/* If we get here, then we were unable to find the name */
	return 0;
}

char *ILPInvokeResolveModule(ILPInvoke *pinvoke)
{
	const char *name;
	const char *remapName;
	int namelen;
#ifndef IL_WIN32_PLATFORM
	int posn;
#endif
	char *baseName;
	char *fullName;
	const char *optPrefix = 0;
	const char *suffix = 0;
	const char *systemType;
	ILMapDirectory *dir;
	ILMapContents *contents;

	/* Validate the module name that was provided */
	if(!pinvoke || !(pinvoke->module) || !(pinvoke->module->name) ||
	   pinvoke->module->name[0] == '\0')
	{
		return 0;
	}
	name = pinvoke->module->name;

	/* Disallow the request if the image containing the PInvoke
	   declaration is marked as insecure */
	if(!(pinvoke->member.programItem.image->secure))
	{
		return 0;
	}

	/* Does the name need to be remapped for this platform? */
	namelen = SearchForDllMap(&(pinvoke->memberInfo->programItem),
							  name, &remapName);
	if(namelen == -1)
	{
		namelen = SearchForDllMap(&(pinvoke->memberInfo->owner->programItem),
							  	  name, &remapName);
		if(namelen == -1)
		{
			namelen = strlen(name);
		}
		else
		{
			name = remapName;
		}
	}
	else
	{
		name = remapName;
	}

	/* If the name already includes a root directory specification,
	   then assume that this is the path we are looking for */
	if(namelen > 0 && (name[0] == '/' || name[0] == '\\'))
	{
		return ILDupNString(name, namelen);
	}
	else if(namelen > 1 && name[1] == ':')
	{
		return ILDupNString(name, namelen);
	}

	/* Determine the platform-specific prefix and suffix to add */
#ifndef IL_WIN32_PLATFORM
	systemType = "so";
	{
		int needSuffix = 1;
		posn = 0;
		while(posn <= (namelen - 3))
		{
			if(name[posn] == '.' && name[posn + 1] == 's' &&
			   name[posn + 2] == 'o' &&
			   ((posn + 3) == namelen || name[posn + 3] == '.'))
			{
				/* The name already includes ".so" somewhere */
				needSuffix = 0;
				break;
			}
			++posn;
		}
		if(needSuffix)
		{
			/* Strip ".dll" from the end of the name if present */
			if(namelen >= 4 && name[namelen - 4] == '.' &&
			   (name[namelen - 3] == 'd' || name[namelen - 3] == 'D') &&
			   (name[namelen - 2] == 'l' || name[namelen - 2] == 'L') &&
			   (name[namelen - 1] == 'l' || name[namelen - 1] == 'L'))
			{
				namelen -= 4;
				needSuffix = 1;
			}
		}
		if(needSuffix)
		{
		#if defined(__APPLE__) && defined(__MACH__)
			suffix = ".dylib";
			systemType = "dylib";
		#else
			suffix = ".so";
		#endif
		}
	}
#else
	/* Add ".dll" to the end of the name if not present */
	systemType = "dll";
	if(namelen < 4 || name[namelen - 4] != '.' ||
	   (name[namelen - 3] != 'd' && name[namelen - 3] != 'D') ||
	   (name[namelen - 2] != 'l' && name[namelen - 2] != 'L') ||
	   (name[namelen - 1] != 'l' && name[namelen - 1] != 'L'))
	{
		suffix = ".dll";
	}
#endif
	if(namelen >= 3 && strncmp(name, "lib", 3) != 0)
	{
		optPrefix = "lib";
	}
	baseName = ILDupNString(name, namelen);

	/* Search the path for the name, without remappings */
	fullName = PInvokeSearchPath(pinvoke, baseName, optPrefix, suffix);
	if(fullName)
	{
		return fullName;
	}

#ifdef IL_WIN32_CYGWIN
	/* retry with the cygwin library prefix cyg */
	if(namelen >= 3 && strncmp(name, "lib", 3) != 0)
	{
		ILFree(baseName);
		optPrefix = "cyg";
		baseName = ILDupNString(name + 3, namelen - 3);

		/* Search the path for the name, without remappings */
		fullName = PInvokeSearchPath(pinvoke, baseName, optPrefix, suffix);
		if(fullName)
		{
			return fullName;
		}
	}
	ILFree(baseName);
	optPrefix = "cyg";
	baseName = ILDupNString(name, namelen);

	/* Search the path for the name, without remappings */
	fullName = PInvokeSearchPath(pinvoke, baseName, optPrefix, suffix);
	if(fullName)
	{
		return fullName;
	}
#endif

	/* Try the "pinvoke.map" files to see if we have a remapping */
	dir = mapDirs;
	while(dir != 0)
	{
		contents = dir->contents;
		while(contents != 0)
		{
			if(contents->nameLen == strlen(baseName) &&
			   !ILMemCmp(contents->name, baseName, contents->nameLen) &&
			   !strcmp(contents->systemType, systemType))
			{
				char *newBase = ILDupString(contents->remappedName);
				if(newBase)
				{
					fullName = PInvokeSearchPath(pinvoke, newBase, 0, 0);
					if(fullName)
					{
						ILFree(baseName);
						return fullName;
					}
				}
			}
			contents = contents->next;
		}
		dir = dir->next;
	}

	/* Let "ILDynLibraryOpen" do the hard work of finding it later */
#ifdef IL_WIN32_PLATFORM
	if(!suffix)
#else
	if(!suffix && !optPrefix)
#endif
	{
		return baseName;
	}
	else
	{
		/* Add the prefix and suffix, to make it look like something
		   that "ILDynLibraryOpen" would be interested in */
		fullName = (char *)ILMalloc
			((optPrefix ? strlen(optPrefix) : 0) +
			 strlen(baseName) +
			 (suffix ? strlen(suffix) : 0) + 1);
		if(!fullName)
		{
			return baseName;
		}
		if(optPrefix)
		{
			strcpy(fullName, optPrefix);
			strcat(fullName, baseName);
		}
		else
		{
			strcpy(fullName, baseName);
		}
		if(suffix)
		{
			strcat(fullName, suffix);
		}
		ILFree(baseName);
		return fullName;
	}
}

#else	/* !IL_CONFIG_PINVOKE */

char *ILPInvokeResolveModule(ILPInvoke *pinvoke)
{
	return 0;
}

#endif	/* !IL_CONFIG_PINVOKE */

#ifdef	__cplusplus
};
#endif
