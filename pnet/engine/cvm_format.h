/*
 * cvm_format.h - Format information for CVM instruction patterns.
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

#ifndef	_ENGINE_CVM_FORMAT_H
#define	_ENGINE_CVM_FORMAT_H

#ifdef	__cplusplus
extern	"C" {
#endif

#ifndef	IL_CVM_DIRECT

/*
 * Bytecode-based instruction patterns.
 */

/*
 * Lengths of CVM instructions that have various kinds of arguments.
 */
#define	CVM_LEN_NONE					1
#define	CVM_LEN_BYTE					2
#define	CVM_LEN_BYTE2					3
#define	CVM_LEN_WORD					5
#define	CVM_LEN_WORD2					9
#define	CVM_LEN_PTR						(1 + sizeof(void *))
#define	CVM_LEN_PTR2					(1 + 2 * sizeof(void *))
#define CVM_LEN_WIDE_SMALL				2
#define	CVM_LEN_WIDE_LARGE				6
#define	CVM_LEN_WIDE_PTR_SMALL			(2 + sizeof(void *))
#define	CVM_LEN_WIDE_PTR_LARGE			(6 + sizeof(void *))
#define	CVM_LEN_DWIDE_SMALL				3
#define	CVM_LEN_DWIDE_LARGE				10
#define	CVM_LEN_DWIDE_PTR_SMALL			(3 + sizeof(void *))
#define	CVM_LEN_DWIDE_PTR_LARGE			(10 + sizeof(void *))
#define	CVM_LEN_LONG					9
#define	CVM_LEN_FLOAT					5
#define	CVM_LEN_DOUBLE					9
#define	CVM_LEN_BRANCH					6
#define	CVM_LEN_TRY						12
#define	CVM_LEN_BREAK					2
#define	CVMP_LEN_NONE					2
#define	CVMP_LEN_BYTE					3
#define	CVMP_LEN_WORD					6
#define	CVMP_LEN_WORD2					10
#define	CVMP_LEN_PTR					(2 + sizeof(void *))
#define	CVMP_LEN_WORD_PTR				(6 + sizeof(void *))
#define	CVMP_LEN_WORD2_PTR				(10 + sizeof(void *))
#define	CVMP_LEN_WORD2_PTR2				(10 + 2 * sizeof(void *))

/*
 * Extract particular arguments from CVM instructions.
 */
#define	CVM_ARG_SUB_OPCODE				(pc[1])
#define	CVM_ARG_BYTE					((ILUInt32)(ILUInt8)(pc[1]))
#define	CVM_ARG_SBYTE					((ILInt32)(ILInt8)(pc[1]))
#define	CVM_ARG_BYTE2					((ILUInt32)(ILUInt8)(pc[2]))
#define	CVM_ARG_SBYTE2					((ILInt32)(ILInt8)(pc[2]))
#define	CVM_ARG_WORD					(IL_READ_UINT32(pc + 1))
#define	CVM_ARG_WORD2					(IL_READ_UINT32(pc + 5))
#define	CVM_ARG_PTR(type)				((type)(ReadPointer(pc + 1)))
#define	CVM_ARG_PTR2(type)				\
			((type)(ReadPointer(pc + 1 + sizeof(void *))))
