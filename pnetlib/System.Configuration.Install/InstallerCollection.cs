/*
 * InstallerCollection.cs - Implementation of the
 *	    "System.Configuration.Install.InstallerCollection" class.
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

namespace System.Configuration.Install
{

#if !ECMA_COMPAT

using System.Collections;

public class InstallerCollection : CollectionBase
{
    // Internal state.
    private Installer parent;

    // Constructor.
    internal InstallerCollection(Installer parent)
			{
				this.parent = parent;
			}

    // Get or set a collection member, by index.
    public Installer this[int index]
			{
				get
				{
					return (Installer)(((IList)this)[index]);
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

    // Add an installer to this collection.
    public int Add(Installer value)
			{
				return ((IList)this).Add(value);
			}

    // Add a range of installers to this collection.
    public void AddRange(Installer[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(Installer inst in value)
				{
					Add(inst);
				}
			}
    public void AddRange(InstallerCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(Installer inst in value)
				{
					Add(inst);
				}
			}

    // Determine if a particular installer is contained in this collection.
    public bool Contains(Installer value)
			{
				return ((IList)this).Contains(value);
			}

    // Copy the elements of this collection into an array.
    public void CopyTo(Installer[] array, int index)
			{
				((IList)this).CopyTo(array, index);
			}

    // Get the index of a specific installer within this collection
    public int IndexOf(Installer value)
			{
				return ((IList)this).IndexOf(value);
			}

    // Insert an installer into this collection.
    public void Insert(int index, Installer value)
			{
				((IList)this).Insert(index, value);
			}

    // Remove an installer from this collection.
    public void Remove(Installer value)
			{
				((IList)this).Remove(value);
			}

    // Record that an installer is being inserted into this collection.
    protected override void OnInsert(int index, Object value)
			{
				Installer inst = (value as Installer);
				if(inst != null)
				{
					if(inst.parent != null && inst.parent != parent)
					{
						inst.parent.Installers.Remove(inst);
					}
					inst.parent = parent;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

    // Record that an installer is being removed from this collection.
    protected override void OnRemove(int index, Object value)
			{
				Installer inst = (value as Installer);
				if(inst != null)
				{
					inst.parent = null;
				}
			}

    // Record that a collection member is being set to a new value.
    protected override void OnSet(int index, Object oldValue, Object newValue)
			{
				Installer oldInst = (oldValue as Installer);
				Installer newInst = (newValue as Installer);
				if(oldValue != newValue)
				{
					if(newInst == null)
					{
						throw new InvalidOperationException();
					}
					oldInst.parent = null;
					if(newInst.parent != null && newInst.parent != parent)
					{
						newInst.parent.Installers.Remove(newInst);
					}
					newInst.parent = parent;
				}
			}

}; // class InstallerCollection

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
