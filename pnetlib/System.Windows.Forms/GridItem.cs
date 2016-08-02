/*
 * GridItem.cs - Implementation of the
 *			"System.Windows.Forms.GridItem" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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
#if CONFIG_COMPONENT_MODEL
using System.ComponentModel;
#endif

public abstract class GridItem
{

	public abstract GridItemCollection GridItems
	{
		get;
	}

	public abstract GridItemType GridItemType
	{
		get;
	}

	public abstract string Label
	{
		get;
	}

	public abstract GridItem Parent
	{
		get;
	}

#if CONFIG_COMPONENT_MODEL
	public abstract PropertyDescriptor PropertyDescriptor
	{
		get;
	}
#endif

	public abstract object Value
	{
		get;
	}

	public virtual bool Expandable
	{
		get
		{
			return false;
		}
	}

	public virtual bool Expanded
	{
		get
		{
			return false;
		}

		set
		{
			throw new NotSupportedException();
		}
	}

	public abstract bool Select();

}; // class GridItem

}; // namespace System.Windows.Forms
