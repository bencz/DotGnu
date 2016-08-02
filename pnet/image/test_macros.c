/*
 * test_macros.c - Test macros in "il_program.h".
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

/*

Note: the purpose of this file is to test that the macros in
"il_program.h" are correct and don't contain invalid identifiers.
It will catch incorrect changes to the macros sooner rather than
later.  It is not actually part of the main code.

*/

#include "il_values.h"
#include "il_program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/* Calling this function for each macro call stops the compiler
   complaining about statements with no effect */
void IgnoreResult(int value)
{
}

#define	IGNORE(x)	IgnoreResult((int)(ILNativeUInt)(x))

/* Passed a class to check that the cast is working */
void TestProgramItemMacros(ILClass *info)
{
	IGNORE(ILProgramItem_Image(info));
	IGNORE(ILProgramItem_Token(info));
	IGNORE(ILProgramItem_HasAttrs(info));
}

void TestAttributeMacros(ILAttribute *attr)
{
}

void TestModuleMacros(ILModule *module)
{
	IGNORE(ILModule_Token(module));
	IGNORE(ILModule_Generation(module));
	IGNORE(ILModule_Name(module));
	IGNORE(ILModule_MVID(module));
	IGNORE(ILModule_EncId(module));
	IGNORE(ILModule_EncBaseId(module));
}

void TestAssemblyMacros(ILAssembly *assem)
{
	IGNORE(ILAssembly_Token(assem));
	IGNORE(ILAssembly_Name(assem));
	IGNORE(ILAssembly_HashAlg(assem));
	IGNORE(ILAssembly_Locale(assem));
	IGNORE(ILAssembly_Attrs(assem));
	IGNORE(ILAssembly_HasPublicKey(assem));
	IGNORE(ILAssembly_IsSideBySideCompatible(assem));
	IGNORE(ILAssembly_IsNotAppDomainCompatible(assem));
	IGNORE(ILAssembly_IsNotProcessCompatible(assem));
	IGNORE(ILAssembly_IsNotMachineCompatible(assem));
	IGNORE(ILAssembly_EnableJITTracking(assem));
	IGNORE(ILAssembly_DisableJITOptimizer(assem));
	IGNORE(ILAssembly_HasFullOriginator(assem));
}

void TestClassMacros(ILClass *info)
{
	IGNORE(ILClass_Token(info));
	IGNORE(ILClass_Attrs(info));
	IGNORE(ILClass_Name(info));
	IGNORE(ILClass_Namespace(info));
	IGNORE(ILClass_SynType(info));
	IGNORE(ILClass_Parent(info));
	IGNORE(ILClass_ParentRef(info));
	IGNORE(ILClass_NestedParent(info));
	IGNORE(ILClass_IsPrivate(info));
	IGNORE(ILClass_IsPublic(info));
	IGNORE(ILClass_IsNestedPublic(info));
	IGNORE(ILClass_IsNestedPrivate(info));
	IGNORE(ILClass_IsNestedFamily(info));
	IGNORE(ILClass_IsNestedAssembly(info));
	IGNORE(ILClass_IsNestedFamAndAssem(info));
	IGNORE(ILClass_IsNestedFamOrAssem(info));
	IGNORE(ILClass_IsAutoLayout(info));
	IGNORE(ILClass_IsSequentialLayout(info));
	IGNORE(ILClass_IsExplicitLayout(info));
	IGNORE(ILClass_IsRegularClass(info));
	IGNORE(ILClass_IsInterface(info));
	IGNORE(ILClass_IsValueType(info));
	IGNORE(ILClass_IsUnmanagedValueType(info));
	IGNORE(ILClass_IsAbstract(info));
	IGNORE(ILClass_IsSealed(info));
	IGNORE(ILClass_IsEnum(info));
	IGNORE(ILClass_HasSpecialName(info));
	IGNORE(ILClass_IsImport(info));
	IGNORE(ILClass_IsSerializable(info));
	IGNORE(ILClass_IsAnsi(info));
	IGNORE(ILClass_IsUnicode(info));
	IGNORE(ILClass_IsAutoChar(info));
	IGNORE(ILClass_IsLateInit(info));
	IGNORE(ILClass_HasRTSpecialName(info));
}

/* Passed a method to check that the cast is working */
void TestMemberMacros(ILMethod *member)
{
	IGNORE(ILMember_Token(member));
	IGNORE(ILMember_Owner(member));
	IGNORE(ILMember_Name(member));
	IGNORE(ILMember_Signature(member));
	IGNORE(ILMember_Attrs(member));
	IGNORE(ILMember_IsMethod(member));
	IGNORE(ILMember_IsField(member));
	IGNORE(ILMember_IsEvent(member));
	IGNORE(ILMember_IsProperty(member));
	IGNORE(ILMember_IsOverride(member));
	IGNORE(ILMember_IsPInvoke(member));
}

