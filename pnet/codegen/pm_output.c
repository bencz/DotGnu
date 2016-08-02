/*
 * pm_output.c - Assembly code output routines for the Parrot Machine.
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
#include "pm_output.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#define	PM_INVALID_REG		(~(PMRegister)0)

#define	PM_ARG_REG			1
#define	PM_LOCAL_REG		2
#define	PM_TEMP_INT_REG		3
#define	PM_TEMP_NUM_REG		4
#define	PM_TEMP_PMC_REG		5
#define	PM_TEMP_STR_REG		6

#define	PM_TYPE_INT			0
#define	PM_TYPE_NUM			1
#define	PM_TYPE_PMC			2
#define	PM_TYPE_STR			3

typedef struct
{
	PMRegister		fullReg;
	ILMachineType	type;
	int				isNonDest;

} PMRegInfo;

struct _tagPMGenInfo
{
	ILMethod *method;				/* Method that is being compiled */
	ILType *signature;				/* Method signature */
	PMRegInfo *regs;				/* Characteristics of the registers */
	int numRegs;					/* Number of registers */
	PMRegister nullReg;				/* Register holding the "null" value */
	int nullRegDuringSetup;			/* "nullReg" set during setup code */
	ILLabel retLabel;				/* Label for the method's return point */
	ILLabel nextLabel;				/* Value to use for the next label */
	int prevIsReturn;				/* Previous instruction was "return" */

};

/*
 * Operator names.
 */
static const char *OpNames[] = {
	"clone",
	"addr",
	"==",
	"!=",
	"<",
	"<=",
	">",
	">=",
	"+",
	"-",
	"*",
	"/",
	"@cmod",
	"-",
	"&",
	"|",
	"~",
	"~",
	"<<",
	">>",
	">>>",
	"!",
	"defined",
};

/*
 * Convert a machine type into a Parrot register type.
 */
static int MachineTypeToParrotType(ILMachineType type)
{
	switch(type)
	{
		case ILMachineType_Void:
		case ILMachineType_Boolean:
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		case ILMachineType_Int16:
		case ILMachineType_UInt16:
		case ILMachineType_Char:
		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
		case ILMachineType_UnmanagedPtr:
			return PM_TYPE_INT;

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
			return PM_TYPE_PMC;

		case ILMachineType_Float32:
		case ILMachineType_Float64:
		case ILMachineType_NativeFloat:
			return PM_TYPE_NUM;

		case ILMachineType_Decimal:
		case ILMachineType_String:
		case ILMachineType_ObjectRef:
		case ILMachineType_ManagedPtr:
		case ILMachineType_TransientPtr:
		case ILMachineType_ManagedValue:
			return PM_TYPE_PMC;
	}
}

/*
 * Initialize a local variable register to a default value.
 */
static void InitLocal(ILGenInfo *info, PMRegister reg, ILType *type)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Convert enum types into their underlying representation */
	type = ILTypeGetEnumType(type);

	/* Determine how to initialize the value */
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
			{
				putc("\t", info->asmOutput);
				PMGenRegister(info, reg);
				fputs(" = 0\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			{
				/* TODO: initialize a Parrot BIGINT */
			}
			break;

			case IL_META_ELEMTYPE_R4:
			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:
			{
				putc("\t", info->asmOutput);
				PMGenRegister(info, reg);
				fputs(" = 0.0\n", info->asmOutput);
			}
			break;

			case IL_META_ELEMTYPE_TYPEDBYREF:
			{
				/* TODO: initialize a typed reference (new PMC?) */
			}
			break;
		}
	}
	else if(ILTypeIsReference(type))
	{
		/* Set references to "null" initially */
		if(pminfo->nullReg == PM_INVALID_REG)
		{
			pminfo->nullReg = PMGenTempReg(info, ILMachineType_ObjectRef);
			pminfo->nullRegDuringSetup = 1;
			PMGenMarkRegNonDest(info, pminfo->nullReg);
			putc("\t", info->asmOutput);
			PMGenRegister(info, pminfo->nullReg);
			fputs(" = global \"cs.null\"\n", info->asmOutput);
		}
		putc("\t", info->asmOutput);
		PMGenRegister(info, reg);
		fputs(" = ", info->asmOutput);
		PMGenRegister(info, pminfo->nullReg);
		putc("\n", info->asmOutput);
	}
	else if(ILType_IsValueType(type))
	{
		/* Call the default constructor for the value type */
		/* TODO */
	}
	else
	{
		/* Assume that everything else is a pointer */
		putc("\t", info->asmOutput);
		PMGenRegister(info, reg);
		fputs(" = 0\n", info->asmOutput);
	}
}

