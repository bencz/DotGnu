/*
 * corhdr.h - Metadata structure definitions.
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

#ifndef	__CORHDR_H__
#define	__CORHDR_H__

/*
 * Note: portable programs should not use the struct's defined in this
 * header to process raw IL binaries.  They are not guaranteed to be
 * portable to 64-bit or big-endian platforms.
 */

/*
 * Include the Win32-like types.
 */
#include "corwin32.h"

/*
 * Basic COR types.
 */
typedef LPVOID					mdScope;
typedef ULONG32					mdToken;
typedef ULONG					RID;
typedef char				   *MDUTF8STR;
typedef const char			   *MDUTF8CSTR;
typedef unsigned char			COR_SIGNATURE;
typedef COR_SIGNATURE		   *PCOR_SIGNATURE;
typedef const COR_SIGNATURE	   *PCCOR_SIGNATURE;
typedef void				   *HCORENUM;
typedef void				   *PSECURITY_PROPS;
typedef void				   *PSECURITY_VALUE;
typedef void				  **PPSECURITY_PROPS;
typedef void				  **PPSECURITY_VALUE;

/*
 * Constants that are useful in COR headers.
 */
typedef enum ReplacesCorHdrNumericDefines
{
	COR_VERSION_MAJOR				= 2,
	COR_VERSION_MAJOR_2				= 2,
	COR_VERSION_MINOR				= 0,

	COMIMAGE_FLAGS_ILONLY			= (1<<0),
	COMIMAGE_FLAGS_32BITREQUIRED	= (1<<1),
	COMIMAGE_FLAGS_IL_LIBRARY		= (1<<2),
#define COMIMAGE_FLAGS_STRONGNAMESIGNED	(1<<3)
	COMIMAGE_FLAGS_TRACKDEBUGDATA	= (1<<16),

	IMAGE_COR_MIH_METHODRVA			= (1<<0),
	IMAGE_COR_MIH_EHRVA				= (1<<1),
	IMAGE_COR_MIH_BASIC_BLOCK		= (1<<3),
	IMAGE_COR_EATJ_THUNK_SIZE		= 32,

	COR_VTABLE_32BIT				= (1<<0),
	COR_VTABLE_64BIT				= (1<<1),
	COR_VTABLE_FROM_UNMANAGED		= (1<<2),
	COR_VTABLE_CALL_MOST_DERIVED	= (1<<4),

	COR_DELETED_NAME_LENGTH			= 8,
	COR_VTABLEGAP_NAME_LENGTH		= 8,
	COR_ILMETHOD_SECT_SMALL_MAX_DATASIZE = 255,

	NATIVE_TYPE_MAX_CB				= 1,
	MAX_CLASS_NAME					= 1024,
	MAX_PACKAGE_NAME				= 1024

} ReplacesCorHdrNumericDefines;

/*
 * Misc constants.
 */
typedef enum ReplacesGeneralNumericDefines
{
	IMAGE_DIRECTORY_ENTRY_COMHEADER = 14,
	_NEW_FLAGS_IMPLEMENTED = 1,
	__NEW_FLAGS_IMPLEMENTED = 1

} ReplacesGeneralNumericDefines;

/*
 * Structure of the CLR header.
 */
#define	__IMAGE_COR20_HEADER_DEFINED__
typedef struct IMAGE_COR20_HEADER
{
	ULONG					cb;
	USHORT					MajorRuntimeVersion;
	USHORT					MinorRuntimeVersion;
	IMAGE_DATA_DIRECTORY	MetaData;
	ULONG					Flags;
	ULONG					EntryPointToken;
	IMAGE_DATA_DIRECTORY	Resources;
	IMAGE_DATA_DIRECTORY	StrongNameSignature;
	IMAGE_DATA_DIRECTORY	CodeManagerTable;
	IMAGE_DATA_DIRECTORY	VTableFixups;
	IMAGE_DATA_DIRECTORY	ExportAddressTableJumps;
	IMAGE_DATA_DIRECTORY	ManagedNativeHeader;

} IMAGE_COR32_HEADER;

/*
 * Aliases for "mdToken".
 */
typedef	mdToken		mdModule;
typedef	mdToken		mdTypeRef;
typedef	mdToken		mdTypeDef;
typedef	mdToken		mdFieldDef;
typedef	mdToken		mdMethodDef;
typedef	mdToken		mdParamDef;
typedef	mdToken		mdInterfaceImpl;
typedef	mdToken		mdMemberRef;
typedef mdToken		mdCustomAttribute;
typedef	mdToken		mdPermission;
typedef	mdToken		mdSignature;
typedef mdToken		mdEvent;
typedef mdToken		mdProperty;
typedef mdToken		mdModuleRef;
typedef mdToken		mdAssembly;
typedef mdToken		mdAssemblyRef;
typedef mdToken		mdFile;
typedef mdToken		mdExportedType;
typedef mdToken		mdManifestResource;
typedef mdToken		mdTypeSpec;
typedef mdToken		mdString;
typedef mdToken		mdCPToken;

/*
 * Token types.
 */
typedef enum CorTokenType
{
	mdtModule				= 0x00000000,
	mdtTypeRef				= 0x01000000,
	mdtTypeDef				= 0x02000000,
	mdtFieldDef				= 0x04000000,
	mdtMethodDef			= 0x06000000,
	mdtParamDef				= 0x08000000,
	mdtInterfaceImpl		= 0x09000000,
	mdtMemberRef			= 0x0A000000,
	mdtCustomAttribute		= 0x0C000000,
	mdtPermission			= 0x0E000000,
	mdtSignature			= 0x11000000,
	mdtEvent				= 0x14000000,
	mdtProperty				= 0x17000000,
	mdtModuleRef			= 0x1A000000,
	mdtTypeSpec				= 0x1B000000,
	mdtAssembly				= 0x20000000,
	mdtAssemblyRef			= 0x23000000,
	mdtFile					= 0x26000000,
	mdtExportedType			= 0x27000000,
	mdtManifestResource		= 0x28000000,
	mdtString				= 0x70000000,
	mdtName					= 0x71000000,
	mdtBaseType				= 0x72000000,

} CorTokenType;

/*
 * Create or pull apart tokens.
 */
#define	TokenFromRid(rid,type)	((CorTokenType)((rid) | (type)))
#define	RidFromToken(token)		((RID)((token) & 0x00FFFFFF))
#define	TypeFromToken(token)	((ULONG32)((token) & 0xFF000000))
#define	RidToToken(rid,type)	((rid) |= (type))

/*
 * Nil tokens of each major category.
 */
#define	IsNilToken(token)		(((token) & 0x00FFFFFF) == 0)
#define	mdTokenNil				((mdToken)0)
#define	mdModuleNil				mdTokenNil
#define	mdTypeRefNil			mdTokenNil
#define	mdTypeDefNil			mdTokenNil
#define	mdFieldDefNil			mdTokenNil
#define	mdMethodDefNil			mdTokenNil
#define	mdParamDefNil			mdTokenNil
#define	mdInterfaceImplNil		mdTokenNil
#define	mdMemberRefNil			mdTokenNil
#define	mdCustomAttributeNil	mdTokenNil
#define	mdPermissionNil			mdTokenNil
#define	mdSignatureNil			mdTokenNil
#define	mdEventNil				mdTokenNil
#define	mdPropertyNil			mdTokenNil
#define	mdModuleRefNil			mdTokenNil
#define	mdAssemblyNil			mdTokenNil
#define	mdAssemblyRefNil		mdTokenNil
#define	mdFileNil				mdTokenNil
#define	mdExportedTypeNil		mdTokenNil
#define	mdManifestResourceNil	mdTokenNil
#define	mdTypeSpecNil			mdTokenNil
#define	mdStringNil				mdTokenNil

/*
 * Type attributes.
 */
