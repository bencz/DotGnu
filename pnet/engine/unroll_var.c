/*
 * unroll_var.c - Variable handling for CVM unrolling.
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

#ifdef IL_UNROLL_CASES
case COP_ILOAD_0:
{
	/* Unroll an access to frame variable 0 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 0, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_word_32(unroll.out, reg, MD_REG_FRAME, 0);
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PLOAD_0:
{
	/* Unroll an access to frame variable 0 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 0, MD_REG1_NATIVE);
	if(reg != -1)
	{
		md_load_membase_word_native(unroll.out, reg, MD_REG_FRAME, 0);
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ILOAD_1:
{
	/* Unroll an access to frame variable 1 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 1, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_word_32(unroll.out, reg,
								MD_REG_FRAME, sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PLOAD_1:
{
	/* Unroll an access to frame variable 1 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 1, MD_REG1_NATIVE);
	if(reg != -1)
	{
		md_load_membase_word_native(unroll.out, reg,
									MD_REG_FRAME, sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ILOAD_2:
{
	/* Unroll an access to frame variable 2 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 2, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_word_32(unroll.out, reg,
								MD_REG_FRAME, 2 * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PLOAD_2:
{
	/* Unroll an access to frame variable 2 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 2, MD_REG1_NATIVE);
	if(reg != -1)
	{
		md_load_membase_word_native(unroll.out, reg,
									MD_REG_FRAME, 2 * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ILOAD_3:
{
	/* Unroll an access to frame variable 3 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 3, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_word_32(unroll.out, reg,
								MD_REG_FRAME, 3 * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PLOAD_3:
{
	/* Unroll an access to frame variable 3 */
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, 3, MD_REG1_NATIVE);
	if(reg != -1)
	{
		md_load_membase_word_native(unroll.out, reg,
									MD_REG_FRAME, 3 * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ILOAD:
{
	/* Unroll an access to an arbitrary frame variable */
	unsigned temp = CVM_ARG_WIDE_SMALL;
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, temp, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_word_32(unroll.out, reg,
								MD_REG_FRAME, temp * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_WIDE_SMALL);
}
break;

case COP_PLOAD:
{
	/* Unroll an access to an arbitrary frame variable */
	unsigned temp = CVM_ARG_WIDE_SMALL;
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, temp, MD_REG1_NATIVE);
	if(reg != -1)
	{
		md_load_membase_word_native(unroll.out, reg,
								    MD_REG_FRAME, temp * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_WIDE_SMALL);
}
break;

case COP_BLOAD:
{
	/* Unroll an access to a byte frame variable */
	unsigned temp = CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetCachedWordRegister(&unroll, temp, MD_REG1_32BIT);
	if(reg != -1)
	{
		md_load_membase_byte(unroll.out, reg,
							 MD_REG_FRAME, temp * sizeof(CVMWord));
	}
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_ISTORE_0:
{
	/* Unroll a store to frame variable 0 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_word_32(unroll.out, reg, MD_REG_FRAME, 0);
	FreeTopRegister(&unroll, 0);
	if(unroll.thisValidated > 0)
	{
		/* The "this" variable must be re-validated */
		unroll.thisValidated = 0;
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSTORE_0:
{
	/* Unroll a store to frame variable 0 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_store_membase_word_native(unroll.out, reg, MD_REG_FRAME, 0);
	FreeTopRegister(&unroll, 0);
	if(unroll.thisValidated > 0)
	{
		/* The "this" variable must be re-validated */
		unroll.thisValidated = 0;
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISTORE_1:
{
	/* Unroll a store to frame variable 1 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_word_32(unroll.out, reg,
							 MD_REG_FRAME, sizeof(CVMWord));
	FreeTopRegister(&unroll, 1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSTORE_1:
{
	/* Unroll a store to frame variable 1 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_store_membase_word_native(unroll.out, reg,
								 MD_REG_FRAME, sizeof(CVMWord));
	FreeTopRegister(&unroll, 1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISTORE_2:
{
	/* Unroll a store to frame variable 2 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_word_32(unroll.out, reg,
							 MD_REG_FRAME, 2 * sizeof(CVMWord));
	FreeTopRegister(&unroll, 2);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSTORE_2:
{
	/* Unroll a store to frame variable 2 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_store_membase_word_native(unroll.out, reg,
								 MD_REG_FRAME, 2 * sizeof(CVMWord));
	FreeTopRegister(&unroll, 2);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISTORE_3:
{
	/* Unroll a store to frame variable 3 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_word_32(unroll.out, reg,
							 MD_REG_FRAME, 3 * sizeof(CVMWord));
	FreeTopRegister(&unroll, 3);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSTORE_3:
{
	/* Unroll a store to frame variable 3 */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_store_membase_word_native(unroll.out, reg,
								 MD_REG_FRAME, 3 * sizeof(CVMWord));
	FreeTopRegister(&unroll, 3);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISTORE:
{
	/* Unroll a store to an arbitrary frame variable */
	unsigned temp = CVM_ARG_WIDE_SMALL;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_word_32(unroll.out, reg,
						     MD_REG_FRAME, temp * sizeof(CVMWord));
	FreeTopRegister(&unroll, temp);
	if(temp == 0 && unroll.thisValidated > 0)
	{
		/* The "this" variable must be re-validated */
		unroll.thisValidated = 0;
	}
	MODIFY_UNROLL_PC(CVM_LEN_WIDE_SMALL);
}
break;

case COP_PSTORE:
{
	/* Unroll a store to an arbitrary frame variable */
	unsigned temp = CVM_ARG_WIDE_SMALL;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_store_membase_word_native(unroll.out, reg,
						         MD_REG_FRAME, temp * sizeof(CVMWord));
	FreeTopRegister(&unroll, temp);
	if(temp == 0 && unroll.thisValidated > 0)
	{
		/* The "this" variable must be re-validated */
		unroll.thisValidated = 0;
	}
	MODIFY_UNROLL_PC(CVM_LEN_WIDE_SMALL);
}
break;

case COP_BSTORE:
{
	/* Unroll a store to a byte frame variable */
	unsigned temp = CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_store_membase_byte(unroll.out, reg, MD_REG_FRAME,
						  temp * sizeof(CVMWord));
	FreeTopRegister(&unroll, temp);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_WADDR:
{
	/* Get the address of a frame word */
	unsigned temp = CVM_ARG_WIDE_SMALL;
	UNROLL_START();
	reg = GetWordRegister(&unroll, MD_REG1_NATIVE);
	md_lea_membase(unroll.out, reg, MD_REG_FRAME, temp * sizeof(CVMWord));
	if(!temp)
	{
		/* We don't know if someone might write to this address,
		   so we have to assume that "this" will need to be
		   validated always from now on */
		unroll.thisValidated = -1;
	}
	MODIFY_UNROLL_PC(CVM_LEN_WIDE_SMALL);
}
break;

case COP_MK_LOCAL_1:
{
	/* Make a single local variable word */
	UNROLL_START();
	FlushRegisterStack(&unroll);
	md_clear_membase_start(unroll.out);
	md_clear_membase(unroll.out, MD_REG_STACK, unroll.stackHeight);
	unroll.stackHeight += sizeof(CVMWord);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_MK_LOCAL_2:
{
	/* Make two local variable words */
	UNROLL_START();
	FlushRegisterStack(&unroll);
	md_clear_membase_start(unroll.out);
	md_clear_membase(unroll.out, MD_REG_STACK, unroll.stackHeight);
	md_clear_membase(unroll.out, MD_REG_STACK,
					 unroll.stackHeight + sizeof(CVMWord));
	unroll.stackHeight += 2 * sizeof(CVMWord);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_MK_LOCAL_3:
{
	/* Make three local variable words */
	UNROLL_START();
	FlushRegisterStack(&unroll);
	md_clear_membase_start(unroll.out);
	md_clear_membase(unroll.out, MD_REG_STACK, unroll.stackHeight);
	md_clear_membase(unroll.out, MD_REG_STACK,
					 unroll.stackHeight + sizeof(CVMWord));
	md_clear_membase(unroll.out, MD_REG_STACK,
					 unroll.stackHeight + 2 * sizeof(CVMWord));
	unroll.stackHeight += 3 * sizeof(CVMWord);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_DUP:
{
	/* Duplicate the top word on the stack */
	UNROLL_START();
	reg = PeekTopWordRegister(&unroll);
	if(reg != -1)
	{
		/* The top is already in a register, so move it to a new register */
		int flags = GetTopRegisterFlags(&unroll);
		reg2 = GetWordRegister(&unroll, flags);
		if(unroll.pseudoStackSize > 1)
		{
			md_mov_reg_reg(unroll.out, reg2, reg);
		}
		else
		{
			/* "GetWordRegister" flushed all registers, so the value
			   we want to duplicate is now on the CVM stack */
			if((flags & MD_REGN_NATIVE) != 0)
			{
				md_load_membase_word_native
						(unroll.out, reg2, MD_REG_STACK,
					     unroll.stackHeight - sizeof(CVMWord));
			}
			else
			{
				md_load_membase_word_32
						(unroll.out, reg2, MD_REG_STACK,
						 unroll.stackHeight - sizeof(CVMWord));
			}
		}
	}
	else
	{
		int i = 0;
		int j = 0;
		/* We don't know if the top is 32-bit or native, so copy
		   the entire native word but leave it on the CVM stack.
		   Even worse, we don't even know if one CVMWord fits 
		   inside a native register, hence we need to keep 
		   copying all the data. A later reload will determine 
		   the true size and type. */
		FlushRegisterStack(&unroll);

		for(i = 0, j = 0; i < sizeof(CVMWord); 
						i = i + sizeof(ILNativeUInt), ++j)
		{
			/* just for fun, I thought I'd use a bunch of regs */
			if(regAllocOrder[j] == -1) j = 0;
			
			md_load_membase_word_native
					(unroll.out, regAllocOrder[j],
					 MD_REG_STACK, 
					 unroll.stackHeight - sizeof(CVMWord) + i);
			md_store_membase_word_native
					(unroll.out, regAllocOrder[j], MD_REG_STACK, 
						unroll.stackHeight + i);
		}
		
		unroll.stackHeight += sizeof(CVMWord);
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_POP:
{
	/* Pop the top word on the stack */
	UNROLL_START();
	reg = PeekTopWordRegister(&unroll);
	if(reg != -1)
	{
		/* Abandon the register's contents */
		FreeTopRegister(&unroll, -1);
	}
	else
	{
		/* Flush the register stack and then decrease its height */
		FlushRegisterStack(&unroll);
		unroll.stackHeight -= sizeof(CVMWord);
	}
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;
#endif /* IL_UNROLL_CASES */
