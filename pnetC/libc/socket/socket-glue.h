/*
 * socket-glue.h - Internal glue routines for socket support.
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

#ifndef	_SOCKET_GLUE_H
#define	_SOCKET_GLUE_H

#include <sys/socket.h>
#include <netinet/in.h>
#include <errno.h>
#include <csharp.h>

using System::Net::Dns;
using System::Net::EndPoint;
using System::Net::IPAddress;
using System::Net::IPEndPoint;
using System::Net::IPHostEntry;
using System::Net::Sockets::Socket;
using System::Net::Sockets::SocketException;
using System::Net::Sockets::SocketShutdown;
using System::Runtime::InteropServices::Marshal;

int __syscall_socket (int domain, int type);

int __syscall_wrap_accept (Socket *socket);

EndPoint *__create_ipv4_endpoint (uint32_t address, int port);
EndPoint *__create_ipv6_endpoint (long address, uint32_t scope, int port);

int __decode_ipv4_endpoint (EndPoint *ep, uint32_t &address, int &port);
int __decode_ipv6_endpoint (EndPoint *ep, long address,
                            uint32_t &scope, int &port);

int __syscall_socket_family (int fd);

int __sockaddr_to_endpoint (int fd, EndPoint &ep,
                            const struct sockaddr *addr, socklen_t addrlen);
int __endpoint_to_sockaddr (int fd, EndPoint *ep,
                            struct sockaddr *addr, socklen_t *addrlen);

IPAddress *__inaddr_to_ipaddress (const void *addr, socklen_t len, int type);

Socket *__syscall_get_socket (int fd, int *error);
#define	syscall_get_socket(fd)	(__syscall_get_socket ((fd), &errno))

int __syscall_is_listening (int fd);
int __syscall_set_listening (int fd, int value);

int __syscall_is_connected (int fd);
int __syscall_set_connected (int fd, int value);

void __syscall_convert_hostent (IPHostEntry *input, long output);

int __netdb_name_match (const char *value, char *name, char **aliases);

#endif /* _SOCKET_GLUE_H */
