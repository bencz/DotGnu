/*
 * SocketFlags.cs - Implementation of the
 *			"System.Net.Sockets.SocketFlags" class.
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

[Flags]
public enum SocketFlags
{
	None				= 0x0000,
	OutOfBand			= 0x0001,
	Peek				= 0x0002,
	DontRoute			= 0x0004,
	MaxIOVectorLength	= 0x0010,
#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	Truncated			= 0x0100,
	ControlDataTruncated= 0x0200,
	Broadcast			= 0x0400,
	Multicast			= 0x0800,
#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	Partial				= 0x8000

}; // enum SocketFlags

}; // namespace System.Net.Sockets
