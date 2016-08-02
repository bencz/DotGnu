%{
/*
 * java_grammar.y - Input file for Yacc that defines Java syntax
 *
 * Copyright (C) 2001 Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003 Gopal.V
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
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */
%}
%token INTEGER_LITERAL
%token FLOAT_LITERAL
%token CHAR_LITERAL
%token BOOLEAN_LITERAL
%token STRING_LITERAL
%token NULL_LITERAL
%token FALSE
%token TRUE

%token IDENTIFIER
%token DIMS

%token ABSTRACT
%token DEFAULT
%token IF
%token PRIVATE
%token THIS
%token BOOLEAN
%token DO
%token IMPLEMENTS
%token PROTECTED
%token THROW
%token BREAK
%token DOUBLE
%token IMPORT
%token PUBLIC
%token THROWS
%token BYTE
%token ELSE
%token INSTANCEOF
%token RETURN
%token TRANSIENT
%token CASE
%token EXTENDS
%token INT
%token SHORT
%token TRY
%token CATCH
%token FINAL
%token INTERFACE
%token STATIC
%token VOID
%token CHAR
%token FINALLY
%token LONG
%token STRICTFP
%token VOLATILE
%token CLASS
%token FLOAT
%token NATIVE
%token SUPER
%token WHILE
%token CONST
%token FOR
%token NEW
%token SWITCH
%token CONTINUE
%token GOTO
%token PACKAGE_KEY
%token SYNCHRONIZED

%token AND_OP
%token OR_OP

%token INC_OP DEC_OP
%token SHL_OP SHR_OP USHR_OP

%token L_OP G_OP LE_OP GE_OP EQ_OP NEQ_OP

%token ADD_ASSIGN_OP SUB_ASSIGN_OP MUL_ASSIGN_OP DIV_ASSIGN_OP
%token AND_ASSIGN_OP OR_ASSIGN_OP XOR_ASSIGN_OP MOD_ASSIGN_OP
%token SHR_ASSIGN_OP SHL_ASSIGN_OP USHR_ASSIGN_OP

%type <name> IDENTIFIER
%type <integer> INTEGER_LITERAL
%type <charValue> CHAR_LITERAL
%type <real> FLOAT_LITERAL
%type <string> STRING_LITERAL
%type <mask> Modifiers Modifier ModifiersOpt

%type <node> Identifier QualifiedIdentifier QualifiedIdentifierList
%type <node> PrimitiveType
%type <string> PackageOrImportIdentifier
%type <node> Literal
%type <count> Dims
%type <node> DimExpr DimExprs
%type <node> Expression
%type <node> Type TypeList
%type <node> ClassDeclaration InterfaceDeclaration
%type <node> InterfaceBody
%type <node> SuperOpt ExtendsOpt
%type <node> InterfaceBodyDeclarationOneOrMore InterfaceBodyDeclaration
%type <member> ClassBodyDeclarationOneOrMore ClassBodyDeclaration
%type <node> FieldDeclaration FieldDeclarators FieldDeclarator
%type <member> ClassBody ClassBodyOpt
%type <node> Block BlockStatements BlockStatement
%type <node> InstanceInitializer 
%type <node> MethodDeclaration ConstructorDeclaration 
%type <node> ThrowsOpt
%type <node> MethodBody
%type <node> ConstantDeclarator ConstantDeclaratorsList
%type <varInit> VariableDeclarators VariableDeclarator
%type <node> ClassOrInterfaceDeclaration
%type <node> InterfaceFieldDeclarator InterfaceMethodDeclarator
%type <node> LocalVariableDeclaration
%type <node> UnaryExpressionNotPlusMinus PostfixExpression UnaryExpression
%type <node> PreIncrementExpression PreDecrementExpression
%type <node> MultiplicativeExpression AdditiveExpression
%type <node> ShiftExpression RelationalExpression
%type <node> EqualityExpression
%type <node> AndExpression ExclusiveOrExpression InclusiveOrExpression
%type <node> ConditionalExpression ConditionalOrExpression ConditionalAndExpression
%type <node> Assignment ConstantExpression CastExpression
%type <node> PostIncrementExpression PostDecrementExpression 
%type <node> Primary RealPrimary
%type <node> ArrayCreationExpression ObjectCreationExpression
%type <node> Statement StatementExpression StatementExpressionList
%type <node> MethodInvocation
%type <node> CatchClauses CatchClause FinallyClause TryStatement
%type <node> WhileStatement DoStatement IfThenStatement 
%type <node> SwitchStatement SwitchBlock SwitchLabels SwitchBlockStatementGroup
%type <node> SwitchBlockStatementGroups
%type <node> ThrowStatement BreakStatement ContinueStatement ReturnStatement
%type <node> LabeledStatement
%type <node> ForStatement ForUpdate ForInit
%type <node> EmptyStatement SynchronizedStatement ExpressionStatement
%type <node> Arguments ExpressionList
%type <node> FieldAccess ArrayInitializer ArrayAccess
%type <node> VariableInitializer VariableInitializersList
%type <node> FormalParameters FormalParametersList FormalParameter
%type <node> LocalVariableType
%expect 3 


%{
#include <stdio.h>
#include "il_system.h"
#include "il_opcodes.h"
#include "il_meta.h"
#include "il_utils.h"

#include "java_internal.h"
#include "java_rename.h"

int yydebug;

/*
 * Imports from the lexical analyser.
 */

extern int yylex();

#ifdef YYTEXT_POINTER
extern char *java_text;
#else
extern char java_text[];
#endif

/*
 * Global code generator object.
 */
ILGenInfo JavaCodeGen;

/*
 * Global state used by the parser.
 */
static unsigned long NestingLevel = 0;
static ILIntString CurrPackage = {"", 0};
static ILNode_JPackage *CurrPackageNode = 0;
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
		ILScopeDeclareNamespace(CCCodeGen.globalScope, "java.lang");
		ILScopeUsing(CCCodeGen.globalScope, "java.lang");
		return CCCodeGen.globalScope;
	}
}

/*
 * Reset the global state ready for the next file to be parsed.
 */
static void ResetState(void)
{
	NestingLevel = 0;
	CurrPackage = ILInternString("", 0);
	CurrPackageNode = 0;
	HaveDecls = 0;
	ILScopeClearUsing(GlobalScope());
}

static void InitGlobalPackage()
{
	if(!CurrPackageNode)
	{
		CurrPackageNode = (ILNode_JPackage*)ILNode_JPackage_create(0);
	}
}

/*
 * Determine if the current file already has an "import"
 * declaration for a particular package.
 */
 
static int HaveImported(const char *name)
{
	ILNode_JImport *import = (ILNode_JImport*)CurrPackageNode->import;
	while(import != 0)
	{
		if(!strcmp(import->name, name))
		{
			return 1;
		}
		import = (ILNode_JImport*)import->next;
	}
	return 0;
}

