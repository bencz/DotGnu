/*
 * WatsonBucketParameters.cs - Implementation of the
 *			"System.Runtime.InteropServices.WatsonBucketParameters" class.
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

using System.Collections;

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_2_0

[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public sealed class WatsonBucketParameters
{
	// Internal state.
	private String eventTypeName;
	private ArrayList list;

	// Constructor.
	public WatsonBucketParameters(String eventTypeName, String[] parameters)
			{
				if(eventTypeName == null)
				{
					throw new ArgumentNullException("eventTypeName");
				}
				if(parameters == null)
				{
					throw new ArgumentNullException("parameters");
				}
				this.eventTypeName = eventTypeName;
				this.list = new ArrayList(parameters);
			}

	// Get the bucket parameters for the current exception.
	public static WatsonBucketParameters GetParametersForCurrentException()
			{
				return new WatsonBucketParameters("", new String[] {});
			}

	// Get this object's properties.
	public String EventTypeName
			{
				get
				{
					return eventTypeName;
				}
				set
				{
					eventTypeName = value;
				}
			}
	public bool IsInited
			{
				get
				{
					return true;
				}
			}

	// Get or set a parameter.
	public String this[int index]
			{
				get
				{
					if(index >= 0 && index < list.Count)
					{
						return (list[index] as String);
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(index >= 0 && index < list.Count)
					{
						list[index] = value;
					}
					else
					{
						while(list.Count < index)
						{
							list.Add("");
						}
						list.Add(value);
					}
				}
			}

}; // class WatsonBucketParameters

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_2_0

}; // namespace System.Runtime.InteropServices