typedef enum CorTypeAttr
{
	tdNotPublic				= 0x00000000,
	tdPublic				= 0x00000001,
	tdNestedPublic			= 0x00000002,
	tdNestedPrivate			= 0x00000003,
	tdNestedFamily			= 0x00000004,
	tdNestedAssembly		= 0x00000005,
	tdNestedFamANDAssem		= 0x00000006,
	tdNestedFamORAssem		= 0x00000007,
	tdVisibilityMask		= 0x00000007,
	tdAutoLayout			= 0x00000000,
	tdSequentialLayout		= 0x00000008,
	tdExplicitLayout		= 0x00000010,
	tdLayoutMask			= 0x00000018,
	tdClass					= 0x00000000,
	tdInterface				= 0x00000020,
	tdClassSemanticsMask	= 0x00000020,
	tdAbstract				= 0x00000080,
	tdSealed				= 0x00000100,
	tdSpecialName			= 0x00000400,
	tdRTSpecialName			= 0x00000800,
	tdImport				= 0x00001000,
	tdSerializable			= 0x00002000,
	tdAnsiClass				= 0x00000000,
	tdUnicodeClass			= 0x00010000,
	tdAutoClass				= 0x00020000,
	tdStringFormatMask		= 0x00030000,
	tdHasSecurity           = 0x00040000,
	tdReservedMask			= 0x00040800,
	tdBeforeFieldInit		= 0x00100000,

} CorTypeAttr;
typedef CorTypeAttr CorRegTypeAttr;
#define	IsTdNotPublic(x)	\
			(((x) & tdVisibilityMask) == tdNotPublic)
#define	IsTdPublic(x)	\
			(((x) & tdVisibilityMask) == tdPublic)
#define	IsTdNestedPublic(x)	\
			(((x) & tdVisibilityMask) == tdNestedPublic)
#define	IsTdNestedPrivate(x)	\
			(((x) & tdVisibilityMask) == tdNestedPrivate)
#define	IsTdNestedFamily(x)	\
			(((x) & tdVisibilityMask) == tdNestedFamily)
#define	IsTdNestedAssembly(x)	\
			(((x) & tdVisibilityMask) == tdNestedAssembly)
#define	IsTdNestedFamANDAssem(x)	\
			(((x) & tdVisibilityMask) == tdNestedFamANDAssem)
#define	IsTdNestedFamORAssem(x)	\
			(((x) & tdVisibilityMask) == tdNestedFamORAssem)
#define	IsTdNested(x)	\
			(((x) & tdVisibilityMask) >= tdNestedPublic)
#define	IsTdAutoLayout(x)	\
			(((x) & tdLayoutMask) == tdAutoLayout)
#define	IsTdSequentialLayout(x)	\
			(((x) & tdLayoutMask) == tdSequentialLayout)
#define	IsTdExplicitLayout(x)	\
			(((x) & tdLayoutMask) == tdExplicitLayout)
#define	IsTdClass(x)	\
			(((x) & tdClassSemanticsMask) == tdClass)
#define	IsTdInterface(x)	\
			(((x) & tdClassSemanticsMask) == tdInterface)
#define	IsTdAbstract(x)			((x) & tdAbstract)
#define	IsTdSealed(x)			((x) & tdSealed)
#define	IsTdSpecialName(x)		((x) & tdSpecialName)
#define	IsTdRTSpecialName(x)	((x) & tdRTSpecialName)
#define	IsTdImport(x)			((x) & tdImport)
#define	IsTdSerializable(x)		((x) & tdSerializable)
#define	IsTdAnsiClass(x)	\
			(((x) & tdStringFormatMask) == tdAnsiClass)
#define	IsTdUnicodeClass(x)	\
			(((x) & tdStringFormatMask) == tdUnicodeClass)
#define	IsTdAutoClass(x)	\
			(((x) & tdStringFormatMask) == tdAutoClass)
#define	IsTdHasSecurity(x)		((x) & tdHasSecurity)
#define	IsTdBeforeFieldInit(x)	((x) & tdBeforeFieldInit)

/*
 * Field attributes.
 */
typedef enum CorFieldAttr
{
	fdPrivateScope			= 0x0000,
	fdPrivate				= 0x0001,
	fdFamANDAssem			= 0x0002,
	fdAssembly				= 0x0003,
	fdFamily				= 0x0004,
	fdFamORAssem			= 0x0005,
	fdPublic				= 0x0006,
	fdFieldAccessMask		= 0x0007,
	fdStatic				= 0x0010,
	fdInitOnly				= 0x0020,
	fdLiteral				= 0x0040,
	fdNotSerialized			= 0x0080,
	fdHasFieldRVA			= 0x0100,
	fdSpecialName			= 0x0200,
	fdRTSpecialName			= 0x0400,
	fdHasFieldMarshal		= 0x1000,
	fdPinvokeImpl			= 0x2000,
	fdHasDefault			= 0x8000,
	fdReservedMask			= 0x9500,

} CorFieldAttr;
#define	IsFdPrivateScope(x)	\
			(((x) & fdFieldAccessMask) == fdPrivateScope)
#define	IsFdPrivate(x)	\
			(((x) & fdFieldAccessMask) == fdPrivate)
#define	IsFdFamANDAssem(x)	\
			(((x) & fdFieldAccessMask) == fdFamANDAssem)
#define	IsFdAssembly(x)	\
			(((x) & fdFieldAccessMask) == fdAssembly)
#define	IsFdFamily(x)	\
			(((x) & fdFieldAccessMask) == fdFamily)
#define	IsFdFamORAssem(x)	\
			(((x) & fdFieldAccessMask) == fdFamORAssem)
#define	IsFdPublic(x)	\
			(((x) & fdFieldAccessMask) == fdPublic)
#define	IsFdStatic(x)			((x) & fdStatic)
#define	IsFdInitOnly(x)			((x) & fdInitOnly)
#define	IsFdLiteral(x)			((x) & fdLiteral)
#define	IsFdNotSerialized(x)	((x) & fdNotSerialized)
#define	IsFdHasFieldRVA(x)		((x) & fdHasFieldRVA)
#define	IsFdSpecialName(x)		((x) & fdSpecialName)
#define	IsFdRTSpecialName(x)	((x) & fdRTSpecialName)
#define	IsFdHasFieldMarshal(x)	((x) & fdHasFieldMarshal)
#define	IsFdPinvokeImpl(x)		((x) & fdPinvokeImpl)
#define	IsFdHasDefault(x)		((x) & fdHasDefault)

/*
 * Method attributes.
 */
typedef enum CorMethodAttr
{
	mdPrivateScope			= 0x0000,
	mdPrivate				= 0x0001,
	mdFamANDAssem			= 0x0002,
	mdAssem					= 0x0003,
	mdFamily				= 0x0004,
	mdFamORAssem			= 0x0005,
	mdPublic				= 0x0006,
	mdMemberAccessMask		= 0x0007,
	mdUnmanagedExport		= 0x0008,
	mdStatic				= 0x0010,
	mdFinal					= 0x0020,
	mdVirtual				= 0x0040,
	mdHideBySig				= 0x0080,
	mdReuseSlot				= 0x0000,
	mdNewSlot				= 0x0100,
	mdVtableLayoutMask		= 0x0100,
	mdAbstract				= 0x0400,
	mdSpecialName			= 0x0800,
	mdRTSpecialName			= 0x1000,
	mdPinvokeImpl			= 0x2000,
	mdHasSecurity			= 0x4000,
	mdRequireSecObject		= 0x8000,
	mdReservedMask			= 0xD000,

} CorMethodAttr;
#define	COR_CTOR_METHOD_NAME		".ctor"
#define	COR_CTOR_METHOD_NAME_W		L".ctor"
#define	COR_CCTOR_METHOD_NAME		".cctor"
#define	COR_CCTOR_METHOD_NAME_W		L".cctor"
#define	IsMdPrivateScope(x)	\
			(((x) & mdMemberAccessMask) == mdPrivateScope)
#define	IsMdPrivate(x)	\
			(((x) & mdMemberAccessMask) == mdPrivate)
#define	IsMdFamANDAssem(x)	\
			(((x) & mdMemberAccessMask) == mdFamANDAssem)
#define	IsMdAssem(x)	\
			(((x) & mdMemberAccessMask) == mdAssem)
#define	IsMdFamily(x)	\
			(((x) & mdMemberAccessMask) == mdFamily)
#define	IsMdFamORAssem(x)	\
			(((x) & mdMemberAccessMask) == mdFamORAssem)
