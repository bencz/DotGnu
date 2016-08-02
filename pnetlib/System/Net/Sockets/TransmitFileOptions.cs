/*
 * TransmitFileOptions.cs - Implementation of the
 *			"System.Net.Sockets.TransmitFileOptions" class.
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

namespace System.Net.Sockets
{

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

[Flags]
public enum TransmitFileOptions
{
	UseDefaultWorkerThread	= 0x0000,
	Disconnect				= 0x0001,
	ReuseSocket				= 0x0002,
	WriteBehind				= 0x0004,
	UseSystemThread			= 0x0010,
	UseKernelApc			= 0x0020
}; // enum TransmitFileOptions

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net.Sockets
