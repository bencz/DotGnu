/*
 * cs_semvalue.h - Semantic value handling.
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

#ifndef	_CSCC_CS_SEMVALUE_H
#define	_CSCC_CS_SEMVALUE_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Define the semantic analysis return type.  This should
 * be treated as opaque.
 */
typedef struct
{
	int				kind__;
	ILType		   *type__;
	void		   *extra__;

} CSSemValue;

/*
 * Some special semantic values.
 */
extern CSSemValue CSSemValueDefault;
extern CSSemValue CSSemValueError;

/*
 * Semantic value kinds.
 */
#define	CS_SEMKIND_VOID				(1<<0)
#define	CS_SEMKIND_RVALUE			(1<<1)
#define	CS_SEMKIND_LVALUE			(1<<2)
#define	CS_SEMKIND_SVALUE			(1<<3)
#define	CS_SEMKIND_TYPE				(1<<4)
#define	CS_SEMKIND_FIELD			(1<<5)
#define	CS_SEMKIND_METHOD_GROUP		(1<<6)
#define	CS_SEMKIND_PROPERTY			(1<<7)
#define	CS_SEMKIND_EVENT			(1<<8)
#define	CS_SEMKIND_NAMESPACE		(1<<9)
#define	CS_SEMKIND_TYPE_NODE		(1<<10)
#define	CS_SEMKIND_AMBIGUOUS		(1<<11)
#define	CS_SEMKIND_INDEXER_GROUP	(1<<12)
#define	CS_SEMKIND_CONSTANT			(1<<13)
#define	CS_SEMKIND_DYN_CONSTANT		(1<<14)
#define	CS_SEMKIND_ERROR			(1<<15)
#define	CS_SEMKIND_BASE				(1<<16)

/*
 * Set a semantic value to "void".
 */
