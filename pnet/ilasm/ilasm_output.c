/*
 * ilasm_output.c - Output method code.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_system.h"
#include "il_utils.h"
#include "il_debug.h"
#include "ilasm_build.h"
#include "ilasm_output.h"
#include "il_opcodes.h"
#include "il_jopcodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

extern int   ILAsmShortInsns;	/* ilasm_main.c */
extern char *ILAsmFilename;		/* ilasm_scanner.l */
extern long  ILAsmLineNum;		/* ilasm_scanner.l */
extern int   ILAsmErrors;		/* ilasm_scanner.l */
extern int   ILAsmParseJava;		/* ilasm_scanner.l */

ILWriter *ILAsmWriter;

/* ilasm_grammar.y */
void ILAsmPrintMessage(const char *filename, long linenum,
					   const char *format, ...);
void ILAsmOutOfMemory(void);

/*
 * Output buffer for the current method.
 */
static unsigned char *buffer = 0;
static ILUInt32		  offset = 0;
static ILUInt32		  length = 0;

/*
 * Other information for the current method.
 */
static ILType        *localVars = 0;
static ILUInt32		  maxStack = 8;
static int			  initLocals = 0;
static ILUInt32		  localIndex = 0;
typedef struct _tagLocalBlock
{
	const char *blockStart;
	const char *blockEnd;
	struct _tagLocalBlock *outer;
	struct _tagLocalBlock *next;

} LocalBlock;
typedef struct _tagLocalInfo
{
	LocalBlock *block;
	const char *name;
	ILUInt32 index;
	int isParam;
	struct _tagLocalInfo *next;

} LocalInfo;
static LocalInfo	 *localNames = 0;
static LocalBlock	 *localBlocks = 0;
static LocalBlock	 *localCurrentBlock = 0;
static ILAsmOutException *exceptionList = 0;
static ILAsmOutException *lastException = 0;
static unsigned long  numExceptions = 0;
static int            haveDebug = 0;
static ILUInt32 switchCountOffset = 0;
static ILUInt32 ssaStartOffset = 0;
static ILUInt32 switchStartOffset = 0;
static ILUInt32 switchHighOffset = 0;
static ILUInt32 switchNPairsOffset = 0;
static const char* defaultTableSwitchLabel = 0;
static const char* defaultLookupSwitchLabel = 0;

/*
 * Output a single byte to the buffer.
 */
static void OutByte(unsigned char byte)
{
	unsigned char *buf = (unsigned char *)ILRealloc(buffer, length + 1024);
	if(!buf)
	{
		ILAsmOutOfMemory();
	}
	buffer = buf;
	length += 1024;
	buffer[offset++] = byte;
}
#define	OUT_BYTE(byte)	\
			do { \
				if(offset < length) \
				{ \
					buffer[offset++] = (unsigned char)(byte); \
				} \
				else \
				{ \
					OutByte((unsigned char)(byte)); \
				} \
			} while (0)

/*
 * List of active labels.
 */
typedef struct _tagLabelRef
{
	ILUInt32	  posn;
	ILUInt8		  offset;
	ILUInt8       addrLen;
	ILUInt8		  squashed;
	ILUInt8		  isSwitchRef;
	ILUInt32	  switchStart;
	char		 *filename;
	long		  linenum;
	struct _tagLabelRef *next;

} LabelRef;
typedef struct _tagLabelInfo
{
	const char	 *name;
	ILUInt32	  address;
	int			  defined : 1;
	int			  tokenFixup : 1;
	LabelRef     *refs;
	struct _tagLabelInfo *next;
	const char	 *debugFilename;
	ILUInt32	  debugLine;
	ILUInt32	  debugColumn;

} LabelInfo;
static ILMemPool labelPool;
static ILMemPool labelRefPool;
static int       initLabelPool = 0;
static LabelInfo *labels = 0;
static int		 haveColumnInfo = 0;

/*
 * Emit a warning when an attempt was made to encode an
 * instruction that was too long and "--no-short-insns"
 * was supplied on the command-line.
 */
static void ShortInsnsWarning(void)
{
	ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
					  "warning: argument too large for short instruction");
}

/*
 * Emit an error when a large argument is used.
 */
static void LargeArgError(void)
{
	ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
					  "argument too large for instruction");
	ILAsmErrors = 1;
}

void ILAsmOutSimple(ILInt32 opcode)
{
	if(opcode < 0x0100)
	{
		OUT_BYTE(opcode);
	}
	else
	{
		OUT_BYTE(opcode >> 8);
		OUT_BYTE(opcode);
	}
}

/*
 * Output a variable reference when a long instruction is used.
 */
static void OutLongVar(ILInt32 lopcode, ILInt32 sopcode,
					   ILInt32 topcode, ILInt64 num)
{
	if(ILAsmShortInsns && sopcode && num >= 0 && num < 256)
	{
		if(topcode && num < 4)
		{
			OUT_BYTE(topcode + num);
		}
		else
		{
			if(sopcode >= 0x0100)
			{
				OUT_BYTE(IL_OP_PREFIX);
			}
			OUT_BYTE(sopcode);
			OUT_BYTE(num);
		}
	}
	else if(num >= 0 && num < (ILInt64)65536)
	{
		if(lopcode >= 0x0100)
		{
			OUT_BYTE(IL_OP_PREFIX);
		}
		OUT_BYTE(lopcode);
		OUT_BYTE(num);
		OUT_BYTE(num >> 8);
	}
	else
	{
		LargeArgError();
	}
}

/*
 * Output a variable reference when a short instruction is used.
 */
static void OutShortVar(ILInt32 lopcode, ILInt32 sopcode,
					    ILInt32 topcode, ILInt64 num)
{
	if(ILAsmShortInsns && topcode && num >= 0 && num < 4)
	{
		OUT_BYTE(topcode + num);
	}
	else if(num >= 0 && num < 256)
	{
		if(sopcode >= 0x0100)
		{
			OUT_BYTE(IL_OP_PREFIX);
		}
		OUT_BYTE(sopcode);
		OUT_BYTE(num);
	}
	else if(num >= 0 && num < (ILInt64)65536)
	{
		if(!ILAsmShortInsns)
		{
			ShortInsnsWarning();
		}
		if(lopcode >= 0x0100)
		{
			OUT_BYTE(IL_OP_PREFIX);
		}
		OUT_BYTE(IL_OP_PREFIX);
		OUT_BYTE(lopcode);
		OUT_BYTE(num);
		OUT_BYTE(num >> 8);
	}
	else
	{
		LargeArgError();
	}
}

void ILAsmOutReset(void)
{
	LocalInfo *local, *nextLocal;
	LocalBlock *block, *nextBlock;
	ILAsmOutException *exception, *nextException;

	/* Destroy allocated data structures */
	if(ILAsmWriter)
	{
		ILWriterDestroy(ILAsmWriter);
	}
	if(buffer)
	{
		ILFree(buffer);
	}
	local = localNames;
	while(local != 0)
	{
		nextLocal = local->next;
		ILFree(local);
		local = nextLocal;
	}
	block = localBlocks;
	while(block != 0)
	{
		nextBlock = block->next;
		ILFree(block);
		block = nextBlock;
	}
	exception = exceptionList;
	while(exception != 0)
	{
		nextException = exception->next;
		ILFree(exception);
		exception = nextException;
	}
	if(initLabelPool)
	{
		ILMemPoolClear(&labelPool);
		ILMemPoolClear(&labelRefPool);
	}

	/* Set the global variables back to their defaults */
	ILAsmWriter = 0;
	buffer = 0;
	length = 0;
	localVars = 0;
	maxStack = 8;
	initLocals = 0;
	localIndex = 0;
	localNames = 0;
	localBlocks = 0;
	exceptionList = 0;
	lastException = 0;
	numExceptions = 0;
	haveDebug = 0;
	switchCountOffset = 0;
	ssaStartOffset = 0;
	switchStartOffset = 0;
	switchHighOffset = 0;
	switchNPairsOffset = 0;
	defaultTableSwitchLabel = 0;
	defaultLookupSwitchLabel = 0;
	initLabelPool = 0;
	labels = 0;
	haveColumnInfo = 0;
}

void ILAsmOutCreate(FILE *stream, int seekable, int type, int flags)
{
	ILAsmWriter = ILWriterCreate(stream, seekable, type, flags);
	if(!ILAsmWriter)
	{
		ILAsmOutOfMemory();
	}
}

int ILAsmOutDestroy(void)
{
	int error;
	ILAsmBuildEndModule();
	ILWriterOutputMetadata(ILAsmWriter, ILAsmImage);
	error = ILWriterDestroy(ILAsmWriter);
	ILAsmWriter = 0;
	if(error < 0)
	{
		ILAsmOutOfMemory();
	}
	return error;
}

