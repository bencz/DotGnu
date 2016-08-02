/*
 * il_program.h - Definitions related to program information.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef	_IL_PROGRAM_H
#define	_IL_PROGRAM_H

#include "il_image.h"
#include "il_types.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque type declarations.
 */
typedef struct _tagILProgramItem	ILProgramItem;
typedef struct _tagILAttribute		ILAttribute;
typedef struct _tagILModule			ILModule;
typedef struct _tagILAssembly		ILAssembly;
typedef struct _tagILNestedInfo		ILNestedInfo;
typedef struct _tagILImplements		ILImplements;
typedef struct _tagILMember			ILMember;
typedef struct _tagILMethod			ILMethod;
typedef struct _tagILMethodInstance	ILMethodInstance;
typedef struct _tagILParameter		ILParameter;
typedef struct _tagILField			ILField;
typedef struct _tagILEvent			ILEvent;
typedef struct _tagILProperty		ILProperty;
typedef struct _tagILPInvoke		ILPInvoke;
typedef struct _tagILOverride		ILOverride;
typedef struct _tagILEventMap		ILEventMap;
typedef struct _tagILPropertyMap	ILPropertyMap;
typedef struct _tagILMethodSem		ILMethodSem;
typedef struct _tagILOSInfo			ILOSInfo;
typedef struct _tagILProcessorInfo	ILProcessorInfo;
typedef struct _tagILTypeSpec		ILTypeSpec;
typedef struct _tagILStandAloneSig	ILStandAloneSig;
typedef struct _tagILConstant		ILConstant;
typedef struct _tagILFieldRVA		ILFieldRVA;
typedef struct _tagILFieldLayout	ILFieldLayout;
typedef struct _tagILFieldMarshal	ILFieldMarshal;
typedef struct _tagILClassLayout	ILClassLayout;
typedef struct _tagILDeclSecurity	ILDeclSecurity;
typedef struct _tagILFileDecl		ILFileDecl;
typedef struct _tagILManifestRes	ILManifestRes;
typedef struct _tagILExportedType	ILExportedType;
typedef struct _tagILGenericPar		ILGenericPar;
typedef struct _tagILGenericConstraint ILGenericConstraint;
typedef struct _tagILMethodSpec		ILMethodSpec;
typedef struct _tagILMemberRef		ILMemberRef;

/*
 * Iterate over the list of attributes that are associated
 * with a program item.  If "attr" is NULL, then return the
 * first attribute in the list.  Otherwise return the next
 * attribute in the list after "attr".  Returns NULL at the
 * end of the list.
 */
ILAttribute *ILProgramItemNextAttribute(ILProgramItem *item,
										ILAttribute *attr);

/*
 * Add an attribute to the list associated with a program item.
 */
void ILProgramItemAddAttribute(ILProgramItem *item, ILAttribute *attr);

/*
 * Remove an attribute from the list associated with a program item.
 * Returns the following attribute in the list.
 */
ILAttribute *ILProgramItemRemoveAttribute(ILProgramItem *item,
										  ILAttribute *attr);

/*
 * Get the number of attributes associated with a program item.
 */
unsigned long ILProgramItemNumAttributes(ILProgramItem *item);

/*
 * Convert the builtin attributes on a program item into their
 * correct metadata representations.  Returns zero if out of memory.
 */
int ILProgramItemConvertAttrs(ILProgramItem *item);

/*
 * Get the image with which a program item is associated.
 */
ILImage *ILProgramItemGetImage(ILProgramItem *item);

/*
 * Get the token code associated with a program item.
 */
ILToken ILProgramItemGetToken(ILProgramItem *item);

/*
 * Convert a pointer to a subclass into ILProgramItem.
 */
#define	ILToProgramItem(item)	((ILProgramItem *)(item))

/*
 * Determine if a program item can be converted into
 * one of the other token structure types.  Returns the
 * converted value, or NULL if of not the correct type.
 * It is safe to supply NULL as an argument.
 */
ILAttribute *ILProgramItemToAttribute(ILProgramItem *item);
ILModule *ILProgramItemToModule(ILProgramItem *item);
ILAssembly *ILProgramItemToAssembly(ILProgramItem *item);
ILClass *ILProgramItemToClass(ILProgramItem *item);
ILMember *ILProgramItemToMember(ILProgramItem *item);
ILMethod *ILProgramItemToMethod(ILProgramItem *item);
ILParameter *ILProgramItemToParameter(ILProgramItem *item);
ILField *ILProgramItemToField(ILProgramItem *item);
ILEvent *ILProgramItemToEvent(ILProgramItem *item);
ILProperty *ILProgramItemToProperty(ILProgramItem *item);
ILPInvoke *ILProgramItemToPInvoke(ILProgramItem *item);
ILOverride *ILProgramItemToOverride(ILProgramItem *item);
ILEventMap *ILProgramItemToEventMap(ILProgramItem *item);
ILPropertyMap *ILProgramItemToPropertyMap(ILProgramItem *item);
ILMethodSem *ILProgramItemToMethodSem(ILProgramItem *item);
ILOSInfo *ILProgramItemToOSInfo(ILProgramItem *item);
ILProcessorInfo *ILProgramItemToProcessorInfo(ILProgramItem *item);
ILTypeSpec *ILProgramItemToTypeSpec(ILProgramItem *item);
ILStandAloneSig *ILProgramItemToStandAloneSig(ILProgramItem *item);
ILConstant *ILProgramItemToConstant(ILProgramItem *item);
ILFieldRVA *ILProgramItemToFieldRVA(ILProgramItem *item);
ILFieldLayout *ILProgramItemToFieldLayout(ILProgramItem *item);
ILFieldMarshal *ILProgramItemToFieldMarshal(ILProgramItem *item);
ILClassLayout *ILProgramItemToClassLayout(ILProgramItem *item);
ILDeclSecurity *ILProgramItemToDeclSecurity(ILProgramItem *item);
ILFileDecl *ILProgramItemToFileDecl(ILProgramItem *item);
ILManifestRes *ILProgramItemToManifestRes(ILProgramItem *item);
ILExportedType *ILProgramItemToExportedType(ILProgramItem *item);
ILGenericPar *ILProgramItemToGenericPar(ILProgramItem *item);
ILMethodSpec *ILProgramItemToMethodSpec(ILProgramItem *item);
ILGenericConstraint *ILProgramItemToGenericConstraint(ILProgramItem *item);

/*
 * Get the underlying class for a program item.
 * If the program item is a TypeDef or TypeRef the program item is returned
 * as a class.
 * If the program item is a TypeSpec for a WithType (generic) then the
 * generic class (MainType) is returned.
 */
ILClass *ILProgramItemToUnderlyingClass(ILProgramItem *item);

/*
 * Get the type for a program item.
 * Only TypeSpecs and Classes are handled here and the type of the TypeSpec
 * or the returnvalue from ILClassToType is returned.
 * If item is NULL or neither a TypeSpec nor a Class 0 is returned.
 */
ILType *ILProgramItemToType(ILProgramItem *item);

/*
 * Get the program item for a type.
 * Only primitive, class/value types, arrays and generic types are handled
 * by now.
 * Returns a TypeSpec for array and with types, a class for primitive,
 * reference and value types and NULL otherwise.
 */
ILProgramItem *ILProgramItemFromType(ILImage *image, ILType *type);

/*
 * Helper macros for querying information about a program item.
 */
#define	ILProgramItem_FromToken(image,token)	\
				((ILProgramItem *)ILImageTokenInfo((image), (token)))
#define	ILProgramItem_Image(item)	\
				(ILProgramItemGetImage((ILProgramItem *)(item)))
#define	ILProgramItem_Token(item)	\
				(ILProgramItemGetToken((ILProgramItem *)(item)))
#define	ILProgramItem_HasAttrs(item)	\
			(ILProgramItemNextAttribute((ILProgramItem *)(item), \
										(ILAttribute *)0) != 0)

/*
 * Values for the attribute target
 * They must be kept in sync with the System.AttributeTargets enumeration.
 */
#define IL_ATTRIBUTE_TARGET_ASSEMBLY		0x00000001
#define IL_ATTRIBUTE_TARGET_MODULE			0x00000002
#define IL_ATTRIBUTE_TARGET_CLASS			0x00000004
#define IL_ATTRIBUTE_TARGET_STRUCT			0x00000008
#define IL_ATTRIBUTE_TARGET_ENUM			0x00000010
#define IL_ATTRIBUTE_TARGET_CONSTRUCTOR		0x00000020
#define IL_ATTRIBUTE_TARGET_METHOD			0x00000040
#define IL_ATTRIBUTE_TARGET_PROPERTY		0x00000080
#define IL_ATTRIBUTE_TARGET_FIELD			0x00000100
#define IL_ATTRIBUTE_TARGET_EVENT			0x00000200
#define IL_ATTRIBUTE_TARGET_INTERFACE		0x00000400
#define IL_ATTRIBUTE_TARGET_PARAMETER		0x00000800
#define IL_ATTRIBUTE_TARGET_DELEGATE		0x00001000
#define IL_ATTRIBUTE_TARGET_RETURNVALUE		0x00002000
#define IL_ATTRIBUTE_TARGET_GENERICPAR		0x00004000

/*
 * Create a custom attribute within a specific image.
 * Returns NULL if out of memory.
 */
ILAttribute *ILAttributeCreate(ILImage *image, ILToken token);

/*
 * Set the type of a custom attribute to a program item.
 */
void ILAttributeSetType(ILAttribute *attr, ILProgramItem *type);

/*
 * Set the type of a custom attribute to string.
 */
void ILAttributeSetString(ILAttribute *attr);

/*
 * Get the owner of a custom attribute.  NULL if not yet assigned.
 */
ILProgramItem *ILAttributeGetOwner(ILAttribute *attr);

/*
 * Determine if an attribute's type is string or item.
 */
int ILAttributeTypeIsString(ILAttribute *attr);
int ILAttributeTypeIsItem(ILAttribute *attr);

/*
 * Convert an attribute's type into an item.
 */
ILProgramItem *ILAttributeTypeAsItem(ILAttribute *attr);

/*
 * Set the value of an attribute to a specific blob.
 * Returns zero if out of memory.
 */
int ILAttributeSetValue(ILAttribute *attr, const void *blob,
						ILUInt32 len);

/*
 * Get the value associated with an attribute.
 */
const void *ILAttributeGetValue(ILAttribute *attr, ILUInt32 *len);

/*
 * Opaque type for accessing the attribute usage attribute.
 */
typedef struct _tagILAttributeUsageAttribute ILAttributeUsageAttribute;

/*
 * Find an Attribute class and retrieve the usage information.
 * Returns 0 if the attribute could not be found.
 */
ILClass *ILFindCustomAttribute(ILContext *context, const char *name,
							   const char *namespace,
							   ILAttributeUsageAttribute **usage);

/*
 * Get the attribute usage information for a custom attribute.
 * If there is no attribute usage available for the attribute a default
 * usage will be created.
 * Returns 0 if attribute is 0.
 */
ILAttributeUsageAttribute *ILClassGetAttributeUsage(ILClass *attribute);

/*
 * Access the members of the attribute usage.
 */
ILUInt32 ILAttributeUsageAttributeGetValidOn(ILAttributeUsageAttribute *usage);
ILBool ILAttributeUsageAttributeGetAllowMultiple(ILAttributeUsageAttribute *usage);
ILBool ILAttributeUsageAttributeGetInherited(ILAttributeUsageAttribute *usage);

/*
 * Helper macros for querying information about an attribute.
 */
#define	ILAttribute_FromToken(image,token)	\
				((ILAttribute *)ILImageTokenInfo((image), (token)))
#define	ILAttribute_Token(attr)			(ILProgramItem_Token((attr)))
#define	ILAttribute_Owner(attr)			(ILAttributeGetOwner((attr)))
#define	ILAttribute_TypeIsString(attr)	(ILAttributeTypeIsString((attr)))
#define	ILAttribute_TypeIsItem(attr)	(ILAttributeTypeIsItem((attr)))
#define	ILAttribute_TypeAsItem(attr)	(ILAttributeTypeAsItem((attr)))

