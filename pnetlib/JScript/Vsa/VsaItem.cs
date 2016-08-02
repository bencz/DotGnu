/*
 * VsaItem.cs - script item for an engine.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

public abstract class VsaItem : IVsaItem
{
	// Internal state.
	internal VsaEngine engine;
	internal String codebaseOption;
	protected bool isDirty;
	protected VsaItemType type;
	protected String name;
	protected VsaItemFlag flag;

	// Constructor.
	internal VsaItem(VsaEngine engine, String name, VsaItemType type,
					 VsaItemFlag flag)
			{
				this.engine = engine;
				this.codebaseOption = null;
				this.isDirty = false;
				this.type = type;
				this.name = name;
				this.flag = flag;
			}

	// Check this object to see if the engine is closed.
	internal void CheckForClosed()
			{
				if(engine == null)
				{
					throw new VsaException(VsaError.EngineClosed);
				}
			}

	// Validate an item name.
	internal static void ValidateName(VsaEngine engine, String name)
			{
				// Validate the name with the engine.
				if(!engine.IsValidIdentifier(name))
				{
					throw new VsaException(VsaError.ItemNameInvalid);
				}

				// Make sure that there are no items with this name.
				foreach(VsaItem item in engine.Items)
				{
					if(item.Name == name)
					{
						throw new VsaException(VsaError.ItemNameInUse);
					}
				}
			}

	// Implement the "IVsaItem" interface;
	public virtual bool IsDirty
			{
				get
				{
					CheckForClosed();
					return isDirty;
				}
				set
				{
					CheckForClosed();
					isDirty = value;
				}
			}
	public VsaItemType ItemType
			{
				get
				{
					CheckForClosed();
					return type;
				}
			}
	public virtual String Name
			{
				get
				{
					CheckForClosed();
					return name;
				}
				set
				{
					CheckForClosed();
					if(name != value)
					{
						ValidateName(engine, name);
					}
					name = value;
					isDirty = true;
					engine.IsDirty = true;
				}
			}
	public virtual Object GetOption(String name)
			{
				CheckForClosed();
				if(String.Compare(name, "codebase", true) == 0)
				{
					return codebaseOption;
				}
				else
				{
					throw new VsaException(VsaError.OptionNotSupported);
				}
			}
	public virtual void SetOption(String name, Object value)
			{
				CheckForClosed();
				if(String.Compare(name, "codebase", true) == 0)
				{
					codebaseOption = (String)value;
					isDirty = true;
					engine.IsDirty = true;
				}
				else
				{
					throw new VsaException(VsaError.OptionNotSupported);
				}
			}

	// Close this item.
	internal virtual void Close()
			{
				engine = null;
			}

	// Notify this item that it is being removed from the item list.
	internal virtual void RemovingItem()
			{
				Close();
			}

	// Reset the compiled state of this item.
	internal virtual void Reset()
			{
				// Nothing to do here.
			}

	// Compile this item.
	internal virtual bool Compile()
			{
				// Nothing to do here.
				return true;
			}

	// Run this item.
	internal virtual void Run()
			{
				// Nothing to do here.
			}

}; // class VsaItem

}; // namespace Microsoft.JScript
