/*
 * ilgac.c - Manage the Global Assembly Cache.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
#include "il_image.h"
#include "il_program.h"
#include "il_utils.h"
#include "il_sysio.h"
#include "il_errno.h"
#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif
#if HAVE_UNISTD_H
	#include <unistd.h>
#endif
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"--install", 'i', 0, 0, 0},
	{"-i", 'i', 0,
		"--install     or -i",
		"Install the specified assembly."},
	{"--uninstall", 'u', 0, 0, 0},
	{"-u", 'u', 0,
		"--uninstall   or -u",
		"Uninstall the specified assembly."},
	{"--list", 'l', 0, 0, 0},
	{"-l", 'l', 0,
		"--list        or -l",
		"List the contents of the cache."},
	{"--force", 'f', 0, 0, 0},
	{"-f", 'f', 0,
		"--force       or -f",
		"Force installation if the assembly is already present."},
	{"--default", 'd', 0, 0, 0},
	{"-d", 'd', 0,
		"--default     or -d",
		"Install as the default assembly version."},
	{"--link", 'L', 0, 0, 0},
	{"-L", 'L', 0,
		"--link        or -L",
		"Create a symbolic link for the assembly instead of copying."},
	{"--subdir", 'S', 1, 0, 0},
	{"-S", 'S', 1,
		"--subdir DIR  or -S DIR",
		"Specify a resource subdirectory (e.g. \"de\")."},
	{"--cache", 'c', 1, 0, 0},
	{"-c", 'c', 1,
		"--cache DIR   or -c DIR",
		"Specify the location of the cache."},
	{"--silent", 's', 0, 0, 0},
	{"--quiet", 's', 0, 0, 0},
	{"-s", 's', 0,
		"--silent      or -s",
		"Do not print status messages."},
	{"--version", 'v', 0,
		"--version     or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},

	/* Backwards-compatibility options */
	{"/f", 'f', 0, 0, 0},
	{"/i", 'i', 0, 0, 0},
	{"/if", 'I', 0, 0, 0},
	{"/l", 'l', 0, 0, 0},
	{"/lf", 'l', 0, 0, 0},
	{"/u", 'u', 0, 0, 0},
	{"/uf", 'U', 0, 0, 0},
	{"/ungen", 'u', 0, 0, 0},
	{"/h", 'h', 0, 0, 0},
	{"/?", 'h', 0, 0, 0},
	{"/help", 'h', 0, 0, 0},
	{"/silent", 's', 0, 0, 0},
	{"/cdl", '?', 0, 0, 0},
	{"/ldl", '?', 0, 0, 0},
	{"/nologo", '?', 0, 0, 0},
	{"/r", 'h', 0, 0, 0},
	{"/ir", 'h', 0, 0, 0},
	{"/ur", 'h', 0, 0, 0},
	{"/il", 'h', 0, 0, 0},
	{"/ul", 'h', 0, 0, 0},

	{0, 0, 0, 0, 0}
};

/*
 * Forward declarations.
 */
static void usage(const char *progname);
static void version(void);
static int getAssemblyVersion(const char *assembly, ILUInt16 *version,
							  int reportErrors, int *imageType);
static int parseVersion(const char *str, ILUInt16 *version);
static char *stripAssemblyName(char *assembly);
static int imageTypeFromName(char *assembly);
static int installAssembly(const char *filename, const char *assembly,
						   const ILUInt16 *version, int imageType);
static int uninstallAssembly(const char *assembly, const ILUInt16 *version,
							 int imageType);
static int listAssemblies(const char *assembly, const ILUInt16 *version);

/*
 * Global option values.
 */
