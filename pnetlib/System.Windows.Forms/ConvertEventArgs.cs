/*
 * ConvertEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.ConvertEventArgs" class.
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

public class ConvertEventArgs : EventArgs
{
	// Internal state.
	private Object value;
	private Type desiredType;

	// Constructor.
	public ConvertEventArgs(Object value, Type desiredType)
			{
				this.value = value;
				this.desiredType = desiredType;
			}

	// Get the desired type for conversion.
	public Type DesiredType
			{
				get
				{
					return desiredType;
				}
			}

	// Get or set the value.
	public Object Value
			{
				get
				{
					return value;
				}
				set
				{
					this.value = value;
				}
			}

}; // class ConvertEventArgs

}; // namespace System.Windows.Forms
