/*
 * lib_socket.c - Internalcall methods for "Platform.SocketMethods".
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

#include "engine.h"
#include "lib_defs.h"
#include "il_sysio.h"
#include "il_errno.h"
#ifdef IL_WIN32_NATIVE
	#include <winsock.h>
#else
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef HAVE_SYS_SOCKET_H
#include <sys/socket.h>
#endif
#ifdef HAVE_NETINET_IN_H
#include <netinet/in.h>
#endif
#ifdef HAVE_NETDB_H
	#include <netdb.h>
#endif
#endif

#ifdef IL_CONFIG_NETWORKING

typedef union
{
	ILUInt8		uint8Values[8];
	ILInt16		int16Value;
	ILInt32		int32Value;
	ILInt64		int64Value;
	
} _ILSocketConversionHelper;

/*
 * public static IntPtr GetInvalidHandle();
 */
ILNativeInt _IL_SocketMethods_GetInvalidHandle(ILExecThread *_thread)
{
	return _IL_FileMethods_GetInvalidHandle(_thread);
}

/*
 * public static bool AddressFamilySupported(int af);
 */
ILBool _IL_SocketMethods_AddressFamilySupported(ILExecThread *_thread,
												ILInt32 af)
{
	return (ILSysIOAddressFamilySupported(af) != 0);
}

/*
 * public static bool Create(int af, int st, int pt, out IntPtr handle);
 */
ILBool _IL_SocketMethods_Create(ILExecThread *_thread, ILInt32 af,
							    ILInt32 st, ILInt32 pt,
								ILNativeInt *handle)
{
	*handle = (ILNativeInt)(ILSysIOSocket(af, st, pt));
	return (*handle != (ILNativeInt)ILSysIOHandle_Invalid);
}

/*
 * public static bool Bind(IntPtr handle, byte[] addr);
 */
ILBool _IL_SocketMethods_Bind(ILExecThread *_thread, ILNativeInt handle,
							  System_Array *addr)
{
	return (ILBool)(ILSysIOSocketBind
				((ILSysIOHandle)handle,
				 (unsigned char *)ArrayToBuffer(addr),
				 ArrayLength(addr)));
}

/*
 * public static bool Shutdown(IntPtr handle, int how);
 */
ILBool _IL_SocketMethods_Shutdown(ILExecThread * _thread,
								  ILNativeInt handle, ILInt32 how)
{
	return (ILBool)(ILSysIOSocketShutdown((ILSysIOHandle)handle, how));
}

/*
 * public static bool Listen(IntPtr handle, int backlog);
 */
ILBool _IL_SocketMethods_Listen(ILExecThread *_thread,
								ILNativeInt handle, ILInt32 backlog)
{
	return (ILBool)(ILSysIOSocketListen((ILSysIOHandle)handle, backlog));
}

/*
 * public static bool Accept(IntPtr handle, byte[] addrReturn,
 *                           out IntPtr newHandle);
 */
ILBool _IL_SocketMethods_Accept(ILExecThread *_thread, ILNativeInt handle,
								System_Array *addrReturn,
								ILNativeInt *newHandle)
{
	*newHandle = (ILNativeInt)(ILSysIOSocketAccept
					((ILSysIOHandle)handle,
				     (unsigned char *)ArrayToBuffer(addrReturn),
				     ArrayLength(addrReturn)));

	return (*newHandle != (ILNativeInt)ILSysIOHandle_Invalid);
}

/*
 * public static bool Connect(IntPtr handle, byte[] addr);
 */
ILBool _IL_SocketMethods_Connect(ILExecThread *_thread, ILNativeInt handle,
								 System_Array *addr)
{
	return (ILBool)(ILSysIOSocketConnect
				((ILSysIOHandle)handle,
				 (unsigned char *)ArrayToBuffer(addr),
				 ArrayLength(addr)));
}

/*
 * public static int Receive(IntPtr handle, byte[] buffer, int offset,
 *							 int size, int flags);
 */
ILInt32 _IL_SocketMethods_Receive(ILExecThread *_thread, ILNativeInt handle,
								  System_Array *buffer, ILInt32 offset,
								  ILInt32 size, ILInt32 flags)
{
	return ILSysIOSocketReceive((ILSysIOHandle)handle,
								((ILUInt8 *)(ArrayToBuffer(buffer))) + offset,
								size, flags);
}