#define	CVM_ARG_WIDE_SMALL				CVM_ARG_BYTE
#define	CVM_ARG_WIDE_LARGE				(IL_READ_UINT32(pc + 2))
#define	CVM_ARG_WIDE_PTR_SMALL(type)	((type)(ReadPointer(pc + 2)))
#define	CVM_ARG_WIDE_PTR_LARGE(type)	((type)(ReadPointer(pc + 6)))
#define	CVM_ARG_DWIDE1_SMALL			CVM_ARG_BYTE
#define	CVM_ARG_DWIDE2_SMALL			CVM_ARG_BYTE2
#define	CVM_ARG_DWIDE1_LARGE			(IL_READ_UINT32(pc + 2))
#define	CVM_ARG_DWIDE2_LARGE			(IL_READ_UINT32(pc + 6))
#define	CVM_ARG_DWIDE_PTR_SMALL(type)	((type)(ReadPointer(pc + 3)))
#define	CVM_ARG_DWIDE_PTR_LARGE(type)	((type)(ReadPointer(pc + 10)))
#define	CVM_ARG_LONG					(IL_READ_INT64(pc + 1))
#define	CVM_ARG_FLOAT					(IL_READ_FLOAT(pc + 1))
#define	CVM_ARG_DOUBLE					(IL_READ_DOUBLE(pc + 1))
#define	CVM_ARG_BRANCH_SHORT			(pc + CVM_ARG_SBYTE)
#define	CVM_ARG_BRANCH_LONG				(pc + IL_READ_INT32(pc + 2))
#define CVM_ARG_JSR_RETURN				(pc + 6)
#define	CVM_ARG_SWITCH_LIMIT			(IL_READ_UINT32(pc + 1))
#define	CVM_ARG_SWITCH_DEFAULT			(pc + IL_READ_INT32(pc + 5))
#define	CVM_ARG_SWITCH_DEST(n)			(pc + IL_READ_INT32(pc + 9 + (n) * 4))
#define	CVM_ARG_CALL_RETURN(pc)			((pc) + sizeof(void *) + 1)
#define	CVM_ARG_CALLIND_RETURN(pc)		((pc) + 1)
#define	CVM_ARG_CALLV_RETURN_SMALL(pc)	((pc) + 3)
#define	CVM_ARG_CALLV_RETURN_LARGE(pc)	((pc) + 10)
#define	CVM_ARG_CALLI_RETURN_SMALL(pc)	((pc) + 3 + sizeof(void *))
#define	CVM_ARG_CALLI_RETURN_LARGE(pc)	((pc) + 10 + sizeof(void *))
#define	CVM_ARG_TAIL_METHOD				((ILMethod *)(ReadPointer(pc + 2)))
#define	CVM_ARG_TRY_START				(IL_READ_UINT32(pc))
#define	CVM_ARG_TRY_END					(IL_READ_UINT32(pc + 4))
#define	CVM_ARG_TRY_LENGTH				(IL_READ_UINT32(pc + 8))
#define	CVM_ARG_BREAK_SUBCODE			(pc[1])
#define	CVMP_ARG_BYTE					((ILUInt32)(ILUInt8)(pc[2]))
#define	CVMP_ARG_SBYTE					((ILInt32)(ILInt8)(pc[2]))
#define	CVMP_ARG_WORD					(IL_READ_UINT32(pc + 2))
#define	CVMP_ARG_WORD2					(IL_READ_UINT32(pc + 6))
#define	CVMP_ARG_PTR(type)				((type)(ReadPointer(pc + 2)))
#define	CVMP_ARG_WORD_PTR(type)			((type)(ReadPointer(pc + 6)))
#define	CVMP_ARG_WORD2_PTR(type)		((type)(ReadPointer(pc + 10)))
#define	CVMP_ARG_WORD2_PTR2(type)		((type)(ReadPointer(pc + 10 + sizeof(void *))))

#else /* IL_CVM_DIRECT */

/*
 * Direct threading instruction patterns.
 */

/*
 * Lengths of CVM instructions that have various kinds of arguments.
 */
#define	_CVM_LEN_FROM_WORDS(n)			((n) * sizeof(void *))
#define	_CVM_LEN_FROM_CONST(n)	\
		((((n) + sizeof(void *) - 1) & ~(sizeof(void *) - 1)) + sizeof(void *))
#define	CVM_LEN_NONE					_CVM_LEN_FROM_WORDS(1)
#define	CVM_LEN_BYTE					_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_BYTE2					_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_WORD					_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_WORD2					_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_PTR						_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_PTR2					_CVM_LEN_FROM_WORDS(3)
#define CVM_LEN_WIDE_SMALL				_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_WIDE_LARGE				_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_WIDE_PTR_SMALL			_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_WIDE_PTR_LARGE			_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_DWIDE_SMALL				_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_DWIDE_LARGE				_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_DWIDE_PTR_SMALL			_CVM_LEN_FROM_WORDS(4)
#define	CVM_LEN_DWIDE_PTR_LARGE			_CVM_LEN_FROM_WORDS(4)
#define	CVM_LEN_LONG					_CVM_LEN_FROM_CONST(8)
#define	CVM_LEN_FLOAT					_CVM_LEN_FROM_CONST(4)
#define	CVM_LEN_DOUBLE					_CVM_LEN_FROM_CONST(8)
#define	CVM_LEN_BRANCH					_CVM_LEN_FROM_WORDS(2)
#define	CVM_LEN_TRY						_CVM_LEN_FROM_WORDS(3)
#define	CVM_LEN_BREAK					_CVM_LEN_FROM_WORDS(2)
#define	CVMP_LEN_NONE					_CVM_LEN_FROM_WORDS(1)
#define	CVMP_LEN_BYTE					_CVM_LEN_FROM_WORDS(2)
#define	CVMP_LEN_WORD					_CVM_LEN_FROM_WORDS(2)
#define	CVMP_LEN_WORD2					_CVM_LEN_FROM_WORDS(3)
#define	CVMP_LEN_PTR					_CVM_LEN_FROM_WORDS(2)
#define	CVMP_LEN_WORD_PTR				_CVM_LEN_FROM_WORDS(3)
#define	CVMP_LEN_WORD2_PTR				_CVM_LEN_FROM_WORDS(4)
#define	CVMP_LEN_WORD2_PTR2				_CVM_LEN_FROM_WORDS(5)

/*
 * Extract particular arguments from CVM instructions.
 */
