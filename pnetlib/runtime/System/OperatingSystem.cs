/*
 * OperatingSystem.cs - Implementation of the "System.OperatingSystem" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if !ECMA_COMPAT

using Platform;

public sealed class OperatingSystem : ICloneable
{
	// Internal state.
	private PlatformID platform;
	private Version version;

	// Constructor.
	public OperatingSystem(PlatformID platform, Version version)
			{
				if(version == null)
				{
					throw new ArgumentNullException("version");
				}
				this.platform = platform;
				this.version = version;
			}

	// Properties.
	public PlatformID Platform
			{
				get
				{
					return platform;
				}
			}
	public Version Version
			{
				get
				{
					return version;
				}
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				return new OperatingSystem
					(platform, (Version)(version.Clone()));
			}

	// Convert the OS version into a string.
	public override String ToString()
			{
				String os;
				switch(platform)
				{
					case PlatformID.Win32S:
						os = "Microsoft Win32S ";
						break;

					case PlatformID.Win32Windows:
						os = "Microsoft Windows 98 ";
						break;

					case PlatformID.Win32NT:
						os = "Microsoft Windows NT ";
						break;

					default:
						os = "Unix [" + InfoMethods.GetPlatformName() + "] ";
						break;
				}
				return os + version.ToString();
			}

}; // class OperatingSystem

#endif // !ECMA_COMPAT

}; // namespace System
