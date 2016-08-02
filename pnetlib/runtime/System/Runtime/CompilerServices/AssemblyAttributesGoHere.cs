/*
 * AssemblyAttributesGoHere.cs - Implementation of the
 *	"System.Runtime.CompilerServices.AssemblyAttributesGoHere" class.
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

namespace System.Runtime.CompilerServices
{

#if !ECMA_COMPAT

// Technically, these classes should not be public.  However, Microsoft's
// assembler has been known to output references to them even though they
// aren't exported from Microsoft's own "mscorlib.dll".  We provide these
// only so that third party applications will load under our system.
//
// They are marked as "NonStandardExtra" so that the csdocvalil tool won't
// complain about them being present.

[NonStandardExtra] public sealed class AssemblyAttributesGoHere {}
[NonStandardExtra] public sealed class AssemblyAttributesGoHereM {}
[NonStandardExtra] public sealed class AssemblyAttributesGoHereS {}
[NonStandardExtra] public sealed class AssemblyAttributesGoHereSM {}

#endif // !ECMA_COMPAT

}; // namespace System.Runtime.CompilerServices
