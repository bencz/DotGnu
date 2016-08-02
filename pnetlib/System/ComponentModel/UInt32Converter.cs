/*
 * UInt32Converter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.UInt32Converter" class.
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

#if CONFIG_COMPONENT_MODEL

using System;
using System.Collections;
using System.Globalization;

public class UInt32Converter : BaseNumberConverter
{
	// Constructor.
	public UInt32Converter()
			{
				// Nothing to do here.
			}

	// Internal conversion from a string.
	internal override Object DoConvertFrom(String value, NumberFormatInfo nfi)
			{
				return UInt32.Parse(value, NumberStyles.Integer, nfi);
			}
	internal override Object DoConvertFromHex(String value)
			{
				return Convert.ToUInt32(value, 32);
			}

	// Internal convert to a string.
	internal override String DoConvertTo(Object value, NumberFormatInfo nfi)
			{
				return ((uint)value).ToString(null, nfi);
			}

}; // class UInt32Converter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
