/*
 * SymVariable.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymVariable" class.
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

public class SymVariable : ISymbolVariable
{
	// Internal state.
	private unsafe ISymUnmanagedVariable *pVariable;
	private String name;
	private int ilOffset;
	private SymScope scope;

	// Constructors.
	public unsafe SymVariable(ISymUnmanagedVariable *pVariable)
			{
				this.pVariable = pVariable;
			}
	internal SymVariable(String name, int ilOffset, SymScope scope)
			{
				this.name = name;
				this.ilOffset = ilOffset;
				this.scope = scope;
			}

	// Destructor (C++ style).
	~SymVariable() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Implement the ISymbolVariable interface.
	public virtual byte[] GetSignature()
			{
				// We don't support signatures in this implementation.
				return null;
			}
	public virtual int AddressField1 
			{
 				get
				{
					// Return the IL offset as the first address.
					return ilOffset;
				}
 			}
	public virtual int AddressField2 
			{
 				get
				{
					// Second addresses aren't needed for IL offsets.
					return 0;
				}
 			}
	public virtual int AddressField3 
			{
 				get
				{
					// Third addresses aren't needed for IL offsets.
					return 0;
				}
 			}
	public virtual SymAddressKind AddressKind 
			{
 				get
				{
					// We only support local variable offsets.
					return SymAddressKind.ILOffset;
				}
 			}
	public virtual Object Attributes 
			{
 				get
				{
					// We don't support attributes in this implementation.
					return null;
				}
 			}
	public virtual int EndOffset 
			{
 				get
				{
					return scope.EndOffset;
				}
 			}
	public virtual String Name 
			{
 				get
				{
					return name;
				}
 			}
	public virtual int StartOffset 
			{
 				get
				{
					return scope.StartOffset;
				}
 			}

}; // class SymVariable

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
