/*
 * dump_flags.c - Utilities that assist with dumping program item flags.
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

#include "il_dumpasm.h"
#include "il_meta.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Type definition flag table.
 */
ILFlagInfo const ILTypeDefinitionFlags[] = {
	{"private", IL_META_TYPEDEF_NOT_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"public", IL_META_TYPEDEF_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested public", IL_META_TYPEDEF_NESTED_PUBLIC,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested private", IL_META_TYPEDEF_NESTED_PRIVATE,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested family", IL_META_TYPEDEF_NESTED_FAMILY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested assembly", IL_META_TYPEDEF_NESTED_ASSEMBLY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested famandassem", IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested famorassem", IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"auto", IL_META_TYPEDEF_AUTO_LAYOUT, IL_META_TYPEDEF_LAYOUT_MASK},
	{"sequential", IL_META_TYPEDEF_LAYOUT_SEQUENTIAL,
				IL_META_TYPEDEF_LAYOUT_MASK},
	{"explicit", IL_META_TYPEDEF_EXPLICIT_LAYOUT,
				IL_META_TYPEDEF_LAYOUT_MASK},
	{"/class", IL_META_TYPEDEF_CLASS,
				IL_META_TYPEDEF_CLASS_SEMANTICS_MASK},
	{"interface", IL_META_TYPEDEF_INTERFACE,
				IL_META_TYPEDEF_CLASS_SEMANTICS_MASK},
	{"value", IL_META_TYPEDEF_VALUE_TYPE,
				IL_META_TYPEDEF_CLASS_SEMANTICS_MASK},
	{"unmanaged", IL_META_TYPEDEF_UNMANAGED_VALUE_TYPE,
				IL_META_TYPEDEF_CLASS_SEMANTICS_MASK},
	{"abstract", IL_META_TYPEDEF_ABSTRACT, 0},
	{"sealed", IL_META_TYPEDEF_SEALED, 0},
	{"enum", IL_META_TYPEDEF_ENUM, 0},
	{"specialname", IL_META_TYPEDEF_SPECIAL_NAME, 0},
	{"import", IL_META_TYPEDEF_IMPORT, 0},
	{"serializable", IL_META_TYPEDEF_SERIALIZABLE, 0},
	{"ansi", IL_META_TYPEDEF_ANSI_CLASS, IL_META_TYPEDEF_STRING_FORMAT_MASK},
	{"unicode", IL_META_TYPEDEF_UNICODE_CLASS,
				IL_META_TYPEDEF_STRING_FORMAT_MASK},
	{"autochar", IL_META_TYPEDEF_AUTO_CLASS,
				IL_META_TYPEDEF_STRING_FORMAT_MASK},
	{"*unknown_string_format", 0x00030000,
				IL_META_TYPEDEF_STRING_FORMAT_MASK},
	{"lateinit", IL_META_TYPEDEF_LATE_INIT, 0},
	{"beforefieldinit", IL_META_TYPEDEF_BEFORE_FIELD_INIT, 0},
	{"rtspecialname", IL_META_TYPEDEF_RT_SPECIAL_NAME, 0},
	{0, IL_META_TYPEDEF_RT_SPECIAL_NAME |
		IL_META_TYPEDEF_HAS_SECURITY, 0xFFEC0800},
};

/*
 * Exported type definition flag table.
 */
ILFlagInfo const ILExportedTypeDefinitionFlags[] = {
	{"private", IL_META_TYPEDEF_NOT_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"public", IL_META_TYPEDEF_PUBLIC, IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested public", IL_META_TYPEDEF_NESTED_PUBLIC,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested private", IL_META_TYPEDEF_NESTED_PRIVATE,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested family", IL_META_TYPEDEF_NESTED_FAMILY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested assembly", IL_META_TYPEDEF_NESTED_ASSEMBLY,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested famandassem", IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{"nested famorassem", IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM,
				IL_META_TYPEDEF_VISIBILITY_MASK},
	{0, IL_META_TYPEDEF_RT_SPECIAL_NAME |
		IL_META_TYPEDEF_HAS_SECURITY, 0xFFEC0800},
};

/*
 * Field definition flag table.
 */
