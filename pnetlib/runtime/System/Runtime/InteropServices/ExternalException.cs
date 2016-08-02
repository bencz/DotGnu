/*
 * ExternalException.cs - Implementation of the "ExternalException" class 
 *
 * Copyright (C) 2001  Free Software Foundation, Inc.
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

#if !ECMA_COMPAT

// Needed for Compact .NET Framework compatibility, so not CONFIG_COM_INTEROP.

using System.Runtime.Serialization;

[Serializable]
public class ExternalException : SystemException
{

	private int errorCode;

	// Constructors.
	public ExternalException()
			: base(_("Exception_System")) {}
	public ExternalException(String msg)
			: base(msg) {}
	public ExternalException(String msg, Exception inner)
			: base(msg, inner) {}
	public ExternalException(String msg, int errorCode)
			: base(msg)
			{
				this.errorCode = errorCode;
			}
#if CONFIG_SERIALIZATION
	protected ExternalException(SerializationInfo info,
								StreamingContext context)
			: base(info, context) {}
#endif

	public virtual int ErrorCode
	{
		get
		{
			return this.errorCode;
		}
	}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_System");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80004005;
				}
			}

}; // class ExternalException

#endif // !ECMA_COMPAT

}; // namespace System.Runtime.InteropServices