/*
 * Create a module structure and attach it to an image.
 * Returns NULL if out of memory.
 */
ILModule *ILModuleCreate(ILImage *image, ILToken token,
						 const char *name, const unsigned char *mvid);

/*
 * Set the name of a module to a new value.  Returns
 * zero if out of memory.
 */
int ILModuleSetName(ILModule *module, const char *name);

/*
 * Get the name of a module.
 */
const char *ILModuleGetName(ILModule *module);

/*
 * Set the MVID of a module to a new value.  If "mvid" is
 * NULL, then generate a random GUID.
 */
void ILModuleSetMVID(ILModule *module, const unsigned char *mvid);

/*
 * Get the MVID of a module.
 */
const unsigned char *ILModuleGetMVID(ILModule *module);

/*
 * Set the Edit & Continue generation for a module.
 * Returns zero if out of memory.
 */
int ILModuleSetGeneration(ILModule *module, ILUInt32 generation);

/*
 * Get the Edit & Continue generation for a module.
 */
ILUInt32 ILModuleGetGeneration(ILModule *module);

/*
 * Set the Edit & Continue identifier of a module to a new value.
 * Returns zero if out of memory.
 */
int ILModuleSetEncId(ILModule *module, const unsigned char *id);

/*
 * Get the Edit & Continue identifier of a module.
 * Returns NULL if no such identifier is registered.
 */
const unsigned char *ILModuleGetEncId(ILModule *module);

/*
 * Set the Edit & Continue base identifier of a module to a new value.
 * Returns zero if out of memory.
 */
int ILModuleSetEncBaseId(ILModule *module, const unsigned char *id);

/*
 * Get the Edit & Continue base identifier of a module.
 * Returns NULL if no such identifier is registered.
 */
const unsigned char *ILModuleGetEncBaseId(ILModule *module);

/*
 * Create a module reference structure and attach it
 * to an image.  Returns NULL if out of memory.
 */
ILModule *ILModuleRefCreate(ILImage *image, ILToken token, const char *name);

/*
 * Create a module reference with a given name if one
 * doesn't already exist with that name.  If one already
 * exists, then return that.  This should only be used
 * when building images.
 */
ILModule *ILModuleRefCreateUnique(ILImage *image, const char *name);

/*
 * Determine if a module is a reference.
 */
int ILModuleIsRef(ILModule *module);

/*
 * Convert a module into a fully resolved image.
 * Returns NULL if insufficient linkages available.
 */
ILImage *ILModuleToImage(ILModule *module);

/*
 * Helper macros for querying information about a module.
 */
#define	ILModule_FromToken(image,token)	\
				((ILModule *)ILImageTokenInfo((image), (token)))
#define	ILModule_Token(module)		(ILProgramItem_Token((module)))
#define	ILModule_Generation(module)	(ILModuleGetGeneration((module)))
#define	ILModule_Name(module)		(ILModuleGetName((module)))
#define	ILModule_MVID(module)		(ILModuleGetMVID((module)))
#define	ILModule_EncId(module)		(ILModuleGetEncId((module)))
#define	ILModule_EncBaseId(module)	(ILModuleGetEncBaseId((module)))

/*
 * Create an assembly structure and attach it to an image.
 * Returns NULL if out of memory.
 */
ILAssembly *ILAssemblyCreate(ILImage *image, ILToken token,
						     const char *name, int isRef);

/*
 * Determine if an assembly structure is a reference.
 */
int ILAssemblyIsRef(ILAssembly *assem);

/*
 * Create an assembly reference within "image" for the
 * image "fromImage".  Returns NULL if out of memory.
 */
ILAssembly *ILAssemblyCreateImport(ILImage *image, ILImage *fromImage);

/*
 * Convert an assembly into a fully resolved image.
 * Returns NULL if insufficient linkages available.
 */
ILImage *ILAssemblyToImage(ILAssembly *assem);

/*
 * Set the hash algorithm code for an assembly.
 */
void ILAssemblySetHashAlgorithm(ILAssembly *assem, ILUInt32 hashAlg);

/*
 * Get the hash algorithm code for an assembly.
 */
ILUInt32 ILAssemblyGetHashAlgorithm(ILAssembly *assem);

/*
 * Set the version number for an assembly.
 */
void ILAssemblySetVersion(ILAssembly *assem, const ILUInt16 *version);

/*
 * Set the version number for an assembly, with the components split apart.
 */
void ILAssemblySetVersionSplit(ILAssembly *assem,
							   ILUInt32 ver1, ILUInt32 ver2,
							   ILUInt32 ver3, ILUInt32 ver4);

/*
 * Get the version number for an assembly.
 */
const ILUInt16 *ILAssemblyGetVersion(ILAssembly *assem);

/*
 * Set or reset specific assembly attributes.
 */
void ILAssemblySetAttrs(ILAssembly *assem, ILUInt32 mask, ILUInt32 values);

/*
 * Set or reset specific assembly reference attributes.
 */
void ILAssemblySetRefAttrs(ILAssembly *assem, ILUInt32 mask, ILUInt32 values);

/*
 * Get the attributes associated with an assembly.
 */
ILUInt32 ILAssemblyGetAttrs(ILAssembly *assem);

/*
 * Get the reference attributes associated with an assembly.
 */
ILUInt32 ILAssemblyGetRefAttrs(ILAssembly *assem);

/*
 * Set the value of an assembly's originator key to
 * a specific value.  Returns zero if out of memory.
 */
int ILAssemblySetOriginator(ILAssembly *assem, const void *key,
						    ILUInt32 len);

/*
 * Get the originator key associated with an assembly.
 */
const void *ILAssemblyGetOriginator(ILAssembly *assem, ILUInt32 *len);

/*
 * Change the name for an assembly.
 * Returns zero if out of memory.
 */
int ILAssemblySetName(ILAssembly *assem, const char *name);

/*
 * Get the name for an assembly.
 */
const char *ILAssemblyGetName(ILAssembly *assem);

/*
 * Set the locale name for an assembly.
 * Returns zero if out of memory.
 */
int ILAssemblySetLocale(ILAssembly *assem, const char *locale);

/*
 * Get the locale name for an assembly.
 */
const char *ILAssemblyGetLocale(ILAssembly *assem);

/*
 * Set the value of an assembly's hash value to a specific value.
 * Returns zero if out of memory.
 */
int ILAssemblySetHash(ILAssembly *assem, const void *hash, ILUInt32 len);

/*
 * Get the hash value associated with an assembly.
 */
const void *ILAssemblyGetHash(ILAssembly *assem, ILUInt32 *len);

/*
 * Helper macros for querying information about an assembly.
 */
#define	ILAssembly_FromToken(image,token)	\
				((ILAssembly *)ILImageTokenInfo((image), (token)))
#define	ILAssembly_Token(assem)		(ILProgramItem_Token((assem)))
#define	ILAssembly_Name(assem)		(ILAssemblyGetName((assem)))
#define	ILAssembly_HashAlg(assem)	(ILAssemblyGetHashAlgorithm((assem)))
#define	ILAssembly_Locale(assem)	(ILAssemblyGetLocale((assem)))
#define	ILAssembly_Attrs(assem)		(ILAssemblyGetAttrs((assem)))
#define	ILAssembly_RefAttrs(assem)	(ILAssemblyGetRefAttrs((assem)))
#define	ILAssembly_HasPublicKey(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_PUBLIC_KEY) != 0)
#define	ILAssembly_IsSideBySideCompatible(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_COMPATIBILITY_MASK) \
				== IL_META_ASSEM_SIDE_BY_SIDE_COMPATIBLE)
#define	ILAssembly_IsNotAppDomainCompatible(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_COMPATIBILITY_MASK) \
				== IL_META_ASSEM_NON_SIDE_BY_SIDE_APP_DOMAIN)
#define	ILAssembly_IsNotProcessCompatible(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_COMPATIBILITY_MASK) \
				== IL_META_ASSEM_NON_SIDE_BY_SIDE_PROCESS)
#define	ILAssembly_IsNotMachineCompatible(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_COMPATIBILITY_MASK) \
				== IL_META_ASSEM_NON_SIDE_BY_SIDE_MACHINE)
#define	ILAssembly_Retargetable(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_RETARGETABLE) != 0)
#define	ILAssembly_EnableJITTracking(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_ENABLE_JIT_TRACKING) != 0)
#define	ILAssembly_DisableJITOptimizer(assem)	\
	((ILAssemblyGetAttrs((assem)) & IL_META_ASSEM_DISABLE_JIT_OPTIMIZER) != 0)
#define	ILAssembly_HasFullOriginator(assem)	\
	((ILAssemblyGetRefAttrs((assem)) & IL_META_ASSEMREF_FULL_ORIGINATOR) != 0)

/*
 * Create a class information block within a particular scope.
 * If "namespace" is not NULL, then prepend it to the name prior
 * to creating the class.  If there is already a reference for
 * the name, then use that.  Returns NULL if out of memory, or if
 * the class already exists.
 */
ILClass *ILClassCreate(ILProgramItem *scope, ILToken token, const char *name,
					   const char *nspace, ILProgramItem *parent);

/*
 * Create a reference class within a particular scope.
 * If "namespace" is not NULL, then prepend it to the name prior
 * to creating the class.
 */
ILClass *ILClassCreateRef(ILProgramItem *scope, ILToken token,
						  const char *name, const char *nspace);

/*
 * Create a wrapper class for a type (usually a TypeSpec) in the same
 * image as the type. The class has no classname.
 * Returns NULL if out of memory.
 */
ILClass *ILClassCreateWrapper(ILProgramItem *scope, ILToken token,
							  ILType *type);

/*
 * Resolve cross-image links for a class.
 */
ILClass *ILClassResolve(ILClass *info);

/*
 * Get the global scope for an image.  Returns NULL
 * if there is no module definition that is appropriate
 * to define the scope.
 */
ILProgramItem *ILClassGlobalScope(ILImage *image);

/*
 * Determine if a scope is a parent type in a nesting relationship.
 */
int ILClassIsNestingScope(ILProgramItem *scope);

/*
 * Import a class from another image and create a reference for it.
 */
ILClass *ILClassImport(ILImage *image, ILClass *info);

/*
 * Determine if a class is a reference.
 */
int ILClassIsRef(ILClass *info);

/*
 * Get the parent of a particular class.  This will cross
 * image boundaries to take linking into account.
 */
ILProgramItem *ILClassGetParent(ILClass *info);

/*
 * Get the underlying parent class of a particular class.
 * This will cross image boundaries to take linking into account.
 */
ILClass *ILClassGetUnderlyingParentClass(ILClass *info);

/*
 * Get the parent class of a particular class.  This will cross
 * image boundaries to take linking into account.
 * If the parent is a type spec the synthetic class for the
 * TypeSpec will be returned.
 */
ILClass *ILClassGetParentClass(ILClass *info);

/*
 * Set the parent of a class, if the class hasn't been
 * marked completed.
 */
void ILClassSetParent(ILClass *info, ILProgramItem *parent);

/*
 * Get the parent of a particular class, but don't cross
 * image boundaries.  A reference will be returned if
 * the image boundary may be crossed.
 */
ILClass *ILClassGetParentRef(ILClass *info);

/*
 * Get the image associated with a class.
 */
ILImage *ILClassToImage(ILClass *info);

/*
 * Get the context associated with a class.
 */
ILContext *ILClassToContext(ILClass *info);

/*
 * Get the name and namespace for a class.
 */
const char *ILClassGetName(ILClass *info);
const char *ILClassGetNamespace(ILClass *info);

/*
 * Get the synthetic type underlying a class, if any.
 * Returns NULL if no synthetic type present.
 */
ILType *ILClassGetSynType(ILClass *info);

/*
 * Get the attributes that are associated with a class.
 */
ILUInt32 ILClassGetAttrs(ILClass *info);

/*
 * Set or reset specific class attributes.
 */
void ILClassSetAttrs(ILClass *info, ILUInt32 mask, ILUInt32 values);

/*
 * Get the import scope for a class.
 */
ILProgramItem *ILClassGetScope(ILClass *info);

/*
 * Determine if a class, its parent, and its interfaces are fully
 * instantiated.  i.e. none of them are place holders.
 */
