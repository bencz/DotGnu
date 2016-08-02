/*
 * gen_python.c - Generate Python source code from "treecc" input files.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

#include "system.h"
#include "input.h"
#include "info.h"
#include "gen.h"
#include "errors.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Declare the type definitions for a node type.
 */
static void DeclareTypeDefs(TreeCCContext *context,
						    TreeCCNode *node)
{
	if((node->flags & TREECC_NODE_ENUM) != 0)
	{
		/* Define an enumerated type */
		TreeCCStream *stream = node->source;
		TreeCCNode *child;
		int value = 0;
		int neednl = 0;
		TreeCCStreamPrint(stream, "class %s:\n", node->name);
		child = node->firstChild;
		while(child != 0)
		{
			if((child->flags & TREECC_NODE_ENUM_VALUE) != 0)
			{
				TreeCCStreamPrint(stream, "    %s = %d\n", child->name, value++);
				neednl = 1;
			}
			child = child->nextSibling;
		}
		if(neednl)
		{
			TreeCCStreamPrint(stream, "\n");
		}
	}
}

/*
 * Output the parameters for a node creation function.
 */
static int CreateParams(TreeCCContext *context, TreeCCStream *stream,
						TreeCCNode *node, int needComma)
{
	TreeCCField *field;
	if(node->parent)
	{
		needComma = CreateParams(context, stream, node->parent, needComma);
	}
	field = node->fields;
	while(field != 0)
	{
		if((field->flags & TREECC_FIELD_NOCREATE) == 0)
		{
			if(needComma)
			{
				TreeCCStreamPrint(stream, ", ");
			}
			TreeCCStreamPrint(stream, "%s", field->name);
			needComma = 1;
		}
		field = field->next;
	}
	return needComma;
}

/*
 * Output the parameters to call an inherited constructor.
 */
static int InheritParamsSource(TreeCCContext *context, TreeCCStream *stream,
							   TreeCCNode *node, int needComma)
{
	TreeCCField *field;
	if(node->parent)
	{
		needComma = InheritParamsSource(context, stream,
									    node->parent, needComma);
	}
	field = node->fields;
	while(field != 0)
	{
		if((field->flags & TREECC_FIELD_NOCREATE) == 0)
		{
			if(needComma)
			{
				TreeCCStreamPrint(stream, ", ");
			}
			TreeCCStreamPrint(stream, "%s", field->name);
			needComma = 1;
		}
		field = field->next;
	}
	return needComma;
}

/*
 * Implement the virtual methods that have implementations in a node type.
 */
static void ImplementVirtuals(TreeCCContext *context, TreeCCStream *stream,
							  TreeCCNode *node, TreeCCNode *actualNode)
{
	TreeCCVirtual *virt;
	TreeCCParam *param;
	TreeCCOperationCase *operCase;
	int declareCase, abstractCase;
	TreeCCNode *tempNode;
	int num, first;
	int needComma;
	if(node->parent)
	{
		ImplementVirtuals(context, stream, node->parent, actualNode);
	}
	virt = node->virtuals;
	while(virt != 0)
	{
		/* Determine if we need a definition for this virtual,
		   and whether the definition is real or abstract */
		operCase = TreeCCOperationFindCase(context, actualNode, virt->name);
		if(!operCase)
		{
			tempNode = actualNode->parent;
			abstractCase = 1;
			while(tempNode != 0)
			{
				operCase = TreeCCOperationFindCase
								(context, tempNode, virt->name);
				if(operCase != 0)
				{
					abstractCase = 0;
					break;
				}
				tempNode = tempNode->parent;
			}
			declareCase = abstractCase;
		}
		else
		{
			declareCase = 1;
			abstractCase = 0;
		}
		if(declareCase)
		{
			if(abstractCase)
			{
				if(node == actualNode)
				{
					TreeCCStreamPrint(stream, "    def %s(", virt->name);
				}
				else
				{
					/* Inherit the "abstract" definition from the parent */
					virt = virt->next;
					continue;
				}
			}
			else
			{
				TreeCCStreamPrint(stream, "    def %s(", virt->name);
			}
			param = virt->oper->params;
			needComma = 0;
			num = 1;
			first = 1;
			while(param != 0)
			{
				if(needComma)
				{
					TreeCCStreamPrint(stream, ", ");
				}
				if(param->name)
				{
					TreeCCStreamPrint(stream, "%s", param->name);
				}
				else
				{
					TreeCCStreamPrint(stream, "P%d__", num);
					++num;
				}
				needComma = 1;
				param = param->next;
			}
			if(!abstractCase)
			{
				TreeCCStreamPrint(stream, "):\n");
				TreeCCStreamCodeIndentPython(stream, operCase->code, 1);
				TreeCCStreamPrint(stream, "\n");
			}
			else
			{
				/* This is an abstract method definition.  Raise an error */
				TreeCCStreamPrint(stream, "):\n");
				TreeCCStreamPrint(stream, "        raise NotImplementedError\n\n");
			}
		}
		virt = virt->next;
	}
}

