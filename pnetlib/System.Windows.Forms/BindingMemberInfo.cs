/*
 * BindingMemberInfo.cs - Implementation of the
 *		"System.Windows.Forms.BindingMemberInfo" class.
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

using System.Globalization;

public struct BindingMemberInfo
{
	// Internal state.
	private String field;
	private Type fieldType;
	private String path;

	// Constructor.
	public BindingMemberInfo(String dataMember)
			{
				if(dataMember == null)
				{
					dataMember = String.Empty;
				}
				int index = dataMember.LastIndexOf('.');
				if(index != -1)
				{
					field = dataMember.Substring(index + 1);
					path = dataMember.Substring(0, index);
				}
				else
				{
					field = dataMember;
					path = String.Empty;
				}
				fieldType = null;
			}

	// Get this object's properties.
	public String BindingField
			{
				get
				{
					return field;
				}
			}
	internal Type BindingFieldType
			{
				get
				{
					return fieldType;
				}
				set
				{
					fieldType = value;
				}
			}
	public String BindingMember
			{
				get
				{
					if(path != String.Empty)
					{
						return path + "." + field;
					}
					else
					{
						return field;
					}
				}
			}
	public String BindingPath
			{
				get
				{
					return path;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object otherObject)
			{
				if(otherObject is BindingMemberInfo)
				{
					String current = BindingMember;
					String other;
					other = ((BindingMemberInfo)otherObject).BindingMember;
				#if !ECMA_COMPAT
					return (String.Compare
								(current, other, true,
								 CultureInfo.InvariantCulture) == 0);
				#else
					return (String.Compare
								(current, other, true) == 0);
				#endif
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				String member = BindingMember;
			#if !ECMA_COMPAT
				member = member.ToLower(CultureInfo.InvariantCulture);
			#else
				member = member.ToLower();
			#endif
				return member.GetHashCode();
			}

}; // struct BindingMemberInfo

}; // namespace System.Windows.Forms