int ILClassIsValid(ILClass *info);

/*
 * Look up a class using its name within a specified scope.
 * If "namespace" is not NULL, then it will be prepended to
 * the name prior to performing the lookup.  Returns NULL
 * if the class does not exist.
 */
ILClass *ILClassLookup(ILProgramItem *scope,
					   const char *name, const char *nspace);

/*
 * Look up a class using its name within a specified scope,
 * using length-delimited names.
 */
ILClass *ILClassLookupLen(ILProgramItem *scope,
					      const char *name, int nameLen,
						  const char *nspace, int namespaceLen);

/*
 * Look up a class using its name within a specified scope,
 * using Unicode-based length-delimited names.
 */
ILClass *ILClassLookupUnicode(ILProgramItem *scope,
					          const ILUInt16 *name, int nameLen,
						  	  const ILUInt16 *nspace, int namespaceLen,
							  int ignoreCase);

/*
 * Look up a global class within any image in a context.
 * Returns NULL if not found.
 */
ILClass *ILClassLookupGlobal(ILContext *context,
							 const char *name, const char *nspace);

/*
 * Look up a global class within any image in a context,
 * using length-delimited names.
 */
ILClass *ILClassLookupGlobalLen(ILContext *context,
							    const char *name, int nameLen,
								const char *nspace, int namespaceLen);

/*
 * Look up a global class within any image in a context,
 * using Unicode-based length-delimited names.
 */
ILClass *ILClassLookupGlobalUnicode(ILContext *context,
					                const ILUInt16 *name, int nameLen,
						  	        const ILUInt16 *nspace, int namespaceLen,
							        int ignoreCase);

/*
 * Add an implements clause to a class.  Returns NULL if out of memory.
 */
ILImplements *ILClassAddImplements(ILClass *info, ILProgramItem *interface,
								   ILToken token);

/*
 * Determine if a particular class inherits from an ancestor class.
 * This only checks parent relationships, and not interface inheritance.
 */
int ILClassInheritsFrom(ILClass *info, ILClass *ancestor);

/*
 * Determine if a particular class implements another class.
 */
int ILClassImplements(ILClass *info, ILClass *interface);

/*
 * Iterate over all implements clauses for a class.
 */
ILImplements *ILClassNextImplements(ILClass *info, ILImplements *last);

/*
 * Get the class information from an implements clause.
 */
ILClass *ILImplementsGetClass(ILImplements *impl);

/*
 * Get the interface information from an implements clause.
 */
ILProgramItem *ILImplementsGetInterface(ILImplements *impl);

/*
 * Get the interface class information from an implements clause.
 * This will create and return a synthetic class if it's a generic
 * interface and cross image boundaries to take linking into account.
 */
ILClass *ILImplementsGetInterfaceClass(ILImplements *impl);

/*
 * Get the class underlying ine interface class. If the interface is not
 * generic the interface class will be returned.
 * This will cross image boundaries to take linking into account.
 */
ILClass *ILImplementsGetUnderlyingInterfaceClass(ILImplements *impl);

/*
 * Determine if "child" is directly or indirectly nested within "parent".
 */
int ILClassIsNested(ILClass *parent, ILClass *child);

/*
 * Determine if "nestedChild" has a containing class
 * that inherits from "ancestor".
 */
int ILClassIsNestedInheritsFrom(ILClass *nestedChild, ILClass *ancestor);

/*
 * Determine if "child" can be nested within "parent".
 */
int ILClassCanNest(ILClass *parent, ILClass *child);

/*
 * Get the parent class in a nesting relationship.
 */
ILClass *ILClassGetNestedParent(ILClass *info);

/*
 * Iterate over the nested children that a class has.
 */
ILNestedInfo *ILClassNextNested(ILClass *info, ILNestedInfo *last);

/*
 * Get the parent for a nesting relationship.
 */
ILClass *ILNestedInfoGetParent(ILNestedInfo *nested);

/*
 * Get the child for a nesting relationship.
 */
ILClass *ILNestedInfoGetChild(ILNestedInfo *nested);

/*
 * Determine if a class is accessible from a specific scope.
 */
int ILClassAccessible(ILClass *info, ILClass *scope);

/*
 * Determine if a class might be extended or an interface might
 * be implemented in a specific scope.
 */
int ILClassInheritable(ILClass *info, ILClass *scope);

/*
 * Iterate over the members that are associated with a class.
 */
ILMember *ILClassNextMember(ILClass *info, ILMember *last);

/*
 * Iterate over the members of a specific kind associated with a class.
 */
ILMember *ILClassNextMemberByKind(ILClass *info, ILMember *last, int kind);

/*
 * Iterate over the members of a class that match the given criteria.
 * If "kind" is zero, then match all kinds.  If "name" is NULL, then
 * match all names.  If "signature" is NULL, then match all signatures.
 * The conditions are AND'ed together.
 */
ILMember *ILClassNextMemberMatch(ILClass *info, ILMember *last, int kind,
								 const char *name, ILType *signature);

/*
 * Mark a class definition as "complete".  This is typically
 * used by compilers and assemblers to prevent further members
 * from being added when the end of the class is reached.
 */
void ILClassMarkComplete(ILClass *info);

/*
 * Determine if a class definition is "complete".
 */
int ILClassIsComplete(ILClass *info);

/*
 * Determine if a class information is a value type.
 */
int ILClassIsValueType(ILClass *info);

/*
 * Convert a class information block into a primitive type.
 * If the class doesn't correspond to a primitive type 0 is returned.
 */
ILType *ILClassToPrimitiveType(ILClass *info);

/*
 * Convert a class information block into a type, with
 * the correct class or value type qualifiers.  If the
 * class is one of the builtin value types, it will be
 * converted into a primitive type.
 */
ILType *ILClassToType(ILClass *info);

/*
 * Convert a class information block into a type, but
 * do not translate primitive types.
 */
ILType *ILClassToTypeDirect(ILClass *info);

/*
 * System type resolver for "ILClassFromType".
 */
typedef ILClass *(*ILSystemTypeResolver)(ILImage *image, void *data,
									     const char *name,
										 const char *nspace);

/*
 * Convert a type value into a class information block.
 * Returns NULL if not possible.  The "func" parameter
 * specifies a function to use to resolve types within
 * the "System" namespace.  If "func" is NULL, then the
 * default resolver is used.
 */
ILClass *ILClassFromType(ILImage *image, void *data, ILType *type,
						 ILSystemTypeResolver func);

/*
 * Default system type resolver.
 */
ILClass *ILClassResolveSystem(ILImage *image, void *data, const char *name,
							  const char *nspace);

/*
 * Get the user data associated with a class.
 */
void *ILClassGetUserData(ILClass *info);

/*
 * Set the user data associated with a class.
 */
void ILClassSetUserData(ILClass *info, void *data);

/*
 * Get the implementation of a particular interface method
 * on a class.  Returns NULL if no implementation found.
 */
ILMethod *ILClassGetMethodImpl(ILClass *info, ILMethod *method);

/*
 * Get the implementation of a particular interface method
 * where a regular implementing method is not available, but
 * we can define a proxy to call through to a non-virtual method
 * with the same name and signature.  Returns NULL if no such
 * proxy implementation was found.
 */
ILMethod *ILClassGetMethodImplForProxy(ILClass *info, ILMethod *method);

/*
 * Detach a member from its owning class.
 */
void ILClassDetachMember(ILMember *member);

/*
 * Attach a member to a new owning class.
 */
void ILClassAttachMember(ILClass *info, ILMember *member);

/*
 * Instantiate a class type that involves generic parameters.
 * Returns a synthetic class for the instantiation, or NULL
 * if insufficient memory to perform the instantiation.
 */
ILClass *ILClassInstantiate(ILImage *image, ILType *classType,
							ILType *classArgs, ILType *methodArgs);

/*
 * Determine if a namespace is valid for a context.  A namespace
 * is valid if there is at least one class with that namespace.
 */
int ILClassNamespaceIsValid(ILContext *context, const char *nspace);

/*
 * Get the underlying class from a generic class reference.
 */
ILClass *ILClassGetUnderlying(ILClass *info);

/*
 * Get the number of generic parameters for the class.
 */
ILUInt32 ILClassGetNumGenericPars(ILClass *info);

/*
 * Set the number of generic parameters for the class.
 */
void ILClassSetNumGenericPars(ILClass *info, ILUInt32 numGenericPars);

/*
 * Get the generic type parameters used to instantiate the class.
 */
ILType *ILClassGetTypeArguments(ILClass *info);

/*
 * Return the generic definition associated with the class instance.
 * NULL is this is not a class instance.
 */
ILClass *ILClassGetGenericDef(ILClass *info);

/*
 * Expand a class instance.
 */
ILClass *ILClassExpand(ILImage *image, ILClass *classInfo,
					   ILType *classArgs, ILType *methodArgs);

/*
 * Check if the instance if a generic class has has to be expanded 
 * with the members of the underlying generic class.
 */
int ILClassNeedsExpansion(ILClass *info);

/*
 * Return true is the generic class is already expanded.
 */
int ILClassIsExpanded(ILClass *info);

/*
 * Return a class instance corresponding to a class definition.
 */
ILClass *ILClassResolveToInstance(ILClass *classInfo, ILMethod *methodCaller);

/*
 * Lookup a method instance in a class instance. Return NULL if such an instance doesn't exist.
 */
ILMethod *ILClassLookupMethodInstance(ILClass *owner, const char *name,
									  ILType *signature, ILType  *methodArgs);

/*
 * Resolve a member definition to the corresponding member instance.
 */
ILMember *ILMemberResolveToInstance(ILMember *member, ILMethod *methodCaller);

/*
 * Resolve a method spec to a method instance.
 */
ILMethod *ILMethodSpecToMethod(ILMethodSpec *mspec, ILMethod *methodCaller);

/*
 * Set the method virtual ancestor and initialize the vtable.
 */
int ILMethodSetVirtualAncestor(ILMethod *method, ILMethod *virtualAncestor);

/*
 * Return the corresponding method instance, or NULL if such instance doesn't exist.
 */
ILMethod *ILMethodGetInstance(ILMethod *method, int index);

/*
 * Add a method instance to the generic method vtable.
 */
int ILMethodAddInstance(ILMethod *method, ILMethod *instance);

/*
 * Return the class type params associated with the method instance. Return NULL if this
 * isn't a generic method instance.
 */
ILType *ILMethodGetClassTypeArguments(ILMethod *method);

/*
 * Return the method type params associated with the method instance. Return NULL if this
 * isn't a generic method instance.
 */
ILType *ILMethodGetMethodTypeArguments(ILMethod *method);

/*
 * Helper macros for querying information about a class.
 */
#define	ILClass_FromToken(image,token)	\
				((ILClass *)ILImageTokenInfo((image), (token)))
#define	ILClass_Token(info)			(ILProgramItem_Token((info)))
#define	ILClass_Attrs(info)			(ILClassGetAttrs((info)))
#define	ILClass_Scope(info)			(ILClassGetScope((info)))
#define	ILClass_Name(info)			(ILClassGetName((info)))
#define	ILClass_Namespace(info)		(ILClassGetNamespace((info)))
#define	ILClass_SynType(info)		(ILClassGetSynType((info)))
#define	ILClass_Parent(info)		(ILClassGetParent((info)))
#define	ILClass_ParentClass(info)	(ILClassGetParentClass((info)))
#define	ILClass_UnderlyingParentClass(info)	\
			(ILClassGetUnderlyingParentClass((info)))
#define	ILClass_ParentRef(info)		(ILClassGetParentRef((info)))
#define	ILClass_NestedParent(info)	(ILClassGetNestedParent((info)))
#define	ILClass_UserData(info)		(ILClassGetUserData((info)))
#define	ILClass_IsPrivate(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NOT_PUBLIC)
#define	ILClass_IsPublic(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_PUBLIC)
#define	ILClass_IsNestedPublic(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_PUBLIC)
#define	ILClass_IsNestedPrivate(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_PRIVATE)
#define	ILClass_IsNestedFamily(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_FAMILY)
#define	ILClass_IsNestedAssembly(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_ASSEMBLY)
#define	ILClass_IsNestedFamAndAssem(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_FAM_AND_ASSEM)
#define	ILClass_IsNestedFamOrAssem(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_VISIBILITY_MASK) \
					== IL_META_TYPEDEF_NESTED_FAM_OR_ASSEM)