#define	CSSemSetVoid(value)	\
			do { \
				(value).kind__ = CS_SEMKIND_VOID; \
				(value).type__ = 0; \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an r-value with no constant.
 */
#define	CSSemSetRValue(value,type)	\
			do { \
				(value).kind__ = CS_SEMKIND_RVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an l-value.
 */
#define	CSSemSetLValue(value,type)	\
			do { \
				(value).kind__ = CS_SEMKIND_LVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an s-value.
 */
#define	CSSemSetSValue(value,type)	\
			do { \
				(value).kind__ = CS_SEMKIND_SVALUE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to an r-value, l-value, or s-value, by kind.
 */
#define	CSSemSetValueKind(value,kind,type)	\
			do { \
				(value).kind__ = (kind); \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to a field, property, etc, by kind.
 */
#define	CSSemSetKind(value,kind,extra)	\
			do { \
				(value).kind__ = (kind); \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(extra); \
			} while (0)

/*
 * Set a semantic value to a type.
 */
#define	CSSemSetType(value,type)	\
			do { \
				(value).kind__ = CS_SEMKIND_TYPE; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Set a semantic value to a type node.
 */
#define	CSSemSetTypeNode(value,node)	\
			do { \
				(value).kind__ = CS_SEMKIND_TYPE_NODE; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(node); \
			} while (0)

/*
 * Set a semantic value to a namespace.
 */
#define	CSSemSetNamespace(value,ns)	\
			do { \
				(value).kind__ = CS_SEMKIND_NAMESPACE; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(ns); \
			} while (0)

/*
 * Set a semantic value to a field.
 */
#define	CSSemSetField(value,field)	\
			do { \
				(value).kind__ = CS_SEMKIND_FIELD; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(field); \
			} while (0)

/*
 * Set a semantic value to a property.
 */
#define	CSSemSetProperty(value,property)	\
			do { \
				(value).kind__ = CS_SEMKIND_PROPERTY; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(property); \
			} while (0)

/*
 * Set a semantic value to an event.
 */
#define	CSSemSetEvent(value,event)	\
			do { \
				(value).kind__ = CS_SEMKIND_EVENT; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(event); \
			} while (0)

/*
 * Set a semantic value to a method group.
 */
#define	CSSemSetMethodGroup(value,group)	\
			do { \
				(value).kind__ = CS_SEMKIND_METHOD_GROUP; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(group); \
			} while (0)

/*
 * Set a semantic value to an indexer group.
 */
#define	CSSemSetIndexerGroup(value,group)	\
			do { \
				(value).kind__ = CS_SEMKIND_INDEXER_GROUP; \
				(value).type__ = ILType_Void; \
				(value).extra__ = (void *)(group); \
			} while (0)

/*
 * Set a semantic value to a compile-time constant.
 */
#define	CSSemSetConstant(value,type,constValue)	\
			do { \
				(value).kind__ = CS_SEMKIND_RVALUE | CS_SEMKIND_CONSTANT; \
				(value).type__ = (type); \
				(value).extra__ = yynodealloc(sizeof(ILEvalValue)); \
				*((ILEvalValue *)((value).extra__)) = (constValue); \
			} while (0)

/*
 * Set a semantic value to a dynamic run-time constant.
 */
#define	CSSemSetDynConstant(value,type)	\
			do { \
				(value).kind__ = CS_SEMKIND_RVALUE | CS_SEMKIND_CONSTANT | \
								 CS_SEMKIND_DYN_CONSTANT; \
				(value).type__ = (type); \
				(value).extra__ = 0; \
			} while (0)

/*
 * Modify a semantic value to indicate "base" access.
 */
#define	CSSemSetBase(value)	\
			do { \
				(value).kind__ |= CS_SEMKIND_BASE; \
			} while (0)

/*
 * Determine if a semantic value has a specific kind.
 */
#define	CSSemHasKind(value,kind)	(((value).kind__ & (kind)) != 0)

/*
 * Get the kind of a semantic value.
 */
#define	CSSemGetKind(value)		((value).kind__)

/*
 * Determine if a semantic value is an l-value, r-value, or s-value.
 */
#define	CSSemIsLValue(value)	(CSSemHasKind((value), CS_SEMKIND_LVALUE))
#define	CSSemIsRValue(value)	(CSSemHasKind((value), CS_SEMKIND_RVALUE))
#define	CSSemIsSValue(value)	(CSSemHasKind((value), CS_SEMKIND_SVALUE))

/*
 * Determine if a semantic value is an l-value or r-value.
 */
#define	CSSemIsValue(value)		\
			(((value).kind__ & (CS_SEMKIND_LVALUE | CS_SEMKIND_RVALUE)) != 0)

/*
 * Determine if a semantic value is an l-value or s-value.
 */
#define	CSSemIsLocation(value)	\
			(((value).kind__ & (CS_SEMKIND_LVALUE | CS_SEMKIND_SVALUE)) != 0)

/*
 * Determine if a semantic value is "void".
 */
#define	CSSemIsVoid(value)		(CSSemHasKind((value), CS_SEMKIND_VOID))

/*
 * Determine if a semantic value is boolean.
 */
#define	CSSemIsBoolean(value)	\
			(CSSemIsValue((value)) && (value)->type__ == ILType_Boolean)

/*
 * Determine if a semantic value is an error.
 */
#define	CSSemIsError(value)		(CSSemHasKind((value), CS_SEMKIND_ERROR))

/*
 * Determine if a semantic value is a constant.
 */
#define	CSSemIsConstant(value)	(CSSemHasKind((value), CS_SEMKIND_CONSTANT))

/*
 * Determine if a semantic value is a dynamic constant.
 */
#define	CSSemIsDynConstant(value)	\
			(CSSemHasKind((value), CS_SEMKIND_DYN_CONSTANT))

/*
 * Get the constant value slot within a value.
 */
#define	CSSemGetConstant(value)	\
			(CSSemHasKind((value), CS_SEMKIND_CONSTANT) ? \
				(ILEvalValue *)((value).extra__) : (ILEvalValue *)0)

/*
 * Determine if a semantic value is a type.
 */
#define	CSSemIsType(value)	(CSSemHasKind((value), CS_SEMKIND_TYPE))

/*
 * Determine if a semantic value is a type node.
 */
#define	CSSemIsTypeNode(value)	(CSSemHasKind((value), CS_SEMKIND_TYPE_NODE))

/*
 * Get the type associated with a semantic value.
 */
#define	CSSemGetType(value)		((value).type__)

/*
 * Get the type node associated with a semantic value.
 */
#define	CSSemGetTypeNode(value)		((ILNode *)((value).extra__))

/*
 * Replace a node with its constant representation, if we
 * were able to evaluate a constant value.
 */
void _CSSemReplaceWithConstant(ILNode **parent, ILEvalValue *value);
#define	CSSemReplaceWithConstant(parent,value)	\
			do { \
				if(CSSemIsConstant((value))) \
				{ \
					_CSSemReplaceWithConstant \
						((parent), CSSemGetConstant((value))); \
				} \
			} while (0)

/*
 * Determine if a semantic value is a field.
 */
#define	CSSemIsField(value)	(CSSemHasKind((value), CS_SEMKIND_FIELD))

/*
 * Get the field record associated with a semantic value.
 */
#define	CSSemGetField(value)	((ILField *)((value).extra__))

/*
 * Determine if a semantic value is a property.
 */
#define	CSSemIsProperty(value)	(CSSemHasKind((value), CS_SEMKIND_PROPERTY))

/*
 * Get the property record associated with a semantic value.
 */
#define	CSSemGetProperty(value)	((ILProperty *)((value).extra__))

/*
 * Determine if a semantic value is an event.
 */
#define	CSSemIsEvent(value)	(CSSemHasKind((value), CS_SEMKIND_EVENT))

/*
 * Get the event record associated with a semantic value.
 */
#define	CSSemGetEvent(value)	((ILEvent *)((value).extra__))

/*
 * Determine if a semantic value is an indexer group.
 */
#define	CSSemIsIndexerGroup(value)	\
			(CSSemHasKind((value), CS_SEMKIND_INDEXER_GROUP))

/*
 * Determine if a semantic value is a method group.
 */
#define	CSSemIsMethodGroup(value)	\
			(CSSemHasKind((value), CS_SEMKIND_METHOD_GROUP))

/*
 * Get the group associated with a semantic value.
 */
#define	CSSemGetGroup(value)	((value).extra__)

/*
 * Modify the group associated with a semantic value.
 */
#define	CSSemModifyGroup(value,group)	((value).extra__ = (void *)(group))

/*
 * Get the namespace associated with a semantic value.
 */
#define	CSSemGetNamespace(value)	((ILScope *)((value).extra__))

/*
 * Determine if a semantic value has the "base" flag.
 */
#define	CSSemIsBase(value)	(CSSemHasKind((value), CS_SEMKIND_BASE))

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CS_SEMVALUE_H */
