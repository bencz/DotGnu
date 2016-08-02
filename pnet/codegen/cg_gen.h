/*
 * cg_gen.h - Definition of the "ILGenInfo" structure and helper routines.
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

#ifndef	_CODEGEN_CG_GEN_H
#define	_CODEGEN_CG_GEN_H

#include "il_varargs.h"
#include "il_profile.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Loop stack entry.  If "continueLabel" is NULL, and "breakLabel"
 * is not NULL, then the entry is a "switch".  If both are NULL,
 * then it indicates the position of a "try" block on the stack.
 */
typedef struct
{
	const char	   *name;
	ILLabel		   *continueLabel;
	ILLabel		   *breakLabel;
	ILLabel		   *finallyLabel;
	int				isForeachCollection;

} ILLoopStack;

/*
 * Goto list entry.
 */
typedef struct _tagILGotoEntry
{
	const char	   *name;
	int				defined : 1;
	int				crossedTry : 1;
	long			loopStackSize;
	long			scopeLevel;
	ILLabel			label;
	struct _tagILGotoEntry *next;

} ILGotoEntry;

/*
 * Information about allocated local variable slots.
 */
typedef struct _tagILLocalVar
{
	const char	   *name;
	long			scopeLevel;
	ILType		   *type;
	short			allocated;

} ILLocalVar;

/*
 * Information about static array initialization classes.
 */
typedef struct _tagILArrayInit
{
	ILLabel			label;
	ILUInt32		size;
	struct _tagILArrayInit *next;

} ILArrayInit;

/*
 * Opaque definition of the Java generator routines.
 */
typedef struct _tagILJavaGenInfo ILJavaGenInfo;

/*
 * Access/Inheritance check function.
 */
typedef int (*ILClassAccessCheck)(ILClass *info, ILClass *scope);

/*
 * Error/Warning handling function
 */
typedef void (*ILErrorFunc)(const char *filename, unsigned long linenum,
							const char *format, IL_VA_LIST va);

/*
 * Structure of the code generation context.
 */
