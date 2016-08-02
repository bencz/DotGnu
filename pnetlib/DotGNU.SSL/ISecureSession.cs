/*
 * ISecureSession.cs - Implementation of the
 *		"DotGNU.SSL.ISecureSession" class.
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
using System.IO;

/// <summary>
/// <para>The <see cref="T:DotGNU.SSL.ISecureSession"/> interface
/// is implemented by classes that provide secure client or server
/// session functionality to the application program.</para>
/// </summary>
public interface ISecureSession : IDisposable
{
	/// <summary>
	/// <para>Determines if this session is handling a client.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if this session is
	/// handling a client, or <see langword="false"/> if this session
	/// is handling a server.</para>
	/// </value>
	bool IsClient { get; }

	/// <summary>
	/// <para>Get or set the X.509 certificate for the local machine.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The ASN.1 form of the certificate.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property must be set for server sessions, and for
	/// client sessions that perform client authentication.</para>
	/// </remarks>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The supplied value was <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentException">
	/// <para>The supplied value was not a valid certificate.</para>
	/// </exception>
	///
	/// <exception cref="T:System.InvalidOperationException">
	/// <para>The certificate was already set previously, or
	/// the handshake has already been performed.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ObjectDisposedException">
	/// <para>The secure session has been disposed.</para>
	/// </exception>
	byte[] Certificate { get; set; }

	/// <summary>
	/// <para>Get or set the private key for the local machine.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The ASN.1 form of the RSA private key.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property must be set for server sessions, and for
	/// client sessions that perform client authentication.</para>
	/// </remarks>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The supplied value was <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentException">
	/// <para>The supplied value was not a valid private key.</para>
	/// </exception>
	///
	/// <exception cref="T:System.InvalidOperationException">
	/// <para>The private key was already set previously, or
	/// the handshake has already been performed.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ObjectDisposedException">
	/// <para>The secure session has been disposed.</para>
	/// </exception>
	byte[] PrivateKey { get; set; }

	/// <summary>
	/// <para>Get the certificate of the remote host once the
	/// secure connection has been established.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The ASN.1 form of the certificate, or <see langword="null"/>
	/// if the secure connection has not yet been established.</para>
	/// </value>
	byte[] RemoteCertificate { get; }

	/// <summary>
	/// <para>Perform the initial handshake on a
	/// <see cref="T:System.Net.Sockets.Socket"/> instance and
	/// return a stream that can be used for secure communications.</para>
	/// </summary>
	///
	/// <param name="socket">
	/// <para>The socket to use for the underlying communications channel.
	/// </para>
	/// </param>
	///
	/// <exception name="T:System.ArgumentNullException">
	/// <para>The <paramref name="socket"/> parameter is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception name="T:System.ArgumentException">
	/// <para>The <paramref name="socket"/> parameter is not a
	/// valid socket object.</para>
	/// </exception>
	///
	/// <exception name="T:System.InvalidOperationException">
	/// <para>The handshake has already been performed.</para>
	/// </exception>
	///
	/// <exception name="T:System.NotSupportedException">
	/// <para>The secure handshake could not be performed because
	/// the necessary provider objects could not be created.
	/// Usually this is because the provider is out of memory.</para>
	/// </exception>
	///
	/// <exception name="T:System.ObjectDisposedException">
	/// <para>The secure session has been disposed.</para>
	/// </exception>
	///
	/// <exception name="T:System.Security.SecurityException">
	/// <para>Some kind of security error occurred while attempting
	/// to establish the secure connection.</para>
	/// </exception>
	Stream PerformHandshake(Object socket);

	/// <summary>
	/// <para>Get the secure communications stream.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns the secure communications stream, or
	/// <see langword="null"/> if the handshake has not yet
	/// been performed.</para>
	/// </value>
	Stream SecureStream { get; }

}; // interface ISecureSession

}; // namespace DotGNU.SSL