void ILAsmOutVar(ILInt32 opcode, ILInt64 num)
{
	switch(opcode)
	{
		case IL_OP_ANN_DEAD:
		{
			OutLongVar(opcode, 0, 0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_LDARG:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_LDARG, IL_OP_LDARG_S,
					   IL_OP_LDARG_0, num);
		}
		break;

		case IL_OP_LDARG_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_LDARG, IL_OP_LDARG_S,
					    IL_OP_LDARG_0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_LDARGA:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_LDARGA, IL_OP_LDARGA_S, 0, num);
		}
		break;

		case IL_OP_LDARGA_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_LDARGA, IL_OP_LDARGA_S, 0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_LDLOC:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_LDLOC, IL_OP_LDLOC_S,
					   IL_OP_LDLOC_0, num);
		}
		break;

		case IL_OP_LDLOC_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_LDLOC, IL_OP_LDLOC_S,
					    IL_OP_LDLOC_0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_LDLOCA:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_LDLOCA, IL_OP_LDLOCA_S, 0, num);
		}
		break;

		case IL_OP_LDLOCA_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_LDLOCA, IL_OP_LDLOCA_S, 0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_STARG:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_STARG, IL_OP_STARG_S, 0, num);
		}
		break;

		case IL_OP_STARG_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_STARG, IL_OP_STARG_S, 0, num);
		}
		break;

		case 0xFE00 | IL_PREFIX_OP_STLOC:
		{
			OutLongVar(0xFE00 | IL_PREFIX_OP_STLOC, IL_OP_STLOC_S,
					   IL_OP_STLOC_0, num);
		}
		break;

		case IL_OP_STLOC_S:
		{
			OutShortVar(0xFE00 | IL_PREFIX_OP_STLOC, IL_OP_STLOC_S,
					    IL_OP_STLOC_0, num);
		}
		break;
	}
}

void ILAsmOutInt(ILInt32 opcode, ILInt64 value)
{
	if(opcode == IL_OP_LDC_I4 || opcode == IL_OP_LDC_I4_S)
	{
		if(ILAsmShortInsns && value >= -1 && value <= 8)
		{
			if(value == -1)
			{
				OUT_BYTE(IL_OP_LDC_I4_M1);
			}
			else
			{
				OUT_BYTE(IL_OP_LDC_I4_0 + value);
			}
		}
		else if((ILAsmShortInsns || opcode == IL_OP_LDC_I4_S) &&
		        value >= -128 && value <= 127)
		{
			OUT_BYTE(IL_OP_LDC_I4_S);
			OUT_BYTE(value);
		}
		else if(value >= -((ILInt64)0x80000000) &&
		        value <= ((ILInt64)0xFFFFFFFF))
		{
			if(!ILAsmShortInsns && opcode == IL_OP_LDC_I4_S)
			{
				ShortInsnsWarning();
			}
			OUT_BYTE(IL_OP_LDC_I4);
			OUT_BYTE(value);
			OUT_BYTE(value >> 8);
			OUT_BYTE(value >> 16);
			OUT_BYTE(value >> 24);
		}
		else
		{
			LargeArgError();
		}
	}
	else if(opcode == IL_OP_LDC_I8)
	{
		if(ILAsmShortInsns)
		{
			if(value == -1)
			{
				OUT_BYTE(IL_OP_LDC_I4_M1);
				OUT_BYTE(IL_OP_CONV_I8);
			}
			else if(value >= 0 && value <= 8)
			{
				OUT_BYTE(IL_OP_LDC_I4_0 + value);
				OUT_BYTE(IL_OP_CONV_I8);
			}
			else if(value >= -((ILInt64)0x80000000) &&
					value <= ((ILInt64)0x7FFFFFFF))
			{
				OUT_BYTE(IL_OP_LDC_I4);
				OUT_BYTE(value);
				OUT_BYTE(value >> 8);
				OUT_BYTE(value >> 16);
				OUT_BYTE(value >> 24);
				OUT_BYTE(IL_OP_CONV_I8);
			}
			else if(value >= 0 &&
					value <= ((ILInt64)0xFFFFFFFF))
			{
				OUT_BYTE(IL_OP_LDC_I4);
				OUT_BYTE(value);
				OUT_BYTE(value >> 8);
				OUT_BYTE(value >> 16);
				OUT_BYTE(value >> 24);
				OUT_BYTE(IL_OP_CONV_U8);
			}
			else
			{
				OUT_BYTE(IL_OP_LDC_I8);
				OUT_BYTE(value);
				OUT_BYTE(value >> 8);
				OUT_BYTE(value >> 16);
				OUT_BYTE(value >> 24);
				OUT_BYTE(value >> 32);
				OUT_BYTE(value >> 40);
				OUT_BYTE(value >> 48);
				OUT_BYTE(value >> 56);
			}
		}
		else
		{
			OUT_BYTE(IL_OP_LDC_I8);
			OUT_BYTE(value);
			OUT_BYTE(value >> 8);
			OUT_BYTE(value >> 16);
			OUT_BYTE(value >> 24);
			OUT_BYTE(value >> 32);
			OUT_BYTE(value >> 40);
			OUT_BYTE(value >> 48);
			OUT_BYTE(value >> 56);
		}
	}
	else if(opcode == (0xFE00 | IL_PREFIX_OP_UNALIGNED))
	{
		OUT_BYTE(IL_OP_PREFIX);
		OUT_BYTE(IL_PREFIX_OP_UNALIGNED);
		OUT_BYTE(value);
	}
}

void ILAsmOutFloat(unsigned char *bytes)
{
	OUT_BYTE(IL_OP_LDC_R4);
	OUT_BYTE(bytes[0]);
	OUT_BYTE(bytes[1]);
	OUT_BYTE(bytes[2]);
	OUT_BYTE(bytes[3]);
}

void ILAsmOutDouble(unsigned char *bytes)
{
	OUT_BYTE(IL_OP_LDC_R8);
	OUT_BYTE(bytes[0]);
	OUT_BYTE(bytes[1]);
	OUT_BYTE(bytes[2]);
	OUT_BYTE(bytes[3]);
	OUT_BYTE(bytes[4]);
	OUT_BYTE(bytes[5]);
	OUT_BYTE(bytes[6]);
	OUT_BYTE(bytes[7]);
}

/*
 * Look up a label or create a new one.
 */
static LabelInfo *GetLabel(const char *name)
{
	LabelInfo *label = labels;
	while(label != 0)
	{
		if((name && label->name == name))
		{
			return label;
		}
		label = label->next;
	}
	if(!initLabelPool)
	{
		ILMemPoolInitType(&labelPool, LabelInfo, 0);
		ILMemPoolInitType(&labelRefPool, LabelRef, 0);
		initLabelPool = 1;
	}
	label = ILMemPoolAlloc(&labelPool, LabelInfo);
	if(!label)
	{
		ILAsmOutOfMemory();
	}
	label->name = name;
	label->address = 0;
	label->defined = 0;
	label->tokenFixup = 0;
	label->refs = 0;
	label->next = labels;
	label->debugFilename = 0;
	label->debugLine = 0;
	label->debugColumn = 0;
	labels = label;
	return label;
}