#define	ILClass_IsAutoLayout(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_LAYOUT_MASK) \
					== IL_META_TYPEDEF_AUTO_LAYOUT)
#define	ILClass_IsSequentialLayout(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_LAYOUT_MASK) \
					== IL_META_TYPEDEF_LAYOUT_SEQUENTIAL)
#define	ILClass_IsExplicitLayout(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_LAYOUT_MASK) \
					== IL_META_TYPEDEF_EXPLICIT_LAYOUT)
#define	ILClass_IsRegularClass(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) \
					== IL_META_TYPEDEF_CLASS)
#define	ILClass_IsInterface(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) \
					== IL_META_TYPEDEF_INTERFACE)
#define	ILClass_IsValueType(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) \
					== IL_META_TYPEDEF_VALUE_TYPE)
#define	ILClass_IsUnmanagedValueType(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) \
					== IL_META_TYPEDEF_UNMANAGED_VALUE_TYPE)
#define	ILClass_IsAbstract(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_ABSTRACT) != 0)
#define	ILClass_IsSealed(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_SEALED) != 0)
#define	ILClass_IsEnum(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_ENUM) != 0)
#define	ILClass_HasSpecialName(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_SPECIAL_NAME) != 0)
#define	ILClass_IsImport(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_IMPORT) != 0)
#define	ILClass_IsSerializable(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_SERIALIZABLE) != 0)
#define	ILClass_IsAnsi(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_STRING_FORMAT_MASK) \
					== IL_META_TYPEDEF_ANSI_CLASS)
#define	ILClass_IsUnicode(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_STRING_FORMAT_MASK) \
					== IL_META_TYPEDEF_UNICODE_CLASS)
#define	ILClass_IsAutoChar(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_STRING_FORMAT_MASK) \
					== IL_META_TYPEDEF_AUTO_CLASS)
#define	ILClass_IsLateInit(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_LATE_INIT) != 0)
#define	ILClass_HasRTSpecialName(info)	\
			((ILClassGetAttrs((info)) & IL_META_TYPEDEF_RT_SPECIAL_NAME) != 0)
#define	ILClass_IsGenericInstance(info)	\
			ILClassIsExpanded(info)

#define ILImplements_Class(impl)	\
			ILImplementsGetClass(impl)
#define ILImplements_Interface(impl)	\
			ILImplementsGetInterface(impl)
#define ILImplements_InterfaceClass(impl)	\
			ILImplementsGetInterfaceClass(impl)
#define ILImplements_UnderlyingInterfaceClass(impl)	\
			ILImplementsGetUnderlyingInterfaceClass(impl)

/*
 * Member kinds.
 */
#define	IL_META_MEMBERKIND_METHOD		1
#define	IL_META_MEMBERKIND_FIELD		2
#define	IL_META_MEMBERKIND_EVENT		3
#define	IL_META_MEMBERKIND_PROPERTY		4
#define	IL_META_MEMBERKIND_PINVOKE		5
#define	IL_META_MEMBERKIND_OVERRIDE		6
#define	IL_META_MEMBERKIND_REF			7

/*
 * Create a MemberRef.
 */
ILMemberRef *ILMemberRefCreate(ILProgramItem *owner, ILToken token,
							   ILUInt32 kind, const char *name,
							   ILType *signature);

/*
 * Create a reference member from a regular member.
 */
ILMember *ILMemberCreateRef(ILMember *member, ILToken token);

/*
 * Resolve a member reference.
 */
ILMember *ILMemberResolveRef(ILMember *member);

/*
 * Resolve all links associated with a member.
 */
ILMember *ILMemberResolve(ILMember *member);

/*
 * Get the class that a member resides within.
 */
ILClass *ILMemberGetOwner(ILMember *member);

/*
 * Get a member's kind.
 */
int ILMemberGetKind(ILMember *member);

/*
 * Get a member's name.
 */
const char *ILMemberGetName(ILMember *member);

/*
 * Get a member's signature.
 */
ILType *ILMemberGetSignature(ILMember *member);

/*
 * Set a member's signature.
 */
void ILMemberSetSignature(ILMember *member, ILType *signature);

/*
 * Get the attributes associated with a member.
 */
ILUInt32 ILMemberGetAttrs(ILMember *member);

/*
 * Set the attributes associated with a member.
 */
void ILMemberSetAttrs(ILMember *member, ILUInt32 mask, ILUInt32 attrs);

/*
 * Determine if a member is accessible from a specific scope.
 */
int ILMemberAccessible(ILMember *member, ILClass *scope);

/*
 * Import a member into a specific image as a reference.
 * This should only be used when building an image.
 * Returns NULL if out of memory.
 */
ILMember *ILMemberImport(ILImage *image, ILMember *member);

/*
 * Get the base "virtual" definition for an "override" member.
 */
ILMember *ILMemberGetBase(ILMember *member);

/*
 * Determine if a member is a generic instanciation.
 */
int ILMemberIsGenericInstance(ILMember *member);

/*
 * Helper macros for querying information about members.
 */
#define	ILMember_FromToken(image,token)	\
				((ILMember *)ILImageTokenInfo((image), (token)))
#define	ILMember_Token(member)		(ILProgramItem_Token((member)))
#define	ILMember_Owner(member)		(ILMemberGetOwner((ILMember *)(member)))
#define	ILMember_Name(member)		(ILMemberGetName((ILMember *)(member)))
#define	ILMember_Signature(member)	(ILMemberGetSignature((ILMember *)(member)))
#define	ILMember_Attrs(member)		(ILMemberGetAttrs((ILMember *)(member)))
#define	ILMember_IsGenericInstance(member)	\
	ILMemberIsGenericInstance((ILMember *)(member))
#define	ILMember_IsMethod(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_METHOD)
#define	ILMember_IsField(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_FIELD)
#define	ILMember_IsEvent(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_EVENT)
#define	ILMember_IsProperty(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_PROPERTY)
#define	ILMember_IsPInvoke(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_PINVOKE)
#define	ILMember_IsOverride(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_OVERRIDE)
#define	ILMember_IsRef(member)	\
	(ILMemberGetKind((ILMember *)(member)) == IL_META_MEMBERKIND_REF)

/*
 * Helper Macros for querying information about MemberRefs.
 */
#define	ILMemberRef_Token(memberRef)		(ILProgramItem_Token((memberRef)))
#define	ILMemberRef_Name(memberRef)			(ILMemberGetName((ILMember *)(memberRef)))
#define	ILMemberRef_Signature(memberRef)	(ILMemberGetSignature((ILMember *)(memberRef)))


/*
 * Create a new method and attach it to a class.
 * Returns NULL if out of memory.
 */
ILMethod *ILMethodCreate(ILClass *info, ILToken token,
						 const char *name, ILUInt32 attributes);

/*
 * Assign a new token to a method because it has just
 * been converted from a MemberRef into a MethodDef.
 * Returns zero if out of memory.
 */
int ILMethodNewToken(ILMethod *method);

/*
 * Get the implementation attributes associated with a method.
 */
ILUInt32 ILMethodGetImplAttrs(ILMethod *method);

/*
 * Set the implementation attributes associated with a method.
 */
void ILMethodSetImplAttrs(ILMethod *method, ILUInt32 mask, ILUInt32 attrs);

/*
 * Get the calling conventions associated with a method.
 */
ILUInt32 ILMethodGetCallConv(ILMethod *method);

/*
 * Set the calling conventions associated with a method.
 */
void ILMethodSetCallConv(ILMethod *method, ILUInt32 callConv);

/*
 * Get the RVA associated with a method.
 */
ILUInt32 ILMethodGetRVA(ILMethod *method);

/*
 * Set the RVA associated with a method.
 */
void ILMethodSetRVA(ILMethod *method, ILUInt32 rva);

/*
 * Iterate over the parameters associated with a method.
 */
ILParameter *ILMethodNextParam(ILMethod *method, ILParameter *last);

/*
 * Information that may be queried about a method's code.
 */
typedef struct
{
	ILUInt32		headerSize;			/* Total size of the header */
	ILUInt32		maxStack;			/* Maximum operand stack size */
	void		   *code;				/* Beginning of the code */
	ILUInt32		codeLen;			/* Length of the code */
	ILStandAloneSig	*localVarSig;		/* Local variable signature */
	ILInt32			initLocals : 1;		/* Non-zero to initialize locals */
	ILInt32			moreSections : 1;	/* Non-zero if more sections */
	ILInt32			javaLocals : 30;	/* Maximum number of Java locals */
	ILUInt32		remaining;			/* Remaining space in code section */

} ILMethodCode;

/*
 * Information that may be queried about exceptions.
 */
typedef struct _tagILException ILException;
struct _tagILException
{
	ILUInt32		flags;				/* Flags for exception block type */
	ILUInt32		tryOffset;			/* Offset of try region */
	ILUInt32		tryLength;			/* Length of try region */
	ILUInt32		handlerOffset;		/* Offset of handler region */
	ILUInt32		handlerLength;		/* Length of handler region */	
	ILUInt32		extraArg;			/* Extra argument for the block */
	ILUInt32		userData;			/* Data for users of this structure */	
	void			*ptrUserData;		/* Additional data for users of this structure */
	ILException    *next;				/* Next exception block */
};

/*
 * Get the method code block associated with a method.
 * Returns zero if the method does not have code
 * associated with it, or if the code is malformed.
 */
int ILMethodGetCode(ILMethod *method, ILMethodCode *code);

/*
 * Get a list of exception blocks for a particular method.
 * Use "ILMethodFreeExceptions" to free the exception list.
 * Returns zero if out of memory, or non-zero if OK.  If OK,
 * NULL may be returned in "*exceptions" to indicate that
 * there are no exception blocks for the method.
 */
int ILMethodGetExceptions(ILMethod *method, ILMethodCode *code,
						  ILException **exceptions);

/*
 * Free an exception list that was returned from "ILMethodGetExceptions".
 */
void ILMethodFreeExceptions(ILException *exceptions);

/*
 * Set the user data for a method.  This can be used by
 * runtime engines to store engine-specific data.
 */
void ILMethodSetUserData(ILMethod *method, void *userData);

/*
 * Get the user data for a method from its information block.
 * Returns NULL if the user data value has not yet been set.
 */
void *ILMethodGetUserData(ILMethod *method);

/*
 * Determine if a method has the correct signature for
 * an instance constructor.
 */
int ILMethodIsConstructor(ILMethod *method);

/*
 * Determine if a method has the correct signature for
 * a static constructor.
 */
int ILMethodIsStaticConstructor(ILMethod *method);

/*
 * Resolve a vararg method call site reference into a method definition.
 * Returns the method itself if not a call site.
 */
ILMethod *ILMethodResolveCallSite(ILMethod *method);

/*
 * Helper macros for querying information about a method.
 */
#define	ILMethod_FromToken(image,token)	\
				((ILMethod *)ILImageTokenInfo((image), (token)))
#define	ILMethod_Token(method)			(ILProgramItem_Token((method)))
#define	ILMethod_Owner(method)			(ILMember_Owner((method)))
#define	ILMethod_Name(method)			(ILMember_Name((method)))
#define	ILMethod_Signature(method)		(ILMember_Signature((method)))
#define	ILMethod_Attrs(method)			(ILMember_Attrs((method)))
#define	ILMethod_ImplAttrs(method)		(ILMethodGetImplAttrs((method)))
#define	ILMethod_CallConv(method)		(ILMethodGetCallConv((method)))
#define	ILMethod_RVA(method)			(ILMethodGetRVA((method)))
#define	ILMethod_UserData(method)		(ILMethodGetUserData((method)))
#define	ILMethod_IsConstructor(method)	(ILMethodIsConstructor((method)))
#define	ILMethod_IsStaticConstructor(method)	\
			(ILMethodIsStaticConstructor((method)))
