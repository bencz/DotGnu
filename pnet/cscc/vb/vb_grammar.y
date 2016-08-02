%{
/*
 * vb_grammar.y - Input file for yacc that defines the syntax of VB.
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

/* Rename the lex/yacc symbols to support multiple parsers */
#include "vb_rename.h"

#include "il_config.h"
#include <stdio.h>
#include <cscc/vb/vb_internal.h>

#define	YYERROR_VERBOSE

/*
 * Imports from the lexical analyser.
 */
extern int yylex(void);
#ifdef YYTEXT_POINTER
extern char *vb_text;
#else
extern char vb_text[];
#endif

static void yyerror(char *msg)
{
	CCPluginParseError(msg, vb_text);
}

/*
 * Insert an "else" clause into an "if" node at the end of the chain.
 */
static void VBInsertElse(ILNode *ifnode, ILNode *elseClause)
{
	while(((ILNode_If *)ifnode)->elseClause != 0)
	{
		ifnode = ((ILNode_If *)ifnode)->elseClause;
	}
	((ILNode_If *)ifnode)->elseClause = elseClause;
}

/*
 * Global state used by the parser.
 */
static unsigned long NestingLevel = 0;
static ILIntString CurrNamespace = {"", 0};
static ILNode_Namespace *CurrNamespaceNode = 0;
static ILNode_Namespace *GlobalNamespaceNode = 0;
static ILScope* localScope = 0;
static int HaveDecls = 0;

/*
 * Get the global scope.
 */
static ILScope *GlobalScope(void)
{
	if(CCCodeGen.globalScope)
	{
		return CCCodeGen.globalScope;
	}
	else
	{
		CCCodeGen.globalScope = ILScopeCreate(&CCCodeGen, 0);
		return CCCodeGen.globalScope;
	}
}

/* 
 * Get the local scope
 */
static ILScope *LocalScope(void)
{
	if(localScope)
	{
		return localScope;
	}
	else
	{
		localScope = ILScopeCreate(&CCCodeGen, GlobalScope());
		return localScope;
	}
}

/*
 * Initialize the global namespace, if necessary.
 */
static void PushIntoNamespace(ILIntString name);
static void InitGlobalNamespace(void)
{
	if(!CurrNamespaceNode)
	{
		char *root;

		/* Create the global namespace node */
		CurrNamespaceNode = (ILNode_Namespace *)ILNode_Namespace_create(0, 0);
		CurrNamespaceNode->localScope = GlobalScope();
		ILNamespaceAddUsing(CurrNamespaceNode,
			(ILNode_UsingNamespace *)ILNode_UsingNamespace_create("System"));
		GlobalNamespaceNode = CurrNamespaceNode;

		/* Set the default options on the global namespace */
		VBOptionInit(GlobalNamespaceNode);

		/* Add extra "using" clauses for command-line supplied namespaces */
		VBAddGlobalImports(GlobalNamespaceNode, GlobalScope());

		/* If we have a root namespace, then push into it */
		root = VBGetRootNamespace();
		if(root)
		{
			PushIntoNamespace(ILInternString(root, -1));
		}
	}
}

/*
 * Push into a new namespace context.
 */
static void PushIntoNamespace(ILIntString name)
{
	int posn, len;
	ILScope *oldLocalScope;
	posn = 0;
	while(posn < name.len)
	{
		/* Extract the next identifier */
		if(name.string[posn] == '.')
		{
			++posn;
			continue;
		}
		len = 0;
		while((posn + len) < name.len &&
			  name.string[posn + len] != '.')
		{
			++len;
		}

		/* Push a new identifier onto the end of the namespace */
		if(CurrNamespace.len != 0)
		{
			CurrNamespace = ILInternAppendedString
				(CurrNamespace,
				 ILInternAppendedString
				 	(ILInternString(".", 1),
					 ILInternString(name.string + posn, len)));
		}
		else
		{
			CurrNamespace = ILInternString(name.string + posn, len);
		}

		/* Create the namespace node */
		InitGlobalNamespace();
		
		oldLocalScope=LocalScope();
		
		CurrNamespaceNode = (ILNode_Namespace *)
			ILNode_Namespace_create(CurrNamespace.string,
									CurrNamespaceNode);

		/* Preserve compilation unit specific local scopes 
		 * or maybe I need to create a new scope as child of
		 * this scope (fix when I find a test case) */
		CurrNamespaceNode->localScope=oldLocalScope;

		/* Declare the namespace within the global scope */
		ILScopeDeclareNamespace(GlobalScope(),
								CurrNamespace.string);

		/* Move on to the next namespace component */
		posn += len;
	}
}

/*
 * Pop out of a namespace context.
 */
static void PopOutOfNamespace(ILIntString name)
{
	if(CurrNamespace.len == name.len)
	{
		CurrNamespace = ILInternString("", 0);
		while(CurrNamespaceNode->enclosing != 0)
		{
			CurrNamespaceNode = CurrNamespaceNode->enclosing;
		}
	}
	else
	{
		CurrNamespace = ILInternString
			(CurrNamespace.string, CurrNamespace.len - name.len - 1);
		while(CurrNamespaceNode->name != CurrNamespace.string)
		{
			CurrNamespaceNode = CurrNamespaceNode->enclosing;
		}
	}
}

/*
 * Reset the global state ready for the next file to be parsed.
 */
static void ResetState(void)
{
	NestingLevel = 0;
	CurrNamespace = ILInternString("", 0);
	CurrNamespaceNode = 0;
	GlobalNamespaceNode = 0;
	localScope = 0;
	ILScopeClearUsing(GlobalScope());
	HaveDecls = 0;
}

/*
 * Determine if the current namespace already has a "using"
 * declaration for a particular namespace.
 */
static int HaveUsingNamespace(const char *name)
{
	ILNode_UsingNamespace *using = CurrNamespaceNode->using;
	while(using != 0)
	{
		if(!strcmp(using->name, name))
		{
			return 1;
		}
		using = using->next;
	}
	return 0;
}

/*
 * Clone the filename/linenum information from one node to another.
 */
static void CloneLine(ILNode *dest, ILNode *src)
{
	yysetfilename(dest, yygetfilename(src));
	yysetlinenum(dest, yygetlinenum(src));
}

/*
 * Make a list from an existing list (may be NULL), and a new node
 * (which may also be NULL).
 */
static ILNode *MakeList(ILNode *list, ILNode *node)
{
	if(!node)
	{
		return list;
	}
	else if(!list)
	{
		list = ILNode_List_create();
	}
	ILNode_List_Add(list, node);
	return list;
}

/*
 * Make a system type name node.
 */
#define	MakeSystemType(name)	\
			(ILNode_GlobalNamespace_create(ILNode_SystemType_create(name)))

/*
 * The class name stack, which is used to verify the names
 * of constructors and destructors against the name of their
 * enclosing classes.  Also used to check if a class has
 * had a constructor defined for it.
 */
static ILNode **classNameStack = 0;
static int     *classNameCtorDefined = 0;
static int     *classNameIsModule = 0;
static int		classNameStackSize = 0;
static int		classNameStackMax = 0;

/*
 * Push an item onto the class name stack.
 */
static void ClassNamePush(ILNode *name, int isModule)
{
	if(classNameStackSize >= classNameStackMax)
	{
		classNameStack = (ILNode **)ILRealloc
			(classNameStack, sizeof(ILNode *) * (classNameStackMax + 4));
		if(!classNameStack)
		{
			CCOutOfMemory();
		}
		classNameCtorDefined = (int *)ILRealloc
			(classNameCtorDefined, sizeof(int) * (classNameStackMax + 4));
		if(!classNameCtorDefined)
		{
			CCOutOfMemory();
		}
		classNameIsModule = (int *)ILRealloc
			(classNameIsModule, sizeof(int) * (classNameStackMax + 4));
		if(!classNameIsModule)
		{
			CCOutOfMemory();
		}
		classNameStackMax += 4;
	}
	classNameStack[classNameStackSize] = name;
	classNameIsModule[classNameStackSize] = isModule;
	classNameCtorDefined[classNameStackSize++] = 0;
}

/*
 * Pop an item from the class name stack.
 */
static void ClassNamePop(void)
{
	--classNameStackSize;
}

/*
 * Record that a constructor was defined for the current class.
 */
static void ClassNameCtorDefined(void)
{
	classNameCtorDefined[classNameStackSize - 1] = 1;
}

/*
 * Determine if a constructor was defined for the current class.
 */
static int ClassNameIsCtorDefined(void)
{
	return classNameCtorDefined[classNameStackSize - 1];
}

/*
 * Determine if the current class is a module.
 */
static int ClassNameIsModule(void)
{
	return classNameIsModule[classNameStackSize - 1];
}

/*
 * Combine two attribute lists.
 */
static ILNode *CombineAttributes(ILNode *list, ILAttrTargetType type,
								 ILNode *attr)
{
	/* TODO */
	return list;
}

%}

/*
 * Define the structure of yylval.
 */
%union {

	struct
	{
		ILInt64		value;
		int			type;
		int			numDigits;
	}					integer;
	struct
	{
		ILDouble	value;
		int			type;
	}					real;
	ILDecimal			decimal;
	ILIntString			string;
	const char		   *name;
	ILNode             *node;
	ILUInt32			rank;
	ILInt64				date;
	int					ampm;
	struct
	{
		ILNode	   *thenClause;
		ILNode	   *elseClause;

	}					ifrest;
	struct
	{
		ILNode	   *name;
		ILNode     *type;

	}					loopvar;
	ILUInt32			modifier;
	struct
	{
		ILNode		   *body;
		ILNode		   *staticCtors;

	} member;
	struct
	{
		ILUInt32	rank;
		ILNode	   *exprs;

	}					arrayName;
	struct
	{
		ILNode	   *type;
		ILNode	   *attrs;

	}					typeInfo;
	ILParameterModifier	paramMod;

}

/*
 * Basic lexical values.
 */
