/*
 * ListViewSubItemConverter.cs - Implementation of the
 *			"System.Windows.Forms.ListViewSubItemConverter" class.
 *
 * Copyright (C) 2004  Neil Cawse.
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
#if CONFIG_COMPONENT_MODEL && CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Drawing;

	class ListViewSubItemConverter : ExpandableObjectConverter
	{

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			else
			{
				return base.CanConvertTo(context, destinationType);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem.ListViewSubItem)
			{
				ConstructorInfo constructorInfo;
				Type[] constructorTypes;

				ListViewItem.ListViewSubItem subItem = value as ListViewItem.ListViewSubItem;
				if (subItem.properties == null)
				{
					constructorTypes = new Type[]{typeof(ListViewItem), typeof(string)};
					constructorInfo = typeof(ListViewItem.ListViewSubItem).GetConstructor(constructorTypes);
					if (constructorInfo != null)
					{
						object[] arguments = new object[] {subItem.Text, null};
						return new InstanceDescriptor(constructorInfo, (ICollection) arguments, true);
					}
				}
				
				constructorTypes = new Type[]{typeof(ListViewItem), typeof(string), typeof(Color), typeof(Color), typeof(Font)};
				constructorInfo = typeof(ListViewItem.ListViewSubItem).GetConstructor(constructorTypes);
				if (constructorInfo != null)
				{
					object[] arguments = new object[] {subItem.Text, subItem.ForeColor, subItem.BackColor, subItem.Font};
					return new InstanceDescriptor(constructorInfo, (ICollection) arguments, true);
				}
				
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif
}

