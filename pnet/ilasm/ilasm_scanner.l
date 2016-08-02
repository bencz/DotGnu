%{
/*
 * ilasm_scanner.l - Input file for lex that defines the ILASM token syntax.
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

/* Rename the lex/yacc symbols to support multiple parsers */
#include "ilasm_rename.h"

#include <stdio.h>
#include "il_system.h"
#include "il_opcodes.h"
#include "il_jopcodes.h"
#include "il_program.h"
#include "il_utils.h"
#include "ilasm_build.h"
#include "ilasm_output.h"
#include "ilasm_grammar.h"

extern YYSTYPE yylval;

char *ILAsmFilename = 0;
long  ILAsmLineNum = 1;
int   ILAsmErrors = 0;
int   ILAsmParseHexBytes = 0;
int   ILAsmParseJava = 0;

/*
 * Parse a floating point value.
 */
static ILDouble ParseFloat(const char *text)
{
#ifdef HAVE_STRTOD
	return (ILDouble)(strtod(text, (char **)0));
#else
	double val;
	if(sscanf(text, "%lf", &val) == 1)
	{
		return (ILDouble)val;
	}
	else
	{
		return (ILDouble)0.0;
	}
#endif
}

/*
 * Parse a decimal integer value.
 */
static ILInt64 ParseDecInt(const char *text)
{
	ILInt64 value = 0;
	while(*text != '\0')
	{
		value = (value * (ILInt64)10) + (ILInt64)(*text - '0');
		++text;
	}
	return value;
}

/*
 * Parse a hexadecimal integer value.
 */
static ILInt64 ParseHexInt(const char *text)
{
	ILInt64 value = 0;
	if(*text == '0' && (text[1] == 'x' || text[1] == 'X'))
	{
		text += 2;
	}
	while(*text != '\0')
	{
		if(*text >= '0' && *text <= '9')
			value = (value * (ILInt64)16) + (ILInt64)(*text - '0');
		else if(*text >= 'A' && *text <= 'F')
			value = (value * (ILInt64)16) + (ILInt64)(*text - 'A' + 10);
		else
			value = (value * (ILInt64)16) + (ILInt64)(*text - 'a' + 10);
		++text;
	}
	return value;
}

/*
 * Parse an octal integer value.
 */
static ILInt64 ParseOctInt(const char *text)
{
	ILInt64 value = 0;
	while(*text != '\0')
	{
		value = (value * (ILInt64)8) + (ILInt64)(*text - '0');
		++text;
	}
	return value;
}

/*
 * Helper macros for returning opcodes of various types.
 */
#define	OPCODE_NONE(name)	yylval.opcode = IL_OP_##name; \
							return I_NONE
#define	OPCODE_NONE_P(name)	yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_NONE
#define	OPCODE_VAR(name)	yylval.opcode = IL_OP_##name; \
							return I_VAR
#define	OPCODE_VAR_P(name)	yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_VAR
#define	OPCODE_BRANCH(name)	yylval.opcode = IL_OP_##name; \
							return I_BRANCH
#define	OPCODE_BRANCH_P(name) yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_BRANCH
#define	OPCODE_METHOD(name)	yylval.opcode = IL_OP_##name; \
							return I_METHOD
#define	OPCODE_METHOD_P(name) yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_METHOD
#define	OPCODE_IMETHOD(name) yylval.opcode = IL_OP_##name; \
							return I_IMETHOD
#define	OPCODE_IMETHOD_P(name) yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_IMETHOD
#define	OPCODE_FIELD(name)	yylval.opcode = IL_OP_##name; \
							return I_FIELD
#define	OPCODE_FIELD_P(name) yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_FIELD
#define	OPCODE_TYPE(name)	yylval.opcode = IL_OP_##name; \
							return I_TYPE
#define	OPCODE_TYPE_P(name) yylval.opcode = 0xFE00 | IL_PREFIX_OP_##name; \
							return I_TYPE

/*
 * Helper macros for returning Java opcodes of various types.
 */
#define	JOPCODE_NONE(name)		yylval.opcode = JAVA_OP_##name; \
								return I_NONE
#define	JOPCODE_VAR(name)		yylval.opcode = JAVA_OP_##name; \
								return I_VAR
#define	JOPCODE_BRANCH(name)	yylval.opcode = JAVA_OP_##name; \
								return I_BRANCH
#define	JOPCODE_METHOD(name)	yylval.opcode = JAVA_OP_##name; \
								return I_METHOD
#define	JOPCODE_FIELD(name)		yylval.opcode = JAVA_OP_##name; \
								return I_FIELD
#define	JOPCODE_TYPE(name)		yylval.opcode = JAVA_OP_##name; \
								return I_TYPE

%}

%option outfile="lex.yy.c"
%option prefix="ilasm_"
%option noyywrap
%option nounput

DIGIT			[0-9]
IDALPHA			[a-zA-Z_$@?]
HEX				[a-fA-F0-9]
EXPONENT		[Ee][+-]?{DIGIT}+
WHITE			[ \t\v\r\f\032]
SIGN			[+-]

%x HEXMODE
%x JAVAMODE
%%
	if(ILAsmParseHexBytes)
		BEGIN(HEXMODE);
	else if(ILAsmParseJava)
		BEGIN(JAVAMODE);
	else
		BEGIN(0);

<HEXMODE>{HEX}+			{ yylval.integer = ParseHexInt(yytext);
						  return HEX_BYTE; }
<HEXMODE>")"			{ return ')'; }

<INITIAL,JAVAMODE>".addon"				{ return D_ADDON; }	 /* Directives */
<INITIAL,JAVAMODE>".algorithm"			{ return D_ALGORITHM; }
<INITIAL,JAVAMODE>".assembly"			{ return D_ASSEMBLY; }
<INITIAL,JAVAMODE>".backing"			{ return D_BACKING; }
<INITIAL,JAVAMODE>".blob"				{ return D_BLOB; }
<INITIAL,JAVAMODE>".capability"			{ return D_CAPABILITY; }
<INITIAL,JAVAMODE>".cctor"				{ return D_CCTOR; }
<INITIAL,JAVAMODE>".class"				{ return D_CLASS; }
<INITIAL,JAVAMODE>".comtype"			{ return D_COMTYPE; }
<INITIAL,JAVAMODE>".config"				{ return D_CONFIG; }
<INITIAL,JAVAMODE>".corflags"			{ return D_CORFLAGS; }
<INITIAL,JAVAMODE>".ctor"				{ return D_CTOR; }
<INITIAL,JAVAMODE>".custom"				{ return D_CUSTOM; }
<INITIAL,JAVAMODE>".data"				{ return D_DATA; }
<INITIAL,JAVAMODE>".emitbyte"			{ return D_EMITBYTE; }
<INITIAL,JAVAMODE>".entrypoint"			{ return D_ENTRYPOINT; }
<INITIAL,JAVAMODE>".event"				{ return D_EVENT; }
<INITIAL,JAVAMODE>".exeloc"				{ return D_EXELOC; }
<INITIAL,JAVAMODE>".export"				{ return D_EXPORT; }
<INITIAL,JAVAMODE>".field"				{ return D_FIELD; }
<INITIAL,JAVAMODE>".file"				{ return D_FILE; }
<INITIAL,JAVAMODE>".fire"				{ return D_FIRE; }
<INITIAL,JAVAMODE>".get"				{ return D_GET; }
<INITIAL,JAVAMODE>".hash"				{ return D_HASH; }
<INITIAL,JAVAMODE>".imagebase"			{ return D_IMAGEBASE; }
<INITIAL,JAVAMODE>".implicitcom"		{ return D_IMPLICITCOM; }
<INITIAL,JAVAMODE>".language"			{ return D_LANGUAGE; }
<INITIAL,JAVAMODE>".library"			{ return D_LIBRARY; }
<INITIAL,JAVAMODE>".line"				{ return D_LINE; }
<INITIAL,JAVAMODE>"#line"				{ return D_LINE; }
<INITIAL,JAVAMODE>".locale"				{ return D_LOCALE; }
<INITIAL,JAVAMODE>".locals"				{ return D_LOCALS; }
<INITIAL,JAVAMODE>".manifestres"		{ return D_MANIFESTRES; }
<INITIAL,JAVAMODE>".maxstack"			{ return D_MAXSTACK; }
<INITIAL,JAVAMODE>".method"				{ return D_METHOD; }
<INITIAL,JAVAMODE>".mime"				{ return D_MIME; }
<INITIAL,JAVAMODE>".module"				{ return D_MODULE; }
<INITIAL,JAVAMODE>".mresource"			{ return D_MRESOURCE; }
<INITIAL,JAVAMODE>".namespace"			{ return D_NAMESPACE; }
<INITIAL,JAVAMODE>".originator"			{ return D_ORIGINATOR; }
<INITIAL,JAVAMODE>".os"					{ return D_OS; }
<INITIAL,JAVAMODE>".other"				{ return D_OTHER; }
<INITIAL,JAVAMODE>".override"			{ return D_OVERRIDE; }
<INITIAL,JAVAMODE>".pack"				{ return D_PACK; }
<INITIAL,JAVAMODE>".param"				{ return D_PARAM; }
<INITIAL,JAVAMODE>".permission"			{ return D_PERMISSION; }
<INITIAL,JAVAMODE>".permissionset"		{ return D_PERMISSIONSET; }
<INITIAL,JAVAMODE>".processor"			{ return D_PROCESSOR; }
<INITIAL,JAVAMODE>".property"			{ return D_PROPERTY; }
<INITIAL,JAVAMODE>".publickey"			{ return D_PUBLICKEY; }
<INITIAL,JAVAMODE>".publickeytoken"		{ return D_PUBLICKEYTOKEN; }
<INITIAL,JAVAMODE>".removeon"			{ return D_REMOVEON; }
<INITIAL,JAVAMODE>".set"				{ return D_SET; }
<INITIAL,JAVAMODE>".size"				{ return D_SIZE; }
<INITIAL,JAVAMODE>".stackreserve"		{ return D_STACKRESERVE; }
<INITIAL,JAVAMODE>".subsystem"			{ return D_SUBSYSTEM; }
<INITIAL,JAVAMODE>".title"				{ return D_TITLE; }
<INITIAL,JAVAMODE>".try"				{ return D_TRY; }
<INITIAL,JAVAMODE>".ver"				{ return D_VER; }
<INITIAL,JAVAMODE>".vtable"				{ return D_VTABLE; }
<INITIAL,JAVAMODE>".vtentry"			{ return D_VTENTRY; }
<INITIAL,JAVAMODE>".vtfixup"			{ return D_VTFIXUP; }
<INITIAL,JAVAMODE>".zeroinit"			{ return D_ZEROINIT; }

