/*
 * SymLanguageType.cs - Implementation of 
 *			"System.Diagnostics.SymbolStore.SymLanguageType" class.
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
	public class SymLanguageType
	{
		public static readonly Guid Basic
			= new Guid("3a12d0b8-c26c-11d0-b442-00a0244a1dd2");

		public static readonly Guid C
			= new Guid("63a08714-fc37-11d2-904c-00c04fa302a1");

		public static readonly Guid Cobol
			= new Guid("af046cd1-d0e1-11d2-977c-00a0c9b4d50c");

		public static readonly Guid CPlusPlus
			= new Guid("3a12d0b7-c26c-11d0-b442-00a0244a1dd2");

		public static readonly Guid CSharp
			= new Guid("3f5162f8-07c6-11d3-9053-00c04fa302a1");

		public static readonly Guid ILAssembly
			= new Guid("af046cd3-d0e1-11d2-977c-00a0c9b4d50c");

		public static readonly Guid Java
			= new Guid("3a12d0b4-c26c-11d0-b442-00a0244a1dd2");

		public static readonly Guid JScript
			= new Guid("3a12d0b6-c26c-11d0-b442-00a0244a1dd2");

		public static readonly Guid MCPlusPlus
			= new Guid("4b35fde8-07c6-11d3-9053-00c04fa302a1");

		public static readonly Guid Pascal
			= new Guid("af046cd2-d0e1-11d2-977c-00a0c9b4d50c");

		public static readonly Guid SMC
			= new Guid("0d9b9f7b-6611-11d3-bd2a-0000f80849bd");
	}
}//namespace

#endif // CONFIG_EXTENDED_DIAGNOSTICS