void TestMethodMacros(ILMethod *method)
{
	IGNORE(ILMethod_Token(method));
	IGNORE(ILMethod_Owner(method));
	IGNORE(ILMethod_Name(method));
	IGNORE(ILMethod_Signature(method));
	IGNORE(ILMethod_Attrs(method));
	IGNORE(ILMethod_ImplAttrs(method));
	IGNORE(ILMethod_CallConv(method));
	IGNORE(ILMethod_RVA(method));
	IGNORE(ILMethod_UserData(method));
	IGNORE(ILMethod_IsConstructor(method));
	IGNORE(ILMethod_IsStaticConstructor(method));
	IGNORE(ILMethod_IsCompilerControlled(method));
	IGNORE(ILMethod_IsPrivate(method));
	IGNORE(ILMethod_IsFamAndAssem(method));
	IGNORE(ILMethod_IsFamily(method));
	IGNORE(ILMethod_IsFamOrAssem(method));
	IGNORE(ILMethod_IsPublic(method));
	IGNORE(ILMethod_IsStatic(method));
	IGNORE(ILMethod_IsFinal(method));
	IGNORE(ILMethod_IsVirtual(method));
	IGNORE(ILMethod_IsHideBySig(method));
	IGNORE(ILMethod_IsReuseSlot(method));
	IGNORE(ILMethod_IsNewSlot(method));
	IGNORE(ILMethod_IsAbstract(method));
	IGNORE(ILMethod_HasSpecialName(method));
	IGNORE(ILMethod_HasRTSpecialName(method));
	IGNORE(ILMethod_HasPInvokeImpl(method));
	IGNORE(ILMethod_IsUnmanagedExport(method));
	IGNORE(ILMethod_IsIL(method));
	IGNORE(ILMethod_IsNative(method));
	IGNORE(ILMethod_IsOptIL(method));
	IGNORE(ILMethod_IsRuntime(method));
	IGNORE(ILMethod_IsManaged(method));
	IGNORE(ILMethod_IsUnmanaged(method));
	IGNORE(ILMethod_HasNoInlining(method));
	IGNORE(ILMethod_IsForwardRef(method));
	IGNORE(ILMethod_IsSynchronized(method));
	IGNORE(ILMethod_IsPreserveSig(method));
	IGNORE(ILMethod_IsInternalCall(method));
	IGNORE(ILMethod_IsJavaFPStrict(method));
	IGNORE(ILMethod_IsJava(method));
}

void TestParameterMacros(ILParameter *param)
{
	IGNORE(ILParameter_Token(param));
	IGNORE(ILParameter_Name(param));
	IGNORE(ILParameter_Num(param));
	IGNORE(ILParameter_Attrs(param));
	IGNORE(ILParameter_IsIn(param));
	IGNORE(ILParameter_IsOut(param));
	IGNORE(ILParameter_IsRetVal(param));
	IGNORE(ILParameter_IsOptional(param));
	IGNORE(ILParameter_HasDefault(param));
	IGNORE(ILParameter_HasFieldMarshal(param));
}

void TestFieldMacros(ILField *field)
{
	IGNORE(ILField_Token(field));
	IGNORE(ILField_Owner(field));
	IGNORE(ILField_Name(field));
	IGNORE(ILField_Type(field));
	IGNORE(ILField_Attrs(field));
	IGNORE(ILField_IsCompilerControlled(field));
	IGNORE(ILField_IsPrivate(field));
	IGNORE(ILField_IsFamAndAssem(field));
	IGNORE(ILField_IsAssembly(field));
	IGNORE(ILField_IsFamily(field));
	IGNORE(ILField_IsFamOrAssem(field));
	IGNORE(ILField_IsPublic(field));
	IGNORE(ILField_IsStatic(field));
	IGNORE(ILField_IsInitOnly(field));
	IGNORE(ILField_IsLiteral(field));
	IGNORE(ILField_IsNotSerialized(field));
	IGNORE(ILField_HasSpecialName(field));
	IGNORE(ILField_HasPInvokeImpl(field));
	IGNORE(ILField_HasRTSpecialName(field));
}

void TestEventMacros(ILEvent *event)
{
	IGNORE(ILEvent_Token(event));
	IGNORE(ILEvent_Owner(event));
	IGNORE(ILEvent_Name(event));
	IGNORE(ILEvent_Type(event));
	IGNORE(ILEvent_Attrs(event));
	IGNORE(ILEvent_AddOn(event));
	IGNORE(ILEvent_RemoveOn(event));
	IGNORE(ILEvent_Fire(event));
	IGNORE(ILEvent_Other(event));
	IGNORE(ILEvent_HasSpecialName(event));
	IGNORE(ILEvent_HasRTSpecialName(event));
}

