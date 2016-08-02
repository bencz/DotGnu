/*
 * cvmc_gen.h - Helper macros for CVM code generation.
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

#ifndef	_ENGINE_CVMC_GEN_H
#define	_ENGINE_CVMC_GEN_H

#ifdef	__cplusplus
extern	"C" {
#endif

#ifndef IL_CVM_DIRECT

/*
 * Bytecode-based code generation macros.
 */

/*
 * Get the current method position.
 */
#define	CVM_POSN()		(ILCacheGetPosn(&(((ILCVMCoder *)coder)->codePosn)))

/*
 * Determine if "n" bytes at "addr" are valid.
 */
#define	_CVM_VALID(addr,n)	\
			((((unsigned char *)(addr)) + (n)) <= \
				((ILCVMCoder *)coder)->codePosn.limit)

/*
 * Output a single byte to the CVM coder buffer.
 */
#define	_CVM_BYTE(byte)		\
			do { \
				ILCacheByte(&(((ILCVMCoder *)coder)->codePosn), (byte)); \
			} while (0)

/*
 * Output a 32-bit value to the CVM coder buffer.
 */
#define	_CVM_WORD(value)	\
			do { \
				ILCacheWord32(&(((ILCVMCoder *)coder)->codePosn), (value)); \
			} while (0)

/*
 * Output a pointer value to the CVM coder buffer.
 */
#ifdef IL_NATIVE_INT32
#define	_CVM_PTR(value)	\
			do { \
				ILCacheWord32(&(((ILCVMCoder *)coder)->codePosn), (value)); \
			} while (0)
#else
#define	_CVM_PTR(value)	\
			do { \
				ILCacheWord64(&(((ILCVMCoder *)coder)->codePosn), (value)); \
			} while (0)
#endif

/*
 * Output a wide instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_WIDE(opcode,value)	\
			do { \
				if((value) < 256) \
				{ \
					_CVM_BYTE((opcode)); \
					_CVM_BYTE((value)); \
				} \
				else \
				{ \
					_CVM_BYTE(COP_WIDE); \
					_CVM_BYTE((opcode)); \
					_CVM_WORD((value)); \
				} \
			} while (0)

/*
 * Output a double wide instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_DWIDE(opcode,value1,value2)	\
			do { \
				if((value1) < 256 && (value2) < 256) \
				{ \
					_CVM_BYTE((opcode)); \
					_CVM_BYTE((value1)); \
					_CVM_BYTE((value2)); \
				} \
				else \
				{ \
					_CVM_BYTE(COP_WIDE); \
					_CVM_BYTE((opcode)); \
					_CVM_WORD((value1)); \
					_CVM_WORD((value2)); \
				} \
			} while (0)

/*
 * Output a double wide with pointer instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_DWIDE_PTR(opcode,value1,value2,value3)	\
			do { \
				if((value1) < 256 && (value2) < 256) \
				{ \
					_CVM_BYTE((opcode)); \
					_CVM_BYTE((value1)); \
					_CVM_BYTE((value2)); \
					_CVM_PTR((value3)); \
				} \
				else \
				{ \
					_CVM_BYTE(COP_WIDE); \
					_CVM_BYTE((opcode)); \
					_CVM_WORD((value1)); \
					_CVM_WORD((value2)); \
					_CVM_PTR((value3)); \
				} \
			} while (0)

/*
 * Output a return instruction.
 */
#define	_CVM_OUT_RETURN(size)	\
			do { \
				if(((ILCVMCoder *)coder)->debugEnabled) \
				{ \
					_CVM_OUT_BREAK(IL_BREAK_EXIT_METHOD); \
				} \
				if((size) == 0) \
				{ \
					_CVM_OUT_NONE(COP_RETURN); \
				} \
				else if((size) == 1) \
				{ \
					_CVM_OUT_NONE(COP_RETURN_1); \
				} \
				else if((size) == 2) \
				{ \
					_CVM_OUT_NONE(COP_RETURN_2); \
				} \
				else \
				{ \
					_CVM_OUT_WORD(COP_RETURN_N, (size)); \
				} \
			} while (0)

/*
 * Output an instruction that takes no parameters.
 */
#define	_CVM_OUT_NONE(opcode)	_CVM_BYTE((opcode))
#define	_CVMP_OUT_NONE(opcode)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
			} while (0)

/*
 * Output an instruction that has a byte argument.
 */
#define	_CVM_OUT_BYTE(opcode,value)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((value)); \
			} while (0)

/*
 * Output an instruction that has two byte arguments.
 */
#define	_CVM_OUT_BYTE2(opcode,value1,value2)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((value1)); \
				_CVM_BYTE((value2)); \
			} while (0)

/*
 * Output an instruction that has a word argument.
 */
#define	_CVM_OUT_WORD(opcode,value)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value)); \
			} while (0)
#define	_CVMP_OUT_WORD(opcode,value)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value)); \
			} while (0)

/*
 * Output an instruction that has two word arguments.
 */
#define	_CVMP_OUT_WORD2(opcode,value1,value2)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
			} while (0)