#define	IsMdPublic(x)	\
			(((x) & mdMemberAccessMask) == mdPublic)
#define	IsMdStatic(x)			((x) & mdStatic)
#define	IsMdUnmanagedExport(x)	((x) & mdUnmanagedExport)
#define	IsMdFinal(x)			((x) & mdFinal)
#define	IsMdVirtual(x)			((x) & mdVirtual)
#define	IsMdHideBySig(x)		((x) & mdHideBySig)
#define	IsMdReuseSlot(x)	\
			(((x) & mdVtableLayoutMask) == mdReuseSlot)
#define	IsMdNewSlot(x)	\
			(((x) & mdVtableLayoutMask) == mdNewSlot)
#define	IsMdAbstract(x)			((x) & mdAbstract)
#define	IsMdSpecialName(x)		((x) & mdSpecialName)
#define	IsMdRTSpecialName(x)	((x) & mdRTSpecialName)
#define	IsMdPinvokeImpl(x)		((x) & mdPinvokeImpl)
#define	IsMdHasSecurity(x)		((x) & mdHasSecurity)
#define	IsMdRequireSecObject(x)	((x) & mdRequireSecObject)
#define	IsMdInstanceInitializer(x,str)	\
			(IsMdRTSpecialName((x)) && !strcmp((str), COR_CTOR_METHOD_NAME))
#define	IsMdInstanceInitializerW(x,str)	\
			(IsMdRTSpecialName((x)) && !wcscmp((str), COR_CTOR_METHOD_NAME_W))
#define	IsMdClassConstructor(x,str)	\
			(IsMdRTSpecialName((x)) && !strcmp((str), COR_CCTOR_METHOD_NAME))
#define	IsMdClassConstructorW(x,str)	\
			(IsMdRTSpecialName((x)) && !wcscmp((str), COR_CCTOR_METHOD_NAME_W))

/*
 * Parameter attributes.
 */
typedef enum CorParamAttr
{
	pdIn					= 0x0001,
	pdOut					= 0x0002,
	pdOptional				= 0x0010,
	pdHasDefault			= 0x1000,
	pdHasFieldMarshal		= 0x2000,
	pdUnused				= 0xCFE0,
	pdReservedMask			= 0xF000,

} CorParamAttr;
#define	IsPdIn(x)					((x) & pdIn)
#define	IsPdOut(x)					((x) & pdOut)
#define	IsPdOptional(x)				((x) & pdOptional)
#define	IsPdHasDefault(x)			((x) & pdHasDefault)
#define	IsPdHasFieldMarshal(x)		((x) & pdHasFieldMarshal)

/*
 * Property attributes.
 */
typedef enum CorPropertyAttr
{
	prSpecialName			= 0x0200,
	prRTSpecialName			= 0x0400,
	prHasDefault			= 0x1000,
	prUnused				= 0xE9FF,
	prReservedMask			= 0xF400,

} CorPropertyAttr;
#define	IsPrSpecialName(x)			((x) & prSpecialName)
#define	IsPrRTSpecialName(x)		((x) & prRTSpecialName)
#define	IsPrHasDefault(x)			((x) & prHasDefault)

/*
 * Event attributes.
 */
typedef enum CorEventAttr
{
	evSpecialName			= 0x0200,
	evRTSpecialName			= 0x0400,
	evReservedMask			= 0x0400,

} CorEventAttr;
#define	IsEvSpecialName(x)			((x) & evSpecialName)
#define	IsEvRTSpecialName(x)		((x) & evRTSpecialName)

/*
 * Method semantics attributes.
 */
typedef enum CorMethodSemanticsAttr
{
	msSetter				= 0x0001,
	msGetter				= 0x0002,
	msOther					= 0x0004,
	msAddOn					= 0x0008,
	msRemoveOn				= 0x0010,
	msFire					= 0x0020,

} CorMethodSemanticsAttr;
#define	IsMsSetter(x)				((x) & msSetter)
#define	IsMsGetter(x)				((x) & msGetter)
#define	IsMsOther(x)				((x) & msOther)
#define	IsMsAddOn(x)				((x) & msAddOn)
#define	IsMsRemoveOn(x)				((x) & msRemoveOn)
#define	IsMsFire(x)					((x) & msFire)

/*
 * Method implementation attributes.
 */
typedef enum CorMethodImpl
{
	miIL					= 0x0000,
	miNative				= 0x0001,
	miOPTIL					= 0x0002,
	miRuntime				= 0x0003,
	miCodeTypeMask			= 0x0003,
	miManaged				= 0x0000,
	miUnmanaged				= 0x0004,
	miManagedMask			= 0x0004,
	miNoInlining			= 0x0008,
	miForwardRef			= 0x0010,
	miSynchronized			= 0x0020,
	miPreserveSig			= 0x0080,
	miInternalCall			= 0x1000,
	miMaxMethodImplVal		= 0xFFFF,

} CorMethodImpl;
#define	IsMiIL(x)					(((x) & miCodeTypeMask) == miIL)
#define	IsMiNative(x)				(((x) & miCodeTypeMask) == miNative)
#define	IsMiOPTIL(x)				(((x) & miCodeTypeMask) == miOPTIL)
#define	IsMiRuntime(x)				(((x) & miCodeTypeMask) == miRuntime)
#define	IsMiManaged(x)				(((x) & miManagedMask) == miManaged)
#define	IsMiUnmanaged(x)			(((x) & miManagedMask) == miUnmanaged)
#define	IsMiNoInlining(x)			((x) & miNoInlining)
#define	IsMiForwardRef(x)			((x) & miForwardReg)
#define	IsMiSynchronized(x)			((x) & miSynchronized)
#define	IsMiPreserveSig(x)			((x) & miPreserveSig)
#define	IsMiInternalCall(x)			((x) & miInternalCall)
#define COR_IS_METHOD_MANAGED_IL(x)	\
			(((x) & 0x000F) == (miIL | miManaged))   
#define COR_IS_METHOD_MANAGED_OPTIL(x)	\
			(((x) & 0x000F) == (miOPTIL | miManaged))    
#define COR_IS_METHOD_MANAGED_NATIVE(x)	\
			(((x) & 0x000F) == (miNative | miManaged))   
#define COR_IS_METHOD_UNMANAGED_NATIVE(x)	\
			(((x) & 0x000F) == (miNative | miUnmanaged)) 

/*
 * Declarative security attributes.
 */
typedef enum CorDeclSecurity
{
	dclActionMask			= 0x000F,
	dclActionNil			= 0x0000,
	dclRequest				= 0x0001,
	dclDemand				= 0x0002,
	dclAssert				= 0x0003,
	dclDeny					= 0x0004,
	dclPermitOnly			= 0x0005,
	dclLinktimeCheck		= 0x0006,
	dclInheritanceCheck		= 0x0007,
	dclRequestMinimum		= 0x0008,
	dclRequestOptional		= 0x0009,
	dclRequestRefuse		= 0x000A,
	dclPrejitGrant			= 0x000B,
	dclPrejitDenied			= 0x000C,
	dclNonCasDemand			= 0x000D,
	dclNonCasLinkDemand		= 0x000E,
	dclNonCaseInheritance	= 0x000F,
	dclMaximumValue			= 0x000F,

} CorDeclSecurity;
#define	IsDclActionNil(x)			(((x) & dclActionMask) == dclActionNil)
#define	IsDclRequest(x)				(((x) & dclActionMask) == dclRequest)
#define	IsDclDemand(x)				(((x) & dclActionMask) == dclDemand)
#define	IsDclAssert(x)				(((x) & dclActionMask) == dclAssert)
#define	IsDclDeny(x)				(((x) & dclActionMask) == dclDeny)
#define	IsDclPermitOnly(x)			(((x) & dclActionMask) == dclPermitOnly)
#define	IsDclLinktimeCheck(x)		(((x) & dclActionMask) == dclLinktimeCheck)
#define	IsDclInheritanceCheck(x)	\
				(((x) & dclActionMask) == dclInheritanceCheck)
#define	IsDclMaximumValue(x)		(((x) & dclActionMask) == dclMaximumValue)

