/*
 * COMException.cs - Implementation of the
 *			"System.Runtime.InteropServices.COMException" class 
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

using System.Runtime.Serialization;

[Serializable]
public class COMException : ExternalException
{

	private int errorCode;

	// Constructors.
	public COMException()
			: base(_("Exception_COM")) {}
	public COMException(String msg)
			: base(msg) {}
	public COMException(String msg, Exception inner)
			: base(msg, inner) {}
	public COMException(String msg, int errorCode)
			: base(msg, errorCode) {}
#if CONFIG_SERIALIZATION
	protected COMException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
#endif

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString();
			}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_COM");
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

}; // class COMException

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