struct _tagILGenInfo
{
	const char	   *progname;			/* Name of the program */
	FILE		   *asmOutput;			/* Assembly output stream */
	ILContext	   *context;			/* Context that is being built */
	ILImage		   *image;				/* Image that is being built */
	ILImage		   *libImage;			/* Image that holds library defns */
	ILMemPool		nodePool;			/* Memory pool for the nodes */
	ILMemPool		scopePool;			/* Memory pool for scopes */
	ILMemPool		scopeDataPool;		/* Memory pool for scope data items */
	ILLabel			nextLabel;			/* Next temporary label to allocate */
	int				overflowInsns : 1;	/* Use overflow instructions */
	int				overflowGlobal : 1;	/* Global version of "overflowInsns" */
	int				overflowChanged : 1;/* Overflow value changed */
	int				pedanticArith : 1;	/* Make arithmetic always accurate */
	int				clsCompliant : 1;	/* Use strict CLS library only */
	int				semAnalysis : 1;	/* Non-zero during semantic analysis */
	int				typeGather : 1;		/* Non-zero during type gathering */
	int				inSemType : 1;		/* Semantic analysis on a type */
	int				inSemStatic : 1;	/* Semantic analysis on a static member */
	int				inAttrArg : 1;		/* Non-zero inside an attribute arg */
	int				useJavaLib : 1;		/* Use Java and not C# library */
	int				outputIsJava : 1;	/* Output Java bytecode */
	int				debugFlag : 1;		/* Non-zero if debug is enabled */
	int				hasUnsafe : 1;		/* Non-zero if unsafe code in source */
	int				needSwitchPop : 1;	/* Non-zero to pop a switch value */
	int				hasGotoScopes : 1;	/* Non-zero if goto scopes used */
	int				resolvingAlias : 1;	/* Non-zero if resolving an alias */
	int				inFixed : 1;		/* Non-Zero if in a fixed statement */
	int             decimalRoundMode;	/* Rounding mode for ILDecimal */
	long			stackHeight;		/* Current stack height */
	long			maxStackHeight;		/* Maximum stack height */
	ILLoopStack    *loopStack;			/* Contents of the loop stack */
	long			loopStackSize;		/* Size of the loop stack */
	long			loopStackMax;		/* Maximum size of the loop stack */
	ILType		   *returnType;			/* Return type for current method */
	long			returnVar;			/* Temp local for return value */
	ILLabel			returnLabel;		/* End of method return code label */
	long			throwVariable;		/* Variable containing thrown value */
	ILGotoEntry    *gotoList;			/* List of goto entries */
	long			scopeLevel;			/* Current scope level */
	ILLocalVar	   *tempVars;			/* Temporary variables in the method */
	unsigned		numTempVars;		/* Number of active variables */
	unsigned		maxTempVars;		/* Maximum variables in "localVars" */
	unsigned        tempLocalBase;		/* Base for temporary local variables */
	int				createLocalsScope;	/* Non-zero to create scope for local variables */
	ILScope		   *currentScope;		/* Current scope for declarations */
	ILScope		   *globalScope;		/* The global scope of the image built. */
	ILJavaGenInfo  *javaInfo;			/* Java-specific information */
	long			unsafeLevel;		/* Number of unsafe contexts */
	int			   *contextStack;		/* Statement context stack */
	long			contextStackSize;	/* Size of statement context stack */
	long			contextStackMax;	/* Max size of context stack */
	int				optimizeFlag ;		/* values of -O0, -O1, -O2, -O3 */
	ILNode         *currentClass;		/* Current class being processed */
	ILNode		   *currentMethod;		/* Current method being processed */
	ILNode         *currentNamespace;	/* Current namespace being processed */
	ILNode         *currentSwitch;		/* Current switch being processed */
	ILArrayInit    *arrayInit;			/* Array initialization information */
	ILHashTable    *itemHash;			/* Hash program items to nodes */
	ILVarUsageTable *varUsage;			/* Variable usage table */
#if IL_VERSION_MAJOR > 1
	ILNode		   *currentTypeFormals;	/* Current generic type formals */
	ILNode		   *currentMethodFormals; /* Current generic method formals */
#endif	/* IL_VERSION_MAJOR > 1 */
	ILLabel			gotoPtrLabel;		/* Label for "goto *" operations */
	ILClassAccessCheck accessCheck;		/* Function for checking access permissions. */
	ILErrorFunc		errFunc;			/* Function to print and handle errors */
	ILErrorFunc		warnFunc;			/* Function to print and handle warnings */
};

/*
 * Adjust the height of the operand stack.
 */
#define	ILGenAdjust(info,amount)	\
			do { \
				(info)->stackHeight += (amount); \
				if((info)->stackHeight > (info)->maxStackHeight) \
				{ \
					(info)->maxStackHeight = (info)->stackHeight; \
				} \
			} while (0)

/*
 * Extend the height of the operand stack to account for temporary values.
 */
#define	ILGenExtend(info,amount)	\
			do { \
				if(((info)->stackHeight + (amount)) > (info)->maxStackHeight) \
				{ \
					(info)->maxStackHeight = (info)->stackHeight + (amount); \
				} \
			} while (0)

/*
 * Initialize an ILGenInfo structure.
 */
void ILGenInfoInit(ILGenInfo *info, char *progname,
				   const char *assemName,
				   FILE *asmOutput, int useBuiltinLibrary);

/*
 * Switch an ILGenInfo structure to Java bytecode generation.
 */
void ILGenInfoToJava(ILGenInfo *info);

/*
 * Destroy an ILGenInfo structure.
 */
void ILGenInfoDestroy(ILGenInfo *info);

/*
 * Report an out of memory error and abort the compiler.
 */
void ILGenOutOfMemory(ILGenInfo *info);

/*
 * Find a type descriptor for something in the "System" namespace.
 */
ILType *ILFindSystemType(ILGenInfo *info, const char *name);

/*
 * Find a type descriptor for something in a non-"System" namespace.
 */