/*
 * Build the type declarations for a node type.
 */
static void BuildTypeDecls(TreeCCContext *context,
						   TreeCCNode *node)
{
	TreeCCStream *stream;
	int needComma;
	TreeCCField *field;

	/* Ignore if this is an enumerated type node */
	if((node->flags & (TREECC_NODE_ENUM | TREECC_NODE_ENUM_VALUE)) != 0)
	{
		return;
	}

	/* Output the class header */
	stream = node->source;
	if(node->parent)
	{
		/* Inherit from a specified parent type */
		TreeCCStreamPrint(stream, "class %s (%s):\n",
						  node->name, node->parent->name);
	}
	else
	{
		/* This type is the base of a class hierarchy */
		if(context->baseType)
		{
			TreeCCStreamPrint(stream, "class %s (%s):\n",
							  node->name, context->baseType);
		}
		else
		{
			TreeCCStreamPrint(stream, "class %s:\n", node->name);
		}
	}

	/* Declare the kind value */
	TreeCCStreamPrint(stream, "    KIND = %d\n", node->number);

	/* Declare the constructor for the node type */
	TreeCCStreamPrint(stream, "    def __init__(self");
	needComma = 1;
	CreateParams(context, stream, node, needComma);
	TreeCCStreamPrint(stream, "):\n");

	/* Call the parent class constructor */
	if(node->parent)
	{
		TreeCCStreamPrint(stream, "        %s.__init__(self", node->parent->name);
		needComma = 1;
		InheritParamsSource(context, stream, node->parent, needComma);
		TreeCCStreamPrint(stream, ")\n");
	}

	/* Set the node kind */
	TreeCCStreamPrint(stream, "        self.kind = %d\n", node->number);

	/* Track the filename and line number if necessary */
	if(context->track_lines && !(node->parent))
	{
		TreeCCStreamPrint(stream, "        self.filename = %scurrfilename()\n",
						  context->yy_replacement);
		TreeCCStreamPrint(stream, "        self.linenum = %scurrlinenum()\n",
						  context->yy_replacement);
	}

	/* Initialize the fields that are specific to this node type */
	field = node->fields;
	while(field != 0)
	{
		if((field->flags & TREECC_FIELD_NOCREATE) == 0)
		{
			TreeCCStreamPrint(stream, "        self.%s = %s\n",
							  field->name, field->name);
		}
		else if(field->value)
		{
			TreeCCStreamPrint(stream, "        self.%s = %s\n",
							  field->name, field->value);
		}
		field = field->next;
	}
	TreeCCStreamPrint(stream, "\n");

	/* Implement the virtual functions */
	ImplementVirtuals(context, stream, node, node);

	/* If this is a base class, then define the "getKindName" method,
	   which returns the name of the node type */
	if(!node->parent)
	{
		TreeCCStreamPrint(stream, "    def getKindName(self):\n");
		TreeCCStreamPrint(stream, "        return self.__class__.__name__\n");
		TreeCCStreamPrint(stream, "\n");
	}
}

/*
 * Output spaces for a specific level of indenting.  We use four
 * spaces for each level of indenting.
 */
