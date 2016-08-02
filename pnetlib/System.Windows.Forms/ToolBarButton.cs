/*
 * ToolBarButton.cs - Implementation of the
 *			"System.Windows.Forms.ToolBarButton" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Free Software Foundation, Inc.
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

using System;
using System.ComponentModel;
using System.Drawing;

public class ToolBarButton
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Variables
	internal ToolBar parent;
	internal int tbbcIndex; // index in ToolBarButtonCollection
	internal int groupID; // index of last separator (used in wrapping)
	internal Rectangle viewRectangle;
	internal Rectangle dropRectangle;
	private Menu dropDownMenu = null;
	private bool enabled = true;
	private int imageIndex = -1;
	private bool partialPush = false;
	private bool pushed = false;
	private ToolBarButtonStyle style = ToolBarButtonStyle.PushButton;
	private object tag = null;
	private string text = "";
	private string toolTipText = "";
	private bool visible = true;



	// Constructors
	public ToolBarButton() : base() {}
#if !CONFIG_COMPACT_FORMS
	public ToolBarButton(String s) : base() { text = s; }
#endif



	// Properties
	public Menu DropDownMenu
	{
		get { return dropDownMenu; }
		set
		{
			// this is called bad design
			// this is the compiler's job
			if (!(value is ContextMenu))
			{
				throw new ArgumentException(/* TODO */);
			}
			if (value == dropDownMenu) { return; }
			dropDownMenu = value;
		}
	}
	public bool Enabled
	{
		get { return enabled; }
		set
		{
			if (value == enabled) { return; }
			enabled = value;
			if (visible && parent != null)
			{
				parent.TBBUpdate(tbbcIndex,true);
			}
		}
	}
	public int ImageIndex
	{
		get { return imageIndex; }
		set
		{
			if (value < -1)
			{
				// ArgumentOutOfRangeException?
				throw new ArgumentException(/* TODO */);
			}
			if (value == imageIndex) { return; }
			imageIndex = value;
			if (visible && parent != null)
			{
				parent.TBBUpdate(tbbcIndex,true);
			}
		}
	}
#if !CONFIG_COMPACT_FORMS
	public ToolBar Parent { get { return parent; } }
	public bool PartialPush
	{
		get { return partialPush; }
		set
		{
			if (value == partialPush) { return; }
			partialPush = value;
			if (style == ToolBarButtonStyle.ToggleButton)
			{
				if (visible && parent != null)
				{
					parent.TBBUpdate(tbbcIndex,true);
				}
			}
		}
	}
#endif
	public bool Pushed
	{
		get { return pushed; }
		set
		{
			if (value == pushed) { return; }
			pushed = value;
			if (style == ToolBarButtonStyle.ToggleButton)
			{
				if (visible && parent != null)
				{
					parent.TBBUpdate(tbbcIndex,true);
				}
			}
		}
	}
#if !CONFIG_COMPACT_FORMS
	public Rectangle Rectangle
	{
		get
		{
			if (parent.Visible && Visible)
			{
				return Rectangle.Union(viewRectangle,dropRectangle);
			}
			return Rectangle.Empty;
		}
	}
#endif
	public ToolBarButtonStyle Style
	{
		get { return style; }
		set
		{
			if (value == style) { return; }

			ToolBarButtonStyle oldStyle = style;
			style = value;
			if (visible && parent != null)
			{
				if (oldStyle == ToolBarButtonStyle.Separator)
				{
					parent.buttons.KillGroupQuiet(tbbcIndex);
				}
				else if (value == ToolBarButtonStyle.Separator)
				{
					parent.buttons.MakeGroupQuiet(tbbcIndex);
				}
				parent.TBBUpdate(tbbcIndex,false);
			}
		}
	}
#if !CONFIG_COMPACT_FORMS
	public object Tag
	{
		get { return tag; }
		set { tag = value; }
	}
	public string Text
	{
		get { return text; }
		set
		{
			if (value == text) { return; }
			if (value == null) { value = ""; }
			text = value;
			if (visible && parent != null)
			{
				parent.TBBUpdate(tbbcIndex,false);
			}
		}
	}
	public string ToolTipText
	{
		get { return toolTipText; }
		set
		{
			if (value == toolTipText) { return; }
			if (value == null) { value = ""; }
			toolTipText = value;
		}
	}
#endif
	public bool Visible
	{
		get { return visible; }
		set
		{
			if (value == visible) { return; }
			visible = value;
			if (!visible)
			{
				viewRectangle = Rectangle.Empty;
				dropRectangle = Rectangle.Empty;
			}
			if (parent != null)
			{
				if (style == ToolBarButtonStyle.Separator)
				{
					if (visible)
					{
						// if the separator is visible
						// it should act as a wrap point
						parent.buttons.MakeGroupQuiet(tbbcIndex);
					}
					else
					{
						// if the separator isn't visible
						// it shouldn't act as a wrap point
						parent.buttons.KillGroupQuiet(tbbcIndex);
					}
					// no point in recalculating all the text
					// and button sizing info for a group change
					parent.TBBCUpdate(true);
				}
				else
				{
					parent.TBBUpdate(tbbcIndex,false);
				}
			}
		}
	}



	// Methods
	/*protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}*/
	internal int Contains(int x, int y, bool dropDowns)
	{
		if (!visible)
		{
			return 0;
		}
		else if (style == ToolBarButtonStyle.Separator)
		{
			if (viewRectangle.Contains(x,y))
			{
				return -1;
			}
			return 0;
		}
		else if (dropDowns)
		{
			if (viewRectangle.Contains(x,y))
			{
				return 1;
			}
			else if (dropRectangle.Contains(x,y))
			{
				return 2;
			}
			else
			{
				return 0;
			}
		}
		else if (style == ToolBarButtonStyle.DropDownButton)
		{
			if (viewRectangle.Contains(x,y))
			{
				return 2;
			}
			else
			{
				return 0;
			}
		}
		else if (viewRectangle.Contains(x,y))
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	internal void Reset()
	{
		parent = null;
		tbbcIndex = -1;
		groupID = -1;
		viewRectangle = Rectangle.Empty;
		dropRectangle = Rectangle.Empty;
	}

	public override string ToString()
	{
		return String.Format("ToolBarButton: {0}, Style: {1}", 
			Text, Style);
	}


}; // class ToolBarButton

}; // namespace System.Windows.Forms
