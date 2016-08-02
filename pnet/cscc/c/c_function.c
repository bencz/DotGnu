/*
 * c_function.c - Declare and output functions for the C language.
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

#include <cscc/c/c_internal.h>
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Variables that keep track of special features that are used in a function.
 */
static int numSetJmpRefs = 0;
static int currSetJmpRef = 0;
static unsigned markerVar = 0;
static unsigned valueVar = 0;
static ILMethod *currMethod = 0;
static ILType *localVarSig = 0;
static int initNumber = 0;

/*
 * List of local variables and temporaries that have complex
 * types, requiring special layout constraints at runtime.
 */
typedef struct
{
	unsigned index;
	ILType *type;

} SpecialLocal;
static SpecialLocal *specials = 0;
static int numSpecials = 0;

/*
 * Report an error that resulted from the compiler trying to
 * infer the prototype of a forwardly-declared function, but
 * failing to do so correctly.
 */
static void ReportInferError(ILGenInfo *info, ILNode *node,
							 const char *name, ILType *signature)
{
	char *typeName;
	unsigned long numParams;
	unsigned long param;
	int needComma;

	/* Print the start of the error message */
	fputs(yygetfilename(node), stderr);
	fprintf(stderr, ":%ld: ", yygetlinenum(node));
	fputs(_("previously inferred prototype was `"), stderr);

	/* Print the return type */
	typeName = CTypeToName(info, CTypeGetReturn(signature));
	fputs(typeName, stderr);
	ILFree(typeName);

	/* Print the function name */
	fprintf(stderr, " %s(", name);

	/* Print the function parameter types */
	numParams = ILTypeNumParams(signature);
	needComma = 0;
	for(param = 1; param <= numParams; ++param)
	{
		if(needComma)
		{
			fputs(", ", stderr);
		}
		else
		{
			needComma = 1;
		}
		typeName = CTypeToName(info, CTypeGetParam(signature, param));
		fputs(typeName, stderr);
		ILFree(typeName);
	}

	/* Print the elipsis argument or "void" for no parameters */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK)
				== IL_META_CALLCONV_VARARG)
	{
		if(needComma)
		{
			fputs(", ...", stderr);
		}
		else
		{
			fputs("...", stderr);
		}
	}
	else if(!needComma)
	{
		fputs("void", stderr);
	}

	/* Terminate the error message */
	fputs(")'\n", stderr);
}

/*
 * Determine if two function signatures are identical.
 */
static int SameSignature(ILType *sig1, ILType *sig2, int forwardKind)
{
	if(forwardKind == C_SCDATA_FUNCTION_FORWARD_KR)
	{
		/* K&R forward definitions must have the same return type,
		   and the new definition must not be "vararg" */
		if(!CTypeIsIdentical(CTypeGetReturn(sig1),
							 CTypeGetReturn(sig2)))
		{
			return 0;
		}
		return ((ILType_CallConv(sig1) & IL_META_CALLCONV_MASK)
					!= IL_META_CALLCONV_VARARG);
	}
	else if(forwardKind == C_SCDATA_FUNCTION_INFERRED)
	{
		/* Ignore type prefixes, because we may have inferred incorrectly */
		return ILTypeIdentical(sig1, sig2);
	}
	else
	{
		/* The ANSI prototype and the redefinition must be identical */
		return CTypeIsIdentical(sig1, sig2);
	}
}

/*
 * Add initializer or finalizer attributes to a program item.
 */
static void AddInitOrFini(ILGenInfo *info, ILProgramItem *item,
						  const char *initName, const char *orderName,
						  ILInt32 order)
{
	ILClass *classInfo;
	ILClass *scope;
	ILMethod *ctor;
	ILAttribute *attr;
	unsigned char value[8];
	static ILType *orderArgs[1] = {ILType_Int32};

	/* Create the "Initializer" or "Finalizer" attribute */
	classInfo = ILTypeToClass(info, ILFindNonSystemType
			(info, initName, "OpenSystem.C"));
	scope = ILClassLookup(ILClassGlobalScope(info->image), "<Module>", 0);
	ctor = ILResolveConstructor(info, classInfo, scope, 0, 0);
	attr = ILAttributeCreate(info->image, 0);
	if(!attr)
	{
		ILGenOutOfMemory(info);
	}
	ILAttributeSetType(attr, ILToProgramItem(ctor));
	value[0] = 0x01;
	value[1] = 0x00;
	value[2] = 0x00;
	value[3] = 0x00;
	if(!ILAttributeSetValue(attr, value, 4))
	{
		ILGenOutOfMemory(info);
	}
	ILProgramItemAddAttribute(item, attr);

	/* Bail out if the "order" value is the default of zero */
	if(!order)
	{
		return;
	}

	/* Create the "InitializerOrder" or "FinalizerOrder" attribute */
	classInfo = ILTypeToClass(info, ILFindNonSystemType
			(info, orderName, "OpenSystem.C"));
	ctor = ILResolveConstructor(info, classInfo, scope, orderArgs, 1);
	attr = ILAttributeCreate(info->image, 0);
	if(!attr)
	{
		ILGenOutOfMemory(info);
	}
	ILAttributeSetType(attr, ILToProgramItem(ctor));
	value[0] = 0x01;
	value[1] = 0x00;
	value[2] = (unsigned char)(order);
	value[3] = (unsigned char)(order >> 8);
	value[4] = (unsigned char)(order >> 16);
	value[5] = (unsigned char)(order >> 24);
	value[6] = 0x00;
	value[7] = 0x00;
	if(!ILAttributeSetValue(attr, value, 8))
	{
		ILGenOutOfMemory(info);
	}
	ILProgramItemAddAttribute(item, attr);
}

