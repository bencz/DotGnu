/*
 * MdiClient.cs - Implementation of the
 *			"System.Windows.Forms.MdiClient" class.
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

namespace System.Windows.Forms
{

using System.Collections;
using System.Drawing;
using System.Drawing.Toolkit;
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[ToolboxItem(false)]
[DesignTimeVisible(false)]
#endif
public sealed class MdiClient : Control
{
	// Internal state.
	private ArrayList controls;
	private Form activeChild;

	// Constructor.
	public MdiClient()
			{
				BackColor = SystemColors.AppWorkspace;
				Dock = DockStyle.Fill;
				SetStyle(ControlStyles.Selectable, false);
				controls = new ArrayList();
			}

	// Get the active MDI child.
	internal Form ActiveChild
			{
				get
				{
					return activeChild;
				}
			}

	// Get or set the background image.
	public override Image BackgroundImage
			{
				get
				{
					return base.BackgroundImage;
				}
				set
				{
					base.BackgroundImage = value;
				}
			}

	// Get the window creation parameters.
	protected override CreateParams CreateParams
			{
				get
				{
					return base.CreateParams;
				}
			}

	// Get the children that are being managed by this MDI client.
	public Form[] MdiChildren
			{
				get
				{
					Form[] children = new Form [controls.Count];
					controls.CopyTo(children, 0);
					return children;
				}
			}

	// Create the toolkit window underlying this control.
	internal override IToolkitWindow CreateToolkitWindow(IToolkitWindow parent)
	{
		CreateParams cp = CreateParams;
		int x = cp.X + ToolkitDrawOrigin.X;
		int y = cp.Y + ToolkitDrawOrigin.Y;
		int width = cp.Width - ToolkitDrawSize.Width;
		int height = cp.Height - ToolkitDrawSize.Height;
						
		if(parent != null)
		{
						// Use the parent's toolkit to create.
			if(Parent is Form)
			{
				// use ControlToolkitManager to create the window thread safe
				return ControlToolkitManager.CreateMdiClient( this, 
						parent, x, y, width, height);
			}
			else
			{
				// use ControlToolkitManager to create the window thread safe
				return ControlToolkitManager.CreateMdiClient( this,
						parent,
						x + Parent.ClientOrigin.X,
						y +  Parent.ClientOrigin.Y, width, height);
			}
		}
		else
		{
			// Use the default toolkit to create.
			// use ControlToolkitManager to create the window thread safe
			return ControlToolkitManager.CreateMdiClient	(this, null, x, y, width, height);
		}
	}

	// Create a new control collection for this instance.
	protected override Control.ControlCollection CreateControlsInstance()
			{
				return new ControlCollection(this);
			}

	// Lay out the children in this MDI client.
	public void LayoutMdi(MdiLayout value)
			{
				IToolkitMdiClient mdi = (toolkitWindow as IToolkitMdiClient);
				if(mdi != null)
				{
					switch(value)
					{
						case MdiLayout.Cascade:
						{
							mdi.Cascade();
						}
						break;

						case MdiLayout.TileHorizontal:
						{
							mdi.TileHorizontally();
						}
						break;

						case MdiLayout.TileVertical:
						{
							mdi.TileVertically();
						}
						break;

						case MdiLayout.ArrangeIcons:
						{
							mdi.ArrangeIcons();
						}
						break;
					}
				}
			}

	// Handle a resize event.
	protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);
			}

	// Inner core of "Scale".
	protected override void ScaleCore(float dx, float dy)
			{
				base.ScaleCore(dx, dy);
			}

	// Inner core of "SetBounds".
	protected override void SetBoundsCore
					(int x, int y, int width, int height,
					 BoundsSpecified specified)
			{
				base.SetBoundsCore(x, y, width, height, specified);
			}

	// Receive notification that a particular child was activated.
	internal override void MdiActivate(IToolkitWindow child) 
			{
				Activate(child);
			}

	internal void Activate(IToolkitWindow child)
			{
				if(child == null)
				{
					activeChild = null;
				}
				else
				{
					activeChild = null;
					foreach(Form form in controls)
					{
						if(form.toolkitWindow == child)
						{
							activeChild = form;
							break;
						}
					}
				}
			}

#if !CONFIG_COMPACT_FORMS

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

	// Special purpose control collection for MDI clients.
	public new class ControlCollection : Control.ControlCollection
	{
		// Internal state.
		private MdiClient mdiClient;

		// Constructor.
		public ControlCollection(MdiClient owner)
				: base(owner)
				{
					this.mdiClient = owner;
				}

		// Add a control to this collection.
		public override void Add(Control value)
				{
					if(!(value is Form) || !(((Form)value).IsMdiChild))
					{
						throw new ArgumentException
							(S._("SWF_NotMdiChild"));
					}
					mdiClient.controls.Add(value);
					// TODO: figure out how to add Forms without reparenting
					//base.Add(value);
				}

		// Remove a control from this collection.
		public override void Remove(Control value)
				{
					mdiClient.controls.Remove(value);
					// TODO: figure out how to add Forms without reparenting
					//base.Remove(value);
				}

	}; // class ControlCollection

}; // class MdiClient

}; // namespace System.Windows.Forms