<INITIAL,JAVAMODE>"at"					{ return K_AT; }	/* Keywords */
<INITIAL,JAVAMODE>"as"					{ return K_AS; }
<INITIAL,JAVAMODE>"implicitcom"			{ return K_IMPLICITCOM; }
<INITIAL,JAVAMODE>"implicitres"			{ return K_IMPLICITRES; }
<INITIAL,JAVAMODE>"noappdomain"			{ return K_NOAPPDOMAIN; }
<INITIAL,JAVAMODE>"noprocess"			{ return K_NOPROCESS; }
<INITIAL,JAVAMODE>"nomachine"			{ return K_NOMACHINE; }
<INITIAL,JAVAMODE>"retargetable"		{ return K_RETARGETABLE; }
<INITIAL,JAVAMODE>"extern"				{ return K_EXTERN; }
<INITIAL,JAVAMODE>"instance"			{ return K_INSTANCE; }
<INITIAL,JAVAMODE>"explicit"			{ return K_EXPLICIT; }
<INITIAL,JAVAMODE>"default"				{ return K_DEFAULT; }
<INITIAL,JAVAMODE>"vararg"				{ return K_VARARG; }
<INITIAL,JAVAMODE>"unmanaged"			{ return K_UNMANAGED; }
<INITIAL,JAVAMODE>"cdecl"				{ return K_CDECL; }
<INITIAL,JAVAMODE>"stdcall"				{ return K_STDCALL; }
<INITIAL,JAVAMODE>"thiscall"			{ return K_THISCALL; }
<INITIAL,JAVAMODE>"fastcall"			{ return K_FASTCALL; }
<INITIAL,JAVAMODE>"marshal"				{ return K_MARSHAL; }
<INITIAL,JAVAMODE>"in"					{ return K_IN; }
<INITIAL,JAVAMODE>"out"					{ return K_OUT; }
<INITIAL,JAVAMODE>"opt"					{ return K_OPT; }
<INITIAL,JAVAMODE>"lcid"				{ return K_LCID; }
<INITIAL,JAVAMODE>"retval"				{ return K_RETVAL; }
<INITIAL,JAVAMODE>"static"				{ return K_STATIC; }
<INITIAL,JAVAMODE>"public"				{ return K_PUBLIC; }
<INITIAL,JAVAMODE>"private"				{ return K_PRIVATE; }
<INITIAL,JAVAMODE>"family"				{ return K_FAMILY; }
<INITIAL,JAVAMODE>"initonly"			{ return K_INITONLY; }
<INITIAL,JAVAMODE>"rtspecialname"		{ return K_RTSPECIALNAME; }
<INITIAL,JAVAMODE>"specialname"			{ return K_SPECIALNAME; }
<INITIAL,JAVAMODE>"assembly"			{ return K_ASSEMBLY; }
<INITIAL,JAVAMODE>"famandassem"			{ return K_FAMANDASSEM; }
<INITIAL,JAVAMODE>"famorassem"			{ return K_FAMORASSEM; }
<INITIAL,JAVAMODE>"privatescope"		{ return K_PRIVATESCOPE; }
<INITIAL,JAVAMODE>"literal"				{ return K_LITERAL; }
<INITIAL,JAVAMODE>"notserialized"		{ return K_NOTSERIALIZED; }
<INITIAL,JAVAMODE>"value"				{ return K_VALUE; }
<INITIAL,JAVAMODE>"not_in_gc_heap"		{ return K_NOT_IN_GC_HEAP; }
<INITIAL,JAVAMODE>"interface"			{ return K_INTERFACE; }
<INITIAL,JAVAMODE>"sealed"				{ return K_SEALED; }
<INITIAL,JAVAMODE>"abstract"			{ return K_ABSTRACT; }
<INITIAL,JAVAMODE>"auto"				{ return K_AUTO; }
<INITIAL,JAVAMODE>"sequential"			{ return K_SEQUENTIAL; }
<INITIAL,JAVAMODE>"ansi"				{ return K_ANSI; }
<INITIAL,JAVAMODE>"unicode"				{ return K_UNICODE; }
<INITIAL,JAVAMODE>"autochar"			{ return K_AUTOCHAR; }
<INITIAL,JAVAMODE>"import"				{ return K_IMPORT; }
<INITIAL,JAVAMODE>"serializable"		{ return K_SERIALIZABLE; }
<INITIAL,JAVAMODE>"nested"				{ return K_NESTED; }
<INITIAL,JAVAMODE>"lateinit"			{ return K_LATEINIT; }
<INITIAL,JAVAMODE>"extends"				{ return K_EXTENDS; }
<INITIAL,JAVAMODE>"implements"			{ return K_IMPLEMENTS; }
<INITIAL,JAVAMODE>"final"				{ return K_FINAL; }
<INITIAL,JAVAMODE>"virtual"				{ return K_VIRTUAL; }
<INITIAL,JAVAMODE>"hidebysig"			{ return K_HIDEBYSIG; }
<INITIAL,JAVAMODE>"newslot"				{ return K_NEWSLOT; }
<INITIAL,JAVAMODE>"unmanagedexp"		{ return K_UNMANAGEDEXP; }
<INITIAL,JAVAMODE>"pinvokeimpl"			{ return K_PINVOKEIMPL; }
<INITIAL,JAVAMODE>"nomangle"			{ return K_NOMANGLE; }
<INITIAL,JAVAMODE>"ole"					{ return K_OLE; }
<INITIAL,JAVAMODE>"lasterr"				{ return K_LASTERR; }
<INITIAL,JAVAMODE>"winapi"				{ return K_WINAPI; }
<INITIAL,JAVAMODE>"native"				{ return K_NATIVE; }
<INITIAL,JAVAMODE>"il"					{ return K_IL; }
<INITIAL,JAVAMODE>"optil"				{ return K_OPTIL; }
<INITIAL,JAVAMODE>"managed"				{ return K_MANAGED; }
<INITIAL,JAVAMODE>"forwardref"			{ return K_FORWARDREF; }
<INITIAL,JAVAMODE>"runtime"				{ return K_RUNTIME; }
<INITIAL,JAVAMODE>"internalcall"		{ return K_INTERNALCALL; }
<INITIAL,JAVAMODE>"synchronized"		{ return K_SYNCHRONIZED; }
<INITIAL,JAVAMODE>"noinlining"			{ return K_NOINLINING; }
<INITIAL,JAVAMODE>"custom"				{ return K_CUSTOM; }
<INITIAL,JAVAMODE>"fixed"				{ return K_FIXED; }
<INITIAL,JAVAMODE>"sysstring"			{ return K_SYSSTRING; }
<INITIAL,JAVAMODE>"array"				{ return K_ARRAY; }
<INITIAL,JAVAMODE>"variant"				{ return K_VARIANT; }
<INITIAL,JAVAMODE>"currency"			{ return K_CURRENCY; }
<INITIAL,JAVAMODE>"syschar"				{ return K_SYSCHAR; }
<INITIAL,JAVAMODE>"void"				{ return K_VOID; }
<INITIAL,JAVAMODE>"bool"				{ return K_BOOL; }
<INITIAL,JAVAMODE>"int8"				{ return K_INT8; }
<INITIAL,JAVAMODE>"int16"				{ return K_INT16; }
<INITIAL,JAVAMODE>"int32"				{ return K_INT32; }
<INITIAL,JAVAMODE>"int64"				{ return K_INT64; }
<INITIAL,JAVAMODE>"float32"				{ return K_FLOAT32; }
<INITIAL,JAVAMODE>"float64"				{ return K_FLOAT64; }
<INITIAL,JAVAMODE>"error"				{ return K_ERROR; }
<INITIAL,JAVAMODE>"unsigned"			{ return K_UNSIGNED; }
<INITIAL,JAVAMODE>"decimal"				{ return K_DECIMAL; }
<INITIAL,JAVAMODE>"date"				{ return K_DATE; }
<INITIAL,JAVAMODE>"bstr"				{ return K_BSTR; }
<INITIAL,JAVAMODE>"lpstr"				{ return K_LPSTR; }
<INITIAL,JAVAMODE>"lpwstr"				{ return K_LPWSTR; }
<INITIAL,JAVAMODE>"lptstr"				{ return K_LPTSTR; }
<INITIAL,JAVAMODE>"objectref"			{ return K_OBJECTREF; }
<INITIAL,JAVAMODE>"iunknown"			{ return K_IUNKNOWN; }
<INITIAL,JAVAMODE>"idispatch"			{ return K_IDISPATCH; }
<INITIAL,JAVAMODE>"struct"				{ return K_STRUCT; }
<INITIAL,JAVAMODE>"safearray"			{ return K_SAFEARRAY; }
<INITIAL,JAVAMODE>"int"					{ return K_INT; }
<INITIAL,JAVAMODE>"byvalstr"			{ return K_BYVALSTR; }
<INITIAL,JAVAMODE>"tbstr"				{ return K_TBSTR; }
<INITIAL,JAVAMODE>"lpvoid"				{ return K_LPVOID; }
<INITIAL,JAVAMODE>"any"					{ return K_ANY; }
<INITIAL,JAVAMODE>"float"				{ return K_FLOAT; }
<INITIAL,JAVAMODE>"lpstruct"			{ return K_LPSTRUCT; }
<INITIAL,JAVAMODE>"null"				{ return K_NULL; }
<INITIAL,JAVAMODE>"vector"				{ return K_VECTOR; }
<INITIAL,JAVAMODE>"hresult"				{ return K_HRESULT; }
<INITIAL,JAVAMODE>"carray"				{ return K_CARRAY; }
<INITIAL,JAVAMODE>"userdefined"			{ return K_USERDEFINED; }
<INITIAL,JAVAMODE>"record"				{ return K_RECORD; }
<INITIAL,JAVAMODE>"filetime"			{ return K_FILETIME; }
<INITIAL,JAVAMODE>"blob"				{ return K_BLOB; }
<INITIAL,JAVAMODE>"stream"				{ return K_STREAM; }
<INITIAL,JAVAMODE>"storage"				{ return K_STORAGE; }
<INITIAL,JAVAMODE>"streamed_object"		{ return K_STREAMED_OBJECT; }
<INITIAL,JAVAMODE>"stored_object"		{ return K_STORED_OBJECT; }
<INITIAL,JAVAMODE>"blob_object"			{ return K_BLOB_OBJECT; }
<INITIAL,JAVAMODE>"cf"					{ return K_CF; }
<INITIAL,JAVAMODE>"clsid"				{ return K_CLSID; }
<INITIAL,JAVAMODE>"method"				{ return K_METHOD; }
<INITIAL,JAVAMODE>"class"				{ return K_CLASS; }
<INITIAL,JAVAMODE>"pinned"				{ return K_PINNED; }
<INITIAL,JAVAMODE>"modreq"				{ return K_MODREQ; }
<INITIAL,JAVAMODE>"modopt"				{ return K_MODOPT; }
<INITIAL,JAVAMODE>"typedref"			{ return K_TYPEDREF; }
<INITIAL,JAVAMODE>"wchar"				{ return K_WCHAR; }
<INITIAL,JAVAMODE>"char"				{ return K_CHAR; }
<INITIAL,JAVAMODE>"fromunmanaged"		{ return K_FROMUNMANAGED; }
<INITIAL,JAVAMODE>"callmostderived"		{ return K_CALLMOSTDERIVED; }
<INITIAL,JAVAMODE>"bytearray"			{ return K_BYTEARRAY; }
<INITIAL,JAVAMODE>"with"				{ return K_WITH; }
<INITIAL,JAVAMODE>"init"				{ return K_INIT; }
<INITIAL,JAVAMODE>"to"					{ return K_TO; }
<INITIAL,JAVAMODE>"catch"				{ return K_CATCH; }
<INITIAL,JAVAMODE>"filter"				{ return K_FILTER; }
<INITIAL,JAVAMODE>"finally"				{ return K_FINALLY; }
<INITIAL,JAVAMODE>"fault"				{ return K_FAULT; }
<INITIAL,JAVAMODE>"handler"				{ return K_HANDLER; }
<INITIAL,JAVAMODE>"tls"					{ return K_TLS; }
<INITIAL,JAVAMODE>"field"				{ return K_FIELD; }
<INITIAL,JAVAMODE>"request"				{ return K_REQUEST; }
<INITIAL,JAVAMODE>"demand"				{ return K_DEMAND; }
<INITIAL,JAVAMODE>"assert"				{ return K_ASSERT; }
<INITIAL,JAVAMODE>"deny"				{ return K_DENY; }
<INITIAL,JAVAMODE>"permitonly"			{ return K_PERMITONLY; }
<INITIAL,JAVAMODE>"linkcheck"			{ return K_LINKCHECK; }
<INITIAL,JAVAMODE>"inheritcheck"		{ return K_INHERITCHECK; }
<INITIAL,JAVAMODE>"reqmin"				{ return K_REQMIN; }
<INITIAL,JAVAMODE>"reqopt"				{ return K_REQOPT; }
<INITIAL,JAVAMODE>"reqrefuse"			{ return K_REQREFUSE; }
<INITIAL,JAVAMODE>"prejitgrant"			{ return K_PREJITGRANT; }
<INITIAL,JAVAMODE>"prejitdeny"			{ return K_PREJITDENY; }
<INITIAL,JAVAMODE>"noncasdemand"		{ return K_NONCASDEMAND; }
<INITIAL,JAVAMODE>"noncaslinkdemand"	{ return K_NONCASLINKDEMAND; }
<INITIAL,JAVAMODE>"noncasinheritance"	{ return K_NONCASINHERITANCE; }
<INITIAL,JAVAMODE>"readonly"			{ return K_READONLY; }
<INITIAL,JAVAMODE>"nometadata"			{ return K_NOMETADATA; }
<INITIAL,JAVAMODE>"algorithm"			{ return K_ALGORITHM; }
<INITIAL,JAVAMODE>"fullorigin"			{ return K_FULLORIGIN; }
<INITIAL,JAVAMODE>"nan"					{ return K_NAN; }
<INITIAL,JAVAMODE>"inf"					{ return K_INF; }
<INITIAL,JAVAMODE>"publickey"			{ return K_PUBLICKEY; }
<INITIAL,JAVAMODE>"enablejittracking"	{ return K_ENABLEJITTRACKING; }
<INITIAL,JAVAMODE>"disablejitoptimizer"	{ return K_DISABLEJITOPTIMIZER; }
<INITIAL,JAVAMODE>"preservesig"			{ return K_PRESERVESIG; }
<INITIAL,JAVAMODE>"beforefieldinit"		{ return K_BEFOREFIELDINIT; }
<INITIAL,JAVAMODE>"compilercontrolled"	{ return K_COMPILERCONTROLLED; }
<INITIAL,JAVAMODE>"cil"					{ return K_CIL; }
<INITIAL,JAVAMODE>"valuetype"			{ return K_VALUETYPE; }
<INITIAL,JAVAMODE>"boxed"				{ return K_BOXED; }
<INITIAL,JAVAMODE>"string"				{ return K_STRING; }
<INITIAL,JAVAMODE>"object"				{ return K_OBJECT; }
<INITIAL,JAVAMODE>"alignment"			{ return K_ALIGNMENT; }
<INITIAL,JAVAMODE>"reqsecobj"			{ return K_REQSECOBJ; }
<INITIAL,JAVAMODE>"true"				{ return K_TRUE; }
<INITIAL,JAVAMODE>"false"				{ return K_FALSE; }
<INITIAL,JAVAMODE>"java"				{ return K_JAVA; }
<INITIAL,JAVAMODE>"nullref"				{ return K_NULLREF; }
"type"									{ return K_TYPE; }