static void Indent(TreeCCStream *stream, int indent)
{
	while(indent > 0)
	{
		TreeCCStreamPrint(stream, "    ");
		--indent;
	}
}

/*
 * Generate the start declarations for a non-virtual operation.
 */
static void PythonGenStart(TreeCCContext *context, TreeCCStream *stream,
					 	   TreeCCOperation *oper)
{
	/* Nothing to do here for Python */
}

/*
 * Generate the entry point for a non-virtual operation.
 */
static void GenEntry(TreeCCContext *context, TreeCCStream *stream,
					 TreeCCOperation *oper, int number)
{
	TreeCCParam *param;
	int num;
	int needComma;
	if(number != -1)
	{
		TreeCCStreamPrint(stream, "def %s_split_%d__(", oper->name, number);
	}
	else
	{
		TreeCCStreamPrint(stream, "def %s(", oper->name);
	}
	param = oper->params;
	num = 1;
	needComma = 0;
	while(param != 0)
	{
		if(needComma)
		{
			TreeCCStreamPrint(stream, ", ");
		}
		if(param->name)
		{
			TreeCCStreamPrint(stream, "%s", param->name);
		}
		else
		{
			TreeCCStreamPrint(stream, "P%d__", num);
			++num;
		}
		needComma = 1;
		param = param->next;
	}
	TreeCCStreamPrint(stream, "):\n");
}

/*
 * Generate the entry point for a non-virtual operation.
 */
static void PythonGenEntry(TreeCCContext *context, TreeCCStream *stream,
					 	   TreeCCOperation *oper)
{
	GenEntry(context, stream, oper, -1);
}

/*
 * Generate the entry point for a split-out function.
 */
static void PythonGenSplitEntry(TreeCCContext *context, TreeCCStream *stream,
					      	    TreeCCOperation *oper, int number)
{
	GenEntry(context, stream, oper, number);
}

/*
 * Stack of switch contexts.  Python doesn't have a "swtich" statement,
 * so we need to emulate it using nested "if" statements.  The following
 * stack allows us to keep track of whether we need an "if" or an "elif",
 * as well as the name of the parameter to match against.
 */
typedef struct
{
	char   *paramName;
	int		nextIsIf;
	int		needsOr;

} SwitchInfo;
#define	MaxSwitchDepth		100
static SwitchInfo switchStack[MaxSwitchDepth];
static int switchStackSize = 0;

/*
 * Generate the head of a "switch" statement.
 */
static void PythonGenSwitchHead(TreeCCContext *context, TreeCCStream *stream,
						  		char *paramName, int level, int isEnum)
{
	if(switchStackSize >= MaxSwitchDepth)
	{
		TreeCCAbort(0, "maximum python switch depth reached");
		return;
	}
	switchStack[switchStackSize].paramName = paramName;
	switchStack[switchStackSize].nextIsIf = 1;
	switchStack[switchStackSize].needsOr = 0;
	++switchStackSize;
}

/*
 * Generate a selector for a "switch" case.
 */
static void PythonGenSelector(TreeCCContext *context, TreeCCStream *stream,
					    	  TreeCCNode *node, int level)
{
	SwitchInfo *info = &(switchStack[switchStackSize - 1]);
	if(info->needsOr)
	{
		/* Generating multiple selectors for the same case */
		TreeCCStreamPrint(stream, " or ");
	}
	else if(info->nextIsIf)
	{
		Indent(stream, level + 1);
		TreeCCStreamPrint(stream, "if ");
		info->nextIsIf = 0;
	}
	else
	{
		Indent(stream, level + 1);
		TreeCCStreamPrint(stream, "elif ");
	}
	if((node->flags & TREECC_NODE_ENUM_VALUE) != 0)
	{
		TreeCCStreamPrint(stream, "%s == %s.%s",
						  info->paramName, node->parent->name, node->name);
	}
	else if((node->flags & TREECC_NODE_ENUM) == 0)
	{
		/* Use the actual node number to prevent unnecessary symbol
		   lookups at runtime when matching node kinds */
		TreeCCStreamPrint(stream, "%s.kind == %d",
						  info->paramName, node->number);
	}
	info->needsOr = 1;
}