/*
 * Output an instruction that has a pointer argument.
 */
#define	_CVM_OUT_PTR(opcode,value)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_PTR((value)); \
			} while (0)
#define	_CVMP_OUT_PTR(opcode,value)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_PTR((value)); \
			} while (0)

/*
 * Output an instruction that has two pointer arguments.
 */
#define	_CVM_OUT_PTR2(opcode,value1,value2)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_PTR((value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has a word argument and a pointer argument.
 */
#define	_CVMP_OUT_WORD_PTR(opcode,value1,value2)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has two word arguments and a pointer argument.
 */
#define	_CVMP_OUT_WORD2_PTR(opcode,value1,value2,value3)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
				_CVM_PTR((value3)); \
			} while (0)

/*
 * Output an instruction that has two word arguments and two pointer arguments.
 */
#define	_CVMP_OUT_WORD2_PTR2(opcode,value1,value2,value3,value4)	\
			do { \
				_CVM_BYTE(COP_PREFIX); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
				_CVM_PTR((value3)); \
				_CVM_PTR((value4)); \
			} while (0)

/*
 * Output an instruction that has a wide argument and a pointer argument.
 */
#define	_CVM_OUT_WIDE_PTR(opcode,value1,value2)	\
			do { \
				_CVM_OUT_WIDE((opcode), (value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has a "long" argument.
 */
#define	_CVM_OUT_LONG(opcode,arg)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((arg)[0]); \
				_CVM_BYTE((arg)[1]); \
				_CVM_BYTE((arg)[2]); \
				_CVM_BYTE((arg)[3]); \
				_CVM_BYTE((arg)[4]); \
				_CVM_BYTE((arg)[5]); \
				_CVM_BYTE((arg)[6]); \
				_CVM_BYTE((arg)[7]); \
			} while (0)

/*
 * Output an instruction that has a "float" argument.
 */
#define	_CVM_OUT_FLOAT(opcode,arg)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((arg)[0]); \
				_CVM_BYTE((arg)[1]); \
				_CVM_BYTE((arg)[2]); \
				_CVM_BYTE((arg)[3]); \
			} while (0)

/*
 * Output an instruction that has a "double" argument.
 */
#define	_CVM_OUT_DOUBLE(opcode,arg)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((arg)[0]); \
				_CVM_BYTE((arg)[1]); \
				_CVM_BYTE((arg)[2]); \
				_CVM_BYTE((arg)[3]); \
				_CVM_BYTE((arg)[4]); \
				_CVM_BYTE((arg)[5]); \
				_CVM_BYTE((arg)[6]); \
				_CVM_BYTE((arg)[7]); \
			} while (0)

/*
 * Output a short branch from the current position.
 */
#define	_CVM_OUT_BRANCH(opcode,offset)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((offset)); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Output a long branch from the current position.
 */
#define	_CVM_OUT_BRANCH_LONG(opcode,offset)	\
			do { \
				_CVM_BYTE(COP_BR_LONG); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD((offset)); \
			} while (0)

/*
 * Output a branch placeholder at the current position.
 */