ILMethod *CFunctionCreate(ILGenInfo *info, const char *name, ILNode *node,
						  CDeclSpec spec, CDeclarator decl,
						  ILNode *declaredParams)
{
	ILMethod *method;
	void *data;
	ILNode *prevNode = 0;
	char newName[64];
	ILType *signature;
	ILType *checkForward = 0;
	int forwardKind = 0;
	ILUInt32 attrs = IL_META_METHODDEF_STATIC | IL_META_METHODDEF_PUBLIC;
	ILInt32 order;

	/* Set the "extern" vs "static" attributes for the method */
	attrs = IL_META_METHODDEF_STATIC;
	if((spec.specifiers & C_SPEC_STATIC) != 0)
	{
		attrs |= IL_META_METHODDEF_PRIVATE;
	}
	else
	{
		attrs |= IL_META_METHODDEF_PUBLIC;
	}

	/* See if we already have a definition for this name */
	data = CScopeLookup(name);
	if(data)
	{
		if(CScopeGetKind(data) == C_SCDATA_FUNCTION_FORWARD)
		{
			/* Process a forward declaration from an "extern" declaration */
			checkForward = CScopeGetType(data);
			prevNode = CScopeGetNode(data);
			forwardKind = C_SCDATA_FUNCTION_FORWARD;
		}
		else if(CScopeGetKind(data) == C_SCDATA_FUNCTION_FORWARD_KR)
		{
			/* Process a forward declaration from a K&R prototype */
			checkForward = CScopeGetType(data);
			prevNode = CScopeGetNode(data);
			forwardKind = C_SCDATA_FUNCTION_FORWARD_KR;
		}
		else if(CScopeGetKind(data) == C_SCDATA_FUNCTION_INFERRED)
		{
			/* Process a forward declaration from an inferred prototype */
			checkForward = CScopeGetType(data);
			prevNode = 0;
			forwardKind = C_SCDATA_FUNCTION_INFERRED;
		}
		else
		{
			/* Report an error for the function redefinition */
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("redefinition of `%s'"), name);
			prevNode = CScopeGetNode(data);
			if(prevNode)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  _("`%s' previously defined here"), name);
			}

			/* Generate a new name to be used from now on */
			sprintf(newName, "function(%lu)",
					ILImageNumTokens
						(info->image, IL_META_TOKEN_METHOD_DEF) + 1);
			name = newName;
		}
	}

	/* Create a new method block for the function */
	method = ILMethodCreate
		(ILClass_FromToken(info->image, IL_META_TOKEN_TYPE_DEF | 1),
		 0, name, attrs);
	if(!method)
	{
		ILGenOutOfMemory(info);
	}

	/* Finalize the declarator and get the method signature */
	signature = CDeclFinalize(info, spec, decl, declaredParams, method);

	/* Update the scope with the required information */
	if(checkForward != 0)
	{
		/* Check that the re-declaration matches the forward declaration */
		if(!SameSignature(signature, checkForward, forwardKind))
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("conflicting types for `%s'"), name);
			if(prevNode)
			{
				CCErrorOnLine(yygetfilename(prevNode), yygetlinenum(prevNode),
							  _("previous declaration of `%s'"), name);
			}
			else
			{
				ReportInferError(info, node, name, checkForward);
			}
		}

		/* Update the scope data item with the actual information */
		CScopeUpdateFunction(data, C_SCDATA_FUNCTION, node, signature);
	}
	else
	{
		/* Add a new entry to the scope */
		CScopeAddFunction(name, node, signature);
	}

	/* Process the function attributes */
	if(CAttrPresent(decl.attrs, "constructor", "__constructor__"))
	{
		order = CAttrGetInt(decl.attrs, "corder", "__corder__", 1);
		AddInitOrFini(info, ILToProgramItem(method),
					  "InitializerAttribute", "InitializerOrderAttribute",
					  order);
		ILMemberSetAttrs((ILMember *)method,
					     IL_META_METHODDEF_SPECIAL_NAME,
					     IL_META_METHODDEF_SPECIAL_NAME);
	}
	if(CAttrPresent(decl.attrs, ".init", ".init"))
	{
		/* This is a "normal" initializer, generated by the compiler */
		AddInitOrFini(info, ILToProgramItem(method),
					  "InitializerAttribute", "InitializerOrderAttribute", 0);
		ILMemberSetAttrs((ILMember *)method,
					     IL_META_METHODDEF_SPECIAL_NAME,
					     IL_META_METHODDEF_SPECIAL_NAME);
	}
	if(CAttrPresent(decl.attrs, "destructor", "__destructor__"))
	{
		order = CAttrGetInt(decl.attrs, "dorder", "__dorder__", 1);
		AddInitOrFini(info, ILToProgramItem(method),
					  "FinalizerAttribute", "FinalizerOrderAttribute",
					  order);
		ILMemberSetAttrs((ILMember *)method,
					     IL_META_METHODDEF_SPECIAL_NAME,
					     IL_META_METHODDEF_SPECIAL_NAME);
	}
	if(ILMethod_HasSpecialName(method))
	{
		/* Check that the constructor/destructor signature is correct */
		if(ILTypeGetReturn(signature) != ILType_Void ||
		   (ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
		   		IL_META_CALLCONV_VARARG ||
		   ILTypeNumParams(signature) > 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  _("incorrect signature for constructor or destructor"));
		}
	}

	/* Clear the local variable signature, ready for allocation */
	localVarSig = 0;
	if(specials)
	{
		ILFree(specials);
		specials = 0;
		numSpecials = 0;
	}

	/* The method block is ready to go */
	return method;
}