<INITIAL,JAVAMODE>"::"					{ return COLON_COLON; }
<INITIAL,JAVAMODE>"..."					{ return DOT_DOT_DOT; }
<INITIAL,JAVAMODE>".."					{ return DOT_DOT; }
"!!"									{ return EXCL_EXCL; }

"add"					{ OPCODE_NONE(ADD); }	/* Opcodes with no args */
"add.ovf"				{ OPCODE_NONE(ADD_OVF); }
"add.ovf.un"			{ OPCODE_NONE(ADD_OVF_UN); }
"and"					{ OPCODE_NONE(AND); }
"ann.catch"				{ OPCODE_NONE(ANN_CATCH); }
"ann.def"				{ OPCODE_NONE(ANN_DEF); }
"ann.hoisted"			{ OPCODE_NONE(ANN_HOISTED); }
"ann.lab"				{ OPCODE_NONE(ANN_LAB); }
"arglist"				{ OPCODE_NONE_P(ARGLIST); }
"break"					{ OPCODE_NONE(BREAK); }
"ceq"					{ OPCODE_NONE_P(CEQ); }
"cgt"					{ OPCODE_NONE_P(CGT); }
"cgt.un"				{ OPCODE_NONE_P(CGT_UN); }
"ckfinite"				{ OPCODE_NONE(CKFINITE); }
"clt"					{ OPCODE_NONE_P(CLT); }
"clt.un"				{ OPCODE_NONE_P(CLT_UN); }
"conv.i"				{ OPCODE_NONE(CONV_I); }
"conv.i1"				{ OPCODE_NONE(CONV_I1); }
"conv.i2"				{ OPCODE_NONE(CONV_I2); }
"conv.i4"				{ OPCODE_NONE(CONV_I4); }
"conv.i8"				{ OPCODE_NONE(CONV_I8); }
"conv.ovf.i"			{ OPCODE_NONE(CONV_OVF_I); }
"conv.ovf.i.un"			{ OPCODE_NONE(CONV_OVF_I_UN); }
"conv.ovf.i1"			{ OPCODE_NONE(CONV_OVF_I1); }
"conv.ovf.i1.un"		{ OPCODE_NONE(CONV_OVF_I1_UN); }
"conv.ovf.i2"			{ OPCODE_NONE(CONV_OVF_I2); }
"conv.ovf.i2.un"		{ OPCODE_NONE(CONV_OVF_I2_UN); }
"conv.ovf.i4"			{ OPCODE_NONE(CONV_OVF_I4); }
"conv.ovf.i4.un"		{ OPCODE_NONE(CONV_OVF_I4_UN); }
"conv.ovf.i8"			{ OPCODE_NONE(CONV_OVF_I8); }
"conv.ovf.i8.un"		{ OPCODE_NONE(CONV_OVF_I8_UN); }
"conv.ovf.u"			{ OPCODE_NONE(CONV_OVF_U); }
"conv.ovf.u.un"			{ OPCODE_NONE(CONV_OVF_U_UN); }
"conv.ovf.u1"			{ OPCODE_NONE(CONV_OVF_U1); }
"conv.ovf.u1.un"		{ OPCODE_NONE(CONV_OVF_U1_UN); }
"conv.ovf.u2"			{ OPCODE_NONE(CONV_OVF_U2); }
"conv.ovf.u2.un"		{ OPCODE_NONE(CONV_OVF_U2_UN); }
"conv.ovf.u4"			{ OPCODE_NONE(CONV_OVF_U4); }
"conv.ovf.u4.un"		{ OPCODE_NONE(CONV_OVF_U4_UN); }
"conv.ovf.u8"			{ OPCODE_NONE(CONV_OVF_U8); }
"conv.ovf.u8.un"		{ OPCODE_NONE(CONV_OVF_U8_UN); }
"conv.r.un"				{ OPCODE_NONE(CONV_R_UN); }
"conv.r4"				{ OPCODE_NONE(CONV_R4); }
"conv.r8"				{ OPCODE_NONE(CONV_R8); }
"conv.u"				{ OPCODE_NONE(CONV_U); }
"conv.u1"				{ OPCODE_NONE(CONV_U1); }
"conv.u2"				{ OPCODE_NONE(CONV_U2); }
"conv.u4"				{ OPCODE_NONE(CONV_U4); }
"conv.u8"				{ OPCODE_NONE(CONV_U8); }
"cpblk"					{ OPCODE_NONE_P(CPBLK); }
"div"					{ OPCODE_NONE(DIV); }
"div.un"				{ OPCODE_NONE(DIV_UN); }
"dup"					{ OPCODE_NONE(DUP); }
"endfault"				{ OPCODE_NONE(ENDFINALLY); }	/* Alias */
"endfilter"				{ OPCODE_NONE_P(ENDFILTER); }
"endfinally"			{ OPCODE_NONE(ENDFINALLY); }
"initblk"				{ OPCODE_NONE_P(INITBLK); }
"ldarg.0"				{ OPCODE_NONE(LDARG_0); }
"ldarg.1"				{ OPCODE_NONE(LDARG_1); }
"ldarg.2"				{ OPCODE_NONE(LDARG_2); }
"ldarg.3"				{ OPCODE_NONE(LDARG_3); }
"ldc.i4.0"				{ OPCODE_NONE(LDC_I4_0); }
"ldc.i4.1"				{ OPCODE_NONE(LDC_I4_1); }
"ldc.i4.2"				{ OPCODE_NONE(LDC_I4_2); }
"ldc.i4.3"				{ OPCODE_NONE(LDC_I4_3); }
"ldc.i4.4"				{ OPCODE_NONE(LDC_I4_4); }
"ldc.i4.5"				{ OPCODE_NONE(LDC_I4_5); }
"ldc.i4.6"				{ OPCODE_NONE(LDC_I4_6); }
"ldc.i4.7"				{ OPCODE_NONE(LDC_I4_7); }
"ldc.i4.8"				{ OPCODE_NONE(LDC_I4_8); }
"ldc.i4.m1"				{ OPCODE_NONE(LDC_I4_M1); }
"ldc.i4.M1"				{ OPCODE_NONE(LDC_I4_M1); }	/* Alias */
"ldelem.i"				{ OPCODE_NONE(LDELEM_I); }
"ldelem.i1"				{ OPCODE_NONE(LDELEM_I1); }
"ldelem.i2"				{ OPCODE_NONE(LDELEM_I2); }
"ldelem.i4"				{ OPCODE_NONE(LDELEM_I4); }
"ldelem.i8"				{ OPCODE_NONE(LDELEM_I8); }
"ldelem.r4"				{ OPCODE_NONE(LDELEM_R4); }
"ldelem.r8"				{ OPCODE_NONE(LDELEM_R8); }
"ldelem.ref"			{ OPCODE_NONE(LDELEM_REF); }
"ldelem.u1"				{ OPCODE_NONE(LDELEM_U1); }
"ldelem.u2"				{ OPCODE_NONE(LDELEM_U2); }
"ldelem.u4"				{ OPCODE_NONE(LDELEM_U4); }
"ldelem.u8"				{ OPCODE_NONE(LDELEM_I8); }	/* Alias */
"ldind.i"				{ OPCODE_NONE(LDIND_I); }
"ldind.i1"				{ OPCODE_NONE(LDIND_I1); }
"ldind.i2"				{ OPCODE_NONE(LDIND_I2); }
"ldind.i4"				{ OPCODE_NONE(LDIND_I4); }
"ldind.i8"				{ OPCODE_NONE(LDIND_I8); }
"ldind.r4"				{ OPCODE_NONE(LDIND_R4); }
"ldind.r8"				{ OPCODE_NONE(LDIND_R8); }
"ldind.ref"				{ OPCODE_NONE(LDIND_REF); }
"ldind.u1"				{ OPCODE_NONE(LDIND_U1); }
"ldind.u2"				{ OPCODE_NONE(LDIND_U2); }
"ldind.u4"				{ OPCODE_NONE(LDIND_U4); }
"ldind.u8"				{ OPCODE_NONE(LDIND_I8); }	/* Alias */
"ldlen"					{ OPCODE_NONE(LDLEN); }
"ldloc.0"				{ OPCODE_NONE(LDLOC_0); }
"ldloc.1"				{ OPCODE_NONE(LDLOC_1); }
"ldloc.2"				{ OPCODE_NONE(LDLOC_2); }
"ldloc.3"				{ OPCODE_NONE(LDLOC_3); }
"ldnull"				{ OPCODE_NONE(LDNULL); }
"localloc"				{ OPCODE_NONE_P(LOCALLOC); }
"mul"					{ OPCODE_NONE(MUL); }
"mul.un"				{ OPCODE_NONE(MUL_OVF_UN); } /* Hack for missing insn */
"mul.ovf"				{ OPCODE_NONE(MUL_OVF); }
"mul.ovf.un"			{ OPCODE_NONE(MUL_OVF_UN); }
"neg"					{ OPCODE_NONE(NEG); }
"nop"					{ OPCODE_NONE(NOP); }
"not"					{ OPCODE_NONE(NOT); }
"or"					{ OPCODE_NONE(OR); }
"pop"					{ OPCODE_NONE(POP); }
"refanytype"			{ OPCODE_NONE_P(REFANYTYPE); }
"rem"					{ OPCODE_NONE(REM); }
"rem.un"				{ OPCODE_NONE(REM_UN); }
"ret"					{ OPCODE_NONE(RET); }
"rethrow"				{ OPCODE_NONE_P(RETHROW); }
"shl"					{ OPCODE_NONE(SHL); }
"shr"					{ OPCODE_NONE(SHR); }
"shr.un"				{ OPCODE_NONE(SHR_UN); }
"stelem.i"				{ OPCODE_NONE(STELEM_I); }
"stelem.i1"				{ OPCODE_NONE(STELEM_I1); }
"stelem.i2"				{ OPCODE_NONE(STELEM_I2); }
"stelem.i4"				{ OPCODE_NONE(STELEM_I4); }
"stelem.i8"				{ OPCODE_NONE(STELEM_I8); }
"stelem.r4"				{ OPCODE_NONE(STELEM_R4); }
"stelem.r8"				{ OPCODE_NONE(STELEM_R8); }
"stelem.ref"			{ OPCODE_NONE(STELEM_REF); }
"stind.i"				{ OPCODE_NONE(STIND_I); }
"stind.i1"				{ OPCODE_NONE(STIND_I1); }
"stind.i2"				{ OPCODE_NONE(STIND_I2); }
"stind.i4"				{ OPCODE_NONE(STIND_I4); }
"stind.i8"				{ OPCODE_NONE(STIND_I8); }
"stind.r4"				{ OPCODE_NONE(STIND_R4); }
"stind.r8"				{ OPCODE_NONE(STIND_R8); }
"stind.ref"				{ OPCODE_NONE(STIND_REF); }
"stloc.0"				{ OPCODE_NONE(STLOC_0); }
"stloc.1"				{ OPCODE_NONE(STLOC_1); }
"stloc.2"				{ OPCODE_NONE(STLOC_2); }
"stloc.3"				{ OPCODE_NONE(STLOC_3); }
"sub"					{ OPCODE_NONE(SUB); }
"sub.ovf"				{ OPCODE_NONE(SUB_OVF); }
"sub.ovf.un"			{ OPCODE_NONE(SUB_OVF_UN); }
"tail."					{ OPCODE_NONE_P(TAIL); }
"tail"					{ OPCODE_NONE_P(TAIL); }		/* Alias */
"throw"					{ OPCODE_NONE(THROW); }
"volatile."				{ OPCODE_NONE_P(VOLATILE); }
"volatile"				{ OPCODE_NONE_P(VOLATILE); }	/* Alias */
"xor"					{ OPCODE_NONE(XOR); }
"readonly."				{ OPCODE_NONE_P(READONLY); }
"readonly"				{ OPCODE_NONE_P(READONLY); }	/* Alias */