/*
 * public static int ReceiveFrom(IntPtr handle, byte[] buffer,
 *								 int offset, int size, int flags,
 *								 byte[] addrReturn);
 */
ILInt32 _IL_SocketMethods_ReceiveFrom(ILExecThread *_thread,
									  ILNativeInt handle,
									  System_Array *buffer,
									  ILInt32 offset, ILInt32 size,
									  ILInt32 flags,
									  System_Array *addrReturn)
{
	return ILSysIOSocketRecvFrom
				((ILSysIOHandle)handle,
			     ((ILUInt8 *)(ArrayToBuffer(buffer))) + offset,
				 size, flags,
				 (unsigned char *)ArrayToBuffer(addrReturn),
				 ArrayLength(addrReturn));
}

/*
 * public static int Send(IntPtr handle, byte[] buffer, int offset,
 *						  int size, int flags);
 */
ILInt32 _IL_SocketMethods_Send(ILExecThread *_thread, ILNativeInt handle,
							   System_Array *buffer, ILInt32 offset,
							   ILInt32 size, ILInt32 flags)
{
	return ILSysIOSocketSend((ILSysIOHandle)handle,
							 ((ILUInt8 *)(ArrayToBuffer(buffer))) + offset,
							 size, flags);
}

/*
 * public static int SendTo(IntPtr handle, byte[] buffer,
 *							int offset, int size, int flags,
 *							byte[] addr)
 */
ILInt32 _IL_SocketMethods_SendTo(ILExecThread *_thread, ILNativeInt handle,
								 System_Array *buffer, ILInt32 offset,
								 ILInt32 size, ILInt32 flags,
								 System_Array *addr)
{
	return ILSysIOSocketSendTo
				((ILSysIOHandle)handle,
			     ((ILUInt8 *)(ArrayToBuffer(buffer))) + offset,
				 size, flags,
				 (unsigned char *)ArrayToBuffer(addr),
				 ArrayLength(addr));
}

ILBool _IL_SocketMethods_Close(ILExecThread *_thread, ILNativeInt handle)
{
	return (ILBool)(ILSysIOSocketClose((ILSysIOHandle)handle));
}

/*
 * public static int Select(IntPtr[] readarray, IntPtr[] writearray,
 *                          IntPtr[] errorarray, long timeout);
 */
ILInt32 _IL_SocketMethods_Select(ILExecThread *_thread,
								 System_Array *readarray,
								 System_Array *writearray,
								 System_Array *errorarray,
								 ILInt64 timeout)
{
	return ILSysIOSocketSelect
		((readarray ? (ILSysIOHandle **)(ArrayToBuffer(readarray)) : 0),
		 (readarray ? ArrayLength(readarray) : 0),
		 (writearray ? (ILSysIOHandle **)(ArrayToBuffer(writearray)) : 0),
		 (writearray ? ArrayLength(writearray) : 0),
		 (errorarray ? (ILSysIOHandle **)(ArrayToBuffer(errorarray)) : 0),
		 (errorarray ? ArrayLength(errorarray) : 0), timeout);
}

/*
 * public static bool SetBlocking(IntPtr handle, bool blocking);
 */
ILBool _IL_SocketMethods_SetBlocking(ILExecThread *_thread,
									 ILNativeInt handle, ILBool blocking)
{
	return (ILSysIOSocketSetBlocking
				((ILSysIOHandle)handle, (int)blocking) != 0);
}

/*
 * public static int GetAvailable(IntPtr handle);
 */
ILInt32 _IL_SocketMethods_GetAvailable(ILExecThread *_thread,
									   ILNativeInt handle)
{
	return ILSysIOSocketGetAvailable((ILSysIOHandle)handle);
}

/*
 * public static bool GetSockName(IntPtr handle, byte[] addrReturn);
 */
ILBool _IL_SocketMethods_GetSockName(ILExecThread * _thread,
									 ILNativeInt handle,
									 System_Array *addrReturn)
{
	return (ILBool)ILSysIOSocketGetName
			((ILSysIOHandle)handle,
			 (unsigned char *)ArrayToBuffer(addrReturn),
			 ArrayLength(addrReturn));
}