/* make sure java.lang is always available */
static void ImportJavaLang()
{
	ILScope *globalScope = GlobalScope();
	InitGlobalPackage();
	if(!HaveImported("java.lang"))
	{
		ILNode_JImportPackage *import;
		import = (ILNode_JImportPackage*)
					ILNode_JImportPackage_create("java.lang");
		import->next = CurrPackageNode->import;
		CurrPackageNode->import=(ILNode*)import;
		ILScopeUsing(globalScope,"java.lang");
	}
}

static void yyerror(char *msg)
{
	CCPluginParseError(msg, java_text);
}


/*
 * Determine if an extension has been enabled using "-f".
 */
#define HaveExtension(name)	\
	(CSStringListContains(extension_flags, num_extension_flags, (name)))

/*
 * Make a simple node and put it into $$.
 */
#define MakeSimple(classSuffix)	\
	do	{	\
		yyval.node = \
			ILNode_##classSuffix##_create(); \
	} while (0)

/*
 * Make a unary node and put it into $$.
 */
#define MakeUnary(classSuffix,expr)	\
	do	{	\
		yyval.node = ILNode_##classSuffix##_create((expr)); \
	} while (0)

/*
 * Make a binary node and put it into $$.
 */
#define MakeBinary(classSuffix,expr1,expr2)	\
	do	{	\
		yyval.node = ILNode_##classSuffix##_create((expr1), (expr2)); \
	} while (0)

/*
 * Make a ternary node and put it into $$.
 */
#define MakeTernary(classSuffix,expr1,expr2,expr3)	\
	do	{	\
		yyval.node = ILNode_##classSuffix##_create((expr1), (expr2), (expr3)); \
	} while (0)

/*
 * Make a quaternary node and put it into $$.
 */
#define MakeQuaternary(classSuffix,expr1,expr2,expr3,expr4)	\
	do	{	\
		yyval.node = ILNode_##classSuffix##_create \
							((expr1), (expr2), (expr3), (expr4)); \
	} while (0)

/*
 * Make a system type name node.
 */
#define MakeSystemType(name)	(ILQualIdentTwo("System", #name))

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
 * Clone the filename/linenum information from one node to another.
 */
static void CloneLine(ILNode *dest, ILNode *src)
{
	yysetfilename(dest, yygetfilename(src));
	yysetlinenum(dest, yygetlinenum(src));
}

/*
 * Negate an integer node.
 */
static ILNode *NegateInteger(ILNode_Integer *node)
{
	if(node->canneg)
	{
		if(yyisa(node, ILNode_Int32))
		{
			node->isneg = !(node->isneg);
			return (ILNode *)node;
		}
		else if(yyisa(node, ILNode_UInt32))
		{
			return ILNode_Int32_create(node->value, 1, 0);
		}
		else if(yyisa(node, ILNode_Int64))
		{
			node->isneg = !(node->isneg);
			return (ILNode *)node;
		}
		else if(yyisa(node, ILNode_UInt64))
		{
			return ILNode_Int64_create(node->value, 1, 0);
		}
	}
	return ILNode_Neg_create((ILNode *)node);
}

/*
 * The class name stack, which is used to verify the names
 * of constructors and destructors against the name of their
 * enclosing classes.  Also used to check if a class has
 * had a constructor defined for it.
 */
static ILNode **classNameStack = 0;
static int     *classNameCtorDefined = 0;
static int classNameStackSize = 0;
static int classNameStackMax = 0;

/*
 * Push an item onto the class name stack.
 */
static void ClassNamePush(ILNode *name)
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
		classNameStackMax += 4;
	}
	classNameStack[classNameStackSize] = name;
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
 * Determine if an identifier is identical to
 * the top of the class name stack.
 */
static int ClassNameSame(ILNode *name)
{
	return (strcmp(((ILNode_Identifier *)name)->name,
	 ((ILNode_Identifier *)(classNameStack[classNameStackSize - 1]))->name)
	 			== 0);
}

%}

%union	{
	struct	{
	ILUInt64 value;
	int type;
	int canneg;
	}integer;
	struct	{
	ILDouble value;
	int type;
	}real;
	ILUInt16 charValue;
	ILIntString string;
	const char	*name;
	ILUInt32 count;
	ILUInt32 mask;
	ILNode	*node;
	struct
	{
		ILNode *body;
		ILNode *staticCtors;
	} member;
	struct
	{
		ILNode		   *decl;
		ILNode		   *init;
	} varInit;
}

%{

%}

%start CompilationUnit

%%

CompilationUnit
	: {
	} 
	ImportDeclarationZeroOrMore TypeDeclarationZeroOrMore	{
		ResetState();
	}
	| 
	PackageDeclaration	{
	}
	ImportDeclarationZeroOrMore TypeDeclarationZeroOrMore	{
		ResetState();
	}
	;

PackageDeclaration
	: PACKAGE_KEY 
	PackageOrImportIdentifier ';'	{ 
				int posn, len;
				posn = 0;
				while(posn < $2.len)
				{
					/* Extract the next identifier */
					if($2.string[posn] == '.')
					{
						++posn;
						continue;
					}
					len = 0;
					while((posn + len) < $2.len &&
	$2.string[posn + len] != '.')
					{
						++len;
					}

					/* Push a new identifier onto the end of the namespace */
					if(CurrPackage.len != 0)
					{
						CurrPackage = ILInternAppendedString
							(CurrPackage,
							 ILInternAppendedString
							 	(ILInternString(".", 1),
								 ILInternString($2.string + posn, len)));
					}
					else
					{
						CurrPackage = ILInternString($2.string + posn, len);
					}

					InitGlobalPackage();

					CurrPackageNode = (ILNode_JPackage*)ILNode_JPackage_create(
											CurrPackage.string);

					/* Declare the namespace within the global scope */
					ILScopeDeclareNamespace(GlobalScope(),
											CurrPackage.string);

					/* Move on to the next namespace component */
					posn += len;
				}
		/** NOTE: there is no enclosing packages idea here , so the
		Package is preserved till the ResetState()
		*/
	}
	| PACKAGE_KEY error ';' { 
		/* TODO */
	}
	| PACKAGE_KEY PackageOrImportIdentifier error	{
		/* TODO */
	}
	;

PackageOrImportIdentifier
	: IDENTIFIER	{  $$ = ILInternString($1,strlen($1)); }
	| PackageOrImportIdentifier '.' IDENTIFIER	{ 
				$$ = ILInternAppendedString
					($1, ILInternAppendedString
					 		(ILInternString(".", 1),
							 ILInternString($3, strlen($3))));
	}
	;

ImportDeclarationZeroOrMore
	: /* empty */ {}
	| ImportDeclarationZeroOrMore ImportDeclaration
	;

