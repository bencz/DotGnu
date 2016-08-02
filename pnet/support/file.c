/*
 * file.c - File-related functions.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * Copyright (C) 2002  Richard Baumann
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
#include <errno.h>
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
#ifdef IL_WIN32_PLATFORM
	#include <windows.h>
	#include <io.h>
#endif
#ifdef HAVE_SYS_UTIME_H
	#include <sys/utime.h>
#else
#ifdef HAVE_UTIME_H
	#include <utime.h>
#endif
#endif
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(__palmos__)
#define	IL_NO_FILE_ROUTINES
#endif

#ifndef IL_NO_FILE_ROUTINES

int ILDeleteFile(const char *filename)
{
#ifdef HAVE_UNLINK
	if(unlink(filename) == 0)
		return IL_ERRNO_Success;
	else
		return ILSysIOConvertErrno(errno);
#elif defined(HAVE_REMOVE)
	if(remove(filename) == 0)
		return IL_ERRNO_Success;
	else
		return ILSysIOConvertErrno(errno);
#else
	return IL_ERRNO_ENOSYS;
#endif
}

int ILFileExists(const char *filename, char **newExePath)
{
	if(newExePath)
	{
		*newExePath = 0;
	}

#ifdef IL_WIN32_PLATFORM
	/* If we are on Windows, then check for ".exe" first */
	if(newExePath)
	{
		char *newPath = (char *)ILMalloc(strlen(filename) + 5);
		if(newPath)
		{
			strcpy(newPath, filename);
			strcat(newPath, ".exe");
			if(access(newPath, 0) >= 0)
			{
				*newExePath = newPath;
				return 1;
			}
			ILFree(newPath);
		}
	}
#endif

#ifdef HAVE_ACCESS
	if(access(filename, 0) >= 0)
	{
		return 1;
	}
#else
#ifdef HAVE_STAT
	{
		struct stat st;
		if(stat(filename, &st) >= 0)
		{
			return 1;
		}
	}
#else
	{
		FILE *file = fopen(filename, "r");
		if(file)
		{
			fclose(file);
			return 1;
		}
	}
#endif
#endif

	return 0;
}

/* TODO: I haven't implemented the Windows/Cygwin parts yet - coming soon */

int ILSysIOGetErrno(void)
{
	return ILSysIOConvertErrno(errno);
}

void ILSysIOSetErrno(int code)
{
	code = ILSysIOConvertFromErrno(code);
	if(code != -1)
	{
		errno = code;
	}
	else
	{
		errno = EPERM;
	}
}

const char *ILSysIOGetErrnoMessage(int code)
{
#ifdef HAVE_STRERROR
	code = ILSysIOConvertFromErrno(code);
	if(code != -1)
	{
		return strerror(code);
	}
	else
	{
		return 0;
	}
#else
	return 0;
#endif
}

int ILSysIOValidatePathname(const char *path)
{
	/* TODO */
	return 1;
}

ILSysIOHandle ILSysIOOpenFile(const char *path, ILUInt32 mode,
						      ILUInt32 access, ILUInt32 share)
{
	int result;
	int newAccess;

	switch(access)
	{
		case ILFileAccess_Read:			newAccess = O_RDONLY; break;
		case ILFileAccess_Write:		newAccess = O_WRONLY; break;
		case ILFileAccess_ReadWrite:	newAccess = O_RDWR; break;

		default:
		{
			errno = EACCES;
			return (ILSysIOHandle)(ILNativeInt)(-1);
		}
		/* Not reached */
	}

#ifdef IL_WIN32_PLATFORM
	/* Unbelievably, windows defaults to text mode!!! */
	newAccess |= _O_BINARY;
#endif

	switch(mode)
	{
		case ILFileMode_CreateNew:
		{
			/* Create a file only if it doesn't already exist */
			result = open(path, newAccess | O_CREAT | O_EXCL, 0666);
		}
		break;

		case ILFileMode_Create:
		{
			/* Open in create/truncate mode */
			result = open(path, newAccess | O_CREAT | O_TRUNC, 0666);
		}
		break;

		case ILFileMode_Open:
		{
			/* Open the file in the regular mode */
			result = open(path, newAccess, 0666);
		}
		break;

		case ILFileMode_OpenOrCreate:
		{
			/* Open an existing file or create a new one */
			result = open(path, newAccess | O_CREAT, 0666);
		}
		break;

		case ILFileMode_Truncate:
		{
			/* Truncate an existing file */
			result = open(path, newAccess | O_TRUNC, 0666);
		}
		break;

		case ILFileMode_Append:
		{
			/* Open in append mode */
			result = open(path, newAccess | O_CREAT | O_APPEND, 0666);
		}
		break;

		default:
		{
			/* We have no idea what this mode is */
			result = -1;
			errno = EACCES;
		}
		break;
	}

	/* TODO: sharing modes */

	return (ILSysIOHandle)(ILNativeInt)result;
}

