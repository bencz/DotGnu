/*
 * dump_utils.c - Utilities used by the disassembler.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#ifdef	__cplusplus
extern	"C" {
#endif

void ILDAsmDumpBinaryBlob(FILE *outstream, ILImage *image,
						  const void *blob, ILUInt32 blobLen)
{
	unsigned char *ptr = (unsigned char *)blob;
	ILUInt32 offset;
	if(blobLen > 16)
	{
		/* Dump the blob on a separate line */
		fputs("\n\t\t(", outstream);
		while(blobLen > 16)
		{
			for(offset = 0; offset < 16; ++offset)
			{
				if(offset)
				{
					putc(' ', outstream);
				}
				fprintf(outstream, "%02X", ptr[offset]);
			}
			fputs("   // ", outstream);
			for(offset = 0; offset < 16; ++offset)
			{
				if(ptr[offset] >= (unsigned char)' ' &&
			   	   ptr[offset] <= (unsigned char)0x7E)
				{
					putc((int)(ptr[offset]), outstream);
				}
				else
				{
					putc('.', outstream);
				}
			}
			fputs("\n\t\t ", outstream);
			ptr += 16;
			blobLen -= 16;
		}
		for(offset = 0; offset < blobLen; ++offset)
		{
			if(offset)
			{
				putc(' ', outstream);
			}
			fprintf(outstream, "%02X", ptr[offset]);
		}
		fputs(")  // ", outstream);
		for(offset = 0; offset < blobLen; ++offset)
		{
			if(ptr[offset] >= (unsigned char)' ' &&
			   ptr[offset] <= (unsigned char)0x7E)
			{
				putc((int)(ptr[offset]), outstream);
			}
			else
			{
				putc('.', outstream);
			}
		}
	}
	else
	{
		/* Dump the blob on the same line */
		fputs(" (", outstream);
		for(offset = 0; offset < blobLen; ++offset)
		{
			if(offset)
			{
				putc(' ', outstream);
			}
			fprintf(outstream, "%02X", ptr[offset]);
		}
		fputs(")   // ", outstream);
		for(offset = 0; offset < blobLen; ++offset)
		{
			if(ptr[offset] >= (unsigned char)' ' &&
			   ptr[offset] <= (unsigned char)0x7E)
			{
				putc((int)(ptr[offset]), outstream);
			}
			else
			{
				putc('.', outstream);
			}
		}
	}
}

void ILDAsmWalkTokens(ILImage *image, FILE *outstream, int flags,
					  unsigned long tokenKind, ILDAsmWalkFunc callback,
					  unsigned long refToken)
{
	unsigned long numTokens;
	unsigned long token;
	void *data;
	numTokens = ILImageNumTokens(image, tokenKind);
	for(token = 1; token <= numTokens; ++token)
	{
		data = ILImageTokenInfo(image, tokenKind | token);
		(*callback)(image, outstream, flags, tokenKind | token, data, refToken);
	}
}

void ILDAsmDumpSecurity(ILImage *image, FILE *outstream,
						ILProgramItem *item, int flags)
{
	ILDeclSecurity *security;

	/* Get the security information, if any */
	security = 0;
	while((security = ILProgramItemNextDeclSecurity(item, security)) != 0)
	{
		const void *blob;
		ILUInt32 blobLen;

		/* Dump the security header */
		fputs("\t.permissionset ", outstream);

		/* Dump the type of security blob */
		ILDumpFlags(outstream, ILDeclSecurity_Type(security), ILSecurityFlags, 0);

		/* Dump the blob */
		blob = ILDeclSecurityGetBlob(security, &blobLen);
		if(blob)
		{
			putc('=', outstream);
			ILDAsmDumpBinaryBlob(outstream, image, blob, blobLen);
		}

		/* Terminate the line */
		putc('\n', outstream);

		/* Dump the text version of the XML within the security blob */
		if(blob)
		{
			ILUInt16 ch;

			fputs("\t// ", outstream);
			while(blobLen >= 2)
			{
				ch = IL_READ_UINT16(blob);
				if(ch == '\n')
				{
					if(blobLen >= 4)
					{
						fputs("\n\t// ", outstream);
					}
				}
				else if(ch >= ' ' && ch <= 0x7E)
				{
					putc((int)ch, outstream);
				}
				else if(ch != '\r')
				{
					fprintf(outstream, "&#x%04lX;", (unsigned long)ch);
				}
				blob = (const void *)(((const char *)blob) + 2);
				blobLen -= 2;
			}
			putc('\n', outstream);
		}
	}
}

void ILDAsmDumpGenericParams(ILImage *image, FILE *outstream,
							 ILProgramItem *item, int flags)
{
	ILUInt32 genericNum, genParFlags;
	ILGenericPar *genPar;
	ILGenericConstraint *genConstr;
	ILProgramItem *constraint;
	ILTypeSpec *spec;
	const char *name;
	
	genericNum = 0;
	genPar = ILGenericParGetFromOwner(item, genericNum);
	if(genPar)
	{
		putc('<', outstream);
		do
		{
			if(genericNum > 0)
			{
				fputs(", ", outstream);
			}
			genParFlags = ILGenericParGetFlags(genPar);
			if ((genParFlags & IL_META_GENPARAM_VARIANCE_MASK) != 0)
			{
				/* This generic parameter is a variance */
				if (genParFlags & IL_META_GENPARAM_COVARIANT)
				{
					fputs("+ ", outstream);
				}
				else if (genParFlags & IL_META_GENPARAM_CONTRAVARIANT)
				{
					fputs("- ", outstream);
				}
			}
			if ((genParFlags & IL_META_GENPARAM_SPECIAL_CONST_MASK) != 0)
			{
				/* This generic parameter has special constraints */
				if (genParFlags & IL_META_GENPARAM_CTOR_CONST)
				{
					fputs(".ctor ", outstream);
				}
				if (genParFlags & IL_META_GENPARAM_CLASS_CONST)
				{
					fputs("class ", outstream);
				}
				if (genParFlags & IL_META_GENPARAM_VALUETYPE_CONST)
				{
					fputs("valuetype ", outstream);
				}
			}
			genConstr = ILGenericParNextConstraint(genPar, 0);
			if(genConstr)
			{
				putc('(', outstream);
				while(genConstr)
				{
					constraint = ILConstraint_Type(genConstr);
					spec = ILProgramItemToTypeSpec(constraint);
					if(spec)
					{
						ILDumpType(outstream, image, ILTypeSpec_Type(spec), flags);
					}
					else
					{
						ILDumpType(outstream, image,
								   ILClassToType((ILClass *)constraint), flags);
					}
					genConstr = ILGenericParNextConstraint(genPar, genConstr);
					if(genConstr)
						fputs(", ", outstream);
				}
				putc(')', outstream);
			}
			name = ILGenericPar_Name(genPar);
			if(name)
			{
				ILDumpIdentifier(outstream, name, 0, flags);
			}
			else
			{
				fprintf(outstream, "G_%d", (int)(genericNum + 1));
			}
			++genericNum;
			genPar = ILGenericParGetFromOwner(item, genericNum);
		}
		while(genPar != 0);
		putc('>', outstream);
	}
}

#ifdef	__cplusplus
};
#endif