void CFunctionDeclareParams(ILGenInfo *info, ILMethod *method)
{
	ILType *signature = ILMethod_Signature(method);
	ILParameter *param;
	const char *name;

	/* Scan the parameter information blocks for the method */
	param = 0;
	while((param = ILMethodNextParam(method, param)) != 0)
	{
		name = ILParameter_Name(param);
		if(name)
		{
			/* Declare this parameter into the scope with the given index */
			CScopeAddParam(name, (unsigned)(ILParameter_Num(param) - 1),
						   CTypeGetParam(signature, ILParameter_Num(param)));
		}
	}
}

/*
 * Dump the memory allocation block for special locals.
 */
static void DumpSpecialLocals(ILGenInfo *info)
{
	int spec;
	for(spec = 0; spec < numSpecials; ++spec)
	{
		CGenSizeOf(info, specials[spec].type);
		ILGenSimple(info, IL_OP_CONV_U);
		ILGenSimple(info, IL_OP_PREFIX + IL_PREFIX_OP_LOCALLOC);
		ILGenStoreLocal(info, specials[spec].index);
		ILGenAdjust(info, -1);
	}
}

void CFunctionOutput(ILGenInfo *info, ILMethod *method, ILNode *body,
					 int initGlobalVars)
{
	FILE *stream = info->asmOutput;
	unsigned indexVar = 0;
	unsigned exceptVar = 0;
	ILLabel label = ILLabel_Undefined;
	ILType *signature = ILMethod_Signature(method);
	ILMachineType returnMachineType;
	int index, outputLabel;
	int alreadyJumped;
	ILLabel switchEndLabel;

	/* Set up the code generation state for this function */
	numSetJmpRefs = 0;
	currSetJmpRef = 0;
	currMethod = method;

	/* Perform semantic analysis on the function body */
	info->currentScope = CCurrentScope;
	if(body != 0)
	{
		ILNode_CSemAnalysis(body, info, &body, 1);
	}
	info->currentScope = CGlobalScope;

	/* Bail out if we don't have an assembly stream or we had errors */
	if(!stream || CCHaveErrors)
	{
		return;
	}

	/* Mark the function's signature to be output at the end of
	   the compilation process */
	CTypeMarkForOutput(info, signature);

	/* Output the function header */
	fputs(".method ", stream);
	ILDumpFlags(stream, ILMethod_Attrs(method), ILMethodDefinitionFlags, 0);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, ILMethod_Name(method), method);
	putc(' ', stream);
	ILDumpFlags(stream, ILMethod_ImplAttrs(method),
	            ILMethodImplementationFlags, 0);
	fputs("\n{\n", stream);

	/* Output the attributes that are attached to the method */
	CGenOutputAttributes(info, stream, ILToProgramItem(method));

	/* Dump the local variable definitions that we have so far */
	ILGenDumpILLocals(info, localVarSig);

	/* Notify the code generator as to what the return type is */
	info->returnType = ILTypeGetReturn(signature);
	returnMachineType = ILTypeToMachineType(info->returnType);

	/* Force initialization of global variables if we were
	   called from C# code and ".init" hasn't been run yet */
	if(initGlobalVars)
	{
		fputs("\tldtoken 'init-on-demand'\n", stream);
		fputs("\tcall\tvoid [.library]System.Runtime.CompilerServices"
				".RuntimeHelpers::RunClassConstructor"
				"(valuetype [.library]System.RuntimeTypeHandle)\n", stream);
	}

	/* Allocate memory for the special locals that have complex layout */
	DumpSpecialLocals(info);

	/* Create the "setjmp" header if necessary */
	if(numSetJmpRefs > 0)
	{
		/* Get temporary variables to hold the values that we need:

		   marker - base of the marker range for this call frame.
		   index  - index of the marker which activated, or -1 on entry.
		   value  - the "longjmp" value that was thrown to us.
		   except - temporary storage for the "LongJmpException" object.
		*/
		markerVar = ILGenTempTypedVar(info, ILType_Int32);
		indexVar = ILGenTempTypedVar(info, ILType_Int32);
		valueVar = ILGenTempTypedVar(info, ILType_Int32);
		exceptVar = ILGenTempTypedVar
			(info, ILFindNonSystemType(info, "LongJmpException",
									   "OpenSystem.C"));

		/* Initialize the setjmp control variables */
		ILGenInt32(info, (ILInt32)numSetJmpRefs);
		fputs("\tcall\tint32 'OpenSystem.C'.'LongJmpException'"
					"::'GetMarkers'(int32)\n", stream);
		ILGenStoreLocal(info, markerVar);
		ILGenSimple(info, IL_OP_LDC_I4_M1);
		ILGenStoreLocal(info, indexVar);
		ILGenSimple(info, IL_OP_LDC_I4_0);
		ILGenStoreLocal(info, valueVar);
		ILGenExtend(info, 1);

		/* Output the setjmp restart label.  The exception handler
		   jumps here when a "longjmp" is detected with the index
		   set to indicate which "setjmp" should be re-activated */
		fputs("Lrestart:\n", stream);

		/* Wrap the rest of the function code in an exception block */
		fputs(".try {\n", stream);

		/* Generate code to check which "setjmp" in the function
		   triggered us to restart execution of the function */
		for(index = 0; index < numSetJmpRefs; ++index)
		{
			ILGenLoadLocal(info, indexVar);
			ILGenInt32(info, (ILInt32)index);
			fprintf(stream, "\tbeq\tLsetjmp%d\n", index);
		}
		ILGenExtend(info, 2);

		/* Tell the code generator to enter a "try" context so that
		   it will cause all "return" sequences to jump to the end
		   of the method before doing the actual return */
		ILGenPushTry(info);
	}

	/* Generate code for the function body */
	if(body != 0)
	{
		ILNode_GenDiscard(body, info);
	}

	/* Output the "goto *" case table if necessary */
	alreadyJumped = 0;
	if(info->gotoPtrLabel != ILLabel_Undefined)
	{
		/* Generate a fake return statement to force the function to jump
		   to the end of the method if control reaches here */
		if(!ILNodeEndsInFlowChange(body, info))
		{
			ILNode *node = ILNode_Return_create();
			ILNode_GenDiscard(node, info);
			alreadyJumped = 1;
		}

		/* Output the label corresponding to the "switch" table.
		   Every instance of "goto *" jumps to this position */
		ILGenLabel(info, &(info->gotoPtrLabel));

		/* Output the start of the switch table, with entry zero pointing
		   at the default location (zero == NULL, which is an invalid label) */
		switchEndLabel = ILLabel_Undefined;
		ILGenSwitchStart(info);
		ILGenSwitchRef(info, &switchEndLabel, 1);

		/* Output the labels for the switch cases */
		CGenGotoPtrLabels(info, &switchEndLabel);

		/* Output the end of the switch table.  We then throw an
		   "InvalidOperationException" if the label number is invalid */
		ILGenSwitchEnd(info);
		ILGenLabel(info, &switchEndLabel);
		ILGenNewObj(info, "[.library]System.InvalidOperationException", "()");
		ILGenSimple(info, IL_OP_THROW);
	}

	/* Create the "setjmp" footer if necessary */
	if(numSetJmpRefs > 0)
	{
		/* Generate a fake return statement to force the function to jump
		   to the end of the method if control reaches here */
		if(!ILNodeEndsInFlowChange(body, info) && !alreadyJumped)
		{
			ILNode *node = ILNode_Return_create();
			ILNode_GenDiscard(node, info);
		}

		/* Output the start of the catch block for "LongJmpException" */
		fputs("} catch 'OpenSystem.C'.'LongJmpException' {\n",
			  stream);

		/* Store the exception reference */
		ILGenExtend(info, 1);
		ILGenStoreLocal(info, exceptVar);

		/* Get the "Marker" value from the "LongJmpException" object,
		   and subtract our local "marker" reference from it */
		ILGenLoadLocal(info, exceptVar);
		fputs("\tcall\tinstance int32 'OpenSystem.C'."
					"'LongJmpException'::get_Marker()\n", stream);
		ILGenLoadLocal(info, markerVar);
		ILGenAdjust(info, 2);
		ILGenSimple(info, IL_OP_SUB);
		ILGenStoreLocal(info, indexVar);
		ILGenAdjust(info, -2);

		/* Determine if "index" is in range for this call frame */
		ILGenLoadLocal(info, indexVar);
		ILGenInt32(info, (ILInt32)numSetJmpRefs);
		ILGenJump(info, IL_OP_BGE_UN, &label);

		/* Get the "Value" from the "LongJmpException" object and then
		   jump back to the top of the function to restart it */
		ILGenLoadLocal(info, exceptVar);
		fputs("\tcall\tinstance int32 'OpenSystem.C'."
					"'LongJmpException'::get_Value()\n", stream);
		ILGenStoreLocal(info, valueVar);
		fputs("\tleave\tLrestart\n", stream);

		/* Re-throw the "LongJmpException" object because it isn't ours */
		ILGenLabel(info, &label);
		ILGenSimple(info, IL_OP_PREFIX + IL_PREFIX_OP_RETHROW);

		/* Output the end of the catch block */
		fputs("}\n", stream);

		/* Exit the try context */
		ILGenPopTry(info);
	}

	/* Add an explicit return instruction if the body didn't */
	outputLabel = 0;
	if(!ILNodeEndsInFlowChange(body,info))
	{
		if(info->returnLabel != ILLabel_Undefined &&
		   info->returnType == ILType_Void)
		{
			/* Use this point in the code for return labels
			   to prevent outputting two "ret"'s in a row */
			ILGenLabel(info, &(info->returnLabel));
			outputLabel = 1;
		}
		ILGenCast(info, ILMachineType_Void, returnMachineType);
		ILGenSimple(info, IL_OP_RET);
		if(info->returnType != ILType_Void)
		{
			ILGenAdjust(info, -1);
		}
	}

	/* If we have a return label, we need to output some final
	   code to return the contents of a temporary local variable.
	   This is used when returning from inside a "try" block */
	if(!outputLabel && info->returnLabel != ILLabel_Undefined)
	{
		ILGenLabel(info, &(info->returnLabel));
		if(info->returnType != ILType_Void)
		{
			ILGenLoadLocal(info, info->returnVar);
			ILGenExtend(info, 1);
		}
		ILGenSimple(info, IL_OP_RET);
	}

	/* Output the maximum stack height for the method */
	fprintf(stream, "\t.maxstack %ld\n", info->maxStackHeight);

	/* Output the function footer */
	fprintf(stream, "} // method %s\n", ILMethod_Name(method));

	/* Clean up temporary state that was used for code generation */
	ILGenEndMethod(info);
}

