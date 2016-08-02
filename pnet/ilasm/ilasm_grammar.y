%{
/*
 * ilasm_grammar.y - Input file for yacc that defines the syntax of
 *                   the ILASM language.
 *
 * Copyright (C) 2001, 2002, 2008, 2009  Southern Storm Software, Pty Ltd.
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

/* Rename the lex/yacc symbols to support multiple parsers */
#include "ilasm_rename.h"

#define YYDEBUG 1

#include "il_config.h"
#include <stdio.h>
#include "il_system.h"
#include "il_opcodes.h"
#include "il_meta.h"
#include "il_utils.h"
#include "ilasm_build.h"
#include "ilasm_output.h"
#include "ilasm_data.h"
#ifdef HAVE_STDARG_H
	#include <stdarg.h>
#else
	#ifdef HAVE_VARARGS_H
		#include <varargs.h>
	#endif
#endif

#define	YYERROR_VERBOSE

/*
 * An ugly hack to work around missing "-lfl" libraries on MacOSX.
 */
#if defined(__APPLE_CC__) && !defined(YYTEXT_POINTER)
	#define	YYTEXT_POINTER 1
#endif

extern char *ILAsmFilename;
extern long  ILAsmLineNum;
extern int   ILAsmErrors;
extern int   ILAsmParseHexBytes;
extern int   ILAsmParseJava;
extern int yylex(void);
#ifdef YYTEXT_POINTER
extern char *ilasm_text;
#else
extern char ilasm_text[];
#endif

static void
yyerror(char *msg)
{
	if(ILAsmFilename && ILAsmLineNum >= 0)
	{
		fprintf(stderr, "%s:%ld: ", ILAsmFilename, ILAsmLineNum);
	}
	else if(ILAsmFilename)
	{
		fprintf(stderr, "%s: ", ILAsmFilename);
	}
	else if(ILAsmLineNum >= 0)
	{
		fprintf(stderr, "%ld: ", ILAsmLineNum);
	}
	if(!strcmp(msg, "parse error") || !strcmp(msg, "syntax error"))
	{
		/* This message is useless, so print something based on ilasm_text */
		fprintf(stderr, "parse error at or near `%s'\n", ilasm_text);
	}
	else
	{
		fprintf(stderr, msg);
		putc('\n', stderr);
	}
	ILAsmErrors = 1;
}

/*
 * Message printing function that is used elsewhere.
 */
void ILAsmPrintMessage(const char *filename, long linenum,
					   const char *format, ...)
{
	va_list va;
	if(filename)
	{
		fputs(filename, stderr);
		if(linenum >= 0)
		{
			fprintf(stderr, ":%ld: ", linenum);
		}
		else
		{
			fputs(": ", stderr);
		}
	}
	else if(linenum >= 0)
	{
		fprintf(stderr, "%ld: ", linenum);
	}
	va_start(va, format);
	vfprintf(stderr, format, va);
	va_end(va);
	putc('\n', stderr);
}

/*
 * Report an out of memory error and abort.
 */
void ILAsmOutOfMemory(void)
{
	fprintf(stderr, "ilasm: virtual memory exhausted\n");
	exit(1);
}

/*
 * Make a simple native specification.
 */
static ILIntString SimpleNative(int type)
{
	char str[1];
	str[0] = (char)type;
	return ILInternString(str, 1);
}

/*
 * Convert a string into its packed representation.
 */
static ILIntString PackString(ILIntString str)
{
	unsigned char prefix[IL_META_COMPRESS_MAX_SIZE];
	ILIntString prefixBlock;
	prefixBlock.string = (char *)prefix;
	prefixBlock.len = ILMetaCompressData(prefix, (ILUInt32)(str.len));
	if(str.len != 0)
	{
		return ILInternAppendedString(prefixBlock, str);
	}
	else
	{
		return ILInternString(prefixBlock.string, prefixBlock.len);
	}
}

/*
 * Convert a length value into its packed representation.
 */
static ILIntString PackLength(ILInt64 len)
{
	unsigned char prefix[IL_META_COMPRESS_MAX_SIZE];
	int prefixLen = ILMetaCompressData(prefix, (ILUInt32)len);
	return ILInternString((char *)prefix, prefixLen);
}

/*
 * Pack a string into its Unicode representation.
 */
static ILIntString PackUnicodeString(ILIntString str)
{
	int posn = 0;
	unsigned char buf[256];
	int outposn = 0;
	ILIntString result = ILInternString("", 0);
	while(posn < str.len)
	{
		outposn += ILUTF16WriteCharAsBytes
			(buf + outposn, ILUTF8ReadChar(str.string, str.len, &posn));
		if(outposn >= 240)
		{
			if(result.len > 0)
			{
				result = ILInternAppendedString
							(result, ILInternString((char *)buf, outposn));
			}
			else
			{
				result = ILInternString((char *)buf, outposn);
			}
			outposn = 0;
		}
	}
	if(outposn > 0)
	{
		if(result.len > 0)
		{
			return ILInternAppendedString(result,
									  	  ILInternString((char *)buf, outposn));
		}
		else
		{
			return ILInternString((char *)buf, outposn);
		}
	}
	else
	{
		return result;
	}
}

#ifdef IL_CONFIG_FP_SUPPORTED
/*
 * Helper function that sets 32-bit version of a "float".
 */
static void SetFloat(unsigned char *fbytes, ILFloat value)
{
	IL_WRITE_FLOAT(fbytes, value);
}

/*
 * Helper function that sets 64-bit version of a "float".
 */
static void SetDouble(unsigned char *dbytes, ILDouble value)
{
	IL_WRITE_DOUBLE(dbytes, value);
}
#endif	/* IL_CONFIG_FP_SUPPORTED */

/*
 * Find a module reference, or create it if not found.
 */
static ILModule *FindModuleRef(ILImage *image, const char *name)
{
	ILModule *module;
	if(!name)
	{
		name = "";
	}
	module = ILModuleRefCreateUnique(image, name);
	if(!module)
	{
		ILAsmOutOfMemory();
	}
	return module;
}

/*
 * Get the number of params.
 */
static int GetNumParams(ILAsmParamInfo *params)
{
	ILUInt32 numParams = 0;

	while(params != 0)
	{
		++numParams;
		params = params->next;
	}

	return numParams;
}

/*
 * Create a method signature type.
 */
static ILType *CreateMethodSig(ILInt64 callingConventions,
							   ILType *returnType,
							   ILAsmParamInfo *params,
							   int numGenericParams,
							   int freeParams)
{
	ILType *type;
	ILAsmParamInfo *nextParam;

	/* Set the "generic" flag if there are generic parameters */
	if(numGenericParams > 0)
	{
		callingConventions |= IL_META_CALLCONV_GENERIC;
	}

	/* Create the main method type block */
	type = ILTypeCreateMethod(ILAsmContext, returnType);
	if(!type)
	{
		ILAsmOutOfMemory();
	}
	ILTypeSetCallConv(type, (ILUInt32)callingConventions);
	ILType_SetNumGen(type, numGenericParams);

	/* Add the parameters to the method signature */
	while(params != 0)
	{
		nextParam = params->next;
		if(params->type)
		{
			if(!ILTypeAddParam(ILAsmContext, type, params->type))
			{
				ILAsmOutOfMemory();
			}
		}
		else
		{
			if(!ILTypeAddSentinel(ILAsmContext, type))
			{
				ILAsmOutOfMemory();
			}
		}
		if(freeParams)
		{
			ILFree(params);
		}
		params = nextParam;
	}

	/* Return the final method type to the caller */
	return type;
}

/*
 * Create a method specification token.
 */
static ILToken CreateMethodSpec(ILToken method, ILAsmParamInfo *genericParams)
{
	ILMember *member;
	ILType *signature;
	ILMethodSpec *spec;
	
	/* Convert the method token back into a MethodDef or MemberRef */
	member = ILMember_FromToken(ILAsmImage, method);
	if(!member)
	{
		return method;
	}

	/* Create the method instantiation signature */
	signature = CreateMethodSig(IL_META_CALLCONV_INSTANTIATION,
								ILType_Invalid, genericParams, 0, 1);

	/* Create the MethodSpec token */
	spec = ILMethodSpecCreate(ILAsmImage, 0, member, signature);
	if(!spec)
	{
		ILAsmOutOfMemory();
	}
	return ILMethodSpec_Token(spec);
}

/*
 * Add parameter definition information to a method signature.
 */
static void AddMethodParams(ILMethod *method, ILUInt32 parameterAttrs,
							ILIntString nativeType, ILAsmParamInfo *params)
{
	ILParameter *param;
	ILFieldMarshal *marshal;
	ILAsmParamInfo *nextParam;
	unsigned int paramNum;
	char nameBuf[64];

	/* Create a parameter definition for the return value */
	if(parameterAttrs != 0 || nativeType.len > 0)
	{
		param = ILParameterCreate(method, 0, "retval",
								  parameterAttrs, 0);
		if(!param)
		{
			ILAsmOutOfMemory();
		}
		if(nativeType.len > 0)
		{
			marshal = ILFieldMarshalCreate(ILAsmImage, 0,
										   ILToProgramItem(param));
			if(!marshal)
			{
				ILAsmOutOfMemory();
			}
			if(!ILFieldMarshalSetType(marshal, nativeType.string,
									  nativeType.len))
			{
				ILAsmOutOfMemory();
			}
		}
	}

	/* Add the parameters to the method signature */
	paramNum = 1;
	while(params != 0)
	{
		nextParam = params->next;
		if(params->type)
		{
			if(params->name)
			{
				param = ILParameterCreate(method, 0, params->name,
							  (ILUInt32)(params->parameterAttrs), paramNum);
			}
			else
			{
				sprintf(nameBuf, "A_%u", paramNum);
				param = ILParameterCreate(method, 0, nameBuf,
							  (ILUInt32)(params->parameterAttrs), paramNum);
			}
			if(!param)
			{
				ILAsmOutOfMemory();
			}
			if(params->nativeType.len > 0)
			{
				marshal = ILFieldMarshalCreate(ILAsmImage, 0,
											   ILToProgramItem(param));
				if(!marshal)
				{
					ILAsmOutOfMemory();
				}
				if(!ILFieldMarshalSetType(marshal, params->nativeType.string,
										  params->nativeType.len))
				{
					ILAsmOutOfMemory();
				}
			}
			++paramNum;
		}
		ILFree(params);
		params = nextParam;
	}
}

/*
 * Create a property signature type.
 */
static ILType *CreatePropertySig(ILInt64 callingConventions,
							     ILType *returnType,
							     ILAsmParamInfo *params)
{
	ILType *type;
	ILAsmParamInfo *nextParam;

	/* Create the main property type block */
	type = ILTypeCreateProperty(ILAsmContext, returnType);
	if(!type)
	{
		ILAsmOutOfMemory();
	}
	ILTypeSetCallConv(type, (ILUInt32)callingConventions);

	/* Add the parameters to the property signature */
	while(params != 0)
	{
		nextParam = params->next;
		if(params->type)
		{
			if(!ILTypeAddParam(ILAsmContext, type, params->type))
			{
				ILAsmOutOfMemory();
			}
		}
		else
		{
			if(!ILTypeAddSentinel(ILAsmContext, type))
			{
				ILAsmOutOfMemory();
			}
		}
		ILFree(params);
		params = nextParam;
	}

	/* Return the final property type to the caller */
	return type;
}

/*
 * Helper macros for setting the attributes of non-terminals.
 */
#define	SET_FIELD(name)		yyval.fieldAttrs.flags = IL_META_FIELDDEF_##name; \
							yyval.fieldAttrs.nativeType.string = 0; \
							yyval.fieldAttrs.nativeType.len = 0; \
							yyval.fieldAttrs.pinvokeAttrs = 0; \
							yyval.fieldAttrs.name1 = 0; \
							yyval.fieldAttrs.name2 = 0
#define	SET_METHOD(name)	yyval.methodAttrs.flags = \
								IL_META_METHODDEF_##name; \
							yyval.methodAttrs.pinvokeAttrs = 0; \
							yyval.methodAttrs.name1 = 0; \
							yyval.methodAttrs.name2 = 0

/*
 * Buffer that is used during byte list parsing.
 */
#define	ILASM_BYTE_BUFSIZ	256
static char byteBuffer[ILASM_BYTE_BUFSIZ];

/*
 * Get the token associated with a program item.
 */
#define	GetToken(item)		(ILProgramItemGetToken(ILToProgramItem((item))))

/*
 * Combine an array shell with an element type.
 */
static ILType *CombineArrayType(ILType *elemType, ILType *shell, int cont)
{
	ILType *temp;
	ILType *temp2;

	if(!elemType || !shell)
	{
		ILAsmOutOfMemory();
	}

	/* If the element type is an array, we need to insert the shell
	   at the inner-most level of the element type */
	if(ILType_IsArray(elemType))
	{
		temp = elemType;
		while(ILType_ElemType(temp) != 0 &&
		      ILType_IsComplex(ILType_ElemType(temp)) &&
			  (ILType_Kind(ILType_ElemType(temp)) == IL_TYPE_COMPLEX_ARRAY ||
			   ILType_Kind(ILType_ElemType(temp))
			   		== IL_TYPE_COMPLEX_ARRAY_CONTINUE))
		{
			temp = ILType_ElemType(temp);
		}
		temp2 = ILType_ElemType(temp);
		ILType_ElemType(temp) = shell;
		if(cont)
		{
			temp->kind__ = IL_TYPE_COMPLEX_ARRAY_CONTINUE;
		}
		temp = shell;
		while(ILType_Kind(temp) == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
		{
			temp = ILType_ElemType(temp);
		}
		ILType_ElemType(temp) = temp2;
		return elemType;
	}

	/* Wrap the shell around the element type */
	temp = shell;
	while(ILType_ElemType(temp) != 0 &&
	      ILType_IsComplex(ILType_ElemType(temp)) &&
		  (ILType_Kind(ILType_ElemType(temp)) == IL_TYPE_COMPLEX_ARRAY ||
		   ILType_Kind(ILType_ElemType(temp))
			   		== IL_TYPE_COMPLEX_ARRAY_CONTINUE))
	{
		temp = ILType_ElemType(temp);
	}
	ILType_ElemType(temp) = elemType;
	return shell;
}

/*
 * Set the originator for the current assembly definition or reference.
 */
static void SetOriginator(const char *orig, int len, int fullOriginator)
{
	if(ILAsmCurrAssemblyRef)
	{
		if(!ILAssemblySetOriginator(ILAsmCurrAssemblyRef,
									(const void *)orig,
									(ILUInt32)len))
		{
			ILAsmOutOfMemory();
		}
		if(fullOriginator)
		{
			ILAssemblySetRefAttrs(ILAsmCurrAssemblyRef,
								  IL_META_ASSEMREF_FULL_ORIGINATOR,
								  IL_META_ASSEMREF_FULL_ORIGINATOR);
		}
	}
	else
	{
		if(!ILAssemblySetOriginator(ILAsmAssembly,
									(const void *)orig,
									(ILUInt32)len))
		{
			ILAsmOutOfMemory();
		}
		ILAssemblySetAttrs(ILAsmAssembly,
						   IL_META_ASSEM_PUBLIC_KEY,
						   IL_META_ASSEM_PUBLIC_KEY);
	}
}


typedef struct _tagFieldDataEntry
{
	const char *name;
	ILField *field;
	char *filename;
	long linenum;
	struct _tagFieldDataEntry * next;  
} FieldDataEntry;

typedef struct _tagFieldDataList
{
	FieldDataEntry * first;
	FieldDataEntry * last;
} FieldDataList;

static FieldDataList *unresolvedFieldList = 0;

static void RegisterFieldRvaLabel(const char * name, ILField * field)
{
	FieldDataEntry *entry;
	
	if(!unresolvedFieldList)
	{
		unresolvedFieldList = (FieldDataList*) ILMalloc(sizeof(FieldDataList)); 
		if(!unresolvedFieldList)
		{
			ILAsmOutOfMemory();
		}
		unresolvedFieldList->first = 0;
		unresolvedFieldList->last = 0;
	}

	entry = (FieldDataEntry*) ILMalloc(sizeof(FieldDataEntry));

	if(!entry)
	{
		ILAsmOutOfMemory();
	}

	entry->name = name;
	entry->field = field;
	entry->filename = ILAsmFilename;
	entry->linenum = ILAsmLineNum;
	entry->next = NULL;


	if(!unresolvedFieldList->first)
	{
		unresolvedFieldList->first = entry;
		unresolvedFieldList->last = entry;
	}
	else
	{
		unresolvedFieldList->last->next = entry;
		unresolvedFieldList->last = entry;
	}
}

static void FinishDataLabels()
{
	FieldDataEntry * tmp;
	if(unresolvedFieldList)
	{
		for(tmp = unresolvedFieldList->first ; tmp != NULL ; tmp = tmp->next)
		{
			ILInt64 offset = ILAsmDataResolveLabel(tmp->name);
			if(offset == -1)
			{
				ILAsmPrintMessage(tmp->filename, tmp->linenum,
						  "data label `%s' undefined", tmp->name);
				ILAsmErrors = 1;
			}
			else
			{
				/* Set the RVA information for the field */
				if(!ILFieldRVACreate(ILAsmImage, 0,
									 tmp->field, (ILUInt32)offset))
				{
					ILAsmOutOfMemory();
				}
			}
			ILFree(tmp);
		}
		ILFree(unresolvedFieldList);
		unresolvedFieldList = 0;
	}
}
%}

/*
 * Define the structure of yylval.
 */
%union {
	ILInt64			integer;
	ILInt16			genParAttrib;
	ILIntString		strValue;
	ILDouble		real;
	char		   *quoteString;
	struct {
		ILUInt8		fbytes[4];
		ILUInt8		dbytes[8];
	}				floatValue;
	struct {
		ILInt64		flags;
		ILIntString	nativeType;
		ILInt64		pinvokeAttrs;
		const char *name1;
		const char *name2;
	}				fieldAttrs;
	struct {
		ILInt64		flags;
		ILInt64		pinvokeAttrs;
		const char *name1;
		const char *name2;
	}				methodAttrs;
	ILInt32			opcode;
	struct {
		ILIntString	interned;
		int			extraSize;
	}				byteList;
	ILClass		   *classInfo;
	ILProgramItem  *programItem;
	ILType		   *type;
	struct
	{
		ILType	   *type;
		ILIntString	nativeType;
	}				marshType;
	struct
	{
		ILAsmParamInfo *paramFirst;
		ILAsmParamInfo *paramLast;
	}				params;
	struct
	{
		ILUInt32	type;
		ILIntString	valueBlob;
	}				fieldInit;
	struct
	{
		ILType		   *type;
		ILProgramItem  *item;

	}				typeSpec;
	struct
	{
		const char *start;
		const char *end;
	}				scope;
	struct
	{
		const char *label;
		ILInt64     offset;
	}				datalabel;
	ILAsmOutException *exception;
	ILToken			token;
	ILAsmGenericTypeConstraint *typeConstraint;
}

/*
 * Primitive lexical tokens.
 */
%token INTEGER_CONSTANT		"integer value"
%token IDENTIFIER			"identifier"
%token DOT_IDENTIFIER		".identifier"
%token SQUOTE_STRING		"single-quoted string"
%token DQUOTE_STRING		"double-quoted string"
%token FLOAT_CONSTANT		"floating point value"
%token HEX_BYTE				"hexadecimal byte"
%token COLON_COLON			"::"
%token DOT_DOT_DOT			"..."
%token DOT_DOT				".."
%token EXCL_EXCL			"!!"

/*
 * Directives.
 */
%token D_ADDON				"`.addon'"
%token D_ALGORITHM			"`.algorithm'"
%token D_ASSEMBLY			"`.assembly'"
%token D_BACKING			"`.backing'"
%token D_BLOB				"`.blob'"
%token D_CAPABILITY			"`.capability'"
%token D_CCTOR				"`.cctor'"
%token D_CLASS				"`.class'"
%token D_COMTYPE			"`.comtype'"
%token D_CONFIG				"`.config'"
%token D_CORFLAGS			"`.corflags'"
%token D_CTOR				"`.ctor'"
%token D_CUSTOM				"`.custom'"
%token D_DATA				"`.data'"
%token D_EMITBYTE			"`.emitbyte'"
%token D_ENTRYPOINT			"`.entrypoint'"
%token D_EVENT				"`.event'"
%token D_EXELOC				"`.exeloc'"
%token D_EXPORT				"`.export'"
%token D_FIELD				"`.field'"
%token D_FILE				"`.file'"
%token D_FIRE				"`.fire'"
%token D_GET				"`.get'"
%token D_HASH				"`.hash'"
%token D_IMAGEBASE			"`.imagebase'"
%token D_IMPLICITCOM		"`.implicitcom'"
%token D_IMPORT				"`.import'"
%token D_LANGUAGE			"`.language'"
%token D_LIBRARY			"`.library'"
%token D_LINE				"`.line'"
%token D_LOCALE				"`.locale'"
%token D_LOCALIZED			"`.localized'"
%token D_LOCALS				"`.locals'"
%token D_MANIFESTRES		"`.manifestres'"
%token D_MAXSTACK			"`.maxstack'"
%token D_METHOD				"`.method'"
%token D_MIME				"`.mime'"
%token D_MODULE				"`.module'"
%token D_MRESOURCE			"`.mresource'"
%token D_NAMESPACE			"`.namespace'"
%token D_ORIGINATOR			"`.originator'"
%token D_OS					"`.os'"
%token D_OTHER				"`.other'"
%token D_OVERRIDE			"`.override'"
%token D_PACK				"`.pack'"
%token D_PARAM				"`.param'"
%token D_PDIRECT			"`.pdirect'"
%token D_PERMISSION			"`.permission'"
%token D_PERMISSIONSET		"`.permissionset'"
%token D_PROCESSOR			"`.processor'"
%token D_PROPERTY			"`.property'"
%token D_PUBLICKEY			"`.publickey'"
%token D_PUBLICKEYTOKEN		"`.publickeytoken'"
%token D_REMOVEON			"`.removeon'"
%token D_SET				"`.set'"
%token D_SIZE				"`.size'"
%token D_STACKRESERVE		"`.stackreserve'"
%token D_SUBSYSTEM			"`.subsystem'"
%token D_TITLE				"`.title'"
%token D_TRY				"`.try'"
%token D_VER				"`.ver'"
%token D_VTABLE				"`.vtable'"
%token D_VTENTRY			"`.vtentry'"
%token D_VTFIXUP			"`.vtfixup'"
%token D_ZEROINIT			"`.zeroinit'"

/*
 * Keywords.
 */
%token K_ABSTRACT			"`abstract'"
%token K_ALGORITHM			"`algorithm'"
%token K_ALIGNMENT			"`alignment'"
%token K_ANSI				"`ansi'"
%token K_ANY				"`any'"
%token K_ARGLIST			"`arglist'"
%token K_ARRAY				"`array'"
%token K_AS					"`as'"
%token K_ASSEMBLY			"`assembly'"
%token K_ASSERT				"`assert'"
%token K_AT					"`at'"
%token K_AUTO				"`auto'"
%token K_AUTOCHAR			"`autochar'"
%token K_BEFOREFIELDINIT    "`beforefieldinit'"
%token K_BLOB				"`blob'"
%token K_BLOB_OBJECT		"`blob_object'"
%token K_BOOL				"`bool'"
%token K_BOXED				"`boxed'"
%token K_BSTR				"`bstr'"
%token K_BYTEARRAY			"`bytearray'"
%token K_BYVALSTR			"`byvalstr'"
%token K_CALLMOSTDERIVED	"`callmostderived'"
%token K_CARRAY				"`carray'"
%token K_CATCH				"`catch'"
%token K_CDECL				"`cdecl'"
%token K_CF					"`cf'"
%token K_CHAR				"`char'"
%token K_CIL				"`cil'"
%token K_CLASS				"`class'"
%token K_CLSID				"`clsid'"
%token K_COMPILERCONTROLLED	"`compilercontrolled'"
%token K_CONST				"`const'"
%token K_CURRENCY			"`currency'"
%token K_CUSTOM				"`custom'"
%token K_DATE				"`date'"
%token K_DECIMAL			"`decimal'"
%token K_DEFAULT			"`default'"
%token K_DEMAND				"`demand'"
%token K_DENY				"`deny'"
%token K_DISABLEJITOPTIMIZER "`disablejitoptimizer'"
%token K_ENABLEJITTRACKING	"`enablejittracking'"
%token K_ENDMAC				"`endmac'"
%token K_ENUM				"`enum'"
%token K_ERROR				"`error'"
%token K_EXPLICIT			"`explicit'"
%token K_EXTENDS			"`extends'"
%token K_EXTERN				"`extern'"
%token K_FALSE				"`false'"
%token K_FAMANDASSEM		"`famandassem'"
%token K_FAMILY				"`family'"
%token K_FAMORASSEM			"`famorassem'"
%token K_FASTCALL			"`fastcall'"
%token K_FAULT				"`fault'"
%token K_FIELD				"`field'"
%token K_FILETIME			"`filetime'"
%token K_FILTER				"`filter'"
%token K_FINAL				"`final'"
%token K_FINALLY			"`finally'"
%token K_FIXED				"`fixed'"
%token K_FLOAT				"`float'"
%token K_FLOAT32			"`float32'"
%token K_FLOAT64			"`float64'"
%token K_FORWARDREF			"`forwardref'"
%token K_FROMUNMANAGED		"`fromunmanaged'"
%token K_FULLORIGIN			"`fullorigin'"
%token K_HANDLER			"`handler'"
%token K_HIDEBYSIG			"`hidebysig'"
%token K_HRESULT			"`hresult'"
%token K_IDISPATCH			"`idispatch'"
%token K_IL					"`il'"
%token K_IMPLEMENTS			"`implements'"
%token K_IMPLICITCOM		"`implicitcom'"
%token K_IMPLICITRES		"`implicitres'"
%token K_IMPORT				"`import'"
%token K_IN					"`in'"
%token K_INF				"`inf'"
%token K_INHERITCHECK		"`inheritcheck'"
%token K_INIT				"`init'"
%token K_INITONLY			"`initonly'"
%token K_INSTANCE			"`instance'"
%token K_INT				"``int''"
%token K_INT16				"`int16'"
%token K_INT32				"`int32'"
%token K_INT64				"`int64'"
%token K_INT8				"`int8'"
%token K_INTERFACE			"`interface'"
%token K_INTERNALCALL		"`internalcall'"
%token K_IUNKNOWN			"`iunknown'"
%token K_JAVA				"`java'"
%token K_LASTERR			"`lasterr'"
%token K_LATEINIT			"`lateinit'"
%token K_LCID				"`lcid'"
%token K_LINKCHECK			"`linkcheck'"
%token K_LITERAL			"`literal'"
%token K_LPSTR				"`lpstr'"
%token K_LPTSTR				"`lptstr'"
%token K_LPSTRUCT			"`lpstruct'"
%token K_LPVOID				"`lpvoid'"
%token K_LPWSTR				"`lpwstr'"
%token K_MANAGED			"`managed'"
%token K_MARSHAL			"`marshal'"
%token K_METHOD				"`method'"
%token K_MODOPT				"`modopt'"
%token K_MODREQ				"`modreq'"
%token K_NAN				"`nan'"
%token K_NATIVE				"`native'"
%token K_NESTED				"`nested'"
%token K_NEWSLOT			"`newslot'"
%token K_NOAPPDOMAIN		"`noappdomain'"
%token K_NOINLINING			"`noinlining'"
%token K_NOMACHINE			"`nomachine'"
%token K_NOMANGLE			"`nomangle'"
%token K_NOMETADATA			"`nometadata'"
%token K_NONCASDEMAND		"`noncasdemand'"
%token K_NONCASINHERITANCE	"`noncasinheritance'"
%token K_NONCASLINKDEMAND	"`noncaslinkdemand'"
%token K_NOPROCESS			"`noprocess'"
%token K_NOT_IN_GC_HEAP		"`not_in_gc_heap'"
%token K_NOTREMOTABLE		"`notremotable'"
%token K_NOTSERIALIZED		"`notserialized'"
%token K_NULL				"`null'"
%token K_NULLREF			"`nullref'"
%token K_OBJECT				"`object'"
%token K_OBJECTREF			"`objectref'"
%token K_OLE				"`ole'"
%token K_OPT				"`opt'"
%token K_OPTIL				"`optil'"
%token K_OUT				"`out'"
%token K_PERMITONLY			"`permitonly'"
%token K_PINNED				"`pinned'"
%token K_PINVOKEIMPL		"`pinvokeimpl'"
%token K_PREJITDENY			"`prejitdeny'"
%token K_PREJITGRANT		"`prejitgrant'"
%token K_PRESERVESIG        "`preservesig'"
%token K_PRIVATE			"`private'"
%token K_PRIVATESCOPE		"`privatescope'"
%token K_PROTECTED			"`protected'"
%token K_PUBLIC				"`public'"
%token K_PUBLICKEY			"`publickey'"
%token K_READONLY			"`readonly'"
%token K_RECORD				"`record'"
%token K_REQMIN				"`reqmin'"
%token K_REQOPT				"`reqopt'"
%token K_REQREFUSE			"`reqrefuse'"
%token K_REQSECOBJ			"`reqsecobj'"
%token K_REQUEST			"`request'"
%token K_RETARGETABLE		"`retargetable'"
%token K_RETVAL				"`retval'"
%token K_RTSPECIALNAME		"`rtspecialname'"
%token K_RUNTIME			"`runtime'"
%token K_SAFEARRAY			"`safearray'"
%token K_SEALED				"`sealed'"
%token K_SEQUENTIAL			"`sequential'"
%token K_SERIALIZABLE		"`serializable'"
%token K_SPECIAL			"`special'"
%token K_SPECIALNAME		"`specialname'"
%token K_STATIC				"`static'"
%token K_STDCALL			"`stdcall'"
%token K_STORAGE			"`storage'"
%token K_STORED_OBJECT		"`stored_object'"
%token K_STREAM				"`stream'"
%token K_STREAMED_OBJECT	"`streamed_object'"
%token K_STRING				"`string'"
%token K_STRUCT				"`struct'"
%token K_SYNCHRONIZED		"`synchronized'"
%token K_SYSCHAR			"`syschar'"
%token K_SYSSTRING			"`sysstring'"
%token K_TBSTR				"`tbstr'"
%token K_THISCALL			"`thiscall'"
%token K_TLS				"`tls'"
%token K_TO					"`to'"
%token K_TRUE				"`true'"
%token K_TYPE				"`type'"
%token K_TYPEDREF			"`typedref'"
%token K_UNICODE			"`unicode'"
%token K_UNMANAGED			"`unmanaged'"
%token K_UNMANAGEDEXP		"`unmanagedexp'"
%token K_UNSIGNED			"`unsigned'"
%token K_UNUSED				"`unused'"
%token K_USERDEFINED		"`userdefined'"
%token K_VALUE				"`value'"
%token K_VALUETYPE			"`valuetype'"
%token K_VARARG				"`vararg'"
%token K_VARIANT			"`variant'"
%token K_VECTOR				"`vector'"
%token K_VIRTUAL			"`virtual'"
%token K_VOID				"`void'"
%token K_VOLATILE			"`volatile'"
%token K_WCHAR				"`wchar'"
%token K_WINAPI				"`winapi'"
%token K_WITH				"`with'"
%token K_WRAPPER			"`wrapper'"

/*
 * Instruction types.
 */
%token I_NONE I_VAR I_INT I_FLOAT I_BRANCH I_METHOD I_FIELD I_TYPE
%token I_STRING I_SIGNATURE I_RVA I_TOKEN I_SSA I_SWITCH I_CONST
%token I_IINC I_LSWITCH I_IMETHOD I_NEWARRAY I_MULTINEWARRAY

/*
 * Define the yylval types of the various non-terminals.
 */
%type <integer>		INTEGER_CONSTANT HEX_BYTE
%type <quoteString> DQUOTE_STRING SQUOTE_STRING
%type <strValue>	ComposedString NativeType
%type <strValue>	QualifiedName Identifier MethodName IDENTIFIER Bytes
%type <strValue>	SlashedName DOT_IDENTIFIER AssemblyName
%type <real>		FLOAT_CONSTANT
%type <integer>		CallingConventions Integer32 Integer64
%type <integer>		ParameterAttributes ParameterAttributeList
%type <integer>		ParameterAttributeName ClassAttributes
%type <integer>		ClassAttributeList ClassAttributeName
%type <integer>		PInvokeAttributes PInvokeAttributeList PInvokeAttributeName
%type <integer>		ImplementationAttributes ImplementationAttributeList
%type <integer>		ImplementationAttributeName VariantType
%type <integer>		VtfixupAttributes VtfixupAttributeList VtfixupAttributeName
%type <integer>		EventAttributes EventAttributeList EventAttributeName
%type <integer>		PropertyAttributes PropertyAttributeList
%type <integer>		PropertyAttributeName SecurityAction
%type <integer>		FileAttributes FileAttributeList FileAttributeName
%type <integer>		AssemblyAttributes AssemblyAttributeList
%type <integer>		AssemblyAttributeName AssemblyRefAttributes
%type <integer>		AssemblyRefAttributeList AssemblyRefAttributeName
%type <integer>		ComTypeAttributes ComTypeAttributeList
%type <integer>		ComTypeAttributeName ManifestResAttributes
%type <integer>		ManifestResAttributeList ManifestResAttributeName
%type <integer>     LayoutOption JavaArrayType
%type <datalabel>   AtOption
%type <fieldAttrs>	FieldAttributes FieldAttributeList FieldAttributeName
%type <methodAttrs>	MethodAttributes MethodAttributeList MethodAttributeName
%type <opcode>		I_NONE I_VAR I_BRANCH I_METHOD I_FIELD I_TYPE
%type <opcode>		I_INT I_FLOAT I_STRING I_SIGNATURE I_RVA I_TOKEN
%type <opcode>		I_SSA I_SWITCH I_CONST I_IINC I_LSWITCH I_IMETHOD
%type <opcode>		I_NEWARRAY I_MULTINEWARRAY
%type <floatValue>	Float64 InstructionFloat
%type <byteList>	ByteList
%type <classInfo>	ClassName CatchClause
%type <programItem> ClassNameTypeSpec
%type <type>		Type ArrayBounds Bounds
%type <marshType>	MarshalledType
%type <typeSpec>	TypeSpecification
%type <params>		OptSignatureArguments SignatureArguments SignatureArgument
%type <fieldInit>	FieldInitialization InitOption
%type <programItem>	CustomType
%type <scope>		ScopeBlock TryBlock FilterClause HandlerBlock
%type <scope>		JavaScopeBlock JavaTryBlock JavaHandlerBlock
%type <exception>	ExceptionClause ExceptionClauses
%type <exception>	JavaExceptionClause JavaExceptionClauses
%type <token>		MethodReference InstanceMethodReference
%type <token>		GenericMethodReference CustomOwner
%type <integer>		DataItemCount
%type <params>		FormalGenericParamsOpt FormalGenericParams
%type <params>		FormalGenericParam
%type <params>		MethodRefGenericParamsOpt MethodRefGenericParams
%type <type>		ActualGenericParams
%type <integer>		GenericArityOpt
%type <genParAttrib> FormalGenericParamAttribute
%type <genParAttrib> FormalGenericParamAttributes
%type <genParAttrib> FormalGenericParamAttributesOpt
%type <typeConstraint> FormalGenericTypeConstraintList
%type <typeConstraint> FormalGenericTypeConstraintsOpt

%expect 9

%start File
%%

/*
 * Outer level of the assembly input file.
 */

File
	: Declarations
	{
		FinishDataLabels();
	}
	;

Declarations
	: /* empty */
	| DeclarationList
	;

DeclarationList
	: Declaration
	| DeclarationList Declaration
	;

Declaration
	: ClassHeading '{' ClassDeclarations '}'	{
	  			ILAsmBuildPopClass();
	  		}
	| D_NAMESPACE QualifiedName 	{
				ILAsmBuildPushNamespace($2);
			} '{' Declarations '}'	{
				ILAsmBuildPopNamespace($2.len);
			}
	| MethodHeading '{' MethodDeclarations '}'
	| MethodHeading K_JAVA '{'		{
				ILAsmParseJava = 1;
				ILJavaAsmInitPool();
			}
		JavaMethodDeclarations '}'	{
				ILAsmParseJava = 0;
			}
	| FieldDeclaration	{}
	| DataDeclaration
	| VtableDeclaration
	| VtfixupDeclaration
	| ExternalSourceSpecification
	| FileDeclaration
	| ExeLocationDeclaration
	| AssemblyHeading '{' AssemblyDeclarations '}'
	| AssemblyRefHeading '{' AssemblyRefDeclarations '}'
	| ExportHeading '{' ComTypeDeclarations '}'
	| ManifestResHeading '{' ManifestResDeclarations '}'
	| ModuleHeading
	| SecurityDeclaration
	| CustomAttributeDeclaration
	| CommentDeclaration
	;

/*
 * Declarations that are used as comments by other assemblers.
 */

CommentDeclaration
	: D_SUBSYSTEM INTEGER_CONSTANT
	| D_CORFLAGS INTEGER_CONSTANT
	| D_IMAGEBASE INTEGER_CONSTANT
	| D_FILE K_ALIGNMENT INTEGER_CONSTANT
	| D_STACKRESERVE INTEGER_CONSTANT
	;

LanguageDeclaration
	: D_LANGUAGE SQUOTE_STRING
	| D_LANGUAGE SQUOTE_STRING ',' SQUOTE_STRING
	| D_LANGUAGE SQUOTE_STRING ',' SQUOTE_STRING ',' SQUOTE_STRING
	;

/*
 * Common definitions.
 */

Identifier
	: IDENTIFIER	{ $$ = $1; }
	| SQUOTE_STRING	{ $$ = ILAsmParseString($1); }
	;

Integer32
	: INTEGER_CONSTANT { $$ = (ILInt64)(ILInt32)($1); }
	;

Integer64
	: INTEGER_CONSTANT { $$ = $1; }
	;

/* The output of this non-terminal is two buffers:
   a 4-byte little endian float and an 8-byte little
   endian double.  With any luck, they should be in
   IEEE form ready for output to the object file */
Float64
	: FLOAT_CONSTANT	{
				/* Literal floating point constant */
			#ifdef IL_CONFIG_FP_SUPPORTED
				SetFloat($$.fbytes, (ILFloat)($1));
				SetDouble($$.dbytes, $1);
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| K_FLOAT32 '(' Integer32 ')' {
			#ifdef IL_CONFIG_FP_SUPPORTED
				/* Convert a raw big endian value into a 32-bit float */
				$$.fbytes[3] = (ILUInt8)($3 >> 24);
				$$.fbytes[2] = (ILUInt8)($3 >> 16);
				$$.fbytes[1] = (ILUInt8)($3 >> 8);
				$$.fbytes[0] = (ILUInt8)($3);
				SetDouble($$.dbytes, (ILDouble)(IL_READ_FLOAT($$.fbytes)));
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| K_FLOAT64 '(' Integer64 ')' {
			#ifdef IL_CONFIG_FP_SUPPORTED
				/* Convert a raw big endian value into a 64-bit float */
				$$.dbytes[7] = (ILUInt8)($3 >> 56);
				$$.dbytes[6] = (ILUInt8)($3 >> 48);
				$$.dbytes[5] = (ILUInt8)($3 >> 40);
				$$.dbytes[4] = (ILUInt8)($3 >> 32);
				$$.dbytes[3] = (ILUInt8)($3 >> 24);
				$$.dbytes[2] = (ILUInt8)($3 >> 16);
				$$.dbytes[1] = (ILUInt8)($3 >> 8);
				$$.dbytes[0] = (ILUInt8)($3);
				SetFloat($$.fbytes, (ILFloat)(IL_READ_DOUBLE($$.dbytes)));
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| K_NAN				{
				/* Not a number */
				$$.fbytes[3] = (ILUInt8)0xFF;
				$$.fbytes[2] = (ILUInt8)0xC0;
				$$.fbytes[1] = (ILUInt8)0x00;
				$$.fbytes[0] = (ILUInt8)0x00;
				$$.dbytes[7] = (ILUInt8)0xFF;
				$$.dbytes[6] = (ILUInt8)0xF8;
				$$.dbytes[5] = (ILUInt8)0x00;
				$$.dbytes[4] = (ILUInt8)0x00;
				$$.dbytes[3] = (ILUInt8)0x00;
				$$.dbytes[2] = (ILUInt8)0x00;
				$$.dbytes[1] = (ILUInt8)0x00;
				$$.dbytes[0] = (ILUInt8)0x00;
			}
	| K_INF				{
				/* Positive infinity */
				$$.fbytes[3] = (ILUInt8)0x7F;
				$$.fbytes[2] = (ILUInt8)0x80;
				$$.fbytes[1] = (ILUInt8)0x00;
				$$.fbytes[0] = (ILUInt8)0x00;
				$$.dbytes[7] = (ILUInt8)0x7F;
				$$.dbytes[6] = (ILUInt8)0xF0;
				$$.dbytes[5] = (ILUInt8)0x00;
				$$.dbytes[4] = (ILUInt8)0x00;
				$$.dbytes[3] = (ILUInt8)0x00;
				$$.dbytes[2] = (ILUInt8)0x00;
				$$.dbytes[1] = (ILUInt8)0x00;
				$$.dbytes[0] = (ILUInt8)0x00;
			}
	| '-' K_INF			{
				/* Negative infinity */
				$$.fbytes[3] = (ILUInt8)0xFF;
				$$.fbytes[2] = (ILUInt8)0x80;
				$$.fbytes[1] = (ILUInt8)0x00;
				$$.fbytes[0] = (ILUInt8)0x00;
				$$.dbytes[7] = (ILUInt8)0xFF;
				$$.dbytes[6] = (ILUInt8)0xF0;
				$$.dbytes[5] = (ILUInt8)0x00;
				$$.dbytes[4] = (ILUInt8)0x00;
				$$.dbytes[3] = (ILUInt8)0x00;
				$$.dbytes[2] = (ILUInt8)0x00;
				$$.dbytes[2] = (ILUInt8)0x00;
				$$.dbytes[0] = (ILUInt8)0x00;
			}
	;

QualifiedName
	: Identifier					{ $$ = $1; }
	| QualifiedName '.' Identifier	{
				ILIntString dot;
				dot.string = ".";
				dot.len = 1;
				$$ = ILInternAppendedString
						($1, ILInternAppendedString(dot, $3));
			}
	| QualifiedName DOT_IDENTIFIER	{
				$$ = ILInternAppendedString($1, $2);
			}
	;

ComposedString
	: DQUOTE_STRING					{
				$$ = ILAsmParseString($1);
			}
	| ComposedString '+' DQUOTE_STRING {
				$$ = ILInternAppendedString($1, ILAsmParseString($3));
			}
	;

Bytes
	: '('			{ ILAsmParseHexBytes = 1; }
	  ByteList ')'	{
	  			ILAsmParseHexBytes = 0;
				if($3.interned.len > 0)
				{
					ILIntString temp;
					temp.string = byteBuffer;
					temp.len = $3.extraSize;
					$$ = ILInternAppendedString($3.interned, temp);
				}
				else
				{
					$$ = ILInternString(byteBuffer, $3.extraSize);
				}
			}
	;

ByteList
	: HEX_BYTE				{
				$$.interned = ILInternString("", 0);
				byteBuffer[0] = (char)($1);
				$$.extraSize = 1;
			}
	| ByteList HEX_BYTE		{
				if($1.extraSize < ILASM_BYTE_BUFSIZ)
				{
					$$.interned = $1.interned;
					$$.extraSize = $1.extraSize + 1;
					byteBuffer[$1.extraSize] = (char)($2);
				}
				else if($1.interned.len > 0)
				{
					ILIntString temp;
					temp.string = byteBuffer;
					temp.len = $1.extraSize;
					$$.interned = ILInternAppendedString($1.interned, temp);
					byteBuffer[0] = (char)($2);
					$$.extraSize = 1;
				}
				else
				{
					$$.interned = ILInternString(byteBuffer, $1.extraSize);
					byteBuffer[0] = (char)($2);
					$$.extraSize = 1;
				}
			}
	;

/*
 * Module definition.
 */

ModuleHeading
	: D_MODULE		{
				/* This form of declaration is used when the programmer
				   wants to attach custom attributes to the current module,
				   so we just modify the "last token" */
				ILAsmLastToken = GetToken(ILAsmModule);
			}
	| D_MODULE QualifiedName	{
				/* Set the name of the current module */
				if(!ILModuleSetName(ILAsmModule, $2.string))
				{
					ILAsmOutOfMemory();
				}
				ILAsmLastToken = GetToken(ILAsmModule);
			}
	| D_MODULE K_EXTERN QualifiedName	{
				/* Declare the name of an external module */
				ILModule *ref = ILModuleRefCreateUnique(ILAsmImage, $3.string);
				if(!ref)
				{
					ILAsmOutOfMemory();
				}
				ILAsmLastToken = GetToken(ref);
			}
	;

/*
 * Class definition.
 */

ClassHeading
	: D_CLASS ClassAttributes Identifier FormalGenericParamsOpt {
				/* Create the new class */
				ILAsmBuildNewClass($3.string, $4.paramFirst,
								   0, (ILUInt32)($2));
				ILAsmBuildPushScope(ILAsmClass);
			}
	  ExtendsClause ImplementsClause
	;

FormalGenericParamsOpt
	: /* empty */					{ $$.paramFirst = 0; $$.paramLast = 0; }
	| '<' FormalGenericParams '>'	{ $$ = $2; }
	;

FormalGenericParams
	: FormalGenericParam			{ $$ = $1; }
	| FormalGenericParams ',' FormalGenericParam	{
				$1.paramLast->next = $3.paramFirst;
				$$.paramFirst = $1.paramFirst;
				$$.paramLast = $3.paramLast;
		}
	;

FormalGenericParam
	: FormalGenericParamAttributesOpt FormalGenericTypeConstraintsOpt Identifier	{
				/* Generic parameter with a type constraint */
				ILAsmParamInfo *param;
				param = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!param)
				{
					ILAsmOutOfMemory();
				}
				param->type = 0;
				param->name = $3.string;
				param->parameterAttrs = $1;
				param->firstTypeConstraint = $2;
				param->next = 0;
				$$.paramFirst = param;
				$$.paramLast = param;
			}
	;

FormalGenericTypeConstraintsOpt
	: /* empty */								{ $$ = 0; }
	| '(' FormalGenericTypeConstraintList ')'	{ $$ = $2; }
	;

FormalGenericTypeConstraintList
	: TypeSpecification		{
				ILAsmGenericTypeConstraint *constraint;

				constraint = (ILAsmGenericTypeConstraint *)ILMalloc(sizeof(ILAsmGenericTypeConstraint));
				if(!constraint)
				{
					ILAsmOutOfMemory();
				}
				constraint->type = $1.type;
				constraint->next = 0;
				$$ = constraint;
			}
	| FormalGenericTypeConstraintList ',' TypeSpecification	{
				ILAsmGenericTypeConstraint *constraint;
				ILAsmGenericTypeConstraint *constraintEntry = $1;

				constraint = (ILAsmGenericTypeConstraint *)ILMalloc(sizeof(ILAsmGenericTypeConstraint));
				if(!constraint)
				{
					ILAsmOutOfMemory();
				}
				constraint->type = $3.type;
				constraint->next = 0;
				/* Append the new type at the end of the list. */
				while(constraintEntry->next != 0)
				{
					constraintEntry = constraintEntry->next;
				}
				constraintEntry->next = constraint;
				$$ = $1;
			}
	;

FormalGenericParamAttributesOpt
	: /* empty */					{ $$ = IL_META_GENPARAM_NONE; }
	| FormalGenericParamAttributes	{ $$ = $1; }
	;

FormalGenericParamAttributes
	: FormalGenericParamAttribute	{ $$ = $1; }
	| FormalGenericParamAttributes FormalGenericParamAttribute	{
				if(($1 & $2) != 0)
				{
						yyerror("duplicate generic parameter attribute");
				}
				else if(($2 & (IL_META_GENPARAM_CLASS_CONST |
							   IL_META_GENPARAM_VALUETYPE_CONST)) != 0)
				{
					if(($1 & (IL_META_GENPARAM_CLASS_CONST |
							  IL_META_GENPARAM_VALUETYPE_CONST)) != 0)
					{
						yyerror("only either class or valuetype constraint can be specified");
					}
				}
				$$ = ($1 | $2);
			}
	;

FormalGenericParamAttribute
	: '+'								{ $$ = IL_META_GENPARAM_COVARIANT; }
	| '-'								{ $$ = IL_META_GENPARAM_CONTRAVARIANT; }
	| K_CLASS							{ $$ = IL_META_GENPARAM_CLASS_CONST; }
	| K_VALUETYPE						{ $$ = IL_META_GENPARAM_VALUETYPE_CONST; }
	| D_CTOR							{ $$ = IL_META_GENPARAM_CTOR_CONST; }
	;

GenericTypeParamDirective
	: D_PARAM K_TYPE '[' Integer32 ']'	{
				ILGenericPar *genPar;
				genPar = ILAsmFindGenericParameter(ILToProgramItem(ILAsmCurrScope),
										   (ILUInt32)($4 - 1));
				if(genPar)
				{
					/* Set the last token, to allow custom attributes
					   to be attached to the parameter */
					ILAsmLastToken = ILProgramItem_Token(ILToProgramItem(genPar));
				}
			}
	| D_PARAM K_TYPE Identifier			{
				ILGenericPar *genPar;
				genPar = ILAsmResolveGenericPar(ILToProgramItem(ILAsmCurrScope),
												($3).string);
				if(genPar)
				{
					/* Set the last token, to allow custom attributes
					   to be attached to the parameter */
					ILAsmLastToken = ILProgramItem_Token(ILToProgramItem(genPar));
				}
			}
	;

ClassAttributes
	: /* empty */			{ $$ = 0; }
	| ClassAttributeList	{ 
				$$ = $1;
				if($$ & IL_META_TYPEDEF_INTERFACE)
				{
					$$ |= IL_META_TYPEDEF_ABSTRACT;
				}
			}
	;

ClassAttributeList
	: ClassAttributeName					{ $$ = $1; }
	| ClassAttributeList ClassAttributeName	{ $$ = $1 | $2; }
	;

ClassAttributeName
	: K_PUBLIC					{ $$ = IL_META_TYPEDEF_PUBLIC; }
	| K_PRIVATE					{ $$ = IL_META_TYPEDEF_NOT_PUBLIC; }
	| K_VALUE					{ $$ = IL_META_TYPEDEF_VALUE_TYPE; }
	| K_UNMANAGED				{ $$ = IL_META_TYPEDEF_UNMANAGED_VALUE_TYPE; }
	| K_NOT_IN_GC_HEAP			{ $$ = IL_META_TYPEDEF_UNMANAGED_VALUE_TYPE; }
	| K_INTERFACE				{ $$ = IL_META_TYPEDEF_INTERFACE; }
	| K_SEALED					{ $$ = IL_META_TYPEDEF_SEALED; }
	| K_ABSTRACT				{ $$ = IL_META_TYPEDEF_ABSTRACT; }
	| K_AUTO					{ $$ = IL_META_TYPEDEF_AUTO_LAYOUT; }
	| K_SEQUENTIAL				{ $$ = IL_META_TYPEDEF_LAYOUT_SEQUENTIAL; }
	| K_EXPLICIT				{ $$ = IL_META_TYPEDEF_EXPLICIT_LAYOUT; }
	| K_ANSI					{ $$ = IL_META_TYPEDEF_ANSI_CLASS; }
	| K_UNICODE					{ $$ = IL_META_TYPEDEF_UNICODE_CLASS; }
	| K_AUTOCHAR				{ $$ = IL_META_TYPEDEF_AUTO_CLASS; }
	| K_IMPORT					{ $$ = IL_META_TYPEDEF_IMPORT; }
	| K_SERIALIZABLE			{ $$ = IL_META_TYPEDEF_SERIALIZABLE; }
	| K_NESTED K_PUBLIC			{ $$ = IL_META_TYPEDEF_NESTED_PUBLIC; }
	| K_NESTED K_PRIVATE		{ $$ = IL_META_TYPEDEF_NESTED_PRIVATE; }
	| K_NESTED K_FAMILY			{ $$ = IL_META_TYPEDEF_NESTED_FAMILY; }
	| K_NESTED K_ASSEMBLY		{ $$ = IL_META_TYPEDEF_NESTED_ASSEMBLY; }
	| K_NESTED K_FAMANDASSEM	{ $$ = IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM; }
	| K_NESTED K_FAMORASSEM		{ $$ = IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM; }
	| K_LATEINIT				{ $$ = IL_META_TYPEDEF_LATE_INIT; }
	| K_BEFOREFIELDINIT			{ $$ = IL_META_TYPEDEF_BEFORE_FIELD_INIT; }
	| K_SPECIALNAME				{ $$ = IL_META_TYPEDEF_SPECIAL_NAME; }
	| K_RTSPECIALNAME			{ $$ = IL_META_TYPEDEF_RT_SPECIAL_NAME; }
	;

ExtendsClause
	: /* empty */			{
				/* Probably "System.Object" or an interface */
			}
	| K_EXTENDS ClassNameTypeSpec	{
				/* Extend a named class */
				ILClassSetParent(ILAsmClass, $2);
			}
	;

ImplementsClause
	: /* empty */
	| K_IMPLEMENTS ClassNameList
	;

ClassNameList
	: ClassNameTypeSpec		{
				if(!ILClassAddImplements(ILAsmClass, $1, 0))
				{
					ILAsmOutOfMemory();
				}
			}
	| ClassNameList ',' ClassNameTypeSpec	{
				if(!ILClassAddImplements(ILAsmClass, $3, 0))
				{
					ILAsmOutOfMemory();
				}
			}
	;

ClassDeclarations
	: /* empty */				{ ILAsmBuildPopScope(); }
	| ClassDeclarationList		{ ILAsmBuildPopScope(); }
	;

ClassDeclarationList
	: ClassDeclaration
	| ClassDeclarationList ClassDeclaration
	;

ClassDeclaration
	: MethodHeading '{' MethodDeclarations '}'
	| MethodHeading K_JAVA '{'	{
				ILAsmParseJava = 1;
				ILJavaAsmInitPool();
			}
	  JavaMethodDeclarations '}'	{
	  			ILAsmParseJava = 0;
	  		}
	| ClassHeading '{' ClassDeclarations '}'	{
				ILAsmBuildPopClass();
			}
	| EventHeading '{' EventDeclarations '}'
	| PropertyHeading '{' PropertyDeclarations '}'
	| FieldDeclaration
	| DataDeclaration
	| SecurityDeclaration
	| ExternalSourceSpecification
	| CustomAttributeDeclaration
	| D_SIZE Integer32	{
				/* Set the class layout size */
				ILClassLayout *layout;
				layout = ILClassLayoutGetFromOwner(ILAsmClass);
				if(!layout)
				{
					/* Create a new layout record */
					layout = ILClassLayoutCreate(ILAsmImage, 0, ILAsmClass,
												 0, (ILUInt32)($2));
					if(!layout)
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					ILClassLayoutSetClassSize(layout, (ILUInt32)($2));
				}
			}
	| D_PACK Integer32	{
				/* Set the class packing size */
				ILClassLayout *layout;
				layout = ILClassLayoutGetFromOwner(ILAsmClass);
				if(!layout)
				{
					/* Create a new layout record */
					layout = ILClassLayoutCreate(ILAsmImage, 0, ILAsmClass,
												 (ILUInt32)($2), 0);
					if(!layout)
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					ILClassLayoutSetPackingSize(layout, (ILUInt32)($2));
				}
			}
	| ExportHeading '{' ComTypeDeclarations '}'
	| D_OVERRIDE TypeSpecification COLON_COLON MethodName K_WITH
			CallingConventions Type TypeSpecification COLON_COLON
			MethodName '(' OptSignatureArguments ')'	{
				ILType *sig;
				ILOverride *over;
				ILToken token;
				ILMethod *decl;
				ILMethod *body;

				/* Create a signature block for the methods */
				sig = CreateMethodSig($6, $7, $12.paramFirst, 0, 1);

				/* Create a MemberRef for the first part of the override */
				token = ILAsmResolveMember($2.item, $4.string, sig,
										   IL_META_MEMBERKIND_METHOD);
				decl = ILMethod_FromToken(ILAsmImage, token);

				/* Create a MemberRef for the second part of the override */
				token = ILAsmResolveMember($8.item, $10.string, sig,
										   IL_META_MEMBERKIND_METHOD);
				body = ILMethod_FromToken(ILAsmImage, token);

				/* Create the override block */
				if(decl && body)
				{
					over = ILOverrideCreate(ILAsmClass, 0, decl, body);
					if(!over)
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| D_OVERRIDE K_METHOD CallingConventions Type TypeSpecification COLON_COLON
			MethodName GenericArityOpt '(' OptSignatureArguments ')' K_WITH
			K_METHOD CallingConventions Type TypeSpecification COLON_COLON
			MethodName GenericArityOpt '(' OptSignatureArguments ')'	{
				ILType *sig;
				ILOverride *over;
				ILToken token;
				ILMethod *decl;
				ILMethod *body;

				/* Create a signature block for the method to override */
				sig = CreateMethodSig($3, $4, $10.paramFirst, $8, 1);

				/* Create a MemberRef for the first part of the override */
				token = ILAsmResolveMember($5.item, $7.string, sig,
										   IL_META_MEMBERKIND_METHOD);
				decl = ILMethod_FromToken(ILAsmImage, token);

				/* Create a signature block for the overriding method */
				sig = CreateMethodSig($14, $15, $21.paramFirst, $19, 1);

				/* Create a MemberRef for the second part of the override */
				token = ILAsmResolveMember($16.item, $18.string, sig,
										   IL_META_MEMBERKIND_METHOD);
				body = ILMethod_FromToken(ILAsmImage, token);

				/* Create the override block */
				if(decl && body)
				{
					over = ILOverrideCreate(ILAsmClass, 0, decl, body);
					if(!over)
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| GenericTypeParamDirective
	;

/*
 * Field declaration.
 */

FieldDeclaration
	: D_FIELD LayoutOption FieldAttributes Type Identifier
			AtOption InitOption {
				ILField *field;
				ILFieldMarshal *marshal;
				ILConstant *constant;
				ILModule *module;

				field = ILAsmFieldCreate(ILAsmClass, $5.string, $3.flags, $4);
				if(($3.flags & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) != 0)
				{
					/* Add marshalling information to the field */
					marshal = ILFieldMarshalCreate(ILAsmImage, 0,
												   ILToProgramItem(field));
					if(!marshal)
					{
						ILAsmOutOfMemory();
					}
					if(!ILFieldMarshalSetType(marshal,
											  $3.nativeType.string,
											  $3.nativeType.len))
					{
						ILAsmOutOfMemory();
					}
				}
				if(($3.flags & IL_META_FIELDDEF_PINVOKE_IMPL) != 0)
				{
					/* Add PInvoke information to the field */
					module = FindModuleRef(ILAsmImage, $3.name1);
					if(!ILPInvokeFieldCreate(field, 0, $3.pinvokeAttrs,
										     module, $3.name2))
					{
						ILAsmOutOfMemory();
					}
				}
				if($2 != (ILInt64)0xFFFFFFFF)
				{
					/* Set the layout information for the field */
					if(!ILFieldLayoutCreate(ILAsmImage, 0,
											field, (ILUInt32)($2)))
					{
						ILAsmOutOfMemory();
					}
				}
				if($6.offset != (ILInt64)0xFFFFFFFF)
				{
					if($6.offset != -1 && $6.label != NULL)
					{
						/* Set the RVA information for the field */
						if(!ILFieldRVACreate(ILAsmImage, 0,
											 field, (ILUInt32)($6.offset)))
						{
							ILAsmOutOfMemory();
						}
					}
					else
					{
						RegisterFieldRvaLabel($6.label,field);
					}
				}
				if($7.type != IL_META_ELEMTYPE_VOID)
				{
					/* Attach a constant to the field */
					constant = ILConstantCreate(ILAsmImage, 0,
												ILToProgramItem(field),
												$7.type);
					if(!constant)
					{
						ILAsmOutOfMemory();
					}
					if(!ILConstantSetValue(constant, $7.valueBlob.string,
										   $7.valueBlob.len))
					{
						ILAsmOutOfMemory();
					}
				}
				ILAsmBuildPushScope(field);

				/* If we are in the global module class, and the field
				   is public, then change the module class to public */
				if(ILAsmClass == ILAsmModuleClass && ILField_IsPublic(field))
				{
					ILClassSetAttrs(ILAsmClass,
									IL_META_TYPEDEF_VISIBILITY_MASK,
									IL_META_TYPEDEF_PUBLIC);
				}
			}
		FieldBody	{
				/* Keep the field token, in case the old style of
				   assigning custom attributes to fields is used */
				ILToken token = ILAsmLastToken;
				ILAsmBuildPopScope();
				ILAsmLastToken = token;
			}
	;

/*
 * Note: FieldBody is specific to this assembler.  It is not
 * part of the ECMA assembly syntax.  This is because ECMA is
 * a little fuzzy when it comes to assigning custom attributes
 * and line numbers to fields, and we want to be precise.
 */
FieldBody
	: /* empty */
	| '{' FieldBodyDeclarations '}'
	;

FieldBodyDeclarations
	: /* empty */
	| FieldBodyDeclarationList
	;

FieldBodyDeclarationList
	: FieldBodyDeclaration
	| FieldBodyDeclarationList FieldBodyDeclaration
	;

FieldBodyDeclaration
	: ExternalSourceSpecification
	| CustomAttributeDeclaration
	;

LayoutOption
	: /* empty */			{ $$ = (ILInt64)0xFFFFFFFF; }
	| '[' Integer32 ']'		{ $$ = $2; }
	;

AtOption
	: /* empty */			{ $$.offset = (ILInt64)0xFFFFFFFF; $$.label = 0; }
	| K_AT Identifier		{
				$$.label = $2.string;
				$$.offset = (ILInt64)ILAsmDataResolveLabel($2.string);
			}
	| K_AT Integer32		{ $$.offset = $2; $$.label = 0; }
	;

InitOption
	: /* empty */	{
				$$.type = IL_META_ELEMTYPE_VOID;
				$$.valueBlob.string = "";
				$$.valueBlob.len = 0;
			}
	| '=' FieldInitialization	{ $$ = $2; }
	;

FieldAttributes
	: /* empty */			{ $$.flags = 0;
							  $$.nativeType.string = 0;
							  $$.nativeType.len = 0; }
	| FieldAttributeList	{ $$ = $1; }
	;

FieldAttributeList
	: FieldAttributeName						{ $$ = $1; }
	| FieldAttributeList FieldAttributeName		{
				$$.flags = $1.flags | $2.flags;
				if(($1.flags & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) != 0)
				{
					$$.nativeType = $1.nativeType;
					if(($2.flags & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) != 0)
					{
						yyerror("duplicate `marshal' attribute on field");
					}
				}
				else if(($2.flags & IL_META_FIELDDEF_HAS_FIELD_MARSHAL) != 0)
				{
					$$.nativeType = $2.nativeType;
				}
				else
				{
					$$.nativeType.string = 0;
					$$.nativeType.len = 0;
				}
				if(($1.flags & IL_META_FIELDDEF_PINVOKE_IMPL) != 0)
				{
					$$.pinvokeAttrs = $1.pinvokeAttrs;
					$$.name1 = $1.name1;
					$$.name2 = $1.name2;
					if(($2.flags & IL_META_FIELDDEF_PINVOKE_IMPL) != 0)
					{
						yyerror("duplicate `pinvokeimpl' attribute on field");
					}
				}
				else if(($2.flags & IL_META_FIELDDEF_PINVOKE_IMPL) != 0)
				{
					$$.pinvokeAttrs = $2.pinvokeAttrs;
					$$.name1 = $2.name1;
					$$.name2 = $2.name2;
				}
				else
				{
					$$.pinvokeAttrs = 0;
					$$.name1 = 0;
					$$.name2 = 0;
				}
			}
	;

FieldAttributeName
	: K_STATIC				{ SET_FIELD(STATIC); }
	| K_PUBLIC				{ SET_FIELD(PUBLIC); }
	| K_PRIVATE				{ SET_FIELD(PRIVATE); }
	| K_FAMILY				{ SET_FIELD(FAMILY); }
	| K_INITONLY			{ SET_FIELD(INIT_ONLY); }
	| K_RTSPECIALNAME		{ SET_FIELD(RT_SPECIAL_NAME); }
	| K_SPECIALNAME			{ SET_FIELD(SPECIAL_NAME); }
	| K_MARSHAL '(' NativeType ')'	{
				$$.flags = IL_META_FIELDDEF_HAS_FIELD_MARSHAL;
				$$.nativeType = $3;
			}
	| K_ASSEMBLY			{ SET_FIELD(ASSEMBLY); }
	| K_FAMANDASSEM			{ SET_FIELD(FAM_AND_ASSEM); }
	| K_FAMORASSEM			{ SET_FIELD(FAM_OR_ASSEM); }
	| K_PRIVATESCOPE		{ SET_FIELD(COMPILER_CONTROLLED); /* Old name */ }
	| K_COMPILERCONTROLLED	{ SET_FIELD(COMPILER_CONTROLLED); }
	| K_LITERAL				{ SET_FIELD(LITERAL); }
	| K_NOTSERIALIZED		{ SET_FIELD(NOT_SERIALIZED); }
	| K_PINVOKEIMPL '(' ComposedString K_AS
			ComposedString PInvokeAttributes ')'	{
				$$.flags = IL_META_FIELDDEF_PINVOKE_IMPL;
				$$.nativeType.string = 0;
				$$.nativeType.len = 0;
			    $$.pinvokeAttrs = $6;
			    $$.name1 = $3.string;
			    $$.name2 = $5.string;
			}
	| K_PINVOKEIMPL '(' ComposedString PInvokeAttributes ')'	{
				$$.flags = IL_META_FIELDDEF_PINVOKE_IMPL;
				$$.nativeType.string = 0;
				$$.nativeType.len = 0;
			    $$.pinvokeAttrs = $4;
				$$.name1 = $3.string;
				$$.name2 = 0;
			}
	| K_PINVOKEIMPL '(' PInvokeAttributes ')'	{
				$$.flags = IL_META_FIELDDEF_PINVOKE_IMPL;
				$$.nativeType.string = 0;
				$$.nativeType.len = 0;
			    $$.pinvokeAttrs = $3;
				$$.name1 = 0;
				$$.name2 = 0;
			}
	;

FieldInitialization
	: K_FLOAT32 '(' FLOAT_CONSTANT ')'	{
			#ifdef IL_CONFIG_FP_SUPPORTED
				unsigned char bytes[4];
				SetFloat(bytes, (ILFloat)($3));
				$$.type = IL_META_ELEMTYPE_R4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| K_FLOAT64 '(' FLOAT_CONSTANT ')'	{
			#ifdef IL_CONFIG_FP_SUPPORTED
				unsigned char bytes[8];
				SetDouble(bytes, (ILDouble)($3));
				$$.type = IL_META_ELEMTYPE_R8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| K_FLOAT32 '(' Integer64 ')'	{
				unsigned char bytes[4];
				bytes[3] = (ILUInt8)($3 >> 24);
				bytes[2] = (ILUInt8)($3 >> 16);
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_R4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	| K_FLOAT64 '(' Integer64 ')'	{
				unsigned char bytes[8];
				bytes[7] = (ILUInt8)($3 >> 56);
				bytes[6] = (ILUInt8)($3 >> 48);
				bytes[5] = (ILUInt8)($3 >> 40);
				bytes[4] = (ILUInt8)($3 >> 32);
				bytes[3] = (ILUInt8)($3 >> 24);
				bytes[2] = (ILUInt8)($3 >> 16);
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_R8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			}
	| K_FLOAT32 '(' K_NAN ')'	{
				unsigned char bytes[4];
				bytes[3] = (ILUInt8)0xFF;
				bytes[2] = (ILUInt8)0xC0;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	| K_FLOAT64 '(' K_NAN ')'	{
				unsigned char bytes[8];
				bytes[7] = (ILUInt8)0xFF;
				bytes[6] = (ILUInt8)0xF8;
				bytes[5] = (ILUInt8)0x00;
				bytes[4] = (ILUInt8)0x00;
				bytes[3] = (ILUInt8)0x00;
				bytes[2] = (ILUInt8)0x00;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			}
	| K_FLOAT32 '(' K_INF ')'	{
				unsigned char bytes[4];
				bytes[3] = (ILUInt8)0x7F;
				bytes[2] = (ILUInt8)0x80;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	| K_FLOAT64 '(' K_INF ')'	{
				unsigned char bytes[8];
				bytes[7] = (ILUInt8)0x7F;
				bytes[6] = (ILUInt8)0xF0;
				bytes[5] = (ILUInt8)0x00;
				bytes[4] = (ILUInt8)0x00;
				bytes[3] = (ILUInt8)0x00;
				bytes[2] = (ILUInt8)0x00;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			}
	| K_FLOAT32 '(' '-' K_INF ')'	{
				unsigned char bytes[4];
				bytes[3] = (ILUInt8)0xFF;
				bytes[2] = (ILUInt8)0x80;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	| K_FLOAT64 '(' '-' K_INF ')'	{
				unsigned char bytes[8];
				bytes[7] = (ILUInt8)0xFF;
				bytes[6] = (ILUInt8)0xF0;
				bytes[5] = (ILUInt8)0x00;
				bytes[4] = (ILUInt8)0x00;
				bytes[3] = (ILUInt8)0x00;
				bytes[2] = (ILUInt8)0x00;
				bytes[1] = (ILUInt8)0x00;
				bytes[0] = (ILUInt8)0x00;
				$$.type = IL_META_ELEMTYPE_R8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			}
	| K_INT64 '(' Integer64 ')'	{
				unsigned char bytes[8];
				bytes[7] = (ILUInt8)($3 >> 56);
				bytes[6] = (ILUInt8)($3 >> 48);
				bytes[5] = (ILUInt8)($3 >> 40);
				bytes[4] = (ILUInt8)($3 >> 32);
				bytes[3] = (ILUInt8)($3 >> 24);
				bytes[2] = (ILUInt8)($3 >> 16);
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_I8;
				$$.valueBlob = ILInternString((char *)bytes, 8);
			}
	| K_INT32 '(' Integer64 ')'	{
				unsigned char bytes[4];
				bytes[3] = (ILUInt8)($3 >> 24);
				bytes[2] = (ILUInt8)($3 >> 16);
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_I4;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	| K_INT16 '(' Integer64 ')'	{
				unsigned char bytes[2];
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_I2;
				$$.valueBlob = ILInternString((char *)bytes, 2);
			}
	| K_INT8 '(' Integer64 ')'	{
				unsigned char bytes[1];
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_I1;
				$$.valueBlob = ILInternString((char *)bytes, 1);
			}
	| K_CHAR '(' Integer64 ')'	{
				unsigned char bytes[2];
				bytes[1] = (ILUInt8)($3 >> 8);
				bytes[0] = (ILUInt8)($3);
				$$.type = IL_META_ELEMTYPE_CHAR;
				$$.valueBlob = ILInternString((char *)bytes, 2);
			}
	| K_BOOL '(' K_TRUE ')' {
				unsigned char bytes[1];
				bytes[0] = 1;
				$$.type = IL_META_ELEMTYPE_BOOLEAN;
				$$.valueBlob = ILInternString((char *)bytes, 1);
			}
	| K_BOOL '(' K_FALSE ')' {
				unsigned char bytes[1];
				bytes[0] = 0;
				$$.type = IL_META_ELEMTYPE_BOOLEAN;
				$$.valueBlob = ILInternString((char *)bytes, 1);
			}
	| ComposedString	{
				$$.type = IL_META_ELEMTYPE_STRING;
				$$.valueBlob = PackUnicodeString($1);
			}
	| K_WCHAR '*' '(' ComposedString ')'	{
				/* obsolete */
				$$.type = IL_META_ELEMTYPE_STRING;
				$$.valueBlob = PackUnicodeString($4);
			}
	| K_BYTEARRAY Bytes	{
				$$.type = IL_META_ELEMTYPE_STRING;
				$$.valueBlob = $2;
			}
	| K_NULLREF	{
				unsigned char bytes[4];
				bytes[0] = 0;
				bytes[1] = 0;
				bytes[2] = 0;
				bytes[3] = 0;
				$$.type = IL_META_ELEMTYPE_CLASS;
				$$.valueBlob = ILInternString((char *)bytes, 4);
			}
	;

/*
 * Method declaration.
 */

MethodHeading
	: D_METHOD MethodAttributes CallingConventions ParameterAttributes
			MarshalledType MethodName FormalGenericParamsOpt
			'(' OptSignatureArguments ')' ImplementationAttributes
					{
				ILType *sig;
				ILMethod *method;
				ILUInt32 callConv;

				/* Create the method definition */
				callConv = (ILUInt32)($3);
				if(($2.flags & IL_META_METHODDEF_STATIC) == 0)
				{
					/* Add "instance" to the calling conventions, because
					   ECMA allows it to be omitted from non-static methods
					   (Partition II, section 14.3) */
					callConv |= IL_META_CALLCONV_HASTHIS;
				}
				sig = CreateMethodSig(callConv, $5.type, $9.paramFirst,
									  GetNumParams($7.paramFirst), 0);
				method = ILAsmMethodCreate
							(ILAsmClass, $6.string, $2.flags, sig);
				ILMethodSetImplAttrs(method, ~0, (ILUInt16)($11));
				ILAsmOutAddParams($9.paramFirst, callConv);
				AddMethodParams(method, $4, $5.nativeType, $9.paramFirst);
				if(($2.flags & IL_META_METHODDEF_PINVOKE_IMPL) != 0)
				{
					ILModule *module;
					module = FindModuleRef(ILAsmImage, $2.name1);
					if(!ILPInvokeCreate(method, 0, $2.pinvokeAttrs,
										module, $2.name2))
					{
						ILAsmOutOfMemory();
					}
				}

				/* Add the formal generic parameters to the method */
				ILAsmAddGenericPars(ILToProgramItem(method), $7.paramFirst);

				/* The current scope is now the method */
				ILAsmBuildPushScope(method);

				/* If we are in the global module class, and the method
				   is public, then change the module class to public */
				if(ILAsmClass == ILAsmModuleClass && ILMethod_IsPublic(method))
				{
					ILClassSetAttrs(ILAsmClass,
									IL_META_TYPEDEF_VISIBILITY_MASK,
									IL_META_TYPEDEF_PUBLIC);
				}
			}
	;

MarshalledType
	: Type									{
				$$.type = $1;
				$$.nativeType.string = "";
				$$.nativeType.len = 0;
			}
	| Type K_MARSHAL '(' NativeType ')'		{
				$$.type = $1;
				$$.nativeType = $4;
			}
	;

MethodName
	: D_CTOR		{ $$ = ILInternString(".ctor", 5); }
	| D_CCTOR		{ $$ = ILInternString(".cctor", 6); }
	| QualifiedName	{ $$ = $1; }
	;

MethodAttributes
	: /* empty */			{ $$.flags = 0; $$.pinvokeAttrs = 0;
							  $$.name1 = 0; $$.name2 = 0; }
	| MethodAttributeList	{ $$ = $1; }
	;

MethodAttributeList
	: MethodAttributeName	{ $$ = $1; }
	| MethodAttributeList MethodAttributeName	{
				$$.flags = $1.flags | $2.flags;
				if(($1.flags & IL_META_METHODDEF_PINVOKE_IMPL) != 0)
				{
					$$.pinvokeAttrs = $1.pinvokeAttrs;
					$$.name1 = $1.name1;
					$$.name2 = $1.name2;
					if(($2.flags & IL_META_METHODDEF_PINVOKE_IMPL) != 0)
					{
						yyerror("duplicate `pinvokeimpl' attribute on method");
					}
				}
				else if(($2.flags & IL_META_METHODDEF_PINVOKE_IMPL) != 0)
				{
					$$.pinvokeAttrs = $2.pinvokeAttrs;
					$$.name1 = $2.name1;
					$$.name2 = $2.name2;
				}
				else
				{
					$$.pinvokeAttrs = 0;
					$$.name1 = 0;
					$$.name2 = 0;
				}
			}
	;

MethodAttributeName
	: K_STATIC				{ SET_METHOD(STATIC); }
	| K_PUBLIC				{ SET_METHOD(PUBLIC); }
	| K_PRIVATE				{ SET_METHOD(PRIVATE); }
	| K_FAMILY				{ SET_METHOD(FAMILY); }
	| K_FINAL				{ SET_METHOD(FINAL); }
	| K_SPECIALNAME			{ SET_METHOD(SPECIAL_NAME); }
	| K_VIRTUAL				{ SET_METHOD(VIRTUAL); }
	| K_ABSTRACT			{ SET_METHOD(ABSTRACT); }
	| K_ASSEMBLY			{ SET_METHOD(ASSEM); }
	| K_FAMANDASSEM			{ SET_METHOD(FAM_AND_ASSEM); }
	| K_FAMORASSEM			{ SET_METHOD(FAM_OR_ASSEM); }
	| K_PRIVATESCOPE		{ SET_METHOD(COMPILER_CONTROLLED); /* Old name */ }
	| K_COMPILERCONTROLLED	{ SET_METHOD(COMPILER_CONTROLLED); }
	| K_HIDEBYSIG			{ SET_METHOD(HIDE_BY_SIG); }
	| K_NEWSLOT				{ SET_METHOD(NEW_SLOT); }
	| K_RTSPECIALNAME		{ SET_METHOD(RT_SPECIAL_NAME); }
	| K_REQSECOBJ			{ SET_METHOD(REQUIRE_SEC_OBJECT); }
	| K_UNMANAGEDEXP		{ SET_METHOD(UNMANAGED_EXPORT); }
	| K_PINVOKEIMPL '(' ComposedString K_AS
			ComposedString PInvokeAttributes ')'	{
				$$.flags = IL_META_METHODDEF_PINVOKE_IMPL;
			    $$.pinvokeAttrs = $6;
			    $$.name1 = $3.string;
			    $$.name2 = $5.string;
			}
	| K_PINVOKEIMPL '(' ComposedString PInvokeAttributes ')'	{
				$$.flags = IL_META_METHODDEF_PINVOKE_IMPL;
			    $$.pinvokeAttrs = $4;
				$$.name1 = $3.string;
				$$.name2 = 0;
			}
	| K_PINVOKEIMPL '(' PInvokeAttributes ')'	{
				$$.flags = IL_META_METHODDEF_PINVOKE_IMPL;
			    $$.pinvokeAttrs = $3;
				$$.name1 = 0;
				$$.name2 = 0;
			}
	;

PInvokeAttributes
	: /* empty */			{ $$ = 0; }
	| PInvokeAttributeList	{ $$ = $1; }
	;

PInvokeAttributeList
	: PInvokeAttributeName						{ $$ = $1; }
	| PInvokeAttributeList PInvokeAttributeName	{ $$ = $1 | $2; }
	;

PInvokeAttributeName
	: K_NOMANGLE			{ $$ = IL_META_PINVOKE_NO_MANGLE; }
	| K_ANSI				{ $$ = IL_META_PINVOKE_CHAR_SET_ANSI; }
	| K_UNICODE				{ $$ = IL_META_PINVOKE_CHAR_SET_UNICODE; }
	| K_AUTOCHAR			{ $$ = IL_META_PINVOKE_CHAR_SET_AUTO; }
	| K_OLE					{ $$ = IL_META_PINVOKE_OLE; }
	| K_LASTERR				{ $$ = IL_META_PINVOKE_SUPPORTS_LAST_ERROR; }
	| K_WINAPI				{ $$ = IL_META_PINVOKE_CALL_CONV_WINAPI; }
	| K_CDECL				{ $$ = IL_META_PINVOKE_CALL_CONV_CDECL; }
	| K_STDCALL				{ $$ = IL_META_PINVOKE_CALL_CONV_STDCALL; }
	| K_THISCALL			{ $$ = IL_META_PINVOKE_CALL_CONV_THISCALL; }
	| K_FASTCALL			{ $$ = IL_META_PINVOKE_CALL_CONV_FASTCALL; }
	;

ImplementationAttributes
	: /* empty */					{ $$ = 0; }
	| ImplementationAttributeList	{ $$ = $1; }
	;

ImplementationAttributeList
	: ImplementationAttributeName	{ $$ = $1; }
	| ImplementationAttributeList ImplementationAttributeName { $$ = $1 | $2; }
	;

ImplementationAttributeName
	: K_NATIVE				{ $$ = IL_META_METHODIMPL_NATIVE; }
	| K_IL					{ $$ = IL_META_METHODIMPL_IL; /* Old name */ }
	| K_CIL					{ $$ = IL_META_METHODIMPL_IL; /* New name */ }
	| K_OPTIL				{ $$ = IL_META_METHODIMPL_OPTIL; }
	| K_MANAGED				{ $$ = IL_META_METHODIMPL_MANAGED; }
	| K_UNMANAGED			{ $$ = IL_META_METHODIMPL_UNMANAGED; }
	| K_FORWARDREF			{ $$ = IL_META_METHODIMPL_FORWARD_REF; }
	| K_OLE					{ $$ = IL_META_METHODIMPL_PRESERVE_SIG; /* Old */ }
	| K_PRESERVESIG			{ $$ = IL_META_METHODIMPL_PRESERVE_SIG; }
	| K_RUNTIME				{ $$ = IL_META_METHODIMPL_RUNTIME; }
	| K_INTERNALCALL		{ $$ = IL_META_METHODIMPL_INTERNAL_CALL; }
	| K_SYNCHRONIZED		{ $$ = IL_META_METHODIMPL_SYNCHRONIZED; }
	| K_NOINLINING			{ $$ = IL_META_METHODIMPL_NO_INLINING; }
	;

GenericArityOpt
	: /* empty */			{ $$ = 0; }
	| '<' '[' Integer32 ']' '>'	{ $$ = $3; }
	;

MethodDeclarations
	: /* empty */			{
				ILAsmOutFinalizeMethod((ILMethod *)ILAsmCurrScope);
			  	ILAsmBuildPopScope();
			}
	| MethodDeclarationList	{
				ILAsmOutFinalizeMethod((ILMethod *)ILAsmCurrScope);
			  	ILAsmBuildPopScope();
			}
	;

MethodDeclarationList
	: MethodDeclaration
	| MethodDeclarationList MethodDeclaration
	;

MethodDeclaration
	: D_EMITBYTE Integer32	{ ILAsmOutSimple($2 & 0xFF); }
	| ExceptionBlock
	| D_MAXSTACK Integer32	{ ILAsmOutMaxStack((ILUInt32)($2)); }
	| D_LOCALS '(' OptSignatureArguments ')'	{
				ILAsmOutAddLocals($3.paramFirst);
			}
	| D_LOCALS K_INIT '(' OptSignatureArguments ')'	{
				ILAsmOutZeroInit();
				ILAsmOutAddLocals($4.paramFirst);
			}
	| D_LOCALS '<' Identifier '=' Integer32 '>'	{
				/* Extension for use with "cscc": declare a variable name */
				ILAsmOutDeclareVarName($3.string, (ILUInt32)($5));
			}
	| D_ENTRYPOINT			{ ILWriterSetEntryPoint
									(ILAsmWriter, (ILMethod *)ILAsmCurrScope); }
	| D_ZEROINIT			{ ILAsmOutZeroInit(); }
	| DataDeclaration
	| Instruction
	| Identifier ':'		{ ILAsmOutLabel($1.string); }
	| Integer32 ':'			{ ILAsmOutIntLabel($1); }
	| SecurityDeclaration
	| ExternalSourceSpecification
	| CustomAttributeDeclaration
	| D_VTENTRY Integer32 ':' Integer32	{ /* TODO */ }
	| D_OVERRIDE TypeSpecification COLON_COLON MethodName	{
				ILType *sig;
				ILOverride *over;
				ILToken token;
				ILMethod *decl;

				/* Get the signature block for the current method */
				sig = ILMethod_Signature((ILMethod *)ILAsmCurrScope);

				/* Create a MemberRef for the first part of the override */
				token = ILAsmResolveMember($2.item, $4.string, sig,
										   IL_META_MEMBERKIND_METHOD);
				decl = ILMethod_FromToken(ILAsmImage, token);

				/* Create the override block */
				if(decl)
				{
					over = ILOverrideCreate(ILAsmClass, 0,
											decl, (ILMethod *)ILAsmCurrScope);
					if(!over)
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| ScopeBlock	{ /* Nothing to do here */ }
	| D_PARAM '[' Integer32 ']'	{
				ILParameter *param;
				param = ILAsmFindParameter((ILMethod *)ILAsmCurrScope,
										   (ILUInt32)($3));
				if(param)
				{
					/* Set the last token, to allow custom attributes
					   to be attached to the parameter */
					ILAsmLastToken = ILParameter_Token(param);
				}
			}
	| D_PARAM '[' Integer32 ']' '=' FieldInitialization	{
				ILParameter *param;
				ILConstant *constant;
				param = ILAsmFindParameter((ILMethod *)ILAsmCurrScope,
										   (ILUInt32)($3));
				if(param)
				{
					/* Attach a constant token to the parameter */
					constant = ILConstantCreate(ILAsmImage, 0,
												ILToProgramItem(param),
												$6.type);
					if(!constant)
					{
						ILAsmOutOfMemory();
					}
					if(!ILConstantSetValue(constant, $6.valueBlob.string,
										   $6.valueBlob.len))
					{
						ILAsmOutOfMemory();
					}

					/* Set the last token, to allow custom attributes
					   to be attached to the parameter */
					ILAsmLastToken = ILParameter_Token(param);
				}
			}
	| GenericTypeParamDirective
	;

JavaMethodDeclarations
	: /* empty */			{
				ILAsmOutFinalizeMethod((ILMethod *)ILAsmCurrScope);
			  	ILAsmBuildPopScope();
			}
	| JavaMethodDeclarationList	{
				ILAsmOutFinalizeMethod((ILMethod *)ILAsmCurrScope);
			  	ILAsmBuildPopScope();
			}
	;

JavaMethodDeclarationList
	: JavaMethodDeclaration
	| JavaMethodDeclarationList JavaMethodDeclaration
	;

JavaMethodDeclaration
	: D_MAXSTACK Integer32	 { ILAsmOutMaxStack((ILUInt32)($2));  }
	| D_LOCALS Integer32	 { ILAsmOutMaxLocals((ILUInt32)($2)); }
	| JavaInstruction
	| JavaExceptionBlock
	| JavaScopeBlock				{ /* nothing to do here */ }
	| Identifier ':'				{ ILAsmOutLabel($1.string); }
	| Integer32 ':'					{ ILAsmOutIntLabel($1); }
	| SecurityDeclaration
	| ExternalSourceSpecification
	| CustomAttributeDeclaration
	;

ScopeBlock
	: '{' 	{
				/* Record the start of the block */
				$<scope>$.start = ILAsmOutUniqueLabel();
				ILAsmOutPushVarScope($<scope>$.start);
			}
	  ScopeDeclarations '}'	{
	  			/* Record the end of the block */
				$$.start = $<scope>2.start;
				$$.end = ILAsmOutUniqueLabel();
				ILAsmOutPopVarScope($$.end);
	  		}
	;

ScopeDeclarations
	: /* empty */
	| MethodDeclarationList
	;

ExceptionBlock
	: TryBlock ExceptionClauses	{
				ILAsmOutAddTryBlock($1.start, $1.end, $2);
			}
	;

ExceptionClauses
	: ExceptionClause ExceptionClauses	{
				$$ = $1;
				$1->next = $2;
			}
	| ExceptionClause	{ $$ = $1; }
	;

TryBlock
	: D_TRY ScopeBlock					{ $$ = $2; }
	| D_TRY Identifier K_TO Identifier	{ $$.start = $2.string;
										  $$.end = $4.string; }
	| D_TRY Integer32 K_TO Integer32	{
				$$.start = ILAsmOutIntToName($2);
				$$.end = ILAsmOutIntToName($4);
			}
	;

ExceptionClause
	: CatchClause HandlerBlock		{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_CATCH, $1, 0,
										   $2.start, $2.end);
			}
	| FilterClause HandlerBlock		{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_FILTER,
										   0, $1.start,
										   $2.start, $2.end);
			}
	| FinallyClause HandlerBlock	{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_FINALLY, 0, 0,
										   $2.start, $2.end);
			}
	| FaultClause HandlerBlock		{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_FAULT, 0, 0,
										   $2.start, $2.end);
			}
	;

CatchClause
	: K_CATCH ClassName		{ $$ = $2; }
	;

FilterClause
	: K_FILTER ScopeBlock	{ $$ = $2; }
	| K_FILTER Identifier	{ $$.start = $2.string; $$.end = 0; }
	| K_FILTER Integer32	{ $$.start = ILAsmOutIntToName($2); $$.end = 0; }
	;

FinallyClause
	: K_FINALLY
	;

FaultClause
	: K_FAULT
	;

HandlerBlock
	: ScopeBlock							{ $$ = $1; }
	| K_HANDLER Identifier K_TO Identifier	{ $$.start = $2.string;
											  $$.end = $4.string; }
	| K_HANDLER Integer32 K_TO Integer32	{
				$$.start = ILAsmOutIntToName($2);
				$$.end = ILAsmOutIntToName($4);
			}
	;

JavaScopeBlock
	: '{' 	{
				/* Record the start of the block */
				$<scope>$.start = ILAsmOutUniqueLabel();
			}
	  JavaScopeDeclarations '}'	{
	  			/* Record the end of the block */
				$$.start = $<scope>2.start;
				$$.end = ILAsmOutUniqueLabel();
	  		}
	;

JavaScopeDeclarations
	: /* empty */
	| JavaMethodDeclarationList
	;

JavaExceptionBlock
	: JavaTryBlock JavaExceptionClauses	{
				ILAsmOutAddTryBlock($1.start, $1.end, $2);
			}
	;

JavaExceptionClauses
	: JavaExceptionClause JavaExceptionClauses	{
				$$ = $1;
				$1->next = $2;
			}
	| JavaExceptionClause	{ $$ = $1; }
	;

JavaTryBlock
	: D_TRY JavaScopeBlock					{ $$ = $2; }
	| D_TRY Identifier K_TO Identifier	{ $$.start = $2.string;
										  $$.end = $4.string; }
	| D_TRY Integer32 K_TO Integer32	{
				$$.start = ILAsmOutIntToName($2);
				$$.end = ILAsmOutIntToName($4);
			}
	;

JavaExceptionClause
	: CatchClause JavaHandlerBlock		{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_CATCH, $1, 0,
										   $2.start, $2.end);
			}
	| FinallyClause JavaHandlerBlock	{
				$$ = ILAsmOutMakeException(IL_META_EXCEPTION_FINALLY, 0, 0,
										   $2.start, $2.end);
			}
	;

JavaHandlerBlock
	: JavaScopeBlock				{ $$ = $1; }
	| K_HANDLER Identifier 			{ $$.start = $2.string;
									  $$.end = 0; }
	| K_HANDLER Integer32 			{ $$.start = ILAsmOutIntToName($2);
									  $$.end = 0; }
	;

/*
 * Event declaration.
 */

EventHeading
	: D_EVENT EventAttributes TypeSpecification Identifier	{
				ILEvent *event;
				event = ILEventCreate(ILAsmClass, 0, $4.string,
									  (ILUInt32)($2),
									  ILProgramItemToClass($3.item));
				if(!event)
				{
					ILAsmOutOfMemory();
				}
				ILAsmBuildPushScope(event);
			}
	| D_EVENT EventAttributes Identifier	{
				ILEvent *event;
				event = ILEventCreate(ILAsmClass, 0, $3.string,
									  (ILUInt32)($2), 0);
				if(!event)
				{
					ILAsmOutOfMemory();
				}
				if(!ILEventMapCreate(ILAsmImage, 0, ILAsmClass, event))
				{
					ILAsmOutOfMemory();
				}
				ILAsmBuildPushScope(event);
			}
	;

EventAttributes
	: /* empty */			{ $$ = 0; }
	| EventAttributeList	{ $$ = $1; }
	;

EventAttributeList
	: EventAttributeName					{ $$ = $1; }
	| EventAttributeList EventAttributeName	{ $$ = $1 | $2; }
	;

EventAttributeName
	: K_RTSPECIALNAME		{ $$ = IL_META_EVENTDEF_RT_SPECIAL_NAME; }
	| K_SPECIALNAME			{ $$ = IL_META_EVENTDEF_SPECIAL_NAME; }
	;

EventDeclarations
	: /* empty */			{ ILAsmBuildPopScope(); }
	| EventDeclarationList	{ ILAsmBuildPopScope(); }
	;

EventDeclarationList
	: EventDeclaration
	| EventDeclarationList EventDeclaration
	;

EventDeclaration
	: D_ADDON MethodReference	{
				ILAsmAddSemantics(IL_META_METHODSEM_ADD_ON, $2);
			}
	| D_REMOVEON MethodReference	{
				ILAsmAddSemantics(IL_META_METHODSEM_REMOVE_ON, $2);
			}
	| D_FIRE MethodReference	{
				ILAsmAddSemantics(IL_META_METHODSEM_FIRE, $2);
			}
	| D_OTHER MethodReference {
				ILAsmAddSemantics(IL_META_METHODSEM_OTHER, $2);
			}
	| ExternalSourceSpecification
	| CustomAttributeDeclaration
	;

/*
 * Method references.
 */

MethodReference
	: CallingConventions Type TypeSpecification COLON_COLON
			MethodName '(' OptSignatureArguments ')'	{
				ILType *sig;
				sig = CreateMethodSig($1, $2, $7.paramFirst, 0, 1);
				$$ = ILAsmResolveMember($3.item, $5.string, sig,
								        IL_META_MEMBERKIND_METHOD);
			}
	| CallingConventions Type MethodName '(' OptSignatureArguments ')' {
				/* Reference a method in the global module class */
				ILType *sig = CreateMethodSig($1, $2, $5.paramFirst, 0, 1);
				$$ = ILAsmResolveMember(ILToProgramItem(ILAsmModuleClass),
									    $3.string, sig,
								        IL_META_MEMBERKIND_METHOD);
			}
	;

GenericMethodReference
	: CallingConventions Type TypeSpecification COLON_COLON
			MethodName MethodRefGenericParamsOpt
			'(' OptSignatureArguments ')'	{
				ILType *sig;
				sig = CreateMethodSig($1, $2, $8.paramFirst,
									  GetNumParams($6.paramFirst), 1);
				$$ = ILAsmResolveMember($3.item, $5.string, sig,
								        IL_META_MEMBERKIND_METHOD);
				if($6.paramFirst)
				{
					$$ = CreateMethodSpec($$, $6.paramFirst);
				}
			}
	| CallingConventions Type MethodName MethodRefGenericParamsOpt
			'(' OptSignatureArguments ')' {
				/* Reference a method in the global module class */
				ILType *sig = CreateMethodSig($1, $2, $6.paramFirst,
											  GetNumParams($4.paramFirst), 1);
				$$ = ILAsmResolveMember(ILToProgramItem(ILAsmModuleClass),
									    $3.string, sig,
								        IL_META_MEMBERKIND_METHOD);
				if($4.paramFirst)
				{
					$$ = CreateMethodSpec($$, $4.paramFirst);
				}
			}
	;

InstanceMethodReference
	: CallingConventions Type TypeSpecification COLON_COLON
			MethodName MethodRefGenericParamsOpt
			'(' OptSignatureArguments ')'	{
				ILType *sig;
				sig = CreateMethodSig($1 | IL_META_CALLCONV_HASTHIS,
									  $2, $8.paramFirst,
									  GetNumParams($6.paramFirst), 1);
				$$ = ILAsmResolveMember($3.item, $5.string, sig,
								        IL_META_MEMBERKIND_METHOD);
				if($6.paramFirst)
				{
					$$ = CreateMethodSpec($$, $6.paramFirst);
				}
			}
	| CallingConventions Type MethodName MethodRefGenericParamsOpt
			'(' OptSignatureArguments ')' {
				/* Reference a method in the global module class */
				ILType *sig = CreateMethodSig
						($1 | IL_META_CALLCONV_HASTHIS,
					     $2, $6.paramFirst, GetNumParams($4.paramFirst), 1);
				$$ = ILAsmResolveMember(ILToProgramItem(ILAsmModuleClass),
									    $3.string, sig,
								        IL_META_MEMBERKIND_METHOD);
				if($4.paramFirst)
				{
					$$ = CreateMethodSpec($$, $4.paramFirst);
				}
			}
	;

MethodRefGenericParamsOpt
	: /* empty */					  { $$.paramFirst = 0; $$.paramLast = 0; }
	| '<' MethodRefGenericParams '>'  { $$ = $2; }
	;

MethodRefGenericParams
	: Type			{
				ILAsmParamInfo *genericParam;
				genericParam = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!genericParam)
				{
					ILAsmOutOfMemory();
				}
				genericParam->type = $1;
				genericParam->parameterAttrs = 0;
				genericParam->name = 0;
				genericParam->firstTypeConstraint = 0;
				genericParam->next = 0;
				$$.paramFirst = genericParam;
				$$.paramLast = genericParam;
			}
	| MethodRefGenericParams ',' Type	{
				ILAsmParamInfo *genericParam;
				genericParam = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!genericParam)
				{
					ILAsmOutOfMemory();
				}
				genericParam->type = $3;
				genericParam->parameterAttrs = 0;
				genericParam->name = 0;
				genericParam->firstTypeConstraint = 0;
				genericParam->next = 0;
				$1.paramLast->next = genericParam;
				$$.paramFirst = $1.paramFirst;
				$$.paramLast = genericParam;
			}
	;

/*
 * Property declaration.
 */

PropertyHeading
	: D_PROPERTY PropertyAttributes CallingConventions Type Identifier
			'(' OptSignatureArguments ')' InitOption	{
				ILType *sig = CreatePropertySig($3, $4, $7.paramFirst);
				ILProperty *property;
				property = ILPropertyCreate(ILAsmClass, 0, $5.string,
											(ILUInt32)($2), sig);
				if(!property)
				{
					ILAsmOutOfMemory();
				}
				if($9.type != IL_META_ELEMTYPE_VOID)
				{
					/* Attach a constant to the property */
					ILConstant *constant;

					constant = ILConstantCreate(ILAsmImage, 0,
												ILToProgramItem(property),
												$9.type);
					if(!constant)
					{
						ILAsmOutOfMemory();
					}
					if(!ILConstantSetValue(constant, $9.valueBlob.string,
										   $9.valueBlob.len))
					{
						ILAsmOutOfMemory();
					}
				}
				ILAsmBuildPushScope(property);
			}
	;

PropertyAttributes
	: /* empty */			{ $$ = 0; }
	| PropertyAttributeList	{ $$ = $1; }
	;

PropertyAttributeList
	: PropertyAttributeName							{ $$ = 0; }
	| PropertyAttributeList PropertyAttributeName	{ $$ = $1 | $2; }
	;

PropertyAttributeName
	: K_RTSPECIALNAME		{ $$ = IL_META_PROPDEF_RT_SPECIAL_NAME; }
	| K_SPECIALNAME			{ $$ = IL_META_PROPDEF_SPECIAL_NAME; }
	;

PropertyDeclarations
	: /* empty */				{ ILAsmBuildPopScope(); }
	| PropertyDeclarationList	{ ILAsmBuildPopScope(); }
	;

PropertyDeclarationList
	: PropertyDeclaration
	| PropertyDeclarationList PropertyDeclaration
	;

PropertyDeclaration
	: D_SET MethodReference	{
				ILAsmAddSemantics(IL_META_METHODSEM_SETTER, $2);
			}
	| D_GET MethodReference {
				ILAsmAddSemantics(IL_META_METHODSEM_GETTER, $2);
			}
	| D_OTHER MethodReference {
				ILAsmAddSemantics(IL_META_METHODSEM_OTHER, $2);
			}
	| D_BACKING Type Identifier		{ /* Obsolete */ }
	| CustomAttributeDeclaration
	| ExternalSourceSpecification
	;

/*
 * Data declaration.
 */

DataDeclaration
	: DataHeading DataBody
	;

DataHeading
	: D_DATA DataTLS Identifier '='		{
				/* Record the section offset with the label */
				ILAsmDataSetLabel($3.string);
			}
	| D_DATA DataTLS					{ /* nothing to do here */ }
	;

DataTLS
	: /* empty */			{ ILAsmDataSetNormal(); }
	| K_TLS					{ ILAsmDataSetTLS(); }
	;

DataBody
	: '{' DataItemList '}'
	| DataItem
	;

DataItemList
	: DataItem
	| DataItemList ',' DataItem
	;

DataItemCount
	: /* empty */			{ $$ = 1; }
	| '[' Integer32 ']'		{ $$ = $2; }
	;

DataItem
	: K_CHAR '*' '(' ComposedString ')'	{
				ILAsmDataWriteBytes((ILUInt8 *)($4.string), $4.len);
			}
	| K_WCHAR '*' '(' ComposedString ')'	{
				ILAsmDataWriteBytes((ILUInt8 *)($4.string), $4.len);
			}
	| '&' '(' Identifier ')'	{
				/* Not supported */
				ILAsmDataWriteInt32(0, 1);
			}
	| K_BYTEARRAY Bytes	{
				ILAsmDataWriteBytes((ILUInt8 *)($2.string), $2.len);
			}
	| K_FLOAT32 '(' Float64 ')' DataItemCount	{
				ILAsmDataWriteFloat32($3.fbytes, (ILUInt32)($5));
			}
	| K_FLOAT64 '(' Float64 ')' DataItemCount	{
				ILAsmDataWriteFloat64($3.dbytes, (ILUInt32)($5));
			}
	| K_INT64 '(' Integer64 ')' DataItemCount	{
				ILAsmDataWriteInt64($3, (ILUInt32)($5));
			}
	| K_INT32 '(' Integer32 ')' DataItemCount	{
				ILAsmDataWriteInt32((ILInt32)($3), (ILUInt32)($5));
			}
	| K_INT16 '(' Integer32 ')' DataItemCount	{
				ILAsmDataWriteInt16((ILInt32)($3), (ILUInt32)($5));
			}
	| K_INT8 '(' Integer32 ')' DataItemCount	{
				ILAsmDataWriteInt8((ILInt32)($3), (ILUInt32)($5));
			}
	| K_FLOAT32 DataItemCount	{
				ILAsmDataPad((ILUInt32)(4 * $2));
			}
	| K_FLOAT64 DataItemCount	{
				ILAsmDataPad((ILUInt32)(8 * $2));
			}
	| K_INT64 DataItemCount		{
				ILAsmDataPad((ILUInt32)(8 * $2));
			}
	| K_INT32 DataItemCount		{
				ILAsmDataPad((ILUInt32)(4 * $2));
			}
	| K_INT16 DataItemCount		{
				ILAsmDataPad((ILUInt32)(2 * $2));
			}
	| K_INT8 DataItemCount		{
				ILAsmDataPad((ILUInt32)($2));
			}
	;

/*
 * Virtual method tables.
 */

VtableDeclaration
	: D_VTABLE '=' Bytes
	;

VtfixupDeclaration
	: D_VTFIXUP '[' Integer32 ']' VtfixupAttributes K_AT Identifier	{}
	;

VtfixupAttributes
	: /* empty */			{ $$ = 0; }
	| VtfixupAttributeList	{ $$ = $1; }
	;

VtfixupAttributeList
	: VtfixupAttributeName						{ $$ = $1; }
	| VtfixupAttributeList VtfixupAttributeName	{ $$ = $1 | $2; }
	;

VtfixupAttributeName
	: K_INT32				{ $$ = IL_META_VTFIXUP_32BIT; }
	| K_INT64				{ $$ = IL_META_VTFIXUP_64BIT; }
	| K_FROMUNMANAGED		{ $$ = IL_META_VTFIXUP_FROM_UNMANAGED; }
	| K_CALLMOSTDERIVED		{ $$ = IL_META_VTFIXUP_CALL_MOST_DERIVED; }
	;

/*
 * Types.
 */

TypeSpecification
	: ClassName			{
				if(ILClassIsValueType($1))
					$$.type = ILType_FromValueType($1);
				else
					$$.type = ILType_FromClass($1);
				$$.item = ILToProgramItem($1);
			}
	| '[' QualifiedName ']'				{
				/* TODO: Reference to a foreign assembly's global type */
				$$.type = ILType_Invalid;
				$$.item = 0;
			}
	| '[' D_MODULE QualifiedName ']'	{
				/* TODO: Reference to a foreign module's global type */
				$$.type = ILType_Invalid;
				$$.item = 0;
			}
	| Type								{
				/* Convert a type into a TypeSpec token of some kind */
				$$.type = $1;
				if(ILType_IsPrimitive($1))
				{
					/* Convert a primitive type to a typedef or typeref */
					$$.item = ILToProgramItem(ILClassFromType(ILAsmImage, 0, $1, 0));
				}
				else if(ILType_IsClass($1) || ILType_IsValueType($1))
				{
					/* Simple class reference */
					$$.item = ILToProgramItem(ILType_ToClass($1));
				}
				else
				{
					/* Search the TypeSpec table for a match */
					ILTypeSpec *spec = 0;
					while((spec = (ILTypeSpec *)
							ILImageNextToken(ILAsmImage,
											 IL_META_TOKEN_TYPE_SPEC,
											 spec)) != 0)
					{
						if(ILTypeIdentical(ILTypeSpec_Type(spec), $1))
						{
							break;
						}
					}
					if(!spec)
					{
						/* Create a new TypeSpec token */
						spec = ILTypeSpecCreate(ILAsmImage, 0, $1);
						if(!spec)
						{
							ILAsmOutOfMemory();
						}
					}
					$$.item = ILToProgramItem(spec);
				}
			}
	;

ClassName
	: '[' QualifiedName ']' SlashedName	{
				ILAssembly *assem;

				/* Get the assembly reference: NULL if current assembly */
				if(!strcmp($2.string, ".library"))
				{
					assem = ILAsmFindAssemblyRef(ILAsmLibraryName);
				}
				else
				{
					assem = ILAsmFindAssemblyRef($2.string);
				}

				/* Look up the class, or create a TypeRef */
				if(assem)
				{
					$$ = ILAsmClassLookup($4.string, (ILProgramItem *)assem);
				}
				else
				{
					$$ = ILAsmClassLookup($4.string,
										  (ILProgramItem *)ILAsmModule);
				}
			}
	| '[' D_LIBRARY ']' SlashedName	{
				ILAssembly *assem;

				/* Find the library: NULL if it is the current assembly */
				assem = ILAsmFindAssemblyRef(ILAsmLibraryName);

				/* Look up the class, or create a TypeRef */
				if(assem)
				{
					$$ = ILAsmClassLookup($4.string, (ILProgramItem *)assem);
				}
				else
				{
					$$ = ILAsmClassLookup($4.string,
										  (ILProgramItem *)ILAsmModule);
				}
			}
	| '[' D_MODULE QualifiedName ']' SlashedName	{
				ILModule *module;

				/* Get the module reference: NULL if current module */
				module = ILAsmFindModuleRef($3.string);
				if(!module)
				{
					module = ILAsmModule;
				}

				/* Look up the class, or create a TypeRef */
				$$ = ILAsmClassLookup($5.string, (ILProgramItem *)module);
			}
	| SlashedName	{
				/* Look up the class or create a reference to this module */
				$$ = ILAsmClassLookup($1.string, (ILProgramItem *)ILAsmModule);
			}
	;

ClassNameTypeSpec
	: TypeSpecification		{
				$$ = $1.item;
			}
	;

SlashedName
	: QualifiedName						{ $$ = $1; }
	| SlashedName '/' QualifiedName		{
				ILIntString slash;
				slash.string = ILASM_NESTED_CLASS_SEP_STR;
				slash.len = 1;
				$$ = ILInternAppendedString
						($1, ILInternAppendedString(slash, $3));
			}
	;

Type
	: K_CLASS ClassName				{ $$ = ILType_FromClass($2); }
	| K_VALUE K_CLASS ClassName		{ $$ = ILType_FromValueType($3); }
	| K_VALUETYPE ClassName			{ $$ = ILType_FromValueType($2); }
	| K_BOXED ClassName				{ $$ = ILType_FromClass($2); }
	| Type '[' '?' ']'	{
				$$ = CombineArrayType
						($1, ILTypeCreateArray(ILAsmContext, 1, 0), 0);
			}
	| Type '[' ArrayBounds ']'	{
				$$ = CombineArrayType($1, $3, 0);
			}
	| Type '&'			{
				$$ = ILTypeCreateRef(ILAsmContext, IL_TYPE_COMPLEX_BYREF, $1);
			}
	| Type '*'	{
				$$ = ILTypeCreateRef(ILAsmContext, IL_TYPE_COMPLEX_PTR, $1);
			}
	| Type '%'	{
				/* Not sure if this is right: '%' doesn't seem to be doc'ed */
				$$ = ILTypeCreateRef(ILAsmContext, IL_TYPE_COMPLEX_PTR, $1);
			}
	| Type K_PINNED		{
				$$ = ILTypeCreateRef(ILAsmContext, IL_TYPE_COMPLEX_PINNED, $1);
			}
	| Type K_MODREQ '(' ClassName ')'	{
				ILType *modifiers =
						ILTypeCreateModifier(ILAsmContext, 0,
											 IL_TYPE_COMPLEX_CMOD_REQD, $4);
				$$ = ILTypeAddModifiers(ILAsmContext, modifiers, $1);
			}
	| Type K_MODOPT '(' ClassName ')'	{
				ILType *modifiers =
						ILTypeCreateModifier(ILAsmContext, 0,
											 IL_TYPE_COMPLEX_CMOD_OPT, $4);
				$$ = ILTypeAddModifiers(ILAsmContext, modifiers, $1);
			}
	| K_METHOD CallingConventions Type '*' '(' OptSignatureArguments ')'	{
				$$ = CreateMethodSig($2, $3, $6.paramFirst, 0, 1);
			}
	| K_TYPEDREF					{ $$ = ILType_TypedRef; }
	| K_WCHAR						{ $$ = ILType_Char; }
	| K_CHAR						{ $$ = ILType_Char; }
	| K_VOID						{ $$ = ILType_Void; }
	| K_BOOL						{ $$ = ILType_Boolean; }
	| K_INT8						{ $$ = ILType_Int8; }
	| K_INT16						{ $$ = ILType_Int16; }
	| K_INT32						{ $$ = ILType_Int32; }
	| K_INT64						{ $$ = ILType_Int64; }
	| K_FLOAT32						{ $$ = ILType_Float32; }
	| K_FLOAT64						{ $$ = ILType_Float64; }
	| K_UNSIGNED K_INT8				{ $$ = ILType_UInt8; }
	| K_UNSIGNED K_INT16			{ $$ = ILType_UInt16; }
	| K_UNSIGNED K_INT32			{ $$ = ILType_UInt32; }
	| K_UNSIGNED K_INT64			{ $$ = ILType_UInt64; }
	| K_NATIVE K_INT				{ $$ = ILType_Int; }
	| K_NATIVE K_UNSIGNED K_INT		{ $$ = ILType_UInt; }
	| K_NATIVE K_FLOAT				{ $$ = ILType_Float; }
	| K_STRING			{ $$ = ILType_FromClass(ILAsmSystemClass("String")); }
	| K_OBJECT			{ $$ = ILType_FromClass(ILAsmSystemClass("Object")); }
	| '!' Integer32					{
				/* Reference to a class generic parameter */
				$$ = ILTypeCreateVarNum
						(ILAsmContext, IL_TYPE_COMPLEX_VAR, (int)($2));
			}
	| '!' Identifier				{
				/* Reference to a class generic parameter */
				$$ = ILAsmResolveGenericClassPar(ILAsmCurrScope, ($2).string);
			}
	| EXCL_EXCL Integer32			{
				/* Reference to a method generic parameter */
				$$ = ILTypeCreateVarNum
						(ILAsmContext, IL_TYPE_COMPLEX_MVAR, (int)($2));
			}
	| EXCL_EXCL Identifier			{
				/* Reference to a method generic parameter */
				$$ = ILAsmResolveGenericMethodPar(ILAsmCurrScope, ($2).string);
			}
	| Type '<' ActualGenericParams '>'	{
				/* Reference to a generic type instantiation */
				($3)->un.method__.retType__ = $1;
				$$ = $3;
			}
	;

ActualGenericParams
	: Type			{
				$$ = ILTypeCreateWith(ILAsmContext, ILType_Invalid);
				if(!($$))
				{
					ILAsmOutOfMemory();
				}
				if(!ILTypeAddWithParam(ILAsmContext, $$, $1))
				{
					ILAsmOutOfMemory();
				}
			}
	| ActualGenericParams ',' Type	{
				if(!ILTypeAddWithParam(ILAsmContext, $1, $3))
				{
					ILAsmOutOfMemory();
				}
				$$ = $1;
			}
	;

ArrayBounds
	: Bounds					{ $$ = $1; }
	| ArrayBounds ',' Bounds	{
				$$ = CombineArrayType($1, $3, 1);
			}
	;

/*
 * Note: the floating point productions are required
 * because there is an ambiguity in the lexer.  The
 * sequence "DDDD." is recognised as floating point,
 * eating one of the dots in the "...".
 */
Bounds
	: /* empty */			{ $$ = ILTypeCreateArray(ILAsmContext, 1, 0); }
	| DOT_DOT_DOT			{ $$ = ILTypeCreateArray(ILAsmContext, 1, 0); }
	| Integer32				{
				$$ = ILTypeCreateArray(ILAsmContext, 1, 0);
				ILTypeSetSize($$, 0, (long)$1);
			}
	| Integer32 DOT_DOT_DOT Integer32	{
				$$ = ILTypeCreateArray(ILAsmContext, 1, 0);
				ILTypeSetSize($$, 0, (long)($3 - $1 + 1));
				ILTypeSetLowBound($$, 0, (long)$1);
			}
	| FLOAT_CONSTANT DOT_DOT Integer32	{
				$$ = ILTypeCreateArray(ILAsmContext, 1, 0);
				ILTypeSetSize($$, 0, (long)($3 - $1 + 1));
				ILTypeSetLowBound($$, 0, (long)$1);
			}
	| Integer32 DOT_DOT_DOT {
				$$ = ILTypeCreateArray(ILAsmContext, 1, 0);
				ILTypeSetLowBound($$, 0, (long)$1);
			}
	| FLOAT_CONSTANT DOT_DOT {
				$$ = ILTypeCreateArray(ILAsmContext, 1, 0);
				ILTypeSetLowBound($$, 0, (long)$1);
			}
	;

NativeType
	: /* empty */			{ $$ = SimpleNative(IL_META_NATIVETYPE_MAX); }
	| K_CUSTOM '(' ComposedString ',' ComposedString ',' ComposedString ','
			ComposedString ')'	{
				ILIntString prefix;
				prefix = SimpleNative(IL_META_NATIVETYPE_CUSTOMMARSHALER);
				$$ = ILInternAppendedString(prefix,
						ILInternAppendedString(PackString($3),
							ILInternAppendedString(PackString($5),
								ILInternAppendedString(PackString($7),
													   PackString($9)))));
			}
	| K_FIXED K_SYSSTRING '[' Integer32 ']'	{
				$$ = ILInternAppendedString
							(SimpleNative(IL_META_NATIVETYPE_FIXEDSYSSTRING),
							 PackLength($4));
			}
	| K_FIXED K_ARRAY '[' Integer32 ']'	{
				$$ = ILInternAppendedString
							(SimpleNative(IL_META_NATIVETYPE_FIXEDARRAY),
							 ILInternAppendedString
							 	(PackLength($4),
							 	 SimpleNative(IL_META_NATIVETYPE_MAX)));
			}
	| K_VARIANT				{ $$ = SimpleNative(IL_META_NATIVETYPE_VARIANT); }
	| K_CURRENCY			{ $$ = SimpleNative(IL_META_NATIVETYPE_CURRENCY); }
	| K_SYSCHAR				{ $$ = SimpleNative(IL_META_NATIVETYPE_SYSCHAR); }
	| K_VOID				{ $$ = SimpleNative(IL_META_NATIVETYPE_VOID); }
	| K_BOOL				{ $$ = SimpleNative(IL_META_NATIVETYPE_BOOLEAN); }
	| K_INT8				{ $$ = SimpleNative(IL_META_NATIVETYPE_I1); }
	| K_INT16				{ $$ = SimpleNative(IL_META_NATIVETYPE_I2); }
	| K_INT32				{ $$ = SimpleNative(IL_META_NATIVETYPE_I4); }
	| K_INT64				{ $$ = SimpleNative(IL_META_NATIVETYPE_I8); }
	| K_FLOAT32				{ $$ = SimpleNative(IL_META_NATIVETYPE_R4); }
	| K_FLOAT64				{ $$ = SimpleNative(IL_META_NATIVETYPE_R8); }
	| K_ERROR				{ $$ = SimpleNative(IL_META_NATIVETYPE_ERROR); }
	| K_UNSIGNED K_INT8		{ $$ = SimpleNative(IL_META_NATIVETYPE_U1); }
	| K_UNSIGNED K_INT16	{ $$ = SimpleNative(IL_META_NATIVETYPE_U2); }
	| K_UNSIGNED K_INT32	{ $$ = SimpleNative(IL_META_NATIVETYPE_U4); }
	| K_UNSIGNED K_INT64	{ $$ = SimpleNative(IL_META_NATIVETYPE_U8); }
	| NativeType '*'	{
				$$ = ILInternAppendedString
							(SimpleNative(IL_META_NATIVETYPE_PTR), $1);
			}
	| NativeType '[' ']'	{
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength(0),
								ILInternAppendedString(PackLength(0),
													   PackLength(0)))));
			}
	| NativeType '[' Integer32 ']'	{
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength(0),
								ILInternAppendedString(PackLength(0),
													   PackLength($3)))));
			}
	| NativeType '[' D_SIZE D_PARAM '=' Integer32 ']'	{
				/* Old format */
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength($6),
								ILInternAppendedString(PackLength(1),
													   PackLength(0)))));
			}
	| NativeType '[' '+' Integer32 ']'	{
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength($4),
								ILInternAppendedString(PackLength(1),
													   PackLength(0)))));
			}
	| NativeType '[' D_SIZE D_PARAM '=' Integer32 '*' Integer32 ']'	{
				/* Old format */
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength($6),
								ILInternAppendedString(PackLength($8),
													   PackLength(0)))));
			}
	| NativeType '[' Integer32 '+' Integer32 ']'	{
				$$ = ILInternAppendedString
						(SimpleNative(IL_META_NATIVETYPE_ARRAY),
						 ILInternAppendedString($1,
						 	ILInternAppendedString(PackLength($5),
								ILInternAppendedString(PackLength(1),
													   PackLength($3)))));
			}
	| K_DECIMAL				{ $$ = SimpleNative(IL_META_NATIVETYPE_DECIMAL); }
	| K_DATE				{ $$ = SimpleNative(IL_META_NATIVETYPE_DATE); }
	| K_BSTR				{ $$ = SimpleNative(IL_META_NATIVETYPE_BSTR); }
	| K_LPSTR				{ $$ = SimpleNative(IL_META_NATIVETYPE_LPSTR); }
	| K_LPWSTR				{ $$ = SimpleNative(IL_META_NATIVETYPE_LPWSTR); }
	| K_LPTSTR				{ $$ = SimpleNative(IL_META_NATIVETYPE_LPTSTR); }
	| K_OBJECTREF			{ $$ = SimpleNative(IL_META_NATIVETYPE_OBJECTREF); }
	| K_IUNKNOWN			{ $$ = SimpleNative(IL_META_NATIVETYPE_IUNKNOWN); }
	| K_IDISPATCH			{ $$ = SimpleNative(IL_META_NATIVETYPE_IDISPATCH); }
	| K_STRUCT				{ $$ = SimpleNative(IL_META_NATIVETYPE_STRUCT); }
	| K_INTERFACE			{ $$ = SimpleNative(IL_META_NATIVETYPE_INTF); }
	| K_SAFEARRAY VariantType	{
				$$ = ILInternAppendedString
							(SimpleNative(IL_META_NATIVETYPE_SAFEARRAY),
							 PackLength($2));
			}
	| K_INT					{ $$ = SimpleNative(IL_META_NATIVETYPE_INT); }
	| K_UNSIGNED K_INT		{ $$ = SimpleNative(IL_META_NATIVETYPE_UINT); }
	| K_NESTED K_STRUCT	{ $$ = SimpleNative(IL_META_NATIVETYPE_NESTEDSTRUCT); }
	| K_BYVALSTR			{ $$ = SimpleNative(IL_META_NATIVETYPE_BYVALSTR); }
	| K_ANSI K_BSTR			{ $$ = SimpleNative(IL_META_NATIVETYPE_ANSIBSTR); }
	| K_TBSTR				{ $$ = SimpleNative(IL_META_NATIVETYPE_TBSTR); }
	| K_VARIANT K_BOOL	{ $$ = SimpleNative(IL_META_NATIVETYPE_VARIANTBOOL); }
	| K_METHOD				{ $$ = SimpleNative(IL_META_NATIVETYPE_FUNC); }
	| K_LPVOID				{ $$ = SimpleNative(IL_META_NATIVETYPE_LPVOID); }
	| K_AS K_ANY			{ $$ = SimpleNative(IL_META_NATIVETYPE_ASANY); }
	| K_FLOAT				{ $$ = SimpleNative(IL_META_NATIVETYPE_R); }
	| K_LPSTRUCT			{ $$ = SimpleNative(IL_META_NATIVETYPE_LPSTRUCT); }
	;

