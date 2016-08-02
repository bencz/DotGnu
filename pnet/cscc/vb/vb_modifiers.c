/*
 * vb_modifiers.c - Verify modifier combinations.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#include "vb_internal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Report errors for each modifier in a mask.
 */
static void VBModifierError(char *filename, long linenum,
						    ILUInt32 modifiers, const char *msg)
{
	if((modifiers & VB_MODIFIER_PUBLIC) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Public");
	}
	if((modifiers & VB_MODIFIER_PROTECTED) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Protected");
	}
	if((modifiers & VB_MODIFIER_FRIEND) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Friend");
	}
	if((modifiers & VB_MODIFIER_PRIVATE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Private");
	}
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Shadows");
	}
	if((modifiers & VB_MODIFIER_MUST_INHERIT) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "MustInherit");
	}
	if((modifiers & VB_MODIFIER_NOT_INHERITABLE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "NotInheritable");
	}
	if((modifiers & VB_MODIFIER_SHARED) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Shared");
	}
	if((modifiers & VB_MODIFIER_OVERRIDABLE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Overridable");
	}
	if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "NotOverridable");
	}
	if((modifiers & VB_MODIFIER_MUST_OVERRIDE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "MustOverride");
	}
	if((modifiers & VB_MODIFIER_OVERRIDES) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Overrides");
	}
	if((modifiers & VB_MODIFIER_OVERLOADS) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Overloads");
	}
	if((modifiers & VB_MODIFIER_READ_ONLY) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "ReadOnly");
	}
	if((modifiers & VB_MODIFIER_WRITE_ONLY) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "WriteOnly");
	}
	if((modifiers & VB_MODIFIER_WITH_EVENTS) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "WithEvents");
	}
	if((modifiers & VB_MODIFIER_DEFAULT) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Default");
	}
	if((modifiers & VB_MODIFIER_EXTERN) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Extern");
	}
	if((modifiers & VB_MODIFIER_VOLATILE) != 0)
	{
		CCErrorOnLine(filename, linenum, msg, "Volatile");
	}
}

/*
 * Report errors for modifiers that cannot be used in a particular context.
 */
static void VBBadModifiers(ILNode *node, ILUInt32 modifiers)
{
	VBModifierError(yygetfilename(node), yygetlinenum(node),
				    modifiers, "`%s' cannot be used in this context");
}

void VBModifiersUsedTwice(char *filename, long linenum, ILUInt32 modifiers)
{
	VBModifierError(filename, linenum,
				    modifiers, "`%s' specified multiple times");
}