/*
 * PInvoke attributes.
 */
typedef enum CorPinvokeMap
{
	pmNoMangle				= 0x0001,
	pmCharSetMask			= 0x0006,
	pmCharSetNotSpec		= 0x0000,
	pmCharSetAnsi			= 0x0002,
	pmCharSetUnicode		= 0x0004,
	pmCharSetAuto			= 0x0006,
	pmSupportsLastError		= 0x0040,
	pmCallConvMask			= 0x0700,
	pmCallConvWinapi		= 0x0100,
	pmCallConvCdecl			= 0x0200,
	pmCallConvStdcall		= 0x0300,
	pmCallConvThiscall		= 0x0400,
	pmCallConvFastcall		= 0x0500,

} CorPinvokeMap;
#define	IsPmNoMangle(x)				((x) & pmNoMangle)
#define	IsPmCharSetNotSpec(x)		(((x) & pmCharSetMask) == pmCharSetNotSpec)
#define	IsPmCharSetAnsi(x)			(((x) & pmCharSetMask) == pmCharSetAnsi)
#define	IsPmCharSetUnicode(x)		(((x) & pmCharSetMask) == pmCharUnicode)
#define	IsPmCharSetAuto(x)			(((x) & pmCharSetMask) == pmCharAuto)
#define	IsPmSupportsLastError(x)	((x) & pmSupportsLastError)
#define	IsPmCallConvWinapi(x)		\
			(((x) & pmCallConvMask) == pmCallConvWinapi)
#define	IsPmCallConvCdecl(x)		\
			(((x) & pmCallConvMask) == pmCallConvCdecl)
#define	IsPmCallConvStdcall(x)		\
			(((x) & pmCallConvMask) == pmCallConvStdcall)
#define	IsPmCallConvThiscall(x)		\
			(((x) & pmCallConvMask) == pmCallConvThiscall)
#define	IsPmCallConvFastcall(x)		\
			(((x) & pmCallConvMask) == pmCallConvFastcall)

/*
 * Assembly attributes.
 */
typedef enum CorAssemblyFlags
{
	afPublicKey						= 0x0001,
	afCompatibilityMask				= 0x0070,
	afSideBySideCompatible			= 0x0000,
	afNonSideBySideAppDomain		= 0x0010,
	afNonSideBySideProcess			= 0x0020,
	afNonSideBySideMachine			= 0x0030,
	afEnableJITcompileTracking		= 0x8000,
	afDisableJITcompileOptimizer	= 0x4000,

} CorAssemblyFlags;
#define	IsAfPublicKey(x)			((x) & afPublicKey)
#define	IsAfPublicKeyToken(x)		(((x) & afPublicKey) == 0)
#define	IsAfSideBySideCompatible(x)			\
			(((x) & afCompatibilityMask) == afSideBySideCompatible)
#define	IsAfNonSideBySideAppDomain(x)		\
			(((x) & afCompatibilityMask) == afNonSideBySideAppDomain)
#define	IsAfNonSideBySideProcess(x)			\
			(((x) & afCompatibilityMask) == afNonSideBySideProcess)
#define	IsAfNonSideBySideMachine(x)			\
			(((x) & afCompatibilityMask) == afNonSideBySideMachine)
#define	IsAfEnableJITcompileTracking(x)		\
			((x) & afEnableJITcompileTracking)
#define	IsAfDisableJITcompileOptimizer(x)	\
			((x) & afDisableJITcompileOptimizer)

/*
 * Manifest resource attributes.
 */
typedef enum CorManifestResourceFlags
{
	mrVisibilityMask		= 0x0007,
	mrPublic				= 0x0001,
	mrPrivate				= 0x0002,

} CorManifestResourceFlags;
#define	IsMrPublic(x)				(((x) & mrVisibilityMask) == mrPublic)
#define	IsMrPrivate(x)				(((x) & mrVisibilityMask) == mrPrivate)

/*
 * File attributes.
 */
typedef enum CorFileFlags
{
	ffContainsMetaData		= 0x0000,
	ffContainsNoMetaData	= 0x0001,

} CorFileFlags;
#define	IsFfContainsMetaData(x)		(((x) & ffContainsNoMetaData) == 0)
#define	IsFfContainsNoMetaData(x)	((x) & ffContainsNoMetaData)

/*
 * Argument types.
 */
typedef enum CorArgType
{
	IMAGE_CEE_CS_END		= 0x00,
	IMAGE_CEE_CS_VOID		= 0x01,
	IMAGE_CEE_CS_I4			= 0x02,
	IMAGE_CEE_CS_I8			= 0x03,
	IMAGE_CEE_CS_R4			= 0x04,
	IMAGE_CEE_CS_R8			= 0x05,
	IMAGE_CEE_CS_PTR		= 0x06,
	IMAGE_CEE_CS_OBJECT		= 0x07,
	IMAGE_CEE_CS_STRUCT4	= 0x08,
	IMAGE_CEE_CS_STRUCT32	= 0x09,
	IMAGE_CEE_CS_BYVALUE	= 0x0A,

} CorArgType;

/*
 * Attribute target types.
 */
typedef enum CorAttributeTargets
{
	catAssembly			= 0x0001,
	catModule			= 0x0002,
	catClass			= 0x0004,
	catStruct			= 0x0008,
	catEnum				= 0x0010,
	catConstructor		= 0x0020,
	catMethod			= 0x0040,
	catProperty			= 0x0080,
	catField			= 0x0100,
	catEvent			= 0x0200,
	catInterface		= 0x0400,
	catParameter		= 0x0800,
	catDelegate			= 0x1000,
	catClassMembers		= 0x17FC,
	catAll				= 0x1FFF,

} CorAttributeTargets;

/*
 * Calling conventions.
 */
typedef enum CorCallingConvention
{
	IMAGE_CEE_CS_CALLCONV_MASK				= 0x0f,
	IMAGE_CEE_CS_CALLCONV_DEFAULT			= 0x00,
	IMAGE_CEE_CS_CALLCONV_VARARG			= 0x05,
	IMAGE_CEE_CS_CALLCONV_FIELD				= 0x06,
	IMAGE_CEE_CS_CALLCONV_LOCAL_SIG			= 0x07,
	IMAGE_CEE_CS_CALLCONV_PROPERTY			= 0x08,
	IMAGE_CEE_CS_CALLCONV_UNMGD				= 0x09,
	IMAGE_CEE_CS_CALLCONV_MAX				= 0x10,
	IMAGE_CEE_CS_CALLCONV_HASTHIS			= 0x20,
	IMAGE_CEE_CS_CALLCONV_EXPLICITTHIS		= 0x40,

} CorCallingConvention;
typedef enum CorUnmanagedCallingConvention
{
	IMAGE_CEE_CS_CALLCONV_C					= 0x01,
	IMAGE_CEE_UNMANAGED_CALLCONV_C			= 0x01,
	IMAGE_CEE_CS_CALLCONV_STDCALL			= 0x02,
	IMAGE_CEE_UNMANAGED_CALLCONV_STDCALL	= 0x02,
	IMAGE_CEE_CS_CALLCONV_THISCALL			= 0x03,
	IMAGE_CEE_UNMANAGED_CALLCONV_THISCALL	= 0x03,
	IMAGE_CEE_CS_CALLCONV_FASTCALL			= 0x04,
	IMAGE_CEE_UNMANAGED_CALLCONV_FASTCALL	= 0x04,

} CorUnmanagedCallingConvention;

/*
 * Duplicate checking flags.
 */