VariantType
	: /* empty */			{ $$ = IL_META_VARIANTTYPE_EMPTY; }
	| K_NULL				{ $$ = IL_META_VARIANTTYPE_NULL; }
	| K_VARIANT				{ $$ = IL_META_VARIANTTYPE_VARIANT; }
	| K_CURRENCY			{ $$ = IL_META_VARIANTTYPE_CY; }
	| K_VOID				{ $$ = IL_META_VARIANTTYPE_VOID; }
	| K_BOOL				{ $$ = IL_META_VARIANTTYPE_BOOL; }
	| K_INT8				{ $$ = IL_META_VARIANTTYPE_I1; }
	| K_INT16				{ $$ = IL_META_VARIANTTYPE_I2; }
	| K_INT32				{ $$ = IL_META_VARIANTTYPE_I4; }
	| K_INT64				{ $$ = IL_META_VARIANTTYPE_I8; }
	| K_FLOAT32				{ $$ = IL_META_VARIANTTYPE_R4; }
	| K_FLOAT64				{ $$ = IL_META_VARIANTTYPE_R8; }
	| K_UNSIGNED K_INT8		{ $$ = IL_META_VARIANTTYPE_UI1; }
	| K_UNSIGNED K_INT16	{ $$ = IL_META_VARIANTTYPE_UI2; }
	| K_UNSIGNED K_INT32	{ $$ = IL_META_VARIANTTYPE_UI4; }
	| K_UNSIGNED K_INT64	{ $$ = IL_META_VARIANTTYPE_UI8; }
	| VariantType '[' ']'	{ $$ = IL_META_VARIANTTYPE_ARRAY | $1; }
	| VariantType K_VECTOR	{ $$ = IL_META_VARIANTTYPE_VECTOR | $1; }
	| VariantType '&'		{ $$ = IL_META_VARIANTTYPE_BYREF | $1; }
	| K_DECIMAL				{ $$ = IL_META_VARIANTTYPE_DECIMAL; }
	| K_DATE				{ $$ = IL_META_VARIANTTYPE_DATE; }
	| K_BSTR				{ $$ = IL_META_VARIANTTYPE_BSTR; }
	| K_LPSTR				{ $$ = IL_META_VARIANTTYPE_LPSTR; }
	| K_LPWSTR				{ $$ = IL_META_VARIANTTYPE_LPWSTR; }
	| K_IUNKNOWN			{ $$ = IL_META_VARIANTTYPE_UNKNOWN; }
	| K_IDISPATCH			{ $$ = IL_META_VARIANTTYPE_DISPATCH; }
	| K_SAFEARRAY			{ $$ = IL_META_VARIANTTYPE_SAFEARRAY; }
	| K_INT					{ $$ = IL_META_VARIANTTYPE_INT; }
	| K_UNSIGNED K_INT		{ $$ = IL_META_VARIANTTYPE_UINT; }
	| K_ERROR				{ $$ = IL_META_VARIANTTYPE_ERROR; }
	| K_HRESULT				{ $$ = IL_META_VARIANTTYPE_HRESULT; }
	| K_CARRAY				{ $$ = IL_META_VARIANTTYPE_CARRAY; }
	| K_USERDEFINED			{ $$ = IL_META_VARIANTTYPE_USERDEFINED; }
	| K_RECORD				{ $$ = IL_META_VARIANTTYPE_RECORD; }
	| K_FILETIME			{ $$ = IL_META_VARIANTTYPE_FILETIME; }
	| K_BLOB				{ $$ = IL_META_VARIANTTYPE_BLOB; }
	| K_STREAM				{ $$ = IL_META_VARIANTTYPE_STREAM; }
	| K_STORAGE				{ $$ = IL_META_VARIANTTYPE_STORAGE; }
	| K_STREAMED_OBJECT		{ $$ = IL_META_VARIANTTYPE_STREAMED_OBJECT; }
	| K_STORED_OBJECT		{ $$ = IL_META_VARIANTTYPE_STORED_OBJECT; }
	| K_BLOB_OBJECT			{ $$ = IL_META_VARIANTTYPE_BLOB_OBJECT; }
	| K_CF					{ $$ = IL_META_VARIANTTYPE_CF; }
	| K_CLSID				{ $$ = IL_META_VARIANTTYPE_CLSID; }
	;