%token IDENTIFIER		"an identifier"
%token TYPED_IDENTIFIER	"a typed identifier"
%token INTEGER_CONSTANT	"an integer value"
%token FLOAT_CONSTANT	"a floating point value"
%token DECIMAL_CONSTANT	"a decimal value"
%token CHAR_LITERAL		"a character literal"
%token STRING_LITERAL	"a string literal"
%token END_LINE			"line terminator"

/*
 * Operators.
 */
%token CONCAT_ASSIGN_OP		"`&='"
%token MUL_ASSIGN_OP		"`*='"
%token DIV_ASSIGN_OP		"`/='"
%token IDIV_ASSIGN_OP		"`\\='"
%token POW_ASSIGN_OP		"`^='"
%token ADD_ASSIGN_OP		"`+='"
%token SUB_ASSIGN_OP		"`-='"
%token NE_OP				"`<>'"
%token LE_OP				"`<='"
%token GE_OP				"`>='"
%token EQ_OP				"`=='"
%token LEFT_OP				"`<<'"
%token RIGHT_OP				"`>>'"

/*
 * Reserved words.
 */
%token K_ADDHANDLER		"`AddHandler'"
%token K_ADDRESSOF		"`AddressOf'"
%token K_ALIAS			"`Alias'"
%token K_AND			"`And'"
%token K_ANDALSO		"`AndAlso'"
%token K_ANSI			"`Ansi'"
%token K_AS				"`As'"
%token K_ASSEMBLY		"`Assembly'"
%token K_AUTO			"`Auto'"
%token K_BOOLEAN		"`Boolean'"
%token K_BYREF			"`ByRef'"
%token K_BYTE			"`Byte'"
%token K_BYVAL			"`ByVal'"
%token K_CALL			"`Call'"
%token K_CASE			"`Case'"
%token K_CATCH			"`Catch'"
%token K_CBOOL			"`CBool'"
%token K_CBYTE			"`CByte'"
%token K_CCHAR			"`CChar'"
%token K_CDATE			"`CDate'"
%token K_CDEC			"`CDec'"
%token K_CDBL			"`CDbl'"
%token K_CHAR			"`Char'"
%token K_CINT			"`CInt'"
%token K_CLASS			"`Class'"
%token K_CLNG			"`CLng'"
%token K_COBJ			"`CObj'"
%token K_CONST			"`Const'"
%token K_CSHORT			"`CShort'"
%token K_CSNG			"`CSng'"
%token K_CSTR			"`CStr'"
%token K_CTYPE			"`CType'"
%token K_DATE			"`Date'"
%token K_DECIMAL		"`Decimal'"
%token K_DECLARE		"`Declare'"
%token K_DEFAULT		"`Default'"
%token K_DELEGATE		"`Delegate'"
%token K_DIM			"`Dim'"
%token K_DIRECTCAST		"`DirectCast'"
%token K_DO				"`Do'"
%token K_DOUBLE			"`Double'"
%token K_EACH			"`Each'"
%token K_ELSE			"`Else'"
%token K_ELSEIF			"`ElseIf'"
%token K_END			"`End'"
%token K_ENUM			"`Enum'"
%token K_ERASE			"`Erase'"
%token K_ERROR			"`Error'"
%token K_EVENT			"`Event'"
%token K_EXIT			"`Exit'"
%token K_FALSE			"`False'"
%token K_FINALLY		"`Finally'"
%token K_FOR			"`For'"
%token K_FRIEND			"`Friend'"
%token K_FUNCTION		"`Function'"
%token K_GET			"`Get'"
%token K_GETTYPE		"`GetType'"
%token K_GOSUB			"`GoSub'"
%token K_GOTO			"`GoTo'"
%token K_HANDLES		"`Handles'"
%token K_IF				"`If'"
%token K_IMPLEMENTS		"`Implements'"
%token K_IMPORTS		"`Imports'"
%token K_IN				"`In'"
%token K_INHERITS		"`Inherits'"
%token K_INTEGER		"`Integer'"
%token K_INTERFACE		"`Interface'"
%token K_IS				"`Is'"
%token K_LET			"`Let'"
%token K_LIB			"`Lib'"
%token K_LIKE			"`Like'"
%token K_LONG			"`Long'"
%token K_LOOP			"`Loop'"
%token K_ME				"`Me'"
%token K_MOD			"`Mod'"
%token K_MODULE			"`Module'"
%token K_MUSTINHERIT	"`MustInherit'"
%token K_MUSTOVERRIDE	"`MustOverride'"
%token K_MYBASE			"`MyBase'"
%token K_MYCLASS		"`MyClass'"
%token K_NAMESPACE		"`Namespace'"
%token K_NEW			"`New'"
%token K_NEXT			"`Next'"
%token K_NOT			"`Not'"
%token K_NOTHING		"`Nothing'"
%token K_NOTINHERITABLE	"`NotInheritable'"
%token K_NOTOVERRIDABLE	"`NotOverridable'"
%token K_OBJECT			"`Object'"
%token K_ON				"`On'"
%token K_OPTION			"`Option'"
%token K_OPTIONAL		"`Optional'"
%token K_OR				"`Or'"
%token K_ORELSE			"`OrElse'"
%token K_OVERLOADS		"`Overloads'"
%token K_OVERRIDABLE	"`Overridable'"
%token K_OVERRIDES		"`Overrides'"
%token K_PARAMARRAY		"`ParamArray'"
%token K_PRESERVE		"`Preserve'"
%token K_PRIVATE		"`Private'"
%token K_PROPERTY		"`Property'"
%token K_PROTECTED		"`Protected'"
%token K_PUBLIC			"`Public'"
%token K_RAISEEVENT		"`RaiseEvent'"
%token K_READONLY		"`ReadOnly'"
%token K_REDIM			"`ReDim'"
%token K_REMOVEHANDLER	"`RemoveHandler'"
%token K_RESUME			"`Resume'"
%token K_RETURN			"`Return'"
%token K_SELECT			"`Select'"
%token K_SET			"`Set'"
%token K_SHADOWS		"`Shadows'"
%token K_SHARED			"`Shared'"
%token K_SHORT			"`Short'"
%token K_SINGLE			"`Single'"
%token K_STATIC			"`Static'"
%token K_STEP			"`Step'"
%token K_STOP			"`Stop'"
%token K_STRING			"`String'"
%token K_STRUCTURE		"`Structure'"
%token K_SUB			"`Sub'"
%token K_SYNCLOCK		"`SyncLock'"
%token K_THEN			"`Then'"
%token K_THROW			"`Throw'"
%token K_TO				"`To'"
%token K_TRUE			"`True'"
%token K_TRY			"`Try'"
%token K_TYPEOF			"`TypeOf'"
%token K_UNICODE		"`Unicode'"
%token K_UNTIL			"`Until'"
%token K_VARIANT		"`Variant'"
%token K_WHEN			"`When'"
%token K_WHILE			"`While'"
%token K_WITH			"`With'"
%token K_WITHEVENTS		"`WithEvents'"
%token K_WRITEONLY		"`WriteOnly'"
%token K_XOR			"`Xor'"

/*
 * Define the yylval types of the various non-terminals.
 */
%type <name>		IDENTIFIER TYPED_IDENTIFIER OptionValue LabelName
%type <node>		QualifiedIdentifier QualifiedIdentifier2
%type <node>		Identifier IdentifierOrKeyword
%type <integer>		INTEGER_CONSTANT CHAR_LITERAL
%type <real>		FLOAT_CONSTANT
%type <decimal>		DECIMAL_CONSTANT
%type <string>		STRING_LITERAL
%type <rank>		ArrayRank RankList

%type <node>		TypeName PredefinedType CastTarget
%type <node>		ArgumentList PositionalArgumentList NamedArgumentList
%type <node>		ArgumentExpression Implements ImplementsClause

%type <node>		Expression ConstantExpression BooleanExpression
%type <node>		OrExpression AndExpression NotExpression
%type <node>		RelationalExpression ShiftExpression
%type <node>		ConcatenationExpression AdditiveExpression
%type <node>		ModExpression IntegerDivisionExpression
%type <node>		MultiplicativeExpression UnaryExpression
%type <node>		PowerExpression PrimaryExpression VariableExpression
%type <node>		ObjectCreationExpression DateLiteral

%type <node>		OptBlock Block OptLineBlock LineBlock Statement
%type <node>		InnerStatement LineStatement InnerLineStatement
%type <node>		ControlFlowStatement AssignmentStatement
%type <node>		IfStatement ElseIfStatements ElseIfStatementList
%type <node>		ElseIfStatement ElseStatement InvocationStatement
%type <node>		LoopStatement StepExpression LetStatement
%type <node>		ExceptionHandlingStatement LabeledStatement
%type <ifrest>		IfRest
%type <loopvar>		LoopControlVariable

%type <node>		SubBody FunctionBody GetBody SetBody

%type <date>		DateValue TimeValue
%type <ampm>		AmPm
%type <modifier>	OptModifiers ModifierList Modifier

%type <node>		TypeDeclaration NonModuleDeclaration EnumDeclaration
%type <node>		StructureDeclaration InterfaceDeclaration
%type <node>		ClassDeclaration DelegateDeclaration ModuleDeclaration
%type <node>		VariableMemberDeclaration ConstantMemberDeclaration
%type <node>		EventMemberDeclaration MethodMemberDeclaration
%type <node>		PropertyMemberDeclaration EnumBase EnumBaseType
%type <node>		EnumBody EnumMemberList EnumMember ClassBase
%type <node>		InterfaceBases SubDeclaration FunctionDeclaration
%type <node>		ExternalMethodDeclaration ExternalSubDeclaration
%type <node>		ExternalFunctionDeclaration MethodDeclaration

%type <node>		FormalParameters FormalParameterList FormalParameter
%type <node>		ParameterType ParameterDefaultValue ParametersOrType

%type <member>		StructBody StructMemberList StructMember
%type <member>		ConstructorMemberDeclaration InterfaceBody
%type <member>		InterfaceMemberList InterfaceMember

%type <node>		OptAttributes AttributeList Attribute
%type <node>		AttributeArgsWithBrackets AttributeArguments
%type <node>		AttributePositionalArguments AttributeNamedArguments
%type <node>		AttributeNamedArgument AttributeModifier

