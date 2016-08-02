/*
 * verify_var.c - Verify instructions related to variables.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if defined(IL_VERIFY_GLOBALS)

	/* Nothing to do here */
	
#elif defined(IL_VERIFY_LOCALS)

ILUInt32 argNum;

#else /* IL_VERIFY_CODE */

case IL_OP_LDARG_0:
{
	/* If argument 0 looks like a "this" pointer, and this instruction
	   is followed by "ldfld", then we attempt to optimise field loads */
	argNum = 0;
	if(argNum >= numArgs)
	{
		VERIFY_INSN_ERROR();
	}
	stack[stackSize].typeInfo = _ILCoderGetParamType(signature, method, argNum);
	if(ILType_IsClass(stack[stackSize].typeInfo))
	{
		if(len >= 6 && pc[1] == IL_OP_LDFLD &&
		   !IsJumpTarget(jumpMask, offset + 1))
		{
			/* We have a match on the pattern "ldarg.0, ldfld".
			   See if the field is valid and instance-based */
			fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc + 1);
			if(fieldInfo)
			{
				classType = ILField_Type(fieldInfo);
				if(IsSubClass(stack[stackSize].typeInfo,
							  ILField_Owner(fieldInfo)))
				{
					if(!ILField_IsStatic(fieldInfo))
					{
						/* Load the "this"-based field onto the stack */
						ILCoderLoadThisField(coder, fieldInfo, classType,
											 &prefixInfo);

						/* Push the field's type onto the stack */
						stack[stackSize].engineType =
							TypeToEngineType(classType);
						stack[stackSize].typeInfo = classType;
						++stackSize;

						/* Skip the following "ldfld" instruction because
						   we have just handled it */
						insnSize += 5;
						break;
					}
				}
			}
		}
	}
	goto checkLDArg2;
}
/* Not reached */

case IL_OP_LDARG_1:
case IL_OP_LDARG_2:
case IL_OP_LDARG_3:
{
	/* Load an argument onto the stack - tiny form */
	argNum = opcode - IL_OP_LDARG_0;
	goto checkLDArg;
}
/* Not reached */