/*
 * Signatures.
 */

OptSignatureArguments
	: /* empty */			{ $$.paramFirst = 0; $$.paramLast = 0; }
	| SignatureArguments	{ $$ = $1; }
	;

SignatureArguments
	: SignatureArgument		{ $$ = $1; }
	| SignatureArguments ',' SignatureArgument	{
				$1.paramLast->next = $3.paramFirst;
				$$.paramFirst = $1.paramFirst;
				$$.paramLast = $3.paramLast;
			}
	;

SignatureArgument
	: DOT_DOT_DOT	{
				ILAsmParamInfo *param;
				param = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!param)
				{
					ILAsmOutOfMemory();
				}
				param->type = 0;
				param->nativeType.string = "";
				param->nativeType.len = 0;
				param->parameterAttrs = 0;
				param->name = 0;
				param->firstTypeConstraint = 0;
				param->next = 0;
				$$.paramFirst = param;
				$$.paramLast = param;
			}
	| ParameterAttributes MarshalledType	{
				ILAsmParamInfo *param;
				param = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!param)
				{
					ILAsmOutOfMemory();
				}
				param->type = $2.type;
				param->nativeType = $2.nativeType;
				param->parameterAttrs = $1;
				param->name = 0;
				param->firstTypeConstraint = 0;
				param->next = 0;
				$$.paramFirst = param;
				$$.paramLast = param;
			}
	| ParameterAttributes MarshalledType Identifier		{
				ILAsmParamInfo *param;
				param = (ILAsmParamInfo *)ILMalloc(sizeof(ILAsmParamInfo));
				if(!param)
				{
					ILAsmOutOfMemory();
				}
				param->type = $2.type;
				param->nativeType = $2.nativeType;
				param->parameterAttrs = $1;
				param->name = $3.string;
				param->firstTypeConstraint = 0;
				param->next = 0;
				$$.paramFirst = param;
				$$.paramLast = param;
			}
	;