#define	ILMethod_IsCompilerControlled(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_COMPILER_CONTROLLED)
#define	ILMethod_IsPrivate(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_PRIVATE)
#define	ILMethod_IsFamAndAssem(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_FAM_AND_ASSEM)
#define	ILMethod_IsFamily(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_FAMILY)
#define	ILMethod_IsFamOrAssem(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_FAM_OR_ASSEM)
#define	ILMethod_IsPublic(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_MEMBER_ACCESS_MASK) \
					== IL_META_METHODDEF_PUBLIC)
#define	ILMethod_IsStatic(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_STATIC) != 0)
#define	ILMethod_IsFinal(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & IL_META_METHODDEF_FINAL) != 0)
#define	ILMethod_IsVirtual(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & IL_META_METHODDEF_VIRTUAL) != 0)
#define	ILMethod_IsHideBySig(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_HIDE_BY_SIG) != 0)
#define	ILMethod_IsReuseSlot(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_VTABLE_LAYOUT_MASK) \
					== IL_META_METHODDEF_REUSE_SLOT)
#define	ILMethod_IsNewSlot(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_VTABLE_LAYOUT_MASK) \
					== IL_META_METHODDEF_NEW_SLOT)
#define	ILMethod_IsAbstract(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_ABSTRACT) != 0)
#define	ILMethod_HasSpecialName(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_SPECIAL_NAME) != 0)
#define	ILMethod_HasRTSpecialName(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_RT_SPECIAL_NAME) != 0)
#define	ILMethod_HasPInvokeImpl(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_PINVOKE_IMPL) != 0)
#define	ILMethod_IsUnmanagedExport(method)	\
	((ILMemberGetAttrs((ILMember *)(method)) & \
				IL_META_METHODDEF_UNMANAGED_EXPORT) != 0)
#define	ILMethod_IsIL(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_CODE_TYPE_MASK) \
					== IL_META_METHODIMPL_IL)
#define	ILMethod_IsNative(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_CODE_TYPE_MASK) \
					== IL_META_METHODIMPL_NATIVE)
#define	ILMethod_IsOptIL(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_CODE_TYPE_MASK) \
					== IL_META_METHODIMPL_OPTIL)
#define	ILMethod_IsRuntime(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_CODE_TYPE_MASK) \
					== IL_META_METHODIMPL_RUNTIME)
#define	ILMethod_IsManaged(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_MANAGED_MASK) \
					== IL_META_METHODIMPL_MANAGED)
#define	ILMethod_IsUnmanaged(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_MANAGED_MASK) \
					== IL_META_METHODIMPL_UNMANAGED)
#define	ILMethod_HasNoInlining(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_NO_INLINING) != 0)
#define	ILMethod_IsForwardRef(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_FORWARD_REF) != 0)
#define	ILMethod_IsSynchronized(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_SYNCHRONIZED) != 0)
#define	ILMethod_IsPreserveSig(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_PRESERVE_SIG) != 0)
#define	ILMethod_IsInternalCall(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_INTERNAL_CALL) != 0)
#define	ILMethod_IsJavaFPStrict(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_JAVA_FP_STRICT) != 0)
#define	ILMethod_IsJava(method)	\
	((ILMethodGetImplAttrs((method)) & IL_META_METHODIMPL_JAVA) != 0)
#define ILMethod_IsVirtualGeneric(method) \
	(((ILMethod_CallConv(method) & IL_META_CALLCONV_GENERIC) != 0) && \
	 ((ILMemberGetAttrs((ILMember *)(method)) & IL_META_METHODDEF_VIRTUAL) != 0))
/*
 * Create a new parameter and attach it to a method.
 * Returns NULL if out of memory.
 */
ILParameter *ILParameterCreate(ILMethod *method, ILToken token,
							   const char *name, ILUInt32 attributes,
							   ILUInt32 paramNum);

/*
 * Get the name associated with a parameter.
 */
const char *ILParameterGetName(ILParameter *param);

/*
 * Get the number associated with a parameter.
 */
ILUInt32 ILParameterGetNum(ILParameter *param);

/*
 * Get the attributes associated with a parameter.
 */
ILUInt32 ILParameterGetAttrs(ILParameter *param);

/*
 * Set the attributes associated with a parameter.
 */
void ILParameterSetAttrs(ILParameter *param, ILUInt32 mask, ILUInt32 attrs);

/*
 * Helper macros for querying information about a parameter.
 */
#define	ILParameter_FromToken(image,token)	\
				((ILParameter *)ILImageTokenInfo((image), (token)))
#define	ILParameter_Token(param)	(ILProgramItem_Token((param)))
#define	ILParameter_Name(param)		(ILParameterGetName((param)))
#define	ILParameter_Num(param)		(ILParameterGetNum((param)))
#define	ILParameter_Attrs(param)	(ILParameterGetAttrs((param)))
#define	ILParameter_IsIn(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_IN) != 0)
#define	ILParameter_IsOut(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_OUT) != 0)
#define	ILParameter_IsRetVal(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_RETVAL) != 0)
#define	ILParameter_IsOptional(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_OPTIONAL) != 0)
#define	ILParameter_HasDefault(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_HAS_DEFAULT) != 0)
#define	ILParameter_HasFieldMarshal(param)	\
	((ILParameterGetAttrs((param)) & IL_META_PARAMDEF_HAS_FIELD_MARSHAL) != 0)

/*
 * Create a new field and attach it to a class.
 * Returns NULL if out of memory.
 */
ILField *ILFieldCreate(ILClass *info, ILToken token,
					   const char *name, ILUInt32 attributes);

/*
 * Assign a new token to field method because it has just
 * been converted from a MemberRef into a FieldDef.
 * Returns zero if out of memory.
 */
int ILFieldNewToken(ILField *field);

/*
 * Get the field's type, without modifier prefixes.
 */
ILType *ILFieldGetType(ILField *field);

/*
 * Get the field's type, including modifier prefixes.
 */
ILType *ILFieldGetTypeWithPrefixes(ILField *field);

/*
 * Determine if a field is marked as thread-static.
 */
int ILFieldIsThreadStatic(ILField *field);

/*
 * Helper macros for querying information about a field.
 */
#define	ILField_FromToken(image,token)	\
				((ILField *)ILImageTokenInfo((image), (token)))
#define	ILField_Token(field)		(ILProgramItem_Token((field)))
#define	ILField_Owner(field)		(ILMember_Owner((field)))
#define	ILField_Name(field)			(ILMember_Name((field)))
#define	ILField_Type(field)			(ILFieldGetType((field)))
#define	ILField_Attrs(field)		(ILMember_Attrs((field)))
#define	ILField_IsCompilerControlled(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_COMPILER_CONTROLLED)
#define	ILField_IsPrivate(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_PRIVATE)
#define	ILField_IsFamAndAssem(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_FAM_AND_ASSEM)
#define	ILField_IsAssembly(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_ASSEMBLY)
#define	ILField_IsFamily(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_FAMILY)
#define	ILField_IsFamOrAssem(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_FAM_OR_ASSEM)
#define	ILField_IsPublic(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_FIELD_ACCESS_MASK) \
					== IL_META_FIELDDEF_PUBLIC)
#define	ILField_IsStatic(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_STATIC) != 0)
#define	ILField_IsInitOnly(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_INIT_ONLY) != 0)
#define	ILField_IsLiteral(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_LITERAL) != 0)
#define	ILField_IsNotSerialized(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_NOT_SERIALIZED) != 0)
#define	ILField_HasSpecialName(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_SPECIAL_NAME) != 0)
#define	ILField_HasPInvokeImpl(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_PINVOKE_IMPL) != 0)
#define	ILField_HasRTSpecialName(field)	\
	((ILMemberGetAttrs((ILMember *)(field)) & \
				IL_META_FIELDDEF_RT_SPECIAL_NAME) != 0)

/*
 * Create a new event and attach it to a class.
 * Returns NULL if out of memory.
 */
ILEvent *ILEventCreate(ILClass *info, ILToken token,
					   const char *name, ILUInt32 attributes,
					   ILClass *type);

/*
 * Get the "add on" method for an event.
 */
ILMethod *ILEventGetAddOn(ILEvent *event);

/*
 * Get the "remove on" method for an event.
 */
ILMethod *ILEventGetRemoveOn(ILEvent *event);

/*
 * Get the "fire" method for an event.
 */
ILMethod *ILEventGetFire(ILEvent *event);

/*
 * Get the "other" method for an event.
 */
ILMethod *ILEventGetOther(ILEvent *event);

/*
 * Helper macros for querying information about an event.
 */
#define	ILEvent_FromToken(image,token)	\
				((ILEvent *)ILImageTokenInfo((image), (token)))
#define	ILEvent_Token(event)		(ILProgramItem_Token((event)))
#define	ILEvent_Owner(event)		(ILMember_Owner((event)))
#define	ILEvent_Name(event)			(ILMember_Name((event)))
#define	ILEvent_Type(event)			(ILMember_Signature((event)))
#define	ILEvent_Attrs(event)		(ILMember_Attrs((event)))
#define	ILEvent_AddOn(event)		(ILEventGetAddOn((event)))
#define	ILEvent_RemoveOn(event)		(ILEventGetRemoveOn((event)))
#define	ILEvent_Fire(event)			(ILEventGetFire((event)))
#define	ILEvent_Other(event)		(ILEventGetOther((event)))
#define	ILEvent_HasSpecialName(event)	\
	((ILMemberGetAttrs((ILMember *)(event)) & \
				IL_META_EVENTDEF_SPECIAL_NAME) != 0)
#define	ILEvent_HasRTSpecialName(event)	\
	((ILMemberGetAttrs((ILMember *)(event)) & \
				IL_META_EVENTDEF_RT_SPECIAL_NAME) != 0)

/*
 * Create a new property and attach it to a class.
 * Returns NULL if out of memory.
 */
ILProperty *ILPropertyCreate(ILClass *info, ILToken token,
							 const char *name, ILUInt32 attributes,
							 ILType *signature);

/*
 * Get the "getter" method for a property.
 */
ILMethod *ILPropertyGetGetter(ILProperty *property);

/*
 * Get the "setter" method for a property.
 */
ILMethod *ILPropertyGetSetter(ILProperty *property);

/*
 * Get the "other" method for a property.
 */
ILMethod *ILPropertyGetOther(ILProperty *property);

/*
 * Helper macros for querying information about a property.
 */
#define	ILProperty_FromToken(image,token)	\
				((ILProperty *)ILImageTokenInfo((image), (token)))
#define	ILProperty_Token(property)		(ILProgramItem_Token((property)))
#define	ILProperty_Owner(property)		(ILMember_Owner((property)))
#define	ILProperty_Name(property)		(ILMember_Name((property)))
#define	ILProperty_Signature(property)	(ILMember_Signature((property)))
#define	ILProperty_Attrs(property)		(ILMember_Attrs((property)))
#define	ILProperty_Getter(property)		(ILPropertyGetGetter((property)))
#define	ILProperty_Setter(property)		(ILPropertyGetSetter((property)))
#define	ILProperty_Other(property)		(ILPropertyGetOther((property)))
#define	ILProperty_HasSpecialName(property)	\
	((ILMemberGetAttrs((ILMember *)(property)) & \
				IL_META_PROPDEF_SPECIAL_NAME) != 0)
#define	ILProperty_HasRTSpecialName(property)	\
	((ILMemberGetAttrs((ILMember *)(property)) & \
				IL_META_PROPDEF_RT_SPECIAL_NAME) != 0)

/*
 * Create a new PInvoke member and attach it to a method.
 */
ILPInvoke *ILPInvokeCreate(ILMethod *method, ILToken token,
						   ILUInt32 attributes, ILModule *module,
						   const char *aliasName);

/*
 * Create a new PInvoke member and attach it to a field.
 */
ILPInvoke *ILPInvokeFieldCreate(ILField *field, ILToken token,
						        ILUInt32 attributes, ILModule *module,
						        const char *aliasName);

/*
 * Get the method associated with a PInvoke member.
 */
ILMethod *ILPInvokeGetMethod(ILPInvoke *pinvoke);

/*
 * Get the field associated with a PInvoke member.
 */
ILField *ILPInvokeGetField(ILPInvoke *pinvoke);

/*
 * Get the module associated with a PInvoke member.
 */