/*
 * Terminate the selectors and begin the body of a "switch" case.
 */
static void PythonGenEndSelectors(TreeCCContext *context, TreeCCStream *stream,
						    	  int level)
{
	SwitchInfo *info = &(switchStack[switchStackSize - 1]);
	TreeCCStreamPrint(stream, ":\n");
	info->needsOr = 0;
}

/*
 * Generate the code for a case within a function.
 */
static void PythonGenCaseFunc(TreeCCContext *context, TreeCCStream *stream,
							  TreeCCOperationCase *operCase, int number)
{
	TreeCCParam *param;
	TreeCCTrigger *trigger;
	int num;
	int needComma;

	/* Output the header for the function */
	TreeCCStreamPrint(stream, "def %s_%d__(",
					  operCase->oper->name, number);
	param = operCase->oper->params;
	trigger = operCase->triggers;
	num = 1;
	needComma = 0;
	while(param != 0)
	{
		if(needComma)
		{
			TreeCCStreamPrint(stream, ", ");
		}
		if(param->name)
		{
			TreeCCStreamPrint(stream, "%s", param->name);
		}
		else
		{
			TreeCCStreamPrint(stream, "P%d__", num);
			++num;
		}
		needComma = 1;
		param = param->next;
	}
	if(!needComma)
	{
		TreeCCStreamPrint(stream, "void");
	}
	TreeCCStreamPrint(stream, "):\n");

	/* Output the code for the operation case */
	if(operCase->code)
	{
		TreeCCStreamCodeIndentPython(stream, operCase->code, 0);
	}
	TreeCCStreamPrint(stream, "\n");
}

/*
 * Generate a call to a case function from within the "switch".
 */
static void PythonGenCaseCall(TreeCCContext *context, TreeCCStream *stream,
							  TreeCCOperationCase *operCase, int number,
							  int level)
{
	TreeCCParam *param;
	int num;
	int needComma;

	/* Indent to the correct level */
	Indent(stream, level + 2);

	/* Add "return" to the front if the operation is non-void */
	if(strcmp(operCase->oper->returnType, "void") != 0)
	{
		TreeCCStreamPrint(stream, "return ");
	}

	/* Print out the call */
	TreeCCStreamPrint(stream, "%s_%d__(", operCase->oper->name, number);
	param = operCase->oper->params;
	num = 1;
	needComma = 0;
	while(param != 0)
	{
		if(needComma)
		{
			TreeCCStreamPrint(stream, ", ");
		}
		if(param->name)
		{
			TreeCCStreamPrint(stream, "%s", param->name);
		}
		else
		{
			TreeCCStreamPrint(stream, "P%d__", num);
			++num;
		}
		needComma = 1;
		param = param->next;
	}
	TreeCCStreamPrint(stream, ")\n");
}

/*
 * Generate the code for a case inline within the "switch".
 */
static void PythonGenCaseInline(TreeCCContext *context, TreeCCStream *stream,
						  		TreeCCOperationCase *operCase, int level)
{
	if(operCase->code)
	{
		TreeCCStreamCodeIndentPython(stream, operCase->code, level + 1);
	}
	else
	{
		/* No code for this case, so just output a "pass" instruction */
		Indent(stream, level + 2);
		TreeCCStreamPrint(stream, "pass\n");
	}
}

/*
 * Generate the code for a call to a split function within the "switch".
 */
