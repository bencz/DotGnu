/*
 * java_internal.h - Internal definitions for the Java compiler.
 *
 * Copyright (C) 2003  Gopal.V
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

#ifndef	_CSCC_JAVA_INTERNAL_H
#define	_CSCC_JAVA_INTERNAL_H

#include <cscc/common/cc_main.h>
#include <cscc/java/java_defs.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Modifier mask bits.
 */
#define	JAVA_MODIFIER_PUBLIC		(1<<0)
#define	JAVA_MODIFIER_PRIVATE		(1<<1)
#define	JAVA_MODIFIER_PROTECTED		(1<<2)
#define	JAVA_MODIFIER_ACCESS_MASK	(JAVA_MODIFIER_PUBLIC | \
					 JAVA_MODIFIER_PRIVATE | \
					 JAVA_MODIFIER_PROTECTED)

#define	JAVA_MODIFIER_FINAL			(1<<4)
#define	JAVA_MODIFIER_ABSTRACT		(1<<5)
#define	JAVA_MODIFIER_NATIVE		(1<<6)
#define	JAVA_MODIFIER_STATIC		(1<<7)
#define	JAVA_MODIFIER_SYNCHRONIZED	(1<<8)
#define	JAVA_MODIFIER_TRANSIENT		(1<<9)
#define	JAVA_MODIFIER_VOLATILE		(1<<10)
#define	JAVA_MODIFIER_STRICTFP		(1<<11)


#define JAVA_SPECIALATTR_STATIC         0x08000000
#define JAVA_SPECIALATTR_STRICTFP       0x04000000
#define JAVA_SPECIALATTR_DEFAULTACCESS  0x02000000
#define JAVA_SPECIALATTR_FINAL          0x01000000
#define	JAVA_SPECIALATTR_VOLATILE		0x00800000

/*
 * Special attribute flags.
 */

/*
 * Flag bit that is used to distinguish args from locals.
 */
#define	JAVA_LOCAL_IS_ARG				0x80000000


/*
 * Type values that are used to classify the size of numeric values.
 */
#define	JAVA_NUMTYPE_INT32			0
#define	JAVA_NUMTYPE_INT64			1
#define	JAVA_NUMTYPE_FLOAT32		3
#define	JAVA_NUMTYPE_FLOAT64		4

/*
 * Gather information about all types in the program.
 * Returns a new top-level list for the program with
 * the classes re-organised so that parent classes and
 * interfaces precede classes that inherit them.
 */
ILNode *JavaTypeGather(ILGenInfo *info, ILScope *globalScope, ILNode *tree);

int JavaIsBaseTypeFor(ILClass *info1, ILClass *info2);

int JavaSignatureIdentical(ILType *sig1, ILType *sig2);

JavaSemValue JavaResolveSimpleName(ILGenInfo *genInfo, ILNode *node,
										const char *name, int literalType);

JavaSemValue JavaResolveNamespaceMemberName(ILGenInfo *genInfo, ILNode *node, 
										JavaSemValue value, const char *name);

JavaSemValue JavaResolveMemberName(ILGenInfo *genInfo, ILNode *node,
							   JavaSemValue value, const char *name,
							   int literalType);

ILClass *JavaGetAccessScope(ILGenInfo *genInfo, int defIsModule);

JavaSemValue JavaResolveConstructor(ILGenInfo *genInfo, ILNode *node,
								ILType *objectType);

ILProgramItem *JavaGetGroupMember(void *group, unsigned long n);

void *JavaRemoveGroupMember(void *group, unsigned long n);

void JavaSetGroupMemberForm(void *group, unsigned long n, int form);

int JavaGetGroupMemberForm(void *group, unsigned long n);

ILUInt32 JavaModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, 
								int isNested);

ILUInt32 JavaModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers);

ILUInt32 JavaModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers);

ILUInt32 JavaModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_JAVA_INTERNAL_H */