ILModule *ILPInvokeGetModule(ILPInvoke *pinvoke);

/*
 * Get the alias name associated with a PInvoke member.
 */
const char *ILPInvokeGetAlias(ILPInvoke *pinvoke);

/*
 * Find the PInvoke member for a particular method.
 * Returns NULL if no PInvoke member.
 */
ILPInvoke *ILPInvokeFind(ILMethod *method);

/*
 * Find the PInvoke member for a particular field.
 * Returns NULL if no PInvoke member.
 */
ILPInvoke *ILPInvokeFindField(ILField *field);

/*
 * Determine the character set to use when marshalling this pinvoke or method.
 */
ILUInt32 ILPInvokeGetCharSet(ILPInvoke *pinvoke, ILMethod *method);

/*
 * Get the marshal conversion type for a method parameter.
 * If "param" is 0, then report about the return type.
 */
ILUInt32 ILPInvokeGetMarshalType(ILPInvoke *pinvoke, ILMethod *method,
								 unsigned long param, char **customName,
								 int *customNameLen, char **customCookie,
								 int *customCookieLen, ILType *type);

/*
 * Resolve the module information for a PInvoke declaration
 * to a pathname that can be used to load the external module
 * using "ILDynLibraryOpen".  Returns an ILMalloc'ed string,
 * or NULL if the module could not be resolved.
 */
char *ILPInvokeResolveModule(ILPInvoke *pinvoke);

/*
 * Helper macros for querying information about a PInvoke.
 */
#define	ILPInvoke_FromToken(image,token)	\
				((ILPInvoke *)ILImageTokenInfo((image), (token)))
#define	ILPInvoke_Token(pinvoke)		(ILProgramItem_Token((pinvoke)))
#define	ILPInvoke_Owner(pinvoke)		(ILMember_Owner((pinvoke)))
#define	ILPInvoke_Method(pinvoke)		(ILPInvokeGetMethod((pinvoke)))
#define	ILPInvoke_Field(pinvoke)		(ILPInvokeGetField((pinvoke)))
#define	ILPInvoke_Module(pinvoke)		(ILPInvokeGetModule((pinvoke)))
#define	ILPInvoke_Alias(pinvoke)		(ILPInvokeGetAlias((pinvoke)))
#define	ILPInvoke_Attrs(pinvoke)		(ILMember_Attrs((pinvoke)))
#define	ILPInvoke_NoMangle(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_NO_MANGLE) != 0)
#define	ILPInvoke_CharSetNotSpec(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CHAR_SET_MASK) \
					== IL_META_PINVOKE_CHAR_SET_NOT_SPEC)
#define	ILPInvoke_CharSetAnsi(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CHAR_SET_MASK) \
					== IL_META_PINVOKE_CHAR_SET_ANSI)
#define	ILPInvoke_CharSetUnicode(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CHAR_SET_MASK) \
					== IL_META_PINVOKE_CHAR_SET_UNICODE)
#define	ILPInvoke_CharSetAuto(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CHAR_SET_MASK) \
					== IL_META_PINVOKE_CHAR_SET_AUTO)
#define	ILPInvoke_OLE(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & IL_META_PINVOKE_OLE) != 0)
#define	ILPInvoke_SupportsLastError(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_SUPPORTS_LAST_ERROR) != 0)
#define	ILPInvoke_CallConvWinApi(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CALL_CONV_MASK) \
					== IL_META_PINVOKE_CALL_CONV_WINAPI)
#define	ILPInvoke_CallConvCDecl(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CALL_CONV_MASK) \
					== IL_META_PINVOKE_CALL_CONV_CDECL)
#define	ILPInvoke_CallConvStdCall(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CALL_CONV_MASK) \
					== IL_META_PINVOKE_CALL_CONV_STDCALL)
#define	ILPInvoke_CallConvThisCall(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CALL_CONV_MASK) \
					== IL_META_PINVOKE_CALL_CONV_THISCALL)
#define	ILPInvoke_CallConvFastCall(pinvoke)	\
	((ILMemberGetAttrs((ILMember *)(pinvoke)) & \
				IL_META_PINVOKE_CALL_CONV_MASK) \
					== IL_META_PINVOKE_CALL_CONV_FASTCALL)

/*
 * Create an override declaration within a class to redirect
 * "decl" to "body".  Returns NULL if out of memory.
 */
ILOverride *ILOverrideCreate(ILClass *info, ILToken token,
							 ILMethod *decl, ILMethod *body);

/*
 * Get the method declaration part of an override declaration.
 */
ILMethod *ILOverrideGetDecl(ILOverride *over);

/*
 * Get the method body part of an override declaration.
 */
ILMethod *ILOverrideGetBody(ILOverride *over);

/*
 * Get an override declaration from a method in the same class.
 * This can be used to determine if a method overrides something else.
 */
ILOverride *ILOverrideFromMethod(ILMethod *method);

/*
 * Helper macros for querying information about an override declaration.
 */
#define	ILOverride_FromToken(image,token)	\
				((ILOverride *)ILImageTokenInfo((image), (token)))
#define	ILOverride_Token(over)		(ILProgramItem_Token((over)))
#define	ILOverride_Decl(over)		(ILOverrideGetDecl((over)))
#define	ILOverride_Body(over)		(ILOverrideGetBody((over)))

/*
 * Create a new event mapping and associate it with an image.
 */
ILEventMap *ILEventMapCreate(ILImage *image, ILToken token,
							 ILClass *info, ILEvent *firstEvent);

/*
 * Get the class that is associated with an event mapping.
 */
ILClass *ILEventMapGetClass(ILEventMap *map);

/*
 * Get the first event that is associated with an event mapping.
 */
ILEvent *ILEventMapGetEvent(ILEventMap *map);

/*
 * Helper macros for querying information about an event mapping.
 */
#define	ILEventMap_FromToken(image,token)	\
				((ILEventMap *)ILImageTokenInfo((image), (token)))
#define	ILEventMap_Token(map)		(ILProgramItem_Token((map)))
#define	ILEventMap_Class(map)		(ILEventMapGetClass((map)))
#define	ILEventMap_Event(map)		(ILEventMapGetEvent((map)))

/*
 * Create a new property mapping and associate it with an image.
 */
ILPropertyMap *ILPropertyMapCreate(ILImage *image, ILToken token,
							 	   ILClass *info, ILProperty *firstProperty);

/*
 * Get the class that is associated with a property mapping.
 */
ILClass *ILPropertyMapGetClass(ILPropertyMap *map);

/*
 * Get the first event that is associated with a property mapping.
 */
ILProperty *ILPropertyMapGetProperty(ILPropertyMap *map);

/*
 * Helper macros for querying information about a property mapping.
 */
#define	ILPropertyMap_FromToken(image,token)	\
				((ILPropertyMap *)ILImageTokenInfo((image), (token)))
#define	ILPropertyMap_Token(map)	(ILProgramItem_Token((map)))
#define	ILPropertyMap_Class(map)	(ILPropertyMapGetClass((map)))
#define	ILPropertyMap_Property(map)	(ILPropertyMapGetProperty((map)))

/*
 * Create a method semantics declaration for an event or property item.
 */
ILMethodSem *ILMethodSemCreate(ILProgramItem *item, ILToken token,
							   ILUInt32 type, ILMethod *method);

/*
 * Get the event associated with a method semantics declaration.
 * Returns NULL if not associated with an event.
 */
ILEvent *ILMethodSemGetEvent(ILMethodSem *sem);

/*
 * Get the property associated with a method semantics declaration.
 * Returns NULL if not associated with a property.
 */
ILProperty *ILMethodSemGetProperty(ILMethodSem *sem);

/*
 * Get the type of a method semantics declaration.
 */
ILUInt32 ILMethodSemGetType(ILMethodSem *sem);

/*
 * Get the method associated with a method semantics declaration.
 */
ILMethod *ILMethodSemGetMethod(ILMethodSem *sem);

/*
 * Get the method associated with an item for a particular semantics type.
 */
ILMethod *ILMethodSemGetByType(ILProgramItem *item, ILUInt32 type);

/*
 * Helper macros for querying information about a method semantics declaration.
 */
#define	ILMethodSem_FromToken(image,token)	\
				((ILProperty *)ILImageTokenInfo((image), (token)))
#define	ILMethodSem_Token(sem)		(ILProgramItem_Token((sem)))
#define	ILMethodSem_Event(sem)		(ILMethodSemGetEvent((sem)))
#define	ILMethodSem_Property(sem)	(ILMethodSemGetProperty((sem)))
#define	ILMethodSem_Type(sem)		(ILMethodSemGetType((sem)))
#define	ILMethodSem_Method(sem)		(ILMethodSemGetMethod((sem)))

/*
 * Create a new OS information block and attach it to an assembly.
 */
ILOSInfo *ILOSInfoCreate(ILImage *image, ILToken token,
					     ILUInt32 identifier, ILUInt32 major,
						 ILUInt32 minor, ILAssembly *assem);

/*
 * Set the information associated with an OS information block.
 */
void ILOSInfoSetInfo(ILOSInfo *osinfo, ILUInt32 identifier,
					 ILUInt32 major, ILUInt32 minor);

/*
 * Get the information associated with an OS information block.
 */
ILUInt32 ILOSInfoGetIdentifier(ILOSInfo *osinfo);
ILUInt32 ILOSInfoGetMajor(ILOSInfo *osinfo);
ILUInt32 ILOSInfoGetMinor(ILOSInfo *osinfo);
ILAssembly *ILOSInfoGetAssembly(ILOSInfo *osinfo);

/*
 * Helper macros for querying information about an OS definition.
 */
#define	ILOSInfo_FromToken(image,token)	\
				((ILOSInfo *)ILImageTokenInfo((image), (token)))
#define	ILOSInfo_Token(osinfo)		(ILProgramItem_Token((osinfo)))
#define	ILOSInfo_Identifier(osinfo)	(ILOSInfoGetIdentifier((osinfo)))
#define	ILOSInfo_Major(osinfo)		(ILOSInfoGetMajor((osinfo)))
#define	ILOSInfo_Minor(osinfo)		(ILOSInfoGetMinor((osinfo)))
#define	ILOSInfo_Assembly(osinfo)	(ILOSInfoGetAssembly((osinfo)))

/*
 * Create a new processor information block and attach it to an assembly.
 */
ILProcessorInfo *ILProcessorInfoCreate(ILImage *image, ILToken token,
									   ILUInt32 number, ILAssembly *assem);

/*
 * Set the processor number within a processor information block.
 */
void ILProcessorInfoSetNumber(ILProcessorInfo *procinfo, ILUInt32 number);

/*
 * Get the information associated with a processor information block.
 */
ILUInt32 ILProcessorInfoGetNumber(ILProcessorInfo *procinfo);
ILAssembly *ILProcessorInfoGetAssembly(ILProcessorInfo *procinfo);

/*
 * Helper macros for querying information about a processor definition.
 */
#define	ILProcessorInfo_FromToken(image,token)	\
				((ILProcessorInfo *)ILImageTokenInfo((image), (token)))
#define	ILProcessorInfo_Token(procinfo)	 (ILProgramItem_Token((procinfo)))
#define	ILProcessorInfo_Number(procinfo) (ILProcessorInfoGetNumber((procinfo)))
#define	ILProcessorInfo_Assembly(procinfo)	\
				(ILProcessorInfoGetAssembly((procinfo)))

/*
 * Iterate over all OS information blocks associated with an assembly.
 */
ILOSInfo *ILOSInfoNext(ILAssembly *assem, ILOSInfo *osinfo);

/*
 * Iterate over all processor information blocks associated with an assembly.
 */
ILProcessorInfo *ILProcessorInfoNext(ILAssembly *assem,
								     ILProcessorInfo *procinfo);

/*
 * Create a TypeSpec token within an image.
 */
ILTypeSpec *ILTypeSpecCreate(ILImage *image, ILToken token, ILType *type);

/*
 * Import a TypeSpec token into the given image.
 * Returns spec if it is in the given image.
 */
ILTypeSpec *ILTypeSpecImport(ILImage *image, ILTypeSpec *spec);

/*
 * Get the type information associated with a TypeSpec.
 */