ILFlagInfo const ILFieldDefinitionFlags[] = {
	{"compilercontrolled", IL_META_FIELDDEF_COMPILER_CONTROLLED,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"private", IL_META_FIELDDEF_PRIVATE,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"famandassem", IL_META_FIELDDEF_FAM_AND_ASSEM,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"assembly", IL_META_FIELDDEF_ASSEMBLY,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"family", IL_META_FIELDDEF_FAMILY,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"famorassem", IL_META_FIELDDEF_FAM_OR_ASSEM,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"public", IL_META_FIELDDEF_PUBLIC,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"*unknown_field_access", 0x0007,
				IL_META_FIELDDEF_FIELD_ACCESS_MASK},
	{"static", IL_META_FIELDDEF_STATIC, 0},
	{"initonly", IL_META_FIELDDEF_INIT_ONLY, 0},
	{"literal", IL_META_FIELDDEF_LITERAL, 0},
	{"notserialized", IL_META_FIELDDEF_NOT_SERIALIZED, 0},
	{"specialname", IL_META_FIELDDEF_SPECIAL_NAME, 0},
	{"/pinvokeimpl", IL_META_FIELDDEF_PINVOKE_IMPL, 0},
	{"/marshal", IL_META_FIELDDEF_HAS_FIELD_MARSHAL, 0},
	{"rtspecialname", IL_META_FIELDDEF_RT_SPECIAL_NAME, 0},
	{0, IL_META_FIELDDEF_RT_SPECIAL_NAME |
		IL_META_FIELDDEF_HAS_FIELD_MARSHAL |
		IL_META_FIELDDEF_HAS_DEFAULT |
		IL_META_FIELDDEF_HAS_FIELD_RVA, 0xFFFF9508},
};

/*
 * Method definition flag table.
 */
ILFlagInfo const ILMethodDefinitionFlags[] = {
	{"compilercontrolled", IL_META_METHODDEF_COMPILER_CONTROLLED,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"private", IL_META_METHODDEF_PRIVATE,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"famandassem", IL_META_METHODDEF_FAM_AND_ASSEM,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"assembly", IL_META_METHODDEF_ASSEM,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"family", IL_META_METHODDEF_FAMILY,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"famorassem", IL_META_METHODDEF_FAM_OR_ASSEM,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"public", IL_META_METHODDEF_PUBLIC,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"*unknown_method_access", 0x0007,
				IL_META_METHODDEF_MEMBER_ACCESS_MASK},
	{"static", IL_META_METHODDEF_STATIC, 0},
	{"final", IL_META_METHODDEF_FINAL, 0},
	{"virtual", IL_META_METHODDEF_VIRTUAL, 0},
	{"hidebysig", IL_META_METHODDEF_HIDE_BY_SIG, 0},
	{"/reuseslot", IL_META_METHODDEF_REUSE_SLOT,
				IL_META_METHODDEF_VTABLE_LAYOUT_MASK},
	{"newslot", IL_META_METHODDEF_NEW_SLOT,
				IL_META_METHODDEF_VTABLE_LAYOUT_MASK},
	{"abstract", IL_META_METHODDEF_ABSTRACT, 0},
	{"specialname", IL_META_METHODDEF_SPECIAL_NAME, 0},
	{"/pinvokeimpl", IL_META_METHODDEF_PINVOKE_IMPL, 0},
	{"unmanagedexp", IL_META_METHODDEF_UNMANAGED_EXPORT, 0},
	{"rtspecialname", IL_META_METHODDEF_RT_SPECIAL_NAME, 0},
	{"reqsecobj", IL_META_METHODDEF_REQUIRE_SEC_OBJECT, 0},
	{0, IL_META_METHODDEF_RT_SPECIAL_NAME |
	    IL_META_METHODDEF_HAS_SECURITY |
		IL_META_METHODDEF_REQUIRE_SEC_OBJECT, 0xFFFFD200},
};

/*
 * Parameter definition flag table.
 */
ILFlagInfo const ILParameterDefinitionFlags[] = {
	{"in", IL_META_PARAMDEF_IN, 0},
	{"out", IL_META_PARAMDEF_OUT, 0},
	{"opt", IL_META_PARAMDEF_OPTIONAL, 0},
	{"retval", IL_META_PARAMDEF_RETVAL, 0},
	{0, 0, 0xFFFFFFE0},
};

/*
 * Property definition flag table.
 */
ILFlagInfo const ILPropertyDefinitionFlags[] = {
	{"specialname", IL_META_PROPDEF_SPECIAL_NAME, 0},
	{"rtspecialname", IL_META_PROPDEF_RT_SPECIAL_NAME, 0},
	{0, IL_META_PROPDEF_RT_SPECIAL_NAME |
		IL_META_PROPDEF_HAS_DEFAULT, 0xFFFFFDFF},
};

/*
 * Event definition flag table.
 */
ILFlagInfo const ILEventDefinitionFlags[] = {
	{"specialname", IL_META_EVENTDEF_SPECIAL_NAME, 0},
	{"rtspecialname", IL_META_EVENTDEF_RT_SPECIAL_NAME, 0},
	{0, IL_META_PROPDEF_RT_SPECIAL_NAME, 0xFFFFFDFF},
};

