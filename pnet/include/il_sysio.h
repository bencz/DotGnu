/*
 * il_sysio.h - Wrapper around system I/O support services that we need.
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

#ifndef	_IL_SYSIO_H
#define	_IL_SYSIO_H

#include "il_system.h"
#include "il_config.h"
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque type for a system I/O handle.
 */
typedef void *ILSysIOHandle;

/*
 * Invalid I/O handle for error returns.
 */
#define	ILSysIOHandle_Invalid	((ILSysIOHandle)(-1))

/*
 * File open modes.
 */
#define ILFileMode_CreateNew	1
#define ILFileMode_Create		2
#define	ILFileMode_Open			3
#define ILFileMode_OpenOrCreate	4
#define ILFileMode_Truncate		5
#define ILFileMode_Append		6

/*
 * File access modes.
 */
#define ILFileAccess_Read		0x01
#define ILFileAccess_Write		0x02
#define ILFileAccess_ReadWrite	(ILFileAccess_Read | ILFileAccess_Write)

/*
 * File share modes.
 */
#define	ILFileShare_None		0x00
#define	ILFileShare_Read		0x01
#define	ILFileShare_Write		0x02
#define	ILFileShare_ReadWrite	(ILFileShare_Read | ILFileAccess_Write)
#define	ILFileShare_Inheritable	0x10

/*
 * File Attibutes.
 */
#define	ILFileAttributes_ReadOnly		0x0001
#define	ILFileAttributes_Hidden			0x0002
#define	ILFileAttributes_System			0x0004
#define	ILFileAttributes_Directory		0x0010
#define	ILFileAttributes_Archive		0x0020
#define	ILFileAttributes_Device			0x0040
#define	ILFileAttributes_Normal			0x0080
#define	ILFileAttributes_Temporary		0x0100
#define	ILFileAttributes_SparseFile		0x0200
#define	ILFileAttributes_ReparsePoint	0x0400
#define	ILFileAttributes_Compressed		0x0800
#define	ILFileAttributes_Offline		0x1000
#define	ILFileAttributes_NotContentIndexed 0x2000
#define	ILFileAttributes_Encrypted		0x4000

/*
 * Get the platform-independent error number for the current thread.
 */
int ILSysIOGetErrno(void);

/*
 * Set the platform-indepedent error number for the current thread.
 */
void ILSysIOSetErrno(int code);

/*
 * Map an underlying system error code (e.g. errno) to
 * a platform-independent error number.
 */
int ILSysIOConvertErrno(int code);

/*
 * Map a platform-independent error number to a system
 * error code.  Returns -1 if no appropriate mapping.
 */
int ILSysIOConvertFromErrno(int code);

/*
 * Get the system error message corresponding to a platform-independent
 * error number.  Returns NULL if no message available.
 */
const char *ILSysIOGetErrnoMessage(int code);

/*
 * Validate a pathname to check for invalid operating system characters.
 */
int ILSysIOValidatePathname(const char *path);

/*
 * Open a file and return a system I/O handle.  Returns an
 * invalid handle on error.
 */
ILSysIOHandle ILSysIOOpenFile(const char *path, ILUInt32 mode,
						      ILUInt32 access, ILUInt32 share);

/*
 * Determine if an I/O handle was opened with a particular
 * file access mode.
 */
int ILSysIOCheckHandleAccess(ILSysIOHandle handle, ILUInt32 access);

/*
 * Close a specific I/O handle.
 */
int ILSysIOClose(ILSysIOHandle handle);

/*
 * Read from an I/O handle.
 */
ILInt32 ILSysIORead(ILSysIOHandle handle, void *buf, ILInt32 size);

/*
 * Write to an I/O handle.
 */
ILInt32 ILSysIOWrite(ILSysIOHandle handle, const void *buf, ILInt32 size);

/*
 * Seek to a new position on an I/O handle.
 */
ILInt64 ILSysIOSeek(ILSysIOHandle handle, ILInt64 offset, int whence);

/*
 * Flush the read data from an I/O handle.
 */
int ILSysIOFlushRead(ILSysIOHandle handle);

/*
 * Flush the write data to an I/O handle.
 */
int ILSysIOFlushWrite(ILSysIOHandle handle);

/*
 * Truncate a file at a particular position.
 */
int ILSysIOTruncate(ILSysIOHandle handle, ILInt64 posn);

/*
 * Lock a region of a file.
 */
int ILSysIOLock(ILSysIOHandle handle, ILInt64 position, ILInt64 length);

/*
 * Unlock a region of a file.
 */
int ILSysIOUnlock(ILSysIOHandle handle, ILInt64 position, ILInt64 length);

/*
 * Determine if it is possible to perform asynchronous I/O operations.
 */
int ILSysIOHasAsync(void);

/*
 * Sets the modification time of a specified file.
 */
int ILSysIOSetModificationTime(const char *path, ILInt64 time);

/*
 * Sets the access time of a specified file.
 */
