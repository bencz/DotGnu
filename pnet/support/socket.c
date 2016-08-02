/*
 * socket.c - Socket-related functions.
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

#include "il_sysio.h"
#include "il_errno.h"

#ifdef IL_CONFIG_NETWORKING

#ifdef IL_WIN32_NATIVE
#include <winsock.h>
#include <time.h>
#define	close	closesocket
#define	ioctl	ioctlsocket
#define	HAVE_SIN6_SCOPE_ID	1
#define	HAVE_IOCTL			1
#define	HAVE_SETSOCKOPT		1
#define	HAVE_GETSOCKOPT		1
#else
#ifdef HAVE_FCNTL_H
#include <fcntl.h>
#endif
#if TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #else
        #include <time.h>
    #endif
#endif
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef HAVE_SYS_SOCKET_H
#include <sys/socket.h>
#endif
#ifdef HAVE_SYS_SELECT_H
#include <sys/select.h>
#endif
#ifdef HAVE_NETINET_IN_H
#include <netinet/in.h>
#endif
#ifdef HAVE_NETINET_TCP_H
#include <netinet/tcp.h>
#endif
#ifdef HAVE_NETINET_UDP_H
#include <netinet/udp.h>
#endif
#ifdef HAVE_SYS_IOCTL_H
#include <sys/ioctl.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#include <errno.h>
#if HAVE_LINUX_IRDA_H
#include <linux/types.h>
#include <linux/irda.h>
#endif
#endif

#ifdef IL_WIN32_NATIVE
	/* Win32 defines IPPROTO_TCP, not SOL_TCP */
	#if !defined (SOL_TCP) && defined(IPPROTO_TCP)
		#define SOL_TCP IPPROTO_TCP
	#endif
	/* Win32 defines IPPROTO_UDP, not SOL_UDP */
	#if !defined (SOL_UDP) && defined(IPPROTO_UDP)
		#define SOL_UDP IPPROTO_UDP
	#endif 
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * See if the platform appears to have IPv6 and IrDA support.
 * We assume that all platforms support IPv4.
 */
#ifdef IN6ADDR_ANY_INIT
#define	IL_IPV6_PRESENT		1
#endif
#if defined(LSAP_ANY) || defined(IL_WIN32_PLATFORM)
#define	IL_IRDA_PRESENT		1
#endif

#ifdef IL_WIN32_PLATFORM

/*
 * Defines from AF_Irda.h
 * This file is not part of cygwin thus we provide necessary definitions here.
 */

#define AF_IRDA				26
#define SOL_IRLMP			0xFF
#define IRLMP_ENUMDEVICES	0x10

struct sockaddr_irda
{
	u_short irdaAddressFamily;
	u_char  irdaDeviceID[4];
	char	irdaServiceName[25];
};

#endif

/*
 * Address families at the C# level, which may not be the same
 * as those at the operating system level.
 */
#define	IL_AF_INET			2
#define	IL_AF_INET6			23
#define	IL_AF_IRDA			26

/*
 * Socket type at the C# level, which may not be the same
 * as those at the operating system level.
 */
#define IL_SOCK_UNKNOWN                 -1
#define IL_SOCK_STREAM                   1
#define IL_SOCK_DGRAM                    2
#define IL_SOCK_RAW                      3
#define IL_SOCK_RDM                      4
#define IL_SOCK_SEQPACKET                5 

/*
 * Combined socket address structure.
 */
typedef union
{
	struct sockaddr			addr;
	struct sockaddr_in		ipv4_addr;
#ifdef IL_IPV6_PRESENT
	struct sockaddr_in6		ipv6_addr;
#endif
#ifdef IL_IRDA_PRESENT
	struct sockaddr_irda	irda_addr;
#endif

} CombinedSockAddr;

#ifdef IL_WIN32_NATIVE

/*
 * Initialize the winsock library.
 */
void _ILWinSockInit(void)
{
	static int volatile initialized = 0;
	WSADATA data;
	if(!initialized)
	{
		WSAStartup(MAKEWORD(1, 1), &data);
		initialized = 1;
	}
}

#endif /* IL_WIN32_NATIVE */

/*
 * Convert a serialized address buffer into a combined socket address.
 * Returns zero if there is something wrong with the buffer.
 *
 * Note: the serialized format must match the one used in the IPEndPoint
 * and IrDAEndPoint classes.
 */
