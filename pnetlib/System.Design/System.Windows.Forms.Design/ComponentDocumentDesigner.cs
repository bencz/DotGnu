/*
 * ComponentDocumentDesigner.cs - Implementation of "System.Windows.Forms.Design.ComponentDocumentDesigner" class 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributions by Adam Ballai <Adam@TheFrontNetworks.net>
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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;

public class ComponentDocumentDesigner : ComponentDesigner, IRootDesigner, IToolboxUser, ITypeDescriptorFilterService 
{
	public ComponentDocumentDesigner()
			{
				// TODO
			}

	public Control Control 
			{
				get
				{
					// TODO
					return null;
				}
			}
	
	public bool TrayAutoArrange
			{
			
				get
				{
					// TODO
					return false;
				}
			
				set
				{
					// TODO
				}
			}

	public bool TrayLargeIcon
			{
				get
				{
					// TODO
					return false;
				}
			
				set
				{
					// TODO
				}
			}
	public override void Initialize(IComponent component)
			{
				// TODO
			}
	
	protected override void Dispose(bool disposing)
			{
				// TODO
			}
	
	bool IToolboxUser.GetToolSupported(ToolboxItem tool)
			{
				// TODO
				return false;
			}

	void IToolboxUser.ToolPicked(System.Drawing.Design.ToolboxItem item)
			{
				// TODO
			}

	object IRootDesigner.GetView(ViewTechnology technology)
			{
				// TODO
				return null;
			}
	
	protected override void PreFilterProperties(IDictionary properties)
			{
				// TODO
			}

	bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
			{
				// TODO
				return false;
			}

	bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
			{
				// TODO
				return false;
			}

	bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
			{
				// TODO
				return false;
			}

	ViewTechnology[] IRootDesigner.SupportedTechnologies
			{
				get
				{
					// TODO
					return null;
				}

			}
	
} // class ComponentDocumentDesigner

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
