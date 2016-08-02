/*
 * jitc_math.c - Jit coder inline functions for the System.Math class.
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

#ifdef	IL_JITC_DECLARATIONS

/*
 * Inline function to handle the calls to System.Math.Abs().
 */
static int _ILJitSystemMathAbs(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Acos().
 */
static int _ILJitSystemMathAcos(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Asin().
 */
static int _ILJitSystemMathAsin(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Atan().
 */
static int _ILJitSystemMathAtan(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Atan2().
 */
static int _ILJitSystemMathAtan2(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Ceiling().
 */
static int _ILJitSystemMathCeiling(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILCoderMethodInfo *methodInfo,
								   ILJitStackItem *args,
								   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Cos().
 */
static int _ILJitSystemMathCos(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Cosh().
 */
static int _ILJitSystemMathCosh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Exp().
 */
static int _ILJitSystemMathExp(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Floor().
 */
static int _ILJitSystemMathFloor(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.IEEERemainder().
 */
static int _ILJitSystemMathIEEERemainder(ILJITCoder *jitCoder,
										 ILMethod *method,
										 ILCoderMethodInfo *methodInfo,
										 ILJitStackItem *args,
										 ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Log().
 * For now only the call with one arg can be handled here (base e).
 * TODO: Handle the Log with a specified base.
 */
static int _ILJitSystemMathLog(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Log10().
 */
static int _ILJitSystemMathLog10(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Max().
 */
static int _ILJitSystemMathMax(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Min().
 */
static int _ILJitSystemMathMin(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Pow().
 */
static int _ILJitSystemMathPow(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Sign().
 */
static int _ILJitSystemMathSign(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Sin().
 */
static int _ILJitSystemMathSin(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Sinh().
 */
static int _ILJitSystemMathSinh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Sqrt().
 */
static int _ILJitSystemMathSqrt(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Tan().
 */
static int _ILJitSystemMathTan(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs);

/*
 * Inline function to handle the calls to System.Math.Tanh().
 */
static int _ILJitSystemMathTanh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Inline function to handle the calls to System.Math.Abs().
 */
static int _ILJitSystemMathAbs(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_abs(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Acos().
 */
static int _ILJitSystemMathAcos(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_acos(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Asin().
 */
static int _ILJitSystemMathAsin(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_asin(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Atan().
 */
static int _ILJitSystemMathAtan(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_atan(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Atan2().
 */
static int _ILJitSystemMathAtan2(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_atan2(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType),
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[1]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Ceiling().
 */
static int _ILJitSystemMathCeiling(ILJITCoder *jitCoder,
								   ILMethod *method,
								   ILCoderMethodInfo *methodInfo,
								   ILJitStackItem *args,
								   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_ceil(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Cos().
 */
static int _ILJitSystemMathCos(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_cos(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Cosh().
 */
static int _ILJitSystemMathCosh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_cosh(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Exp().
 */
static int _ILJitSystemMathExp(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_exp(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Floor().
 */
static int _ILJitSystemMathFloor(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_floor(jitCoder->jitFunction,
									  _ILJitValueConvertImplicit(jitCoder->jitFunction,
																 _ILJitStackItemValue(args[0]),
																 returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.IEEERemainder().
 */
static int _ILJitSystemMathIEEERemainder(ILJITCoder *jitCoder,
										 ILMethod *method,
										 ILCoderMethodInfo *methodInfo,
										 ILJitStackItem *args,
										 ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_rem_ieee(jitCoder->jitFunction,
										 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																	_ILJitStackItemValue(args[0]),
																	returnType),
										 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																	_ILJitStackItemValue(args[1]),
																	returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Log().
 * For now only the call with one arg can be handled here (base e).
 * TODO: Handle the Log with a specified base.
 */
static int _ILJitSystemMathLog(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_log(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Log10().
 */
static int _ILJitSystemMathLog10(ILJITCoder *jitCoder,
								 ILMethod *method,
								 ILCoderMethodInfo *methodInfo,
								 ILJitStackItem *args,
								 ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_log10(jitCoder->jitFunction,
									  _ILJitValueConvertImplicit(jitCoder->jitFunction,
																 _ILJitStackItemValue(args[0]),
																 returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Max().
 */
static int _ILJitSystemMathMax(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_max(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType),
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[1]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Min().
 */
static int _ILJitSystemMathMin(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_min(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType),
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[1]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Pow().
 */
static int _ILJitSystemMathPow(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_pow(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType),
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[1]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Sign().
 */
static int _ILJitSystemMathSign(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType argType;
	ILJitValue returnValue;
	unsigned int jitNumArgs;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}

	jitNumArgs = jit_type_num_params(signature);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(jitNumArgs != 2)
#else
	if(jitNumArgs != 1)
#endif
	{
		/* wrong number of args. */
		return 0;
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(!(argType = jit_type_get_param(signature, 1)))
#else
	if(!(argType = jit_type_get_param(signature, 0)))
#endif
	{
		return 0;
	}

	if(!(returnValue = jit_insn_sign(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																argType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Sin().
 */
static int _ILJitSystemMathSin(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_sin(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Sinh().
 */
static int _ILJitSystemMathSinh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_sinh(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Sqrt().
 */
static int _ILJitSystemMathSqrt(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_sqrt(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Tan().
 */
static int _ILJitSystemMathTan(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_tan(jitCoder->jitFunction,
									_ILJitValueConvertImplicit(jitCoder->jitFunction,
															   _ILJitStackItemValue(args[0]),
															   returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Inline function to handle the calls to System.Math.Tanh().
 */
static int _ILJitSystemMathTanh(ILJITCoder *jitCoder,
								ILMethod *method,
								ILCoderMethodInfo *methodInfo,
								ILJitStackItem *args,
								ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature;
	ILJitType returnType;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}
	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	/* The return type is equal to the param type so we can use it for both. */
	if(!(returnType = jit_type_get_return(signature)))
	{
		return 0;
	}

	if(!(returnValue = jit_insn_tanh(jitCoder->jitFunction,
									 _ILJitValueConvertImplicit(jitCoder->jitFunction,
																_ILJitStackItemValue(args[0]),
																returnType))))
	{
		return 0;
	}
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

#endif	/* IL_JITC_FUNCTIONS */