%type <arrayName>	ArrayNameModifier InitializationRankList
%type <typeInfo>	ReturnType
%type <paramMod>	OptParameterModifiers ParameterModifierList
%type <paramMod>	ParameterModifier

%expect 45

%start File
%%

/*
 * Top level of a source file.
 */

File
	: Options Imports NamespaceBody	{
				/* Reset the parse state for the next file */
				ResetState();
			}
	;

Options
	: /* empty */
	| OptionList
	;

OptionList
	: OptionList Option
	| Option
	;

Option
	: K_OPTION Identifier OptionValue END_LINE	{
				const char *name = ILQualIdentName($2, 0);
				const char *value = $3;
				int invalidValue = 0;
				InitGlobalNamespace();
				if(!ILStrICmp(name, "Explicit"))
				{
					if(!value || !ILStrICmp(value, "On"))
					{
						VBOptionSet(GlobalNamespaceNode,
									VB_OPTION_EXPLICIT, 1);
					}
					else if(!ILStrICmp(value, "Off"))
					{
						VBOptionSet(GlobalNamespaceNode,
								    VB_OPTION_EXPLICIT, 0);
					}
					else
					{
						invalidValue = 1;
					}
				}
				else if(!ILStrICmp(name, "Strict"))
				{
					if(!value || !ILStrICmp(value, "On"))
					{
						VBOptionSet(GlobalNamespaceNode,
									VB_OPTION_STRICT, 1);
					}
					else if(!ILStrICmp(value, "Off"))
					{
						VBOptionSet(GlobalNamespaceNode,
									VB_OPTION_STRICT, 0);
					}
					else
					{
						invalidValue = 1;
					}
				}
				else if(!ILStrICmp(name, "Compare"))
				{
					if(!value)
					{
						CCError(_("missing value for `%s' option"), name);
					}
					else if(!ILStrICmp(value, "Binary"))
					{
						VBOptionSet(GlobalNamespaceNode,
									VB_OPTION_BINARY_COMPARE, 1);
					}
					else if(!ILStrICmp(value, "Text"))
					{
						VBOptionSet(GlobalNamespaceNode,
									VB_OPTION_BINARY_COMPARE, 0);
					}
					else
					{
						invalidValue = 1;
					}
				}
				else
				{
					CCError(_("unknown option `%s'"), name);
				}
				if(invalidValue)
				{
					CCError(_("invalid value `%s' for `%s' option"),
							name, value);
				}
			}
	;

OptionValue
	: /* empty */		{ $$ = 0; }
	| Identifier		{ $$ = ILQualIdentName($1, 0); }
	| K_ON				{ $$ = "On"; }
	;

Imports
	: /* empty */
	| ImportList
	;

ImportList
	: ImportList Import
	| Import
	;

Import
	: K_IMPORTS ImportClauses END_LINE
	;

ImportClauses
	: ImportClauses ',' ImportClause
	| ImportClause
	;

ImportClause
	: QualifiedIdentifier		{
				ILScope *globalScope = GlobalScope();
				ILNode_UsingNamespace *using;
				const char *name = ILQualIdentName($1, 0);
				if(!ILScopeUsing(globalScope, name))
				{
					CCError("`%s' is not a namespace", name);
				}
				InitGlobalNamespace();
				if(!HaveUsingNamespace(name))
				{
					using = (ILNode_UsingNamespace *)
						ILNode_UsingNamespace_create(name);
					using->next = CurrNamespaceNode->using;
					CurrNamespaceNode->using = using;
				}
			}
	| Identifier '=' QualifiedIdentifier	{
				ILNode *alias;
				const char *name;

				InitGlobalNamespace();
				name = ILInternString(ILQualIdentName($1, 0), -1).string;
				if((alias = ILNamespaceResolveAlias(CurrNamespaceNode, name, 0)))
				{
					CCError("the alias `%s' is already declared", name);
				}
				else
				{
					alias = ILNode_UsingAlias_create(name, $3);
					ILNamespaceAddAlias(CurrNamespaceNode, (ILNode_Alias *)alias);
				}
			}
	;

/*
 * Identifiers.
 */

Identifier
	: IDENTIFIER				{ $$ = ILQualIdentSimple($1); }
	| TYPED_IDENTIFIER			{ $$ = ILQualIdentSimple($1);
								  /* TODO: strip type suffix */ }
	;

IdentifierOrKeyword
	: Identifier				{ $$ = $1; /* TODO: keywords */ }
	;

QualifiedIdentifier
	: QualifiedIdentifier '.' IdentifierOrKeyword	{
				$$ = ILNode_QualIdent_create($1, ILQualIdentGetName($3));
			}
	| Identifier
	;

QualifiedIdentifier2
	: QualifiedIdentifier2 '.' IDENTIFIER	{
				$$ = ILNode_QualIdent_create($1, $3);
			}
	| Identifier
	;

/*
 * Attributes.
 */

OptAttributes
	: /* empty */				{ $$ = 0; }
	| '<' AttributeList '>'		{ $$ = ILNode_AttributeTree_create($2); }
	;

AttributeList
	: AttributeList Attribute	{
				$$ = $1;
				if($2)
				{
					ILNode_List_Add($1, $2);
				}
			}
	| Attribute	{
				$$ = ILNode_List_create();
				if($1)
				{
					ILNode_List_Add($$, $1);
				}
			}
	;

Attribute
	: AttributeModifier TypeName AttributeArgsWithBrackets	{
				ILNode *attr;
				ILNode *list;

				/* Create the attribute */
				attr = ILNode_Attribute_create($2, $3);

				/* Wrap it in a list node */
				list = ILNode_List_create();
				ILNode_List_Add(list, attr);

				/* Wrap the list in an attribute section node */
				if($1)
				{
					$$ = ILNode_AttributeSection_create
							(ILAttrTargetType_Named, $1, list);
				}
				else
				{
					$$ = ILNode_AttributeSection_create
							(ILAttrTargetType_None, 0, list);
				}
			}
	;

AttributeModifier
	: /* empty */			{ $$ = 0; }
	| K_ASSEMBLY ':'		{
				$$ = ILQualIdentSimple(ILInternString("assembly", 8).string);
			}
	| K_MODULE ':'			{
				$$ = ILQualIdentSimple(ILInternString("module", 6).string);
			}
	;

AttributeArgsWithBrackets
	: /* empty */					{ $$ = 0; }
	| '(' ')'						{ $$ = 0; }
	| '(' AttributeArguments ')'	{ $$ = $2; }
	;

AttributeArguments
	: AttributePositionalArguments	{
				$$ = ILNode_AttrArgs_create($1, 0);
			}
	| AttributePositionalArguments ',' AttributeNamedArguments	{
				$$ = ILNode_AttrArgs_create($1, $3);
			}
	| AttributeNamedArguments	{
				$$ = ILNode_AttrArgs_create(0, $1);
			}
	;

AttributePositionalArguments
	: AttributePositionalArguments ',' ConstantExpression	{
				$$ = $1;
				ILNode_List_Add($$, $3);
			}
	| ConstantExpression		{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	;

AttributeNamedArguments
	: AttributeNamedArguments ',' AttributeNamedArgument	{
				$$ = $1;
				ILNode_List_Add($$, $3);
			}
	| AttributeNamedArgument	{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	;

AttributeNamedArgument
	: Identifier ':' '=' ConstantExpression		{
				$$ = ILNode_NamedArg_create($1, $4);
			}
	;

/*
 * Type names.
 */

TypeName
	: QualifiedIdentifier
	| PredefinedType
	| TypeName '(' ArrayRank ')'	{
				ILNode_ArrayType_create($1, $3);
			}
	;

PredefinedType
	: K_OBJECT	{ $$ = ILNode_SystemType_create("Object"); }
	| K_VARIANT	{ CCWarning(_("`Variant' is obsolete; use `Object' instead"));
				  $$ = ILNode_SystemType_create("Object"); }
	| K_STRING	{ $$ = ILNode_SystemType_create("String"); }
	| K_BOOLEAN	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_BOOLEAN); }
	| K_DATE	{ $$ = ILNode_SystemType_create("DateTime"); }
	| K_CHAR	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_CHAR); }
	| K_DECIMAL	{ $$ = ILNode_SystemType_create("Decimal"); }
	| K_BYTE	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_U1); }
	| K_SHORT	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I2); }
	| K_INTEGER	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I4); }
	| K_LONG	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I8); }
	| K_SINGLE	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_R4); }
	| K_DOUBLE	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_R8); }
	;

ArrayRank
	: /* empty */		{ $$ = 1; }
	| RankList			{ $$ = $1; }
	;

RankList
	: RankList ','		{ $$ = $1 + 1; }
	| ','				{ $$ = 2; }
	;

ArrayNameModifier
	: '(' ArrayRank ')'					{ $$.rank = $2; $$.exprs = 0; }
	| '(' InitializationRankList ')'	{ $$ = $2; }
	;

InitializationRankList
	: InitializationRankList ',' Expression	{
				$$.rank = $1.rank + 1;
				$$.exprs = ILNode_ArgList_create($1.exprs, $3);
			}
	| Expression		{
				$$.rank = 1;
				$$.exprs = $1;
			}
	;

/*
 * Namespaces.
 */

NamespaceDeclaration
	: K_NAMESPACE QualifiedIdentifier END_LINE {
				PushIntoNamespace(ILInternString(ILQualIdentName($2, 0), -1));
			} NamespaceBody K_END K_NAMESPACE END_LINE {
				PopOutOfNamespace(ILInternString(ILQualIdentName($2, 0), -1));
			}
	;

NamespaceBody
	: /* empty */
	| NamespaceMemberList
	;

NamespaceMemberList
	: NamespaceMemberList NamespaceMember
	| NamespaceMember
	;

NamespaceMember
	: NamespaceDeclaration
	| TypeDeclaration			{ CCPluginAddTopLevel($1); }
	;

TypeDeclaration
	: ModuleDeclaration
	| NonModuleDeclaration
	;

NonModuleDeclaration
	: EnumDeclaration
	| StructureDeclaration
	| InterfaceDeclaration
	| ClassDeclaration
	| DelegateDeclaration
	;