static int SerializedToCombined(unsigned char *buf, ILInt32 len,
								CombinedSockAddr *addr, int *addrlen)
{
	int af, port, value;

	/* Clear the result */
	ILMemZero(addr, sizeof(CombinedSockAddr));

	/* Get the address family, which is stored in little-endian order */
	if(len < 2)
	{
		return 0;
	}
	af = ((((int)(buf[1])) << 8) | ((int)(buf[0])));

	/* Determine how to convert the buffer based on the address family */
	if(af == IL_AF_INET)
	{
		if(len < 8)
		{
			return 0;
		}
		addr->ipv4_addr.sin_family = AF_INET;
		port = ((((int)(buf[2])) << 8) | ((int)(buf[3])));
		addr->ipv4_addr.sin_port = htons((unsigned short)port);
		value = ((((long)(buf[4])) << 24) |
		         (((long)(buf[5])) << 16) |
		         (((long)(buf[6])) << 8) |
		          ((long)(buf[7])));
		addr->ipv4_addr.sin_addr.s_addr = htonl((long)value);
		*addrlen = sizeof(struct sockaddr_in);
		return 1;
	}
#ifdef IL_IPV6_PRESENT
	else if(af == IL_AF_INET6)
	{
		if(len < 28)
		{
			return 0;
		}
		addr->ipv6_addr.sin6_family = AF_INET6;
		port = ((((int)(buf[2])) << 8) | ((int)(buf[3])));
		addr->ipv6_addr.sin6_port = htons((unsigned short)port);
		value = ((((long)(buf[4])) << 24) |
		         (((long)(buf[5])) << 16) |
		         (((long)(buf[6])) << 8) |
		          ((long)(buf[7])));
		addr->ipv6_addr.sin6_flowinfo = htonl((long)value);
		ILMemCpy(&(addr->ipv6_addr.sin6_addr), buf + 8, 16);
		value = ((((long)(buf[24])) << 24) |
		         (((long)(buf[25])) << 16) |
		         (((long)(buf[26])) << 8) |
		          ((long)(buf[27])));
#if HAVE_SIN6_SCOPE_ID
		addr->ipv6_addr.sin6_scope_id = htonl((long)value);
#endif
		*addrlen = sizeof(struct sockaddr_in6);
		return 1;
	}
#endif
#ifdef IL_IRDA_PRESENT
	else if(af == IL_AF_IRDA)
	{
		if(len < 32)
		{
			return 0;
		}
#if IL_WIN32_PLATFORM
		addr->irda_addr.irdaAddressFamily = AF_IRDA;
		addr->irda_addr.irdaDeviceID[0] = buf[2];
		addr->irda_addr.irdaDeviceID[1] = buf[3];
		addr->irda_addr.irdaDeviceID[2] = buf[4];
		addr->irda_addr.irdaDeviceID[3] = buf[5];
		ILMemCpy(addr->irda_addr.irdaServiceName, buf + 6, 24);
#else
		addr->irda_addr.sir_family = AF_IRDA;
		addr->irda_addr.sir_lsap_sel = LSAP_ANY;
		value = ((((long)(buf[2])) << 24) |
		         (((long)(buf[3])) << 16) |
		         (((long)(buf[4])) << 8) |
		          ((long)(buf[5])));

		addr->irda_addr.sir_addr = htonl((long)value);
		ILMemCpy(addr->irda_addr.sir_name, buf + 6, 24);
#endif
		*addrlen = sizeof(struct sockaddr_irda);
		return 1;
	}
#endif

	/* If we get here, then the address cannot be converted */
	return 0;
}

/*
 * Convert a combined socket address into its serialized form.
 * Returns zero if the buffer isn't big enough.
 */
