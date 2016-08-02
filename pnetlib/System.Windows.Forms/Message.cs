/*
 * Message.cs - Implementation of the
 *			"System.Windows.Forms.Message" class.
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

namespace System.Windows.Forms
{

// Note: while "Message" is not a Compact Forms class, we need it
// to do proper key event dispatching.

public struct Message
{
	// Internal state.
	private IntPtr hWnd;
	private int msg;
	private IntPtr wParam;
	private IntPtr lParam;
	private IntPtr result;
	internal Keys key;

	// Get or set the message parameters.
	public IntPtr HWnd
			{
				get
				{
					return hWnd;
				}
				set
				{
					hWnd = value;
				}
			}
	public int Msg
			{
				get
				{
					return msg;
				}
				set
				{
					msg = value;
				}
			}
	public IntPtr WParam
			{
				get
				{
					return wParam;
				}
				set
				{
					wParam = value;
				}
			}
	public IntPtr LParam
			{
				get
				{
					return lParam;
				}
				set
				{
					lParam = value;
				}
			}
	public IntPtr Result
			{
				get
				{
					return result;
				}
				set
				{
					result = value;
				}
			}

	// Create a message from its raw components.
	public static Message Create(IntPtr hWnd, int msg,
								 IntPtr wparam, IntPtr lparam)
			{
				Message value;
				value.hWnd = hWnd;
				value.msg = msg;
				value.wParam = wparam;
				value.lParam = lparam;
				value.result = IntPtr.Zero;
				value.key = Keys.None;
				return value;
			}

	// Create a fake key message.
	internal static Message CreateKeyMessage(int msg, Keys key)
			{
				Message value;
				value.hWnd = IntPtr.Zero;
				value.msg = msg;
				value.wParam = new IntPtr((int)(key & Keys.KeyCode));
				value.lParam = IntPtr.Zero;
				value.result = IntPtr.Zero;
				value.key = key;
				return value;
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				if(obj is Message)
				{
					Message other = (Message)obj;
					return (hWnd == other.hWnd &&
							msg == other.msg &&
							wParam == other.wParam &&
							lParam == other.lParam &&
							result == other.result);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return msg;
			}

	// Convert the LParam value into an object structure.
	// For security reasons, this doesn't do anything.
	// The control classes decode the "Args" value instead
	// of "LParam", which is safe.
	public Object GetLParam(Type cls)
			{
				return null;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return String.Format
					("hWnd=0x{0:x}, msg=0x{1:x}, wparam=0x{2:x}, " +
					 "lparam=0x{3:x}, result=0x{4:x}",
					 hWnd.ToInt64(), msg, wParam.ToInt64(),
					 lParam.ToInt64(), result.ToInt64());
			}

}; // struct Message

}; // namespace System.Windows.Forms
