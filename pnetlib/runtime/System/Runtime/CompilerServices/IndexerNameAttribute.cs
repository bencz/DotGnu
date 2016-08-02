/*
 * IndexerNameAttribute.cs - Implementation of the
 *		"System.Runtime.CompilerServices.IndexerNameAttribute" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

// This class is not ECMA-compatible, but is needed to implement
// the C# compiler's indexer name override mechanism.

#if CONFIG_FRAMEWORK_2_0
[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
#else
[AttributeUsage(AttributeTargets.Property, Inherited=true)]
#endif
public sealed class IndexerNameAttribute : Attribute
{

	// Constructors.
	public IndexerNameAttribute(String indexerName) : base() {}

}; // class IndexerNameAttribute

}; // namespace System.Runtime.CompilerServices
