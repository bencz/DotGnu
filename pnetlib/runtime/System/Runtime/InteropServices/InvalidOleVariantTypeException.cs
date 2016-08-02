/*
 * InvalidOleVariantTypeException.cs - Implementation of the
 *	"System.Runtime.InteropServices.InvalidOleVariantTypeException" class 
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
public class InvalidOleVariantTypeException : SystemException
{

	// Constructors.
	public InvalidOleVariantTypeException()
			: base(_("Exception_InvalidOleType")) {}
	public InvalidOleVariantTypeException(String msg)
			: base(msg) {}
	public InvalidOleVariantTypeException(String msg, Exception inner)
			: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected InvalidOleVariantTypeException(SerializationInfo info,
										     StreamingContext context)
			: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_InvalidOleType");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131531;
				}
			}

}; // class InvalidOleVariantTypeException

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