int ILSysIOCheckHandleAccess(ILSysIOHandle handle, ILUInt32 access)
{
#if defined(HAVE_FCNTL) && defined(F_GETFL)
	int flags = fcntl((int)(ILNativeInt)handle, F_GETFL, 0);
  	if(flags != -1)
    {
		switch(access)
		{
			case ILFileAccess_Read:		return ((flags & O_RDONLY) != 0);
			case ILFileAccess_Write:	return ((flags & O_WRONLY) != 0);
			case ILFileAccess_ReadWrite:
					return ((flags & O_RDWR) == O_RDWR);
		}
	}
	return 0;
#else
	return 0;
#endif
}

int ILSysIOClose(ILSysIOHandle handle)
{
	int result;
	while((result = close((int)(ILNativeInt)handle)) < 0)
	{
		/* Retry if the system call was interrupted */
		if(errno != EINTR)
		{
			break;
		}
	}
	return (result == 0);
}

ILInt32 ILSysIORead(ILSysIOHandle handle, void *buf, ILInt32 size)
{
	int result;
	while((result = read((int)(ILNativeInt)handle,
						 buf, (unsigned int)size)) < 0)
	{
		/* Retry if the system call was interrupted */
		if(errno != EINTR)
		{
			break;
		}
	}
	return (ILInt32)result;
}

ILInt32 ILSysIOWrite(ILSysIOHandle handle, const void *buf, ILInt32 size)
{
	int written = 0;
	int result = 0;
	while(size > 0)
	{
		/* Write as much as we can, and retry if system call was interrupted */
		result = write((int)(ILNativeInt)handle, buf, (unsigned int)size);
		if(result >= 0)
		{
			written += result;
			size -= result;
			buf += result;
		}
		else if(errno != EINTR)
		{
			break;
		}
	}
	if(written > 0)
	{
		return written;
	}
	else
	{
		return ((result < 0) ? -1 : 0);
	}
}

ILInt64 ILSysIOSeek(ILSysIOHandle handle, ILInt64 offset, int whence)
{
	ILInt64 result;
	while((result = (ILInt64)(lseek((int)(ILNativeInt)handle,
									(off_t)offset, whence)))
				== (ILInt64)(-1))
	{
		/* Retry if the system call was interrupted */
		if(errno != EINTR)
		{
			break;
		}
	}
	return result;
}

int ILSysIOFlushRead(ILSysIOHandle handle)
{
	/* TODO: mostly of use for tty devices, not files or sockets */
	return 1;
}

int ILSysIOFlushWrite(ILSysIOHandle handle)
{
	/* TODO: mostly of use for tty devices, not files or sockets */
	return 1;
}

ILInt32 ILCopyFile(const char *src, const char *dest)
{
	FILE *infile;
	FILE *outfile;
	char buffer[BUFSIZ];
	int len;

	/* Bail out if either of the arguments is invalid */
	if(!src || !dest)
	{
	    return IL_ERRNO_ENOENT;
	}

	/* Open the input file */
	if((infile = fopen(src, "rb")) == NULL)
	{
		if((infile = fopen(src, "r")) == NULL)
		{
			return ILSysIOConvertErrno(errno);
		}
	}

	/* Open the output file */
	if((outfile = fopen(dest, "wb")) == NULL)
	{
		if((outfile = fopen(dest, "w")) == NULL)
		{
			int error = ILSysIOConvertErrno(errno);
			fclose(infile);
			return error;
		}
	}

	/* Copy the file contents */
	while((len = (int)fread(buffer, 1, sizeof(buffer), infile)) > 0)
	{
		fwrite(buffer, 1, len, outfile);
		if(len < sizeof(buffer))
		{
			break;
		}
	}

	/* Close the files and exit */
	fclose(infile);
	fclose(outfile);
	return 0;
}

int ILSysIOTruncate(ILSysIOHandle handle, ILInt64 posn)
{
#ifdef HAVE_FTRUNCATE
	int result;
	while((result = ftruncate((int)(ILNativeInt)handle, (off_t)posn)) < 0)
	{
		/* Retry if the system call was interrupted */
		if(errno != EINTR)
		{
			break;
		}
	}
	return (result == 0);
#else
	errno = EINVAL;
	return 0;
#endif
}

