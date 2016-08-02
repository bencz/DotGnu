/*
 * ISecureSessionProvider.cs - Implementation of the
 *		"DotGNU.SSL.ISecureSessionProvider" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.SSL
{

using System;

/// <summary>
/// <para>The <see cref="T:DotGNU.SSL.ISecureSessionProvider"/> interface
/// is implemented by classes that wish to provide secure session
/// functionality to the application program.</para>
/// </summary>
public interface ISecureSessionProvider
{
	/// <summary>
	/// <para>Create a new session handling object for a client,
	/// to allow it to connect to a secure server.</para>
	/// </summary>
	///
	/// <param name="protocol">
	/// <para>Specifies the <see cref="T:DotGNU.SSL.Protocol"/> to use to
	/// connect to the server.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns the session object.</para>
	/// </returns>
	///
	/// <exception cref="T:System.NotSupportedException">
	/// <para>The requested protocol is not supported.</para>
	/// </exception>
	ISecureSession CreateClientSession(Protocol protocol);

	/// <summary>
	/// <para>Create a new session handling object for a server,
	/// to allow it to process incoming requests from a secure client.</para>
	/// </summary>
	///
	/// <param name="protocol">
	/// <para>Specifies the <see cref="T:DotGNU.SSL.Protocol"/> that the
	/// client is allowed to use when connecting.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns the session object.</para>
	/// </returns>
	///
	/// <exception cref="T:System.NotSupportedException">
	/// <para>The requested protocol is not supported.</para>
	/// </exception>
	ISecureSession CreateServerSession(Protocol protocol);

}; // interface ISecureSessionProvider

}; // namespace DotGNU.SSL
