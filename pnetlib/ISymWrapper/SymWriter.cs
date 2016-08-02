/*
 * SymWriter.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymWriter" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics.SymbolStore
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.Reflection;

// We don't support symbol file writing in this implementation.

public class SymWriter : ISymbolWriter
{
	// Constructors.
	public SymWriter() {}
	public SymWriter(bool noUnderlyingWriter) {}

	// Destructor (C++ style).
	~SymWriter() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Implement the ISymbolWriter interface.
	public virtual void Close()
			{
				// Nothing to do here.
			}
	public virtual void CloseMethod()
			{
				// Nothing to do here.
			}
	public virtual void CloseNamespace()
			{
				// Nothing to do here.
			}
	public virtual void CloseScope(int endOffset)
			{
				// Nothing to do here.
			}
	public virtual ISymbolDocumentWriter DefineDocument
				(String url, Guid language, Guid languageVendor,
				 Guid documentType)
			{
				// Nothing to do here.
				return new SymDocumentWriter();
			}
	public virtual void DefineField
				(SymbolToken parent, String name,
				 FieldAttributes attributes, 
				 byte[] signature, 
				 SymAddressKind addrKind, 
				 int addr1, int addr2, int addr3)
			{
				throw new NotSupportedException();
			}
	public virtual void DefineGlobalVariable
				(String name, FieldAttributes attributes, 
				 byte[] signature, SymAddressKind addrKind,
				 int addr1, int addr2, int addr3)
			{
				throw new NotSupportedException();
			}
	public virtual void DefineLocalVariable
				(String name, FieldAttributes attributes, 
				 byte[] signature, SymAddressKind addrKind, 
				 int addr1, int addr2, int addr3, 
				 int startOffset, int endOffset)
			{
				// Nothing to do here.
			}
	public virtual void DefineParameter
				(String name, ParameterAttributes attributes, 
				 int sequence, SymAddressKind addrKind, 
				 int addr1, int addr2, int addr3)
			{
				throw new NotSupportedException();
			}
	public virtual void DefineSequencePoints
				(ISymbolDocumentWriter document, 
				 int[] offsets, int[] lines, 
				 int[] columns, int[] endLines, 
				 int[] endColumns)
			{
				// Nothing to do here.
			}
	public virtual void Initialize
				(IntPtr emitter, String filename, bool fFullBuild)
			{
				// Nothing to do here.
			}
	public virtual void OpenMethod(SymbolToken method)
			{
				// Nothing to do here.
			}
	public virtual void OpenNamespace(String name)
			{
				// Nothing to do here.
			}
	public virtual int OpenScope(int startOffset)
			{
				// Nothing to do here.
				return 0;
			}
	public virtual void SetMethodSourceRange
				(ISymbolDocumentWriter startDoc, 
				 int startLine, int startColumn, 
				 ISymbolDocumentWriter endDoc, 
				 int endLine, int endColumn)
			{
				throw new NotSupportedException();
			}
	public virtual void SetScopeRange
				(int scopeID, int startOffset, int endOffset)
			{
				// Nothing to do here.
			}
	public virtual void SetSymAttribute
				(SymbolToken parent, String name, byte[] data)
			{
				// Nothing to do here.
			}
	public virtual void SetUnderlyingWriter(IntPtr underlyingWriter)
			{
				// Nothing to do here.
			}
	public virtual void SetUserEntryPoint(SymbolToken entryMethod)
			{
				// Nothing to do here.
			}
	public virtual void UsingNamespace(String fullName)
			{
				// Nothing to do here.
			}

}; // class SymWriter

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