void ILAsmOutToken(ILInt32 opcode, ILUInt32 token)
{
	ILAsmOutSimple(opcode);
	OUT_BYTE(token);
	OUT_BYTE(token >> 8);
	OUT_BYTE(token >> 16);
	OUT_BYTE(token >> 24);
	if((token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_REF ||
	   (token & IL_META_TOKEN_MASK) == IL_META_TOKEN_MEMBER_REF ||
	   (token & IL_META_TOKEN_MASK) == IL_META_TOKEN_FIELD_DEF ||
	   (token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF)
	{
		/* We need to record this position in the code because
		   it will need to be relocated when references are
		   compacted at the end of the assembly process */
		const char *label = ILAsmOutUniqueLabel();
		LabelInfo *info = GetLabel(label);
		info->tokenFixup = 1;
	}
}

void ILAsmOutString(ILIntString interned)
{
	ILUInt32 token;
	OUT_BYTE(IL_OP_LDSTR);
	token = ILImageAddUserString(ILAsmImage, interned.string, interned.len);
	if(!token)
	{
		ILAsmOutOfMemory();
	}
	token |= IL_META_TOKEN_STRING;
	OUT_BYTE(token);
	OUT_BYTE(token >> 8);
	OUT_BYTE(token >> 16);
	OUT_BYTE(token >> 24);
}

/*
 * Assemble a branch instruction.
 */
static void Branch(ILInt32 opcode, const char *name)
{
	LabelInfo *labelInfo;
	long delta;
	LabelRef *ref;
	ILInt32 lopcode;
	ILInt32 sopcode;
	int canUseShort;

	/* Convert the opcode into its long and short forms */
	switch(opcode)
	{
		case (0xFE00 | IL_PREFIX_OP_ANN_DATA):
		case IL_OP_ANN_DATA_S:
		{
			lopcode = (0xFE00 | IL_PREFIX_OP_ANN_DATA);
			sopcode = IL_OP_ANN_DATA_S;
		}
		break;

		case IL_OP_BEQ_S:
		case IL_OP_BEQ:
		{
			lopcode = IL_OP_BEQ;
			sopcode = IL_OP_BEQ_S;
		}
		break;

		case IL_OP_BGE_S:
		case IL_OP_BGE:
		{
			lopcode = IL_OP_BGE;
			sopcode = IL_OP_BGE_S;
		}
		break;

		case IL_OP_BGE_UN_S:
		case IL_OP_BGE_UN:
		{
			lopcode = IL_OP_BGE_UN;
			sopcode = IL_OP_BGE_UN_S;
		}
		break;

		case IL_OP_BGT_S:
		case IL_OP_BGT:
		{
			lopcode = IL_OP_BGT;
			sopcode = IL_OP_BGT_S;
		}
		break;

		case IL_OP_BGT_UN_S:
		case IL_OP_BGT_UN:
		{
			lopcode = IL_OP_BGT_UN;
			sopcode = IL_OP_BGT_UN_S;
		}
		break;

		case IL_OP_BLE_S:
		case IL_OP_BLE:
		{
			lopcode = IL_OP_BLE;
			sopcode = IL_OP_BLE_S;
		}
		break;

		case IL_OP_BLE_UN_S:
		case IL_OP_BLE_UN:
		{
			lopcode = IL_OP_BLE_UN;
			sopcode = IL_OP_BLE_UN_S;
		}
		break;

		case IL_OP_BLT_S:
		case IL_OP_BLT:
		{
			lopcode = IL_OP_BLT;
			sopcode = IL_OP_BLT_S;
		}
		break;

		case IL_OP_BLT_UN_S:
		case IL_OP_BLT_UN:
		{
			lopcode = IL_OP_BLT_UN;
			sopcode = IL_OP_BLT_UN_S;
		}
		break;

		case IL_OP_BNE_UN_S:
		case IL_OP_BNE_UN:
		{
			lopcode = IL_OP_BNE_UN;
			sopcode = IL_OP_BNE_UN_S;
		}
		break;

		case IL_OP_BR_S:
		case IL_OP_BR:
		{
			lopcode = IL_OP_BR;
			sopcode = IL_OP_BR_S;
		}
		break;

		case IL_OP_BRFALSE_S:
		case IL_OP_BRFALSE:
		{
			lopcode = IL_OP_BRFALSE;
			sopcode = IL_OP_BRFALSE_S;
		}
		break;

		case IL_OP_BRTRUE_S:
		case IL_OP_BRTRUE:
		{
			lopcode = IL_OP_BRTRUE;
			sopcode = IL_OP_BRTRUE_S;
		}
		break;

		case IL_OP_LEAVE_S:
		case IL_OP_LEAVE:
		{
			lopcode = IL_OP_LEAVE;
			sopcode = IL_OP_LEAVE_S;
		}
		break;

		default:
		{
			/* Shouldn't happen, but keep the compiler happy */
			lopcode = 0;
			sopcode = 0;
		}
		break;
	}

	/* Find the label information block */
	labelInfo = GetLabel(name);

	/* Add a reference for this branch instruction so
	   that we can fix it up again later when other
	   branches are squashed to remove redundant space */
	ref = ILMemPoolAlloc(&labelRefPool, LabelRef);
	if(!ref)
	{
		ILAsmOutOfMemory();
	}
	ref->posn = offset;
	ref->isSwitchRef = 0;
	ref->switchStart = 0;
	ref->filename = ILAsmFilename;
	ref->linenum = ILAsmLineNum;
	ref->next = labelInfo->refs;
	labelInfo->refs = ref;


	if(ILAsmParseJava) 
	{
		ref->squashed = 1; /* java labels don't need squashing */
		ref->offset = 1;
		OUT_BYTE(opcode);
		OUT_BYTE(0);
		OUT_BYTE(0);
		ref->addrLen = 2;
	} 
	else if(labelInfo->defined) 	/* Is the label defined? */
	{
		/* Yes, so generate the final instruction now */
		delta = (long)(labelInfo->address - (offset + 2));
		canUseShort = (delta >= -128 && delta < 128);
		if(ILAsmShortInsns)
		{
			/* Use the shortest form of branch possible */
			ref->squashed = 1;
			if(canUseShort)
			{
				if(sopcode >= 0x0100)
				{
					OUT_BYTE(IL_OP_PREFIX);
					ref->offset = 2;
				}
				else
				{
					ref->offset = 1;
				}
				OUT_BYTE(sopcode);
				OUT_BYTE(0);
				ref->addrLen = 1;
			}
			else
			{
				if(lopcode >= 0x0100)
				{
					OUT_BYTE(IL_OP_PREFIX);
					ref->offset = 2;
				}
				else
				{
					ref->offset = 1;
				}
				OUT_BYTE(lopcode);
				OUT_BYTE(0);
				OUT_BYTE(0);
				OUT_BYTE(0);
				OUT_BYTE(0);
				ref->addrLen = 4;
			}
		}
		else if(opcode == sopcode)
		{
			/* Always use the short-form branch */
			ref->squashed = 1;
			if(!canUseShort)
			{
				if(labelInfo->name)
				{
					ILAsmPrintMessage(ref->filename, ref->linenum,
						  "label `%s' is too far to be reached by branch",
						  labelInfo->name);
				}
				else
				{
					ILAsmPrintMessage(ref->filename, ref->linenum,
						  "absolute label `%lu' is too far to be "
				  		  "reached by branch",
						  (unsigned long)(labelInfo->address));
				}
				ILAsmErrors = 1;
			}
			if(sopcode >= 0x0100)
			{
				OUT_BYTE(IL_OP_PREFIX);
				ref->offset = 2;
			}
			else
			{
				ref->offset = 1;
			}
			OUT_BYTE(sopcode);
			OUT_BYTE(0);
			ref->addrLen = 1;
		}
		else
		{
			/* Always use the long-form branch */
			ref->squashed = 1;
			if(lopcode >= 0x0100)
			{
				OUT_BYTE(IL_OP_PREFIX);
				ref->offset = 2;
			}
			else
			{
				ref->offset = 1;
			}
			OUT_BYTE(lopcode);
			OUT_BYTE(0);
			OUT_BYTE(0);
			OUT_BYTE(0);
			OUT_BYTE(0);
			ref->addrLen = 4;
		}
	}
	else
	{
		/* No, so insert a place-holder that will be fixed up later */
		if(ILAsmShortInsns || opcode == lopcode)
		{
			/* Use the long form for the place-holder */
			ref->squashed = 0;
			if(lopcode >= 0x0100)
			{
				OUT_BYTE(IL_OP_PREFIX);
				ref->offset = 2;
			}
			else
			{
				ref->offset = 1;
			}
			OUT_BYTE(lopcode);
			OUT_BYTE(0);
			OUT_BYTE(0);
			OUT_BYTE(0);
			OUT_BYTE(0);
			ref->addrLen = 4;
		}
		else
		{
			/* Use the short form for the place-holder */
			ref->squashed = 1;
			if(sopcode >= 0x0100)
			{
				OUT_BYTE(IL_OP_PREFIX);
				ref->offset = 2;
			}
			else
			{
				ref->offset = 1;
			}
			OUT_BYTE(sopcode);
			OUT_BYTE(0);
			ref->addrLen = 1;
		}
	}
}

void ILAsmOutBranchInt(ILInt32 opcode, ILInt64 addr)
{
	Branch(opcode, ILAsmOutIntToName(addr));
}

void ILAsmOutBranch(ILInt32 opcode, const char *label)
{
	Branch(opcode, label);
}

/*
 * Output a switch label reference.
 */
static void SwitchRef(const char *name)
{
	LabelInfo *labelInfo;
	LabelRef *ref;

	/* Find the label information block */
	labelInfo = GetLabel(name);

	/* Add a reference for this switch case so that
	   we can fix it up again later when other branches
	   are squashed to remove redundant space */
	ref = ILMemPoolAlloc(&labelRefPool, LabelRef);
	if(!ref)
	{
		ILAsmOutOfMemory();
	}
	ref->posn = offset;
	ref->offset = 0;
	ref->addrLen = 4;
	ref->squashed = 1;
	ref->isSwitchRef = 1;
	ref->switchStart = switchCountOffset;
	ref->filename = ILAsmFilename;
	ref->linenum = ILAsmLineNum;
	ref->next = labelInfo->refs;
	labelInfo->refs = ref;

	/* Output a place-holder for the switch case */
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
}

void ILAsmOutSwitchStart(void)
{
	OUT_BYTE(IL_OP_SWITCH);
	switchCountOffset = offset;
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
}

void ILAsmOutSwitchRefInt(ILInt64 addr)
{
	SwitchRef(ILAsmOutIntToName(addr));
}

void ILAsmOutSwitchRef(const char *label)
{
	SwitchRef(label);
}

void ILAsmOutSwitchEnd(void)
{
	ILUInt32 count = ((offset - switchCountOffset) / 4) - 1;
	buffer[switchCountOffset] = (unsigned char)count;
	buffer[switchCountOffset + 1] = (unsigned char)(count >> 8);
	buffer[switchCountOffset + 2] = (unsigned char)(count >> 16);
	buffer[switchCountOffset + 3] = (unsigned char)(count >> 24);
}

/*
 * Squash the instruction within a label reference to convert
 * it into the short form of a branch.
 */
static void Squash(LabelRef *ref)
{
	ILInt32 opcode;
	ILUInt32 removed;
	LabelInfo *labelInfo;
	LabelRef *tempRef;

	/* Fetch the current opcode */
	opcode = (((ILInt32)(buffer[ref->posn])) & 0xFF);
	if(opcode == IL_OP_PREFIX)
	{
		opcode = (0xFE00 | (((ILInt32)(buffer[ref->posn + 1])) & 0xFF));
	}

	/* Get the short version of the opcode */
	switch(opcode)
	{
		case (0xFE00 | IL_PREFIX_OP_ANN_DATA):
		{
			opcode = IL_OP_ANN_DATA_S;
		}
		break;

		case IL_OP_BEQ:
		{
			opcode = IL_OP_BEQ_S;
		}
		break;

		case IL_OP_BGE:
		{
			opcode = IL_OP_BGE_S;
		}
		break;

		case IL_OP_BGE_UN:
		{
			opcode = IL_OP_BGE_UN_S;
		}
		break;

		case IL_OP_BGT:
		{
			opcode = IL_OP_BGT_S;
		}
		break;

		case IL_OP_BGT_UN:
		{
			opcode = IL_OP_BGT_UN_S;
		}
		break;

		case IL_OP_BLE:
		{
			opcode = IL_OP_BLE_S;
		}
		break;

		case IL_OP_BLE_UN:
		{
			opcode = IL_OP_BLE_UN_S;
		}
		break;

		case IL_OP_BLT:
		{
			opcode = IL_OP_BLT_S;
		}
		break;

		case IL_OP_BLT_UN:
		{
			opcode = IL_OP_BLT_UN_S;
		}
		break;

		case IL_OP_BNE_UN:
		{
			opcode = IL_OP_BNE_UN_S;
		}
		break;

		case IL_OP_BR:
		{
			opcode = IL_OP_BR_S;
		}
		break;

		case IL_OP_BRFALSE:
		{
			opcode = IL_OP_BRFALSE_S;
		}
		break;

		case IL_OP_BRTRUE:
		{
			opcode = IL_OP_BRTRUE_S;
		}
		break;

		case IL_OP_LEAVE:
		{
			opcode = IL_OP_LEAVE_S;
		}
		break;
	}

	/* How many bytes do we need to remove? */
	removed = ref->offset + 4 - 2;

	/* Update the reference appropriately */
	buffer[ref->posn] = (unsigned char)opcode;
	ref->offset = 1;
	ref->addrLen = 1;
	ref->squashed = 1;

	/* Remove the bytes from the middle of the buffer */
	if((ref->posn + 2 + removed) != offset)
	{
		ILMemMove(buffer + ref->posn + 2,
				  buffer + ref->posn + 2 + removed,
				  offset - (ref->posn + 2 + removed));
	}
	offset -= removed;

	/* Adjust the offset of all labels after the removed section */
	labelInfo = labels;
	while(labelInfo != 0)
	{
		if(labelInfo->name && labelInfo->defined &&
		   labelInfo->address > ref->posn)
		{
			labelInfo->address -= removed;
		}
		tempRef = labelInfo->refs;
		while(tempRef != 0)
		{
			if(tempRef->posn > ref->posn)
			{
				tempRef->posn -= removed;
			}
			if(tempRef->switchStart > ref->posn)
			{
				tempRef->switchStart -= removed;
			}
			tempRef = tempRef->next;
		}
		labelInfo = labelInfo->next;
	}
}

void ILAsmOutLabel(const char *label)
{
	LabelInfo *labelInfo = GetLabel(label);
	LabelRef *ref;
	long delta;
	if(!(labelInfo->defined))
	{
		/* Define the label */
		labelInfo->defined = 1;
		labelInfo->address = offset;

		/* Update all references to point at the label's location */
		ref = labelInfo->refs;
		while(ref != 0)
		{
			if(ref->addrLen == 4)
			{
				/* Is this a candidate for squashing? */
				delta = (long)(offset - (ref->posn + 2));
				if(ILAsmShortInsns && delta >= -128 && delta < 128 &&
				   !(ref->squashed))
				{
					/* Squash the instruction to its short form */
					Squash(ref);
				}
				else
				{
					/* Leave the instruction in its long form */
					ref->squashed = 1;
				}
			}
			else if(ref->addrLen == 2) 
			{
				/* java labels: nothing to do */
			}
			else
			{
				delta = (long)(offset - (ref->posn + ref->offset + 1));
				if(delta < -128 || delta >= 128)
				{
					if(labelInfo->name)
					{
						ILAsmPrintMessage(ref->filename, ref->linenum,
							  "label `%s' is too far to be reached by branch",
							  labelInfo->name);
					}
					else
					{
						ILAsmPrintMessage(ref->filename, ref->linenum,
							  "absolute label `%lu' is too far to be "
					  		  "reached by branch",
							  (unsigned long)(labelInfo->address));
					}
					ILAsmErrors = 1;
				}
			}
			ref = ref->next;
		}
	}
	else
	{
		ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
						  "label `%s' defined multiple times", label);
		ILAsmErrors = 1;
	}
}

const char *ILAsmOutIntToName(ILInt64 label)
{
	char numbuf[32];
	if(label >= 0 && label <= (ILInt64)IL_MAX_INT32)
	{
		sprintf(numbuf, "0x%lX", (unsigned long)label);
	}
	else
	{
		sprintf(numbuf, "0x%08lX%08lX",
				(unsigned long)((label >> 32) & 0xFFFFFFFF),
				(unsigned long)(label & 0xFFFFFFFF));
	}
	return (ILInternString(numbuf, -1)).string;
}

const char *ILAsmOutIntLabel(ILInt64 label)
{
	const char *name = ILAsmOutIntToName(label);
	ILAsmOutLabel(name);
	return name;
}

const char *ILAsmOutUniqueLabel(void)
{
	static unsigned long unique = 0;
	char numbuf[32];
	const char *name;
	sprintf(numbuf, ".?$.%lu", unique);
	++unique;
	name = (ILInternString(numbuf, -1)).string;
	ILAsmOutLabel(name);
	return name;
}

void ILAsmOutDebugLine(const char *filename, ILUInt32 line, ILUInt32 column)
{
	/* Output a unique label at this position */
	const char *label = ILAsmOutUniqueLabel();
	LabelInfo *info = GetLabel(label);

	/* Attach the debug information to the label */
	info->debugFilename = filename;
	info->debugLine = line;
	info->debugColumn = column;
	haveDebug = 1;
	haveColumnInfo |= (column != 0);
}

void ILAsmOutSSAStart(ILInt32 opcode)
{
	ILAsmOutSimple(opcode);
	OUT_BYTE(0);
	OUT_BYTE(0);
	ssaStartOffset = offset;
}

void ILAsmOutSSAValue(ILInt64 value)
{
	OUT_BYTE(value);
	OUT_BYTE(value >> 8);
}

void ILAsmOutSSAEnd(void)
{
	ILUInt32 num = ((offset - ssaStartOffset) / 2);
	buffer[ssaStartOffset - 2] = (unsigned char)num;
	buffer[ssaStartOffset - 1] = (unsigned char)(num >> 8);
}

void ILAsmOutMaxStack(ILUInt32 _maxStack)
{
	maxStack = _maxStack;
}

void ILAsmOutMaxLocals(ILUInt32 _maxLocals)
{
	initLocals = _maxLocals;
}

void ILAsmOutZeroInit(void)
{
	initLocals = 1;
}

/*
 * Add a local variable name and index to the current method.
 */
static void AddLocalName(const char *name, ILUInt32 index, int isParam)
{
	LocalInfo *local = (LocalInfo *)ILMalloc(sizeof(LocalInfo));
	if(!local)
	{
		ILAsmOutOfMemory();
	}
	local->block = localCurrentBlock;
	local->name = name;
	local->index = index;
	local->next = localNames;
	local->isParam = isParam;
	localNames = local;
}

void ILAsmOutAddLocals(ILAsmParamInfo *vars)
{
	ILAsmParamInfo *nextVar;
	while(vars != 0)
	{
		if(!localVars)
		{
			localVars = ILTypeCreateLocalList(ILAsmContext);
			if(!localVars)
			{
				ILAsmOutOfMemory();
			}
		}
		if(!ILTypeAddLocal(ILAsmContext, localVars, vars->type))
		{
			ILAsmOutOfMemory();
		}
		if(vars->name)
		{
			AddLocalName(vars->name, localIndex, 0);
		}
		++localIndex;
		nextVar = vars->next;
		ILFree(vars);
		vars = nextVar;
	}
}

void ILAsmOutAddParams(ILAsmParamInfo *vars, ILUInt32 callConv)
{
	ILUInt32 paramIndex;
	if((callConv & IL_META_CALLCONV_HASTHIS) != 0 &&
	   (callConv & IL_META_CALLCONV_EXPLICITTHIS) == 0)
	{
		paramIndex = 1;
	}
	else
	{
		paramIndex = 0;
	}
	while(vars != 0)
	{
		if(vars->name)
		{
			AddLocalName(vars->name, paramIndex, 1);
		}
		++paramIndex;
		vars = vars->next;
	}
}

ILUInt32 ILAsmOutLookupVar(const char *name)
{
	LocalInfo *local = localNames;
	while(local != 0)
	{
		if(local->name == name)
		{
			return local->index;
		}
		local = local->next;
	}
	ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
					  "no local or parameter named `%s'", name);
	ILAsmErrors = 1;
	return 0;
}