int ILSysIOLock(ILSysIOHandle handle, ILInt64 position, ILInt64 length)
{
#if defined(IL_WIN32_PLATFORM)
	/* Bypass the system library and call LockFile directly under Win32 */
#ifdef IL_WIN32_CYGWIN
	HANDLE osHandle = (HANDLE)get_osfhandle((int)(ILNativeInt)handle);
#else
	HANDLE osHandle = (HANDLE)_get_osfhandle((int)(ILNativeInt)handle);
#endif
	if(osHandle == (HANDLE)INVALID_HANDLE_VALUE)
	{
		ILSysIOSetErrno(IL_ERRNO_EBADF);
		return 0;
	}
	if(LockFile(osHandle,
				(DWORD)(position & IL_MAX_UINT32),
			 	(DWORD)((position >> 32) & IL_MAX_UINT32),
			 	(DWORD)(length & IL_MAX_UINT32),
			 	(DWORD)((length >> 32) & IL_MAX_UINT32)))
	{
		return 1;
	}
	else
	{
		ILSysIOSetErrno(IL_ERRNO_ENOLCK);
		return 0;
	}
#elif defined(HAVE_FCNTL) && defined(HAVE_F_SETLKW)
	struct flock cntl_data;
	/* set fields individually...who knows what extras are there? */
	cntl_data.l_type = F_WRLCK;
	cntl_data.l_whence = SEEK_SET;
	/* actually, off_t changes in LFS on 32bit, so be careful */
	cntl_data.l_start = (off_t)(ILNativeInt) position;
	cntl_data.l_len = (off_t)(ILNativeInt) length;
	/* -1 is error, anything else is OK */
	if (fcntl ((int)(ILNativeInt) handle, F_SETLKW, &cntl_data) != -1)
		return 1;
	else
		return 0;
#else /* !defined(HAVE_FCNTL) || !defined(HAVE_F_SETLKW) */
	/* Locking is not supported w/o fcntl - TODO */
	return 1;
#endif /* !defined(HAVE_FCNTL) || !defined(HAVE_F_SETLKW) */
}

int ILSysIOUnlock(ILSysIOHandle handle, ILInt64 position, ILInt64 length)
{
#if defined(IL_WIN32_PLATFORM)
	/* Bypass the system library and call UnlockFile directly under Win32 */
#ifdef IL_WIN32_CYGWIN
	HANDLE osHandle = (HANDLE)get_osfhandle((int)(ILNativeInt)handle);
#else
	HANDLE osHandle = (HANDLE)_get_osfhandle((int)(ILNativeInt)handle);
#endif
	if(osHandle == (HANDLE)INVALID_HANDLE_VALUE)
	{
		ILSysIOSetErrno(IL_ERRNO_EBADF);
		return 0;
	}
	if(UnlockFile(osHandle,
				  (DWORD)(position & IL_MAX_UINT32),
			 	  (DWORD)((position >> 32) & IL_MAX_UINT32),
			 	  (DWORD)(length & IL_MAX_UINT32),
			 	  (DWORD)((length >> 32) & IL_MAX_UINT32)))
	{
		return 1;
	}
	else
	{
		ILSysIOSetErrno(IL_ERRNO_ENOLCK);
		return 0;
	}
#elif defined(HAVE_FCNTL) && defined(HAVE_F_SETLKW)
	struct flock cntl_data;
	/* set fields individually...who knows what extras are there? */
	cntl_data.l_type = F_UNLCK;
	cntl_data.l_whence = SEEK_SET;
	/* actually, off_t changes in LFS on 32bit, so be careful */
	cntl_data.l_start = (off_t)(ILNativeInt) position;
	cntl_data.l_len = (off_t)(ILNativeInt) length;
	/* -1 is error, anything else is OK */
	if (fcntl ((int)(ILNativeInt) handle, F_SETLKW, &cntl_data) != -1)
		return 1;
	else
		return 0;
#else /* !defined(HAVE_FCNTL) || !defined(HAVE_F_SETLKW) */
	/* Unlocking is not supported w/o fcntl - TODO */
	return 1;
#endif /* !defined(HAVE_FCNTL) || !defined(HAVE_F_SETLKW) */
}

int ILSysIOHasAsync(void)
{
	/* TODO: asynchronous I/O is not yet supported */
	return 0;
}

int ILSysIOPathGetLastAccess(const char *path, ILInt64 *time)
{
	int err;
	struct stat buf;
	if (!(err = stat(path,&buf)))
	{
		*time = ILUnixToCLITime(buf.st_atime);
	}
	else
	{
		err = ILSysIOGetErrno();
	}
	return err;
}

