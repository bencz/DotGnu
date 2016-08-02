/*
 * ISymbolWriter.cs - Implementation of 
 *				"ISymbolWriter" interface.
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
using System.Reflection;

namespace System.Diagnostics.SymbolStore
{
	public interface ISymbolWriter
	{
		void Close();

		void CloseMethod();

		void CloseNamespace();

		void CloseScope(int endOffset);

		ISymbolDocumentWriter DefineDocument(String url, Guid language, 
							Guid languageVendor, Guid documentType);

		void DefineField(SymbolToken parent, String name,
						 FieldAttributes attributes, 
						 byte[] signature, 
						 SymAddressKind addrKind, 
						 int addr1, int addr2, int addr3);

		void DefineGlobalVariable(String name, FieldAttributes attributes, 
								  byte[] signature, SymAddressKind addrKind,
								  int addr1, int addr2, int addr3);

		void DefineLocalVariable(String name, FieldAttributes attributes, 
								 byte[] signature, SymAddressKind addrKind, 
								 int addr1, int addr2, int addr3, 
								 int startOffset, int endOffset);
								 
		void DefineParameter(String name, ParameterAttributes attributes, 
							 int sequence, SymAddressKind addrKind, 
							 int addr1, int addr2, int addr3);

		void DefineSequencePoints(ISymbolDocumentWriter document, 
								  int[] offsets, int[] lines, 
								  int[] columns, int[] endLines, 
								  int[] endColumns);

		void Initialize(IntPtr emitter, String filename, bool fFullBuild);

		void OpenMethod(SymbolToken method);

		void OpenNamespace(String name);

		int OpenScope(int startOffset);

		void SetMethodSourceRange(ISymbolDocumentWriter startDoc, 
								  int startLine, int startColumn, 
								  ISymbolDocumentWriter endDoc, 
								  int endLine, int endColumn);

		void SetScopeRange(int scopeID, int startOffset, int endOffset);

		void SetSymAttribute(SymbolToken parent, String name, byte[] data);

		void SetUnderlyingWriter(IntPtr underlyingWriter);

		void SetUserEntryPoint(SymbolToken entryMethod);

		void UsingNamespace(String fullName);

	}
}//namespace

#endif // CONFIG_EXTENDED_DIAGNOSTICS