/*
 * Modifiers.
 */

OptModifiers
	: /* empty */			{ $$ = 0; }
	| ModifierList			{ $$ = $1; }
	;

ModifierList
	: ModifierList Modifier	{
				if(($1 & $2) != 0)
				{
					VBModifiersUsedTwice
						(yycurrfilename(), yycurrlinenum(), ($1 & $2));
				}
				$$ = ($1 | $2);
			}
	| Modifier			{ $$ = $1; }
	;

Modifier
	: K_PUBLIC			{ $$ = VB_MODIFIER_PUBLIC; }
	| K_PROTECTED		{ $$ = VB_MODIFIER_PROTECTED; }
	| K_FRIEND			{ $$ = VB_MODIFIER_FRIEND; }
	| K_PRIVATE			{ $$ = VB_MODIFIER_PRIVATE; }
	| K_SHADOWS			{ $$ = VB_MODIFIER_SHADOWS; }
	| K_MUSTINHERIT		{ $$ = VB_MODIFIER_MUST_INHERIT; }
	| K_NOTINHERITABLE	{ $$ = VB_MODIFIER_NOT_INHERITABLE; }
	| K_SHARED			{ $$ = VB_MODIFIER_SHARED; }
	| K_STATIC			{
				CCWarning(_("`Static' is obsolete; use `Shared' instead"));
				$$ = VB_MODIFIER_SHARED;
			}
	| K_OVERRIDABLE		{ $$ = VB_MODIFIER_OVERRIDABLE; }
	| K_NOTOVERRIDABLE	{ $$ = VB_MODIFIER_NOT_OVERRIDABLE; }
	| K_MUSTOVERRIDE	{ $$ = VB_MODIFIER_MUST_OVERRIDE; }
	| K_OVERRIDES		{ $$ = VB_MODIFIER_OVERRIDES; }
	| K_OVERLOADS		{ $$ = VB_MODIFIER_OVERLOADS; }
	| K_READONLY		{ $$ = VB_MODIFIER_READ_ONLY; }
	| K_WRITEONLY		{ $$ = VB_MODIFIER_WRITE_ONLY; }
	| K_WITHEVENTS		{ $$ = VB_MODIFIER_WITH_EVENTS; }
	| K_DEFAULT			{ $$ = VB_MODIFIER_DEFAULT; }
	;

/*
 * Enumerated type declarations.
 */

EnumDeclaration
	: OptAttributes OptModifiers K_ENUM Identifier EnumBase END_LINE	{
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($4, 0);
			}
		EnumBody K_END K_ENUM END_LINE	{
				ILNode *baseList;
				ILNode *bodyList;
				ILNode *fieldDecl;
				ILUInt32 attrs;

				/* Validate the modifiers */
				attrs = VBModifiersToTypeAttrs($4, $2, (NestingLevel > 1));

				/* Add extra attributes that enums need */
				attrs |= IL_META_TYPEDEF_SERIALIZABLE |
						 IL_META_TYPEDEF_SEALED;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Make sure that we have "Enum" in the base list */
				baseList = MakeSystemType("Enum");

				/* Add an instance field called "value__" to the body,
				   which is used to hold the enumerated value */
				bodyList = $8;
				if(!bodyList)
				{
					bodyList = ILNode_List_create();
				}
				fieldDecl = ILNode_List_create();
				ILNode_List_Add(fieldDecl,
					ILNode_FieldDeclarator_create
						(ILQualIdentSimple("value__"), 0));
				ILNode_List_Add(bodyList,
					ILNode_FieldDeclaration_create
						(0, IL_META_FIELDDEF_PUBLIC |
							IL_META_FIELDDEF_SPECIAL_NAME |
							IL_META_FIELDDEF_RT_SPECIAL_NAME, $5, fieldDecl));

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1,					/* OptAttributes */
							 attrs,					/* OptModifiers */
							 ILQualIdentName($4, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 baseList,				/* ClassBase */
							 bodyList,				/* EnumBody */
							 0);					/* StaticCtors */
				CloneLine($$, $4);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

EnumBase
	: /* empty */	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I4); }
	| K_AS EnumBaseType	{ $$ = $2; }
	;

EnumBaseType
	: K_BYTE		{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_U1); }
	| K_SHORT		{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I2); }
	| K_INTEGER		{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I4); }
	| K_LONG		{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I8); }
	;

EnumBody
	: /* empty */		{ $$ = 0; }
	| EnumMemberList	{ $$ = $1; }
	;

