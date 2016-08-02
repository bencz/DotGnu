/*
 * strerror.c - Get error messages for "errno" codes.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include <string.h>
#include <stdio.h>

const char * const sys_errlist[] = {
	"Success",
	"Operation not permitted",
	"No such file or directory",
	"No such process",
	"Interrupted system call",
	"Input/output error",
	"No such device or address",
	"Argument list too long",
	"Exec format error",
	"Bad file descriptor",
	"No child processes",
	"Resource temporarily unavailable",
	"Cannot allocate memory",
	"Permission denied",
	"Bad address",
	"Block device required",
	"Device or resource busy",
	"File exists",
	"Invalid cross-device link",
	"No such device",
	"Not a directory",
	"Is a directory",
	"Invalid argument",
	"Too many open files in system",
	"Too many open files",
	"Inappropriate ioctl for device",
	"Text file busy",
	"File too large",
	"No space left on device",
	"Illegal seek",
	"Read-only file system",
	"Too many links",
	"Broken pipe",
	"Numerical argument out of domain",
	"Numerical result out of range",
	"Resource deadlock avoided",
	"File name too long",
	"No locks available",
	"Function not implemented",
	"Directory not empty",
	"Too many levels of symbolic links",
	"Unknown error 41",
	"No message of desired type",
	"Identifier removed",
	"Channel number out of range",
	"Level 2 not synchronized",
	"Level 3 halted",
	"Level 3 reset",
	"Link number out of range",
	"Protocol driver not attached",
	"No CSI structure available",
	"Level 2 halted",
	"Invalid exchange",
	"Invalid request descriptor",
	"Exchange full",
	"No anode",
	"Invalid request code",
	"Invalid slot",
	"Unknown error 58",
	"Bad font file format",
	"Device not a stream",
	"No data available",
	"Timer expired",
	"Out of streams resources",
	"Machine is not on the network",
	"Package not installed",
	"Object is remote",
	"Link has been severed",
	"Advertise error",
	"Srmount error",
	"Communication error on send",
	"Protocol error",
	"Multihop attempted",
	"RFS specific error",
	"Bad message",
	"Value too large for defined data type",
	"Name not unique on network",
	"File descriptor in bad state",
	"Remote address changed",
	"Can not access a needed shared library",
	"Accessing a corrupted shared library",
	".lib section in a.out corrupted",
	"Attempting to link in too many shared libraries",
	"Cannot exec a shared library directly",
	"Invalid or incomplete multibyte or wide character",
	"Interrupted system call should be restarted",
	"Streams pipe error",
	"Too many users",
	"Socket operation on non-socket",
	"Destination address required",
	"Message too long",
	"Protocol wrong type for socket",
	"Protocol not available",
	"Protocol not supported",
	"Socket type not supported",
	"Operation not supported",
	"Protocol family not supported",
	"Address family not supported by protocol",
	"Address already in use",
	"Cannot assign requested address",
	"Network is down",
	"Network is unreachable",
	"Network dropped connection on reset",
	"Software caused connection abort",
	"Connection reset by peer",
	"No buffer space available",
	"Transport endpoint is already connected",
	"Transport endpoint is not connected",
	"Cannot send after transport endpoint shutdown",
	"Too many references: cannot splice",
	"Connection timed out",
	"Connection refused",
	"Host is down",
	"No route to host",
	"Operation already in progress",
	"Operation now in progress",
	"Stale NFS file handle",
	"Structure needs cleaning",
	"Not a XENIX named type file",
	"No XENIX semaphores available",
	"Is a named type file",
	"Remote I/O error",
	"Disk quota exceeded",
	"No medium found",
	"Wrong medium type"
};
int sys_nerr = sizeof (sys_errlist) / sizeof (char *);

char *
strerror (int errnum)
{
  if (errnum >= 0 && errnum < sys_nerr)
    return (char *)(sys_errlist[errnum]);
  else
    return "Unknown error";
}

char *
strerror_r (int errnum, char *buf, size_t buflen)
{
  strncpy (buf, strerror (errnum), buflen);
  return buf;
}
