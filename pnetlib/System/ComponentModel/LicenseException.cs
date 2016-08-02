/*
 * LicenseException.cs - Implementation of the
 *		"System.ComponentModel.LicenseException" class.
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

using System;

public class LicenseException : SystemException
{
	// Internal state.
	private Type type;
	private Object instance;

	// Constructors.
	public LicenseException(Type type)
			: base(S._("Exception_License"))
			{
				this.type = type;
				HResult = unchecked((int)0x80131901);
			}
	public LicenseException(Type type, Object instance)
			: base(S._("Exception_License"))
			{
				this.type = type;
				this.instance = instance;
				HResult = unchecked((int)0x80131901);
			}
	public LicenseException(Type type, Object instance, String message)
			: base(message)
			{
				this.type = type;
				this.instance = instance;
				HResult = unchecked((int)0x80131901);
			}
	public LicenseException(Type type, Object instance, String message,
							Exception innerException)
			: base(message, innerException)
			{
				this.type = type;
				this.instance = instance;
				HResult = unchecked((int)0x80131901);
			}

	// Get the licensed type that caused the exception.
	public Type LicensedType
			{
				get
				{
					return type;
				}
			}

}; // class LicenseException

#endif // !ECMA_COMPAT

}; // namespace System.ComponentModel