static int force = 0;
static int setAsDefault = 0;
static int silent = 0;
static int createLink = 0;
static char *subdir = 0;
static char *cache = 0;

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int install = 0;
	int uninstall = 0;
	int list = 0;
	char *assembly = 0;
	char *sourceAssembly = 0;
	ILUInt16 versionDetails[4] = {0, 0, 0, 0};
	int type;
	int state, opt;
	char *param;
	int result;

	/* Parse the command-line arguments */
	cache = ILGetStandardLibraryPath("cscc/lib");
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'i':
			{
				install = 1;
			}
			break;

			case 'I':
			{
				install = 1;
				force = 1;
			}
			break;

			case 'u':
			{
				uninstall = 1;
			}
			break;

			case 'U':
			{
				uninstall = 1;
				force = 1;
			}
			break;

			case 'l':
			{
				list = 1;
			}
			break;

			case 'f':
			{
				force = 1;
			}
			break;

			case 'd':
			{
				setAsDefault = 1;
			}
			break;

			case 'L':
			{
				createLink = 1;
			}
			break;

			case 's':
			{
				silent = 1;
			}
			break;

			case 'S':
			{
				subdir = param;
			}
			break;

			case 'c':
			{
				cache = param;
			}
			break;

			case '?': break;

			case 'v':
			{
				version();
				return 0;
			}
			/* Not reached */

			default:
			{
				usage(progname);
				return 1;
			}
			/* Not reached */
		}
	}

	/* Make sure that a command was specified */
	if((!install && !uninstall && !list) || (install + uninstall + list) != 1)
	{
		fprintf(stderr, "%s: Must have one of `--install', `--uninstall', "
				"or `--list'\n", progname);
		return 1;
	}

	/* Perform the requested operation */
	if(install)
	{
		/* Install the specified assembly files in the cache */
		if(argc <= 1)
		{
			fprintf(stderr, "Usage: %s --install assembly\n", progname);
			return 1;
		}
		result = 1;
		while(argc > 1)
		{
			/* Get the name and version information for the assembly */
			sourceAssembly = argv[1];
			if(!getAssemblyVersion(sourceAssembly, versionDetails, 1, &type))
			{
				return 1;
			}
			assembly = stripAssemblyName(sourceAssembly);

			/* Install the specified assembly into the cache */
			result = installAssembly
				(sourceAssembly, assembly, versionDetails, type);
			if(!result)
			{
				break;
			}
			++argv;
			--argc;
		}
	}
	else if(uninstall)
	{
		/* Uninstall the specified assemblies from the cache */
		if(argc <= 1)
		{
			fprintf(stderr, "Usage: %s --uninstall assembly [version]\n",
					progname);
			return 1;
		}
		result = 1;
		while(argc > 1)
		{
			/* Need either an assembly file, or an assembly name and version */
			if(parseVersion(argv[1], versionDetails))
			{
				/* Version number is out of position on the command-line */
				fprintf(stderr, "Usage: %s --uninstall assembly [version]\n",
						progname);
				return 1;
			}
			if(argc > 2 && parseVersion(argv[2], versionDetails))
			{
				/* Assembly name and version supplied */
				assembly = stripAssemblyName(argv[1]);
				type = imageTypeFromName(argv[1]);
				argv += 2;
				argc -= 2;
			}
			else if(!getAssemblyVersion(argv[1], versionDetails, 1, &type))
			{
				/* Invalid assembly supplied */
				return 1;
			}
			else
			{
				/* Assembly filename supplied, with embedded version */
				assembly = stripAssemblyName(argv[1]);
				++argv;
				--argc;
			}
			result = uninstallAssembly(assembly, versionDetails, type);
			if(!result)
			{
				break;
			}
		}
	}
	else if(list)
	{
		if(argc <= 1)
		{
			/* List everything that is present in the cache */
			result = listAssemblies(0, 0);
		}
		else
		{
			/* List only the specified assemblies, versions, or both */
			result = 1;
			while(argc > 1)
			{
				/* Need either an assembly name, a version, or an
				   assembly name and version */
				if(parseVersion(argv[1], versionDetails))
				{
					result = listAssemblies(0, versionDetails);
					if(!result)
					{
						break;
					}
					++argv;
					--argc;
				}
				else if(argc > 2 && parseVersion(argv[2], versionDetails))
				{
					/* Assembly name and version supplied */
					assembly = stripAssemblyName(argv[1]);
					result = listAssemblies(assembly, versionDetails);
					if(!result)
					{
						break;
					}
					argv += 2;
					argc -= 2;
				}
				else
				{
					/* Assembly name supplied, but no version */
					assembly = stripAssemblyName(argv[1]);
					result = listAssemblies(assembly, 0);
					if(!result)
					{
						break;
					}
					++argv;
					--argc;
				}
			}
		}
	}
	else
	{
		result = 1;
	}

	/* Done */
	return (result ? 0 : 1);
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILGAC " VERSION " - IL Global Assembly Cache Utility\n");
	fprintf(stdout, "Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] [assembly [version]]\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{
	printf("ILGAC " VERSION " - IL Global Assembly Cache Utility\n");
	printf("Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILGAC comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Get the version information for an assembly file.  Returns zero
 * if the assembly file does not have the correct format.
 */
static int getAssemblyVersion(const char *assembly, ILUInt16 *version,
							  int reportErrors, int *imageType)
{
	ILContext *context;
	ILImage *image;
	ILAssembly *assem;

	/* Create a context and load the image */
	if((context = ILContextCreate()) == 0)
	{
		fprintf(stderr, "%s: virtual memory exhausted\n", assembly);
		return 0;
	}
	if(ILImageLoadFromFile(assembly, context, &image,
						   IL_LOADFLAG_FORCE_32BIT |
						   IL_LOADFLAG_NO_RESOLVE, reportErrors) != 0)
	{
		ILContextDestroy(context);
		return 0;
	}

	/* The assembly must be a DLL or EXE to be installed in the GAC */
	if(ILImageType(image) != IL_IMAGETYPE_DLL &&
	   ILImageType(image) != IL_IMAGETYPE_EXE)
	{
		fprintf(stderr, "%s: image is not a DLL or EXE\n", assembly);
		ILContextDestroy(context);
		return 0;
	}
	if(imageType)
	{
		*imageType = ILImageType(image);
	}

	/* Extract the version information from the assembly */
	assem = (ILAssembly *)ILImageTokenInfo(image, IL_META_TOKEN_ASSEMBLY | 1);
	if(!assem)
	{
		fprintf(stderr, "%s: Does not contain assembly version information\n",
				assembly);
		ILContextDestroy(context);
		return 0;
	}
	ILMemCpy(version, ILAssemblyGetVersion(assem), sizeof(ILUInt16) * 4);

	/* Clean up the context and exit */
	ILContextDestroy(context);
	return 1;
}

/*
 * Parse a version string.  Returns zero if the format is incorrect.
 */
static int parseVersion(const char *str, ILUInt16 *version)
{
	int posn;
	ILUInt32 value;
	for(posn = 0; posn < 4; ++posn)
	{
		if(*str < '0' || *str > '9')
		{
			return 0;
		}
		value = (ILUInt32)(*str++ - '0');
		while(*str >= '0' && *str <= '9')
		{
			value = value * ((ILUInt32)10) + (ILUInt32)(*str++ - '0');
			if(value >= (ILUInt32)0x10000)
			{
				return 0;
			}
		}
		version[posn] = (ILUInt16)value;
		if(posn == 3)
		{
			break;
		}
		if(*str != ':' && *str != '.')
		{
			return 0;
		}
		++str;
	}
	return (*str == '\0');
}

/*
 * Strip an assembly name down to its base name.
 */
static char *stripAssemblyName(char *assembly)
{
	if(assembly)
	{
		int len = strlen(assembly);
		while(len > 0 && assembly[len - 1] != '/' && assembly[len - 1] != '\\')
		{
			--len;
		}
		assembly += len;
		len = strlen(assembly);
		if(len > 4 && assembly[len - 4] == '.' &&
		   (assembly[len - 3] == 'd' || assembly[len - 3] == 'D') &&
		   (assembly[len - 2] == 'l' || assembly[len - 2] == 'L') &&
		   (assembly[len - 1] == 'l' || assembly[len - 1] == 'L'))
		{
			assembly = ILDupNString(assembly, len - 4);
		}
		else if(len > 4 && assembly[len - 4] == '.' &&
		        (assembly[len - 3] == 'e' || assembly[len - 3] == 'E') &&
		        (assembly[len - 2] == 'x' || assembly[len - 2] == 'X') &&
		        (assembly[len - 1] == 'e' || assembly[len - 1] == 'E'))
		{
			assembly = ILDupNString(assembly, len - 4);
		}
	}
	return assembly;
}

/*
 * Determine the image type (DLL or EXE) from an assembly name.
 */
static int imageTypeFromName(char *assembly)
{
	int len = strlen(assembly);
	if(len > 4 && assembly[len - 4] == '.' &&
	   (assembly[len - 3] == 'e' || assembly[len - 3] == 'E') &&
	   (assembly[len - 2] == 'x' || assembly[len - 2] == 'X') &&
	   (assembly[len - 1] == 'e' || assembly[len - 1] == 'E'))
	{
		return IL_IMAGETYPE_EXE;
	}
	else
	{
		return IL_IMAGETYPE_DLL;
	}
}

/*
 * Determine if we can use symlinks to manage the default versions.
 */
#ifndef IL_WIN32_PLATFORM
	#if defined(HAVE_READLINK) && defined(HAVE_SYMLINK)
		#define	IL_USE_SYMLINKS	1
	#endif
#endif

/*
 * Get the directory separator to use.
 */
#ifdef IL_WIN32_NATIVE
	#define	DIR_SEP		"\\"
#else
	#define	DIR_SEP		"/"
#endif

/*
 * Build a full pathname from a number of components.
 */
static char *BuildPath(const char *path1, const char *path2,
					   const char *path3, const char *path4,
					   const char *extension)
{
	int len;
	char *path;

	/* Determine the length of the full path */
	len = strlen(path1);
	if(path2 != 0 && path2[0] != '\0')
	{
		len += strlen(path2) + 1;
	}
	if(path3 != 0 && path3[0] != '\0')
	{
		len += strlen(path3) + 1;
	}
	if(path4 != 0 && path4[0] != '\0')
	{
		len += strlen(path4) + 1;
	}
	if(extension != 0 && extension[0] != '\0')
	{
		len += strlen(extension) + 1;
	}

	/* Allocate space for the path */
	if((path = (char *)ILMalloc(len + 1)) == 0)
	{
		fputs("virtual memory exhausted\n", stderr);
		exit(1);
	}

	/* Build the full path */
	strcpy(path, path1);
	if(path2 != 0 && path2[0] != '\0')
	{
		strcat(path, DIR_SEP);
		strcat(path, path2);
	}
	if(path3 != 0 && path3[0] != '\0')
	{
		strcat(path, DIR_SEP);
		strcat(path, path3);
	}
	if(path4 != 0 && path4[0] != '\0')
	{
		strcat(path, DIR_SEP);
		strcat(path, path4);
	}
	if(extension != 0 && extension[0] != '\0')
	{
		strcat(path, ".");
		strcat(path, extension);
	}
	return path;
}

/*
 * Print an error that resulted from a sysio routine.
 */
static void PrintError(const char *filename, int error)
{
	errno = ILSysIOConvertFromErrno(error);
	perror(filename);
}

/*
 * Create a directory.  Returns zero if not possible.
 */
static int CreateDirectory(const char *path, const char *suffix1,
						   const char *suffix2)
{
	char *fullpath = BuildPath(path, suffix1, suffix2, 0, 0);
	int error;
	if(!ILFileExists(fullpath, (char **)0))
	{
		if((error = ILCreateDir(fullpath)) != 0)
		{
			PrintError(fullpath, error);
			ILFree(fullpath);
			return 0;
		}
	#ifndef IL_WIN32_PLATFORM
		chmod(fullpath, 0755);
	#endif
	}
	ILFree(fullpath);
	return 1;
}

/*
 * Delete a file if it exists.
 */
static int DeleteFile(const char *filename)
{
#ifdef HAVE_LSTAT
	/* Be careful about symlinks which may not point at anything */
	struct stat st;
	if(lstat(filename, &st) >= 0)
	{
		return ILDeleteFile(filename);
	}
	else
	{
		return IL_ERRNO_Success;
	}
#else
	if(ILFileExists(filename, (char **)0))
	{
		return ILDeleteFile(filename);
	}
	else
	{
		return IL_ERRNO_Success;
	}
#endif
}

/*
 * Install an assembly into the global cache.
 */
static int installAssembly(const char *filename, const char *assembly,
						   const ILUInt16 *version, int imageType)
{
	char versionbuf[64];
	char *path;
	int error;
	const char *extension;

	/* Make sure that the top-level cache directory exists */
	if(!CreateDirectory(cache, 0, 0))
	{
		return 0;
	}

	/* Determine the extension to use */
	if(imageType == IL_IMAGETYPE_EXE)
	{
		extension = "exe";
	}
	else
	{
		extension = "dll";
	}

	/* Get the name of the version sub-directory */
	if(version[0] != 0 || version[1] != 0 || version[2] != 0 || version[3] != 0)
	{
		sprintf(versionbuf, "%d.%d.%d.%d",
				(int)(version[0]), (int)(version[1]),
				(int)(version[2]), (int)(version[3]));
	}
	else
	{
		versionbuf[0] = '\0';
	}
#ifndef IL_USE_SYMLINKS
	if(setAsDefault)
	{
		/* If we cannot create symlinks, then install the default
		   version directly into the main cache directory */
		versionbuf[0] = '\0';
	}
#endif

	/* Create the various sub-directories that we need */
	if(!CreateDirectory(cache, subdir, 0))
	{
		return 0;
	}
	if(!CreateDirectory(cache, versionbuf, 0))
	{
		return 0;
	}
	if(!CreateDirectory(cache, versionbuf, subdir))
	{
		return 0;
	}

	/* Copy the assembly into place */
	path = BuildPath(cache, versionbuf, subdir, assembly, extension);
	if(!force && ILFileExists(path, (char **)0))
	{
		fprintf(stderr, "%s: Assembly already exists and `--force' "
				"was not supplied\n", path);
		ILFree(path);
		return 0;
	}
#ifdef IL_USE_SYMLINKS
	if(createLink)
	{
		char *expanded = ILExpandFilename(filename, (char *)0);
		if(!expanded)
		{
			fputs("virtual memory exhausted\n", stderr);
			exit(1);
		}
		if(!silent)
		{
			printf("linking %s to %s\n", path, expanded);
		}
		error = DeleteFile(path);
		if(error != 0)
		{
			PrintError(path, error);
			ILFree(path);
			ILFree(expanded);
			return 0;
		}
		if(symlink(expanded, path) < 0)
		{
			perror(path);
			ILFree(path);
			ILFree(expanded);
			return 0;
		}
		ILFree(expanded);
	}
	else
#endif
	{
		if(!silent)
		{
			printf("copying %s to %s\n", filename, path);
		}
		error = DeleteFile(path);
		if(error != 0)
		{
			PrintError(path, error);
			ILFree(path);
			return 0;
		}
		error = ILCopyFile(filename, path);
		if(error != 0)
		{
			PrintError(path, error);
			ILFree(path);
			return 0;
		}
#ifndef IL_WIN32_PLATFORM
		chmod(path, 0644);
#endif
	}
	ILFree(path);

	/* Create a symlink for the default version */
#ifdef IL_USE_SYMLINKS
	if(setAsDefault && versionbuf[0] != '\0')
	{
		char *link, *fullLink;
		if(subdir)
		{
			link = BuildPath("..", versionbuf, subdir, assembly, extension);
		}
		else
		{
			link = BuildPath(versionbuf, 0, 0, assembly, extension);
		}
		fullLink = BuildPath(cache, versionbuf, subdir, assembly, extension);
		path = BuildPath(cache, subdir, 0, assembly, extension);
		if(!force && ILFileExists(path, (char **)0))
		{
			fprintf(stderr, "%s: Link already exists and `--force' "
					"was not supplied\n", path);
			ILFree(link);
			ILFree(fullLink);
			ILFree(path);
			return 0;
		}
		if(!silent)
		{
			printf("linking %s to %s\n", path, fullLink);
		}
		error = DeleteFile(path);
		if(error != 0)
		{
			PrintError(path, error);
			ILFree(link);
			ILFree(fullLink);
			ILFree(path);
			return 0;
		}
		if(symlink(link, path) < 0)
		{
			perror(path);
			ILFree(link);
			ILFree(fullLink);
			ILFree(path);
			return 0;
		}
		ILFree(link);
		ILFree(fullLink);
		ILFree(path);
	}
#endif

	/* The assembly has been installed successfully */
	return 1;
}

/*
 * Uninstall an assembly from the global cache.
 */
static int uninstallAssembly(const char *assembly, const ILUInt16 *version,
							 int imageType)
{
	char versionbuf[64];
	char *path;
	int error;
	const char *extension;

	/* Get the name of the version sub-directory */
	if(version[0] != 0 || version[1] != 0 || version[2] != 0 || version[3] != 0)
	{
		sprintf(versionbuf, "%d.%d.%d.%d",
				(int)(version[0]), (int)(version[1]),
				(int)(version[2]), (int)(version[3]));
	}
	else
	{
		versionbuf[0] = '\0';
	}

	/* Determine the extension to use */
	if(imageType == IL_IMAGETYPE_EXE)
	{
		extension = "exe";
	}
	else
	{
		extension = "dll";
	}

	/* Remove the main assembly instance */
	path = BuildPath(cache, versionbuf, subdir, assembly, extension);
#ifndef IL_USE_SYMLINKS
	if(!ILFileExists(path, (char **)0) && versionbuf[0] != '\0')
	{
		/* The assembly may have been installed as the default, so we
		   need to look at the default location and compare versions */
		char *path2 = BuildPath(cache, subdir, 0, assembly, extension);
		if(ILFileExists(path2, (char **)0))
		{
			ILUInt16 assemblyVersion[4];
			if(getAssemblyVersion(path2, assemblyVersion, 1, 0))
			{
				if(version[0] == assemblyVersion[0] &&
				   version[1] == assemblyVersion[1] &&
				   version[2] == assemblyVersion[2] &&
				   version[3] == assemblyVersion[3])
				{
					/* Use this path instead of the original */
					ILFree(path);
					path = path2;
					path2 = 0;
					versionbuf[0] = '\0';
				}
			}
		}
		if(path2 != 0)
		{
			ILFree(path2);
		}
	}
#endif
	if(!silent)
	{
		printf("removing %s\n", path);
	}
	error = DeleteFile(path);
	if(error != 0)
	{
		PrintError(path, error);
		ILFree(path);
		return 0;
	}

	/* Remove the default symlink also, if it points to what we just removed */
#ifdef IL_USE_SYMLINKS
	if(versionbuf[0] != '\0')
	{
		char *link;
		char contents[1024];
		int len;
		if(subdir)
		{
			link = BuildPath("..", versionbuf, subdir, assembly, extension);
		}
		else
		{
			link = BuildPath(versionbuf, 0, 0, assembly, extension);
		}
		path = BuildPath(cache, subdir, 0, assembly, extension);
		len = readlink(path, contents, sizeof(contents) - 1);
		if(len >= 0)
		{
			contents[len] = '\0';
			if(!strcmp(contents, link))
			{
				if(!silent)
				{
					printf("removing %s\n", path);
				}
				error = ILDeleteFile(path);
				if(error != 0)
				{
					PrintError(path, error);
					ILFree(link);
					ILFree(path);
					return 0;
				}
			}
		}
		ILFree(link);
		ILFree(path);
	}
#endif

	/* Try to remove unnecessary empty subdirectories */
#ifdef HAVE_RMDIR
	if(subdir)
	{
		if(versionbuf[0] != '\0')
		{
			path = BuildPath(cache, versionbuf, subdir, 0, 0);
			rmdir(path);
			ILFree(path);
		}
		path = BuildPath(cache, subdir, 0, 0, 0);
		rmdir(path);
		ILFree(path);
	}
	else if(versionbuf[0] != '\0')
	{
		path = BuildPath(cache, versionbuf, 0, 0, 0);
		rmdir(path);
		ILFree(path);
	}
#endif

	/* The assembly has been uninstalled successfully */
	return 1;
}

/*
 * The directory name stack for "listAssemblies".
 */
static char **directoryStack = 0;
static int *directoryDefaultLevel = 0;
static int directoryStackSize = 0;
static int directoryStackMax = 0;

/*
 * Add an entry to the directory stack.
 */
static int addToDirectoryStack(const char *filename, int isDefaultLevel)
{
	char **newDirectoryStack;
	int *newDirectoryLevel;
	if(directoryStackSize >= directoryStackMax)
	{
		newDirectoryStack = (char **)ILRealloc
			(directoryStack, sizeof(char *) * (directoryStackSize + 8));
		newDirectoryLevel = (int *)ILRealloc
			(directoryDefaultLevel, sizeof(int) * (directoryStackSize + 8));
		if(!newDirectoryStack || !newDirectoryLevel)
		{
			fputs("virtual memory exhausted\n", stderr);
			return 0;
		}
		directoryStack = newDirectoryStack;
		directoryDefaultLevel = newDirectoryLevel;
		directoryStackMax += 8;
	}
	directoryStack[directoryStackSize] = ILDupString(filename);
	directoryDefaultLevel[directoryStackSize] = isDefaultLevel;
	if(!(directoryStack[directoryStackSize]))
	{
		fputs("virtual memory exhausted\n", stderr);
		return 0;
	}
	++directoryStackSize;
	return 1;
}

/*
 * Recursively scan a cache directory, looking for list matches.
 */
static int scanDirectory(const char *directory, const char *assembly,
						 const ILUInt16 *version, int isDefaultLevel)
{
	ILDir *dir;
	ILDirEnt *entry;
	const char *name;
	char *combined;
	ILUInt16 assemblyVersion[4];
	int len;

	/* Open the directory */
	if((dir = ILOpenDir((char *)directory)) == 0)
	{
		if(isDefaultLevel)
		{
			perror(directory);
			return 0;
		}
		else
		{
			return 1;
		}
	}

	/* Process the members of the directory */
	while((entry = ILReadDir(dir)) != 0)
	{
		name = ILDirEntName(entry);
		if(!strcmp(name, ".") || !strcmp(name, ".."))
		{
			continue;
		}
		combined = (char *)ILMalloc(strlen(directory) + strlen(name) + 2);
		if(!combined)
		{
			fputs("virtual memory exhausted\n", stderr);
			return 0;
		}
		strcpy(combined, directory);
	#ifdef IL_WIN32_NATIVE
		strcat(combined, "\\");
	#else
		strcat(combined, "/");
	#endif
		strcat(combined, name);
		if(ILDirEntType(entry) == ILFileType_DIR)
		{
			/* Queue a sub-directory to be scanned later */
			if(parseVersion(name, assemblyVersion))
			{
				/* Probably a version-specific directory */
				if(!addToDirectoryStack(combined, 0))
				{
					ILCloseDir(dir);
					ILFree(combined);
					return 0;
				}
			}
			else
			{
				/* Probably a resource name directory */
				if(!addToDirectoryStack(combined, isDefaultLevel))
				{
					ILCloseDir(dir);
					ILFree(combined);
					return 0;
				}
			}
		}
		else
		{
			/* We are only interested in files that end in ".dll" or ".exe" */
			len = strlen(name);
			if(!(len > 4 && name[len - 4] == '.' &&
			     (name[len - 3] == 'd' || name[len - 3] == 'D') &&
			     (name[len - 2] == 'l' || name[len - 2] == 'L') &&
			     (name[len - 1] == 'l' || name[len - 1] == 'L')))
			{
				if(!(len > 4 && name[len - 4] == '.' &&
				     (name[len - 3] == 'e' || name[len - 3] == 'E') &&
				     (name[len - 2] == 'x' || name[len - 2] == 'X') &&
				     (name[len - 1] == 'e' || name[len - 1] == 'E')))
				{
					ILFree(combined);
					continue;
				}
			}

			/* Retrieve the actual assembly version from the image */
			if(!getAssemblyVersion(combined, assemblyVersion, 0, 0))
			{
				ILFree(combined);
				continue;
			}

			/* Check for matches against the list criteria */
			if(assembly)
			{
				if(strlen(assembly) != (len - 4) ||
				   strncmp(assembly, name, len - 4) != 0)
				{
					ILFree(combined);
					continue;
				}
			}
			if(version)
			{
				if(ILMemCmp(version, assemblyVersion,
						    sizeof(ILUInt16) * 4) != 0)
				{
					ILFree(combined);
					continue;
				}
			}

			/* Report the default mapping, or the actual filename */
			if(isDefaultLevel &&
			   (assemblyVersion[0] != 0 || assemblyVersion[1] != 0 ||
			    assemblyVersion[2] != 0 || assemblyVersion[3] != 0))
			{
				if(setAsDefault || ILDirEntType(entry) != ILFileType_LNK)
				{
					printf("%s (version=%d.%d.%d.%d)\n", combined,
						   (int)(assemblyVersion[0]),
						   (int)(assemblyVersion[1]),
						   (int)(assemblyVersion[2]),
						   (int)(assemblyVersion[3]));
				}
			}
			else
			{
				puts(combined);
			}
		}
		ILFree(combined);
	}

	/* Close the directory and exit */
	ILCloseDir(dir);
	return 1;
}

/*
 * List the assemblies in the global cache that match certain name criteria.
 */
static int listAssemblies(const char *assembly, const ILUInt16 *version)
{
	char *directory;
	if(!addToDirectoryStack(cache, 1))
	{
		return 0;
	}
	while(directoryStackSize > 0)
	{
		directory = directoryStack[--directoryStackSize];
		if(!scanDirectory(directory, assembly, version,
						  directoryDefaultLevel[directoryStackSize]))
		{
			return 0;
		}
	}
	return 1;
}

#ifdef	__cplusplus
};
#endif