ILAsmOutException *ILAsmOutMakeException(ILUInt32 flags, ILClass *classInfo,
									     const char *filterLabel,
										 const char *handlerStart,
									     const char *handlerEnd)
{
	ILAsmOutException *exception;

	/* Allocate memory for the exception */
	exception = (ILAsmOutException *)ILMalloc(sizeof(ILAsmOutException));
	if(!exception)
	{
		ILAsmOutOfMemory();
	}

	/* Initialize the exception details */
	exception->flags = flags;
	exception->blockStart = 0;
	exception->blockEnd = 0;
	exception->blockOffset = 0;
	exception->blockLength = 0;
	exception->handlerStart = handlerStart;
	exception->handlerEnd = handlerEnd;
	exception->handlerOffset = 0;
	exception->handlerLength = 0;
	exception->classToCatch = classInfo;
	exception->filterLabel = filterLabel;
	exception->filterOffset = 0;
	exception->next = 0;
	++numExceptions;

	/* Return the exception handler to the caller */
	return exception;
}

void ILAsmOutAddTryBlock(const char *blockStart, const char *blockEnd,
						 ILAsmOutException *handlers)
{
	ILAsmOutException *current;
	ILAsmOutException *last;

	/* Set the try range for all handlers */
	current = handlers;
	last = 0;
	while(current != 0)
	{
		current->blockStart = blockStart;
		current->blockEnd = blockEnd;
		last = current;
		current = current->next;
	}

	/* Append the handlers to the full list of handlers for the method */
	if(lastException)
	{
		lastException->next = handlers;
	}
	else
	{
		exceptionList = handlers;
	}
	lastException = last;
}