int ILSysIOPathGetLastModification(const char *path, ILInt64 *time)
{
	int err;
	struct stat buf;
	if (!(err = stat(path,&buf)))
	{
		*time = ILUnixToCLITime(buf.st_mtime);
	}
	else
	{
		err = ILSysIOGetErrno();
	}
	return err;
}

int ILSysIOPathGetCreation(const char *path, ILInt64 *time)
{
	int err;
	struct stat buf;
	if (!(err = stat(path,&buf)))
	{
		*time = ILUnixToCLITime(buf.st_ctime);
	}
	else
	{
		err = ILSysIOGetErrno();
	}
	return err;
}

int ILSysIOSetModificationTime(const char *path, ILInt64 time)
{
#if defined(HAVE_STAT) && defined(HAVE_UTIME)
	int retVal;
	ILInt64 unix_time;
	struct utimbuf utbuf;
	struct stat statbuf;

	/* Clear errno */
	errno = 0;

	unix_time = ILCLIToUnixTime(time);

	/* Grab the old time data first */
	retVal = stat(path, &statbuf);

	if(retVal != 0)
	{
		/* Throw out the Errno */
		return ILSysIOGetErrno();
	}

	/* Copy over the old atime value */
	utbuf.actime = statbuf.st_atime;
	
	/* Set the new mod time */
	utbuf.modtime = unix_time;

	/* And write the inode */
	retVal = utime(path, &utbuf);

	if(retVal != 0)
	{
		/* Throw out the errno */
		return ILSysIOGetErrno();
	}
	else
	{
		return IL_ERRNO_Success;
	}
#else
	return IL_ERRNO_ENOSYS;
#endif
}

int ILSysIOSetAccessTime(const char *path, ILInt64 time)
{
#if defined(HAVE_STAT) && defined(HAVE_UTIME)
	int retVal;
	ILInt64 unix_time;
	struct utimbuf utbuf;
	struct stat statbuf;

	/* Clear errno */
	errno = 0;

	unix_time = ILCLIToUnixTime(time);

	/* Grab the old time data first */
	retVal = stat(path, &statbuf);

	if(retVal != 0)
	{
		/* Throw out the Errno */
		return ILSysIOGetErrno();
	}

	/* Copy over the old mtime value */
	utbuf.modtime = statbuf.st_mtime;
	
	/* Set the new actime time */
	utbuf.actime = unix_time;

	/* And write the inode */
	retVal = utime(path, &utbuf);

	if(retVal != 0)
	{
		/* Throw out the errno */
		return ILSysIOGetErrno();
	}
	else
	{
		return IL_ERRNO_Success;
	}
#else
	return IL_ERRNO_ENOSYS;
#endif
}

int ILSysIOSetCreationTime(const char *path, ILInt64 time)
{
	return IL_ERRNO_ENOSYS;
}

int ILSysIOGetFileLength(const char *path, ILInt64 *length)
{
#if defined(HAVE_STAT)
	int retVal;
	struct stat statbuf;

	/* Clear errno */
	errno = 0;

	/* Read the file size */
	retVal = stat(path, &statbuf);

	if(retVal != 0)
	{
		/* Throw out the Errno */
		return ILSysIOGetErrno();
	}

	/*
	 * The MS docs for System.IO.FileInfo.Length say a file not
	 * found exception should be thrown if the file is a
	 * directory.  I will interpret that to mean "anything bar a
	 * regular file".
	 */
	if ((statbuf.st_mode & S_IFMT) != S_IFREG)
	{
	  	errno = ENOENT;
		return ILSysIOGetErrno();
	}

	/* Get the size */
	*length = statbuf.st_size;

	return IL_ERRNO_Success;
#else
	return IL_ERRNO_ENOSYS;
#endif
}

int ILSysIOGetFileAttributes(const char *path, ILInt32 *attributes)
{
#if defined(HAVE_STAT)
	int retVal;
	struct stat statbuf;
	int mode;

	/* Clear errno */
	errno = 0;

	retVal = stat(path, &statbuf);

	if(retVal != 0)
	{
		/* Throw out the Errno */
		return ILSysIOGetErrno();
	}

	switch (statbuf.st_mode & S_IFMT)
	{
		case S_IFBLK:
		case S_IFCHR:
		  	mode = ILFileAttributes_Device;
			break;
	        case S_IFDIR:
			mode = ILFileAttributes_Directory;
			break;
		default:
			mode = 0;
			break;
	}

	if (!(statbuf.st_mode & 0200))
	  	mode |= ILFileAttributes_ReadOnly;

	if (mode == 0)
	  	mode = ILFileAttributes_Normal;

	*attributes = mode;

	return IL_ERRNO_Success;

#else
	return IL_ERRNO_ENOSYS;
#endif
}