EnumMemberList
	: EnumMemberList EnumMember	{
				$$ = $1;
				ILNode_List_Add($1, $2);
			}
	| EnumMember	{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	;

EnumMember
	: OptAttributes Identifier '=' ConstantExpression END_LINE	 {
				$$ = ILNode_EnumMemberDeclaration_create($1, $2, $4);
			}
	| OptAttributes Identifier END_LINE	 {
				$$ = ILNode_EnumMemberDeclaration_create($1, $2, 0);
			}
	;

/*
 * Structure declarations.
 */

StructureDeclaration
	: OptAttributes OptModifiers K_STRUCTURE Identifier END_LINE
			ImplementsClause	{
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($4, 0);
			} StructBody K_END K_STRUCTURE END_LINE		{
				ILNode *baseList;
				ILUInt32 attrs;

				/* Validate the modifiers */
				attrs = VBModifiersToTypeAttrs($4, $2, (NestingLevel > 1));

				/* Add extra attributes that structs need */
				attrs |= IL_META_TYPEDEF_LAYOUT_SEQUENTIAL |
						 IL_META_TYPEDEF_SERIALIZABLE |
						 IL_META_TYPEDEF_SEALED;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Make sure that we have "ValueType" in the base list */
				baseList = MakeSystemType("ValueType");
				if($6 != 0)
				{
					baseList = ILNode_ArgList_create($6, baseList);
				}

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1,					/* OptAttributes */
							 attrs,					/* OptModifiers */
							 ILQualIdentName($4, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 baseList,				/* ClassBase */
							 ($8).body,				/* StructBody */
							 ($8).staticCtors);		/* StaticCtors */
				CloneLine($$, $4);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

ImplementsClause
	: /* empty */						{ $$ = 0; }
	| K_IMPLEMENTS Implements END_LINE	{ $$ = $2; }
	;

Implements
	: Implements ',' TypeName		{
				$$ = ILNode_ArgList_create($1, $3);
			}
	| TypeName
	;

StructBody
	: /* empty */				{ $$.body = 0; $$.staticCtors = 0; }
	| StructMemberList			{ $$ = $1; }
	;

StructMemberList
	: StructMember	{
				$$.body = MakeList(0, $1.body);
				$$.staticCtors = MakeList(0, $1.staticCtors);
			}
	| StructMemberList StructMember	{
				$$.body = MakeList($1.body, $2.body);
				$$.staticCtors = MakeList($1.staticCtors, $2.staticCtors);
			}
	;

StructMember
	: NonModuleDeclaration				{ $$.body = $1; $$.staticCtors = 0; }
	| VariableMemberDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| ConstantMemberDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| EventMemberDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| MethodMemberDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| PropertyMemberDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| ConstructorMemberDeclaration		{ $$ = $1; }
	;

/*
 * Class declarations.
 */

ClassDeclaration
	: OptAttributes OptModifiers K_CLASS Identifier END_LINE
			ClassBase ImplementsClause	{
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($4, 0);
			}
			StructBody K_END K_CLASS END_LINE	{
				ILNode *classBody = ($9).body;
				ILNode *classBase;

				/* Validate the modifiers */
				ILUInt32 attrs =
					VBModifiersToTypeAttrs($4, $2, (NestingLevel > 1));

				/* Exit the current nesting level */
				--NestingLevel;

				/* Combine the class base and interface information */
				if($6 && $7)
				{
					classBase = ILNode_ArgList_create($7, $6);
				}
				else if($6)
				{
					classBase = $6;
				}
				else
				{
					classBase = $7;
				}

				/* Determine if we need to add a default constructor */
				if(!ClassNameIsCtorDefined())
				{
					ILUInt32 ctorMods =
						(((attrs & IL_META_TYPEDEF_ABSTRACT) != 0)
							? VB_MODIFIER_PROTECTED : VB_MODIFIER_PUBLIC);
					ILNode *cname = ILQualIdentSimple
							(ILInternString(".ctor", 5).string);
					ILNode *body = ILNode_NewScope_create
							(ILNode_Compound_CreateFrom
								(ILNode_NonStaticInit_create(),
								 ILNode_InvocationExpression_create
									(ILNode_BaseInit_create(), 0)));
					ILNode *ctor = ILNode_MethodDeclaration_create
						  (0, VBModifiersToConstructorAttrs(cname, ctorMods, 0),
						   0 /* "void" */, cname, 0,
						   ILNode_Empty_create(), body);
					if(!classBody)
					{
						classBody = ILNode_List_create();
					}
					ILNode_List_Add(classBody, ctor);
				}

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1,					/* OptAttributes */
							 attrs,					/* OptModifiers */
							 ILQualIdentName($4, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 classBase,				/* ClassBase */
							 classBody,
							 ($9).staticCtors);
				CloneLine($$, $4);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

ClassBase
	: /* empty */						{ $$ = 0; }
	| K_INHERITS TypeName END_LINE		{ $$ = $2; }
	;

/*
 * Module declarations.
 */

ModuleDeclaration
	: OptAttributes OptModifiers K_MODULE Identifier END_LINE	{
				/* Enter a new nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($4, 1);
			}
			StructBody K_END K_MODULE END_LINE	{
				ILNode *classBody = ($7).body;

				/* Validate the modifiers */
				ILUInt32 attrs =
					VBModifiersToTypeAttrs($4, $2, (NestingLevel > 1));

				/* Modules are always sealed */
				attrs |= IL_META_TYPEDEF_SEALED;

				/* Exit the current nesting level */
				--NestingLevel;

				/* Determine if we need to add a default constructor */
				if(!ClassNameIsCtorDefined())
				{
					ILUInt32 ctorMods = VB_MODIFIER_PRIVATE;
					ILNode *cname = ILQualIdentSimple
							(ILInternString(".ctor", 5).string);
					ILNode *body = ILNode_NewScope_create
							(ILNode_Compound_CreateFrom
								(ILNode_NonStaticInit_create(),
								 ILNode_InvocationExpression_create
									(ILNode_BaseInit_create(), 0)));
					ILNode *ctor = ILNode_MethodDeclaration_create
						  (0, VBModifiersToConstructorAttrs(cname, ctorMods, 0),
						   0 /* "void" */, cname, 0,
						   ILNode_Empty_create(), body);
					if(!classBody)
					{
						classBody = ILNode_List_create();
					}
					ILNode_List_Add(classBody, ctor);
				}
				else
				{
					/* Modules are not supposed to have constructors */
					CCErrorOnLine(yygetfilename($4), yygetlinenum($4),
							 _("modules cannot have instance constructors"));
				}

				/* TODO: add the "StandardModule" attribute to the class */

				/* Create the class definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1,					/* OptAttributes */
							 attrs,					/* OptModifiers */
							 ILQualIdentName($4, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 0,						/* ClassBase */
							 classBody,
							 ($7).staticCtors);
				CloneLine($$, $4);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

/*
 * Interface declarations.
 */

InterfaceDeclaration
	: OptAttributes OptModifiers K_INTERFACE Identifier END_LINE
			InterfaceBases 	{
				/* Increase the nesting level */
				++NestingLevel;

				/* Push the identifier onto the class name stack */
				ClassNamePush($4, 0);
			}
			InterfaceBody K_END K_INTERFACE END_LINE	{
				/* Validate the modifiers */
				ILUInt32 attrs =
					VBModifiersToTypeAttrs($4, $2, (NestingLevel > 1));

				/* Add extra attributes that interfaces need */
				attrs |= IL_META_TYPEDEF_INTERFACE |
						 IL_META_TYPEDEF_ABSTRACT;

				/* Exit from the current nesting level */
				--NestingLevel;

				/* Create the interface definition */
				InitGlobalNamespace();
				$$ = ILNode_ClassDefn_create
							($1,					/* OptAttributes */
							 attrs,					/* OptModifiers */
							 ILQualIdentName($4, 0),/* Identifier */
							 CurrNamespace.string,	/* Namespace */
							 (ILNode *)CurrNamespaceNode,
							 0,						/* TypeFormals */
							 $6,					/* ClassBase */
							 ($8).body,				/* InterfaceBody */
							 ($8).staticCtors);		/* StaticCtors */
				CloneLine($$, $4);

				/* Pop the class name stack */
				ClassNamePop();

				/* We have declarations at the top-most level of the file */
				HaveDecls = 1;
			}
	;

InterfaceBases
	: /* empty */						{ $$ = 0; }
	| K_INHERITS Implements END_LINE	{ $$ = $2; }
	;

InterfaceBody
	: /* empty */				{ $$.body = 0; $$.staticCtors = 0; }
	| InterfaceMemberList		{ $$ = $1; }
	;

InterfaceMemberList
	: InterfaceMember	{
				$$.body = MakeList(0, $1.body);
				$$.staticCtors = MakeList(0, $1.staticCtors);
			}
	| InterfaceMemberList InterfaceMember	{
				$$.body = MakeList($1.body, $2.body);
				$$.staticCtors = MakeList($1.staticCtors, $2.staticCtors);
			}
	;

InterfaceMember
	: NonModuleDeclaration			{ $$.body = $1; $$.staticCtors = 0; }
	| EventMemberDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| MethodMemberDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	| PropertyMemberDeclaration		{ $$.body = $1; $$.staticCtors = 0; }
	;

/*
 * Delegate declarations.
 */

DelegateDeclaration
	: OptAttributes OptModifiers K_DELEGATE MethodDeclaration	{
				/* TODO */
			}
	;

/*
 * Method declarations.
 */

MethodMemberDeclaration
	: MethodDeclaration
	| ExternalMethodDeclaration
	;

MethodDeclaration
	: SubDeclaration
	| FunctionDeclaration
	;

ExternalMethodDeclaration
	: ExternalSubDeclaration
	| ExternalFunctionDeclaration
	;

SubDeclaration
	: OptAttributes OptModifiers K_SUB Identifier FormalParameters
			HandlesOrImplements END_LINE SubBody	{
				ILUInt32 attrs;
				attrs = VBModifiersToMethodAttrs
						($4, $2, ClassNameIsModule());
				$$ = ILNode_MethodDeclaration_create
						($1, attrs, ILNode_PrimitiveType_create
										(IL_META_ELEMTYPE_VOID),
						 $4, 0, $5, $8);
				CloneLine($$, $4);
				/* TODO: HandlesOrImplements clause */
			}
	;

SubBody
	: /* empty */						{ $$ = 0; }
	| Block K_END K_SUB END_LINE		{ $$ = $1; }
	| K_END K_SUB END_LINE				{ $$ = ILNode_Empty_create(); }
	;

FunctionDeclaration
	: OptAttributes OptModifiers K_FUNCTION Identifier FormalParameters
			ReturnType HandlesOrImplements END_LINE FunctionBody {
				ILUInt32 attrs;
				ILNode *funcattrs;
				attrs = VBModifiersToMethodAttrs
						($4, $2, ClassNameIsModule());
				funcattrs = CombineAttributes
					($1, ILAttrTargetType_Return, $6.attrs);
				$$ = ILNode_MethodDeclaration_create
						(funcattrs, attrs, $6.type, $4, 0, $5, $9);
				CloneLine($$, $4);
				/* TODO: HandlesOrImplements clause */
			}
	;

FunctionBody
	: /* empty */						{ $$ = 0; }
	| Block K_END K_FUNCTION END_LINE	{ $$ = $1; }
	| K_END K_FUNCTION END_LINE			{ $$ = ILNode_Empty_create(); }
	;

ExternalSubDeclaration
	: OptAttributes OptModifiers K_DECLARE CharsetModifier K_SUB Identifier
			LibraryClause AliasClause FormalParameters END_LINE {
				ILUInt32 attrs;
				attrs = VBModifiersToMethodAttrs
						($6, $2, ClassNameIsModule());
				attrs |= CS_SPECIALATTR_EXTERN;
				$$ = ILNode_MethodDeclaration_create
						($1, attrs, ILNode_PrimitiveType_create
										(IL_META_ELEMTYPE_VOID),
						 $6, 0, $9, 0);
				CloneLine($$, $6);
				/* TODO: PInvoke information */
			}
	;

ExternalFunctionDeclaration
	: OptAttributes OptModifiers K_DECLARE CharsetModifier K_FUNCTION
			Identifier LibraryClause AliasClause FormalParameters
			ReturnType END_LINE {
				ILUInt32 attrs;
				ILNode *funcattrs;
				attrs = VBModifiersToMethodAttrs
						($6, $2, ClassNameIsModule());
				attrs |= CS_SPECIALATTR_EXTERN;
				funcattrs = CombineAttributes
					($1, ILAttrTargetType_Return, $10.attrs);
				$$ = ILNode_MethodDeclaration_create
						(funcattrs, attrs, $10.type, $6, 0, $9, 0);
				CloneLine($$, $6);
				/* TODO: PInvoke information */
			}
	;

ReturnType
	: /* empty */		{
				$$.type = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_VOID);
				$$.attrs = 0;
			}
	| K_AS OptAttributes TypeName	{
				$$.type = $3;
				$$.attrs = $2;
			}
	;

FormalParameters
	: /* empty */						{ $$ = ILNode_Empty_create(); }
	| '(' ')'							{ $$ = ILNode_Empty_create(); }
	| '(' FormalParameterList ')'		{ $$ = $2; }
	;

FormalParameterList
	: FormalParameterList ',' FormalParameter	{
				$$ = $1;
				ILNode_List_Add($1, $3);
			}
	| FormalParameter	{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}
	;

FormalParameter
	: OptAttributes OptParameterModifiers Identifier ParameterType
			ParameterDefaultValue {
				$$ = ILNode_FormalParameter_create($1, $2, $4, $3);
				/* TODO: default value handling */
			}
	;

OptParameterModifiers
	: /* empty */				{ $$ = ILParamMod_empty; }
	| ParameterModifierList		{ $$ = $1; }
	;

ParameterModifierList
	: ParameterModifierList ParameterModifier	{
				if($1 == ILParamMod_empty)
				{
					$$ = $2;
				}
				else if($2 == ILParamMod_empty || $1 == $2)
				{
					$$ = $1;
				}
				else
				{
					CCError(_("conflicting parameter modifiers"));
				}
			}
	| ParameterModifier		{ $$ = $1; }
	;

ParameterModifier
	: K_BYREF			{ $$ = ILParamMod_ref; }
	| K_BYVAL			{ $$ = ILParamMod_empty; }
	| K_OPTIONAL		{ $$ = ILParamMod_empty;	/* TODO */ }
	| K_PARAMARRAY		{ $$ = ILParamMod_params; }
	;

ParameterType
	: /* empty */		{ $$ = ILNode_SystemType_create("Object"); }
	| K_AS TypeName		{ $$ = $2; }
	;

ParameterDefaultValue
	: /* empty */				{ $$ = 0; }
	| '=' ConstantExpression	{ $$ = $2; }
	;

HandlesOrImplements
	: /* empty */
	| K_HANDLES EventHandlerList
	| MethodImplementsClause
	;

OptMethodImplementsClause
	: /* empty */
	| MethodImplementsClause
	;

MethodImplementsClause
	: K_IMPLEMENTS MethodImplementsList
	;

EventHandlerList
	: EventHandlerList EventMemberSpecifier
	| EventMemberSpecifier
	;

EventMemberSpecifier
	: Identifier '.' QualifiedIdentifier2		{}
	| K_MYBASE '.' QualifiedIdentifier2			{}
	;

MethodImplementsList
	: MethodImplementsList MethodImplements
	| MethodImplements
	;

MethodImplements
	: TypeName '.' Identifier		{}
	;

CharsetModifier
	: /* empty */
	| K_ANSI
	| K_UNICODE
	| K_AUTO
	;

LibraryClause
	: K_LIB STRING_LITERAL
	;

AliasClause
	: /* empty */
	| K_ALIAS STRING_LITERAL
	;

/*
 * Constructor declarations.
 */

ConstructorMemberDeclaration
	: OptAttributes OptModifiers K_SUB K_NEW FormalParameters
			END_LINE SubBody	{
				ILUInt32 attrs =
					VBModifiersToConstructorAttrs
						($5, $2, ClassNameIsModule());
				ILNode *cname;
				ILNode *body = $7;
				if((attrs & IL_META_METHODDEF_STATIC) != 0)
				{
					cname = ILQualIdentSimple
								(ILInternString(".cctor", 6).string);
				}
				else
				{
					cname = ILQualIdentSimple
								(ILInternString(".ctor", 5).string);
					ClassNameCtorDefined();
				}
				if((attrs & IL_META_METHODDEF_STATIC) != 0)
				{
					$$.body = 0;
					$$.staticCtors = body;
				}
				else
				{
					$$.body = ILNode_MethodDeclaration_create
						  ($1, attrs, 0 /* "void" */, cname, 0, $5, body);
					CloneLine($$.body, $5);
					$$.staticCtors = 0;
				}
			}
	;

/*
 * Event declarations.
 */

EventMemberDeclaration
	: OptAttributes OptModifiers K_EVENT Identifier ParametersOrType
			EventImplements END_LINE		{ /* TODO */ }
	;

ParametersOrType
	: FormalParameters			{ $$ = $1; }
	| K_AS TypeName				{ $$ = $2; }
	;

EventImplements
	: /* empty */
	| K_IMPLEMENTS MethodImplementsList
	;

/*
 * Constant declarations.
 */

ConstantMemberDeclaration
	: OptAttributes OptModifiers K_CONST Identifier ParameterType
			'=' ConstantExpression END_LINE		{
				ILUInt32 attrs = VBModifiersToConstAttrs
					($4, $2, ClassNameIsModule());
				ILNode *decl = ILNode_List_create();
				ILNode_List_Add
					(decl, ILNode_FieldDeclarator_create($4, $7));
				$$ = ILNode_FieldDeclaration_create($1, attrs, $5, decl);
			}
	;

/*
 * Variable member (field) declarations.
 */

VariableMemberDeclaration
	: OptAttributes OptModifiers OptDim VariableDeclarators END_LINE	{
				/* TODO */
				$$ = 0;
			}
	;

OptDim
	: /* empty */
	| K_DIM
	;

VariableDeclarators
	: VariableDeclarators ',' VariableDeclarator
	| VariableDeclarator
	;

VariableDeclarator
	: VariableIdentifier VarType
	| VariableIdentifier VarType '=' VariableInitializer
	;

VariableIdentifier
	: Identifier						{}
	| Identifier ArrayNameModifier		{}
	;

VarType
	: /* empty */
	| K_AS TypeName '(' ArgumentList ')'
	| K_AS TypeName
	| K_AS K_NEW TypeName '(' ArgumentList ')'
	| K_AS K_NEW TypeName
	;

VariableInitializer
	: Expression		{}
	| '{' '}'
	| '{' VariableInitializerList '}'
	;

VariableInitializerList
	: VariableInitializerList ',' VariableInitializer
	| VariableInitializer
	;

/*
 * Property declarations.
 */

PropertyMemberDeclaration
	: OptAttributes OptModifiers K_PROPERTY Identifier FormalParameters
			ReturnType OptMethodImplementsClause END_LINE PropertyBody	{
				/* TODO */
			}
	;

PropertyBody
	: /* empty */
	| PropertyAccessors K_END K_PROPERTY END_LINE
	| K_END K_PROPERTY END_LINE
	;

PropertyAccessors
	: Getter
	| Setter
	| Getter Setter
	| Setter Getter
	;

Getter
	: OptAttributes K_GET END_LINE GetBody	{}
	;

GetBody
	: /* empty */						{ $$ = 0; }
	| Block K_END K_GET END_LINE		{ $$ = $1; }
	| K_END K_GET END_LINE				{ $$ = ILNode_Empty_create(); }
	;

Setter
	: OptAttributes K_SET END_LINE SetBody	{}
	;

SetBody
	: /* empty */						{ $$ = 0; }
	| Block K_END K_SET END_LINE		{ $$ = $1; }
	| K_END K_SET END_LINE				{ $$ = ILNode_Empty_create(); }
	;

/*
 * Statements.
 */

OptBlock
	: /* empty */					{ $$ = ILNode_Empty_create(); }
	| Block							{ $$ = $1; }
	;

Block
	: Block Statement END_LINE		{
				$$ = ILNode_Compound_CreateFrom($1, $2);
			}
	| Statement END_LINE			{ $$ = $1; }
	;

OptLineBlock
	: /* empty */					{ $$ = ILNode_Empty_create(); }
	| LineBlock						{ $$ = $1; }
	;

LineBlock
	: LineBlock ':' LineStatement	{
				$$ = ILNode_Compound_CreateFrom($1, $3);
			}
	| LineStatement					{ $$ = $1; }
	;

Statement
	: InnerStatement			{
			#ifdef YYBISON
				if(debug_flag)
				{
					$$ = ILNode_LineInfo_create($1);
					yysetlinenum($$, @1.first_line);
				}
				else
			#endif
				{
					$$ = $1;
				}
			}
	;

LineStatement
	: InnerLineStatement			{
			#ifdef YYBISON
				if(debug_flag)
				{
					$$ = ILNode_LineInfo_create($1);
					yysetlinenum($$, @1.first_line);
				}
				else
			#endif
				{
					$$ = $1;
				}
			}
	;

InnerStatement
	: LabeledStatement
	| LocalDeclarationStatement			{ $$ = 0; /* TODO */ }
	| WithStatement						{ $$ = 0; /* TODO */ }
	| SyncLockStatement					{ $$ = 0; /* TODO */ }
	| RaiseEventStatement				{ $$ = 0; /* TODO */ }
	| AddHandlerStatement				{ $$ = 0; /* TODO */ }
	| RemoveHandlerStatement			{ $$ = 0; /* TODO */ }
	| AssignmentStatement
	| LetStatement
	| InvocationStatement
	| IfStatement
	| SelectStatement					{ $$ = 0; /* TODO */ }
	| LoopStatement
	| ExceptionHandlingStatement
	| TryStatement						{ $$ = 0; /* TODO */ }
	| ControlFlowStatement
	| ArrayHandlingStatement			{ $$ = 0; /* TODO */ }
	;

/* Statements that occupy a single line */
InnerLineStatement
	: LocalDeclarationStatement			{ $$ = 0; /* TODO */ }
	| RaiseEventStatement				{ $$ = 0; /* TODO */ }
	| AddHandlerStatement				{ $$ = 0; /* TODO */ }
	| RemoveHandlerStatement			{ $$ = 0; /* TODO */ }
	| AssignmentStatement
	| LetStatement
	| InvocationStatement
	| ExceptionHandlingStatement
	| ControlFlowStatement
	| ArrayHandlingStatement			{ $$ = 0; /* TODO */ }
	;

LabeledStatement
	: LabelName ':' Statement	{
				$$ = ILNode_Compound_CreateFrom
					(ILNode_GotoLabel_create($1), $3);
			}
	| LabelName ':'			{
				$$ = ILNode_GotoLabel_create($1);
			}
	;

LabelName
	: Identifier			{ $$ = ILQualIdentName($1, 0); /* TODO: case */ }
	| INTEGER_CONSTANT		{
				/* Convert the integer into a string so that we
				   can handle all labels as string-form names */
				char numbuf[32];
				sprintf(numbuf, "%lu", (unsigned long)($1.value));
				$$ = ILInternString(numbuf, -1).string;
			}
	;

LocalDeclarationStatement
	: K_DIM VariableDeclarator
	| K_CONST VariableDeclarator
	;

WithStatement
	: K_WITH Expression END_LINE OptBlock K_END K_WITH
	;

SyncLockStatement
	: K_SYNCLOCK Expression END_LINE OptBlock K_END K_SYNCLOCK
	;

RaiseEventStatement
	: K_RAISEEVENT PrimaryExpression
	;

AddHandlerStatement
	: K_ADDHANDLER HandlerArguments
	;

RemoveHandlerStatement
	: K_REMOVEHANDLER HandlerArguments
	;

HandlerArguments
	: ArgumentExpression ',' ArgumentExpression	{}
	;

AssignmentStatement
	: PrimaryExpression '=' Expression	{
				$$ = ILNode_Assign_create($1, $3);
			}
	| PrimaryExpression '=' K_ADDRESSOF PrimaryExpression	{
				/* TODO */
				$$ = 0;
			}
	| PrimaryExpression POW_ASSIGN_OP Expression	{
				/* TODO */
				$$ = 0;
			}
	| PrimaryExpression MUL_ASSIGN_OP Expression	{
				$$ = ILNode_AssignMul_create(ILNode_Mul_create($1, $3));
			}
	| PrimaryExpression DIV_ASSIGN_OP Expression	{
				$$ = ILNode_AssignDiv_create(ILNode_Div_create($1, $3));
			}
	| PrimaryExpression IDIV_ASSIGN_OP Expression	{
				/* TODO: integer division */
				$$ = ILNode_AssignDiv_create(ILNode_Div_create($1, $3));
			}
	| PrimaryExpression ADD_ASSIGN_OP Expression	{
				$$ = ILNode_AssignAdd_create(ILNode_Add_create($1, $3));
			}
	| PrimaryExpression SUB_ASSIGN_OP Expression	{
				$$ = ILNode_AssignSub_create(ILNode_Sub_create($1, $3));
			}
	| PrimaryExpression CONCAT_ASSIGN_OP Expression	{
				/* TODO: concat assign */
				$$ = ILNode_AssignAdd_create(ILNode_Add_create($1, $3));
			}
	;

LetStatement
	: K_LET AssignmentStatement		{
				CCWarning(_("`Let' is obsolete; it is no longer required"));
				$$ = $2;
			}
	;

VariableExpression
	: Expression
	;

InvocationStatement
	: PrimaryExpression				{
				/* TODO: wrap with a node that checks the expression
				   to make sure that it is a procedure call */
				$$ = $1;
			}
	| K_CALL PrimaryExpression		{
				/* TODO: wrap with a node that checks the expression
				   to make sure that it is a procedure or function call */
				$$ = $2;
			}
	| K_GOSUB PrimaryExpression		{
				CCWarning(_("`GoSub' is obsolete; it is no longer required"));
				/* TODO: wrap with a node that checks the expression
				   to make sure that it is a procedure or function call */
				$$ = $2;
			}
	;

IfStatement
	: K_IF BooleanExpression END_LINE IfRest	{
				$$ = ILNode_If_create($2, $4.thenClause, $4.elseClause);
			}
	| K_IF BooleanExpression K_THEN END_LINE IfRest	{
				$$ = ILNode_If_create($2, $5.thenClause, $5.elseClause);
			}
	| K_IF BooleanExpression K_THEN LineBlock	{
				$$ = ILNode_If_create($2, $4, ILNode_Empty_create());
			}
	| K_IF BooleanExpression K_THEN LineBlock K_ELSE OptLineBlock	{
				$$ = ILNode_If_create($2, $4, $6);
			}
	;

IfRest
	: OptBlock ElseIfStatements ElseStatement K_END K_IF	{
				$$.thenClause = $1;
				if($2)
				{
					$$.elseClause = $2;
					VBInsertElse($2, $3);
				}
				else
				{
					$$.elseClause = $3;
				}
			}
	;

ElseIfStatements
	: /* empty */					{ $$ = 0; }
	| ElseIfStatementList			{ $$ = $1; }
	;

ElseIfStatementList
	: ElseIfStatementList ElseIfStatement	{
				VBInsertElse($1, $2);
				$$ = $1;
			}
	| ElseIfStatement				{ $$ = $1; }
	;

ElseIfStatement
	: K_ELSEIF BooleanExpression END_LINE OptBlock	{
				$$ = ILNode_If_create($2, $4, 0);
			}
	| K_ELSEIF BooleanExpression K_THEN END_LINE OptBlock	{
				$$ = ILNode_If_create($2, $5, 0);
			}
	;

ElseStatement
	: /* empty */					{ $$ = ILNode_Empty_create(); }
	| K_ELSE END_LINE OptBlock		{ $$ = $3; }
	;

BooleanExpression
	: Expression			{ $$ = ILNode_ToBool_create($1); }
	;

/* TODO */
SelectStatement
	: K_SELECT Expression END_LINE SelectRest
	| K_SELECT K_CASE Expression END_LINE SelectRest
	;

SelectRest
	: CaseStatements CaseElseStatement K_END K_SELECT END_LINE
	;

CaseStatements
	: /* empty */
	| CaseStatementList
	;

CaseStatementList
	: CaseStatementList CaseStatement
	| CaseStatement
	;

CaseStatement
	: K_CASE CaseClauses END_LINE OptBlock
	;

CaseClauses
	: CaseClauses ',' CaseClause
	| CaseClause
	;

CaseClause
	: ComparisonOperator Expression
	| K_IS ComparisonOperator Expression
	| Expression					{}
	| Expression K_TO Expression	{}
	;

ComparisonOperator
	: '='
	| '<'
	| '>'
	| LE_OP
	| GE_OP
	| NE_OP
	;

CaseElseStatement
	: /* empty */
	| K_CASE K_ELSE OptBlock
	;

LoopStatement
	: K_WHILE BooleanExpression END_LINE OptBlock K_END K_WHILE	{
				$$ = ILNode_While_create($2, $4);
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("while", -1).string;
			}
	| K_DO K_WHILE BooleanExpression END_LINE OptBlock K_LOOP	{
				$$ = ILNode_While_create($3, $5);
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("do", -1).string;
			}
	| K_DO K_UNTIL BooleanExpression END_LINE OptBlock K_LOOP	{
				$$ = ILNode_While_create
					(ILNode_LogicalNot_create($3), $5);
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("do", -1).string;
			}
	| K_DO END_LINE OptBlock K_LOOP K_WHILE BooleanExpression	{
				$$ = ILNode_Do_create($3, $6);
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("do", -1).string;
			}
	| K_DO END_LINE OptBlock K_LOOP K_UNTIL BooleanExpression	{
				$$ = ILNode_Do_create
					($3, ILNode_LogicalNot_create($6));
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("do", -1).string;
			}
	| K_FOR LoopControlVariable '=' Expression K_TO Expression
		StepExpression END_LINE OptBlock K_NEXT NextExpression	{
				/* TODO */
				$$ = 0;
			/*
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("for", -1).string;
			*/
			}
	| K_FOR K_EACH LoopControlVariable K_IN Expression
		END_LINE K_NEXT NextExpression	{
				/* TODO */
				$$ = 0;
			/*
				((ILNode_LabelledStatement *)$$)->name =
					ILInternString("for", -1).string;
			*/
			}
	;

LoopControlVariable
	: Identifier K_AS TypeName		{ $$.name = $1; $$.type = $3; }
	| PrimaryExpression				{ $$.name = $1; $$.type = 0; }
	;

StepExpression
	: /* empty */					{ $$ = 0; }
	| K_STEP Expression				{ $$ = $2; }
	;

NextExpression
	: /* empty */
	| Identifier					{}
	;

ExceptionHandlingStatement
	: K_THROW						{ $$ = ILNode_Throw_create(); }
	| K_THROW Expression			{ $$ = ILNode_ThrowExpr_create($2); }
	| K_ERROR Expression			{ $$ = 0; /* TODO */ }
	| K_ON K_ERROR K_RESUME K_NEXT	{ $$ = 0; /* TODO */ }
	| K_ON K_ERROR K_GOTO '-' INTEGER_CONSTANT	{ $$ = 0; /* TODO */ }
	| K_ON K_ERROR K_GOTO LabelName	{ $$ = 0; /* TODO */ }
	| K_RESUME						{ $$ = 0; /* TODO */ }
	| K_RESUME K_NEXT				{ $$ = 0; /* TODO */ }
	| K_RESUME LabelName			{ $$ = 0; /* TODO */ }
	;

/* TODO */
TryStatement
	: K_TRY OptBlock CatchStatements FinallyStatement K_END K_TRY
	;

CatchStatements
	: /* empty */
	| CatchStatementList
	;

CatchStatementList
	: CatchStatementList CatchStatement
	| CatchStatement
	;

CatchStatement
	: K_CATCH CatchName CatchWhen END_LINE OptBlock
	;

CatchName
	: /* empty */
	| Identifier K_AS TypeName			{}
	;

CatchWhen
	: /* empty */
	| K_WHEN BooleanExpression
	;

FinallyStatement
	: /* empty */
	| K_FINALLY END_LINE OptBlock
	;

ControlFlowStatement
	: K_GOTO LabelName		{ $$ = ILNode_Goto_create($2); /* TODO: case */ }
	| K_EXIT K_DO				{
				$$ = ILNode_LabelledBreak_create
						(ILInternString("do", -1).string);
			}
	| K_EXIT K_FOR				{
				$$ = ILNode_LabelledBreak_create
						(ILInternString("for", -1).string);
			}
	| K_EXIT K_WHILE			{
				$$ = ILNode_LabelledBreak_create
						(ILInternString("while", -1).string);
			}
	| K_EXIT K_SELECT			{
				$$ = ILNode_LabelledBreak_create
						(ILInternString("select", -1).string);
			}
	| K_EXIT K_SUB				{ $$ = ILNode_Return_create(); }
	| K_EXIT K_FUNCTION			{ $$ = ILNode_Return_create(); }
	| K_EXIT K_TRY				{ $$ = 0; /* TODO */ }
	| K_STOP					{
				/* TODO: Call "System.Diagnostics.Debugger.Break()" */
				$$ = ILNode_Empty_create();
			}
	| K_END						{
				/* TODO: Call "Microsoft.VisualBasic.CompilerServices.
				   ProjectData.EndApp()" */
				$$ = ILNode_Empty_create();
			}
	| K_RETURN					{ $$ = ILNode_Return_create(); }
	| K_RETURN Expression		{ $$ = ILNode_ReturnExpr_create($2); }
	;

ArrayHandlingStatement
	: K_REDIM RedimClauses
	| K_REDIM K_PRESERVE RedimClauses
	| K_ERASE VariableExpressions
	;

RedimClauses
	: RedimClauses ',' RedimClause
	| RedimClause
	;

RedimClause
	: VariableExpression '(' InitializationRankList ')'	{}
	;

VariableExpressions
	: VariableExpressions ',' VariableExpression		{}
	| VariableExpression								{}
	;

/*
 * Expressions.
 */

PrimaryExpression
	: INTEGER_CONSTANT	{
				switch($1.type)
				{
					case CS_NUMTYPE_INT32:
					{
						if($1.value < 0)
						{
							$$ = ILNode_Int32_create
								((ILUInt64)(-($1.value)), 1, 1);
						}
						else
						{
							$$ = ILNode_Int32_create
								((ILUInt64)($1.value), 0, 1);
						}
					}
					break;

					case CS_NUMTYPE_INT64:
					default:
					{
						if($1.value < 0)
						{
							$$ = ILNode_Int64_create
								((ILUInt64)(-($1.value)), 1, 1);
						}
						else
						{
							$$ = ILNode_Int64_create
								((ILUInt64)($1.value), 0, 1);
						}
					}
					break;
				}
			}
	| FLOAT_CONSTANT	{
				if($1.type == CS_NUMTYPE_FLOAT32)
				{
					$$ = ILNode_Float32_create($1.value);
				}
				else
				{
					$$ = ILNode_Float64_create($1.value);
				}
			}
	| DECIMAL_CONSTANT	{
				$$ = ILNode_Decimal_create($1);
			}
	| CHAR_LITERAL		{
				$$ = ILNode_Char_create((ILUInt64)($1.value), 0, 1);
			}
	| STRING_LITERAL	{
				$$ = ILNode_String_create($1.string, $1.len);
			}
	| DateLiteral					{ $$ = $1; }
	| K_TRUE						{ $$ = ILNode_True_create(); }
	| K_FALSE						{ $$ = ILNode_False_create(); }
	| K_NOTHING						{ $$ = ILNode_Null_create(); }
	| Identifier
	| '(' Expression ')'			{ $$ = $2; }
	| K_ME							{ $$ = ILNode_This_create(); }
	| K_MYCLASS						{ $$ = 0; /* TODO */ }
	| K_MYBASE						{ $$ = 0; /* TODO */ }
	| K_GETTYPE '(' Expression ')'	{ $$ = ILNode_TypeOf_create($3); }
	| PrimaryExpression '(' ')'		{
				$$ = ILNode_InvocationExpression_create($1, 0);
			}
	| PrimaryExpression '(' ArgumentList ')'	{
				/* TODO: might actually be an array reference instead */
				$$ = ILNode_InvocationExpression_create($1, $3);
			}
	| PrimaryExpression '.' IdentifierOrKeyword	{
				$$ = ILNode_MemberAccess_create($1, $3);
			}
	| '.' IdentifierOrKeyword		{
				$$ = ILNode_MemberAccess_create(ILNode_This_create(), $2);
			}
	| ObjectCreationExpression
	| '!' IdentifierOrKeyword		{
				/* TODO */
				$$ = 0;
			}
	| PrimaryExpression '!' IdentifierOrKeyword	{
				/* TODO */
				$$ = 0;
			}
	| K_CTYPE '(' Expression ',' TypeName ')'	{
				$$ = ILNode_UserCast_create($5, $3);
			}
	| K_DIRECTCAST '(' Expression ',' TypeName ')'	{
				/* TODO: operand must be exactly the specified type */
				$$ = ILNode_UserCast_create($5, $3);
			}
	| CastTarget '(' Expression ')'		{
				$$ = ILNode_UserCast_create($1, $3);
			}
	;

DateLiteral
	: '#' DateValue TimeValue '#'	{
				/* Create a "DateTime" instance from the tick value */
				ILInt64 ticks = $2 + $3;
				$$ = ILNode_ObjectCreationExpression_create
					(ILNode_SystemType_create("DateTime"),
					 ILNode_Int64_create((ILUInt64)ticks, 0, 0));
			}
	;

DateValue
	: /* empty */					{ $$ = 0; }
	| INTEGER_CONSTANT DateSeparator INTEGER_CONSTANT
			DateSeparator INTEGER_CONSTANT	{
				/* The "-freversed-dates" flag causes dates to be
				   interpreted as "DD/MM/YYYY" instead of "MM/DD/YYYY" */
				if(CCStringListContains(extension_flags, num_extension_flags,
										"reversed-dates"))
				{
					$$ = VBDate($3.value, $1.value, $5.value, $5.numDigits);
				}
				else
				{
					$$ = VBDate($1.value, $3.value, $5.value, $5.numDigits);
				}
			}
	;

DateSeparator
	: '-'
	| '/'
	;

TimeValue
	: /* empty */				{ $$ = 0; }
	| INTEGER_CONSTANT AmPm		{ $$ = VBTime($1.value, 0, 0, $2); }
	| INTEGER_CONSTANT ':' INTEGER_CONSTANT AmPm	{
				$$ = VBTime($1.value, $3.value, 0, $4);
			}
	| INTEGER_CONSTANT ':' INTEGER_CONSTANT ':' INTEGER_CONSTANT AmPm	{
				$$ = VBTime($1.value, $3.value, $5.value, $6);
			}
	;

AmPm
	: /* empty */			{ $$ = VB_TIME_UNSPEC; }
	| IDENTIFIER			{
				if(!ILStrICmp($1, "Am"))
				{
					$$ = VB_TIME_AM;
				}
				else if(!ILStrICmp($1, "Pm"))
				{
					$$ = VB_TIME_PM;
				}
				else
				{
					CCError(_("invalid AM/PM specifier"));
					$$ = VB_TIME_UNSPEC;
				}
			}
	;

ArgumentList
	: PositionalArgumentList ',' NamedArgumentList	{
				/* TODO: append the two lists */
				$$ = $1;
			}
	| PositionalArgumentList	{ $$ = $1; }
	| NamedArgumentList			{ $$ = $1; }
	;

PositionalArgumentList
	: PositionalArgumentList ',' ArgumentExpression	{
				$$ = ILNode_ArgList_create($1, $3);
			}
	| ArgumentExpression
	;

NamedArgumentList
	: NamedArgumentList ',' Identifier ':' '=' ArgumentExpression	{
				/* TODO */
				$$ = 0;
			}
	| Identifier ':' '=' ArgumentExpression	{
				/* TODO */
				$$ = 0;
			}
	;

ArgumentExpression
	: Expression
	| K_ADDRESSOF PrimaryExpression	{
				/* TODO */
				$$ = 0;
			}
	;

ObjectCreationExpression
	: K_NEW TypeName ArrayElementInitializer			{}
	| K_NEW TypeName '(' ')' ArrayElementInitializer	{}
	| K_NEW TypeName '(' ArgumentList ')' ArrayElementInitializer	{}
	;

ArrayElementInitializer
	: /* empty */
	| '{' '}'
	| '{' VariableInitializerList '}'
	;

CastTarget
	: K_CBOOL	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_BOOLEAN); }
	| K_CBYTE	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_U1); }
	| K_CCHAR	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_CHAR); }
	| K_CDATE	{ $$ = ILNode_SystemType_create("DateTime"); }
	| K_CDEC	{ $$ = ILNode_SystemType_create("Decimal"); }
	| K_CDBL	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_R8); }
	| K_CINT	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I4); }
	| K_CLNG	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I8); }
	| K_COBJ	{ $$ = ILNode_SystemType_create("Object"); }
	| K_CSHORT	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_I2); }
	| K_CSNG	{ $$ = ILNode_PrimitiveType_create(IL_META_ELEMTYPE_R4); }
	| K_CSTR	{ $$ = ILNode_SystemType_create("String"); }
	;