static int CombinedToSerialized(unsigned char *buf, ILInt32 len,
								CombinedSockAddr *addr)
{
	int af, port, value;

	/* Clear the result */
	ILMemZero(buf, len);

	/* Map and store the address family, in little-endian order */
	if(len < 2)
	{
		return 0;
	}
	af = addr->addr.sa_family;
	if(af == AF_INET)
	{
		af = IL_AF_INET;
	}
#ifdef IL_IPV6_PRESENT
	else if(af == AF_INET6)
	{
		af = IL_AF_INET6;
	}
#endif
#ifdef IL_IRDA_PRESENT
	else if(af == AF_IRDA)
	{
		af = IL_AF_IRDA;
	}
#endif
	buf[0] = (unsigned char)af;
	buf[1] = (unsigned char)(af >> 8);

	/* Determine how to convert the address based on the address family */
	if(af == IL_AF_INET)
	{
		if(len < 8)
		{
			return 0;
		}
		port = (int)(ntohs(addr->ipv4_addr.sin_port));
		buf[2] = (unsigned char)(port >> 8);
		buf[3] = (unsigned char)port;
		value = (long)(ntohl(addr->ipv4_addr.sin_addr.s_addr));
		buf[4] = (unsigned char)(value >> 24);
		buf[5] = (unsigned char)(value >> 16);
		buf[6] = (unsigned char)(value >> 8);
		buf[7] = (unsigned char)value;
		return 1;
	}
#ifdef IL_IPV6_PRESENT
	else if(af == IL_AF_INET6)
	{
		if(len < 28)
		{
			return 0;
		}
		port = (int)(ntohs(addr->ipv6_addr.sin6_port));
		buf[2] = (unsigned char)(port >> 8);
		buf[3] = (unsigned char)port;
		value = (long)(ntohl(addr->ipv6_addr.sin6_flowinfo));
		buf[4] = (unsigned char)(value >> 24);
		buf[5] = (unsigned char)(value >> 16);
		buf[6] = (unsigned char)(value >> 8);
		buf[7] = (unsigned char)value;
		ILMemCpy(buf + 8, &(addr->ipv6_addr.sin6_addr), 16);
#if HAVE_SIN6_SCOPE_ID
		value = (long)(ntohl(addr->ipv6_addr.sin6_scope_id));
#else
		value = 0;
#endif
		buf[24] = (unsigned char)(value >> 24);
		buf[25] = (unsigned char)(value >> 16);
		buf[26] = (unsigned char)(value >> 8);
		buf[27] = (unsigned char)value;
		return 1;
	}
#endif
#ifdef IL_IRDA_PRESENT
	else if(af == IL_AF_IRDA)
	{
		if(len < 32)
		{
			return 0;
		}
#if IL_WIN32_PLATFORM
		buf[2] = addr->irda_addr.irdaDeviceID[0];
		buf[3] = addr->irda_addr.irdaDeviceID[1];
		buf[4] = addr->irda_addr.irdaDeviceID[2];
		buf[5] = addr->irda_addr.irdaDeviceID[3];
		ILMemCpy(buf + 6, addr->irda_addr.irdaServiceName, 24);
#else
		value = (long)(ntohl(addr->irda_addr.sir_addr));
		buf[2] = (unsigned char)(value >> 24);
		buf[3] = (unsigned char)(value >> 16);
		buf[4] = (unsigned char)(value >> 8);
		buf[5] = (unsigned char)value;
		ILMemCpy(buf + 6, addr->irda_addr.sir_name, 24);
#endif
		return 1;
	}
#endif

	/* If we get here, then the address cannot be converted */
	return 0;
}

int ILSysIOAddressFamilySupported(ILInt32 af)
{
	if(af == IL_AF_INET)
	{
		return 1;
	}
#ifdef IL_IPV6_PRESENT
	if(af == IL_AF_INET6)
	{
		return 1;
	}
#endif
#ifdef IL_IRDA_PRESENT
	if(af == IL_AF_IRDA)
	{
		return 1;
	}
#endif
	return 0;
}

ILSysIOHandle ILSysIOSocket(ILInt32 domain, ILInt32 type, ILInt32 protocol)
{
	int iSocket = 0;

#ifdef IL_WIN32_NATIVE
	_ILWinSockInit();
#endif
	if(domain == IL_AF_INET)
	{
		domain = AF_INET;
	}
#ifdef IL_IPV6_PRESENT
	else if(domain == IL_AF_INET6)
	{
		domain = AF_INET6;
	}
#endif
#ifdef IL_IRDA_PRESENT
	else if(domain == IL_AF_IRDA)
	{
		domain = AF_IRDA;
	}
#endif
	
	switch(type)
	{
		case IL_SOCK_STREAM:	
		{
			type = SOCK_STREAM; 
		}
		break;
		case IL_SOCK_DGRAM:		
		{
			type = SOCK_DGRAM; 
		}
		break;
/* BeOS has incomplete socket support */
#ifdef SOCK_RAW
		case IL_SOCK_RAW:
		{
			type = SOCK_RAW; 
		}
		break;
#endif
#ifdef SOCK_SEQPACKET
		case IL_SOCK_SEQPACKET:	
		{
			type = SOCK_SEQPACKET; 
		}
		break;
#endif
		default:
		{
			type = -1; /* Unknown */
		}
		break;
	}
	
	iSocket = socket(domain, type, protocol);
#ifdef HAVE_FCNTL
	if( iSocket >= 0 ) fcntl( iSocket, F_SETFD, FD_CLOEXEC );
#endif
	return (ILSysIOHandle)(ILNativeInt)(iSocket);
}

