/*
 * PropertyTab.cs - Implementation of the
 *			"System.Windows.Forms.Design.PropertyTab" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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

namespace System.Windows.Forms.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.ComponentModel;
using System.Drawing;

	public abstract class PropertyTab : IExtenderProvider
	{
		private object[] components;

		[TODO]
		public virtual Bitmap Bitmap
		{
			get
			{
				return new Bitmap(16,16);
			}
		}

		public virtual object[] Components
		{
			get
			{
				return components;
			}

			set
			{
				components = value;
			}
		}

		public abstract string TabName
		{
			get;
		}


		public virtual bool CanExtend(object extendee)
		{
			return true;
		}

		public virtual PropertyDescriptorCollection GetProperties(object component)
		{
			return GetProperties(component, null);
		}

		public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

		public virtual PropertyDescriptor GetDefaultProperty(object component)
		{
			return TypeDescriptor.GetDefaultProperty(component);
		}

		public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			return GetProperties(component, attributes);
		}

		public virtual string HelpKeyword
		{
			get
			{
				return TabName;
			}
		}

	}
#endif
}
