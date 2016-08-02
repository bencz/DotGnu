/*
 * LocalBuilder.cs - Implementation of "System.Reflection.Emit.LocalBuilder" 
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * 
 * Contributions from Gopal.V <gopalv82@symonds.net> 
 *                    Rhys Weatherley <rweather@southern-storm.com.au>
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

using System;

#if CONFIG_REFLECTION_EMIT

namespace System.Reflection.Emit
{

public sealed class LocalBuilder
{
	// Internal state.
	private ModuleBuilder module;
	private String name;
	private Type type;
	internal int index;

	// Constructor.
	internal LocalBuilder(ModuleBuilder module, Type type, int index)
			{
				this.module = module;
				this.type = type;
				this.index = index;
			}

	// Set the symbol information for a local variable.  Not used here.
	public void SetLocalSymInfo(String lname, int startOffset, int endOffset)
			{
				name = lname;
			}
	public void SetLocalSymInfo(String lname)
			{
				SetLocalSymInfo(lname, 0, 0);
			}

	// Get the type of the local variable.
	public Type LocalType 
			{ 
				get
				{
					return type;
				}
			}

}; // class LocalBuilder

}; // namespace System.Reflection.Emit

#endif // CONFIG_REFLECTION_EMIT
