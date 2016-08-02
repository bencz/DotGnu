/*
 * ControlDesigner.cs - Implementation of "System.Windows.Forms.Design.ControlDesigner" class 
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices; 

public class ControlDesigner : ComponentDesigner
{
	public ControlDesigner() 
			{
				// TODO
			}	

	protected AccessibleObject accessibilityObj;
	protected static readonly Point InvalidPoint;

	protected void BaseWndProc(ref Message m)
			{
				// TODO
			}

	public virtual bool CanBeParentedTo(IDesigner parentDesigner)
			{
				// TODO
				return false;
			}

	protected void DefWndProc(ref Message m)
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

	protected void EnableDragDrop(bool value)
			{
				// TODO
			}

	protected virtual bool GetHitTest(Point point)
			{
				// TODO
				return false;
			}

	protected void HookChildControls(Control firstChild)
			{
				// TODO
			}

	public override void Initialize(IComponent component)
			{
				// TODO
			}

	public override void InitializeNonDefault()
			{
				// TODO
			}

	protected virtual void OnContextMenu( int x, int y )
			{
				// TODO
			}

	protected virtual void OnCreateHandle()
			{
				// TODO
			}

	protected virtual void OnDragDrop(DragEventArgs de)
			{
				// TODO
			}

	protected virtual void OnDragEnter(DragEventArgs de)
			{
				// TODO
			}

	protected virtual void OnDragLeave(EventArgs e)
			{
				// TODO
			}

	protected virtual void OnDragOver(DragEventArgs de)
			{
				// TODO
			}

	protected virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
			{
				// TODO
			}

	protected virtual void OnMouseDragBegin( int x, int y )
			{
				// TODO
			}

	protected virtual void OnMouseDragEnd(bool cancel)
			{
				// TODO
			}

	protected virtual void OnMouseDragMove(int x, int y)
			{
				// TODO
			}

	protected virtual void OnMouseEnter()
			{
				// TODO
			}

	protected virtual void OnMouseHover()
			{
				// TODO
			}

	protected virtual void OnMouseLeave()
			{
				// TODO
			}

	protected virtual void OnPaintAdornments(PaintEventArgs pe)
			{
				// TODO
			}

	public override void OnSetComponentDefaults()
			{
				// TODO
			}

	protected virtual void OnSetCursor()
			{
				// TODO
			}

	protected override void PreFilterProperties(IDictionary properties)
			{
				// TODO
			}

	protected void UnhookChildControls(Control firstChld)
			{
				// TODO
			}

	protected virtual void WndProc(ref Message m)
			{
				// TODO
			}

	public virtual AccessibleObject AccessibilityObject 
			{
				get
				{
					// TODO
					return null;
				}

			}
	
	public override ICollection AssociatedComponents  
			{
				get
				{
					// TODO
					return null;
				}
			}

	public virtual Control Control 
			{
				get
				{
					// TODO
					return null;
				}
			}
	
	protected virtual bool EnableDragRect	
			{
				get
				{
					// TODO
					return false;
				}
			}

	public virtual SelectionRules SelectionRules 
			{
				get
				{
					// TODO
					return new SelectionRules();
				}
			}
	
	[ComVisible(true)]
	public class ControlDesignerAccessibleObject : AccessibleObject
	{
		private Rectangle bounds;
		private AccessibleRole role;
		private AccessibleStates state;

		public ControlDesignerAccessibleObject(ControlDesigner designer, Control control)
				{
					// TODO
				}

		public override AccessibleObject GetChild(int index)
				{
					// TODO
					return null;
				}
		
		public override int GetChildCount()
				{
					// TODO
					return -1;
				}
		
		public override AccessibleObject GetFocused()
				{
					// TODO
					return null;
				}

		public override AccessibleObject GetSelected()
				{
					// TODO
					return null;
				}

		public override AccessibleObject HitTest( int x, int y )
				{
					// TODO
					return null;
				}

		public override Rectangle Bounds 
				{
					get
					{
						// TODO
						return bounds;
					}
				}

		public override string DefaultAction 
				{
					get
					{
						// TODO 
						return String.Empty;
					}
				}

		public override string Description 
				{
					get
					{
						// TODO
						return String.Empty;
					}
				}

		public override string Name 
				{
					get
					{
						// TODO
						return String.Empty;
					}
				}

		public override AccessibleObject Parent 
				{
					get
					{
						// TODO
						return null;
					}
				}

		public override AccessibleRole Role	
				{
					get
					{
						// TODO
						return role;
					}
				}

		public override AccessibleStates State 
				{
					get
					{
						// TODO
						return state;
					}	
				}

		public override string Value
				{
					get
					{
						// TODO
						return String.Empty;
					}
				}


	} // class ControlDesignerAccessibleObject
} // class ControlDesigner

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