/*
 * public static bool SetSocketOption(IntPtr handle, int level,
 *									  int name, int value);
 */
ILBool _IL_SocketMethods_SetSocketOption(ILExecThread *_thread,
										 ILNativeInt handle,
										 ILInt32 level,
										 ILInt32 name,
										 ILInt32 value)
{
	return (ILSysIOSocketSetOption((ILSysIOHandle)handle,
								   level, name, value) != 0);
}

/*
 * public static bool GetSocketOption(IntPtr handle, int level,
 *									  int name, out int value);
 */
ILBool _IL_SocketMethods_GetSocketOption(ILExecThread *_thread,
										 ILNativeInt handle,
										 ILInt32 level,
										 ILInt32 name,
										 ILInt32 *value)
{
	*value = 0;
	return (ILSysIOSocketGetOption((ILSysIOHandle)handle,
								   level, name, value) != 0);
}

/*
 * public static bool SetLingerOption(IntPtr handle, bool enabled,
 *									  int seconds);
 */
ILBool _IL_SocketMethods_SetLingerOption(ILExecThread *_thread,
										 ILNativeInt handle,
										 ILBool enabled,
										 ILInt32 seconds)
{
	return (ILSysIOSocketSetLinger((ILSysIOHandle)handle,
								   (enabled != 0), (int)seconds) != 0);
}

/*
 * public static bool GetLingerOption(IntPtr handle, out bool enabled,
 *									  out int seconds);
 */
extern ILBool _IL_SocketMethods_GetLingerOption(ILExecThread *_thread,
												ILNativeInt handle,
												ILBool *enabled,
												ILInt32 *seconds)
{
	int enab, secs;
	if(ILSysIOSocketGetLinger((ILSysIOHandle)handle, &enab, &secs))
	{
		*enabled = (enab != 0);
		*seconds = (ILInt32)secs;
		return 1;
	}
	else
	{
		*enabled = 0;
		*seconds = 0;
		return 0;
	}
}

/*
 * public static bool SetMulticastOption(IntPtr handle, int af, int name,
 *									     byte[] group, byte[] mcint);
 */
ILBool _IL_SocketMethods_SetMulticastOption(ILExecThread *_thread,
											ILNativeInt handle,
											ILInt32 af, ILInt32 name,
											System_Array *group,
											System_Array *mcint)
{
	return (ILSysIOSocketSetMulticast
				((ILSysIOHandle)handle, af, name,
				 (unsigned char *)ArrayToBuffer(group), ArrayLength(group),
				 (unsigned char *)ArrayToBuffer(mcint), ArrayLength(mcint)) != 0);
}

/*
 * public static bool GetMulticastOption(IntPtr handle, int af, int name,
 *									     byte[] group, byte[] mcint);
 */
ILBool _IL_SocketMethods_GetMulticastOption(ILExecThread *_thread,
											ILNativeInt handle,
											ILInt32 af, ILInt32 name,
											System_Array *group,
											System_Array *mcint)
{
	return (ILSysIOSocketGetMulticast
				((ILSysIOHandle)handle, af, name,
				 (unsigned char *)ArrayToBuffer(group), ArrayLength(group),
				 (unsigned char *)ArrayToBuffer(mcint), ArrayLength(mcint)) != 0);
}

ILBool _IL_SocketMethods_DiscoverIrDADevices(ILExecThread *_thread,
											 ILNativeInt handle,
											 System_Array *buf)
{
	return (ILSysIODiscoverIrDADevices
				((ILSysIOHandle)handle,
				 (unsigned char *)ArrayToBuffer(buf), ArrayLength(buf)) != 0);
}

/*
 * public static Errno GetErrno();
 */
ILInt32 _IL_SocketMethods_GetErrno(ILExecThread *thread)
{
	return ILSysIOGetErrno();
}

/*
 * public static String GetErrnoMessage(Errno error);
 */
ILString *_IL_SocketMethods_GetErrnoMessage(ILExecThread *thread, ILInt32 error)
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
 * public static bool CanStartThreads();
 */
ILBool _IL_SocketMethods_CanStartThreads(ILExecThread *_thread)
{
	return _IL_Thread_CanStartThreads(_thread);
}

