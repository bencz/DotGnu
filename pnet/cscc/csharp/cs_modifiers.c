/*
 * cs_modifiers.c - Verify modifier combinations.
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

#include "cs_internal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int CSNoHideBySig = 0;

/*
 * Define invalid modifiers
 */
#if IL_VERSION_MAJOR > 1
#define INVALID_CLASS_MODIFIERS \
	(CS_MODIFIER_READONLY | CS_MODIFIER_VIRTUAL | CS_MODIFIER_OVERRIDE | \
	CS_MODIFIER_EXTERN | CS_MODIFIER_VOLATILE)
#else /* IL_VERSION_MAJOR == 1 */
#define INVALID_CLASS_MODIFIERS \
	(CS_MODIFIER_STATIC | CS_MODIFIER_READONLY | CS_MODIFIER_VIRTUAL | \
	CS_MODIFIER_OVERRIDE | CS_MODIFIER_EXTERN | CS_MODIFIER_VOLATILE)
#endif /* IL_VERSION_MAJOR == 1 */

/*
 * Report errors for each modifier in a mask.
 */
static void ModifierError(char *filename, long linenum,
						  ILUInt32 modifiers, const char *msg)
{
	if((modifiers & CS_MODIFIER_PUBLIC) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "public");
	}
	if((modifiers & CS_MODIFIER_PRIVATE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "private");
	}
	if((modifiers & CS_MODIFIER_PROTECTED) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "protected");
	}
	if((modifiers & CS_MODIFIER_INTERNAL) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "internal");
	}
	if((modifiers & CS_MODIFIER_NEW) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "new");
	}
	if((modifiers & CS_MODIFIER_ABSTRACT) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "abstract");
	}
	if((modifiers & CS_MODIFIER_SEALED) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "sealed");
	}
	if((modifiers & CS_MODIFIER_STATIC) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "static");
	}
	if((modifiers & CS_MODIFIER_READONLY) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "readonly");
	}
	if((modifiers & CS_MODIFIER_VIRTUAL) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "virtual");
	}
	if((modifiers & CS_MODIFIER_OVERRIDE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "override");
	}
	if((modifiers & CS_MODIFIER_EXTERN) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "extern");
	}
	if((modifiers & CS_MODIFIER_UNSAFE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "unsafe");
	}
	if((modifiers & CS_MODIFIER_VOLATILE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "volatile");
	}
	if((modifiers & CS_MODIFIER_PARTIAL) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "partial");
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

void CSModifiersUsedTwice(char *filename, long linenum, ILUInt32 modifiers)
{
	ModifierError(filename, linenum,
				  modifiers, "`%s' specified multiple times");
}

ILUInt32 CSModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, int isNested)
{
	ILUInt32 attrs;

	/* Determine the access level of the class */
	if(!isNested)
	{
		/* Only "public" and "internal" can be used at the outermost scope */
		if((modifiers & CS_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `internal'");
			}
		}
		else if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `internal' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `internal' and `protected'");
			}
		}
		else
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`private' modifier is not permitted "
						"on non-nested types");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`protected' modifier is not permitted "
							  "on non-nested types");
			}
		}

		/* The "new" modifier is not allowed on top-level classes */
		if((modifiers & CS_MODIFIER_NEW) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "`new' modifier is not permitted on non-nested types");
		}
	}
	else
	{
		/* Nested classes have a greater range of accessibilities */
		if((modifiers & CS_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `internal'");
			}
		}
		else if((modifiers & CS_MODIFIER_PRIVATE) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `private' and `internal'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `private' and `protected'");
			}
		}
		else if((modifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				attrs = IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM;
			}
			else
			{
				attrs = IL_META_TYPEDEF_NESTED_FAMILY;
			}
		}
		else if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_ASSEMBLY;
		}
		else
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
		}
	}

	/* Process "sealed", "abstract", and "unsafe" modifiers */
	if((modifiers & CS_MODIFIER_SEALED) != 0)
	{
		attrs |= IL_META_TYPEDEF_SEALED;
	}
	if((modifiers & CS_MODIFIER_ABSTRACT) != 0)
	{
		attrs |= IL_META_TYPEDEF_ABSTRACT;
	}

	/* Report errors for any remaining modifiers */
	BadModifiers(node,
				 modifiers & INVALID_CLASS_MODIFIERS);

	/* We have the attributes we wanted now */
	return attrs;
}

