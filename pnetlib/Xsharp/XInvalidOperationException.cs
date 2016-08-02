/*
 * XInvalidOperationException.cs - Exception that is thrown when
 *		X# has detected an invalid state (e.g. window destroyed).
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp
{

using System;

/// <summary>
/// <para>Exception type that is thrown when the library detects an
/// invalid state.  For example, attempting to move a destroyed window,
/// creating a window on a closed display connection, etc.</para>
/// </summary>
public sealed class XInvalidOperationException : XException
{

	// Constructors.
	public XInvalidOperationException()
		: base(S._("X_InvalidOperation")) {}
	public XInvalidOperationException(String msg)
		: base(msg) {}
	public XInvalidOperationException(String msg, Exception inner)
		: base(msg, inner) {}

} // class XInvalidOperationException

} // namespace Xsharp