#define	_CVM_OUT_BRANCH_PLACEHOLDER(opcode)	\
			do { \
				_CVM_BYTE((opcode)); \
				_CVM_BYTE((opcode)); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a short branch.
 */
#define	_CVM_BACKPATCH_BRANCH(pc,relative)	\
			do { \
				if(_CVM_VALID((pc), 2)) \
				{ \
					(pc)[1] = (unsigned char)(relative); \
				} \
			} while (0)

/*
 * Backpatch a long branch.
 */
#define	_CVM_BACKPATCH_BRANCH_LONG(pc,relative)	\
			do { \
				if(_CVM_VALID((pc), 6)) \
				{ \
					(pc)[0] = (unsigned char)COP_BR_LONG; \
					IL_WRITE_INT32((pc) + 2, (ILInt32)(relative)); \
				} \
			} while (0)

/*
 * Compute the offset of the default switch case from the
 * number of entries.
 */
#define	_CVM_DEFCASE_OFFSET(numEntries)	\
			(9 + (numEntries) * 4)

/*
 * Output the head of a switch instruction.
 */
#define	_CVM_OUT_SWHEAD(numEntries,defCase)	\
			do { \
				_CVM_BYTE(COP_SWITCH); \
				_CVM_WORD((numEntries)); \
				_CVM_WORD((defCase)); \
			} while (0)

/*
 * Output a switch entry at the current position.
 */
#define	_CVM_OUT_SWENTRY(swstart,relative)	\
			do { \
				_CVM_WORD((relative)); \
			} while (0)

/*
 * Output a switch entry placeholder at the current position.
 */
#define	_CVM_OUT_SWENTRY_PLACEHOLDER()	\
			do { \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a switch entry.
 */
#define	_CVM_BACKPATCH_SWENTRY(swstart,relative,writepc)	\
			do { \
				if(_CVM_VALID((writepc), 4)) \
				{ \
					IL_WRITE_INT32((writepc), (ILInt32)(relative)); \
				} \
			} while (0)

/*
 * Output the start of a try header.
 */
#define	_CVM_OUT_TRY(start,end)	\
			do { \
				_CVM_WORD((start)); \
				_CVM_WORD((end)); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a try header with the length value.
 */
#define	_CVM_BACKPATCH_TRY(trypc)	\
			do { \
				if(_CVM_VALID((trypc), 12)) \
				{ \
					IL_WRITE_UINT32	\
						((trypc) + 8, (ILUInt32)(CVM_POSN() - (trypc))); \
				} \
			} while (0)

/*
 * Output a "pushdown" instruction placeholder.
 */
#define	_CVM_OUT_PUSHDOWN()		\
			do { \
				_CVM_BYTE(COP_PUSHDOWN); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a "pushdown" instruction.
 */
#define	_CVM_BACKPATCH_PUSHDOWN(pushpc,value)	\
			do { \
				if(_CVM_VALID((pushpc), 5)) \
				{ \
					IL_WRITE_UINT32((pushpc) + 1, (ILUInt32)(value)); \
				} \
			} while (0)

/*
 * Output a "ckheight" instruction placeholder.
 */
#define	_CVM_OUT_CKHEIGHT()		\
			do { \
				_CVM_BYTE(COP_CKHEIGHT_N); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a "ckheight" instruction.
 */
#define	_CVM_BACKPATCH_CKHEIGHT(ckpc,value)	\
			do { \
				if(_CVM_VALID((ckpc), 5)) \
				{ \
					if((value) <= 8) \
					{ \
						*(ckpc) = COP_CKHEIGHT; \
					} \
					else \
					{ \
						IL_WRITE_UINT32((ckpc) + 1, (ILUInt32)(value)); \
					} \
				} \
			} while (0)

/*
 * Output a "jump over" instruction to skip a non-constructor entry point.
 */
#define	_CVM_OUT_JUMPOVER(pc,relative)	\
			do { \
				if(_CVM_VALID((pc), 6)) \
				{ \
					if((relative) >= 0 && (relative) <= 127) \
					{ \
						(pc)[0] = COP_BR; \
						(pc)[1] = (unsigned char)(relative); \
					} \
					else \
					{ \
						(pc)[0] = COP_BR_LONG; \
						(pc)[1] = COP_BR; \
						IL_WRITE_INT32((pc) + 2, (ILInt32)(relative)); \
					} \
				} \
			} while (0)

/*
 * Output a "break" instruction.
 */
#define	_CVM_OUT_BREAK(subcode)	\
			do { \
				_CVM_BYTE(COP_BREAK); \
				_CVM_BYTE((subcode)); \
			} while (0)

#else /* IL_CVM_DIRECT */

/*
 * Direct threading code generation macros.
 */

/*
 * Get the current method position.
 */
#define	CVM_POSN()		(ILCacheGetPosn(&(((ILCVMCoder *)coder)->codePosn)))

/*
 * Determine if "n" words at "addr" are valid.
 */
#define	_CVM_VALID(addr,n)	\
			((((unsigned char *)(addr)) + (n) * sizeof(void *)) <= \
				((ILCVMCoder *)coder)->codePosn.limit)

/*
 * Output a pointer value to the CVM coder buffer.
 */
#define	_CVM_PTR(value)		\
			do { \
				if(ILCacheCheckForN(&(((ILCVMCoder *)coder)->codePosn), \
									sizeof(void *))) \
				{ \
					*((void **)(((ILCVMCoder *)coder)->codePosn.ptr)) = \
						(void *)(value); \
					((ILCVMCoder *)coder)->codePosn.ptr += sizeof(void *); \
				} \
				else \
				{ \
					((ILCVMCoder *)coder)->codePosn.ptr = \
						((ILCVMCoder *)coder)->codePosn.limit; \
				} \
			} while (0)

/*
 * Output a 32-bit value to the CVM coder buffer.
 */
#define	_CVM_WORD(value)	\
			do { \
				_CVM_PTR((ILNativeUInt)(ILUInt32)(value)); \
			} while (0)

/*
 * Copy "n" bytes to the CVM coder buffer, and then round to
 * the nearest word size.
 */
#define	_CVM_COPY_ROUND(n)	\
			(((n) + sizeof(void *) - 1) & ~(sizeof(void *) - 1))
#define	_CVM_COPY(arg,n)	\
			do { \
				if(ILCacheCheckForN(&(((ILCVMCoder *)coder)->codePosn), \
									_CVM_COPY_ROUND((n)))) \
				{ \
					ILMemCpy(((ILCVMCoder *)coder)->codePosn.ptr, \
							 (arg), (n)); \
					((ILCVMCoder *)coder)->codePosn.ptr += \
						_CVM_COPY_ROUND((n)); \
				} \
				else \
				{ \
					((ILCVMCoder *)coder)->codePosn.ptr = \
						((ILCVMCoder *)coder)->codePosn.limit; \
				} \
			} while (0)

/*
 * Output a regular opcode pointer to the CVM coder buffer.
 */
#define	_CVM_OPCODE(value)	\
			do { \
				_CVM_PTR(CVM_LABEL_FOR_OPCODE((value))); \
			} while (0)

/*
 * Output a prefixed opcode pointer to the CVM coder buffer.
 */
#define	_CVMP_OPCODE(value)	\
			do { \
				_CVM_PTR(CVMP_LABEL_FOR_OPCODE((value))); \
			} while (0)

/*
 * Output a wide instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_WIDE(opcode,value)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((value)); \
			} while (0)

/*
 * Output a double wide instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_DWIDE(opcode,value1,value2)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
			} while (0)

/*
 * Output a double wide with pointer instruction to the CVM coder buffer.
 */
#define	_CVM_OUT_DWIDE_PTR(opcode,value1,value2,value3)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
				_CVM_PTR((value3)); \
			} while (0)

/*
 * Output a return instruction.
 */
#define	_CVM_OUT_RETURN(size)	\
			do { \
				if(((ILCVMCoder*)coder)->flags & IL_CODER_FLAG_METHOD_TRACE)\
				{\
					CVMP_OUT_WORD(COP_PREFIX_TRACE_OUT,0);\
				}\
				if(((ILCVMCoder *)coder)->debugEnabled) \
				{ \
					_CVM_OUT_BREAK(IL_BREAK_EXIT_METHOD); \
				} \
				if((size) == 0) \
				{ \
					_CVM_OUT_NONE(COP_RETURN); \
				} \
				else if((size) == 1) \
				{ \
					_CVM_OUT_NONE(COP_RETURN_1); \
				} \
				else if((size) == 2) \
				{ \
					_CVM_OUT_NONE(COP_RETURN_2); \
				} \
				else \
				{ \
					_CVM_OUT_WORD(COP_RETURN_N, (size)); \
				} \
			} while (0)

/*
 * Output an instruction that takes no parameters.
 */
#define	_CVM_OUT_NONE(opcode)	_CVM_OPCODE((opcode))
#define	_CVMP_OUT_NONE(opcode)	_CVMP_OPCODE((opcode))

/*
 * Output an instruction that has a byte argument.
 */
#define	_CVM_OUT_BYTE(opcode,value)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((ILInt32)(value)); \
			} while (0)

/*
 * Output an instruction that has two byte arguments.
 */
#define	_CVM_OUT_BYTE2(opcode,value1,value2)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((ILInt32)(value1)); \
				_CVM_WORD((ILInt32)(value2)); \
			} while (0)

/*
 * Output an instruction that has a word argument.
 */
#define	_CVM_OUT_WORD(opcode,value)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((value)); \
			} while (0)
#define	_CVMP_OUT_WORD(opcode,value)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_WORD((value)); \
			} while (0)