/*
 * public static bool QueueCompletionItem(AsyncCallback callback,
 *										  IAsyncResult state);
 */
ILBool _IL_SocketMethods_QueueCompletionItem(ILExecThread *_thread,
											 ILObject *callback,
											 ILObject *state)
{
	/* This provides backdoor access to "ThreadPool.QueueCompletionItem",
	   which cannot be called directly from C# code due to security checks */
	ILBool result = 0;
	ILExecThreadCallNamed(_thread, "System.Threading.ThreadPool",
						  "QueueCompletionItem",
						  "(oSystem.AsyncCallback;oSystem.IAsyncResult;)Z",
						  &result, callback, state);
	return result;
}

/*
 * public static WaitHandle CreateManualResetEvent();
 */
ILObject *_IL_SocketMethods_CreateManualResetEvent(ILExecThread *_thread)
{
	return ILExecThreadNew(_thread, "System.Threading.ManualResetEvent",
						   "(TZ)V", (ILVaInt)0);
}

/*
 * public static void WaitHandleSet(WaitHandle waitHandle);
 */
void _IL_SocketMethods_WaitHandleSet(ILExecThread *_thread,
									 ILObject *waitHandle)
{
	/* This provides backdoor access to "ManualResetEvent.Set",
	   which cannot be called directly in ECMA_COMPAT mode */
	ILBool result = 0;
	ILExecThreadCallNamed(_thread, "System.Threading.ManualResetEvent",
						  "Set", "(T)Z", &result, waitHandle);
}

/*
 * public static long HostToNetworkOrder(long host);
 */
ILInt64 _IL_IPAddress_HostToNetworkOrder_l(ILExecThread *thread, ILInt64 host)
{
	_ILSocketConversionHelper conv;

	conv.uint8Values[0] = (unsigned char)(host >> 56);
	conv.uint8Values[1] = (unsigned char)(host >> 48);
	conv.uint8Values[2] = (unsigned char)(host >> 40);
	conv.uint8Values[3] = (unsigned char)(host >> 32);
	conv.uint8Values[4] = (unsigned char)(host >> 24);
	conv.uint8Values[5] = (unsigned char)(host >> 16);
	conv.uint8Values[6] = (unsigned char)(host >> 8);
	conv.uint8Values[7] = (unsigned char)host;
	return conv.int64Value;
}

/*
 * public static int HostToNetworkOrder(int host);
 */
ILInt32 _IL_IPAddress_HostToNetworkOrder_i(ILExecThread *thread, ILInt32 host)
{
	_ILSocketConversionHelper conv;
	
	conv.uint8Values[0] = (ILUInt8)(host >> 24);
	conv.uint8Values[1] = (ILUInt8)(host >> 16);
	conv.uint8Values[2] = (ILUInt8)(host >> 8);
	conv.uint8Values[3] = (ILUInt8)host;
	return conv.int32Value;
}

/*
 * public static short HostToNetworkOrder(short host);
 */
ILInt16 _IL_IPAddress_HostToNetworkOrder_s(ILExecThread *thread, ILInt16 host)
{
	_ILSocketConversionHelper conv;
	
	conv.uint8Values[0] = (unsigned char)(host >> 8);
	conv.uint8Values[1] = (unsigned char)host;
	return conv.int16Value;
}

#define	GETBYTE(type,value,offset,shift)	\
			(((type)(((unsigned char *)&(value))[(offset)])) << (shift))

/*
 * public static long NetworkToHostOrder(long host);
 */
ILInt64 _IL_IPAddress_NetworkToHostOrder_l(ILExecThread *thread,
										   ILInt64 network)
{
	return GETBYTE(ILInt64, network, 0, 56) |
		   GETBYTE(ILInt64, network, 1, 48) |
		   GETBYTE(ILInt64, network, 2, 40) |
		   GETBYTE(ILInt64, network, 3, 32) |
		   GETBYTE(ILInt64, network, 4, 24) |
		   GETBYTE(ILInt64, network, 5, 16) |
		   GETBYTE(ILInt64, network, 6,  8) |
		   GETBYTE(ILInt64, network, 7,  0);
}

/*
 * public static int NetworkToHostOrder(int host);
 */
