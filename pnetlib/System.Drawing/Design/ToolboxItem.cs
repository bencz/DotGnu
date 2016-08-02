/*
 * ToolboxItem.cs - Implementation of the
 *		"System.Drawing.Design.ToolboxItem" class.
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
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
[Serializable]
public class ToolboxItem : ISerializable
{
	public ToolboxItem()
			{
				// TODO
			}
	
	public ToolboxItem(Type toolType)
			{
				// TODO
			}

	public event ToolboxComponentsCreatedEventHandler ComponentsCreated;

	public event ToolboxComponentsCreatingEventHandler ComponentsCreating;

	protected void CheckUnlocked()
			{
				// TODO
			}

	public IComponent[] CreateComponents()
			{
				// TODO
				return null;
			}

	public IComponent[] CreateComponents(IDesignerHost host)
			{
				// TODO
				return null;
			}

	protected virtual IComponent[] CreateComponentsCore(IDesignerHost host)
			{
				// TODO
				return null;
			}
	
	protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
			{
				// TODO
			}

	public override bool Equals(Object obj)
			{
				// TODO
				return false;
			}

	public override int GetHashCode()	
			{
				// TODO
				return -1;
			}

	protected virtual Type GetType(IDesignerHost host, AssemblyName assemblyName,
					String typeName, bool reference)
			{
				// TODO
				return null;
			}

	public virtual void Initialize(Type type)
			{
				// TODO
			}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				// TODO
			}

	public void Lock()
			{
				// TODO
			}

	protected virtual void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
			{
				// TODO
			}
	
	protected virtual void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
			{
				// TODO
			}
	
	protected virtual void Serialize(SerializationInfo info, StreamingContext context)
			{
				// TODO
			}

	public override String ToString()
			{
				// TODO
				return String.Empty;
			}

	public AssemblyName AssemblyName
			{
				get
				{
					// TODO
					return null;
				}
				set
				{
					// TODO
				}

			}

	public Bitmap Bitmap 
			{
				get
				{
					// TODO
					return null;
				}
				set
				{
					// TODO

				}
			}

	public string DisplayName
			{
				get
				{
					// TODO
					return String.Empty;
				}
				set
				{
					// TODO
				}


			}

	public ICollection Filter 
			{
				get
				{
					// TODO
					return null;
				}
				set
				{
					// TODO
				}
			}

	protected bool Locked 
			{
				get
				{
					// TODO
					return false;
				}
				set
				{
					// TODO
				}	
			}

	public string TypeName
			{
				get
				{
					// TODO
					return String.Empty;
				}
				set
				{
					// TODO	
				}	
			}
	
} // class ToolboxItem
#endif
} // namespace System.Drawing.Design