ParameterAttributes
	: /* empty */				{ $$ = 0; }
	| ParameterAttributeList	{ $$ = $1; }
	;

ParameterAttributeList
	: '[' ParameterAttributeName ']'	{ $$ = $2; }
	| ParameterAttributeList '[' ParameterAttributeName ']'	{ $$ = $1 | $3; }
	;

ParameterAttributeName
	: K_IN			{ $$ = IL_META_PARAMDEF_IN; }
	| K_OUT			{ $$ = IL_META_PARAMDEF_OUT; }
	| K_OPT			{ $$ = IL_META_PARAMDEF_OPTIONAL; }
	| K_RETVAL		{ $$ = IL_META_PARAMDEF_RETVAL; }
	| Integer32		{ $$ = $1; }
	;

/*
 * Calling conventions.
 */

CallingConventions
	: /* empty */					{ $$ = IL_META_CALLCONV_DEFAULT; }
	| K_INSTANCE CallingConventions	{ $$ = $2 | IL_META_CALLCONV_HASTHIS; }
	| K_EXPLICIT CallingConventions	{ $$ = $2 | IL_META_CALLCONV_EXPLICITTHIS; }
	| K_DEFAULT						{ $$ = IL_META_CALLCONV_DEFAULT; }
	| K_VARARG						{ $$ = IL_META_CALLCONV_VARARG; }
	| K_UNMANAGED K_CDECL			{ $$ = IL_META_CALLCONV_C; }
	| K_UNMANAGED K_STDCALL			{ $$ = IL_META_CALLCONV_STDCALL; }
	| K_UNMANAGED K_THISCALL		{ $$ = IL_META_CALLCONV_THISCALL; }
	| K_UNMANAGED K_FASTCALL		{ $$ = IL_META_CALLCONV_FASTCALL; }
	;