void PMGenStartMethod(ILGenInfo *info, ILMethod *method, ILType *localVarSig)
{
	PMGenInfo *pminfo = info->pminfo;
	unsigned long numArgs;
	PMRegister reg;
	ILType *type;

	/* Set up the Parrot-specific generation state */
	pminfo->method = method;
	pminfo->signature = ILMethod_Signature(method);
	pminfo->numRegs = 0;
	pminfo->nullReg = PM_INVALID_REG;
	pminfo->nullRegDuringSetup = 0;
	pminfo->retLabel = ILLabel_Undefined;
	pminfo->prevIsReturn = 0;

	/* TODO: if this is the "Main" method, then output the
	   program crt0 startup logic to call this method  */

	/* Output the method header */
	if(!(info->asmOutput))
	{
		return;
	}
	fputs(".sub ", info->asmOutput);
	GenMemberName(info, method);
	fputs("\n\tsaveall\n, info->asmOutput);

	/* Unload the arguments from the stack */
	numArgs = ILTypeNumParams(pminfo->signature);
	for(argNum = numArgs; argNum > 0; --argNum)
	{
		if(ILType_HasThis(pminfo->signature))
		{
			reg = PMGenArgReg(info, argNum);
		}
		else
		{
			reg = PMGenArgReg(info, argNum - 1);
		}
		fputs("\t.param ", info->asmOutput);
		PMGenRegister(info, reg);
		putc('\n', info->asmOutput);
	}
	if(ILType_HasThis(pminfo->signature))
	{
		reg = PMGenArgReg(info, 0);
		fputs("\t.param ", info->asmOutput);
		PMGenRegister(info, reg);
		putc('\n', info->asmOutput);
	}

	/* Initialize the local variables */
	if(localVarSig)
	{
		numArgs = ILTypeNumLocals(localVarSig);
		for(argNum = 0; argNum < numArgs; ++argNum)
		{
			reg = PMGenLocalReg(info, argNum);
			type = ILTypeGetLocal(localVarSig, argNum);
			InitLocal(info, reg, type);
		}
	}
}

void PMGenEndMethod(ILGenInfo *info)
{
	PMGenInfo *pminfo = info->pminfo;
	if(info->asmOutput)
	{
		if(pminfo->retLabel != ILLabel_Undefined)
		{
			fprintf(info->asmOutput, "L_%lu:\n",
					(unsigned long)(pminfo->retLabel));
		}
		fputs("\trestoreall\n\tret\n\n", info->asmOutput);
	}
}

/*
 * Flush a pending "goto return point".
 */
static void FlushReturn(ILGenInfo *info)
{
	PMGenInfo *pminfo = info->pminfo;
	if(pminfo->prevIsReturn)
	{
		pminfo->prevIsReturn = 0;
		PMGenJump(info, &(pminfo->retLabel));
	}
}

void PMGenReturn(ILGenInfo *info, PMRegister retValue)
{
	PMGenInfo *pminfo = info->pminfo;
	FlushReturn(info);
	if(info->asmOutput)
	{
		if(ILTypeGetReturn(pminfo->signature) != ILType_Void)
		{
			fputs("\t.return ", info->asmOutput);
			PMGenRegister(info, retValue);
			putc('\n', info->asmOutput);
		}
		pminfo->prevIsReturn = 1;
	}
}

void PMGenRegister(ILGenInfo *info, PMRegister reg)
{
	const char *prefix
	switch(reg & 7)
	{
		case PM_ARG_REG:		prefix = "A"; break;
		case PM_LOCAL_REG:		prefix = "L"; break;
		case PM_TEMP_INT_REG:	prefix = "$I"; break;
		case PM_TEMP_NUM_REG:	prefix = "$N"; break;
		case PM_TEMP_PMC_REG:	prefix = "$P"; break;
		case PM_TEMP_STR_REG:	prefix = "$S"; break;
		default:				prefix = "R"; break;
	}
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "%s%lu", prefix, (unsigned long)(reg >> 3));
	}
}

PMRegister PMGenTempReg(ILGenInfo *info, ILMachineType type)
{
	/* TODO */
}

PMRegister PMGenArgReg(ILGenInfo *info, ILUInt32 num)
{
	/* TODO */
}

PMRegister PMGenLocalReg(ILGenInfo *info, ILUInt32 num)
{
	/* TODO */
}

void PMGenMarkRegNonDest(ILGenInfo *info, PMRegister reg)
{
	int index = (int)(reg >> 3);
	if(index >= 0 && index < info->pminfo->numRegs)
	{
		info->pminfo[index].isNonDest = 1;
	}
}