/*
 * Method semantics flag table.
 */
ILFlagInfo const ILMethodSemanticsFlags[] = {
	{"setter", IL_META_METHODSEM_SETTER, 0},
	{"getter", IL_META_METHODSEM_GETTER, 0},
	{"other", IL_META_METHODSEM_OTHER, 0},
	{"addon", IL_META_METHODSEM_ADD_ON, 0},
	{"removeon", IL_META_METHODSEM_REMOVE_ON, 0},
	{"fire", IL_META_METHODSEM_FIRE, 0},
	{0, 0, 0xFFFFFFC0},
};

/*
 * Method implementation flag table.
 */
ILFlagInfo const ILMethodImplementationFlags[] = {
	{"cil", IL_META_METHODIMPL_IL,
				IL_META_METHODIMPL_CODE_TYPE_MASK | IL_META_METHODIMPL_JAVA},
	{"native", IL_META_METHODIMPL_NATIVE,
				IL_META_METHODIMPL_CODE_TYPE_MASK},
	{"optil", IL_META_METHODIMPL_OPTIL,
				IL_META_METHODIMPL_CODE_TYPE_MASK},
	{"runtime", IL_META_METHODIMPL_RUNTIME,
				IL_META_METHODIMPL_CODE_TYPE_MASK},
	{"managed", IL_META_METHODIMPL_MANAGED,
				IL_META_METHODIMPL_MANAGED_MASK},
	{"unmanaged", IL_META_METHODIMPL_UNMANAGED,
				IL_META_METHODIMPL_MANAGED_MASK},
	{"noinlining", IL_META_METHODIMPL_NO_INLINING, 0},
	{"forwardref", IL_META_METHODIMPL_FORWARD_REF, 0},
	{"synchronized", IL_META_METHODIMPL_SYNCHRONIZED, 0},
	{"preservesig", IL_META_METHODIMPL_PRESERVE_SIG, 0},
	{"internalcall", IL_META_METHODIMPL_INTERNAL_CALL, 0},
	{"java_fp_strict", IL_META_METHODIMPL_JAVA_FP_STRICT, 0},
	{"java", IL_META_METHODIMPL_JAVA, 0},
	{0, 0, 0xFFFF8F40},
};

/*
 * Method calling convention flag table.
 */
ILFlagInfo const ILMethodCallConvFlags[] = {
	{"instance", IL_META_CALLCONV_HASTHIS, 0},
	{"explicit", IL_META_CALLCONV_EXPLICITTHIS, 0},
	{"/default", IL_META_CALLCONV_DEFAULT, IL_META_CALLCONV_MASK},
	{"unmanaged cdecl", IL_META_CALLCONV_C, IL_META_CALLCONV_MASK},
	{"unmanaged stdcall", IL_META_CALLCONV_STDCALL, IL_META_CALLCONV_MASK},
	{"unmanaged thiscall", IL_META_CALLCONV_THISCALL, IL_META_CALLCONV_MASK},
	{"unmanaged fastcall", IL_META_CALLCONV_FASTCALL, IL_META_CALLCONV_MASK},
	{"vararg", IL_META_CALLCONV_VARARG, IL_META_CALLCONV_MASK},
	{"*field", IL_META_CALLCONV_FIELD, IL_META_CALLCONV_MASK},
	{"*local_sig", IL_META_CALLCONV_LOCAL_SIG, IL_META_CALLCONV_MASK},
	{"*property", IL_META_CALLCONV_PROPERTY, IL_META_CALLCONV_MASK},
	{"*unmanaged", IL_META_CALLCONV_UNMGD, IL_META_CALLCONV_MASK},
	{"/instantiation", IL_META_CALLCONV_INSTANTIATION, IL_META_CALLCONV_MASK},
	{"*callconvB", 0x0B, IL_META_CALLCONV_MASK},
	{"*callconvC", 0x0C, IL_META_CALLCONV_MASK},
	{"*callconvD", 0x0D, IL_META_CALLCONV_MASK},
	{"*callconvE", 0x0E, IL_META_CALLCONV_MASK},
	{"*callconvF", 0x0F, IL_META_CALLCONV_MASK},
	{"/generic", IL_META_CALLCONV_GENERIC, 0},
	{0, 0, 0xFFFFFF80},
};

/*
 * Security flag table.
 */
