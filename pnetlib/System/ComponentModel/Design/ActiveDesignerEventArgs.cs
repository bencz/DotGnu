/*
 * ActiveDesignerEventArgs.cs - Implementation of the
 *		"System.ComponentModel.Design.ActiveDesignerEventArgs" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

public class ActiveDesignerEventArgs : EventArgs
{
	// Internal state.
	private IDesignerHost oldDesigner;
	private IDesignerHost newDesigner;

	// Constructor.
	public ActiveDesignerEventArgs
				(IDesignerHost oldDesigner, IDesignerHost newDesigner)
			{
				this.oldDesigner = oldDesigner;
				this.newDesigner = newDesigner;
			}

	// Get this object's properties.
	public IDesignerHost NewDesigner
			{
				get
				{
					return newDesigner;
				}
			}
	public IDesignerHost OldDesigner
			{
				get
				{
					return oldDesigner;
				}
			}

}; // class ActiveDesignerEventArgs

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
