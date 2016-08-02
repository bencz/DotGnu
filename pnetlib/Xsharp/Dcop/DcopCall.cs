/*
 * DcopCallAttribute.cs - used to mark DCOP call stubs.
 * 
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Dcop
{
using System;
// This class is not ECMA-compatible strictly speaking, but it is
// needed to support thread-static variables in the ECMA engine.

#if !ECMA_COMPAT
[Serializable]
#endif
[AttributeUsage(AttributeTargets.Method, Inherited=true)]
public class DcopCallAttribute : Attribute
{
	// FIXME: Tell me if this eats up memory, 'cause it's not really used for now
	[NonSerializedAttribute]
	DcopFunction function;
	// Constructor.
	public DcopCallAttribute(string fun)
		{
			function = new DcopFunction(fun);
		}

}; // class DcopCallAttribute

}; // namespace Xsharp.Dcop