void TestPropertyMacros(ILProperty *property)
{
	IGNORE(ILProperty_Token(property));
	IGNORE(ILProperty_Owner(property));
	IGNORE(ILProperty_Name(property));
	IGNORE(ILProperty_Signature(property));
	IGNORE(ILProperty_Attrs(property));
	IGNORE(ILProperty_Getter(property));
	IGNORE(ILProperty_Setter(property));
	IGNORE(ILProperty_Other(property));
	IGNORE(ILProperty_HasSpecialName(property));
	IGNORE(ILProperty_HasRTSpecialName(property));
}

void TestPInvokeMacros(ILPInvoke *pinvoke)
{
	IGNORE(ILPInvoke_Token(pinvoke));
	IGNORE(ILPInvoke_Owner(pinvoke));
	IGNORE(ILPInvoke_Method(pinvoke));
	IGNORE(ILPInvoke_Module(pinvoke));
	IGNORE(ILPInvoke_Alias(pinvoke));
	IGNORE(ILPInvoke_Attrs(pinvoke));
	IGNORE(ILPInvoke_NoMangle(pinvoke));
	IGNORE(ILPInvoke_CharSetNotSpec(pinvoke));
	IGNORE(ILPInvoke_CharSetAnsi(pinvoke));
	IGNORE(ILPInvoke_CharSetUnicode(pinvoke));
	IGNORE(ILPInvoke_CharSetAuto(pinvoke));
	IGNORE(ILPInvoke_OLE(pinvoke));
	IGNORE(ILPInvoke_SupportsLastError(pinvoke));
	IGNORE(ILPInvoke_CallConvWinApi(pinvoke));
	IGNORE(ILPInvoke_CallConvCDecl(pinvoke));
	IGNORE(ILPInvoke_CallConvStdCall(pinvoke));
	IGNORE(ILPInvoke_CallConvThisCall(pinvoke));
	IGNORE(ILPInvoke_CallConvFastCall(pinvoke));
}

void TestOverrideMacros(ILOverride *over)
{
	IGNORE(ILOverride_Token(over));
	IGNORE(ILOverride_Decl(over));
	IGNORE(ILOverride_Body(over));
}

void TestEventMapMacros(ILEventMap *map)
{
	IGNORE(ILEventMap_Token(map));
	IGNORE(ILEventMap_Class(map));
	IGNORE(ILEventMap_Event(map));
}

void TestPropertyMapMacros(ILPropertyMap *map)
{
	IGNORE(ILPropertyMap_Token(map));
	IGNORE(ILPropertyMap_Class(map));
	IGNORE(ILPropertyMap_Property(map));
}

void TestMethodSemMacros(ILMethodSem *sem)
{
	IGNORE(ILMethodSem_Token(sem));
	IGNORE(ILMethodSem_Event(sem));
	IGNORE(ILMethodSem_Property(sem));
	IGNORE(ILMethodSem_Type(sem));
	IGNORE(ILMethodSem_Method(sem));
}

void TestOSInfoMacros(ILOSInfo *osinfo)
{
	IGNORE(ILOSInfo_Token(osinfo));
	IGNORE(ILOSInfo_Identifier(osinfo));
	IGNORE(ILOSInfo_Major(osinfo));
	IGNORE(ILOSInfo_Minor(osinfo));
	IGNORE(ILOSInfo_Assembly(osinfo));
}

void TestProcessorInfoMacros(ILProcessorInfo *procinfo)
{
	IGNORE(ILProcessorInfo_Token(procinfo));
	IGNORE(ILProcessorInfo_Number(procinfo));
	IGNORE(ILProcessorInfo_Assembly(procinfo));
}

void TestTypeSpecMacros(ILTypeSpec *spec)
{
	IGNORE(ILTypeSpec_Token(spec));
	IGNORE(ILTypeSpec_Type(spec));
	IGNORE(ILTypeSpec_Class(spec));
}

void TestStandAloneSigMacros(ILStandAloneSig *sig)
{
	IGNORE(ILStandAloneSig_Token(sig));
	IGNORE(ILStandAloneSig_Type(sig));
	IGNORE(ILStandAloneSig_IsLocals(sig));
}

void TestConstantMacros(ILConstant *constant)
{
	IGNORE(ILConstant_Token(constant));
	IGNORE(ILConstant_Owner(constant));
	IGNORE(ILConstant_ElemType(constant));
}

void TestFieldRVAMacros(ILFieldRVA *rva)
{
	IGNORE(ILFieldRVA_Token(rva));
	IGNORE(ILFieldRVA_Owner(rva));
	IGNORE(ILFieldRVA_RVA(rva));
}

