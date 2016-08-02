/*
 * DocumentDesigner.cs - Implementation of "System.Windows.Forms.Design.DocumentDesigner" class 
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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

public class DocumentDesigner : ScrollableControlDesigner,
   IRootDesigner, IToolboxUser
{
	protected IMenuEditorService menuEditorService;
	
	public DocumentDesigner() 
			{
				// TODO
			}
	
	protected override void Dispose(bool disposing)
			{
				// TODO
			}

	protected virtual void EnsureMenuEditorService(IComponent c)
			{
				// TODO 
			}

	protected virtual bool GetToolSupported(ToolboxItem tool)
			{
				// TODO
				return false;
			}

	public override void Initialize(IComponent component)
			{
				// TODO
			}

	ViewTechnology[] IRootDesigner.SupportedTechnologies
			{
				get
				{
					// TODO
					return null;
				}
			}


	object IRootDesigner.GetView(ViewTechnology technology)
			{
				// TODO
				return null;
			}

	bool IToolboxUser.GetToolSupported(ToolboxItem tool)
			{
				// TODO
				return false;

			}


	void IToolboxUser.ToolPicked(ToolboxItem tool)
			{
				// TODO
			}

	protected override void OnContextMenu( int x , int y )
			{
				// TODO
			}

	protected override void OnCreateHandle()
			{
				// TODO
			}

	protected override void PreFilterProperties(IDictionary properties)
			{
				// TODO
			}

	protected virtual void ToolPicked(ToolboxItem tool)
			{
				// TODO
			}

	protected override void WndProc(ref Message m)
			{
				// TODO
			}

	public override SelectionRules SelectionRules 
			{
				get
				{
					// TODO
					return new SelectionRules();
				}
			}
						
} // class DocumentDesigner

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
