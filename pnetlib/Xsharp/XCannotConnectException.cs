/*
 * XCannotConnectException.cs - Exception that is thrown when X#
 *		cannot connect to an X display server.
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
/// <para>Exception type that is thrown when <c>Xsharp.Display.Open</c>
/// cannot connect to an X display server.</para>
/// </summary>
public sealed class XCannotConnectException : XException
{

	// Constructors.
	public XCannotConnectException()
		: base(S._("X_CannotOpen")) {}
	public XCannotConnectException(String msg)
		: base(msg) {}
	public XCannotConnectException(String msg, Exception inner)
		: base(msg, inner) {}

} // class XCannotConnectException

} // namespace Xsharp