ImportDeclaration
	: IMPORT PackageOrImportIdentifier '.' IDENTIFIER ';'	{
	/* TODO: import only type , not the package 
		ILScope *globalScope = GlobalScope();
		ILNode_JImportType *import;
		ILIntString str= ILInternAppendedString
					($2, ILInternAppendedString
					 		(ILInternString(".", 1),
							 ILInternString($4, strlen($4))));
		if(!ILScopeUsing(globalScope, $2.string))
		{
			CCError("`%s' is not a pacakge", $2.string);
		}
		InitGlobalPackage();
		if(!HaveImported($2.string))
		{
			import = (ILNode_JImportType *)
				ILNode_JImportType_create(str.string);
			import->next = CurrPackageNode->import;
			CurrPackageNode->import = (ILNode*)import;
		}*/
		ILScope *globalScope = GlobalScope();
		ILNode_JImportPackage *import;
		if(!ILScopeUsing(globalScope, $2.string))
		{
			CCError("`%s' is not a pacakge", $2.string);
		}
		InitGlobalPackage();
		if(!HaveImported($2.string))
		{
			import = (ILNode_JImportPackage *)
				ILNode_JImportPackage_create($2.string);
			import->next = CurrPackageNode->import;
			CurrPackageNode->import = (ILNode*)import;
		}
	}
	| IMPORT PackageOrImportIdentifier '.' '*' ';'	{ 
		ILScope *globalScope = GlobalScope();
		ILNode_JImportPackage *import;
		if(!ILScopeUsing(globalScope, $2.string))
		{
			CCError("`%s' is not a pacakge", $2.string);
		}
		InitGlobalPackage();
		if(!HaveImported($2.string))
		{
			import = (ILNode_JImportPackage *)
				ILNode_JImportPackage_create($2.string);
			import->next = CurrPackageNode->import;
			CurrPackageNode->import = (ILNode*)import;
		}
	}
	| IMPORT error ';'{
	}
	| IMPORT PackageOrImportIdentifier '.' '*' error	{
	}
	| IMPORT PackageOrImportIdentifier error	{
	}

	;

PrimitiveType
	: BYTE	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_U1); }
	| SHORT	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_I2); }
	| CHAR	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_CHAR); }
	| INT	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_I4); }
	| LONG	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_I8); }
	| FLOAT	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_R4); }
	| DOUBLE	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_R8); }
	| BOOLEAN	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_BOOLEAN); }
	| VOID	{ MakeUnary(JPrimitiveType, IL_META_ELEMTYPE_VOID); }
	;

Literal
	: INTEGER_LITERAL	{
			switch($1.type)
			{
				case JAVA_NUMTYPE_INT32:
				{
					$$ = ILNode_Int32_create($1.value, 0, $1.canneg);
				}
				break;
				case JAVA_NUMTYPE_INT64:
				{
					$$ = ILNode_Int64_create($1.value, 0, $1.canneg);
				}
				break;
				default:
				{
					$$ = ILNode_Int64_create($1.value, 0, $1.canneg);
				}
				break;
			}
	}
	| FLOAT_LITERAL	{ 
				if($1.type == JAVA_NUMTYPE_FLOAT32)
				{
					$$ = ILNode_Float32_create($1.value);
				}
				else
				{
					$$ = ILNode_Float64_create($1.value);
				}
	}
	| CHAR_LITERAL	{ $$ = ILNode_Char_create((ILUInt64)($1), 0, 1); }
	| STRING_LITERAL	{ $$ = ILNode_String_create($1.string, $1.len); }
	| NULL_LITERAL	{ MakeSimple(Null); }
	| FALSE	{ MakeSimple(False); }
	| TRUE	{ MakeSimple(True); }
	;

Identifier
	: IDENTIFIER	{ 
	$$ = ILQualIdentSimple(ILInternString($1, strlen($1)).string);
	}
	;

QualifiedIdentifier
	: Identifier	{ $$ = $1; }
	| QualifiedIdentifier '.' IDENTIFIER	{
		MakeBinary(QualIdent, $1, ILInternString($3, strlen($3)).string);
	}
	;

QualifiedIdentifierList
	: QualifiedIdentifier	{ 
		$$ = ILNode_List_create();
		ILNode_List_Add($$, $1); 
	}
	| QualifiedIdentifierList ',' QualifiedIdentifier	{
		$$ = $1;
		ILNode_List_Add($$, $3);
	}
	;

TypeList
	: Type	{ $$ = $1; }
	| TypeList ',' Type	{ MakeBinary(ArgList, $1, $3); }
	;