#define	_CVM_ARG(n)				(((void **)(pc))[(n)])
#define	_CVM_ARG_WORD(n)		((ILUInt32)(ILNativeUInt)(_CVM_ARG((n))))
#define	_CVM_OFFSET(n)			((n) * sizeof(void *))
#define	CVM_ARG_BYTE			((ILUInt32)(ILUInt8)(_CVM_ARG_WORD(1)))
#define	CVM_ARG_SBYTE			((ILInt32)(ILInt8)(_CVM_ARG_WORD(1)))
#define	CVM_ARG_BYTE2			((ILUInt32)(ILUInt8)(_CVM_ARG_WORD(2)))
#define	CVM_ARG_SBYTE2			((ILInt32)(ILInt8)(_CVM_ARG_WORD(2)))
#define	CVM_ARG_WORD			(_CVM_ARG_WORD(1))
#define	CVM_ARG_WORD2			(_CVM_ARG_WORD(2))
#define	CVM_ARG_PTR(type)		((type)(_CVM_ARG(1)))
#define	CVM_ARG_PTR2(type)		((type)(_CVM_ARG(2)))
#define	CVM_ARG_WIDE_SMALL		(_CVM_ARG_WORD(1))
#define	CVM_ARG_WIDE_LARGE		(_CVM_ARG_WORD(1))
#define	CVM_ARG_WIDE_PTR_SMALL(type)	((type)(_CVM_ARG(2)))
#define	CVM_ARG_WIDE_PTR_LARGE(type)	((type)(_CVM_ARG(2)))
#define	CVM_ARG_DWIDE1_SMALL	(_CVM_ARG_WORD(1))
#define	CVM_ARG_DWIDE2_SMALL	(_CVM_ARG_WORD(2))
#define	CVM_ARG_DWIDE1_LARGE	(_CVM_ARG_WORD(1))
#define	CVM_ARG_DWIDE2_LARGE	(_CVM_ARG_WORD(2))
#define	CVM_ARG_DWIDE_PTR_SMALL(type)	((type)(_CVM_ARG(3)))
#define	CVM_ARG_DWIDE_PTR_LARGE(type)	((type)(_CVM_ARG(3)))
#define	CVM_ARG_LONG			(IL_READ_INT64(pc + sizeof(void *)))
#define	CVM_ARG_FLOAT			(IL_READ_FLOAT(pc + sizeof(void *)))
#define	CVM_ARG_DOUBLE			(IL_READ_DOUBLE(pc + sizeof(void *)))
#define	CVM_ARG_BRANCH_SHORT	((unsigned char *)(_CVM_ARG(1)))
#define	CVM_ARG_BRANCH_LONG		((unsigned char *)(_CVM_ARG(1)))
#define CVM_ARG_JSR_RETURN		(pc + _CVM_OFFSET(2))
#define	CVM_ARG_SWITCH_LIMIT	(_CVM_ARG_WORD(1))
#define	CVM_ARG_SWITCH_DEFAULT	((unsigned char *)(_CVM_ARG(2)))
#define	CVM_ARG_SWITCH_DEST(n)	((unsigned char *)(_CVM_ARG((n) + 3)))
#define	CVM_ARG_CALL_RETURN(pc)	((pc) + _CVM_OFFSET(2))
#define	CVM_ARG_CALLIND_RETURN(pc)		((pc) + _CVM_OFFSET(1))
#define	CVM_ARG_CALLV_RETURN_SMALL(pc)	((pc) + _CVM_OFFSET(3))
#define	CVM_ARG_CALLV_RETURN_LARGE(pc)	((pc) + _CVM_OFFSET(3))
#define	CVM_ARG_CALLI_RETURN_SMALL(pc)	((pc) + _CVM_OFFSET(4))
#define	CVM_ARG_CALLI_RETURN_LARGE(pc)	((pc) + _CVM_OFFSET(4))
#define	CVM_ARG_TAIL_METHOD		CVM_ARG_PTR(ILMethod *)
#define	CVM_ARG_TRY_START		((ILUInt32)(ILNativeUInt)(((void **)(pc))[0]))
#define	CVM_ARG_TRY_END			((ILUInt32)(ILNativeUInt)(((void **)(pc))[1]))
#define	CVM_ARG_TRY_LENGTH		((ILUInt32)(ILNativeUInt)(((void **)(pc))[2]))
#define	CVM_ARG_BREAK_SUBCODE	CVM_ARG_WORD
#define	CVMP_ARG_BYTE			CVM_ARG_BYTE
#define	CVMP_ARG_SBYTE			CVM_ARG_SBYTE
#define	CVMP_ARG_WORD			CVM_ARG_WORD
#define	CVMP_ARG_WORD2			CVM_ARG_WORD2
#define	CVMP_ARG_PTR(type)		CVM_ARG_PTR(type)
#define	CVMP_ARG_WORD_PTR(type)			((type)(_CVM_ARG(2)))
#define	CVMP_ARG_WORD2_PTR(type)		((type)(_CVM_ARG(3)))
#define	CVMP_ARG_WORD2_PTR2(type)		((type)(_CVM_ARG(4)))

#endif /* IL_CVM_DIRECT */

#ifdef	__cplusplus
};
#endif

#endif	/* _ENGINE_CVM_FORMAT_H */