ILMachineType PMGenRegType(ILGenInfo *info, PMRegister reg)
{
	int index = (int)(reg >> 3);
	if(index >= 0 && index < info->pminfo->numRegs)
	{
		return info->pminfo[index].type;
	}
	else
	{
		return ILMachineType_Void;
	}
}

/*
 * Determine if a register is valid as a destination and has
 * a specific machine type associated with it.
 */
static int IsDestReg(ILGenInfo *info, PMRegister reg, ILMachineType type)
{
	int index = (int)(reg >> 3);
	if(index >= 0 && index < info->pminfo->numRegs)
	{
		return (!(info->pminfo[index].isNonDest) &&
				info->pminfo[index].type == type);
	}
	else
	{
		return 0;
	}
}

PMRegister PMGenDestUnary(ILGenInfo *info, PMRegister arg, ILMachineType type)
{
	if(IsDestReg(info, arg, type))
	{
		return arg;
	}
	else
	{
		return PMGenTempReg(info, type);
	}
}

PMRegister PMGenDestBinary(ILGenInfo *info, PMRegister arg1,
						   PMRegister arg2, ILMachineType type)
{
	if(IsDestReg(info, arg1, type))
	{
		return arg1;
	}
	else if(IsDestReg(info, arg2, type))
	{
		return arg2;
	}
	else
	{
		return PMGenTempReg(info, type);
	}
}

void PMGenCopy(ILGenInfo info, PMRegster dest, PMRegister src)
{
	FlushReturn(info);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		PMGenRegister(info, dest);
		fputs(" = ", info->asmOutput);
		PMGenRegister(info, src);
		putc('\n', info->asmOutput);
	}
}

void PMGenFetchKeyed(ILGenInfo info, PMRegster dest, PMRegister aggregate,
					 PMRegister key)
{
	FlushReturn(info);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		PMGenRegister(info, dest);
		fputs(" = ", info->asmOutput);
		PMGenRegister(info, aggregate);
		putc('[', info->asmOutput);
		PMGenRegister(info, key);
		fputs("]\n", info->asmOutput);
	}
}

void PMGenStoreKeyed(ILGenInfo info, PMRegister aggregate, PMRegister key,
					 PMRegister src)
{
	FlushReturn(info);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		PMGenRegister(info, aggregate);
		putc('[', info->asmOutput);
		PMGenRegister(info, key);
		fputs("] = ", info->asmOutput);
		PMGenRegister(info, src);
		putc('\n', info->asmOutput);
	}
}

void PMGenUnary(ILGenInfo *info, PMOperation oper, PMRegister dest,
				PMRegister src)
{
	FlushReturn(info);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(OpNames[oper] == '@')
		{
			fprintf(info->asmOutput, "%s ", OpNames[oper] + 1);
			PMGenRegister(info, dest);
			fputs(", ", info->asmOutput);
			PMGenRegister(info, src);
		}
		else
		{
			PMGenRegister(info, dest);
			fprintf(info->asmOutput, " = %s ", OpNames[oper]);
			PMGenRegister(info, src);
		}
		putc('\n', info->asmOutput);
	}
}

void PMGenBinary(ILGenInfo *info, PMOperation oper, PMRegister dest,
				 PMRegister src1, PMRegister src2)
{
	FlushReturn(info);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		if(OpNames[oper] == '@')
		{
			fprintf(info->asmOutput, "%s ", OpNames[oper] + 1);
			PMGenRegister(info, dest);
			fputs(", ", info->asmOutput);
			PMGenRegister(info, src1);
			fputs(", ", info->asmOutput);
			PMGenRegister(info, src2);
		}
		else
		{
			PMGenRegister(info, dest);
			fputs(" = ", info->asmOutput);
			PMGenRegister(info, src1);
			fprintf(info->asmOutput, " %s ", OpNames[oper]);
			PMGenRegister(info, src2);
		}
		putc('\n', info->asmOutput);
	}
}

void PMGenLabel(ILGenInfo *info, ILLabel *label)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Flush a pending return, if any */
	FlushReturn(info);

	/* Clear the "null" register if it is possible that control
	   could reach here with the register not yet set */
	if(!(pminfo->nullRegDuringSetup))
	{
		pminfo->nullReg = PM_INVALID_REG;
	}

	/* Allocate a new label if necessary */
	if(*label == ILLabel_Undefined)
	{
		*label = (pminfo->nextLabel)++;
	}

	/* Output the label */
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "L_%lu:\n", (unsigned long)(*label));
	}
}