/*
 * Debugging stuff.
 */

ExternalSourceSpecification
	: D_LINE INTEGER_CONSTANT SQUOTE_STRING	{
				ILAsmDebugLine((ILUInt32)($2), 0,
							   ILInternString($3, -1).string);
			}
	| D_LINE INTEGER_CONSTANT ':' INTEGER_CONSTANT SQUOTE_STRING	{
				ILAsmDebugLine((ILUInt32)($2), (ILUInt32)($4),
							   ILInternString($5, -1).string);
			}
	| D_LINE INTEGER_CONSTANT DQUOTE_STRING	{
				ILAsmDebugLine((ILUInt32)($2), 0,
							   ILInternString($3, -1).string);
			}
	| D_LINE INTEGER_CONSTANT ':' INTEGER_CONSTANT DQUOTE_STRING	{
				ILAsmDebugLine((ILUInt32)($2), (ILUInt32)($4),
							   ILInternString($5, -1).string);
			}
	| D_LINE INTEGER_CONSTANT	{
				ILAsmDebugLine((ILUInt32)($2), 0, ILAsmDebugLastFile);
			}
	| D_LINE INTEGER_CONSTANT ':' INTEGER_CONSTANT	{
				ILAsmDebugLine((ILUInt32)($2), (ILUInt32)($4),
							   ILAsmDebugLastFile);
			}
	;