ILInt32 _IL_IPAddress_NetworkToHostOrder_i(ILExecThread *thread,
										   ILInt32 network)
{
	return GETBYTE(ILInt32, network, 0, 24) |
		   GETBYTE(ILInt32, network, 1, 16) |
		   GETBYTE(ILInt32, network, 2,  8) |
		   GETBYTE(ILInt32, network, 3,  0);
}

/*
 * public static int NetworkToHostOrder(int host);
 */
ILInt16 _IL_IPAddress_NetworkToHostOrder_s(ILExecThread *thread,
										   ILInt16 network)
{
	return GETBYTE(ILInt16, network, 0, 8) |
		   GETBYTE(ILInt16, network, 1, 0);
}


ILBool ToIPHostEntry (ILExecThread *_thread,
				struct hostent *h_ent,
				ILString ** h_name,
				System_Array ** h_aliases,
				System_Array ** h_addr_list
				)
{
	int length=-1;
	ILObject *objs;
	ILInt64 *buffer;

	*h_name=ILStringCreate(_thread,h_ent->h_name);
	
	/* Count the aliases using the NULL sentinel */
	while(h_ent->h_aliases[++length]!=NULL);

	/* Construct an array */
	*h_aliases = (System_Array *)ILExecThreadNew(_thread, "[oSystem.String;", 
					"(Ti)V",(ILVaInt)length);
	
	if(!(*h_aliases)) 
	{
		return 0; /* Assert memory is allocated */
	}

	objs=(ILObject*)(*h_aliases);
	
	while(length--)
	{
		ILExecThreadSetElem(_thread, objs, (ILInt32)length,
			(ILObject*)ILStringCreate(_thread,h_ent->h_aliases[length]));
	}

	/* length=-1; */ /* I'm already sure it should be */
	
	/* Count the addresses using the NULL sentinel */
	while(h_ent->h_addr_list[++length]!=NULL);

	/* Construct an array */
	*h_addr_list = (System_Array *)ILExecThreadNew(_thread, "[l", 
					"(Ti)V",(ILVaInt)length);
	
	if(!(*h_addr_list)) return 0; /* Assert memory is allocated */

	buffer=ArrayToBuffer(*h_addr_list);
	while(length--)
	{
		buffer[length]=(ILInt64)*((ILUInt32 *)(h_ent->h_addr_list[length]));
	}
	return 1;
}

/*
 * static bool InternalGetHostByName(String host, out String h_name ,
 * 							out String [] h_aliases, out long[] h_addr_list);
 */

ILBool _IL_DnsMethods_InternalGetHostByName(ILExecThread * _thread, 
				ILString * host, ILString * * h_name, 
				System_Array * * h_aliases, System_Array * * h_addr_list)
{
	struct hostent* h_ent;
	
	h_ent=ILGetHostByName(ILStringToAnsi(_thread,host));
	
	if(!h_ent)
	{
		return 0; /* false on error */
	}

	return ToIPHostEntry(_thread,h_ent,h_name,h_aliases,h_addr_list);
}

/*
 * static bool InternalGetHostByAddr(long address, out String h_name ,
 * 							out String [] h_aliases, out long[] h_addr_list);
 */

ILBool _IL_DnsMethods_InternalGetHostByAddr(ILExecThread * _thread, ILInt64 address, 
				ILString * * h_name, System_Array * * h_aliases, 
				System_Array * * h_addr_list)
{
	struct hostent *h_ent;	
	ILInt32 ip=address;//attempt a conversion to 4 byte form...

	h_ent=ILGetHostByAddr(&ip,sizeof(ip),AF_INET);
	
	if(!h_ent)
	{
		return 0; /* false on error */
	}

	return ToIPHostEntry(_thread,h_ent,h_name,h_aliases,h_addr_list);
}

/*
 *	static System.String InternalGetHostName(void);
 */
extern ILString * _IL_DnsMethods_InternalGetHostName(ILExecThread * _thread)
{
	char hostName[1024+1]; /* hmm... stack .. */
	if(ILGetHostName(hostName,1024*sizeof(char)) == -1)
	{
		return 0; /* error */
	}
	return ILStringCreate(_thread, hostName);
}

#endif /* IL_CONFIG_NETWORKING */