"ann.dead"				{ OPCODE_VAR(ANN_DEAD); }	/* Local var opcodes */
"ann.ref.s"				{ OPCODE_VAR(ANN_REF_S); }
"ldarg"					{ OPCODE_VAR_P(LDARG); }
"ldarg.s"				{ OPCODE_VAR(LDARG_S); }
"ldarga"				{ OPCODE_VAR_P(LDARGA); }
"ldarga.s"				{ OPCODE_VAR(LDARGA_S); }
"ldloc"					{ OPCODE_VAR_P(LDLOC); }
"ldloc.s"				{ OPCODE_VAR(LDLOC_S); }
"ldloca"				{ OPCODE_VAR_P(LDLOCA); }
"ldloca.s"				{ OPCODE_VAR(LDLOCA_S); }
"starg"					{ OPCODE_VAR_P(STARG); }
"starg.s"				{ OPCODE_VAR(STARG_S); }
"stloc"					{ OPCODE_VAR_P(STLOC); }
"stloc.s"				{ OPCODE_VAR(STLOC_S); }

"ldc.i4"				{ yylval.opcode = IL_OP_LDC_I4; /* Constant opcodes */
						  return I_INT; }
"ldc.i4.s"				{ yylval.opcode = IL_OP_LDC_I4_S;
						  return I_INT; }