ILUInt32 CSModifiersToDelegateAttrs(ILNode *node, ILUInt32 modifiers,
									int isNested)
{
	ILUInt32 attrs;

	/* Determine the access level of the delegate */
	if(!isNested)
	{
		/* Only "public" and "internal" can be used at the outermost scope */
		if((modifiers & CS_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `internal'");
			}
		}
		else if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `internal' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `internal' and `protected'");
			}
		}
		else
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`private' modifier is not permitted "
							  "on non-nested delegates");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`protected' modifier is not permitted "
							  "on non-nested delegates");
			}
		}

		/* The "new" modifier is not allowed on top-level delegates */
		if((modifiers & CS_MODIFIER_NEW) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "`new' modifier is not permitted on non-nested delegates");
		}
	}
	else
	{
		/* Nested delegates have a greater range of accessibilities */
		if((modifiers & CS_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PUBLIC;
			if((modifiers & CS_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `private'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `protected'");
			}
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `public' and `internal'");
			}
		}
		else if((modifiers & CS_MODIFIER_PRIVATE) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `private' and `internal'");
			}
			if((modifiers & CS_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `private' and `protected'");
			}
		}
		else if((modifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			if((modifiers & CS_MODIFIER_INTERNAL) != 0)
			{
				attrs = IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM;
			}
			else
			{
				attrs = IL_META_TYPEDEF_NESTED_FAMILY;
			}
		}
		else if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_ASSEMBLY;
		}
		else
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
		}
	}

	/* Report errors for any remaining modifiers */
	BadModifiers(node,
				 modifiers & (CS_MODIFIER_STATIC | CS_MODIFIER_READONLY |
							  CS_MODIFIER_VIRTUAL | CS_MODIFIER_OVERRIDE |
							  CS_MODIFIER_EXTERN | CS_MODIFIER_VOLATILE |
							  CS_MODIFIER_SEALED | CS_MODIFIER_ABSTRACT));

	/* Delegates are always sealed and serializable */
	attrs |= IL_META_TYPEDEF_SEALED | IL_META_TYPEDEF_SERIALIZABLE;

	/* We have the attributes we wanted now */
	return attrs;
}

/*
 * Validate access modifiers and return the access level.
 */
static ILUInt32 ValidateAccess(ILNode *node, ILUInt32 modifiers)
{
	if((modifiers & CS_MODIFIER_PUBLIC) != 0)
	{
		if((modifiers & CS_MODIFIER_PRIVATE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `public' and `private'");
		}
		if((modifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `public' and `protected'");
		}
		if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `public' and `internal'");
		}
		return IL_META_FIELDDEF_PUBLIC;
	}
	else if((modifiers & CS_MODIFIER_PRIVATE) != 0)
	{
		if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `private' and `internal'");
		}
		if((modifiers & CS_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `private' and `protected'");
		}
		return IL_META_FIELDDEF_PRIVATE;
	}
	else if((modifiers & CS_MODIFIER_PROTECTED) != 0)
	{
		if((modifiers & CS_MODIFIER_INTERNAL) != 0)
		{
			return IL_META_FIELDDEF_FAM_OR_ASSEM;
		}
		else
		{
			return IL_META_FIELDDEF_FAMILY;
		}
	}
	else if((modifiers & CS_MODIFIER_INTERNAL) != 0)
	{
		return IL_META_FIELDDEF_ASSEMBLY;
	}
	else
	{
		return IL_META_FIELDDEF_PRIVATE;
	}
}

ILUInt32 CSModifiersToConstAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Add the "literal" and "static" attributes */
	attrs |= IL_META_FIELDDEF_LITERAL | IL_META_FIELDDEF_STATIC;

	/* Report errors for the remaining modifiers */
	BadModifiers(node,
				 modifiers & (CS_MODIFIER_ABSTRACT | CS_MODIFIER_SEALED |
							  CS_MODIFIER_STATIC | CS_MODIFIER_READONLY |
							  CS_MODIFIER_VIRTUAL | CS_MODIFIER_OVERRIDE |
							  CS_MODIFIER_EXTERN | CS_MODIFIER_UNSAFE |
							  CS_MODIFIER_VOLATILE));

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the "static", "readonly", and "new" modifiers */
	if((modifiers & CS_MODIFIER_STATIC) != 0)
	{
		attrs |= IL_META_FIELDDEF_STATIC;
	}
	if((modifiers & CS_MODIFIER_READONLY) != 0)
	{
		attrs |= IL_META_FIELDDEF_INIT_ONLY;
	}

	/* Report errors for the remaining modifiers */
	BadModifiers(node,
				 modifiers & (CS_MODIFIER_ABSTRACT | CS_MODIFIER_SEALED |
							  CS_MODIFIER_VIRTUAL | CS_MODIFIER_OVERRIDE |
							  CS_MODIFIER_EXTERN));

	/* Done */
	return attrs;
}

