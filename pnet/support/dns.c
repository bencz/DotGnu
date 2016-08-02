/*
 * dns.c - Routines for processing DNS queries
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
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

#include "il_system.h"
#include "il_utils.h"
#ifdef IL_WIN32_NATIVE
#include <winsock.h>
#define	HAVE_GETHOSTBYNAME	1
#define	HAVE_GETHOSTBYADDR	1
#else
#if HAVE_STDLIB_H
#include <stdlib.h>
#endif
#if HAVE_NETDB_H
#include <netdb.h>
#endif 
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#endif 

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_WIN32_NATIVE

void _ILWinSockInit(void);

#endif

struct hostent* ILGetHostByName(const char *name)
{
#ifdef HAVE_GETHOSTBYNAME
#ifdef IL_WIN32_NATIVE
	_ILWinSockInit();
#endif
	return gethostbyname(name);
#else
	return NULL;
#endif

}

struct hostent* ILGetHostByAddr(const void* addr, unsigned int len,int type)
{
#ifdef HAVE_GETHOSTBYADDR
#ifdef IL_WIN32_NATIVE
	_ILWinSockInit();
#endif
	return gethostbyaddr(addr,len,type);
#else
	return NULL;
#endif
}
#ifdef	__cplusplus
};
#endif

int ILGetHostName(char * name, unsigned int size)
{
#ifdef HAVE_GETHOSTNAME
#ifdef IL_WIN32_NATIVE
	_ILWinSockInit();
#endif
	return gethostname(name, size);
#else
	return -1;
#endif
}