ILFlagInfo const ILSecurityFlags[] = {
	{"/nil", IL_META_SECURITY_ACTION_NIL,
				IL_META_SECURITY_ACTION_MASK},
	{"request", IL_META_SECURITY_REQUEST,
				IL_META_SECURITY_ACTION_MASK},
	{"demand", IL_META_SECURITY_DEMAND,
				IL_META_SECURITY_ACTION_MASK},
	{"assert", IL_META_SECURITY_ASSERT,
				IL_META_SECURITY_ACTION_MASK},
	{"deny", IL_META_SECURITY_DENY,
				IL_META_SECURITY_ACTION_MASK},
	{"permitonly", IL_META_SECURITY_PERMIT_ONLY,
				IL_META_SECURITY_ACTION_MASK},
	{"linkcheck", IL_META_SECURITY_LINK_TIME_CHECK,
				IL_META_SECURITY_ACTION_MASK},
	{"inheritcheck", IL_META_SECURITY_INHERITANCE_CHECK,
				IL_META_SECURITY_ACTION_MASK},
	{"reqmin", IL_META_SECURITY_REQUEST_MINIMUM,
				IL_META_SECURITY_ACTION_MASK},
	{"reqopt", IL_META_SECURITY_REQUEST_OPTIONAL,
				IL_META_SECURITY_ACTION_MASK},
	{"reqrefuse", IL_META_SECURITY_REQUEST_REFUSE,
				IL_META_SECURITY_ACTION_MASK},
	{"prejitgrant", IL_META_SECURITY_PREJIT_GRANT,
				IL_META_SECURITY_ACTION_MASK},
	{"prejitdeny", IL_META_SECURITY_PREJIT_DENIED,
				IL_META_SECURITY_ACTION_MASK},
	{"noncasdemand", IL_META_SECURITY_NON_CAS_DEMAND,
				IL_META_SECURITY_ACTION_MASK},
	{"noncaslinkdemand", IL_META_SECURITY_NON_CAS_LINK_DEMAND,
				IL_META_SECURITY_ACTION_MASK},
	{"noncasinheritance", IL_META_SECURITY_NON_CAS_INHERITANCE,
				IL_META_SECURITY_ACTION_MASK},
	{0, 0, 0xFFFFFFF0},
};

/*
 * PInvoke implementation flag table.
 */
ILFlagInfo const ILPInvokeImplementationFlags[] = {
	{"nomangle", IL_META_PINVOKE_NO_MANGLE, 0},
	{"/charset_unspecified", IL_META_PINVOKE_CHAR_SET_NOT_SPEC,
				IL_META_PINVOKE_CHAR_SET_MASK},
	{"ansi", IL_META_PINVOKE_CHAR_SET_ANSI,
				IL_META_PINVOKE_CHAR_SET_MASK},
	{"unicode", IL_META_PINVOKE_CHAR_SET_UNICODE,
				IL_META_PINVOKE_CHAR_SET_MASK},
	{"autochar", IL_META_PINVOKE_CHAR_SET_AUTO,
				IL_META_PINVOKE_CHAR_SET_MASK},
	{"ole", IL_META_PINVOKE_OLE, 0},
	{"lasterr", IL_META_PINVOKE_SUPPORTS_LAST_ERROR, 0},
	{"*callconv0", 0x0000, IL_META_PINVOKE_CALL_CONV_MASK},
	{"winapi", IL_META_PINVOKE_CALL_CONV_WINAPI,
			IL_META_PINVOKE_CALL_CONV_MASK},
	{"cdecl", IL_META_PINVOKE_CALL_CONV_CDECL,
			IL_META_PINVOKE_CALL_CONV_MASK},
	{"stdcall", IL_META_PINVOKE_CALL_CONV_STDCALL,
			IL_META_PINVOKE_CALL_CONV_MASK},
	{"thiscall", IL_META_PINVOKE_CALL_CONV_THISCALL,
			IL_META_PINVOKE_CALL_CONV_MASK},
	{"fastcall", IL_META_PINVOKE_CALL_CONV_FASTCALL,
			IL_META_PINVOKE_CALL_CONV_MASK},
	{"*callconv6", 0x0600, IL_META_PINVOKE_CALL_CONV_MASK},
	{"*callconv7", 0x0700, IL_META_PINVOKE_CALL_CONV_MASK},
	{0, 0, 0xFFFFF880},
};

/*
 * Assembly flag table.
 */