Type
	: PrimitiveType	{
		$$ = $1;
	}
	| PrimitiveType Dims	{ 
		int i;
		ILNode *type=$1;
		for(i=0;i<$2;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		$$ = type;
	}
	| QualifiedIdentifier	{
		$$ = $1;
	}
	| QualifiedIdentifier Dims	{
		int i;
		ILNode *type=$1;
		for(i=0;i<$2;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		$$ = type;
	}
	;

LocalVariableType
	: PrimitiveType	{
		$$ = $1;
	}
	| PrimitiveType Dims	{ 
		int i;
		ILNode *type=$1;
		for(i=0;i<$2;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		$$ = type;
	}
	| FieldAccess
	{
		$$ = $1;
	}
	| FieldAccess Dims
	{
		int i;
		ILNode *type=$1;
		for(i=0;i<$2;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		$$ = type;
	}
	;

DimExprs
	: DimExpr	{ 
		$$ = $1;
	}
	| DimExprs DimExpr	{ 
		MakeBinary(ArgList,$1,$2);
	}
	;

DimExpr
	: '[' Expression ']'	{ $$=$2;}
	;

Dims
	: '[' ']'	{ $$ = 1; }
	| Dims '[' ']'  { $$ = $$ + 1; }
	;

ModifiersOpt
	: /* empty */ { $$ = 0; }
	| Modifiers	{ $$ = $1; }
	;

Modifiers
	: Modifier	{ $$ = $1; }
	| Modifiers Modifier	{ 
		if($1 & $2)
		{
			CCWarning("repeated modifier");
		}
		$$ = $1 | $2;
	}
	;

Modifier
	: PROTECTED	{ $$ = JAVA_MODIFIER_PROTECTED; }
	| PUBLIC	{ $$ = JAVA_MODIFIER_PUBLIC; }
	| PRIVATE	{ $$ = JAVA_MODIFIER_PRIVATE; }
	| STATIC	{ $$ = JAVA_MODIFIER_STATIC; }
	| ABSTRACT	{ $$ = JAVA_MODIFIER_ABSTRACT; }
	| FINAL	{ $$ = JAVA_MODIFIER_FINAL; }
	| NATIVE	{ $$ = JAVA_MODIFIER_NATIVE; }
	| SYNCHRONIZED	{ $$ = JAVA_MODIFIER_SYNCHRONIZED; }
	| TRANSIENT	{ $$ = JAVA_MODIFIER_TRANSIENT; }
	| VOLATILE	{ $$ = JAVA_MODIFIER_VOLATILE; }
	| STRICTFP	{ $$ = JAVA_MODIFIER_STRICTFP; }
	;

TypeDeclarationZeroOrMore
	: /* empty */ { 
		/* Note: I'm calling it here because this production will
		be used only once for a set of TypeDeclarations , sort of
		designed optimisation */
		ImportJavaLang(); 
	}  
	| TypeDeclarationZeroOrMore TypeDeclaration
	;

TypeDeclaration
	: ClassOrInterfaceDeclaration	{ CCPluginAddTopLevel($1);}
	| ';'
	;

FormalParameters
	: '(' FormalParametersList 	')' {$$ = $2; }
	| '(' 				')'               { $$ = NULL; }
	| '(' error ')' {
		CCError("bad formal parameters");
	}
	;

FormalParametersList
	: FormalParameter	{
		$$ = ILNode_List_create();
		ILNode_List_Add($$, $1);
	}
	| FormalParametersList ',' FormalParameter	{
		$$ = $1;
		ILNode_List_Add($$, $3);
	}
	;

FormalParameter
	: FINAL Type Identifier	{
		/* TODO */
	}
	| 	Type Identifier	{
		MakeQuaternary(FormalParameter,NULL, ILParamMod_empty, $1,$2);
	}
	| FINAL Type Identifier Dims	{
		/* TODO */
	}
	| 	Type Identifier Dims	{ 
		int i;
		ILNode *type=$1;
		for(i=0;i<$3;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		MakeQuaternary(FormalParameter,NULL,ILParamMod_empty, 
						type,$2);
	}
	;

ArrayInitializer
	: '{' 					'}'                     { $$ = NULL; }
	| '{' VariableInitializersList 		'}'   { $$ = $2; }
	| '{' VariableInitializersList ',' 	'}' { $$ = $2; }
	| '{' VariableInitializersList error	{
		CCError("missing '}' in array initializer");
	}

	;

VariableInitializersList
	: VariableInitializer	{
		$$ = ILNode_List_create();	  
		ILNode_List_Add($$, $1);
	}
	| VariableInitializersList ',' VariableInitializer	{
		$$ = $1;	  
		ILNode_List_Add($$, $3);
	}
	| error ',' VariableInitializer	{
		CCError("invalid variable initializer");
	}
	;

VariableInitializer
	: Expression
	| ArrayInitializer	{ MakeUnary(ArrayInit,$1); }
	;

ConstantDeclaratorsList
	: ConstantDeclarator	{
		$$ = ILNode_List_create();
		ILNode_List_Add($$, $1);
	}
	| ConstantDeclaratorsList ',' ConstantDeclarator	{
		ILNode_List_Add($1, $3);
		$$ = $1;
	}
	;

ConstantDeclarator
	: Identifier Dims '=' VariableInitializer	{
	}
	| Identifier 	  '=' VariableInitializer	{
		MakeBinary(FieldDeclarator, $1, $3);	
	}
	| Identifier Dims error VariableInitializer	{
		CCError("missing '=' in constant declarator");
	}
	| Identifier error VariableInitializer{
		CCError("missing '=' in constant declarator");
	}
	;

VariableDeclarators
	: VariableDeclarator	{
		$$.decl = ILNode_List_create();
		ILNode_List_Add($$.decl, $1.decl);
		$$.init = $1.init;
	}
	| VariableDeclarators ',' VariableDeclarator	{
		ILNode_List_Add($1.decl, $3.decl);
		$$.decl = $1.decl;
		if($1.init)
		{
			if($3.init)
			{
				$$.init = ILNode_Compound_CreateFrom($1.init, $3.init);
			}
			else
			{
				$$.init = $1.init;
			}
		}
		else if($3.init)
		{
			$$.init = $3.init;
		}
		else
		{
			$$.init = 0;
		}
	}
	;

VariableDeclarator
	: Identifier	{
		$$.decl = $1;
		$$.init = NULL;
	}
	| Identifier 	  '=' VariableInitializer	{
		$$.decl = $1;
		$$.init = ILNode_Assign_create($1,$3);
	}
	;

ClassOrInterfaceDeclaration
	: ClassDeclaration
	| InterfaceDeclaration
	;

ClassDeclaration
	: ModifiersOpt CLASS Identifier SuperOpt	{
		++NestingLevel;
		ClassNamePush($3);
	} ClassBody	{
		int currentModifiers = 0;
		ILNode *classBody = ($6).body;
		NestingLevel--;
		currentModifiers = JavaModifiersToTypeAttrs($3, $1, NestingLevel);
		InitGlobalPackage();
		if(!ClassNameIsCtorDefined())
		{
			ILUInt32 ctorMods =
				(((currentModifiers & IL_META_TYPEDEF_ABSTRACT) != 0)
					? JAVA_MODIFIER_PROTECTED : JAVA_MODIFIER_PUBLIC);
			ILNode *cname = ILQualIdentSimple
							(ILInternString(".ctor", 5).string);
			ILNode *body = ILNode_NewScope_create
							(ILNode_Compound_CreateFrom
								(ILNode_NonStaticInit_create(),
								 ILNode_InvocationExpression_create
									(ILNode_BaseInit_create(), 0)));
			ILNode *ctor = ILNode_MethodDeclaration_create
						  (0, JavaModifiersToConstructorAttrs(cname, ctorMods),
						   0 /* "void" */, cname, 0,
						   ILNode_Empty_create(), body);
			if(!classBody)
			{
				classBody = ILNode_List_create();
			}
			ILNode_List_Add(classBody, ctor);
		}
	
		$$ = ILNode_ClassDefn_create(NULL,	    /* attributes */
				currentModifiers,	            /* modifiers */
				ILQualIdentName($3, 0),         /* class name */
				CurrPackage.string,   			/* scope name */
				(ILNode*)CurrPackageNode,      	/* namespace node */
				NULL,							/* (generics) */
				$4,		                      	/* super class */
				classBody,	                   	/* class body */
				$6.staticCtors);				/* static ctors */

		ClassNamePop();

		HaveDecls = 1;
	}
	;

SuperOpt
	: /* empty */
	{
		$$ = 0;
	}
	| EXTENDS Type	{
		$$ = $2;
	}
	| IMPLEMENTS TypeList	{
		$$ = $2;
	}
	| EXTENDS Type IMPLEMENTS TypeList	{
		MakeBinary(ArgList,$4, $2);
	}
	;

ClassBodyOpt
	: /* empty */ { $$.body = NULL; $$.staticCtors = NULL;}
	| ClassBody	{ $$ = $1; }
	;

ClassBody 
	: '{' ClassBodyDeclarationOneOrMore '}' { $$ = $2; }
	| '{'  '}' { $$.body = NULL; $$.staticCtors = NULL; }
	;

ClassBodyDeclarationOneOrMore
	: ClassBodyDeclaration	{
		$$.body = MakeList(0,$1.body);
		$$.staticCtors = MakeList(0,$1.staticCtors);
	}
	| ClassBodyDeclarationOneOrMore ClassBodyDeclaration	{
		$$.body = MakeList($1.body, $2.body);
		$$.staticCtors = MakeList($1.staticCtors, $2.staticCtors);
	}
	;

ClassBodyDeclaration
	: ';'	{ $$.body = NULL; $$.staticCtors=NULL;} 
	| 		    InstanceInitializer	{ $$.body = NULL; $$.staticCtors = NULL;}
	| STATIC InstanceInitializer	{ }
	| ConstructorDeclaration	{ $$.body = $1; $$.staticCtors=NULL;} 
	| FieldDeclaration	{ $$.body = $1; $$.staticCtors=NULL;} 
	| MethodDeclaration	{ $$.body=$1; $$.staticCtors=NULL; }
	| ClassOrInterfaceDeclaration	{ $$.body = $1; $$.staticCtors=0; }
	;

ThrowsOpt
	: /* empty */ {$$ = NULL;} 
	|THROWS QualifiedIdentifierList	{$$ = $2;}
	;

InstanceInitializer
	: Block
	;

/*
 * Fields.
 */
FieldDeclaration
	: ModifiersOpt Type FieldDeclarators ';'	{
		ILUInt32 attrs = JavaModifiersToFieldAttrs($2, $1);
		$$ = ILNode_FieldDeclaration_create(NULL, attrs, $2, $3);
	}
	| ModifiersOpt error FieldDeclarators ';' {
		CCError("invalid variable type");
	}
	| ModifiersOpt Type FieldDeclarators error	{
		CCError("missing ';' after variable declarations");
	}
	;

FieldDeclarators
	: FieldDeclarator	{
				$$ = ILNode_List_create();
				ILNode_List_Add($$, $1);
			}	
	| FieldDeclarators ',' FieldDeclarator	{
				ILNode_List_Add($1, $3);
				$$ = $1;
			}
		
	;

FieldDeclarator
	: Identifier	{
				MakeBinary(FieldDeclarator, $1, 0);
			}
	| Identifier '=' VariableInitializer	{
				MakeBinary(FieldDeclarator, $1, $3);
			}
	| Identifier Dims
			{
				MakeBinary(FieldDeclarator, 
					ILNode_TypeSuffixDeclarator_create($1,$2), 0);
			}
	| Identifier Dims '=' VariableInitializer	{
				MakeBinary(FieldDeclarator, 
					ILNode_TypeSuffixDeclarator_create($1,$2), $4);
			}
	;

ConstructorDeclaration
	: ModifiersOpt Identifier FormalParameters ThrowsOpt MethodBody	{
		
		ILNode *cname;
		ILUInt32 attrs;
		cname =  ILQualIdentSimple( ILInternString(".ctor",5).string);
		ClassNameCtorDefined();
		if(!ClassNameSame($2))
		{
				CCErrorOnLine(yygetfilename($2), yygetlinenum($2),
					"constructor name does not match class name");
		}
		attrs=JavaModifiersToConstructorAttrs($2,$1);
		$$ = ILNode_MethodDeclaration_create (0,	/* attributes */
											attrs,		/* modifiers */
											0, 			/* "void" */
											cname,  	/* name */
											0,			/* typeFormals */
											$3, 		/* params */
											$5);		/* body */
		CloneLine($$, $2);
	}

	| ModifiersOpt Identifier FormalParameters error QualifiedIdentifierList	{
		CCError("missing 'throws'");
		yyerrok;
	}
	| ModifiersOpt Identifier FormalParameters THROWS error	{
		CCError("bad exception names");
		yyerrok;
	}
	;

MethodDeclaration
	: ModifiersOpt Type Identifier FormalParameters ThrowsOpt MethodBody	{ 
		ILNode *cname;
		ILUInt32 attrs;
		cname =  $3;
		attrs=JavaModifiersToMethodAttrs($3,$1);
		$$ = ILNode_MethodDeclaration_create (0,	/* attributes */
											attrs,		/* modifiers */
											$2,			/* retval */
											cname,  	/* name */
											0,			/* typeFormals */
											$4, 		/* params */
											$6);		/* body */
		CloneLine($$, $2);
	}
	| ModifiersOpt error Identifier FormalParameters	{
		CCError("invalid return type");
		yyerrok;
	}
	| ModifiersOpt Type Identifier FormalParameters error QualifiedIdentifierList	{
		CCError("missing 'throws'");
		yyerrok;
	}
	| ModifiersOpt Type Identifier FormalParameters THROWS error	{
		CCError("bad exception names");
		yyerrok;
	}

	;

InterfaceDeclaration
	: ModifiersOpt INTERFACE Identifier ExtendsOpt	{
		NestingLevel++;
		/* Push the identifier onto the class name stack */
		ClassNamePush($4);
	} InterfaceBody	{
		/* Validate the modifiers */
		ILUInt32 attrs =
			JavaModifiersToTypeAttrs($3, $1, (NestingLevel > 1));

		/* Add extra attributes that interfaces need */
		attrs |= IL_META_TYPEDEF_INTERFACE |
				 IL_META_TYPEDEF_ABSTRACT;
		InitGlobalPackage();
		$$ = ILNode_ClassDefn_create
					(NULL,					/* Attributes */
					 attrs,					/* OptModifiers */
					 ILQualIdentName($3, 0),/* Identifier */
					 CurrPackage.string,	/* Namespace */
					 (ILNode *)CurrPackageNode,
					 NULL,					/* TypeFormals */
					 $4,					/* ClassBase */
					 $6,					/* InterfaceBody */
					 0);					/* StaticCtors */
		CloneLine($$, $3);

		/* Pop the class name stack */
		ClassNamePop();


		/* Exit from the current nesting level */
		--NestingLevel;
	}
	;

ExtendsOpt
	: EXTENDS TypeList	{ $$ = $2; }
	| /* empty */ { $$ = NULL; }
	| error QualifiedIdentifierList	{
		CCError("missing 'extends'");
	}
	;

InterfaceBody
	: '{' InterfaceBodyDeclarationOneOrMore '}' { $$ = $2; }
	| '{' '}' { $$ = NULL; }
	;

InterfaceBodyDeclarationOneOrMore
	: InterfaceBodyDeclaration	{
		$$ = ILNode_List_create();
		ILNode_List_Add($$, $1);
	}
	| InterfaceBodyDeclarationOneOrMore InterfaceBodyDeclaration	{
		ILNode_List_Add($1, $2);
	}
	;

InterfaceBodyDeclaration
	: ';'	{ $$ = NULL; }
	| InterfaceFieldDeclarator
	| InterfaceMethodDeclarator
	| ClassOrInterfaceDeclaration
	;

InterfaceFieldDeclarator
	: ModifiersOpt Type ConstantDeclaratorsList ';'	{
		ILUInt32 attrs = JavaModifiersToFieldAttrs($2, $1);
		$$ = ILNode_FieldDeclaration_create(NULL, attrs, $2, $3);
	}
	| ModifiersOpt error ConstantDeclaratorsList ';' {
		CCError("invalid variable type");
	}
	| ModifiersOpt Type ConstantDeclaratorsList error	{
		CCError("missing ';' after variable declarations");
	}
	;

InterfaceMethodDeclarator
	: ModifiersOpt Type Identifier FormalParameters ';'	{
		ILUInt32 attrs =JavaModifiersToMethodAttrs($3,$1);
		attrs = attrs | IL_META_METHODDEF_ABSTRACT;
		$$ = ILNode_MethodDeclaration_create
					(NULL, attrs, $2, $3, 0, $4, 0);
		CloneLine($$, $3);
	}
	| ModifiersOpt error Identifier FormalParameters ';' {
		CCError("invalid return type");
	}
	| ModifiersOpt Type error FormalParameters	{
		CCError("invalid function name");
	}
	| ModifiersOpt Type Identifier FormalParameters error	{
		CCError("missing ';' after function declaration");
	}
	;

MethodBody
	: Block 
	| ';'	{ $$ = NULL; }
	;

Block
	: '{' 			'}' { MakeSimple(Empty); }
	| '{' BlockStatements '}' { 
		$$ = ILNode_NewScope_create($2);
		yysetfilename($$, yygetfilename($2));
		yysetlinenum($$, yygetlinenum($2));
	}
	;

BlockStatements
	: BlockStatement	{ $$ = $1; }
	| BlockStatements BlockStatement	{ 
		$$ = ILNode_Compound_CreateFrom($1, $2); 
	}
	;

BlockStatement
	:	LocalVariableDeclaration
	|	Statement
	|	ClassOrInterfaceDeclaration
	;

LocalVariableDeclaration
	: FINAL LocalVariableType VariableDeclarators ';' { /* TODO */ }
	|     	LocalVariableType VariableDeclarators ';' {
		/* "VariableDeclarators" has split the declaration into
		   a list of variable names, plus a list of assignment
		   statements to set the initial values.  Turn the result
		   into a local variable declaration followed by the
		   assignment statements */
		if($2.init)
		{
			$$ = ILNode_Compound_CreateFrom
					(ILNode_LocalVarDeclaration_create($1, $2.decl),
					 $2.init);
		}
		else
		{
			$$ = ILNode_LocalVarDeclaration_create($1, $2.decl);
		}
	}

/* error recovery */
	| FINAL LocalVariableType VariableDeclarators error	{
		CCError("missing ';' after local variable declaration");
	}
	| 	LocalVariableType VariableDeclarators error	{
	CCError("missing ';' after local variable declaration");
	}
	| FINAL error VariableDeclarators ';' {
		CCError("invalid type in local variable declaration");
	}
	| 	error VariableDeclarators ';'{
		CCError("invalid type in local variable declaration");
	}

	;

Statement
	: LabeledStatement
	| ForStatement
	| IfThenStatement
	| WhileStatement
	| EmptyStatement
	| SwitchStatement
	| DoStatement
	| BreakStatement
	| ContinueStatement
	| ReturnStatement
	| SynchronizedStatement
	| ThrowStatement
	| TryStatement
	| ExpressionStatement
	| Block
	;

LabeledStatement
	: Identifier ':' Statement	{ 
	$$ = ILNode_Compound_CreateFrom(ILNode_GotoLabel_create(ILQualIdentName($1, 0)),$3);
	}
	;

EmptyStatement
	: ';'	{ MakeSimple(Empty); }
	;

BreakStatement
	: BREAK ';'	{ MakeSimple(Break); }
	| BREAK Identifier ';'	{ /* TODO */}
	;

ContinueStatement
	: CONTINUE ';'	{ MakeSimple(Continue); }
	| CONTINUE Identifier ';'	{ /* TODO */}
	;

ReturnStatement
	: RETURN ';'	{ MakeSimple(Return); }
	| RETURN Expression ';'	{ MakeUnary(ReturnExpr, $2); }
	;

ThrowStatement
	: THROW Expression ';'	{ MakeUnary(ThrowExpr, $2); }
	;

IfThenStatement
	: IF '(' Expression ')' Statement	{ 
		MakeTernary(If, ILNode_ToBool_create($3), $5, ILNode_Empty_create()); 
	}
	| IF '(' Expression ')' Statement ELSE Statement	{ 
		MakeTernary(If, ILNode_ToBool_create($3), $5, $7); 
	}
	;

WhileStatement
	: WHILE '(' Expression ')' Statement	{ 
		MakeBinary(While, ILNode_ToBool_create($3), $5); 
	}
	;

ForStatement
	: FOR '(' ForInit  ';' Expression 	';' ForUpdate ')' Statement	{
		MakeQuaternary(For, $3, ILNode_ToBool_create($5), $7, $9);
		$$=ILNode_NewScope_create($$);
	}
	| FOR '(' ForInit  ';' 	 		';' ForUpdate ')' Statement	{
		MakeQuaternary(For, $3, ILNode_ToBool_create(ILNode_True_create()), 
						$6, $8);
		$$=ILNode_NewScope_create($$);
	}
	;

ForInit
	: /* empty */	{ MakeSimple(Empty); }
	| StatementExpressionList	{ $$ = $1; }
	| FINAL Type VariableDeclarators	{ /* TODO */ }
	| LocalVariableType VariableDeclarators	{
		/* "VariableDeclarators" has split the declaration into
		   a list of variable names, plus a list of assignment
		   statements to set the initial values.  Turn the result
		   into a local variable declaration followed by the
		   assignment statements */
		if($2.init)
		{
			$$ = ILNode_Compound_CreateFrom
					(ILNode_LocalVarDeclaration_create($1, $2.decl),
					 $2.init);
		}
		else
		{
			$$ = ILNode_LocalVarDeclaration_create($1, $2.decl);
		}
	}
	;

ForUpdate
	: /* empty */	{ MakeSimple(Empty); }
	| StatementExpressionList	{ $$ = $1; }
	;

DoStatement
	: DO Statement WHILE '(' Expression ')' ';'	{ 
			MakeBinary(Do, $2, ILNode_ToBool_create($5)); 
	}
	;

TryStatement
	: TRY Block CatchClauses	{ MakeTernary(Try, $2, $3, NULL); }
	| TRY Block CatchClauses FinallyClause	{ MakeTernary(Try, $2, $3, $4); }
	| TRY Block FinallyClause	{ MakeTernary(Try, $2, NULL, $3); }
	;

CatchClauses
	: CatchClause	{
		$$ = ILNode_CatchClauses_create();
		ILNode_List_Add($$, $1);
	}
	| CatchClauses CatchClause{
		$$ = $1;
		ILNode_List_Add($$, $2);
	}
	;

CatchClause 
	: CATCH '(' FINAL QualifiedIdentifier Identifier Dims ')' Block	{}
	| CATCH '(' 	QualifiedIdentifier Identifier Dims ')' Block	{}
	| CATCH '(' FINAL QualifiedIdentifier Identifier ')' Block	{}
	| CATCH '(' 	QualifiedIdentifier Identifier ')' Block	{	
		MakeQuaternary(CatchClause ,$3,ILQualIdentName($4,0),$4,$6);
	}
	;

FinallyClause 
	: FINALLY Block	{ MakeUnary(FinallyClause, $2); }
	;

SwitchStatement
	: SWITCH '(' Expression ')' '{' SwitchBlock 	'}' { 
				MakeTernary(Switch, $3, $6, 0);
	}
	;

SwitchBlock
	: /* empty */	{ $$ = NULL; }
	| SwitchBlockStatementGroups	{ $$ = $1; }
	| SwitchBlockStatementGroups SwitchLabels	{ /* fixme */ }
	| 			 	SwitchLabels	{ /* fixme */ }
	;

SwitchBlockStatementGroups
	: SwitchBlockStatementGroup	{
		$$ = ILNode_SwitchSectList_create();	  
		ILNode_List_Add($$, $1);
	}
	| SwitchBlockStatementGroups SwitchBlockStatementGroup	{
		$$ = $1;
		ILNode_List_Add($$, $2);
	}
	;

SwitchBlockStatementGroup
	: SwitchLabels BlockStatements	{ MakeBinary(SwitchSection, $1, $2); }
	;

SwitchLabels
	: CASE ConstantExpression 		':'	{
		$$ = ILNode_CaseList_create();
		ILNode_List_Add($$, ILNode_CaseLabel_create($2));
	}
	| DEFAULT 				':'	{
		$$ = ILNode_CaseList_create();
		ILNode_List_Add($$, ILNode_DefaultLabel_create());
	}
	| SwitchLabels CASE ConstantExpression 	':'	{
		$$ = $1;
		ILNode_List_Add($$, ILNode_CaseLabel_create($3));
	}
	| SwitchLabels DEFAULT 			':'	{
		$$ = $1;
		ILNode_List_Add($$, ILNode_DefaultLabel_create());
	}
	;

SynchronizedStatement
	: SYNCHRONIZED '(' Expression ')' Block	{ /* TODO */ }
	;

ExpressionStatement
	: StatementExpression ';'	{ $$ = $1; }
	| StatementExpression error	{
		CCError("missing ';' after statement");
		yyerrok;
	}
	;

StatementExpression
	: Assignment
	| PreIncrementExpression
	| PreDecrementExpression
	| PostIncrementExpression
	| PostDecrementExpression
	| MethodInvocation
	| ObjectCreationExpression
	;

StatementExpressionList
	: StatementExpression	{ $$ = $1; }
	| StatementExpressionList ',' StatementExpression	{ 
		$$ = ILNode_Compound_CreateFrom($1, $3); }
	;

ConstantExpression
	: Expression
	;

Expression
	: ConditionalExpression
	| Assignment
	;

Assignment
	: Expression '=' ConditionalExpression	{MakeBinary(Assign, $1, $3);}
	| Expression ADD_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignAdd, ILNode_Add_create($1, $3)); 
	}
	| Expression SUB_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignSub, ILNode_Sub_create($1, $3)); 
	}
	| Expression MUL_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignMul, ILNode_Mul_create($1, $3)); 
	}
	| Expression DIV_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignDiv, ILNode_Div_create($1, $3)); 
	}
	| Expression AND_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignAnd, ILNode_And_create($1, $3)); 
	}
	| Expression OR_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignOr,   ILNode_Or_create($1, $3)); 
	}
	| Expression XOR_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignXor, ILNode_Xor_create($1, $3)); 
	}
	| Expression MOD_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignRem, ILNode_Rem_create($1, $3)); 
	}
	| Expression SHR_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignShr, ILNode_Shr_create($1, $3)); 
	}
	| Expression SHL_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignShl, ILNode_Shl_create($1, $3)); 
	}
	| Expression USHR_ASSIGN_OP ConditionalExpression	{ 
		MakeUnary(AssignUShr, ILNode_UShr_create($1,$3));
		
	}
	;

