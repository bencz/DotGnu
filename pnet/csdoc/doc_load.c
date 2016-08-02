/*
 * doc_load.c - Load an XML-ized documentation tree into memory.
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

#include "doc_tree.h"
#include "il_system.h"
#include "il_utils.h"
#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Allocate a documentation tag node.
 */
static ILDocText *AllocTagNode(ILXMLReader *reader, ILDocText *parent)
{
	int size = ILXMLGetPackedSize(reader);
	ILDocText *text = (ILDocText *)ILMalloc(sizeof(ILDocText) + size - 1);
	if(text)
	{
		text->parent = parent;
		text->next = 0;
		text->isTag = 1;
		text->size = size;
		text->children = 0;
		ILXMLGetPacked(reader, text->text);
		return text;
	}
	else
	{
		return 0;
	}
}

/*
 * Allocate a documentation text node.
 */
static ILDocText *AllocTextNode(ILXMLReader *reader, ILDocText *parent)
{
	const char *data = ILXMLGetText(reader);
	unsigned size = strlen(data);
	ILDocText *text = (ILDocText *)ILMalloc(sizeof(ILDocText) + size);
	if(text)
	{
		text->parent = parent;
		text->next = 0;
		text->isTag = 0;
		text->size = 0;
		text->children = 0;
		strcpy(text->text, data);
		return text;
	}
	else
	{
		return 0;
	}
}

/*
 * Duplicate a string after stripping white space from the
 * start and end of it.
 */
static char *DupStrippedString(const char *str)
{
	int len;
	if(!str)
	{
		return 0;
	}
	while(*str != '\0' && *str == ' ')
	{
		++str;
	}
	len = strlen(str);
	while(len > 0 && str[len - 1] == ' ')
	{
		--len;
	}
	return ILDupNString(str, len);
}

/*
 * Parse a documentation element.  The XML stream is positioned
 * on the "Docs" element itself.
 */
static int ParseDocs(ILDocText **doc, ILXMLReader *reader)
{
	ILXMLItem item;
	unsigned long depth = 0;
	ILDocText *last = 0;
	ILDocText *parent = 0;
	ILDocText *text;

	/* White space is significant within documentation */
	ILXMLWhiteSpace(reader, 1);

	/* Parse the documentation elements */
	while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF)
	{
		if(item == ILXMLItem_StartTag)
		{
			/* Allocate a tag node and go in one level */
			text = AllocTagNode(reader, parent);
			if(!text)
			{
				ILXMLWhiteSpace(reader, 0);
				return 0;
			}
			if(last)
			{
				last->next = text;
			}
			else if(parent)
			{
				parent->children = text;
			}
			else
			{
				*doc = text;
			}
			last = 0;
			parent = text;
			++depth;
		}
		else if(item == ILXMLItem_SingletonTag)
		{
			/* Allocate a tag node */
			text = AllocTagNode(reader, parent);
			if(!text)
			{
				ILXMLWhiteSpace(reader, 0);
				return 0;
			}
			if(last)
			{
				last->next = text;
			}
			else if(parent)
			{
				parent->children = text;
			}
			else
			{
				*doc = text;
			}
			last = text;
		}
		else if(item == ILXMLItem_Text)
		{
			/* Allocate a text node */
			text = AllocTextNode(reader, parent);
			if(!text)
			{
				ILXMLWhiteSpace(reader, 0);
				return 0;
			}
			if(last)
			{
				last->next = text;
			}
			else if(parent)
			{
				parent->children = text;
			}
			else
			{
				*doc = text;
			}
			last = text;
		}
		else if(item == ILXMLItem_EndTag)
		{
			/* Go out one level */
			if(!depth)
			{
				break;
			}
			--depth;
			last = parent;
			parent = parent->parent;
		}
		else
		{
			/* Don't know what this is, so skip it */
			ILXMLSkip(reader);
		}
	}

	/* White space is no longer significant */
	ILXMLWhiteSpace(reader, 0);
	return 1;
}

/*
 * Parse a set of metadata flags for a program item.
 */
