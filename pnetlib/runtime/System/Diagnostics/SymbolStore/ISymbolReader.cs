/*
 * ISymbolReader.cs - Implementation of 
 *				"System.Diagnostics.SymbolStore.ISymbolReader" interface.
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

#if CONFIG_EXTENDED_DIAGNOSTICS

using System;

namespace System.Diagnostics.SymbolStore
{
	public interface ISymbolReader
	{
		ISymbolDocument GetDocument(String url, Guid language, 
									Guid languageVendor, Guid documentType);
		
		ISymbolDocument[] GetDocuments();

		System.Diagnostics.SymbolStore.ISymbolVariable[] GetGlobalVariables();

		ISymbolMethod GetMethod(SymbolToken method);

		ISymbolMethod GetMethod(SymbolToken method, int version);
		
		ISymbolMethod GetMethodFromDocumentPosition(ISymbolDocument document, 
								int line, int column);

		ISymbolNamespace[] GetNamespaces();

		byte[] GetSymAttribute(SymbolToken parent, String name);

		ISymbolVariable[] GetVariables(SymbolToken parent);
		
		SymbolToken UserEntryPoint 
		{
 			get;
 		}
	}
}//namespace

#endif // CONFIG_EXTENDED_DIAGNOSTICS