ILType *ILTypeSpecGetType(ILTypeSpec *spec);

/*
 * Set the class information block that corresponds to a TypeSpec.
 */
void ILTypeSpecSetClass(ILTypeSpec *spec, ILClass *classInfo);

/*
 * Get the class information block that corresponds to a TypeSpec.
 * The returned class may be in the foreign synthetic image.
 */
ILClass *ILTypeSpecGetClass(ILTypeSpec *spec);

/*
 * Get the class information block that corresponds to a TypeSpec.
 * The returned class is guarnteed to be in the same image.
 */
ILClass *ILTypeSpecGetClassRef(ILTypeSpec *spec);

/*
 * Get the wrapper class information block that correspondends to the TypeSpec.
 * The returned wrapper class is guaranteed to be in the same image.
 * The wrapper class in intended to be used as owner of MemberRefs during
 * image building.
 */
ILClass *ILTypeSpecGetClassWrapper(ILTypeSpec *spec);

/*
 * Helper macros for querying information about a TypeSpec's.
 */
#define	ILTypeSpec_FromToken(image,token)	\
				((ILTypeSpec *)ILImageTokenInfo((image), (token)))
#define	ILTypeSpec_Token(spec)	(ILProgramItem_Token((spec)))
#define	ILTypeSpec_Type(spec)	(ILTypeSpecGetType((spec)))
#define	ILTypeSpec_Class(spec)	(ILTypeSpecGetClass((spec)))
#define	ILTypeSpec_ClassWrapper(spec)	(ILTypeSpecGetClassWrapper((spec)))

/*
 * Create a stand alone signature token within an image.
 */
ILStandAloneSig *ILStandAloneSigCreate(ILImage *image, ILToken token,
									   ILType *type);

/*
 * Get the type information from a stand alone signature.
 */
ILType *ILStandAloneSigGetType(ILStandAloneSig *sig);

/*
 * Determine if a stand alone signature is a local variable signature.
 */
int ILStandAloneSigIsLocals(ILStandAloneSig *sig);

/*
 * Helper macros for querying information about stand alone signatures.
 */
#define	ILStandAloneSig_FromToken(image,token)	\
				((ILStandAloneSig *)ILImageTokenInfo((image), (token)))
#define	ILStandAloneSig_Token(sig)		(ILProgramItem_Token((sig)))
#define	ILStandAloneSig_Type(sig)		(ILStandAloneSigGetType((sig)))
#define	ILStandAloneSig_IsLocals(sig)	(ILStandAloneSigIsLocals((sig)))

/*
 * Create a constant and associate it with an image.
 */
ILConstant *ILConstantCreate(ILImage *image, ILToken token,
							 ILProgramItem *owner, ILUInt32 elemType);

/*
 * Get the owner of a constant.
 */
ILProgramItem *ILConstantGetOwner(ILConstant *constant);

/*
 * Get the element type of a constant.
 */
ILUInt32 ILConstantGetElemType(ILConstant *constant);

/*
 * Set the value of a constant.  Returns zero if out of memory.
 */
int ILConstantSetValue(ILConstant *constant, const void *blob,
					   unsigned long len);

/*
 * Get the value of a constant.  Returns NULL if no value.
 */
const void *ILConstantGetValue(ILConstant *constant, ILUInt32 *len);

/*
 * Get the constant that is associated with a program item.
 * Returns NULL if no such constant.
 */
ILConstant *ILConstantGetFromOwner(ILProgramItem *owner);

/*
 * Helper macros for querying information about constants.
 */
#define	ILConstant_FromToken(image,token)	\
				((ILConstant *)ILImageTokenInfo((image), (token)))
#define	ILConstant_Token(constant)		(ILProgramItem_Token((constant)))
#define	ILConstant_Owner(constant)		(ILConstantGetOwner((constant)))
#define	ILConstant_ElemType(constant)	(ILConstantGetElemType((constant)))

/*
 * Create a field RVA record and associate it with an image.
 */
ILFieldRVA *ILFieldRVACreate(ILImage *image, ILToken token,
							 ILField *owner, ILUInt32 rva);

/*
 * Get the owner of a field RVA record.
 */
ILField *ILFieldRVAGetOwner(ILFieldRVA *rva);

/*
 * Get the RVA associated with a field RVA record.
 */
ILUInt32 ILFieldRVAGetRVA(ILFieldRVA *rva);

/*
 * Set the RVA associated with a field RVA record.
 */
void ILFieldRVASetRVA(ILFieldRVA *rva, ILUInt32 value);

/*
 * Get the field RVA that is associated with a field.
 * Returns NULL if no such field RVA.
 */
ILFieldRVA *ILFieldRVAGetFromOwner(ILField *owner);

/*
 * Helper macros for querying information about field RVA's.
 */
#define	ILFieldRVA_FromToken(image,token)	\
				((ILFieldRVA *)ILImageTokenInfo((image), (token)))
#define	ILFieldRVA_Token(rva)	(ILProgramItem_Token((rva)))
#define	ILFieldRVA_Owner(rva)	(ILFieldRVAGetOwner((rva)))
#define	ILFieldRVA_RVA(rva)		(ILFieldRVAGetRVA((rva)))

/*
 * Create a field layout record and associate it with an image.
 */
ILFieldLayout *ILFieldLayoutCreate(ILImage *image, ILToken token,
							       ILField *owner, ILUInt32 offset);

/*
 * Get the owner of a field layout record.
 */
ILField *ILFieldLayoutGetOwner(ILFieldLayout *layout);

/*
 * Get the offset associated with a field layout record.
 */
ILUInt32 ILFieldLayoutGetOffset(ILFieldLayout *layout);

/*
 * Set the offset associated with a field layout record.
 */
void ILFieldLayoutSetOffset(ILFieldLayout *layout, ILUInt32 offset);

/*
 * Get the field layout that is associated with a field.
 * Returns NULL if no such field layout.
 */
ILFieldLayout *ILFieldLayoutGetFromOwner(ILField *owner);

/*
 * Helper macros for querying information about field layouts.
 */
#define	ILFieldLayout_FromToken(image,token)	\
				((ILFieldLayout *)ILImageTokenInfo((image), (token)))
#define	ILFieldLayout_Token(layout)		(ILProgramItem_Token((layout)))
#define	ILFieldLayout_Owner(layout)		(ILFieldLayoutGetOwner((layout)))
#define	ILFieldLayout_Offset(layout)	(ILFieldLayoutGetOffset((layout)))

/*
 * Create a field marshal record and associate it with an image.
 * Such records can be applied to both fields and parameters.
 */
ILFieldMarshal *ILFieldMarshalCreate(ILImage *image, ILToken token,
							         ILProgramItem *owner);

/*
 * Get the owner of a field marshal record.
 */
ILProgramItem *ILFieldMarshalGetOwner(ILFieldMarshal *marshal);

/*
 * Set the native type blob for a field marshal record.
 * Returns zero if out of memory.
 */
int ILFieldMarshalSetType(ILFieldMarshal *marshal, const void *blob,
					      unsigned long len);

/*
 * Get the native type block for a field marshal record.
 * Returns NULL if no value.
 */
const void *ILFieldMarshalGetType(ILFieldMarshal *marshal, ILUInt32 *len);

/*
 * Get the field marshaller that is associated with a program item.
 * Returns NULL if no such field marshaller.
 */
ILFieldMarshal *ILFieldMarshalGetFromOwner(ILProgramItem *owner);

/*
 * Helper macros for querying information about field marshal records.
 */
#define	ILFieldMarshal_FromToken(image,token)	\
				((ILFieldMarshal *)ILImageTokenInfo((image), (token)))
#define	ILFieldMarshal_Token(marshal)	(ILProgramItem_Token((marshal)))
#define	ILFieldMarshal_Owner(marshal)	(ILFieldMarshalGetOwner((marshal)))

/*
 * Create a class layout record and associate it with an image.
 */
ILClassLayout *ILClassLayoutCreate(ILImage *image, ILToken token,
							       ILClass *owner,
								   ILUInt32 packingSize,
								   ILUInt32 classSize);

/*
 * Get the owner of a class layout record.
 */
ILClass *ILClassLayoutGetOwner(ILClassLayout *layout);

/*
 * Get the packing size from a class layout record.
 */
ILUInt32 ILClassLayoutGetPackingSize(ILClassLayout *layout);

/*
 * Set the packing size for a class layout record.
 */
void ILClassLayoutSetPackingSize(ILClassLayout *layout, ILUInt32 size);

/*
 * Get the class size from a class layout record.
 */
ILUInt32 ILClassLayoutGetClassSize(ILClassLayout *layout);

/*
 * Set the class size for a class layout record.
 */
void ILClassLayoutSetClassSize(ILClassLayout *layout, ILUInt32 size);

/*
 * Get the class layout record that is associated with a class.
 * Returns NULL if no such layout record.
 */
ILClassLayout *ILClassLayoutGetFromOwner(ILClass *owner);

/*
 * Helper macros for querying information about class layout records.
 */
#define	ILClassLayout_FromToken(image,token)	\
				((ILClassLayout *)ILImageTokenInfo((image), (token)))
#define	ILClassLayout_Token(layout)		(ILProgramItem_Token((layout)))
#define	ILClassLayout_Owner(layout)		(ILClassLayoutGetOwner((layout)))
#define	ILClassLayout_PackingSize(layout)	\
				(ILClassLayoutGetPackingSize((layout)))
#define	ILClassLayout_ClassSize(layout)	\
				(ILClassLayoutGetClassSize((layout)))

/*
 * Create a security record and associate it with a particular item.
 */
ILDeclSecurity *ILDeclSecurityCreate(ILImage *image, ILToken token,
							         ILProgramItem *owner, ILUInt32 type);

/*
 * Get the owner of a security record.
 */
ILProgramItem *ILDeclSecurityGetOwner(ILDeclSecurity *security);

/*
 * Get the capability type of a security record.
 */
ILUInt32 ILDeclSecurityGetType(ILDeclSecurity *security);

/*
 * Set the security blob for a security record.
 * Returns zero if out of memory.
 */
int ILDeclSecuritySetBlob(ILDeclSecurity *security, const void *blob,
					      ILUInt32 len);

/*
 * Get the security block for a security record.
 * Returns NULL if no value.
 */
const void *ILDeclSecurityGetBlob(ILDeclSecurity *security, ILUInt32 *len);

/*
 * Get the first security record that is associated with a program item.
 * Returns NULL if no such security record.
 */
ILDeclSecurity *ILDeclSecurityGetFromOwner(ILProgramItem *owner);

/*
 * Iterate over the list of security records that are associated
 * with a program item.  If "security" is NULL, then return the
 * first security record in the list.  Otherwise return the next
 * security record in the list after "security".  Returns NULL at the
 * end of the list.
 */
ILDeclSecurity *ILProgramItemNextDeclSecurity(ILProgramItem *item,
											  ILDeclSecurity *security);

/*
 * Helper macros for querying information about security records.
 */
#define	ILDeclSecurity_FromToken(image,token)	\
				((ILDeclSecurity *)ILImageTokenInfo((image), (token)))
#define	ILDeclSecurity_Token(security)	(ILProgramItem_Token((security)))
#define	ILDeclSecurity_Owner(security)	(ILDeclSecurityGetOwner((security)))
#define	ILDeclSecurity_Type(security)	(ILDeclSecurityGetType((security)))

/*
 * Create a file declaration and associate it with an image.
 */
ILFileDecl *ILFileDeclCreate(ILImage *image, ILToken token,
							 const char *name, ILUInt32 attrs);

/*
 * Get the name associated with a file declaration.
 */
const char *ILFileDeclGetName(ILFileDecl *decl);

/*
 * Get the attributes associated with a file declaration.
 */
ILUInt32 ILFileDeclGetAttrs(ILFileDecl *decl);

/*
 * Set the hash value for a file declaration.
 * Returns zero if out of memory.
 */
int ILFileDeclSetHash(ILFileDecl *decl, const void *hash, ILUInt32 len);

/*
 * Get the hash value for a file declaration.
 * Returns NULL if no value.
 */
const void *ILFileDeclGetHash(ILFileDecl *decl, ILUInt32 *len);