/*
 * Finish processing of labels in the current method.
 * Returns non-zero if we need fat exception blocks.
 */
static int FinishLabels(void)
{
	LabelInfo *label;
	LabelRef *ref;
	long delta;
	int fatExceptions = 0;
	ILAsmOutException *exception;

	/* Process the labels in the exception handlers */
	exception = exceptionList;
	while(exception != 0)
	{
		/* Find the offset of the beginning of the try block */
		label = GetLabel(exception->blockStart);
		if(label->defined)
		{
			exception->blockOffset = label->address;
			if(exception->blockOffset > (ILUInt32)0xFFFF)
			{
				fatExceptions = 1;
			}
		}

		/* Find the length of the try block */
		label = GetLabel(exception->blockEnd);
		if(label->defined)
		{
			if(label->address < exception->blockOffset)
			{
				ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
						  "label `%s' should come after `%s', but doesn't",
						  exception->blockEnd, exception->blockStart);
				ILAsmErrors = 1;
			}
			exception->blockLength = label->address - exception->blockOffset;
			if(exception->blockLength > (ILUInt32)0xFF)
			{
				fatExceptions = 1;
			}
		}

		/* Find the offset of the beginning of the handler block */
		label = GetLabel(exception->handlerStart);
		if(label->defined)
		{
			exception->handlerOffset = label->address;
			if(exception->handlerOffset > (ILUInt32)0xFFFF)
			{
				fatExceptions = 1;
			}
		}

		/* Find the length of the handler block */
		label = GetLabel(exception->handlerEnd);
		if(label->defined)
		{
			if(label->address < exception->blockOffset)
			{
				ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
						  "label `%s' should come after `%s', but doesn't",
						  exception->handlerEnd, exception->handlerStart);
				ILAsmErrors = 1;
			}
			exception->handlerLength =
					label->address - exception->handlerOffset;
			if(exception->handlerLength > (ILUInt32)0xFF)
			{
				fatExceptions = 1;
			}
		}

		/* Find the address of the filter label */
		if(exception->filterLabel)
		{
			label = GetLabel(exception->filterLabel);
			if(label->defined)
			{
				exception->filterOffset = label->address;
			}
		}

		/* Move on to the next exception */
		exception = exception->next;
	}

	/* Fix up all labels, and report errors for those that are undefined */
	label = labels;
	while(label != 0)
	{
		if(label->defined)
		{
			/* Write the final offsets into all branch instructions */
			ref = label->refs;
			while(ref != 0)
			{
				if(ILAsmParseJava)
				{
					if(ref->isSwitchRef)
					{
						/* Find the start of the "switch" instruction */
						delta = (long)(ref->switchStart);

						/* Compute the delta and write it to the "switch" */
						delta = (long)(label->address - (ILUInt32)delta);

						buffer[ref->posn + ref->offset] =
							(unsigned char)(delta >> 24); 
						buffer[ref->posn + ref->offset + 1] =
							(unsigned char)(delta >> 16);
						buffer[ref->posn + ref->offset + 2] =
							(unsigned char)(delta >> 8);
						buffer[ref->posn + ref->offset + 3] =
							(unsigned char)delta;
					}
					else if(ref->addrLen == 2) /* java labels */
					{
						delta = (long)(label->address - ref->posn);
						buffer[ref->posn + ref->offset] =
							(unsigned char)(delta >> 8);
						buffer[ref->posn + ref->offset + 1] =
							(unsigned char)delta;
					}
				}
				else if(ref->isSwitchRef)
				{
					/* Find the end of the "switch" instruction */
					delta = (long)(ref->switchStart + 4 *
							(IL_READ_UINT32(buffer + ref->switchStart) + 1));

					/* Compute the delta and write it to the "switch" */
					delta = (long)(label->address - (ILUInt32)delta);
					buffer[ref->posn + ref->offset] =
							(unsigned char)delta;
					buffer[ref->posn + ref->offset + 1] =
							(unsigned char)(delta >> 8);
					buffer[ref->posn + ref->offset + 2] =
							(unsigned char)(delta >> 16);
					buffer[ref->posn + ref->offset + 3] =
							(unsigned char)(delta >> 24);
				}
				else if(ref->addrLen == 4)
				{
					delta = (long)(label->address -
								   (ref->posn + ref->offset + 4));
					buffer[ref->posn + ref->offset] =
							(unsigned char)delta;
					buffer[ref->posn + ref->offset + 1] =
							(unsigned char)(delta >> 8);
					buffer[ref->posn + ref->offset + 2] =
							(unsigned char)(delta >> 16);
					buffer[ref->posn + ref->offset + 3] =
							(unsigned char)(delta >> 24);
				}
				else
				{
					delta = (long)(label->address -
								   (ref->posn + ref->offset + 1));
					buffer[ref->posn + ref->offset] =
							(unsigned char)delta;
				}
				ref = ref->next;
			}
		}
		else
		{
			/* Print an error for this undefined label */
			ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
							  "label `%s' not defined in the current method",
							  label->name);
			ILAsmErrors = 1;
		}
		label = label->next;
	}

	/* Return the exception header type to the caller */
	return fatExceptions;
}

/*
 * Output debug information for a method.
 */