void CFunctionFlushInits(ILGenInfo *info, ILNode *list)
{
	CDeclSpec spec;
	CDeclarator decl;
	char buf[64];
	const char *name;
	ILMethod *method;
	ILNode *attributes;
	ILScope *scope;

	/* Set the return type, which is "void" */
	CDeclSpecSetType(spec, ILType_Void);
	spec.specifiers = C_SPEC_STATIC;

	/* Build the initializer name */
	sprintf(buf, ".init-%d", initNumber);
	++initNumber;
	name = ILInternString(buf, -1).string;

	/* Build the attribute list, containing just ".init" */
	attributes = ILNode_List_create();
	ILNode_List_Add(attributes, ILNode_CAttribute_create(".init", 0));

	/* Build a prototype for the initializer */
	CDeclSetName(decl, name, ILQualIdentSimple(name));
	decl = CDeclCreatePrototype(info, decl, 0, attributes);

	/* Create the method header for the initializer */
	method = CFunctionCreate
		(info, name, list, spec, decl, ILNode_List_create());

	/* Wrap the function body in a new scope */
	scope = ILScopeCreate(info, CGlobalScope);
	list = ILNode_NewScope_create(list);
	((ILNode_NewScope *)list)->scope = scope;

	/* Output the initializer */
	CFunctionOutput(info, method, list, 0);
}

