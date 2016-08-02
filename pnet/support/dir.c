/*
 * dir.c - Directory Related Functions
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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
#include "il_sysio.h"
#include "il_errno.h"

#ifdef HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#ifdef HAVE_FCNTL_H
	#include <fcntl.h>
#endif
#ifdef HAVE_DIRENT_H
	#include <dirent.h>
#endif
#ifdef IL_WIN32_NATIVE
	#include <windows.h>
	#include <io.h>
#endif
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_WIN32_NATIVE
	#define	USE_WIN32_FIND 1
#endif

#if defined(__palmos__)
#define	IL_NO_DIR_ROUTINES
#endif

#ifndef IL_NO_DIR_ROUTINES

/*
 * Note: this may be called to delete either a directory or a file.
 */
ILInt32 ILDeleteDir(const char *path)
{
	if(!path)
	{
		return IL_ERRNO_ENOENT;
	}
#ifdef HAVE_RMDIR
	if(rmdir(path) == 0)
		return IL_ERRNO_Success;
#endif
#ifdef HAVE_UNLINK
	if(unlink(path) == 0)
		return IL_ERRNO_Success;
	else
		return ILSysIOConvertErrno(errno);
#elif defined(HAVE_REMOVE)
	if(remove(path) == 0)
		return IL_ERRNO_Success;
	else
		return ILSysIOConvertErrno(errno);
#else
    return IL_ERRNO_ENOSYS;
#endif

}

ILInt32 ILRenameDir(const char *old_name, const char *new_name)
{
	if(!old_name || !new_name)
	{
	    return IL_ERRNO_ENOENT;
	}

#ifdef HAVE_RENAME
	if(rename(old_name, new_name) == 0)
		return IL_ERRNO_Success;
	else
        return ILSysIOConvertErrno(errno);
#else
	return IL_ERRNO_ENOSYS;
#endif	

}

ILInt32 ILChangeDir(const char *path)
{
	if(!path)
	{
		return IL_ERRNO_ENOENT;
	}
#ifdef HAVE_CHDIR
	if(chdir(path) == 0)
		return IL_ERRNO_Success;
	else
		return ILSysIOConvertErrno(errno);
#else
	return IL_ERRNO_ENOSYS;
#endif

}

ILInt32 ILCreateDir(const char *path)
{
	if(!path)
	{
		return IL_ERRNO_ENOENT;
	}
#ifdef HAVE_MKDIR
	#ifdef IL_WIN32_NATIVE
		if(mkdir(path) == 0)
			return IL_ERRNO_Success;
		else
			return ILSysIOConvertErrno(errno);
	#else
		if(mkdir(path, 0777) == 0)
			return IL_ERRNO_Success;
		else
			return ILSysIOConvertErrno(errno);
	#endif
#else
	return IL_ERRNO_ENOSYS;
#endif
}

#ifndef USE_WIN32_FIND

int ILGetFileType(const char *path)
{
	struct stat st;
#ifdef HAVE_LSTAT
	if(lstat(path, &st) >= 0)
#else
	if(stat(path, &st) >= 0)
#endif
	{
	#ifdef S_ISFIFO
		if(S_ISFIFO(st.st_mode))
		{
			return ILFileType_FIFO;
		}
	#endif
	#ifdef S_ISCHR
		if(S_ISCHR(st.st_mode))
		{
			return ILFileType_CHR;
		}
	#endif
	#ifdef S_ISDIR
		if(S_ISDIR(st.st_mode))
		{
			return ILFileType_DIR;
		}
	#endif
	#ifdef S_ISBLK
		if(S_ISBLK(st.st_mode))
		{
			return ILFileType_BLK;
		}
	#endif
	#ifdef S_ISREG
		if(S_ISREG(st.st_mode))
		{
			return ILFileType_REG;
		}
	#endif
	#ifdef S_ISLNK
		if(S_ISLNK(st.st_mode))
		{
			return ILFileType_LNK;
		}
	#endif
	#ifdef S_ISSOCK
		if(S_ISSOCK(st.st_mode))
		{
			return ILFileType_SOCK;
		}
	#endif
	}
	return ILFileType_Unknown;
}

#ifdef HAVE_DIRENT_H

/*
 * Define the ILDir type.
 */
struct _tagILDir
{
	char *pathname;
	DIR *dir;
};

/*
 * Determine if the "struct dirent" definition is broken.
 * Broken means that "d_name" is declared with a size of 1.
 */
