/*
 * IAppDomainSetup.cs - Implementation of the "System.IAppDomainSetup" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

using System.Runtime.InteropServices;

[Guid("27FFF232-A7A8-40dd-8D4A-734AD59FCD41")]
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
public interface IAppDomainSetup
{

	String ApplicationBase { get; set; }
	String ApplicationName { get; set; }
	String CachePath { get; set; }
	String ConfigurationFile { get; set; }
	String DynamicBase { get; set; }
	String LicenseFile { get; set; }
	String PrivateBinPath { get; set; }
	String PrivateBinPathProbe { get; set; }
	String ShadowCopyDirectories { get; set; }
	String ShadowCopyFiles { get; set; }

}; // interface IAppDomainSetup

#endif // !ECMA_COMPAT

}; // namespace System
