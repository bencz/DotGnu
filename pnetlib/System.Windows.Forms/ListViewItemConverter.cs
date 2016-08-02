/*
 * ListViewItemConverter.cs - Implementation of the
 *			"System.Windows.Forms.ListViewItemConverter" class.
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

	public class ListViewItemConverter : ExpandableObjectConverter
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
			if (value is ListViewItem && destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo constructorInfo;

				Type[] constructorTypes;
				object[] arguments;

				ListViewItem item = value as ListViewItem;
				for (int i = 1; i < item.SubItems.Count; i++)
				{
					if (item.SubItems[i].properties != null)
					{
						constructorTypes = new Type[]{typeof(ListViewItem.ListViewSubItem[]), typeof(int)};
						constructorInfo = typeof(ListViewItem).GetConstructor(constructorTypes);
						if (constructorInfo != null)
						{
							ListViewItem.ListViewSubItem[] listViewSubItems = new ListViewItem.ListViewSubItem[item.SubItems.Count];
							(item.SubItems as ICollection).CopyTo(listViewSubItems, 0);
							arguments = new object[]{listViewSubItems, item.ImageIndex};
							return new InstanceDescriptor(constructorInfo, (ICollection) arguments, false);
						}
						break;
					}
				}
				string[] strs = new string[item.SubItems.Count];
				for (int i = 0; i < strs.Length; i++)
				{
					strs[i] = item.SubItems[i].Text;
				}
				ListViewItem.ListViewSubItem subItem = item.SubItems[0];
				if (subItem.properties != null)
				{
					constructorTypes = new Type[]{typeof(string[]), typeof(int), typeof(Color), typeof(Color), typeof(Font)};
					constructorInfo = typeof(ListViewItem).GetConstructor(constructorTypes);
					if (constructorInfo != null)
					{
						Color foreColor = subItem.properties.foreColor;
						if (foreColor == Color.Empty)
						{
							foreColor = item.ForeColor;
						}
						Color backColor = subItem.properties.backColor;
						if (backColor == Color.Empty)
						{
							backColor = item.BackColor;
						}
						Font font = subItem.properties.font;
						if (font == null)
						{
							font = item.Font;
						}
						arguments = new object[]{strs, item.ImageIndex, foreColor, backColor, font};
						return new InstanceDescriptor(constructorInfo, (ICollection) arguments, false);
					}
				}
				if (item.SubItems.Count <= 1)
				{
					if (item.ImageIndex == -1)
					{
						constructorTypes = new Type[]{typeof(string)};
						constructorInfo = typeof(ListViewItem).GetConstructor(constructorTypes);
						if (constructorInfo != null)
						{
							arguments = new object[]{item.Text};
							return new InstanceDescriptor(constructorInfo, (ICollection) arguments, false);
						}
					}
					else
					{
						constructorTypes = new Type[]{typeof(string), typeof(int)};
						constructorInfo = typeof(ListViewItem).GetConstructor(constructorTypes);
						if (constructorInfo != null)
						{
							arguments = new object[]{item.Text, item.ImageIndex};
							return new InstanceDescriptor(constructorInfo, (ICollection) arguments, false);
						}
					}
				}
				constructorTypes = new Type[]{typeof(string[]), typeof(int)};
				constructorInfo = typeof(ListViewItem).GetConstructor(constructorTypes);
				if (constructorInfo != null)
				{
					arguments = new object[]{strs, item.ImageIndex};
					return new InstanceDescriptor(constructorInfo, (ICollection) arguments, false);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif
}
