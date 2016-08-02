/*
 * Package.cs - Package nodes.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using Microsoft.JScript.Vsa;

// Dummy class for backwards-compatibility.

public sealed class Package : AST
{
	// Constructor.
	internal Package() : base() {}

	// Declare a package within an engine instance.
	public static void JScriptPackage(String name, VsaEngine engine)
			{
				// TODO
			}

}; // class Package

}; // namespace Microsoft.JScript