static void PythonGenCaseSplit(TreeCCContext *context, TreeCCStream *stream,
						 	   TreeCCOperationCase *operCase, int number,
							   int level)
{
	TreeCCParam *param;
	int num;
	int needComma;

	/* Indent to the correct level */
	Indent(stream, level + 2);

	/* Add "return" to the front if the operation is non-void */
	if(strcmp(operCase->oper->returnType, "void") != 0)
	{
		TreeCCStreamPrint(stream, "return ");
	}

	/* Print out the call */
	TreeCCStreamPrint(stream, "%s_split_%d__(", operCase->oper->name, number);
	param = operCase->oper->params;
	num = 1;
	needComma = 0;
	while(param != 0)
	{
		if(needComma)
		{
			TreeCCStreamPrint(stream, ", ");
		}
		if(param->name)
		{
			TreeCCStreamPrint(stream, "%s", param->name);
		}
		else
		{
			TreeCCStreamPrint(stream, "P%d__", num);
			++num;
		}
		needComma = 1;
		param = param->next;
	}
	TreeCCStreamPrint(stream, ")\n");
}

/*
 * Terminate a "switch" case.
 */
static void PythonGenEndCase(TreeCCContext *context, TreeCCStream *stream,
					   		 int level)
{
	/* Nohing to do here for Python */
}

/*
 * Terminate the "switch" statement.
 */
static void PythonGenEndSwitch(TreeCCContext *context, TreeCCStream *stream,
						 	   int level)
{
	--switchStackSize;
}

/*
 * Generate the exit point for a non-virtual operation.
 */
static void PythonGenExit(TreeCCContext *context, TreeCCStream *stream,
						  TreeCCOperation *oper)
{
	if(strcmp(oper->returnType, "void") != 0)
	{
		/* Generate a default return value for the function */
		if(oper->defValue)
		{
			TreeCCStreamPrint(stream, "    return %s\n", oper->defValue);
		}
		else
		{
			TreeCCStreamPrint(stream, "    return None\n");
		}
	}
	TreeCCStreamPrint(stream, "\n");
}

/*
 * Generate the end declarations for a non-virtual operation.
 */
static void PythonGenEnd(TreeCCContext *context, TreeCCStream *stream,
				   		  TreeCCOperation *oper)
{
	/* Nothing to do here for Python */
}

/*
 * Table of non-virtual code generation functions.
 */
static TreeCCNonVirtual const TreeCCNonVirtualFuncsPython = {
	PythonGenStart,
	PythonGenEntry,
	PythonGenSplitEntry,
	PythonGenSwitchHead,
	PythonGenSelector,
	PythonGenEndSelectors,
	PythonGenCaseFunc,
	PythonGenCaseCall,
	PythonGenCaseInline,
	PythonGenCaseSplit,
	PythonGenEndCase,
	PythonGenEndSwitch,
	PythonGenExit,
	PythonGenEnd
};

/*
 * Write out header information for all streams.
 */
static void WritePythonHeaders(TreeCCContext *context)
{
	TreeCCStream *stream = context->streamList;
	while(stream != 0)
	{
		if(!(stream->isHeader))
		{
			TreeCCStreamSourceTopSpecial(stream, '#');
		}
		if(stream->defaultFile)
		{
			/* Reset the dirty flag if this is a default stream,
			   because we don't want to write out the final file
			   if it isn't actually written to in practice */
			stream->dirty = 0;
		}
		stream = stream->nextStream;
	}
}

/*
 * Write out footer information for all streams.
 */
static void WritePythonFooters(TreeCCContext *context)
{
	TreeCCStream *stream = context->streamList;
	while(stream != 0)
	{
		if(stream->defaultFile && !(stream->dirty))
		{
			/* Clear the default file's contents, which we don't need */
			TreeCCStreamClear(stream);
		}
		else if(!(stream->isHeader))
		{
			TreeCCStreamSourceBottom(stream);
		}
		stream = stream->nextStream;
	}
}

void TreeCCGeneratePython(TreeCCContext *context)
{
	/* Don't ever print line numbers for Python */
	context->print_lines = 0;

	/* Write all stream headers */
	WritePythonHeaders(context);

	/* Generate the contents of the source stream */
	TreeCCNodeVisitAll(context, DeclareTypeDefs);
	TreeCCNodeVisitAll(context, BuildTypeDecls);
	TreeCCGenerateNonVirtuals(context, &TreeCCNonVirtualFuncsPython);

	/* Write all stream footers */
	WritePythonFooters(context);
}

#ifdef	__cplusplus
};
#endif