static void OutputDebugInfo(ILMethod *method)
{
	unsigned char buf[256];
	const char *prevFilename = 0;
	LabelInfo *label = labels;
	ILUInt32 len = 0;
	int type;
	LocalInfo *local;
	LocalBlock *lastBlock;

	/* Output the line number information for the method */
	type = IL_DEBUGTYPE_LINE_OFFSETS;
	if(haveColumnInfo)
	{
		type = IL_DEBUGTYPE_LINE_COL_OFFSETS;
	}
	while(label != 0)
	{
		if(label->debugFilename)
		{
			/* If the filename has changed, then flush the buffer */
			if(prevFilename != label->debugFilename)
			{
				if(len > 0)
				{
					ILWriterDebugAdd(ILAsmWriter, (ILProgramItem *)method,
									 type, buf, len);
					len = 0;
				}
				len += ILMetaCompressData
					(buf + len, ILWriterDebugString
									(ILAsmWriter, label->debugFilename));
				prevFilename = label->debugFilename;
			}

			/* Add the line and offset information */
			len += ILMetaCompressData(buf + len, label->debugLine);
			if(haveColumnInfo)
			{
				len += ILMetaCompressData(buf + len, label->debugColumn);
			}
			len += ILMetaCompressData(buf + len, label->address);

			/* Flush the buffer if it is nearly full */
			if(len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 4))
			{
				ILWriterDebugAdd(ILAsmWriter, (ILProgramItem *)method,
								 type, buf, len);
				prevFilename = 0;
				len = 0;
			}
		}
		label = label->next;
	}
	if(len > 0)
	{
		/* Flush the remainder of the debug information */
		ILWriterDebugAdd(ILAsmWriter, (ILProgramItem *)method,
						 type, buf, len);
		len = 0;
	}

	/* Output the local variable name information for the method */
	local = localNames;
	lastBlock = (LocalBlock *)(ILNativeInt)(-1);
	type = IL_DEBUGTYPE_VARS;
	while(local != 0)
	{
		if(local->isParam)
		{
			local = local->next;
			continue;
		}
		if(!len || len >= (sizeof(buf) - IL_META_COMPRESS_MAX_SIZE * 4) ||
		   local->block != lastBlock)
		{
			/* We've encountered a change in local variable scope,
			   or the buffer is almost full */
			if(len > 0)
			{
				ILWriterDebugAdd(ILAsmWriter, (ILProgramItem *)method,
								 type, buf, len);
				len = 0;
			}
			lastBlock = local->block;
			if(!lastBlock)
			{
				/* Outermost local variable scope for the method */
				type = IL_DEBUGTYPE_VARS;
			}
			else
			{
				/* Inner local variable scope for the method */
				type = IL_DEBUGTYPE_VARS_OFFSETS;
				label = GetLabel(lastBlock->blockStart);
				len += ILMetaCompressData(buf + len, label->address);
				label = GetLabel(lastBlock->blockEnd);
				len += ILMetaCompressData(buf + len, label->address);
			}
		}
		len += ILMetaCompressData
			(buf + len, ILWriterDebugString(ILAsmWriter, local->name));
		len += ILMetaCompressData(buf + len, local->index);
		local = local->next;
	}
	if(len > 0)
	{
		/* Flush the remainder of the debug information */
		ILWriterDebugAdd(ILAsmWriter, (ILProgramItem *)method,
						 type, buf, len);
	}
}

void ILAsmOutFinalizeMethod(ILMethod *method)
{
	unsigned char header[24];
	ILStandAloneSig *sig;
	ILToken token;
	LocalInfo *local;
	LocalInfo *nextLocal;
	LocalBlock *block;
	LocalBlock *nextBlock;
	ILAsmOutException *exception;
	ILAsmOutException *nextException;
	int fatExceptions;
	unsigned long temp;
	unsigned long startRVA;
	LabelInfo *label;

#ifdef IL_CONFIG_JAVA
	/* check if parsing java method */
	if(ILAsmParseJava) 
	{
		int attrLen;
		int codeLen;
		int index;
		int attrCount;
		int exceptLength;

		/* Finish the label processing for the method */
		fatExceptions = FinishLabels();

		index = ILJavaSetUTF8String(ILAsmWriter, ILMethod_Owner(method), "Code", 4);
		if(!index) 
		{
			ILAsmOutOfMemory();
			goto cleanup;
		}

		header[0] = (unsigned char)(index >> 8);
		header[1] = (unsigned char)index ;

		attrLen = offset + 12;
		attrLen += 8 * numExceptions;
		header[2] = (unsigned char)(attrLen >> 24);
		header[3] = (unsigned char)(attrLen >> 16);
		header[4] = (unsigned char)(attrLen >> 8);
		header[5] = (unsigned char)attrLen;

		header[6] = (unsigned char)(maxStack >> 8);
		header[7] = (unsigned char)maxStack;

		header[8] = (unsigned char)(initLocals >> 8);
		header[9] = (unsigned char)initLocals;
		
		codeLen = offset; 
		header[10] = (unsigned char)(codeLen >> 24);
		header[11] = (unsigned char)(codeLen >> 16);
		header[12] = (unsigned char)(codeLen >> 8);
		header[13] = (unsigned char)codeLen;

		ILJavaAppendCode(ILAsmWriter, ILMethod_Owner(method), method, header, 14);

		/* Output the method code */
		ILJavaAppendCode(ILAsmWriter, ILMethod_Owner(method), method, buffer, offset);

		/* exceptions */
		header[0] = (unsigned char)(numExceptions >> 8);
		header[1] = (unsigned char)numExceptions;

		ILJavaAppendCode(ILAsmWriter, ILMethod_Owner(method), method, header, 2);
		exception = exceptionList;
		while(exception != 0)
		{
			header[0] = (unsigned char)(exception->blockOffset >> 8);
			header[1] = (unsigned char)exception->blockOffset;

			exceptLength = exception->blockLength + exception->blockOffset;

			header[2] = (unsigned char)(exceptLength >> 8);
			header[3] = (unsigned char)exceptLength;
			header[4] = (unsigned char)(exception->handlerOffset >> 8);
			header[5] = (unsigned char)exception->handlerOffset;

			if(exception->classToCatch)
			{
				temp = ILClass_Token(exception->classToCatch);
				index = ILJavaSetClass(ILAsmWriter, ILMethod_Owner(method), exception->classToCatch);
			}
			else
			{
				index = 0;
			}
			header[6] = (unsigned char)(index >> 8);
			header[7] = (unsigned char)index;
			ILJavaAppendCode(ILAsmWriter, ILMethod_Owner(method), method, header, 8);
			exception = exception->next;
		}


		/* attributes */
		attrCount = 0; /* TODO */
		header[0] = (unsigned char)(attrCount >> 8);
		header[1] = (unsigned char)attrCount;

		ILJavaAppendCode(ILAsmWriter, ILMethod_Owner(method), method, header, 2);
	} 
	else
#endif
	{

	/* Finish the label processing for the method */
	fatExceptions = FinishLabels();

	/* Bail out if something is wrong with the method context */
	if(!method)
	{
		goto cleanup;
	}

	/* Bail out if the method is empty (i.e. not implemented using il) */
	if(!offset)
	{
		goto cleanup;
	}

	/* Output debug information for the method */
	if(haveDebug)
	{
		OutputDebugInfo(method);
	}

	/* Align the text section on the next 4-byte boundary */
	ILWriterTextAlign(ILAsmWriter);

	/* Set the RVA for the method */
	ILMethodSetRVA(method, ILWriterGetTextRVA(ILAsmWriter));

	/* Output the method header */
	if(offset < 0x40 && !localVars && !exceptionList && maxStack <= 2)
	{
		/* Use the tiny format */
		header[0] = ((unsigned char)((offset << 2) | 0x02));
		ILWriterTextWrite(ILAsmWriter, header, 1);
	}
	else
	{
		/* Use the fat format */
		header[0] = (unsigned char)(initLocals ? 0x13 : 0x03);
		if(exceptionList)
		{
			/* There will be more sections following the method code */
			header[0] |= (unsigned char)0x08;
		}
		header[1] = (unsigned char)0x30;
		header[2] = (unsigned char)maxStack;
		header[3] = (unsigned char)(maxStack >> 8);
		header[4] = (unsigned char)offset;
		header[5] = (unsigned char)(offset >> 8);
		header[6] = (unsigned char)(offset >> 16);
		header[7] = (unsigned char)(offset >> 24);
		if(localVars)
		{
			sig = ILStandAloneSigCreate(ILAsmImage, 0, localVars);
			if(!sig)
			{
				ILAsmOutOfMemory();
			}
			token = ILProgramItem_Token(sig);
			header[8]  = (unsigned char)token;
			header[9]  = (unsigned char)(token >> 8);
			header[10] = (unsigned char)(token >> 16);
			header[11] = (unsigned char)(token >> 24);
		}
		else
		{
			header[8]  = (unsigned char)0x00;
			header[9]  = (unsigned char)0x00;
			header[10] = (unsigned char)0x00;
			header[11] = (unsigned char)0x00;
		}
		ILWriterTextWrite(ILAsmWriter, header, 12);
	}

	/* Output the method code */
	startRVA = ILWriterGetTextRVA(ILAsmWriter);
	ILWriterTextWrite(ILAsmWriter, buffer, offset);

	/* Output the exception information if necessary */
	if(exceptionList)
	{
		/* Align the text section on a 4-byte boundary */
		ILWriterTextAlign(ILAsmWriter);

		/* Switch formats if the section size is too big for tiny */
		if(!fatExceptions && ((numExceptions * 12) + 4) > (unsigned long)0xFF)
		{
			fatExceptions = 1;
		}

		/* What type of exception header should we use? */
		if(fatExceptions)
		{
			/* Use the fat format for the exception information */
			temp = (numExceptions * 24) + 4;
			header[0] = (unsigned char)0x41;
			header[1] = (unsigned char)temp;
			header[2] = (unsigned char)(temp >> 8);
			header[3] = (unsigned char)(temp >> 16);
			ILWriterTextWrite(ILAsmWriter, header, 4);
			exception = exceptionList;
			while(exception != 0)
			{
				header[0]  = (unsigned char)(exception->flags);
				header[1]  = (unsigned char)(exception->flags >> 8);
				header[2]  = (unsigned char)(exception->flags >> 16);
				header[3]  = (unsigned char)(exception->flags >> 24);
				header[4]  = (unsigned char)(exception->blockOffset);
				header[5]  = (unsigned char)(exception->blockOffset >> 8);
				header[6]  = (unsigned char)(exception->blockOffset >> 16);
				header[7]  = (unsigned char)(exception->blockOffset >> 24);
				header[8]  = (unsigned char)(exception->blockLength);
				header[9]  = (unsigned char)(exception->blockLength >> 8);
				header[10] = (unsigned char)(exception->blockLength >> 16);
				header[11] = (unsigned char)(exception->blockLength >> 24);
				header[12] = (unsigned char)(exception->handlerOffset);
				header[13] = (unsigned char)(exception->handlerOffset >> 8);
				header[14] = (unsigned char)(exception->handlerOffset >> 16);
				header[15] = (unsigned char)(exception->handlerOffset >> 24);
				header[16] = (unsigned char)(exception->handlerLength);
				header[17] = (unsigned char)(exception->handlerLength >> 8);
				header[18] = (unsigned char)(exception->handlerLength >> 16);
				header[19] = (unsigned char)(exception->handlerLength >> 24);
				if(exception->filterLabel)
				{
					header[20] = (unsigned char)(exception->filterOffset);
					header[21] = (unsigned char)(exception->filterOffset >> 8);
					header[22] = (unsigned char)(exception->filterOffset >> 16);
					header[23] = (unsigned char)(exception->filterOffset >> 24);
				}
				else if(exception->classToCatch)
				{
					temp = ILClass_Token(exception->classToCatch);
					header[20] = (unsigned char)(temp);
					header[21] = (unsigned char)(temp >> 8);
					header[22] = (unsigned char)(temp >> 16);
					header[23] = (unsigned char)(temp >> 24);
					ILWriterSetFixup(ILAsmWriter,
						ILWriterGetTextRVA(ILAsmWriter) + 20,
						ILToProgramItem(exception->classToCatch));
				}
				else
				{
					header[20] = (unsigned char)0x00;
					header[21] = (unsigned char)0x00;
					header[22] = (unsigned char)0x00;
					header[23] = (unsigned char)0x00;
				}
				ILWriterTextWrite(ILAsmWriter, header, 24);
				exception = exception->next;
			}
		}
		else
		{
			/* Use the tiny format for the exception information */
			temp = (numExceptions * 12) + 4;
			header[0] = (unsigned char)0x01;
			header[1] = (unsigned char)temp;
			header[2] = (unsigned char)0x00;
			header[3] = (unsigned char)0x00;
			ILWriterTextWrite(ILAsmWriter, header, 4);
			exception = exceptionList;
			while(exception != 0)
			{
				header[0] = (unsigned char)(exception->flags);
				header[1] = (unsigned char)(exception->flags >> 8);
				header[2] = (unsigned char)(exception->blockOffset);
				header[3] = (unsigned char)(exception->blockOffset >> 8);
				header[4] = (unsigned char)(exception->blockLength);
				header[5] = (unsigned char)(exception->handlerOffset);
				header[6] = (unsigned char)(exception->handlerOffset >> 8);
				header[7] = (unsigned char)(exception->handlerLength);
				if(exception->filterLabel)
				{
					header[8]  = (unsigned char)(exception->filterOffset);
					header[9]  = (unsigned char)(exception->filterOffset >> 8);
					header[10] = (unsigned char)(exception->filterOffset >> 16);
					header[11] = (unsigned char)(exception->filterOffset >> 24);
				}
				else if(exception->classToCatch)
				{
					temp = ILClass_Token(exception->classToCatch);
					header[8]  = (unsigned char)(temp);
					header[9]  = (unsigned char)(temp >> 8);
					header[10] = (unsigned char)(temp >> 16);
					header[11] = (unsigned char)(temp >> 24);
					ILWriterSetFixup(ILAsmWriter,
						ILWriterGetTextRVA(ILAsmWriter) + 8,
						ILToProgramItem(exception->classToCatch));
				}
				else
				{
					header[8]  = (unsigned char)0x00;
					header[9]  = (unsigned char)0x00;
					header[10] = (unsigned char)0x00;
					header[11] = (unsigned char)0x00;
				}
				ILWriterTextWrite(ILAsmWriter, header, 12);
				exception = exception->next;
			}
		}
	}

	/* Record the RVA's of all instructions within the method
	   that refer to a TypeRef or a MemberRef */
	label = labels;
	while(label != 0)
	{
		if(label->tokenFixup)
		{
			ILWriterSetFixup(ILAsmWriter, startRVA + label->address - 4,
							 ILProgramItem_FromToken(ILAsmImage,
							 	IL_READ_UINT32(buffer + label->address - 4)));
		}
		label = label->next;
	}

	}

	/* Clean up all data used to represent the method */
cleanup:
	offset = 0;
	localVars = 0;
	maxStack = 8;
	initLocals = 0;
	localIndex = 0;
	local = localNames;
	while(local != 0)
	{
		nextLocal = local->next;
		ILFree(local);
		local = nextLocal;
	}
	localNames = 0;
	block = localBlocks;
	while(block != 0)
	{
		nextBlock = block->next;
		ILFree(block);
		block = nextBlock;
	}
	localBlocks = 0;
	exception = exceptionList;
	while(exception != 0)
	{
		nextException = exception->next;
		ILFree(exception);
		exception = nextException;
	}
	exceptionList = 0;
	lastException = 0;
	numExceptions = 0;
	if(initLabelPool)
	{
		ILMemPoolClear(&labelPool);
		ILMemPoolClear(&labelRefPool);
	}
	labels = 0;
	haveDebug = 0;
	haveColumnInfo = 0;
}