"ldc.i8"				{ yylval.opcode = IL_OP_LDC_I8;
						  return I_INT; }
"ldc.r4"				{ yylval.opcode = IL_OP_LDC_R4;
						  return I_FLOAT; }
"ldc.r8"				{ yylval.opcode = IL_OP_LDC_R8;
						  return I_FLOAT; }
"unaligned."			{ yylval.opcode = 0xFE00 | IL_PREFIX_OP_UNALIGNED;
						  return I_INT; }
"unaligned"				{ yylval.opcode = 0xFE00 | IL_PREFIX_OP_UNALIGNED;
						  return I_INT; }	/* Alias */
"no."					{ yylval.opcode = 0xFE00 | IL_PREFIX_OP_NO;
						  return I_INT; }
"no"					{ yylval.opcode = 0xFE00 | IL_PREFIX_OP_NO;
						  return I_INT; }	/* Alias */

"ann.data"				{ OPCODE_BRANCH_P(ANN_DATA); } /* Branch opcodes */
"ann.data.s"			{ OPCODE_BRANCH(ANN_DATA_S); }
"beq"					{ OPCODE_BRANCH(BEQ); }
"beq.s"					{ OPCODE_BRANCH(BEQ_S); }
"bge"					{ OPCODE_BRANCH(BGE); }
"bge.s"					{ OPCODE_BRANCH(BGE_S); }
"bge.un"				{ OPCODE_BRANCH(BGE_UN); }
"bge.un.s"				{ OPCODE_BRANCH(BGE_UN_S); }
"bgt"					{ OPCODE_BRANCH(BGT); }
"bgt.s"					{ OPCODE_BRANCH(BGT_S); }
"bgt.un"				{ OPCODE_BRANCH(BGT_UN); }
"bgt.un.s"				{ OPCODE_BRANCH(BGT_UN_S); }
"ble"					{ OPCODE_BRANCH(BLE); }
"ble.s"					{ OPCODE_BRANCH(BLE_S); }
"ble.un"				{ OPCODE_BRANCH(BLE_UN); }
"ble.un.s"				{ OPCODE_BRANCH(BLE_UN_S); }
"blt"					{ OPCODE_BRANCH(BLT); }
"blt.s"					{ OPCODE_BRANCH(BLT_S); }
"blt.un"				{ OPCODE_BRANCH(BLT_UN); }
"blt.un.s"				{ OPCODE_BRANCH(BLT_UN_S); }
"bne.un"				{ OPCODE_BRANCH(BNE_UN); }
"bne.un.s"				{ OPCODE_BRANCH(BNE_UN_S); }
"br"					{ OPCODE_BRANCH(BR); }
"br.s"					{ OPCODE_BRANCH(BR_S); }
"brfalse"				{ OPCODE_BRANCH(BRFALSE); }
"brfalse.s"				{ OPCODE_BRANCH(BRFALSE_S); }
"brtrue"				{ OPCODE_BRANCH(BRTRUE); }
"brtrue.s"				{ OPCODE_BRANCH(BRTRUE_S); }
"leave"					{ OPCODE_BRANCH(LEAVE); }
"leave.s"				{ OPCODE_BRANCH(LEAVE_S); }

"brnull"				{ OPCODE_BRANCH(BRFALSE); }	/* Branch aliases */
"brnull.s"				{ OPCODE_BRANCH(BRFALSE_S); }
"brzero"				{ OPCODE_BRANCH(BRFALSE); }
"brzero.s"				{ OPCODE_BRANCH(BRFALSE_S); }
"brinst"				{ OPCODE_BRANCH(BRTRUE); }
"brinst.s"				{ OPCODE_BRANCH(BRTRUE_S); }

"ann.call"				{ OPCODE_METHOD(ANN_CALL); } /* Method opcodes */
"ann.hoisted_call"		{ OPCODE_METHOD(ANN_HOISTED_CALL); }
"call"					{ OPCODE_METHOD(CALL); }
"jmp"					{ OPCODE_METHOD(JMP); }
"ldftn"					{ OPCODE_METHOD_P(LDFTN); }

"callvirt"				{ OPCODE_IMETHOD(CALLVIRT); }
"ldvirtftn"				{ OPCODE_IMETHOD_P(LDVIRTFTN); }
"newobj"				{ OPCODE_IMETHOD(NEWOBJ); }