/*
 * Convert a file declaration into a fully resolved image.
 * Returns NULL if insufficient linkages available.
 */
ILImage *ILFileDeclToImage(ILFileDecl *decl);

/*
 * Helper macros for querying information about file declarations.
 */
#define	ILFileDecl_FromToken(image,token)	\
				((ILFileDecl *)ILImageTokenInfo((image), (token)))
#define	ILFileDecl_Token(decl)	(ILProgramItem_Token((decl)))
#define	ILFileDecl_Name(decl)	(ILFileDeclGetName((decl)))
#define	ILFileDecl_Attrs(decl)	(ILFileDeclGetAttrs((decl)))
#define	ILFileDecl_HasMetaData(decl)	\
			((ILFileDeclGetAttrs((decl)) & \
					IL_META_FILE_CONTAINS_NO_META_DATA) == 0)
#define	ILFileDecl_IsWriteable(decl)	\
			((ILFileDeclGetAttrs((decl)) & IL_META_FILE_WRITEABLE) != 0)

/*
 * Create a manifest resource record and attach it to an image.
 */
ILManifestRes *ILManifestResCreate(ILImage *image, ILToken token,
								   const char *name, ILUInt32 attrs, int offset);

/*
 * Get the name of a manifest resource.
 */
const char *ILManifestResGetName(ILManifestRes *res);

/*
 * Set the owner of a manifest resource to a file declaration.
 */
void ILManifestResSetOwnerFile(ILManifestRes *res, ILFileDecl *decl,
							   ILUInt32 offset);

/*
 * Set the owner of a manifest resource to an assembly reference.
 */
void ILManifestResSetOwnerAssembly(ILManifestRes *res, ILAssembly *assem);

/*
 * Get the owner of a manifest resource as a file.
 * Returns NULL if the owner is not a file.
 */
ILFileDecl *ILManifestResGetOwnerFile(ILManifestRes *res);

/*
 * Get the owner of a manifest resource as an assembly reference.
 * Returns NULL if the owner is not an assembly reference.
 */
ILAssembly *ILManifestResGetOwnerAssembly(ILManifestRes *res);

/*
 * Get the file offset of a manifest resource that is owned by a file.
 */
ILUInt32 ILManifestResGetOffset(ILManifestRes *res);

/*
 * Get the attributes that are associated with a manifest resource.
 */
ILUInt32 ILManifestResGetAttrs(ILManifestRes *res);

/*
 * Helper macros for querying information about manifest resources.
 */
#define	ILManifestRes_FromToken(image,token)	\
				((ILManifestRes *)ILImageTokenInfo((image), (token)))
#define	ILManifestRes_Token(res)		(ILProgramItem_Token((res)))
#define	ILManifestRes_Name(res)			(ILManifestResGetName((res)))
#define	ILManifestRes_OwnerFile(res)	(ILManifestResGetOwnerFile((res)))
#define	ILManifestRes_OwnerAssembly(res)	\
				(ILManifestResGetOwnerAssembly((res)))
#define	ILManifestRes_Offset(res)		(ILManifestResGetOffset((res)))
#define	ILManifestRes_Attrs(res)		(ILManifestResGetAttrs((res)))
#define	ILManifestRes_IsPublic(res)	\
				((ILManifestResGetAttrs((res)) & \
						IL_META_MANIFEST_VISIBILITY_MASK) == \
					IL_META_MANIFEST_PUBLIC)
#define	ILManifestRes_IsPrivate(res)	\
				((ILManifestResGetAttrs((res)) & \
						IL_META_MANIFEST_VISIBILITY_MASK) == \
					IL_META_MANIFEST_PRIVATE)

/*
 * Create an exported type declaration within an image.
 */
ILExportedType *ILExportedTypeCreate(ILImage *image, ILToken token,
									 ILUInt32 attributes,
									 const char *name,
									 const char *nspace,
									 ILProgramItem *scope);

/*
 * Set the foreign type ID associated with an exported type declaration.
 */
void ILExportedTypeSetId(ILExportedType *type, ILUInt32 identifier);

/*
 * Get the foreign type ID associated with an exported type declaration.
 */
ILUInt32 ILExportedTypeGetId(ILExportedType *type);

/*
 * Set the scope in which an exported type can be found to a file.
 */
void ILExportedTypeSetScopeFile(ILExportedType *type, ILFileDecl *decl);

/*
 * Set the scope in which an exported type can be found to an assembly.
 */
void ILExportedTypeSetScopeAssembly(ILExportedType *type, ILAssembly *assem);

/*
 * Set the scope in which an exported type can be found to
 * another exported type.
 */
void ILExportedTypeSetScopeType(ILExportedType *type, ILExportedType *scope);

/*
 * Find an exported type from its name and namespace.
 * Returns NULL if not found.
 */
ILExportedType *ILExportedTypeFind(ILImage *image,
								   const char *name,
								   const char *nspace);

/*
 * Helper macros for querying information about exported types.
 */
#define	ILExportedType_FromToken(image,token)	\
				((ILExportedType *)ILImageTokenInfo((image), (token)))
#define	ILExportedType_Token(type)		(ILProgramItem_Token((type)))
#define	ILExportedType_Attrs(type)		(ILClassGetAttrs((ILClass *)(type)))
#define	ILExportedType_Id(type)			(ILExportedTypeGetId((type)))
#define	ILExportedType_Name(type)		(ILClassGetName((ILClass *)(type)))
#define	ILExportedType_Namespace(type)	(ILClassGetNamespace((ILClass *)(type)))
#define	ILExportedType_Scope(type)		(ILClassGetScope((ILClass *)(type)))

/*
 * Create a generic parameter record.
 */
ILGenericPar *ILGenericParCreate(ILImage *image, ILToken token,
								 ILProgramItem *owner, ILUInt32 number);

/*
 * Get the number associated with a generic parameter.
 */
ILUInt32 ILGenericParGetNumber(ILGenericPar *genPar);

/*
 * Get the flags associated with a generic parameter.
 */
ILUInt32 ILGenericParGetFlags(ILGenericPar *genPar);

/*
 * Set the flags associated with a generic parameter.
 */
void ILGenericParSetFlags(ILGenericPar *genPar, ILUInt32 mask, ILUInt32 value);

/*
 * Get the owner associated with a generic parameter.
 */
ILProgramItem *ILGenericParGetOwner(ILGenericPar *genPar);

/*
 * Get the name associated with a generic parameter.
 */
const char *ILGenericParGetName(ILGenericPar *genPar);

/*
 * Set the name associated with a generic parameter.  Returns
 * zero if out of memory.
 */
int ILGenericParSetName(ILGenericPar *genPar, const char *name);

/*
 * Get the next constraint associated with generic parameter.
 */
ILGenericConstraint *ILGenericParNextConstraint(ILGenericPar *genPar, ILGenericConstraint *last);

/*
 * Add a constraint to a generic parameter.
 */
ILGenericConstraint *ILGenericParAddConstraint(ILGenericPar *genPar,
											   ILToken token,
											   ILProgramItem *constraint);

/*
 * Get a generic parameter record for a particular owner and number.
 * Returns NULL if the record could not be found.
 */
ILGenericPar *ILGenericParGetFromOwner(ILProgramItem *owner, ILUInt32 number);

/*
 * Get the number of generic parameters that are owned by an item.
 */
ILUInt32 ILGenericParGetNumParams(ILProgramItem *owner);

/*
 * Helper macros for querying information about generic parameters.
 */
#define	ILGenericPar_FromToken(image,token)	\
				((ILGenericPar *)ILImageTokenInfo((image), (token)))
#define	ILGenericPar_Token(genPar)		(ILProgramItem_Token((genPar)))
#define	ILGenericPar_Number(genPar)		(ILGenericParGetNumber((genPar)))
#define	ILGenericPar_Flags(genPar)		(ILGenericParGetFlags((genPar)))
#define	ILGenericPar_Owner(genPar)		(ILGenericParGetOwner((genPar)))
#define	ILGenericPar_Name(genPar)		(ILGenericParGetName((genPar)))

/*
 * Create a generic constraint.
 */
ILGenericConstraint *ILConstraintCreate(ILImage *image, ILToken token,
										ILProgramItem *owner,
										ILProgramItem *classInfo);

/*
 * Return the associated Parameter to the generic constraint
 */
ILGenericPar *ILConstraintGetParam(ILGenericConstraint *constraint);

/*
 * Return the type associated specified by the generic constraint
 */
ILProgramItem *ILConstraintGetType(ILGenericConstraint *constraint);

/*
 * Helper macros for querying information about generic constraints.
 */
#define ILConstraint_Param(constraint) (ILConstraintGetParam(constraint))
#define ILConstraint_Type(constraint) (ILConstraintGetType(constraint))

/*
 * Create a method specification record.
 */
ILMethodSpec *ILMethodSpecCreate(ILImage *image, ILToken token,
								 ILMember *method, ILType *type);

/*
 * Get the method associated with a MethodSpec token.
 */
ILMember *ILMethodSpecGetMethod(ILMethodSpec *spec);

/*
 * Get the type associated with a MethodSpec token.
 */
ILType *ILMethodSpecGetType(ILMethodSpec *spec);

/*
 * Helper macros for querying information about method specifications.
 */
#define	ILMethodSpec_FromToken(image,token)	\
				((ILMethodSpec *)ILImageTokenInfo((image), (token)))
#define	ILMethodSpec_Token(spec)		(ILProgramItem_Token((spec)))
#define	ILMethodSpec_Method(spec)		(ILMethodSpecGetMethod((spec)))
#define	ILMethodSpec_Type(spec)			(ILMethodSpecGetType((spec)))

/*
 * Get the type of a Java constant pool entry for a class.
 * Returns zero if the constant index is invalid.
 */
int ILJavaGetConstType(ILClass *info, ILUInt32 index);

/*
 * Get a UTF-8 string from a Java constant pool entry for a class.
 * Returns NULL if the constant index is invalid.
 */
const char *ILJavaGetUTF8String(ILClass *info, ILUInt32 index, ILUInt32 *len);

/*
 * Get a string object from a Java constant pool entry for a class.
 * Returns NULL if the constant index is invalid.
 */
const char *ILJavaGetString(ILClass *info, ILUInt32 index, ILUInt32 *len);

/*
 * Get an integer value from a Java constant pool entry.
 * Returns zero if the constant index is invalid.
 */
int ILJavaGetInteger(ILClass *info, ILUInt32 index, ILInt32 *value);

/*
 * Get a long integer value from a Java constant pool entry.
 * Returns zero if the constant index is invalid.
 */
int ILJavaGetLong(ILClass *info, ILUInt32 index, ILInt64 *value);

/*
 * Get a float value from a Java constant pool entry.
 * Returns zero if the constant index is invalid.
 */
int ILJavaGetFloat(ILClass *info, ILUInt32 index, ILFloat *value);

/*
 * Get a double value from a Java constant pool entry.
 * Returns zero if the constant index is invalid.
 */
int ILJavaGetDouble(ILClass *info, ILUInt32 index, ILDouble *value);

/*
 * Resolve a class reference from a constant pool entry.
 * Returns NULL if the constant index is invalid, or the
 * class could not be resolved.  If the "refOnly" flag
 * is zero, then the class must be present.
 */
ILClass *ILJavaGetClass(ILClass *info, ILUInt32 index, int refOnly);

/*
 * Resolve a method reference from a constant pool entry.
 * Returns NULL if the constant index is invalid, or the
 * method could not be resolved.  If the "refOnly" flag
 * is zero, then the method must be present.  If the
 * "isStatic" field is non-zero, then the method is being
 * used with a static instruction.
 */
ILMethod *ILJavaGetMethod(ILClass *info, ILUInt32 index,
						  int refOnly, int isStatic);

/*
 * Resolve a field reference from a constant pool entry.
 * Returns NULL if the constant index is invalid, or the
 * field could not be resolved.  If the "refOnly" flag
 * is zero, then the field must be present.  If the
 * "isStatic" field is non-zero, then the field is being
 * used with a static instruction.
 */
ILField *ILJavaGetField(ILClass *info, ILUInt32 index,
						int refOnly, int isStatic);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_PROGRAM_H */
