/*
 * ComponentTray.cs - Implementation of "System.Windows.Forms.Design.ComponentTray" class 
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

public class ComponentTray : ScrollableControl, IExtenderProvider
{
	public ComponentTray(IDesigner mainDesigner, IServiceProvider serviceProvider) : base()
			{
			}
	
	public virtual void AddComponent ( IComponent component )
			{
				// TODO
			}
	
	protected virtual bool CanCreateComponentFromTool ( ToolboxItem tool )
			{
				// TODO
				return false;
			}
	
	protected virtual bool CanDisplayComponent ( IComponent component )
			{
				// TODO
				return false;
			}
	
	public void CreateComponentFromTool(ToolboxItem tool)
			{
				// TODO
			}
	
	protected void DisplayError(Exception e)
			{
				// TODO
			}
	
	protected override void Dispose(bool disposing)
			{
				// TODO
			}
	
	public Point GetLocation(IComponent receiver)
			{
				// TODO
				return new Point();
			}
	
	protected override object GetService(Type serviceType)
			{
				// TODO
				return null;
			}	
	
	bool IExtenderProvider.CanExtend(Object component)
			{
				// TODO
				return false;
			}

	protected override void OnDoubleClick(EventArgs e)
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
	
	protected override void OnDragLeave(EventArgs e )
			{
				// TODO
			}
	
	protected override void OnDragOver(DragEventArgs de)
			{
				// TODO	
			}
	
	protected override void OnGiveFeedback(GiveFeedbackEventArgs gfevent)
			{
				// TODO
			}
	
	protected override void OnLayout(LayoutEventArgs levent)
			{
				// TODO
			}
	
	protected virtual void OnLostCapture()
			{
				// TODO
			}
	
	protected override void OnMouseDown(MouseEventArgs e)
			{
				// TODO
			}
	
	protected override void OnMouseMove(MouseEventArgs e)
			{
				// TODO
			}
	
	protected override void OnMouseUp(MouseEventArgs e)
			{
				// TODO
			}
	
	protected override void OnPaint(PaintEventArgs pe)
			{
				// TODO
			}
	
	protected virtual void OnSetCursor()
			{
				// TODO
			}
	
	public virtual void RemoveComponent(IComponent component)
			{
				// TODO
			}
	
	public void SetLocation(IComponent receiver, Point location)
			{
				// TODO
			}
	
	protected override void WndProc(ref Message m)
			{
				// TODO
			}
	
	public bool AutoArrange 
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

	public int ComponentCount 
			{ 
				get
				{
					// TODO
					return -1;
				}
			}

	public bool ShowLargeIcons 
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

	
} // class ComponentTray

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
