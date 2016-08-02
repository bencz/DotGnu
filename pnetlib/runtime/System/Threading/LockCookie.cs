/*
 * LockCookie.cs - Implementation of the
 *		"System.Threading.LockCookie" class.
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

namespace System.Threading
{

#if !ECMA_COMPAT

[Serializable]
public struct LockCookie
{
	// Types of lock cookies.
	internal enum CookieType
	{
		None,
		Upgrade,
		Saved

	}; // enum CookieType

	// Internal state.
	internal CookieType type;
	internal Thread thread;
	internal int readCount;
	internal int writeCount;

	// Constructor.
	internal LockCookie(CookieType type, Thread thread,
						int readCount, int writeCount)
			{
				this.type = type;
				this.thread = thread;
				this.readCount = readCount;
				this.writeCount = writeCount;
			}

}; // struct LockCookie

#endif // !ECMA_COMPAT

}; // namespace System.Threading