void CFunctionPredeclare(ILGenInfo *info)
{
	ILType *signature;
	ILType *type;

	/* Bail out if the "-fno-builtin" option was supplied */
	if(CCStringListContainsInv
			(extension_flags, num_extension_flags, "builtin"))
	{
		return;
	}

	/* Build the signature "int (*)(const char *, ...)" */
	signature = ILTypeCreateMethod(info->context, ILType_Int32);
	if(!signature)
	{
		ILGenOutOfMemory(info);
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_VARARG);
	type = CTypeCreatePointer(info, CTypeAddConst(info, ILType_Int8));
	if(!ILTypeAddParam(info->context, signature, type))
	{
		ILGenOutOfMemory(info);
	}

	/* Declare the inferred definitions of "printf" and "scanf" */
	CScopeAddInferredFunction("printf", signature);
	CScopeAddInferredFunction("scanf", signature);
}

ILType *CFunctionNaturalType(ILGenInfo *info, ILType *type, int vararg)
{
	if(ILType_IsPrimitive(type))
	{
		/* We pass unsigned types as their signed counterparts in
		   vararg mode, to get around cast issues in "refanyval" */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:		return ILType_Int32;

			case IL_META_ELEMTYPE_U4:
			{
				if(vararg)
				{
					return ILType_Int32;
				}
				else
				{
					return ILType_UInt32;
				}
			}
			/* Not reached */

			case IL_META_ELEMTYPE_U8:
			{
				if(vararg)
				{
					return ILType_Int64;
				}
				else
				{
					return ILType_UInt64;
				}
			}
			/* Not reached */

			case IL_META_ELEMTYPE_I:
			{
				return ILType_Int;
			}
			/* Not reached */

			case IL_META_ELEMTYPE_U:
			{
				if(vararg)
				{
					return ILType_Int;
				}
				else
				{
					return ILType_UInt;
				}
			}
			/* Not reached */

			case IL_META_ELEMTYPE_R4:
			case IL_META_ELEMTYPE_R:		return ILType_Float64;
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR || ILType_IsMethod(type))
		{
			/* Pointers are passed as "long" in vararg mode to get
			   around cast issues in the "refanyval" instruction */
			if(vararg)
			{
				return ILType_Int;
			}
		}
		else if(ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_OPT ||
				ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_REQD)
		{
			return CFunctionNaturalType
				(info, type->un.modifier__.type__, vararg);
		}
	}
	else if(ILTypeIsEnum(type))
	{
		/* Enumerated values are passed as "int32" in vararg mode to
		   get around cast issues in the "refanyval" instruction */
		if(vararg)
		{
			return ILType_Int32;
		}
	}
	return type;
}