static ILUInt32 VBValidateTypeAccess(ILNode *node, ILUInt32 modifiers,
								     int isNested, ILUInt32 extras)
{
	ILUInt32 attrs;

	/* Determine the access level of the class */
	if(!isNested)
	{
		/* Only "public" and "internal" can be used at the outermost scope */
		if((modifiers & VB_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_PUBLIC;
			if((modifiers & VB_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Private'");
			}
			if((modifiers & VB_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Protected'");
			}
			if((modifiers & VB_MODIFIER_FRIEND) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Friend'");
			}
		}
		else if((modifiers & VB_MODIFIER_FRIEND) != 0)
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & VB_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Friend' and `Private'");
			}
			if((modifiers & VB_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Friend' and `Protected'");
			}
		}
		else
		{
			attrs = IL_META_TYPEDEF_NOT_PUBLIC;
			if((modifiers & VB_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`Private' modifier is not permitted "
							  "on non-nested types");
			}
			if((modifiers & VB_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "`Protected' modifier is not permitted "
							  "on non-nested types");
			}
		}

		/* The "Shadows" modifier is not allowed on top-level classes */
		if((modifiers & VB_MODIFIER_SHADOWS) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
			  "`Shadows' modifier is not permitted on non-nested types");
		}
	}
	else
	{
		/* Nested classes have a greater range of accessibilities */
		if((modifiers & VB_MODIFIER_PUBLIC) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PUBLIC;
			if((modifiers & VB_MODIFIER_PRIVATE) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Private'");
			}
			if((modifiers & VB_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Protected'");
			}
			if((modifiers & VB_MODIFIER_FRIEND) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Public' and `Friend'");
			}
		}
		else if((modifiers & VB_MODIFIER_PRIVATE) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
			if((modifiers & VB_MODIFIER_FRIEND) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Private' and `Friend'");
			}
			if((modifiers & VB_MODIFIER_PROTECTED) != 0)
			{
				CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
							  "cannot use both `Private' and `Protected'");
			}
		}
		else if((modifiers & VB_MODIFIER_PROTECTED) != 0)
		{
			if((modifiers & VB_MODIFIER_FRIEND) != 0)
			{
				attrs = IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM;
			}
			else
			{
				attrs = IL_META_TYPEDEF_NESTED_FAMILY;
			}
		}
		else if((modifiers & VB_MODIFIER_FRIEND) != 0)
		{
			attrs = IL_META_TYPEDEF_NESTED_ASSEMBLY;
		}
		else
		{
			attrs = IL_META_TYPEDEF_NESTED_PRIVATE;
		}

		/* Process the "Shadows" modifier */
		if((modifiers & VB_MODIFIER_SHADOWS) != 0)
		{
			attrs |= CS_SPECIALATTR_NEW;
		}
	}

	/* Process "NotInheritable" and "MustInherit" */
	if((modifiers & extras & VB_MODIFIER_NOT_INHERITABLE) != 0)
	{
		attrs |= IL_META_TYPEDEF_SEALED;
	}
	if((modifiers & extras & VB_MODIFIER_MUST_INHERIT) != 0)
	{
		attrs |= IL_META_TYPEDEF_ABSTRACT;
	}

	/* Report errors for any remaining modifiers */
	VBBadModifiers(node, modifiers & ~extras & ~VB_MODIFIER_TYPE);

	/* We have the attributes we wanted now */
	return attrs;
}

ILUInt32 VBModifiersToTypeAttrs(ILNode *node, ILUInt32 modifiers, int isNested)
{
	return VBValidateTypeAccess(node, modifiers, isNested,
							    VB_MODIFIER_NOT_INHERITABLE |
							    VB_MODIFIER_MUST_INHERIT);
}

ILUInt32 VBModifiersToDelegateAttrs(ILNode *node, ILUInt32 modifiers,
									int isNested)
{
	return VBValidateTypeAccess(node, modifiers, isNested,
							    VB_MODIFIER_NOT_INHERITABLE |
							    VB_MODIFIER_MUST_INHERIT) |
			IL_META_TYPEDEF_SEALED | IL_META_TYPEDEF_SERIALIZABLE;
}

/*
 * Validate access modifiers and return the access level.
 */
static ILUInt32 VBValidateAccess(ILNode *node, ILUInt32 modifiers,
								 int isModule)
{
	int shared = 0;
	if(isModule)
	{
		/* The access defaults to "public" in a module */
		if((modifiers & VB_MODIFIER_ACCESS) == 0)
		{
			modifiers |= VB_MODIFIER_PUBLIC;
		}
	}
	if((modifiers & VB_MODIFIER_PUBLIC) != 0)
	{
		if((modifiers & VB_MODIFIER_PRIVATE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Public' and `Private'");
		}
		if((modifiers & VB_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Public' and `Protected'");
		}
		if((modifiers & VB_MODIFIER_FRIEND) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Public' and `Friend'");
		}
		return IL_META_FIELDDEF_PUBLIC | shared;
	}
	else if((modifiers & VB_MODIFIER_PRIVATE) != 0)
	{
		if((modifiers & VB_MODIFIER_FRIEND) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Private' and `Friend'");
		}
		if((modifiers & VB_MODIFIER_PROTECTED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Private' and `Protected'");
		}
		return IL_META_FIELDDEF_PRIVATE | shared;
	}
	else if((modifiers & VB_MODIFIER_PROTECTED) != 0)
	{
		if((modifiers & VB_MODIFIER_FRIEND) != 0)
		{
			return IL_META_FIELDDEF_FAM_OR_ASSEM | shared;
		}
		else
		{
			return IL_META_FIELDDEF_FAMILY | shared;
		}
	}
	else if((modifiers & VB_MODIFIER_FRIEND) != 0)
	{
		return IL_META_FIELDDEF_ASSEMBLY | shared;
	}
	else
	{
		return IL_META_FIELDDEF_PRIVATE | shared;
	}
}

ILUInt32 VBModifiersToConstAttrs(ILNode *node, ILUInt32 modifiers,
								 int isModule)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = VBValidateAccess(node, modifiers, isModule);

	/* Process the "Shadows" modifier */
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		attrs |= CS_SPECIALATTR_NEW;
	}

	/* Add the "literal" and "static" attributes */
	attrs |= IL_META_FIELDDEF_LITERAL | IL_META_FIELDDEF_STATIC;

	/* Report errors for the remaining modifiers */
	VBBadModifiers(node,
				   modifiers & ~(VB_MODIFIER_ACCESS | VB_MODIFIER_SHADOWS));

	/* Done */
	return attrs;
}

ILUInt32 VBModifiersToFieldAttrs(ILNode *node, ILUInt32 modifiers, int isModule)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = VBValidateAccess(node, modifiers, isModule);

	/* Process the "shared", "readonly", and "shadows" modifiers */
	if((modifiers & VB_MODIFIER_SHARED) != 0)
	{
		attrs |= IL_META_FIELDDEF_STATIC;
	}
	if((modifiers & VB_MODIFIER_READ_ONLY) != 0)
	{
		attrs |= IL_META_FIELDDEF_INIT_ONLY;
	}
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		attrs |= CS_SPECIALATTR_NEW;
	}

	/* Report errors for the remaining modifiers */
	VBBadModifiers(node,
				   modifiers & ~(VB_MODIFIER_ACCESS |
				   				 VB_MODIFIER_SHARED |
				   				 VB_MODIFIER_READ_ONLY |
								 VB_MODIFIER_SHADOWS));

	/* Done */
	return attrs;
}

