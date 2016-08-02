/*
 * ControlBindingsCollection.cs - Implementation of the
 *			"System.Windows.Forms.ControlBindingsCollection" class.
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
using System.ComponentModel;
	
public class ControlBindingsCollection: BindingsCollection
{
	private Control control;
	
	protected internal ControlBindingsCollection(Control control)
			{
				this.control = control;
			}

	new public void Add(Binding binding)
			{
				binding.AssociateControl(control);
				base.AddCore(binding);
			}

	public Binding Add(String propertyName, Object dataSource, String dataMember)
			{
				Binding binding = new Binding(propertyName, dataSource, dataMember);
				binding.AssociateControl(control);
				base.AddCore(binding);
			#if CONFIG_COMPONENT_MODEL			
				OnCollectionChanged(new CollectionChangeEventArgs
						(CollectionChangeAction.Add, binding));
		#endif	
				return binding;
			}

	private void OnFormat(object sender, ConvertEventArgs e)
			{
				
			}

	private void OnParse(object sender, ConvertEventArgs e)
			{
				
			}
	
	protected override void AddCore(Binding dataBinding)
			{
				dataBinding.AssociateControl(control);
				base.AddCore(dataBinding);
			}

	new public void Clear()
			{
				ClearCore();
			}

	
	protected override void ClearCore()
			{
				int iCount = this.Count;
				for( int i = 0; i < iCount; i++ ) {
					base[i].AssociateControl(null);
				}
				base.ClearCore();
			}

	new public void Remove(Binding binding)
			{
				base.Remove(binding);
			}		

	new public void RemoveAt(int index)
			{
				base.Remove(base[index]);
			}

	protected override void RemoveCore(Binding dataBinding)
			{
				dataBinding.AssociateControl(null);
				base.RemoveCore(dataBinding);
			}

	public Control Control 
			{
				get
				{
					return control;
				}

 			}

	public Binding this[String propertyName] 
			{
				get
				{
					foreach(Binding b in List)
					{
						if( b.PropertyName == propertyName )
							return b;
					}
					return null;
				}

 			}

	
}; // class ControlBindingsCollection

}; // namespace System.Windows.Forms