int ILSysIOSocketBind(ILSysIOHandle sockfd, unsigned char *addr,
					  ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
	int sa_len;

	/* Convert the socket address into its platform version */
	if(!SerializedToCombined(addr, addrLen, &sa_addr, &sa_len))
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}

	/* Perform the bind operation */
	return (bind((int)(ILNativeInt)sockfd, &sa_addr.addr, sa_len) == 0);
}

int ILSysIOSocketConnect(ILSysIOHandle sockfd, unsigned char *addr,
						 ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
	int sa_len;

	/* Convert the socket address into its platform version */
	if(!SerializedToCombined(addr, addrLen, &sa_addr, &sa_len))
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}

	/* Perform the connect operation */
	return (connect((int)(ILNativeInt)sockfd, &sa_addr.addr, sa_len) == 0);
}

int ILSysIOSocketListen(ILSysIOHandle sockfd, ILInt32 backlog)
{
	return (listen((int)(ILNativeInt)sockfd, backlog) == 0);
}

ILSysIOHandle ILSysIOSocketAccept(ILSysIOHandle sockfd, unsigned char *addr,
								  ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
#ifdef IL_WIN32_NATIVE
	int sa_len;
#else
	socklen_t sa_len;
#endif
	int newfd;

	/* Accept the incoming connection */
	sa_len = sizeof(CombinedSockAddr);
	ILMemZero(&sa_addr, sizeof(sa_addr));
	newfd = accept((int)(ILNativeInt)sockfd, &sa_addr.addr, &sa_len);
	if(newfd < 0)
	{
		return (ILSysIOHandle)(ILNativeInt)newfd;
	}

	/* Convert the platform address into its serialized form */
	if(!CombinedToSerialized(addr, addrLen, &sa_addr))
	{
		close(newfd);
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return (ILSysIOHandle)(ILNativeInt)(-1);
	}

  	/* Return the file descriptor to the caller */
	return (ILSysIOHandle)(ILNativeInt)newfd;
}

ILInt32 ILSysIOSocketReceive(ILSysIOHandle sockfd, void *buff,
						     ILInt32 len, ILInt32 flags)
{
	return (ILInt32)(recv((int)(ILNativeInt)sockfd, buff, len, flags));
}

ILInt32 ILSysIOSocketSend(ILSysIOHandle sockfd, const void *msg,
					      ILInt32 len, ILInt32 flags)
{
#ifdef MSG_NOSIGNAL
	return (ILInt32)(send((int)(ILNativeInt)sockfd, msg, len,
							flags | MSG_NOSIGNAL));
#else
	return (ILInt32)(send((int)(ILNativeInt)sockfd, msg, len, flags));
#endif
}

ILInt32 ILSysIOSocketSendTo(ILSysIOHandle sockfd, const void *msg,
					        ILInt32 len, ILInt32 flags,
							unsigned char *addr, ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
	int sa_len;

	/* Convert the socket address into its platform version */
	if(!SerializedToCombined(addr, addrLen, &sa_addr, &sa_len))
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}

	/* Perform the sendto operation */
#ifdef MSG_NOSIGNAL
	return sendto((int)(ILNativeInt)sockfd, msg, len, flags | MSG_NOSIGNAL,
				  &sa_addr.addr, sa_len);
#else
	return sendto((int)(ILNativeInt)sockfd, msg, len, flags,
				  &sa_addr.addr, sa_len);
#endif
}

ILInt32 ILSysIOSocketRecvFrom(ILSysIOHandle sockfd, void *buf,
							  ILInt32 len, ILInt32 flags,
							  unsigned char *addr, ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
#ifdef IL_WIN32_NATIVE
	int sa_len;
#else
	socklen_t sa_len;
#endif
	int result;

	/* Receive the incoming data */
	sa_len = sizeof(CombinedSockAddr);
	ILMemZero(&sa_addr, sizeof(sa_addr));
	result = recvfrom((int)(ILNativeInt)sockfd, buf, len, flags,
					  &sa_addr.addr, &sa_len);
	if(result < 0)
	{
		return (ILInt32)result;
	}

	/* Convert the platform address into its serialized form */
	if(!CombinedToSerialized(addr, addrLen, &sa_addr))
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return (ILInt32)(-1);
	}
  
  	/* Return the receive result to the caller */
	return (ILInt32)result;
}

int ILSysIOSocketClose(ILSysIOHandle sockfd)
{
	return (close((int)(ILNativeInt)sockfd) == 0);
}

int ILSysIOSocketShutdown(ILSysIOHandle sockfd, ILInt32 how)
{
	return (shutdown((int)(ILNativeInt)sockfd, how) == 0);
}

ILInt32 ILSysIOSocketSelect(ILSysIOHandle **readfds, ILInt32 numRead,
						    ILSysIOHandle **writefds, ILInt32 numWrite,
						    ILSysIOHandle **exceptfds, ILInt32 numExcept,
						    ILInt64 timeout)
{
	fd_set readSet, writeSet, exceptSet;
	fd_set *readPtr, *writePtr, *exceptPtr;
	int highest = -1;
	int fd, result;
	ILCurrTime currtime;
	ILCurrTime endtime;
	struct timeval difftime;
	ILInt32 index;

	/* Convert the read array into an "fd_set" */
	FD_ZERO(&readSet);
	if(readfds)
	{
		readPtr = &readSet;
		for(index = 0; index < numRead; ++index)
		{
			fd = (int)(ILNativeInt)(readfds[index]);
			if(fd != -1)
			{
				FD_SET(fd, &readSet);
				if(fd > highest)
				{
					highest = fd;
				}
			}
		}
	}
	else
	{
		readPtr = 0;
	}

	/* Convert the write array into an "fd_set" */
	FD_ZERO(&writeSet);
	if(writefds)
	{
		writePtr = &writeSet;
		for(index = 0; index < numWrite; ++index)
		{
			fd = (int)(ILNativeInt)(writefds[index]);
			if(fd != -1)
			{
				FD_SET(fd, &writeSet);
				if(fd > highest)
				{
					highest = fd;
				}
			}
		}
	}
	else
	{
		writePtr = 0;
	}

	/* Convert the except array into an "fd_set" */
	FD_ZERO(&exceptSet);
	if(exceptfds)
	{
		exceptPtr = &exceptSet;
		for(index = 0; index < numExcept; ++index)
		{
			fd = (int)(ILNativeInt)(exceptfds[index]);
			if(fd != -1)
			{
				FD_SET(fd, &exceptSet);
				if(fd > highest)
				{
					highest = fd;
				}
			}
		}
	}
	else
	{
		exceptPtr = 0;
	}

	/* Is this a timed select or an infinite select? */
	if(timeout >= 0)
	{
		/* Get the current time of day and determine the end time */
		ILGetCurrTime(&currtime);
		endtime.secs = currtime.secs + (long)(timeout / (ILInt64)1000000);
		endtime.nsecs = currtime.nsecs +
			(long)((timeout % (ILInt64)1000000) * (ILInt64)1000);
		if(endtime.nsecs >= 1000000000L)
		{
			++(endtime.secs);
			endtime.nsecs -= 1000000000L;
		}

		/* Loop while we are interrupted by signals */
		for(;;)
		{
			/* How long until the timeout? */
			difftime.tv_sec = (time_t)(endtime.secs - currtime.secs);
			if(endtime.nsecs >= currtime.nsecs)
			{
				difftime.tv_usec =
					(long)((endtime.nsecs - currtime.nsecs) / 1000);
			}
			else
			{
				difftime.tv_usec =
					(endtime.nsecs + 1000000000L - currtime.nsecs) / 1000;
				difftime.tv_sec -= 1;
			}

			/* Perform a trial select, which may be interrupted */
			result = select(highest + 1, readPtr, writePtr,
							exceptPtr, &difftime);

			if(result >= 0 || errno != EINTR)
			{
				break;
			}

			/* We were interrupted, so get the current time again.
			   We have to this because many systems do not update
			   the 5th paramter of "select" to reflect how much time
			   is left to go */
			ILGetCurrTime(&currtime);

			/* Are we past the end time? */
			if(currtime.secs > endtime.secs ||
			   (currtime.secs == endtime.secs &&
			    currtime.nsecs >= endtime.nsecs))
			{
				/* We are, so simulate timeout */
				result = 0;
				break;
			}
		}
	}
	else
	{
		/* Infinite select */
		while((result = select(highest + 1, readPtr, writePtr, exceptPtr,
							   (struct timeval *)0)) < 0)
		{
			/* Keep looping while we are being interrupted by signals */
			if(errno != EINTR)
			{
				break;
			}
		}
	}

	/* Update the descriptor sets if something fired */
	if(result > 0)
	{
		/* Update the read descriptors */
		if(readPtr)
		{
			for(index = 0; index < numRead; ++index)
			{
				fd = (int)(ILNativeInt)(readfds[index]);
				if(fd != -1 && !FD_ISSET(fd, &readSet))
				{
					readfds[index] = ILSysIOHandle_Invalid;
				}
			}
		}

		/* Update the write descriptors */
		if(writePtr)
		{
			for(index = 0; index < numWrite; ++index)
			{
				fd = (int)(ILNativeInt)(writefds[index]);
				if(fd != -1 && !FD_ISSET(fd, &writeSet))
				{
					writefds[index] = ILSysIOHandle_Invalid;
				}
			}
		}

		/* Update the except descriptors */
		if(exceptPtr)
		{
			for(index = 0; index < numExcept; ++index)
			{
				fd = (int)(ILNativeInt)(exceptfds[index]);
				if(fd != -1 && !FD_ISSET(fd, &exceptSet))
				{
					exceptfds[index] = ILSysIOHandle_Invalid;
				}
			}
		}
	}

	/* Return the result to the caller */
	return (ILInt32)result;
}

