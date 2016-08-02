/*
 * InvalidEnumArgumentException.cs - Implementation of the
 *		"System.ComponentModel.InvalidEnumArgumentException" class.
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

[Serializable]
public class InvalidEnumArgumentException : ArgumentException
{
	// Internal state.
	private int invalidValue;
	private Type enumClass;

	// Constructors.
	public InvalidEnumArgumentException()
			: base(S._("Exception_InvalidEnum"))
			{
				HResult = unchecked((int)0x80070057);
			}
	public InvalidEnumArgumentException(String message)
			: base(message)
			{
				HResult = unchecked((int)0x80070057);
			}
	public InvalidEnumArgumentException(String argumentName,
										int invalidValue,
										Type enumClass)
			: base(null, argumentName)
			{
				HResult = unchecked((int)0x80070057);
				this.invalidValue = invalidValue;
				this.enumClass = enumClass;
			}

}; // class InvalidEnumArgumentException

#endif // !ECMA_COMPAT

}; // namespace System.ComponentModel
