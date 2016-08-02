/*
 * lib_file.c - Internalcall methods for "System.IO" and subclasses
 *
 * Copyright (C) 2002  FSF INDIA and Southern Storm Software, Pty Ltd.
 *
 * Authors: Gopal.V
 *          Rhys Weatherley
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

#include "engine.h"
#include "lib_defs.h"
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

char *ILStringToPathname(ILExecThread *thread, ILString *str)
{
#ifdef IL_WIN32_NATIVE
	/* Win32 understands both '/' and '\' in pathnames, so leave as-is */
	return ILStringToAnsi(thread, str);
#else
	char *path = ILStringToAnsi(thread, str);
	char *temp;
	if(!path)
	{
		return 0;
	}
	temp = path;
	while(*temp != '\0')
	{
		if(*temp == '\\')
		{
			*temp = '/';
		}
		++temp;
	}
	return path;
#endif
}

/*
 * public static IntPtr GetInvalidHandle();
 */
ILNativeInt _IL_FileMethods_GetInvalidHandle(ILExecThread *thread)
{
	return (ILNativeInt)ILSysIOHandle_Invalid;
}

/*
 * public static bool ValidatePathname(String path);
 */
ILBool _IL_FileMethods_ValidatePathname(ILExecThread *thread, ILString *path)
{
	char *cpath = ILStringToPathname(thread, path);
	if(cpath)
	{
		return (ILBool)(ILSysIOValidatePathname(cpath));
	}
	else
	{
		return 0;
	}
}

/*
 * public static FileType GetFileType(String path);
 */
ILInt32 _IL_FileMethods_GetFileType(ILExecThread * _thread, ILString * path)
{
	const char *cpath = ILStringToPathname(_thread,path);
	if(cpath)
	{
		return ILGetFileType(cpath);
	}
	else
	{
		return 0;
	}
}

/*
 * public static bool Open(String path, FileMode mode,
 *						   FileAccess access, FileShare share,
 *						   out IntPtr handle);
 */
ILBool _IL_FileMethods_Open(ILExecThread *thread, ILString *path, 
						    ILInt32 mode, ILInt32 access, ILInt32 share,
							ILNativeInt *handle)
{
	char *cpath = ILStringToPathname(thread, path);
	if(!cpath)
	{
		ILSysIOSetErrno(IL_ERRNO_ENOMEM);
		return 0;
	}
	*handle = (ILNativeInt)ILSysIOOpenFile(cpath, mode, access, share);
	return (*handle != (ILNativeInt)ILSysIOHandle_Invalid);
}

/*
 * public static bool Write(IntPtr handle, byte[] buffer,
 *                          int offset, int count);
 */
ILBool _IL_FileMethods_Write(ILExecThread *thread, ILNativeInt handle,
						     System_Array *array, ILInt32 offset,
							 ILInt32 count)
{
	ILUInt8 *buf = (ILUInt8 *)(ArrayToBuffer(array));
	return (ILSysIOWrite((ILSysIOHandle)handle, buf + offset, count) == count);
}

/*
 * public static int Read(IntPtr handle, byte[] buffer,
 *                        int offset, int count);
 */
ILInt32 _IL_FileMethods_Read(ILExecThread *thread, ILNativeInt handle,
							 System_Array *array, ILInt32 offset,
							 ILInt32 count)
{
	ILUInt8 *buf = (ILUInt8 *)(ArrayToBuffer(array));
	return ILSysIORead((ILSysIOHandle)handle, buf + offset, count);
}

/*
 * public static bool HasAsync();
 */
ILBool _IL_FileMethods_HasAsync(ILExecThread *thread)
{
	return ILSysIOHasAsync();
}

/*
 * public static bool CanSeek(IntPtr handle);
 */
ILBool _IL_FileMethods_CanSeek(ILExecThread *thread, ILNativeInt handle)
{
	/* Try seeking to the current position, which will fail
	   on non-seekable streams like pipes and sockets */
	return (ILSysIOSeek((ILSysIOHandle)handle, (ILInt64)0, 1) != (ILInt64)(-1));
}

/*
 * public static bool CheckHandleAccess(IntPtr handle, FileAccess access);
 */
ILBool _IL_FileMethods_CheckHandleAccess(ILExecThread *thread,
										 ILNativeInt handle, ILInt32 access)
{
	return (ILBool)(ILSysIOCheckHandleAccess((ILSysIOHandle)handle, access));
}

/*
 * public static long Seek(IntPtr handle, long offset, SeekOrigin origin);
 */
ILInt64 _IL_FileMethods_Seek(ILExecThread *thread, ILNativeInt handle,
						     ILInt64 offset, ILInt32 origin)
{
	return ILSysIOSeek((ILSysIOHandle)handle, offset, origin);
}

/*
 * public static bool Close(IntPtr handle);
 */
ILBool _IL_FileMethods_Close(ILExecThread *thread, ILNativeInt handle)
{
	return (ILBool)(ILSysIOClose((ILSysIOHandle)handle));
}

/*
 * public static bool FlushWrite(IntPtr handle);
 */
ILBool _IL_FileMethods_FlushWrite(ILExecThread *thread, ILNativeInt handle)
{
	return (ILBool)(ILSysIOFlushWrite((ILSysIOHandle)handle));
}

/*
 * public static bool SetLength(IntPtr handle, long value);
 */
ILBool _IL_FileMethods_SetLength(ILExecThread *thread, ILNativeInt handle,
							     ILInt64 value)
{
	return (ILBool)(ILSysIOTruncate((ILSysIOHandle)handle, value));
}

/*
 * public static bool Lock(IntPtr handle, long position, long length);
 */