void ILAsmOutAddResource(const char *name, FILE *stream)
{
	unsigned long rva;
	unsigned long length;
	char buffer[BUFSIZ];
	int len;
	ILManifestRes *res;

	/* Strip the directory information from the filename */
	len = strlen(name);
	while(len > 0 && name[len - 1] != '/' && name[len - 1] != '\\')
	{
		--len;
	}
	name += len;

	/* Initialize the resources section */
	ILWriterTextAlign(ILAsmWriter);
	rva = ILWriterGetTextRVA(ILAsmWriter);

	/* Write a place-holder for the resource length */
	ILMemZero(buffer, 4);
	ILWriterTextWrite(ILAsmWriter, buffer, 4);

	/* Copy the contents of the input stream to the resource section */
	length = 0;
	while((len = fread(buffer, 1, BUFSIZ, stream)) > 0)
	{
		ILWriterTextWrite(ILAsmWriter, buffer, len);
		length += (unsigned long)len;
		if(len < BUFSIZ)
		{
			break;
		}
	}

	/* Back-patch the place-holder with the resource length */
	ILWriterTextWrite32Bit(ILAsmWriter, rva, length);

	/* Update the total size of the resources section */
	ILWriterUpdateHeader(ILAsmWriter, IL_IMAGEENTRY_RESOURCES, rva,
						 ILWriterGetTextRVA(ILAsmWriter) - rva);

	/* Add a manifest resource record to the metadata */
	res = ILManifestResCreate(ILAsmImage, 0, name, IL_META_MANIFEST_PUBLIC, 0);
	if(!res)
	{
		ILAsmOutOfMemory();
	}
}

void ILAsmOutDeclareVarName(const char *name, ILUInt32 index)
{
	AddLocalName(name, index, 0);
}

void ILAsmOutPushVarScope(const char *name)
{
	LocalBlock *block = (LocalBlock *)ILMalloc(sizeof(LocalBlock));
	if(!block)
	{
		ILAsmOutOfMemory();
	}
	block->blockStart = name;
	block->blockEnd = 0;
	block->outer = localCurrentBlock;
	block->next = localBlocks;
	localBlocks = block;
	localCurrentBlock = block;
}

void ILAsmOutPopVarScope(const char *name)
{
	if(localCurrentBlock)
	{
		localCurrentBlock->blockEnd = name;
		localCurrentBlock = localCurrentBlock->outer;
	}
}

void ILJavaAsmInitPool()
{
#ifdef IL_CONFIG_JAVA
	ILJavaInitPool(ILAsmWriter, ILAsmClass);
#endif
}

void ILJavaAsmOutSimple(ILInt32 opcode)
{
	OUT_BYTE(opcode);
}

void ILJavaAsmOutVar(ILInt32 opcode, ILInt64 num) 
{
	if(num <= 3)
	{
		/* TODO: use short version */
	}
	OUT_BYTE(opcode);
	OUT_BYTE(num);
}

void ILJavaAsmOutInc(ILInt32 opcode, ILInt64 index, ILInt64 val) 
{
	OUT_BYTE(opcode);
	OUT_BYTE(index);
	OUT_BYTE(val);
}

void ILJavaAsmOutInt(ILInt32 opcode, ILInt64 value)
{
	if(opcode == JAVA_OP_BIPUSH)
	{
		OUT_BYTE(opcode);
		OUT_BYTE(value);
	}
	else if(opcode == JAVA_OP_SIPUSH)
	{
		OUT_BYTE(opcode);
		OUT_BYTE(value >> 8);
		OUT_BYTE(value);
	}
}

#ifdef IL_CONFIG_JAVA