static unsigned long ParseMetadataFlags(const char *value,
										const ILFlagInfo *flags)
{
	unsigned long attrs = 0;
	const ILFlagInfo *temp;
	int len;

	/* Sanity-check the string */
	if(!value)
	{
		return ILDocInvalidAttrs;
	}

	/* Parse the value, one word at a time */
	while(*value != '\0')
	{
		/* Skip white space */
		if(*value == ' ')
		{
			++value;
			continue;
		}

		/* Extract the word */
		len = 1;
		while(value[len] != '\0' && value[len] != ' ')
		{
			++len;
		}

		/* Search for the word in the flag table */
		temp = flags;
		while(temp->name != 0)
		{
			if(!strncmp(temp->name, value, len))
			{
				if(temp->name[len] == '\0')
				{
					attrs |= temp->flag;
					break;
				}
				else if((temp->name[len] == ' ') &&
						(value[len] == ' ') && 
						(strlen(temp->name) > len))
				{
					int tempLen = len + 1; /* skip the space */

					while(value[tempLen] != '\0' && value[tempLen] != ' ')
					{
						++tempLen;
					}
					if(!strncmp(temp->name, value, tempLen) &&
					   (temp->name[tempLen] == '\0'))
					{
						attrs |= temp->flag;
						len = tempLen;
						break;
					}
				}
			}
			++temp;
		}

		/* Advance to the next word */
		value += len;
	}

	/* Return the final set of attribute flags to the caller */
	return attrs;
}

/*
 * Parse a list of interfaces.  The XML stream is positioned
 * on the first item within the "Interfaces" element.
 */
