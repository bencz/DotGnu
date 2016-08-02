/*
 * java_semvalue.h - Semantic value handling
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#ifndef	_JavaCC_JAVA_SEMVALUE_H
#define	_JavaCC_JAVA_SEMVALUE_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Define the semantic analysis return type.
 */
typedef struct
{
	int				 kind__;
	ILType			*type__;
	void			*extra__;
} JavaSemValue;

extern JavaSemValue JavaSemValueDefault;
extern JavaSemValue JavaSemValueError;

/*
 * Semantic value kinds.
 */
#define	JAVA_SEMKIND_VOID			(1<<0)
#define	JAVA_SEMKIND_RVALUE			(1<<1)
#define	JAVA_SEMKIND_LVALUE			(1<<2)
#define	JAVA_SEMKIND_SVALUE			(1<<3)
#define	JAVA_SEMKIND_TYPE			(1<<4)
#define	JAVA_SEMKIND_FIELD			(1<<5)
#define	JAVA_SEMKIND_METHOD_GROUP	(1<<6)
#define	JAVA_SEMKIND_NAMESPACE		(1<<9)
#define	JAVA_SEMKIND_TYPE_NODE		(1<<10)
#define	JAVA_SEMKIND_AMBIGUOUS		(1<<11)
#define	JAVA_SEMKIND_CONSTANT		(1<<13)
#define	JAVA_SEMKIND_ERROR			(1<<15)
#define	JAVA_SEMKIND_SUPER			(1<<16)

/*
 * Set a semantic value to "void".
 */
#define	JavaSemSetVoid(value)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_VOID; \
				(value).type__ = 0; \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an r-value with no constant.
 */
#define	JavaSemSetRValue(value,type)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_RVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an l-value.
 */
#define	JavaSemSetLValue(value,type)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_LVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an s-value.
 */
#define	JavaSemSetSValue(value,type)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_SVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an r-value, l-value, or s-value, by kind.
 */
#define	JavaSemSetValueKind(value,kind,type)	\
			do { \
				(value).kind__ = (kind); \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)


/*
 * Set a semantic value to a field, etc, by kind.
 */
#define	JavaSemSetKind(value,kind,extra)	\
			do { \
				(value).kind__ = (kind); \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(extra); \
			} while (0)
/*
 * Set a semantic value to a type.
 */
#define	JavaSemSetType(value,type)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_TYPE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to a type node.
 */
#define	JavaSemSetTypeNode(value,node)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_TYPE_NODE; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(node); \
			} while (0)

/*
 * Set a semantic value to a namespace.
 */
#define	JavaSemSetNamespace(value,ns)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_TYPE; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(ns); \
			} while (0)

/*
 * Set a semantic value to a compile-time constant.
 */
#define	JavaSemSetConstant(value,type,constValue)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_RVALUE | JAVA_SEMKIND_CONSTANT; \
				(value).type__ = (type); \
				(value).extra__ = yynodealloc(sizeof(ILEvalValue)); \
				*((ILEvalValue *)((value).extra__)) = (constValue); \
			} while (0)

/*
 * Set a semantic value to a method group.
 */
#define	JavaSemSetMethodGroup(value,group)	\
			do { \
				(value).kind__ = JAVA_SEMKIND_METHOD_GROUP; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(group); \
			} while (0)

/*
 * Modify a semantic value to indicate "base" access.
 */
#define	JavaSemSetSuper(value)	\
			do { \
				(value).kind__ |= JAVA_SEMKIND_SUPER; \
			} while (0)


/*
 * Determine if a semantic value has a specific kind.
 */
#define	JavaSemHasKind(value,kind)	(((value).kind__ & (kind)) != 0)

/*
 * Get the kind of a semantic value.
 */
#define	JavaSemGetKind(value)		((value).kind__)

/*
 * Determine if a semantic value is an error.
 */
#define	JavaSemIsError(value)		(JavaSemHasKind((value), JAVA_SEMKIND_ERROR))

/*
 * Determine if a semantic value is "void".
 */
#define	JavaSemIsVoid(value)		(JavaSemHasKind((value), JAVA_SEMKIND_VOID))

/*
 * Get the namespace associated with a semantic value.
 */
#define JavaSemGetNamespace(value)	((char *)((value).extra__))

/*
 * Determine if a semantic value is a type.
 */
#define	JavaSemIsType(value)	(JavaSemHasKind((value), JAVA_SEMKIND_TYPE))

/*
 * Determine if a semantic value is a type node.
 */
#define	JavaSemIsTypeNode(value)	(JavaSemHasKind((value), JAVA_SEMKIND_TYPE_NODE))

/*
 * Get the type associated with a semantic value.
 */
#define	JavaSemGetType(value)		((value).type__)

/*
 * Get the type node associated with a semantic value.
 */
#define	JavaSemGetTypeNode(value)		((ILNode *)((value).extra__))


/*
 * Determine if a semantic value is an l-value, r-value, or s-value.
 */
#define	JavaSemIsLValue(value)	(JavaSemHasKind((value), JAVA_SEMKIND_LVALUE))
#define	JavaSemIsRValue(value)	(JavaSemHasKind((value), JAVA_SEMKIND_RVALUE))
#define	JavaSemIsSValue(value)	(JavaSemHasKind((value), JAVA_SEMKIND_SVALUE))

/*
 * Determine if a semantic value is an l-value or r-value.
 */
#define	JavaSemIsValue(value)		\
			(((value).kind__ & (JAVA_SEMKIND_LVALUE | JAVA_SEMKIND_RVALUE)) != 0)

/*
 * Determine if a semantic value is a field.
 */
#define	JavaSemIsField(value)	(JavaSemHasKind((value), JAVA_SEMKIND_FIELD))

/*
 * Get the field record associated with a semantic value.
 */
#define	JavaSemGetField(value)	((ILField *)((value).extra__))

/*
 * Determine if a semantic value is a method group.
 */
#define	JavaSemIsMethodGroup(value)	\
			(JavaSemHasKind((value), JAVA_SEMKIND_METHOD_GROUP))

/*
 * Modify the group associated with a semantic value.
 */
#define	JavaSemModifyGroup(value,group)	((value).extra__ = (void *)(group))

/*
 * Get the group associated with a semantic value.
 */
#define	JavaSemGetGroup(value)	((value).extra__)

/*
 * Determine if a semantic value has the "base" flag.
 */
#define	JavaSemIsSuper(value)	(JavaSemHasKind((value), JAVA_SEMKIND_SUPER))

/*
 * Determine if a semantic value is a constant.
 */
#define	JavaSemIsConstant(value)	(JavaSemHasKind((value), JAVA_SEMKIND_CONSTANT))

/*
 * Get the constant value slot within a value.
 */
#define	JavaSemGetConstant(value)	\
			(JavaSemHasKind((value), JAVA_SEMKIND_CONSTANT) ? \
				(ILEvalValue *)((value).extra__) : (ILEvalValue *)0)

/*
 * Replace a node with its constant representation, if we
 * were able to evaluate a constant value.
 */
void _JavaSemReplaceWithConstant(ILNode **parent, ILEvalValue *value);

#define	JavaSemReplaceWithConstant(parent,value)	\
			do { \
				if(JavaSemIsConstant((value))) \
				{ \
					_JavaSemReplaceWithConstant \
						((parent), JavaSemGetConstant((value))); \
				} \
			} while (0)

#ifdef	__cplusplus
};
#endif

#endif	/* _JavaCC_JAVA_SEMVALUE_H */