PowerExpression
	: PrimaryExpression
	| PowerExpression '^' UnaryExpression	{
				/* TODO */
				$$ = 0;
			}
	;

UnaryExpression
	: PowerExpression
	| '-' UnaryExpression	{ $$ = ILNode_UnaryPlus_create($2); }
	| '+' UnaryExpression	{ $$ = ILNode_Neg_create($2); }
	;

MultiplicativeExpression
	: UnaryExpression
	| MultiplicativeExpression '*' UnaryExpression		{
				$$ = ILNode_Mul_create($1, $3);
			}
	| MultiplicativeExpression '/' UnaryExpression		{
				$$ = ILNode_Div_create($1, $3);
			}
	;

IntegerDivisionExpression
	: MultiplicativeExpression
	| IntegerDivisionExpression '\\' MultiplicativeExpression	{
				/* TODO: integer division */
				$$ = ILNode_Div_create($1, $3);
			}
	;

ModExpression
	: IntegerDivisionExpression
	| MultiplicativeExpression K_MOD IntegerDivisionExpression	{
				$$ = ILNode_Rem_create($1, $3);
			}
	;

AdditiveExpression
	: ModExpression
	| AdditiveExpression '+' ModExpression			{
				$$ = ILNode_Add_create($1, $3);
			}
	| AdditiveExpression '-' ModExpression			{
				$$ = ILNode_Sub_create($1, $3);
			}
	;

