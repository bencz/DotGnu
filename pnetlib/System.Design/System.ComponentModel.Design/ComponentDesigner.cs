/*
 * ComponentDesigner.cs - Implementation of "System.ComponentModel.Design.ComponentDesigner" class 
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


namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN
	
using System.Collections;

public class ComponentDesigner : IDesigner, IDisposable,
	                                 IDesignerFilter
{
	protected sealed class ShadowPropertyCollection
	{
		public object this [string propertyName] 
				{
					get
				       	{ 
						throw new NotImplementedException(); 
					} 

					set
				       	{ 
						throw new NotImplementedException(); 
					}
				}

		public bool Contains (string propertyName)
				{
					throw new NotImplementedException();
				}

		~ShadowPropertyCollection()
				{
				}
		
	} // class ShadowPropertyCollection

	public ComponentDesigner()
			{
			}

	public virtual ICollection AssociatedComponents 
			{
				get 
				{
				       	throw new NotImplementedException(); 
				}
			}

	public IComponent Component 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}

	public virtual DesignerVerbCollection Verbs 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}

	public void Dispose()
			{
				throw new NotImplementedException();
			}

	protected virtual void Dispose (bool disposing)
			{
				throw new NotImplementedException();
			}

	public virtual void DoDefaultAction()
			{
				throw new NotImplementedException();
			}

	public virtual void Initialize (IComponent component)
			{
				throw new NotImplementedException();
			}

	public virtual void InitializeNonDefault()
			{
				throw new NotImplementedException();
			}

	public virtual void OnSetComponentDefaults()
			{
				throw new NotImplementedException();
			}
	
	protected InheritanceAttribute InheritanceAttribute 
			{
				get
			       	{ 
					throw new NotImplementedException(); 
				}
			}

	protected bool Inherited 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}

	protected ShadowPropertyCollection ShadowProperties 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}
		
	protected virtual object GetService (Type serviceType)
			{
				throw new NotImplementedException();
			}

	protected InheritanceAttribute InvokeGetInheritanceAttribute (ComponentDesigner toInvoke)
			{
				throw new NotImplementedException();
			}

	void IDesignerFilter.PostFilterAttributes (IDictionary attributes)
			{
				PostFilterAttributes( attributes );
			}

	protected virtual void PostFilterAttributes (IDictionary attributes)
			{
				throw new NotImplementedException();
			}

	void IDesignerFilter.PostFilterEvents (IDictionary events)
			{
				PostFilterEvents( events );
			}
	
	protected virtual void PostFilterEvents (IDictionary events)
			{
				throw new NotImplementedException();
			}
	
	void IDesignerFilter.PostFilterProperties (IDictionary properties)
			{
				PostFilterProperties( properties );
			}

	
	protected virtual void PostFilterProperties (IDictionary properties)
			{
				throw new NotImplementedException();
			}

	void IDesignerFilter.PreFilterAttributes (IDictionary attributes)
			{
				PreFilterAttributes( attributes );
			}

	protected virtual void PreFilterAttributes (IDictionary attributes)
			{
				throw new NotImplementedException();	
			}

	void IDesignerFilter.PreFilterEvents (IDictionary events)
			{
				PreFilterEvents( events );
			}	
	
	protected virtual void PreFilterEvents (IDictionary events)
			{
				throw new NotImplementedException();
			}
	
	void IDesignerFilter.PreFilterProperties (IDictionary properties)
			{
				PreFilterProperties( properties );
			}
	
	protected virtual void PreFilterProperties (IDictionary properties)
			{
				throw new NotImplementedException();
			}

	protected void RaiseComponentChanged (MemberDescriptor member, 
				object oldValue,
				object newValue)
			{
				throw new NotImplementedException();
			}

	protected void RaiseComponentChanging (MemberDescriptor member)
			{
				throw new NotImplementedException();
			}

	~ComponentDesigner()
			{
			}

} // class ComponentDesigner

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.ComponentModel.Design
