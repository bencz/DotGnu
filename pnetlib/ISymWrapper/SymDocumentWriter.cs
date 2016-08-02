/*
 * SymDocumentWriter.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymDocumentWriter" class.
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

public class SymDocumentWriter : ISymbolDocumentWriter
{
	// Internal state.
	private unsafe ISymUnmanagedDocumentWriter *pDocumentWriter;

	// Constructors.
	public unsafe SymDocumentWriter
				(ISymUnmanagedDocumentWriter *pDocumentWriter)
			{
				this.pDocumentWriter = pDocumentWriter;
			}
	internal SymDocumentWriter() {}

	// Destructor (C++ style).
	~SymDocumentWriter() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Get the unmanaged version of this document.
	public unsafe ISymUnmanagedDocumentWriter *GetUnmanaged()
			{
				return pDocumentWriter;
			}

	// Implement the ISymbolDocumentWriter interface.
	public virtual void SetCheckSum(Guid algorithmId, byte[] checkSum)
			{
				throw new NotSupportedException();
			}
	public virtual void SetSource(byte[] source)
			{
				throw new NotSupportedException();
			}

}; // class SymDocumentWriter

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
