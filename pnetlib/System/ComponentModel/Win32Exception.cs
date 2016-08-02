/*
 * Win32Exception.cs - Implementation of the
 *			"System.ComponentModel.Win32Exception" class.
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

namespace System.ComponentModel
{

#if !ECMA_COMPAT

using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

[Serializable]
#if CONFIG_PERMISSIONS
[SuppressUnmanagedCodeSecurity]
#endif
public class Win32Exception : ExternalException
{
	// Internal state.
	private int error;

	// Constructors.
	public Win32Exception() : base()
			{
				HResult = unchecked((int)0x80004005);
			}
	public Win32Exception(int error) : base()
			{
				this.error = error;
				HResult = unchecked((int)0x80004005);
			}
	public Win32Exception(int error, String message) : base(message)
			{
				this.error = error;
				HResult = unchecked((int)0x80004005);
			}
	internal Win32Exception(String message) : base(message)
			{
				HResult = unchecked((int)0x80004005);
			}
	internal Win32Exception(String message, Exception inner) 
			: base(message,inner)
			{
				HResult = unchecked((int)0x80004005);
			}
#if CONFIG_SERIALIZATION
	protected Win32Exception(SerializationInfo info, StreamingContext context)
			: base(info, context)
			{
				error = info.GetInt32("NativeErrorCode");
			}
#endif

	// Get the native error code corresponding to this exception.
	public int NativeErrorCode 
			{
				get
				{
					return error;
				}
			}

#if CONFIG_SERIALIZATION

	// Get serialization data for this object.
	public override void GetObjectData
				(SerializationInfo info, StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("NativeErrorCode", error);
			}

#endif

}; // class Win32Exception

#endif // !ECMA_COMPAT

}; // namespace System.ComponentModel
