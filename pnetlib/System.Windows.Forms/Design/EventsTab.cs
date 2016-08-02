/*
 * EventsTab.cs - Implementation of the
 *			"System.Windows.Forms.Design.EventsTab" class.
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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

	public class EventsTab : PropertyTab
	{
		private bool sunkEvent;
		private IServiceProvider sp;
		private IDesignerHost host;

		public EventsTab(IServiceProvider sp)
		{
			this.sp = sp;
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return base.GetProperties(null, component, attributes);
		}

		public override PropertyDescriptor GetDefaultProperty(object obj)
		{
			IEventBindingService iEventBindingService = GetEventPropertyService(obj, null);
			if (iEventBindingService == null)
				return null;
			EventDescriptor eventDescriptor = TypeDescriptor.GetDefaultEvent(obj);
			if (eventDescriptor != null)
				return iEventBindingService.GetEventProperty(eventDescriptor);
			else
				return null;
		}

		private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs adevent)
		{
			host = adevent.NewDesigner;
		}

		private IEventBindingService GetEventPropertyService(object obj, ITypeDescriptorContext context)
		{
			IEventBindingService iEventBindingService = null;
			if (!sunkEvent)
			{
				sunkEvent = true;
				IDesignerEventService iDesignerEventService = sp.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
				if (iDesignerEventService != null)
					iDesignerEventService.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
			}

			if (iEventBindingService == null && host != null)
				iEventBindingService = host.GetService(typeof(IEventBindingService)) as IEventBindingService;

			if (iEventBindingService == null && obj is IComponent)
			{
				if (obj != null)
					iEventBindingService = (obj as IComponent).Site.GetService(typeof(IEventBindingService)) as IEventBindingService;
			}
			if (iEventBindingService == null && context != null)
				iEventBindingService = context.GetService(typeof(IEventBindingService)) as IEventBindingService;

			return iEventBindingService;
		}

		public override string TabName
		{
			get
			{
				return "EventsTab";
			}
		}

		public override string HelpKeyword
		{
			get
			{
				return "EventsTab";
			}
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{
			IEventBindingService iEventBindingService = GetEventPropertyService(component, context);
			if (iEventBindingService == null)
				return new PropertyDescriptorCollection(null);

			PropertyDescriptorCollection properties = iEventBindingService.GetEventProperties(TypeDescriptor.GetEvents(component, attributes));
			Attribute[] newAttributes = new Attribute[attributes.Length + 1];
			Array.Copy(attributes, 0, newAttributes, 0, attributes.Length);
			newAttributes[attributes.Length] = DesignerSerializationVisibilityAttribute.Content;
			PropertyDescriptorCollection newProperties = TypeDescriptor.GetProperties(component, newAttributes);
			if (newProperties.Count > 0)
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < newProperties.Count; i++)
				{
					PropertyDescriptor property = newProperties[i];
					if (property.Converter.GetPropertiesSupported() && TypeDescriptor.GetEvents(property.GetValue(component), attributes).Count > 0)
					{
						property = TypeDescriptor.CreateProperty(property.ComponentType, property, new Attribute[]{MergablePropertyAttribute.No});
						arrayList.Add(property);
					}
				}
				if (arrayList.Count > 0)
				{
					// Combine properties
					PropertyDescriptor[] propertyArray = new PropertyDescriptor[(arrayList.Count + properties.Count)];
					properties.CopyTo(propertyArray, 0);
					Array.Copy(arrayList.ToArray(typeof(PropertyDescriptor)), 0, propertyArray, properties.Count, arrayList.Count);
					properties = new PropertyDescriptorCollection(propertyArray);
				}
			}
			return properties;
		}
	}
#endif
}
