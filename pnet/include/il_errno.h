/*
 * il_errno.h - Platform-independent error codes.
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

#ifndef	_IL_ERRNO_H
#define	_IL_ERRNO_H

#ifdef	__cplusplus
extern	"C" {
#endif

/* These values must match "Platform.Errno" in the pnetlib library */

#define IL_ERRNO_Success 		0	/* Operation succeeded */
#define IL_ERRNO_EPERM			1	/* Operation not permitted */
#define IL_ERRNO_ENOENT			2	/* No such file or directory */
#define IL_ERRNO_ENOFILE		IL_ERRNO_ENOENT /* No such file */
#define IL_ERRNO_ESRCH			3	/* No such process */
#define IL_ERRNO_EINTR			4	/* Interrupted system call */
#define IL_ERRNO_EIO			5	/* I/O error */
#define IL_ERRNO_ENXIO			6	/* No such device or address */
#define IL_ERRNO_E2BIG			7	/* Arg list too long */
#define IL_ERRNO_ENOEXEC		8	/* Exec format error */
#define IL_ERRNO_EBADF			9	/* Bad file number */
#define IL_ERRNO_ECHILD			10	/* No child processes */
#define IL_ERRNO_EAGAIN			11	/* Try again */
#define IL_ERRNO_ENOMEM			12	/* Out of memory */
#define IL_ERRNO_EACCES			13	/* Permission denied */
#define IL_ERRNO_EFAULT			14	/* Bad address */
#define IL_ERRNO_ENOTBLK		15	/* Block device required */
#define IL_ERRNO_EBUSY			16	/* Device or resource busy */
#define IL_ERRNO_EEXIST			17	/* File exists */
#define IL_ERRNO_EXDEV			18	/* Cross-device link */
#define IL_ERRNO_ENODEV			19	/* No such device */
#define IL_ERRNO_ENOTDIR		20	/* Not a directory */
#define IL_ERRNO_EISDIR			21	/* Is a directory */
#define IL_ERRNO_EINVAL			22	/* Invalid argument */
#define IL_ERRNO_ENFILE			23	/* File table overflow */
#define IL_ERRNO_EMFILE			24	/* Too many open files */
#define IL_ERRNO_ENOTTY			25	/* Not a typewriter */
#define IL_ERRNO_ETXTBSY		26	/* Text file busy */
#define IL_ERRNO_EFBIG			27	/* File too large */
#define IL_ERRNO_ENOSPC			28	/* No space left on device */
#define IL_ERRNO_ESPIPE			29	/* Illegal seek */
#define IL_ERRNO_EROFS			30	/* Read-only file system */
#define IL_ERRNO_EMLINK			31	/* Too many links */
#define IL_ERRNO_EPIPE			32	/* Broken pipe */
#define IL_ERRNO_EDOM			33	/* Math argument out of domain of func */
#define IL_ERRNO_ERANGE			34	/* Math result not representable */
#define IL_ERRNO_EDEADLK		35	/* Resource deadlock would occur */
#define IL_ERRNO_ENAMETOOLONG	36	/* File name too long */
#define IL_ERRNO_ENOLCK			37	/* No record locks available */
#define IL_ERRNO_ENOSYS			38	/* Function not implemented */
#define IL_ERRNO_ENOTEMPTY		39	/* Directory not empty */
#define IL_ERRNO_ELOOP			40	/* Too many symbolic links encountered */
#define IL_ERRNO_EWOULDBLOCK	IL_ERRNO_EAGAIN /* Operation would block */
#define IL_ERRNO_ENOMSG			42	/* No message of desired type */
#define IL_ERRNO_EIDRM			43	/* Identifier removed */
#define IL_ERRNO_ECHRNG			44	/* Channel number out of range */
#define IL_ERRNO_EL2NSYNC		45	/* Level 2 not synchronized */
#define IL_ERRNO_EL3HLT			46	/* Level 3 halted */
#define IL_ERRNO_EL3RST			47	/* Level 3 reset */
#define IL_ERRNO_ELNRNG			48	/* Link number out of range */
#define IL_ERRNO_EUNATCH		49	/* Protocol driver not attached */
#define IL_ERRNO_ENOCSI			50	/* No CSI structure available */
#define IL_ERRNO_EL2HLT			51	/* Level 2 halted */
#define IL_ERRNO_EBADE			52	/* Invalid exchange */
#define IL_ERRNO_EBADR			53	/* Invalid request descriptor */
#define IL_ERRNO_EXFULL			54	/* Exchange full */
#define IL_ERRNO_ENOANO			55	/* No anode */
#define IL_ERRNO_EBADRQC		56	/* Invalid request code */
#define IL_ERRNO_EBADSLT		57	/* Invalid slot */
#define IL_ERRNO_EDEADLOCK		IL_ERRNO_EDEADLK
#define IL_ERRNO_EBFONT			59	/* Bad font file format */
#define IL_ERRNO_ENOSTR			60	/* Device not a stream */
#define IL_ERRNO_ENODATA		61	/* No data available */
#define IL_ERRNO_ETIME			62	/* Timer expired */
#define IL_ERRNO_ENOSR			63	/* Out of streams resources */
#define IL_ERRNO_ENONET			64	/* Machine is not on the network */
#define IL_ERRNO_ENOPKG			65	/* Package not installed */
#define IL_ERRNO_EREMOTE		66	/* Object is remote */
#define IL_ERRNO_ENOLINK		67	/* Link has been severed */
#define IL_ERRNO_EADV			68	/* Advertise error */
#define IL_ERRNO_ESRMNT			69	/* Srmount error */
#define IL_ERRNO_ECOMM			70	/* Communication error on send */
#define IL_ERRNO_EPROTO			71	/* Protocol error */
#define IL_ERRNO_EMULTIHOP		72	/* Multihop attempted */
#define IL_ERRNO_EDOTDOT		73	/* RFS specific error */
#define IL_ERRNO_EBADMSG		74	/* Not a data message */
#define IL_ERRNO_EOVERFLOW		75	/* Value too large for defined data type */
#define IL_ERRNO_ENOTUNIQ		76	/* Name not unique on network */
#define IL_ERRNO_EBADFD			77	/* File descriptor in bad state */
#define IL_ERRNO_EREMCHG		78	/* Remote address changed */
#define IL_ERRNO_ELIBACC		79	/* Can not access a needed shared library */
#define IL_ERRNO_ELIBBAD		80	/* Accessing a corrupted shared library */
#define IL_ERRNO_ELIBSCN		81	/* .lib section in a.out corrupted */
#define IL_ERRNO_ELIBMAX		82	/* Attempting to link in too many shared libs */
#define IL_ERRNO_ELIBEXEC		83	/* Cannot exec a shared library directly */
#define IL_ERRNO_EILSEQ			84	/* Illegal byte sequence */
#define IL_ERRNO_ERESTART		85	/* Interrupted system call should be restarted */
#define IL_ERRNO_ESTRPIPE		86	/* Streams pipe error */
#define IL_ERRNO_EUSERS			87	/* Too many users */
#define IL_ERRNO_ENOTSOCK		88	/* Socket operation on non-socket */
#define IL_ERRNO_EDESTADDRREQ	89	/* Destination address required */
#define IL_ERRNO_EMSGSIZE		90	/* Message too long */
#define IL_ERRNO_EPROTOTYPE		91	/* Protocol wrong type for socket */
#define IL_ERRNO_ENOPROTOOPT	92	/* Protocol not available */
#define IL_ERRNO_EPROTONOSUPPORT 93	/* Protocol not supported */
#define IL_ERRNO_ESOCKTNOSUPPORT 94	/* Socket type not supported */
#define IL_ERRNO_EOPNOTSUPP		95	/* Operation not supported on transport endpoint */
#define IL_ERRNO_EPFNOSUPPORT	96	/* Protocol family not supported */
#define IL_ERRNO_EAFNOSUPPORT	97	/* Address family not supported by protocol */
#define IL_ERRNO_EADDRINUSE		98	/* Address already in use */
#define IL_ERRNO_EADDRNOTAVAIL	99	/* Cannot assign requested address */
#define IL_ERRNO_ENETDOWN		100	/* Network is down */
#define IL_ERRNO_ENETUNREACH	101	/* Network is unreachable */
#define IL_ERRNO_ENETRESET		102	/* Network dropped connection because of reset */
#define IL_ERRNO_ECONNABORTED	103	/* Software caused connection abort */
#define IL_ERRNO_ECONNRESET		104	/* Connection reset by peer */
#define IL_ERRNO_ENOBUFS		105	/* No buffer space available */
#define IL_ERRNO_EISCONN		106	/* Transport endpoint is already connected */
#define IL_ERRNO_ENOTCONN		107	/* Transport endpoint is not connected */
#define IL_ERRNO_ESHUTDOWN		108	/* Cannot send after transport endpoint shutdown */
#define IL_ERRNO_ETOOMANYREFS	109	/* Too many references: cannot splice */
#define IL_ERRNO_ETIMEDOUT		110	/* Connection timed out */
#define IL_ERRNO_ECONNREFUSED	111	/* Connection refused */
#define IL_ERRNO_EHOSTDOWN		112	/* Host is down */
#define IL_ERRNO_EHOSTUNREACH	113	/* No route to host */
#define IL_ERRNO_EALREADY		114	/* Operation already in progress */
#define IL_ERRNO_EINPROGRESS	115	/* Operation now in progress */
#define IL_ERRNO_ESTALE			116	/* Stale NFS file handle */
#define IL_ERRNO_EUCLEAN		117	/* Structure needs cleaning */
#define IL_ERRNO_ENOTNAM		118	/* Not a XENIX named type file */
#define IL_ERRNO_ENAVAIL		119	/* No XENIX semaphores available */
#define IL_ERRNO_EISNAM			120	/* Is a named type file */
#define IL_ERRNO_EREMOTEIO		121	/* Remote I/O error */
#define IL_ERRNO_EDQUOT			122	/* Quota exceeded */
#define IL_ERRNO_ENOMEDIUM		123	/* No medium found */
#define IL_ERRNO_EMEDIUMTYPE	124	/* Wrong medium type */

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_ERRNO_H */