FileDeclaration
	: D_FILE FileAttributes QualifiedName D_HASH '=' Bytes	{
				/* Create the file declaration and attach the hash */
				ILFileDecl *decl = ILFileDeclCreate(ILAsmImage, 0,
													$3.string,
													(ILUInt32)($2));
				if(!decl)
				{
					ILAsmOutOfMemory();
				}
				if(!ILFileDeclSetHash(decl, $6.string, $6.len))
				{
					ILAsmOutOfMemory();
				}
				ILAsmLastToken = ILFileDecl_Token(decl);
			}
	| D_FILE FileAttributes QualifiedName	{
				/* Create the file declaration */
				ILFileDecl *decl = ILFileDeclCreate(ILAsmImage, 0,
													$3.string,
													(ILUInt32)($2));
				if(!decl)
				{
					ILAsmOutOfMemory();
				}
				ILAsmLastToken = ILFileDecl_Token(decl);
			}
	;

FileAttributes
	: /* empty */		{ $$ = IL_META_FILE_WRITEABLE; }
	| FileAttributeList	{ $$ = $1 ^ IL_META_FILE_WRITEABLE; }
	;

FileAttributeList
	: FileAttributeName						{ $$ = $1; }
	| FileAttributeList FileAttributeName	{ $$ = $1 | $2; }
	;