ILFlagInfo const ILAssemblyFlags[] = {
	{"/publickey", IL_META_ASSEM_PUBLIC_KEY, 0},
	{"/side_by_side", IL_META_ASSEM_SIDE_BY_SIDE_COMPATIBLE,
				IL_META_ASSEM_COMPATIBILITY_MASK},
	{"noappdomain", IL_META_ASSEM_NON_SIDE_BY_SIDE_APP_DOMAIN,
				IL_META_ASSEM_COMPATIBILITY_MASK},
	{"noprocess", IL_META_ASSEM_NON_SIDE_BY_SIDE_PROCESS,
				IL_META_ASSEM_COMPATIBILITY_MASK},
	{"nomachine", IL_META_ASSEM_NON_SIDE_BY_SIDE_MACHINE,
				IL_META_ASSEM_COMPATIBILITY_MASK},
	{"retargetable", IL_META_ASSEM_RETARGETABLE, 0},
	{"enablejittracking", IL_META_ASSEM_ENABLE_JIT_TRACKING, 0},
	{"disablejitoptimizer", IL_META_ASSEM_DISABLE_JIT_OPTIMIZER, 0},
	{0, 0, 0xFFFF3E8E},
};

/*
 * Assembly reference flag table.
 */
ILFlagInfo const ILAssemblyRefFlags[] = {
	{"/fullorigin", IL_META_ASSEMREF_FULL_ORIGINATOR, 0},
	{0, 0, 0xFFFFFFFE},
};

/*
 * Manifest resources flag table.
 */
ILFlagInfo const ILManifestResFlags[] = {
	{"*manifest_access0", 0x0000, IL_META_MANIFEST_VISIBILITY_MASK},
	{"public", IL_META_MANIFEST_PUBLIC, IL_META_MANIFEST_VISIBILITY_MASK},
	{"private", IL_META_MANIFEST_PRIVATE, IL_META_MANIFEST_VISIBILITY_MASK},
	{"*manifest_access3", 0x0003, IL_META_MANIFEST_VISIBILITY_MASK},
	{"*manifest_access4", 0x0004, IL_META_MANIFEST_VISIBILITY_MASK},
	{"*manifest_access5", 0x0005, IL_META_MANIFEST_VISIBILITY_MASK},
	{"*manifest_access6", 0x0006, IL_META_MANIFEST_VISIBILITY_MASK},
	{"*manifest_access7", 0x0007, IL_META_MANIFEST_VISIBILITY_MASK},
	{0, 0, 0xFFFFFFF8},
};

/*
 * File flag table.
 */
ILFlagInfo const ILFileFlags[] = {
	{"readonly", IL_META_FILE_WRITEABLE, 0xFFFFFFFF},
	{"nometadata", IL_META_FILE_CONTAINS_NO_META_DATA, 0},
	{0, 0, 0xFFFFFFFC},
};

/*
 * Vtable fixup flag table.
 */
ILFlagInfo const ILVtableFixupFlags[] = {
	{"int32", IL_META_VTFIXUP_32BIT, 0},
	{"int64", IL_META_VTFIXUP_64BIT, 0},
	{"fromunmanaged", IL_META_VTFIXUP_FROM_UNMANAGED, 0},
	{"callmostderived", IL_META_VTFIXUP_CALL_MOST_DERIVED, 0},
	{0, 0, 0xFFFFFFE8},
};

void ILDumpFlags(FILE *stream, unsigned long flags, const ILFlagInfo *table,
				 int suppressed)
{
	const char *name;
	while(table->name)
	{
		name = 0;
		if(table->mask == (unsigned long)0xFFFFFFFF)
		{
			if((flags & table->flag) == 0)
			{
				name = table->name;
			}
		}
		else if(table->mask)
		{
			if((flags & table->mask) == table->flag)
			{
				name = table->name;
			}
		}
		else
		{
			if((flags & table->flag) != 0)
			{
				name = table->name;
			}
		}
		if(name)
		{
			if(*name == '/' && name[1] == '/')
			{
				/* Check for unknown variant type values */
				if((flags & IL_META_VARIANTTYPE_BASE_TYPE_MASK)
						> IL_META_VARIANTTYPE_MAX)
				{
					fprintf(stream, "/* unknown_variant_type: %02lX */ ",
							(flags & IL_META_VARIANTTYPE_BASE_TYPE_MASK));
				}
			}
			else if(*name == '/')
			{
				if(suppressed)
				{
					fputs(name + 1, stream);
					putc(' ', stream);
				}
			}
			else if(*name == '*' && name[1] != '\0')
			{
				fputs("/* ", stream);
				fputs(name + 1, stream);
				fputs(" */ ", stream);
			}
			else
			{
				fputs(name, stream);
				putc(' ', stream);
			}
		}
		++table;
	}
	if((flags & (table->mask & ~(table->flag))) != 0)
	{
		fprintf(stream, "/* unknown_bits: 0x%04lX */ ", (flags & table->mask));
	}
}

#ifdef	__cplusplus
};
#endif