int ILSysIOSetFileAttributes(const char *path, ILInt32 attributes)
{
#if defined(HAVE_STAT) && defined(HAVE_CHMOD)
  	int isReadOnly;
	int retVal;
	struct stat statbuf;
	int uMask;

	/* Clear errno */
	errno = 0;

	/* Grab the old attributes first */
	retVal = stat(path, &statbuf);

	if(retVal != 0)
	{
		/* Throw out the Errno */
		return ILSysIOGetErrno();
	}

	/*
	 * The only mode we can change is readonly.  If it already
	 * matches ILSysIOGetFileAttributes definition of ReadOnly,
	 * then don't change it.
	 */
	isReadOnly = (attributes & ILFileAttributes_ReadOnly) != 0;
	if (((statbuf.st_mode & 0200) == 0) == isReadOnly)
	  	return IL_ERRNO_Success;

	/*
	 * Set the write bits accordingly.
	 */
	if (isReadOnly)
	  	statbuf.st_mode &= ~0222;
	else
	{
	  	statbuf.st_mode |= 0222;
		/*
		* Don't set all write bits - that would be dangerous.
		* Respect the umask if there is one.
		*/
#ifdef	HAVE_UMASK
		uMask = umask(0);
		umask(uMask);
		/*
		* Regardless of what umask() says we ill make GetAttributes()
		* respect the new setting.
		*/
		uMask &= ~0200;
		statbuf.st_mode &= ~uMask;
#endif
	}

	retVal = chmod(path, statbuf.st_mode & 0xfff);

	if(retVal != 0)
	{
		/* Throw out the errno */
		return ILSysIOGetErrno();
	}
	else
	{
		return IL_ERRNO_Success;
	}
#else
	return IL_ERRNO_ENOSYS;
#endif
}

#else /* IL_NO_FILE_ROUTINES */

int ILDeleteFile(const char *filename)
{
	return IL_ERRNO_ENOSYS;
}

int ILFileExists(const char *filename, char **newExePath)
{
	return 0;
}

int ILSysIOGetErrno(void)
{
	return ILSysIOConvertErrno(errno);
}

void ILSysIOSetErrno(int code)
{
	code = ILSysIOConvertFromErrno(code);
	if(code != -1)
	{
		errno = code;
	}
	else
	{
		errno = ENOSYS;
	}
}

const char *ILSysIOGetErrnoMessage(int code)
{
#ifdef HAVE_STRERROR
	code = ILSysIOConvertFromErrno(code);
	if(code != -1)
	{
		return strerror(code);
	}
	else
	{
		return 0;
	}
#else
	return 0;
#endif
}

int ILSysIOValidatePathname(const char *path)
{
	return 1;
}

ILSysIOHandle ILSysIOOpenFile(const char *path, ILUInt32 mode,
						      ILUInt32 access, ILUInt32 share)
{
	errno = ENOENT;
	return (ILSysIOHandle)(ILNativeInt)(-1);
}

int ILSysIOCheckHandleAccess(ILSysIOHandle handle, ILUInt32 access)
{
	return 0;
}

int ILSysIOClose(ILSysIOHandle handle)
{
	return 1;
}

ILInt32 ILSysIORead(ILSysIOHandle handle, void *buf, ILInt32 size)
{
	return 0;
}

ILInt32 ILSysIOWrite(ILSysIOHandle handle, const void *buf, ILInt32 size)
{
	return size;
}

ILInt64 ILSysIOSeek(ILSysIOHandle handle, ILInt64 offset, int whence)
{
	return offset;
}

int ILSysIOFlushRead(ILSysIOHandle handle)
{
	return 1;
}

int ILSysIOFlushWrite(ILSysIOHandle handle)
{
	return 1;
}

ILInt32 ILCopyFile(const char *src, const char *dest)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOTruncate(ILSysIOHandle handle, ILInt64 posn)
{
	errno = ENOSYS;
	return 0;
}

int ILSysIOHasAsync(void)
{
	return 0;
}

int ILSysIOPathGetLastAccess(const char *path, ILInt64 *time)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOPathGetLastModification(const char *path, ILInt64 *time)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOPathGetCreation(const char *path, ILInt64 *time)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOSetModificationTime(const char *path, ILInt64 time)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOSetAccessTime(const char *path, ILInt64 time)
{
	return IL_ERRNO_ENOENT;
}

int ILSysIOSetCreationTime(const char *path, ILInt64 time)
{
	return IL_ERRNO_ENOENT;
}

#endif /* IL_NO_FILE_ROUTINES */

#ifdef	__cplusplus
};
#endif