void CFunctionSawSetJmp(void)
{
	++numSetJmpRefs;
}

void CGenSetJmp(ILGenInfo *info)
{
	ILLabel label = ILLabel_Undefined;

	/* Get the marker value for this particular "setjmp" point */
	ILGenLoadLocal(info, markerVar);
	ILGenAdjust(info, 1);
	if(currSetJmpRef > 0)
	{
		ILGenInt32(info, (ILInt32)currSetJmpRef);
		ILGenSimple(info, IL_OP_ADD);
		ILGenExtend(info, 1);
	}

	/* Store the marker value into the "jmp_buf" array */
	ILGenSimple(info, IL_OP_STIND_I4);
	ILGenAdjust(info, -2);

	/* Push zero onto the stack and jump past the following code */
	ILGenSimple(info, IL_OP_LDC_I4_0);
	ILGenJump(info, IL_OP_BR, &label);

	/* Output the "setjmp" label.  The code at the top of the
	   function jumps here when a "longjmp" is detected */
	if(info->asmOutput)
	{
		fprintf(info->asmOutput, "Lsetjmp%d:\n", currSetJmpRef);
	}

	/* Load the "longjmp" value onto the stack */
	ILGenLoadLocal(info, valueVar);
	ILGenAdjust(info, 1);

	/* Mark the end of the "setjmp" handling code */
	ILGenLabel(info, &label);

	/* Advance the index for the next "setjmp" point in this function */
	++currSetJmpRef;
}

ILMethod *CFunctionGetCurrent(void)
{
	return currMethod;
}