typedef enum CorCheckDuplicatesFor
{
	MDDupAll                = 0xFFFFFFFF,
	MDDupENC                = 0xFFFFFFFF,
	MDNoDupChecks           = 0x00000000,
	MDDupTypeDef            = 0x00000001,
	MDDupInterfaceImpl      = 0x00000002,
	MDDupMethodDef          = 0x00000004,
	MDDupTypeRef            = 0x00000008,
	MDDupMemberRef          = 0x00000010,
	MDDupCustomAttribute    = 0x00000020,
	MDDupParamDef           = 0x00000040,
	MDDupPermission         = 0x00000080,
	MDDupProperty           = 0x00000100,
	MDDupEvent              = 0x00000200,
	MDDupFieldDef           = 0x00000400,
	MDDupSignature          = 0x00000800,
	MDDupModuleRef          = 0x00001000,
	MDDupTypeSpec           = 0x00002000,
	MDDupImplMap            = 0x00004000,
	MDDupAssemblyRef        = 0x00008000,
	MDDupFile               = 0x00010000,
	MDDupExportedType       = 0x00020000,
	MDDupManifestResource   = 0x00040000,
	MDDupAssembly           = 0x10000000,
	MDDupDefault			= MDDupTypeRef | MDDupMemberRef |
							  MDDupSignature | MDDupTypeSpec,

} CorCheckDuplicatesFor;

/*
 * Element types.
 */
typedef enum CorElementType
{
	ELEMENT_TYPE_END		= 0x00,
	ELEMENT_TYPE_VOID		= 0x01,
	ELEMENT_TYPE_BOOLEAN	= 0x02,
	ELEMENT_TYPE_CHAR		= 0x03,
	ELEMENT_TYPE_I1			= 0x04,
	ELEMENT_TYPE_U1			= 0x05,
	ELEMENT_TYPE_I2			= 0x06,
	ELEMENT_TYPE_U2			= 0x07,
	ELEMENT_TYPE_I4			= 0x08,
	ELEMENT_TYPE_U4			= 0x09,
	ELEMENT_TYPE_I8			= 0x0A,
	ELEMENT_TYPE_U8			= 0x0B,
	ELEMENT_TYPE_R4			= 0x0C,
	ELEMENT_TYPE_R8			= 0x0D,
	ELEMENT_TYPE_STRING		= 0x0E,
	ELEMENT_TYPE_PTR		= 0x0F,
	ELEMENT_TYPE_BYREF		= 0x10,
	ELEMENT_TYPE_VALUETYPE	= 0x11,
	ELEMENT_TYPE_CLASS		= 0x12,
	ELEMENT_TYPE_ARRAY		= 0x14,
	ELEMENT_TYPE_TYPEDBYREF	= 0x16,
	ELEMENT_TYPE_I			= 0x18,
	ELEMENT_TYPE_U			= 0x19,
	ELEMENT_TYPE_FNPTR		= 0x1B,
	ELEMENT_TYPE_OBJECT		= 0x1C,
	ELEMENT_TYPE_SZARRAY	= 0x1D,
	ELEMENT_TYPE_CMOD_REQD	= 0x1F,
	ELEMENT_TYPE_CMOD_OPT	= 0x20,
	ELEMENT_TYPE_INTERNAL	= 0x21,
	ELEMENT_TYPE_MAX		= 0x22,
	ELEMENT_TYPE_MODIFIER	= 0x40,
	ELEMENT_TYPE_SENTINEL	= 0x41,
	ELEMENT_TYPE_PINNED		= 0x45,

} CorElementType;

/*
 * Special tokens for base types.
 */
typedef enum CorBaseType
{
	mdtBaseType_BOOLEAN	= (mdtBaseType | ELEMENT_TYPE_BOOLEAN),
	mdtBaseType_CHAR	= (mdtBaseType | ELEMENT_TYPE_CHAR),
	mdtBaseType_I1		= (mdtBaseType | ELEMENT_TYPE_I1),
	mdtBaseType_U1		= (mdtBaseType | ELEMENT_TYPE_U1),
	mdtBaseType_I2		= (mdtBaseType | ELEMENT_TYPE_I2),
	mdtBaseType_U2		= (mdtBaseType | ELEMENT_TYPE_U2),
	mdtBaseType_I4		= (mdtBaseType | ELEMENT_TYPE_I4),
	mdtBaseType_U4		= (mdtBaseType | ELEMENT_TYPE_U4),
	mdtBaseType_I8		= (mdtBaseType | ELEMENT_TYPE_I8),
	mdtBaseType_U8		= (mdtBaseType | ELEMENT_TYPE_U8),
	mdtBaseType_R4		= (mdtBaseType | ELEMENT_TYPE_R4),
	mdtBaseType_R8		= (mdtBaseType | ELEMENT_TYPE_R8),
	mdtBaseType_STRING	= (mdtBaseType | ELEMENT_TYPE_STRING),
	mdtBaseType_I		= (mdtBaseType | ELEMENT_TYPE_I),
	mdtBaseType_U		= (mdtBaseType | ELEMENT_TYPE_U),

} CorBaseType;

/*
 * Error flags for out of order emits.
 */
typedef enum CorErrorIfEmitOutOfOrder
{
	MDErrorOutOfOrderDefault	= 0x00000000,
	MDErrorOutOfOrderNone		= 0x00000000,
	MDMethodOutOfOrder			= 0x00000001,
	MDFieldOutOfOrder			= 0x00000002,
	MDParamOutOfOrder			= 0x00000004,
	MDPropertyOutOfOrder		= 0x00000008,
	MDEventOutOfOrder			= 0x00000010,
	MDErrorOutOfOrderAll		= 0xFFFFFFFF,

} CorErrorIfEmitOutOfOrder;

/*
 * Exception clause types.
 */
typedef enum CorExceptionFlag
{
	COR_ILEXCEPTION_CLAUSE_NONE			= 0x0000,
	COR_ILEXCEPTION_CLAUSE_OFFSETLEN	= 0x0000,
	COR_ILEXCEPTION_CLAUSE_DEPRECATED	= 0x0000,
	COR_ILEXCEPTION_CLAUSE_FILTER		= 0x0001,
	COR_ILEXCEPTION_CLAUSE_FINALLY		= 0x0002,
	COR_ILEXCEPTION_CLAUSE_FAULT		= 0x0004,

} CorExceptionFlag;

/*
 * IL method header flags.
 */
typedef enum CorILMethodFlags
{
	CorILMethod_FormatShift		= 0x0003,
	CorILMethod_FormatMask		= 0x0007,
	CorILMethod_SmallFormat		= 0x0000,
	CorILMethod_TinyFormat		= 0x0002,
	CorILMethod_FatFormat		= 0x0003,
	CorILMethod_TinyFormat1		= 0x0006,
	CorILMethod_MoreSects		= 0x0008,
	CorILMethod_InitLocals		= 0x0010,
	CorILMethod_CompressedIL	= 0x0040,

} CorILMethodFlags;

/*
 * IL method section types.
 */
typedef enum CorILMethodSect
{
	CorILMethod_Sect_KindMask    = 0x3F,
	CorILMethod_Sect_Reserved    = 0x00,
	CorILMethod_Sect_EHTable     = 0x01,
	CorILMethod_Sect_OptILTable  = 0x02,
	CorILMethod_Sect_FatFormat   = 0x40,
	CorILMethod_Sect_MoreSects   = 0x80,

} CorILMethodSect;

/*
 * Import options.
 */
typedef enum CorImportOptions
{
	MDImportOptionDefault				= 0x00000000,
	MDImportOptionAllTypeDefs			= 0x00000001,
	MDImportOptionAllMethodDefs			= 0x00000002,
	MDImportOptionAllFieldDefs			= 0x00000004,
	MDImportOptionAllProperties			= 0x00000008,
	MDImportOptionAllEvents				= 0x00000010,
	MDImportOptionAllCustomAttributes	= 0x00000020,
	MDImportOptionAllExportedTypes		= 0x00000040,
	MDImportOptionAll					= 0xFFFFFFFF,

} CorImportOptions;

/*
 * Linker options.
 */
typedef enum CorLinkerOptions
{
	MDAssembly				= 0,
	MDNetModule				= 1,

} CorLinkerOptions;

/*
 * Native types.
 */