/*
 * Output an instruction that has two word arguments.
 */
#define	_CVMP_OUT_WORD2(opcode,value1,value2)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
			} while (0)

/*
 * Output an instruction that has a pointer argument.
 */
#define	_CVM_OUT_PTR(opcode,value)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_PTR((value)); \
			} while (0)
#define	_CVMP_OUT_PTR(opcode,value)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_PTR((value)); \
			} while (0)

/*
 * Output an instruction that has two pointer arguments.
 */
#define	_CVM_OUT_PTR2(opcode,value1,value2)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_PTR((value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has a word argument and a pointer argument.
 */
#define	_CVMP_OUT_WORD_PTR(opcode,value1,value2)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has two word arguments and a pointer argument.
 */
#define	_CVMP_OUT_WORD2_PTR(opcode,value1,value2,value3)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
				_CVM_PTR((value3)); \
			} while (0)

/*
 * Output an instruction that has two word arguments and two pointer arguments.
 */
#define	_CVMP_OUT_WORD2_PTR2(opcode,value1,value2,value3,value4)	\
			do { \
				_CVMP_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_WORD((value2)); \
				_CVM_PTR((value3)); \
				_CVM_PTR((value4)); \
			} while (0)

/*
 * Output an instruction that has a wide argument and a pointer argument.
 */
#define	_CVM_OUT_WIDE_PTR(opcode,value1,value2)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_WORD((value1)); \
				_CVM_PTR((value2)); \
			} while (0)

/*
 * Output an instruction that has a "long" argument.
 */
#define	_CVM_OUT_LONG(opcode,arg)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_COPY((arg), 8); \
			} while (0)

/*
 * Output an instruction that has a "float" argument.
 */
#define	_CVM_OUT_FLOAT(opcode,arg)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_COPY((arg), 4); \
			} while (0)

/*
 * Output an instruction that has a "double" argument.
 */
#define	_CVM_OUT_DOUBLE(opcode,arg)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_COPY((arg), 8); \
			} while (0)

/*
 * Output a short branch from the current position.
 */
#define	_CVM_OUT_BRANCH(opcode,offset)	\
			do { \
				unsigned char *_posn = CVM_POSN(); \
				_CVM_OPCODE((opcode)); \
				_CVM_PTR(_posn + (ILNativeInt)(offset)); \
			} while (0)

/*
 * Output a long branch from the current position.
 */
#define	_CVM_OUT_BRANCH_LONG(opcode,offset)	\
			_CVM_OUT_BRANCH((opcode), (offset))