#if defined(__sun__) || defined(__BEOS__)
#define	BROKEN_DIRENT	1
#endif

/*
 * Define the ILDirEnt type.
 */
struct _tagILDirEnt
{
	int type;
	struct dirent *dptr;
	struct dirent de;
#ifdef BROKEN_DIRENT
	char name[256];
#endif
};

/*
 * Get the type of a directory entry.
 */
static void GetDirEntryType(ILDir *dir, ILDirEnt *entry)
{
	char *fullName;
	entry->type = ILFileType_Unknown;
	fullName = (char *)ILMalloc(strlen(dir->pathname) +
								strlen(entry->dptr->d_name) + 2);
	if(fullName)
	{
		strcpy(fullName, dir->pathname);
		strcat(fullName, "/");
		strcat(fullName, entry->dptr->d_name);
		entry->type = ILGetFileType(fullName);
		ILFree(fullName);
	}
}

#endif /* HAVE_DIRENT_H */

/*
 * Implementing this way because opendir seems to be somewhat non-standardised.
 * so basically I think this way will be a lot more portable.
 */
ILDir *ILOpenDir(const char *path)
{
#ifdef HAVE_DIRENT_H
	ILDir *dir = (ILDir *)ILMalloc(sizeof(ILDir));
	if(dir)
	{
		dir->pathname = ILDupString(path);
		if(!(dir->pathname))
		{
			ILFree(dir);
			return (ILDir *)0;
		}
		dir->dir = opendir(dir->pathname);
		if(!(dir->dir))
		{
			ILFree(dir->pathname);
			ILFree(dir);
			return (ILDir *)0;
		}
	}
	return dir;
#else
	errno = ENOENT;
	return (ILDir *)0;
#endif
}


/*  This function will return NULL on error  */
ILDirEnt *ILReadDir(ILDir *directory)
{
#ifdef HAVE_DIRENT_H
    
#if defined(HAVE_READDIR_R) && !defined(BROKEN_DIRENT)
	ILDirEnt *result = NULL;

	/* Threadsafe version of readdir() */
	/*  Fetch a directory entry  */
	if((result = (ILDirEnt *)ILMalloc(sizeof(ILDirEnt))) == NULL)
    {
        return NULL;
    }
    
	if(readdir_r(directory->dir, &(result->de), &(result->dptr)) != 0)
	{
		ILFree(result);
		return NULL;
	}
	if(!(result->dptr)) /* yet another terminating condition */
	{
		ILFree(result);
		return NULL;
	}

	GetDirEntryType(directory, result);
	return result;
#else
#ifdef HAVE_READDIR
	/*  Not Threadsafe, so maybe if systems need it, we should rewrite it.  */
	struct dirent *result;
	ILDirEnt *allocatedResult = NULL;
	if((result = readdir(directory->dir)) == NULL)
	{
		return NULL;
	}

	/*  After we know we HAVE a result, we copy it's contents into our 
	 * 	own struct  */
	allocatedResult = (ILDirEnt *)ILMalloc(sizeof(ILDirEnt));
	if(allocatedResult != NULL)
	{
		allocatedResult->dptr = &(allocatedResult->de);
		ILMemCpy(&(allocatedResult->de), result, sizeof(struct dirent));
#if defined(BROKEN_DIRENT)
		strcpy(allocatedResult->de.d_name, result->d_name);
#endif
		GetDirEntryType(directory, allocatedResult);
	}
	return allocatedResult;
#else
	return NULL;
#endif
#endif
#else
	return NULL;
#endif
}

int ILCloseDir(ILDir *directory)
{
#ifdef HAVE_DIRENT_H
	int result = (closedir(directory->dir) == 0);
	ILFree(directory->pathname);
	ILFree(directory);
	return result;
#else
	return 0;
#endif
}

const char *ILDirEntName(ILDirEnt *entry)
{
#ifdef HAVE_DIRENT_H
	return entry->dptr->d_name;
#else
	return (const char *)0;
#endif
}

int ILDirEntType(ILDirEnt *entry)
{
#ifdef HAVE_DIRENT_H
	return entry->type;
#else
	return ILFileType_Unknown;
#endif
}

#else /* USE_WIN32_FIND */

