/*
 * vb_internal.h - Internal definitions for the VB compiler front end.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#ifndef	_CSCC_VB_INTERNAL_H
#define	_CSCC_VB_INTERNAL_H

#include <cscc/csharp/cs_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Build a date or time value.  The return value is in ticks.
 */
#define	VB_TIME_AM		1
#define	VB_TIME_PM		2
#define	VB_TIME_UNSPEC	0
ILInt64 VBDate(ILInt64 month, ILInt64 day, ILInt64 year, int yearDigits);
ILInt64 VBTime(ILInt64 hour, ILInt64 minute, ILInt64 second, int ampm);

/*
 * Modifier bits.
 */
#define	VB_MODIFIER_PUBLIC			(1<<0)
#define	VB_MODIFIER_PROTECTED		(1<<1)
#define	VB_MODIFIER_FRIEND			(1<<2)
#define	VB_MODIFIER_PRIVATE			(1<<3)
#define	VB_MODIFIER_SHADOWS			(1<<4)
#define	VB_MODIFIER_MUST_INHERIT	(1<<5)
#define	VB_MODIFIER_NOT_INHERITABLE	(1<<6)
#define	VB_MODIFIER_SHARED			(1<<7)
#define	VB_MODIFIER_OVERRIDABLE		(1<<8)
#define	VB_MODIFIER_NOT_OVERRIDABLE	(1<<9)
#define	VB_MODIFIER_MUST_OVERRIDE	(1<<10)
#define	VB_MODIFIER_OVERRIDES		(1<<11)
#define	VB_MODIFIER_OVERLOADS		(1<<12)
#define	VB_MODIFIER_READ_ONLY		(1<<13)
#define	VB_MODIFIER_WRITE_ONLY		(1<<14)
#define	VB_MODIFIER_WITH_EVENTS		(1<<15)
#define	VB_MODIFIER_DEFAULT			(1<<16)
#define	VB_MODIFIER_EXTERN			(1<<17)
#define	VB_MODIFIER_VOLATILE		(1<<18)

/*
 * Special modifier classes.
 */
#define	VB_MODIFIER_ACCESS	\
			(VB_MODIFIER_PUBLIC | VB_MODIFIER_PROTECTED | \
			 VB_MODIFIER_FRIEND | VB_MODIFIER_PRIVATE)
#define	VB_MODIFIER_CALLING	\
			(VB_MODIFIER_SHARED | VB_MODIFIER_OVERRIDABLE | \
			 VB_MODIFIER_NOT_OVERRIDABLE | VB_MODIFIER_MUST_OVERRIDE | \
			 VB_MODIFIER_OVERRIDES | VB_MODIFIER_OVERLOADS)
#define	VB_MODIFIER_TYPE	\
			(VB_MODIFIER_ACCESS | VB_MODIFIER_SHADOWS | \
			 VB_MODIFIER_MUST_INHERIT | VB_MODIFIER_NOT_INHERITABLE)

/*
 * Report an error for modifiers that have been used twice.
 */
void VBModifiersUsedTwice(char *filename, long linenum, ILUInt32 modifiers);

/*
 * Option flags.
 */
#define	VB_OPTION_EXPLICIT			(1<<0)
#define	VB_OPTION_STRICT			(1<<1)
#define	VB_OPTION_BINARY_COMPARE	(1<<2)

/*
 * Get the name of the root namespace.  NULL if not specified.
 */
char *VBGetRootNamespace(void);

/*
 * Add using clauses to a global namespace node for all
 * namespaces in the user-supplied command-line list.
 */
void VBAddGlobalImports(ILNode_Namespace *namespaceNode, ILScope *scope);

/*
 * Initialize the options for the global namespace node of a file.
 */
void VBOptionInit(ILNode_Namespace *namespaceNode);

/*
 * Set an option on a global namespace node.
 */
void VBOptionSet(ILNode_Namespace *namespaceNode, int option, int value);

/*
 * Determine if a particular option is set.  This may be passed
 * any namespace node in the file, not just the global one.
 */
int VBOptionIsSet(ILNode_Namespace *namespaceNode, int option);

/*
 * Process modifier flags.
 */
ILUInt32 VBModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, int isNested);
ILUInt32 VBModifiersToDelegateAttrs(ILNode *node, ILUInt32 modifiers,
									int isNested);
ILUInt32 VBModifiersToConstAttrs(ILNode *node, ILUInt32 modifiers,
								 int isModule);
ILUInt32 VBModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers,
								 int isModule);
ILUInt32 VBModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers,
								  int isModule);
ILUInt32 VBModifiersToEventAttrs(ILNode *node, ILUInt32 modifiers,
								 int isModule);
ILUInt32 VBModifiersToPropertyAttrs(ILNode *node, ILUInt32 modifiers,
								    int isModule);
ILUInt32 VBModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers,
								  	   int isModule);

#ifdef	__cplusplus
};
#endif

#endif /* _CSCC_VB_INTERNAL_H */
