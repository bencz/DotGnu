/*
 * verify_stack.c - Verify instructions related to stack manipulation.
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

/* No globals required */

#elif defined(IL_VERIFY_LOCALS)

/* No locals required */

#else /* IL_VERIFY_CODE */

case IL_OP_DUP:
{
	/* Duplicate the current top of stack */
	ILCoderDup(coder, stack[stackSize - 1].engineType,
			   stack[stackSize - 1].typeInfo);
	stack[stackSize] = stack[stackSize - 1];
	++stackSize;
}
break;

case IL_OP_POP:
{
	/* Pop the current top of stack */
	ILCoderPop(coder, stack[stackSize - 1].engineType,
			   stack[stackSize - 1].typeInfo);
	--stackSize;
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_LOCALLOC:
{
	/* Allocate a block of memory from the local stack */
	if(unsafeAllowed)
	{
		if(stackSize == 1)
		{
			if(STK_UNARY == ILEngineType_I4 ||
			   STK_UNARY == ILEngineType_I)
			{
				ILCoderLocalAlloc(coder, STK_UNARY);
				stack[stackSize - 1].engineType = ILEngineType_T;
				stack[stackSize - 1].typeInfo = ILType_Int8;
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else
		{
			/* The stack must only contain a size or a run-time error occurs */
			ThrowSystem("System", "ExecutionEngineException");
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

#endif /* IL_VERIFY_CODE */
