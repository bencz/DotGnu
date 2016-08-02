/*
 * RegistryValueKind.cs - Implementation of the
 *			"Microsoft.Win32.RegistryValueKind" class.
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

namespace Microsoft.Win32
{

#if CONFIG_WIN32_SPECIFICS && CONFIG_FRAMEWORK_2_0

public enum RegistryValueKind
{
	Unknown			= 0,
	String			= 1,
	ExpandString	= 2,
	Binary			= 3,
	DWord			= 4,
	MultiString		= 7,
	QWord			= 11

}; // enum RegistryValueKind

#endif // CONFIG_WIN32_SPECIFICS && CONFIG_FRAMEWORK_2_0

}; // namespace Microsoft.Win32