ConcatenationExpression
	: AdditiveExpression
	| ConcatenationExpression '&' AdditiveExpression	{
				$$ = ILNode_Concat_create($1, $3);
			}
	;

ShiftExpression
	: ConcatenationExpression
	| ShiftExpression LEFT_OP ConcatenationExpression		{
				$$ = ILNode_Shl_create($1, $3);
			}
	| ShiftExpression RIGHT_OP ConcatenationExpression		{
				$$ = ILNode_Shr_create($1, $3);
			}
	;

RelationalExpression
	: ShiftExpression
	| RelationalExpression '=' ShiftExpression		{
				/* TODO: value, not object, equality */
				$$ = ILNode_Eq_create($1, $3);
			}
	| RelationalExpression EQ_OP ShiftExpression	{
				$$ = ILNode_Eq_create($1, $3);
			}
	| RelationalExpression NE_OP ShiftExpression	{
				$$ = ILNode_Ne_create($1, $3);
			}
	| RelationalExpression '<' ShiftExpression		{
				$$ = ILNode_Lt_create($1, $3);
			}
	| RelationalExpression '>' ShiftExpression		{
				$$ = ILNode_Gt_create($1, $3);
			}
	| RelationalExpression LE_OP ShiftExpression	{
				$$ = ILNode_Le_create($1, $3);
			}
	| RelationalExpression GE_OP ShiftExpression	{
				$$ = ILNode_Ge_create($1, $3);
			}
	| RelationalExpression K_IS ShiftExpression		{
				/* TODO: object identity */
				$$ = ILNode_Eq_create($1, $3);
			}
	| RelationalExpression K_LIKE ShiftExpression	{
				/* TODO */
				$$ = 0;
			}
	| K_TYPEOF Expression K_IS TypeName				{
				$$ = ILNode_IsUntyped_create($2, $4);
			}
	;

NotExpression
	: RelationalExpression
	| K_NOT NotExpression		{
				$$ = ILNode_Not_create($2);
			}
	;

AndExpression
	: NotExpression
	| AndExpression K_AND NotExpression		{
				$$ = ILNode_And_create($1, $3);
			}
	| AndExpression K_ANDALSO NotExpression	{
				$$ = ILNode_LogicalAnd_create($1, $3);
			}
	;

OrExpression
	: AndExpression
	| OrExpression K_OR AndExpression		{
				$$ = ILNode_Or_create($1, $3);
			}
	| OrExpression K_ORELSE AndExpression	{
				$$ = ILNode_LogicalOr_create($1, $3);
			}
	| OrExpression K_XOR AndExpression		{
				$$ = ILNode_Xor_create($1, $3);
			}
	;

Expression
	: OrExpression			{ $$ = $1; }
	;

ConstantExpression
	: Expression			{ $$ = ILNode_ToConst_create($1); }
	;
