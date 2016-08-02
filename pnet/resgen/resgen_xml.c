/*
 * resgen_xml.c - XML resource loading and writing routines.
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

#include <stdio.h>
#include "resgen.h"
#include "il_system.h"
#include "il_utils.h"
#include "il_xml.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * XML reader function.
 */
static int xmlRead(void *data, void *buffer, int len)
{
	if(!feof((FILE *)data))
	{
		return fread(buffer, 1, len, (FILE *)data);
	}
	else
	{
		return 0;
	}
}

int ILResLoadXML(const char *filename, FILE *stream)
{
	ILXMLReader *reader;
	ILXMLItem item;
	int sawRoot;
	const char *name;
	char *nameCopy;
	char *value;
	int error;

	/* Initialize the XML reader */
	reader = ILXMLCreate(xmlRead, stream, 0);
	if(!reader)
	{
		ILResOutOfMemory();
	}

	/* Look for a top-level "root" element */
	sawRoot = 0;
	error = 0;
	while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF)
	{
		if(ILXMLIsStartTag(reader, "root"))
		{
			/* Look for "data" elements */
			sawRoot = 1;
			while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF &&
			      item != ILXMLItem_EndTag)
			{
				if(ILXMLIsTag(reader, "data"))
				{
					name = ILXMLGetParam(reader, "name");
					if(name)
					{
						/* Make a copy of the name for later */
						nameCopy = ILDupString(name);
						if(!nameCopy)
						{
							ILResOutOfMemory();
						}

						/* Look for the first "value" element */
						value = 0;
						if(item == ILXMLItem_StartTag)
						{
							while((item = ILXMLReadNext(reader))
										!= ILXMLItem_EOF &&
								  item != ILXMLItem_EndTag)
							{
								if(ILXMLIsStartTag(reader, "value") && !value)
								{
									value = ILXMLGetContents(reader, 1);
									if(!value)
									{
										ILResOutOfMemory();
									}
								}
								else
								{
									ILXMLSkip(reader);
								}
							}
						}

						/* Add the string to the hash table */
						if(value)
						{
							error |= ILResAddResource(filename, -1,
												 nameCopy, strlen(nameCopy),
												 value, strlen(value));
							ILFree(value);
						}
						else
						{
							error |= ILResAddResource(filename, -1,
												 nameCopy, strlen(nameCopy),
												 "", 0);
						}
						ILFree(nameCopy);
					}
					else
					{
						ILXMLSkip(reader);
					}
				}
				else
				{
					ILXMLSkip(reader);
				}
			}
		}
		else
		{
			ILXMLSkip(reader);
		}
	}

	/* Done */
	ILXMLDestroy(reader);
	if(!sawRoot)
	{
		fprintf(stderr, "%s: not an XML resource file\n", filename);
		return 1;
	}
	else
	{
		return error;
	}
}

/*
 * Header and footer material for string resources in the XML
 * format.  We output this information to keep other tools happy.
 * We don't use it ourselves.
 */
static char const xmlHeader[] =
"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
"<root>\n"
"  <xsd:schema id=\"root\" targetNamespace=\"\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">\n"
"    <xsd:element name=\"root\" msdata:IsDataSet=\"true\">\n"
"      <xsd:complexType>\n"
"        <xsd:choice maxOccurs=\"unbounded\">\n"
"          <xsd:element name=\"data\">\n"
"            <xsd:complexType>\n"
"              <xsd:sequence>\n"
"                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\"/>\n"
"                <xsd:element name=\"comment\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"2\"/>\n"
"              </xsd:sequence>\n"
"              <xsd:attribute name=\"name\" type=\"xsd:string\"/>\n"
"              <xsd:attribute name=\"type\" type=\"xsd:string\"/>\n"
"              <xsd:attribute name=\"mimetype\" type=\"xsd:string\"/>\n"
"            </xsd:complexType>\n"
"          </xsd:element>\n"
"          <xsd:element name=\"resheader\">\n"
"            <xsd:complexType>\n"
"              <xsd:sequence>\n"
"                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\"/>\n"
"              </xsd:sequence>\n"
"              <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\"/>\n"
"            </xsd:complexType>\n"
"          </xsd:element>\n"
"        </xsd:choice>\n"
"      </xsd:complexType>\n"
"    </xsd:element>\n"
"  </xsd:schema>\n";

static char const xmlFooter[] =
"  <resheader name=\"ResMimeType\">\n"
"    <value>text/microsoft-resx</value>\n"
"  </resheader>\n"
"  <resheader name=\"Version\">\n"
"    <value>1.0.0.0</value>\n"
"  </resheader>\n"
"  <resheader name=\"Reader\">\n"
"    <value>System.Resources.ResXResourceReader</value>\n"
"  </resheader>\n"
"  <resheader name=\"Writer\">\n"
"    <value>System.Resources.ResXResourceWriter</value>\n"
"  </resheader>\n"
"</root>\n";

/*
 * Write an XML-encoded string to an output stream.
 */
static void writeXMLString(FILE *stream, const char *str, int len)
{
	while(len > 0)
	{
		if(*str == '<')
		{
			fputs("&lt;", stream);
		}
		else if(*str == '>')
		{
			fputs("&gt;", stream);
		}
		else if(*str == '&')
		{
			fputs("&amp;", stream);
		}
		else if(*str == '"')
		{
			fputs("&quot;", stream);
		}
		else if(*str == '\'')
		{
			fputs("&apos;", stream);
		}
		else
		{
			putc(*str, stream);
		}
		++str;
		--len;
	}
}

void ILResWriteXML(FILE *stream)
{
	int hash;
	ILResHashEntry *entry;

	/* Write out the XML header and schema definition */
	fputs(xmlHeader, stream);

	/* Output the strings */
	for(hash = 0; hash < IL_RES_HASH_TABLE_SIZE; ++hash)
	{
		entry = ILResHashTable[hash];
		while(entry != 0)
		{
			fputs("  <data name=\"", stream);
			writeXMLString(stream, entry->data, entry->nameLen);
			fputs("\">\n    <value>", stream);
			writeXMLString(stream, entry->data + entry->nameLen,
						   entry->valueLen);
			fputs("</value>\n  </data>\n", stream);
			entry = entry->next;
		}
	}

	/* Write out the XML footer */
	fputs(xmlFooter, stream);
}

#ifdef	__cplusplus
};
#endif