typedef enum CorNativeType
{
	NATIVE_TYPE_END				= 0x00,
	NATIVE_TYPE_VOID			= 0x01,
	NATIVE_TYPE_BOOLEAN			= 0x02,
	NATIVE_TYPE_I1				= 0x03,
	NATIVE_TYPE_U1				= 0x04,
	NATIVE_TYPE_I2				= 0x05,
	NATIVE_TYPE_U2				= 0x06,
	NATIVE_TYPE_I4				= 0x07,
	NATIVE_TYPE_U4				= 0x08,
	NATIVE_TYPE_I8				= 0x09,
	NATIVE_TYPE_U8				= 0x0A,
	NATIVE_TYPE_R4				= 0x0B,
	NATIVE_TYPE_R8				= 0x0C,
	NATIVE_TYPE_SYSCHAR			= 0x0D,
	NATIVE_TYPE_VARIANT			= 0x0E,
	NATIVE_TYPE_CURRENCY		= 0x0F,
	NATIVE_TYPE_PTR				= 0x10,
	NATIVE_TYPE_DECIMAL			= 0x11,
	NATIVE_TYPE_DATE			= 0x12,
	NATIVE_TYPE_BSTR			= 0x13,
	NATIVE_TYPE_LPSTR			= 0x14,
	NATIVE_TYPE_LPWSTR			= 0x15,
	NATIVE_TYPE_LPTSTR			= 0x16,
	NATIVE_TYPE_FIXEDSYSSTRING	= 0x17,
	NATIVE_TYPE_OBJECTREF		= 0x18,
	NATIVE_TYPE_IUNKNOWN		= 0x19,
	NATIVE_TYPE_IDISPATCH		= 0x1A,
	NATIVE_TYPE_STRUCT			= 0x1B,
	NATIVE_TYPE_INTF			= 0x1C,
	NATIVE_TYPE_SAFEARRAY		= 0x1D,
	NATIVE_TYPE_FIXEDARRAY		= 0x1E,
	NATIVE_TYPE_INT				= 0x1F,
	NATIVE_TYPE_UINT			= 0x20,
	NATIVE_TYPE_NESTEDSTRUCT	= 0x21,
	NATIVE_TYPE_BYVALSTR		= 0x22,
	NATIVE_TYPE_ANSIBSTR		= 0x23,
	NATIVE_TYPE_TBSTR			= 0x24,
	NATIVE_TYPE_VARIANTBOOL		= 0x25,
	NATIVE_TYPE_FUNC			= 0x26,
	NATIVE_TYPE_ASANY			= 0x28,
	NATIVE_TYPE_ARRAY			= 0x2A,
	NATIVE_TYPE_LPSTRUCT		= 0x2B,
	NATIVE_TYPE_CUSTOMMARSHALER	= 0x2C,
	NATIVE_TYPE_ERROR			= 0x2D,
	NATIVE_TYPE_MAX				= 0x50,

} CorNativeType;

/*
 * Notification flags for when tokens move.
 */
typedef enum CorNotificationForTokenMovement
{
	MDNotifyNone			= 0x00000000,
	MDNotifyMethodDef		= 0x00000001,
	MDNotifyMemberRef		= 0x00000002,
	MDNotifyFieldDef		= 0x00000004,
	MDNotifyTypeRef			= 0x00000008,
	MDNotifyTypeDef			= 0x00000010,
	MDNotifyParamDef		= 0x00000020,
	MDNotifyInterfaceImpl	= 0x00000040,
	MDNotifyProperty		= 0x00000080,
	MDNotifyEvent			= 0x00000100,
	MDNotifySignature		= 0x00000200,
	MDNotifyTypeSpec		= 0x00000400,
	MDNotifyCustomAttribute	= 0x00000800,
	MDNotifySecurityValue	= 0x00001000,
	MDNotifyPermission		= 0x00002000,
	MDNotifyModuleRef		= 0x00004000,
	MDNotifyNameSpace		= 0x00008000,
	MDNotifyAssemblyRef		= 0x01000000,
	MDNotifyFile			= 0x02000000,
	MDNotifyExportedType	= 0x04000000,
	MDNotifyResource		= 0x08000000,
	MDNotifyDefault			= 0x0000000F,
	MDNotifyAll				= 0xFFFFFFFF,

} CorNotificationForTokenMovement;

/*
 * Open flags.
 */
typedef enum CorOpenFlags
{
	ofRead			= 0x00,
	ofWrite			= 0x01,
	ofCopyMemory	= 0x02,
	ofCacheImage	= 0x04,
	ofNoTypeLib		= 0x80,

} CorOpenFlags;

/*
 * Flags for checking reference to definition resolution.
 */
typedef enum CorRefToDefCheck
{
	MDRefToDefNone				= 0x00000000,
	MDTypeRefToDef				= 0x00000001,
	MDMemberRefToDef			= 0x00000002,
	MDRefToDefDefault			= 0x00000003,
	MDRefToDefAll				= 0xFFFFFFFF,

} CorRefToDefCheck;

/*
 * Save size accuracy.
 */
#ifndef _CORSAVESIZE_DEFINED_
#define _CORSAVESIZE_DEFINED_
typedef enum CorSaveSize
{
	cssAccurate				= 0x0000,
    cssQuick				= 0x0001,
    cssDiscardTransientCAs	= 0x0002,

} CorSaveSize;
#endif

/*
 * Serialization types.
 */
typedef enum CorSerializationType
{
	SERIALIZATION_TYPE_BOOLEAN			= 0x02,
	SERIALIZATION_TYPE_CHAR				= 0x03,
	SERIALIZATION_TYPE_I1				= 0x04,
	SERIALIZATION_TYPE_U1				= 0x05,
	SERIALIZATION_TYPE_I2				= 0x06,
	SERIALIZATION_TYPE_U2				= 0x07,
	SERIALIZATION_TYPE_I4				= 0x08,
	SERIALIZATION_TYPE_U4				= 0x09,
	SERIALIZATION_TYPE_I8				= 0x0A,
	SERIALIZATION_TYPE_U8				= 0x0B,
	SERIALIZATION_TYPE_R4				= 0x0C,
	SERIALIZATION_TYPE_R8				= 0x0D,
	SERIALIZATION_TYPE_STRING			= 0x0E,
	SERIALIZATION_TYPE_SZARRAY			= 0x1D,
	SERIALIZATION_TYPE_TYPE				= 0x50,
	SERIALIZATION_TYPE_TAGGED_OBJECT	= 0x51,
	SERIALIZATION_TYPE_FIELD			= 0x53,
	SERIALIZATION_TYPE_PROPERTY			= 0x54,
	SERIALIZATION_TYPE_ENUM				= 0x55,

} CorSerializationType;

/*
 * Flags for the "Edit and Continue" mode.
 */
typedef enum CorSetENC
{
	MDSetENCOn			= 0x01,
	MDUpdateENC			= 0x01,
	MDSetENCOff			= 0x02,
	MDUpdateFull		= 0x02,
	MDUpdateExtension	= 0x03,
	MDUpdateIncremental	= 0x04,
	MDUpdateMask		= 0x07,
	MDUpdateDelta		= 0x08,

} CorSetENC;

/*
 * Thread safety options.
 */
typedef enum CorThreadSafetyOptions
{
	MDThreadSafetyDefault	= 0,
	MDThreadSafetyOff		= 0,
	MDThreadSafetyOn		= 1,

} CorThreadSafetyOptions;

/*
 * Other random values.
 */
enum
{
	DESCR_GROUP_METHODDEF  = 0,
	DESCR_GROUP_METHODIMPL = 1

};

/*
 * Method headers.
 */
typedef struct IMAGE_COR_ILMETHOD_TINY
{
	BYTE	Flags_CodeSize;

} IMAGE_COR_ILMETHOD_TINY;
typedef struct IMAGE_COR_ILMETHOD_FAT
{
	unsigned int	Flags    : 12;
	unsigned int	Size     : 4;
	unsigned int	MaxStack : 16;
	DWORD			CodeSize;
	mdSignature		LocalVarSigTok;

} IMAGE_COR_ILMETHOD_FAT;
typedef union IMAGE_COR_ILMETHOD
{
	IMAGE_COR_ILMETHOD_TINY		Tiny;
	IMAGE_COR_ILMETHOD_FAT		Fat;

} IMAGE_COR_ILMETHOD;

/*
 * Method section headers.
 */
typedef struct IMAGE_COR_ILMETHOD_SECT_SMALL
{
	BYTE	Kind;
	BYTE	DataSize;

} IMAGE_COR_ILMETHOD_SECT_SMALL;
typedef struct IMAGE_COR_ILMETHOD_SECT_FAT
{
	unsigned int Kind     : 8;
	unsigned int DataSize : 24;

} IMAGE_COR_ILMETHOD_SECT_FAT;

