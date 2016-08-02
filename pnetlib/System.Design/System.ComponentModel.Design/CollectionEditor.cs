/*
 * CollectionEditor.cs - Implementation of "System.ComponentModel.Design.CollectionEditor" class 
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

using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

public class CollectionEditor : UITypeEditor
{
	protected abstract class CollectionForm : Form
	{
		public CollectionForm (CollectionEditor editor)
				{
				}

		public Object EditValue 
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

		public override ISite Site 
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

		protected Type CollectionItemType 
				{
					get 
					{ 
						throw new NotImplementedException(); 
					}
				}

		protected Type CollectionType 
				{
					get
				       	{ 
						throw new NotImplementedException(); 
					}
				}

		protected ITypeDescriptorContext Context 
				{
					get 
					{ 
						throw new NotImplementedException(); 
					}
				}

		protected override ImeMode DefaultImeMode 
				{
					get 
					{ 
						throw new NotImplementedException(); 		
					}
				}

		protected Object[] Items 
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

		protected Type[] NewItemTypes 
				{
					get 
					{ 
						throw new NotImplementedException(); 
					}
				}

		protected bool CanRemoveInstance (Object value)
				{
					throw new NotImplementedException();
				}

		protected virtual bool CanSelectMultipleInstances()
				{
					throw new NotImplementedException();
				}

		protected Object CreateInstance (Type itemType)
				{
					throw new NotImplementedException();
				}

		protected void DestroyInstance (Object instance)
				{
					throw new NotImplementedException();
				}

		protected virtual void DisplayError (Exception e)
				{
					throw new NotImplementedException();
				}

		protected override Object GetService (Type serviceType)
				{
					throw new NotImplementedException();
				}

		protected abstract void OnEditValueChanged();

		protected internal virtual DialogResult ShowEditorDialog (IWindowsFormsEditorService edSvc)
				{
					throw new NotImplementedException();
				}

		~CollectionForm ()
				{
				}

	} // class CollectionForm

	public CollectionEditor (Type type)
			{
				
			}

	public override Object EditValue (ITypeDescriptorContext context,
						  IServiceProvider provider,
						  Object value)
			{
				throw new NotImplementedException();
			}

	public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context)
			{
				throw new NotImplementedException();
			}

	protected Type CollectionItemType 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}

	protected Type CollectionType 
			{
				get
			       	{ 
					throw new NotImplementedException(); 
				}
			}

	protected ITypeDescriptorContext Context 
			{
				get 
				{ 
					throw new NotImplementedException(); 
				}
			}
		
	protected virtual string HelpTopic 
			{
			
				get
			       	{ 
					throw new NotImplementedException(); 
				}
			}

	protected Type[] NewItemTypes 
			{
				get 
				{
				       	throw new NotImplementedException(); 
				}
			}

	protected virtual bool CanRemoveInstance (Object value)
			{
				throw new NotImplementedException();
			}

	protected virtual bool CanSelectMultipleInstances()
			{
				throw new NotImplementedException();
			}

	protected virtual CollectionForm CreateCollectionForm()
			{
				throw new NotImplementedException();
			}

	protected virtual Type CreateCollectionItemType()
			{
				throw new NotImplementedException();
			}

	protected virtual Object CreateInstance (Type itemType)
			{
				throw new NotImplementedException();
			}

	protected virtual Type[] CreateNewItemTypes()
			{
				throw new NotImplementedException();
			}

	protected virtual void DestroyInstance (Object instance)
			{
				throw new NotImplementedException();
			}

	protected virtual Object[] GetItems (Object editValue)
			{
				throw new NotImplementedException();
			}

	protected Object GetService (Type serviceType)
			{
				throw new NotImplementedException();
			}

	protected virtual Object SetItems (Object editValue,
						   Object[] value)
			{
				throw new NotImplementedException();
			}

	protected virtual void ShowHelp()
			{
				throw new NotImplementedException();
			}
		
	~CollectionEditor()
			{

			}
		
} // class CollectionEditor

#endif // CONFIG_COMPONENT_MODEL_DESIGN
} // namespace System.ComponentModel.Design 