ConditionalExpression
	: ConditionalOrExpression
	| ConditionalOrExpression '?' Expression ':' ConditionalExpression	{ MakeTernary(Conditional, ILNode_ToBool_create($1), $3, $5); }
	;

ConditionalOrExpression
	: ConditionalAndExpression
	| ConditionalOrExpression OR_OP ConditionalAndExpression	{ MakeBinary(LogicalOr, $1, $3); }
	;

ConditionalAndExpression
	: InclusiveOrExpression
	| ConditionalAndExpression AND_OP InclusiveOrExpression	{ 
		MakeBinary(LogicalAnd, $1, $3);
	}
	;

InclusiveOrExpression
	: ExclusiveOrExpression
	| InclusiveOrExpression '|' ExclusiveOrExpression	{ 
		MakeBinary(Or, $1, $3); 
	}
	;

ExclusiveOrExpression
	: AndExpression
	| ExclusiveOrExpression '^' AndExpression	{ MakeBinary(Xor, $1, $3); }
	;

AndExpression
	: EqualityExpression
	| AndExpression '&' EqualityExpression	{ MakeBinary(And, $1, $3); }
	;

EqualityExpression
	: RelationalExpression
	| EqualityExpression EQ_OP RelationalExpression	{ MakeBinary(Eq, $1, $3); }
	| EqualityExpression NEQ_OP RelationalExpression	{ MakeBinary(Ne, $1, $3); }
	;