"ldfld"					{ OPCODE_FIELD(LDFLD); }	/* Field opcodes */
"ldflda"				{ OPCODE_FIELD(LDFLDA); }
"ldsfld"				{ OPCODE_FIELD(LDSFLD); }
"ldsflda"				{ OPCODE_FIELD(LDSFLDA); }
"stfld"					{ OPCODE_FIELD(STFLD); }
"stsfld"				{ OPCODE_FIELD(STSFLD); }

"box"					{ OPCODE_TYPE(BOX); }		/* Type opcodes */
"castclass"				{ OPCODE_TYPE(CASTCLASS); }
"cpobj"					{ OPCODE_TYPE(CPOBJ); }
"initobj"				{ OPCODE_TYPE_P(INITOBJ); }
"isinst"				{ OPCODE_TYPE(ISINST); }
"ldelema"				{ OPCODE_TYPE(LDELEMA); }
"ldobj"					{ OPCODE_TYPE(LDOBJ); }
"mkrefany"				{ OPCODE_TYPE(MKREFANY); }
"newarr"				{ OPCODE_TYPE(NEWARR); }
"refanyval"				{ OPCODE_TYPE(REFANYVAL); }
"sizeof"				{ OPCODE_TYPE_P(SIZEOF); }
"stobj"					{ OPCODE_TYPE(STOBJ); }
"unbox"					{ OPCODE_TYPE(UNBOX); }
"constrained."			{ OPCODE_TYPE_P(CONSTRAINED); }
"constrained"			{ OPCODE_TYPE_P(CONSTRAINED); } /* Alias */

"ldstr"					{ yylval.opcode = IL_OP_LDSTR;	/* String opcodes */
						  return I_STRING; }

"calli"					{ yylval.opcode = IL_OP_CALLI;	/* Signature opcodes */
						  return I_SIGNATURE; }
"jmpi"					{ yylval.opcode = 0xFE00 | IL_PREFIX_OP_JMPI;
						  return I_SIGNATURE; }

"ldptr"					{ yylval.opcode = IL_OP_LDPTR;	/* RVA opcodes */
						  return I_RVA; }

"ldtoken"				{ yylval.opcode = IL_OP_LDTOKEN; /* Metadata opcodes */
						  return I_TOKEN; }

"ann.phi"				{ yylval.opcode = IL_OP_ANN_PHI; /* SSA opcodes */
						  return I_SSA; }

"switch"				{ yylval.opcode = IL_OP_SWITCH;	/* Switch opcodes */
						  return I_SWITCH; }

"ldelem"				{ OPCODE_TYPE(LDELEM); }
"ldelem.any"			{ OPCODE_TYPE(LDELEM); }	/* Alias */
"stelem"				{ OPCODE_TYPE(STELEM); }
"stelem.any"			{ OPCODE_TYPE(STELEM); }	/* Alias */
"unbox.any"				{ OPCODE_TYPE(UNBOX_ANY); }