/*
 * Method exception section headers.
 */
typedef struct IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL
{
	CorExceptionFlag		Flags         : 16;
	unsigned int			TryOffset     : 16;
	unsigned int			TryLength     : 8;
	unsigned int			HandlerOffset : 16;
	unsigned int			HandlerLength : 8;
	union
	{
		DWORD				ClassToken;
		DWORD				FilterOffset;

    }						u;

} IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL;
typedef struct IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT
{
	CorExceptionFlag		Flags;
	DWORD					TryOffset;
	DWORD					TryLength;
	DWORD					HandlerOffset;
	DWORD					HandlerLength;
	union
	{
		DWORD				ClassToken;
		DWORD				FilterOffset;

	}						u;

} IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT;

/*
 * Combined method section and exception headers.
 */
typedef struct IMAGE_COR_ILMETHOD_SECT_EH_SMALL
{
	IMAGE_COR_ILMETHOD_SECT_SMALL			SectSmall;
	WORD									Reserved;
	IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_SMALL	Clauses[1];

} IMAGE_COR_ILMETHOD_SECT_EH_SMALL;
typedef struct IMAGE_COR_ILMETHOD_SECT_EH_FAT
{
	IMAGE_COR_ILMETHOD_SECT_FAT				SectFat;
	IMAGE_COR_ILMETHOD_SECT_EH_CLAUSE_FAT	Clauses[1];

} IMAGE_COR_ILMETHOD_SECT_EH_FAT;
typedef union IMAGE_COR_ILMETHOD_SECT_EH
{
	IMAGE_COR_ILMETHOD_SECT_EH_SMALL		Small;
	IMAGE_COR_ILMETHOD_SECT_EH_FAT			Fat;

} IMAGE_COR_ILMETHOD_SECT_EH;

/*
 * Miscellaneous structures.
 */
typedef struct COR_SECATTR
{
	mdMemberRef		tkCtor;
	const void     *pCustomAttribute;
	ULONG			cbCustomAttribute;

} COR_SECATTR;
typedef struct IMAGE_COR_FIELD_OFFSET
{
	mdFieldDef		ridOfField;
	ULONG			ulOffset;

} IMAGE_COR_FIELD_OFFSET;
typedef struct IMAGE_COR_FIXUP_ENTRY
{
	ULONG			ulRVA;
	ULONG			Count;

} IMAGE_COR_FIXUP_ENTRY;
typedef struct IMAGE_COR_MIH_ENTRY
{
	ULONG			EHRVA;
	ULONG			MethodRVA;
	mdToken			Token;
	BYTE			Flags;
	BYTE			CodeManager;
	BYTE			MIHData[0];

} IMAGE_COR_MIH_ENTRY;
typedef struct IMAGE_COR_NATIVE_DESCRIPTOR
{
	DWORD			GCInfo;
	DWORD			EHInfo;

} IMAGE_COR_NATIVE_DESCRIPTOR;
typedef struct IMAGE_COR_VTABLE_FIXUP
{
	ULONG			RVA;
	USHORT			Count;
	USHORT			Type;

} IMAGE_COR_VTABLE_FIXUP;
typedef struct IMAGE_COR_X86_RUNTIME_FUNCTION_ENTRY
{
	ULONG			BeginAddress;
	ULONG			EndAddress;
	ULONG			MIH;

} IMAGE_COR_X86_RUNTIME_FUNCTION_ENTRY;

/*
 * Useful string constants.
 */
#define CMOD_CALLCONV_NAMESPACE_OLD	"System.Runtime.InteropServices"
#define CMOD_CALLCONV_NAMESPACE		"System.Runtime.CompilerServices"
#define CMOD_CALLCONV_NAME_CDECL	"CallConvCdecl"
#define CMOD_CALLCONV_NAME_STDCALL	"CallConvStdcall"
#define CMOD_CALLCONV_NAME_THISCALL	"CallConvThiscall"
#define CMOD_CALLCONV_NAME_FASTCALL	"CallConvFastcall"

#define	COR_DELETED_NAME_A			"_Deleted"
#define	COR_DELETED_NAME_W			L"_Deleted"

#define	COR_ENUM_FIELD_NAME			"value__"
#define	COR_ENUM_FIELD_NAME_W		L"value__"

#define	COR_VTABLEGAP_NAME_A		"_VtblGap"
#define	COR_VTABLEGAP_NAME_W		L"_VtblGap"

#define DEFAULTDOMAIN_LOADEROPTIMIZATION_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I1}
#define DEFAULTDOMAIN_LOADEROPTIMIZATION_TYPE	\
			"System.LoaderOptimizationAttribute"
#define DEFAULTDOMAIN_LOADEROPTIMIZATION_TYPE_W	\
			L"System.LoaderOptimizationAttribute"

#define DEFAULTDOMAIN_MTA_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define DEFAULTDOMAIN_MTA_TYPE	\
			"System.MTAThreadAttribute"
#define DEFAULTDOMAIN_MTA_TYPE_W	\
			L"System.MTAThreadAttribute"

#define DEFAULTDOMAIN_STA_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define DEFAULTDOMAIN_STA_TYPE	\
			"System.STAThreadAttribute"
#define DEFAULTDOMAIN_STA_TYPE_W	\
			L"System.STAThreadAttribute"

#define	FRAMEWORK_REGISTRY_KEY		"Software\\Microsoft\\.NETFramework"
#define	FRAMEWORK_REGISTRY_KEY_W	L"Software\\Microsoft\\.NETFramework"

#ifndef IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS
#define IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS	\
			(IMAGE_CEE_CS_CALLCONV_DEFAULT | IMAGE_CEE_CS_CALLCONV_HASTHIS)
#endif

#define INTEROP_AUTOPROXY_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_BOOLEAN}
#define INTEROP_AUTOPROXY_TYPE	\
			"System.Runtime.InteropServices.AutomationProxyAttribute"
#define INTEROP_AUTOPROXY_TYPE_W	\
			L"System.Runtime.InteropServices.AutomationProxyAttribute"

#define INTEROP_CLASSINTERFACE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_CLASSINTERFACE_TYPE	\
			"System.Runtime.InteropServices.ClassInterfaceAttribute"
#define INTEROP_CLASSINTERFACE_TYPE_W	\
			L"System.Runtime.InteropServices.ClassInterfaceAttribute"

#define INTEROP_COCLASS_TYPE_W	\
			L"System.Runtime.InteropServices.CoClassAttribute"
#define INTEROP_COCLASS_TYPE	\
			"System.Runtime.InteropServices.CoClassAttribute"

#define INTEROP_COMALIASNAME_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_COMALIASNAME_TYPE	\
			"System.Runtime.InteropServices.ComAliasNameAttribute"
#define INTEROP_COMALIASNAME_TYPE_W	\
			L"System.Runtime.InteropServices.ComAliasNameAttribute"

#define INTEROP_COMCONVERSIONLOSS_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_COMCONVERSIONLOSS_TYPE	\
			"System.Runtime.InteropServices.ComConversionLossAttribute"
#define INTEROP_COMCONVERSIONLOSS_TYPE_W	\
			L"System.Runtime.InteropServices.ComConversionLossAttribute"

#define INTEROP_COMEMULATE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_COMEMULATE_TYPE	\
			"System.Runtime.InteropServices.ComEmulateAttribute"
#define INTEROP_COMEMULATE_TYPE_W	\
			L"System.Runtime.InteropServices.ComEmulateAttribute"

#define INTEROP_COMEVENTINTERFACE_TYPE	\
			"System.Runtime.InteropServices.ComEventInterfaceAttribute"
#define INTEROP_COMEVENTINTERFACE_TYPE_W	\
			L"System.Runtime.InteropServices.ComEventInterfaceAttribute"

#define INTEROP_COMIMPORT_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_COMIMPORT_TYPE	\
			"System.Runtime.InteropServices.ComImportAttribute"