RelationalExpression
	: ShiftExpression
	| RelationalExpression L_OP ShiftExpression	{ MakeBinary(Lt, $1, $3); }
	| RelationalExpression G_OP ShiftExpression	{ MakeBinary(Gt, $1, $3); }
	| RelationalExpression LE_OP ShiftExpression	{ MakeBinary(Le, $1, $3); }
	| RelationalExpression GE_OP ShiftExpression	{ MakeBinary(Ge, $1, $3); }
	| RelationalExpression INSTANCEOF Type	{ MakeBinary(InstanceOf,$1,$3); }
	;

ShiftExpression
	: AdditiveExpression
	| ShiftExpression SHL_OP AdditiveExpression	{ MakeBinary(Shl, $1, $3); }
	| ShiftExpression SHR_OP AdditiveExpression	{ MakeBinary(Shr, $1, $3); }
	| ShiftExpression USHR_OP AdditiveExpression	{ MakeBinary(UShr,$1,$3); }
	;

AdditiveExpression
	: MultiplicativeExpression
	| AdditiveExpression '+' MultiplicativeExpression	{ 
		MakeBinary(Add, $1, $3); 
	}
	| AdditiveExpression '-' MultiplicativeExpression	{ 
		MakeBinary(Sub, $1, $3); 
	}
	;

