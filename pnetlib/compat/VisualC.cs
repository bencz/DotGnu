/*
 * VisualC.cs - Marker classes for Microsoft Visual C++ interoperability.
 *
 * Copyright (C) 2002, 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

// These classes exist to emulate behaviour that is expected by
// programs compiled with Microsoft Visual C++.  We do not use them
// in the Portable.NET C compiler, as the ABI's are different.
// See "pnetlib/csupport" for the Portable.NET C ABI definitions.
//
// Note: just because we supply these classes doesn't mean that
// programs compiled with MSVC will actually run on the Portable.NET
// engine.  MSVC inserts calls to native Windows .dll's in most
// programs that it compiles.  Such code is not portable, which is
// why we defined an alternative ABI which is portable.

namespace Microsoft.VisualC
{

using System;

// Indicate that debug information is in a ".pdb" file, not metadata.
[AttributeUsage(AttributeTargets.All)]
public sealed class DebugInfoInPDBAttribute : Attribute {}

// Indicate that a name is decorated.
[AttributeUsage(AttributeTargets.All)]
public sealed class DecoratedNameAttribute : Attribute
{
	public DecoratedNameAttribute() {}
	public DecoratedNameAttribute(String pszDecoratedName) {}
}

// Modifier that is used to indicate C++ reference types.
public sealed class IsCXXReferenceModifier {}

// Modifier that is used to indicate constant types.
public sealed class IsConstModifier {}

// Modifier that is used to distinguish "int" and "long" on 32-bit systems.
public sealed class IsLongModifier {}

// Modifier that is used to distinguish "X" and "signed X".
public sealed class IsSignedModifier {}

// Modifier that is used to indicate volatile types.
public sealed class IsVolatileModifier {}

// Mark miscellaneous bit information.
[AttributeUsage(AttributeTargets.All)]
public sealed class MiscellaneousBitsAttribute : Attribute
{
	public int m_dwAttrs;
	public MiscellaneousBitsAttribute(int dwAttrs) { m_dwAttrs = dwAttrs; }
}

// Modifier that is used to mark types that need copy construction.
public sealed class NeedsCopyConstructorModifier {}

// Modifier that is used to mark a "char" type with no explicit
// "signed" or "unsigned" qualification.
public sealed class NoSignSpecifiedModifier {}

#if CONFIG_FRAMEWORK_1_2

// Modifier that marks a UDT return style.
public sealed class CxxUdtReturnStyleModifier {}

// HFA marker attribute.
[AttributeUsage(AttributeTargets.All)]
public sealed class HFAAttribute : Attribute
{
	public int m_dwAttrs;
	public HFAAttribute(int dwAttrs) { m_dwAttrs = dwAttrs; }
}

// Attribute that indicates that a value has copy semantics.
[AttributeUsage(AttributeTargets.All)]
public sealed class HasCopySemanticsAttribute : Attribute {}

// Modifier that indicates a boxed type.
public sealed class IsBoxedModifier {}

// Modifier that indicates a C++ pointer type.
public sealed class IsCXXPointerModifier {}

// Modifier that indicates a copy constructor.
public sealed class IsCopyCtorModifier {}

// Modifier that indicates an intrinsic.
public sealed class IsIntrinsicModifier {}

// Modifier that indicates a marshal workaround.
public sealed class IsMarshalWorkaround {}

// Indicate that an enum is native.
[AttributeUsage(AttributeTargets.All)]
public sealed class NativeEnumAttribute : Attribute {}

#endif // CONFIG_FRAMEWORK_1_2

} // namespace Microsoft.VisualC
