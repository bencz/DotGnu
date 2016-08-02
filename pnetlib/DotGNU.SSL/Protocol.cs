/*
 * Protocol.cs - Implementation of the "DotGNU.SSL.Protocol" class.
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

/// <summary>
/// <para>The <see cref="T:DotGNU.SSL.Protocol"/> enumeration specifies
/// which protocol to use when establishing or accepting a secure
/// connection.</para>
/// </summary>
public enum Protocol
{
	/// <summary>
	/// <para>Accept any protocol, be it SSLv2, SSLv3, or TLSv1.
	/// The client and server will negotiate the best protocol.</para>
	/// </summary>
	AutoDetect,

	/// <summary>
	/// <para>Use the SSLv2 protocol only.</para>
	/// </summary>
	SSLv2,

	/// <summary>
	/// <para>Use the SSLv3 protocol only.</para>
	/// </summary>
	SSLv3,

	/// <summary>
	/// <para>Use the TLSv1 protocol only.</para>
	/// </summary>
	TLSv1

}; // enum Protocol

}; // namespace DotGNU.SSL
