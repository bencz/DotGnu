/*
 * PropertyValueUIItem.cs - Implementation of the
 *		"System.Drawing.Design.PropertyValueUIItem" class.
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

namespace System.Drawing.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.Drawing;

	public class PropertyValueUIItem
	{
		private Image image;
		private PropertyValueUIItemInvokeHandler handler;
		private string tooltip;

		public virtual Image Image
		{
			get
			{
				return image;
			}
		}

		public virtual PropertyValueUIItemInvokeHandler InvokeHandler
		{
			get
			{
				return handler;
			}
		}

		public virtual string ToolTip
		{
			get
			{
				return tooltip;
			}
		}

		public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
		{
			image = uiItemImage;
			this.handler = handler;
			this.tooltip = tooltip;
		}

		public virtual void Reset()
		{
		}
	}
#endif
}
