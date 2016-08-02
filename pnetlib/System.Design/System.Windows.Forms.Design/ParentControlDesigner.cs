/*
 * ParentControlDesigner.cs - Implementation of "System.Windows.Forms.Design.ParentControlDesigner" class 
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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
public class ParentControlDesigner : ControlDesigner
{
	public ParentControlDesigner()
			{
				// TODO
			}

	public virtual bool CanParent(Control control)
			{
				// TODO
				return false;
			}

	public virtual bool CanParent(ControlDesigner controlDesigner)
			{
				// TODO
				return false;
			}

	protected void CreateTool(ToolboxItem tool)
			{
				// TODO
			}

	protected void CreateTool(ToolboxItem tool, Point location)
			{
				// TODO
			}

	protected void CreateTool(ToolboxItem tool, Rectangle bounds)
			{
				// TODO
			}
	
	protected virtual IComponent[] CreateToolCore(ToolboxItem tool, 
					int x, int y, int width, int height,
					bool hasLocation, bool hasSize)
			{
				// TODO
				return null;
			}
	
	protected override void Dispose(bool disposing)
			{
				// TODO
			}

	protected Control GetControl(Object component)
			{
				// TODO
				return null;
			}
	
	protected Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
			{
				// TODO
				return new Rectangle();
			}

	public override void Initialize(IComponent component)
			{
				// TODO
			}	

	protected static void InvokeCreateTool(ParentControlDesigner toInvoke, ToolboxItem tool)
			{
				// TODO
			}

	protected override void OnDragDrop(DragEventArgs de)
			{
				// TODO
			}

	protected override void OnDragEnter(DragEventArgs de)
			{
				// TODO
			}
	
	protected override void OnDragLeave(EventArgs e)
			{
				// TODO
			}

	protected override void OnDragOver(DragEventArgs de)
			{
				// TODO
			}

	protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
			{
				// TODO
			}

	protected override void OnMouseDragBegin(int x, int y)
			{
				// TODO
			}

	protected override void OnMouseDragEnd(	bool cancel )
			{
				// TODO
			}

	protected override void OnMouseDragMove(int x, int y)
			{
				// TODO
			}

	protected override void OnMouseEnter()
			{
				// TODO
			}

	protected override void OnMouseHover()
			{
				// TODO
			}

	protected override void OnMouseLeave()
			{
				// TODO
			}

	protected override void OnPaintAdornments(PaintEventArgs pe)
			{
				// TODO
			}

	protected override void OnSetCursor()
			{
				// TODO
			}

	protected override void PreFilterProperties(IDictionary properties)
			{
				// TODO
			}

	protected override void WndProc(ref Message m)
			{
				// TODO
			}

	protected virtual Point DefaultControlLocation 
			{
				get
				{
					// TODO
					return new Point();
				}
			}

	protected virtual bool DrawGrid
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

	protected override bool EnableDragRect
			{
				get
				{
					// TODO
					return false;
				}
			}

	protected Size GridSize	
			{
				get
				{
					// TODO
					return new Size();
				}
				set
				{
					// TODO
				}
			}
	
} // class ParentControlDesigner

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
