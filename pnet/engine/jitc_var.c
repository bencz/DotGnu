/*
 * jitc_var.c - Coder implementation for JIT variables.
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

#ifdef IL_JITC_CODE

/*
 * Handle a load from an argument.
 */
static void JITCoder_LoadArg(ILCoder *coder, ILUInt32 argNum, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadArg: %i\n", 
			argNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPushArg(jitCoder, argNum);
}

/*
 * Handle a load from a local variable.
 */
static void JITCoder_LoadLocal(ILCoder *coder, ILUInt32 localNum, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadLocal: %i\n", 
			localNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPushLocal(jitCoder, localNum);
}

/*
 * Handle a store to an argument.
 */
static void JITCoder_StoreArg(ILCoder *coder, ILUInt32 argNum,
							  ILEngineType engineType, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"StoreArg: %i\n", 
			argNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPopToArg(jitCoder, argNum);
}

/*
 * Handle a store to a local variable.
 */
static void JITCoder_StoreLocal(ILCoder *coder, ILUInt32 localNum,
								ILEngineType engineType, ILType *type)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"StoreLocal: %i\n", 
			localNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPopToLocal(jitCoder, localNum);
}

/*
 * Load the address of an argument onto the stack.
 */
static void JITCoder_AddrOfArg(ILCoder *coder, ILUInt32 argNum)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Address of Arg: %i\n", 
			argNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPushAddressOfArg(jitCoder, argNum);
}

/*
 * Load the address of a local onto the stack.
 */
static void JITCoder_AddrOfLocal(ILCoder *coder, ILUInt32 localNum)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Address of Local: %i\n", 
			localNum);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPushAddressOfLocal(jitCoder, localNum);
}

/*
 * Allocate local stack space.
 */
static void JITCoder_LocalAlloc(ILCoder *coder, ILEngineType sizeType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(value) ;
	ILJitValue address;

	_ILJitStackPop(jitCoder, value);
	address = jit_insn_alloca(jitCoder->jitFunction, _ILJitStackItemValue(value));
	_ILJitStackPushNotNullValue(jitCoder, address);
}

#endif	/* IL_JITC_CODE */
