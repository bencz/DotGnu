/*
 * DllImportAttribute.cs - Implementation of the
 *			"System.Runtime.InteropServices.DllImportAttribute" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_RUNTIME_INFRA

// The ECMA spec says that "DllImportAttribute" can only be used on
// methods.  However, the underlying metadata allows fields also, and
// it is useful to be able to import global variables from shared objects.

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field,
				AllowMultiple=false, Inherited=false)]
public sealed class DllImportAttribute : Attribute
{
	// Internal state.
	private String name;

	// Constructors.
	public DllImportAttribute(String dllName)
			{
				name = dllName;
			}

	// Public fields.
	public System.Runtime.InteropServices.CallingConvention CallingConvention;
	public System.Runtime.InteropServices.CharSet CharSet;
	public String EntryPoint;
	public bool ExactSpelling;
	public bool PreserveSig;
	public bool SetLastError;
#if !ECMA_COMPAT
	public bool BestFitMapping;
	public bool ThrowOnUnmappableChar;
#endif

	// Properties.
	public String Value
			{
				get
				{
					return name;
				}
			}

}; // class DllImportAttribute

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
