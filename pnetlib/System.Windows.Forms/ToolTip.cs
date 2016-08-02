/*
 * ToolTip.cs - Implementation of the
 *			"System.Windows.Forms.ToolTip" class.
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


public sealed class ToolTip
#if CONFIG_COMPONENT_MODEL
	: Component, IExtenderProvider
#endif
{
	private bool active;
	private int automaticDelay;
	private int autopopDelay;
	private int initialDelay;
	private int reshowDelay;
	private bool showAlways;

	[TODO]
	public ToolTip()
	{
		active = true;
		automaticDelay = 500;
		autopopDelay = 10 * automaticDelay;
		initialDelay = automaticDelay;
		reshowDelay = automaticDelay / 5;
		showAlways = false;
	}

#if CONFIG_COMPONENT_MODEL
	[TODO]
	public ToolTip(IContainer cont) : this()
	{

	}
#endif

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public int AutomaticDelay
	{
		get
		{
			return automaticDelay;
		}
		set
		{
			automaticDelay = value;
		}
	}

	public int AutoPopDelay 
	{
		get
		{
			return autopopDelay;
		}
		set
		{
			autopopDelay = value;
		}
	}

	public int ReshowDelay
	{
		get
		{	
			return reshowDelay;
		}
		set
		{
			reshowDelay = value;
		}
	}

	public bool ShowAlways 
	{
		get
		{
			return showAlways;
		}
		set
		{
			showAlways = value;
		}
	}

	public int InitialDelay
	{
		get
		{
			return initialDelay;
		}
		set
		{
			initialDelay = value;
		}
	}

	[TODO]
	public String GetToolTip(Control control)
			{
				return String.Empty;
			}
	[TODO]
	public bool CanExtend(Object target)
			{
				return false;
			}

	[TODO]
#if CONFIG_COMPONENT_MODEL
	protected override void Dispose(bool disposing)
#else
	protected virtual void Dispose(bool disposing)
#endif
			{
				return;
			}

	[TODO]
	public void RemoveAll()
			{
				return;
			}

	[TODO]
	public void SetToolTip(Control control, String caption)
			{
				return;
			}

	[TODO]
	public override string ToString()
			{
				return base.ToString() + " InitialDelay: " + initialDelay.ToString() + ", ShowAlways: " + showAlways.ToString();
			}
	
	~ToolTip()
			{	
				Dispose(false);
			}	

}	// class ToolTip

}	// namespace System.Windows.Forms