int ILGetFileType(const char *path)
{
	struct _finddata_t fileinfo;
	long handle;
	handle = _findfirst(path, &fileinfo);
	if(handle >= 0)
	{
		_findclose(handle);
		if((fileinfo.attrib & _A_SUBDIR) != 0)
		{
			return ILFileType_DIR;
		}
		else
		{
			return ILFileType_REG;
		}
	}
	else
	{
		return ILFileType_Unknown;
	}
}

/*
 * Define the ILDir type.
 */
struct _tagILDir
{
	long handle;
	struct _finddata_t fileinfo;
	int havefirst;
};

/*
 * Define the ILDirEnt type.
 */
struct _tagILDirEnt
{
	unsigned attrib;
	char name[1];
};

ILDir *ILOpenDir(const char *path)
{
	char *spec;
	int len;
	ILDir *dir = (ILDir *)ILMalloc(sizeof(ILDir));
	if(!dir)
	{
		return 0;
	}
	spec = (char *)ILMalloc(strlen(path) + 5);
	if(!spec)
	{
		ILFree(dir);
		return 0;
	}
	strcpy(spec, path);
	len = strlen(spec);
	if(len > 0 && spec[len - 1] != '/' && spec[len - 1] != '\\')
	{
		spec[len++] = '\\';
	}
	strcpy(spec + len, "*.*");
	dir->handle = _findfirst(spec, &(dir->fileinfo));
	dir->havefirst = 1;
	if(dir->handle < 0)
	{
		int error = errno;
		ILFree(dir);
		ILFree(spec);
		errno = error;
		return 0;
	}
	ILFree(spec);
	return dir;
}

ILDirEnt *ILReadDir(ILDir *dir)
{
	ILDirEnt *entry;
	if(!(dir->havefirst))
	{
		if(_findnext(dir->handle, &(dir->fileinfo)) < 0)
		{
			return 0;
		}
	}
	else
	{
		dir->havefirst = 0;
	}
	entry = (ILDirEnt *)ILMalloc(sizeof(ILDirEnt) + strlen(dir->fileinfo.name));
	if(!entry)
	{
		return 0;
	}
	entry->attrib = dir->fileinfo.attrib;
	strcpy(entry->name, dir->fileinfo.name);
	return entry;
}

int ILCloseDir(ILDir *dir)
{
	int result;

	result = _findclose(dir->handle) == 0;
	ILFree(dir);
	return result;
}

const char *ILDirEntName(ILDirEnt *entry)
{
	return entry->name;
}


int ILDirEntType(ILDirEnt *entry)
{
	if((entry->attrib & _A_SUBDIR) != 0)
	{
		return ILFileType_DIR;
	}
	else
	{
		return ILFileType_REG;
	}
}

#endif /* USE_WIN32_FIND */

void ILGetPathInfo(ILPathInfo *info)
{
#if defined(IL_WIN32_NATIVE)
	info->dirSep = '\\';
	info->altDirSep = '/';
	info->volumeSep = ':';
	info->pathSep = ';';
	info->invalidPathChars = "\"<>|\r\n";
#elif defined(IL_WIN32_CYGWIN)
	info->dirSep = '/';
	info->altDirSep = '\\';
	info->volumeSep = 0;
	info->pathSep = ':';
	info->invalidPathChars = "\"<>|\r\n";
#else
	info->dirSep = '/';
	info->altDirSep = 0;
	info->volumeSep = 0;
	info->pathSep = ':';
	info->invalidPathChars = "\r\n";
#endif
}

#else /* IL_NO_DIR_ROUTINES */

ILInt32 ILDeleteDir(const char *path)
{
	return IL_ERRNO_ENOENT;
}

ILInt32 ILRenameDir(const char *old_name, const char *new_name)
{
	return IL_ERRNO_ENOENT;
}

ILInt32 ILChangeDir(const char *path)
{
	return IL_ERRNO_ENOENT;
}

ILInt32 ILCreateDir(const char *path)
{
	return IL_ERRNO_ENOENT;
}

int ILGetFileType(const char *path)
{
	return ILFileType_REG;
}

ILDir *ILOpenDir(const char *path)
{
	return 0;
}

int ILCloseDir(ILDir *dir)
{
	return 0;
}

const char *ILDirEntName(ILDirEnt *entry)
{
	return 0;
}

int ILDirEntType(ILDirEnt *entry)
{
	return ILFileType_Unknown;
}

#endif /* IL_NO_DIR_ROUTINES */

#ifdef	__cplusplus
};
#endif