MultiplicativeExpression
	: UnaryExpression
	| MultiplicativeExpression '*' UnaryExpression	{ 
		MakeBinary(Mul, $1, $3); 
	}
	| MultiplicativeExpression '/' UnaryExpression	{ 
		MakeBinary(Div, $1, $3); 
	}
	| MultiplicativeExpression '%' UnaryExpression	{ 
		MakeBinary(Rem, $1, $3); 
	}
	;

UnaryExpression
	: PreIncrementExpression
	| PreDecrementExpression
	| '+' UnaryExpression	{ 
		MakeUnary(UnaryPlus, $2); 
	}
	| '-' UnaryExpression	{ 
		if(yyisa($2, ILNode_Integer))
		{
			$$ = NegateInteger((ILNode_Integer *)$2);
		} 
		else if(yyisa($2, ILNode_Real)) 
		{
			((ILNode_Real *)($2))->value = -(((ILNode_Real *)($2))->value);
			$$ = $2;
		}
		else if(yyisa($2, ILNode_Decimal)) 
		{
			ILDecimalNeg(&(((ILNode_Decimal *)($2))->value),
					 &(((ILNode_Decimal *)($2))->value));
			$$ = $2;
		}
		else
		{ 
			MakeUnary(Neg, $2);
		}
	}
	| CastExpression
	;