FileAttributeName
	: K_READONLY		{ $$ = IL_META_FILE_WRITEABLE; /* invert later */ }
	| K_NOMETADATA		{ $$ = IL_META_FILE_CONTAINS_NO_META_DATA; }
	;

ExeLocationDeclaration
	: D_EXELOC QualifiedName '(' ComposedString ')' K_AT ComposedString
	| D_EXELOC QualifiedName K_AT ComposedString
	;

/*
 * Assemblies.
 */

AssemblyHeading
	: D_ASSEMBLY AssemblyAttributes AssemblyName AltName	{
				ILAssemblySetAttrs(ILAsmAssembly, ~0, (ILUInt32)($2));
				if(!ILAssemblySetName(ILAsmAssembly, $3.string))
				{
					ILAsmOutOfMemory();
				}
				ILAsmBuildPushScope(ILAsmAssembly);
				ILAsmCurrAssemblyRef = 0;
			}
	;

AssemblyName
	: QualifiedName		{
				if(!strcmp($1.string, ".library"))
				{
					$$ = ILInternString(ILAsmLibraryName, -1);
				}
				else
				{
					$$ = $1;
				}
			}
	| D_LIBRARY			{
				$$ = ILInternString(ILAsmLibraryName, -1);
			}
	;

/* Obsolete syntax */
AltName
	: /* empty */
	| K_AS ComposedString
	;

AssemblyAttributes
	: /* empty */				{ $$ = 0; }
	| AssemblyAttributeList		{ $$ = $1; }
	;

AssemblyAttributeList
	: AssemblyAttributeName							{ $$ = $1; }
	| AssemblyAttributeList AssemblyAttributeName	{ $$ = $1 | $2; }
	;

AssemblyAttributeName
	: K_PUBLICKEY			{ $$ = IL_META_ASSEM_PUBLIC_KEY; }
	| K_NOAPPDOMAIN			{ $$ = IL_META_ASSEM_NON_SIDE_BY_SIDE_APP_DOMAIN; }
	| K_NOPROCESS			{ $$ = IL_META_ASSEM_NON_SIDE_BY_SIDE_PROCESS; }
	| K_NOMACHINE			{ $$ = IL_META_ASSEM_NON_SIDE_BY_SIDE_MACHINE; }
	| K_RETARGETABLE		{ $$ = IL_META_ASSEM_RETARGETABLE; }
	| K_ENABLEJITTRACKING	{ $$ = IL_META_ASSEM_ENABLE_JIT_TRACKING; }
	| K_DISABLEJITOPTIMIZER	{ $$ = IL_META_ASSEM_DISABLE_JIT_OPTIMIZER; }
	;

AssemblyDeclarations
	: /* empty */				{ ILAsmBuildPopScope(); }
	| AssemblyDeclarationList	{ ILAsmBuildPopScope(); }
	;

AssemblyDeclarationList
	: AssemblyDeclaration
	| AssemblyDeclarationList AssemblyDeclaration
	;

AssemblyDeclaration
	: D_TITLE ComposedString '(' ComposedString ')'	{ /* Obsolete */ }
	| D_TITLE ComposedString						{ /* Obsolete */ }
	| D_HASH K_ALGORITHM Integer32	{
				ILAssemblySetHashAlgorithm(ILAsmAssembly, $3);
			}
	| D_HASH D_ALGORITHM Integer32	{
				ILAssemblySetHashAlgorithm(ILAsmAssembly, $3);
			}
	| SecurityDeclaration
	| AsmOrRefDeclaration
	;

