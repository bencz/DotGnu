/*
 * XPathExpression.cs - Implementation of "System.Xml.XPath.XPathExpression" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V 
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

using System;
using System.Xml;
using System.Collections;

#if CONFIG_XPATH

namespace System.Xml.XPath
{
#if ECMA_COMPAT
internal
#else
public
#endif
abstract class XPathExpression
{
	public abstract void AddSort(Object expr, IComparer comparer);

	public abstract void AddSort(Object expr, XmlSortOrder order, 
								XmlCaseOrder caseOrder, 
								String lang, XmlDataType dataType);
	
	public abstract XPathExpression Clone();

	public abstract void SetContext(XmlNamespaceManager nsManager);

	public abstract String Expression
	{
		get;
	}

	public abstract XPathResultType ReturnType
	{
		get;
	}

}
}//namespace

#endif // CONFIG_XPATH
