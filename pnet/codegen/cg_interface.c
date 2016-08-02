/*
 * cg_interface.c - Interface method check routines.
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

#include "cg_nodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Determine if "classInfo" implements a particular interface method.
 * Note: Returns non-zero on error.
 */
static int ImplementsMethod(ILNode *node, ILClass *classInfo, ILMethod *method,
							ILGenInterfaceErrorFunc error,
							ILGenInterfaceProxyFunc proxy)
{
	if(!ILClassGetMethodImpl(classInfo, method))
	{
		ILMethod *proxyMethod;
		proxyMethod = ILClassGetMethodImplForProxy(classInfo, method);
		if(!proxyMethod)
		{
			(*error)(node, classInfo, (ILMember *)method);
			return 1;
		}
		else
		{
			(*proxy)(node, classInfo, method, proxyMethod);
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

/*
 * Determine if "classInfo" implements a particular method semantics
 * member (property or event).  Note: Returns non-zero on error.
 */
static int ImplementsMethodSem(ILNode *node, ILClass *classInfo,
							   ILMember *member, ILGenInterfaceErrorFunc error,
							   ILGenInterfaceProxyFunc proxy)
{
	ILUInt16 semType;
	ILMethod *method;
	ILMethod *proxyMethod;

	/* Scan through all method semantics kinds on the member,
	   and then check that each method found is implemented */
	semType = 0x0100;
	while(semType != 0)
	{
		method = ILMethodSemGetByType((ILProgramItem *)member, semType);
		if(method && !ILClassGetMethodImpl(classInfo, method))
		{
			proxyMethod = ILClassGetMethodImplForProxy(classInfo, method);
			if(!proxyMethod)
			{
				(*error)(node, classInfo, member);
				return 1;
			}
			else
			{
				(*proxy)(node, classInfo, method, proxyMethod);
			}
		}
		semType >>= 1;
	}

	/* If we get here, then all semantics methods are present */
	return 0;
}

/*
 * Forward declaration.
 */
static int ImplementsAllInterfaces(ILNode *node, ILClass *classInfo,
						           ILClass *refClass,
								   ILGenInterfaceErrorFunc error,
								   ILGenInterfaceProxyFunc proxy,
							       ILClass **visited, int *visitedSize);

/*
 * Determine if "classInfo" fully implements "interface".
 * Note: Returns non-zero on error.
 */
static int ImplementsInterface(ILNode *node, ILClass *classInfo,
						       ILClass *interface,
							   ILGenInterfaceErrorFunc error,
							   ILGenInterfaceProxyFunc proxy,
							   ILClass **visited, int *visitedSize)
{
	int posn;
	ILMember *member;
	int sawErrors;

#if IL_VERSION_MAJOR > 1
	if(ILClassNeedsExpansion(interface))
	{
		ILType *classType;
		ILClass *instanceInfo;

		classType = ILClass_SynType(interface);
		instanceInfo = ILClassInstantiate(ILProgramItem_Image(interface),
										  classType, classType, 0);
		if(!instanceInfo)
		{
			sawErrors = 1;
		}
		else
		{
			interface = instanceInfo;
		}
	}
#endif	/* IL_VERSION_MAJOR > 1 */

	/* Bail out if we've already visited this interface */
	for(posn = 0; posn < *visitedSize; ++posn)
	{
		if(visited[posn] == interface)
		{
			return 0;
		}
	}
	visited[(*visitedSize)++] = interface;

	/* Process all members within the interface */
	sawErrors = 0;
	member = 0;
	while((member = ILClassNextMember(interface, member)) != 0)
	{
		if(ILMember_IsMethod(member) && !ILMethod_HasSpecialName(member))
		{
			sawErrors |= ImplementsMethod(node, classInfo,
										  (ILMethod *)member, error, proxy);
		}
		else if(ILMember_IsProperty(member) || ILMember_IsEvent(member))
		{
			sawErrors |= ImplementsMethodSem(node, classInfo, member,
											 error, proxy);
		}
	}

	/* Process all of the parent interfaces */
	return sawErrors | ImplementsAllInterfaces(node, classInfo, interface,
											   error, proxy, visited,
											   visitedSize);
}

/*
 * Determine if "classInfo" fully implements all parent interfaces
 * of "refClass".  Note: Returns non-zero on error.
 */
static int ImplementsAllInterfaces(ILNode *node, ILClass *classInfo,
						           ILClass *refClass,
								   ILGenInterfaceErrorFunc error,
								   ILGenInterfaceProxyFunc proxy,
							       ILClass **visited, int *visitedSize)
{
	ILImplements *impl = 0;
	int sawErrors = 0;
	while((impl = ILClassNextImplements(refClass, impl)) != 0)
	{
		sawErrors |= ImplementsInterface
				(node, classInfo,
				 ILImplements_InterfaceClass(impl),
				 error, proxy, visited, visitedSize);
	}
	return sawErrors;
}

/*
 * Get the total spanning size of an interface inheritance tree.
 */
static int GetSpanningSize(ILClass *interface)
{
	int size = 1;
	ILImplements *impl = 0;
	while((impl = ILClassNextImplements(interface, impl)) != 0)
	{
		size += GetSpanningSize(ILImplements_UnderlyingInterfaceClass(impl));
	}
	return size;
}

int ILGenImplementsAllInterfaces(ILGenInfo *info, ILNode *node,
							     ILClass *classInfo,
								 ILGenInterfaceErrorFunc error,
								 ILGenInterfaceProxyFunc proxy)
{
	ILClass **visited;
	int visitedSize;
	int sawErrors;

	/* Allocate a temporary array to keep track of which interfaces
	   we have already visited, in case the same interface is present
	   along multiple inheritance paths */
	visited = (ILClass **)ILMalloc(sizeof(ILClass *) *
								   GetSpanningSize(classInfo));
	if(!visited)
	{
		ILGenOutOfMemory(info);
	}
	visitedSize = 0;

	/* Recursively visit all interfaces */
	sawErrors = ImplementsAllInterfaces(node, classInfo, classInfo, error,
									    proxy, visited, &visitedSize);

	/* Clean up and exit */
	ILFree(visited);
	return !sawErrors;
}

#ifdef	__cplusplus
};
#endif
