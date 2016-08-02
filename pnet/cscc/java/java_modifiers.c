/*
 * java_modifiers.c - Various access modifiers and other flags
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Gopal.V
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

#include "java_internal.h"

/*
 * Report errors for each modifier in a mask.
 */
static void ModifierError(const char *filename, long linenum,
						  ILUInt32 modifiers, const char *msg)
{
	if((modifiers & JAVA_MODIFIER_PUBLIC) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "public");
	}
	if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "private");
	}
	if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "protected");
	}
	if((modifiers & JAVA_MODIFIER_ABSTRACT) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "abstract");
	}
	if((modifiers & JAVA_MODIFIER_STATIC) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "static");
	}
	if((modifiers & JAVA_MODIFIER_VOLATILE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "volatile");
	}
}

/*
 * Report errors for modifiers that cannot be used in a particular context.
 */
static void BadModifiers(ILNode *node, ILUInt32 modifiers)
{
	ModifierError(yygetfilename(node), yygetlinenum(node),
				  modifiers, "`%s' cannot be used in this context");
}

/*
 * Validate access modifiers and return the access level.
 */
static ILUInt32 ValidateAccess(ILNode *node, ILUInt32 modifiers)
{
	if((modifiers & JAVA_MODIFIER_PUBLIC) != 0)
	{
		if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `public' and `private'");
		}
		if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `public' and `protected'");
		}
		return IL_META_FIELDDEF_PUBLIC;
	}
	else if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
	{
		if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `private' and `protected'");
		}
		return IL_META_FIELDDEF_PRIVATE;
	}
	else if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
	{
		return IL_META_FIELDDEF_FAMILY;
	}
	else
	{
		return IL_META_FIELDDEF_PRIVATE;
	}
}

static void JavaModifiersUsedTwice(const char *filename, long linenum,
								   ILUInt32 modifiers)
{
	ModifierError(filename, linenum,
				  modifiers, "`%s' specified multiple times");
}

/*
 * Validate calling conventions for a method-like construct.
 */
static ILUInt32 ValidateCalling(ILNode *node, ILUInt32 modifiers,
								ILUInt32 access)
{
	ILUInt32 attrs = 0;

	/* Process the calling convention modifiers */
	if((modifiers & JAVA_MODIFIER_STATIC) != 0)
	{
		attrs |= IL_META_METHODDEF_STATIC;
		if((modifiers & JAVA_MODIFIER_ABSTRACT) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `abstract'");
		}
		if((modifiers & JAVA_MODIFIER_FINAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `sealed'");
		}
	}
	else if((modifiers & JAVA_MODIFIER_ABSTRACT) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_ABSTRACT;
		if((modifiers & JAVA_MODIFIER_FINAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `abstract' and `final'");
		}
	}
	else if((modifiers & IL_META_METHODDEF_FINAL)==0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL;
	}

	/* Virtual methods cannot be private */
	if((modifiers & (JAVA_MODIFIER_ABSTRACT)) != 0)
	{
		if(access == IL_META_FIELDDEF_PRIVATE)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  "cannot declare abstract methods to be private");
		}
	}

	/* Methods always need "hide by sig" */
	attrs |= IL_META_METHODDEF_HIDE_BY_SIG;

	/* Done */
	return attrs;
}

ILUInt32 JavaModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, 
									int isNested)
{
	ILUInt32 attrs;

	if(isNested)
	{
		modifiers = modifiers & (~JAVA_MODIFIER_STATIC);
	}

	/* Determine the access level of the class */
	if(!isNested)
	{
		/* Only "public" and "internal" can be used at the outermost scope */
		if((modifiers & JAVA_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_PUBLIC;
			if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
		}
		else
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`private' modifier is not permitted "
						"on non-nested types");
			}
			if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`protected' modifier is not permitted "
							  "on non-nested types");
			}
		}
	}
	else
	{
		/* Nested classes have a greater range of accessibilities */
		if((modifiers & JAVA_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PUBLIC;
			if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
		}
		else if((modifiers & JAVA_MODIFIER_PRIVATE) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
			if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `private' and `protected'");
			}
		}
		else if((modifiers & JAVA_MODIFIER_PROTECTED) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_FAMILY;
		}
		else
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
		}

	}

	if((modifiers & JAVA_MODIFIER_ABSTRACT) != 0)
	{
		attrs |= IL_META_TYPEDEF_ABSTRACT;
	}

	/* Report errors for any remaining modifiers */
	BadModifiers(node,
				 modifiers & (JAVA_MODIFIER_STATIC | 
						 	  JAVA_MODIFIER_VOLATILE));

	/* We have the attributes we wanted now */
	return attrs;
}

ILUInt32 JavaModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Different flags are used for instance and static constructors */
	if((modifiers & JAVA_MODIFIER_STATIC) == 0)
	{
		attrs = ValidateAccess(node, modifiers);
		BadModifiers(node,
					 modifiers & ~(JAVA_MODIFIER_PUBLIC |
								   JAVA_MODIFIER_PRIVATE |
								   JAVA_MODIFIER_PROTECTED));
	}
	else
	{
		BadModifiers(node,
					 modifiers & ~(JAVA_MODIFIER_STATIC));
		attrs = IL_META_METHODDEF_PRIVATE | IL_META_METHODDEF_STATIC;
	}

	/* Add the "hidebysig" and "*specialname" attributes always */
	attrs |= IL_META_METHODDEF_HIDE_BY_SIG |
			 IL_META_METHODDEF_SPECIAL_NAME |
			 IL_META_METHODDEF_RT_SPECIAL_NAME;

	/* Done */
	return attrs;
}

ILUInt32 JavaModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the calling convention attributes */
	attrs |= ValidateCalling(node, modifiers, attrs);
	if((attrs & IL_META_METHODDEF_STATIC) == 0)
	{
		attrs|=IL_META_METHODDEF_VIRTUAL;
	}

	/* Report errors for the remaining modifiers */
	BadModifiers(node, modifiers & JAVA_MODIFIER_VOLATILE);

	/* Done */
	return attrs;
}


ILUInt32 JavaModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the "static" modifier */
	if((modifiers & JAVA_MODIFIER_STATIC) != 0)
	{
		attrs |= IL_META_FIELDDEF_STATIC;
	}

	/* Report errors for the remaining modifiers */
	BadModifiers(node,
				 modifiers & (JAVA_MODIFIER_ABSTRACT | JAVA_MODIFIER_FINAL 
						  | JAVA_MODIFIER_SYNCHRONIZED));

	/* Done */
	return attrs;
}