ILBool _IL_FileMethods_Lock(ILExecThread *_thread, ILNativeInt handle,
							ILInt64 position, ILInt64 length)
{
	return (ILBool)(ILSysIOLock((ILSysIOHandle)handle, position, length));
}

/*
 * public static bool Unlock(IntPtr handle, long position, long length);
 */
ILBool _IL_FileMethods_Unlock(ILExecThread *_thread, ILNativeInt handle,
							  ILInt64 position, ILInt64 length)
{
	return (ILBool)(ILSysIOUnlock((ILSysIOHandle)handle, position, length));
}

/*
 * public static Errno GetErrno();
 */
ILInt32 _IL_FileMethods_GetErrno(ILExecThread *thread)
{
	return ILSysIOGetErrno();
}

/*
 * public static String GetErrnoMessage(Errno error);
 */
ILString *_IL_FileMethods_GetErrnoMessage(ILExecThread *thread, ILInt32 error)
{
	const char *msg = ILSysIOGetErrnoMessage(error);
	if(msg)
	{
		return ILStringCreate(thread, msg);
	}
	else
	{
		return 0;
	}
}

/*
 * public static Errno Copy(String src, String dest);
 */
ILInt32 _IL_FileMethods_Copy(ILExecThread *_thread,
							ILString *src, ILString *dest)
{
	char * src_ansi  = ILStringToPathname(_thread, src );
	char * dest_ansi = ILStringToPathname(_thread, dest);
	if(src_ansi == NULL || dest_ansi == NULL)
	{
		return IL_ERRNO_ENOMEM;
	}
	return ILCopyFile(src_ansi, dest_ansi);
}

/*
 * public static Errno SetLastWriteTime(String path, long ticks);
 */
ILInt32 _IL_FileMethods_SetLastWriteTime(ILExecThread *thread, ILString *path, ILInt64 ticks)
{
	char *path_ansi = ILStringToPathname(thread, path);
	
	if(!path_ansi)
	{
		ILSysIOSetErrno(IL_ERRNO_ENOMEM);
		return 0;
	}

	return ILSysIOSetModificationTime(path_ansi, ticks);
}

/*
 * public static Errno SetLastAccessTime(String path, long ticks);
 */
ILInt32 _IL_FileMethods_SetLastAccessTime(ILExecThread *thread, ILString *path, ILInt64 ticks)
{
	char *path_ansi = ILStringToPathname(thread, path);
	
	if(!path_ansi)
	{
		ILSysIOSetErrno(IL_ERRNO_ENOMEM);
		return 0;
	}

	return ILSysIOSetAccessTime(path_ansi, ticks);
}

/*
 * public static Errno SetCreationTime(String path, long ticks);
 */
ILInt32 _IL_FileMethods_SetCreationTime(ILExecThread *thread, ILString *path, ILInt64 ticks)
{
	char *path_ansi = ILStringToPathname(thread, path);
	
	if(!path_ansi)
	{
		ILSysIOSetErrno(IL_ERRNO_ENOMEM);
		return 0;
	}

	return ILSysIOSetCreationTime(path_ansi, ticks);
}

/*
 * public static Errno GetAttributes(String path, out int attrs);
 */
ILInt32 _IL_FileMethods_GetAttributes(ILExecThread *_thread,
									  ILString *path, ILInt32 *attrs)
{
	char *path_ansi = ILStringToPathname(_thread, path);

	return ILSysIOGetFileAttributes(path_ansi, attrs);
}

/*
 * public static Errno SetAttributes(String path, int attrs);
 */
ILInt32 _IL_FileMethods_SetAttributes(ILExecThread *_thread,
									  ILString *path, ILInt32 attrs)
{
	char *path_ansi = ILStringToPathname(_thread, path);

	return ILSysIOSetFileAttributes(path_ansi, attrs);
}

/*
 * public static Errno GetLength(String path, out long length);
 */
ILInt32 _IL_FileMethods_GetLength(ILExecThread *_thread,
								  ILString *path, ILInt64 *length)
{
	char *path_ansi = ILStringToPathname(_thread, path);

	return ILSysIOGetFileLength(path_ansi, length);
}

/*
 * public static Errno ReadLink(String path, out String contents);
 */
ILInt32 _IL_FileMethods_ReadLink(ILExecThread *_thread, ILString *path,
								 ILString **contents)
{
#ifdef HAVE_READLINK
	char *pathAnsi = ILStringToPathname(_thread, path);
	char buf[1024];
	int len;
	if(!pathAnsi)
	{
		return IL_ERRNO_ENOMEM;
	}
	len = readlink(pathAnsi, buf, sizeof(buf) - 1);
	if(len >= 0)
	{
		buf[len] = '\0';
		*contents = ILStringCreate(_thread, buf);
		return IL_ERRNO_Success;
	}
	else if(errno == EINVAL)
	{
		*contents = 0;
		return IL_ERRNO_Success;
	}
	else
	{
		*contents = 0;
		return ILSysIOGetErrno();
	}
#else
	*contents = 0;
	return IL_ERRNO_Success;
#endif
}

/*
 * public static Errno CreateLink(String oldpath, String newpath);
 */
ILInt32 _IL_FileMethods_CreateLink(ILExecThread *_thread, ILString *oldpath,
								   ILString *newpath)
{
#ifdef HAVE_SYMLINK
	char *path1 = ILStringToPathname(_thread, oldpath);
	char *path2 = ILStringToPathname(_thread, newpath);
	if(!path1 || !path2)
	{
		return IL_ERRNO_ENOMEM;
	}
	if(symlink(path1, path2) >= 0)
	{
		return IL_ERRNO_Success;
	}
	else
	{
		return ILSysIOGetErrno();
	}
#else
	return IL_ERRNO_EPERM;
#endif
}