/*
 * Validate calling conventions for a method-like construct.
 */
static ILUInt32 VBValidateCalling(ILNode *node, ILUInt32 modifiers,
								  ILUInt32 access, int isModule)
{
	ILUInt32 attrs = 0;

	/* Process the calling convention modifiers */
	if(isModule)
	{
		attrs |= IL_META_METHODDEF_STATIC;
		if((modifiers & VB_MODIFIER_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("cannot use `Overridable' in modules"));
		}
		if((modifiers & VB_MODIFIER_MUST_OVERRIDE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("cannot use `MustOverride' in modules"));
		}
		if((modifiers & VB_MODIFIER_OVERRIDES) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("cannot use `Overrides' in modules"));
		}
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("cannot use `NotOverridable' in modules"));
		}
		if((modifiers & VB_MODIFIER_SHARED) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("cannot use `Shared' in modules"));
		}
	}
	else if((modifiers & VB_MODIFIER_SHARED) != 0)
	{
		attrs |= IL_META_METHODDEF_STATIC;
		if((modifiers & VB_MODIFIER_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Shared' and `Overridable'");
		}
		if((modifiers & VB_MODIFIER_MUST_OVERRIDE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Shared' and `MustOverride'");
		}
		if((modifiers & VB_MODIFIER_OVERRIDES) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Shared' and `Overrides'");
		}
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Shared' and `NotOverridable'");
		}
	}
	else if((modifiers & VB_MODIFIER_MUST_OVERRIDE) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_ABSTRACT;
		if((modifiers & VB_MODIFIER_OVERRIDES) == 0)
		{
			attrs |= IL_META_METHODDEF_NEW_SLOT;
		}
		else if((modifiers & VB_MODIFIER_SHADOWS) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Overrides' and `Shadows'");
			attrs |= CS_SPECIALATTR_OVERRIDE;
		}
		else
		{
			attrs |= CS_SPECIALATTR_OVERRIDE;
		}
		if((modifiers & VB_MODIFIER_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `MustOverride' and `Overridable'");
		}
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "cannot use both `MustOverride' and `NotOverridable'");
		}
	}
	else if((modifiers & VB_MODIFIER_OVERRIDABLE) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | IL_META_METHODDEF_NEW_SLOT;
		if((modifiers & VB_MODIFIER_OVERRIDES) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Overrideable' and `Overrides'");
		}
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "cannot use both `Overridable' and `NotOverridable'");
		}
	}
	else if((modifiers & VB_MODIFIER_OVERRIDES) != 0)
	{
		attrs |= IL_META_METHODDEF_VIRTUAL | CS_SPECIALATTR_OVERRIDE;
		if((modifiers & VB_MODIFIER_SHADOWS) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  "cannot use both `Overrides' and `Shadows'");
		}
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			attrs |= IL_META_METHODDEF_FINAL;
		}
	}
	else
	{
		if((modifiers & VB_MODIFIER_NOT_OVERRIDABLE) != 0)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  "cannot use `NotOverridable' without `Overrides'");
		}
	}

	/* Virtual methods cannot be private */
	if((modifiers & (VB_MODIFIER_MUST_OVERRIDE | VB_MODIFIER_OVERRIDABLE |
					 VB_MODIFIER_OVERRIDES)) != 0)
	{
		if(access == IL_META_FIELDDEF_PRIVATE)
		{
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
				  "cannot declare overridable methods to be private");
		}
	}

	/* Methods always need "hide by sig" */
	attrs |= IL_META_METHODDEF_HIDE_BY_SIG;

	/* Done */
	return attrs;
}

