/*
 * InstallationFlags.cs - Implementation of the
 *			"System.EnterpriseServices.InstallationFlags" class.
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

namespace System.EnterpriseServices
{

[Flags]
public enum InstallationFlags
{
	Default							= 0x0000,
	ExpectExistingTypeLib			= 0x0001,
	CreateTargetApplication			= 0x0002,
	FindOrCreateTargetApplication	= 0x0004,
	ReconfigureExistingApplication	= 0x0008,
	ConfigureComponentsOnly			= 0x0010,
	ReportWarningsToConsole			= 0x0020,
	Register						= 0x0100,
	Install							= 0x0200,
	Configure						= 0x0400

}; // enum InstallationFlags

}; // namespace System.EnterpriseServices
