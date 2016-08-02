/*
 * XmlImplementation.cs - Implementation of the
 *		"System.Xml.XmlImplementation" class.
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

using System;
using System.Globalization;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlImplementation
{
	// Internal state.
	internal XmlNameTable nameTable;

	// Constructor.
	public XmlImplementation()
			{
				nameTable = new NameTable();
			}
	internal XmlImplementation(XmlNameTable nt)
			{
				nameTable = nt;
			}

	// Create a new document.
	public virtual XmlDocument CreateDocument()
			{
				return new XmlDocument(this);
			}

	// Determine if this implementation has a specific feature.
	public bool HasFeature(String strFeature, String strVersion)
			{
			#if !ECMA_COMPAT
				if(String.Compare(strFeature, "XML",
								  true, CultureInfo.InvariantCulture) == 0 &&
			#else
				if(String.Compare(strFeature, "XML", true) == 0 &&
			#endif
				   (strVersion == "1.0" || strVersion == "2.0"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

}; // class XmlImplementation

}; // namespace System.Xml