int ILSysIOSetAccessTime(const char *path, ILInt64 time);

/*
 * Sets the creation time of a specified file.
 */
int ILSysIOSetCreationTime(const char *path, ILInt64 time);

/*
 * Copies a file from src to dest
 */
ILInt32 ILCopyFile(const char * src, const char * dest);

/*
 * Creates a directory
 */
ILInt32 ILCreateDir(const char * path);

/*
 * Determine if a socket address family is supported.
 */
int ILSysIOAddressFamilySupported(ILInt32 af);

/*
 * Create a new socket.  Returns ILSysIOHandle_Invalid on error.
 */
ILSysIOHandle ILSysIOSocket(ILInt32 domain, ILInt32 type, ILInt32 protocol);

/*
 * Bind a socket to an address.  Returns zero on error.
 */
int ILSysIOSocketBind(ILSysIOHandle sockfd, unsigned char *addr,
					  ILInt32 addrLen);

/*
 * Connect to a remote socket address.  Returns zero on error.
 */
int ILSysIOSocketConnect(ILSysIOHandle sockfd, unsigned char *addr,
						 ILInt32 addrLen);

/*
 * Set a socket to listen mode.  Returns zero on error.
 */
int ILSysIOSocketListen(ILSysIOHandle sockfd, ILInt32 backlog);

/*
 * Accept a connection from a remote address on a socket.
 */
ILSysIOHandle ILSysIOSocketAccept(ILSysIOHandle sockfd,
								  unsigned char *addr, ILInt32 addrLen);

/*
 * Receive data on a socket.
 */
ILInt32 ILSysIOSocketReceive(ILSysIOHandle sockfd, void *buff,
							 ILInt32 len, ILInt32 flags);

/*
 * Send data on a socket.
 */
ILInt32 ILSysIOSocketSend(ILSysIOHandle sockfd, const void *msg,
					      ILInt32 len, ILInt32 flags);

/*
 * Send data on a socket to a specific address.
 */
ILInt32 ILSysIOSocketSendTo(ILSysIOHandle sockfd, const void *msg, ILInt32 len,
					        ILInt32 flags, unsigned char *addr,
							ILInt32 addrLen);

/*
 * Receive data on a socket from a specific address.
 */
ILInt32 ILSysIOSocketRecvFrom(ILSysIOHandle sockfd, void *buf, ILInt32 len,
							  ILInt32 flags, unsigned char *addr,
							  ILInt32 addrLen);

/*
 * Close a socket.  Returns zero on error.
 */
int ILSysIOSocketClose(ILSysIOHandle sockfd);

/*
 * Perform a shutdown operation on one or more socket directions.
 * Returns zero on error.
 *
 * Values for "how":
 *		0 - Further receives are disallowed
 *		1 - Further sends are disallowed
 *		2 - Further sends and receives are disallowed
 */
int ILSysIOSocketShutdown(ILSysIOHandle sockfd, ILInt32 how);

/*
 * Perform a select operation on a collection of sockets.
 * Returns -1 on error, 0 on timeout, or the number of
 * file descriptors that fired.  The input arrays will be
 * modified so that any descriptors that did not fire are
 * replaced with ILSysIOHandle_Invalid.
 */
ILInt32 ILSysIOSocketSelect(ILSysIOHandle **readfds, ILInt32 numRead,
						    ILSysIOHandle **writefds, ILInt32 numWrite,
						    ILSysIOHandle **exceptfds, ILInt32 numExcept,
						    ILInt64 timeout);

/*
 * Set or reset the blocking flag on a socket.  Returns zero on error.
 */
int ILSysIOSocketSetBlocking(ILSysIOHandle sockfd, int flag);

/*
 * Get the number of bytes that are available on a socket.
 * Returns -1 on error.
 */
ILInt32 ILSysIOSocketGetAvailable(ILSysIOHandle sockfd);

/*
 * Get the name of a local socket's end-point.  Returns zero on error.
 */
int ILSysIOSocketGetName(ILSysIOHandle sockfd, unsigned char *addr,
						 ILInt32 addrLen);

/*
 * Socket option levels.  Must match "System.Net.Sockets.SocketOptionLevel".
 */
#define	IL_SOL_IP			0
#define	IL_SOL_TCP			6
#define	IL_SOL_UDP			17
#define	IL_SOL_SOCKET		65535

/*
 * Socket option names.  Must match "System.Net.Sockets.SocketOptionName".
 */
#define	IL_SO_ADD_MEMBERSHIP	12		/* IP options */
#define	IL_SO_DROP_MEMBERSHIP	13
#define	IL_SO_NO_DELAY			1		/* TCP options */
#define	IL_SO_EXPEDITED			2
#define	IL_SO_NO_CHECKSUM		1		/* UDP options */
#define	IL_SO_CHKSUM_COVERAGE	20
#define	IL_SO_REUSE_ADDRESS		0x0004	/* Socket options */
#define	IL_SO_KEEP_ALIVE		0x0008
#define	IL_SO_SEND_BUFFER		0x1001
#define	IL_SO_RECV_BUFFER		0x1002
#define	IL_SO_SEND_TIMEOUT		0x1005
#define	IL_SO_RECV_TIMEOUT		0x1006