static int ParseInterfaces(ILDocInterface **list, ILXMLReader *reader)
{
	ILDocInterface *last = 0;
	ILDocInterface *interface;

	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsStartTag(reader, "Interface"))
		{
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "InterfaceName"))
				{
					/* Allocate a new interface object */
					interface = (ILDocInterface *)ILMalloc
									(sizeof(ILDocInterface));
					if(!interface)
					{
						return 0;
					}
					interface->next = 0;
					if(last)
					{
						last->next = interface;
					}
					else
					{
						*list = interface;
					}
					last = interface;

					/* Copy the contents of this element to the object */
					interface->name = ILXMLGetContents(reader, 0);
					if(!(interface->name))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

/*
 * Parse a list of attributes.  The XML stream is positioned
 * on the first item within the "Attributes" element.
 */
static int ParseAttributes(ILDocAttribute **list, ILXMLReader *reader)
{
	ILDocAttribute *last = 0;
	ILDocAttribute *attr;

	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsStartTag(reader, "Attribute"))
		{
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "AttributeName"))
				{
					/* Allocate a new attribute object */
					attr = (ILDocAttribute *)ILMalloc(sizeof(ILDocAttribute));
					if(!attr)
					{
						return 0;
					}
					attr->next = 0;
					if(last)
					{
						last->next = attr;
					}
					else
					{
						*list = attr;
					}
					last = attr;

					/* Copy the contents of this element to the object */
					attr->name = ILXMLGetContents(reader, 0);
					if(!(attr->name))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

/*
 * Parse a type member.  The XML stream is positioned on
 * the "Member" element itself.
 */
static int ParseMember(ILDocTree *tree, ILDocType *type,
					   ILDocMember **last, ILXMLReader *reader)
{
	ILDocMember *member;
	const char *name;
	const char *value;
	char *contents;
	ILDocParameter *lastParam = 0;
	ILDocParameter *param;

	/* Construct the member object */
	if((member = (ILDocMember *)ILMalloc(sizeof(ILDocMember))) == 0)
	{
		return 0;
	}
	member->tree = tree;
	member->type = type;
	member->name = 0;
	member->memberType = ILDocMemberType_Unknown;
	member->fullyQualify = 0;
	member->ilasmSignature = 0;
	member->csSignature = 0;
	member->returnType = 0;
	member->libraryName = 0;
	member->memberAttrs = ILDocInvalidAttrs;
	member->parameters = 0;
	member->attributes = 0;
	member->index = 0;
	member->doc = 0;
	member->next = 0;
	if(*last)
	{
		(*last)->next = member;
	}
	else
	{
		type->members = member;
	}
	*last = member;

	/* Set the member name */
	name = ILXMLGetParam(reader, "MemberName");
	if(name)
	{
		if((member->name = DupStrippedString(name)) == 0)
		{
			return 0;
		}
	}

	/* Parse the contents of the "Member" element */
	ILXMLReadNext(reader);
	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsTag(reader, "MemberSignature"))
		{
			/* What language is the signature in? */
			name = ILXMLGetParam(reader, "Language");
			if(name && !ILStrICmp(name, "ILASM"))
			{
				/* Assembly code signature */
				value = ILXMLGetParam(reader, "Value");
				if(value && !(member->ilasmSignature))
				{
					if((member->ilasmSignature = ILDupString(value)) == 0)
					{
						return 0;
					}
				}
			}
			else if(name && !ILStrICmp(name, "C#"))
			{
				/* C# signature */
				value = ILXMLGetParam(reader, "Value");
				if(value && !(member->csSignature))
				{
					if((member->csSignature = ILDupString(value)) == 0)
					{
						return 0;
					}
				}
			}
			ILXMLSkip(reader);
		}
		else if(ILXMLIsStartTag(reader, "MemberSignture") &&
		        member->csSignature == 0)
		{
			/* This mispelled element was used in older formats */
			member->csSignature = ILXMLGetContents(reader, 0);
			if(!(member->csSignature))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "MemberType"))
		{
			/* Parse the type of member */
			contents = ILXMLGetContents(reader, 0);
			if(!contents)
			{
				return 0;
			}
			if(!ILStrICmp(contents, "Field"))
			{
				member->memberType = ILDocMemberType_Field;
				member->memberAttrs =
					ParseMetadataFlags(member->ilasmSignature,
									   ILFieldDefinitionFlags);
			}
			else if(!ILStrICmp(contents, "Method"))
			{
				member->memberType = ILDocMemberType_Method;
				member->memberAttrs =
					ParseMetadataFlags(member->ilasmSignature,
									   ILMethodDefinitionFlags);
			}
			else if(!ILStrICmp(contents, "Constructor"))
			{
				member->memberType = ILDocMemberType_Constructor;
				member->memberAttrs =
					ParseMetadataFlags(member->ilasmSignature,
									   ILMethodDefinitionFlags);
			}
			else if(!ILStrICmp(contents, "Property"))
			{
				member->memberType = ILDocMemberType_Property;
				member->memberAttrs =
					ParseMetadataFlags(member->ilasmSignature,
									   ILMethodDefinitionFlags);
			}
			else if(!ILStrICmp(contents, "Event"))
			{
				member->memberType = ILDocMemberType_Event;
				member->memberAttrs =
					ParseMetadataFlags(member->ilasmSignature,
									   ILMethodDefinitionFlags);
			}
			else
			{
				member->memberType = ILDocMemberType_Unknown;
			}
			ILFree(contents);
		}
		else if(ILXMLIsStartTag(reader, "Docs") && member->doc == 0)
		{
			/* Parse the documentation for this member */
			if(!ParseDocs(&(member->doc), reader))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "Attributes") &&
		        member->attributes == 0)
		{
			/* Parse the attributes */
			ILXMLReadNext(reader);
			if(!ParseAttributes(&(member->attributes), reader))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "ReturnValue") &&
				member->returnType == 0)
		{
			/* Parse the return type */
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "ReturnType") &&
				   member->returnType == 0)
				{
					member->returnType = ILXMLGetContents(reader, 0);
					if(!(member->returnType))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else if(ILXMLIsStartTag(reader, "Parameters"))
		{
			/* Parse the parameters */
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsTag(reader, "Parameter"))
				{
					/* Allocate the parameter object */
					param = (ILDocParameter *)ILMalloc(sizeof(ILDocParameter));
					if(!param)
					{
						return 0;
					}
					param->name = 0;
					param->type = 0;
					param->next = 0;
					if(lastParam)
					{
						lastParam->next = param;
					}
					else
					{
						member->parameters = param;
					}
					lastParam = param;

					/* Set the name and type */
					name = ILXMLGetParam(reader, "Name");
					if(name)
					{
						if((param->name = ILDupString(name)) == 0)
						{
							return 0;
						}
					}
					name = ILXMLGetParam(reader, "Type");
					if(name)
					{
						if((param->type = ILDupString(name)) == 0)
						{
							return 0;
						}
					}
				}
				ILXMLSkip(reader);
				ILXMLReadNext(reader);
			}
		}
		else if(ILXMLIsStartTag(reader, "ExcludedLibrary") ||
		        ILXMLIsStartTag(reader, "ExcludedLibraryName"))
		{
			/* Parse the library name that overrides the default */
			member->libraryName = ILXMLGetContents(reader, 0);
			if(!(member->libraryName))
			{
				return 0;
			}
		}
		else
		{
			/* Don't know what this is, so skip it */
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

/*
 * Parse the contents of a type.  The XML stream is positioned
 * on the first item within the "Type" element.
 */
static int ParseTypeContents(ILDocTree *tree, ILDocType *type,
							 ILXMLReader *reader)
{
	const char *lang;
	const char *value;
	ILDocMember *last = 0;

	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsTag(reader, "TypeSignature"))
		{
			/* What kind of signature is this? */
			lang = ILXMLGetParam(reader, "Language");
			if(lang && !ILStrICmp(lang, "ILASM"))
			{
				/* Assembly code signature */
				value = ILXMLGetParam(reader, "Value");
				if(value && !(type->ilasmSignature))
				{
					if((type->ilasmSignature = ILDupString(value)) == 0)
					{
						return 0;
					}
				}
				type->typeAttrs =
					ParseMetadataFlags(value, ILTypeDefinitionFlags);
			}
			else if(lang && !ILStrICmp(lang, "C#"))
			{
				/* C# signature */
				value = ILXMLGetParam(reader, "Value");
				if(value && !(type->csSignature))
				{
					if((type->csSignature = ILDupString(value)) == 0)
					{
						return 0;
					}
				}
			}
			ILXMLSkip(reader);
		}
		else if(ILXMLIsStartTag(reader, "Docs") && type->doc == 0)
		{
			/* Parse the documentation for this type */
			if(!ParseDocs(&(type->doc), reader))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "Base"))
		{
			/* Parse the base type */
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "BaseTypeName") &&
				   type->baseType == 0)
				{
					type->baseType = ILXMLGetContents(reader, 0);
					if(!(type->baseType))
					{
						return 0;
					}
				}
				else if(ILXMLIsStartTag(reader, "ExcludedBaseTypeName") &&
						type->excludedBaseType == 0)
				{
					/* Some classes in the ECMA specification use this for
					   a base type that is excluded in some profiles */
					type->excludedBaseType = ILXMLGetContents(reader, 0);
					if(!(type->excludedBaseType))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else if(ILXMLIsStartTag(reader, "Interfaces") && type->interfaces == 0)
		{
			/* Parse the interfaces */
			ILXMLReadNext(reader);
			if(!ParseInterfaces(&(type->interfaces), reader))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "Attributes") && type->attributes == 0)
		{
			/* Parse the attributes */
			ILXMLReadNext(reader);
			if(!ParseAttributes(&(type->attributes), reader))
			{
				return 0;
			}
		}
		else if(ILXMLIsStartTag(reader, "Members"))
		{
			/* Parse the type members */
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
				  ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "Member"))
				{
					if(!ParseMember(tree, type, &last, reader))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else if(ILXMLIsStartTag(reader, "AssemblyInfo"))
		{
			/* Parse the assembly name information */
			ILXMLReadNext(reader);
			while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
			      ILXMLGetItem(reader) != ILXMLItem_EndTag)
			{
				if(ILXMLIsStartTag(reader, "AssemblyName") &&
				   type->assembly == 0)
				{
					type->assembly = ILXMLGetContents(reader, 0);
					if(!(type->assembly))
					{
						return 0;
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
				ILXMLReadNext(reader);
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

/*
 * Get the kind associated with a type.
 */
static ILDocTypeKind GetTypeKind(char *signature)
{
	int len;
	while(*signature != '\0')
	{
		if(*signature == ' ' || *signature == '\t' ||
		   *signature == '\n' || *signature == '\r' ||
		   *signature == '\f' || *signature == '\v')
		{
			++signature;
			continue;
		}
		len = 1;
		while(signature[len] != '\0' &&
		      signature[len] != ' ' && signature[len] != '\t' &&
		      signature[len] != '\n' && signature[len] != '\r' &&
		      signature[len] != '\f' && signature[len] != '\v')
		{
			++len;
		}
		if(len == 5 && !strncmp(signature, "class", 5))
		{
			return ILDocTypeKind_Class;
		}
		else if(len == 9 && !strncmp(signature, "interface", 9))
		{
			return ILDocTypeKind_Interface;
		}
		else if(len == 6 && !strncmp(signature, "struct", 6))
		{
			return ILDocTypeKind_Struct;
		}
		else if(len == 4 && !strncmp(signature, "enum", 4))
		{
			return ILDocTypeKind_Enum;
		}
		else if(len == 8 && !strncmp(signature, "delegate", 8))
		{
			return ILDocTypeKind_Delegate;
		}
		signature += len;
	}
	return ILDocTypeKind_Class;
}

/*
 * Parse a list of types within a library.  The XML stream
 * is positioned on the first item within the "Types" element.
 */
static int ParseTypes(ILDocTree *tree, ILDocLibrary *library,
					  ILXMLReader *reader)
{
	ILDocType *last = 0;
	ILDocType *type;
	const char *name;
	const char *fullName;
	int len;
	unsigned long hash;

	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsTag(reader, "Type"))
		{
			/* Allocate memory for the type object */
			if((type = (ILDocType *)ILMalloc(sizeof(ILDocType))) == 0)
			{
				return 0;
			}
			type->tree = tree;
			type->library = library;
			type->namespace = 0;
			type->kind = ILDocTypeKind_Class;
			type->name = 0;
			type->fullName = 0;
			type->fullyQualify = 0;
			type->assembly = 0;
			type->ilasmSignature = 0;
			type->csSignature = 0;
			type->baseType = 0;
			type->excludedBaseType = 0;
			type->typeAttrs = ILDocInvalidAttrs;
			type->interfaces = 0;
			type->attributes = 0;
			type->doc = 0;
			type->members = 0;
			type->next = 0;
			type->nextNamespace = 0;
			if(last)
			{
				last->next = type;
			}
			else
			{
				library->types = type;
			}
			last = type;

			/* Get the short and full names for the type */
			name = ILXMLGetParam(reader, "Name");
			fullName = ILXMLGetParam(reader, "FullName");
			if(name && !fullName)
			{
				fullName = name;
			}
			else if(!name && fullName)
			{
				len = strlen(fullName);
				while(len > 0 && fullName[len - 1] != '.')
				{
					--len;
				}
				name = fullName + len;
			}
			if(name)
			{
				if((type->name = ILDupString(name)) == 0)
				{
					return 0;
				}
				if((type->fullName = ILDupString(fullName)) == 0)
				{
					return 0;
				}
			}

			/* Add the type to the hash table */
			hash = (ILHashString(0, type->fullName, strlen(type->fullName))
						% IL_DOC_HASH_SIZE);
			type->nextHash = tree->hash[hash];
			tree->hash[hash] = type;

			/* Parse the contents of the type */
			if(ILXMLGetItem(reader) == ILXMLItem_StartTag)
			{
				ILXMLReadNext(reader);
				if(!ParseTypeContents(tree, type, reader))
				{
					return 0;
				}
			}

			/* Determine the type kind */
			if(type->csSignature)
			{
				type->kind = GetTypeKind(type->csSignature);
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

/*
 * Parse the contents of a "Libraries" or "Packages" element.
 * The XML stream is positioned on the first item within the element.
 */
static int ParseLibraries(ILDocTree *tree, ILXMLReader *reader)
{
	ILDocLibrary *temp;
	const char *name;

	while(ILXMLGetItem(reader) != ILXMLItem_EOF &&
	      ILXMLGetItem(reader) != ILXMLItem_EndTag)
	{
		if(ILXMLIsTag(reader, "Types"))
		{
			/* Allocate the library object */
			if((temp = (ILDocLibrary *)ILMalloc(sizeof(ILDocLibrary))) == 0)
			{
				return 0;
			}
			temp->tree = tree;
			temp->name = 0;
			temp->types = 0;
			temp->next = 0;
			if(tree->lastLibrary)
			{
				tree->lastLibrary->next = temp;
			}
			else
			{
				tree->libraries = temp;
			}
			tree->lastLibrary = temp;

			/* Set the name of the library object */
			name = ILXMLGetParam(reader, "Library");
			if(!name)
			{
				/* May be an file in the older "package" format */
				name = ILXMLGetParam(reader, "package");
			}
			if(name)
			{
				temp->name = ILDupString(name);
				if(!(temp->name))
				{
					return 0;
				}
			}

			/* Parse the contents of the library */
			if(ILXMLGetItem(reader) == ILXMLItem_StartTag)
			{
				ILXMLReadNext(reader);
				if(!ParseTypes(tree, temp, reader))
				{
					return 0;
				}
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
		ILXMLReadNext(reader);
	}
	return 1;
}

ILDocTree *ILDocTreeCreate()
{
	ILDocTree *tree;
	int hash;
	if((tree = (ILDocTree *)ILMalloc(sizeof(ILDocTree))) == 0)
	{
		return 0;
	}
	tree->libraries = 0;
	tree->lastLibrary = 0;
	tree->namespaces = 0;
	for(hash = 0; hash < IL_DOC_HASH_SIZE; ++hash)
	{
		tree->hash[hash] = 0;
	}
	return tree;
}

int ILDocTreeLoad(ILDocTree *tree, ILXMLReader *reader)
{
	/* Look for top-level "Libraries" or "Packages" (old format) elements */
	while(ILXMLReadNext(reader) != ILXMLItem_EOF)
	{
		if((ILXMLIsTag(reader, "Libraries") ||
		    ILXMLIsTag(reader, "Packages")) &&
		   ILXMLGetItem(reader) == ILXMLItem_StartTag)
		{
			ILXMLReadNext(reader);
			if(!ParseLibraries(tree, reader))
			{
				/* We ran out of memory at some point */
				return 0;
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
	}
	return 1;
}

ILDocType *ILDocTypeFind(ILDocTree *tree, const char *name)
{
	unsigned long hash;
	ILDocType *type;
	hash = (ILHashString(0, name, strlen(name)) % IL_DOC_HASH_SIZE);
	type = tree->hash[hash];
	while(type != 0)
	{
		if(!strcmp(type->fullName, name))
		{
			return type;
		}
		type = type->nextHash;
	}
	return 0;
}

const char *ILDocTextGetParam(ILDocText *text, const char *name)
{
	if(text && text->isTag)
	{
		return ILXMLGetPackedParam(text->text, text->size, name);
	}
	else
	{
		return 0;
	}
}

ILDocText *ILDocTextFirstChild(ILDocText *text, const char *name)
{
	ILDocText *child;
	if(!text)
	{
		return 0;
	}
	child = text;
	while(child != 0)
	{
		if(child->isTag && !strcmp(child->text, name))
		{
			return child;
		}
		child = child->next;
	}
	return 0;
}

ILDocText *ILDocTextGetChild(ILDocText *text, const char *name)
{
	ILDocText *child;
	if(!text || !(text->isTag))
	{
		return 0;
	}
	child = text->children;
	while(child != 0)
	{
		if(child->isTag && !strcmp(child->text, name))
		{
			return child;
		}
		child = child->next;
	}
	return 0;
}

ILDocText *ILDocTextNextChild(ILDocText *text, const char *name)
{
	ILDocText *child = text->next;
	while(child != 0)
	{
		if(child->isTag && !strcmp(child->text, name))
		{
			return child;
		}
		child = child->next;
	}
	return 0;
}

int ILDocTextAllSpaces(ILDocText *text)
{
	if(!(text->isTag))
	{
		const char *temp = text->text;
		while(*temp != '\0')
		{
			if(*temp != ' ' && *temp != '\t' &&
			   *temp != '\r' && *temp != '\n' &&
			   *temp != '\f' && *temp != '\v')
			{
				return 0;
			}
			++temp;
		}
		return 1;
	}
	else
	{
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