<JAVAMODE>"nop"			{ JOPCODE_NONE(NOP); }	/* Java opcodes with no args */
<JAVAMODE>"aconst_null"	{ JOPCODE_NONE(ACONST_NULL); }
<JAVAMODE>"iconst_m1"	{ JOPCODE_NONE(ICONST_M1); }
<JAVAMODE>"iconst_0"	{ JOPCODE_NONE(ICONST_0); }
<JAVAMODE>"iconst_1"	{ JOPCODE_NONE(ICONST_1); }
<JAVAMODE>"iconst_2"	{ JOPCODE_NONE(ICONST_2); }
<JAVAMODE>"iconst_3"	{ JOPCODE_NONE(ICONST_3); }
<JAVAMODE>"iconst_4"	{ JOPCODE_NONE(ICONST_4); }
<JAVAMODE>"iconst_5"	{ JOPCODE_NONE(ICONST_5); }
<JAVAMODE>"lconst_0"	{ JOPCODE_NONE(LCONST_0); }
<JAVAMODE>"lconst_1"	{ JOPCODE_NONE(LCONST_1); }
<JAVAMODE>"fconst_0"	{ JOPCODE_NONE(FCONST_0); }
<JAVAMODE>"fconst_1"	{ JOPCODE_NONE(FCONST_1); }
<JAVAMODE>"fconst_2"	{ JOPCODE_NONE(FCONST_2); }
<JAVAMODE>"dconst_0"	{ JOPCODE_NONE(DCONST_0); }
<JAVAMODE>"dconst_1"	{ JOPCODE_NONE(DCONST_1); }
<JAVAMODE>"iload_0"		{ JOPCODE_NONE(ILOAD_0); }
<JAVAMODE>"iload_1"		{ JOPCODE_NONE(ILOAD_1); }
<JAVAMODE>"iload_2"		{ JOPCODE_NONE(ILOAD_2); }
<JAVAMODE>"iload_3"		{ JOPCODE_NONE(ILOAD_3); }
<JAVAMODE>"lload_0"		{ JOPCODE_NONE(LLOAD_0); }
<JAVAMODE>"lload_1"		{ JOPCODE_NONE(LLOAD_1); }
<JAVAMODE>"lload_2"		{ JOPCODE_NONE(LLOAD_2); }
<JAVAMODE>"lload_3"		{ JOPCODE_NONE(LLOAD_3); }
<JAVAMODE>"fload_0"		{ JOPCODE_NONE(FLOAD_0); }
<JAVAMODE>"fload_1"		{ JOPCODE_NONE(FLOAD_1); }
<JAVAMODE>"fload_2"		{ JOPCODE_NONE(FLOAD_2); }
<JAVAMODE>"fload_3"		{ JOPCODE_NONE(FLOAD_3); }
<JAVAMODE>"dload_0"		{ JOPCODE_NONE(DLOAD_0); }
<JAVAMODE>"dload_1"		{ JOPCODE_NONE(DLOAD_1); }
<JAVAMODE>"dload_2"		{ JOPCODE_NONE(DLOAD_2); }
<JAVAMODE>"dload_3"		{ JOPCODE_NONE(DLOAD_3); }
<JAVAMODE>"aload_0"		{ JOPCODE_NONE(ALOAD_0); }
<JAVAMODE>"aload_1"		{ JOPCODE_NONE(ALOAD_1); }
<JAVAMODE>"aload_2"		{ JOPCODE_NONE(ALOAD_2); }
<JAVAMODE>"aload_3"		{ JOPCODE_NONE(ALOAD_3); }
<JAVAMODE>"iaload"		{ JOPCODE_NONE(IALOAD); }
<JAVAMODE>"laload"		{ JOPCODE_NONE(LALOAD); }
<JAVAMODE>"faload"		{ JOPCODE_NONE(FALOAD); }
<JAVAMODE>"daload"		{ JOPCODE_NONE(DALOAD); }
<JAVAMODE>"aaload"		{ JOPCODE_NONE(AALOAD); }
<JAVAMODE>"baload"		{ JOPCODE_NONE(BALOAD); }
<JAVAMODE>"caload"		{ JOPCODE_NONE(CALOAD); }
<JAVAMODE>"saload"		{ JOPCODE_NONE(SALOAD); }
<JAVAMODE>"istore_0"	{ JOPCODE_NONE(ISTORE_0); }
<JAVAMODE>"istore_1"	{ JOPCODE_NONE(ISTORE_1); }
<JAVAMODE>"istore_2"	{ JOPCODE_NONE(ISTORE_2); }
<JAVAMODE>"istore_3"	{ JOPCODE_NONE(ISTORE_3); }
<JAVAMODE>"lstore_0"	{ JOPCODE_NONE(LSTORE_0); }
<JAVAMODE>"lstore_1"	{ JOPCODE_NONE(LSTORE_1); }
<JAVAMODE>"lstore_2"	{ JOPCODE_NONE(LSTORE_2); }
<JAVAMODE>"lstore_3"	{ JOPCODE_NONE(LSTORE_3); }
<JAVAMODE>"fstore_0"	{ JOPCODE_NONE(FSTORE_0); }
<JAVAMODE>"fstore_1"	{ JOPCODE_NONE(FSTORE_1); }
<JAVAMODE>"fstore_2"	{ JOPCODE_NONE(FSTORE_2); }
<JAVAMODE>"fstore_3"	{ JOPCODE_NONE(FSTORE_3); }
<JAVAMODE>"dstore_0"	{ JOPCODE_NONE(DSTORE_0); }
<JAVAMODE>"dstore_1"	{ JOPCODE_NONE(DSTORE_1); }
<JAVAMODE>"dstore_2"	{ JOPCODE_NONE(DSTORE_2); }
<JAVAMODE>"dstore_3"	{ JOPCODE_NONE(DSTORE_3); }
<JAVAMODE>"astore_0"	{ JOPCODE_NONE(ASTORE_0); }
<JAVAMODE>"astore_1"	{ JOPCODE_NONE(ASTORE_1); }
<JAVAMODE>"astore_2"	{ JOPCODE_NONE(ASTORE_2); }
<JAVAMODE>"astore_3"	{ JOPCODE_NONE(ASTORE_3); }
<JAVAMODE>"iastore"		{ JOPCODE_NONE(IASTORE); }
<JAVAMODE>"lastore"		{ JOPCODE_NONE(LASTORE); }
<JAVAMODE>"fastore"		{ JOPCODE_NONE(FASTORE); }
<JAVAMODE>"dastore"		{ JOPCODE_NONE(DASTORE); }
<JAVAMODE>"aastore"		{ JOPCODE_NONE(AASTORE); }
<JAVAMODE>"bastore"		{ JOPCODE_NONE(BASTORE); }
<JAVAMODE>"castore"		{ JOPCODE_NONE(CASTORE); }
<JAVAMODE>"sastore"		{ JOPCODE_NONE(SASTORE); }
<JAVAMODE>"pop"			{ JOPCODE_NONE(POP); }
<JAVAMODE>"pop2"		{ JOPCODE_NONE(POP2); }
<JAVAMODE>"dup"			{ JOPCODE_NONE(DUP); }
<JAVAMODE>"dup_x1"		{ JOPCODE_NONE(DUP_X1); }
<JAVAMODE>"dup_x2"		{ JOPCODE_NONE(DUP_X2); }
<JAVAMODE>"dup2"		{ JOPCODE_NONE(DUP2); }
<JAVAMODE>"dup2_x1"		{ JOPCODE_NONE(DUP2_X1); }
<JAVAMODE>"dup2_x2"		{ JOPCODE_NONE(DUP2_X2); }
<JAVAMODE>"swap"		{ JOPCODE_NONE(SWAP); }
<JAVAMODE>"iadd"		{ JOPCODE_NONE(IADD); }
<JAVAMODE>"ladd"		{ JOPCODE_NONE(LADD); }
<JAVAMODE>"fadd"		{ JOPCODE_NONE(FADD); }
<JAVAMODE>"dadd"		{ JOPCODE_NONE(DADD); }
<JAVAMODE>"isub"		{ JOPCODE_NONE(ISUB); }
<JAVAMODE>"lsub"		{ JOPCODE_NONE(LSUB); }
<JAVAMODE>"fsub"		{ JOPCODE_NONE(FSUB); }
<JAVAMODE>"dsub"		{ JOPCODE_NONE(DSUB); }
<JAVAMODE>"imul"		{ JOPCODE_NONE(IMUL); }
<JAVAMODE>"lmul"		{ JOPCODE_NONE(LMUL); }
<JAVAMODE>"fmul"		{ JOPCODE_NONE(FMUL); }
<JAVAMODE>"dmul"		{ JOPCODE_NONE(DMUL); }
<JAVAMODE>"idiv"		{ JOPCODE_NONE(IDIV); }
<JAVAMODE>"ldiv"		{ JOPCODE_NONE(LDIV); }
<JAVAMODE>"fdiv"		{ JOPCODE_NONE(FDIV); }
<JAVAMODE>"ddiv"		{ JOPCODE_NONE(DDIV); }
<JAVAMODE>"irem"		{ JOPCODE_NONE(IREM); }
<JAVAMODE>"lrem"		{ JOPCODE_NONE(LREM); }
<JAVAMODE>"frem"		{ JOPCODE_NONE(FREM); }
<JAVAMODE>"drem"		{ JOPCODE_NONE(DREM); }
<JAVAMODE>"ineg"		{ JOPCODE_NONE(INEG); }
<JAVAMODE>"lneg"		{ JOPCODE_NONE(LNEG); }
<JAVAMODE>"fneg"		{ JOPCODE_NONE(FNEG); }
<JAVAMODE>"dneg"		{ JOPCODE_NONE(DNEG); }
<JAVAMODE>"ishl"		{ JOPCODE_NONE(ISHL); }
<JAVAMODE>"lshl"		{ JOPCODE_NONE(LSHL); }
<JAVAMODE>"ishr"		{ JOPCODE_NONE(ISHR); }
<JAVAMODE>"lshr"		{ JOPCODE_NONE(LSHR); }
<JAVAMODE>"iushr"		{ JOPCODE_NONE(IUSHR); }
<JAVAMODE>"lushr"		{ JOPCODE_NONE(LUSHR); }
<JAVAMODE>"iand"		{ JOPCODE_NONE(IAND); }
<JAVAMODE>"land"		{ JOPCODE_NONE(LAND); }
<JAVAMODE>"ior"			{ JOPCODE_NONE(IOR); }
<JAVAMODE>"lor"			{ JOPCODE_NONE(LOR); }
<JAVAMODE>"ixor"		{ JOPCODE_NONE(IXOR); }
<JAVAMODE>"lxor"		{ JOPCODE_NONE(LXOR); }
<JAVAMODE>"i2l"			{ JOPCODE_NONE(I2L); }
<JAVAMODE>"i2f"			{ JOPCODE_NONE(I2F); }
<JAVAMODE>"i2d"			{ JOPCODE_NONE(I2D); }
<JAVAMODE>"l2i"			{ JOPCODE_NONE(L2I); }
<JAVAMODE>"l2f"			{ JOPCODE_NONE(L2F); }
<JAVAMODE>"l2d"			{ JOPCODE_NONE(L2D); }
<JAVAMODE>"f2i"			{ JOPCODE_NONE(F2I); }
<JAVAMODE>"f2l"			{ JOPCODE_NONE(F2L); }
<JAVAMODE>"f2d"			{ JOPCODE_NONE(F2D); }
<JAVAMODE>"d2i"			{ JOPCODE_NONE(D2I); }
<JAVAMODE>"d2l"			{ JOPCODE_NONE(D2L); }
<JAVAMODE>"d2f"			{ JOPCODE_NONE(D2F); }
<JAVAMODE>"i2b"			{ JOPCODE_NONE(I2B); }
<JAVAMODE>"i2c"			{ JOPCODE_NONE(I2C); }
<JAVAMODE>"i2s"			{ JOPCODE_NONE(I2S); }
<JAVAMODE>"lcmp"		{ JOPCODE_NONE(LCMP); }
<JAVAMODE>"fcmpl"		{ JOPCODE_NONE(FCMPL); }
<JAVAMODE>"fcmpg"		{ JOPCODE_NONE(FCMPG); }
<JAVAMODE>"dcmpl"		{ JOPCODE_NONE(DCMPL); }
<JAVAMODE>"dcmpg"		{ JOPCODE_NONE(DCMPG); }
<JAVAMODE>"ireturn"		{ JOPCODE_NONE(IRETURN); }
<JAVAMODE>"lreturn"		{ JOPCODE_NONE(LRETURN); }
<JAVAMODE>"freturn"		{ JOPCODE_NONE(FRETURN); }
<JAVAMODE>"dreturn"		{ JOPCODE_NONE(DRETURN); }
<JAVAMODE>"areturn"		{ JOPCODE_NONE(ARETURN); }
<JAVAMODE>"return"		{ JOPCODE_NONE(RETURN); }
<JAVAMODE>"arraylength"	{ JOPCODE_NONE(ARRAYLENGTH); }
<JAVAMODE>"athrow"		{ JOPCODE_NONE(ATHROW); }
<JAVAMODE>"monitorenter" { JOPCODE_NONE(MONITORENTER); }
<JAVAMODE>"monitorexit"	{ JOPCODE_NONE(MONITOREXIT); }
<JAVAMODE>"breakpoint"	{ JOPCODE_NONE(BREAKPOINT); }

<JAVAMODE>"bipush"		{ yylval.opcode = JAVA_OP_BIPUSH; /* Int opcodes */
						  return I_INT; }
<JAVAMODE>"sipush"		{ yylval.opcode = JAVA_OP_SIPUSH;
						  return I_INT; }

<JAVAMODE>"ldc"			{ yylval.opcode = JAVA_OP_LDC;	/* Constant opcodes */
						  return I_CONST; }
<JAVAMODE>"ldc_w"		{ yylval.opcode = JAVA_OP_LDC_W;
						  return I_CONST; }
<JAVAMODE>"ldc2_w"		{ yylval.opcode = JAVA_OP_LDC2_W;
						  return I_CONST; }

