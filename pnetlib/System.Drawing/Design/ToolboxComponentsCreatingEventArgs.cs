/*
 * ToolboxComponentsCreatingEventArgs.cs - Implementation of the
 *		"System.Drawing.Design.ToolboxComponentsCreatingEventArgs" class.
 *
 * Copyright (C) 2003  FSF.
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

namespace System.Drawing.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
public class ToolboxComponentsCreatingEventArgs : EventArgs
{
	private IDesignerHost host;
	public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
			{
				this.host = host;
			}
	
	public IDesignerHost DesignerHost
			{
				get
				{
					return host;
				}	
			}				

} // class ToolboxComponentsCreatingEventArgs
#endif
}