#define IL_SO_BROADCAST 32

/*
 * Set an integer or boolean socket option.  Returns zero on error.
 */
int ILSysIOSocketSetOption(ILSysIOHandle sockfd, ILInt32 level,
						   ILInt32 name, ILInt32 value);

/*
 * Get an integer or boolean socket option.  Returns zero on error.
 */
int ILSysIOSocketGetOption(ILSysIOHandle sockfd, ILInt32 level,
						   ILInt32 name, ILInt32 *value);

/*
 * Set the linger option on a socket.  Returns zero on error.
 */
int ILSysIOSocketSetLinger(ILSysIOHandle handle, int enabled, int seconds);

/*
 * Get the linger option on a socket.  Returns zero on error.
 */
int ILSysIOSocketGetLinger(ILSysIOHandle handle, int *enabled, int *seconds);

/*
 * Set a multicast option on a socket.  Returns zero on error.
 */
int ILSysIOSocketSetMulticast(ILSysIOHandle handle, ILInt32 af, ILInt32 name,
							  unsigned char *group, ILInt32 groupLen,
							  unsigned char *mcint, ILInt32 mcintLen);

/*
 * Get a multicast option on a socket.  Returns zero on error.
 */
int ILSysIOSocketGetMulticast(ILSysIOHandle handle, ILInt32 af, ILInt32 name,
							  unsigned char *group, ILInt32 groupLen,
							  unsigned char *mcint, ILInt32 mcintLen);

/*
 * Discover the IrDA devices that are accesible via a socket.
 * Returns zero on error.
 */
int ILSysIODiscoverIrDADevices(ILSysIOHandle handle, unsigned char *buf,
							   ILInt32 bufLen);

/* dns.c */
struct hostent* ILGetHostByName(const char *name);
struct hostent* ILGetHostByAddr(const void *addr, unsigned int len, int type);

/*
 * Platform wrapper over gethostname
 */
int ILGetHostName(const char *name, unsigned int size);
	
/*
 * Obtains the last access time of 'path' and stores that information in 'time'.
 * Returns 0 on success, errno otherwise.
 */
int ILSysIOPathGetLastAccess(const char *path, ILInt64 *time);

/*
 * Obtains the last modification time of 'path' and stores that information in 'time'.
 * Returns 0 on success, errno otherwise.
 */
int ILSysIOPathGetLastModification(const char *path, ILInt64 *time);

/*
 * Obtains the creation time of 'path' and stores that information in 'time'.
 * Returns 0 on success, errno otherwise.
 */
int ILSysIOPathGetCreation(const char *path, ILInt64 *time);


/*
 * Opaque type that is used to represent an open directory and an entry.
 */
typedef struct _tagILDir    ILDir;
typedef struct _tagILDirEnt ILDirEnt;

/*
 * Values returned from "ILDirEntType".
 */
#define ILFileType_Unknown			0
#define	ILFileType_FIFO				1
#define	ILFileType_CHR				2
#define	ILFileType_DIR				4
#define	ILFileType_BLK				6
#define	ILFileType_REG				8
#define	ILFileType_LNK				10
#define	ILFileType_SOCK				12
#define	ILFileType_WHT				14

/*
 * Directory access functions.
 */
ILInt32 ILDeleteDir(const char *path);
ILInt32 ILRenameDir(const char *old_name, const char *new_name);
ILDir *ILOpenDir(const char *path);
ILDirEnt *ILReadDir(ILDir *directory);
int ILCloseDir(ILDir *directory);
const char *ILDirEntName(ILDirEnt *entry);
int ILDirEntType(ILDirEnt *entry);

/*
 * Change Directory
 */
ILInt32 ILChangeDir(const char *path);

/*
 * Get the type of file at a particular location.  Returns
 * "ILFileType_Unknown" if the file does not exist.
 */
int ILGetFileType(const char *path);

/*
 * Get the length of a file in bytes.  Returns 0 on success,
 * the errno otherwise.
 */
int ILSysIOGetFileLength(const char *path, ILInt64 *length);

/*
 * Get the file attributes.  Returns 0 on success, the errno
 * otherwise.  The mapping of host OS attributes to .Net
 * attributes can not be done exactly.
 */
int ILSysIOGetFileAttributes(const char *path, ILInt32 *attributes);

/*
 * Set the file attributes.  Returns 0 on success, the errno
 * otherwise.  The function does nothing if the ReadOnly bit is
 * not changed.  Setting ReadOnly removes all write bits from
 * the files permissions, setting ReadOnly sets all bits that
 * aren't in the umask().
 */
int ILSysIOSetFileAttributes(const char *path, ILInt32 attributes);

#ifdef	__cplusplus 
};
#endif

#endif	/* _IL_SYSIO_H */