<JAVAMODE>"iload"		{ JOPCODE_VAR(ILOAD); }		/* Variable opcodes */
<JAVAMODE>"lload"		{ JOPCODE_VAR(LLOAD); }
<JAVAMODE>"fload"		{ JOPCODE_VAR(FLOAD); }
<JAVAMODE>"dload"		{ JOPCODE_VAR(DLOAD); }
<JAVAMODE>"aload"		{ JOPCODE_VAR(ALOAD); }
<JAVAMODE>"istore"		{ JOPCODE_VAR(ISTORE); }
<JAVAMODE>"lstore"		{ JOPCODE_VAR(LSTORE); }
<JAVAMODE>"fstore"		{ JOPCODE_VAR(FSTORE); }
<JAVAMODE>"dstore"		{ JOPCODE_VAR(DSTORE); }
<JAVAMODE>"astore"		{ JOPCODE_VAR(ASTORE); }
<JAVAMODE>"ret"			{ JOPCODE_VAR(RET); }

<JAVAMODE>"iinc"		{ yylval.opcode = JAVA_OP_IINC;	/* Increment opcodes */
						  return I_IINC; }

<JAVAMODE>"ifeq"		{ JOPCODE_BRANCH(IFEQ); }	/* Branch opcodes */
<JAVAMODE>"ifne"		{ JOPCODE_BRANCH(IFNE); }
<JAVAMODE>"iflt"		{ JOPCODE_BRANCH(IFLT); }
<JAVAMODE>"ifge"		{ JOPCODE_BRANCH(IFGE); }
<JAVAMODE>"ifgt"		{ JOPCODE_BRANCH(IFGT); }
<JAVAMODE>"ifle"		{ JOPCODE_BRANCH(IFLE); }
<JAVAMODE>"if_icmpeq"	{ JOPCODE_BRANCH(IF_ICMPEQ); }
<JAVAMODE>"if_icmpne"	{ JOPCODE_BRANCH(IF_ICMPNE); }
<JAVAMODE>"if_icmplt"	{ JOPCODE_BRANCH(IF_ICMPLT); }
<JAVAMODE>"if_icmpge"	{ JOPCODE_BRANCH(IF_ICMPGE); }
<JAVAMODE>"if_icmpgt"	{ JOPCODE_BRANCH(IF_ICMPGT); }
<JAVAMODE>"if_icmple"	{ JOPCODE_BRANCH(IF_ICMPLE); }
<JAVAMODE>"if_acmpeq"	{ JOPCODE_BRANCH(IF_ACMPEQ); }
<JAVAMODE>"if_acmpne"	{ JOPCODE_BRANCH(IF_ACMPNE); }
<JAVAMODE>"goto"		{ JOPCODE_BRANCH(GOTO); }
<JAVAMODE>"jsr"			{ JOPCODE_BRANCH(JSR); }
<JAVAMODE>"ifnull"		{ JOPCODE_BRANCH(IFNULL); }
<JAVAMODE>"ifnonnull"	{ JOPCODE_BRANCH(IFNONNULL); }
<JAVAMODE>"goto_w"		{ JOPCODE_BRANCH(GOTO_W); }
<JAVAMODE>"jsr_w"		{ JOPCODE_BRANCH(JSR_W); }

<JAVAMODE>"tableswitch"	{ yylval.opcode = JAVA_OP_TABLESWITCH; /* Switches */
						  return I_SWITCH; }
<JAVAMODE>"lookupswitch" { yylval.opcode = JAVA_OP_LOOKUPSWITCH;
						  return I_LSWITCH; }

<JAVAMODE>"getstatic"	{ JOPCODE_FIELD(GETSTATIC); }	/* Field opcodes */
<JAVAMODE>"putstatic"	{ JOPCODE_FIELD(PUTSTATIC); }
<JAVAMODE>"getfield"	{ JOPCODE_FIELD(GETFIELD); }
<JAVAMODE>"putfield"	{ JOPCODE_FIELD(PUTFIELD); }

<JAVAMODE>"invokevirtual"	{ JOPCODE_METHOD(INVOKEVIRTUAL); } /* Methods */
<JAVAMODE>"invokespecial"	{ JOPCODE_METHOD(INVOKESPECIAL); }
<JAVAMODE>"invokestatic"	{ JOPCODE_METHOD(INVOKESTATIC); }
<JAVAMODE>"invokeinterface"	{ yylval.opcode = JAVA_OP_INVOKEINTERFACE;
							  return I_IMETHOD; }

<JAVAMODE>"new"			{ JOPCODE_TYPE(NEW); }		/* Type opcodes */
<JAVAMODE>"anewarray"	{ JOPCODE_TYPE(ANEWARRAY); }
<JAVAMODE>"checkcast"	{ JOPCODE_TYPE(CHECKCAST); }
<JAVAMODE>"instanceof"	{ JOPCODE_TYPE(INSTANCEOF); }

<JAVAMODE>"newarray"	{ yylval.opcode = JAVA_OP_NEWARRAY;	/* Arrays */
						  return I_NEWARRAY; }
<JAVAMODE>"multianewarray"	{ yylval.opcode = JAVA_OP_MULTIANEWARRAY;
						      return I_MULTINEWARRAY; }

{IDALPHA}({DIGIT}|{IDALPHA}|"`")*	{
			yylval.strValue = ILInternString(yytext, strlen(yytext));
			return IDENTIFIER;
		}

"."{IDALPHA}({DIGIT}|{IDALPHA}|"`")*	{
			yylval.strValue = ILInternString(yytext, strlen(yytext));
			return DOT_IDENTIFIER;
		}

<JAVAMODE>{IDALPHA}({DIGIT}|{IDALPHA})*	{
			yylval.strValue = ILInternString(yytext, strlen(yytext));
			return IDENTIFIER;
		}

<JAVAMODE>"."{IDALPHA}({DIGIT}|{IDALPHA})*	{
			yylval.strValue = ILInternString(yytext, strlen(yytext));
			return DOT_IDENTIFIER;
		}

<INITIAL,JAVAMODE>{SIGN}?{DIGIT}+{EXPONENT}		{
			yylval.real = ParseFloat(yytext);
			return FLOAT_CONSTANT;
		}
<INITIAL,JAVAMODE>{SIGN}?{DIGIT}*"."{DIGIT}+({EXPONENT})?	{
			yylval.real = ParseFloat(yytext);
			return FLOAT_CONSTANT;
		}
<INITIAL,JAVAMODE>{SIGN}?{DIGIT}+"."{DIGIT}*({EXPONENT})?	{
			yylval.real = ParseFloat(yytext);
			return FLOAT_CONSTANT;
		}

<INITIAL,JAVAMODE>0[xX]{HEX}+			{
			yylval.integer = ParseHexInt(yytext);
			return INTEGER_CONSTANT;
		}
<INITIAL,JAVAMODE>-0[xX]{HEX}+			{
			yylval.integer = -(ParseHexInt(yytext + 1));
			return INTEGER_CONSTANT;
		}
<INITIAL,JAVAMODE>0[0-7]*				{
			yylval.integer = ParseOctInt(yytext);
			return INTEGER_CONSTANT;
		}
<INITIAL,JAVAMODE>-0[0-7]*				{
			yylval.integer = -(ParseOctInt(yytext + 1));
			return INTEGER_CONSTANT;
		}
<INITIAL,JAVAMODE>{DIGIT}+				{
			yylval.integer = ParseDecInt(yytext);
			return INTEGER_CONSTANT;
		}
<INITIAL,JAVAMODE>-{DIGIT}+				{
			yylval.integer = -(ParseDecInt(yytext + 1));
			return INTEGER_CONSTANT;
		}

<INITIAL,JAVAMODE>'(\\.|[^\\'])*'		{
			/* Remove the quote characters */
			char *str = yytext + 1;
			str[strlen(str) - 1] = '\0';
			yylval.quoteString = str;
			return SQUOTE_STRING;
		}
<INITIAL,JAVAMODE>\"(\\.|[^\\"])*\"		{
			/* Remove the quote characters */
			char *str = yytext + 1;
			str[strlen(str) - 1] = '\0';
			yylval.quoteString = str;
			return DQUOTE_STRING;
		}

<*>"//"				{
						/* Skip a single-line comment */
						int ch = input();
						while(ch != EOF && ch != '\n')
						{
							ch = input();
						}
						++ILAsmLineNum;
					}
<*>"/*"				{
						/* Skip a multi-line comment */
						int ch;
						for(;;)
						{
							ch = input();
							while(ch != '*' && ch != EOF)
							{
								if(ch == '\n')
								{
									++ILAsmLineNum;
								}
								ch = input();
							}
							if(ch == '*')
							{
								ch = input();
								while(ch == '*')
								{
									ch = input();
								}
								if(ch == '/')
								{
									break;
								}
								else if(ch == '\n')
								{
									++ILAsmLineNum;
								}
							}
							if(ch == EOF)
							{
								break;
							}
						}
					}

<*>{WHITE}+			;

"\357\273\277"		{ /* Weird Microsoft UTF-8 marker */ }

<*>\n				{ ++ILAsmLineNum; }

<INITIAL,JAVAMODE>.	{ return (((int)(yytext[0])) & 0xFF); }

<HEXMODE>.			{ /* illegal character within hex bytes */ }

%%