ILType *ILFindNonSystemType(ILGenInfo *info, const char *name,
							const char *namespace);

/*
 * Convert a type into its class form.  Returns NULL if
 * there is no class form for the type.
 */
ILClass *ILTypeToClass(ILGenInfo *info, ILType *type);

/*
 * Convert a type into its program item form.
 * Returns either a class or typespec or NULL if
 * there is no class or typespec form for the type.
 */
ILProgramItem *ILTypeToProgramItem(ILGenInfo *info, ILType *type);

/*
 * Convert a type into a machine type.
 */
ILMachineType ILTypeToMachineType(ILType *type);

/*
 * Convert a runtime value type into a language type.
 */
ILType *ILValueTypeToType(ILGenInfo *info, ILMachineType valueType);

/*
 * Allocate a temporary local variable of a particular primitive type.
 */
unsigned ILGenTempVar(ILGenInfo *info, ILMachineType type);

/*
 * Allocate a temporary local variable of a particular real type.
 */
unsigned ILGenTempTypedVar(ILGenInfo *info, ILType *type);

/*
 * Release a temporary local variable that is no longer required.
 */
void ILGenReleaseTempVar(ILGenInfo *info, unsigned localNum);

/*
 * Determine the common type to use for a binary operator.
 */
ILMachineType ILCommonType(ILGenInfo *info, ILMachineType type1,
						   ILMachineType type2, int intonly);

/*
 * Make the system library in "info->libImage".
 */
void ILGenMakeLibrary(ILGenInfo *info);

/*
 * Determine if a program item has a particular "System"
 * attribute attached to it.
 */
int ILGenItemHasAttribute(ILProgramItem *item, const char *name);

/*
 * Add a particular "System" attribute to program item.
 */
void ILGenItemAddAttribute(ILGenInfo *info, ILProgramItem *item,
						   const char *name);

/*
 * Get the number of "usable" parameters for a method signature.
 */
int ILGenNumUsableParams(ILType *signature);

/*
 * Get the parameter information associated with a specific
 * method parameter.  "signature" can be NULL, or a cached
 * copy of the method's signature.  "method" can be NULL if
 * the signature does not correspond to a known method.
 */
ILParameterModifier ILGenGetParamInfo(ILMethod *method, ILType *signature,
									  ILUInt32 num, ILType **type);

/*
 * Determine if "classInfo" fully implements all of its interfaces.
 * The node is used for error reporting.  Returns zero if there
 * were errors.
 */
typedef void (*ILGenInterfaceErrorFunc)(ILNode *node, ILClass *classInfo,
										ILMember *missingMember);
typedef void (*ILGenInterfaceProxyFunc)(ILNode *node, ILClass *classInfo,
										ILMethod *missingMember,
										ILMethod *proxyReplacement);
int ILGenImplementsAllInterfaces(ILGenInfo *info, ILNode *node,
							     ILClass *classInfo,
								 ILGenInterfaceErrorFunc error,
								 ILGenInterfaceProxyFunc proxy);

/*
 * Perform cleanup processing at the end of code generation
 * for a method.
 */
void ILGenEndMethod(ILGenInfo *info);

/*
 * Push a "try" context.
 */
void ILGenPushTry(ILGenInfo *info);

/*
 * Pop a "try" context.
 */
void ILGenPopTry(ILGenInfo *info);

/*
 * Create a basic image structure with an initial module,
 * assembly, and "<Module>" type.
 */
ILImage *ILGenCreateBasicImage(ILContext *context, const char *assemName);

/*
 * Dump the local variable signature for a method.
 */
void ILGenDumpILLocals(ILGenInfo *info, ILType *localVarSig);

/*
 * Output the attributes on a program item.
 */
void ILGenOutputAttributes(ILGenInfo *info, FILE *stream, ILProgramItem *item);

/*
 * Output the attributes attached to generic parameters of a program item.
 */
void ILGenOutputGenericParamAttributes(ILGenInfo *info, FILE *stream,
									   ILProgramItem *item);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_GEN_H */