ILUInt32 VBModifiersToMethodAttrs(ILNode *node, ILUInt32 modifiers,
								  int isModule)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = VBValidateAccess(node, modifiers, isModule);

	/* Process the calling convention attributes */
	attrs |= VBValidateCalling(node, modifiers, attrs, isModule);

	/* Process the other method modifiers */
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		attrs |= CS_SPECIALATTR_NEW;
	}
	if((modifiers & VB_MODIFIER_EXTERN) != 0)
	{
		attrs |= CS_SPECIALATTR_EXTERN;
	}

	/* Report errors for the remaining modifiers */
	VBBadModifiers(node, modifiers & ~(VB_MODIFIER_ACCESS |
									   VB_MODIFIER_CALLING |
									   VB_MODIFIER_SHADOWS |
									   VB_MODIFIER_EXTERN));

	/* Done */
	return attrs;
}

ILUInt32 VBModifiersToEventAttrs(ILNode *node, ILUInt32 modifiers,
								 int isModule)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = VBValidateAccess(node, modifiers, isModule);

	/* Process the calling convention attributes */
	attrs |= VBValidateCalling(node, modifiers, attrs, isModule);

	/* Process the other property modifiers */
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		attrs |= CS_SPECIALATTR_NEW;
	}

	/* Events always need the "specialname" attribute */
	attrs |= IL_META_METHODDEF_SPECIAL_NAME;

	/* Report errors for the remaining modifiers */
	VBBadModifiers(node, modifiers & ~(VB_MODIFIER_ACCESS |
									   VB_MODIFIER_CALLING |
									   VB_MODIFIER_SHADOWS));

	/* Done */
	return attrs;
}

ILUInt32 VBModifiersToPropertyAttrs(ILNode *node, ILUInt32 modifiers,
									int isModule)
{
	ILUInt32 attrs;

	/* Process the common attributes */
	attrs = VBValidateAccess(node, modifiers, isModule);

	/* Process the calling convention attributes */
	attrs |= VBValidateCalling(node, modifiers, attrs, isModule);

	/* Process the other property modifiers */
	if((modifiers & VB_MODIFIER_SHADOWS) != 0)
	{
		attrs |= CS_SPECIALATTR_NEW;
	}
	if((modifiers & VB_MODIFIER_EXTERN) != 0)
	{
		attrs |= CS_SPECIALATTR_EXTERN;
	}

	/* Properties always need the "specialname" attribute */
	attrs |= IL_META_METHODDEF_SPECIAL_NAME;

	/* Report errors for the remaining modifiers */
	VBBadModifiers(node, modifiers & ~(VB_MODIFIER_ACCESS |
									   VB_MODIFIER_CALLING |
									   VB_MODIFIER_SHADOWS |
									   VB_MODIFIER_EXTERN));

	/* Done */
	return attrs;
}

ILUInt32 VBModifiersToConstructorAttrs(ILNode *node, ILUInt32 modifiers,
									   int isModule)
{
	ILUInt32 attrs;

	/* Different flags are used for instance and static constructors */
	if(!isModule && (modifiers & VB_MODIFIER_SHARED) == 0)
	{
		attrs = VBValidateAccess(node, modifiers, isModule);
		if((modifiers & VB_MODIFIER_EXTERN) != 0)
		{
			attrs |= CS_SPECIALATTR_EXTERN;
		}
		VBBadModifiers(node, modifiers & ~(VB_MODIFIER_ACCESS |
										   VB_MODIFIER_EXTERN));
	}
	else
	{
		VBBadModifiers(node, modifiers & ~(VB_MODIFIER_SHARED));
		attrs = IL_META_METHODDEF_PRIVATE | IL_META_METHODDEF_STATIC;
	}

	/* Add the "hidebysig" and "*specialname" attributes always */
	attrs |= IL_META_METHODDEF_HIDE_BY_SIG |
			 IL_META_METHODDEF_SPECIAL_NAME |
			 IL_META_METHODDEF_RT_SPECIAL_NAME;

	/* Done */
	return attrs;
}

#ifdef	__cplusplus
};
#endif
