/*
 * IconData.cs - Data handling for icons in X applications.
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
using System.IO;
using System.Reflection;

/// <summary>
/// <para>The <see cref="T:Xsharp.IconData"/> class manages simple
/// icons on an X display screen.</para>
/// </summary>
public sealed class IconData : IDisposable
{
	// Internal state.
	private byte[] data;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.IconData"/> instance
	/// that represents an icon.</para>
	/// </summary>
	///
	/// <param name="data">
	/// <para>The icon data, containing the color table
	/// and pixel values.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="data"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="data"/> value is invalid in some way.</para>
	/// </exception>
	public IconData(byte[] data)
			{
				if(data == null)
				{
					throw new ArgumentNullException("data");
				}
				Load(data);
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.IconData"/> instance
	/// that represents an icon from an embedded resource.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The name of the resource in the calling assembly.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="name"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="name"/> resource is invalid
	/// in some way.</para>
	/// </exception>
	public IconData(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				try
				{
					Load(name, Assembly.GetCallingAssembly());
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not have reflection support.
					InvalidFormat();
				}
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.IconData"/> instance
	/// that represents an icon from an embedded resource.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The name of the resource.</para>
	/// </param>
	///
	/// <param name="assembly">
	/// <para>The assembly to look for the resource in.  This can be
	/// <see langword="null"/> to use the calling assembly.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="name"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="name"/> resource is invalid
	/// in some way.</para>
	/// </exception>
	public IconData(String name, Assembly assembly)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(assembly == null)
				{
					try
					{
						assembly = Assembly.GetCallingAssembly();
					}
					catch(NotImplementedException)
					{
						// The runtime engine does not have reflection support.
						InvalidFormat();
					}
				}
				Load(name, assembly);
			}

	// Load the contents of an icon resource from a specific assembly.
	private void Load(String name, Assembly assembly)
			{
				Stream stream = null;

				// Get a stream for the icon resource.
				try
				{
					stream = assembly.GetManifestResourceStream(name);
					if(stream == null)
					{
						InvalidFormat();
					}
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not have reflection support.
					InvalidFormat();
				}

				// Load the contents of the stream into a buffer.
				int len = (int)(stream.Length);
				byte[] data = new byte [len];
				if(stream.Read(data, 0, len) != len)
				{
					stream.Close();
					InvalidFormat();
				}
				stream.Close();

				// Load the icon data.
				Load(data);
			}

	// Load an icon resource from an explicit data buffer.
	private void Load(byte[] data)
			{
				this.data = data;
			}

	// Throw an invalid icon format exception.
	private static void InvalidFormat()
			{
				throw new XException(S._("X_InvalidIcon"));
			}

	/// <summary>
	/// <para>Dispose an instance of <see cref="T:Xsharp.IconData"/>.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.IDisposeable"/>
	/// interface.</para>
	/// </remarks>
	public void Dispose()
			{
				data = null;
			}

} // class IconData

} // namespace Xsharp