void PMGenBranch(ILGenInfo *info, PMOperation oper, PMRegister src1,
				 PMRegister src2, ILLabel *label)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Flush a pending return, if any */
	FlushReturn(info);

	/* Allocate a new label if necessary */
	if(*label == ILLabel_Undefined)
	{
		*label = (pminfo->nextLabel)++;
	}

	/* Generate the jump instruction */
	if(info->asmOutput)
	{
		fputs("\tif ", info->asmOutput);
		PMGenRegister(info, src1);
		fprintf(info->asmOutput, " %s ", OpNames[oper]);
		PMGenRegister(info, src2);
		fprintf(info->asmOutput, " goto L_%lu\n", (unsigned long)(*label));
	}
}

void PMGenBranchTrue(ILGenInfo *info, PMRegister src, ILLabel *label)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Flush a pending return, if any */
	FlushReturn(info);

	/* Allocate a new label if necessary */
	if(*label == ILLabel_Undefined)
	{
		*label = (pminfo->nextLabel)++;
	}

	/* Generate the jump instruction */
	if(info->asmOutput)
	{
		fputs("\tif ", info->asmOutput);
		PMGenRegister(info, src);
		fprintf(info->asmOutput, " goto L_%lu\n", (unsigned long)(*label));
	}
}

void PMGenBranchFalse(ILGenInfo *info, PMRegister src, ILLabel *label)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Flush a pending return, if any */
	FlushReturn(info);

	/* Allocate a new label if necessary */
	if(*label == ILLabel_Undefined)
	{
		*label = (pminfo->nextLabel)++;
	}

	/* Generate the jump instruction */
	if(info->asmOutput)
	{
		fputs("\tunless ", info->asmOutput);
		PMGenRegister(info, src);
		fprintf(info->asmOutput, " goto L_%lu\n", (unsigned long)(*label));
	}
}

void PMGenJump(ILGenInfo *info, ILLabel *label)
{
	PMGenInfo *pminfo = info->pminfo;

	/* Flush a pending return, if any */
	FlushReturn(info);

	/* Allocate a new label if necessary */
	if(*label == ILLabel_Undefined)
	{
		*label = (pminfo->nextLabel)++;
	}

	/* Generate the jump instruction */
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "\tgoto L_%lu\n", (unsigned long)(*label));
	}
}

PMRegister PMGenNull(ILGenInfo *info)
{
	PMGenInfo *pminfo = info->pminfo;
	if(pminfo->nullReg == PM_INVALID_REG)
	{
		/* The null value is a preset PMC of type "undef", which we try
		   to cache so that we don't need to fetch it multiple times */
		pminfo->nullReg = PMGenTempReg(info, ILMachineType_ObjectRef);
		PMGenMarkRegNonDest(info, pminfo->nullReg);
		if(info->asmOutput)
		{
			putc("\t", info->asmOutput);
			PMGenRegister(info, pminfo->nullReg);
			fputs(" = global \"cs.null\"\n", info->asmOutput);
		}
	}
	return pminfo->nullReg;
}

PMRegister PMGenInt32(ILGenInfo *info, ILInt32 num)
{
	PMRegister reg = PMGenTempReg(info, ILMachineType_Int32);
	if(info->asmOutput)
	{
		putc('\t', info->asmOutput);
		PMGenRegister(info, reg);
		fprintf(info->asmOutput, " = %ld\n", (long)num);
	}
	return reg;
}

PMRegister PMGenUInt32(ILGenInfo *info, ILUInt32 num)
{
	PMRegister reg = PMGenTempReg(info, ILMachineType_UInt32);
	if(info->asmOutput)
	{
		/* TODO: what if the Parrot int is 64 bits? */
		putc('\t', info->asmOutput);
		PMGenRegister(info, reg);
		fprintf(info->asmOutput, " = %lu\n", (unsigned long)num);
	}
	return reg;
}

PMRegister PMGenInt64(ILGenInfo *info, ILInt64 num)
{
	/* TODO */
	return PM_INVALID_REG;
}

PMRegister PMGenUInt64(ILGenInfo *info, ILUInt64 num)
{
	/* TODO */
	return PM_INVALID_REG;
}

PMRegister PMGenFloat32(ILGenInfo *info, ILFloat num)
{
	return PMGenFloat64(info, (ILDouble)num);
}

PMRegister PMGenFloat64(ILGenInfo *info, ILDouble num)
{
	/* TODO */
	return PM_INVALID_REG;
}

PMRegister PMGenString(ILGenInfo *info, const char *str, int len)
{
	/* TODO */
	return PM_INVALID_REG;
}

#ifdef	__cplusplus
};
#endif