int ILSysIOSocketSetBlocking(ILSysIOHandle sockfd, int flag)
{
#if defined(FIONBIO) && defined(HAVE_IOCTL)
	return (ioctl((int)(ILNativeInt)sockfd, FIONBIO, (void *)&flag) >= 0);
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

ILInt32 ILSysIOSocketGetAvailable(ILSysIOHandle sockfd)
{
#if defined(FIONREAD) && defined(HAVE_IOCTL)
	int result = 0;
	if(ioctl((int)(ILNativeInt)sockfd, FIONREAD, (void *)&result) >= 0)
	{
		return (ILInt32)result;
	}
	else
	{
		return -1;
	}
#else
	return 0;
#endif
}

int ILSysIOSocketGetName(ILSysIOHandle sockfd, unsigned char *addr,
						 ILInt32 addrLen)
{
	CombinedSockAddr sa_addr;
#ifdef IL_WIN32_NATIVE
	int sa_len;
#else
	socklen_t sa_len;
#endif

	/* Accept the incoming connection */
	sa_len = sizeof(CombinedSockAddr);
	ILMemZero(&sa_addr, sizeof(sa_addr));
	if(getsockname((int)(ILNativeInt)sockfd, &sa_addr.addr, &sa_len) < 0)
	{
		return 0;
	}

	/* Convert the platform address into its serialized form */
	if(!CombinedToSerialized(addr, addrLen, &sa_addr))
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}
	return 1;
}

/* Convert the IL options to the corresponding system values 
 * Note: Modify to port to new platforms 
 * */
