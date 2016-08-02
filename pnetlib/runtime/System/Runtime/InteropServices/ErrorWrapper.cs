/*
 * ErrorWrapper.cs - Implementation of the
 *			"System.Runtime.InteropServices.ErrorWrapper" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

public sealed class ErrorWrapper
{
	// Internal state.
	private int code;

	// Constructors.
	public ErrorWrapper(Exception e)
			{
				code = Marshal.GetHRForException(e);
			}
	public ErrorWrapper(int errorCode)
			{
				code = errorCode;
			}
	public ErrorWrapper(Object errorCode)
			{
				if(errorCode is int)
				{
					code = (int)errorCode;
				}
				else
				{
					throw new ArgumentException
						(_("Arg_MustBeInt32"), "errorCode");
				}
			}

	// Get the wrapped error code.
	public int ErrorCode
			{
				get
				{
					return code;
				}
			}

}; // class ErrorWrapper

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