AsmOrRefDeclaration
	: D_ORIGINATOR '=' Bytes	{
					SetOriginator($3.string, $3.len, 1);
				}
	| D_PUBLICKEY '=' Bytes		{
					SetOriginator($3.string, $3.len, 1);
				}
	| D_PUBLICKEYTOKEN '=' Bytes	{
					SetOriginator($3.string, $3.len, 0);
				}
	| D_VER Integer32 ':' Integer32 ':' Integer32 ':' Integer32	{
				/* Set the assembly version */
				if(ILAsmCurrAssemblyRef)
				{
					ILAssemblySetVersionSplit
						(ILAsmCurrAssemblyRef,
						 (ILUInt32)($2), (ILUInt32)($4),
						 (ILUInt32)($6), (ILUInt32)($8));
				}
				else
				{
					ILAssemblySetVersionSplit
						(ILAsmAssembly,
						 (ILUInt32)($2), (ILUInt32)($4),
						 (ILUInt32)($6), (ILUInt32)($8));
				}
			}
	| D_LOCALE ComposedString	{
				/* Set the assembly locale */
				if(ILAsmCurrAssemblyRef)
				{
					if(!ILAssemblySetLocale(ILAsmCurrAssemblyRef, $2.string))
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					if(!ILAssemblySetLocale(ILAsmAssembly, $2.string))
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| D_LOCALE '=' Bytes	{
				/* Set the assembly locale to a byte list */
				if(ILAsmCurrAssemblyRef)
				{
					if(!ILAssemblySetLocale(ILAsmCurrAssemblyRef, $3.string))
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					if(!ILAssemblySetLocale(ILAsmAssembly, $3.string))
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| D_PROCESSOR Integer32	{
				/* Add a processor definition to the assembly */
				if(ILAsmCurrAssemblyRef)
				{
					if(!ILProcessorInfoCreate(ILAsmImage, 0, (ILUInt32)($2),
							 				  ILAsmCurrAssemblyRef))
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					if(!ILProcessorInfoCreate(ILAsmImage, 0, (ILUInt32)($2),
											  ILAsmAssembly))
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| D_OS Integer32 D_VER Integer32 ':' Integer32	{
				/* Add an OS definition to the assembly */
				if(ILAsmCurrAssemblyRef)
				{
					if(!ILOSInfoCreate(ILAsmImage, 0, (ILUInt32)($2),
							 (ILUInt32)($4), (ILUInt32)($6),
							 ILAsmCurrAssemblyRef))
					{
						ILAsmOutOfMemory();
					}
				}
				else
				{
					if(!ILOSInfoCreate(ILAsmImage, 0, (ILUInt32)($2),
									   (ILUInt32)($4), (ILUInt32)($6),
									   ILAsmAssembly))
					{
						ILAsmOutOfMemory();
					}
				}
			}
	| D_CONFIG ComposedString	{ /* Obsolete */ }
	| D_CONFIG '=' Bytes		{ /* Obsolete */ }
	| CustomAttributeDeclaration
	;

AssemblyRefHeading
	: D_ASSEMBLY K_EXTERN AssemblyRefAttributes AssemblyName
			K_AS ComposedString	{
				ILAssembly *assem;
				assem = ILAssemblyCreate(ILAsmImage, 0, $4.string, 1);
				if(!assem)
				{
					ILAsmOutOfMemory();
				}
				ILAssemblySetRefAttrs(assem, ~0, (ILUInt32)($3));
				ILAsmBuildPushScope(assem);
				ILAsmCurrAssemblyRef = assem;
			}
	| D_ASSEMBLY K_EXTERN AssemblyRefAttributes AssemblyName	{
				ILAssembly *assem;
				assem = ILAssemblyCreate(ILAsmImage, 0, $4.string, 1);
				if(!assem)
				{
					ILAsmOutOfMemory();
				}
				ILAssemblySetRefAttrs(assem, ~0, (ILUInt32)($3));
				ILAsmBuildPushScope(assem);
				ILAsmCurrAssemblyRef = assem;
			}
	;

AssemblyRefAttributes
	: /* empty */					{ $$ = 0; }
	| AssemblyRefAttributeList		{ $$ = $1; }
	;

AssemblyRefAttributeList
	: AssemblyRefAttributeName								{ $$ = $1; }
	| AssemblyRefAttributeList AssemblyRefAttributeName		{ $$ = $1 | $2; }
	;

AssemblyRefAttributeName
	: K_FULLORIGIN			{ $$ = IL_META_ASSEMREF_FULL_ORIGINATOR; }
	;

AssemblyRefDeclarations
	: /* empty */					{ ILAsmBuildPopScope(); }
	| AssemblyRefDeclarationList	{ ILAsmBuildPopScope(); }
	;

AssemblyRefDeclarationList
	: AssemblyRefDeclaration
	| AssemblyRefDeclarationList AssemblyRefDeclaration
	;

AssemblyRefDeclaration
	: D_HASH '=' Bytes	{
				/* Set the hash value for the assembly reference */
				if(!ILAssemblySetHash(ILAsmCurrAssemblyRef,
									  $3.string, $3.len))
				{
					ILAsmOutOfMemory();
				}
			}
	| D_EXELOC QualifiedName
	| AsmOrRefDeclaration
	;

/*
 * Exported type definitions.
 */

ExportHeading
	: ExportKeyword ComTypeAttributes QualifiedName ExportAsName	{
				/* Create the exported type */
				ILExportedType *type;
				const char *name;
				const char *namespace;
				ILAsmSplitName($3.string, $3.len, &name, &namespace);
				type = ILExportedTypeCreate(ILAsmImage, 0, (ILUInt32)($2),
											name, namespace, 0);
				ILAsmBuildPushScope(type);
			}
	;

ExportKeyword
	: D_CLASS K_EXTERN
	| D_EXPORT
	| D_COMTYPE
	;

ExportAsName	/* obsolete */
	: /* empty */
	| '(' ComposedString ')'
	;

ComTypeAttributes
	: /* empty */				{ $$ = 0; }
	| ComTypeAttributeList		{ $$ = $1; }
	;

ComTypeAttributeList
	: ComTypeAttributeName							{ $$ = $1; }
	| ComTypeAttributeList ComTypeAttributeName		{ $$ = $1 | $2; }
	;

ComTypeAttributeName
	: K_PRIVATE					{ $$ = IL_META_TYPEDEF_NOT_PUBLIC; }
	| K_PUBLIC					{ $$ = IL_META_TYPEDEF_PUBLIC; }
	| K_NESTED K_PUBLIC			{ $$ = IL_META_TYPEDEF_NESTED_PUBLIC; }
	| K_NESTED K_PRIVATE		{ $$ = IL_META_TYPEDEF_NESTED_PRIVATE; }
	| K_NESTED K_FAMILY			{ $$ = IL_META_TYPEDEF_NESTED_FAMILY; }
	| K_NESTED K_ASSEMBLY		{ $$ = IL_META_TYPEDEF_NESTED_ASSEMBLY; }
	| K_NESTED K_FAMANDASSEM	{ $$ = IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM; }
	| K_NESTED K_FAMORASSEM		{ $$ = IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM; }
	;

ComTypeDeclarations
	: /* empty */				{ ILAsmBuildPopScope(); }
	| ComTypeDeclarationList	{ ILAsmBuildPopScope(); }
	;

ComTypeDeclarationList
	: ComTypeDeclaration
	| ComTypeDeclarationList ComTypeDeclaration
	;

ComTypeDeclaration
	: D_FILE QualifiedName	{
				/* Set the exported type's scope to a file */
				ILExportedType *type =
					ILProgramItemToExportedType
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				ILFileDecl *decl = ILAsmFindFile($2.string,
												 IL_META_FILE_WRITEABLE, 1);
				if(type && decl)
				{
					ILExportedTypeSetScopeFile(type, decl);
				}
			}
	| D_ASSEMBLY K_EXTERN QualifiedName	{
				/* Set the exported type's scope to an assembly reference */
				ILExportedType *type =
					ILProgramItemToExportedType
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				ILAssembly *assem = ILAsmFindAssemblyRef($3.string);
				if(type && assem)
				{
					ILExportedTypeSetScopeAssembly(type, assem);
				}
			}
	| D_COMTYPE QualifiedName	{
				/* Set the exported type's scope to another exported type */
				const char *name;
				const char *namespace;
				ILExportedType *type =
					ILProgramItemToExportedType
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				ILExportedType *expType;
				ILAsmSplitName($2.string, $2.len, &name, &namespace);
				expType = ILExportedTypeFind(ILAsmImage, name, namespace);
				if(!expType)
				{
					ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
									  "exported type `%s' not declared",
									  $2.string);
				}
				if(type && expType)
				{
					ILExportedTypeSetScopeType(type, expType);
				}
			}
	| D_CLASS Integer32	{
				/* Set the foreign class identifier */
				ILExportedType *type =
					ILProgramItemToExportedType
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				if(type)
				{
					ILExportedTypeSetId(type, (ILUInt32)($2));
				}
			}
	| D_EXELOC QualifiedName		/* obsolete */
	| CustomAttributeDeclaration
	;

/*
 * Manifest resources.
 */

ManifestResHeading
	: ManifestResKeyword ManifestResAttributes QualifiedName
			ManifestResDescription {
				/* Create the manifest resource */
				ILManifestRes *res;
				res = ILManifestResCreate(ILAsmImage, 0,
										  $3.string, (ILUInt32)($2), 0);
				if(!res)
				{
					ILAsmOutOfMemory();
				}
				ILAsmBuildPushScope(res);
			}
	;

ManifestResKeyword
	: D_MANIFESTRES		{ /* Old name */ }
	| D_MRESOURCE		{ /* New name */ }
	;

ManifestResDescription	/* obsolete */
	: /* empty */
	| '(' ComposedString ')'
	;

ManifestResAttributes
	: /* empty */					{ $$ = 0; }
	| ManifestResAttributeList		{ $$ = $1; }
	;

ManifestResAttributeList
	: ManifestResAttributeName								{ $$ = $1; }
	| ManifestResAttributeList ManifestResAttributeName		{ $$ = $1 | $2; }
	;

ManifestResAttributeName
	: K_PUBLIC				{ $$ = IL_META_MANIFEST_PUBLIC; }
	| K_PRIVATE				{ $$ = IL_META_MANIFEST_PRIVATE; }
	;

ManifestResDeclarations
	: /* empty */					{ ILAsmBuildPopScope(); }
	| ManifestResDeclarationList	{ ILAsmBuildPopScope(); }
	;

ManifestResDeclarationList
	: ManifestResDeclaration
	| ManifestResDeclarationList ManifestResDeclaration
	;

ManifestResDeclaration
	: D_MIME ComposedString			/* obsolete */
	| D_MIME '=' Bytes				/* obsolete */
	| D_FILE QualifiedName K_AT Integer32	{
				/* Set the resource owner to a file */
				ILManifestRes *res =
					ILProgramItemToManifestRes
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				ILFileDecl *decl = ILAsmFindFile($2.string,
												 IL_META_FILE_WRITEABLE, 1);
				if(res && decl)
				{
					ILManifestResSetOwnerFile(res, decl, (ILUInt32)($4));
				}
			}
	| D_ASSEMBLY K_EXTERN QualifiedName	{
				/* Set the resource owner to an assembly reference */
				ILManifestRes *res =
					ILProgramItemToManifestRes
						(ILProgramItem_FromToken(ILAsmImage, ILAsmLastToken));
				ILAssembly *assem = ILAsmFindAssemblyRef($3.string);
				if(res && assem)
				{
					ILManifestResSetOwnerAssembly(res, assem);
				}
			}
	| D_LOCALE '=' Bytes			/* obsolete */
	| CustomAttributeDeclaration
	;

/*
 * Security declarations.
 */

SecurityDeclaration
	: D_PERMISSION SecurityAction ClassName '(' NameValuePairs ')'
	| D_CAPABILITY SecurityAction SQUOTE_STRING	{
				ILIntString unicode = PackUnicodeString(ILAsmParseString($3));
				ILAsmSecurityCreate($2, unicode.string, unicode.len);
			}
	| D_CAPABILITY SecurityAction '=' Bytes		{
				ILAsmSecurityCreate($2, $4.string, $4.len);
			}
	| D_PERMISSIONSET SecurityAction SQUOTE_STRING	{
				ILIntString unicode = PackUnicodeString(ILAsmParseString($3));
				ILAsmSecurityCreate($2, unicode.string, unicode.len);
			}
	| D_PERMISSIONSET SecurityAction '=' Bytes	{
				ILAsmSecurityCreate($2, $4.string, $4.len);
			}
	;

SecurityAction
	: K_REQUEST					{ $$ = IL_META_SECURITY_REQUEST; }
	| K_DEMAND					{ $$ = IL_META_SECURITY_DEMAND; }
	| K_ASSERT					{ $$ = IL_META_SECURITY_ASSERT; }
	| K_DENY					{ $$ = IL_META_SECURITY_DENY; }
	| K_PERMITONLY				{ $$ = IL_META_SECURITY_PERMIT_ONLY; }
	| K_LINKCHECK				{ $$ = IL_META_SECURITY_LINK_TIME_CHECK; }
	| K_INHERITCHECK			{ $$ = IL_META_SECURITY_INHERITANCE_CHECK; }
	| K_REQMIN					{ $$ = IL_META_SECURITY_REQUEST_MINIMUM; }
	| K_REQOPT					{ $$ = IL_META_SECURITY_REQUEST_OPTIONAL; }
	| K_REQREFUSE				{ $$ = IL_META_SECURITY_REQUEST_REFUSE; }
	| K_PREJITGRANT				{ $$ = IL_META_SECURITY_PREJIT_GRANT; }
	| K_PREJITDENY				{ $$ = IL_META_SECURITY_PREJIT_DENIED; }
	| K_NONCASDEMAND			{ $$ = IL_META_SECURITY_NON_CAS_DEMAND; }
	| K_NONCASLINKDEMAND		{ $$ = IL_META_SECURITY_NON_CAS_LINK_DEMAND; }
	| K_NONCASINHERITANCE		{ $$ = IL_META_SECURITY_NON_CAS_INHERITANCE; }
	;

NameValuePairs
	: NameValuePair
	| NameValuePair ',' NameValuePairs
	;

NameValuePair
	: SQUOTE_STRING '=' SQUOTE_STRING	{}
	;

/*
 * Custom attributes.
 */

CustomAttributeDeclaration
	: D_CUSTOM CustomType	 { ILAsmAttributeCreate($2, 0); }
	| D_CUSTOM CustomType '=' ComposedString {
				ILAsmAttributeCreate($2, &($4));
			}
	| D_CUSTOM CustomType '=' Bytes	{
				ILAsmAttributeCreate($2, &($4));
			}
	| D_CUSTOM '(' CustomOwner ')' CustomType '=' Bytes	{
				ILAsmAttributeCreateFor($3, $5, &($7));
			}
	| D_CUSTOM '(' CustomOwner ')' CustomType '=' ComposedString	{
				ILAsmAttributeCreateFor($3, $5, &($7));
			}
	| D_CUSTOM '(' CustomOwner ')' CustomType {
				ILAsmAttributeCreateFor($3, $5, 0);
			}
	| LanguageDeclaration	{
				/* Nothing to do here: ".language" is ignored */
			}
	;

CustomType
	: MethodReference   { $$ = ILProgramItem_FromToken(ILAsmImage, $1); }
	;

CustomOwner
	: TypeSpecification				{ $$ = ILProgramItem_Token($1.item); }
	| K_METHOD MethodReference		{ $$ = $2; }
	| K_FIELD Type TypeSpecification COLON_COLON Identifier	{
				$$ = ILAsmResolveMember($3.item, $5.string, $2,
								        IL_META_MEMBERKIND_FIELD);
			}
	| K_FIELD Type Identifier	{
				$$ = ILAsmResolveMember(ILToProgramItem(ILAsmClass),
										$3.string, $2,
										IL_META_MEMBERKIND_FIELD);
			}
	;

/*
 * Instructions.
 */

Instruction
	: I_NONE				{ ILAsmOutSimple($1); }
	| I_VAR Integer32		{ ILAsmOutVar($1, $2); }
	| I_VAR Identifier		{ ILAsmOutVar($1, ILAsmOutLookupVar($2.string)); }
	| I_INT Integer64		{ ILAsmOutInt($1, $2); }
	| I_FLOAT InstructionFloat {
				/* Determine what form of floating point value to output */
				if($1 == IL_OP_LDC_R4)
				{
					ILAsmOutFloat($2.fbytes);
				}
				else
				{
					ILAsmOutDouble($2.dbytes);
				}
			}
	| I_BRANCH Integer32		{ ILAsmOutBranchInt($1, $2); }
	| I_BRANCH Identifier		{ ILAsmOutBranch($1, $2.string); }
	| I_METHOD GenericMethodReference	{ ILAsmOutToken($1, $2); }
	| I_IMETHOD InstanceMethodReference { ILAsmOutToken($1, $2); }
	| I_FIELD Type TypeSpecification COLON_COLON Identifier	{
				/* Refer to a field in some other class */
				ILToken token = ILAsmResolveMember($3.item, $5.string, $2,
											       IL_META_MEMBERKIND_FIELD);
				ILAsmOutToken($1, token);
			}
	| I_FIELD Type Identifier	{
				/* Refer to a field in the global module class */
				ILToken token = ILAsmResolveMember
						(ILToProgramItem(ILAsmModuleClass),
					     $3.string, $2,
					     IL_META_MEMBERKIND_FIELD);
				ILAsmOutToken($1, token);
			}
	| I_TYPE TypeSpecification	{
				if($2.item != 0)
				{
					ILAsmOutToken($1, ILProgramItem_Token($2.item));
				}
				else
				{
					ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
									  "no token for type specification");
					ILAsmErrors = 1;
				}
			}
	| I_STRING ComposedString		{ ILAsmOutString($2); }
	| I_STRING K_BYTEARRAY Bytes	{ ILAsmOutString($3); }
	| I_SIGNATURE CallingConventions Type '(' OptSignatureArguments ')'	{
				ILType *sig = CreateMethodSig($2, $3, $5.paramFirst, 0, 1);
				ILStandAloneSig *stand =
					ILStandAloneSigCreate(ILAsmImage, 0, sig);
				if(!stand)
				{
					ILAsmOutOfMemory();
				}
				ILAsmOutToken($1, ILStandAloneSig_Token(stand));
			}
	| I_RVA Identifier	{
				ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
								  "unsafe RVA instructions are not supported");
				ILAsmErrors = 1;
			}
	| I_RVA Integer32	{
				ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
								  "unsafe RVA instructions are not supported");
				ILAsmErrors = 1;
			}
	| I_TOKEN K_METHOD MethodReference	{ ILAsmOutToken($1, $3); }
	| I_TOKEN K_FIELD Type TypeSpecification COLON_COLON Identifier	{
				/* Refer to a field in some other class */
				ILToken token = ILAsmResolveMember($4.item, $6.string, $3,
											       IL_META_MEMBERKIND_FIELD);
				ILAsmOutToken($1, token);
			}
	| I_TOKEN K_FIELD Type Identifier	{
				/* Refer to a field in the current class */
				ILToken token = ILAsmResolveMember(ILToProgramItem(ILAsmClass),
												   $4.string, $3,
											       IL_META_MEMBERKIND_FIELD);
				ILAsmOutToken($1, token);
			}
	| I_TOKEN TypeSpecification	{
				if($2.item != 0)
				{
					ILAsmOutToken($1, ILProgramItem_Token($2.item));
				}
				else
				{
					ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
									  "no token for type specification");
					ILAsmErrors = 1;
				}
			}
	| I_SSA 		{
				ILAsmOutSSAStart($1);
			}
	  Integer16List	{
				ILAsmOutSSAEnd();
			}
	| I_SWITCH '('		{
				ILAsmOutSwitchStart();
			}
	  SwitchLabels ')'	{
	  			ILAsmOutSwitchEnd();
			}
	;

InstructionFloat
	: Float64			{ $$ = $1; }
 	| Integer64			{
			#ifdef IL_CONFIG_FP_SUPPORTED
				/* Convert a 64-bit integer into a float */
				SetFloat($$.fbytes, (ILFloat)($1));
				SetDouble($$.dbytes, (ILDouble)($1));
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	| Bytes				{
			#ifdef IL_CONFIG_FP_SUPPORTED
				/* Convert a group of bytes into a float */
				if($1.len == 4)
				{
					/* Exact float supplied, so synthesize the double */
					ILMemCpy($$.fbytes, $1.string, 4);
					SetDouble($$.dbytes, (ILDouble)(IL_READ_FLOAT($$.fbytes)));
				}
				else if($1.len == 8)
				{
					/* Exact double supplied, so synthesize the float */
					ILMemCpy($$.dbytes, $1.string, 8);
					SetFloat($$.fbytes, (ILFloat)(IL_READ_DOUBLE($$.dbytes)));
				}
				else
				{
					/* Bad floating point value supplied */
					ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
									  "invalid floating point value");
					ILAsmErrors = 1;
					$$.fbytes[0] = 0;
					$$.fbytes[1] = 0;
					$$.fbytes[2] = 0;
					$$.fbytes[3] = 0;
					$$.dbytes[0] = 0;
					$$.dbytes[1] = 0;
					$$.dbytes[2] = 0;
					$$.dbytes[3] = 0;
					$$.dbytes[4] = 0;
					$$.dbytes[5] = 0;
					$$.dbytes[6] = 0;
					$$.dbytes[7] = 0;
				}
			#else	/* !IL_CONFIG_FP_SUPPORTED */
				yyerror("no floating point support on this system");
			#endif	/* IL_CONFIG_FP_SUPPORTED */
			}
	;

Integer16List
	: /* empty */
	| Integer16List2
	;

Integer16List2
	: Integer32					{ ILAsmOutSSAValue($1); }
	| Integer16List2 Integer32	{ ILAsmOutSSAValue($2); }
	;

SwitchLabels
	: /* empty */
	| SwitchLabelList
	;

SwitchLabelList
	: SwitchLabel
	| SwitchLabelList ',' SwitchLabel
	;

SwitchLabel
	: Identifier		{ ILAsmOutSwitchRef($1.string); }
	| Integer32			{ ILAsmOutSwitchRefInt($1); }
	;

JavaInstruction
	: I_NONE				{ ILJavaAsmOutSimple($1); }
	| I_VAR Integer32		{ ILJavaAsmOutVar($1, $2); }
	| I_IINC Integer32 Integer32 { ILJavaAsmOutInc($1, $2, $3); }
	| I_INT Integer64		{ ILJavaAsmOutInt($1, $2);	  }
	| I_CONST K_INT32 '(' Integer32 ')'		{ ILJavaAsmOutConstInt32($1, $4); }
	| I_CONST K_INT64 '(' Integer64 ')'		{ ILJavaAsmOutConstInt64($1, $4);}
	| I_CONST K_FLOAT32 '(' InstructionFloat ')'   { 
			ILJavaAsmOutConstFloat32($1, $4.fbytes);
		}
	| I_CONST K_FLOAT64 '(' InstructionFloat ')' { 
			ILJavaAsmOutConstFloat64($1, $4.dbytes);
		}
	| I_CONST ComposedString	{ ILJavaAsmOutString($2); }
	| I_BRANCH Integer32		{ ILAsmOutBranchInt($1, $2); }
	| I_BRANCH Identifier		{ ILAsmOutBranch($1, $2.string); }
	| I_METHOD MethodReference		{ ILJavaAsmOutToken($1, $2); }
	| I_METHOD ComposedString ComposedString ComposedString	{
			ILJavaAsmOutRef($1, 1, $2.string, $3.string, $4.string);
		}
	| I_FIELD Type TypeSpecification COLON_COLON Identifier	{
			/* Refer to a field in some other class */
			ILToken token = ILAsmResolveMember($3.item, $5.string, $2,
												   IL_META_MEMBERKIND_FIELD);
			ILJavaAsmOutToken($1, token);
		}
	| I_FIELD ComposedString ComposedString ComposedString	{
			ILJavaAsmOutRef($1, 0, $2.string, $3.string, $4.string);
		}
	| I_TYPE TypeSpecification	{
			if($2.item != 0)
			{
				ILJavaAsmOutToken($1, ILProgramItem_Token($2.item));
			}
			else
			{
				ILAsmPrintMessage(ILAsmFilename, ILAsmLineNum,
								  "no token for type specification");
				ILAsmErrors = 1;
			}
		}
	| I_TYPE ComposedString	{ ILJavaAsmOutType($1, $2.string); }
	| I_IMETHOD MethodReference Integer32		{ 
			/* FIXME: what is the meaning of the Integer32 node here ?? */
			ILJavaAsmOutToken($1, $2);
		}
	| I_IMETHOD ComposedString ComposedString ComposedString Integer32	{
			/* FIXME: what is the meaning of the Integer32 node here ?? */
			ILJavaAsmOutRef($1, 1, $2.string, $3.string, $4.string);
		}
	| I_NEWARRAY JavaArrayType	{ ILJavaAsmOutNewarray($1, $2); }
	| I_MULTINEWARRAY TypeSpecification Integer32	{
			ILJavaAsmOutMultinewarray($1, $2.type, $3);
		}
	| I_MULTINEWARRAY ComposedString Integer32	{ 
			ILJavaAsmOutMultinewarrayFromName($1, $2.string, $3);
		}
	| I_SWITCH TableSwitchDefaultLabel '(' Integer32 ':' {
			ILJavaAsmOutTableSwitchStart($4);
		} 
	  TableSwitchLabels ')'	{
			ILJavaAsmOutTableSwitchEnd($4);
		}
	| I_LSWITCH LookupSwitchDefaultLabel '(' { 
			ILJavaAsmOutLookupSwitchStart();
		}
	  LookupSwitchLabels ')' {
			ILJavaAsmOutLookupSwitchEnd();
		}
	;

JavaArrayType
	: K_BOOL	  { $$ = 4; }
	| K_CHAR	  { $$ = 5; }
	| K_FLOAT32	  { $$ = 6; }
	| K_FLOAT64	  { $$ = 7; }
	| K_INT8	  { $$ = 8; }
	| K_INT16	  { $$ = 9; }
	| K_INT32	  { $$ = 10; }
	| K_INT64	  { $$ = 11; }
	;

TableSwitchLabels
	: /* empty */
	| TableSwitchLabelList
	;

TableSwitchLabelList
	: TableSwitchLabel
	| TableSwitchLabelList ',' TableSwitchLabel
	;

TableSwitchDefaultLabel
	: Identifier	{ ILJavaAsmOutTableSwitchDefaultRef($1.string); }
	| Integer32		{ ILJavaAsmOutTableSwitchDefaultRefInt($1); }
	;

TableSwitchLabel
	: Identifier	{ ILJavaAsmOutTableSwitchRef($1.string); }
	| Integer32		{ ILJavaAsmOutTableSwitchRefInt($1); }
	;

LookupSwitchLabels
	: /* empty */
	| LookupSwitchLabelList
	;

LookupSwitchLabelList
	: LookupSwitchLabel
	| LookupSwitchLabelList ',' LookupSwitchLabel
	;

LookupSwitchDefaultLabel
	: Identifier	{ ILJavaAsmOutLookupSwitchDefaultRef($1.string); }
	| Integer32		{ ILJavaAsmOutLookupSwitchDefaultRefInt($1); }
	;

LookupSwitchLabel
	: Integer32 ':' Identifier	{ ILJavaAsmOutLookupSwitchRef($1, $3.string); }
	| Integer32 ':' Integer32	{ ILJavaAsmOutLookupSwitchRefInt($1, $3); }
	;