case IL_OP_LDARG_S:
{
	/* Load an argument onto the stack - short form */
	argNum = (ILUInt32)(pc[1]);
	goto checkLDArg;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_LDARG:
{
	/* Load an argument onto the stack */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkLDArg:
	if(argNum >= numArgs)
	{
		VERIFY_INSN_ERROR();
	}
	stack[stackSize].typeInfo = _ILCoderGetParamType(signature, method, argNum);
checkLDArg2:
	if(stack[stackSize].typeInfo == ILType_Invalid)
	{
		/* The "this" parameter is being passed as a managed pointer */
		classInfo = ILMethod_Owner(method);
		stack[stackSize].engineType = ILEngineType_M;
		stack[stackSize].typeInfo = ILClassToType(classInfo);

		/* Fool the coder into thinking that the parameter is a pointer
		   so that it doesn't try to load an entire managed value */
		ILCoderLoadArg(coder, argNum, ILType_FromClass(classInfo));
	}
	else if((stack[stackSize].engineType =
				TypeToEngineType(stack[stackSize].typeInfo)) == ILEngineType_M)
	{
		/* Convert the type of a "BYREF" parameter */
		ILCoderLoadArg(coder, argNum, stack[stackSize].typeInfo);
		stack[stackSize].typeInfo = ILType_Ref(stack[stackSize].typeInfo);
	}
	else if(!IsUnsafeType(stack[stackSize].typeInfo) || unsafeAllowed)
	{
		ILCoderLoadArg(coder, argNum, stack[stackSize].typeInfo);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
	++stackSize;
}
break;

case IL_OP_STARG_S:
{
	/* Store the top of the stack into an argument */
	argNum = (ILUInt32)(pc[1]);
	goto checkSTArg;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_STARG:
{
	/* Store the top of the stack into an argument */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkSTArg:
	if(argNum >= numArgs)
	{
		VERIFY_INSN_ERROR();
	}
	type = _ILCoderGetParamType(signature, method, argNum);
	if(type == ILType_Invalid)
	{
		/* Storing into the "this" argument of a value type method.
		   This should be done using "stobj" instead */
		VERIFY_TYPE_ERROR();
	}
	else if(!AssignCompatible(method, &(stack[stackSize - 1]),
							  type, unsafeAllowed))
	{
		VERIFY_TYPE_ERROR();
	}
	ILCoderStoreArg(coder, argNum,
				    stack[stackSize - 1].engineType, type);
	--stackSize;
}
break;

case IL_OP_LDLOC_0:
case IL_OP_LDLOC_1:
case IL_OP_LDLOC_2:
case IL_OP_LDLOC_3:
{
	/* Load one of the first four locals onto the stack */
	argNum = opcode - IL_OP_LDLOC_0;
	goto checkLDLoc;
}
/* Not reached */

case IL_OP_LDLOC_S:
{
	/* Load a local variable onto the stack */
	argNum = (ILUInt32)(pc[1]);
	goto checkLDLoc;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_LDLOC:
{
	/* Load a local variable onto the stack */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkLDLoc:
	if(argNum >= numLocals)
	{
		VERIFY_INSN_ERROR();
	}
	stack[stackSize].typeInfo =
			ILTypeGetLocal(localVars, argNum);
	if((stack[stackSize].engineType =
			TypeToEngineType(stack[stackSize].typeInfo)) == ILEngineType_M)
	{
		/* Convert the type of a "BYREF" local */
		ILCoderLoadLocal(coder, argNum, stack[stackSize].typeInfo);
		stack[stackSize].typeInfo = ILType_Ref(stack[stackSize].typeInfo);
	}
	else if(IsUnsafeType(stack[stackSize].typeInfo) && !unsafeAllowed)
	{
		VERIFY_TYPE_ERROR();
	}
	else
	{
		ILCoderLoadLocal(coder, argNum, stack[stackSize].typeInfo);
	}
	++stackSize;
}
break;

case IL_OP_STLOC_0:
case IL_OP_STLOC_1:
case IL_OP_STLOC_2:
case IL_OP_STLOC_3:
{
	/* Store the stack top to one of the first four locals */
	argNum = opcode - IL_OP_STLOC_0;
	goto checkSTLoc;
}
/* Not reached */

case IL_OP_STLOC_S:
{
	/* Store the top of the stack into a local variable */
	argNum = (ILUInt32)(pc[1]);
	goto checkSTLoc;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_STLOC:
{
	/* Store the top of the stack into a local variable */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkSTLoc:
	if(argNum >= numLocals)
	{
		VERIFY_INSN_ERROR();
	}
	type = ILTypeGetLocal(localVars, argNum);
	if(!AssignCompatible(method, &(stack[stackSize - 1]),
						 type, unsafeAllowed))
	{
		VERIFY_TYPE_ERROR();
	}
	ILCoderStoreLocal(coder, argNum,
				      stack[stackSize - 1].engineType, type);
	--stackSize;
}
break;

case IL_OP_LDARGA_S:
{
	/* Load the address of an argument onto the stack */
	argNum = (ILUInt32)(pc[1]);
	goto checkLDArgA;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_LDARGA:
{
	/* Load the address of an argument onto the stack */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkLDArgA:
	if(argNum >= numArgs)
	{
		VERIFY_INSN_ERROR();
	}
	stack[stackSize].typeInfo = _ILCoderGetParamType(signature, method, argNum);
	if(stack[stackSize].typeInfo == ILType_Invalid)
	{
		/* Cannot take the address of the "this" parameter in
		   a value type method: use "ldarg" instead */
		VERIFY_TYPE_ERROR();
	}
	stack[stackSize].engineType = ILEngineType_T;
	ILCoderAddrOfArg(coder, argNum);
	++stackSize;
}
break;

case IL_OP_LDLOCA_S:
{
	/* Load the address of a local variable onto the stack */
	argNum = (ILUInt32)(pc[1]);
	goto checkLDLocA;
}
/* Not reached */

case IL_OP_PREFIX + IL_PREFIX_OP_LDLOCA:
{
	/* Load the address of a local variable onto the stack */
	argNum = (ILUInt32)(IL_READ_UINT16(pc + 2));
checkLDLocA:
	if(argNum >= numLocals)
	{
		VERIFY_INSN_ERROR();
	}
	stack[stackSize].typeInfo = ILTypeGetLocal(localVars, argNum);
	stack[stackSize].engineType = ILEngineType_T;
	ILCoderAddrOfLocal(coder, argNum);
	++stackSize;
}
break;

#endif /* IL_VERIFY_CODE */
