/*
 * VsaItems.cs - script item list for an engine.
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
using System.Collections;
using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

public class VsaItems : IVsaItems
{
	// Internal state.
	private VsaEngine engine;
	private bool isClosed;
	private ArrayList itemList;

	// Constructor.
	public VsaItems(VsaEngine engine)
			{
				this.engine = engine;
				this.isClosed = false;
				this.itemList = new ArrayList();
			}

	// Check this object to see if the engine is closed.
	private void CheckForClosed()
			{
				if(isClosed)
				{
					throw new VsaException(VsaError.EngineClosed);
				}
			}

	// Implement the "IVsaItems" interface.
	public virtual int Count
			{
				get
				{
					lock(this)
					{
						CheckForClosed();
						return itemList.Count;
					}
				}
			}
	public IVsaItem this[int index]
			{
				get
				{
					lock(this)
					{
						CheckForClosed();
						if(index >= 0 && index < itemList.Count)
						{
							return (IVsaItem)(itemList[index]);
						}
						else
						{
							throw new VsaException(VsaError.ItemNotFound);
						}
					}
				}
			}
	public IVsaItem this[String name]
			{
				get
				{
					lock(this)
					{
						CheckForClosed();
						if(name != null)
						{
							foreach(IVsaItem item in itemList)
							{
								if(item.Name == name)
								{
									return item;
								}
							}
						}
						throw new VsaException(VsaError.ItemNotFound);
					}
				}
			}
	public virtual IVsaItem CreateItem
				(String name, VsaItemType itemType, VsaItemFlag itemFlag)
			{
				lock(this)
				{
					CheckForClosed();
					if(engine.IsRunning)
					{
						throw new VsaException(VsaError.EngineRunning);
					}
					if(itemType != VsaItemType.Code)
					{
						// We only support code items in this implementation.
						throw new VsaException(VsaError.ItemTypeNotSupported);
					}
					if(itemFlag == VsaItemFlag.Class)
					{
						// We don't support class flags.
						throw new VsaException(VsaError.ItemFlagNotSupported);
					}
					VsaItem.ValidateName(engine, name);
					VsaItem item = new VsaCodeItem(engine, name, itemFlag);
					itemList.Add(item);
					return item;
				}
			}
	public virtual void Remove(int index)
			{
				lock(this)
				{
					CheckForClosed();
					if(index >= 0 && index < itemList.Count)
					{
						VsaItem item = (VsaItem)(itemList[index]);
						item.RemovingItem();
						itemList.RemoveAt(index);
						engine.IsDirty = true;
					}
					else
					{
						throw new VsaException(VsaError.ItemNotFound);
					}
				}
			}
	public virtual void Remove(String name)
			{
				lock(this)
				{
					CheckForClosed();
					if(name == null)
					{
						throw new ArgumentNullException("name");
					}
					foreach(VsaItem item in itemList)
					{
						if(item.Name == name)
						{
							item.RemovingItem();
							itemList.Remove(item);
							engine.IsDirty = true;
						}
					}
					throw new VsaException(VsaError.ItemNotFound);
				}
			}

	// Implement the "IEnumerable" interface.
	public virtual IEnumerator GetEnumerator()
			{
				lock(this)
				{
					CheckForClosed();
					return itemList.GetEnumerator();
				}
			}

	// Close this item list.
	public virtual void Close()
			{
				lock(this)
				{
					CheckForClosed();
					foreach(VsaItem item in itemList)
					{
						item.Close();
					}
					isClosed = true;
					itemList = null;
					engine = null;
				}
			}

	// Reset the compiled state of all of the items.
	internal void Reset()
			{
				lock(this)
				{
					CheckForClosed();
					foreach(VsaItem item in itemList)
					{
						item.Reset();
					}
				}
			}

	// Compile all items.
	internal bool Compile()
			{
				lock(this)
				{
					CheckForClosed();
					foreach(VsaItem item in itemList)
					{
						if(!item.Compile())
						{
							return false;
						}
					}
					return true;
				}
			}

	// Run all items.
	internal void Run()
			{
				lock(this)
				{
					CheckForClosed();
					foreach(VsaItem item in itemList)
					{
						item.Run();
					}
				}
			}

}; // class VsaItems

}; // namespace Microsoft.JScript