unsigned CGenAllocLocal(ILGenInfo *info, ILType *type)
{
	unsigned num;
	CTypeLayoutInfo layout;
	CTypeMarkForOutput(info, type);
	if(!localVarSig)
	{
		localVarSig = ILTypeCreateLocalList(info->context);
		if(!localVarSig)
		{
			ILGenOutOfMemory(info);
		}
	}
	num = (unsigned)ILTypeNumLocals(localVarSig);
	CTypeGetLayoutInfo(type, &layout);
	if(layout.category != C_TYPECAT_COMPLEX)
	{
		/* We can allocate the local directly in the local variable frame */
		if(!ILTypeAddLocal(info->context, localVarSig, type))
		{
			ILGenOutOfMemory(info);
		}
	}
	else
	{
		/* We need to use "localloc" to allocate memory for the
		   complex type, and then store it as a pointer */
		if((specials = (SpecialLocal *)ILRealloc
				(specials, sizeof(SpecialLocal) * (numSpecials + 1))) == 0)
		{
			ILGenOutOfMemory(info);
		}
		specials[numSpecials].index = num;
		specials[numSpecials].type = type;
		++numSpecials;
		if(!ILTypeAddLocal(info->context, localVarSig, ILType_Int))
		{
			ILGenOutOfMemory(info);
		}
	}
	return num;
}

/*
 * Print the hex version of an attribute string.
 */
static void PrintHex(FILE *stream, unsigned char *buf, int len)
{
	while(len > 0)
	{
		fprintf(stream, "%02X ", (int)(*buf));
		++buf;
		--len;
	}
}
static void PrintAttributeString(FILE *stream, const char *str)
{
	unsigned char header[IL_META_COMPRESS_MAX_SIZE];
	int len;
	len = ILMetaCompressData(header, strlen(str));
	PrintHex(stream, header, len);
	PrintHex(stream, (unsigned char *)str, strlen(str));
}

void CFunctionWeakAlias(ILGenInfo *info, const char *name, ILNode *node,
						ILType *signature, const char *aliasFor,
						int isPrivate)
{
	unsigned long numParams;
	unsigned long paramNum;
	FILE *stream = info->asmOutput;
	if(!stream)
	{
		return;
	}

	/* Create a strong alias instead if the function involves varargs,
	   because it is impossible to set up the necessary call-through logic */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		CFunctionStrongAlias(info, name, node, signature, aliasFor, isPrivate);
		return;
	}

	/* Declare the "name-alias" field, which contains the method pointer */
	if(isPrivate)
		fputs(".field private static specialname ", stream);
	else
		fputs(".field public static specialname ", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, 0, 0);
	fprintf(stream, " '%s-alias'\n", name);

	/* Dump the method header */
	fputs(".method ", stream);
	if(isPrivate)
		fputs("private static ", stream);
	else
		fputs("public static ", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, name, 0);
	fputs(" cil managed\n{\n", stream);

	/* Dump the weak alias name attribute */
	fputs(".custom instance void OpenSystem.C.WeakAliasForAttribute::.ctor"
				"(class [.library]System.String) = (01 00 ", stream);
	PrintAttributeString(stream, aliasFor);
	fputs("00 00)\n", stream);

	/* Dump a dummy method body to redirect control to the
	   aliased function if this one is called */
	numParams = ILTypeNumParams(signature);
	for(paramNum = 0; paramNum < numParams; ++paramNum)
	{
		fprintf(stream, "\tldarg\t%lu\n", paramNum);
	}
	fputs("\tldsfld\t", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, 0, 0);
	fprintf(stream, " '%s-alias'\n", name);
	fputs("\ttail.\n", stream);
	fputs("\tcalli\t", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, "", 0);
	fputs("\n\tret\n", stream);
	fprintf(stream, "\t.maxstack %lu\n", numParams + 1);

	/* Dump the method footer */
	fputs("}\n", stream);

	/* Dump the method header for the alias initializer */
	fprintf(stream, ".method private static specialname void '.init-%s'",
			name);
	fputs("() cil managed\n{\n", stream);
	fputs(".custom instance void OpenSystem.C.InitializerAttribute::.ctor()"
				" = (01 00 00 00)\n", stream);

	/* Initialize the "name-alias" variable */
	fputs("\tldftn\t", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, aliasFor, 0);
	fputs("\n\tstsfld\t", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, 0, 0);
	fprintf(stream, " '%s-alias'\n", name);
	fputs("\tret\n\t.maxstack 1\n", stream);

	/* Dump the method footer for the alias initializer */
	fputs("}\n", stream);
}

