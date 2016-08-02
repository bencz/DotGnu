/*
 * XmlNodeType.cs - Implementation of the "System.Xml.XmlNodeType" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

public enum XmlNodeType
{
	None                  = 0,
	Element               = 1,
	Attribute             = 2,
	Text                  = 3,
	CDATA                 = 4,
	EntityReference       = 5,
	Entity                = 6,
	ProcessingInstruction = 7,
	Comment               = 8,
	Document              = 9,
	DocumentType          = 10,
	DocumentFragment      = 11,
	Notation              = 12,
	Whitespace            = 13,
	SignificantWhitespace = 14,
	EndElement            = 15,
	EndEntity             = 16,
	XmlDeclaration        = 17

}; // enum XmlNodeType

}; // namespace System.Xml