static int SocketOptionsToNative(ILInt32 level, ILInt32 name, 
							ILInt32 *nativeLevel, ILInt32 *nativeName)
{
	switch(level)
	{
		case (IL_SOL_IP):
		{
#ifdef SOL_IP
			(*nativeLevel)=SOL_IP;
			switch(name)
			{
				case IL_SO_ADD_MEMBERSHIP:
				{
					#ifdef SO_ATTACH_FILTER
						(*nativeName) = SO_ATTACH_FILTER;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_DROP_MEMBERSHIP:
				{
					#ifdef SO_DETACH_FILTER
						(*nativeName) = SO_DETACH_FILTER;
					#else
						return 0;
					#endif
				}
				break;

				default:
					return 0;
			}
#else
			return 0;
#endif
		}
		break;
		
		case (IL_SOL_TCP):
		{
#ifdef SOL_TCP
			(*nativeLevel)=SOL_TCP;
			switch(name)
			{
				case IL_SO_NO_DELAY:
				{
				#ifdef TCP_NODELAY
					(*nativeName) = TCP_NODELAY;
				#else
					return 0;
				#endif
				}
				break;
				
				case IL_SO_EXPEDITED:
				{
					/* TODO */
					return 0;
				}
				break;
				default:
					return 0;
			}
#else
			#warning "SOL_TCP not available"
			return 0;
#endif
		}
		break;
		
		case (IL_SOL_UDP):
		{
#ifdef SOL_UDP
			(*nativeLevel)=SOL_UDP;
			/* TODO */
			return 0;
#else
			#warning "SOL_UDP not available"
			return 0;
#endif
		}
		break;
		
		case (IL_SOL_SOCKET):
		{
#ifdef SOL_SOCKET
			(*nativeLevel)=SOL_SOCKET;
			switch(name)
			{
				case IL_SO_REUSE_ADDRESS:
				{
					#ifdef SO_REUSEADDR
						(*nativeName) = SO_REUSEADDR;
					#else
						return 0;
					#endif
				}
				break;
					
				case IL_SO_KEEP_ALIVE:
				{
					#ifdef SO_KEEPALIVE
						(*nativeName) = SO_KEEPALIVE;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_SEND_BUFFER:
				{
					#ifdef SO_SNDBUF
						(*nativeName) = SO_SNDBUF;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_RECV_BUFFER:
				{
					#ifdef SO_RCVBUF
						(*nativeName) = SO_RCVBUF;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_SEND_TIMEOUT:
				{
					#ifdef SO_SNDTIMEO
						(*nativeName) = SO_SNDTIMEO;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_RECV_TIMEOUT:
				{
					#ifdef SO_RCVTIMEO
						(*nativeName) = SO_RCVTIMEO;
					#else
						return 0;
					#endif
				}
				break;
				
				case IL_SO_BROADCAST:
				{
					#ifdef SO_BROADCAST
						(*nativeName) = SO_BROADCAST;
					#else
						return 0;
					#endif
				}
				break;
				
				default:
					return 0;
			}
#else
			#warning "SOL_SOCKET is not defined"
			return 0;
#endif
		}
		break;
		
		default:
			return 0;
	}
	return 1;
}

int ILSysIOSocketSetOption(ILSysIOHandle sockfd, ILInt32 level,
						   ILInt32 name, ILInt32 value)
{
#ifdef HAVE_SETSOCKOPT
	ILInt32 nativeLevel, nativeName;
	int optlen;
#ifdef IL_WIN32_NATIVE
	ILInt32 optval;
#else
	union
	{
		ILInt32 value;
		struct timeval timeout;
	} optval;
#endif

	if(SocketOptionsToNative(level, name, &nativeLevel,&nativeName)==0)
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}
#ifdef IL_WIN32_NATIVE
	optlen = sizeof(optval);
	optval = value;
#else
	if(name == IL_SO_SEND_TIMEOUT || name == IL_SO_RECV_TIMEOUT)
	{
		optlen = sizeof(optval.timeout);
		optval.timeout.tv_sec = value / 1000;
		optval.timeout.tv_usec = (value % 1000) * 1000;
	}
	else
	{
		optlen = sizeof(optval.value);
		optval.value = value;
	}
#endif	
	return (setsockopt((int)(ILNativeInt)sockfd,nativeLevel,nativeName,
			(void *)&optval, optlen)== 0);
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

int ILSysIOSocketGetOption(ILSysIOHandle sockfd, ILInt32 level,
						   ILInt32 name, ILInt32 *value)
{
#ifdef HAVE_GETSOCKOPT
	ILInt32 nativeLevel, nativeName;
#ifdef IL_WIN32_NATIVE
	int optlen;
	ILInt32 optval;
#else
	socklen_t optlen;
	union
	{
		ILInt32 value;
		struct timeval timeout;
	} optval;
#endif

	if(SocketOptionsToNative(level, name, &nativeLevel,&nativeName)==0)
	{
		ILSysIOSetErrno(IL_ERRNO_EINVAL);
		return 0;
	}

#ifdef IL_WIN32_NATIVE
	optlen = sizeof(optval);
#else
	if(name == IL_SO_SEND_TIMEOUT || name == IL_SO_RECV_TIMEOUT)
	{
		optlen = sizeof(optval.timeout);
	}
	else
	{
		optlen = sizeof(optval.value);
	}
#endif

	if(getsockopt((int)(ILNativeInt)sockfd,nativeLevel,nativeName,
								(void *)&optval,&optlen) != 0)
	{
		return 0;
	}

#ifdef IL_WIN32_NATIVE
	*value = optval;
#else
	if(name == IL_SO_SEND_TIMEOUT || name == IL_SO_RECV_TIMEOUT)
	{
		*value = (ILInt32)(optval.timeout.tv_sec * 1000 +
						   optval.timeout.tv_usec / 1000);
	}
	else
	{
		*value = optval.value;
	}
#endif

	return 1;
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

int ILSysIOSocketSetLinger(ILSysIOHandle handle, int enabled, int seconds)
{
#if defined(HAVE_SETSOCKOPT) && defined(SO_LINGER)
	struct linger _linger;
	_linger.l_onoff=enabled;
	_linger.l_linger=seconds;
	if(setsockopt((int)(ILNativeInt)handle, SOL_SOCKET, SO_LINGER,
						(void *)&(_linger), sizeof(struct linger)) != 0)
	{
		return 0;
	}
	return 1;
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

int ILSysIOSocketGetLinger(ILSysIOHandle handle, int *enabled, int *seconds)
{
#if defined(HAVE_SETSOCKOPT) && defined(SO_LINGER)
	struct linger _linger;
#ifdef IL_WIN32_NATIVE
	int size;
#else
	socklen_t size;
#endif
	size=sizeof(struct linger);
	if(getsockopt((int)(ILNativeInt)handle, SOL_SOCKET, SO_LINGER,
				  (void *)&(_linger), &size) != 0)
	{
		return 0;
	}
	*enabled=_linger.l_onoff;
	*seconds=_linger.l_linger;
	return 1;
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

int ILSysIOSocketSetMulticast(ILSysIOHandle handle, ILInt32 af, ILInt32 name,
							  unsigned char *group, ILInt32 groupLen,
							  unsigned char *mcint, ILInt32 mcintLen)
{
	/* TODO */
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
}

int ILSysIOSocketGetMulticast(ILSysIOHandle handle, ILInt32 af, ILInt32 name,
							  unsigned char *group, ILInt32 groupLen,
							  unsigned char *mcint, ILInt32 mcintLen)
{
	/* TODO */
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
}

int ILSysIODiscoverIrDADevices(ILSysIOHandle handle, unsigned char *buf,
							   ILInt32 bufLen)
{
#if defined(HAVE_GETSOCKOPT) && defined(IL_WIN32_PLATFORM)
	/*
	 * Note: we must compile with --disable-cygwin to make IrDA working on
	 * windows, because cygwin seems to be blocking irda socket options.
	 */
	if(getsockopt((int)(ILNativeInt)handle, SOL_IRLMP, IRLMP_ENUMDEVICES,
				  buf, &bufLen) != 0)
	{
		return 0;
	}
	return 1;
#elif defined(HAVE_GETSOCKOPT) && defined(IRLMP_ENUMDEVICES)
	char *nativeBuf;
#ifdef IL_WIN32_NATIVE
	int nativeLen;
#else
	socklen_t nativeLen;
#endif
	struct irda_device_list *list;
	int i;
	int index;

	/* Compute the option length for unix. Given option length is
	   4 bytes for device count and 32 bytes for each device info */
	nativeLen = sizeof(struct irda_device_list) +
				sizeof(struct irda_device_info) * ((bufLen - 4) / 32);

	nativeBuf = (void *) ILMalloc(nativeLen);
	if(nativeBuf == 0)
	{
		ILSysIOSetErrno(IL_ERRNO_ENOMEM);
		return 0;
	}

	/* Perform a discovery and get device list */
	if(getsockopt((int)(ILNativeInt)handle, SOL_IRLMP, IRLMP_ENUMDEVICES,
											nativeBuf, &nativeLen) != 0)
	{
		ILFree(nativeBuf);
		return 0;
	}

	/* Convert list to buf that is used to construct IrDADeviceInfo */
	list = (struct irda_device_list *) nativeBuf;

	ILMemCpy((void *)(buf), (const void *)(&(list->len)), 4);
	index = 4;

	for(i = 0; i < list->len; ++i)
	{
		ILMemCpy((void *)(buf + index),
									(const void *)(&(list->dev[i].daddr)), 4);
		index += 4;
		ILMemCpy((void *)(buf + index), (const void *)(list->dev[i].info), 22);
		index += 22;
		ILMemCpy((void *)(buf + index), (const void *)(list->dev[i].hints), 2);
		index += 2;
		buf[index] = list->dev[i].charset;
		index += 4;
	}

	ILFree(nativeBuf);
	return 1;
#else
	ILSysIOSetErrno(IL_ERRNO_EINVAL);
	return 0;
#endif
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_CONFIG_NETWORKING */