void TestFieldLayoutMacros(ILFieldLayout *layout)
{
	IGNORE(ILFieldLayout_Token(layout));
	IGNORE(ILFieldLayout_Owner(layout));
	IGNORE(ILFieldLayout_Offset(layout));
}

void TestFieldMarshalMacros(ILFieldMarshal *marshal)
{
	IGNORE(ILFieldMarshal_Token(marshal));
	IGNORE(ILFieldMarshal_Owner(marshal));
}

void TestClassLayoutMacros(ILClassLayout *layout)
{
	IGNORE(ILClassLayout_Token(layout));
	IGNORE(ILClassLayout_Owner(layout));
	IGNORE(ILClassLayout_PackingSize(layout));
	IGNORE(ILClassLayout_ClassSize(layout));
}

void TestDeclSecurityMacros(ILDeclSecurity *security)
{
	IGNORE(ILDeclSecurity_Token(security));
	IGNORE(ILDeclSecurity_Owner(security));
	IGNORE(ILDeclSecurity_Type(security));
}

void TestFileDeclMacros(ILFileDecl *decl)
{
	IGNORE(ILFileDecl_Token(decl));
	IGNORE(ILFileDecl_Name(decl));
	IGNORE(ILFileDecl_Attrs(decl));
	IGNORE(ILFileDecl_HasMetaData(decl));
	IGNORE(ILFileDecl_IsWriteable(decl));
}

void TestManifestResMacros(ILManifestRes *res)
{
	IGNORE(ILManifestRes_Token(res));
	IGNORE(ILManifestRes_Name(res));
	IGNORE(ILManifestRes_OwnerFile(res));
	IGNORE(ILManifestRes_OwnerAssembly(res));
	IGNORE(ILManifestRes_Offset(res));
	IGNORE(ILManifestRes_Attrs(res));
	IGNORE(ILManifestRes_IsPublic(res));
	IGNORE(ILManifestRes_IsPrivate(res));
}

void TestExportedTypeMacros(ILExportedType *type)
{
	IGNORE(ILExportedType_Token(type));
	IGNORE(ILExportedType_Attrs(type));
	IGNORE(ILExportedType_Id(type));
	IGNORE(ILExportedType_Name(type));
	IGNORE(ILExportedType_Namespace(type));
	IGNORE(ILExportedType_Scope(type));
}

void TestFromTokenMacros(ILImage *image, ILToken token)
{
	IGNORE(ILProgramItem_FromToken(image, token));
	IGNORE(ILModule_FromToken(image, token));
	IGNORE(ILAssembly_FromToken(image, token));
	IGNORE(ILClass_FromToken(image, token));
	IGNORE(ILMember_FromToken(image, token));
	IGNORE(ILMethod_FromToken(image, token));
	IGNORE(ILParameter_FromToken(image, token));
	IGNORE(ILField_FromToken(image, token));
	IGNORE(ILEvent_FromToken(image, token));
	IGNORE(ILProperty_FromToken(image, token));
	IGNORE(ILPInvoke_FromToken(image, token));
	IGNORE(ILOverride_FromToken(image, token));
	IGNORE(ILEventMap_FromToken(image, token));
	IGNORE(ILPropertyMap_FromToken(image, token));
	IGNORE(ILMethodSem_FromToken(image, token));
	IGNORE(ILOSInfo_FromToken(image, token));
	IGNORE(ILProcessorInfo_FromToken(image, token));
	IGNORE(ILTypeSpec_FromToken(image, token));
	IGNORE(ILStandAloneSig_FromToken(image, token));
	IGNORE(ILConstant_FromToken(image, token));
	IGNORE(ILFieldRVA_FromToken(image, token));
	IGNORE(ILFieldLayout_FromToken(image, token));
	IGNORE(ILFieldMarshal_FromToken(image, token));
	IGNORE(ILClassLayout_FromToken(image, token));
	IGNORE(ILDeclSecurity_FromToken(image, token));
	IGNORE(ILFileDecl_FromToken(image, token));
	IGNORE(ILManifestRes_FromToken(image, token));
	IGNORE(ILExportedType_FromToken(image, token));
}

void TestGenericParMacros(ILGenericPar *genPar)
{
	IGNORE(ILGenericPar_Token(genPar));
	IGNORE(ILGenericPar_Number(genPar));
	IGNORE(ILGenericPar_Flags(genPar));
	IGNORE(ILGenericPar_Owner(genPar));
	IGNORE(ILGenericPar_Name(genPar));
	IGNORE(ILGenericPar_Kind(genPar));
	IGNORE(ILGenericPar_Constraint(genPar));
}

void TestMethodSpecMacros(ILMethodSpec *spec)
{
	IGNORE(ILMethodSpec_Token(spec));
	IGNORE(ILMethodSpec_Method(spec));
	IGNORE(ILMethodSpec_Type(spec));
}

#ifdef	__cplusplus
};
#endif
