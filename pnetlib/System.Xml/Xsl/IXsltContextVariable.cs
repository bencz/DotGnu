/*
 * IXsltContextVariable.cs - Implementation of 
 *			"System.Xml.Xsl.IXsltContextVariable" interface.
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

#if !ECMA_COMPAT
#if CONFIG_XSL

using System;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public interface IXsltContextVariable
	{
		 Object Evaluate(XsltContext xsltContext);

		 bool IsLocal { get; }

		 bool IsParam { get; }

		 XPathResultType VariableType { get; }

	}
}//namespace
#endif // CONFIG_XSL
#endif // !ECMA_COMPAT
