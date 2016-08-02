/*
 * cg_genattrs.h - handle custom attributes.
 *
 * Copyright (C) 2009  Free Software Foundation, Inc.
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

#ifndef	__CODEGEN_CG_GENATTR_H__
#define	__CODEGEN_CG_GENATTR_H__

#include "cg_nodes.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Structure to hold constructor arguments.
 */
typedef struct _tagCGAttrCtorArg  CGAttrCtorArg;
struct _tagCGAttrCtorArg
{
	ILType		   *type;			/* type of the ctor argument */
	ILNode		   *node;			/* only for error output */
	ILNode		  **parent;			/* not used, only for convinience */
	ILEvalValue		evalValue;		/* constant value passed to the ctor */
};

typedef struct _tagCGAttrNamedArg  CGAttrNamedArg;
struct _tagCGAttrNamedArg
{
	ILType		   *type;			/* type stored in item */
	ILNode		   *node;			/* only for error output */
	ILNode		  **parent;			/* not used, only for convinience */
	ILMember	   *member;			/* field or property */
	ILEvalValue		evalValue;		/* constant value passed to the named argument */
};

typedef struct _tagCGAttributeInfo CGAttributeInfo;
struct _tagCGAttributeInfo
{
	ILNode		   *node;			/* only for error output */
	ILProgramItem  *owner;
	ILMethod	   *ctor;
	CGAttrCtorArg  *ctorArgs;
	CGAttrNamedArg *namedArgs;
	ILUInt32		numArgs;
	ILUInt32		numNamed;
	CGAttributeInfo *next;
		
};

typedef struct _tagCGSecurityAttributeInfo CGSecurityAttributeInfo;
struct _tagCGSecurityAttributeInfo
{
	ILNode					   *node;				/* only for error output */
	ILClass					   *securityAttribute;
	CGAttrNamedArg			   *namedArgs;
	ILUInt32					numNamed;
	CGSecurityAttributeInfo	   *next;
};

/*
 * Opaque type to reference permissionsets.
 */
typedef struct _tagCGPermissionSets CGPermissionSets;

/*
 * Block to hold the information for the custom attributes
 * to be emitted for a program item.
 */
typedef struct _tagCGAttributeInfos  CGAttributeInfos;
struct _tagCGAttributeInfos
{
	ILMemStack			memstack;
	ILGenInfo		   *info;
	CGAttributeInfo	   *attributes;
	CGPermissionSets   *permissionSets;
};

/*
 * Initialize an attribute info block,
 */
void CGAttributeInfosInit(ILGenInfo *info,
						  CGAttributeInfos *attributeInfos);

/*
 * Destroy an attribute info block,
 */
void CGAttributeInfosDestroy(CGAttributeInfos *attributeInfos);

/*
 * Allocate numArgs CGAttrCtorArgs.
 * Returns 0 if numArgs is 0 or attributeInfos is 0.
 */
CGAttrCtorArg *CGAttrCtorArgAlloc(CGAttributeInfos *attributeInfos,
								  ILUInt32 numArgs);

/*
 * Allocate numNamed CGAttrNamedArgs.
 * Returns 0 if numNamed is 0 or attributeInfos is 0.
 */
CGAttrNamedArg *CGAttrNamedArgAlloc(CGAttributeInfos *attributeInfos,
									ILUInt32 numNamed);

/*
 * Add an attribute to the attribute infos.
 * Returns 0 if any argumant is not valid and 1 on success.
 * Success is even if the attribute is not added because of a target error.
 */
int CGAttributeInfosAddAttribute(CGAttributeInfos *attributeInfos,
								 ILNode *node,
								 ILProgramItem *owner,
								 ILMethod *ctor,
								 CGAttrCtorArg *ctorArgs,
								 ILUInt32 numArgs,
								 CGAttrNamedArg *namedArgs,
								 ILUInt32 numNamed,
								 ILUInt32 target);

/*
 * Process the collected attributes.
 */
int CGAttributeInfosProcess(CGAttributeInfos *attributeInfos);


#ifdef	__cplusplus
};
#endif

#endif	/* __CODEGEN_CG_GENATTR_H__ */