#define INTEROP_COMIMPORT_TYPE_W	\
			L"System.Runtime.InteropServices.ComImportAttribute"

#define INTEROP_COMREGISTERFUNCTION_SIG		\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_COMREGISTERFUNCTION_TYPE	\
			"System.Runtime.InteropServices.ComRegisterFunctionAttribute"
#define INTEROP_COMREGISTERFUNCTION_TYPE_W	\
			L"System.Runtime.InteropServices.ComRegisterFunctionAttribute"

#define INTEROP_COMSOURCEINTERFACES_SIG		\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_COMSOURCEINTERFACES_TYPE	\
			"System.Runtime.InteropServices.ComSourceInterfacesAttribute"
#define INTEROP_COMSOURCEINTERFACES_TYPE_W	\
			L"System.Runtime.InteropServices.ComSourceInterfacesAttribute"

#define INTEROP_COMSUBSTITUTABLEINTERFACE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_COMSUBSTITUTABLEINTERFACE_TYPE	\
			"System.Runtime.InteropServices.ComSubstitutableInterfaceAttribute"
#define INTEROP_COMSUBSTITUTABLEINTERFACE_TYPE_W	\
			L"System.Runtime.InteropServices.ComSubstitutableInterfaceAttribute"

#define INTEROP_COMUNREGISTERFUNCTION_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_COMUNREGISTERFUNCTION_TYPE	\
			"System.Runtime.InteropServices.ComUnregisterFunctionAttribute"
#define INTEROP_COMUNREGISTERFUNCTION_TYPE_W	\
			L"System.Runtime.InteropServices.ComUnregisterFunctionAttribute"

#define INTEROP_COMVISIBLE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_BOOLEAN}
#define INTEROP_COMVISIBLE_TYPE	\
			"System.Runtime.InteropServices.ComVisibleAttribute"
#define INTEROP_COMVISIBLE_TYPE_W	\
			L"System.Runtime.InteropServices.ComVisibleAttribute"

#define INTEROP_DATETIMEVALUE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I8}
#define INTEROP_DATETIMEVALUE_TYPE	\
			"System.Runtime.CompilerServices.DateTimeConstantAttribute"
#define INTEROP_DATETIMEVALUE_TYPE_W	\
			L"System.Runtime.CompilerServices.DateTimeConstantAttribute"

#define INTEROP_DECIMALVALUE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 5, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_U1, ELEMENT_TYPE_U1, \
			 ELEMENT_TYPE_U4, ELEMENT_TYPE_U4, ELEMENT_TYPE_U4}
#define INTEROP_DECIMALVALUE_TYPE	\
			"System.Runtime.CompilerServices.DecimalConstantAttribute"
#define INTEROP_DECIMALVALUE_TYPE_W	\
			L"System.Runtime.CompilerServices.DecimalConstantAttribute"

#define INTEROP_DEFAULTMEMBER_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_DEFAULTMEMBER_TYPE	\
			"System.Reflection.DefaultMemberAttribute"
#define INTEROP_DEFAULTMEMBER_TYPE_W	\
			L"System.Reflection.DefaultMemberAttribute"

#define INTEROP_DISPID_SIG		\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I4}
#define INTEROP_DISPID_TYPE		\
			"System.Runtime.InteropServices.DispIdAttribute"
#define INTEROP_DISPID_TYPE_W	\
			L"System.Runtime.InteropServices.DispIdAttribute"

#define INTEROP_GUID_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_GUID_TYPE	\
			"System.Runtime.InteropServices.GuidAttribute"
#define INTEROP_GUID_TYPE_W	\
			L"System.Runtime.InteropServices.GuidAttribute"

#define INTEROP_IDISPATCHIMPL_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_IDISPATCHIMPL_TYPE	\
			"System.Runtime.InteropServices.IDispatchImplAttribute"
#define INTEROP_IDISPATCHIMPL_TYPE_W	\
			L"System.Runtime.InteropServices.IDispatchImplAttribute"

#define INTEROP_IDISPATCHVALUE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_IDISPATCHVALUE_TYPE	\
			"System.Runtime.CompilerServices.IDispatchConstantAttribute"
#define INTEROP_IDISPATCHVALUE_TYPE_W	\
			L"System.Runtime.CompilerServices.IDispatchConstantAttribute"

#define INTEROP_IMPORTEDFROMTYPELIB_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_STRING}
#define INTEROP_IMPORTEDFROMTYPELIB_TYPE	\
			"System.Runtime.InteropServices.ImportedFromTypeLibAttribute"
#define INTEROP_IMPORTEDFROMTYPELIB_TYPE_W	\
			L"System.Runtime.InteropServices.ImportedFromTypeLibAttribute"

#define INTEROP_IN_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_IN_TYPE	\
			"System.Runtime.InteropServices.InAttribute"
#define INTEROP_IN_TYPE_W	\
			L"System.Runtime.InteropServices.InAttribute"

#define INTEROP_INTERFACETYPE_SIG		\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_INTERFACETYPE_TYPE		\
			"System.Runtime.InteropServices.InterfaceTypeAttribute"
#define INTEROP_INTERFACETYPE_TYPE_W	\
			L"System.Runtime.InteropServices.InterfaceTypeAttribute"

#define INTEROP_IUNKNOWNVALUE_SIG 	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_IUNKNOWNVALUE_TYPE	\
			"System.Runtime.CompilerServices.IUnknownConstantAttribute"
#define INTEROP_IUNKNOWNVALUE_TYPE_W	\
			L"System.Runtime.CompilerServices.IUnknownConstantAttribute"

#define INTEROP_LCIDCONVERSION_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I4}
#define INTEROP_LCIDCONVERSION_TYPE	\
			"System.Runtime.InteropServices.LCIDConversionAttribute"
#define INTEROP_LCIDCONVERSION_TYPE_W	\
			L"System.Runtime.InteropServices.LCIDConversionAttribute"

#define INTEROP_MARSHALAS_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_MARSHALAS_TYPE	\
			"System.Runtime.InteropServices.MarshalAsAttribute"
#define INTEROP_MARSHALAS_TYPE_W	\
			L"System.Runtime.InteropServices.MarshalAsAttribute"

#define INTEROP_OUT_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_OUT_TYPE	\
			"System.Runtime.InteropServices.OutAttribute"
#define INTEROP_OUT_TYPE_W	\
			L"System.Runtime.InteropServices.OutAttribute"

#define INTEROP_PARAMARRAY_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_VOID}
#define INTEROP_PARAMARRAY_TYPE	\
			"System.ParamArrayAttribute"
#define INTEROP_PARAMARRAY_TYPE_W	\
			L"System.ParamArrayAttribute"

#define INTEROP_PRESERVESIG_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 0, ELEMENT_TYPE_BOOLEAN}
#define INTEROP_PRESERVESIG_TYPE	\
			"System.Runtime.InteropServices.PreserveSigAttribure"
#define INTEROP_PRESERVESIG_TYPE_W	\
			L"System.Runtime.InteropServices.PreserveSigAttribure"

#define INTEROP_TYPELIBFUNC_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_TYPELIBFUNC_TYPE	\
			"System.Runtime.InteropServices.TypeLibFuncAttribute"
#define INTEROP_TYPELIBFUNC_TYPE_W	\
			L"System.Runtime.InteropServices.TypeLibFuncAttribute"

#define INTEROP_TYPELIBTYPE_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_TYPELIBTYPE_TYPE	\
			"System.Runtime.InteropServices.TypeLibTypeAttribute"
#define INTEROP_TYPELIBTYPE_TYPE_W	\
			L"System.Runtime.InteropServices.TypeLibTypeAttribute"

#define INTEROP_TYPELIBVAR_SIG	\
			{IMAGE_CEE_CS_CALLCONV_DEFAULT_HASTHIS, 1, \
			 ELEMENT_TYPE_VOID, ELEMENT_TYPE_I2}
#define INTEROP_TYPELIBVAR_TYPE	\
			"System.Runtime.InteropServices.TypeLibVarAttribute"
#define INTEROP_TYPELIBVAR_TYPE_W	\
			L"System.Runtime.InteropServices.TypeLibVarAttribute"

#endif /* __CORHDR_H__ */