/*
 * Validate calling conventions for a method-like construct.
 */
static ILUInt32 ValidateCalling(ILNode *node, ILUInt32 modifiers,
								ILUInt32 access)
{
	ILUInt32 attrs = 0;

	/* Process the calling convention modifiers */
	if((modifiers & CS_MODIFIER_STATIC) != 0)
	{
		attrs |= IL_META_METHODDEF_STATIC;
		if((modifiers & CS_MODIFIER_VIRTUAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `virtual'");
		}
		if((modifiers & CS_MODIFIER_ABSTRACT) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `abstract'");
		}
		if((modifiers & CS_MODIFIER_OVERRIDE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `override'");
		}
		if((modifiers & CS_MODIFIER_SEALED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `static' and `sealed'");
		}
	}
	else if((modifiers & CS_MODIFIER_ABSTRACT) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_ABSTRACT;
		if((modifiers & CS_MODIFIER_OVERRIDE) == 0)
		{
			attrs |= IL_META_METHODDEF_NEW_SLOT;
		}
		else if((modifiers & CS_MODIFIER_NEW) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `override' and `new'");
		}
		if((modifiers & CS_MODIFIER_VIRTUAL) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `abstract' and `virtual'");
		}
		if((modifiers & CS_MODIFIER_SEALED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `abstract' and `sealed'");
		}
	}
	else if((modifiers & CS_MODIFIER_VIRTUAL) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT;
		if((modifiers & CS_MODIFIER_OVERRIDE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `virtual' and `override'");
		}
		if((modifiers & CS_MODIFIER_SEALED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `virtual' and `sealed'");
		}
	}
	else if((modifiers & CS_MODIFIER_OVERRIDE) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL;
		if((modifiers & CS_MODIFIER_NEW) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `override' and `new'");
		}
		if((modifiers & CS_MODIFIER_SEALED) != 0)
		{
			attrs |= IL_META_METHODDEF_FINAL;
		}
	}
	else
	{
		if((modifiers & CS_MODIFIER_SEALED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use `sealed' without `override'");
		}
	}

	/* Virtual methods cannot be private */
	if((modifiers & (CS_MODIFIER_ABSTRACT | CS_MODIFIER_VIRTUAL |
					 CS_MODIFIER_OVERRIDE)) != 0)
	{
		if(access == IL_META_FIELDDEF_PRIVATE)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  "cannot declare virtual or abstract methods to be private");
		}
	}

	/* Methods always need "hide by sig" */
	if(!CSNoHideBySig)
	{
		attrs |= IL_META_METHODDEF_HIDE_BY_SIG;
	}

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the calling convention attributes */
	attrs |= ValidateCalling(node, modifiers, attrs);

	/* Process the other method modifiers */
	if((modifiers & CS_MODIFIER_METHOD_SPECIAL_NAME) != 0)
	{
		attrs |= IL_META_METHODDEF_SPECIAL_NAME;
	}
	if((modifiers & CS_MODIFIER_METHOD_RT_SPECIAL_NAME) != 0)
	{
		attrs |= IL_META_METHODDEF_RT_SPECIAL_NAME;
	}
	if((modifiers & CS_MODIFIER_METHOD_COMPILER_CONTROLED) != 0)
	{
		attrs |= IL_META_METHODDEF_COMPILER_CONTROLLED;
	}
	if((modifiers & CS_MODIFIER_METHOD_HIDE_BY_SIG) != 0)
	{
		attrs |= IL_META_METHODDEF_HIDE_BY_SIG;
	}

	/* Report errors for the remaining modifiers */
	BadModifiers(node, modifiers & CS_MODIFIER_VOLATILE);

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToEventAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the calling convention attributes */
	attrs |= ValidateCalling(node, modifiers, attrs);

	/* Events always need the "specialname" attribute */
	attrs |= IL_META_METHODDEF_SPECIAL_NAME;

	/* Report errors for the remaining modifiers */
	BadModifiers(node, modifiers & (CS_MODIFIER_EXTERN | CS_MODIFIER_VOLATILE));

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToPropertyAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = ValidateAccess(node, modifiers);

	/* Process the calling convention attributes */
	attrs |= ValidateCalling(node, modifiers, attrs);

	/* Properties always need the "specialname" attribute */
	attrs |= IL_META_METHODDEF_SPECIAL_NAME;

	/* Report errors for the remaining modifiers */
	BadModifiers(node, modifiers & (CS_MODIFIER_VOLATILE));

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToOperatorAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs = 0;
	if((modifiers & CS_MODIFIER_PUBLIC) == 0)
	{
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "operators must have `public' access");
	}
	if((modifiers & CS_MODIFIER_STATIC) == 0)
	{
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "operators must have `static' access");
	}

	BadModifiers(node,
				 modifiers & ~(CS_MODIFIER_PUBLIC | CS_MODIFIER_STATIC |
							   CS_MODIFIER_UNSAFE));
	return (attrs | IL_META_METHODDEF_PUBLIC | IL_META_METHODDEF_STATIC |
			IL_META_METHODDEF_SPECIAL_NAME | IL_META_METHODDEF_HIDE_BY_SIG);
}

ILUInt32 CSModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs;

	/* Different flags are used for instance and static constructors */
	if((modifiers & CS_MODIFIER_STATIC) == 0)
	{
		attrs = ValidateAccess(node, modifiers);

		BadModifiers(node,
					 modifiers & ~(CS_MODIFIER_PUBLIC |
								   CS_MODIFIER_PRIVATE |
								   CS_MODIFIER_PROTECTED |
								   CS_MODIFIER_INTERNAL |
								   CS_MODIFIER_EXTERN |
								   CS_MODIFIER_UNSAFE));
	}
	else
	{
		BadModifiers(node,
					 modifiers & ~(CS_MODIFIER_STATIC | CS_MODIFIER_UNSAFE));
		attrs = IL_META_METHODDEF_PRIVATE | IL_META_METHODDEF_STATIC;
	}

	/* Add the "hidebysig" and "*specialname" attributes always */
	attrs |= IL_META_METHODDEF_HIDE_BY_SIG |
			 IL_META_METHODDEF_SPECIAL_NAME |
			 IL_META_METHODDEF_RT_SPECIAL_NAME;

	/* Done */
	return attrs;
}

ILUInt32 CSModifiersToDestructorAttrs(ILNode *node, ILUInt32 modifiers)
{
	ILUInt32 attrs=0;
	
	BadModifiers(node,
				 modifiers & ~(CS_MODIFIER_EXTERN |
							   CS_MODIFIER_UNSAFE));

	if((modifiers & (CS_MODIFIER_EXTERN | CS_MODIFIER_EXTERN)) != 0)
	{
		CCWarningOnLine(yygetfilename(node),yygetlinenum(node),
							"'extern' and 'unsafe' modifiers used together");
	}

	/* Add the "hidebysig" and "*specialname" attributes always */
	attrs |= IL_META_METHODDEF_FAMILY |
			 IL_META_METHODDEF_HIDE_BY_SIG |
			 IL_META_METHODDEF_VIRTUAL;

	/* Done */
	return attrs;
}

#ifdef	__cplusplus
};
#endif