PreIncrementExpression
	: INC_OP UnaryExpression	{ 
		MakeUnary(PreInc, $2); 
	}
	;

PreDecrementExpression
	: DEC_OP UnaryExpression	{ 
		MakeUnary(PreDec, $2); 
	}
	;

CastExpression
	: UnaryExpressionNotPlusMinus
	| '(' PrimitiveType 		')' UnaryExpression	{ 
				MakeBinary(UserCast, $2, $4);
	}
	| '(' PrimitiveType Dims	')' UnaryExpression	{ 
		ILNode *type=$2;
		int i;
		
		for(i=0;i<$3;i++)
		{
			type = ILNode_JArrayType_create(type,1);
		}
		
		MakeBinary(UserCast,type, $5);
	}
	| '(' Expression		')' UnaryExpressionNotPlusMinus	{ 
				MakeBinary(UserCast, $2, $4);
	}
	;

UnaryExpressionNotPlusMinus
	: PostfixExpression
	| '~' UnaryExpression	{ MakeUnary(Not, $2); }
	| '!' UnaryExpression	{ MakeUnary(LogicalNot, $2); }
	;

PostfixExpression
	: Primary
	| PostIncrementExpression
	| PostDecrementExpression
	;


PostIncrementExpression
	: PostfixExpression INC_OP	{ MakeUnary(PostInc, $1); }
	;

PostDecrementExpression
	: PostfixExpression DEC_OP	{ MakeUnary(PostDec, $1); }
	;

Primary
	: RealPrimary
	| ArrayCreationExpression
	| ObjectCreationExpression
	;

RealPrimary
	: Literal
	| PrimitiveType 		'.' CLASS	{ 
		/* TODO */
	}
	| PrimitiveType Dims	'.' CLASS	{ 
		/* TODO */
	}
	| FieldAccess
	| FieldAccess		'.' CLASS	{  
		/* TODO */
	}
	| FieldAccess Dims 	'.' CLASS	{ 
		/* TODO */
	}
	| FieldAccess		'.' THIS	{ 
		/* TODO */
	}
	| THIS	{ MakeSimple(This); }
	| MethodInvocation
	| ArrayAccess
	|'(' Expression ')'	{ $$ = $2; }
	;

FieldAccess
	: RealPrimary	 		  '.' Identifier	{
		MakeBinary(MemberAccess, $1, $3);
	}
	| ObjectCreationExpression '.' Identifier	{
		MakeBinary(MemberAccess, $1, $3);
	}
	| FieldAccess		  '.' SUPER '.' Identifier	{
	}
	|FieldAccess    '.' Identifier	{
		MakeBinary(MemberAccess, $1, $3);
	}
	| SUPER 			  '.' Identifier	{
		MakeUnary(BaseAccess,$3);
	}
	| Identifier
	;

MethodInvocation
	: FieldAccess Arguments	{ 
		MakeBinary(InvocationExpression, $1, $2); 
	}
	| RealPrimary		'.' SUPER Arguments	{ 
		/* TODO */
	}
	| SUPER Arguments	{ 
		$$ = ILNode_Compound_CreateFrom
				(ILNode_NonStaticInit_create(),
				 ILNode_InvocationExpression_create
					(ILNode_BaseInit_create(), $2));
	}
	| THIS Arguments	{ 
		/* TODO */
	}
	;

ArrayAccess
	: FieldAccess 		'[' Expression ']'	{
		MakeBinary(ArrayAccess, $1, $3);
	}
	| MethodInvocation 	'[' Expression ']'	{
	}
	| ArrayAccess	 	'[' Expression ']'	{
		MakeBinary(ArrayAccess,$1,$3);
	}
	;

Arguments
	: '(' ')'	{ $$ = NULL; }
	| '(' ExpressionList ')'	{ $$ = $2; }
	;

ExpressionList
	: Expression	{ $$ = $1; }
	| ExpressionList ',' Expression	{ MakeBinary(ArgList, $1, $3); }
	;

ObjectCreationExpression
	: NEW QualifiedIdentifier Arguments ClassBodyOpt	{
				MakeBinary(ObjectCreationExpression, $2, $3); 
	}
	| RealPrimary 		'.' 	NEW Identifier Arguments ClassBodyOpt	{
		/* TODO */
	}
	| FieldAccess 	'.' 	NEW Identifier Arguments ClassBodyOpt	{
		/* TODO */
	}
	;

/*
 * Note: We have to provide DimExprs and the rank , because Java has
 * arrays of arrays like int [][]x=new int[10][10];
 */
ArrayCreationExpression
	: NEW PrimitiveType DimExprs Dims	{
		ILNode *list=ILNode_List_create();
		int i;
		for(i=0;i<$4;i++)
		{
			ILNode_List_Add(list,ILNode_JTypeSuffix_create(1));
		}
		$$ = ILNode_JNewExpression_create($2,$3,list,NULL);	
	}
	| NEW PrimitiveType DimExprs	{
		$$ = ILNode_JNewExpression_create($2,$3,NULL,NULL);	
	}
	| NEW PrimitiveType Dims ArrayInitializer	{
		ILNode *type=$2;
		int i;
		for(i=0;i<$3;i++)
		{
			type=ILNode_JArrayType_create(type,1);
		}
		$$ = ILNode_JNewExpression_create(type,NULL,NULL,$4);	
	}
	| NEW QualifiedIdentifier DimExprs Dims	{
		ILNode *list=ILNode_List_create();
		int i;
		for(i=0;i<$4;i++)
		{
			ILNode_List_Add(list,ILNode_JTypeSuffix_create(1));
		}
		$$ = ILNode_JNewExpression_create($2,$3,list,NULL);	
	}
	| NEW QualifiedIdentifier DimExprs	{
		$$ = ILNode_JNewExpression_create($2,$3,NULL,NULL);	
	}
	| NEW QualifiedIdentifier Dims ArrayInitializer	{
		ILNode *list=ILNode_List_create();
		int i;
		for(i=0;i<$3;i++)
		{
			ILNode_List_Add(list,ILNode_JTypeSuffix_create(1));
		}
		$$ = ILNode_JNewExpression_create($2,NULL,list,$4);	
	}
	;
%%
