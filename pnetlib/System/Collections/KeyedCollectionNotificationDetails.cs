/*
 * KeyedCollectionNotificationDetails.cs - Implementation of
 *		"System.Collections.KeyedCollectionNotificationDetails".
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

namespace System.Collections
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

public struct KeyedCollectionNotificationDetails
{
	// Internal state.
	private bool accessedByKey;
	private int index;
	private Object key;
	private Object newValue;
	private Object oldValue;

	// Get or set this object's properties.
	public bool AccessedByKey
			{
				get
				{
					return accessedByKey;
				}
				set
				{
					accessedByKey = value;
				}
			}
	public int Index
			{
				get
				{
					return index;
				}
				set
				{
					index = value;
				}
			}
	public Object Key
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}
	public Object NewValue
			{
				get
				{
					return newValue;
				}
				set
				{
					newValue = value;
				}
			}
	public Object OldValue
			{
				get
				{
					return oldValue;
				}
				set
				{
					oldValue = value;
				}
			}

}; // struct KeyedCollectionNotificationDetails

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Collections