/*
 * Output a branch placeholder at the current position.
 */
#define	_CVM_OUT_BRANCH_PLACEHOLDER(opcode)	\
			do { \
				_CVM_OPCODE((opcode)); \
				_CVM_PTR(0); \
			} while (0)

/*
 * Backpatch a short branch.
 */
#define	_CVM_BACKPATCH_BRANCH(pc,relative)	\
			do { \
				if(_CVM_VALID((pc), 2)) \
				{ \
					((void **)(pc))[1] = \
						(void *)((pc) + (ILNativeInt)(relative)); \
				} \
			} while (0)

/*
 * Backpatch a long branch.
 */
#define	_CVM_BACKPATCH_BRANCH_LONG(pc,relative)	\
			CVM_BACKPATCH_BRANCH((pc), (relative))

/*
 * Compute the offset of the default switch case from the
 * number of entries.
 */
#define	_CVM_DEFCASE_OFFSET(numEntries)	\
			(((numEntries) + 3) * sizeof(void *))

/*
 * Output the head of a switch instruction.
 */
#define	_CVM_OUT_SWHEAD(numEntries,defCase)	\
			do { \
				unsigned char *_posn = CVM_POSN(); \
				_CVM_OPCODE(COP_SWITCH); \
				_CVM_WORD((numEntries)); \
				_CVM_PTR(_posn + (ILNativeInt)(defCase)); \
			} while (0)

/*
 * Output a switch entry at the current position.
 */
#define	_CVM_OUT_SWENTRY(swstart,relative)	\
			do { \
				_CVM_PTR((swstart) + (ILNativeInt)(relative)); \
			} while (0)

/*
 * Output a switch entry placeholder at the current position.
 */
#define	_CVM_OUT_SWENTRY_PLACEHOLDER()	\
			do { \
				_CVM_PTR(0); \
			} while (0)

/*
 * Backpatch a switch entry.
 */
#define	_CVM_BACKPATCH_SWENTRY(swstart,relative,writepc)	\
			do { \
				if(_CVM_VALID((writepc), 1)) \
				{ \
					*((void **)(writepc)) = (void *)((swstart) + \
						(ILNativeInt)(ILInt32)(relative)); \
				} \
			} while (0)

/*
 * Output the start of a try header.
 */
#define	_CVM_OUT_TRY(start,end)	\
			do { \
				_CVM_WORD((start)); \
				_CVM_WORD((end)); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a try header with the length value.
 */
#define	_CVM_BACKPATCH_TRY(trypc)	\
			do { \
				if(_CVM_VALID((trypc), 3)) \
				{ \
					((void **)(trypc))[2] = (void *)(ILNativeUInt) \
						(ILUInt32)(CVM_POSN() - (trypc)); \
				} \
			} while (0)

/*
 * Output a "pushdown" instruction placeholder.
 */
#define	_CVM_OUT_PUSHDOWN()		\
			do { \
				_CVM_OPCODE(COP_PUSHDOWN); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a "pushdown" instruction.
 */
#define	_CVM_BACKPATCH_PUSHDOWN(pushpc,value)	\
			do { \
				if(_CVM_VALID((pushpc), 2)) \
				{ \
					((void **)(pushpc))[1] = (void *)(ILNativeUInt) \
						(ILUInt32)(value); \
				} \
			} while (0)

/*
 * Output a "ckheight" instruction placeholder.
 */
#define	_CVM_OUT_CKHEIGHT()		\
			do { \
				_CVM_OPCODE(COP_CKHEIGHT_N); \
				_CVM_WORD(0); \
			} while (0)

/*
 * Backpatch a "ckheight" instruction.
 */
#define	_CVM_BACKPATCH_CKHEIGHT(ckpc,value)	\
			do { \
				if(_CVM_VALID((ckpc), 2)) \
				{ \
					if((value) <= 8) \
					{ \
						*((void **)(ckpc)) = \
							CVM_LABEL_FOR_OPCODE(COP_CKHEIGHT); \
					} \
					else \
					{ \
						((void **)(ckpc))[1] = (void *)(ILNativeUInt) \
							(ILUInt32)(value); \
					} \
				} \
			} while (0)

/*
 * Output a "jump over" instruction to skip a non-constructor entry point.
 */
#define	_CVM_OUT_JUMPOVER(pc,relative)	\
			do { \
				if(_CVM_VALID((pc), 2)) \
				{ \
					((void **)(pc))[0] = \
						CVM_LABEL_FOR_OPCODE(COP_BR); \
					((void **)(pc))[1] = (void *) \
						((pc) + (ILNativeInt)(ILInt32)(relative)); \
				} \
			} while (0)

/*
 * Output a "break" instruction.
 */
#define	_CVM_OUT_BREAK(subcode)	\
			do { \
				_CVM_OUT_WORD(COP_BREAK, (subcode)); \
			} while (0)

#endif /* IL_CVM_DIRECT */

/*
 * Public versions of the above macros.  If we are reducing code
 * sizes, then we define them as functions instead.
 */
#ifndef	IL_CONFIG_REDUCE_CODE

#define	CVM_OUT_WIDE(opcode,value)	_CVM_OUT_WIDE((opcode), (value))
#define	CVM_OUT_DWIDE(opcode,value1,value2)	\
			_CVM_OUT_DWIDE((opcode), (value1), (value2))
#define	CVM_OUT_DWIDE_PTR(opcode,value1,value2,value3)	\
			_CVM_OUT_DWIDE_PTR((opcode), (value1), (value2), (value3))
#define	CVM_OUT_RETURN(size)		do {\
				if(((ILCVMCoder*)coder)->flags & IL_CODER_FLAG_METHOD_TRACE)\
				{\
					CVMP_OUT_WORD(COP_PREFIX_TRACE_OUT,0);\
				}\
				_CVM_OUT_RETURN((size));\
				}while(0)
#define	CVM_OUT_NONE(opcode)		_CVM_OUT_NONE((opcode))
#define	CVMP_OUT_NONE(opcode)		_CVMP_OUT_NONE((opcode))
#define	CVM_OUT_BYTE(opcode,value)	_CVM_OUT_BYTE((opcode), (value))
#define	CVM_OUT_BYTE2(opcode,value1,value2)	\
			_CVM_OUT_BYTE2((opcode), (value1), (value2))
#define	CVM_OUT_WORD(opcode,value)	_CVM_OUT_WORD((opcode), (value))
#define	CVMP_OUT_WORD(opcode,value)	_CVMP_OUT_WORD((opcode), (value))
#define	CVMP_OUT_WORD2(opcode,value1,value2)	\
			_CVMP_OUT_WORD2((opcode), (value1), (value2))
#define	CVM_OUT_PTR(opcode,value)	_CVM_OUT_PTR((opcode), (value))
#define	CVMP_OUT_PTR(opcode,value)	_CVMP_OUT_PTR((opcode), (value))
#define	CVM_OUT_PTR2(opcode,value1,value2)	\
			_CVM_OUT_PTR2((opcode), (value1), (value2))
#define	CVMP_OUT_WORD_PTR(opcode,value1,value2)	\
			_CVMP_OUT_WORD_PTR((opcode), (value1), (value2))
#define	CVMP_OUT_WORD2_PTR(opcode,value1,value2,value3)	\
			_CVMP_OUT_WORD2_PTR((opcode), (value1), (value2), (value3))
#define	CVMP_OUT_WORD2_PTR2(opcode,value1,value2,value3,value4)	\
	_CVMP_OUT_WORD2_PTR2((opcode), (value1), (value2), (value3), (value4))
#define	CVM_OUT_WIDE_PTR(opcode,value1,value2)	\
			_CVM_OUT_WIDE_PTR((opcode), (value1), (value2))
#define	CVM_OUT_LONG(opcode,arg)	_CVM_OUT_LONG((opcode), (arg))
#define	CVM_OUT_FLOAT(opcode,arg)	_CVM_OUT_FLOAT((opcode), (arg))
#define	CVM_OUT_DOUBLE(opcode,arg)	_CVM_OUT_DOUBLE((opcode), (arg))
#define	CVM_OUT_BRANCH(opcode,offset)	\
			_CVM_OUT_BRANCH((opcode), (offset))
#define	CVM_OUT_BRANCH_LONG(opcode,offset)	\
			_CVM_OUT_BRANCH_LONG((opcode), (offset))
#define	CVM_OUT_BRANCH_PLACEHOLDER(opcode)	\
			_CVM_OUT_BRANCH_PLACEHOLDER((opcode))
#define	CVM_BACKPATCH_BRANCH(pc,relative)	\
			_CVM_BACKPATCH_BRANCH((pc), (relative))
#define	CVM_BACKPATCH_BRANCH_LONG(pc,relative)	\
			_CVM_BACKPATCH_BRANCH_LONG((pc), (relative))
#define	CVM_DEFCASE_OFFSET(numEntries)	\
			_CVM_DEFCASE_OFFSET((numEntries))
#define	CVM_OUT_SWHEAD(numEntries,defCase)	\
			_CVM_OUT_SWHEAD((numEntries), (defCase))
#define	CVM_OUT_SWENTRY(swstart,relative)	\
			_CVM_OUT_SWENTRY((swstart), (relative))
#define	CVM_OUT_SWENTRY_PLACEHOLDER()	\
			_CVM_OUT_SWENTRY_PLACEHOLDER()
#define	CVM_BACKPATCH_SWENTRY(swstart,relative,writepc)	\
			_CVM_BACKPATCH_SWENTRY((swstart), (relative), (writepc))
#define	CVM_OUT_TRY(start,end)	\
			_CVM_OUT_TRY((start), (end))
#define	CVM_BACKPATCH_TRY(trypc)	\
			_CVM_BACKPATCH_TRY((trypc))
#define	CVM_OUT_PUSHDOWN()		\
			_CVM_OUT_PUSHDOWN()
#define	CVM_BACKPATCH_PUSHDOWN(pushpc,value)	\
			_CVM_BACKPATCH_PUSHDOWN((pushpc), (value))
#define	CVM_OUT_CKHEIGHT()		\
			_CVM_OUT_CKHEIGHT()
#define	CVM_BACKPATCH_CKHEIGHT(ckpc,value)	\
			_CVM_BACKPATCH_CKHEIGHT((ckpc), (value))
#define	CVM_OUT_JUMPOVER(pc,relative)	\
			_CVM_OUT_JUMPOVER((pc), (relative))
#define	CVM_OUT_BREAK(subcode)	\
			_CVM_OUT_BREAK((subcode))

#else /* IL_CONFIG_REDUCE_CODE */

static void cvm_out_wide(ILCoder *coder, int opcode, ILInt32 value)
{
	_CVM_OUT_WIDE(opcode, value);
}
static void cvm_out_dwide(ILCoder *coder, int opcode,
						  ILInt32 value1, ILInt32 value2)
{
	_CVM_OUT_DWIDE(opcode, value1, value2);
}
static void cvm_out_dwide_ptr(ILCoder *coder, int opcode,
						      ILInt32 value1, ILInt32 value2,
							  void *value3)
{
	_CVM_OUT_DWIDE_PTR(opcode, value1, value2, value3);
}
static void cvm_out_return(ILCoder *coder, ILInt32 size)
{
	_CVM_OUT_RETURN(size);
}
static void cvm_out_none(ILCoder *coder, int opcode)
{
	_CVM_OUT_NONE(opcode);
}
static void cvmp_out_none(ILCoder *coder, int opcode)
{
	_CVMP_OUT_NONE(opcode);
}
static void cvm_out_byte(ILCoder *coder, int opcode, ILInt32 value)
{
	_CVM_OUT_BYTE(opcode, value);
}
#ifdef IL_NATIVE_INT64
static void cvm_out_byte2(ILCoder *coder, int opcode,
						  ILInt32 value1, ILInt32 value2)
{
	_CVM_OUT_BYTE2(opcode, value1, value2);
}
#endif
static void cvm_out_word(ILCoder *coder, int opcode, ILInt32 value)
{
	_CVM_OUT_WORD(opcode, value);
}
static void cvmp_out_word(ILCoder *coder, int opcode, ILInt32 value)
{
	_CVMP_OUT_WORD(opcode, value);
}
static void cvmp_out_word2(ILCoder *coder, int opcode,
						   ILInt32 value1, ILInt32 value2)
{
	_CVMP_OUT_WORD2(opcode, value1, value2);
}
static void cvm_out_ptr(ILCoder *coder, int opcode, void *value)
{
	_CVM_OUT_PTR(opcode, value);
}
static void cvmp_out_ptr(ILCoder *coder, int opcode, void *value)
{
	_CVMP_OUT_PTR(opcode, value);
}
static void cvm_out_ptr2(ILCoder *coder, int opcode, void *value1, void *value2)
{
	_CVM_OUT_PTR2(opcode, value1, value2);
}
static void cvmp_out_word2_ptr(ILCoder *coder, int opcode,
							   ILInt32 value1, ILInt32 value2, void *value3)
{
	_CVMP_OUT_WORD2_PTR(opcode, value1, value2, value3);
}
static void cvmp_out_word2_ptr2(ILCoder *coder, int opcode,
							    ILInt32 value1, ILInt32 value2,
								void *value3, void *value4)
{
	_CVMP_OUT_WORD2_PTR2(opcode, value1, value2, value3, value4);
}

#define	CVM_OUT_WIDE(opcode,value)	\
	do { \
		cvm_out_wide(((ILCoder *)coder), (opcode), (ILInt32)(value)); \
	} while (0)
#define	CVM_OUT_DWIDE(opcode,value1,value2)	\
	do { \
		cvm_out_dwide(((ILCoder *)coder), (opcode), \
					  (ILInt32)(value1), (ILInt32)(value2)); \
	} while (0)
#define	CVM_OUT_DWIDE_PTR(opcode,value1,value2,value3)	\
	do { \
		cvm_out_dwide_ptr(((ILCoder *)coder), (opcode), \
					      (ILInt32)(value1), (ILInt32)(value2), (value3)); \
	} while (0)
#define	CVM_OUT_RETURN(size)	\
	do { \
		cvm_out_return(((ILCoder *)coder), (ILInt32)(size)); \
	} while (0)
#define	CVM_OUT_NONE(opcode)	\
	do { \
		cvm_out_none(((ILCoder *)coder), (opcode)); \
	} while (0)
#define	CVMP_OUT_NONE(opcode)	\
	do { \
		cvmp_out_none(((ILCoder *)coder), (opcode)); \
	} while (0)
#define	CVM_OUT_BYTE(opcode,value)	\
	do { \
		cvm_out_byte(((ILCoder *)coder), (opcode), (ILInt32)(value)); \
	} while (0)
#define	CVM_OUT_BYTE2(opcode,value1,value2)	\
	do { \
		cvm_out_byte2(((ILCoder *)coder), (opcode), \
					  (ILInt32)(value1), (ILInt32)(value2)); \
	} while (0)
#define	CVM_OUT_WORD(opcode,value)	\
	do { \
		cvm_out_word(((ILCoder *)coder), (opcode), (ILInt32)(value)); \
	} while (0)
#define	CVMP_OUT_WORD(opcode,value)	\
	do { \
		cvmp_out_word(((ILCoder *)coder), (opcode), (ILInt32)(value)); \
	} while (0)
#define	CVMP_OUT_WORD2(opcode,value1,value2)	\
	do { \
		cvmp_out_word2(((ILCoder *)coder), (opcode), \
					   (ILInt32)(value1), (ILInt32)(value2)); \
	} while (0)
#define	CVM_OUT_PTR(opcode,value)	\
	do { \
		cvm_out_ptr(((ILCoder *)coder), (opcode), (value)); \
	} while (0)
#define	CVMP_OUT_PTR(opcode,value)	\
	do { \
		cvmp_out_ptr(((ILCoder *)coder), (opcode), (value)); \
	} while (0)
#define	CVM_OUT_PTR2(opcode,value1,value2)	\
	do { \
		cvm_out_ptr2(((ILCoder *)coder), (opcode), (value1), (value2)); \
	} while (0)
#define	CVMP_OUT_WORD_PTR(opcode,value1,value2)	\
			_CVMP_OUT_WORD_PTR((opcode), (value1), (value2))
#define	CVMP_OUT_WORD2_PTR(opcode,value1,value2,value3)	\
	do { \
		cvmp_out_word2_ptr(((ILCoder *)coder), (opcode), \
						   (ILInt32)(value1), (ILInt32)(value2), (value3)); \
	} while (0)
#define	CVMP_OUT_WORD2_PTR2(opcode,value1,value2,value3,value4)	\
	do { \
		cvmp_out_word2_ptr2(((ILCoder *)coder), (opcode), \
						    (ILInt32)(value1), (ILInt32)(value2), \
							(value3), (value4)); \
	} while (0)
#define	CVM_OUT_WIDE_PTR(opcode,value1,value2)	\
			_CVM_OUT_WIDE_PTR((opcode), (value1), (value2))
#define	CVM_OUT_LONG(opcode,arg)	_CVM_OUT_LONG((opcode), (arg))
#define	CVM_OUT_FLOAT(opcode,arg)	_CVM_OUT_FLOAT((opcode), (arg))
#define	CVM_OUT_DOUBLE(opcode,arg)	_CVM_OUT_DOUBLE((opcode), (arg))
#define	CVM_OUT_BRANCH(opcode,offset)	\
			_CVM_OUT_BRANCH((opcode), (offset))
#define	CVM_OUT_BRANCH_LONG(opcode,offset)	\
			_CVM_OUT_BRANCH_LONG((opcode), (offset))
#define	CVM_OUT_BRANCH_PLACEHOLDER(opcode)	\
			_CVM_OUT_BRANCH_PLACEHOLDER((opcode))
#define	CVM_BACKPATCH_BRANCH(pc,relative)	\
			_CVM_BACKPATCH_BRANCH((pc), (relative))
#define	CVM_BACKPATCH_BRANCH_LONG(pc,relative)	\
			_CVM_BACKPATCH_BRANCH_LONG((pc), (relative))
#define	CVM_DEFCASE_OFFSET(numEntries)	\
			_CVM_DEFCASE_OFFSET((numEntries))
#define	CVM_OUT_SWHEAD(numEntries,defCase)	\
			_CVM_OUT_SWHEAD((numEntries), (defCase))
#define	CVM_OUT_SWENTRY(swstart,relative)	\
			_CVM_OUT_SWENTRY((swstart), (relative))
#define	CVM_OUT_SWENTRY_PLACEHOLDER()	\
			_CVM_OUT_SWENTRY_PLACEHOLDER()
#define	CVM_BACKPATCH_SWENTRY(swstart,relative,writepc)	\
			_CVM_BACKPATCH_SWENTRY((swstart), (relative), (writepc))
#define	CVM_OUT_TRY(start,end)	\
			_CVM_OUT_TRY((start), (end))
#define	CVM_BACKPATCH_TRY(trypc)	\
			_CVM_BACKPATCH_TRY((trypc))
#define	CVM_OUT_PUSHDOWN()		\
			_CVM_OUT_PUSHDOWN()
#define	CVM_BACKPATCH_PUSHDOWN(pushpc,value)	\
			_CVM_BACKPATCH_PUSHDOWN((pushpc), (value))
#define	CVM_OUT_CKHEIGHT()		\
			_CVM_OUT_CKHEIGHT()
#define	CVM_BACKPATCH_CKHEIGHT(ckpc,value)	\
			_CVM_BACKPATCH_CKHEIGHT((ckpc), (value))
#define	CVM_OUT_JUMPOVER(pc,relative)	\
			_CVM_OUT_JUMPOVER((pc), (relative))
#define	CVM_OUT_BREAK(subcode)	\
			_CVM_OUT_BREAK((subcode))

#endif /* IL_CONFIG_REDUCE_CODE */

#ifdef	__cplusplus
};
#endif

#endif	/* _ENGINE_CVMC_GEN_H */