void CFunctionStrongAlias(ILGenInfo *info, const char *name, ILNode *node,
						  ILType *signature, const char *aliasFor,
						  int isPrivate)
{
	unsigned long numParams;
	unsigned long paramNum;
	FILE *stream = info->asmOutput;
	if(!stream)
	{
		return;
	}

	/* Dump the method header */
	fputs(".method ", stream);
	if(isPrivate)
		fputs("private static ", stream);
	else
		fputs("public static ", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, name, 0);
	fputs(" cil managed\n{\n", stream);

	/* Dump the strong alias name attribute */
	fputs(".custom instance void OpenSystem.C.StrongAliasForAttribute::.ctor"
				"(class [.library]System.String) = (01 00 ", stream);
	PrintAttributeString(stream, aliasFor);
	fputs("00 00)\n", stream);

	/* If the alias involves varargs, then there is no way to call
	   through to the underlying function using IL instructions.
	   But we still need to output a method body, to keep Microsoft's
	   CLR happy: it won't load an assembly that contains a method
	   with an empty body but no PInvoke declaration.  Even if that
	   method is never called.  So we throw an exception instead */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		fputs("\tnewobj\tinstance void [.library]System.NotSupportedException"
					"::.ctor()\n", stream);
		fputs("\tthrow\n", stream);
		fputs("\t.maxstack 1\n", stream);
		fputs("}\n", stream);
		return;
	}

	/* Dump a dummy method body to redirect control to the
	   aliased function if this one is called */
	numParams = ILTypeNumParams(signature);
	for(paramNum = 0; paramNum < numParams; ++paramNum)
	{
		fprintf(stream, "\tldarg\t%lu\n", paramNum);
	}
	fputs("\ttail.\n", stream);
	fputs("\tcall\t", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, aliasFor, 0);
	fputs("\n\tret\n", stream);
	if(ILTypeGetReturn(signature) != ILType_Void && numParams == 0)
	{
		fputs("\t.maxstack 1\n", stream);
	}
	else
	{
		fprintf(stream, "\t.maxstack %lu\n", numParams);
	}

	/* Dump the method footer */
	fputs("}\n", stream);
}

void CFunctionPInvoke(ILGenInfo *info, const char *name, ILNode *node,
					  ILType *signature, const char *moduleName,
					  const char *aliasName, ILUInt32 flags, int isPrivate)
{
	FILE *stream = info->asmOutput;
	if(!stream)
	{
		return;
	}
	CTypeMarkForOutput(info, signature);
	fputs(".method ", stream);
	if(isPrivate)
		fputs("private static pinvokeimpl(", stream);
	else
		fputs("public static pinvokeimpl(", stream);
	ILDumpString(stream, moduleName);
	if(aliasName)
	{
		fputs(" as ", stream);
		ILDumpString(stream, aliasName);
	}
	putc(' ', stream);
	ILDumpFlags(stream, flags, ILPInvokeImplementationFlags, 0);
	fputs(") ", stream);
	ILDumpMethodType(stream, info->image, signature,
					 IL_DUMP_QUOTE_NAMES, 0, name, 0);
	fputs(" cil managed {}\n", stream);
}

int CAttrPresent(ILNode *attrs, const char *name, const char *altName)
{
	ILNode_ListIter iter;
	ILNode_CAttribute *arg;
	ILNode_ListIter_Init(&iter, attrs);
	while((arg = (ILNode_CAttribute *)ILNode_ListIter_Next(&iter)) != 0)
	{
		if((!strcmp(arg->name, name) || !strcmp(arg->name, altName)) &&
		   arg->args == 0)
		{
			return 1;
		}
	}
	return 0;
}

const char *CAttrGetString(ILNode *attrs, const char *name,
						   const char *altName)
{
	ILNode_ListIter iter;
	ILNode_CAttribute *arg;
	ILEvalValue *value;
	ILNode_ListIter_Init(&iter, attrs);
	while((arg = (ILNode_CAttribute *)ILNode_ListIter_Next(&iter)) != 0)
	{
		if((!strcmp(arg->name, name) || !strcmp(arg->name, altName)) &&
		   yyisa(arg->args, ILNode_CAttributeValue))
		{
			value = &(((ILNode_CAttributeValue *)(arg->args))->value);
			if(value->valueType == ILMachineType_String)
			{
				return value->un.strValue.str;
			}
		}
	}
	return 0;
}

ILInt32 CAttrGetInt(ILNode *attrs, const char *name,
					const char *altName, ILInt32 defValue)
{
	ILNode_ListIter iter;
	ILNode_CAttribute *arg;
	ILEvalValue *value;
	ILNode_ListIter_Init(&iter, attrs);
	while((arg = (ILNode_CAttribute *)ILNode_ListIter_Next(&iter)) != 0)
	{
		if((!strcmp(arg->name, name) || !strcmp(arg->name, altName)) &&
		   yyisa(arg->args, ILNode_CAttributeValue))
		{
			value = &(((ILNode_CAttributeValue *)(arg->args))->value);
			if(value->valueType == ILMachineType_Int8 ||
			   value->valueType == ILMachineType_UInt8 ||
			   value->valueType == ILMachineType_Int16 ||
			   value->valueType == ILMachineType_UInt16 ||
			   value->valueType == ILMachineType_Int32 ||
			   value->valueType == ILMachineType_UInt32)
			{
				return value->un.i4Value;
			}
		}
	}
	return defValue;
}

#ifdef	__cplusplus
};
#endif