static void OutConst32(int index)
{
	if(index < 256)
	{
		OUT_BYTE(JAVA_OP_LDC);
		OUT_BYTE(index);
	}
	else
	{
		OUT_BYTE(JAVA_OP_LDC_W);
		OUT_BYTE(index >> 8);
		OUT_BYTE(index);
	}
}

static void OutConst64(int index)
{
	OUT_BYTE(JAVA_OP_LDC2_W);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
}

#endif

void ILJavaAsmOutConstInt32(ILInt32 opcode, ILInt64 value)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetInteger(ILAsmWriter, ILAsmClass, (ILInt32)value);

	OutConst32(index);
#endif
}

void ILJavaAsmOutConstInt64(ILInt32 opcode, ILInt64 value)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetLong(ILAsmWriter, ILAsmClass, value);

	OutConst64(index);
#endif
}

void ILJavaAsmOutConstFloat32(ILInt32 opcode, ILUInt8 *value)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetFloat(ILAsmWriter, ILAsmClass, IL_READ_FLOAT(value));
	
	OutConst32(index);
#endif
}

void ILJavaAsmOutConstFloat64(ILInt32 opcode, ILUInt8 *value)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetDouble(ILAsmWriter, ILAsmClass, IL_READ_DOUBLE(value));

	OutConst64(index);
#endif
}

void ILJavaAsmOutString(ILIntString interned) 
{
#ifdef IL_CONFIG_JAVA
	int strIndex = ILJavaSetUTF8String(ILAsmWriter, ILAsmClass, interned.string, 
									   interned.len);
	int index = ILJavaSetString(ILAsmWriter, ILAsmClass, strIndex);

	OutConst32(index);
#endif
}

void ILJavaAsmOutToken(ILInt32 opcode, ILUInt32 token)
{
#ifdef IL_CONFIG_JAVA
	ILMember *member;
	ILClass *class;
	ILUInt32 index = 0;
	int refType;

	switch(token & IL_META_TOKEN_MASK)
	{
	case IL_META_TOKEN_TYPE_REF:
	case IL_META_TOKEN_TYPE_DEF:
	{
		class = ILClass_FromToken(ILAsmImage, token);
		index = ILJavaSetClass(ILAsmWriter, ILAsmClass, class);
	}
	break;

	case IL_META_TOKEN_FIELD_DEF:
	{
		refType = JAVA_CONST_FIELDREF;
		goto setRef; 
	}
	break;

	case IL_META_TOKEN_MEMBER_REF:
	case IL_META_TOKEN_METHOD_DEF:
	{
		refType = JAVA_CONST_METHODREF;
	setRef:
		member = ILMember_FromToken(ILAsmImage, token);
		index = ILJavaSetref(ILAsmWriter, ILAsmClass, refType, ILMember_Owner(member), 
							 (char*)ILMember_Name(member), ILMember_Signature(member));
	}
	break;

	case IL_META_TOKEN_TYPE_SPEC:
	{
		/* TODO */
	} 
	break;
	}

	OUT_BYTE(opcode);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
#endif
}

void ILJavaAsmOutRef(ILInt32 opcode, int isMethod, const char *className,
					 const char *refName, const char *sigName)
{
#ifdef IL_CONFIG_JAVA
	int index = 
		ILJavaSetrefFromName(ILAsmWriter, ILAsmClass, 
							 isMethod ? JAVA_CONST_METHODREF : JAVA_CONST_FIELDREF,
							 className, refName, sigName);

	OUT_BYTE(opcode);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
#endif
}

void ILJavaAsmOutType(ILInt32 opcode, const char *className)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetClassFromName(ILAsmWriter, ILAsmClass, className);

	OUT_BYTE(opcode);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
#endif
}

void ILJavaAsmOutNewarray(ILInt32 opcode, ILInt64 type)
{
	OUT_BYTE(opcode);
	OUT_BYTE(type);
}

void ILJavaAsmOutMultinewarray(ILInt32 opcode, ILType *type, ILInt64 dim)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetClassFromType(ILAsmWriter, ILAsmClass, type);

	OUT_BYTE(opcode);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
	OUT_BYTE(dim);
#endif
}

void ILJavaAsmOutMultinewarrayFromName(ILInt32 opcode, const char *typeName, ILInt64 dim)
{
#ifdef IL_CONFIG_JAVA
	int index = ILJavaSetClassFromName(ILAsmWriter, ILAsmClass, typeName);

	OUT_BYTE(opcode);
	OUT_BYTE(index >> 8);
	OUT_BYTE(index);
	OUT_BYTE(dim);
#endif
}

/*
 * Output a switch label reference in java.
 */
static void JavaSwitchRef(const char *name)
{
	LabelInfo *labelInfo;
	LabelRef *ref;

	/* Find the label information block */
	labelInfo = GetLabel(name);

	/* Add a reference for this switch case so that
	   we can fix it up again later when other branches
	   are squashed to remove redundant space */
	ref = ILMemPoolAlloc(&labelRefPool, LabelRef);
	if(!ref)
	{
		ILAsmOutOfMemory();
	}
	ref->posn = offset;
	ref->offset = 0;
	ref->addrLen = 4;
	ref->squashed = 1;
	ref->isSwitchRef = 1;
	ref->switchStart = switchStartOffset;
	ref->filename = ILAsmFilename;
	ref->linenum = ILAsmLineNum;
	ref->next = labelInfo->refs;
	labelInfo->refs = ref;

	/* Output a place-holder for the switch case */
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
}

void ILJavaAsmOutTableSwitchDefaultRefInt(ILInt64 addr)
{
	defaultTableSwitchLabel = ILAsmOutIntToName(addr);
}

void ILJavaAsmOutTableSwitchDefaultRef(const char *label)
{
	defaultTableSwitchLabel = label;
}

void ILJavaAsmOutTableSwitchStart(ILInt64 low)
{
	switchStartOffset = offset;
	OUT_BYTE(JAVA_OP_TABLESWITCH);

	/* some padding to a 4 bytes boundary */
	while(((offset) & 3) != 0)
	{
		OUT_BYTE(0);
	}

	/* default offset */
	JavaSwitchRef(defaultTableSwitchLabel);

	/* low */
	OUT_BYTE(low >> 24);
	OUT_BYTE(low >> 16);
	OUT_BYTE(low >> 8);
	OUT_BYTE(low);
	switchHighOffset = offset;

	/* high */
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
}

void ILJavaAsmOutTableSwitchRefInt(ILInt64 addr)
{
	JavaSwitchRef(ILAsmOutIntToName(addr));
}

void ILJavaAsmOutTableSwitchRef(const char *label)
{
	JavaSwitchRef(label);
}

void ILJavaAsmOutTableSwitchEnd(ILUInt64 low)
{
	ILUInt32 high;
	ILUInt32 count = ((offset - switchHighOffset) / 4) - 2;
	high = count + low;
	
	buffer[switchHighOffset] = (unsigned char)(high >> 24);
	buffer[switchHighOffset + 1] = (unsigned char)(high >> 16);
	buffer[switchHighOffset + 2] = (unsigned char)(high >> 8);
	buffer[switchHighOffset + 3] = (unsigned char)high;
}

void ILJavaAsmOutLookupSwitchDefaultRefInt(ILInt64 addr)
{
	defaultLookupSwitchLabel = ILAsmOutIntToName(addr);
}

void ILJavaAsmOutLookupSwitchDefaultRef(const char *label)
{
	defaultLookupSwitchLabel = label;
}

void ILJavaAsmOutLookupSwitchStart(void)
{
	switchStartOffset = offset;
	OUT_BYTE(JAVA_OP_LOOKUPSWITCH);

	/* some padding to a 4 bytes boundary */
	while(((offset) & 3) != 0)
	{
		OUT_BYTE(0);
	}

	/* default offset */
	JavaSwitchRef(defaultLookupSwitchLabel);
	switchNPairsOffset = offset;

	/* npairs */
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
	OUT_BYTE(0);
}

void ILJavaAsmOutLookupSwitchRefInt(ILInt64 match, ILInt64 addr)
{
	ILJavaAsmOutLookupSwitchRef(match, ILAsmOutIntToName(addr));
}

void ILJavaAsmOutLookupSwitchRef(ILInt64 match, const char *label)
{
	OUT_BYTE(match >> 24);
	OUT_BYTE(match >> 16);
	OUT_BYTE(match >> 8);
	OUT_BYTE(match);
	JavaSwitchRef(label);
}

void ILJavaAsmOutLookupSwitchEnd(void)
{
	ILUInt32 nPairs = (((offset - switchNPairsOffset) / 4) - 1) / 2;

	buffer[switchNPairsOffset] = (unsigned char)(nPairs >> 24);
	buffer[switchNPairsOffset + 1] = (unsigned char)(nPairs >> 16);
	buffer[switchNPairsOffset + 2] = (unsigned char)(nPairs >> 8);
	buffer[switchNPairsOffset + 3] = (unsigned char)nPairs;
}

#ifdef	__cplusplus
};
#endif
