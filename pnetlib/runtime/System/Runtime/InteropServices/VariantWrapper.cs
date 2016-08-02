/*
 * VariantWrapper.cs - Implementation of the
 *			"System.Runtime.InteropServices.VariantWrapper" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[ComVisible(false)]
public sealed class VariantWrapper
{
	// Internal state.
	private Object value;

	// Constructors.
	public VariantWrapper(Object obj)
			{
				value = obj;
			}

	// Get the wrapped dispatch object value.
	public Object WrappedObject
			{
				get
				{
					return value;
				}
			}

}; // class VariantWrapper

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices
