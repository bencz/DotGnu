/*
 * il_xml.h - Light-weight routines for reading XML documents.
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

#ifndef	_IL_XML_H
#define	_IL_XML_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * XML data read function.
 */
typedef int (*ILXMLReadFunc)(void *data, void *buffer, int len);

/*
 * Opaque XML reader control data structure.
 */
typedef struct _tagILXMLReader ILXMLReader;

/*
 * Types of XML document items that may be encountered.
 */
typedef enum
{
	ILXMLItem_EOF,
	ILXMLItem_StartTag,
	ILXMLItem_EndTag,
	ILXMLItem_SingletonTag,
	ILXMLItem_Text,

} ILXMLItem;

/*
 * Create an XML reader for processing a specific stream of data.
 * If "maxText" is non-zero, it indicates the maximum text data
 * block and element size that will be accepted.  This is useful
 * for preventing downloaded XML streams from supplying arbitrary
 * long text runs to force this library to allocate all of system
 * memory.  If "maxText" is zero, then text of any arbitrary size
 * is supported.
 */
ILXMLReader *ILXMLCreate(ILXMLReadFunc readFunc, void *data, int maxText);

/*
 * Destroy an XML reader.
 */
void ILXMLDestroy(ILXMLReader *reader);

/*
 * Read the next item from an XML data stream.
 */
ILXMLItem ILXMLReadNext(ILXMLReader *reader);

/*
 * Get the item code for the current item.
 */
ILXMLItem ILXMLGetItem(ILXMLReader *reader);

/*
 * Get the name of the current XML tag item without any
 * namespace qualifiers.  Returns NULL if not a tag item.
 */
const char *ILXMLTagName(ILXMLReader *reader);

/*
 * Get the name of the current XML tag item with a namespace
 * qualifier, if present.  Returns NULL if not a tag item.
 */
const char *ILXMLTagNameWithNS(ILXMLReader *reader);

/*
 * Determine if the current item is a start tag
 * with a specific name.
 */
int ILXMLIsStartTag(ILXMLReader *reader, const char *name);

/*
 * Determine if the current item is a start tag
 * or singleton tag with a specific name.
 */
int ILXMLIsTag(ILXMLReader *reader, const char *name);

/*
 * Get the value of a specific tag parameter.
 * Returns NULL if the parameter is not defined.
 */
const char *ILXMLGetParam(ILXMLReader *reader, const char *name);

/*
 * Get the buffer containing the current text item.
 * Returns NULL if not a text item.  It is possible
 * for long text items to be split into multiple
 * pieces, which must be retrieved separately.
 */
const char *ILXMLGetText(ILXMLReader *reader);

/*
 * Get the contents of the current element.  The
 * stream should be positioned on a start tag.  If
 * not, an empty string is returned.  The return
 * value should be free'd with ILFree.  The value
 * NULL is returned if the system is out of memory.
 * If "whiteSpace" is non-zero, then white space is
 * significant within the element.
 */
char *ILXMLGetContents(ILXMLReader *reader, int whiteSpace);

/*
 * Skip to the end of the current item, passing
 * over nested elements as necessary.  The next
 * call to "ILXMLReadNext" will get the next
 * logical item within the stream.
 */
void ILXMLSkip(ILXMLReader *reader);

/*
 * Change the whitespace handling flag.  If the flag
 * is non-zero, then whitespace is significant in
 * text items.
 */
void ILXMLWhiteSpace(ILXMLReader *reader, int flag);

/*
 * Get the size of the packed arguments for a tag.
 */
int ILXMLGetPackedSize(ILXMLReader *reader);

/*
 * Copy the packed arguments for a tag into a buffer.
 */
void ILXMLGetPacked(ILXMLReader *reader, void *buffer);

/*
 * Get the value of a parameter from within a packed
 * argument buffer.  Returns NULL if the parameter
 * is not present.
 */
const char *ILXMLGetPackedParam(void *buffer, int len, const char *name);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_XML_H */
